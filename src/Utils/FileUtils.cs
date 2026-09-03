namespace WallpaperManager.Utils;

public static class FileUtils
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png"
    };

    public static bool IsSupportedImageFormat(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        return SupportedExtensions.Contains(extension);
    }
}