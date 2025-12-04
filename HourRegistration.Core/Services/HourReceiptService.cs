using UrenRegistratie.Core.Interfaces.Repositories;
using UrenRegistratie.Core.Interfaces.Services;
using UrenRegistratie.Core.Models;

namespace UrenRegistratie.Core.Services;

public class HourReceiptService : IHourReceiptService
{
    private readonly IHourReceiptRepository _hourReceiptRepository;

    public HourReceiptService(IHourReceiptRepository hourReceiptRepository, IUserRepository userRepository)
    {
        _hourReceiptRepository = hourReceiptRepository;
    }

    public List <HourReceipt> GetAll()
    {
        return _hourReceiptRepository.GetAll();
    }
}