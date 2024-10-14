using ChatClient.Domain.Entities;
using ChatClient.Domain.Interfaces;

namespace ChatClient.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public User Authenticate(string username, string password)
        {
            // Buscar el usuario por nombre de usuario
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Username == username);

            // Verificar si la contraseña coincide
            if (user == null || user.Password != password)
            {
                return null; // Retornar null si no se puede autenticar
            }

            return user; // Retornar el usuario si la autenticación es exitosa
        }


        public void Register(User user)
        {
            // Verificar si el usuario ya existe por su nombre de usuario
            var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            {
                // Si el nombre de usuario ya está en uso, lanza una excepción o retorna un mensaje de error
                throw new Exception("El nombre de usuario ya está en uso.");
            }

            // Si no existe, proceder con la adición del nuevo usuario
            _userRepository.Add(user);
        }

    }
}
