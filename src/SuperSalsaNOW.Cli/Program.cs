using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperSalsaNOW.Cli;
using SuperSalsaNOW.Cli.Configuration;
using SuperSalsaNOW.Cli.Menu;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var appSettings = configuration.Get<AppSettings>() ?? new AppSettings();

// Build host with DI
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(context.Configuration.GetSection("Logging"));
            builder.AddConsole();
        });

        services.AddSuperSalsaNOWServices(appSettings);
    })
    .Build();

// Run application
var mainMenu = host.Services.GetRequiredService<MainMenu>();
await mainMenu.RunAsync();
