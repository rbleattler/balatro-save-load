using System.Collections.ObjectModel;

namespace BalatroSaveToolkit.Models
{    public class SaveData
    {
        // Basic Game Stats
        public int RunNumber { get; set; }
        public int Chips { get; set; }
        public string Level { get; set; } = string.Empty;
        public int Ante { get; set; }
        public int ArcanaUnlocked { get; set; }
        public bool IsCompleted { get; set; }

        // Deck Information
        public string DeckType { get; set; } = string.Empty;
        public string DeckDescription { get; set; } = string.Empty;

        // Cards
        public ObservableCollection<CardInfo> Cards { get; set; } = new();
    }
      public class CardInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string Suit { get; set; } = string.Empty;
        public bool IsJoker { get; set; }
        public bool IsTarot { get; set; }
        public bool IsEnhanced { get; set; }
    }
}
