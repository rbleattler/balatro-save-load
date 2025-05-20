#!/bin/bash

# This script consolidates duplicate work items with different naming conventions
# It identifies and consolidates work items that have the same ID but different names

# Usage: ./consolidate_work_items.sh
# Example: ./consolidate_work_items.sh

# Exit immediately if a command exits with a non-zero status.
set -e

# Store the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
# Navigate to the work-management directory
WORK_MGMT_DIR="$SCRIPT_DIR/.."
BACKLOG_DIR="$WORK_MGMT_DIR/backlog"
OPEN_DIR="$WORK_MGMT_DIR/open"
CLOSED_DIR="$WORK_MGMT_DIR/closed"

# Function to standardize work item filenames
get_standardized_work_item_name() {
    local item_id="$1"
    local title="$2"

    # Convert spaces and special characters to dashes, remove multiple dashes, and trim
    local clean_title=$(echo "$title" | sed 's/:/-/g' | sed 's/ /-/g' | sed 's/[^a-zA-Z0-9-]//g' | sed 's/--/-/g')
    echo "${item_id}-${clean_title}.md"
}

# Function to parse work item title from content
get_work_item_title() {
    local content="$1"

    # Extract title from markdown header
    if [[ $content =~ \#\ [A-Za-z0-9]+:\ (.+) ]]; then
        echo "${BASH_REMATCH[1]}" | xargs
    else
        echo ""
    fi
}

# Function to merge content from duplicate work items
merge_work_item_content() {
    local files=("$@")
    local all_contents=()

    # Get the content from each file
    for file in "${files[@]}"; do
        if [ -f "$file" ]; then
            content=$(cat "$file")
            all_contents+=("$content")
        fi
    done

    # Find the longest content (assuming it's the most complete)
    local longest_content=""
    local max_length=0

    for content in "${all_contents[@]}"; do
        length=${#content}
        if (( length > max_length )); then
            max_length=$length
            longest_content="$content"
        fi
    done

    echo "$longest_content"
}

# Function to process directories and consolidate work items
consolidate_directory_work_items() {
    local directory="$1"

    echo "Checking for duplicate work items in $directory directory..."

    # Create a map of work item IDs to file paths
    declare -A item_groups

    for file in "$directory"/*.md; do
        if [ -f "$file" ]; then
            filename=$(basename "$file")
            # Extract the work item ID from the filename
            if [[ $filename =~ ^(EP|FT|US|TSK)[0-9]{3} ]]; then
                item_id="${BASH_REMATCH[0]}"

                if [ -z "${item_groups[$item_id]}" ]; then
                    item_groups[$item_id]="$file"
                else
                    item_groups[$item_id]="${item_groups[$item_id]} $file"
                fi
            fi
        fi
    done

    # Process each group of files
    for item_id in "${!item_groups[@]}"; do
        # Convert the space-separated string to an array
        IFS=' ' read -ra item_files <<< "${item_groups[$item_id]}"

        # Skip if there's only one file for this ID
        if [ ${#item_files[@]} -le 1 ]; then
            continue
        fi

        echo "Found ${#item_files[@]} files for work item $item_id"

        # Merge the content from all files
        merged_content=$(merge_work_item_content "${item_files[@]}")

        # Parse the title from the merged content
        title=$(get_work_item_title "$merged_content")

        if [ -z "$title" ]; then
            echo "Warning: Could not parse title for work item $item_id, skipping consolidation"
            continue
        fi

        # Generate a standardized filename
        standardized_name=$(get_standardized_work_item_name "$item_id" "$title")
        target_path="$directory/$standardized_name"

        # Create the consolidated file
        echo "$merged_content" > "$target_path"

        # Delete the original files except the one we just created
        for file in "${item_files[@]}"; do
            if [ "$file" != "$target_path" ]; then
                filename=$(basename "$file")
                echo "Removing duplicate file: $filename"
                rm "$file"
            fi
        done

        echo "Consolidated work item $item_id into $standardized_name"
    done
}

# Process each directory
consolidate_directory_work_items "$BACKLOG_DIR"
consolidate_directory_work_items "$OPEN_DIR"
consolidate_directory_work_items "$CLOSED_DIR"

echo "Work item consolidation completed."
