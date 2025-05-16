using BalatroSaveToolkit.Models;

namespace BalatroSaveToolkit.Services.Interfaces
{
    public interface ILogService
    {
        Task LogDebugAsync(string message);
        Task LogInfoAsync(string message);
        Task LogWarningAsync(string message);
        Task LogErrorAsync(string message, Exception exception = null);
        Task<List<ActivityLogItem>> GetRecentActivitiesAsync(int count);
        Task<List<ActivityLogItem>> GetLogEntriesAsync(DateTime start, DateTime end);
    }
}
