using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace WallpaperManager.Services;

public static class WallpaperSwapper
{
    // Win32 API Params
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;
    // Registry Config Values
    private const string SPAN = "22";
    private const string NO_TILING = "0";

    // Import Win32 API method to change wallpaper
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public static void ApplyNewWallpaper(string imagePath)
    {
        // Configure Registry
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallpaperStyle", SPAN);
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "TileWallpaper", NO_TILING);

        // Refresh desktop wallpaper
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        Console.WriteLine("Wallpaper updated successfully!");
    }
}