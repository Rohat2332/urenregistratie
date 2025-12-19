using HourRegistration.Core.Models;

namespace HourRegistration.Core.Interfaces.Repositories;

/// <summary>
/// Defines the contract for a repository that manages operations related to hour receipts.
/// </summary>
public interface IHourReceiptRepository
{
    /// <summary>
    /// Retrieves all hour receipts associated with a specific user by their identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user whose hour receipts are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="HourReceipt"/> objects.</returns>
    public Task<List<HourReceipt>> GetAllByUserId(int id);

    /// <summary>
    /// Retrieves all hour receipts from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="HourReceipt"/> objects.</returns>
    public Task<List<HourReceipt>> GetAll();

    /// <summary>
    /// Adds a new hour receipt to the repository.
    /// </summary>
    /// <param name="hourReceipt">The hour receipt object containing details such as user ID, project ID, status, hours worked, minutes worked, remark, and date.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Add(HourReceipt hourReceipt);

    /// <summary>
    /// Retrieves a specific hour receipt by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the hour receipt to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HourReceipt"/> object corresponding to the given identifier.</returns>
    public Task<HourReceipt?> GetById(int id);

}