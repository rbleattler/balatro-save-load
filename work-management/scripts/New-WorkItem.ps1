<#
.SYNOPSIS
    Creates a new work item in the backlog directory
.DESCRIPTION
    This script creates a new work item with the proper format and adds it to the backlog directory
.PARAMETER Type
    The type of work item to create (Epic, Feature, UserStory, Task)
.PARAMETER Title
    The title of the work item
.PARAMETER Parent
    The parent work item ID (optional for Epics, required for others)
.PARAMETER Description
    A description of the work item (optional)
.PARAMETER AcceptanceCriteria
    Acceptance criteria for the work item (optional for Epics and Features)
.EXAMPLE
    .\New-WorkItem.ps1 -Type Epic -Title "Avalonia Migration"
.EXAMPLE
    .\New-WorkItem.ps1 -Type Feature -Title "Project Setup and Infrastructure" -Parent EP001
.EXAMPLE
    .\New-WorkItem.ps1 -Type UserStory -Title "Create New Avalonia Solution Structure" -Parent FT001 -AcceptanceCriteria "Solution structure created","Projects added to solution"
.EXAMPLE
    .\New-WorkItem.ps1 -Type Task -Title "Create new solution and project structure" -Parent US001 -Description "Create a new solution for the Avalonia version" -AcceptanceCriteria "Solution file created","Projects added"
#>

param (
    [Parameter(Mandatory = $true)]
    [ValidateSet("Epic", "Feature", "UserStory", "Task")]
    [string]$Type,

    [Parameter(Mandatory = $true)]
    [string]$Title,

    [Parameter(Mandatory = $false)]
    [ValidatePattern('^(EP|FT|US)[0-9]{3}$')]
    [string]$Parent,

    [Parameter(Mandatory = $false)]
    [string]$Description,

    [Parameter(Mandatory = $false)]
    [string[]]$AcceptanceCriteria
)

# Get the script directory and work-management paths
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$WorkMgmtDir = Split-Path -Path $ScriptDir -Parent
$BacklogDir = Join-Path -Path $WorkMgmtDir -ChildPath "backlog"

# Map the type to its prefix
$typePrefix = @{
    "Epic" = "EP"
    "Feature" = "FT"
    "UserStory" = "US"
    "Task" = "TSK"
}[$Type]

# Validate parent based on type
if ($typePrefix -ne "EP" -and -not $Parent) {
    Write-Error "Parent is required for $Type work items"
    exit 1
}

if ($Parent -and $typePrefix -eq "EP") {
    Write-Warning "Parent should not be specified for Epics. Ignoring parent."
    $Parent = $null
}

if ($Parent -and $typePrefix -eq "FT" -and -not $Parent.StartsWith("EP")) {
    Write-Error "Parent of a Feature must be an Epic (EP###)"
    exit 1
}

if ($Parent -and $typePrefix -eq "US" -and -not $Parent.StartsWith("FT")) {
    Write-Error "Parent of a User Story must be a Feature (FT###)"
    exit 1
}

if ($Parent -and $typePrefix -eq "TSK" -and -not $Parent.StartsWith("US")) {
    Write-Error "Parent of a Task must be a User Story (US###)"
    exit 1
}

# Find the next available ID
$existingFiles = @()
$existingFiles += Get-ChildItem -Path "$BacklogDir\$typePrefix*.md" -File -ErrorAction SilentlyContinue
$existingFiles += Get-ChildItem -Path "$WorkMgmtDir\open\$typePrefix*.md" -File -ErrorAction SilentlyContinue
$existingFiles += Get-ChildItem -Path "$WorkMgmtDir\closed\$typePrefix*.md" -File -ErrorAction SilentlyContinue

$maxId = 0

foreach ($file in $existingFiles) {
    if ($file.Name -match "$typePrefix([0-9]{3})") {
        $idNumber = [int]$matches[1]
        if ($idNumber -gt $maxId) {
            $maxId = $idNumber
        }
    }
}

$nextId = $maxId + 1
$formattedId = "$typePrefix{0:D3}" -f $nextId
$sanitizedTitle = ($Title -replace '[\\/:*?"<>|]', '-') -replace '\s+', '-'
$fileName = "$formattedId-$sanitizedTitle.md"
$filePath = Join-Path -Path $BacklogDir -ChildPath $fileName

# Create the file content based on the type
$creationDate = Get-Date -Format "yyyy-MM-dd"
$content = "# $Title`r`n`r`n"
$content += "ID: $formattedId`r`n"
$content += "Type: $Type`r`n"
$content += "Status: Backlog`r`n"
$content += "Creation Date: $creationDate`r`n"

if ($Parent) {
    $content += "Parent: $Parent`r`n"
}

$content += "`r`n## Description`r`n`r`n"
if ($Description) {
    $content += "$Description`r`n"
} else {
    $content += "TO BE COMPLETED`r`n"
}

if ($Type -eq "Epic") {
    $content += "`r`n## Goals`r`n`r`n- TO BE ADDED`r`n"
    $content += "`r`n## Features`r`n`r`n- None yet`r`n"
}
elseif ($Type -eq "Feature") {
    $content += "`r`n## User Stories`r`n`r`n- None yet`r`n"
}
elseif ($Type -eq "UserStory" -or $Type -eq "Task") {
    $content += "`r`n## Acceptance Criteria`r`n`r`n"

    if ($AcceptanceCriteria -and $AcceptanceCriteria.Count -gt 0) {
        foreach ($criterion in $AcceptanceCriteria) {
            $content += "- [ ] $criterion`r`n"
        }
    } else {
        $content += "- [ ] TO BE ADDED`r`n"
    }
}

if ($Type -eq "UserStory") {
    $content += "`r`n## Tasks`r`n`r`n- None yet`r`n"
}

# Write the file
Set-Content -Path $filePath -Value $content

Write-Host "Created new $Type work item: $formattedId - $Title"
Write-Host "File: $filePath"

# Update parent work item if specified
if ($Parent) {
    $parentFiles = @()
    $parentFiles += Get-ChildItem -Path "$BacklogDir\$Parent*.md" -File -ErrorAction SilentlyContinue
    $parentFiles += Get-ChildItem -Path "$WorkMgmtDir\open\$Parent*.md" -File -ErrorAction SilentlyContinue

    if ($parentFiles.Count -gt 0) {
        $parentFile = $parentFiles[0]
        $parentContent = Get-Content -Path $parentFile.FullName -Raw

        $sectionName = switch ($typePrefix) {
            "FT" { "## Features" }
            "US" { "## User Stories" }
            "TSK" { "## Tasks" }
        }

        # Check if section exists and contains "None yet"
        if ($parentContent -match "$sectionName\r?\n\r?\n- None yet") {
            $updatedContent = $parentContent -replace "$sectionName\r?\n\r?\n- None yet", "$sectionName`r`n`r`n- $formattedId - Backlog - $Title"
        }
        # Check if section exists
        elseif ($parentContent -match "$sectionName\r?\n\r?\n") {
            $updatedContent = $parentContent -replace "($sectionName\r?\n\r?\n)", "`$1- $formattedId - Backlog - $Title`r`n"
        }
        # If section doesn't exist, add it
        else {
            $updatedContent = $parentContent + "`r`n$sectionName`r`n`r`n- $formattedId - Backlog - $Title`r`n"
        }

        # Update the parent file
        Set-Content -Path $parentFile.FullName -Value $updatedContent
        Write-Host "Updated parent work item: $Parent with reference to $formattedId"
    } else {
        Write-Warning "Could not find parent work item file for $Parent. Parent was not updated."
    }
}

return $formattedId
