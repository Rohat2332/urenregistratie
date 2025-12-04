using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Repositories;

public interface IHourReceiptRepository
{
    public List<HourReceipt> GetAllByUserId(int id);
    public List <HourReceipt> GetAll();

}