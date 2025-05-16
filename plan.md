# Balatro Save and Load Tool - Migration Plan from WPF to .NET MAUI

## Overview

This document outlines the plan to migrate the Balatro Save and Load Tool from WPF to .NET MAUI. The migration aims to make the application cross-platform, simplify its architecture, and improve modularity for future enhancements.

## Current Architecture

The current WPF application follows a simple structure:

- **MainWindow.xaml/MainWindow.xaml.cs**: The main interface that handles most of the application logic
- **DebugWindow.xaml/DebugWindow.xaml.cs**: A secondary window that shows debug logs
- **JsonViewerWindow.xaml/JsonViewerWindow.xaml.cs**: A window that displays the Lua dictionary content of save files
- **Themes/Dark.xaml and Themes/Light.xaml**: Theme resources for styling
- **App.xaml/App.xaml.cs**: Application entry point

The application is heavily dependent on WPF-specific features:

- Custom window chrome (titlebar, buttons)
- WPF-specific UI controls
- Platform-specific file paths
- Windows-specific interop for features like window flashing
- Direct file system access with Windows-specific paths

## Target Architecture for .NET MAUI

### Core Components

1. **Core Logic Layer**:
   - SaveManager: Handles saving/loading operations
   - ConfigManager: Manages application configuration
   - LogManager: Manages logging
   - SaveFileParser: Handles parsing and formatting of Balatro save files

2. **UI Layer**:
   - MainPage: Primary UI
   - DebugPage: Page for viewing logs
   - SaveViewerPage: Page for viewing save file content with improved visualization

3. **Services**:
   - FileSystemService: Platform-agnostic file operations
   - GameMonitorService: Process monitoring across platforms
   - ThemeService: Handling application theming
   - LoggingService: Application-wide logging

### Key Architectural Changes

1. **MVVM Architecture**:
   - Implement proper separation of concerns using MVVM pattern
   - Move business logic from UI code into ViewModels
   - Use commanding pattern for UI actions

2. **Dependency Injection**:
   - Use MAUI's built-in dependency injection container
   - Register and inject services instead of direct instantiation

3. **File System Abstraction**:
   - Create platform-agnostic file system service
   - Use MAUI's `FileSystem` APIs for cross-platform compatibility

4. **Configuration Management**:
   - Create a configuration system with default settings
   - Support custom paths for save files
   - Persist settings across app restarts

## Specific Areas for Improvement

### 1. Custom Title Bar Removal

The current implementation uses a custom title bar with custom buttons. In MAUI:

- Use native title bars on each platform
- Remove custom chrome code (TitleBar_MouseLeftButtonDown, MinimizeButton_Click, CloseButton_Click)
- Leverage platform-specific behavior for window management

```csharp
// Current WPF custom title bar handling:
private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
    if (e.ClickCount == 2) {
        // Optional: maximize/restore on double-click
    } else {
        DragMove();
    }
}

// In MAUI, this will be handled by the native shell
```

### 2. Process Detection

Current implementation uses Windows-specific `Process.GetProcessesByName()` to detect if Balatro is running:

```csharp
private bool IsBalatroProcessRunning() {
    try {
        return Process.GetProcessesByName("balatro").Any();
    }
    catch { return false; }
}
```

In MAUI:

- Create a platform-specific service implementation using DI
- Implement specific detection strategies for each platform

```csharp
public interface IProcessMonitorService
{
    bool IsProcessRunning(string processName);
}

// Windows implementation
public class WindowsProcessMonitorService : IProcessMonitorService
{
    public bool IsProcessRunning(string processName) => Process.GetProcessesByName(processName).Any();
}

// macOS implementation
public class MacOSProcessMonitorService : IProcessMonitorService
{
    public bool IsProcessRunning(string processName)
    {
        // Use NSWorkspace or process output from "ps" to detect process
    }
}
```

### 3. Replace Window Flashing with Notifications

The current app flashes the window using P/Invoke to Windows APIs:

```csharp
[System.Runtime.InteropServices.DllImport("user32.dll")]
private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
```

Replace with cross-platform notifications:

- Use MAUI's notification system
- Provide visual feedback through UI animations that work cross-platform

### 4. Save File Handling and Paths

The current implementation assumes Windows path structures:

```csharp
string GetCurrentSaveFile() {
    var profileNumber = ProfileComboBox.SelectedIndex + 1;
    return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Balatro",
        profileNumber.ToString(),
        "save.jkr"
    );
}
```

Replace with platform-agnostic paths:

- Use MAUI's `FileSystem.AppDataDirectory` for app data
- Create configuration system to define platform-specific paths
- Allow user configuration of save file locations

### 5. Debug View & Logging

Current implementation uses a separate window for debug logs:

```csharp
private DebugWindow? _debugWindow;
private ObservableCollection<string> _debugLog = new ObservableCollection<string>();
```

Replace with:

- Proper logging system using Microsoft.Extensions.Logging
- Log file persistence
- Debug page (not window) with filtered views
- Session-specific log display

### 6. Save File Viewer ("JSON View")

The current JsonViewerWindow is misnamed since it displays Lua dictionary data, not JSON:

```csharp
private JsonViewerWindow? _jsonViewerWindow;
private string _currentJsonContent = "";
```

Replace with:

- SaveViewerPage with improved visualization
- Proper highlighting using a syntax highlighting library
- Structured view that can be navigated (tree view or expandable sections)
- Rename to reflect actual content (LuaDictionaryViewer or SaveDataViewer)

### 7. Timers

The current implementation uses multiple WPF DispatcherTimer instances:

```csharp
private readonly DispatcherTimer _timer = new DispatcherTimer();
private readonly DispatcherTimer _statusResetTimer = new DispatcherTimer();
private readonly DispatcherTimer _errorCheckTimer = new DispatcherTimer();
// etc...
```

Replace with:

- MAUI's IDispatcher Timer or System.Timers.Timer
- Consolidate timer management into a service
- Use async/await patterns for better resource management

### 8. Status Management

Current status display is handled directly in UI code:

```csharp
private void SetErrorStatus(string errorMessage, Func<bool>? conditionChecker = null) {
    _hasActiveError = true;
    _currentErrorMessage = errorMessage;
    // etc...
}
```

Replace with:

- Status service with events/notifications
- Better error handling with proper exception management
- Toast notifications for status changes

## Migration Strategy

### Phase 1: Project Setup and Core Services

1. Create new MAUI project targeting .NET 9
2. Set up project structure with MVVM pattern
3. Implement core services (logging, configuration, file system)
4. Create data models and interfaces

### Phase 2: UI Framework

1. Design main page layout
2. Create navigation system between pages
3. Implement theme system with light/dark mode
4. Design and implement save list with selection

### Phase 3: Core Functionality

1. Implement save file loading/parsing
2. Implement save file management
3. Create game process detection service
4. Implement auto-save functionality

### Phase 4: Advanced Features

1. Create enhanced save viewer with structured navigation
2. Implement logging system with persistent logs
3. Add configuration options for paths and settings
4. Create auto-cleanup functionality

### Phase 5: Testing and Polishing

1. Test on multiple platforms
2. Optimize UI for different screen sizes
3. Add platform-specific enhancements
4. Create installers/packages for different platforms

## Technical Implementation Details

### Project Structure

```
BalatroSaveAndLoad.Maui/
  |- App.xaml/App.xaml.cs          # App entry point
  |- AppShell.xaml/AppShell.xaml.cs # Main shell with navigation
  |- MauiProgram.cs                # Service registration & config
  |- Models/                       # Data models
  |    |- SaveFile.cs              # Save file data model
  |    |- AppConfig.cs             # App configuration model
  |    |- LogEntry.cs              # Logging entry model
  |- Services/
  |    |- IFileSystemService.cs    # File system abstraction interface
  |    |- FileSystemService.cs     # Cross-platform file operations
  |    |- IProcessMonitorService.cs # Process monitor interface
  |    |- ProcessMonitorService.cs # Platform-specific implementations
  |    |- IConfigService.cs        # Configuration interface
  |    |- ConfigService.cs         # Settings management
  |    |- ILogService.cs           # Logging interface
  |    |- LogService.cs            # Logging implementation
  |    |- ISaveFileService.cs      # Save file operations interface
  |    |- SaveFileService.cs       # Save file operations implementation
  |- ViewModels/
  |    |- ViewModelBase.cs         # Base ViewModel with INotifyPropertyChanged
  |    |- MainViewModel.cs         # Main page logic
  |    |- SaveViewerViewModel.cs   # Save content viewer logic
  |    |- LogViewModel.cs          # Debug log viewer logic
  |    |- SettingsViewModel.cs     # App settings logic
  |- Views/
  |    |- MainPage.xaml            # Main UI
  |    |- SaveViewerPage.xaml      # Save content viewer
  |    |- LogPage.xaml             # Debug log viewer
  |    |- SettingsPage.xaml        # Settings page
  |- Resources/
       |- Styles/
       |    |- Colors.xaml         # Color definitions
       |    |- Styles.xaml         # UI styles
       |- Images/                  # App icons and images
```

### Configuration File Format

```json
{
  "GamePaths": {
    "Windows": {
      "SavePath": "%APPDATA%/Balatro"
    },
    "macOS": {
      "SavePath": "~/Library/Application Support/Balatro"
    },
    "Linux": {
      "SavePath": "~/.local/share/Balatro"
    }
  },
  "AppSettings": {
    "DefaultProfile": 1,
    "AutoSaveInterval": 5,
    "AutoCleanupDays": 7,
    "LogLevel": "Information",
    "Theme": "System"
  }
}
```

### Code Examples

#### MVVM Implementation

```csharp
// MainViewModel.cs
public class MainViewModel : ViewModelBase
{
    private readonly ISaveFileService _saveService;
    private readonly IProcessMonitorService _processMonitor;
    private readonly ILogService _logService;

    private bool _isGameRunning;
    public bool IsGameRunning
    {
        get => _isGameRunning;
        set => SetProperty(ref _isGameRunning, value);
    }

    public ObservableCollection<SaveFileModel> SaveFiles { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand OpenFolderCommand { get; }

    public MainViewModel(
        ISaveFileService saveService,
        IProcessMonitorService processMonitor,
        ILogService logService)
    {
        _saveService = saveService;
        _processMonitor = processMonitor;
        _logService = logService;

        SaveCommand = new AsyncCommand(SaveGameAsync, CanSaveGame);
        LoadCommand = new AsyncCommand<SaveFileModel>(LoadGameAsync, CanLoadGame);
        OpenFolderCommand = new AsyncCommand(OpenSaveFolderAsync);

        // Start monitoring game process
        StartProcessMonitoring();
    }

    private async Task SaveGameAsync()
    {
        try
        {
            await _saveService.SaveCurrentGameAsync();
            await RefreshSaveListAsync();
        }
        catch (Exception ex)
        {
            _logService.LogError($"Error saving game: {ex.Message}");
            await NotificationService.ShowErrorAsync("Save Error", ex.Message);
        }
    }

    // Other methods...
}
```

## Save File Parser and Viewer Improvements

The current "JSON View" is actually displaying Lua dictionary data and has limited formatting capabilities:

```csharp
// Current formatting approach in JsonViewerWindow.xaml.cs
private string FormatLuaContent(string content)
{
    // Basic text formatting with simple rules
}
```

A more sophisticated approach in MAUI would be:

1. **Parsing**: Create a proper parser for Lua dictionaries
2. **Tree View**: Show structured data in a tree view
3. **Data Binding**: Bind parsed data to UI controls
4. **Syntax Highlighting**: Use a syntax highlighting library for better visualization

Example structure:

```csharp
// Data model
public class LuaNode
{
    public string Key { get; set; }
    public object Value { get; set; }
    public NodeType Type { get; set; }
    public ObservableCollection<LuaNode> Children { get; } = new();
}

// Parser service
public interface ISaveFileParserService
{
    LuaNode ParseSaveFile(string content);
}
```

This would power a tree view or expandable list view:

```xml
<!-- MAUI XAML -->
<CollectionView ItemsSource="{Binding RootNodes}">
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:LuaNode">
            <StackLayout>
                <Label Text="{Binding Key}" />
                <Label Text="{Binding Value}" IsVisible="{Binding HasSimpleValue}" />
                <Button Text="Expand" IsVisible="{Binding HasChildren}"
                        Command="{Binding Source={RelativeSource AncestorType={x:Type viewmodels:SaveViewerViewModel}}, Path=ExpandNodeCommand}"
                        CommandParameter="{Binding}" />
            </StackLayout>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

## Logging System Implementation

The current debug logging is very basic:

```csharp
private void DebugLog(string message)
{
    var timestamp = DateTime.Now.ToString("HH:mm:ss");
    var entry = $"[{timestamp}] {message}";
    _debugLog.Add(entry);
    if (_debugWindow != null) { _debugWindow.AppendLog(entry); }
}
```

A robust MAUI logging system would:

1. Use Microsoft.Extensions.Logging
2. Write logs to files
3. Support different log levels
4. Provide filtering by category or level

Implementation:

```csharp
// Registration in MauiProgram.cs
builder.Services.AddLogging(builder =>
{
    builder.AddDebug();
    builder.AddFile(options =>
    {
        options.LogFilePath = Path.Combine(FileSystem.CacheDirectory, "logs", "balatro-save-tool.log");
        options.RetainDays = 7;
    });
});

// Usage throughout the app
public class SaveFileService : ISaveFileService
{
    private readonly ILogger<SaveFileService> _logger;

    public SaveFileService(ILogger<SaveFileService> logger)
    {
        _logger = logger;
    }

    public async Task SaveGameAsync()
    {
        _logger.LogInformation("Starting save operation at {Time}", DateTime.Now);
        // Implementation
    }
}
```

## Benefits of Migration

1. **Cross-platform support**: Run on Windows, macOS, and Linux
2. **Modern architecture**: Clean separation of concerns with MVVM
3. **Improved maintainability**: Modular design for easier future enhancements
4. **Better user experience**: Native look and feel on each platform
5. **Configuration flexibility**: User-configurable paths and settings
6. **Enhanced save viewer**: Better visualization of save file content
7. **Proper logging**: Full logging system with persistent storage
8. **Modern UI**: MAUI controls with responsive design

## Potential Challenges

1. **Platform-specific behaviors**: Handling process detection on different platforms
2. **File system permissions**: Working with different OS security models
3. **Save file format compatibility**: Ensuring save files work the same across platforms
4. **Performance considerations**: Ensuring MAUI app is as responsive as WPF version
5. **Testing requirements**: Need to test on multiple platforms

## Timeline and Priorities

1. Core functionality (saving/loading): 1-2 weeks
2. UI implementation: 1-2 weeks
3. Platform-specific services: 1 week
4. Enhanced save viewer: 1-2 weeks
5. Settings and logging systems: 1 week
6. Testing and polishing: 1-2 weeks

Total estimated time: 6-10 weeks depending on complexity and testing requirements.

## Conclusion

Migrating from WPF to .NET MAUI will enable the Balatro Save and Load Tool to reach a broader audience by supporting multiple platforms. The migration also presents an opportunity to modernize the codebase with better architecture, improved modularity, and enhanced features. By addressing the current limitations and leveraging the strengths of MAUI, the application will be more maintainable and extensible for future enhancements.
