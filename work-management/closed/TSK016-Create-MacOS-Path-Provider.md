# Task: TSK016 - Create macOS Path Provider Implementation

## Parent User Story

- [US005 - Create Platform-Specific File Path Providers](../open/US005-Create-Platform-Specific-File-Path-Providers.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

4

## Status

Completed

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

## Implementation Details

Implemented a MacPathProvider class that:
- Properly resolves Balatro save locations on macOS (~/Library/Application Support/Balatro)
- Supports Steam installation detection via common installation paths
- Normalizes macOS paths and handles Unicode normalization (NFD)
- Provides macOS-specific application data folder handling
- Includes detection of resource forks and extended attributes
- Correctly handles macOS path conventions (forward slashes, tilde expansion)
- Provides robust error handling and logging

The implementation includes:
- Proper handling of macOS Library folder structure
- Steam game detection for both standard and Steam Cloud saves
- Unicode normalization for proper file path handling on macOS (NFD normalization)
- Path normalization for consistent handling
- Proper exception handling with fallbacks

## Notes

macOS path handling has several unique aspects, including the use of the Library folder, Unicode normalization differences, and resource forks. The implementation handles all these macOS-specific aspects while presenting a consistent API to the rest of the application.
