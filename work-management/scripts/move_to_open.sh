#!/bin/bash

# This script moves the specified work item(s) from the backlog directory to the open directory
# and updates the status in the file to "Open"

# Usage: ./move_to_open.sh [--consolidate] <work_item_id> [<work_item_id> ...]
# Example: ./move_to_open.sh TSK001 TSK002
# Example: ./move_to_open.sh US001 FT001
# Example: ./move_to_open.sh --consolidate TSK001 TSK002

# Exit immediately if a command exits with a non-zero status.
set -e

# Store the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
# Navigate to the work-management directory
WORK_MGMT_DIR="$SCRIPT_DIR/.."
BACKLOG_DIR="$WORK_MGMT_DIR/backlog"
OPEN_DIR="$WORK_MGMT_DIR/open"

# Check if at least one work item ID was provided
if [ $# -eq 0 ]; then
    echo "Error: No work item ID(s) provided"
    echo "Usage: ./move_to_open.sh <work_item_id> [<work_item_id> ...]"
    echo "Example: ./move_to_open.sh TSK001 TSK002"
    exit 1
fi

# Process each provided work item ID
for item_id in "$@"; do
    # Validate work item ID format
    if ! [[ $item_id =~ ^(EP|FT|US|TSK)[0-9]{3}$ ]]; then
        echo "Warning: Invalid work item ID format: $item_id. Skipping..."
        continue
    fi

    # Find the matching file in the backlog directory
    found_files=()
    for file in "$BACKLOG_DIR"/${item_id}*.md; do
        if [ -f "$file" ]; then
            found_files+=("$file")
        fi
    done

    # Check if we found any matching files
    if [ ${#found_files[@]} -eq 0 ]; then
        echo "Warning: No file found for work item $item_id in backlog directory. Skipping..."
        continue
    elif [ ${#found_files[@]} -gt 1 ]; then
        echo "Multiple files found for work item $item_id:"
        for ((i=0; i<${#found_files[@]}; i++)); do
            echo "[$i] ${found_files[$i]##*/}"
        done
        echo "Please enter the number of the file you want to move (or 'all' to move all):"
        read -r selection

        if [ "$selection" = "all" ]; then
            selected_files=("${found_files[@]}")
        elif [[ $selection =~ ^[0-9]+$ ]] && [ "$selection" -lt ${#found_files[@]} ]; then
            selected_files=("${found_files[$selection]}")
        else
            echo "Invalid selection. Skipping..."
            continue
        fi
    else
        selected_files=("${found_files[0]}")
    fi

    # Process each selected file
    for file in "${selected_files[@]}"; do
        filename=$(basename "$file")
        target_file="$OPEN_DIR/$filename"

        echo "Moving $filename to open directory..."

        # Update the status in the file content
        sed -i 's/^Status:.*$/Status: Open/' "$file"

        # Add start date if it doesn't exist
        if ! grep -q "^Start Date:" "$file"; then
            current_date=$(date +"%Y-%m-%d")
            # Find the line with "Status:" and append Start Date after it
            sed -i "/^Status:/a Start Date: $current_date" "$file"
        fi

        # Move the file to the open directory
        mv "$file" "$target_file"

        echo "Successfully moved and updated $filename"

        # Update parent work items if any
        if grep -q "^Parent:" "$target_file"; then
            parent_id=$(grep "^Parent:" "$target_file" | sed 's/^Parent:[ \t]*\([^ \t]*\).*/\1/')

            if [[ $parent_id =~ ^(EP|FT|US)[0-9]{3}$ ]]; then
                echo "Found parent work item: $parent_id. Updating parent status..."

                # Look for parent in both open and backlog directories
                for parent_dir in "$OPEN_DIR" "$BACKLOG_DIR"; do
                    for parent_file in "$parent_dir"/${parent_id}*.md; do
                        if [ -f "$parent_file" ]; then
                            echo "Updating parent file: $parent_file"

                            # Update the child item status in parent file
                            if grep -q "$item_id" "$parent_file"; then
                                sed -i "s/$item_id - [^O].*/$item_id - Open/" "$parent_file"
                                echo "Updated child status in parent file"
                            fi

                            # If parent is in backlog, consider moving it to open
                            if [[ "$parent_file" == "$BACKLOG_DIR"/* ]]; then
                                echo "Parent work item is still in backlog. Consider moving it to open as well."
                            fi
                        fi
                    done
                done
            fi
        fi
    done
done

# Check if consolidation was requested
if [ "$1" == "--consolidate" ]; then
    echo "Running work item consolidation to handle duplicates..."
    if [ -f "$SCRIPT_DIR/consolidate_work_items.sh" ]; then
        bash "$SCRIPT_DIR/consolidate_work_items.sh"
    else
        echo "Warning: consolidate_work_items.sh script not found. Skipping consolidation."
    fi
fi

echo "Operation completed."