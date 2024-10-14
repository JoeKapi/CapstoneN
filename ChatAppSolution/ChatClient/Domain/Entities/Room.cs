using ChatClient.Domain.Interfaces;

namespace ChatClient.Domain.Entities
{
    public class Room : IEntity
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
    }
}
