using HourRegistration.Core.Interfaces.Repositories;
using HourRegistration.Core.Models;

namespace HourRegistration.Core.Data.Repositories;

public class UserRepository  : IUserRepository
{
    private readonly List<User> _users;

    public UserRepository()
    {
        _users =
        [
            new User(0,"user1", "password123"),
            new User(0,"user2", "password123"),
            new User(0,"user3", "password123")
        ];
    }

    public User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }

    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }
}