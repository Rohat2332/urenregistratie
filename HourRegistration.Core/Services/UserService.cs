using UrenRegistratie.Core.Interfaces.Repositories;
using UrenRegistratie.Core.Interfaces.Services;
using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? Get(int id)
    {
        return _userRepository.GetById(id);
    }

    public User? GetByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public User? GetById(int id)
    {
        return _userRepository.GetById(id);
    }
}