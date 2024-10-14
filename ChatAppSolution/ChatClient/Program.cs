using ChatClient.Application.Services;
using ChatClient.Infrastructure.Repositories;
using uPLibrary.Networking.M2Mqtt;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inicializar el contexto de MongoDB
            MongoDbContext context = new MongoDbContext();

            // Crear el cliente MQTT
            //MqttClient mqttClient = new MqttClient("146.190.213.152");
            //mqttClient.Connect(Guid.NewGuid().ToString());
            MqttClient mqttClient = new MqttClient("test.mosquitto.org");
            mqttClient.Connect(Guid.NewGuid().ToString());


            // Crear el repositorio de mensajes
            MongoMessageRepository messageRepository = new MongoMessageRepository(context);

            // Crear el servicio de la sala con el repositorio de mensajes
            RoomService roomService = new RoomService(mqttClient, messageRepository);

            // Lógica para unirse a la sala y manejar mensajes
            Console.WriteLine("Ingresa el nombre de la sala de chat a la que deseas unirte: ");
            string roomName = Console.ReadLine();

            roomService.JoinRoom(roomName);
            roomService.ReceiveMessages();

            while (true)
            {
                Console.WriteLine("Escribe tu mensaje: ");
                string message = Console.ReadLine();

                roomService.SendMessageToRoom(roomName, "TuNombreUsuario", message);
            }
        }
    }
}
