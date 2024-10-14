using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using ChatClient.Domain.Entities;
using ChatClient.Infrastructure.Repositories;

namespace ChatClient.Application.Services
{
    public class RoomService
    {
        private readonly MqttClient _mqttClient;
        private readonly MongoMessageRepository _messageRepository;

        public RoomService(MqttClient mqttClient, MongoMessageRepository messageRepository)
        {
            _mqttClient = mqttClient;
            _messageRepository = messageRepository;
        }

        // Unirse a una sala de chat
        public void JoinRoom(string roomName)
        {
            string topic = $"/v1/room/{roomName}/messages";
            _mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            Console.WriteLine($"Te has unido a la sala: {roomName}");
        }

        // Enviar un mensaje a la sala
        public void SendMessageToRoom(string roomName, string sender, string messageContent)
        {
            string topic = $"/v1/room/{roomName}/messages";
            string message = $"{sender}: {messageContent}";

            // Guardar el mensaje en la base de datos
            var chatMessage = new Message(roomName, messageContent, sender);
            _messageRepository.SaveMessage(chatMessage);

            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Console.WriteLine($"Mensaje enviado a {roomName}: {message}");
        }


        // Manejar la recepción de mensajes
        public void ReceiveMessages()
        {
            _mqttClient.MqttMsgPublishReceived += (sender, e) =>
            {
                string receivedMessage = Encoding.UTF8.GetString(e.Message);
                Console.WriteLine($"Mensaje recibido en {e.Topic}: {receivedMessage}");
            };
        }
    }
}
