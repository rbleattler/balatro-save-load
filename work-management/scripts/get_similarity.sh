#!/usr/bin/env bash

# SYNOPSIS
#   Calculates the similarity between two strings using the Levenshtein distance
#   algorithm, with support for penalizing specific distinguishing words.
#
# DESCRIPTION
#   This function computes the similarity score between two input strings based
#   on the Levenshtein distance. The Levenshtein distance represents the minimum
#   number of single-character edits required to change one string into the other.
#   The similarity score is calculated as 1 - (Levenshtein Distance / Length of the longer string).
#   If the only difference between the strings (after filtering certain patterns)
#   is a word from the distinguishing_words_csv list, the similarity is set to 0.
#
# PARAMETERS
#   string1: The first string to compare.
#   string2: The second string to compare.
#   distinguishing_words_csv: Optional. A comma-separated string of words that,
#                             if they are the only difference between the two strings
#                             (after specific filtering), will cause the similarity to be set to 0.
#
# EXAMPLE
#   ./get_similarity.sh "save-linux" "save-windows" "linux,windows"
#   # Output: 0
#
#   ./get_similarity.sh "apple" "apply"
#   # Output: 0.8000
#
# NOTES
#   The function uses dynamic programming to efficiently calculate the Levenshtein distance.
#   Requires Bash 4+ for associative arrays and mapfile.
#   Uses 'bc' for floating-point arithmetic.

# Helper function for min of two integers
min() {
  if (($1 < $2)); then echo "$1"; else echo "$2"; fi
}

get_similarity() {
  local string1="$1"
  local string2="$2"
  local distinguishing_words_csv="$3"
  local distinguishing_words=()

  if [[ -n "$distinguishing_words_csv" ]]; then
    IFS=',' read -r -a distinguishing_words <<< "$distinguishing_words_csv"
  fi

  # Normalize input strings for Levenshtein distance: lowercase, remove all non-alphanum.
  local norm1
  norm1=$(echo "$string1" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]//g')
  local norm2
  norm2=$(echo "$string2" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]//g')

  # Distinguishing Words Logic
  if [[ ${#distinguishing_words[@]} -gt 0 ]]; then
    # Get words from original strings: lowercase, replace non-alphanum with space, split.
    local words1_str_temp
    words1_str_temp=$(echo "$string1" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-zA-Z0-9]/ /g' | tr -s ' ')
    local words2_str_temp
    words2_str_temp=$(echo "$string2" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-zA-Z0-9]/ /g' | tr -s ' ')

    local temp_arr1=()
    read -r -a temp_arr1 <<< "$words1_str_temp"
    local temp_arr2=()
    read -r -a temp_arr2 <<< "$words2_str_temp"

    local words1_arr=()
    for w in "${temp_arr1[@]}"; do [[ -n "$w" ]] && words1_arr+=("$w"); done
    local words2_arr=()
    for w in "${temp_arr2[@]}"; do [[ -n "$w" ]] && words2_arr+=("$w"); done

    mapfile -t s_uniq_words1 < <(printf "%s\n" "${words1_arr[@]}" | sort -u)
    mapfile -t s_uniq_words2 < <(printf "%s\n" "${words2_arr[@]}" | sort -u)

    local diff1_words=()
    mapfile -t diff1_words < <(comm -23 <(printf "%s\n" "${s_uniq_words1[@]}") <(printf "%s\n" "${s_uniq_words2[@]}"))

    local diff2_words=()
    mapfile -t diff2_words < <(comm -13 <(printf "%s\n" "${s_uniq_words1[@]}") <(printf "%s\n" "${s_uniq_words2[@]}"))

    local all_diffs_raw=("${diff1_words[@]}" "${diff2_words[@]}")
    local all_diffs_filtered=()

    for word_to_check in "${all_diffs_raw[@]}"; do
      if [[ -n "$word_to_check" ]] && ! echo "$word_to_check" | grep -qEi '(ep|ft|us|tsk)[0-9]{3}'; then
        all_diffs_filtered+=("$word_to_check")
      fi
    done

    if [[ ${#all_diffs_filtered[@]} -gt 0 ]]; then
      local all_are_distinguishing=true
      for word_in_diff in "${all_diffs_filtered[@]}"; do
        local is_dist_flag=false
        for dist_w in "${distinguishing_words[@]}"; do
          if [[ "$word_in_diff" == "$dist_w" ]]; then
            is_dist_flag=true
            break
          fi
        done
        if ! $is_dist_flag; then
          all_are_distinguishing=false
          break
        fi
      done

      if $all_are_distinguishing; then
        echo "0"
        return
      fi
    fi
  fi

  # Levenshtein Distance Calculation
  local len1=${#norm1}
  local len2=${#norm2}
  declare -A matrix # Associative array for the matrix (Bash 4+)

  for ((i = 0; i <= len1; i++)); do
    matrix["$i,0"]=$i
  done

  for ((j = 0; j <= len2; j++)); do
    matrix["0,$j"]=$j
  done

  for ((i = 1; i <= len1; i++)); do
    for ((j = 1; j <= len2; j++)); do
      local cost=1
      if [[ "${norm1:i-1:1}" == "${norm2:j-1:1}" ]]; then
        cost=0
      fi

      local val_del=$(( ${matrix["$((i-1)),$j"]} + 1 ))
      local val_ins=$(( ${matrix["$i,$((j-1))"]} + 1 ))
      local val_sub=$(( ${matrix["$((i-1)),$((j-1))"]} + cost ))

      matrix["$i,$j"]=$(min "$val_del" "$(min "$val_ins" "$val_sub")")
    done
  done

  local distance=${matrix["$len1,$len2"]}

  # Similarity Score Calculation
  local max_length=$len1
  if (( len2 > len1 )); then
    max_length=$len2
  fi

  if (( max_length == 0 )); then # Both strings are empty after normalization
    echo "1"
    return
  fi

  local similarity
  similarity=$(echo "scale=10; 1 - ($distance / $max_length)" | bc -l)

  printf "%.4f\n" "$similarity"
}

# Main execution block (if script is run directly)
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
  if [[ $# -lt 2 ]] || [[ $# -gt 3 ]]; then
    echo "Usage: $0 <string1> <string2> [distinguishing_words_csv]"
    echo "Example: $0 \"test string 1\" \"test string 2\" \"word1,word2\""
    exit 1
  fi
  get_similarity "$1" "$2" "$3"
fi