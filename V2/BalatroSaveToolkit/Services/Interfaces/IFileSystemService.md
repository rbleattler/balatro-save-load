# IFileSystemService Interface Documentation

## Overview

The `IFileSystemService` interface provides a comprehensive abstraction for file system operations across different platforms. It is designed to handle platform-specific differences while providing a consistent API for the application to use.

## Key Features

- **Platform Agnostic Operations**: Common file operations work the same way across Windows, macOS, and Linux
- **Platform-Specific Handling**: Internally handles platform differences in paths, permissions, and APIs
- **Proper Error Handling**: Structured exception hierarchy for different types of file system errors
- **File Monitoring**: Support for watching files and directories for changes
- **UI Integration**: Methods for showing platform-native file and folder pickers

## Usage Examples

### Basic File Operations

```csharp
// Reading and writing text files
string content = await fileSystem.ReadTextAsync("path/to/file.txt");
await fileSystem.WriteTextAsync("path/to/output.txt", "Hello, world!");

// Working with binary data
byte[] data = await fileSystem.ReadBytesAsync("path/to/image.png");
await fileSystem.WriteBytesAsync("path/to/copy.png", data);

// File manipulation
await fileSystem.CopyFileAsync("source.txt", "destination.txt");
await fileSystem.MoveFileAsync("old.txt", "new.txt");
await fileSystem.DeleteFileAsync("unwanted.txt");

// File info and verification
FileInfo info = await fileSystem.GetFileInfoAsync("data.json");
string hash = await fileSystem.GetFileHashAsync("important.dat");
```

### Directory Operations

```csharp
// Creating and managing directories
await fileSystem.CreateDirectoryAsync("new/nested/directory", recursive: true);
await fileSystem.DeleteDirectoryAsync("temp/dir", recursive: true);

// Listing directory contents
IEnumerable<string> files = await fileSystem.GetFilesAsync("docs", "*.md", recursive: true);
IEnumerable<string> subDirs = await fileSystem.GetDirectoriesAsync("project");

// Directory information
DirectoryInfo dirInfo = await fileSystem.GetDirectoryInfoAsync("logs");
```

### Path Manipulation

```csharp
// Combining paths in a platform-appropriate way
string fullPath = fileSystem.CombinePaths("base", "sub", "file.txt");

// Path extraction
string dir = fileSystem.GetDirectoryName("/path/to/file.txt");  // Returns "/path/to"
string file = fileSystem.GetFileName("/path/to/file.txt");      // Returns "file.txt"
string name = fileSystem.GetFileNameWithoutExtension("/path/to/file.txt"); // Returns "file"
string ext = fileSystem.GetExtension("/path/to/file.txt");      // Returns ".txt"

// Path normalization and conversion
string normalized = fileSystem.NormalizePath("path\\to/file.txt"); // Normalizes to platform format
string absolute = fileSystem.GetAbsolutePath("relative/path");
string relative = fileSystem.GetRelativePath("/base/dir", "/base/dir/sub/file.txt"); // Returns "sub/file.txt"
```

### Platform-Specific Operations

```csharp
// Getting platform-specific directories
string saveDir = await fileSystem.GetBalatroSaveDirectoryAsync();
string appDataDir = await fileSystem.GetApplicationDataDirectoryAsync("MyApp");
string tempDir = await fileSystem.GetTempDirectoryAsync();

// Creating temporary files and directories
string tempFile = await fileSystem.CreateTempFileAsync(".tmp");
string tempDir = await fileSystem.CreateTempDirectoryAsync();
```

### UI Integration

```csharp
// Showing file pickers
string selectedFile = await fileSystem.PickFileAsync("Select a file", "*.txt");
string selectedFolder = await fileSystem.PickFolderAsync("Select a folder");
string saveLocation = await fileSystem.PickSaveFileAsync("Save As", "document.txt", "*.txt");
string[] multipleFiles = await fileSystem.PickFilesAsync("Select files", "*.png");
```

### File Monitoring

```csharp
// Start monitoring a directory for changes
fileSystem.FileChanged += OnFileChanged;
string token = await fileSystem.StartMonitoringAsync("watch/directory", true, "*.json");

// Handle file change events
void OnFileChanged(object sender, FileSystemEventArgs e)
{
    Console.WriteLine($"File {e.Path} was {e.ChangeType} at {e.ChangeTime}");

    // React to different change types
    switch (e.ChangeType)
    {
        case FileSystemChangeType.Created:
            // Handle new file
            break;
        case FileSystemChangeType.Modified:
            // Handle updated file
            break;
        // Handle other change types...
    }
}

// Stop monitoring when done
await fileSystem.StopMonitoringAsync(token);
```

## Error Handling

```csharp
try
{
    await fileSystem.WriteTextAsync("path/to/file.txt", "content");
}
catch (FileAccessException ex)
{
    // Handle permission issues
    Console.WriteLine($"Cannot access file: {ex.FilePath}");
}
catch (DirectoryNotFoundException ex)
{
    // Handle missing directory
    Console.WriteLine("Directory does not exist");
}
catch (FileAlreadyExistsException ex)
{
    // Handle file already exists
    Console.WriteLine($"File already exists: {ex.FilePath}");
}
catch (FileSystemException ex)
{
    // Handle other file system errors
    Console.WriteLine($"File system error: {ex.Message}");
}
```

## Platform-Specific Considerations

### Windows

- Windows uses backslashes (`\`) as path separators
- Has a 260 character path length limit (unless using extended paths)
- Uses drive letters (e.g., `C:`)
- Filenames are case-insensitive

### macOS

- Uses forward slashes (`/`) as path separators
- Uses Unix-style paths starting with `/`
- Uses Unicode normalization form NFD for filenames
- Filenames are case-insensitive by default but on a case-preserving file system

### Linux

- Uses forward slashes (`/`) as path separators
- Uses Unix-style paths starting with `/`
- Case-sensitive filenames
- Hidden files and directories start with a dot (`.`)

## Best Practices

1. **Always use async methods**: All file I/O operations should be performed asynchronously to avoid blocking the UI thread.

2. **Handle exceptions appropriately**: Each method documents the specific exceptions it throws. Catch and handle these appropriately.

3. **Use cancellation tokens for long operations**: For long-running operations, provide a cancellation token to allow cancellation.

4. **Dispose of resources**: Ensure any resources returned are properly disposed of when done.

5. **Be mindful of platform differences**: While the interface abstracts platform differences, be aware of platform-specific behaviors when designing cross-platform functionality.

## Implementation Notes

When implementing this interface:

- Use MAUI's built-in file APIs when possible for cross-platform support
- Use platform-specific code only when necessary for performance or functionality
- Implement proper error handling and retry logic for transient errors
- Use platform detection to select the appropriate implementation at runtime
