using System.Text.Json;
using WallpaperManager.Utils;

namespace WallpaperManager.Services;

public class AppConfig
{
    public string BaseFolder { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "wallpapers").Replace('\\', '/'); // C:/Users/<Username>/Pictures/wallpapers

    public static AppConfig LoadOrCreate()
    {
        string configFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.json");
        
        if (File.Exists(configFilePath))
        {
            try
            {
                string loadedJsonStr = File.ReadAllText(configFilePath);
                AppConfig? loadedConfig = JsonSerializer.Deserialize<AppConfig>(loadedJsonStr);
                if (loadedConfig != null)
                    return loadedConfig;
            }
            catch (JsonException)
            {
                CliUtils.ShowWarning("config.json was corrupted. Resetting to defaults.");
            }
        }
        
        AppConfig defaultConfig = new AppConfig();
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        string defaultJsonStr = JsonSerializer.Serialize(defaultConfig, serializerOptions);
        File.WriteAllText(configFilePath, defaultJsonStr);
        return defaultConfig;
    }
}