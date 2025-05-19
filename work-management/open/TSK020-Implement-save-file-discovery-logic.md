# TSK020: Implement save file discovery logic

## Description
Implement logic to discover and manage Balatro save files across different platforms.

## Steps
1. Implement methods to locate save files on Windows, macOS, and Linux
2. Add file listing capabilities to scan for backup save files
3. Implement save file info extraction
4. Add cleanup functionality for old save files

## Status
- **Current State**: Open
- **Priority**: High
- **Start Date**: May 19, 2025

## Parent Work Item
- US005: Implement File System Service

## Acceptance Criteria
- Service can discover save files on all platforms
- Save file metadata (timestamp, size) is extracted correctly
- Save files can be listed, filtered, and sorted
- Old save files can be automatically cleaned up

## Estimated Effort
- 2 hours

## Dependencies
- TSK018: Implement platform-specific file system service implementations
