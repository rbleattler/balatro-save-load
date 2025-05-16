using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    public class MockLogService : ILogService
    {
        private readonly List<ActivityLogItem> _logs = new();

        public MockLogService()
        {
            // Add some sample log entries
            _logs.Add(new ActivityLogItem(DateTime.Now.AddDays(-1), "Info", "Application started"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddDays(-1).AddMinutes(5), "Info", "Game process detected"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddDays(-1).AddMinutes(10), "Info", "Backup created"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddDays(-1).AddHours(2), "Info", "Game process closed"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddHours(-5), "Info", "Application started"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddHours(-4), "Warning", "Could not detect game process"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddHours(-2), "Info", "Loaded save file"));
            _logs.Add(new ActivityLogItem(DateTime.Now.AddHours(-1), "Info", "Settings updated"));
        }

        public async Task<List<ActivityLogItem>> GetLogEntriesAsync(DateTime start, DateTime end)
        {
            return _logs
                .Where(log => log.Timestamp >= start && log.Timestamp <= end)
                .OrderByDescending(log => log.Timestamp)
                .ToList();
        }

        public async Task<List<ActivityLogItem>> GetRecentActivitiesAsync(int count)
        {
            return _logs
                .OrderByDescending(log => log.Timestamp)
                .Take(count)
                .ToList();
        }

        public Task LogDebugAsync(string message)
        {
            _logs.Add(new ActivityLogItem(DateTime.Now, "Debug", message));
            return Task.CompletedTask;
        }

        public Task LogErrorAsync(string message, Exception exception = null)
        {
            _logs.Add(new ActivityLogItem(
                DateTime.Now,
                "Error",
                message,
                exception?.ToString() ?? string.Empty));
            return Task.CompletedTask;
        }

        public Task LogInfoAsync(string message)
        {
            _logs.Add(new ActivityLogItem(DateTime.Now, "Info", message));
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string message)
        {
            _logs.Add(new ActivityLogItem(DateTime.Now, "Warning", message));
            return Task.CompletedTask;
        }
    }
}
