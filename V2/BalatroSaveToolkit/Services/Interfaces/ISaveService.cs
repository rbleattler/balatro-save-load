using BalatroSaveToolkit.Models;

namespace BalatroSaveToolkit.Services.Interfaces
{
    public interface ISaveService
    {
        Task<List<SaveFileInfo>> GetSaveFilesAsync();
        Task<List<SaveFileInfo>> SearchSavesAsync(string searchTerm);
        Task<SaveData> LoadSaveDataAsync(string filePath);
        Task BackupCurrentSaveAsync();
        Task LoadSaveAsync(string filePath);
        Task LoadCustomSaveAsync(string filePath);
        Task DeleteSaveAsync(string filePath);
        Task<string> GetCurrentSavePathAsync();
    }
}
