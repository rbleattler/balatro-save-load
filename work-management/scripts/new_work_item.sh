#!/bin/bash

# This script creates a new work item in the backlog directory

# Usage: ./new_work_item.sh [options]
# Options:
#   -t, --type TYPE           Type of work item (Epic, Feature, UserStory, Task)
#   -n, --name TITLE          Title of the work item
#   -p, --parent ID           Parent work item ID (e.g. EP001, FT002)
#   -d, --description DESC    Description of the work item (optional)
#   -a, --acceptance CRITERIA Acceptance criteria (can be used multiple times)
# Examples:
#   ./new_work_item.sh -t Epic -n "Avalonia Migration"
#   ./new_work_item.sh -t Feature -n "Project Setup" -p EP001
#   ./new_work_item.sh -t UserStory -n "Create Solution" -p FT001 -a "Solution file created" -a "Projects added"
#   ./new_work_item.sh -t Task -n "Add Packages" -p US001 -d "Add Avalonia packages to project" -a "Packages added"

# Set default values
TYPE=""
TITLE=""
PARENT=""
DESCRIPTION=""
ACCEPTANCE_CRITERIA=()

# Store the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
# Navigate to the work-management directory
WORK_MGMT_DIR="$SCRIPT_DIR/.."
BACKLOG_DIR="$WORK_MGMT_DIR/backlog"
OPEN_DIR="$WORK_MGMT_DIR/open"
CLOSED_DIR="$WORK_MGMT_DIR/closed"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
  key="$1"

  case $key in
    -t|--type)
      TYPE="$2"
      shift # past argument
      shift # past value
      ;;
    -n|--name)
      TITLE="$2"
      shift # past argument
      shift # past value
      ;;
    -p|--parent)
      PARENT="$2"
      shift # past argument
      shift # past value
      ;;
    -d|--description)
      DESCRIPTION="$2"
      shift # past argument
      shift # past value
      ;;
    -a|--acceptance)
      ACCEPTANCE_CRITERIA+=("$2")
      shift # past argument
      shift # past value
      ;;
    *)
      # unknown option
      echo "Unknown option: $1"
      echo "Usage: ./new_work_item.sh [options]"
      echo "Run ./new_work_item.sh without arguments to see help"
      exit 1
      ;;
  esac
done

# Validate required parameters
if [ -z "$TYPE" ]; then
  echo "Error: Type is required"
  echo "Usage: ./new_work_item.sh -t TYPE -n TITLE [-p PARENT] [-d DESCRIPTION] [-a ACCEPTANCE]..."
  exit 1
fi

if [ -z "$TITLE" ]; then
  echo "Error: Title is required"
  echo "Usage: ./new_work_item.sh -t TYPE -n TITLE [-p PARENT] [-d DESCRIPTION] [-a ACCEPTANCE]..."
  exit 1
fi

# Convert type to proper prefix
case "${TYPE,,}" in
  epic|ep)
    TYPE_PREFIX="EP"
    TYPE_FULL="Epic"
    ;;
  feature|ft)
    TYPE_PREFIX="FT"
    TYPE_FULL="Feature"
    ;;
  userstory|us|story)
    TYPE_PREFIX="US"
    TYPE_FULL="User Story"
    ;;
  task|tsk)
    TYPE_PREFIX="TSK"
    TYPE_FULL="Task"
    ;;
  *)
    echo "Error: Invalid type: $TYPE"
    echo "Valid types: Epic, Feature, UserStory, Task"
    exit 1
    ;;
esac

# Validate parent based on type
if [ "$TYPE_PREFIX" != "EP" ] && [ -z "$PARENT" ]; then
    echo "Error: Parent is required for $TYPE_FULL work items"
    exit 1
fi

if [ -n "$PARENT" ] && [ "$TYPE_PREFIX" = "EP" ]; then
    echo "Warning: Parent should not be specified for Epics. Ignoring parent."
    PARENT=""
fi

if [ -n "$PARENT" ] && [ "$TYPE_PREFIX" = "FT" ] && [[ ! "$PARENT" =~ ^EP[0-9]{3}$ ]]; then
    echo "Error: Parent of a Feature must be an Epic (EP###)"
    exit 1
fi

if [ -n "$PARENT" ] && [ "$TYPE_PREFIX" = "US" ] && [[ ! "$PARENT" =~ ^FT[0-9]{3}$ ]]; then
    echo "Error: Parent of a User Story must be a Feature (FT###)"
    exit 1
fi

if [ -n "$PARENT" ] && [ "$TYPE_PREFIX" = "TSK" ] && [[ ! "$PARENT" =~ ^US[0-9]{3}$ ]]; then
    echo "Error: Parent of a Task must be a User Story (US###)"
    exit 1
fi

# Find the next available ID
max_id=0
# Check backlog dir
for file in "$BACKLOG_DIR"/${TYPE_PREFIX}*.md; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        id_num=$(echo "$filename" | grep -oP "${TYPE_PREFIX}\K[0-9]{3}")
        if [ -n "$id_num" ] && [ "$id_num" -gt "$max_id" ]; then
            max_id=$id_num
        fi
    fi
done

# Check open dir
for file in "$OPEN_DIR"/${TYPE_PREFIX}*.md; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        id_num=$(echo "$filename" | grep -oP "${TYPE_PREFIX}\K[0-9]{3}")
        if [ -n "$id_num" ] && [ "$id_num" -gt "$max_id" ]; then
            max_id=$id_num
        fi
    fi
done

# Check closed dir
for file in "$CLOSED_DIR"/${TYPE_PREFIX}*.md; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        id_num=$(echo "$filename" | grep -oP "${TYPE_PREFIX}\K[0-9]{3}")
        if [ -n "$id_num" ] && [ "$id_num" -gt "$max_id" ]; then
            max_id=$id_num
        fi
    fi
done

# Calculate next ID
next_id=$((max_id + 1))
formatted_id=$(printf "%s%03d" "$TYPE_PREFIX" "$next_id")
sanitized_title=$(echo "$TITLE" | tr -d '\\/:*?"<>|' | tr ' ' '-')
file_name="${formatted_id}-${sanitized_title}.md"
file_path="$BACKLOG_DIR/$file_name"

# Create the file content
creation_date=$(date +"%Y-%m-%d")
content="# $TITLE\n\n"
content+="ID: $formatted_id\n"
content+="Type: $TYPE_FULL\n"
content+="Status: Backlog\n"
content+="Creation Date: $creation_date\n"

if [ -n "$PARENT" ]; then
    content+="Parent: $PARENT\n"
fi

content+="\n## Description\n\n"
if [ -n "$DESCRIPTION" ]; then
    content+="$DESCRIPTION\n"
else
    content+="TO BE COMPLETED\n"
fi

if [ "$TYPE_PREFIX" = "EP" ]; then
    content+="\n## Goals\n\n- TO BE ADDED\n"
    content+="\n## Features\n\n- None yet\n"
elif [ "$TYPE_PREFIX" = "FT" ]; then
    content+="\n## User Stories\n\n- None yet\n"
elif [ "$TYPE_PREFIX" = "US" ] || [ "$TYPE_PREFIX" = "TSK" ]; then
    content+="\n## Acceptance Criteria\n\n"

    if [ ${#ACCEPTANCE_CRITERIA[@]} -gt 0 ]; then
        for criterion in "${ACCEPTANCE_CRITERIA[@]}"; do
            content+="- [ ] $criterion\n"
        done
    else
        content+="- [ ] TO BE ADDED\n"
    fi
fi

if [ "$TYPE_PREFIX" = "US" ]; then
    content+="\n## Tasks\n\n- None yet\n"
fi

# Write the file
echo -e "$content" > "$file_path"

echo "Created new $TYPE_FULL work item: $formatted_id - $TITLE"
echo "File: $file_path"

# Update parent work item if specified
if [ -n "$PARENT" ]; then
    parent_file=""
    for file in "$BACKLOG_DIR"/${PARENT}*.md "$OPEN_DIR"/${PARENT}*.md; do
        if [ -f "$file" ]; then
            parent_file="$file"
            break
        fi
    done

    if [ -n "$parent_file" ]; then
        case "$TYPE_PREFIX" in
            "FT")
                section="## Features"
                ;;
            "US")
                section="## User Stories"
                ;;
            "TSK")
                section="## Tasks"
                ;;
        esac

        # Check if section exists and contains "None yet"
        if grep -q "$section\s*\n\s*- None yet" "$parent_file"; then
            sed -i "s/$section\\s*\n\\s*- None yet/$section\n\n- $formatted_id - Backlog - $TITLE/" "$parent_file"
        # Check if section exists
        elif grep -q "$section" "$parent_file"; then
            sed -i "/$section/a - $formatted_id - Backlog - $TITLE" "$parent_file"
        # If section doesn't exist, add it
        else
            echo -e "\n$section\n\n- $formatted_id - Backlog - $TITLE" >> "$parent_file"
        fi

        echo "Updated parent work item: $PARENT with reference to $formatted_id"
    else
        echo "Warning: Could not find parent work item file for $PARENT. Parent was not updated."
    fi
fi

# Return the created ID
echo "$formatted_id"
