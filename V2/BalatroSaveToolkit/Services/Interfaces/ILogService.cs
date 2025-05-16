using BalatroSaveToolkit.Models;

namespace BalatroSaveToolkit.Services.Interfaces
{
    /// <summary>
    /// Service for logging application events and errors
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Log detailed debug information
        /// </summary>
        /// <param name="source">The source of the log (class name, method, etc.)</param>
        /// <param name="message">The message to log</param>
        /// <param name="details">Optional additional details</param>
        void LogDebug(string source, string message, string? details = null);

        /// <summary>
        /// Log informational message
        /// </summary>
        /// <param name="source">The source of the log (class name, method, etc.)</param>
        /// <param name="message">The message to log</param>
        /// <param name="details">Optional additional details</param>
        void LogInfo(string source, string message, string? details = null);

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="source">The source of the log (class name, method, etc.)</param>
        /// <param name="message">The message to log</param>
        /// <param name="details">Optional additional details</param>
        void LogWarning(string source, string message, string? details = null);

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="source">The source of the log (class name, method, etc.)</param>
        /// <param name="message">The message to log</param>
        /// <param name="details">Optional additional details</param>
        /// <param name="exception">Optional exception that caused the error</param>
        void LogError(string source, string message, string? details = null, Exception? exception = null);

        /// <summary>
        /// Log critical error message
        /// </summary>
        /// <param name="source">The source of the log (class name, method, etc.)</param>
        /// <param name="message">The message to log</param>
        /// <param name="details">Optional additional details</param>
        /// <param name="exception">Optional exception that caused the error</param>
        void LogCritical(string source, string message, string? details = null, Exception? exception = null);

        /// <summary>
        /// Get recent log entries
        /// </summary>
        /// <param name="count">Number of entries to retrieve</param>
        /// <returns>List of activity log items</returns>
        Task<List<ActivityLogItem>> GetRecentActivitiesAsync(int count);

        /// <summary>
        /// Get log entries within a date range
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>List of activity log items</returns>
        Task<List<ActivityLogItem>> GetLogEntriesAsync(DateTime start, DateTime end);

        /// <summary>
        /// Export logs to a file
        /// </summary>
        /// <param name="filePath">Path to save the log file</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> ExportLogsAsync(string filePath);
    }
}
