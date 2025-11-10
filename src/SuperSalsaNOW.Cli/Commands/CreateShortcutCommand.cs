namespace SuperSalsaNOW.Cli.Commands;

using Microsoft.Extensions.Logging;
using Spectre.Console;
using SuperSalsaNOW.Core.Interfaces;
using SuperSalsaNOW.Windows.Services;
using SuperSalsaNOW.Cli.Configuration;

public class CreateShortcutCommand
{
    private readonly IShortcutService _shortcutService;
    private readonly PathSettings _paths;
    private readonly ILogger<CreateShortcutCommand> _logger;

    public CreateShortcutCommand(
        IShortcutService shortcutService,
        PathSettings paths,
        ILogger<CreateShortcutCommand> logger)
    {
        _shortcutService = shortcutService;
        _paths = paths;
        _logger = logger;
    }

    public void Execute()
    {
        AnsiConsole.MarkupLine("[bold cyan]Create Desktop Shortcut[/]");
        AnsiConsole.WriteLine();

        // For ERR, the launcher is typically in the mod directory
        var errModPath = Path.Combine(_paths.FullModsPath, "elden-ring-reforged");

        // Look for launcher executable
        var launcherCandidates = new[]
        {
            Path.Combine(errModPath, "Launch ELDEN RING Reforged.bat"),
            Path.Combine(errModPath, "!! Launch ELDEN RING Reforged.BAT"),
            Path.Combine(errModPath, "launch.bat")
        };

        var launcher = launcherCandidates.FirstOrDefault(File.Exists);

        if (launcher == null)
        {
            AnsiConsole.MarkupLine("[red]✗[/] ERR launcher not found");
            AnsiConsole.MarkupLine($"Searched in: {errModPath}");
            _logger.LogWarning("ERR launcher not found at {Path}", errModPath);
        }
        else
        {
            try
            {
                var desktopPath = WindowsShortcutService.GetDesktopPath();
                var shortcutPath = Path.Combine(desktopPath, "Elden Ring Reforged.lnk");

                _shortcutService.CreateShortcut(
                    targetPath: launcher,
                    shortcutPath: shortcutPath,
                    workingDirectory: Path.GetDirectoryName(launcher)
                );

                AnsiConsole.MarkupLine($"[green]✓[/] Shortcut created: {shortcutPath}");
                _logger.LogInformation("Shortcut created at {Path}", shortcutPath);
            }
            catch (PlatformNotSupportedException)
            {
                AnsiConsole.MarkupLine("[red]✗[/] Shortcut creation requires Windows");
                _logger.LogWarning("Attempted to create shortcut on non-Windows platform");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗[/] Failed to create shortcut: {ex.Message}");
                _logger.LogError(ex, "Failed to create shortcut");
            }
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}
