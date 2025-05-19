# Balatro Save and Load Tool - Migration Project

## Project Overview

This repository contains a migration project to convert the Balatro Save and Load Tool from WPF to Avalonia. The goal is to make the application cross-platform (Windows, macOS, Linux) while improving its architecture, modularity, and user experience.

All work is handled via the work management system. Always refer to the work items for the latest status and details. Always work on the next most relevant and important work item. If you are unsure what to work on next, ask.

## Work Management System

### Work Item Hierarchy

The project uses a hierarchical work management system with the following levels:

1. **Epic (EP###)** - The highest level that represents the entire migration project
2. **Feature (FT###)** - Major components of work that deliver specific functionality
3. **User Story (US###)** - Implementable requirements that deliver value
4. **Task (TSK###)** - Individual, actionable items that complete a user story

### Work Item Status

Work items move through three states:

1. **Backlog** - Defined but not yet started
2. **Open** - Currently being worked on
3. **Closed** - Completed and verified

### File Naming Convention

All work items are stored as Markdown files following this pattern:

- `EP###-Name.md` - Epic files
- `FT###-Name.md` - Feature files
- `US###-Name.md` - User story files
- `TSK###-Name.md` - Task files

## Working with Work Items

### Starting Work on an Item

When beginning work on a work item:

1. Move the file from `/work-management/backlog/` to `/work-management/open/`
2. Update any parent work items to reflect that the child item is now in progress
3. Create any necessary code or documentation files related to the work item

### Completing Work on an Item

When completing a work item:

1. Move the file from `/work-management/open/` to `/work-management/closed/`
2. Update any parent work items to reflect that the child item is now complete
3. Document any relevant information learned during implementation
4. Update the status in the work item markdown file
