using ChatClient.Application.Services;
using ChatClient.Infrastructure.Repositories;
using uPLibrary.Networking.M2Mqtt;
using System;
using ChatClient.Application;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inicializar MQTT Client
            var mqttClient = new MqttClient("broker.hivemq.com");

            // Inicializar MongoDBContext y repositorios
            var mongoDbContext = new MongoDbContext();
            var messageRepository = new MongoMessageRepository(mongoDbContext);  // Repositorio de mensajes
            var roomRepository = new MongoRoomRepository(mongoDbContext);       // Repositorio de salas
            var userRepository = new MongoUserRepository(mongoDbContext);       // Repositorio de usuarios

            // Inicializar servicios de aplicación
            var authService = new AuthService(userRepository);
            var roomService = new RoomService(mqttClient, messageRepository, roomRepository);  

            // Conectar al broker MQTT
            mqttClient.Connect(Guid.NewGuid().ToString());

            // Crear y ejecutar el servicio principal de chat
            var chatAppService = new ChatAppService(authService, roomService);
            chatAppService.Run();
        }
    }
}
