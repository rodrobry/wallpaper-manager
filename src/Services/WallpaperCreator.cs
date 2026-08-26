using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

public static class WallpaperCreator
{
    public static string MergeImages(string[] imagePaths)
    {
        // Get exact total desktop bounds from Windows
        Rectangle virtualScreen = SystemInformation.VirtualScreen;
        using var canvas = new Bitmap(virtualScreen.Width, virtualScreen.Height);
        using var canvasGraphics = Graphics.FromImage(canvas);

        // Enable high-quality rendering modes
        canvasGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        canvasGraphics.SmoothingMode = SmoothingMode.HighQuality;
        canvasGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        canvasGraphics.CompositingQuality = CompositingQuality.HighQuality;

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

        // User Temp folder -> C:\Users\<user>\AppData\Local\Temp
        string tempPath = Path.Combine(Path.GetTempPath(), "spanned_wallpaper.png");
        // Save as PNG to prevent lossy re-compression
        canvas.Save(tempPath, ImageFormat.Png);

        return tempPath;
    }
}