# Feature: FT002 - Cross-Platform File System Implementation

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

High (1) - Core functionality required for save file operations

## Description

Create a platform-agnostic file system service that handles file operations consistently across Windows, macOS, and Linux. This feature will replace the Windows-specific file path handling in the current WPF implementation.

## User Stories

- [In Progress] US004: Implement IFileSystemService Interface (3/6 tasks completed)
- [In Progress] US005: Create Platform-Specific File Path Providers (1/4 tasks completed)
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
- [x] TSK014: Document Interface Method Behaviors - COMPLETED
- [x] TSK015: Create Windows Path Provider implementation - COMPLETED
- [ ] TSK016: Create macOS path provider implementation
- [ ] TSK017: Create Linux path provider implementation
- [ ] TSK018: Create path provider unit tests
- [ ] TSK019: Implement macOS FileSystemService
- [ ] TSK020: Implement Linux FileSystemService
- [ ] TSK021: Create tests for FileSystemService
- [ ] TSK022: Create file monitoring service
- [ ] TSK023: Handle file permissions and access errors
