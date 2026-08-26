namespace WallpaperManager.Utils;

public static class CliUtils
{
    public static void ShowWarning(string message)
    {
        ShowMessage($"WARNING: {message}", ConsoleColor.Yellow);
    }

    public static void ShowError(string message)
    {
        ShowMessage($"ERROR: {message}", ConsoleColor.Red);
    }

    private static void ShowMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}