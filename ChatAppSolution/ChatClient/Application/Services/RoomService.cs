using System;
using System.IO;
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
        private readonly Logger _logger;

        public RoomService(MqttClient mqttClient, MongoMessageRepository messageRepository, MongoRoomRepository roomRepository)
        {
            _mqttClient = mqttClient;
            _messageRepository = messageRepository;
            _roomRepository = roomRepository;
            _logger = new Logger(); 
        }

        public bool CreateRoom(string roomName)
        {
            try
            {
                var existingRoom = _roomRepository.GetRoomByName(roomName);
                if (existingRoom != null)
                {
                    Console.WriteLine("La sala ya existe.");
                    return false;
                }

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

        public bool JoinRoom(string roomName, string currentUser)
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

                // Comienza a recibir mensajes en tiempo real
                ReceiveMessages(roomName, currentUser);

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
            var message = new Message(roomName, EncryptionHelper.Encrypt(content), sender);

            string jsonMessage = MessageSerializer.SerializeToJson(message);

            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(jsonMessage), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

            _logger.LogJson(roomName, jsonMessage);

            _messageRepository.SaveMessage(message);
        }


        public void SendMessageToRoomV2(string roomName, string content, string sender)
        {
            string topic = $"/v2/room/{roomName}/messages";
            var message = new Message(roomName, EncryptionHelper.Encrypt(content), sender);

            byte[] messagePackData = MessageSerializer.SerializeToMessagePack(message);

            _mqttClient.Publish(topic, messagePackData, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

            _logger.LogMessagePack(roomName, messagePackData);

            _messageRepository.SaveMessage(message);
        }


        // Manejar la recepción de mensajes
        public void ReceiveMessages(string roomName, string currentUser)
        {
            _mqttClient.MqttMsgPublishReceived += (sender, e) =>
            {
                string jsonMessage = Encoding.UTF8.GetString(e.Message);
                var message = MessageSerializer.DeserializeFromJson(jsonMessage);

                if (message.Sender != currentUser)
                {
                    string decryptedContent = EncryptionHelper.Decrypt(message.Content); 
                    Console.WriteLine($"Mensaje recibido en {e.Topic}: {message.Sender}: {decryptedContent}");
                }
            };
        }
    }
}
