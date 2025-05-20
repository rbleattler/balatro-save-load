# Balatro Save and Load Tool - Migration Project

## Project Overview

This repository contains a migration project to convert the Balatro Save and Load Tool from WPF to Avalonia. The goal is to make the application cross-platform (Windows, macOS, Linux) while improving its architecture, modularity, and user experience.

All work is handled via the work management system. Always refer to the work items for the latest status and details. Always work on the next most relevant and important work item. If you are unsure what to work on next, ask.

## Required Workflow

**CRITICAL**: You MUST read `progress-summary.md` before starting any work to understand the current project state.

**MANDATORY**: After making substantial changes or completing work items, you MUST update `progress-summary.md` so the next contributor can easily pick up where you left off. This file serves as the primary handoff document between different contributors.

## Build and Testing

The project includes a PowerShell script called `localbuild.ps1` in the `V2/scripts` directory, which can be used to build the solution and capture output for analysis.

### Using the Build Script

- **To restore packages**:
  ```powershell
  .\V2\scripts\localbuild.ps1 -Restore
  ```

- **To clean the solution**:
  ```powershell
  .\V2\scripts\localbuild.ps1 -Clean
  ```

- **To build the solution**:
  ```powershell
  .\V2\scripts\localbuild.ps1 -Build
  ```

- **You can combine these operations**:
  ```powershell
  .\V2\scripts\localbuild.ps1 -Clean -Restore -Build
  ```

### Local Build Script

The `localbuild.ps1` script in the `V2/scripts` directory is a critical tool for building and troubleshooting the project. It provides structured output to help identify and fix issues efficiently.

#### Script Parameters

The build script accepts the following parameters:

- `-Restore`: Restores NuGet packages for the solution
- `-Clean`: Cleans the solution, removing build artifacts
- `-Build`: Builds the solution
- `-Configuration [Debug|Release]`: Specifies the build configuration (defaults to Debug)
- `-WarnLevel [0-4]`: Sets the warning level for the compiler (defaults to 4)

#### Output Files

When the script runs, it generates three output files in the `V2/scripts` directory:

1. **`build_output.txt`**: Contains the complete, unfiltered build output including all information, warnings, and errors.
2. **`errors.txt`**: Contains only error messages extracted from the build output. This should be your first stop when troubleshooting build failures.
3. **`warnings.txt`**: Contains only warning messages from the build. These should be reviewed and addressed to prevent future issues.

#### Recommended Usage

1. When starting work on the codebase, run a full clean, restore, and build:
   ```powershell
   .\V2\scripts\localbuild.ps1 -Clean -Restore -Build
   ```

2. For subsequent builds during development, you can typically just use:
   ```powershell
   .\V2\scripts\localbuild.ps1 -Build
   ```

3. If you update NuGet package references, make sure to include the `-Restore` flag:
   ```powershell
   .\V2\scripts\localbuild.ps1 -Restore -Build
   ```

#### Troubleshooting Build Failures

When a build fails, follow this process:

1. **Examine `errors.txt` first**: This file contains all errors that caused the build to fail. Fix these issues first.

2. **Review `warnings.txt`**: After fixing errors, check warnings to identify potential problems or code quality issues.

3. **Consult `build_output.txt`**: If you need more context about an error or warning, check the complete build output.

4. **Iterative fixes**: Make small, focused changes to fix one error at a time, then rebuild to validate the fix.

5. **Verification**: After all errors and warnings are addressed, run a full clean, restore, and build to verify everything works correctly:
   ```powershell
   .\V2\scripts\localbuild.ps1 -Clean -Restore -Build
   ```

Remember to update `progress-summary.md` with any significant changes or fixes you make during the build troubleshooting process.

### Analyzing Build Output

The script generates three output files in the `V2/scripts` directory:

1. **`build_output.txt`** - Contains the complete build output
2. **`errors.txt`** - Contains only error messages from the build
3. **`warnings.txt`** - Contains only warning messages from the build

When troubleshooting build issues:
1. First check `errors.txt` for compilation errors that need to be fixed
2. Then review `warnings.txt` to address any warnings that might cause problems
3. For more context, refer to the complete `build_output.txt` file

Always run the build script after making significant changes to verify that the code compiles correctly.

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

## Work Management Scripts

The project includes scripts to help manage work items efficiently. These scripts are available in both Bash (for Linux/macOS) and PowerShell (for Windows) formats and are located in the `/work-management/scripts/` directory.

### Creating Work Items

- **`new_work_item.sh`** / **`New-WorkItem.ps1`**: Creates new work items with proper formatting and automatically assigns the next available ID.
  - Usage:
    ```
    ./new_work_item.sh -t Task -n "Add Packages" -p US001 -d "Add Avalonia packages to project" -a "All packages installed" -a "References updated"
    ```
  - PowerShell:
    ```
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
