using System;
using System.Configuration;

namespace Films_DB.Model;

public static class ConfigManager
{
    public static string ReadSetting(string key)
    {
        try
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? "Not Found";
        }
        catch (ConfigurationErrorsException)
        {
            Console.WriteLine("Error reading app settings");
            return "Not found";
        }
    }
}