# FT002: Core Services Implementation

## Description
Implement the core services required for the Balatro Save and Load Tool's functionality using a platform-agnostic approach. This includes file system operations, settings management, theme services, and game process detection.

## Objectives
- Create service interfaces for all core functionality
- Implement platform-specific service implementations where needed
- Ensure all services work across Windows, macOS, and Linux
- Replace direct WPF dependencies with abstracted interfaces

## Status
- **Current State**: Open
- **Priority**: High (2)
- **Start Date**: May 19, 2025

## Parent Work Item
- EP001: WPF to Avalonia Migration

## User Stories
- US005: Implement File System Service ✓
- US006: Implement Settings Service ✓
- US007: Implement Theme Service
- US008: Implement Game Process Service

## Acceptance Criteria
- All services have clean interfaces with proper abstraction
- Platform-specific implementations are properly isolated
- Services can be easily mocked for testing
- All core functionality from the WPF version is supported

## Timeline
- **Start Date**: TBD
- **Target Completion**: TBD

## Dependencies
- FT001: Project Setup and Infrastructure
