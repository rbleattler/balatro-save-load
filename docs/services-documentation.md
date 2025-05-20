# Balatro Save and Load Tool - Services Documentation

This document provides detailed information about all services in the application, their responsibilities, and how they should be implemented. This will help prevent duplication of functionality across the codebase.

## Service Implementation Guidelines

1. All service interfaces should be defined in the `BalatroSaveToolkit.Core` project
2. All service implementations should be placed in the `BalatroSaveToolkit.Services` project
3. Services should be registered in the dependency injection container in `BalatroSaveToolkit.Desktop`
4. Services should be consumed via constructor injection in ViewModels

## Core Services

### FileSystemService

**Interface**: `IFileSystemService` (Core project)
**Implementation**: `FileSystemService` (Services project)
**Responsibility**: Handles all file system operations, including:

- Reading and writing save files
- Backing up save files
- Managing application data directories
- Checking file existence and permissions

### SettingsService

**Interface**: `ISettingsService` (Core project)
**Implementation**: `SettingsService` (Services project)
**Responsibility**: Manages application settings, including:

- Reading and writing settings
- Providing default settings
- Validating settings
- Notifying when settings change

### ThemeService

**Interface**: `IThemeService` (Core project)
**Implementation**: `ThemeService` (Services project)
**Responsibility**: Manages application theming, including:

- Setting application theme (light/dark)
- Loading theme resources
- Notifying when theme changes

### GameProcessService

**Interface**: `IGameProcessService` (Core project)
**Implementation**: `GameProcessService` (Services project)
**Responsibility**: Interacts with the Balatro game process, including:

- Detecting when the game is running
- Monitoring save file changes
- Managing timing of save file operations
- Notifying when game state changes

### NavigationService

**Interface**: `INavigationService` (Core project)
**Implementation**: `NavigationService` (Services project)
**Responsibility**: Handles navigation between views, including:

- Navigating to different views/pages
- Passing parameters between views
- Managing navigation history
- Supporting back navigation

**IMPORTANT**: There should be only ONE implementation of the NavigationService. Any navigation functionality should use this service rather than creating new implementations.

## Planned Services

### DialogService

**Interface**: `IDialogService` (Core project)
**Implementation**: `DialogService` (Services project)
**Responsibility**: Manages dialog displays, including:

- Showing message dialogs
- Displaying file dialogs
- Presenting confirmation dialogs
- Showing progress dialogs

### SaveFileParserService

**Interface**: `ISaveFileParserService` (Core project)
**Implementation**: `SaveFileParserService` (Services project)
**Responsibility**: Parses and interprets Balatro save files, including:

- Reading save file content
- Extracting game state data
- Validating save file integrity
- Providing structured access to save data

### SaveFileModificationService

**Interface**: `ISaveFileModificationService` (Core project)
**Implementation**: `SaveFileModificationService` (Services project)
**Responsibility**: Modifies Balatro save files, including:

- Updating specific game state values
- Ensuring save file integrity after changes
- Providing pre-defined modification templates
- Validating modifications

## Service Dependencies

Services may depend on other services. Below is the dependency graph:

- `SaveFileModificationService` → `SaveFileParserService` → `FileSystemService`
- `NavigationService` → (no dependencies)
- `ThemeService` → `SettingsService` → `FileSystemService`
- `GameProcessService` → `FileSystemService`
- `DialogService` → (may depend on platform-specific implementations)

## Avoiding Service Duplication

To avoid duplication of services:

1. **Check Existing Code**: Before implementing a new service, check the `Core` and `Services` projects to see if the functionality already exists
2. **Extend Existing Services**: If functionality is similar to an existing service, consider extending that service rather than creating a new one
3. **Centralize Common Functionality**: Put shared functionality in base classes or utility methods
4. **Document New Services**: When adding a new service, update this document and the architecture overview
