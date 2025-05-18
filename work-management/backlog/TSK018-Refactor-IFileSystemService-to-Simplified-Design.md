# Task: TSK018 - Refactor IFileSystemService to a Simplified Design

## Parent User Story

- [US004 - Implement Simplified IFileSystemService Interface](../open/US004-Implement-IFileSystemService-Interface.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

3

## Description

Refactor the IFileSystemService interface to a simplified design that leverages .NET MAUI's built-in file system helpers rather than reimplementing common file operations. The refactored service should focus on platform-specific needs while deferring to standard .NET APIs for common operations.

## Steps

1. Analyze the current IFileSystemService interface and implementation to identify:
   - Methods that duplicate standard .NET File/Directory APIs
   - Platform-specific methods that need to be retained
   - MAUI FileSystem features that can be leveraged

2. Create a simplified IFileSystemService interface that:
   - Contains only platform-specific methods or methods that provide significant value over standard APIs
   - Uses standard .NET exceptions rather than custom exception types
   - Removes methods that duplicate MAUI's FileSystem functionality

3. Implement a streamlined BaseFileSystemService that:
   - Uses MAUI's FileSystem.Current for standard directories
   - Provides platform detection and routing for platform-specific operations
   - Is significantly smaller and more maintainable than the current implementation

4. Create extension methods (optional) for convenience functions that wrap standard .NET APIs

5. Update documentation to reflect the simplified approach

## Acceptance Criteria

- Simplified IFileSystemService interface with ~10 focused methods
- BaseFileSystemService implementation that uses MAUI's FileSystem APIs
- No custom exception types in the new implementation
- Maintained support for Balatro save directory resolution
- Maintained support for file monitoring (if required)
- Documentation updated to reflect new approach

## Dependencies

- Completed tasks TSK012-014

## Notes

This refactoring represents a significant shift in approach, moving from a comprehensive custom file system service to a focused service that leverages MAUI's built-in capabilities. Since the interface does not appear to be in use throughout the application yet, this is the ideal time to make this change before platform-specific implementations are created.
