<#
.SYNOPSIS
    Moves work items from backlog to open status
.DESCRIPTION
    This script moves the specified work item(s) from the backlog directory to the open directory
    and updates the status in the file to "Open"
.PARAMETER WorkItemId
    The ID(s) of the work item(s) to move (e.g., TSK001, US002, FT003, EP001)
.PARAMETER ConsolidateDuplicates
    If specified, runs the Consolidate-WorkItems script after moving items to handle any duplicates
.EXAMPLE
    .\Move-ToOpen.ps1 -WorkItemId TSK001
.EXAMPLE
    .\Move-ToOpen.ps1 -WorkItemId TSK001,US002
.EXAMPLE
    .\Move-ToOpen.ps1 -WorkItemId TSK001,US002 -ConsolidateDuplicates
#>

param (
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidatePattern('^(EP|FT|US|TSK)[0-9]{3}$')]
    [string[]]$WorkItemId,

    [Parameter(Mandatory = $false)]
    [switch]$ConsolidateDuplicates
)

# Get the script directory and work-management paths
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$WorkMgmtDir = Split-Path -Path $ScriptDir -Parent
$BacklogDir = Join-Path -Path $WorkMgmtDir -ChildPath "backlog"
$OpenDir = Join-Path -Path $WorkMgmtDir -ChildPath "open"

foreach ($itemId in $WorkItemId) {
    # Find matching files in backlog directory
    $files = Get-ChildItem -Path $BacklogDir -Filter "$itemId*.md" -File -ErrorAction SilentlyContinue

    if (-not $files) {
        Write-Warning "No file found for work item $itemId in backlog directory. Skipping..."
        continue
    }

    # Handle multiple matching files
    if ($files.Count -gt 1) {
        Write-Host "Multiple files found for work item $itemId`:"
        for ($i = 0; $i -lt $files.Count; $i++) {
            Write-Host "[$i] $($files[$i].Name)"
        }
        $selection = Read-Host "Please enter the number of the file you want to move (or 'all' to move all)"

        if ($selection -eq "all") {
            $selectedFiles = $files
        }
        elseif ($selection -match '^\d+$' -and [int]$selection -lt $files.Count) {
            $selectedFiles = @($files[[int]$selection])
        }
        else {
            Write-Warning "Invalid selection. Skipping..."
            continue
        }
    }
    else {
        $selectedFiles = @($files)
    }

    # Process each selected file
    foreach ($file in $selectedFiles) {
        $targetFile = Join-Path -Path $OpenDir -ChildPath $file.Name

        Write-Host "Moving $($file.Name) to open directory..."

        # Read file content
        $content = Get-Content -Path $file.FullName -Raw

        # Update status in content
        $content = $content -replace '(?m)^Status:.*$', 'Status: Open'

        # Add start date if it doesn't exist
        if (-not ($content -match '(?m)^Start Date:')) {
            $currentDate = Get-Date -Format "yyyy-MM-dd"
            $content = $content -replace '(?m)^(Status:.*)$', "`$1`r`nStart Date: $currentDate"
        }

        # Write updated content to file
        Set-Content -Path $file.FullName -Value $content

        # Move file to open directory
        Move-Item -Path $file.FullName -Destination $targetFile

        Write-Host "Successfully moved and updated $($file.Name)"

        # Update parent work items if any
        if ($content -match '(?m)^Parent:\s*([^\s]*)') {
            $parentId = $matches[1]

            if ($parentId -match '^(EP|FT|US)[0-9]{3}$') {
                Write-Host "Found parent work item: $parentId. Updating parent status..."

                # Look for parent in both open and backlog directories
                foreach ($parentDir in @($OpenDir, $BacklogDir)) {
                    $parentFiles = Get-ChildItem -Path $parentDir -Filter "$parentId*.md" -File -ErrorAction SilentlyContinue

                    foreach ($parentFile in $parentFiles) {
                        Write-Host "Updating parent file: $($parentFile.FullName)"

                        # Read parent content
                        $parentContent = Get-Content -Path $parentFile.FullName -Raw

                        # Update the child item status in parent file
                        if ($parentContent -match $itemId) {
                            $parentContent = $parentContent -replace "$itemId - [^O].*", "$itemId - Open"

                            # Write updated content back to parent file
                            Set-Content -Path $parentFile.FullName -Value $parentContent
                            Write-Host "Updated child status in parent file"
                        }

                        # If parent is in backlog, consider moving it to open
                        if ($parentFile.DirectoryName -eq $BacklogDir) {
                            Write-Host "Parent work item is still in backlog. Consider moving it to open as well."
                        }
                    }
                }
            }
        }    }
}

# Run consolidation if requested
if ($ConsolidateDuplicates) {
    Write-Host "Running work item consolidation to handle duplicates..."
    $consolidateScript = Join-Path -Path $ScriptDir -ChildPath "Consolidate-WorkItems.ps1"
    if (Test-Path $consolidateScript) {
        & $consolidateScript
    } else {
        Write-Warning "Consolidate-WorkItems.ps1 script not found. Skipping consolidation."    }
}

# Run consolidation if requested
if ($ConsolidateDuplicates) {
    Write-Host "Running work item consolidation to handle duplicates..."
    $consolidateScript = Join-Path -Path $ScriptDir -ChildPath "Consolidate-WorkItems.ps1"
    if (Test-Path $consolidateScript) {
        & $consolidateScript
    } else {
        Write-Warning "Consolidate-WorkItems.ps1 script not found. Skipping consolidation."
    }
}

Write-Host "Operation completed."
