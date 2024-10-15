using ChatClient.Domain.Entities;
using ChatClient.Infrastructure.Repositories;

public class AuthService
{
    private readonly MongoUserRepository _userRepository;
    private readonly UserRegistryService _userRegistryService;

    public AuthService(MongoUserRepository userRepository, UserRegistryService userRegistryService)
    {
        _userRepository = userRepository;
        _userRegistryService = userRegistryService;
    }

    // Registro de nuevos usuarios
    public bool Register(string username, string password)
    {
        var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            Console.WriteLine("El usuario ya existe.");
            return false;
        }

        var newUser = new User(username, password);
        try
        {
            _userRepository.Add(newUser);
            _userRegistryService.RegisterUser(newUser); // Añadir a los usuarios registrados
            Console.WriteLine("Usuario registrado exitosamente.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar usuario: {ex.Message}");
            return false;
        }
    }

    // Autenticación de usuarios
    public bool Login(string username, string password)
    {
        var user = _userRepository.GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            // Verificar si el usuario ya está logueado
            if (_userRegistryService.IsUserLoggedIn(username))
            {
                Console.WriteLine($"El usuario {username} ya está logueado.");
                return false;
            }

            _userRegistryService.LoginUser(username); // Añadir a los usuarios logueados
            return true;
        }
        else
        {
            Console.WriteLine("Usuario o contraseña incorrectos.");
            return false;
        }
    }

    public void Logout(string username)
    {
        // Remover de los usuarios logueados
        if (_userRegistryService.IsUserLoggedIn(username))
        {
            _userRegistryService.LogoutUser(username);
            Console.WriteLine($"Usuario {username} ha cerrado sesión.");
        }
        else
        {
            Console.WriteLine($"Usuario {username} no estaba logueado.");
        }
    }

    // Obtener el usuario actual
    public User GetCurrentUser(string username)
    {
        return _userRepository.GetAll().FirstOrDefault(u => u.Username == username);
    }
}
