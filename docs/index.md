# Balatro Save and Load Tool - Documentation

Welcome to the documentation for the Balatro Save and Load Tool migration project. This documentation provides comprehensive information about the architecture, implementation status, and design decisions of the application.

## Table of Contents

1. [Architecture Overview](architecture-overview.md) - High-level overview of the application architecture
2. [Component Diagram](component-diagram.md) - Visual representation of the application's components
3. [Service Interaction](service-interaction.md) - How the various services interact with each other
4. [Implementation Status](implementation-status.md) - Current status and roadmap
5. [Cross-Platform Implementation](cross-platform-implementation.md) - Details on the cross-platform strategy
6. [MVVM and ReactiveUI](mvvm-reactiveui.md) - Explanation of the MVVM pattern implementation

## Project Overview

The Balatro Save and Load Tool is being migrated from WPF to Avalonia to provide cross-platform compatibility (Windows, macOS, Linux) while improving its architecture, modularity, and user experience.

### Key Goals

1. **Cross-Platform Support** - Enable the application to run natively on Windows, macOS, and Linux
2. **Improved Architecture** - Implement a clean, modular architecture with clear separation of concerns
3. **Enhanced User Experience** - Develop a responsive, intuitive interface that follows modern design principles
4. **Better Maintainability** - Structure the code to be more maintainable and testable
5. **Performance Optimization** - Ensure the application performs well across all platforms

### Core Technologies

- **Avalonia** - Cross-platform UI framework
- **ReactiveUI** - Functional reactive MVVM framework
- **Splat** - Dependency injection container
- **Reactive Extensions** - Reactive programming library
- **System.IO.Abstractions** - File system abstraction for testability
- **Newtonsoft.Json** - JSON serialization

## Development Workflow

The project follows a structured workflow with work items organized in a hierarchical system:

1. **Epic** - The highest level representing the entire migration project
2. **Feature** - Major components of work that deliver specific functionality
3. **User Story** - Implementable requirements that deliver value
4. **Task** - Individual, actionable items that complete a user story

For more information on the workflow, see the [instructions](../.github/instructions/copilot-instructions.md).

## For Contributors

If you're contributing to this project, please start by:

1. Reading the [progress summary](../progress-summary.md) to understand the current state
2. Checking the [implementation status](implementation-status.md) for areas that need work
3. Following the development workflow outlined in the instructions

After completing significant work, be sure to update the progress summary and relevant documentation.

## Building and Testing

The project includes a PowerShell script called `localbuild.ps1` in the `V2/scripts` directory for building and analyzing the solution. For details on using this script, see the [instructions](../.github/instructions/copilot-instructions.md#build-and-testing).
