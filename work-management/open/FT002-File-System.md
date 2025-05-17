# Feature: FT002 - Cross-Platform File System Implementation

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

High (1) - Core functionality required for save file operations

## Description

Create a platform-agnostic file system service that handles file operations consistently across Windows, macOS, and Linux. This feature will replace the Windows-specific file path handling in the current WPF implementation.

## User Stories

- [In Progress] US004: Implement IFileSystemService Interface (2/6 tasks completed)
- US005: Create Platform-Specific File Path Providers
- US006: Implement File Operations Abstraction
- US007: Create File Monitoring Service

## Technical Details

- Use MAUI's FileSystem APIs for cross-platform compatibility
- Create platform-specific implementations for Windows, macOS, and Linux
- Handle different path structures for each platform
- Implement file watching capabilities

## Dependencies

- FT001 - Project Setup and Core Architecture

## Acceptance Criteria

- File operations work consistently across all supported platforms
- Balatro save file locations are correctly identified on each platform
- File monitoring works for detecting changes in save files
- Error handling provides clear feedback for file permission issues
- Unit tests confirm functionality on different platforms

## Estimation

1 week

## Tasks

- [x] TSK012: Create IFileSystemService interface - COMPLETED
- [x] TSK013: Create base FileSystemService implementation - COMPLETED
- [ ] TSK014: Implement Windows FileSystemService
- [ ] TSK015: Implement macOS FileSystemService
- [ ] TSK016: Implement Linux FileSystemService
- [ ] TSK017: Create tests for FileSystemService
- [ ] TSK018: Implement save file location detection for each platform
- [ ] TSK019: Create file monitoring service
- [ ] TSK020: Handle file permissions and access errors
