# US008: Implement Game Process Service

## Description

As a developer, I want to implement a game process service so that the application can detect if Balatro is running across different operating systems.

## Tasks

- TSK029: Create IGameProcessService interface (Completed)
- TSK030: Implement Windows-specific process detection (Completed)
- TSK031: Implement macOS-specific process detection (Completed)
- TSK032: Implement Linux-specific process detection (Completed)
- TSK049: Consolidate Game Process Detection (Completed) - Unified all platform-specific implementations into single cross-platform solution

## Status

- **Current State**: Closed
- **Priority**: High (4)
- **Start Date**: 2025-05-19
- **Completion Date**: 2025-05-19

## Parent Work Item

- FT002: Core Services Implementation

## Acceptance Criteria

- Application can detect if Balatro is running on all platforms
- Process detection is efficient and doesn't consume excessive resources
- Service provides events for game start and exit
- Detection works consistently across platforms

## Definition of Done

- Game detection works on Windows, macOS, and Linux
- Low resource usage for continuous monitoring
- Events fire correctly when game starts or stops
- Unit tests verify core functionality

## Estimated Effort

- 3 story points

## Dependencies

- US003: Implement Dependency Injection Framework
- US004: Set Up Platform Detection
