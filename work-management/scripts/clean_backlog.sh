#!/bin/bash

# This script cleans up duplicate work items in the backlog directory
# by checking if they have been moved to the open or closed directories

# Usage: ./clean_backlog.sh
# Example: ./clean_backlog.sh

# Exit immediately if a command exits with a non-zero status.
set -e

# Store the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
# Navigate to the work-management directory
WORK_MGMT_DIR="$SCRIPT_DIR/.."
BACKLOG_DIR="$WORK_MGMT_DIR/backlog"
OPEN_DIR="$WORK_MGMT_DIR/open"
CLOSED_DIR="$WORK_MGMT_DIR/closed"

echo "Checking for work items that can be removed from backlog..."

# Get all work item IDs from backlog directory
backlog_items=()
for file in "$BACKLOG_DIR"/*.md; do
    if [ -f "$file" ]; then
        # Extract the work item ID from the filename
        filename=$(basename "$file")
        item_id=$(echo "$filename" | grep -oP '^(EP|FT|US|TSK)[0-9]{3}')
        if [ -n "$item_id" ]; then
            backlog_items+=("$item_id")
        fi
    fi
done

items_removed=0
items_checked=0

# Check each backlog item if it exists in open or closed directories
for item_id in "${backlog_items[@]}"; do
    items_checked=$((items_checked + 1))

    # Check if the work item exists in open or closed directories
    open_exists=false
    closed_exists=false

    # Check open directory
    for file in "$OPEN_DIR"/${item_id}*.md; do
        if [ -f "$file" ]; then
            open_exists=true
            break
        fi
    done

    # Check closed directory
    for file in "$CLOSED_DIR"/${item_id}*.md; do
        if [ -f "$file" ]; then
            closed_exists=true
            break
        fi
    done

    # If the work item exists in open or closed directories, remove it from backlog
    if [ "$open_exists" = true ] || [ "$closed_exists" = true ]; then
        # Find all matching files in backlog
        for file in "$BACKLOG_DIR"/${item_id}*.md; do
            if [ -f "$file" ]; then
                filename=$(basename "$file")

                if [ "$open_exists" = true ]; then
                    echo "Work item $item_id is in OPEN directory. Removing $filename from backlog."
                else
                    echo "Work item $item_id is in CLOSED directory. Removing $filename from backlog."
                fi

                # Remove the file from backlog
                rm "$file"
                items_removed=$((items_removed + 1))
            fi
        done
    fi
done

echo "Backlog cleanup completed. Checked $items_checked work items, removed $items_removed duplicates."