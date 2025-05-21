using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Parses Balatro save files to extract game statistics and structured data
    /// </summary>
    public class SaveFileParser
    {
        /// <summary>
        /// Game statistics extracted from a save file
        /// </summary>
        public class GameStatistics
        {
            /// <summary>
            /// Gets or sets the player's coin balance
            /// </summary>
            public int Coins { get; set; }

            /// <summary>
            /// Gets or sets the player's current run number
            /// </summary>
            public int Round { get; set; }

            /// <summary>
            /// Gets or sets the player's current hand (deck name)
            /// </summary>
            public string DeckName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the collection of jokers the player has
            /// </summary>
            public List<string> Jokers { get; set; } = new List<string>();

            /// <summary>
            /// Gets or sets the collection of cards in the player's hand
            /// </summary>
            public List<string> Cards { get; set; } = new List<string>();

            /// <summary>
            /// Gets or sets any special items or modifiers
            /// </summary>
            public List<string> SpecialItems { get; set; } = new List<string>();

            /// <summary>
            /// Gets or sets the save's timestamp
            /// </summary>
            public DateTime Timestamp { get; set; }

            /// <summary>
            /// Gets or sets the profile number
            /// </summary>
            public int ProfileNumber { get; set; }

            /// <summary>
            /// Gets or sets additional metadata about the save file
            /// </summary>
            public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        }

        /// <summary>
        /// Parses a save file content string and extracts game statistics
        /// </summary>
        /// <param name="saveContent">The raw save file content</param>
        /// <returns>A GameStatistics object containing the extracted data</returns>
        public GameStatistics Parse(string saveContent)
        {
            if (string.IsNullOrEmpty(saveContent))
            {
                throw new ArgumentNullException(nameof(saveContent));
            }

            var statistics = new GameStatistics
            {
                Timestamp = DateTime.Now // Default to current time if not found in file
            };

            try
            {
                // Extract deck name - common pattern in save files
                var deckNameMatch = Regex.Match(saveContent, @"\[""BACK""\]=\{\[""name""\]=""([^""]+)""");
                if (deckNameMatch.Success)
                {
                    statistics.DeckName = deckNameMatch.Groups[1].Value;
                }

                // Extract round number
                var roundMatch = Regex.Match(saveContent, @"\[""round""\]=(\d+)");
                if (roundMatch.Success)
                {
                    statistics.Round = int.Parse(roundMatch.Groups[1].Value);
                }

                // Extract coins
                var coinsMatch = Regex.Match(saveContent, @"\[""dollars""\]=(\d+)");
                if (coinsMatch.Success)
                {
                    statistics.Coins = int.Parse(coinsMatch.Groups[1].Value);
                }

                // Extract jokers (this is more complex and would need more detailed parsing)
                var jokersPattern = @"\[""jokers""\]=\{([^\}]+)\}";
                var jokersMatch = Regex.Match(saveContent, jokersPattern);
                if (jokersMatch.Success)
                {
                    string jokersSection = jokersMatch.Groups[1].Value;
                    var jokerNameMatches = Regex.Matches(jokersSection, @"\[""name""\]=""([^""]+)""");
                    foreach (Match match in jokerNameMatches)
                    {
                        statistics.Jokers.Add(match.Groups[1].Value);
                    }
                }

                // Extract cards (would need detailed parsing as well)
                var cardsPattern = @"\[""cards""\]=\{([^\}]+)\}";
                var cardsMatch = Regex.Match(saveContent, cardsPattern);
                if (cardsMatch.Success)
                {
                    string cardsSection = cardsMatch.Groups[1].Value;
                    var cardMatches = Regex.Matches(cardsSection, @"\[""id""\]=""([^""]+)""");
                    foreach (Match match in cardMatches)
                    {
                        statistics.Cards.Add(match.Groups[1].Value);
                    }
                }

                return statistics;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse save file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Asynchronously parses a save file content string
        /// </summary>
        /// <param name="saveContent">The raw save file content</param>
        /// <returns>A GameStatistics object containing the extracted data</returns>
        public async Task<GameStatistics> ParseAsync(string saveContent)
        {
            return await Task.Run(() => Parse(saveContent));
        }

        /// <summary>
        /// Detects the profile number from a save file path
        /// </summary>
        /// <param name="filePath">Path to the save file</param>
        /// <returns>The profile number (1-4) or 0 if can't be determined</returns>
        public static int DetermineProfileNumber(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return 0;
            }

            // Check for pattern like "P1 2025-05-21" in the filename
            var profileMatch = Regex.Match(filePath, @"P(\d)");
            if (profileMatch.Success && int.TryParse(profileMatch.Groups[1].Value, out int profileNumber))
            {
                return profileNumber;
            }

            // Check for folders like "/1/save.jkr" (default save location)
            var folderProfileMatch = Regex.Match(filePath, @"[\\/](\d)[\\/]save\.jkr$");
            if (folderProfileMatch.Success && int.TryParse(folderProfileMatch.Groups[1].Value, out profileNumber))
            {
                return profileNumber;
            }

            return 0;
        }
    }
}
