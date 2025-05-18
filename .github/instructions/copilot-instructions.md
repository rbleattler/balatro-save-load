# Balatro Save and Load Tool - Migration Project

# Shells

When working on windows, always assume you're in a pwsh (powershell) terminal if you need to run commands. Non-powershell commands cannot be guaranteed to work.

For builds, use `cd d:\Repos\balatro-save-load && dotnet restore .\V2\BalatroSaveToolkit\ && dotnet build .\V2\BalatroSaveToolkit\ | Out-File ./build_output.txt` and check the output in `build_output.txt` for errors. This will ensure you're seeing the latest build output when assessing the state of things.

## Project Overview

This repository contains a migration project to convert the Balatro Save and Load Tool from WPF to .NET MAUI. The goal is to make the application cross-platform (Windows, macOS, Linux) while improving its architecture, modularity, and user experience.

## Repository Structure

- `/work-management/` - Contains the work item tracking system
  - `/work-management/backlog/` - All defined work items awaiting implementation
  - `/work-management/open/` - Work items currently being implemented
  - `/work-management/closed/` - Completed work items

## Work Management System

### Work Item Hierarchy

The project uses a hierarchical work management system with the following levels:

1. **Epic (EP###)** - The highest level that represents the entire migration project
2. **Feature (FT###)** - Major components of work that deliver specific functionality
3. **User Story (US###)** - Implementable requirements that deliver value
4. **Task (TSK###)** - Individual, actionable items that complete a user story

### Work Item Status

Work items move through three states:

1. **Backlog** - Defined but not yet started
2. **Open** - Currently being worked on
3. **Closed** - Completed and verified

### File Naming Convention

All work items are stored as Markdown files following this pattern:
- `EP###-Name.md` - Epic files
- `FT###-Name.md` - Feature files
- `US###-Name.md` - User story files
- `TSK###-Name.md` - Task files

## Working with Work Items

### Starting Work on an Item

When beginning work on a work item:

1. Move the file from `/work-management/backlog/` to `/work-management/open/`
2. Update any parent work items to reflect that the child item is now in progress
3. Create any necessary code or documentation files related to the work item

### Completing Work on an Item

When completing a work item:

1. Move the file from `/work-management/open/` to `/work-management/closed/`
2. Update any parent work items to reflect that the child item is now complete
3. Document any relevant information learned during implementation
4. Update the status in the work item markdown file

## Implementation Guidelines

### Feature Implementation Order

Follow the recommended implementation order outlined in `WorkItemSummary.md`:

1. **Phase 1**: Foundation (FT001, FT002, FT008-initial)
2. **Phase 2**: Core Functionality (FT003, FT004, FT006-initial)
3. **Phase 3**: User Interface (FT005, FT007, FT006-complete, FT008-complete)
4. **Phase 4**: Testing and Deployment (FT009)

### Architectural Principles

When implementing features, follow these architectural principles:

1. **MVVM Pattern** - Proper separation of concerns between View, ViewModel, and Model
2. **Dependency Injection** - Use MAUI's built-in DI container
3. **Interface-Based Design** - Create interfaces for services to allow for platform-specific implementations
4. **Platform Abstraction** - Abstract platform-specific code behind interfaces
5. **Unit Testing** - Write tests for core functionality

### Cross-Platform Considerations

- Use platform-specific implementations where necessary (file paths, process monitoring)
- Leverage MAUI's built-in platform detection for conditional code
- Use platform-agnostic APIs when available

## Technical Focus Areas

1. **Remove Custom Title Bar** - Replace with native title bars
2. **Cross-Platform File System** - Create abstraction for file operations
3. **Process Monitoring** - Platform-specific implementations for detecting game process
4. **Save Data Viewer** - Improve the visualization and navigation of save data
5. **Logging System** - Implement proper logging with file persistence
6. **Configuration System** - Create user-configurable settings with defaults

## Role as an Agent

As a GitHub Copilot agent working on this project, you should:

1. Understand which work items are currently open and their dependencies
2. Help implement the code required for the current work items
3. Suggest moving items between directories when they are started or completed
4. Provide context and guidance based on the architectural principles
5. Help maintain the work management system's integrity

## Completing the Migration

The migration will be considered complete when all features have been implemented and moved to the `/work-management/closed/` directory, and the application successfully runs on all target platforms with all the functionality of the original WPF application, plus the enhancements outlined in the work items.