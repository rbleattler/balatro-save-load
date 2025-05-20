# Balatro Save and Load Tool - Current State and Roadmap

This document provides an overview of the current state of the project and outlines the planned work to complete the migration from WPF to Avalonia.

## Current State

As of the current implementation, the following components have been completed:

### Core Infrastructure

- **Project Structure**: The solution structure has been set up with Core, Services, UI, and Desktop projects
- **Basic Interfaces**: Core service interfaces have been defined
- **Command Infrastructure**: ReactiveCommandWrapper has been implemented

### Services

- **FileSystemService**: Implemented to handle file operations
- **SettingsService**: Implemented to manage application settings
- **ThemeService**: Implemented to handle theming
- **GameProcessService**: Implemented to interact with the Balatro game process
- **Navigation Infrastructure**: Currently in progress

### UI Framework

- **Avalonia Setup**: Base Avalonia project has been configured
- **ReactiveUI Integration**: ReactiveUI has been integrated

## Work in Progress

The following components are currently being worked on:

- **Navigation Service**: Implementing a centralized navigation system using ReactiveUI routing
- **MVVM Infrastructure**: Setting up base classes for ViewModels and Views

## Planned Work

The following components are planned but not yet implemented:

### UI Features

- **Main Shell**: The main application shell with navigation
- **Dashboard View**: Home screen showing game status
- **Save Manager View**: Interface for managing save files
- **Save Editor View**: Interface for editing save files
- **Settings View**: Interface for configuring application settings

### Core Features

- **Save File Parser**: Service to read and interpret save files
- **Save File Modifier**: Service to modify save files
- **Backup System**: System for backing up save files before modification

### Platform-Specific

- **Windows Implementation**: Complete functionality on Windows
- **macOS Implementation**: Adapting for macOS platform
- **Linux Implementation**: Adapting for Linux platform

## Known Issues and Challenges

- **Cross-Platform File Access**: Ensuring file paths work correctly across platforms
- **Game Detection**: Different approaches needed for detecting the game on different platforms
- **UI Consistency**: Ensuring UI looks and behaves consistently across platforms

## Priority Roadmap

1. **Complete Navigation Infrastructure**: Finish the navigation service implementation
2. **Implement Main Shell**: Create the main application container and navigation
3. **Implement Dashboard**: Create the main dashboard view
4. **Implement Basic Save Management**: Allow viewing and backup of save files
5. **Implement Settings**: Allow configuration of the application
6. **Implement Save Editing**: Add capability to modify save files
7. **Cross-Platform Testing**: Ensure functionality works on all target platforms
8. **Polish and Refinement**: Improve UI, fix bugs, enhance user experience

## Architecture Evolution

The architecture is designed to evolve as the migration progresses:

1. **Current Phase**: Core infrastructure and basic services
2. **Next Phase**: UI shell and navigation
3. **Feature Phase**: Implementation of key features
4. **Refinement Phase**: Performance optimization and user experience improvements
5. **Cross-Platform Phase**: Testing and adapting for different platforms
