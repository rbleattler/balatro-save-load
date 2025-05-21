#!/usr/bin/env bash

# Strict mode
set -euo pipefail

# .SYNOPSIS
#     Finds duplicate work items by ID and/or name in the work-management system.
# .DESCRIPTION
#     This script scans all work item markdown files in the backlog, open, and closed directories
#     of the work-management system. It compares work items by ID and by name (using a
#     similarity algorithm) to detect duplicates or near-duplicates. Optionally, it can
#     compare file contents for further similarity analysis. The script outputs a report of
#     matches and suspected matches, with options for table, or markdown output.
# .PARAMETER -c
#     If specified, also compares the contents of files for similarity (not just names).
# .PARAMETER -t
#     If specified, outputs the results as a formatted table.
# .PARAMETER -g
#     If specified, outputs the results to console (GridView is PowerShell specific).
# .PARAMETER -m
#     If specified, outputs the results as a markdown file (duplicates.md).
# .EXAMPLE
#     ./find_duplicates.sh
#     Runs the script and outputs duplicate work items to the console.
# .EXAMPLE
#     ./find_duplicates.sh -c -t
#     Runs the script, compares file contents, and outputs results as a table.
# .NOTES
#     Requires the get_similarity.sh script in the same directory. Uses Levenshtein distance for similarity.
#     Designed for use in the Balatro Save and Load Tool work-management system.
#     Requires 'bc' for floating point calculations.

usage() {
    echo "Usage: $0 [-c] [-t] [-g] [-m]"
    echo "  -c: Compare contents of files for similarity."
    echo "  -t: Output results as a formatted table."
    echo "  -g: Output results to console (GridView equivalent)."
    echo "  -m: Output results as a markdown file (duplicates.md)."
    exit 1
}

# Default values for flags
compare_contents=false
out_table=false
out_gridview=false
out_markdown=false
output_markdown_file="duplicates.md"

# Parse options
while getopts "ctgmh" opt; do
  case $opt in
    c) compare_contents=true ;;
    t) out_table=true ;;
    g) out_gridview=true ;;
    m) out_markdown=true ;;
    h) usage ;;
    *) usage ;;
  esac
done
shift $((OPTIND -1))

# --- Begin block ---
script_dir=$(dirname "$(realpath "$0")")
work_mgmt_dir=$(dirname "$script_dir") # Assumes script is in a 'scripts' subdirectory
backlog_dir="$work_mgmt_dir/backlog"
open_dir="$work_mgmt_dir/open"
closed_dir="$work_mgmt_dir/closed"

# Check for bc command
if ! command -v bc &> /dev/null; then
    echo "Error: 'bc' command is not installed. Please install it to continue." >&2
    exit 1
fi

# Source the Get-Similarity script
if [[ -f "$script_dir/get_similarity.sh" ]]; then
    # shellcheck source=./get_similarity.sh
    source "$script_dir/get_similarity.sh"
else
    echo "Error: get_similarity.sh not found in $script_dir" >&2
    exit 1
fi

if ! declare -F get_similarity > /dev/null; then
    echo "Error: get_similarity function not found. Please ensure it is defined in get_similarity.sh." >&2
    exit 1
fi

get_similarity() {
  local string1="$1"
  local string2="$2"
  local distinguishing_words_csv="${3-}" # Use default empty string if $3 is unset
  local distinguishing_words=()

  if [[ -n "$distinguishing_words_csv" ]]; then
    IFS=',' read -r -a distinguishing_words <<< "$distinguishing_words_csv"
  fi

  # Normalize strings
  string1=$(echo "$string1" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-zA-Z0-9]//g')
  string2=$(echo "$string2" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-zA-Z0-9]//g')

  # Remove distinguishing words
  for word in "${distinguishing_words[@]}"; do
    string1=${string1//$word/}
    string2=${string2//$word/}
  done

  # Calculate Levenshtein distance
  local len1=${#string1}
  local len2=${#string2}
  local dist_matrix=()

  for ((i = 0; i <= len1; i++)); do
    dist_matrix[i,0]=$i
  done

  for ((j = 0; j <= len2; j++)); do
    dist_matrix[0,j]=$j
  done

  for ((i = 1; i <= len1; i++)); do
    for ((j = 1; j <= len2; j++)); do
      local char1="${string1:$((i-1)):1}"
      local char2="${string2:$((j-1)):1}"
      local cost
      if [[ "$char1" == "$char2" ]]; then
        cost=0
      else
        cost=1
      fi
      dist_matrix[i,j]=$((dist_matrix[i-1,j] + 1))
      dist_matrix[i,j]=$((dist_matrix[i,j-1] + 1 < dist_matrix[i,j] ? dist_matrix[i,j-1] + 1 : dist_matrix[i,j]))
      dist_matrix[i,j]=$((dist_matrix[i-1,j-1] + cost < dist_matrix[i,j] ? dist_matrix[i-1,j-1] + cost : dist_matrix[i,j]))
    done
  done

  local distance=${dist_matrix[len1,len2]}
  local max_len=$((len1 > len2 ? len1 : len2))
  local similarity=$(echo "scale=2; (1 - $distance / $max_len) * 100" | bc)
  echo "$similarity"
}
# --- End Begin block ---

main() {
    echo "Checking for duplicate work items in the work backlog..."

    local files_to_process=()
    # Using find and process substitution to populate the array safely, ignoring errors from find
    while IFS= read -r -d $'\0' file; do
        files_to_process+=("$file")
    done < <(find "$backlog_dir" "$open_dir" "$closed_dir" -maxdepth 1 -name "*.md" -type f -print0 2>/dev/null)

    if [[ ${#files_to_process[@]} -eq 0 ]]; then
        echo "No markdown files found in backlog, open, or closed directories."
        exit 0
    fi

    local items_data=() # Array to store "ID|Name|FullFilePath"
    local total_files=${#files_to_process[@]}
    local current_file_num=0

    echo "Processing $total_files files..."
    for file_path in "${files_to_process[@]}"; do
        current_file_num=$((current_file_num + 1))
        # Simple progress, can be made more sophisticated if needed
        if (( current_file_num % 10 == 0 || current_file_num == total_files )); then
            echo -ne "Processing file $current_file_num/$total_files: $(basename "$file_path")\033[0K\r"
        fi

        local filename
        filename=$(basename "$file_path")
        # Regex to capture ID: (EP|FT|US|TSK)[0-9]{3}
        if [[ "$filename" =~ ^((EP|FT|US|TSK)[0-9]{3}).*\.md$ ]]; then
            local item_id="${BASH_REMATCH[1]}"
            items_data+=("$item_id|$filename|$file_path")
        fi
    done
    echo -e "\nFile processing complete."

    local duplicates_data=() # Array to store "Type|ID|Similarity|Dir1|Name1|File1|Dir2|Name2|File2"
    local total_items=${#items_data[@]}

    if [[ "$total_items" -lt 2 ]]; then
        echo "Not enough items to compare."
        exit 0
    fi

    echo "Comparing $total_items items..."
    for ((i = 0; i < total_items; i++)); do
        # Simple progress for comparison
        if (( i % 10 == 0 || i == total_items -1 )); then
             echo -ne "Comparing item $i/$total_items\033[0K\r"
        fi

        local item1_id item1_name item1_file item1_dir
        IFS='|' read -r item1_id item1_name item1_file <<< "${items_data[$i]}"
        item1_dir=$(basename "$(dirname "$item1_file")")

        for ((j = i + 1; j < total_items; j++)); do
            local item2_id item2_name item2_file item2_dir
            IFS='|' read -r item2_id item2_name item2_file <<< "${items_data[$j]}"
            item2_dir=$(basename "$(dirname "$item2_file")")

            if [[ "$item1_id" == "$item2_id" ]]; then
                duplicates_data+=("ID|$item1_id|100.00%|$item1_dir|$item1_name|$item1_file|$item2_dir|$item2_name|$item2_file")
            else
                local norm_name1 norm_name2 similarity_score
                norm_name1=$(echo "$item1_name" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-zA-Z0-9]//g')
                norm_name2=$(echo "$item2_name" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-zA-Z0-9]//g')

                similarity_score=$(get_similarity "$norm_name1" "$norm_name2")
                # Validate similarity_score (must be a number)
                if ! [[ "$similarity_score" =~ ^[0-9]+(\.[0-9]+)?$ ]]; then
                    echo "Warning: Invalid similarity score '$similarity_score' received for '$item1_name' and '$item2_name'. Skipping this pair." >&2
                else
                    # Define a similarity threshold (e.g., 75.00 for 75%)
                    # This could be made a script parameter or a variable at the top.
                    local name_similarity_threshold="75.00"
                    # Use bc for floating point comparison: bc returns 1 for true, 0 for false
                    if [[ $(echo "$similarity_score > $name_similarity_threshold" | bc -l) -eq 1 ]]; then
                        # Assuming get_similarity returns a number like "80.00", add % for display
                        local display_similarity="${similarity_score}%"
                        duplicates_data+=("Name|N/A|$display_similarity|$item1_dir|$item1_name|$item1_file|$item2_dir|$item2_name|$item2_file")
                    fi
                fi
            fi # Closes the 'else' from 'if [[ "$item1_id" == "$item2_id" ]]'
        done # Closes inner loop 'j'
    done # Closes outer loop 'i'
    echo -e "\nComparison complete."

    if [[ ${#duplicates_data[@]} -gt 0 ]]; then
        if [[ "$out_table" == true ]]; then
            echo "Type|ID|Similarity|Dir1|Name1|File1|Dir2|Name2|File2" # Header
            printf "%s\n" "${duplicates_data[@]}" | column -t -s '|'
        elif [[ "$out_markdown" == true ]]; then
            {
                echo "| Type | ID | Similarity | Dir1 | Name1 | File1 | Dir2 | Name2 | File2 |"
                echo "|------|----|------------|------|-------|-------|------|-------|-------|"
                for entry in "${duplicates_data[@]}"; do
                    echo "| $(echo "$entry" | sed 's/|/ | /g') |"
                done
            } > "$output_markdown_file"
            echo "Markdown report generated: $output_markdown_file"
        else # Default to simple list (covers -g or no specific format)
            echo "Found potential duplicates:"
            printf "%s\n" "${duplicates_data[@]}"
        fi
    else
        echo "No duplicates found based on current criteria."
    fi
}

# Script execution starts here
main "$@"
