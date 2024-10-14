using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using ChatClient.Domain.Entities;
using ChatClient.Infrastructure.Repositories;
using ChatClient.Utils;

namespace ChatClient.Application.Services
{
    public class RoomService
    {
        private readonly MqttClient _mqttClient;
        private readonly MongoMessageRepository _messageRepository;
        private readonly MongoRoomRepository _roomRepository;

        public RoomService(MqttClient mqttClient, MongoMessageRepository messageRepository, MongoRoomRepository roomRepository)
        {
            _mqttClient = mqttClient;
            _messageRepository = messageRepository;
            _roomRepository = roomRepository;
        }
        public bool CreateRoom(string roomName)
        {
            try
            {
                // Verificar si la sala ya existe
                var existingRoom = _roomRepository.GetRoomByName(roomName);
                if (existingRoom != null)
                {
                    Console.WriteLine("La sala ya existe.");
                    return false;
                }

                // Crear y guardar la nueva sala
                _roomRepository.CreateRoom(roomName);
                Console.WriteLine($"Sala creada: {roomName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la sala: {ex.Message}");
                return false;
            }
        }

        public List<Room> ListRooms()
        {
            try
            {
                return _roomRepository.GetAllRooms();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar las salas: {ex.Message}");
                return new List<Room>();
            }
        }

        public bool LeaveRoom(string roomName)
        {
            try
            {
                string topic = $"/v1/room/{roomName}/messages";
                _mqttClient.Unsubscribe(new string[] { topic });
                Console.WriteLine($"Has salido de la sala: {roomName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al salir de la sala: {ex.Message}");
                return false;
            }
        }

        public List<Message> GetMessagesFromRoom(string roomName)
        {
            return _messageRepository.GetMessagesByRoom(roomName);
        }

        public bool JoinRoom(string roomName)
        {
            try
            {
                var room = _roomRepository.GetRoomByName(roomName);
                if (room == null)
                {
                    Console.WriteLine("La sala no existe.");
                    return false;
                }

                string topic = $"/v1/room/{roomName}/messages";
                _mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                Console.WriteLine($"Te has unido a la sala: {roomName}");

                // Recuperar mensajes anteriores
                var previousMessages = _messageRepository.GetMessagesByRoom(roomName);
                if (previousMessages.Count > 0)
                {
                    foreach (var message in previousMessages)
                    {
                        Console.WriteLine($"{message.Sender}: {message.Content} ({message.Timestamp})");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al unirse a la sala: {ex.Message}");
                return false;
            }
        }


        // Enviar un mensaje a la sala
        public void SendMessageToRoomV1(string roomName, string content, string sender)
        {
            string topic = $"/v1/room/{roomName}/messages";
            var message = new Message(roomName, content, sender);

            // Serializar el mensaje en JSON
            string jsonMessage = MessageSerializer.SerializeToJson(message);

            // Publicar el mensaje en MQTT
            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(jsonMessage), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Console.WriteLine($"Mensaje enviado en JSON: {jsonMessage}");

            // Guardar el mensaje en MongoDB
            _messageRepository.SaveMessage(message);
        }

        public void SendMessageToRoomV2(string roomName, string content, string sender)
        {
            string topic = $"/v2/room/{roomName}/messages";
            var message = new Message(roomName, content, sender);

            // Serializar el mensaje en MessagePack
            byte[] messagePackData = MessageSerializer.SerializeToMessagePack(message);

            // Publicar el mensaje en MQTT
            _mqttClient.Publish(topic, messagePackData, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Console.WriteLine($"Mensaje enviado en MessagePack");

            // Guardar el mensaje en MongoDB
            _messageRepository.SaveMessage(message);
        }


        // Manejar la recepción de mensajes
        public void ReceiveMessages()
        {
            // Registrar el evento de recepción de mensajes
            _mqttClient.MqttMsgPublishReceived += (sender, e) =>
            {
                if (e.Topic.StartsWith("/v1/"))
                {
                    // Deserializar JSON
                    string jsonMessage = Encoding.UTF8.GetString(e.Message);
                    var message = MessageSerializer.DeserializeFromJson(jsonMessage);
                    Console.WriteLine($"Mensaje recibido en {e.Topic}: {message.Sender}: {message.Content}");
                }
                else if (e.Topic.StartsWith("/v2/"))
                {
                    // Deserializar MessagePack
                    var message = MessageSerializer.DeserializeFromMessagePack(e.Message);
                    Console.WriteLine($"Mensaje recibido en {e.Topic}: {message.Sender}: {message.Content}");
                }
            };
        }
    }
}
