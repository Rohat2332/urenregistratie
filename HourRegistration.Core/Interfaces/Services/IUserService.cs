using HourRegistration.Core.Models;

namespace HourRegistration.Core.Interfaces.Services;

public interface IUserService
{
    public User? GetByUsername(string username);
    public User? GetById(int id);
}