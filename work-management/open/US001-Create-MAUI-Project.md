# User Story: US001 - Create MAUI Project Structure

## Parent Feature

- [FT001 - Project Setup and Core Architecture](FT001-Project-Setup.md)

## Priority

High (1) - Foundational step for the entire project

## Story Points

3

## Description

As a developer, I need to set up a new .NET MAUI project with the appropriate structure so that I can begin migrating the application from WPF.

## Acceptance Criteria

- New MAUI project is created targeting .NET 9
- Project structure includes folders for Views, ViewModels, Models, and Services
- Basic app resources are set up (icons, splash screen, etc.)
- Project builds successfully with no errors
- Basic AppShell structure is created

## Tasks

- [x] TSK001: Create new .NET MAUI project using the latest SDK
- [x] TSK002: Set up folder structure (Views, ViewModels, Models, Services)
- [x] TSK003: Configure project properties and target platforms
- [x] TSK004: Add required NuGet packages (completed)
- [x] TSK005: Create basic resources (icons, splash) (completed with default resources)
- [x] TSK007: Implement Error Handling System (completed)
- [x] TSK006: Implement Platform-Specific File Services (completed)
- [x] TSK008: Implement Logging System (completed)

## Technical Notes

```csharp
// Implemented structure for MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services registered with proper dependency injection
        builder.Services.AddSingleton<IFileService, FileService>();
        builder.Services.AddSingleton<ILogService, MockLogService>();
        builder.Services.AddSingleton<ISettingsService, MockSettingsService>();
        builder.Services.AddSingleton<IGameProcessService, MockGameProcessService>();
        builder.Services.AddSingleton<ISaveService, MockSaveService>();

        // ViewModels registered
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();

        return builder.Build();
    }
}
```

## Dependencies

None - This is the first user story to complete

## Testing

- ✅ Verified project builds successfully
- ✅ Configured Windows as the primary target platform
- ✅ Added configurations for macOS and Linux (to be enabled later)

## Implementation Notes

- Added proper nullable reference type handling throughout the project
- Fixed file service implementation to use correct MAUI APIs
- Added proper error handling for file operations
- Created GlobalUsings.cs for consistent imports
- Added two new tasks (TSK006 and TSK007) to improve cross-platform support and error handling
- Project now follows MVVM pattern with dependency injection

## Next Steps

1. Move this user story to the closed folder
2. Begin work on the next feature according to the implementation order
3. Prioritize completing TSK006 and TSK007 to ensure robust cross-platform support
