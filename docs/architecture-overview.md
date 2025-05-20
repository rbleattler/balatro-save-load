# Balatro Save and Load Tool - Architecture Overview

## Project Structure

The Balatro Save and Load Tool is organized as a multi-project solution following a modular, layered architecture. The solution is structured to separate concerns and ensure maintainability, testability, and extensibility. This document provides an overview of the architecture and the responsibilities of each project.

## Solution Projects

The solution consists of the following projects:

### BalatroSaveToolkit.Core

**Purpose**: Contains the core domain logic, interfaces, and abstractions that are platform-independent.

**Responsibilities**:

- Core domain models and entities
- Interface definitions for services
- Common utilities and helpers
- Command infrastructure (ReactiveCommandWrapper)

**Key Components**:

- Service interfaces (IFileSystemService, ISettingsService, etc.)
- Shared models and DTOs
- Command infrastructure

### BalatroSaveToolkit.Services

**Purpose**: Implements the service interfaces defined in the Core project.

**Responsibilities**:

- Concrete service implementations
- Business logic
- Data access and persistence
- Game process interaction

**Key Components**:

- FileSystemService - Handles file system operations
- SettingsService - Manages application settings
- ThemeService - Manages UI theming
- GameProcessService - Interacts with the Balatro game process
- NavigationService - Handles navigation between views (SINGLE IMPLEMENTATION HERE)

### BalatroSaveToolkit.UI

**Purpose**: Contains the UI layer built with Avalonia.

**Responsibilities**:

- Views and ViewModels
- UI-specific logic
- User interactions
- Styling and theming

**Key Components**:

- App entry point
- MainWindow and other views
- ViewModels
- Styles and resources

### BalatroSaveToolkit.Desktop

**Purpose**: Platform-specific desktop application project.

**Responsibilities**:

- Platform-specific initialization
- Dependency registration
- Application bootstrapping

**Key Components**:

- Platform-specific implementations
- Main entry point
- Dependency injection configuration

## Architecture Patterns

The application follows these architectural patterns:

1. **MVVM (Model-View-ViewModel)**: Separates UI from business logic
2. **Dependency Injection**: Services are registered and injected where needed
3. **Reactive Programming**: Using ReactiveUI for reactive UI development
4. **Command Pattern**: Using ReactiveCommand for encapsulating actions

## Service Localization

To avoid duplicated service implementations:

1. All service interfaces should be defined in the Core project
2. Service implementations should be placed in the Services project
3. Services should be registered in the Desktop project's DI container
4. ViewModels should receive services via constructor injection

## Navigation System

Navigation between views is handled by a single Navigation Service:

1. **Interface**: `INavigationService` (defined in Core)
2. **Implementation**: `NavigationService` (implemented in Services)
3. **Registration**: Registered as a singleton in the DI container
4. **Usage**: Injected into ViewModels that need navigation capabilities

## Data Flow

1. **User Input**: Captured in Views
2. **Commands**: Execute business logic via ViewModels
3. **Services**: Perform operations and return results
4. **State Updates**: Propagated via ReactiveUI properties and observables
