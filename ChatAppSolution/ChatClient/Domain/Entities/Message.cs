using System.Security.Cryptography;
using System.Text;

public class Message
{
    public Guid Id { get; set; }
    public string RoomName { get; set; }
    public string Content { get; set; }
    public string Sender { get; set; }
    public DateTime Timestamp { get; set; }
    public string Hash { get; set; }  

    public Message(string roomName, string content, string sender)
    {
        Id = Guid.NewGuid();
        RoomName = roomName;
        Content = content;
        Sender = sender;
        Timestamp = DateTime.Now;
        Hash = GenerateHash(content); 
    }

    private string GenerateHash(string content)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
