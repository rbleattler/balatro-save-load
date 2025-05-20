<#
.SYNOPSIS
Calculates the similarity between two strings using the Levenshtein distance algorithm, with support for penalizing specific distinguishing words.

.DESCRIPTION
This function computes the similarity score between two input strings based on the Levenshtein distance.
The Levenshtein distance represents the minimum number of single-character edits required to change one string into the other.
The similarity score is calculated as 1 - (Levenshtein Distance / Length of the longer string).
If the only difference between the strings is a word from the -DistinguishingWords list, the similarity is set to 0 (not similar).

.PARAMETER String1
The first string to compare.

.PARAMETER String2
The second string to compare.

.PARAMETER DistinguishingWords
A list of words that, if they are the only difference between the two strings, will cause the similarity to be set to 0.

.EXAMPLE
Get-Similarity -String1 "save-linux" -String2 "save-windows" -DistinguishingWords linux,windows

Returns 0 if the only difference is the distinguishing word.

.NOTES
The function uses dynamic programming to efficiently calculate the Levenshtein distance.
#>
function Get-Similarity {
  param (
    [string]$String1,
    [string]$String2,
    [string[]]$DistinguishingWords = @()
  )
  # Normalize input
  $norm1 = $String1.ToLower()
  $norm2 = $String2.ToLower()
  # Remove non-alphanumeric for comparison
  $norm1 = -join ($norm1 -replace '[^a-z0-9]', ' ').Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
  $norm2 = -join ($norm2 -replace '[^a-z0-9]', ' ').Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)

  if ($DistinguishingWords.Count -gt 0) {
    $words1 = ($String1 -replace '[^a-zA-Z0-9]', ' ').ToLower().Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
    $words2 = ($String2 -replace '[^a-zA-Z0-9]', ' ').ToLower().Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
    $set1 = [System.Collections.Generic.HashSet[string]]::new()
    $set2 = [System.Collections.Generic.HashSet[string]]::new()
    $words1 | ForEach-Object { [void]$set1.Add($_) }
    $words2 | ForEach-Object { [void]$set2.Add($_) }
    $diff1 = [System.Collections.Generic.HashSet[string]]::new($set1)
    $diff2 = [System.Collections.Generic.HashSet[string]]::new($set2)
    $diff1.ExceptWith($set2)
    $diff2.ExceptWith($set1)
    $allDiffs = @($diff1.Split("`n") + $diff2.Split("`n"))

    $allDiffs = $allDiffs | Where-Object {
      $matches = [Regex]::Match($PSItem, "([eE][pP]|[fF][tT]|[uU][sS]|[Tt][Ss][Kk])[0-9]{3}")
      $matches.Captures.Count -eq 0
    }
    if ($allDiffs.Count -gt 0 -and ($allDiffs | Where-Object { $DistinguishingWords -contains $_ }).Count -eq $allDiffs.Count) {
      # Only distinguishing words are different
      return 0
    }
  }

  $Distance = 0
  $Matrix = @(, @()) * ($norm1.Length + 1)
  for ($i = 0; $i -le $norm1.Length; $i++) {
    $Matrix[$i] = @()
    for ($j = 0; $j -le $norm2.Length; $j++) {
      $Matrix[$i] += 0
    }
  }

  for ($i = 0; $i -le $norm1.Length; $i++) {
    $Matrix[$i][0] = $i
  }

  for ($j = 0; $j -le $norm2.Length; $j++) {
    $Matrix[0][$j] = $j
  }

  for ($i = 1; $i -le $norm1.Length; $i++) {
    for ($j = 1; $j -le $norm2.Length; $j++) {
      $Cost = if ($norm1[$i - 1] -eq $norm2[$j - 1]) { 0 } else { 1 }
      $Matrix[$i][$j] = [Math]::Min($Matrix[$i - 1][$j] + 1, [Math]::Min($Matrix[$i][$j - 1] + 1, $Matrix[$i - 1][$j - 1] + $Cost))
    }
  }

  $Distance = $Matrix[$norm1.Length][$norm2.Length]
  $MaxLength = [Math]::Max($norm1.Length, $norm2.Length)
  if ($MaxLength -eq 0) { return 1 }
  return 1 - ($Distance / $MaxLength)
}