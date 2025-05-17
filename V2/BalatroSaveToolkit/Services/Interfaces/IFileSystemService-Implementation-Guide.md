# IFileSystemService - Implementation Guidelines

## Overview

This document provides detailed guidelines for implementing the `IFileSystemService` interface across different platforms. It covers platform-specific considerations, common pitfalls, and best practices to ensure consistent behavior across Windows, macOS, and Linux.

## General Implementation Approach

### Base Implementation Pattern

The recommended implementation approach is:

1. Create a base `BaseFileSystemService` class that implements platform-agnostic functionality
2. Create platform-specific implementations that extend the base class:
   - `WindowsFileSystemService`
   - `MacFileSystemService`
   - `LinuxFileSystemService`
3. Use dependency injection to select the appropriate implementation at runtime

```csharp
// Base implementation with platform-agnostic code
public class BaseFileSystemService : IFileSystemService
{
    // Implementation of platform-agnostic methods
    // Virtual methods for platform-specific operations
}

// Platform-specific implementation
public class WindowsFileSystemService : BaseFileSystemService
{
    // Override virtual methods for Windows-specific behavior
}
```

### Designing for Testability

Make your implementation testable by:

1. Avoiding direct static calls to file system APIs where possible
2. Using dependency injection for services like logging and error handling
3. Creating clear seams for mocking in unit tests
4. Exposing internal behaviors through protected virtual methods

```csharp
// Example of a testable implementation
public class BaseFileSystemService : IFileSystemService
{
    private readonly ILogService _logger;
    private readonly IErrorHandlingService _errorHandler;

    public BaseFileSystemService(ILogService logger, IErrorHandlingService errorHandler)
    {
        _logger = logger;
        _errorHandler = errorHandler;
    }

    // Protected virtual method that can be overridden in tests
    protected virtual Stream OpenFileStream(string path, FileMode mode)
    {
        return File.Open(path, mode);
    }

    // Public method that uses the protected virtual method
    public async Task<string> ReadTextAsync(string path, Encoding encoding = null)
    {
        try
        {
            using var stream = OpenFileStream(path, FileMode.Open);
            using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading file: {path}", ex);
            throw; // Re-throw after logging
        }
    }
}
```

## File Operation Guidelines

### Reading and Writing Files

1. **True Async Implementation**:
   - Use truly asynchronous APIs (`FileStream.ReadAsync`, etc.)
   - Avoid wrapping synchronous calls in `Task.Run`

2. **Encoding Handling**:
   - Default to UTF-8 without BOM
   - Respect user-specified encodings when provided
   - Handle encoding detection where possible

3. **Buffer Management**:
   - Use appropriate buffer sizes for efficiency
   - Consider memory usage for large files
   - Implement streaming for very large files

4. **Error Handling**:
   - Check for file existence before operations
   - Use try/catch blocks around all I/O
   - Translate system exceptions to application exceptions

### File Copying and Moving

1. **Security and Attributes**:
   - Preserve file attributes when copying
   - Maintain file security settings where appropriate
   - Handle read-only files gracefully

2. **Overwrite Behavior**:
   - Respect the `overwrite` parameter strictly
   - Throw the correct exception type when overwrite is false
   - Ensure atomic operations where possible

3. **Cross-Volume Operations**:
   - Handle moves across volumes correctly
   - Fall back to copy+delete when necessary
   - Track and report progress for large files

## Path Operations Guidelines

### Path Handling

1. **Normalization**:
   - Convert separators to the platform standard
   - Handle relative paths correctly
   - Resolve `.` and `..` path components

2. **Character Encoding**:
   - Handle Unicode paths properly
   - Apply platform-specific normalization
   - Handle special characters safely

3. **Path Validation**:
   - Sanitize input paths for security
   - Check for invalid characters
   - Validate path lengths against platform limits

## Platform-Specific Implementation Details

### Windows Implementation

1. **Long Path Support**:

```csharp
// Windows long path handling
public override string NormalizePath(string path)
{
    // Convert to extended path format for paths over MAX_PATH
    if (path.Length >= 260 && !path.StartsWith(@"\\?\"))
    {
        path = @"\\?\" + Path.GetFullPath(path);
    }
    return path;
}
```

2. **File Picker Integration**:

```csharp
// Windows file picker implementation
public override async Task<string> PickFileAsync(string title, string filter)
{
    var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
    filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

    // Convert filter string from common format to Windows format
    string[] filterParts = filter.Split('|');
    for (int i = 1; i < filterParts.Length; i += 2)
    {
        string extension = filterParts[i].Trim();
        if (extension.StartsWith("*."))
        {
            extension = extension.Substring(1);
        }
        filePicker.FileTypeFilter.Add(extension);
    }

    // Initialize with window handle
    WinRT.Interop.InitializeWithWindow.Initialize(filePicker, GetWindowHandle());

    var file = await filePicker.PickSingleFileAsync();
    return file?.Path ?? string.Empty;
}

private IntPtr GetWindowHandle()
{
    var window = Application.Current.Windows[0];
    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
    return handle;
}
```

3. **Balatro Save Location Detection**:

```csharp
// Windows Balatro save location
public override async Task<string> GetBalatroSaveDirectoryAsync()
{
    // Standard location
    string defaultPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Balatro"
    );

    // Check for Steam location
    string steamPath = GetSteamPath();
    if (!string.IsNullOrEmpty(steamPath))
    {
        string steamDataPath = Path.Combine(
            steamPath,
            "userdata",
            GetSteamUserId(),
            "2379780", // Balatro Steam App ID
            "remote"
        );

        if (Directory.Exists(steamDataPath))
        {
            return steamDataPath;
        }
    }

    return defaultPath;
}

private string GetSteamPath()
{
    // Look up Steam installation path in registry
    using var key = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
    return key?.GetValue("SteamPath") as string;
}

private string GetSteamUserId()
{
    // Get the active Steam user ID
    // Implementation details omitted
    return "0"; // Default fallback
}
```

### macOS Implementation

1. **Application Support Directories**:

```csharp
// macOS application support directory
public override async Task<string> GetApplicationDataDirectoryAsync(string appName = null)
{
    string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    string appSupportDir = Path.Combine(homeDir, "Library", "Application Support");

    if (!string.IsNullOrEmpty(appName))
    {
        appSupportDir = Path.Combine(appSupportDir, appName);
        if (!Directory.Exists(appSupportDir))
        {
            Directory.CreateDirectory(appSupportDir);
        }
    }

    return appSupportDir;
}
```

2. **File Monitoring**:

```csharp
// macOS file monitoring considerations
public override async Task<string> StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*")
{
    // macOS uses FSEvents API behind the scenes
    // The standard FileSystemWatcher works on macOS, but has some limitations

    // Additional logic for macOS-specific monitoring
    // This is just a skeleton of what would be needed

    return await base.StartMonitoringAsync(path, includeSubdirectories, filter);
}
```

3. **Unicode Normalization**:

```csharp
// macOS Unicode handling
public override string NormalizePath(string path)
{
    // macOS uses NFD normalization form by default
    // Convert to NFD for file system operations
    return path.Normalize(NormalizationForm.FormD);
}
```

### Linux Implementation

1. **XDG Base Directories**:

```csharp
// Linux XDG directory support
public override async Task<string> GetApplicationDataDirectoryAsync(string appName = null)
{
    // Follow XDG Base Directory specification
    string xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
    string baseDir;

    if (string.IsNullOrEmpty(xdgDataHome))
    {
        // Default fallback
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        baseDir = Path.Combine(homeDir, ".local", "share");
    }
    else
    {
        baseDir = xdgDataHome;
    }

    if (!string.IsNullOrEmpty(appName))
    {
        baseDir = Path.Combine(baseDir, appName);
        if (!Directory.Exists(baseDir))
        {
            Directory.CreateDirectory(baseDir);
        }
    }

    return baseDir;
}
```

2. **File Permissions**:

```csharp
// Linux file permissions handling
protected virtual void EnsureFilePermissions(string path, FilePermissions permissions)
{
    // Set file permissions using P/Invoke to chmod
    // Implementation details omitted
}

public override async Task<bool> CreateFileAsync(string path)
{
    bool created = await base.CreateFileAsync(path);
    if (created)
    {
        // Set default permissions for created files
        EnsureFilePermissions(path, FilePermissions.OwnerReadWrite);
    }
    return created;
}
```

3. **Case-Sensitivity Handling**:

```csharp
// Linux case-sensitivity handling
public override async Task<bool> FileExistsAsync(string path)
{
    // On Linux, case matters
    // Standard File.Exists handles this correctly, but custom implementations
    // need to be case-sensitive
    return await base.FileExistsAsync(path);
}
```

## Error Handling Patterns

### Exception Wrapping

```csharp
public async Task<string> ReadTextAsync(string path, Encoding encoding = null)
{
    try
    {
        // Attempt to read the file
        if (!await FileExistsAsync(path))
        {
            throw new FileNotFoundException($"File not found: {path}", path);
        }

        return await File.ReadAllTextAsync(path, encoding ?? Encoding.UTF8);
    }
    catch (UnauthorizedAccessException ex)
    {
        // Convert to application-specific exception
        throw new FileAccessException(path, "Access denied reading file", ex);
    }
    catch (IOException ex)
    {
        // Determine the type of IO exception and convert accordingly
        if (ex.Message.Contains("used by another process"))
        {
            throw new FileAccessException(path, "File is locked by another process", ex);
        }
        throw new FileSystemException($"IO error reading file: {path}", ex);
    }
    catch (Exception ex) when (ex is not FileSystemException)
    {
        // Generic handler for unexpected exceptions
        throw new FileSystemException($"Unexpected error reading file: {path}", ex);
    }
}
```

### Retry Logic

```csharp
public async Task<string> ReadTextWithRetryAsync(string path, Encoding encoding = null, int maxRetries = 3)
{
    int attempts = 0;
    Exception lastException = null;

    while (attempts < maxRetries)
    {
        try
        {
            return await ReadTextAsync(path, encoding);
        }
        catch (FileAccessException ex) when (IsTransient(ex))
        {
            // Only retry for transient errors
            lastException = ex;
            await Task.Delay(100 * (attempts + 1)); // Exponential backoff
        }
        catch (Exception ex)
        {
            // Don't retry for non-transient errors
            throw;
        }

        attempts++;
    }

    throw new FileSystemException($"Failed to read file after {maxRetries} attempts: {path}", lastException);
}

private bool IsTransient(Exception ex)
{
    // Determine if an exception is likely transient (could succeed on retry)
    return ex.Message.Contains("used by another process") ||
           ex.Message.Contains("network") ||
           ex.Message.Contains("timeout");
}
```

## Testing Strategies

### Mock File System for Unit Tests

Create a mock implementation of `IFileSystemService` for unit testing:

```csharp
public class MockFileSystemService : IFileSystemService
{
    private readonly Dictionary<string, byte[]> _files = new();
    private readonly Dictionary<string, HashSet<string>> _directories = new();

    // Simple in-memory implementation
    public Task<bool> FileExistsAsync(string path)
    {
        return Task.FromResult(_files.ContainsKey(NormalizePath(path)));
    }

    public Task<string> ReadTextAsync(string path, Encoding encoding = null)
    {
        path = NormalizePath(path);
        if (!_files.TryGetValue(path, out byte[] data))
        {
            throw new FileNotFoundException($"File not found: {path}", path);
        }

        encoding ??= Encoding.UTF8;
        return Task.FromResult(encoding.GetString(data));
    }

    public Task WriteTextAsync(string path, string content, Encoding encoding = null, bool overwrite = true)
    {
        path = NormalizePath(path);

        if (_files.ContainsKey(path) && !overwrite)
        {
            throw new FileAlreadyExistsException(path, "File already exists");
        }

        encoding ??= Encoding.UTF8;
        _files[path] = encoding.GetBytes(content);

        // Ensure directory exists in our mock
        string directory = GetDirectoryName(path);
        EnsureDirectoryExists(directory);

        return Task.CompletedTask;
    }

    // Implement other methods...

    private string NormalizePath(string path)
    {
        // Normalize path for consistent dictionary lookups
        return path.Replace('\\', '/').TrimEnd('/');
    }

    private void EnsureDirectoryExists(string directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            return;
        }

        directory = NormalizePath(directory);

        // Create parent directories first
        string parent = GetDirectoryName(directory);
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureDirectoryExists(parent);
        }

        // Create this directory
        if (!_directories.TryGetValue(parent ?? "", out var children))
        {
            children = new HashSet<string>();
            _directories[parent ?? ""] = children;
        }

        string dirName = GetFileName(directory);
        if (!string.IsNullOrEmpty(dirName))
        {
            children.Add(dirName);
        }

        // Ensure this directory has a children collection
        if (!_directories.ContainsKey(directory))
        {
            _directories[directory] = new HashSet<string>();
        }
    }
}
```

### Integration Testing

For integration tests, use real file system operations in isolated test directories:

```csharp
[TestClass]
public class FileSystemServiceIntegrationTests
{
    private IFileSystemService _fileSystem;
    private string _testDirectory;

    [TestInitialize]
    public void Setup()
    {
        // Use the real implementation
        _fileSystem = new BaseFileSystemService(
            new MockLogService(),
            new MockErrorHandlingService()
        );

        // Create a unique test directory
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "FileSystemTests",
            Guid.NewGuid().ToString()
        );
        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up test directory
        try
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [TestMethod]
    public async Task WriteAndReadText_ReturnsCorrectContent()
    {
        // Arrange
        string testPath = Path.Combine(_testDirectory, "test.txt");
        string content = "Hello, world!";

        // Act
        await _fileSystem.WriteTextAsync(testPath, content);
        string result = await _fileSystem.ReadTextAsync(testPath);

        // Assert
        Assert.AreEqual(content, result);
    }

    // More integration tests...
}
```

## Common Pitfalls and Solutions

### Path Handling Issues

1. **Problem**: Inconsistent path separators across platforms
   **Solution**: Always use `Path.Combine` or the service's `CombinePaths` method

2. **Problem**: Long paths on Windows
   **Solution**: Use extended path prefix (`\\?\`) for paths > 260 characters

3. **Problem**: Case sensitivity differences
   **Solution**: Always compare paths using `StringComparison.OrdinalIgnoreCase` on Windows/macOS

### File Locking

1. **Problem**: Files remain locked after use
   **Solution**: Always use `using` statements for disposable resources

2. **Problem**: Concurrent access to the same file
   **Solution**: Implement file locking strategy with retry mechanism

3. **Problem**: Cannot delete or move files in use
   **Solution**: Force garbage collection or delay operation until file is available

### Performance Issues

1. **Problem**: Slow sequential file operations
   **Solution**: Use parallelization for independent operations

2. **Problem**: Memory pressure with large files
   **Solution**: Use streaming APIs instead of loading entire files into memory

3. **Problem**: Excessive disk I/O
   **Solution**: Implement basic caching for frequently accessed files

## Conclusion

Implementing `IFileSystemService` correctly requires careful attention to platform differences, error handling, and performance considerations. By following these guidelines, you can create a robust file system service that works consistently across Windows, macOS, and Linux platforms.
