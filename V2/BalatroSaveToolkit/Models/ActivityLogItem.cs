using BalatroSaveToolkit.Services.Interfaces;

namespace BalatroSaveToolkit.Models
{    /// <summary>
    /// Represents a log entry in the application
    /// </summary>
    public class ActivityLogItem
    {
        /// <summary>
        /// When the log was created
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Log level (Debug, Info, Warning, Error, Critical)
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Source of the log (class, method, component, etc.)
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Main log message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional details (stack trace, context, etc.)
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Whether the log entry has additional details
        /// </summary>
        public bool HasDetails => !string.IsNullOrEmpty(Details);

        /// <summary>
        /// Create a new activity log item
        /// </summary>
        public ActivityLogItem(DateTime timestamp, string level, string source, string message, string? details = null)
        {
            Timestamp = timestamp;
            Level = level;
            Source = source;
            Message = message;
            Details = details;
        }

        /// <summary>
        /// Maps ErrorSeverity to a log level string
        /// </summary>
        public static string GetLogLevelFromSeverity(ErrorSeverity severity)
        {
            return severity switch
            {
                ErrorSeverity.Information => "INFO",
                ErrorSeverity.Warning => "WARN",
                ErrorSeverity.Error => "ERROR",
                ErrorSeverity.Critical => "CRITICAL",
                _ => "INFO"
            };
        }
    }
}
