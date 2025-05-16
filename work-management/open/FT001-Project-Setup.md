# Feature: FT001 - Project Setup and Core Architecture

## Parent Epic

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Priority

High (1) - Foundational component for all other features

## Description

Set up the new .NET MAUI project structure, establish the core architecture following MVVM pattern, and implement dependency injection for services.

## User Stories

- [x] US001: Create MAUI Project Structure
- US002: Implement MVVM Architecture Foundation
- US003: Set Up Dependency Injection
- US004: Create Base Services Interfaces

## Technical Details

- Target .NET 9 with MAUI
- Follow MVVM design pattern throughout the project
- Implement proper service interfaces and abstraction layers
- Set up project structure with separation of Views, ViewModels, Models, and Services

## Dependencies

None - This is the first feature to be completed

## Acceptance Criteria

- New MAUI project structure is created
- Base MVVM implementation is in place
- Dependency injection framework is set up
- Core service interfaces are defined
- Project builds successfully with no errors

## Estimation

1-2 weeks

## Tasks

- [x] TSK001: Create new MAUI project
- [x] TSK002: Set up project folder structure (Views, ViewModels, Models, Services)
- [x] TSK003: Configure project properties and target platforms
- [x] TSK004: Add required NuGet packages
- [x] TSK005: Create basic resources (icons, splash)
- [ ] TSK006: Implement Platform-Specific File Services (added to backlog)
- [ ] TSK007: Implement Error Handling System (added to backlog)
