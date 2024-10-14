using ChatClient.Domain.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace ChatClient.Infrastructure.Repositories
{
    public class MongoMessageRepository
    {
        private readonly MongoDbContext _context;

        public MongoMessageRepository(MongoDbContext context)
        {
            _context = context;
        }

        public void SaveMessage(Message message)
        {
            _context.Messages.InsertOne(message);
        }

        // Obtener mensajes por sala de chat
        public List<Message> GetMessagesByRoom(string roomName)
        {
            return _context.Messages.Find(m => m.RoomName == roomName).ToList();
        }
    }
}
