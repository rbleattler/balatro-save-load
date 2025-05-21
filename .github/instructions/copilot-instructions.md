---
applyTo: "**"
---

# Balatro Save and Load Tool - Migration Project

## Project Overview

This repository contains a migration project to convert the Balatro Save and Load Tool from WPF to Avalonia, making the application cross-platform (Windows, macOS, Linux) while improving its architecture and user experience.

## Project Structure

- `/V2/` - Contains the Avalonia implementation (active development)
- `/work-management/` - Work tracking system with tasks, user stories, and progress summaries
- `/docs/` - Project documentation

## Technical Stack

- **UI Framework**: Avalonia UI (cross-platform)
- **Architecture**: MVVM with ReactiveUI
- **DI Container**: Splat
- **Target Platforms**: Windows, macOS, Linux

## Work Management System

All work is handled via the work management system using hierarchical items (Epics, Features, User Stories, Tasks). Always work on the highest priority tasks as indicated in the progress summary. **YOU MUST** Follow the guidance in `work-management/README.md` for creating and managing work items.

## Rules for Contributors

1. **FIRST STEP**: Read `work-management/progress-summary.md` before starting any work to understand the current state
2. Follow established architectural patterns (MVVM, ReactiveUI routing, Dependency Injection)
3. Keep UI and business logic separated
4. Implement platform-specific code through abstraction layers
5. Write cross-platform compatible code
6. Update documentation when adding features or making architectural changes
7. Use the scripts in `V2/scripts` for building and testing

**MANDATORY**: After completing work items, update `work-management/progress-summary.md` with your changes so the next contributor can continue seamlessly.