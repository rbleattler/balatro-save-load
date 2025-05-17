# User Story: US005 - Create Platform-Specific File Path Providers

**Status: Completed**

## Parent Feature

- [FT002 - Cross-Platform File System Implementation](../open/FT002-File-System.md)

## Priority

High (1) - Essential for accessing save files correctly on each platform

## Story Points

3

## Description

As a developer, I need to create platform-specific file path providers that can correctly locate and manage paths to Balatro save files and application data folders on Windows, macOS, and Linux, so that the application can find and manipulate save files regardless of the operating system.

## Acceptance Criteria

- Platform-specific file path providers are implemented for:
  - Windows
  - macOS
  - Linux
- Each provider correctly locates Balatro save file locations for its platform
- Each provider correctly resolves application data paths for storing configuration and logs
- Path providers handle Unicode paths correctly
- Providers use appropriate system APIs for path resolution
- Implementation includes proper error handling for missing or inaccessible paths

## Tasks

- ✅ TSK015: Create Windows path provider implementation (Completed)
- ✅ TSK016: Create macOS path provider implementation (Completed)
- ✅ TSK017: Create Linux path provider implementation (Completed)
- ✅ TSK018: Create path provider unit tests (Completed)

## Completion Summary

All four tasks have been successfully completed, providing a robust cross-platform path provider system:

1. **Windows Path Provider**: Implemented in `WindowsPathProvider.cs`, handling Windows-specific paths including AppData locations, Steam installations via registry, and long path support.

2. **macOS Path Provider**: Implemented in `MacPathProvider.cs`, supporting macOS-specific paths including Library folders, Application Support directories, Unicode normalization (NFD), and resource fork handling.

3. **Linux Path Provider**: Implemented in `LinuxPathProvider.cs`, providing Linux-specific path handling including XDG Base Directory support, Flatpak installations, Proton/Wine compatibility, and symbolic link resolution.

4. **Comprehensive Testing**: Created extensive unit tests using a custom file system mocking framework that allows testing all providers on any platform. Tests cover path normalization, special folder handling, error scenarios, and platform-specific behaviors.

The path provider system has been successfully integrated with dependency injection in `MauiProgram.cs`, enabling automatic platform detection and appropriate provider selection at runtime.

## Technical Notes

The platform-specific path providers account for:
- Different save file locations for Balatro on each platform:
  - Windows: %USERPROFILE%/AppData/Local/Balatro
  - macOS: ~/Library/Application Support/Balatro
  - Linux: ~/.local/share/Balatro
- Different path separators and naming conventions
- Platform API differences for resolving special folders
- Steam installation detection on all platforms
