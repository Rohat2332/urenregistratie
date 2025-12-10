using HourRegistration.Core.Interfaces.Repositories;
using HourRegistration.Core.Models;
using UrenRegistratie.Core.Interfaces.Services;

namespace HourRegistration.Core.Services;

/// <summary>
/// Service class responsible for handling business logic related to hour receipts.
/// Provides functionality to manage hour receipt operations, such as retrieving
/// and adding hour receipts.
/// </summary>
public class HourReceiptService : IHourReceiptService
{
    private readonly IHourReceiptRepository _hourReceiptRepository;

    public HourReceiptService(IHourReceiptRepository hourReceiptRepository, IUserRepository userRepository)
    {
        _hourReceiptRepository = hourReceiptRepository;
    }


    /// <summary>
    /// Retrieves all hour receipts from the repository.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a list of all hour receipts as the result.</returns>
    public async Task<List<HourReceipt>> GetAll()
    {
        return await _hourReceiptRepository.GetAll();
    }

    /// <summary>
    /// Adds a new hour receipt to the repository.
    /// </summary>
    /// <param name="hourReceipt">The hour receipt to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Add(HourReceipt hourReceipt)
    {
        await _hourReceiptRepository.Add(hourReceipt);
    }
}