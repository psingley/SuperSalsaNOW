namespace SuperSalsaNOW.Cli.Configuration;

public class AppSettings
{
    public ManifestSettings Manifest { get; set; } = new();
    public NexusSettings Nexus { get; set; } = new();
    public PathSettings Paths { get; set; } = new();
}

public class ManifestSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class NexusSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultApiKey { get; set; } = string.Empty;
}

public class PathSettings
{
    public string InstallRoot { get; set; } = string.Empty;
    public string GameDirectory { get; set; } = string.Empty;
    public string ModsDirectory { get; set; } = string.Empty;
    public string ToolsDirectory { get; set; } = string.Empty;

    public string FullGamePath => Path.Combine(InstallRoot, GameDirectory);
    public string FullModsPath => Path.Combine(InstallRoot, ModsDirectory);
    public string FullToolsPath => Path.Combine(InstallRoot, ToolsDirectory);
}
