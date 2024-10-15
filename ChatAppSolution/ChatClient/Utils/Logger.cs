public class Logger
{
    private void EnsureLogDirectoryExists(string roomName)
    {
        string logDirectory = $"logs/{roomName}";
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
    }

    // Guardar el mensaje en formato JSON
    public void LogJson(string roomName, string jsonMessage)
    {
        EnsureLogDirectoryExists(roomName);
        string filePath = $"logs/{roomName}/{roomName}.json";
        File.AppendAllText(filePath, jsonMessage + Environment.NewLine);
    }

    // Guardar el mensaje en formato MessagePack
    public void LogMessagePack(string roomName, byte[] messagePackData)
    {
        EnsureLogDirectoryExists(roomName);
        string filePath = $"logs/{roomName}/{roomName}.msgpack";
        using (var fileStream = new FileStream(filePath, FileMode.Append))
        {
            fileStream.Write(messagePackData, 0, messagePackData.Length);
        }
    }
}