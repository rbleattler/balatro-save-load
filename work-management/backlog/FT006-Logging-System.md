# Feature: FT006 - Logging System Implementation

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

Medium (2) - Important for diagnostics but not blocking core functionality

## Description

Replace the current debug window implementation with a proper logging system that writes to files and displays session logs in a debug view.

## User Stories

- US022: Create Logging Service Interface
- US023: Implement File-based Logging
- US024: Create Debug Log Page UI
- US025: Implement Log Level Filtering
- US026: Add Log Persistence Configuration

## Technical Details

- Use Microsoft.Extensions.Logging for core functionality
- Create file logging provider for persistent logs
- Implement UI to display current session logs
- Support different log levels (Debug, Info, Warning, Error)
- Allow filtering logs by level or category

## Dependencies

- FT001 - Project Setup and Core Architecture
- FT005 - UI Migration and Enhancement

## Current Implementation Issues

Current implementation uses a simple ObservableCollection for logs:

```csharp
private void DebugLog(string message)
{
    var timestamp = DateTime.Now.ToString("HH:mm:ss");
    var entry = $"[{timestamp}] {message}";
    _debugLog.Add(entry);
    if (_debugWindow != null) { _debugWindow.AppendLog(entry); }
}
```

## Acceptance Criteria

- Logging system writes to file with proper formatting
- Log files are rotated/managed to prevent excessive growth
- Debug view shows current session logs only
- Logs can be filtered by level or category
- Log entries include timestamp, level, and category
- Log system is properly integrated with DI container
- Log files are stored in an appropriate platform-specific location

## Estimation

3-5 days

## Tasks

- TSK036: Create ILogService interface
- TSK037: Implement FileLogProvider
- TSK038: Create LogViewModel for UI binding
- TSK039: Implement LogPage UI
- TSK040: Add log level filtering
- TSK041: Implement log rotation
- TSK042: Create log entry model
- TSK043: Add log configuration options
- TSK044: Integrate logging throughout application
