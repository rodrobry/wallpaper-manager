using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

class Program
{
    // Win32 API to change wallpaper
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


    static void Main()
    {
        // Dynamically gets C:\Users\<Username>\Pictures\wallpapers
        string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            @"wallpapers"
        );

        // Set paths to  3 images (Horizontal, Horizontal, Vertical)
        string[] imagePaths = {
            Path.Combine(baseFolder, @"gta6\Horizontal\Vice_City_Postcard_landscape.jpg"),
            Path.Combine(baseFolder, @"gta6\Horizontal\Official_Cover_Art_landscape.jpg"),
            Path.Combine(baseFolder, @"gta6\Vertical\Jason_and_Lucia_Robbery_With_Logo_portrait.jpg")
        };

        // Get exact total desktop bounds from Windows
        Rectangle virtualScreen = SystemInformation.VirtualScreen;
        using var canvas = new Bitmap(virtualScreen.Width, virtualScreen.Height);
        using var canvasGraphics = Graphics.FromImage(canvas);

        // Enable high-quality rendering modes
        canvasGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        canvasGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        canvasGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

        // Sort monitors left-to-right by physical X position
        var screens = Screen.AllScreens.OrderBy(s => s.Bounds.X).ToArray();

        // Draw each image at its exact OS-defined coordinates
        for (int i = 0; i < screens.Length && i < imagePaths.Length; i++)
        {
            if (!File.Exists(imagePaths[i]))
            {
                Console.WriteLine($"WARNING: File not found -> {imagePaths[i]}");
                continue;
            }

            // Normalize Windows screen coordinates to 0-based canvas coordinates
            int drawX = screens[i].Bounds.X - virtualScreen.X;
            int drawY = screens[i].Bounds.Y - virtualScreen.Y;
            int width = screens[i].Bounds.Width;
            int height = screens[i].Bounds.Height;

            using var img = Image.FromFile(imagePaths[i]);
            canvasGraphics.DrawImage(img, drawX, drawY, width, height);
        }

        // Save canvas to temp folder as PNG to prevent lossy re-compression
        string tempPath = Path.Combine(Path.GetTempPath(), "spanned_wallpaper.png");
        canvas.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);

        // Configure Registry for "Span" mode (Style 22)
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallpaperStyle", "22");
        // No tiling/repeat
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "TileWallpaper", "0");

        // Refresh desktop wallpaper (SPI_SETDESKWALLPAPER = 0x0014)
        SystemParametersInfo(0x0014, 0, tempPath, 0x01 | 0x02);
        Console.WriteLine("Wallpaper updated successfully!");
    }
}