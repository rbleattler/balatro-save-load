<#
.SYNOPSIS
    Consolidates duplicate work items with different naming conventions
.DESCRIPTION
    This script identifies and consolidates work items that have the same ID but different names.
    It merges the content from all versions into a single work item with a standardized name.
.EXAMPLE
    .\Consolidate-WorkItems.ps1
#>

# Get the script directory and work-management paths
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$WorkMgmtDir = Split-Path -Path $ScriptDir -Parent
$BacklogDir = Join-Path -Path $WorkMgmtDir -ChildPath "backlog"
$OpenDir = Join-Path -Path $WorkMgmtDir -ChildPath "open"
$ClosedDir = Join-Path -Path $WorkMgmtDir -ChildPath "closed"

# Function to standardize work item filenames
function Get-StandardizedWorkItemName {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ItemId,
        [Parameter(Mandatory = $true)]
        [string]$Title
    )

    # Convert spaces and special characters to dashes, remove multiple dashes, and trim
    $cleanTitle = $Title -replace ':', '' -replace '\s+', '-' -replace '[^\w\-]', '' -replace '\-+', '-'
    return "$ItemId-$cleanTitle.md"
}

# Function to parse work item title from content
function Get-WorkItemTitle {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Content
    )

    if ($Content -match '# [A-Za-z0-9]+: (.+)') {
        return $matches[1].Trim()
    }
    return $null
}

# Function to merge content from duplicate work items
function Merge-WorkItemContent {
    param (
        [Parameter(Mandatory = $true)]
        [array]$Files
    )

    # Get the content from each file
    $allContents = @()
    foreach ($file in $Files) {
        $content = Get-Content -Path $file.FullName -Raw
        $allContents += $content
    }

    # Start with the most complete content
    $mergedContent = $allContents | Sort-Object -Property Length -Descending | Select-Object -First 1

    return $mergedContent
}

# Function to process directories and consolidate work items
function Consolidate-DirectoryWorkItems {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Directory
    )

    Write-Host "Checking for duplicate work items in $Directory directory..."

    # Get all work item files
    $files = Get-ChildItem -Path $Directory -Filter "*.md" -File -ErrorAction SilentlyContinue
    Write-Host "Found $($files.Count) total files in $Directory"

    # Group files by work item ID
    $itemGroups = @{}
    foreach ($file in $files) {
        if ($file.Name -match '^(EP|FT|US|TSK)[0-9]{3}') {
            $itemId = $matches[0]
            Write-Host "  Identified work item $itemId in file $($file.Name)"

            if (-not $itemGroups.ContainsKey($itemId)) {
                $itemGroups[$itemId] = @()
            }

            $itemGroups[$itemId] += $file
        }
    }

    # Process each group of files
    foreach ($itemId in $itemGroups.Keys) {
        $itemFiles = $itemGroups[$itemId]

        # Skip if there's only one file for this ID
        if ($itemFiles.Count -le 1) {
            continue
        }

        Write-Host "Found $($itemFiles.Count) files for work item $itemId"

        # Merge the content from all files
        $mergedContent = Merge-WorkItemContent -Files $itemFiles

        # Parse the title from the merged content
        $title = Get-WorkItemTitle -Content $mergedContent
        if (-not $title) {
            Write-Warning "Could not parse title for work item $itemId, skipping consolidation"
            continue
        }

        # Generate a standardized filename
        $standardizedName = Get-StandardizedWorkItemName -ItemId $itemId -Title $title
        $targetPath = Join-Path -Path $Directory -ChildPath $standardizedName

        # Create the consolidated file
        Set-Content -Path $targetPath -Value $mergedContent

        # Delete the original files except the one we just created
        foreach ($file in $itemFiles) {
            if ($file.FullName -ne $targetPath) {
                Write-Host "Removing duplicate file: $($file.Name)"
                Remove-Item -Path $file.FullName -Force
            }
        }

        Write-Host "Consolidated work item $itemId into $standardizedName"
    }
}

# Process each directory
Consolidate-DirectoryWorkItems -Directory $BacklogDir
Consolidate-DirectoryWorkItems -Directory $OpenDir
Consolidate-DirectoryWorkItems -Directory $ClosedDir

Write-Host "Work item consolidation completed."
