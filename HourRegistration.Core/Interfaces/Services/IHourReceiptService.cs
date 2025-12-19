using HourRegistration.Core.Models;

namespace UrenRegistratie.Core.Interfaces.Services;

/// <summary>
/// Provides methods for managing hour receipts, including retrieving and adding receipts.
/// </summary>
public interface IHourReceiptService
{
    /// <summary>
    /// Retrieves all hour receipts from the service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of hour receipt objects.</returns>
    public Task<List<HourReceipt>> GetAll();

    /// <summary>
    /// Adds a new hour receipt to the service.
    /// </summary>
    /// <param name="hourReceipt">The hour receipt object containing details such as user ID, project ID, status, hours worked, minutes worked, remark, and date.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Add(HourReceipt hourReceipt);

    /// <summary>
    /// Retrieves an hour receipt by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the hour receipt to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the hour receipt object associated with the specified identifier.</returns>
    public Task<HourReceipt?> GetById(int id);
}