# User Story: US004 - Implement Simplified IFileSystemService Interface

## Parent Feature

- [FT002 - File System](FT002-File-System.md)

## Priority

High

## Estimate

5 hours

## Description

As a developer, I need a simplified and focused cross-platform file system service that leverages .NET MAUI's built-in file system capabilities while handling platform-specific operations required by the application. This service should focus on platform-specific paths and operations not covered by MAUI's FileSystem APIs, rather than reimplementing standard file operations.

## Acceptance Criteria

- A simplified IFileSystemService interface that complements MAUI's built-in FileSystem APIs
- The implementation works across all target platforms (Windows, macOS, Linux)
- The service properly locates Balatro save directories across platforms
- The service integrates with MAUI's FileSystem.Current for common directories
- Standard file operations use .NET APIs rather than custom implementations
- Platform-specific operations are properly abstracted
- File monitoring is supported for save file watching

## Tasks

- [x] [TSK012 - Create IFileSystemService Interface](../closed/TSK012-Create-IFileSystemService-Interface.md)
- [x] [TSK013 - Create Base FileSystemService Implementation](../closed/TSK013-Create-Base-FileSystemService-Implementation.md)
- [x] [TSK014 - Document Interface Method Behaviors](../closed/TSK014-Document-Interface-Method-Behaviors.md)
- [ ] [TSK018 - Refactor IFileSystemService to a Simplified Design](../backlog/TSK018-Refactor-IFileSystemService-to-Simplified-Design.md)
- [ ] [TSK015 - Implement Platform-Specific Path Resolution](../backlog/TSK015-Implement-Platform-Specific-Path-Resolution.md)
- [ ] [TSK017 - Create Tests for FileSystemService](../backlog/TSK017-Create-Tests-for-FileSystemService.md)

## Notes

- The implementation should leverage MAUI's built-in FileSystem APIs rather than reimplementing them
- Standard file operations should use the .NET File and Directory classes directly
- Focus on platform-specific concerns like save game locations and monitoring
- Consider extension methods to provide additional utility functions while keeping the interface clean
- The service should integrate with the logging system for debugging and error reporting

## Status

In Progress - 3 of 6 tasks completed