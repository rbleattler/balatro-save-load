# Balatro Save and Load Tool - Architecture Overview

## Introduction

This document provides a comprehensive overview of the Balatro Save and Load Tool architecture, focusing on the V2 migration from WPF to Avalonia. The migration aims to make the application cross-platform compatible (Windows, macOS, Linux) while improving architecture, modularity, and user experience.

## Architecture Principles

The application follows these core architectural principles:

1. **Separation of Concerns** - Clear separation between UI, business logic, and data access
2. **MVVM Pattern** - Model-View-ViewModel architecture for UI organization
3. **Reactive Programming** - Using ReactiveUI for responsive, event-driven interactions
4. **Dependency Injection** - Services are registered and injected to promote loose coupling
5. **Cross-Platform Design** - Platform-specific code is isolated and abstracted
6. **Testability** - Services and components are designed to be testable

## Solution Structure

The solution is organized into the following projects:

- **BalatroSaveToolkit.Core** - Core interfaces, models, and platform-agnostic services
- **BalatroSaveToolkit.UI** - Avalonia UI implementation using MVVM pattern
- **BalatroSaveToolkit.Desktop** - Desktop application entry point and platform-specific code

## Service Layer

The service layer provides the core functionality of the application through a set of interfaces and their implementations:

1. **IFileSystemService** - Handles file I/O operations with platform-specific implementations
2. **ISettingsService** - Manages application settings across sessions
3. **IThemeService** - Provides theme management and OS theme detection
4. **IGameProcessService** - Monitors and interacts with the Balatro game process
5. **INavigationService** - Manages application navigation between views
6. **IDialogService** - Handles application dialogs and user prompts

## UI Architecture

The UI follows the MVVM pattern with ReactiveUI integration:

1. **Views** - Avalonia XAML-based UI components
2. **ViewModels** - Classes that expose data and commands to the views
3. **Models** - Data structures representing application entities

Navigation is implemented using ReactiveUI's routing capabilities, allowing for a flexible, testable navigation system.

## Command Infrastructure

Commands utilize a ReactiveCommand wrapper to provide:

1. Consistent error handling
2. Loading state tracking
3. Execution tracking
4. Automatic UI thread dispatching

## Data Flow

1. User interacts with Views
2. Views trigger commands in ViewModels
3. ViewModels use Services to perform operations
4. Services return data to ViewModels
5. ViewModels update observable properties
6. Views react to property changes through data binding

## Cross-Platform Strategy

The application achieves cross-platform compatibility through:

1. **Platform Abstraction** - Core interfaces with platform-specific implementations
2. **Dependency Injection** - Platform-specific services are registered at startup
3. **Avalonia UI** - Cross-platform UI framework replacing WPF
4. **Platform Detection** - Runtime detection of operating system for behavior switching

## Future Expansion

The architecture is designed to accommodate future enhancements:

1. Additional game support beyond Balatro
2. Plugin system for extensibility
3. Advanced save management features
4. Cloud synchronization capabilities

## Diagrams

For visual representations of the architecture, see:

- [Component Diagram](component-diagram.md)
- [Service Interaction Diagram](service-interaction.md)
