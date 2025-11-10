namespace SuperSalsaNOW.Cli.Commands;

using Microsoft.Extensions.Logging;
using Spectre.Console;
using SuperSalsaNOW.Windows.Services;
using SuperSalsaNOW.Cli.Configuration;

public class InstallEldenRingCommand
{
    private readonly DepotDownloaderService _depotDownloader;
    private readonly PathSettings _paths;
    private readonly ILogger<InstallEldenRingCommand> _logger;

    public InstallEldenRingCommand(
        DepotDownloaderService depotDownloader,
        PathSettings paths,
        ILogger<InstallEldenRingCommand> logger)
    {
        _depotDownloader = depotDownloader;
        _paths = paths;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        AnsiConsole.MarkupLine("[bold cyan]Install Elden Ring[/]");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose installation method:")
                .AddChoices(
                    "Via Steam (recommended)",
                    "Via DepotDownloader (automated)",
                    "Skip (already installed)"
                )
        );

        if (choice.Contains("Skip"))
        {
            AnsiConsole.MarkupLine("[yellow]Skipping installation[/]");
            return;
        }

        if (choice.Contains("Steam"))
        {
            await InstallViaSteamAsync();
        }
        else
        {
            await InstallViaDepotDownloaderAsync();
        }
    }

    private Task InstallViaSteamAsync()
    {
        AnsiConsole.MarkupLine("[yellow]Manual Installation via Steam:[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("1. Open Steam client");
        AnsiConsole.MarkupLine("2. Go to Library");
        AnsiConsole.MarkupLine("3. Find 'Elden Ring'");
        AnsiConsole.MarkupLine($"4. Install to: [cyan]{_paths.FullGamePath}[/]");
        AnsiConsole.MarkupLine("5. Wait for installation to complete");
        AnsiConsole.MarkupLine("6. Exit Steam");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[dim]Press any key when installation is complete...[/]");
        Console.ReadKey(true);

        _logger.LogInformation("User installed Elden Ring via Steam");
        return Task.CompletedTask;
    }

    private async Task InstallViaDepotDownloaderAsync()
    {
        AnsiConsole.MarkupLine("[yellow]Automated Installation via DepotDownloader[/]");
        AnsiConsole.WriteLine();

        var username = AnsiConsole.Ask<string>("Steam [cyan]username[/]:");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Steam [cyan]password[/]:")
                .Secret()
        );

        var progress = new Progress<string>(msg => AnsiConsole.MarkupLine($"[dim]{msg}[/]"));

        await AnsiConsole.Status()
            .StartAsync("Installing Elden Ring...", async ctx =>
            {
                try
                {
                    var success = await _depotDownloader.InstallEldenRingAsync(
                        username,
                        password,
                        _paths.FullGamePath,
                        progress
                    );

                    if (success)
                    {
                        AnsiConsole.MarkupLine("[green]✓[/] Elden Ring installed successfully");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]✗[/] Installation failed");
                    }
                }
                catch (NotImplementedException)
                {
                    AnsiConsole.MarkupLine("[red]✗[/] DepotDownloader auto-download not yet implemented");
                    AnsiConsole.MarkupLine("[yellow]Please download manually from:[/]");
                    AnsiConsole.MarkupLine("https://github.com/SteamRE/DepotDownloader/releases");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Installation failed");
                    AnsiConsole.MarkupLine($"[red]✗[/] {ex.Message}");
                }
            });

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}
