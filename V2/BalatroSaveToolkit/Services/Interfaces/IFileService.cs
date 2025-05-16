namespace BalatroSaveToolkit.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> ReadTextAsync(string filePath);
        Task WriteTextAsync(string filePath, string content);
        Task<bool> FileExistsAsync(string filePath);
        Task<string> GetApplicationDataDirectoryAsync();
        Task<string> PickFileAsync(string title, string filter);
        Task<string> PickFolderAsync(string title);
        Task<string> PickSaveFileAsync(string title, string suggestedName, string filter);
        Task CopyFileAsync(string sourcePath, string destinationPath);
    }
}
