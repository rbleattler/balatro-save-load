# Feature: FT003 - Game Process Monitoring

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

Medium (2) - Important for auto-save functionality but dependent on core file system

## Description

Create a cross-platform service to detect if the Balatro game is running on the host system. This replaces the Windows-specific Process detection in the current WPF implementation.

## User Stories

- US008: Create Process Monitor Service Interface
- US009: Implement Windows Process Detection
- US010: Implement macOS Process Detection
- US011: Implement Linux Process Detection

## Technical Details

- Replace `Process.GetProcessesByName()` Windows-specific code
- Use platform-specific APIs for process detection
- Create abstraction layer through IProcessMonitorService
- Implement platform-specific service registration via dependency injection

## Dependencies

- FT001 - Project Setup and Core Architecture

## Current Implementation Issues

Current WPF implementation is Windows-specific:

```csharp
private bool IsBalatroProcessRunning() {
    try {
        return Process.GetProcessesByName("balatro").Any();
    }
    catch { return false; }
}
```

## Acceptance Criteria

- Process monitoring works on Windows, macOS, and Linux
- Service correctly detects when Balatro is running
- Process status changes trigger appropriate UI updates
- Implementation is properly abstracted through interfaces
- Error handling is implemented for edge cases

## Estimation

3-5 days

## Tasks

- TSK012: Create IProcessMonitorService interface
- TSK013: Implement WindowsProcessMonitorService
- TSK014: Implement MacOSProcessMonitorService
- TSK015: Implement LinuxProcessMonitorService
- TSK016: Create platform-specific registration in DI container
- TSK017: Implement error handling and edge cases
- TSK018: Unit tests for process detection
