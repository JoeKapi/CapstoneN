using ChatClient.Application.Services;
using ChatClient.Domain.Entities;

public class CommandService
{
    private readonly RoomService _roomService;
    private readonly AuthService _authService;
    private User _currentUser; // Usuario autenticado

    public CommandService(RoomService roomService, AuthService authService)
    {
        _roomService = roomService;
        _authService = authService;
    }

    public void Run()
    {
        while (true)
        {
            MostrarMenuPrincipal();
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    if (Login())
                    {
                        MostrarMenuComandos();
                    }
                    break;
                case "3":
                    Console.WriteLine("Saliendo de la aplicación...");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    break;
            }
        }
    }

    private void MostrarMenuPrincipal()
    {
        Console.WriteLine("1. Registrar nuevo usuario");
        Console.WriteLine("2. Iniciar sesión");
        Console.WriteLine("3. Salir");
        Console.Write("Elige una opción: ");
    }

    private void MostrarMenuComandos()
    {
        while (true)
        {
            Console.WriteLine("\nComandos disponibles:");
            Console.WriteLine("/crear – Crear una nueva sala de chat y unirse a ella");
            Console.WriteLine("/listar – Listar salas de chat");
            Console.WriteLine("/unirse – Unirse a una sala de chat");
            Console.WriteLine("/salir – Salir de una sala de chat");
            Console.WriteLine("/salirapp – Salir del chat");
            Console.WriteLine("/logout – Cerrar sesión");
            Console.WriteLine("/menu – Mostrar este menú");
            Console.Write("Escribe un comando: ");
            string comando = Console.ReadLine();

            switch (comando)
            {
                case "/crear":
                    CreateRoom();
                    break;
                case "/listar":
                    ListRooms();
                    break;
                case "/unirse":
                    JoinRoom();
                    break;
                case "/salir":
                    LeaveRoom();
                    break;
                case "/salirapp":
                    Console.WriteLine("Saliendo de la aplicación...");
                    return;
                case "/logout":
                    Logout();
                    return;
                case "/menu":
                    MostrarMenuComandos();
                    break;
                default:
                    Console.WriteLine("Comando no reconocido. Escribe /menu para ver las opciones.");
                    break;
            }
        }
    }

    private bool Login()
    {
        Console.WriteLine("Ingresa tu nombre de usuario:");
        string username = Console.ReadLine();

        Console.WriteLine("Ingresa tu contraseña:");
        string password = Console.ReadLine();

        bool autenticado = _authService.Login(username, password);
        if (autenticado)
        {
            _currentUser = _authService.GetCurrentUser(username);
            Console.WriteLine($"Bienvenido {username}.");
            return true;
        }
        return false;
    }

    private void Logout()
    {
        if (_currentUser != null)
        {
            _authService.Logout(_currentUser.Username);  // Delegar logout a AuthService
            _currentUser = null;
        }
        else
        {
            Console.WriteLine("No estás logueado.");
        }
    }

    private void Register()
    {
        Console.WriteLine("Ingresa tu nombre de usuario:");
        string username = Console.ReadLine();

        Console.WriteLine("Ingresa tu contraseña:");
        string password = Console.ReadLine();

        if (_authService.Register(username, password))
        {
            Console.WriteLine("Usuario registrado exitosamente.");
        }
        else
        {
            Console.WriteLine("No se pudo registrar el usuario. Intenta con otro nombre de usuario.");
        }
    }

    private void CreateRoom()
    {
        Console.WriteLine("Ingresa el nombre de la nueva sala:");
        string roomName = Console.ReadLine();
        if (_roomService.CreateRoom(roomName))
        {
            Console.WriteLine($"Sala creada: {roomName}");
            _roomService.JoinRoom(roomName, _currentUser.Username);
            Console.WriteLine($"Te has unido a la sala: {roomName}");
            IniciarChat(roomName);
        }
        else
        {
            Console.WriteLine("No se pudo crear la sala.");
        }
    }

    private void ListRooms()
    {
        var rooms = _roomService.ListRooms();
        if (rooms.Count > 0)
        {
            Console.WriteLine("Salas disponibles:");
            foreach (var room in rooms)
            {
                Console.WriteLine($"- {room.Name}");
            }
        }
        else
        {
            Console.WriteLine("No tienes salas disponibles.");
        }
    }

    private void JoinRoom()
    {
        Console.WriteLine("Ingresa el nombre de la sala a la que deseas unirte:");
        string roomName = Console.ReadLine();
        if (_roomService.JoinRoom(roomName, _currentUser.Username))
        {
            Console.WriteLine($"Te has unido a la sala: {roomName}");
            IniciarChat(roomName);
        }
        else
        {
            Console.WriteLine("No se pudo encontrar o unirse a la sala.");
        }
    }

    private void LeaveRoom()
    {
        Console.WriteLine("Ingresa el nombre de la sala de la que deseas salir:");
        string roomName = Console.ReadLine();
        if (_roomService.LeaveRoom(roomName))
        {
            Console.WriteLine($"Has salido de la sala: {roomName}");
        }
        else
        {
            Console.WriteLine("No se pudo encontrar o salir de la sala.");
        }
    }

    private void IniciarChat(string roomName)
    {
        Console.WriteLine("Escribe tu mensaje (o escribe /salir para salir de la sala):");

        while (true)
        {
            string inputMessage = Console.ReadLine();

            if (inputMessage == "/salir")
            {
                _roomService.LeaveRoom(roomName);
                Console.WriteLine($"Has salido de la sala: {roomName}");
                break;
            }

            _roomService.SendMessageToRoomV1(roomName, inputMessage, _currentUser.Username);
        }
    }
}
