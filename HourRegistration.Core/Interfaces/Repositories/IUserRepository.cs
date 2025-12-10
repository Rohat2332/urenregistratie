using HourRegistration.Core.Models;

namespace HourRegistration.Core.Interfaces.Repositories;

public interface IUserRepository
{
    public User? GetByUsername(string username);
    public User? GetById(int id);
}