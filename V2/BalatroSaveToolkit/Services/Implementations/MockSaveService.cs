using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;
using System.Collections.ObjectModel;

namespace BalatroSaveToolkit.Services.Implementations
{
    public class MockSaveService : ISaveService
    {
        private readonly IFileService _fileService;
        private readonly List<SaveFileInfo> _mockSaves;

        public MockSaveService(IFileService fileService)
        {
            _fileService = fileService;

            // Create mock save data
            _mockSaves = new List<SaveFileInfo>
            {
                new SaveFileInfo("save_001.txt", "C:\\Users\\User\\AppData\\Roaming\\Balatro\\SaveData\\save_001.txt", DateTime.Now.AddDays(-5), 10240),
                new SaveFileInfo("save_002.txt", "C:\\Users\\User\\AppData\\Roaming\\Balatro\\SaveData\\save_002.txt", DateTime.Now.AddDays(-3), 12288),
                new SaveFileInfo("save_003.txt", "C:\\Users\\User\\AppData\\Roaming\\Balatro\\SaveData\\save_003.txt", DateTime.Now.AddDays(-1), 15360),
                new SaveFileInfo("backup_20250514_123045.txt", "C:\\Users\\User\\AppData\\Roaming\\BalatroSaveToolkit\\Backups\\backup_20250514_123045.txt", DateTime.Now.AddDays(-2), 10240),
                new SaveFileInfo("backup_20250515_183022.txt", "C:\\Users\\User\\AppData\\Roaming\\BalatroSaveToolkit\\Backups\\backup_20250515_183022.txt", DateTime.Now.AddHours(-10), 12288),
            };
        }

        public Task BackupCurrentSaveAsync()
        {
            // Mock backup operation
            return Task.CompletedTask;
        }

        public Task DeleteSaveAsync(string filePath)
        {
            _mockSaves.RemoveAll(s => s.FilePath == filePath);
            return Task.CompletedTask;
        }

        public async Task<string> GetCurrentSavePathAsync()
        {
            return "C:\\Users\\User\\AppData\\Roaming\\Balatro\\SaveData\\save_001.txt";
        }

        public async Task<List<SaveFileInfo>> GetSaveFilesAsync()
        {
            return _mockSaves;
        }

        public async Task LoadCustomSaveAsync(string filePath)
        {
            // Mock loading a custom save
            await Task.Delay(500); // Simulate operation
        }

        public async Task LoadSaveAsync(string filePath)
        {
            // Mock loading a save
            await Task.Delay(500); // Simulate operation
        }

        public async Task<SaveData> LoadSaveDataAsync(string filePath)
        {
            // Return mock save data
            await Task.Delay(200); // Simulate loading time

            return new SaveData
            {
                RunNumber = 42,
                Chips = 3500,
                Level = "Blind 6",
                Ante = 20,
                ArcanaUnlocked = 12,
                IsCompleted = false,
                DeckType = "Standard Deck",
                DeckDescription = "A standard playing card deck with some enhancements.",
                Cards = new ObservableCollection<CardInfo>
                {
                    new CardInfo { Name = "Ace", Rank = "A", Suit = "Spades", IsEnhanced = true },
                    new CardInfo { Name = "King", Rank = "K", Suit = "Hearts", IsEnhanced = false },
                    new CardInfo { Name = "Queen", Rank = "Q", Suit = "Clubs", IsEnhanced = false },
                    new CardInfo { Name = "Jack", Rank = "J", Suit = "Diamonds", IsEnhanced = false },
                    new CardInfo { Name = "Joker", Rank = "Joker", Suit = "None", IsJoker = true },
                }
            };
        }

        public async Task<List<SaveFileInfo>> SearchSavesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _mockSaves;

            searchTerm = searchTerm.ToLower();
            return _mockSaves
                .Where(s => s.Name.ToLower().Contains(searchTerm) ||
                            s.FilePath.ToLower().Contains(searchTerm))
                .ToList();
        }
    }
}
