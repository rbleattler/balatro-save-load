# Balatro Save and Load Tool

A simple tool that allows you to save your Balatro game at any point, and load it again later.

## How it works

- Identifies the balatro data directory -- in windows this is "C:\Users\UserName\AppData\Roaming\Balatro"
  - Looks at the profile directories beneath (I.e. "C:\Users\UserName\AppData\Roaming\Balatro\1", "...\2", etc.), specifically the one that matches the selected profile in the dropdown
  - Allows the user to copy the "save.jkr" file from the selected profile to the application's save directory (default: "C:\Users\UserName\AppData\Roaming\BalatroSaveAndLoad") with a custom filename which reflects the profile, round, ante, and time/date of the save by
    - Pressing the save button manually / automatically saving every x minutes when that feature is enabled
- Displays a single-selectable list of all saves in the save directory
- Allows users to load any selected save (copy save into the selected profile directory as 'save.jkr') instantly
  - This will overwrite the current save in the selected profile, so be careful!
- Allows users to see what is going on with a pop-out debug pane which shows log messages
- Gives the status of the application in a status bar at the bottom of the window
- Gives the status of the Balatro application in a status bar at the bottom of the window (I.E. Running/Not Running)
- Flashes the window in the taskbar and changes the color of the status bar to red when there are errors

## How to use

- Hit "Save" whenever you want to save your current run in the selected profile.
- Check "Auto save" if you want the current profile to be saved every X minutes.
- All Saves show up in the list
- To load a save file:
  - In Balatro: go to the main menu (you need to exit your current run)
  - Select the save file in the list, and click "Load". WARNING! Your current run in Balatro will be overwritten!
  - In Balatro: continue your current run, it will be the one you just loaded.

## Download

Download the latest release here: <https://github.com/papauschek/balatro-save-load/releases/tag/v1.1.0>

## Screenshot

![image](https://github.com/papauschek/balatro-save-load/assets/1398727/9a5bd799-37ea-4704-9ead-fa63ce22b87e)

## Build Status

[![Build WPF app](https://github.com/papauschek/balatro-save-load/actions/workflows/build.yml/badge.svg)](https://github.com/papauschek/balatro-save-load/actions/workflows/build.yml)

This project is automatically built using GitHub Actions. You can find the latest build artifact by following these steps:

1. Go to the [Actions tab](https://github.com/papauschek/balatro-save-load/actions) in this repository.
2. Click on the latest "Build WPF app" workflow run.
3. Scroll down to the "Artifacts" section.
4. Download the "BalatroSaveAndLoad" artifact.

The artifact contains the latest executable build of the application.

## Development

To build this project locally:

1. Ensure you have .NET 8.0 SDK installed.
2. Clone this repository.
3. Open a terminal in the project directory.
4. Run the following commands:

    ```dotnetcli
    dotnet restore BalatroSaveAndLoad/BalatroSaveAndLoad.csproj
    dotnet build BalatroSaveAndLoad/BalatroSaveAndLoad.csproj --configuration Release
    dotnet publish BalatroSaveAndLoad/BalatroSaveAndLoad.csproj -c Release -o publish --self-contained true -r win-x64 /p:PublishSingleFile=true
    ```

5. The built executable will be in the `publish` directory.
