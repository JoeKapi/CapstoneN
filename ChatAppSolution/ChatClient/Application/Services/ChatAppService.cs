using ChatClient.Application.Services;

namespace ChatClient.Application
{
    public class ChatAppService
    {
        private readonly CommandService _commandService;

        public ChatAppService(AuthService authService, RoomService roomService)
        {
            _commandService = new CommandService(roomService, authService);
        }

        public void Run()
        {
            _commandService.Run(); 
        }
    }
}