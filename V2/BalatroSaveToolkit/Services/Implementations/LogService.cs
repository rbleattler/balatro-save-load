using System.Collections.Concurrent;
using System.Text;
using BalatroSaveToolkit.Models;
using BalatroSaveToolkit.Services.Interfaces;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Implementation of ILogService using Serilog for logging
    /// </summary>
    public class LogService : ILogService
    {
        private readonly IFileService _fileService;
        private readonly ConcurrentQueue<ActivityLogItem> _recentLogs = new(); // In-memory cache for recent logs
        private const int MaxCachedLogs = 500; // Maximum number of logs to keep in memory
        private const string LogFileName = "app_log.txt";
        private const string LogFolder = "Logs";
        private Logger? _logger;
        private string _logFilePath = string.Empty;

        /// <summary>
        /// Creates a new instance of LogService
        /// </summary>
        /// <param name="fileService">File service for file operations</param>
        public LogService(IFileService fileService)
        {
            _fileService = fileService;
            InitializeLogger();
        }

        /// <summary>
        /// Initialize the Serilog logger with configuration
        /// </summary>
        private async void InitializeLogger()
        {
            try
            {
                // Get the app data directory for the logs
                string appDataDirectory = await _fileService.GetApplicationDataDirectoryAsync();
                string logDirectory = Path.Combine(appDataDirectory, LogFolder);

                // Create logs directory if it doesn't exist
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Construct log file path
                string logFilePath = Path.Combine(logDirectory, LogFileName);                _logFilePath = logFilePath;
                // Configure Serilog
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.WithProperty("Application", "BalatroSaveToolkit")
                    .Enrich.WithProperty("Version", GetAppVersion())
                    .WriteTo.File(
                        logFilePath,
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 10 * 1024 * 1024, // 10 MB max file size
                        retainedFileCountLimit: 10, // Keep 10 most recent files
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(new JsonFormatter(),
                        Path.Combine(logDirectory, "app_log.json"),
                        rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                // Log initialization success
                LogInfo("LogService", "Logger initialized successfully", $"Log file: {logFilePath}");
            }
            catch (Exception ex)
            {
                // Fallback to debug output if logger initialization fails
                System.Diagnostics.Debug.WriteLine($"ERROR: Failed to initialize logger: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current application version
        /// </summary>
        private string GetAppVersion()
        {
            return "1.0.0"; // TODO: Get from assembly version
        }

        /// <inheritdoc />
        public void LogDebug(string source, string message, string? details = null)
        {
            AddLog(LogEventLevel.Debug, source, message, details);
        }

        /// <inheritdoc />
        public void LogInfo(string source, string message, string? details = null)
        {
            AddLog(LogEventLevel.Information, source, message, details);
        }

        /// <inheritdoc />
        public void LogWarning(string source, string message, string? details = null)
        {
            AddLog(LogEventLevel.Warning, source, message, details);
        }

        /// <inheritdoc />
        public void LogError(string source, string message, string? details = null, Exception? exception = null)
        {
            AddLog(LogEventLevel.Error, source, message, details, exception);
        }

        /// <inheritdoc />
        public void LogCritical(string source, string message, string? details = null, Exception? exception = null)
        {
            AddLog(LogEventLevel.Fatal, source, message, details, exception);
        }

        /// <inheritdoc />
        public async Task<List<ActivityLogItem>> GetRecentActivitiesAsync(int count)
        {
            return _recentLogs
                .OrderByDescending(log => log.Timestamp)
                .Take(count)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<List<ActivityLogItem>> GetLogEntriesAsync(DateTime start, DateTime end)
        {
            // First check in-memory cache
            var cachedEntries = _recentLogs
                .Where(log => log.Timestamp >= start && log.Timestamp <= end)
                .OrderByDescending(log => log.Timestamp)
                .ToList();

            // If we need to go beyond the cache, read from log file
            if (_recentLogs.Count > 0 && start < _recentLogs.Min(l => l.Timestamp))
            {
                // Read from file (this is a simplified approach - a real implementation
                // would parse the log file more efficiently)
                try
                {
                    // Attempt to read logs from file
                    if (File.Exists(_logFilePath))
                    {
                        var fileEntries = await ParseLogFileAsync(start, end);

                        // Merge file entries with cached entries, removing duplicates
                        var allEntries = new HashSet<ActivityLogItem>(fileEntries);
                        foreach (var entry in cachedEntries)
                        {
                            allEntries.Add(entry);
                        }

                        return allEntries
                            .OrderByDescending(log => log.Timestamp)
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but don't propagate it - just return whatever we have in memory
                    AddLog(LogEventLevel.Error, nameof(LogService),
                        "Failed to read logs from file", $"Error: {ex.Message}", ex);
                }
            }

            return cachedEntries;
        }

        /// <inheritdoc />
        public async Task<bool> ExportLogsAsync(string filePath)
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    // Copy the log file to the requested location
                    await _fileService.CopyFileAsync(_logFilePath, filePath);
                    return true;
                }

                // If no log file exists, export what we have in memory
                var recentLogs = await GetRecentActivitiesAsync(MaxCachedLogs);

                if (recentLogs.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var log in recentLogs.OrderBy(l => l.Timestamp))
                    {
                        sb.AppendLine($"{log.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{log.Level}] [{log.Source}] {log.Message}");
                        if (!string.IsNullOrEmpty(log.Details))
                        {
                            sb.AppendLine($"    Details: {log.Details}");
                        }
                    }

                    await _fileService.WriteTextAsync(filePath, sb.ToString());
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AddLog(LogEventLevel.Error, nameof(LogService),
                    "Failed to export logs", $"Target file: {filePath}, Error: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Parse the log file to extract entries within the given date range
        /// </summary>
        private async Task<List<ActivityLogItem>> ParseLogFileAsync(DateTime start, DateTime end)
        {
            var entries = new List<ActivityLogItem>();

            try
            {
                if (File.Exists(_logFilePath))
                {
                    // Read the file line by line
                    var lines = await File.ReadAllLinesAsync(_logFilePath);
                    ActivityLogItem? currentEntry = null;

                    foreach (var line in lines)
                    {
                        // Check if this line is a new log entry (starts with timestamp)
                        if (line.Length > 20 && char.IsDigit(line[0]) && line[4] == '-' && line[7] == '-')
                        {
                            // If we have a current entry, add it to the list
                            if (currentEntry != null)
                            {
                                entries.Add(currentEntry);
                                currentEntry = null;
                            }

                            // Parse the new entry
                            currentEntry = ParseLogLine(line);

                            // Skip if outside date range
                            if (currentEntry != null && (currentEntry.Timestamp < start || currentEntry.Timestamp > end))
                            {
                                currentEntry = null;
                            }
                        }
                        // Otherwise, it's a continuation of the current entry (e.g. details or stack trace)
                        else if (currentEntry != null)
                        {
                            // Append to details
                            currentEntry.Details = currentEntry.Details == null
                                ? line.Trim()
                                : $"{currentEntry.Details}{Environment.NewLine}{line.Trim()}";
                        }
                    }

                    // Add the last entry if it exists
                    if (currentEntry != null)
                    {
                        entries.Add(currentEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing log file: {ex.Message}");
            }

            return entries;
        }

        /// <summary>
        /// Parse a single log line into an ActivityLogItem
        /// </summary>
        private ActivityLogItem? ParseLogLine(string line)
        {
            try
            {
                // Expected format: 2025-05-16 12:34:56.789 [INFO] [Source] Message
                var timestampEnd = line.IndexOf(' ', 20); // Find end of timestamp
                if (timestampEnd < 0) return null;

                var timestamp = DateTime.Parse(line.Substring(0, timestampEnd));

                // Extract level between first [] brackets
                var levelStart = line.IndexOf('[', timestampEnd) + 1;
                var levelEnd = line.IndexOf(']', levelStart);
                if (levelStart < 0 || levelEnd < 0) return null;

                var level = line.Substring(levelStart, levelEnd - levelStart).Trim();

                // Extract source between second [] brackets
                var sourceStart = line.IndexOf('[', levelEnd) + 1;
                var sourceEnd = line.IndexOf(']', sourceStart);
                if (sourceStart < 0 || sourceEnd < 0) return null;

                var source = line.Substring(sourceStart, sourceEnd - sourceStart).Trim();

                // Everything after the second ] is the message
                var messageStart = line.IndexOf(' ', sourceEnd);
                if (messageStart < 0) return null;

                var message = line.Substring(messageStart).Trim();

                return new ActivityLogItem(timestamp, level, source, message);
            }
            catch
            {
                return null; // Skip malformed lines
            }
        }

        /// <summary>
        /// Add a log entry with the specified level
        /// </summary>
        private void AddLog(LogEventLevel level, string source, string message, string? details = null, Exception? exception = null)
        {
            // Convert Serilog level to our log level
            string logLevel = level switch
            {
                LogEventLevel.Verbose => "TRACE",
                LogEventLevel.Debug => "DEBUG",
                LogEventLevel.Information => "INFO",
                LogEventLevel.Warning => "WARN",
                LogEventLevel.Error => "ERROR",
                LogEventLevel.Fatal => "CRITICAL",
                _ => "INFO"
            };

            // Create activity log item for our cache
            var logEntry = new ActivityLogItem(
                DateTime.Now,
                logLevel,
                source,
                message,
                details
            );

            // Add to in-memory queue
            _recentLogs.Enqueue(logEntry);

            // Trim the queue if it exceeds the limit
            while (_recentLogs.Count > MaxCachedLogs && _recentLogs.TryDequeue(out _)) { }

            // Log to Serilog
            if (_logger != null)
            {
                // Make full details message if both details and exception exist
                string? fullDetails = details;
                if (exception != null)
                {
                    fullDetails = string.IsNullOrEmpty(fullDetails)
                        ? exception.ToString()
                        : $"{fullDetails}{Environment.NewLine}{exception}";
                }

                // Log to file with source context for better filtering
                var contextLogger = _logger.ForContext("SourceContext", source);

                switch (level)
                {
                    case LogEventLevel.Verbose:
                        contextLogger.Verbose(exception, "{Message}{NewLine}{Details}", message, fullDetails ?? "");
                        break;
                    case LogEventLevel.Debug:
                        contextLogger.Debug(exception, "{Message}{NewLine}{Details}", message, fullDetails ?? "");
                        break;
                    case LogEventLevel.Information:
                        contextLogger.Information(exception, "{Message}{NewLine}{Details}", message, fullDetails ?? "");
                        break;
                    case LogEventLevel.Warning:
                        contextLogger.Warning(exception, "{Message}{NewLine}{Details}", message, fullDetails ?? "");
                        break;
                    case LogEventLevel.Error:
                        contextLogger.Error(exception, "{Message}{NewLine}{Details}", message, fullDetails ?? "");
                        break;
                    case LogEventLevel.Fatal:
                        contextLogger.Fatal(exception, "{Message}{NewLine}{Details}", message, fullDetails ?? "");
                        break;
                }
            }
            else
            {
                // Fallback to debug output if logger is not initialized
                var logMessage = $"[{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{logLevel}] [{source}] {message}";
                System.Diagnostics.Debug.WriteLine(logMessage);

                if (!string.IsNullOrEmpty(details))
                {
                    System.Diagnostics.Debug.WriteLine($"   Details: {details}");
                }

                if (exception != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Exception: {exception}");                }
            }
        }
    }
}

// Extension methods to maintain backward compatibility with viewmodel calls
namespace BalatroSaveToolkit.Extensions
{
    public static class LogServiceExtensions
    {
        public static Task LogInfoAsync(this ILogService logService, string message)
        {
            logService.LogInfo("Application", message);
            return Task.CompletedTask;
        }

        public static Task LogErrorAsync(this ILogService logService, string message, Exception? exception = null)
        {
            logService.LogError("Application", message, null, exception);
            return Task.CompletedTask;
        }

        public static Task LogWarningAsync(this ILogService logService, string message)
        {
            logService.LogWarning("Application", message);
            return Task.CompletedTask;
        }

        public static Task LogDebugAsync(this ILogService logService, string message)
        {
            logService.LogDebug("Application", message);
            return Task.CompletedTask;
        }
    }
}
