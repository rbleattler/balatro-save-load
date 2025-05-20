<#
.SYNOPSIS
    Find duplicates by ID and/or name
.DESCRIPTION
    This script finds duplicate work items in the work-management structure by comparing ID and name of all work items to find similar work items. It outputs a report of matches & suspected matches
.EXAMPLE
    .\Find-Duplicates.ps1
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
    function Get-Similarity {
        <#
        .SYNOPSIS
        Calculates the similarity between two strings using the Levenshtein distance algorithm.

        .DESCRIPTION
        This function computes the similarity score between two input strings based on the Levenshtein distance.
        The Levenshtein distance represents the minimum number of single-character edits required to change one string into the other.
        The similarity score is calculated as 1 - (Levenshtein Distance / Length of the longer string).

        .PARAMETER String1
        The first string to compare.

        .PARAMETER String2
        The second string to compare.

        .EXAMPLE
        Get-Similarity -String1 "kitten" -String2 "sitting"

        Returns a similarity score between the strings "kitten" and "sitting".

        .NOTES
        The function uses dynamic programming to efficiently calculate the Levenshtein distance.
        #>
        param (
            [string]$String1,
            [string]$String2
        )
        $Distance = 0
        $Matrix = @(, @()) * ($String1.Length + 1)
        for ($i = 0; $i -le $String1.Length; $i++) {
            $Matrix[$i] = @()
            for ($j = 0; $j -le $String2.Length; $j++) {
                $Matrix[$i] += 0
            }
        }

        for ($i = 0; $i -le $String1.Length; $i++) {
            $Matrix[$i][0] = $i
        }

        for ($j = 0; $j -le $String2.Length; $j++) {
            $Matrix[0][$j] = $j
        }

        for ($i = 1; $i -le $String1.Length; $i++) {
            for ($j = 1; $j -le $String2.Length; $j++) {
                $Cost = if ($String1[$i - 1] -eq $String2[$j - 1]) { 0 } else { 1 }
                $Matrix[$i][$j] = [Math]::Min($Matrix[$i - 1][$j] + 1, [Math]::Min($Matrix[$i][$j - 1] + 1, $Matrix[$i - 1][$j - 1] + $Cost))
            }
        }

        $Distance = $Matrix[$String1.Length][$String2.Length]
        $MaxLength = [Math]::Max($String1.Length, $String2.Length)
        return 1 - ($Distance / $MaxLength)
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
    }
    else {
        $duplicates
    }

}
