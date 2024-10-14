using System;

namespace ChatClient.Domain.Entities
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public Room(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CreatedAt = DateTime.Now;
        }
    }
}
