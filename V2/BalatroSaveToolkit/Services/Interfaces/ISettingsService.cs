namespace BalatroSaveToolkit.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<Models.AppSettings> GetSettingsAsync();
        Task SaveSettingsAsync(Models.AppSettings settings);
        Task ResetToDefaultsAsync();
        Task<string> GetSettingAsync(string key, string defaultValue = "");
        Task SetSettingAsync(string key, string value);
    }
}
