using System.Text;
using MessagePack;
using Newtonsoft.Json;
using ChatClient.Domain.Entities;

namespace ChatClient.Utils
{
    public static class MessageSerializer
    {
        // Serialización para versión 1 (JSON)
        public static string SerializeToJson(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }

        // Deserialización para versión 1 (JSON)
        public static Message DeserializeFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }

        // Serialización para versión 2 (MessagePack)
        public static byte[] SerializeToMessagePack(Message message)
        {
            return MessagePackSerializer.Serialize(message);
        }

        // Deserialización para versión 2 (MessagePack)
        public static Message DeserializeFromMessagePack(byte[] data)
        {
            return MessagePackSerializer.Deserialize<Message>(data);
        }
    }
}
