# TSK011: Implement Service Location for ViewModels

## Description
Implement service location for ViewModels to allow them to access services through dependency injection.

## Steps
1. Set up ViewModel factory with dependency injection support
2. Configure constructor injection for ViewModels
3. Implement service resolution in ViewModels
4. Set up property injection if needed
5. Configure service location for design-time ViewModels
6. Implement test support for service mocking

## Acceptance Criteria
- Service location works correctly for ViewModels
- ViewModels can access services through constructor injection
- Services are properly resolved and injected
- Design-time ViewModels can access design-time services
- Service mocking works for testing

## Parent User Story
- US003: Dependency Injection Setup

## Status
- **Status**: Backlog
- **Start Date**: Not Started
- **Target Completion**: TBD

## Dependencies
- TSK008: Set Up Dependency Injection Container
- TSK009: Create Service Registration Module
- TSK004: Create Base ViewModel Classes

## Priority
Medium - Required for proper ViewModel-service interaction

## Notes
- Ensure consistent service access pattern across all ViewModels
- Consider design-time support for XAML preview
