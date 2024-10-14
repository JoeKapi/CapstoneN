using ChatClient.Domain.Entities;
using ChatClient.Infrastructure.Repositories;
using System;

namespace ChatClient.Application.Services
{
    public class AuthService
    {
        private readonly MongoUserRepository _userRepository;
        private User _currentUser; 

        public AuthService(MongoUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                _currentUser = user; 
                return true;
            }
            return false;
        }

        // Obtener el usuario autenticado actual
        public User GetCurrentUser()
        {
            return _currentUser; 
        }
    }
}
