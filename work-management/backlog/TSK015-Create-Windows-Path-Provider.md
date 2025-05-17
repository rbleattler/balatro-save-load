# Task: TSK015 - Create Windows Path Provider Implementation

## Parent User Story

- [US005 - Create Platform-Specific File Path Providers](US005-Create-Platform-Specific-File-Path-Providers.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

4

## Description

Create a Windows-specific implementation of the file path provider that correctly handles Windows file paths, special folders, and Balatro save locations on Windows systems.

## Steps

1. Create a WindowsPathProvider class that implements IPathProvider:
   - Implement methods to resolve Windows-specific paths for Balatro saves
   - Handle Windows-specific path conventions (backslashes, drive letters)
   - Use Windows APIs or .NET's Environment.SpecialFolder for special folder resolution
   - Implement proper Unicode path handling

2. Implement Windows-specific save file location detection:
   - Locate Balatro save files in %USERPROFILE%/AppData/Local/Balatro
   - Handle Steam and non-Steam installations
   - Handle cases where the game may be installed on different drives

3. Implement application folder management:
   - Create application data folder in appropriate Windows location
   - Handle roaming vs. local application data
   - Implement logging directory structure

4. Add proper error handling:
   - File access permissions
   - Missing directories
   - Long path handling (>260 characters)

## Acceptance Criteria

- WindowsPathProvider correctly resolves all paths on Windows systems
- Save file locations are correctly detected for different installation scenarios
- Application data paths follow Windows conventions
- Long paths and Unicode characters are handled correctly
- All path operations include proper error handling and logging
- Implementation works on different Windows versions (10, 11)

## Dependencies

- TSK012: Create IFileSystemService interface with core file operations

## Notes

Windows path handling has several quirks, including drive letters, backslash separators, and path length limitations. The implementation should handle all these Windows-specific aspects while presenting a consistent API to the rest of the application.
