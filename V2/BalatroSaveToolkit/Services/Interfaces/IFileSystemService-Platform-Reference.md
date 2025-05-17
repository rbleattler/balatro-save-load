# IFileSystemService - Platform-Specific Behavior Reference

## Overview

This document details the platform-specific behaviors and differences when implementing the `IFileSystemService` interface across Windows, macOS, and Linux. Understanding these differences is critical for developing a truly cross-platform file system abstraction.

## File System Fundamentals

### Path Separators

| Platform | Primary Separator | Also Supports | Notes |
|----------|------------------|--------------|-------|
| Windows  | Backslash (`\`)  | Forward slash (`/`) | Windows API converts forward slashes internally |
| macOS    | Forward slash (`/`) | None | Historical BSD Unix-based system |
| Linux    | Forward slash (`/`) | None | POSIX-compliant |

### Root Directory Structure

| Platform | Root Format | Example Absolute Path | Notes |
|----------|------------|---------------------|-------|
| Windows  | Drive letter + colon | `C:\Program Files\App\file.txt` | Drive letters are not case-sensitive |
| Windows (UNC) | `\\server\share` | `\\server\share\folder\file.txt` | Network paths use UNC format |
| macOS    | Forward slash | `/Applications/App.app/Contents/file.txt` | Single root hierarchy |
| Linux    | Forward slash | `/usr/share/app/file.txt` | Single root hierarchy |

### Case Sensitivity

| Platform | Case-Sensitive | Notes |
|----------|---------------|-------|
| Windows  | No (case-preserving) | File operations are case-insensitive but preserve case |
| macOS    | Mostly No* | Default is case-insensitive but case-preserving, can be case-sensitive if formatted as such |
| Linux    | Yes | File operations are strictly case-sensitive |

*macOS APFS can be formatted as case-sensitive, but the default is case-insensitive. HFS+ behaves similarly.

### Path Length Limitations

| Platform | Max Path Length | Extended Support | Notes |
|----------|----------------|-----------------|-------|
| Windows  | 260 characters | `\\?\` prefix allows ~32,767 | MAX_PATH limitation in standard APIs |
| macOS    | 1024 characters | N/A | PATH_MAX constant (set to 1024) |
| Linux    | 4096+ characters | N/A | Varies by distro and filesystem type |

### Character Encoding

| Platform | File Name Encoding | Illegal Characters | Notes |
|----------|-------------------|-------------------|-------|
| Windows  | UTF-16 (internally) | `< > : " / \ | ? *` and control chars | Cannot use reserved device names (CON, PRN, etc.) |
| macOS    | UTF-8 (NFD form) | `/` and NUL | Uses Unicode normalization form D by default |
| Linux    | No restrictions* | `/` and NUL | Encoding depends on locale, usually UTF-8 |

*Linux technically allows any byte sequence except `/` and NUL, but utilities and applications may have restrictions.

## Directory Structure

### Standard Directories

| Purpose | Windows | macOS | Linux | Environment Variable |
|---------|----------|-------|-------|---------------------|
| User profile | `C:\Users\username` | `/Users/username` | `/home/username` | `%USERPROFILE%` / `$HOME` |
| App data | `C:\Users\username\AppData\Roaming` | `/Users/username/Library/Application Support` | `/home/username/.config` | `%APPDATA%` / `$XDG_CONFIG_HOME` |
| Local app data | `C:\Users\username\AppData\Local` | `/Users/username/Library/Application Support` | `/home/username/.local/share` | `%LOCALAPPDATA%` / `$XDG_DATA_HOME` |
| Temp directory | `C:\Users\username\AppData\Local\Temp` | `/var/folders/XX/XXXXXXX/T` or `/tmp` | `/tmp` | `%TEMP%` / `$TMPDIR` |
| Program files | `C:\Program Files` | `/Applications` | `/usr/bin` or `/usr/local/bin` | N/A |

### Balatro Save Locations

| Platform | Default Location | Steam Location | Notes |
|----------|-----------------|---------------|-------|
| Windows | `%LOCALAPPDATA%\Balatro` | `[Steam]\userdata\[SteamID]\2379780\remote` | Steam cloud saves |
| macOS | `~/Library/Application Support/Balatro` | `~/Library/Application Support/Steam/userdata/[SteamID]/2379780/remote` | Steam cloud saves |
| Linux | `~/.local/share/Balatro` | `~/.steam/steam/userdata/[SteamID]/2379780/remote` | Steam location varies by distro |

### Hidden Files and Directories

| Platform | Hidden Format | Notes |
|----------|--------------|-------|
| Windows | File attribute | Set via `FILE_ATTRIBUTE_HIDDEN` |
| macOS | Prefix with `.` | By convention, Finder hides these files |
| Linux | Prefix with `.` | By convention, not a filesystem attribute |

## File Operations

### File Locking Behavior

| Platform | Locking Model | Notes |
|----------|--------------|-------|
| Windows | Mandatory locking | OS enforces locks, processes cannot access locked regions |
| macOS | Advisory locking | Processes must cooperate with locks, not enforced by OS |
| Linux | Advisory locking | Processes must cooperate with locks, not enforced by OS |

### Symbolic Links and Hard Links

| Platform | Symlink Command | Hard Link Command | Notes |
|----------|----------------|------------------|-------|
| Windows | `mklink link target` | `mklink /h link target` | Requires admin rights or developer mode |
| macOS | `ln -s target link` | `ln target link` | No special permissions needed |
| Linux | `ln -s target link` | `ln target link` | No special permissions needed |

### File Attributes

| Attribute | Windows | macOS | Linux | Notes |
|-----------|---------|-------|-------|-------|
| Creation time | Supported | Supported | Not universally supported | Linux often only has mtime and ctime |
| Read-only | Attribute | chmod | chmod | Different implementation methods |
| Hidden | Attribute | Filename convention | Filename convention | Windows uses attributes, Unix uses `.` prefix |
| System | Attribute | N/A | N/A | Windows-specific |
| Archive | Attribute | N/A | N/A | Windows-specific |
| Executable | N/A | chmod +x | chmod +x | Unix permissions model |

### Permissions Model

| Platform | Permission Model | Notes |
|----------|-----------------|-------|
| Windows | Access Control Lists (ACLs) | Complex permission system with inheritance |
| macOS | POSIX permissions + ACLs | User, group, others + extended ACLs |
| Linux | POSIX permissions | User, group, others (rwx bits) |

## File Monitoring

### Change Detection Methods

| Platform | Primary API | Capabilities | Limitations |
|----------|------------|--------------|------------|
| Windows | ReadDirectoryChangesW | Detailed change notifications | Limited number of watches |
| macOS | FSEvents | Volume-level event stream | Less granular than Windows |
| Linux | inotify | File-level notifications | Watch descriptor limits |

### Monitoring Behavior Differences

| Behavior | Windows | macOS | Linux |
|----------|---------|-------|-------|
| Subdirectory changes | Can watch recursively | Built-in recursive watching | Must add watches for each subdirectory |
| Network paths | Limited support | Partial support | Generally not supported |
| Resource usage | Higher at scale | Efficient even at scale | Watch limits (configurable) |
| Event coalescing | Some coalescing | Heavy coalescing | Minimal coalescing |
| Event details | Detailed information | Less detailed | Moderately detailed |

## UI Integration

### File Pickers

| Platform | Native API | Filter Format | Notes |
|----------|-----------|--------------|-------|
| Windows | Windows.Storage.Pickers | `.ext` | Modern UI via WinRT |
| macOS | NSOpenPanel | `com.example.extension` | UTIs preferred over extensions |
| Linux | GTK FileChooserDialog | `*.ext` | Varies by desktop environment |

### Filter Expression Examples

| Description | Windows | macOS | Linux |
|-------------|---------|-------|-------|
| Text files | `*.txt` | `public.plain-text` | `*.txt` |
| Images | `*.jpg;*.png;*.gif` | `public.image` | `*.jpg *.png *.gif` |
| All files | `*.*` | `public.item` | `*` |

## Performance Considerations

### Disk I/O Characteristics

| Platform | Default FS | Cache Behavior | Notes |
|----------|-----------|---------------|-------|
| Windows | NTFS/ReFS | Aggressive caching | Large cache with lazy write |
| macOS | APFS | Balanced caching | Less aggressive than Windows |
| Linux | ext4/XFS/others | Configurable | Tunable via system settings |

### Network File System Performance

| Platform | Network Performance | Notes |
|----------|---------------------|-------|
| Windows | Good SMB performance | Native protocol, optimized |
| macOS | Good SMB, AFP | SMB preferred on modern macOS |
| Linux | Good NFS, moderate SMB | Best with NFS |

## Error Handling

### Common Error Patterns

| Error Scenario | Windows | macOS | Linux |
|----------------|---------|-------|-------|
| File in use | `ERROR_SHARING_VIOLATION` (32) | `EBUSY` or `ETXTBSY` | `EBUSY` |
| No permission | `ERROR_ACCESS_DENIED` (5) | `EACCES` | `EACCES` |
| Not found | `ERROR_FILE_NOT_FOUND` (2) | `ENOENT` | `ENOENT` |
| Path too long | `ERROR_PATH_NOT_FOUND` (3) | `ENAMETOOLONG` | `ENAMETOOLONG` |
| Disk full | `ERROR_DISK_FULL` (112) | `ENOSPC` | `ENOSPC` |

### Retry Recommendations

| Error Type | Should Retry? | Retry Strategy | Notes |
|------------|--------------|---------------|-------|
| File in use | Yes | Exponential backoff | May be temporary if file is being processed |
| Permission denied | No | N/A | Permission issues rarely resolve themselves |
| Not found | Conditional | Single retry | Only if file might be created during operation |
| Network errors | Yes | Exponential backoff | Temporary network issues often resolve |
| Disk full | No | N/A | Requires user intervention |

## Platform Detection

### Detection Methods

For proper platform detection in .NET MAUI, use the following:

```csharp
// Platform detection in .NET MAUI
if (OperatingSystem.IsWindows())
{
    // Windows-specific code
}
else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
{
    // macOS-specific code
}
else if (OperatingSystem.IsLinux())
{
    // Linux-specific code
}
else
{
    // Unsupported platform
    throw new PlatformNotSupportedException("Current platform not supported");
}
```

### Feature Detection

For platform-specific feature detection:

```csharp
// Check if extended path support is available
public bool SupportsExtendedPaths()
{
    return OperatingSystem.IsWindows() && Environment.OSVersion.Version >= new Version(10, 0, 1607);
}

// Check if file system is case-sensitive
public bool IsFileSystemCaseSensitive()
{
    if (OperatingSystem.IsLinux())
    {
        return true; // Linux is always case-sensitive
    }
    else if (OperatingSystem.IsMacOS())
    {
        // macOS might be case-sensitive depending on filesystem
        // This is a simplified check
        try
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "CaseTest.txt");
            File.WriteAllText(tempFile, "test");
            bool exists = File.Exists(tempFile.ToLower());
            File.Delete(tempFile);
            return !exists;
        }
        catch
        {
            return false; // Default to case-insensitive
        }
    }

    return false; // Windows is not case-sensitive
}
```

## Appendix: Platform-Specific Gotchas

### Windows

1. **Reparse Points**: Special directory entries that can cause recursive traversal issues
2. **8.3 Filenames**: Short name aliases for long filenames (legacy DOS support)
3. **Reserved Names**: Cannot use CON, PRN, AUX, NUL, etc. as filenames
4. **Alternate Data Streams**: Files can have hidden streams not visible in normal listings
5. **Junction Points**: Similar to symlinks but only for directories and more limited

### macOS

1. **Resource Forks**: Historical feature where files contain separate data and resource sections
2. **Bundle Structures**: Application bundles appear as files but are directories
3. **Unicode Normalization**: macOS uses NFD normalization which can cause comparison issues
4. **Extended Attributes**: Files can have extended attributes not visible in normal listings
5. **Quarantine Flags**: Security feature that marks files downloaded from the internet

### Linux

1. **Mount Points**: Directories can be mountpoints for different filesystems
2. **Special Files**: Everything is a file, including devices and sockets
3. **File Permissions**: Execute permission is needed for directories to be traversable
4. **Filesystem Variety**: Many different filesystems with different capabilities
5. **Symbolic Links**: Symlinks not followed across mount boundaries unless configured

## Reference Implementation

Each platform needs a specialized implementation of `IFileSystemService`. The key methods that usually require platform-specific implementation are:

1. `GetBalatroSaveDirectoryAsync()`
2. `GetApplicationDataDirectoryAsync(string? appName = null)`
3. `PickFileAsync(string title, string filter)`
4. `PickFolderAsync(string title)`
5. `PickSaveFileAsync(string title, string suggestedName, string filter)`
6. `StartMonitoringAsync(string path, bool includeSubdirectories = false, string filter = "*")`

For all other methods, the base implementation can often be used across platforms with minimal adjustments.
