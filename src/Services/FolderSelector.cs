public static class FolderSelector
{
    public static string SelectFolder(string[] folderPaths)
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
            selectedFolder = PromptUser(folderPaths);
        }

        Console.WriteLine($"Selected: {Path.GetFileName(selectedFolder)}");
        return selectedFolder;
    }

    private static string? PromptUser(string[] folderPaths)
    {
        Console.Write("Enter a name or number (leave blank for random): ");
        string? input = Console.ReadLine()?.Trim();

        // Blank input -> Random selection
        if (string.IsNullOrEmpty(input))
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

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"'{input}' is not a valid option.");
        Console.ResetColor();

        return null;
    }
}