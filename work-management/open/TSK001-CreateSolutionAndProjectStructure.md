# TSK001: Create Solution and Project Structure

## Description
Create the new solution and project structure for the Avalonia-based Balatro Save and Load Tool.

## Steps
1. Create a new solution file in the V2 directory
2. Add the following projects to the solution:
   - BalatroSaveToolkit (Main application project)
   - BalatroSaveToolkit.Core (Core library for shared code)
   - BalatroSaveToolkit.Services (Services implementation)
   - BalatroSaveToolkit.Tests (Unit tests project)
3. Set up appropriate directory structure for each project:
   - Models
   - ViewModels
   - Views
   - Services (for interfaces)
4. Add solution items: README, .gitignore, etc.

## Acceptance Criteria
- Solution structure is created with all required projects
- Directory structure follows best practices for Avalonia/MVVM applications
- Solution can be opened and built without errors

## Parent User Story
- US001: New Solution Structure

## Status
- **Status**: Open
- **Start Date**: 2025-05-19
- **Target Completion**: 2025-05-19

## Dependencies
None

## Priority
Highest - First task in the migration process

## Notes
- Follow Avalonia project templates for best practices
- Ensure solution structure supports cross-platform development
