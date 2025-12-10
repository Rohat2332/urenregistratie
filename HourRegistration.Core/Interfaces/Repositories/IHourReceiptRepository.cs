using HourRegistration.Core.Models;

namespace HourRegistration.Core.Interfaces.Repositories;

public interface IHourReceiptRepository
{
    public Task<List<HourReceipt>> GetAllByUserId(int id);
    public Task<List<HourReceipt>> GetAll();

}