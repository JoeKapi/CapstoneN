using ChatClient.Domain.Entities;
using MongoDB.Driver;
using ChatClient.Infrastructure.Repositories;

public class MongoRoomRepository
{
    private readonly MongoDbContext _context;

    public MongoRoomRepository(MongoDbContext context)
    {
        _context = context;
    }

    public void CreateRoom(string roomName)
    {
        var newRoom = new Room(roomName);
        _context.Rooms.InsertOne(newRoom);
    }

    public List<Room> GetAllRooms()
    {
        return _context.Rooms.Find(_ => true).ToList(); 
    }

    public Room GetRoomByName(string roomName)
    {
        return _context.Rooms.Find(room => room.Name == roomName).FirstOrDefault();
    }

}
