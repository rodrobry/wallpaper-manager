using WallpaperManager.Services;

namespace WallpaperManager;

class Program
{
    static int Main()
    {
        // C:\Users\<Username>\Pictures\wallpapers
        string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "wallpapers"
        );
        if (!Directory.Exists(baseFolder))
           return ExitWithError($"'wallpapers' folder does not exist: {baseFolder}");

        // Get all category subfolders -> wallpapers\<category>
        string[] categoryFolders = Directory.GetDirectories(baseFolder);
        if (categoryFolders.Length == 0)
           return ExitWithError("No subfolders inside the 'wallpapers' directory!");

        string categoryFolder = FolderSelector.SelectFolder(categoryFolders);

        // Define image orientation subpaths
        string horizontalPath = Path.Combine(categoryFolder, "horizontal");
        string verticalPath = Path.Combine(categoryFolder, "vertical");
        if (!Directory.Exists(horizontalPath) || !Directory.Exists(verticalPath))
           return ExitWithError(
                $"No 'horizontal' and/or 'vertical' subfolders in category '{Path.GetFileName(categoryFolder)}'.");

        // Fetch all category images
        List<string> horizontalImages = GetImages(horizontalPath);
        List<string> verticalImages = GetImages(verticalPath);
        if (horizontalImages.Count == 0 || verticalImages.Count == 0)
           return ExitWithError("One or both layout folders are empty!");

        // Select  3  random images (Horizontal, Horizontal, Vertical)
        string[] imagePaths = {
            PickAndRemoveRandom(horizontalImages),
            PickAndRemoveRandom(horizontalImages),
            PickAndRemoveRandom(verticalImages)
        };

        string newWallpaperPath = WallpaperCreator.MergeImages(imagePaths);
        WallpaperSwapper.ApplyNewWallpaper(newWallpaperPath);
        return 0;
    }

    // Helper to grab all images from a directory
    private static List<string> GetImages(string path) =>
        Directory.GetFiles(path, "*.*")
            .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToList();

    // Helper to pick a random image and remove it from the pool to avoid duplicates
    private static string PickAndRemoveRandom(List<string> list)
    {
        if (list.Count == 0) return string.Empty;
        int index = Random.Shared.Next(list.Count);
        string chosen = list[index];
        if (list.Count > 1) list.RemoveAt(index);
        return chosen;
    }

    private static int ExitWithError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey(intercept: true); // Wait for input without printing the key to the screen
        return 1;
    }
}
