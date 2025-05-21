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
- [x] Implement a SaveContentView with appropriate UI elements (basic layout created)
- [ ] Add save file parsing functionality to extract meaningful information
- [ ] Display key game statistics from the save file (e.g., coins, jokers, cards)
- [x] Implement navigation to allow users to view save content from the DashboardView
- [ ] Add validation to ensure invalid save files are handled gracefully

## Dependencies

- IFileSystemService implementation
- DashboardViewModel

## Implementation Progress

- Created SaveContentViewModel.cs with basic properties and commands
- Created SaveContentView.axaml with UI for displaying save content
- Created SaveContentView.axaml.cs code-behind file
- Integrated navigation from DashboardView to SaveContentViewModel
- Added DoubleTapped event handler in DashboardView for file selection
- Registered SaveContentView in App.axaml.cs for routing

## Remaining Work

- Implement actual file content loading using IFileSystemService
- Parse save file content to extract structured game data
- Enhance UI to display game statistics in a more readable format
- Add search and copy functionality for content exploration
- Implement error handling for invalid save files

## Notes

The SaveContentViewer will enhance the application by providing insight into save files before restoration. This helps users make informed decisions and provides a more comprehensive save management experience.
