using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

public static class WallpaperCreator
{
    public static string CreateSpannedImage(string[] imagePaths)
    {
        // Get exact total desktop bounds from Windows
        Rectangle virtualScreen = SystemInformation.VirtualScreen;
        using var canvas = new Bitmap(virtualScreen.Width, virtualScreen.Height);
        using var canvasGraphics = Graphics.FromImage(canvas);

        // Enable high-quality rendering modes
        canvasGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        canvasGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        canvasGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        canvasGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

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

        // User Temp folder 
        string tempPath = Path.Combine(Path.GetTempPath(), "spanned_wallpaper.png");
        // Save as PNG to prevent lossy re-compression
        canvas.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);

        return tempPath;
    }
}