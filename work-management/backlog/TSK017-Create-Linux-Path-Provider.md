# Task: TSK017 - Create Linux Path Provider Implementation

## Parent User Story

- [US005 - Create Platform-Specific File Path Providers](US005-Create-Platform-Specific-File-Path-Providers.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

4

## Description

Create a Linux-specific implementation of the file path provider that correctly handles Linux file paths, special folders, and Balatro save locations on Linux systems.

## Steps

1. Create a LinuxPathProvider class that implements IPathProvider:
   - Implement methods to resolve Linux-specific paths for Balatro saves
   - Handle Linux-specific path conventions (forward slashes, hidden dot files)
   - Use appropriate methods for special folder resolution
   - Implement proper Unicode path handling

2. Implement Linux-specific save file location detection:
   - Locate Balatro save files in ~/.local/share/Balatro
   - Handle Steam and non-Steam installations (Steam may use ~/.steam/steam/userdata)
   - Account for different Linux distributions' variations

3. Implement application folder management:
   - Create application data folder in appropriate Linux location (~/.local/share/BalatroSaveToolkit)
   - Follow XDG Base Directory Specification
   - Implement logging directory structure

4. Add proper error handling:
   - File access permissions
   - Missing directories
   - Symbolic link handling

## Acceptance Criteria

- LinuxPathProvider correctly resolves all paths on Linux systems
- Save file locations are correctly detected for different installation scenarios
- Application data paths follow Linux/XDG conventions
- Unicode characters are handled correctly
- All path operations include proper error handling and logging
- Implementation works on different Linux distributions

## Dependencies

- TSK012: Create IFileSystemService interface with core file operations

## Notes

Linux path handling follows different conventions from Windows and macOS, particularly around hidden files and application data. The implementation should follow the XDG Base Directory Specification where appropriate and handle the variety of Linux distribution differences while presenting a consistent API to the rest of the application.
