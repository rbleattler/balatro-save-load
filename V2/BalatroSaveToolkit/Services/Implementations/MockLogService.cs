using System.Collections.Concurrent;
using System.Diagnostics;
using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// A simple in-memory implementation of the logging service for development purposes.
    /// This will be replaced by a full logging solution in TSK008.
    /// </summary>
    public class MockLogService : ILogService
    {
        // In-memory log storage (limited to 1000 entries)
        private readonly ConcurrentQueue<ActivityLogItem> _logs = new();
        private const int MaxLogEntries = 1000;

        public MockLogService()
        {
            // Add some sample log entries for initial state
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddDays(-1), "INFO", "Application", "Application started"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddDays(-1).AddMinutes(5), "INFO", "GameProcessMonitor", "Game process detected"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddDays(-1).AddMinutes(10), "INFO", "BackupService", "Backup created"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddDays(-1).AddHours(2), "INFO", "GameProcessMonitor", "Game process closed"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddHours(-5), "INFO", "Application", "Application started"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddHours(-4), "WARN", "GameProcessMonitor", "Could not detect game process"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddHours(-2), "INFO", "SaveService", "Loaded save file"));
            _logs.Enqueue(new ActivityLogItem(DateTime.Now.AddHours(-1), "INFO", "SettingsService", "Settings updated"));
        }

        public void LogDebug(string source, string message, string? details = null)
        {
            AddLogEntry("DEBUG", source, message, details);
        }

        public void LogInfo(string source, string message, string? details = null)
        {
            AddLogEntry("INFO", source, message, details);
        }

        public void LogWarning(string source, string message, string? details = null)
        {
            AddLogEntry("WARN", source, message, details);
        }

        public void LogError(string source, string message, string? details = null, Exception? exception = null)
        {
            string fullDetails = details ?? string.Empty;
            if (exception != null)
            {
                fullDetails = string.IsNullOrEmpty(fullDetails)
                    ? $"Exception: {exception.GetType().Name}\nMessage: {exception.Message}\nStack: {exception.StackTrace}"
                    : $"{fullDetails}\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStack: {exception.StackTrace}";
            }
            AddLogEntry("CRITICAL", source, message, fullDetails);
        }

        public async Task<List<ActivityLogItem>> GetRecentActivitiesAsync(int count)
        {
            // Return the most recent log entries
            var recent = _logs.Reverse().Take(count).ToList();
            return recent;
        }

        public async Task<List<ActivityLogItem>> GetLogEntriesAsync(DateTime start, DateTime end)
        {
            // Filter log entries by date range
            var entries = _logs.Where(e => e.Timestamp >= start && e.Timestamp <= end).ToList();
            return entries;
        }

        public async Task<bool> ExportLogsAsync(string filePath)
        {
            // This is just a mock implementation - doesn't actually save to a file
            return true;
        }

        private void AddLogEntry(string level, string source, string message, string? details = null)
        {
            var logEntry = new ActivityLogItem(DateTime.Now, level, source, message, details);

            // Add to queue
            _logs.Enqueue(logEntry);

            // Trim if too many entries
            while (_logs.Count > MaxLogEntries && _logs.TryDequeue(out _)) { }

            // Also output to debug console for development
            Debug.WriteLine($"[{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{logEntry.Level}] [{logEntry.Source}] {logEntry.Message}");
            if (!string.IsNullOrEmpty(logEntry.Details))
            {
                Debug.WriteLine($"   Details: {logEntry.Details}");
            }
        }
    }
}
