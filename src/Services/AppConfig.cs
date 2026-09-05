using System.Text.Json;
using WallpaperManager.Utils;

namespace WallpaperManager.Services;

public class AppConfig
{
    private static string ConfigFilePath { get; } = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.json");

    private string _baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "wallpapers").Replace('\\', '/'); // C:/Users/<Username>/Pictures/wallpapers

    public string BaseFolder
    {
        get => _baseFolder;
        set => _baseFolder = value?.Replace('\\', '/') ?? string.Empty;
    }

    public static AppConfig LoadOrCreate()
    {
        if (File.Exists(ConfigFilePath))
        {
            try
            {
                string loadedJsonStr = File.ReadAllText(ConfigFilePath);
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
        defaultConfig.Save();
        return defaultConfig;
    }

    public void Save()
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(this, serializerOptions);
        File.WriteAllText(ConfigFilePath, jsonString);
    }
}