namespace UrenRegistratie.Core.Interfaces.Services;

public interface IConfigurationService
{
    string GetSetting(string key, string defaultValue ="");
    bool GetBoolSetting(string key, bool defaultValue =false);
    string this[string key] { get; }
}