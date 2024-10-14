using ChatClient.Domain.Entities;
using ChatClient.Domain.Interfaces;
using MongoDB.Driver;

namespace ChatClient.Infrastructure.Repositories
{
    public class MongoUserRepository : IRepository<User>
    {
        private readonly MongoDbContext _context;

        public MongoUserRepository(MongoDbContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        public IEnumerable<User> GetAll()
        {
            return _context.Users.Find(user => true).ToList();
        }

        // Obtener usuario por ID
        public User GetById(Guid id)
        {
            return _context.Users.Find(user => user.Id == id).FirstOrDefault();
        }

        // Agregar nuevo usuario
        public void Add(User user)
        {
            var existingUser = _context.Users.Find(u => u.Username == user.Username).FirstOrDefault();
            if (existingUser == null)
            {
                _context.Users.InsertOne(user);
            }
            else
            {
                throw new Exception("El usuario ya existe.");
            }
        }

        // Actualizar un usuario existente
        public void Update(User user)
        {
            var result = _context.Users.ReplaceOne(u => u.Id == user.Id, user);
            if (result.MatchedCount == 0)
            {
                throw new Exception("Usuario no encontrado para actualización.");
            }
        }

        // Eliminar un usuario por ID
        public void Delete(Guid id)
        {
            var result = _context.Users.DeleteOne(user => user.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new Exception("Usuario no encontrado para eliminación.");
            }
        }

        // Autenticar usuario
        public User Authenticate(string username, string password)
        {
            return _context.Users.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
        }
    }
}
