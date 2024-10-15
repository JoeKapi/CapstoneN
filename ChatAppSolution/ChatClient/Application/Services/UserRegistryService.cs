using System;
using System.Collections.Generic;
using ChatClient.Domain.Entities;
using ChatClient.Infrastructure.Repositories;

public class UserRegistryService
{
    private readonly HashSet<string> _loggedInUsers = new HashSet<string>();
    private readonly HashSet<string> _registeredUsers = new HashSet<string>();

    public UserRegistryService(MongoUserRepository userRepository)
    {
        // Cargar usuarios registrados desde la base de datos
        var usersFromDb = userRepository.GetAll();
        foreach (var user in usersFromDb)
        {
            _registeredUsers.Add(user.Username);
        }
    }

    // Registrar un usuario nuevo en la aplicación
    public void RegisterUser(User user)
    {
        if (_registeredUsers.Contains(user.Username))
        {
            Console.WriteLine($"El usuario {user.Username} ya está registrado.");
            return;
        }

        _registeredUsers.Add(user.Username);
        Console.WriteLine($"Usuario {user.Username} registrado correctamente.");
    }

    // Iniciar sesión del usuario
    public void LoginUser(string username)
    {
        if (!_registeredUsers.Contains(username))
        {
            Console.WriteLine($"El usuario {username} no está registrado.");
            return;
        }

        if (_loggedInUsers.Contains(username))
        {
            Console.WriteLine($"El usuario {username} ya está logueado.");
            return;
        }

        _loggedInUsers.Add(username);
        Console.WriteLine($"Usuario {username} logueado exitosamente.");
    }

    // Cerrar sesión del usuario
    public void LogoutUser(string username)
    {
        if (!_loggedInUsers.Contains(username))
        {
            Console.WriteLine($"El usuario {username} no estaba logueado.");
            return;
        }

        _loggedInUsers.Remove(username);
        Console.WriteLine($"Usuario {username} ha cerrado sesión.");
    }

    // Verificar si un usuario está logueado
    public bool IsUserLoggedIn(string username)
    {
        return _loggedInUsers.Contains(username);
    }
}
