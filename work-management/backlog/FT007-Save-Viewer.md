# Feature: FT007 - Save Data Viewer Implementation

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

Medium (2) - Enhances user experience but not critical for core functionality

## Description

Replace the current "JSON Viewer" (which actually displays Lua dictionary data) with an improved hierarchical viewer that allows easier navigation and understanding of save file contents.

## User Stories

- US027: Create Save Data Parser
- US028: Design Save Viewer UI
- US029: Implement Hierarchical Data Navigation
- US030: Add Search/Filter Functionality
- US031: Create Syntax Highlighting

## Technical Details

- Properly parse Lua dictionary format into navigable structure
- Create model for hierarchical data representation
- Implement tree view or expandable list for navigation
- Add search and filtering capabilities
- Implement basic syntax highlighting

## Dependencies

- FT001 - Project Setup and Core Architecture
- FT004 - Save File Management
- FT005 - UI Migration and Enhancement

## Current Implementation Issues

Current "JSON Viewer" is misnamed and has limited formatting:

```csharp
private string FormatLuaContent(string content)
{
    // Basic text formatting with simple rules
}
```

## Acceptance Criteria

- Save data is properly parsed into a navigable structure
- UI allows expanding/collapsing sections of data
- Renamed to properly reflect content (Save Data Viewer)
- Basic syntax highlighting improves readability
- Search/filter functionality works correctly
- Navigation is intuitive and user-friendly
- Large save files can be handled without performance issues

## Estimation

1 week

## Tasks

- TSK045: Create LuaDictionary parser
- TSK046: Implement hierarchical data model
- TSK047: Design SaveViewerPage UI
- TSK048: Create TreeView or expandable list component
- TSK049: Implement basic syntax highlighting
- TSK050: Add search/filter functionality
- TSK051: Create SaveViewerViewModel
- TSK052: Implement navigation between data sections
- TSK053: Optimize for performance with large files
