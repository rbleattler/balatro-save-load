# Cross-Platform Implementation

This document details the cross-platform implementation strategy for the Balatro Save and Load Tool, covering how platform-specific code is organized and how the application behaves differently across Windows, macOS, and Linux.

## Platform Detection

The application uses runtime platform detection to determine the current operating system:

```csharp
public static class PlatformDetection
{
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public static OSPlatform CurrentPlatform
    {
        get
        {
            if (IsWindows) return OSPlatform.Windows;
            if (IsMacOS) return OSPlatform.OSX;
            if (IsLinux) return OSPlatform.Linux;
            throw new PlatformNotSupportedException("Current platform is not supported");
        }
    }
}
```

## Service Architecture

For platform-specific implementations, we use a common interface with platform-specific implementations:

```
IService (Interface)
  ├── WindowsServiceImpl
  ├── MacOSServiceImpl
  └── LinuxServiceImpl
```

The appropriate implementation is registered with the dependency injection container at runtime based on the detected platform.

## FileSystemService

The `FileSystemService` handles platform-specific file system operations:

### Save File Locations

- **Windows**: `%USERPROFILE%\Documents\Balatro\Saves\`
- **macOS**: `~/Library/Application Support/Balatro/Saves/`
- **Linux**: `~/.local/share/Balatro/Saves/`

### Implementation Strategy

```csharp
public interface IFileSystemService
{
    string GetSaveDirectory();
    IEnumerable<SaveFileInfo> GetSaveFiles();
    // ...other methods
}

public class WindowsFileSystemService : IFileSystemService
{
    public string GetSaveDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Balatro", "Saves");
    }
    // ...other methods
}

public class MacOSFileSystemService : IFileSystemService
{
    public string GetSaveDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Library", "Application Support", "Balatro", "Saves");
    }
    // ...other methods
}

public class LinuxFileSystemService : IFileSystemService
{
    public string GetSaveDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local", "share", "Balatro", "Saves");
    }
    // ...other methods
}
```

## GameProcessService

The `GameProcessService` handles platform-specific game process detection:

### Process Detection

- **Windows**: Uses `Process.GetProcessesByName("Balatro")`
- **macOS**: Uses `ps -A | grep Balatro` command execution
- **Linux**: Uses a combination of `ps` and `grep` commands

### Implementation

```csharp
public interface IGameProcessService
{
    bool IsGameRunning();
    void MonitorGameProcess();
    event EventHandler<GameProcessStatusChangedEventArgs> GameProcessStatusChanged;
    // ...other methods
}

public class WindowsGameProcessService : IGameProcessService
{
    public bool IsGameRunning()
    {
        var processes = Process.GetProcessesByName("Balatro");
        return processes.Length > 0;
    }
    // ...other methods
}

public class MacOSGameProcessService : IGameProcessService
{
    public bool IsGameRunning()
    {
        // Use process command to find Balatro on macOS
        var startInfo = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = "-c \"ps -A | grep -i Balatro\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        // ...process execution and result parsing
    }
    // ...other methods
}
```

## ThemeService

The `ThemeService` implements platform-specific theme detection for light/dark mode:

### System Theme Detection

- **Windows**: Uses Windows registry or UWP APIs
- **macOS**: Uses `defaults read -g AppleInterfaceStyle` command
- **Linux**: Depends on desktop environment (GNOME, KDE, etc.)

### Implementation

```csharp
public interface IThemeService
{
    Theme CurrentTheme { get; }
    void SetTheme(Theme theme);
    Theme GetSystemTheme();
    // ...other methods
}

public class WindowsThemeService : IThemeService
{
    public Theme GetSystemTheme()
    {
        try
        {
            // Windows 10 registry approach
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null)
            {
                var value = key.GetValue("AppsUseLightTheme");
                if (value != null && value is int lightThemeEnabled)
                {
                    return lightThemeEnabled == 1 ? Theme.Light : Theme.Dark;
                }
            }
            return Theme.Light; // Default
        }
        catch
        {
            return Theme.Light; // Default on error
        }
    }
    // ...other methods
}

public class MacOSThemeService : IThemeService
{
    public Theme GetSystemTheme()
    {
        try
        {
            // macOS command to check dark mode
            var startInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = "-c \"defaults read -g AppleInterfaceStyle 2>/dev/null\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd().Trim();

            // If "Dark" is returned, the system is in dark mode
            return string.Equals(output, "Dark", StringComparison.OrdinalIgnoreCase)
                ? Theme.Dark
                : Theme.Light;
        }
        catch
        {
            return Theme.Light; // Default on error
        }
    }
    // ...other methods
}
```

## UI Considerations

While Avalonia handles most platform-specific UI concerns, there are still platform-specific considerations:

1. **Window Chrome**: Window frame and controls adapt to platform conventions
2. **Keyboard Shortcuts**: Different modifier keys on macOS (Command vs. Control)
3. **Dialog Behavior**: File pickers and other dialogs use native dialogs
4. **Fonts and Rendering**: Font rendering differs across platforms

## Platform-Specific Configuration

The application applies platform-specific configurations at startup:

```csharp
public static class AppBootstrapper
{
    public static void RegisterPlatformServices(IServiceCollection services)
    {
        if (PlatformDetection.IsWindows)
        {
            services.AddSingleton<IFileSystemService, WindowsFileSystemService>();
            services.AddSingleton<IGameProcessService, WindowsGameProcessService>();
            services.AddSingleton<IThemeService, WindowsThemeService>();
        }
        else if (PlatformDetection.IsMacOS)
        {
            services.AddSingleton<IFileSystemService, MacOSFileSystemService>();
            services.AddSingleton<IGameProcessService, MacOSGameProcessService>();
            services.AddSingleton<IThemeService, MacOSThemeService>();
        }
        else if (PlatformDetection.IsLinux)
        {
            services.AddSingleton<IFileSystemService, LinuxFileSystemService>();
            services.AddSingleton<IGameProcessService, LinuxGameProcessService>();
            services.AddSingleton<IThemeService, LinuxThemeService>();
        }
        else
        {
            throw new PlatformNotSupportedException("Current platform is not supported");
        }
    }
}
```

## Packaging and Distribution

The application will be packaged differently for each platform:

- **Windows**: MSI installer or MSIX package
- **macOS**: App bundle in DMG container
- **Linux**: AppImage, DEB, and RPM packages

## Testing Considerations

Each platform-specific implementation must be tested on its target platform to ensure correct behavior. The testing strategy includes:

1. Unit tests for platform-agnostic code
2. Platform-specific unit tests using mocking
3. Manual integration testing on each target platform
