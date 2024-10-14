using ChatClient.Application.Services;
using ChatClient.Infrastructure.Repositories;
using MongoDB.Driver;
using uPLibrary.Networking.M2Mqtt;

namespace ChatClient.Application
{
    public class ChatAppService
    {
        private readonly CommandService _commandService;

        public ChatAppService(AuthService authService, RoomService roomService)
        {
            // Inicializar CommandService con los servicios de autenticación y gestión de salas
            _commandService = new CommandService(roomService, authService);
        }

        public void Run()
        {
            _commandService.Run();  // Ejecuta el flujo principal del comando
        }
    }
}