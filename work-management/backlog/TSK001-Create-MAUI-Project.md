# Task: TSK001 - Create new .NET MAUI project

## Parent User Story

- [US001 - Create MAUI Project Structure](US001-Create-MAUI-Project.md)

## Assigned To

[Developer]

## Priority

High (1)

## Estimated Hours

2

## Description

Create a new .NET MAUI project targeting .NET 9 that will serve as the foundation for the Balatro Save and Load Tool. Configure it to support Windows, macOS, and Linux platforms.

## Steps

1. Install the latest .NET 9 SDK and MAUI workload
2. Create a new MAUI project using the following command:

    ```dotnetcli
    dotnet new maui -n BalatroSaveAndLoad.Maui
    ```

3. Configure project settings in .csproj file to target .NET 9
4. Add references to required NuGet packages
5. Set up application identifier and version information
6. Configure supported platforms (Windows, macOS, Linux)

## Acceptance Criteria

- Project is created with proper .NET MAUI structure
- Project builds successfully without errors
- Application can be launched in debug mode
- Project is configured for all target platforms
- Basic Resources.xaml is set up for styling

## Dependencies

- None

## Notes

Ensure the project name matches the naming convention for the application.
Make sure to configure appropriate app icons and splash screens early on.
