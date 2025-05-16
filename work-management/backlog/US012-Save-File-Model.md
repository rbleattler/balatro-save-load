# User Story: US012 - Create Save File Model

## Parent Feature

- [FT004 - Save File Management](FT004-Save-Management.md)

## Priority

High (1) - Core data model required for save functionality

## Story Points

5

## Description

As a developer, I need to create a proper data model for Balatro save files so that the application can properly represent and manipulate save data across platforms.

## Acceptance Criteria

- SaveFile model class is created with appropriate properties
- Model includes metadata (timestamp, profile, version)
- Model supports serialization/deserialization
- Model handles platform-specific path differences
- Model includes backup/version tracking capabilities
- Unit tests validate model functionality

## Tasks

- TSK019: Create SaveFileModel class with core properties
- TSK020: Implement serialization/deserialization methods
- TSK021: Add metadata properties for tracking
- TSK022: Create unit tests for the model
- TSK023: Implement validation methods

## Technical Notes

```csharp
// Current save path implementation in WPF:
string GetCurrentSaveFile() {
    var profileNumber = ProfileComboBox.SelectedIndex + 1;
    return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Balatro",
        profileNumber.ToString(),
        "save.jkr"
    );
}

// Example of new model implementation:
public class SaveFileModel
{
    public int ProfileId { get; set; }
    public DateTime Timestamp { get; set; }
    public string FilePath { get; set; }
    public string BackupPath { get; set; }
    public string Content { get; set; }
    public bool IsBackup { get; set; }
    public string Version { get; set; }

    // Methods for handling paths
    public string GetPlatformPath(IPlatformPathProvider pathProvider)
    {
        return pathProvider.GetSaveFilePath(ProfileId);
    }
}
```

## Dependencies

- US001 - Create MAUI Project Structure
- US004 - Implement IFileSystemService Interface

## Testing

- Unit tests for path generation across platforms
- Tests for serialization/deserialization
- Validation tests for different save file formats
