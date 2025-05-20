<#
.SYNOPSIS
    Cleans up duplicate work items from the backlog directory
.DESCRIPTION
    This script cleans up duplicate work items in the backlog directory
    by checking if they have been moved to the open or closed directories
.EXAMPLE
    .\Clean-Backlog.ps1
.NOTES
    Consider running Consolidate-WorkItems.ps1 afterward to merge work items with the same ID but different names
#>

# Get the script directory and work-management paths
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$WorkMgmtDir = Split-Path -Path $ScriptDir -Parent
$BacklogDir = Join-Path -Path $WorkMgmtDir -ChildPath "backlog"
$OpenDir = Join-Path -Path $WorkMgmtDir -ChildPath "open"
$ClosedDir = Join-Path -Path $WorkMgmtDir -ChildPath "closed"

Write-Host "Checking for work items that can be removed from backlog..."

# Get all work item IDs from backlog directory
$backlogFiles = Get-ChildItem -Path $BacklogDir -Filter "*.md" -File -ErrorAction SilentlyContinue
$backlogItems = @()

foreach ($file in $backlogFiles) {
    if ($file.Name -match '^(EP|FT|US|TSK)[0-9]{3}') {
        $itemId = $matches[0]
        $backlogItems += $itemId
    }
}

$itemsRemoved = 0
$itemsChecked = 0

# Check each backlog item if it exists in open or closed directories
foreach ($itemId in $backlogItems) {
    $itemsChecked++

    # Check if the work item exists in open or closed directories
    $openExists = (Get-ChildItem -Path $OpenDir -Filter "$itemId*.md" -File -ErrorAction SilentlyContinue).Count -gt 0
    $closedExists = (Get-ChildItem -Path $ClosedDir -Filter "$itemId*.md" -File -ErrorAction SilentlyContinue).Count -gt 0

    # If the work item exists in open or closed directories, remove it from backlog
    if ($openExists -or $closedExists) {
        # Find all matching files in backlog
        $filesToRemove = Get-ChildItem -Path $BacklogDir -Filter "$itemId*.md" -File -ErrorAction SilentlyContinue

        foreach ($file in $filesToRemove) {
            if ($openExists) {
                Write-Host "Work item $itemId is in OPEN directory. Removing $($file.Name) from backlog."
            } else {
                Write-Host "Work item $itemId is in CLOSED directory. Removing $($file.Name) from backlog."
            }

            # Remove the file from backlog
            Remove-Item -Path $file.FullName -Force
            $itemsRemoved++
        }
    }
}

Write-Host "Backlog cleanup completed. Checked $itemsChecked work items, removed $itemsRemoved duplicates."
