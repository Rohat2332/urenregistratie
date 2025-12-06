using System.Diagnostics;
using MySqlConnector;
using UrenRegistratie.Core.Interfaces.Repositories;
using UrenRegistratie.Core.Models;

namespace HourRegistration.Core.Data.Repositories;

public class HourReceiptsRepository : IHourReceiptRepository
{
    private readonly DatabaseConnection _databaseConnection;
    private  List<HourReceipt> _hoursReceipts;

    public HourReceiptsRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection =  databaseConnection;
        _hoursReceipts = [];
    }

    public async Task<List<HourReceipt>> GetAllByUserId(int userId)
    {
        _hoursReceipts = new List<HourReceipt>();
        
        const string query = "SELECT * FROM hour_receipts WHERE UserId = @UserId";

        try
        {
            await using var connection = await _databaseConnection.GetOpenConnectionAsync();
            await using var command = new MySqlCommand(query, connection);
            
            command.Parameters.AddWithValue("@UserId", userId);
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                _hoursReceipts.Add(MapReaderToHourReceipt(reader));
            }

            return _hoursReceipts;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error retrieving hour receipts for user {userId}: {e.Message}");
            throw;
        }
    }

    public async Task<List<HourReceipt>> GetAll()
    {
        _hoursReceipts = new List<HourReceipt>();
        
        const string query = "SELECT * FROM hour_receipts;";

        try
        {
            await using var connection = await _databaseConnection.GetOpenConnectionAsync();
            await using var command = new MySqlCommand(query, connection);
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                _hoursReceipts.Add(MapReaderToHourReceipt(reader));
            }

            return _hoursReceipts;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error retrieving all hour receipts: {e.Message}");
            throw;
        }
    }
    
    private HourReceipt MapReaderToHourReceipt(MySqlDataReader reader)
    {
        
        // Dit gebruikt de kolomnamen uit uw SQL query
        return new HourReceipt(
            reader.GetInt32("Id"),
            reader.GetInt32("UserId"),
            reader.GetInt32("ProjectId"),
            reader.GetString("Status"),
            reader.GetInt32("HoursWorked"),
            reader.GetInt32("MinutesWorked"),
            reader.GetString("remark"),
            reader.GetDateTime("Date")
        );
    }
}