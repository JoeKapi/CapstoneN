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
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Username == username);

            if (user == null || user.Password != password)
            {
                return null;
            }

            return user;
        }


        public void Register(User user)
        {
            // Verificar si el usuario ya existe por su nombre de usuario
            var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            {
                throw new Exception("El nombre de usuario ya está en uso.");
            }
            _userRepository.Add(user);
        }

    }
}
