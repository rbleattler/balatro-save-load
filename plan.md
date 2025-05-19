# Balatro Save and Load Tool - Migration Plan from WPF to Avalonia

## Overview

This document outlines a comprehensive plan to migrate the Balatro Save and Load Tool from WPF to Avalonia. The migration will target .NET 9 and focus on creating a modern, cross-platform application with clean architecture while avoiding platform-specific code wherever possible.

## Current Architecture Analysis

The current application is a WPF-based tool with the following characteristics:

1. **Monolithic Design**: Most functionality is in the MainWindow class (over 800 lines)
2. **Custom UI Elements**: Including a custom title bar implementation
3. **Direct File System Access**: No abstraction layer for file operations
4. **Platform-Specific Code**: Windows Registry access for theme detection and process management
5. **Global State**: State management through direct properties
6. **Manual Data Binding**: Implements INotifyPropertyChanged but without a formal MVVM pattern
7. **Windows-Only Features**: Window flashing, Explorer integration, etc.
8. **Multiple Timer Usage**: Several timer instances for different functional aspects

### Current Dependencies

- .NET 8.0 Windows-specific target framework
- Built-in WPF controls and styling
- No external NuGet packages

### Key Functionality to Preserve

1. **Save File Management**:
   - Detecting and accessing Balatro save files
   - Creating backups with meaningful names
   - Loading save files back into the game

2. **Auto-Save Features**:
   - Timed auto-saving when Balatro is running
   - Countdown display for next auto-save

3. **Game Process Detection**:
   - Detecting if Balatro is running
   - Disabling save functionality when the game is not running

4. **Save Analysis**:
   - Viewing save file content in a structured format
   - Search and navigation functionality

5. **File Management**:
   - Listing save backups
   - Deleting old saves
   - Auto-cleaning based on age
   - Opening the save directory

6. **Theming**:
   - Light and dark theme support
   - Following system theme preference

## Migration Strategy

### 1. Project Structure Changes

The new solution will adopt a modular architecture:

```
V2/
  BalatroSaveToolkit.sln
  BalatroSaveToolkit/                 # Core project (UI + application logic)
    App.axaml
    App.axaml.cs
    Program.cs                        # Entry point
    MainWindow.axaml
    MainWindow.axaml.cs
    ViewModels/                       # MVVM pattern view models
      MainViewModel.cs
      SaveFilesViewModel.cs
      SaveViewerViewModel.cs
      SettingsViewModel.cs
    Views/                            # MVVM pattern views
      MainView.axaml
      MainView.axaml.cs
      SaveFilesView.axaml
      SaveFilesView.axaml.cs
      SaveViewerView.axaml
      SaveViewerView.axaml.cs
      SettingsView.axaml
      SettingsView.axaml.cs
    Models/                           # Domain models
      SaveFile.cs
      AppSettings.cs
    Services/                         # Service interfaces and implementations
      Interfaces/
        IFileSystemService.cs
        IGameProcessService.cs
        ISettingsService.cs
        IThemeService.cs
      Implementations/
        FileSystemService.cs
        GameProcessService.cs
        SettingsService.cs
        ThemeService.cs
      Platform/                       # Platform-specific implementations
        Windows/
        Linux/
        MacOS/
    Styles/                           # Themes and styling
      LightTheme.axaml
      DarkTheme.axaml
  BalatroSaveToolkit.Core/            # Platform-agnostic business logic
    Services/
      Interfaces/
      Implementations/
    Models/
  BalatroSaveToolkit.Tests/           # Unit tests
```

### 2. Architectural Changes

1. **MVVM Pattern** - Implement proper MVVM architecture:
   - Replace direct event handlers with commands
   - Move logic from code-behind to view models
   - Implement proper data binding

2. **Dependency Injection** - Add service registration:

   ```csharp
   // Program.cs
   public static class Program
   {
       public static void Main(string[] args)
       {
           var builder = AppBuilder.Configure<App>()
               .UsePlatformDetect()
               .LogToTrace();

           builder.ConfigureServices((context, services) =>
           {
               // Register services with DI
               services.AddSingleton<IFileSystemService, FileSystemService>();
               services.AddSingleton<IGameProcessService, GameProcessService>();
               services.AddSingleton<ISettingsService, SettingsService>();
               services.AddSingleton<IThemeService, ThemeService>();

               // Register ViewModels
               services.AddSingleton<MainViewModel>();
               services.AddSingleton<SaveFilesViewModel>();
               services.AddSingleton<SaveViewerViewModel>();
               services.AddSingleton<SettingsViewModel>();
           });

           builder.Build().Run();
       }
   }
   ```

3. **Platform Abstraction** - Create platform-agnostic interfaces:

   ```csharp
   public interface IFileSystemService
   {
       Task<string> GetBalatroSaveDirectoryAsync();
       Task<IEnumerable<SaveFileInfo>> GetSaveBackupsAsync();
       Task BackupCurrentSaveAsync(int profileNumber);
       Task RestoreSaveAsync(string backupPath, int profileNumber);
       Task DeleteBackupsAsync(IEnumerable<string> backupPaths);
       Task<bool> OpenFolderAsync(string path);
   }

   public interface IGameProcessService
   {
       bool IsGameRunning { get; }
       event EventHandler<bool> GameProcessStateChanged;
       void StartMonitoring();
       void StopMonitoring();
   }
   ```

4. **Service-Based Components** - Replace direct functionality with services:
   - Replace manual file operations with `IFileSystemService`
   - Replace process checking with `IGameProcessService`
   - Replace registry theme detection with `IThemeService`
   - Replace in-memory settings with `ISettingsService`

5. **Cross-Platform UI** - Replace WPF-specific UI:
   - Replace custom title bar with Avalonia window chrome
   - Replace WPF-specific controls with Avalonia equivalents
   - Use ReactiveUI for MVVM implementation with ObservableAsPropertyHelper

### 3. Technology Stack

1. **Core Framework**:
   - Avalonia UI 11.x (latest stable)
   - .NET 9.0 (multi-platform target)

2. **Key Packages**:
   - Avalonia.Desktop
   - Avalonia.Themes.Fluent
   - ReactiveUI
   - Microsoft.Extensions.DependencyInjection
   - System.Text.Json (for settings persistence)
   - System.IO.Compression (for save file handling)
   - Avalonia.Diagnostics (for development)
   - xUnit (for testing)

### 4. Implementation Phases

#### Phase 1: Setup and Core Infrastructure

1. Create solution and project structure
2. Configure .NET 9.0 target
3. Set up Avalonia UI framework
4. Implement dependency injection
5. Create core service interfaces

#### Phase 2: Platform-Agnostic Services

1. Implement `FileSystemService` with platform detection
2. Implement `SettingsService` with JSON persistence
3. Implement `ThemeService` with platform-specific light/dark detection
4. Implement `GameProcessService` with cross-platform game detection

#### Phase 3: Core UI and Functionality

1. Create main window and navigation
2. Implement save files listing and management
3. Implement save/load functionality
4. Implement auto-save mechanism
5. Add save file viewer

#### Phase 4: Polish and Testing

1. Implement theming and styling
2. Add platform-specific optimizations
3. Write unit tests
4. Performance optimization
5. UX improvements

## Complexity Reduction Opportunities

### 1. Replace Custom Title Bar

The current implementation uses a custom title bar with manual window dragging:

```csharp
// Current implementation
private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
    if (e.ClickCount == 2) {
        // Optional: maximize/restore on double-click
    } else { DragMove(); }
}
```

Replace with Avalonia's built-in window chrome:

```xaml
<!-- Avalonia implementation -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Balatro Save and Load"
        ExtendClientAreaToDecorationsHint="True"
        ExtendClientAreaChromeHints="SystemChrome"
        ExtendClientAreaTitleBarHeightHint="-1">
    <Panel>
        <ExperimentalAcrylicBorder IsHitTestVisible="False" />
        <DockPanel>
            <Panel Name="TitleBarHost" DockPanel.Dock="Top" Height="30" />
            <ContentControl Content="{Binding CurrentPage}" />
        </DockPanel>
    </Panel>
</Window>
```

### 2. Replace Multiple Timers with ReactiveUI

The current implementation uses several dispatcher timers:

```csharp
// Current implementation
private readonly DispatcherTimer _timer = new DispatcherTimer();
private readonly DispatcherTimer _statusResetTimer = new DispatcherTimer();
private readonly DispatcherTimer _errorCheckTimer = new DispatcherTimer();
private readonly DispatcherTimer _countdownTimer = new DispatcherTimer();
private readonly DispatcherTimer _cleanupTimer = new DispatcherTimer();
private readonly DispatcherTimer _balatroCheckTimer = new DispatcherTimer();
```

Replace with ReactiveUI observables:

```csharp
// Avalonia implementation with ReactiveUI
public class MainViewModel : ReactiveObject
{
    private readonly IGameProcessService _gameProcessService;
    private readonly IFileSystemService _fileSystemService;

    // Auto-save timer
    private readonly ObservableAsPropertyHelper<TimeSpan> _timeUntilNextSave;
    public TimeSpan TimeUntilNextSave => _timeUntilNextSave.Value;

    public MainViewModel(IGameProcessService gameService, IFileSystemService fileService)
    {
        _gameProcessService = gameService;
        _fileSystemService = fileService;

        // Set up timer using observable
        _timeUntilNextSave = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => CalculateTimeUntilNextSave())
            .ToProperty(this, x => x.TimeUntilNextSave);

        // Game state changes
        this.WhenAnyValue(x => x._gameProcessService.IsGameRunning)
            .Subscribe(isRunning => {
                IsGameRunning = isRunning;
                if (!isRunning) StopAutoSave();
            });
    }
}
```

### 3. Simplify Theme Management

The current implementation polls Windows registry for theme changes:

```csharp
// Current implementation
private void ApplySystemTheme() {
    var isDark = IsSystemInDarkMode();
    if (_lastIsDark == isDark) return;
    _lastIsDark = isDark;
    var dicts = Resources.MergedDictionaries;
    dicts.Clear();
    dicts.Add(
              isDark
                  ? new ResourceDictionary { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) }
                  : new ResourceDictionary { Source = new Uri("Themes/Light.xaml", UriKind.Relative) }
             );
}

private static bool IsSystemInDarkMode() {
    try {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var value = key?.GetValue("AppsUseLightTheme");
        if (value is int i) return i == 0;
    }
    catch {
        // Handle exceptions if necessary
    }
    return false; // Default to light
}
```

Replace with Avalonia's cross-platform theme service:

```csharp
// Avalonia implementation
public class ThemeService : IThemeService
{
    public ReactiveProperty<ThemeVariant> CurrentTheme { get; } = new(ThemeVariant.Light);

    public ThemeService()
    {
        // Set initial theme based on platform
        CurrentTheme.Value = GetSystemTheme();

        // Set up monitoring of system theme changes (platform-specific)
        StartThemeMonitoring();
    }

    private ThemeVariant GetSystemTheme()
    {
        if (OperatingSystem.IsWindows())
            return GetWindowsTheme();
        else if (OperatingSystem.IsMacOS())
            return GetMacOSTheme();
        else if (OperatingSystem.IsLinux())
            return GetLinuxTheme();

        return ThemeVariant.Light;
    }

    // Platform-specific implementations
}

// In App.axaml.cs
public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var themeService = this.GetRequiredService<IThemeService>();

            // React to theme changes
            themeService.CurrentTheme
                .Subscribe(theme => {
                    Current!.RequestedThemeVariant = theme == ThemeVariant.Dark ?
                        ThemeVariant.Dark : ThemeVariant.Light;
                });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

### 4. Replace File System Platform-Specific Code

The current implementation uses Windows-specific paths and Explorer:

```csharp
// Current implementation
string GetCurrentSaveFile() {
    var profileNumber = ProfileComboBox.SelectedIndex + 1;
    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Balatro",
                        profileNumber.ToString(),
                        "save.jkr"
                       );
}

private void OpenSavesFolder_Click(object sender, RoutedEventArgs e) {
    Process.Start("explorer.exe", _directoryPath);
}
```

Replace with platform-agnostic code:

```csharp
// Avalonia implementation
public class FileSystemService : IFileSystemService
{
    public async Task<string> GetBalatroSaveDirectoryAsync()
    {
        if (OperatingSystem.IsWindows())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Balatro");
        else if (OperatingSystem.IsMacOS())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Library", "Application Support", "Balatro");
        else if (OperatingSystem.IsLinux())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                ".local", "share", "Balatro");

        throw new PlatformNotSupportedException("Current platform is not supported");
    }

    public async Task<bool> OpenFolderAsync(string path)
    {
        try
        {
            if (OperatingSystem.IsWindows())
                Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
            else if (OperatingSystem.IsMacOS())
                Process.Start(new ProcessStartInfo("open", path) { UseShellExecute = true });
            else if (OperatingSystem.IsLinux())
                Process.Start(new ProcessStartInfo("xdg-open", path) { UseShellExecute = true });
            else
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

### 5. Replace Status Management with Message Bus

The current implementation uses direct status updates with timers:

```csharp
// Current implementation
private void SetSuccessStatus(string message) {
    _hasActiveError = false;
    _currentErrorMessage = string.Empty;
    _errorConditionChecker = null;
    Status = message;
    UpdateStatusDisplay(false);
    _statusResetTimer.Stop();
    _statusResetTimer.Start();
}
```

Replace with a reactive message bus:

```csharp
// Avalonia implementation
public record StatusMessage(string Text, StatusType Type, TimeSpan Duration);

public enum StatusType { Info, Success, Warning, Error }

public interface IMessageBus
{
    void Publish<T>(T message);
    IObservable<T> Listen<T>();
}

// Usage in ViewModel
public class MainViewModel : ReactiveObject
{
    private readonly IMessageBus _messageBus;

    private ObservableAsPropertyHelper<StatusMessage> _currentStatus;
    public StatusMessage CurrentStatus => _currentStatus.Value;

    public MainViewModel(IMessageBus messageBus)
    {
        _messageBus = messageBus;

        // Listen for status messages
        _currentStatus = _messageBus.Listen<StatusMessage>()
            .ToProperty(this, x => x.CurrentStatus);
    }

    public void DoSomething()
    {
        try {
            // Do work...
            _messageBus.Publish(new StatusMessage("Operation completed", StatusType.Success, TimeSpan.FromSeconds(5)));
        }
        catch (Exception ex) {
            _messageBus.Publish(new StatusMessage(ex.Message, StatusType.Error, TimeSpan.FromSeconds(5)));
        }
    }
}

// Status bar in view
<Border Grid.Row="1" Classes="StatusBar">
    <TextBlock Text="{Binding CurrentStatus.Text}"
               Classes="{Binding CurrentStatus.Type}"/>
</Border>
```

## Migration Tasks and Timeline

### Week 1: Project Setup and Core Architecture

- Create solution and project structure
- Configure .NET 9.0 target framework
- Set up Avalonia UI framework
- Implement core service interfaces
- Set up dependency injection

### Week 2: Core Services Implementation

- Implement file system service
- Implement game process service
- Implement settings service
- Implement theme service
- Create platform-specific implementations

### Week 3: UI Implementation - Main Features

- Create main window layout
- Implement save file listing
- Implement save file loading/backup
- Create save file viewer
- Implement auto-save functionality

### Week 4: UI Implementation - Secondary Features

- Implement settings page
- Add theming support
- Implement file cleanup functionality
- Add error handling and notifications
- Create debug/logging view

### Week 5: Testing and Refinement

- Write unit tests for core functionality
- Test on different operating systems
- Performance optimization
- UX improvements
- Documentation

## Conclusion

By migrating from WPF to Avalonia, we'll achieve several key benefits:

1. **Cross-platform compatibility**: Run on Windows, macOS, and Linux
2. **Modern architecture**: Clean MVVM pattern with ReactiveUI
3. **Simplified code**: Better separation of concerns
4. **Improved maintainability**: Modular design and DI
5. **Enhanced UX**: Native look and feel across platforms
6. **Future-proofing**: .NET 9 support and modern UI framework

The migration will focus on preserving all existing functionality while improving the codebase structure and eliminating platform-specific dependencies where possible. The resulting application will be more maintainable, extensible, and user-friendly across multiple operating systems.
