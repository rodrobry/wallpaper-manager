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
            "wallpapers"
        );
        if (!Directory.Exists(baseFolder))
        {
            Console.WriteLine($"Wallpapers folder does not exist: {baseFolder}");
            return;
        }

        // Get all subfolders in Pictures\wallpapers
        string[] categoryFolders = Directory.GetDirectories(baseFolder);
        if (categoryFolders.Length == 0)
        {
            Console.WriteLine("No subfolders found inside your 'wallpapers' directory!");
            return;
        }

        // Display available categories
        Console.WriteLine("Select a wallpaper category:");
        for (int i = 0; i < categoryFolders.Length; i++)
        {
            Console.WriteLine($"  [{i + 1}] {Path.GetFileName(categoryFolders[i])}");
        }

        // Prompt for input (defaults to random if left blank or invalid)
        Console.Write("\nEnter name or number (or press Enter for random): ");
        string? input = Console.ReadLine();

        string categoryFolder;
        // Valid number
        if (int.TryParse(input, out int choice) &&
            choice >= 1 && choice <= categoryFolders.Length)
        {
            categoryFolder = categoryFolders[choice - 1];
        }
        // Valid name - assign to "matchedFolder" if LINQ doesn't return null (the default)
        else if (categoryFolders
            .FirstOrDefault(f => Path.GetFileName(f)
                .Equals(input, StringComparison.OrdinalIgnoreCase)) is string matchedFolder)
        {
            categoryFolder = matchedFolder;
        }
        // Invalid or blank input
        else
        {
            categoryFolder = categoryFolders[Random.Shared.Next(categoryFolders.Length)];
            Console.WriteLine($"Selecting random category: {Path.GetFileName(categoryFolder)}");
        }

        Console.WriteLine($"Loaded: {Path.GetFileName(categoryFolder)}");

        // Define subpaths
        string horizontalPath = Path.Combine(categoryFolder, "horizontal");
        string verticalPath = Path.Combine(categoryFolder, "vertical");
        if (!Directory.Exists(horizontalPath) || !Directory.Exists(verticalPath))
        {
            Console.WriteLine($"\nERROR: Category '{Path.GetFileName(categoryFolder)}' is invalid!");
            Console.WriteLine("Make sure both 'Horizontal' and 'Vertical' subfolders exist.");
            return;
        }

        // Fetch all category images
        List<string> horizontalImages = GetImages(horizontalPath);
        List<string> verticalImages = GetImages(verticalPath);
        if (horizontalImages.Count == 0 || verticalImages.Count == 0)
        {
            Console.WriteLine("\nERROR: One or both layout folders are empty!");
            return;
        }

        // Set paths to  3  random images (Horizontal, Horizontal, Vertical)
        string[] imagePaths = {
            PickAndRemoveRandom(horizontalImages),
            PickAndRemoveRandom(horizontalImages),
            PickAndRemoveRandom(verticalImages)
        };

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

        // Configure Registry for "Span" mode (Style 22)
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallpaperStyle", "22");
        // No tiling/repeat
        Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "TileWallpaper", "0");

        // Refresh desktop wallpaper (SPI_SETDESKWALLPAPER = 0x0014)
        SystemParametersInfo(0x0014, 0, tempPath, 0x01 | 0x02);
        Console.WriteLine("Wallpaper updated successfully!");
    }

    // Helper to grab all images from a directory
    static List<string> GetImages(string path) =>
        Directory.GetFiles(path, "*.*")
            .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToList();

    // Helper to pick a random image and remove it from the pool to avoid duplicates
    static string PickAndRemoveRandom(List<string> list)
    {
        if (list.Count == 0) return string.Empty;
        int index = Random.Shared.Next(list.Count);
        string chosen = list[index];
        if (list.Count > 1) list.RemoveAt(index);
        return chosen;
    }
}
