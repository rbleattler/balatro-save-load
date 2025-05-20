<#
.SYNOPSIS
    Gets a work item based on the ID provided.
.DESCRIPTION
    This script retrieves a work item from the workspace based on the ID provided.
    The ID should be in the format of 'EP001', 'FT002', etc.
    It searches for markdown files in the specified directory and its subdirectories.
.EXAMPLE
    .\Get-WorkItem.ps1 -ID 'EP001'
    This command retrieves the work item(s) with ID 'EP001'.

#>
param(
  [Parameter(Mandatory = $true)]
  [ValidateScript({
    if ($_ -match '^[A-Z]{2,3}[0-9]{3,5}$') {
      return $true
    } else {
      throw "Invalid ID format. Expected format: 'EP001', 'FT002', etc."
    }
  })]
  [string]$ID
)

function Get-WorkItem {
  [CmdletBinding()]
  param (
    [Parameter(Mandatory = $true)]
    [ValidateScript({
      if ($_ -match '^[A-Z]{2,3}[0-9]{3,5}$') {
        return $true
      } else {
        throw "Invalid ID format. Expected format: 'EP001', 'FT002', etc."
      }
    })]
    [string]$ID
  )
  begin{
    $workRoot = "$PSScriptRoot\..\"
  }
  process{
    $workItem = Get-ChildItem -Path $workRoot -Recurse -Filter "$ID*.md" -ErrorAction SilentlyContinue
    if ($workItem) {
      $workItem | ForEach-Object {
        Write-Host "Found work item: $($_.FullName)"
        # Add any additional processing you want to do with the work item here
      }
    } else {
      Write-Host "No work item found with ID: $ID"
    }
  }
  end{
    Write-Host "Finished searching for work item."
  }
}

Get-WorkItem -ID $ID