using MySqlConnector;
using UrenRegistratie.Core.Interfaces.Services;

namespace HourRegistartion.Core.Data;

public class DatabaseConnection
{
    private readonly IConfigurationService _configurationService;
    private readonly string _connectionStringName = "HourRegistartion";

    public DatabaseConnection(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public async Task<MySqlConnection> GetOpenConnectionAsync()
    {
        string connectionString = _configurationService.GetSetting(_connectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{_connectionStringName}' not found in configuration.");
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