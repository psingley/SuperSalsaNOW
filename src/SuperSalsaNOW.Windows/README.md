# SuperSalsaNOW.Windows

Windows-specific implementations for SuperSalsaNOW functionality.

## 🎯 Purpose

This project contains Windows-only code that enables:
- Desktop shortcut creation (.lnk files)
- Steam game installation without Steam client (via DepotDownloader)
- Windows API interop (window management)

## 📁 Project Structure

```
SuperSalsaNOW.Windows/
├── Services/
│   ├── WindowsShortcutService.cs    # Create desktop shortcuts via COM
│   └── DepotDownloaderService.cs    # Install Steam games
├── Interop/
│   └── WindowsAPI.cs                # P/Invoke wrappers for Win32 API
├── SuperSalsaNOW.Windows.csproj     # Project configuration
├── WINDOWS_TESTING_REQUIRED.md      # Testing requirements
├── IMPLEMENTATION_SUMMARY.md        # Detailed implementation docs
└── README.md                        # This file
```

## 🔧 Technical Details

### Platform Support
- **Compiles on**: macOS, Linux, Windows
- **Runs on**: Windows only (throws `PlatformNotSupportedException` elsewhere)

### Key Technologies
- **COM Interop**: Dynamic invocation for WScript.Shell (shortcuts)
- **P/Invoke**: Win32 API calls (user32.dll)
- **Process Management**: DepotDownloader execution
- **NuGet**: Microsoft.Windows.Compatibility (8.0.0)

### Build Strategy
Uses **dynamic COM invocation** to enable cross-platform compilation:
- ✅ Builds on any platform
- ✅ No Windows build agent required for CI/CD
- ⚠️ Runtime platform checks ensure Windows-only execution

## 🚀 Usage Examples

### Creating a Desktop Shortcut
```csharp
var logger = serviceProvider.GetRequiredService<ILogger<WindowsShortcutService>>();
var shortcutService = new WindowsShortcutService(logger);

shortcutService.CreateShortcut(
    targetPath: @"C:\Games\ELDENRING\Game\eldenring.exe",
    shortcutPath: Path.Combine(WindowsShortcutService.GetDesktopPath(), "Elden Ring.lnk"),
    workingDirectory: @"C:\Games\ELDENRING\Game",
    arguments: "-offline"
);
```

### Installing Elden Ring
```csharp
var logger = serviceProvider.GetRequiredService<ILogger<DepotDownloaderService>>();
var service = new DepotDownloaderService(@"C:\Tools", logger);

var progress = new Progress<string>(msg => Console.WriteLine(msg));

var success = await service.InstallEldenRingAsync(
    username: "steamuser",
    password: "steampass",
    installDirectory: @"I:\Games\ELDENRING",
    progress: progress
);
```

### Window Management
```csharp
using SuperSalsaNOW.Windows.Interop;

// Check if a window is open
if (WindowsAPI.IsWindowOpen("ELDEN RING™"))
{
    Console.WriteLine("Game is running");
}

// Close a window
WindowsAPI.CloseWindow("ELDEN RING™");
```

## 📋 Dependencies

### NuGet Packages
- `Microsoft.Windows.Compatibility` (8.0.0)
- `Microsoft.Extensions.Logging.Abstractions` (inherited from Core)

### External Tools
- **DepotDownloader**: https://github.com/SteamRE/DepotDownloader
  - Required for Steam game installation
  - Must be downloaded manually (auto-download TODO)
  - Place in: `{ToolsDirectory}/DepotDownloader/DepotDownloader.exe`

## ✅ Build Status

| Platform | Build | Run |
|----------|-------|-----|
| macOS    | ✅    | ❌  |
| Linux    | ✅    | ❌  |
| Windows  | ✅    | ✅  |

**Current Build Output (macOS)**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## 🧪 Testing

### On macOS (Development)
```bash
# Build succeeds
dotnet build src/SuperSalsaNOW.Windows

# Unit tests for business logic only
# Integration tests will throw PlatformNotSupportedException
```

### On Windows (Production)
See [WINDOWS_TESTING_REQUIRED.md](WINDOWS_TESTING_REQUIRED.md) for:
- Feature testing checklist
- Expected behavior
- Known limitations

## 🛠️ Development Notes

### COM Reference
The .csproj contains a **commented-out** COMReference for better IntelliSense:
```xml
<!-- Uncomment on Windows build agents for type safety -->
<!--
<ItemGroup>
  <COMReference Include="IWshRuntimeLibrary">
    ...
  </COMReference>
</ItemGroup>
-->
```

**When to uncomment**:
- Windows development machine
- Windows build agents
- Want compile-time type checking

**Keep commented when**:
- Building on macOS/Linux
- CI/CD runs on non-Windows
- Cross-platform compatibility needed

### Error Handling
All methods include:
- Platform checks (`OperatingSystem.IsWindows()`)
- Comprehensive logging
- Clear exception messages
- Graceful degradation

## 📝 TODO

### High Priority
- [ ] Implement DepotDownloader auto-download from GitHub
- [ ] Add Steam Guard 2FA handling
- [ ] Create unit tests for business logic

### Medium Priority
- [ ] Support additional Steam games (beyond Elden Ring)
- [ ] Add shortcut icon customization
- [ ] Implement DepotDownloader update checking

### Low Priority
- [ ] Retry logic for network failures
- [ ] Detailed installation progress (percentage)
- [ ] Checksum verification for DepotDownloader

## 🔐 Security

### Credentials
- Steam credentials passed directly to DepotDownloader
- Consider environment variables for sensitive data
- Never commit credentials to source control

### Process Execution
- DepotDownloader runs with user privileges
- Validate tool authenticity before use
- Consider implementing checksum verification

## 📖 Additional Documentation

- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)**: Detailed technical implementation
- **[WINDOWS_TESTING_REQUIRED.md](WINDOWS_TESTING_REQUIRED.md)**: Testing requirements
- **Core Project**: `../SuperSalsaNOW.Core/` for shared interfaces

## 🤝 Contributing

When adding Windows-specific features:
1. Implement interface from `SuperSalsaNOW.Core`
2. Add platform checks (`OperatingSystem.IsWindows()`)
3. Use dynamic COM or P/Invoke as needed
4. Ensure builds on macOS
5. Document Windows testing requirements
6. Add comprehensive logging

---

**Platform**: Windows 10/11  
**Framework**: .NET 8.0  
**Language**: C# 12  
**Build Status**: ✅ Compiles on macOS
