#!/usr/bin/env bash

# .SYNOPSIS
#     Gets a work item based on the ID provided.
# .DESCRIPTION
#     This script retrieves a work item from the workspace based on the ID provided.
#     The ID should be in the format of 'EP001', 'FT002', etc.
#     It searches for markdown files in the specified directory and its subdirectories.
# .EXAMPLE
#     ./get_workitem.sh -i 'EP001'
#     This command retrieves the work item(s) with ID 'EP001'.

# Function to validate the ID format
validate_id() {
  local id="$1"
  if [[ "$id" =~ ^[A-Z]{2,3}[0-9]{3,5}$ ]]; then
    return 0 # Success
  else
    return 1 # Failure
  fi
}

# Function to get the work item
get_workitem() {
  local id="$1"
  local work_root="../"

  # echo "Searching for work item with ID: $id"

  # Validate the ID
  # echo "Validating ID format..."
  if ! validate_id "$id"; then
    echo "Invalid ID format. Expected format: 'EP001', 'FT002', etc."
    return 1
  fi
  # echo "ID format is valid."

  # Find the work item(s) and store them in an array
  # echo "mapping workitems"
  # Find the work item(s) and store them in an array
  mapfile -t work_items < <(find "$(dirname "${BASH_SOURCE[0]}")/../" -name "$id*.md")

  # echo "beginning outputs"
  if [ "${#work_items[@]}" -eq 0 ]; then
    echo "No work item found with ID: $id"
  else
    for item in "${work_items[@]}"; do
      echo "$item"
    done
  fi
  # echo "Finished searching for work item."
  return 0
}

# echo "Starting work item retrieval script..."

# Read command-line arguments
while getopts ":i:" opt; do
  # echo "Processing option: -$opt with argument: $OPTARG"
  case $opt in
  i)
    # echo "Option -$opt with argument: $OPTARG"
    ID="$OPTARG"
    ;;
  \?)
    # echo "Invalid option: -$opt" >&2
    exit 1
    ;;
  :)
    # echo "Option -$opt requires an argument." >&2
    exit 1
    ;;
  esac
done

# echo "broke the loop"

# Check if ID is provided
if [ -z "$ID" ]; then
  echo "ID is required. Use -i option to provide the ID."
  exit 1
fi

# Get the work item
get_workitem "$ID"
