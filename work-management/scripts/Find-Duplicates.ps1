#requires -Modules PSMarkdown
<#
.SYNOPSIS
    Finds duplicate work items by ID and/or name in the work-management system.
.DESCRIPTION
    This script scans all work item markdown files in the backlog, open, and closed directories of the work-management system. It compares work items by ID and by name (using a similarity algorithm) to detect duplicates or near-duplicates. Optionally, it can compare file contents for further similarity analysis. The script outputs a report of matches and suspected matches, with options for table, grid view, or markdown output.
.PARAMETER CompareContents
    If specified, also compares the contents of files for similarity (not just names).
.PARAMETER OutTable
    If specified, outputs the results as a formatted table.
.PARAMETER OutGridView
    If specified, outputs the results in a grid view window (requires GUI).
.PARAMETER OutMarkdown
    If specified, outputs the results as a markdown file (duplicates.md).
.EXAMPLE
    .\Find-Duplicates.ps1
    Runs the script and outputs duplicate work items to the console.
.EXAMPLE
    .\Find-Duplicates.ps1 -CompareContents -OutTable
    Runs the script, compares file contents, and outputs results as a table.
.NOTES
    Requires the Get-Similarity.ps1 script in the same directory. Uses Levenshtein distance for similarity. Designed for use in the Balatro Save and Load Tool work-management system.
#>
[CmdletBinding()]
param (
    [switch]$CompareContents,
    [switch]$OutTable,
    [switch]$OutGridView,
    [switch]$OutMarkdown
)
begin {
    $ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
    $WorkMgmtDir = Split-Path -Path $ScriptDir -Parent
    $BacklogDir = Join-Path -Path $WorkMgmtDir -ChildPath "backlog"
    $OpenDir = Join-Path -Path $WorkMgmtDir -ChildPath "open"
    $ClosedDir = Join-Path -Path $WorkMgmtDir -ChildPath "closed"
    . $PSScriptRoot\Get-Similarity.ps1
    # Load the Get-Similarity function from the script
    if (-not (Get-Command -Name Get-Similarity -ErrorAction SilentlyContinue)) {
        Write-Error "Get-Similarity function not found. Please ensure it is defined in the script."
        return
    }
}
process {

    # Get the script directory and work-management paths

    Write-Host "Checking for duplicate work items in the work backlog..."

    # Get all work item IDs from backlog directory
    $backlogFiles = Get-ChildItem -Path $BacklogDir -Filter "*.md" -File -ErrorAction SilentlyContinue
    $closedFiles = Get-ChildItem -Path $ClosedDir -Filter "*.md" -File -ErrorAction SilentlyContinue
    $openFiles = Get-ChildItem -Path $OpenDir -Filter "*.md" -File -ErrorAction SilentlyContinue

    $files = @($backlogFiles + $closedFiles + $openFiles)

    $items = [System.Collections.Generic.List[PSCustomObject]]::new()

    $totalFiles = $files.Count
    $currentFile = 0

    foreach ($file in $files) {
        $currentFile++
        $percentComplete = ($currentFile / $totalFiles) * 100
        Write-Progress -Activity "Processing Files" -Status "Processing $($file.Name)" -PercentComplete $percentComplete

        if ($file.Name -match "^(?<ID>(EP|FT|US|TSK)[0-9]{3}).*`.md") {
            $itemId = $matches["ID"]
            $name = $file.Name
            $item = [pscustomobject]@{
                ID   = $itemId
                Name = $name
                File = $file
            }
            $items.Add($item)
        }
    }

    $duplicates = [System.Collections.Generic.List[PSCustomObject]]::new()
    # Compare all items to each other
    $totalItems = $items.Count
    for ($i = 0; $i -lt $items.Count; $i++) {
        for ($j = $i + 1; $j -lt $items.Count; $j++) {
            $percentComplete = (($i * $items.Count + $j) / ($totalItems * $totalItems)) * 100
            Write-Progress -Activity "Comparing Items" -Status "Comparing $($items[$i].Name) and $($items[$j].Name)" -PercentComplete $percentComplete

            if ($items[$i].ID -eq $items[$j].ID) {
                $duplicate = [pscustomobject]@{
                    Type       = "ID"
                    ID         = $items[$i].ID
                    Similarity = "100%"
                    Name1      = $items[$i].Name
                    Dir1       = $items[$i].File.DirectoryName.Split([System.IO.Path]::DirectorySeparatorChar)[-1]
                    Dir2       = $items[$j].File.DirectoryName.Split([System.IO.Path]::DirectorySeparatorChar)[-1]
                    Name2      = $items[$j].Name
                    File1      = $items[$i].File.FullName
                    File2      = $items[$j].File.FullName
                }
                $duplicates.Add($duplicate)
            } else {
                # Normalize names for comparison (lowercase, remove spaces and special characters)
                $name1 = ($items[$i].Name -replace "[^a-zA-Z0-9]", "").ToLower()
                $name2 = ($items[$j].Name -replace "[^a-zA-Z0-9]", "").ToLower()

                # Calculate similarity
                $similarity = Get-Similarity -String1 $name1 -String2 $name2

                if ($CompareContents) {
                    if ($similarity -gt 0.75) {
                        $content1 = Get-Content -Path $items[$i].File.FullName -Raw
                        $content2 = Get-Content -Path $items[$j].File.FullName -Raw
                        # if either content is empty, do not compare, set the similarity to 0, unless both are empty, then set to 1
                        if ([string]::IsNullOrWhiteSpace($content1) -and [string]::IsNullOrWhiteSpace($content2)) {
                            Write-Verbose "$($items[$i].Name) and $($items[$j].Name) are identical (both empty)"
                            $similarity = 1.0
                        } elseif ([string]::IsNullOrWhiteSpace($content1) -or [string]::IsNullOrWhiteSpace($content2)) {
                            Write-Verbose "$($items[$i].Name) and $($items[$j].Name) are not similar (one empty)"
                            $similarity = 0
                        } else {
                            $content1 = ([string]($content1 -replace "[^a-zA-Z0-9]", "")).ToLower()
                            $content2 = ([string]($content2 -replace "[^a-zA-Z0-9]", "")).ToLower()
                            $contentSimilarity = Get-Similarity -String1 $content1 -String2 $content2
                            $similarity = ($similarity + $contentSimilarity) / 2
                        }
                    }
                }

                # Define a similarity threshold (adjust as needed)
                $similarityThreshold = 0.75

                if ($similarity -gt $similarityThreshold) {
                    Write-Verbose "$($items[$i].Name) and $($items[$j].Name) are similar with a similarity score of $similarity"
                    $duplicate = [pscustomobject]@{
                        Type       = "Name"
                        ID         = $items[$i].ID
                        Similarity = "{0}%" -f [math]::Round(($similarity * 100), 2)
                        Dir1       = $items[$i].File.DirectoryName.Split([System.IO.Path]::DirectorySeparatorChar)[-1]
                        Dir2       = $items[$j].File.DirectoryName.Split([System.IO.Path]::DirectorySeparatorChar)[-1]
                        Name1      = $items[$i].Name
                        Name2      = $items[$j].Name
                        File1      = $items[$i].File.FullName
                        File2      = $items[$j].File.FullName
                    }
                    $duplicates.Add($duplicate)
                }
            }
        }
    }
}
end {
    # A formatted report of duplicates
    Write-Progress -Activity "Finalizing" -Status "Generating Report" -PercentComplete 100
    if ($OutTable) {
        $duplicates | Format-Table -AutoSize -Property Type, Dir1, Dir2, Similarity, Name1, Name2
    } elseif ($OutGridView) {
        $duplicates | Out-GridView
    } elseif ($OutMarkdown) {
        $duplicates | ConvertTo-Markdown | Out-File -FilePath "duplicates.md"
    } else {
        $duplicates
    }

}
