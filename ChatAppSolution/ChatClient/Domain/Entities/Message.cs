using System;

namespace ChatClient.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(string roomName, string content, string sender)
        {
            Id = Guid.NewGuid();
            RoomName = roomName;
            Content = content;
            Sender = sender;
            Timestamp = DateTime.Now;
        }
    }
}