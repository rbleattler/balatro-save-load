# Task: Save Content Viewer Implementation

**ID**: TSK046
**Type**: Task
**Status**: In Progress
**Parent**: US001
**Created**: 2025-05-20

## Description

Implement a Save Content Viewer that allows users to see and explore the contents of their save files. This will help users understand their game progress and make informed decisions about which saves to restore.

## Acceptance Criteria

- [x] Create a SaveContentViewModel to manage the save content viewing (basic structure implemented)
- [x] Implement a SaveContentView with appropriate UI elements
- [x] Add save file parsing functionality to extract meaningful information (moved to TSK048)
- [x] Display key game statistics from the save file (e.g., coins, jokers, cards) (moved to TSK048 and completed)
- [x] Implement navigation to allow users to view save content from the DashboardView
- [x] Add validation to ensure invalid save files are handled gracefully

## Dependencies

- IFileSystemService implementation
- DashboardViewModel

## Implementation Progress

- Created SaveContentViewModel.cs with enhanced properties and commands
- Enhanced SaveContentView.axaml with improved UI for displaying save content
- Implemented SaveContentView.axaml.cs code-behind file with additional functionality
- Added clipboard functionality for copying content
- Added search functionality for finding text in content
- Added save-as functionality for exporting content
- Added dynamic status information
- Added font size adjustment options
- Added error handling for invalid save files
- Integrated navigation from DashboardView to SaveContentViewModel
- Added DoubleTapped event handler in DashboardView for file selection
- Registered SaveContentView in App.axaml.cs for routing

## Remaining Work

- ✅ Complete integration with TSK048 (Game Statistics Extraction)
  - ✅ SaveFileParser class created and implemented in Core project
  - ✅ Integrated parser with SaveContentViewModel
  - ✅ Created UI components for displaying structured game data
  - ✅ Added toggle functionality to switch between raw content and statistics views
- Add automated tests for the save content viewer
- Add documentation for the feature
- Test on various save file formats and edge cases

## Notes

The SaveContentViewer will enhance the application by providing insight into save files before restoration. This helps users make informed decisions and provides a more comprehensive save management experience.
