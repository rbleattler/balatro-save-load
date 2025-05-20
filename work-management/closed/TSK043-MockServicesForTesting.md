# Task: Mock Services for Testing

**ID**: TSK043
**Type**: Task
**Status**: Closed
**Parent**: US001
**Created**: 2025-05-20
**Completed**: 2025-05-20

## Description

Create mock implementations of key services to facilitate unit testing of components that depend on these services. This includes mocks for file system operations, notifications, and other external dependencies.

## Acceptance Criteria

- [x] Create MockFileSystemService that simulates file operations without touching the real file system
- [x] Implement MockNotificationService for testing notification-dependent code
- [x] Ensure mock services implement the same interfaces as their real counterparts
- [x] Add configuration options to control mock behavior in tests
- [x] Provide detailed tracking of mock service interactions for test assertions
- [x] Create unit tests to validate the behavior of mock services themselves

## Implementation Details

- Created `MockFileSystemService` with an in-memory file system
- Implemented `MockNotificationService` that tracks notifications instead of displaying them
- Added tracking of method calls and parameters for test assertions
- Created configuration options to simulate various scenarios (errors, file existence, etc.)
- Added unit tests for the mock services to verify their behavior

## Related Files

- `BalatroSaveToolkit.Tests\Mocks\MockFileSystemService.cs`
- `BalatroSaveToolkit.Tests\Mocks\MockNotificationService.cs`
- `BalatroSaveToolkit.Tests\Services\MockFileSystemServiceTests.cs`

## Notes

Mock services are essential for proper unit testing of components that depend on external services. They allow tests to run quickly and reliably without requiring access to the real file system or UI components.
