public static class FolderSelector {
    public static string SelectFolder(string[] folderPaths)
    {
        // Display available categories
        Console.WriteLine("Select a wallpaper category:");
        for (int i = 0; i < folderPaths.Length; i++)
        {
            Console.WriteLine($"  [{i + 1}] {Path.GetFileName(folderPaths[i])}");
        }

        // Prompt for input (defaults to random if left blank or invalid)
        Console.Write("\nEnter name or number (or press Enter for random): ");
        string? input = Console.ReadLine();

        string selectedFolder;
        // Valid number
        if (int.TryParse(input, out int choice) &&
            choice >= 1 && choice <= folderPaths.Length)
        {
            selectedFolder = folderPaths[choice - 1];
        }
        // Valid name - assign to "matchedFolder" if LINQ doesn't return null (the default)
        else if (folderPaths
            .FirstOrDefault(f => Path.GetFileName(f)
                .Equals(input, StringComparison.OrdinalIgnoreCase)) is string matchedFolder)
        {
            selectedFolder = matchedFolder;
        }
        // Invalid or blank input
        else
        {
            selectedFolder = folderPaths[Random.Shared.Next(folderPaths.Length)];
            Console.WriteLine($"Selecting random category: {Path.GetFileName(selectedFolder)}");
        }

        Console.WriteLine($"Selected: {Path.GetFileName(selectedFolder)}");
        return selectedFolder;
    }
}