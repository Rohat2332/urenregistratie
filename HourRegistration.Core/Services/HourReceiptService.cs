using HourRegistration.Core.Interfaces.Repositories;
using HourRegistration.Core.Models;
using UrenRegistratie.Core.Interfaces.Services;

namespace HourRegistration.Core.Services;

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