#!/bin/bash

# This script generates a report of all work items in the workspace
# showing counts by type and status

# Usage: ./work_status_report.sh
# Example: ./work_status_report.sh

# Exit immediately if a command exits with a non-zero status.
set -e

# Store the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
# Navigate to the work-management directory
WORK_MGMT_DIR="$SCRIPT_DIR/.."
BACKLOG_DIR="$WORK_MGMT_DIR/backlog"
OPEN_DIR="$WORK_MGMT_DIR/open"
CLOSED_DIR="$WORK_MGMT_DIR/closed"

# Initialize counters for each work item type and state
EP_backlog=0
EP_open=0
EP_closed=0

FT_backlog=0
FT_open=0
FT_closed=0

US_backlog=0
US_open=0
US_closed=0

TSK_backlog=0
TSK_open=0
TSK_closed=0

# Count items in backlog
for file in "$BACKLOG_DIR"/*.md; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        if [[ $filename =~ ^EP[0-9]{3} ]]; then
            EP_backlog=$((EP_backlog + 1))
        elif [[ $filename =~ ^FT[0-9]{3} ]]; then
            FT_backlog=$((FT_backlog + 1))
        elif [[ $filename =~ ^US[0-9]{3} ]]; then
            US_backlog=$((US_backlog + 1))
        elif [[ $filename =~ ^TSK[0-9]{3} ]]; then
            TSK_backlog=$((TSK_backlog + 1))
        fi
    fi
done

# Count items in open
for file in "$OPEN_DIR"/*.md; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        if [[ $filename =~ ^EP[0-9]{3} ]]; then
            EP_open=$((EP_open + 1))
        elif [[ $filename =~ ^FT[0-9]{3} ]]; then
            FT_open=$((FT_open + 1))
        elif [[ $filename =~ ^US[0-9]{3} ]]; then
            US_open=$((US_open + 1))
        elif [[ $filename =~ ^TSK[0-9]{3} ]]; then
            TSK_open=$((TSK_open + 1))
        fi
    fi
done

# Count items in closed
for file in "$CLOSED_DIR"/*.md; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        if [[ $filename =~ ^EP[0-9]{3} ]]; then
            EP_closed=$((EP_closed + 1))
        elif [[ $filename =~ ^FT[0-9]{3} ]]; then
            FT_closed=$((FT_closed + 1))
        elif [[ $filename =~ ^US[0-9]{3} ]]; then
            US_closed=$((US_closed + 1))
        elif [[ $filename =~ ^TSK[0-9]{3} ]]; then
            TSK_closed=$((TSK_closed + 1))
        fi
    fi
done

# Calculate totals
EP_total=$((EP_backlog + EP_open + EP_closed))
FT_total=$((FT_backlog + FT_open + FT_closed))
US_total=$((US_backlog + US_open + US_closed))
TSK_total=$((TSK_backlog + TSK_open + TSK_closed))

total_backlog=$((EP_backlog + FT_backlog + US_backlog + TSK_backlog))
total_open=$((EP_open + FT_open + US_open + TSK_open))
total_closed=$((EP_closed + FT_closed + US_closed + TSK_closed))
total_all=$((total_backlog + total_open + total_closed))

# Calculate percentages for closed items
if [ $EP_total -gt 0 ]; then
    EP_percent=$((EP_closed * 100 / EP_total))
else
    EP_percent=0
fi

if [ $FT_total -gt 0 ]; then
    FT_percent=$((FT_closed * 100 / FT_total))
else
    FT_percent=0
fi

if [ $US_total -gt 0 ]; then
    US_percent=$((US_closed * 100 / US_total))
else
    US_percent=0
fi

if [ $TSK_total -gt 0 ]; then
    TSK_percent=$((TSK_closed * 100 / TSK_total))
else
    TSK_percent=0
fi

if [ $total_all -gt 0 ]; then
    total_percent=$((total_closed * 100 / total_all))
else
    total_percent=0
fi

# Format the output as a table
printf "\n%s\n" "=== Balatro Save and Load Tool - Work Status Report ==="
printf "%s\n" "$(date '+%Y-%m-%d %H:%M:%S')"
printf "\n"
printf "%-10s | %-10s | %-10s | %-10s | %-10s | %-10s\n" "Type" "Backlog" "Open" "Closed" "Total" "% Complete"
printf "%s\n" "------------------------------------------------------------"
printf "%-10s | %-10s | %-10s | %-10s | %-10s | %-10s\n" "Epics" "$EP_backlog" "$EP_open" "$EP_closed" "$EP_total" "$EP_percent%"
printf "%-10s | %-10s | %-10s | %-10s | %-10s | %-10s\n" "Features" "$FT_backlog" "$FT_open" "$FT_closed" "$FT_total" "$FT_percent%"
printf "%-10s | %-10s | %-10s | %-10s | %-10s | %-10s\n" "Stories" "$US_backlog" "$US_open" "$US_closed" "$US_total" "$US_percent%"
printf "%-10s | %-10s | %-10s | %-10s | %-10s | %-10s\n" "Tasks" "$TSK_backlog" "$TSK_open" "$TSK_closed" "$TSK_total" "$TSK_percent%"
printf "%s\n" "------------------------------------------------------------"
printf "%-10s | %-10s | %-10s | %-10s | %-10s | %-10s\n" "TOTAL" "$total_backlog" "$total_open" "$total_closed" "$total_all" "$total_percent%"
printf "\n"

# Optional: Generate a list of in-progress items
if [ $total_open -gt 0 ]; then
    printf "%s\n" "=== Currently Open Work Items ==="

    # Print epics
    if [ $EP_open -gt 0 ]; then
        printf "%s\n" "--- Epics ---"
        for file in "$OPEN_DIR"/EP*.md; do
            if [ -f "$file" ]; then
                filename=$(basename "$file")
                title=$(grep -m 1 "^# " "$file" | sed 's/^# //')
                printf "%-10s: %s\n" "${filename%.*}" "$title"
            fi
        done
        printf "\n"
    fi

    # Print features
    if [ $FT_open -gt 0 ]; then
        printf "%s\n" "--- Features ---"
        for file in "$OPEN_DIR"/FT*.md; do
            if [ -f "$file" ]; then
                filename=$(basename "$file")
                title=$(grep -m 1 "^# " "$file" | sed 's/^# //')
                printf "%-10s: %s\n" "${filename%.*}" "$title"
            fi
        done
        printf "\n"
    fi

    # Print user stories
    if [ $US_open -gt 0 ]; then
        printf "%s\n" "--- User Stories ---"
        for file in "$OPEN_DIR"/US*.md; do
            if [ -f "$file" ]; then
                filename=$(basename "$file")
                title=$(grep -m 1 "^# " "$file" | sed 's/^# //')
                printf "%-10s: %s\n" "${filename%.*}" "$title"
            fi
        done
        printf "\n"
    fi

    # Print tasks
    if [ $TSK_open -gt 0 ]; then
        printf "%s\n" "--- Tasks ---"
        for file in "$OPEN_DIR"/TSK*.md; do
            if [ -f "$file" ]; then
                filename=$(basename "$file")
                title=$(grep -m 1 "^# " "$file" | sed 's/^# //')
                printf "%-10s: %s\n" "${filename%.*}" "$title"
            fi
        done
        printf "\n"
    fi
fi

echo "Report completed."
