public class Logger
{
    private readonly string _logDirectory;

    public Logger(string logDirectory = "logs")
    {
        _logDirectory = logDirectory;
        EnsureLogDirectoryExists();
    }

    // Verificar y crear la carpeta de logs si no existe
    private void EnsureLogDirectoryExists()
    {
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    // Guardar mensajes serializados en JSON
    public void LogJson(string roomName, string jsonMessage)
    {
        string filePath = Path.Combine(_logDirectory, $"{roomName}.json");
        File.AppendAllText(filePath, jsonMessage + Environment.NewLine);
    }

    // Guardar mensajes serializados en MessagePack
    public void LogMessagePack(string roomName, byte[] messagePackData)
    {
        string filePath = Path.Combine(_logDirectory, $"{roomName}.msgpack");
        using (var fileStream = new FileStream(filePath, FileMode.Append))
        {
            fileStream.Write(messagePackData, 0, messagePackData.Length);
        }
    }
}
