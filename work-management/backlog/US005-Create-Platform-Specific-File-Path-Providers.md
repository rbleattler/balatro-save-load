# User Story: US005 - Create Platform-Specific File Path Providers

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

- TSK015: Create Windows path provider implementation
- TSK016: Create macOS path provider implementation
- TSK017: Create Linux path provider implementation
- TSK018: Create path provider unit tests

## Technical Notes

The platform-specific path providers need to account for:
- Different save file locations for Balatro on each platform:
  - Windows: %USERPROFILE%/AppData/Local/Balatro
  - macOS: ~/Library/Application Support/Balatro
  - Linux: ~/.local/share/Balatro
- Different path separators and naming conventions
- Platform API differences for resolving special folders

## Dependencies

- US004: Implement IFileSystemService Interface

## Testing

Tests should verify that:
- Providers correctly resolve platform-specific paths
- Error handling works for missing or inaccessible paths
- Special folder resolution works correctly for each platform

## Implementation Notes

Implementations should use FileSystemService for actual file operations but provide platform-specific path resolution and management.
