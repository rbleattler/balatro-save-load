# Balatro Save and Load Tool - MAUI Migration Work Item Summary

## Epic (1)

- [EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI](EP001-MAUI-Migration.md)

## Features (9)

1. [FT001 - Project Setup and Core Architecture](FT001-Project-Setup.md) - **High Priority**
2. [FT002 - Cross-Platform File System Implementation](FT002-File-System.md) - **High Priority**
3. [FT003 - Game Process Monitoring](FT003-Process-Monitoring.md) - **Medium Priority**
4. [FT004 - Save File Management](FT004-Save-Management.md) - **High Priority**
5. [FT005 - UI Migration and Enhancement](FT005-UI-Migration.md) - **High Priority**
6. [FT006 - Logging System Implementation](FT006-Logging-System.md) - **Medium Priority**
7. [FT007 - Save Data Viewer Implementation](FT007-Save-Viewer.md) - **Medium Priority**
8. [FT008 - Configuration System](FT008-Configuration.md) - **Medium Priority**
9. [FT009 - Testing and Deployment](FT009-Testing-Deployment.md) - **High Priority**

## Implementation Order

The features should be implemented in the following order based on dependencies:

### Phase 1: Foundation (Weeks 1-2)

1. FT001 - Project Setup and Core Architecture
1. FT002 - Cross-Platform File System Implementation
1. FT008 - Configuration System (initial implementation)

### Phase 2: Core Functionality (Weeks 3-4)

1. FT003 - Game Process Monitoring
1. FT004 - Save File Management
1. FT006 - Logging System Implementation (initial implementation)

### Phase 3: User Interface (Weeks 5-6)

1. FT005 - UI Migration and Enhancement
1. FT007 - Save Data Viewer Implementation
1. Complete FT006 - Logging System Implementation (UI components)
1. Complete FT008 - Configuration System (UI components)

### Phase 4: Testing and Deployment (Weeks 7-8)

1. FT009 - Testing and Deployment

## Feature Dependencies

```plaintext
FT001 <- FT002, FT003, FT004, FT005, FT006, FT007, FT008
FT002 <- FT004, FT008
FT004 <- FT007
FT005 <- FT006, FT007, FT008
FT009 <- All other features
```

## Key Areas of Improvement from WPF Version

1. **Removal of custom title bar** - Replace with native title bars on each platform
2. **Cross-platform file paths** - Proper handling of different OS file structures
3. **Proper MVVM implementation** - Clear separation of concerns
4. **Dependency Injection** - Services properly registered and injected
5. **Enhanced save viewer** - Better visualization and navigation of save data
6. **Proper logging system** - Persistent logs with filtering and UI display
7. **Configuration system** - User-configurable settings with defaults
8. **Platform-specific implementations** - Properly abstracted for Windows, macOS, and Linux

## Risk Assessment

1. **Platform differences in file access** - High risk, mitigate with proper abstraction
2. **Process monitoring across platforms** - Medium risk, mitigate with platform-specific implementations
3. **UI consistency across platforms** - Medium risk, follow MAUI best practices
4. **Performance with large save files** - Low risk, implement optimization techniques

## Initial Setup Recommendations

1. Create the initial MAUI project structure
2. Set up CI/CD pipeline early
3. Create core interfaces for key services
4. Implement platform-specific path handling first
