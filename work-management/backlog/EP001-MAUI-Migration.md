# Epic: EP001 - Migrate Balatro Save and Load Tool from WPF to .NET MAUI

## Description

This epic covers the complete migration of the Balatro Save and Load Tool from WPF to .NET MAUI, making it cross-platform compatible while improving architecture, modularity, and user experience.

## Business Value

- Enable cross-platform support (Windows, macOS, Linux) to match the game's availability
- Modernize codebase with improved architecture for better maintainability
- Enhance user experience with improved UI and additional features
- Enable easier feature expansion in the future through modular design

## Timeline

- Estimated: 6-10 weeks
- Target completion: August 1, 2025

## Features

- FT001: Project Setup and Core Architecture
- FT002: Cross-Platform File System Implementation
- FT003: Game Process Monitoring
- FT004: Save File Management
- FT005: UI Migration and Enhancement
- FT006: Logging System Implementation
- FT007: Save Data Viewer Implementation
- FT008: Configuration System
- FT009: Testing and Deployment

## Dependencies

- .NET 9 SDK
- .NET MAUI workload
- Visual Studio 2025 or later with MAUI support

## Acceptance Criteria

- Application runs on Windows, macOS, and Linux
- All existing functionality is preserved or improved
- Cross-platform file paths are handled correctly
- Configuration system allows customization of paths and settings
- Logging system captures runtime information
- Save viewer provides improved navigation of save data
- UI is responsive and maintains MAUI design standards
- Code follows MVVM pattern with proper separation of concerns

## Risks and Mitigation

- **Risk**: Platform-specific behaviors may cause inconsistencies
  **Mitigation**: Create abstraction layers for platform-specific functionality

- **Risk**: MAUI performance may differ from WPF
  **Mitigation**: Regular performance testing throughout development

- **Risk**: File system permissions vary across platforms
  **Mitigation**: Implement proper error handling and user feedback

## Progress Tracking

- [ ] FT001 Complete
- [ ] FT002 Complete
- [ ] FT003 Complete
- [ ] FT004 Complete
- [ ] FT005 Complete
- [ ] FT006 Complete
- [ ] FT007 Complete
- [ ] FT008 Complete
- [ ] FT009 Complete
