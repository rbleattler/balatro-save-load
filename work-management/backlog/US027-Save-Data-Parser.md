# User Story: US027 - Create Save Data Parser

## Parent Feature

- [FT007 - Save Data Viewer Implementation](FT007-Save-Viewer.md)

## Priority

Medium (2) - Required for save viewer but not critical for core saving/loading

## Story Points

8

## Description

As a developer, I need to create a parser that can convert Balatro's Lua dictionary save format into a structured, navigable object model so that users can more easily explore and understand their save data.

## Acceptance Criteria

- Parser correctly handles all Balatro save file formats
- Lua dictionary syntax is properly parsed into an object model
- Hierarchical structure is maintained for nested data
- Parser handles edge cases (large files, special characters)
- Performance is acceptable even with large save files
- Unit tests validate parsing accuracy

## Tasks

- TSK045: Research Lua dictionary format specifics
- TSK046: Create data model for parsed content (LuaNode)
- TSK047: Implement core parsing algorithm
- TSK048: Create handling for nested structures
- TSK049: Add error handling for malformed content
- TSK050: Optimize parsing performance
- TSK051: Create unit tests with sample save files

## Technical Notes

```csharp
// Current "JSON" formatting implementation:
private string FormatLuaContent(string content)
{
    // Basic text formatting with simple rules
}

// New structured approach:
public class LuaNode
{
    public string Key { get; set; }
    public object Value { get; set; }
    public NodeType Type { get; set; }
    public ObservableCollection<LuaNode> Children { get; } = new();
}

public interface ISaveFileParserService
{
    LuaNode ParseSaveFile(string content);
    string FormatSaveFile(LuaNode rootNode);
}
```

## Dependencies

- US012 - Create Save File Model

## Testing

- Parse sample save files of varying complexity
- Test with large save files to verify performance
- Validate structure is maintained correctly
