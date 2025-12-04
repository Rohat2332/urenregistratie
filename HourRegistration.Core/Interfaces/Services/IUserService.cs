using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Services;

public interface IUserService
{
    public User? GetByUsername(string username);
    public User? GetById(int id);
}