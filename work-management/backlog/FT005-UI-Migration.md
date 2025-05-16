# Feature: FT005 - UI Migration and Enhancement

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

High (1) - User-facing component critical for application usability

## Description

Migrate and enhance the user interface from WPF-specific components to MAUI's cross-platform UI, removing the custom title bar and implementing proper navigation between pages.

## User Stories

- US017: Create MainPage UI
- US018: Implement Save Profile Selection
- US019: Create Save/Load UI Controls
- US020: Implement Navigation System
- US021: Create App Shell Structure

## Technical Details

- Replace WPF-specific UI with MAUI components
- Remove custom title bar implementation in favor of native title bars
- Create AppShell for navigation between pages
- Redesign UI with proper MVVM binding
- Implement theme support (Light/Dark)

## Dependencies

- FT001 - Project Setup and Core Architecture
- FT004 - Save File Management

## Current Implementation Issues

Current implementation uses a custom title bar with non-standard controls:

```csharp
private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
    if (e.ClickCount == 2) {
        // Optional: maximize/restore on double-click
    } else {
        DragMove();
    }
}
```

## Acceptance Criteria

- UI works consistently across Windows, macOS, and Linux
- Main functionality (save/load/backup) is easily accessible
- Navigation between different views/pages is intuitive
- Theme support works correctly
- UI is responsive and follows MAUI design guidelines
- Custom title bar is replaced with native platform title bar
- UI elements properly update based on application state

## Estimation

1-2 weeks

## Tasks

- TSK027: Create AppShell.xaml with navigation structure
- TSK028: Implement MainPage UI with profile selection
- TSK029: Create save/load/backup controls
- TSK030: Implement ViewModel binding for UI elements
- TSK031: Create theme system with Light/Dark modes
- TSK032: Implement responsive layout for different devices
- TSK033: Create status display system
- TSK034: Implement file browser functionality
- TSK035: Create UI state management
