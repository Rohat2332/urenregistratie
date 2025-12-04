using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Repositories;

public interface IUserRepository
{
    public User? GetByUsername(string username);
    public User? GetById(int id);
}