using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        // C:\Users\<Username>\Pictures\wallpapers
        string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "wallpapers"
        );
        if (!Directory.Exists(baseFolder))
        {
            Console.WriteLine($"'wallpapers' folder does not exist: {baseFolder}");
            return;
        }

        // Get all category subfolders -> wallpapers\<category>
        string[] categoryFolders = Directory.GetDirectories(baseFolder);
        if (categoryFolders.Length == 0)
        {
            Console.WriteLine("No subfolders inside the 'wallpapers' directory!");
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

        // Define image orientation subpaths
        string horizontalPath = Path.Combine(categoryFolder, "horizontal");
        string verticalPath = Path.Combine(categoryFolder, "vertical");
        if (!Directory.Exists(horizontalPath) || !Directory.Exists(verticalPath))
        {
            Console.WriteLine($"No 'horizontal' and/or 'vertical' subfolders in category '{Path.GetFileName(categoryFolder)}'.");
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

        // Select  3  random images (Horizontal, Horizontal, Vertical)
        string[] imagePaths = {
            PickAndRemoveRandom(horizontalImages),
            PickAndRemoveRandom(horizontalImages),
            PickAndRemoveRandom(verticalImages)
        };

        string newWallpaperPath = WallpaperCreator.CreateSpannedImage(imagePaths);

        WallpaperSwapper.ApplyNewWallpaper(newWallpaperPath);

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
