using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Repositories;

public interface IHourReceiptRepository
{
    public Task<List<HourReceipt>> GetAllByUserId(int id);
    public Task<List<HourReceipt>> GetAll();

}