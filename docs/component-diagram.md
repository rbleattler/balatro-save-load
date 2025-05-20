# Component Diagram

## Application Components

```
+-------------------------------------+
|           Application               |
+-----------------+-----------------+-+
                  |
      +-----------+-----------+
      |                       |
+-----v------+        +------v------+
|    UI      |        |    Core     |
+------------+        +-------------+
| - Views    |        | - Services  |
| - ViewModels|       | - Models    |
| - Styles   |        | - Commands  |
+-----+------+        +------+------+
      |                      |
      |                      |
+-----v----------------------v------+
|        Platform Adapters          |
+-----------------------------------+
| - Windows Implementation          |
| - macOS Implementation            |
| - Linux Implementation            |
+-----------------------------------+
```

## Core Service Dependencies

```
+----------------+     +----------------+     +----------------+
| NavigationSvc  |<----|  ViewModels   |---->|  DialogSvc     |
+----------------+     +----------------+     +----------------+
                              |
                              v
+----------------+     +----------------+     +----------------+
| FileSystemSvc  |<----|  CommandInfra  |---->| SettingsSvc    |
+----------------+     +----------------+     +----------------+
                              |
                              v
+----------------+     +----------------+
| GameProcessSvc |<----|  ThemeSvc      |
+----------------+     +----------------+
```

## MVVM Architecture

```
+------------------+     +------------------+     +------------------+
|      View        |<--->|    ViewModel     |<--->|      Model       |
+------------------+     +------------------+     +------------------+
| - XAML UI        |     | - Properties     |     | - Data Structure |
| - Styles         |     | - Commands       |     | - Business Rules |
| - Event Handlers |     | - State Logic    |     | - Validation     |
+------------------+     +------------------+     +------------------+
        ^                        |
        |                        v
+------------------+     +------------------+
|  User Interface  |     |     Services     |
+------------------+     +------------------+
```
