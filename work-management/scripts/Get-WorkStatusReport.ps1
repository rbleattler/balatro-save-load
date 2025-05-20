<#
.SYNOPSIS
    Generates a status report for all work items
.DESCRIPTION
    This script generates a report of all work items in the workspace
    showing counts by type and status
.EXAMPLE
    .\Get-WorkStatusReport.ps1
#>

# Get the script directory and work-management paths
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$WorkMgmtDir = Split-Path -Path $ScriptDir -Parent
$BacklogDir = Join-Path -Path $WorkMgmtDir -ChildPath "backlog"
$OpenDir = Join-Path -Path $WorkMgmtDir -ChildPath "open"
$ClosedDir = Join-Path -Path $WorkMgmtDir -ChildPath "closed"

# Initialize counters for each work item type and state
$EP_backlog = 0
$EP_open = 0
$EP_closed = 0

$FT_backlog = 0
$FT_open = 0
$FT_closed = 0

$US_backlog = 0
$US_open = 0
$US_closed = 0

$TSK_backlog = 0
$TSK_open = 0
$TSK_closed = 0

# Count items in backlog
$backlogFiles = Get-ChildItem -Path $BacklogDir -Filter "*.md" -File -ErrorAction SilentlyContinue
foreach ($file in $backlogFiles) {
    if ($file.Name -match '^EP[0-9]{3}') {
        $EP_backlog++
    }
    elseif ($file.Name -match '^FT[0-9]{3}') {
        $FT_backlog++
    }
    elseif ($file.Name -match '^US[0-9]{3}') {
        $US_backlog++
    }
    elseif ($file.Name -match '^TSK[0-9]{3}') {
        $TSK_backlog++
    }
}

# Count items in open
$openFiles = Get-ChildItem -Path $OpenDir -Filter "*.md" -File -ErrorAction SilentlyContinue
foreach ($file in $openFiles) {
    if ($file.Name -match '^EP[0-9]{3}') {
        $EP_open++
    }
    elseif ($file.Name -match '^FT[0-9]{3}') {
        $FT_open++
    }
    elseif ($file.Name -match '^US[0-9]{3}') {
        $US_open++
    }
    elseif ($file.Name -match '^TSK[0-9]{3}') {
        $TSK_open++
    }
}

# Count items in closed
$closedFiles = Get-ChildItem -Path $ClosedDir -Filter "*.md" -File -ErrorAction SilentlyContinue
foreach ($file in $closedFiles) {
    if ($file.Name -match '^EP[0-9]{3}') {
        $EP_closed++
    }
    elseif ($file.Name -match '^FT[0-9]{3}') {
        $FT_closed++
    }
    elseif ($file.Name -match '^US[0-9]{3}') {
        $US_closed++
    }
    elseif ($file.Name -match '^TSK[0-9]{3}') {
        $TSK_closed++
    }
}

# Calculate totals
$EP_total = $EP_backlog + $EP_open + $EP_closed
$FT_total = $FT_backlog + $FT_open + $FT_closed
$US_total = $US_backlog + $US_open + $US_closed
$TSK_total = $TSK_backlog + $TSK_open + $TSK_closed

$total_backlog = $EP_backlog + $FT_backlog + $US_backlog + $TSK_backlog
$total_open = $EP_open + $FT_open + $US_open + $TSK_open
$total_closed = $EP_closed + $FT_closed + $US_closed + $TSK_closed
$total_all = $total_backlog + $total_open + $total_closed

# Calculate percentages for closed items
$EP_percent = if ($EP_total -gt 0) { [math]::Round(($EP_closed / $EP_total) * 100, 0) } else { 0 }
$FT_percent = if ($FT_total -gt 0) { [math]::Round(($FT_closed / $FT_total) * 100, 0) } else { 0 }
$US_percent = if ($US_total -gt 0) { [math]::Round(($US_closed / $US_total) * 100, 0) } else { 0 }
$TSK_percent = if ($TSK_total -gt 0) { [math]::Round(($TSK_closed / $TSK_total) * 100, 0) } else { 0 }
$total_percent = if ($total_all -gt 0) { [math]::Round(($total_closed / $total_all) * 100, 0) } else { 0 }

# Format the output as a table
Write-Host "`n=== Balatro Save and Load Tool - Work Status Report ===" -ForegroundColor Cyan
Write-Host (Get-Date -Format "yyyy-MM-dd HH:mm:ss") -ForegroundColor Gray
Write-Host ""

$formatString = "{0,-10} | {1,-10} | {2,-10} | {3,-10} | {4,-10} | {5,-10}"

Write-Host ($formatString -f "Type", "Backlog", "Open", "Closed", "Total", "% Complete")
Write-Host ("------------------------------------------------------------")
Write-Host ($formatString -f "Epics", $EP_backlog, $EP_open, $EP_closed, $EP_total, "$EP_percent%")
Write-Host ($formatString -f "Features", $FT_backlog, $FT_open, $FT_closed, $FT_total, "$FT_percent%")
Write-Host ($formatString -f "Stories", $US_backlog, $US_open, $US_closed, $US_total, "$US_percent%")
Write-Host ($formatString -f "Tasks", $TSK_backlog, $TSK_open, $TSK_closed, $TSK_total, "$TSK_percent%")
Write-Host ("------------------------------------------------------------")
Write-Host ($formatString -f "TOTAL", $total_backlog, $total_open, $total_closed, $total_all, "$total_percent%")
Write-Host ""

# Optional: Generate a list of in-progress items
if ($total_open -gt 0) {
    Write-Host "=== Currently Open Work Items ===" -ForegroundColor Yellow

    # Print epics
    if ($EP_open -gt 0) {
        Write-Host "--- Epics ---" -ForegroundColor Yellow
        $epics = Get-ChildItem -Path $OpenDir -Filter "EP*.md" -File -ErrorAction SilentlyContinue
        foreach ($file in $epics) {
            $id = $file.Name -replace "\.md$", ""
            $title = (Get-Content -Path $file.FullName -TotalCount 10 | Where-Object { $_ -match '^# ' } | Select-Object -First 1) -replace '^# ', ''
            Write-Host ("{0,-10}: {1}" -f $id, $title)
        }
        Write-Host ""
    }

    # Print features
    if ($FT_open -gt 0) {
        Write-Host "--- Features ---" -ForegroundColor Yellow
        $features = Get-ChildItem -Path $OpenDir -Filter "FT*.md" -File -ErrorAction SilentlyContinue
        foreach ($file in $features) {
            $id = $file.Name -replace "\.md$", ""
            $title = (Get-Content -Path $file.FullName -TotalCount 10 | Where-Object { $_ -match '^# ' } | Select-Object -First 1) -replace '^# ', ''
            Write-Host ("{0,-10}: {1}" -f $id, $title)
        }
        Write-Host ""
    }

    # Print user stories
    if ($US_open -gt 0) {
        Write-Host "--- User Stories ---" -ForegroundColor Yellow
        $stories = Get-ChildItem -Path $OpenDir -Filter "US*.md" -File -ErrorAction SilentlyContinue
        foreach ($file in $stories) {
            $id = $file.Name -replace "\.md$", ""
            $title = (Get-Content -Path $file.FullName -TotalCount 10 | Where-Object { $_ -match '^# ' } | Select-Object -First 1) -replace '^# ', ''
            Write-Host ("{0,-10}: {1}" -f $id, $title)
        }
        Write-Host ""
    }

    # Print tasks
    if ($TSK_open -gt 0) {
        Write-Host "--- Tasks ---" -ForegroundColor Yellow
        $tasks = Get-ChildItem -Path $OpenDir -Filter "TSK*.md" -File -ErrorAction SilentlyContinue
        foreach ($file in $tasks) {
            $id = $file.Name -replace "\.md$", ""
            $title = (Get-Content -Path $file.FullName -TotalCount 10 | Where-Object { $_ -match '^# ' } | Select-Object -First 1) -replace '^# ', ''
            Write-Host ("{0,-10}: {1}" -f $id, $title)
        }
        Write-Host ""
    }
}

Write-Host "Report completed."
