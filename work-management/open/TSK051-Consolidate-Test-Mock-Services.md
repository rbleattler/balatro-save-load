# TSK051: Consolidate Test Mock Services

## Status

📋 **Open** - 2024-12-28

## Objective

Create a dedicated MockLoggingService in the Tests/Mocks folder to match the pattern used by other mock services, removing the inline implementation in DashboardViewModelTests.

## Background

Currently, `DashboardViewModelTests.cs` contains an inline `MockLoggingService` implementation, while other services like `MockFileSystemService` and `MockNotificationService` are properly organized in the `Tests/Mocks` folder.

## Tasks

1. Create `BalatroSaveToolkit.Tests/Mocks/MockLoggingService.cs` following the same pattern as other mock services
2. Update `DashboardViewModelTests.cs` to use the new dedicated mock service
3. Ensure the mock service includes tracking capabilities for test assertions

## Acceptance Criteria

- MockLoggingService is moved to Tests/Mocks folder
- Implementation follows the same pattern as other mock services
- DashboardViewModelTests uses the new mock service
- All tests continue to pass

## Priority

Low - Code organization improvement

## Estimated Effort

30 minutes

## Parent Work Item

TSK050: Consolidate Navigation Services (follow-up)

## Related Files

- `BalatroSaveToolkit.Tests/ViewModels/DashboardViewModelTests.cs` (to be updated)
- `BalatroSaveToolkit.Tests/Mocks/MockLoggingService.cs` (to be created)

## Dependencies

None

## Notes

This is a minor code organization improvement to maintain consistency in the testing infrastructure. While not critical, it helps maintain clean separation of concerns and follows the established patterns in the codebase.
