# Task: TSK016 - Create macOS Path Provider Implementation

## Parent User Story

- [US005 - Create Platform-Specific File Path Providers](US005-Create-Platform-Specific-File-Path-Providers.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

4

## Description

Create a macOS-specific implementation of the file path provider that correctly handles macOS file paths, special folders, and Balatro save locations on macOS systems.

## Steps

1. Create a MacPathProvider class that implements IPathProvider:
   - Implement methods to resolve macOS-specific paths for Balatro saves
   - Handle macOS-specific path conventions (forward slashes, hidden files)
   - Use macOS APIs or MAUI's platform-specific APIs for special folder resolution
   - Implement proper Unicode path handling with NFD normalization

2. Implement macOS-specific save file location detection:
   - Locate Balatro save files in ~/Library/Application Support/Balatro
   - Handle Steam and non-Steam installations
   - Handle case sensitivity considerations

3. Implement application folder management:
   - Create application data folder in appropriate macOS location
   - Follow macOS application data conventions
   - Implement logging directory structure according to macOS standards

4. Add proper error handling:
   - File access permissions
   - Missing directories
   - Resource fork and extended attribute handling if needed

## Acceptance Criteria

- MacPathProvider correctly resolves all paths on macOS systems
- Save file locations are correctly detected for different installation scenarios
- Application data paths follow macOS conventions
- Unicode characters are handled correctly with proper normalization
- All path operations include proper error handling and logging
- Implementation works on different macOS versions

## Dependencies

- TSK012: Create IFileSystemService interface with core file operations

## Notes

macOS path handling has several unique aspects, including the use of the Library folder, Unicode normalization differences, and resource forks. The implementation should handle all these macOS-specific aspects while presenting a consistent API to the rest of the application.
