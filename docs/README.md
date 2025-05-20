# Balatro Save and Load Tool - Documentation Index

This documentation covers the architecture, design patterns, and implementation details of the Balatro Save and Load Tool migration project. The goal is to provide comprehensive information for developers to understand the codebase and continue development without duplicating functionality.

## Available Documentation

### Project Overview

- [Project Status and Roadmap](project-status.md) - Current state and planned work
- [Architecture Overview](architecture-overview.md) - High-level architecture of the application

### Detailed Documentation

- [Services Documentation](services-documentation.md) - Detailed information about all services and their responsibilities
- [UI Architecture](ui-architecture.md) - MVVM implementation and UI organization
- [Navigation System](navigation-system.md) - Detailed explanation of the navigation system

## Critical Guidelines for Development

1. **Check Existing Services**: Before implementing a new service or feature, check the existing codebase and documentation to ensure it doesn't already exist
2. **Follow Established Patterns**: Use the established architectural patterns and practices documented here
3. **Update Documentation**: When adding significant features or making architectural changes, update the relevant documentation
4. **Single Navigation Service**: There should be only ONE implementation of the Navigation Service
5. **Dependency Injection**: Use constructor injection for dependencies
6. **Update Progress Summary**: Always update the progress-summary.md file after making substantial changes

## How to Use This Documentation

- Start with the architecture overview to understand the high-level structure
- Refer to the services documentation when implementing or modifying services
- Consult the UI architecture documentation when working on the UI layer
- Refer to the navigation system documentation when implementing navigation

## Directory Structure

```
BalatroSaveToolkit.Core/         # Core domain models and interfaces
├── Models/                      # Domain models
├── Services/                    # Service interfaces
├── Commands/                    # Command infrastructure
└── Extensions/                  # Extension methods

BalatroSaveToolkit.Services/     # Service implementations
├── FileSystem/                  # File system services
├── Settings/                    # Settings services
├── Theming/                     # Theme services
├── GameProcess/                 # Game process services
└── Navigation/                  # Navigation services

BalatroSaveToolkit.UI/           # UI layer
├── Assets/                      # Static assets
├── ViewModels/                  # ViewModels
├── Views/                       # Views
└── Resources/                   # UI resources

BalatroSaveToolkit.Desktop/      # Platform-specific code
└── DependencyInjection/         # DI setup
```
