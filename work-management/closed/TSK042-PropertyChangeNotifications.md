# Task: Property Change Notifications for Derived Properties

**ID**: TSK042
**Type**: Task
**Status**: Closed
**Parent**: US001
**Created**: 2025-05-20
**Completed**: 2025-05-20

## Description

Enhance view models with proper property change notifications for derived properties using ReactiveUI's WhenAnyValue and ToProperty pattern. This will ensure that UI elements properly update when related properties change.

## Acceptance Criteria

- [x] Update SaveFileViewModel to use ReactiveUI's WhenAnyValue for derived properties
- [x] Implement ToProperty pattern for derived properties
- [x] Ensure DisplayName updates when ProfileNumber or Timestamp changes
- [x] Fix nullable reference warnings in view models
- [x] Follow ReactiveUI best practices for observable properties
- [x] Make classes internal when appropriate to match project architecture

## Implementation Details

- Updated SaveFileViewModel with ReactiveUI property notifications
- Used `WhenAnyValue` and `ToProperty` pattern for derived properties
- Fixed nullable reference warnings with proper null checking
- Made classes internal to match project architecture
- Ensured DisplayName updates automatically when source properties change

## Related Files

- `BalatroSaveToolkit\ViewModels\SaveFileViewModel.cs`

## Notes

The ReactiveUI property change notification pattern provides a more robust approach to property updates compared to manual notification raising. This ensures that the UI stays in sync with the view model data.
