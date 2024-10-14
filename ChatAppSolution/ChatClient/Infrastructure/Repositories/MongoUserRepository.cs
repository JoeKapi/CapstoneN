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

        public IEnumerable<User> GetAll()
        {
            return _context.Users.Find(user => true).ToList();
        }

        public User GetById(Guid id)
        {
            return _context.Users.Find(user => user.Id == id).FirstOrDefault();
        }

        public void Add(User user)
        {
            _context.Users.InsertOne(user);
        }

        public void Update(User user)
        {
            _context.Users.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            _context.Users.DeleteOne(user => user.Id == id);
        }
    }
}
