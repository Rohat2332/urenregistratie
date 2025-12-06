using MySqlConnector;
using UrenRegistratie.Core.Interfaces.Services;

namespace HourRegistration.Core.Data;

public class DatabaseConnection
{
    private readonly IConfigurationService _configurationService;
    private const string ConnectionStringName = "HourRegistration";

    public DatabaseConnection(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<MySqlConnection> GetOpenConnectionAsync()
    {
        string connectionString = _configurationService.GetSetting(ConnectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found in configuration.");
        }

        try
        {
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            return connection;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Error connecting to MySQL: {ex.Message}");
            throw; 
        }
    }
}