# Feature: FT004 - Save File Management

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

High (1) - Core functionality of the application

## Description

Implement the core save file management functionality, including loading, saving, backing up, and restoring Balatro save files across platforms.

## User Stories

- US012: Create Save File Model
- US013: Implement Save File Loading
- US014: Implement Save File Backup
- US015: Implement Save File Restoration
- US016: Create Auto-Save Functionality

## Technical Details

- Create SaveFileService with platform-agnostic functionality
- Parse Balatro save file format (.jkr files)
- Implement backup naming/versioning strategy
- Support multiple profiles (1-10)
- Implement auto-save detection and backup

## Dependencies

- FT001 - Project Setup and Core Architecture
- FT002 - Cross-Platform File System Implementation

## Current Implementation Issues

Current implementation uses hardcoded Windows paths:

```csharp
string GetCurrentSaveFile() {
    var profileNumber = ProfileComboBox.SelectedIndex + 1;
    return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Balatro",
        profileNumber.ToString(),
        "save.jkr"
    );
}
```

## Acceptance Criteria

- Save files can be loaded from any supported platform
- Save files can be backed up with proper versioning
- Save files can be restored from backups
- Auto-save detects changes and creates backups as needed
- Multiple game profiles (1-10) are supported
- Error handling provides clear feedback for file issues

## Estimation

1 week

## Tasks

- TSK019: Create SaveFileModel class
- TSK020: Implement ISaveFileService interface
- TSK021: Create save file loading functionality
- TSK022: Implement backup creation with versioning
- TSK023: Implement save file restoration
- TSK024: Create auto-save detection
- TSK025: Add multiple profile support
- TSK026: Implement error handling for save operations
