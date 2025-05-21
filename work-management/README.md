# Work Management System

## Work Item Hierarchy

The project uses a hierarchical work management system with the following levels:

1. **Epic (EP###)** - The highest level that represents the entire migration project
2. **Feature (FT###)** - Major components of work that deliver specific functionality
3. **User Story (US###)** - Implementable requirements that deliver value
4. **Task (TSK###)** - Individual, actionable items that complete a user story

## Work Item Status

Work items move through three states:

1. **Backlog** - Defined but not yet started
2. **Open** - Currently being worked on
3. **Closed** - Completed and verified

## File Naming Convention

All work items are stored as Markdown files following this pattern:

- `EP###-Name.md` - Epic files
- `FT###-Name.md` - Feature files
- `US###-Name.md` - User story files
- `TSK###-Name.md` - Task files

## Work Management Scripts

The project includes scripts to help manage work items efficiently. These scripts are available in both Bash (for Linux/macOS) and PowerShell (for Windows) formats and are located in the `/work-management/scripts/` directory.

### Script Overview

- **new_work_item.sh / New-WorkItem.ps1**: Create new work items with proper formatting and ID assignment.
- **move_to_open.sh / Move-ToOpen.ps1**: Move work items from backlog to open, update status, and parent references.
- **move_to_closed.sh / Move-ToClosed.ps1**: Move work items from open to closed, update status, and parent references.
- **clean_backlog.sh / Clean-Backlog.ps1**: Remove duplicate work items from backlog if they exist in open or closed.
- **consolidate_work_items.sh / Consolidate-WorkItems.ps1**: Consolidate duplicate work items with the same ID but different names, merging content and standardizing filenames.
- **find_duplicates.sh / Find-Duplicates.ps1**: Detect duplicate or near-duplicate work items by ID, name, or content similarity.
- **work_status_report.sh / Get-WorkStatusReport.ps1**: Generate a status report of all work items, including counts and completion percentage.
- **get_workitem.sh / Get-WorkItem.ps1**: Retrieve a work item by ID from the workspace.

All scripts are self-documented and provide usage instructions via comments or help output. Use the Bash or PowerShell version as appropriate for your platform.

### Creating Work Items

- **`new_work_item.sh`** / **`New-WorkItem.ps1`**: Creates new work items with proper formatting and automatically assigns the next available ID.
  - Usage:

    ```sh
    ./new_work_item.sh -t Task -n "Add Packages" -p US001 -d "Add Avalonia packages to project" -a "All packages installed" -a "References updated"
    ```

  - PowerShell:

    ```pwsh
    .\New-WorkItem.ps1 -Type Task -Title "Add Packages" -Parent US001 -Description "Add Avalonia packages to project" -AcceptanceCriteria "All packages installed", "References updated"
    ```

  - These scripts will:
    - Generate the correct work item ID based on existing items
    - Create the file with proper formatting in the backlog directory
    - Automatically update the parent work item with a reference to the new item
    - Validate parent-child relationships (e.g., Tasks must have User Story parents)

### Managing Work Item Status

- **`move_to_open.sh`** / **`Move-ToOpen.ps1`**: Moves items from backlog to open and updates status metadata
  - Usage: `./move_to_open.sh TSK001`
  - PowerShell: `.\Move-ToOpen.ps1 -WorkItemId TSK001`
  - These scripts will:
    - Update the item status to "Open"
    - Add a start date timestamp
    - Move the file to the open directory
    - Update parent work items to show the child as "Open"

- **`move_to_closed.sh`** / **`Move-ToClosed.ps1`**: Moves items from open to closed and updates status metadata
  - Usage: `./move_to_closed.sh TSK001`
  - PowerShell: `.\Move-ToClosed.ps1 -WorkItemId TSK001`
  - Multiple items can be closed at once: `./move_to_closed.sh TSK001 TSK002 TSK003`
  - These scripts will:
    - Update the item status to "Closed"
    - Add a completion date timestamp
    - Move the file to the closed directory
    - Update parent work items to show the child as "Closed"
    - Suggest closing the parent if all children are closed

- **`clean_backlog.sh`** / **`Clean-Backlog.ps1`**: Removes duplicate work items from backlog
  - Usage: `./clean_backlog.sh`
  - PowerShell: `.\Clean-Backlog.ps1`
  - These scripts identify and remove backlog items that also exist in open or closed states
  - Run periodically to ensure the backlog stays clean and accurate

### Reporting

- **`work_status_report.sh`** / **`Get-WorkStatusReport.ps1`**: Generates a detailed status report of all work items
  - Usage: `./work_status_report.sh`
  - PowerShell: `.\Get-WorkStatusReport.ps1`
  - The report includes:
    - Counts of items by type (Epic, Feature, User Story, Task) and status
    - Overall project completion percentage
    - List of currently open items for quick reference
    - Run this script to get a snapshot of project status before planning work

## Working with Work Items

### Starting Work on an Item

When beginning work on a work item:

1. Move the file from `/work-management/backlog/` to `/work-management/open/` (or use the move_to_open script)
2. Update any parent work items to reflect that the child item is now in progress
3. Create any necessary code or documentation files related to the work item

### Completing Work on an Item

When completing a work item:

1. Move the file from `/work-management/open/` to `/work-management/closed/` (or use the move_to_closed script)
2. Update any parent work items to reflect that the child item is now complete
3. Document any relevant information learned during implementation
4. Update the status in the work item markdown file
