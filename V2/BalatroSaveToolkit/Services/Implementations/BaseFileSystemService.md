# BaseFileSystemService Implementation

## Overview

The `BaseFileSystemService` is a core implementation of the `IFileSystemService` interface designed to provide platform-agnostic file system operations for the Balatro Save Toolkit. This service provides a robust foundation for platform-specific implementations while implementing the majority of file system operations in a way that works across all MAUI-supported platforms.

## Design Principles

1. **Error Handling**: All operations include comprehensive error handling with proper exception wrapping and logging
2. **Asynchronous Operations**: File operations are implemented using async/await for responsive UI
3. **Abstraction**: Platform-specific details are abstracted behind virtual methods
4. **Dependency Injection**: The service uses DI to get logging and error handling services
5. **Exception Translation**: System exceptions are converted to application-specific exception types

## Key Features

### File Operations
- Reading and writing text and binary files
- File copying, moving, and deletion
- File hashing and information retrieval
- Creation of empty files

### Directory Operations
- Directory creation, deletion, and checking
- Listing files and subdirectories
- Directory information retrieval

### Path Operations
- Cross-platform path normalization and combination
- Directory and file name extraction
- Absolute and relative path conversions

### File Monitoring
- Watching files or directories for changes
- Event-based notification system
- Ability to monitor multiple paths simultaneously

### Platform-Specific Hooks
- Virtual methods for UI operations (file/folder picking)
- Virtual method for Balatro save directory detection
- Application data and temporary directory management

## Usage Examples

### Reading a File
```csharp
try
{
    string content = await fileSystemService.ReadTextAsync("path/to/file.txt");
    // Process content
}
catch (FileNotFoundException ex)
{
    // Handle file not found
}
catch (FileAccessException ex)
{
    // Handle access denied
}
catch (FileSystemException ex)
{
    // Handle other errors
}
```

### File Monitoring
```csharp
// Register for the event
fileSystemService.FileChanged += (sender, args) =>
{
    string filePath = args.Path;
    FileSystemChangeType changeType = args.ChangeType;
    DateTime changeTime = args.ChangeTime;

    // React to file change
};

// Start monitoring
string monitorToken = await fileSystemService.StartMonitoringAsync("path/to/watch", true);

// Later, stop monitoring
await fileSystemService.StopMonitoringAsync(monitorToken);
```

## Platform-Specific Overrides

The base implementation marks certain methods as `virtual` to allow platform-specific implementations to override them. These methods include:

1. **GetBalatroSaveDirectoryAsync()**: Must be implemented by each platform to return the correct save location
2. **PickFileAsync()**: For platform-specific file picker UI
3. **PickFolderAsync()**: For platform-specific folder picker UI
4. **PickSaveFileAsync()**: For platform-specific save dialogs
5. **PickFilesAsync()**: For platform-specific multi-file selection

## Error Handling Strategy

The service implements a comprehensive error handling strategy:

1. **Logging**: All errors are logged with appropriate severity levels
2. **Exception Translation**: System exceptions are translated to app-specific types
3. **Graceful Degradation**: Where possible, fallbacks are provided
4. **User Feedback**: Integration with the error handling service to show user-friendly messages

## Implementation Notes

- File system watchers are tracked in a dictionary to allow multiple monitoring sessions
- Path operations have safeguards for invalid inputs
- Hash computation is extensible to support multiple algorithms
- Temporary file/directory creation uses platform-specific paths when possible
