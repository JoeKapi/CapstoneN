using ChatClient.Domain.Entities;

namespace ChatClient.Domain.Interfaces
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        void Register(User user);
    }
}
