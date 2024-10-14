using ChatClient.Domain.Entities;
using MongoDB.Driver;

namespace ChatClient.Infrastructure.Repositories
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext()
        {
            // Cadena de conexión desde MongoDB Atlas
            var client = new MongoClient("mongodb+srv://Kappa:ChatApp@cluster0.ngv6h1x.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");

            // Nombre de la base de datos
            _database = client.GetDatabase("ChatApp");
        }

        // Colección de usuarios
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");

        // Colecciones
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");
        public IMongoCollection<Room> Rooms => _database.GetCollection<Room>("rooms");
    }
}
