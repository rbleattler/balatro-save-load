# IFileSystemService Interface - Detailed Specification

## Overview

The `IFileSystemService` interface provides a comprehensive abstraction for file system operations across Windows, macOS, and Linux platforms. It is designed to hide platform-specific implementation details while offering a consistent API for application code.

## Method Contracts and Behaviors

### File Operations

#### `Task<bool> FileExistsAsync(string path)`

**Purpose:**
Checks whether a file exists at the specified path.

**Input Validation:**
- `path` should not be null or empty
- Path should be properly formatted for the current platform

**Return Value:**
- `true` if the file exists and is accessible
- `false` if the file does not exist or cannot be verified

**Exception Behavior:**
- Does not throw for non-existent files (returns `false` instead)
- May throw `FileSystemException` for severe access issues

**Platform Considerations:**
- Windows: Case-insensitive file matching
- macOS: Generally case-insensitive but can be case-sensitive depending on filesystem
- Linux: Case-sensitive file matching

**Thread Safety:**
Thread-safe, but the file system state may change between calls.

#### `Task<string> ReadTextAsync(string path, System.Text.Encoding? encoding = null)`

**Purpose:**
Reads the entire contents of a text file asynchronously.

**Input Validation:**
- `path` must not be null or empty
- File must exist (throws `FileNotFoundException` otherwise)

**Return Value:**
The complete text contents of the file as a string.

**Exception Behavior:**
- `FileNotFoundException`: File does not exist
- `FileAccessException`: Cannot access due to permissions, locks
- `FileSystemException`: Other file system errors

**Platform Considerations:**
- Windows: Uses CRLF (`\r\n`) line endings by default
- macOS/Linux: Uses LF (`\n`) line endings by default
- Default encoding is UTF-8 without BOM on all platforms

**Thread Safety:**
Thread-safe, but requires exclusive read access to the file.

#### `Task<byte[]> ReadBytesAsync(string path)`

**Purpose:**
Reads the entire contents of a file as a byte array asynchronously.

**Input Validation:**
- `path` must not be null or empty
- File must exist (throws `FileNotFoundException` otherwise)

**Return Value:**
The complete contents of the file as a byte array.

**Exception Behavior:**
- `FileNotFoundException`: File does not exist
- `FileAccessException`: Cannot access due to permissions, locks
- `FileSystemException`: Other file system errors

**Performance Considerations:**
- For large files, consider using stream-based alternatives not included in this interface
- Memory usage scales with file size

**Thread Safety:**
Thread-safe, but requires exclusive read access to the file.

#### `Task WriteTextAsync(string path, string content, System.Text.Encoding? encoding = null, bool overwrite = true)`

**Purpose:**
Writes text to a file asynchronously, optionally overwriting existing content.

**Input Validation:**
- `path` must not be null or empty
- `content` can be null or empty (creates empty file)

**Behavior:**
- Creates necessary parent directories if they don't exist
- Overwrites existing file if `overwrite` is true
- Throws exception if file exists and `overwrite` is false

**Exception Behavior:**
- `FileAccessException`: Cannot access due to permissions, locks
- `DirectoryNotFoundException`: Parent directory doesn't exist and cannot be created
- `FileAlreadyExistsException`: File exists and overwrite is false
- `FileSystemException`: Other file system errors

**Platform Considerations:**
- Windows: Default encoding is UTF-8 without BOM
- Line ending normalization is not performed (use platform-appropriate endings)

**Thread Safety:**
Thread-safe, but requires exclusive write access to the file.

### Directory Operations

#### `Task<bool> DirectoryExistsAsync(string path)`

**Purpose:**
Checks whether a directory exists at the specified path.

**Input Validation:**
- `path` should not be null or empty
- Path should be properly formatted for the current platform

**Return Value:**
- `true` if the directory exists and is accessible
- `false` if the directory does not exist or cannot be verified

**Exception Behavior:**
- Does not throw for non-existent directories (returns `false` instead)
- May throw `FileSystemException` for severe access issues

**Thread Safety:**
Thread-safe, but the file system state may change between calls.

#### `Task<bool> CreateDirectoryAsync(string path, bool recursive = true)`

**Purpose:**
Creates a directory at the specified path, optionally creating parent directories.

**Input Validation:**
- `path` should not be null or empty
- Path should be properly formatted for the current platform

**Return Value:**
- `true` if the directory was created
- `false` if the directory already exists

**Behavior:**
- When `recursive` is true, creates all parent directories as needed
- When `recursive` is false, fails if any parent directory does not exist

**Exception Behavior:**
- `DirectoryAccessException`: Cannot create due to permissions, locks
- `DirectoryNotFoundException`: Parent directory doesn't exist and recursive is false
- `FileSystemException`: Other file system errors

**Thread Safety:**
Thread-safe, but concurrent creation attempts may interfere.

### Path Operations

#### `string CombinePaths(params string[] parts)`

**Purpose:**
Combines multiple path parts into a single path using the platform-specific separator.

**Input Validation:**
- Handles null or empty segments gracefully
- Validates part formatting where possible

**Return Value:**
A combined path string using platform-appropriate path separators.

**Behavior:**
- Normalizes separators to the platform standard
- Handles consecutive separators appropriately
- Preserves root indicators (e.g., Windows drive letters, UNC paths, Unix root)

**Exception Behavior:**
- Does not throw exceptions (returns best-effort result)

**Platform Considerations:**
- Windows: Uses backslash (`\`) as separator
- macOS/Linux: Uses forward slash (`/`) as separator

**Thread Safety:**
Thread-safe (stateless operation).

### Platform-Specific Operations

#### `Task<string> GetBalatroSaveDirectoryAsync()`

**Purpose:**
Gets the platform-specific directory where Balatro stores save files.

**Return Value:**
The full path to the Balatro save directory.

**Exception Behavior:**
- `PlatformNotSupportedException`: Current platform is not supported
- `DirectoryNotFoundException`: Save directory cannot be found
- `FileSystemException`: Other file system errors

**Platform Considerations:**
- Windows: `%USERPROFILE%\AppData\Local\Balatro`
- macOS: `~/Library/Application Support/Balatro`
- Linux: `~/.local/share/Balatro`

**Special Cases:**
- Steam installations may have different save locations
- Considers portable installations
- Handles non-default installation paths where possible

**Thread Safety:**
Thread-safe, but directory may not exist or may change.

### UI Operations

#### `Task<string> PickFileAsync(string title, string filter)`

**Purpose:**
Displays a platform-native file picker dialog to let the user select a file.

**Input Validation:**
- `title` should not be null, used as dialog title
- `filter` specifies file extensions (format varies by platform)

**Return Value:**
- The full path to the selected file
- Empty string if the user cancels the dialog

**Exception Behavior:**
- `PlatformNotSupportedException`: Current platform doesn't support file pickers
- `FileSystemException`: Other file system errors

**Platform Considerations:**
- Windows: Uses Windows file picker dialog
- macOS: Uses Cocoa file picker dialog
- Linux: Uses GTK file picker dialog

**Thread Safety:**
Not thread-safe, must be called from the UI thread.

### File Monitoring

#### `Task<string> StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*")`

**Purpose:**
Starts monitoring a file or directory for changes.

**Input Validation:**
- `path` must exist and be accessible
- `filter` follows standard wildcard patterns

**Return Value:**
A monitoring token that can be used to stop monitoring.

**Behavior:**
- Monitors file creation, modification, deletion, and renaming
- If path is a file, monitors the file; if a directory, monitors the directory
- Raises `FileChanged` event when changes occur

**Exception Behavior:**
- `FileNotFoundException`: File doesn't exist
- `DirectoryNotFoundException`: Directory doesn't exist
- `FileSystemException`: Other file system errors

**Platform Considerations:**
- Windows: Uses FileSystemWatcher internally
- macOS: Uses FSEvents API
- Linux: Uses inotify or equivalent

**Resource Management:**
Monitoring consumes system resources; use `StopMonitoringAsync` to release.

**Event Dispatch:**
- Events may be raised on background threads
- Multiple events may be raised for a single logical operation
- Event order is not guaranteed

## Integration Patterns

### Dependency Injection

The service should be registered in the application's DI container:

```csharp
// In MAUI application startup
builder.Services.AddSingleton<IFileSystemService, BaseFileSystemService>();
builder.Services.AddSingleton<IFileSystemService, WindowsFileSystemService>(
    serviceProvider => OperatingSystem.IsWindows()
        ? new WindowsFileSystemService(...)
        : null);
// Add similar conditionals for macOS and Linux
```

### Platform-Specific Resolution

```csharp
// In application code that needs to use the service
public class SaveFileManager
{
    private readonly IFileSystemService _fileSystem;

    public SaveFileManager(IFileSystemService fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<SaveData> LoadSaveDataAsync()
    {
        // The correct platform implementation will be injected
        string saveDir = await _fileSystem.GetBalatroSaveDirectoryAsync();
        string saveFilePath = _fileSystem.CombinePaths(saveDir, "save1.json");

        if (await _fileSystem.FileExistsAsync(saveFilePath))
        {
            string json = await _fileSystem.ReadTextAsync(saveFilePath);
            return JsonSerializer.Deserialize<SaveData>(json);
        }

        return null;
    }
}
```

## Implementation Guidelines

### Thread Safety Requirements

1. **Concurrent Access**: Methods should be thread-safe for concurrent calls
2. **State Management**: Avoid instance state that could be corrupted by concurrent access
3. **Event Dispatch**: Events should include proper synchronization
4. **UI Methods**: UI-related methods should post to the UI thread as needed

### Error Handling Best Practices

1. **Specific Exceptions**: Use the most specific exception type from the hierarchy
2. **Don't Swallow Exceptions**: Allow exceptions to propagate where appropriate
3. **Meaningful Messages**: Include path information in exception messages
4. **Inner Exceptions**: Preserve inner exceptions for debugging
5. **Retry Logic**: For transient errors, implement appropriate retry logic

### Performance Optimization

1. **Minimize Disk Access**: Batch operations where possible
2. **Buffer Management**: Use appropriate buffer sizes for large files
3. **Async Patterns**: Implement true async I/O, not just Task wrappers
4. **Caching**: Consider caching for frequently accessed paths
5. **Memory Usage**: Be conscious of memory usage with large files

## Testing Recommendations

### Unit Testing

1. **Mock Implementation**: Create a mock implementation for testing
2. **File System Virtualization**: Use in-memory file system for tests
3. **Test Edge Cases**: Ensure tests cover:
   - Non-existent files/directories
   - Permission issues
   - Special characters in paths
   - Very large files
   - Concurrency issues

### Integration Testing

1. **Cross-Platform Testing**: Test on all target platforms
2. **Real File System Testing**: Test with actual file system, not just mocks
3. **UI Testing**: Test file picker integration with actual UI
4. **Error Conditions**: Test error handling with corrupted files

## Security Considerations

1. **Path Traversal**: Validate paths to prevent directory traversal attacks
2. **Sensitive Data**: Use secure file operations for sensitive data
3. **Permissions**: Use least-privilege access when possible
4. **Temp Files**: Secure and clean up temporary files

## Platform-Specific Notes

### Windows Implementation

1. **Path Length Limitations**:
   - Standard MAX_PATH is 260 characters
   - Use extended paths (`\\?\` prefix) for longer paths

2. **File Locking**:
   - Windows locks files more aggressively than other platforms
   - Use FileShare options appropriately

3. **UNC Paths**:
   - Support network shares with UNC paths
   - Handle timeouts appropriately

### macOS Implementation

1. **File System**:
   - APFS (Apple File System) on modern macOS
   - Support case-sensitive and case-insensitive modes

2. **Resource Forks**:
   - Handle files with resource forks
   - Avoid creating `.DS_Store` files

3. **Bundle Structure**:
   - Respect macOS application bundle structure
   - Use NSFileManager for certain operations

### Linux Implementation

1. **File System Variety**:
   - Support various Linux file systems (ext4, XFS, etc.)
   - Handle different mount point restrictions

2. **Permissions**:
   - Respect Linux file permissions model
   - Handle symbolic links correctly

3. **Special Files**:
   - Handle device files, sockets, and other special file types
   - Support file system mount points
