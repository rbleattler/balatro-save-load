# Task: Implement Game Statistics Extraction for Save Files

**ID**: TSK048
**Type**: Task
**Status**: Completed
**Parent**: TSK046
**Created**: 2025-05-21

## Description

Extract and display structured game statistics from Balatro save files in the SaveContentViewer. This will provide users with key information about their game progress without requiring them to parse through raw save data.

## Acceptance Criteria

- [x] Create a SaveFileParser class to extract game data from save files
- [x] Identify and extract key game statistics (coins, jokers, cards, etc.)
- [x] Add a structured game statistics view to SaveContentView
- [x] Display statistics in a user-friendly format
- [x] Support for different game versions and save file formats
- [x] Handle parsing errors gracefully

## Dependencies

- SaveContentViewModel implementation
- SaveContentView implementation
- IFileSystemService implementation

## Notes

The save file format is a Lua dictionary structure that contains nested information about the game state. The parser will need to handle this format and extract meaningful data. This task is a sub-task of TSK046 (SaveContentViewer functionality) but has been separated out for better tracking and implementation focus.

## Implementation Guidelines

1. Start by analyzing save file content format from example files
2. Identify patterns for key game statistics
3. Create a structured model for game statistics data
4. Implement the parser to convert raw content to structured data
5. Enhance the SaveContentView to display this structured data
6. Add tabs or sections to switch between raw content and statistics view
