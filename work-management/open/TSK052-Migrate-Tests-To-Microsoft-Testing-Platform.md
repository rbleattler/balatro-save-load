# TSK052: Migrate Tests to Microsoft.Testing.Platform

**ID**: TSK052
**Type**: Task
**Status**: Open
**Parent**: US001
**Created**: 2025-01-27
**Started**: 2025-01-27

## Description

Migrate the test project from xunit to Microsoft.Testing.Platform framework using MSTest to provide a more modern testing experience with better integration and performance.

## Acceptance Criteria

- [ ] Update test project file to use MSTest SDK
- [ ] Enable Microsoft.Testing.Platform features
- [ ] Convert all xunit tests to MSTest format
- [ ] Update test attributes ([Fact] → [TestMethod])
- [ ] Update Assert statements to MSTest format
- [ ] Update using statements and namespaces
- [ ] Ensure all tests compile and run successfully
- [ ] Verify test discovery and execution works in VS Code

## Steps

1. Update `BalatroSaveToolkit.Tests.csproj` to use MSTest SDK
2. Remove xunit package references
3. Add MSTest package references and enable Microsoft.Testing.Platform
4. Convert test files:
   - `DashboardViewModelTests.cs`
   - `SaveFileViewModelTests.cs`
   - `MockFileSystemServiceTests.cs`
5. Update using statements and test attributes
6. Convert Assert statements to MSTest format
7. Test compilation and execution

## Dependencies

- All existing test implementations

## Priority

Medium - Improves testing infrastructure

## Notes

- Using MSTest SDK for simplified configuration
- Microsoft.Testing.Platform provides better performance and integration
- This aligns with Microsoft's current testing recommendations
