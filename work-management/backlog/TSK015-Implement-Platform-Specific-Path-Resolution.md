# Task: TSK015 - Implement Platform-Specific Path Resolution

## Parent User Story

- [US004 - Implement Simplified IFileSystemService Interface](../open/US004-Implement-IFileSystemService-Interface.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

2

## Description

Implement the platform-specific path resolution features of the simplified IFileSystemService, focusing on locating game save directories across Windows, macOS, and Linux platforms.

## Steps

1. Research the save file locations for Balatro on each platform:
   - Windows
   - macOS
   - Linux

2. Implement the GetBalatroSaveDirectoryAsync method in BaseFileSystemService with proper platform detection:
   ```csharp
   public virtual async Task<string> GetBalatroSaveDirectoryAsync()
   {
       if (OperatingSystem.IsWindows())
       {
           // Windows-specific implementation
       }
       else if (OperatingSystem.IsMacOS())
       {
           // macOS-specific implementation
       }
       else if (OperatingSystem.IsLinux())
       {
           // Linux-specific implementation
       }
       else
       {
           throw new PlatformNotSupportedException("Current platform not supported for Balatro save location");
       }
   }
   ```

3. Implement any other platform-specific path resolution methods

4. Add unit tests for each platform's path resolution

## Acceptance Criteria

- Correct implementation of GetBalatroSaveDirectoryAsync for Windows, macOS, and Linux
- Proper platform detection using OperatingSystem static class
- Appropriate error handling for unsupported platforms
- Unit tests for each platform implementation

## Dependencies

- TSK018: Refactor IFileSystemService to a Simplified Design

## Notes

Focus on the platform-specific aspects rather than reimplementing functionality available in standard .NET APIs or MAUI's FileSystem. This task is about correctly locating Balatro save files on each supported platform.
