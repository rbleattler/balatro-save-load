# Service Interaction Diagram

This document illustrates how the various services in the Balatro Save and Load Tool interact with each other and with the rest of the application.

## Service Dependency Flow

```mermaid
graph TD
    App[Application Entry] --> DI[Dependency Injection Container]
    DI --> FS[FileSystemService]
    DI --> SS[SettingsService]
    DI --> TS[ThemeService]
    DI --> GPS[GameProcessService]
    DI --> NS[NavigationService]
    DI --> DS[DialogService]

    VM[ViewModels] --> FS
    VM --> SS
    VM --> TS
    VM --> GPS
    VM --> NS
    VM --> DS

    FS --> SS
    GPS --> FS
```

## Service Responsibilities

### FileSystemService

```mermaid
graph LR
    FS[FileSystemService] --> FO[File Operations]
    FS --> PD[Path Detection]
    FS --> FW[File Watching]
    FS --> SF[Save File Management]

    FO --> R[Read Files]
    FO --> W[Write Files]
    FO --> D[Delete Files]

    PD --> PS[Platform-Specific Paths]
    PD --> AD[App Data Location]
    PD --> GD[Game Data Location]

    FW --> CE[Change Events]
    FW --> PC[Platform-Specific Watchers]

    SF --> SD[Save Detection]
    SF --> SM[Save Metadata]
```

### SettingsService

```mermaid
graph LR
    SS[SettingsService] --> SR[Settings Repository]
    SS --> SV[Settings Validation]
    SS --> PC[Property Change Notification]

    SR --> LP[Load Preferences]
    SR --> SP[Save Preferences]
    SR --> DP[Default Preferences]

    SV --> TV[Type Validation]
    SV --> RV[Range Validation]

    PC --> CE[Change Events]
```

### ThemeService

```mermaid
graph LR
    TS[ThemeService] --> TD[Theme Detection]
    TS --> TM[Theme Management]
    TS --> TC[Theme Customization]

    TD --> OT[OS Theme Detection]
    TD --> AP[Avalonia Platform Integration]

    TM --> ST[Switch Themes]
    TM --> TL[Theme Loading]

    TC --> CT[Custom Themes]
    TC --> CC[Color Customization]
```

### GameProcessService

```mermaid
graph LR
    GPS[GameProcessService] --> PD[Process Detection]
    GPS --> PM[Process Monitoring]
    GPS --> PI[Process Interaction]

    PD --> FP[Find Process]
    PD --> PP[Platform-Specific Detection]

    PM --> CE[Change Events]
    PM --> PS[Process Status]

    PI --> MC[Memory Commands]
```

### NavigationService

```mermaid
graph LR
    NS[NavigationService] --> RM[Route Management]
    NS --> VP[View Presentation]
    NS --> HP[History Preservation]

    RM --> RR[Register Routes]
    RM --> NR[Navigate Routes]

    VP --> VM[View Models]
    VP --> PA[Parameter Passing]

    HP --> NB[Navigation Back]
    HP --> NS[Navigation Stack]
```

### DialogService

```mermaid
graph LR
    DS[DialogService] --> DM[Dialog Management]
    DS --> DP[Dialog Presentation]
    DS --> DR[Dialog Results]

    DM --> CD[Custom Dialogs]
    DM --> SD[System Dialogs]

    DP --> ME[Message Dialogs]
    DP --> FD[File Dialogs]
    DP --> CP[Custom Prompts]

    DR --> RC[Result Callbacks]
    DR --> TP[Task-based Promises]
```

## Sequence Diagrams

### Save File Loading

```mermaid
sequenceDiagram
    participant U as User
    participant V as View
    participant VM as ViewModel
    participant FS as FileSystemService
    participant GPS as GameProcessService

    U->>V: Select Save File
    V->>VM: LoadSaveCommand
    VM->>FS: LoadSaveFile(path)
    FS-->>VM: SaveFileData
    VM->>GPS: CheckGameRunning()
    GPS-->>VM: GameStatus
    VM-->>V: Update UI
    V-->>U: Display Save Info
```

### Theme Switching

```mermaid
sequenceDiagram
    participant U as User
    participant V as View
    participant VM as ViewModel
    participant TS as ThemeService
    participant SS as SettingsService

    U->>V: Change Theme
    V->>VM: SwitchThemeCommand
    VM->>TS: SetTheme(themeName)
    TS->>SS: SaveSetting("Theme", themeName)
    SS-->>TS: Saved
    TS-->>VM: ThemeChanged
    VM-->>V: Update UI
    V-->>U: Display New Theme
```
