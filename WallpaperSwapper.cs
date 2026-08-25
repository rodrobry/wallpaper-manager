using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

public static class WallpaperSwapper
{
    // Import Win32 API to change wallpaper
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public static void ApplyNewWallpaper(string imagePath)
    {
        // Configure Registry for "Span" mode (Style 22 (span), Tile 0 (no tiling/repeating))
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallpaperStyle", "22");
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "TileWallpaper", "0");

        // Refresh desktop wallpaper (SPI_SETDESKWALLPAPER = 0x0014)
        SystemParametersInfo(0x0014, 0, imagePath, 0x01 | 0x02);
        Console.WriteLine("Wallpaper updated successfully!");
    }
}