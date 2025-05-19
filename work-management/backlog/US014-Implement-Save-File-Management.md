# US014: Implement Save File Management

## Description
As a user, I want to be able to manage my Balatro save files so that I can back up my game progress.

## Tasks
- TSK053: Implement save file creation
- TSK054: Create save file backup logic
- TSK055: Implement save file deletion
- TSK056: Add save file export/import functionality

## Status
- **Current State**: Backlog
- **Priority**: High (1)

## Parent Work Item
- FT004: Save/Load Functionality Migration

## Acceptance Criteria
- Users can create new save files from current game state
- Save files are backed up automatically
- Users can delete unwanted save files
- Save files can be exported and imported

## Definition of Done
- Save file creation works on all platforms
- Backups are stored in appropriate locations
- Delete operations have confirmation dialogs
- Export/import handles file format correctly

## Estimated Effort
- 3 story points

## Dependencies
- US005: Implement File System Service
- US011: Implement List View for Save Files
