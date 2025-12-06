using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Services;

public interface IHourReceiptService
{
    public Task<List<HourReceipt>> GetAll();
}