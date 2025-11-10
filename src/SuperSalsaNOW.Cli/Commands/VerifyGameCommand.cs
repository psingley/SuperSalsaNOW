namespace SuperSalsaNOW.Cli.Commands;

using Microsoft.Extensions.Logging;
using Spectre.Console;
using SuperSalsaNOW.Windows.Services;
using SuperSalsaNOW.Cli.Configuration;

public class VerifyGameCommand
{
    private readonly DepotDownloaderService _depotDownloader;
    private readonly PathSettings _paths;
    private readonly ILogger<VerifyGameCommand> _logger;

    public VerifyGameCommand(
        DepotDownloaderService depotDownloader,
        PathSettings paths,
        ILogger<VerifyGameCommand> logger)
    {
        _depotDownloader = depotDownloader;
        _paths = paths;
        _logger = logger;
    }

    public void Execute()
    {
        AnsiConsole.MarkupLine("[bold cyan]Verify Vanilla Installation[/]");
        AnsiConsole.WriteLine();

        var installed = _depotDownloader.VerifyEldenRingInstallation(_paths.FullGamePath);

        if (!installed)
        {
            AnsiConsole.MarkupLine($"[red]✗[/] Elden Ring not found at: {_paths.FullGamePath}");
            AnsiConsole.MarkupLine("[yellow]Please install the game first[/]");
            _logger.LogWarning("Elden Ring not found at {Path}", _paths.FullGamePath);
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]✓[/] Elden Ring found at: {_paths.FullGamePath}");
            _logger.LogInformation("Elden Ring verified at {Path}", _paths.FullGamePath);

            var launch = AnsiConsole.Confirm("Launch game to verify?", defaultValue: false);

            if (launch)
            {
                AnsiConsole.MarkupLine("[yellow]Launching Elden Ring...[/]");
                var process = _depotDownloader.LaunchEldenRing(_paths.FullGamePath);

                if (process != null)
                {
                    AnsiConsole.MarkupLine("[green]✓[/] Game launched");
                    AnsiConsole.MarkupLine("[dim]Please verify the game reaches main menu, then exit[/]");
                    _logger.LogInformation("Game launched for verification");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]✗[/] Failed to launch game");
                }
            }
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}
