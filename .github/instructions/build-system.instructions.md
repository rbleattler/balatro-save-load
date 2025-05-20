---
applyTo: '**'
---
# Required Workflow

**CRITICAL**: You MUST read `progress-summary.md` before starting any work to understand the current project state.

**MANDATORY**: After making substantial changes or completing work items, you MUST update `progress-summary.md` so the next contributor can easily pick up where you left off. This file serves as the primary handoff document between different contributors.

## Build and Testing

The project includes a PowerShell script called `localbuild.ps1` in the `V2/scripts` directory, which can be used to build the solution and capture output for analysis.

### Using the Build Script

- **To restore packages**:

  ```powershell
  .\V2\scripts\localbuild.ps1 -Restore
  ```

- **To clean the solution**:

  ```powershell
  .\V2\scripts\localbuild.ps1 -Clean
  ```

- **To build the solution**:

  ```powershell
  .\V2\scripts\localbuild.ps1 -Build
  ```

- **You can combine these operations**:

  ```powershell
  .\V2\scripts\localbuild.ps1 -Clean -Restore -Build
  ```

### Local Build Script

The `localbuild.ps1` script in the `V2/scripts` directory is a critical tool for building and troubleshooting the project. It provides structured output to help identify and fix issues efficiently.

#### Script Parameters

The build script accepts the following parameters:

- `-Restore`: Restores NuGet packages for the solution
- `-Clean`: Cleans the solution, removing build artifacts
- `-Build`: Builds the solution
- `-Configuration [Debug|Release]`: Specifies the build configuration (defaults to Debug)
- `-WarnLevel [0-4]`: Sets the warning level for the compiler (defaults to 4)

#### Output Files

When the script runs, it generates three output files in the `V2/scripts` directory:

1. **`build_output.txt`**: Contains the complete, unfiltered build output including all information, warnings, and errors.
2. **`errors.txt`**: Contains only error messages extracted from the build output. This should be your first stop when troubleshooting build failures.
3. **`warnings.txt`**: Contains only warning messages from the build. These should be reviewed and addressed to prevent future issues.

#### Recommended Usage

1. When starting work on the codebase, run a full clean, restore, and build:

   ```powershell
   .\V2\scripts\localbuild.ps1 -Clean -Restore -Build
   ```

2. For subsequent builds during development, you can typically just use:

   ```powershell
   .\V2\scripts\localbuild.ps1 -Build
   ```

3. If you update NuGet package references, make sure to include the `-Restore` flag:

   ```powershell
   .\V2\scripts\localbuild.ps1 -Restore -Build
   ```

#### Troubleshooting Build Failures

When a build fails, follow this process:

1. **Examine `errors.txt` first**: This file contains all errors that caused the build to fail. Fix these issues first.

2. **Review `warnings.txt`**: After fixing errors, check warnings to identify potential problems or code quality issues.

3. **Consult `build_output.txt`**: If you need more context about an error or warning, check the complete build output.

4. **Iterative fixes**: Make small, focused changes to fix one error at a time, then rebuild to validate the fix.

5. **Verification**: After all errors and warnings are addressed, run a full clean, restore, and build to verify everything works correctly:

   ```powershell
   .\V2\scripts\localbuild.ps1 -Clean -Restore -Build
   ```

Remember to update `progress-summary.md` with any significant changes or fixes you make during the build troubleshooting process.

### Analyzing Build Output

The script generates three output files in the `V2/scripts` directory:

1. **`build_output.txt`** - Contains the complete build output
2. **`errors.txt`** - Contains only error messages from the build
3. **`warnings.txt`** - Contains only warning messages from the build

When troubleshooting build issues:

1. First check `errors.txt` for compilation errors that need to be fixed
2. Then review `warnings.txt` to address any warnings that might cause problems
3. For more context, refer to the complete `build_output.txt` file

Always run the build script after making significant changes to verify that the code compiles correctly.
