using System.Diagnostics;
using HourRegistration.Core.Interfaces.Repositories;
using HourRegistration.Core.Models;
using MySqlConnector;

namespace HourRegistration.Core.Data.Repositories;

public class HourReceiptsRepository : IHourReceiptRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public HourReceiptsRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection =  databaseConnection;
    }

    /// <summary>
    /// Retrieves all hour receipts associated with a specific user by their identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose hour receipts are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="HourReceipt"/> objects.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the retrieval of hour receipts.</exception>
    public async Task<List<HourReceipt>> GetAllByUserId(int userId)
    {
        var hoursReceipts = new List<HourReceipt>();
        
        const string query = "SELECT * FROM hour_receipts WHERE id = @UserId";

        try
        {
            await using var connection = await _databaseConnection.GetOpenConnectionAsync();
            await using var command = new MySqlCommand(query, connection);
            
            command.Parameters.AddWithValue("@UserId", userId);
            
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                hoursReceipts.Add(MapReaderToHourReceipt(reader));
            }

            return hoursReceipts;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error retrieving hour receipts for user {userId}: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all hour receipts from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="HourReceipt"/> objects.</returns>
    /// <exception cref="Exception">Thrown when an error occurs while fetching hour receipts from the database.</exception>
    public async Task<List<HourReceipt>> GetAll()
    {
        var hoursReceipts = new List<HourReceipt>();
        
        const string query = "SELECT * FROM hour_receipts;";

        try
        {
            await using var connection = await _databaseConnection.GetOpenConnectionAsync();
            await using var command = new MySqlCommand(query, connection);
            
            await using var reader = await command.ExecuteReaderAsync();
            
            var hasRows = reader.HasRows; 
            Debug.WriteLine($"DB Antwoord: Heeft rijen = {hasRows}"); // Voeg dit toe
            
            while (await reader.ReadAsync())
            {
                hoursReceipts.Add(MapReaderToHourReceipt(reader));
            }

            return hoursReceipts;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error retrieving all hour receipts: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Adds a new hour receipt to the database.
    /// </summary>
    /// <param name="hourReceipt">The hour receipt object containing information like user ID, project ID, status,
    /// hours worked, minutes worked, remark, and date to be inserted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when an error occurs while adding the hour receipt to the database.</exception>
    public async Task Add(HourReceipt hourReceipt)
    {
        const string query =
            "INSERT INTO hour_receipts (user_id, status, date, hours_worked, minutes_worked) VALUES (@UserId, @Status, @Date,@hours_worked, @minutes_worked)";

        try
        {
            await using var connection = await _databaseConnection.GetOpenConnectionAsync();
            await using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", hourReceipt.UserId);
            command.Parameters.AddWithValue("@Status", hourReceipt.Status);
            command.Parameters.AddWithValue("@Date", hourReceipt.Date);
            command.Parameters.AddWithValue("@hours_worked", hourReceipt.HoursWorked);
            command.Parameters.AddWithValue("@minutes_worked", hourReceipt.MinutesWorked);

            var reader = await command.ExecuteNonQueryAsync();

            if (reader == 0)
            {
                throw new Exception("Error inserting hour receipt");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific hour receipt by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the hour receipt to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HourReceipt"/> object if found, or null if no match is found.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the retrieval operation.</exception>
    public async Task<HourReceipt?> GetById(int id)
    {
        HourReceipt? hourReceipt = null;
        const string query = "SELECT * FROM hour_receipts WHERE id = @Id;";

        try
        {
            await using var connection = await _databaseConnection.GetOpenConnectionAsync();
            await using var command = new MySqlCommand(query, connection);
            
            command.Parameters.AddWithValue("@Id", id);
            
            await using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                hourReceipt = MapReaderToHourReceipt(reader);
                
            }

            return hourReceipt;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error retrieving hour receipt with ID {id}: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Maps a data reader to an HourReceipt object by extracting and converting the appropriate columns.
    /// </summary>
    /// <param name="reader">The MySqlDataReader containing the result set from the database query.</param>
    /// <returns>An HourReceipt object created from the data in the reader.</returns>
    private static HourReceipt MapReaderToHourReceipt(MySqlDataReader reader)
    {
        // Hulpfunctie om een nullable int veilig te lezen
        int? GetNullableInt(string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }
    
        // Hulpfunctie om een nullable string veilig te lezen
        string? GetNullableString(string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        return new HourReceipt(
            reader.GetInt32("Id"),
            reader.GetInt32("user_id"), 
            GetNullableInt("project_id"), 
            reader.GetString("Status"), 
            reader.GetInt32("hours_worked"), 
            reader.GetInt32("minutes_worked"),
            GetNullableString("remark"), 
            reader.GetDateTime("Date")
        );
    }
}