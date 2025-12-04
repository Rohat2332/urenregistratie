using UrenRegistratie.Core.Interfaces.Repositories;
using UrenRegistratie.Core.Models;

namespace HourRegistartion.Core.Data.Repositories;

public class HourReceiptsRepository : IHourReceiptRepository
{
    private readonly List<HourReceipt> _hoursReceipts;

    public HourReceiptsRepository()
    {
        _hoursReceipts =
        [
            new HourReceipt(0,0,new DateTime(2025, 12,4),"no Entry"),
            new HourReceipt(1,0,new DateTime(2025, 12,3),"no Entry"),
            new HourReceipt(2,0,new DateTime(2025, 12,2),"no Entry"),
            new HourReceipt(3,0,new DateTime(2025, 12,1),"no Entry")
        ];
    }

    public HourReceipt? GetById(int id)
    {
        return _hoursReceipts.Find(h => h.Id == id);
    }

    public List<HourReceipt> GetAllByUserId(int id)
    {
       return _hoursReceipts.FindAll(h => h.UserId == id);
    }

    public List<HourReceipt> GetAll()
    {
        return _hoursReceipts;

    }
}