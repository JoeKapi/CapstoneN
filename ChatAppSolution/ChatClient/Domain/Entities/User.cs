using System.Security.Principal;
using ChatClient.Domain.Interfaces;

namespace ChatClient.Domain.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }  // ID único para cada usuario
        public string Username { get; set; }
        public string Password { get; set; }

        public User(string username, string password)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
        }
    }
}
