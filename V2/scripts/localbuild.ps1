# runs local builds with outputs to files for copilot analysis
# redirects all output to $PWD/build_output.txt (overwrite)
# redirects all errors to $PWD/errors.txt (overwrite)
# redirects all warnings to $PWD/warnings.txt (overwrite)

param(
  [switch] $Restore,
  [switch] $Clean,
  [switch] $Build,
  [Parameter()]
  [ValidateSet("Debug", "Release")]
  [string] $Configuration,
  [int] $WarnLevel = 4
)
begin {

  $script:SlnRoot = "$PSScriptRoot/../"
  $script:BuildOutput = "$PSScriptRoot/build_output.txt"
  $script:ErrorOutput = "$PSScriptRoot/errors.txt"
  $script:WarningOutput = "$PSScriptRoot/warnings.txt"

  Push-Location $script:SlnRoot
  Write-Verbose "Current Location $PWD"

  function errorsOnly {
    [CmdletBinding()]
    param(
      [Parameter(ValueFromPipeline = $true)]
      [string[]]
      $inputToFilter
    )
    Write-Verbose "items: $($inputToFilter.Count)"
    Write-Verbose "$($inputToFilter[0])"
    $filteredOutput = ($inputToFilter | Where-Object {
        $PSItem -like "*error*" -and $PSItem -notlike "*warning*" -and $PSItem -notlike "*info*"
      }) -join "`n"
    return $filteredOutput
  }

  function warningsOnly {
    [CmdletBinding()]
    param(
      [Parameter(ValueFromPipeline = $true)]
      [string[]]
      $inputToFilter
    )
    $filteredOutput = ($inputToFilter | Where-Object {
        $PSItem -like "*warning*" -and $PSItem -notlike "*error*" -and $PSItem -notlike "*info*"
      }) -join "`n"
    return $filteredOutput
  }

  function infoOnly {
    [CmdletBinding()]
    param(
      [Parameter(ValueFromPipeline = $true)]
      [string[]]
      $inputToFilter
    )
    $filteredOutput = ($inputToFilter | Where-Object {
        $PSItem -like "*info*" -and $PSItem -notlike "*error*" -and $PSItem -notlike "*warning*"
      }) -join "`n"
    return $filteredOutput
  }
}
process {

  # Process the arguments and set flags accordingly

  if ($Clean) {
    Write-Verbose "Cleaning solution..."

    $rawOut = dotnet 'clean'
    $rawOut | Out-File -FilePath $script:BuildOutput -Force
    errorsOnly $rawOut | Out-File -FilePath $script:ErrorOutput -Force
    warningsOnly $rawOut | Out-File -FilePath $script:WarningOutput -Force
    # $rawOut
  }

  if ($Restore) {
    Write-Verbose "Restoring packages..."

    $rawOut = dotnet 'restore'
    $rawOut | Out-File -FilePath $script:BuildOutput -Force
    errorsOnly $rawOut | Out-File -FilePath $script:ErrorOutput -Force
    warningsOnly $rawOut | Out-File -FilePath $script:WarningOutput -Force
    $rawOut
  }

  if ($Build) {
    Write-Verbose "Building solution..."

    $rawOut = dotnet build BalatroSaveToolkit.sln #--configuration $Configuration --property WarningLevel=$WarnLevel
    $rawOut | Out-File -FilePath $script:BuildOutput -Force
    errorsOnly $rawOut | Out-File -FilePath $script:ErrorOutput -Force
    warningsOnly $rawOut | Out-File -FilePath $script:WarningOutput -Force
    $rawOut
  }

}
end {
  Pop-Location

}
clean {

}

