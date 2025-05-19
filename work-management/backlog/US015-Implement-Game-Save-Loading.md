# US015: Implement Game Save Loading

## Description
As a user, I want to be able to load my saved Balatro games so that I can continue from a previous point.

## Tasks
- TSK057: Implement save file loading into game
- TSK058: Create game state verification
- TSK059: Add confirmation dialogs for loading
- TSK060: Implement error handling for failed loads

## Status
- **Current State**: Backlog
- **Priority**: High (2)

## Parent Work Item
- FT004: Save/Load Functionality Migration

## Acceptance Criteria
- Users can load saved games back into Balatro
- Application verifies game is in a loadable state
- Confirmation is required before overwriting current game
- Errors during loading are handled gracefully

## Definition of Done
- Save loading works on all platforms
- Verification prevents loading when inappropriate
- Confirmation dialogs are clear and informative
- Error handling provides useful feedback

## Estimated Effort
- 3 story points

## Dependencies
- US005: Implement File System Service
- US008: Implement Game Process Service
- US014: Implement Save File Management
