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

- TSK001: Create new .NET MAUI project using the latest SDK
- TSK002: Set up folder structure (Views, ViewModels, Models, Services)
- TSK003: Configure project properties and target platforms
- TSK004: Add required NuGet packages
- TSK005: Create basic resources (icons, splash)

## Technical Notes

```csharp
// Example structure for MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services here

        return builder.Build();
    }
}
```

## Dependencies

None - This is the first user story to complete

## Testing

- Verify project builds successfully
- Confirm all target platforms can be selected
- Ensure basic app launches on Windows
