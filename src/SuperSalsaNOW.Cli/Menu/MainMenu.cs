namespace SuperSalsaNOW.Cli.Menu;

using Spectre.Console;
using SuperSalsaNOW.Cli.Commands;

public class MainMenu
{
    private readonly ConfigureNexusCommand _configureNexus;
    private readonly InstallEldenRingCommand _installGame;
    private readonly VerifyGameCommand _verifyGame;
    private readonly InstallModCommand _installMod;
    private readonly CreateShortcutCommand _createShortcut;

    public MainMenu(
        ConfigureNexusCommand configureNexus,
        InstallEldenRingCommand installGame,
        VerifyGameCommand verifyGame,
        InstallModCommand installMod,
        CreateShortcutCommand createShortcut)
    {
        _configureNexus = configureNexus;
        _installGame = installGame;
        _verifyGame = verifyGame;
        _installMod = installMod;
        _createShortcut = createShortcut;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            ShowHeader();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]Main Menu[/]")
                    .AddChoices(
                        "1. Install Elden Ring",
                        "2. Verify Vanilla Installation",
                        "3. Configure Nexus API Key",
                        "4. Install Elden Ring Reforged (ERR)",
                        "5. Create Desktop Shortcut",
                        "Q. Quit"
                    )
            );

            try
            {
                Console.Clear();

                switch (choice[0])
                {
                    case '1':
                        await _installGame.ExecuteAsync();
                        break;
                    case '2':
                        _verifyGame.Execute();
                        break;
                    case '3':
                        _configureNexus.Execute();
                        break;
                    case '4':
                        await _installMod.ExecuteAsync();
                        break;
                    case '5':
                        _createShortcut.Execute();
                        break;
                    case 'Q':
                    case 'q':
                        return;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }
    }

    private static void ShowHeader()
    {
        var rule = new Rule("[bold cyan]SuperSalsaNOW - Elden Ring Modding[/]");
        rule.Justification = Justify.Center;
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }
}
