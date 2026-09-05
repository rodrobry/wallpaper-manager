using WallpaperManager.Utils;

namespace WallpaperManager.Services;

public static class FolderSelector
{
    public static void PromptForBaseFolder(AppConfig config)
    {
        while (!Directory.Exists(config.BaseFolder))
        {
            Console.Write("Please provide a valid base folder path: ");
            string? input = Console.ReadLine()?.Trim(' ', '"');
            if (input is null)
            {
                Console.WriteLine("\nOperation canceled.");
                Environment.Exit(0);
            }
            if (string.IsNullOrWhiteSpace(input))
            {
                CliUtils.ShowWarning("Path cannot be empty.");
                continue;
            }
            if (!Directory.Exists(input))
            {
                CliUtils.ShowWarning($"Directory '{input}' does not exist.");
                continue;
            }
            config.BaseFolder = input;
        }
        config.Save();
    }

    public static string PromptForCategoryFolder(string[] folderPaths)
    {
        // Display available categories
        Console.WriteLine("Available folders:");
        for (int i = 0; i < folderPaths.Length; i++)
        {
            Console.WriteLine($"  [{i + 1}] {Path.GetFileName(folderPaths[i])}");
        }

        string? selectedFolder = null;
        while (selectedFolder == null)
        {
            Console.Write("Enter a name or number (leave blank for random): ");
            string? input = Console.ReadLine()?.Trim();
            selectedFolder = ParseCategoryInput(input, folderPaths);
        }

        Console.WriteLine($"Selected: {Path.GetFileName(selectedFolder)}");
        return selectedFolder;
    }

    private static string? ParseCategoryInput(string? input, string[] folderPaths)
    {
        // Stream closed or aborted (like Ctrl+Z or Ctrl+C)
        if (input is null)
        {
            Console.WriteLine("\nOperation canceled.");
            Environment.Exit(0);
        }
        // Blank input -> Random selection
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine($"Selecting random folder");
            return folderPaths[Random.Shared.Next(folderPaths.Length)];
        }
        // Valid index number
        if (int.TryParse(input, out int choice) &&
            choice >= 1 && choice <= folderPaths.Length)
        {
            return folderPaths[choice - 1];
        }
        // Valid name - assign to "matchedFolder" if LINQ doesn't return null (the default)
        if (folderPaths
            .FirstOrDefault(f => Path.GetFileName(f)
                .Equals(input, StringComparison.OrdinalIgnoreCase)) is string matchedFolder)
        {
            return matchedFolder;
        }

        CliUtils.ShowWarning($"'{input}' is not a valid option.");
        return null;
    }
}