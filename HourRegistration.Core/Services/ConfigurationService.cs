using UrenRegistratie.Core.Interfaces.Services;

namespace UrenRegistratie.Core.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfigurationService _configuration;

    public ConfigurationService(IConfigurationService configuration)
    {
        _configuration = configuration;
    }


    public string GetSetting(string key, string defaultValue = "")
    {
        return _configuration[key] ?? defaultValue;
    }

    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        string value = GetSetting(key);

        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        if (bool.TryParse(value, out bool result))
        {
            return result;
        }

        // Return default if parsing fails
        return defaultValue;
    }

    public string this[string key] => throw new NotImplementedException();
}