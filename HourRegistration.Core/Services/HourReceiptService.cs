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


    public async Task<List<HourReceipt>> GetAll()
    {
        return await _hourReceiptRepository.GetAll();
    }
}