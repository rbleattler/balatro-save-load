namespace BalatroSaveToolkit.Services.Interfaces
{
    /// <summary>
    /// Service responsible for consistent error handling across the application
    /// </summary>
    public interface IErrorHandlingService
    {
        /// <summary>
        /// Handle an exception with default severity (warning)
        /// </summary>
        /// <param name="ex">The exception to handle</param>
        /// <param name="source">The source of the exception (class name, method, etc.)</param>
        /// <param name="message">User-friendly message to display</param>
        /// <param name="shouldDisplayToUser">Whether to show a message to the user</param>
        void HandleException(Exception ex, string source, string message, bool shouldDisplayToUser = true);
        
        /// <summary>
        /// Handle an exception with specified severity
        /// </summary>
        /// <param name="ex">The exception to handle</param>
        /// <param name="source">The source of the exception (class name, method, etc.)</param>
        /// <param name="message">User-friendly message to display</param>
        /// <param name="severity">The severity level of the error</param>
        /// <param name="shouldDisplayToUser">Whether to show a message to the user</param>
        void HandleException(Exception ex, string source, string message, ErrorSeverity severity, bool shouldDisplayToUser = true);
        
        /// <summary>
        /// Log an error without an associated exception
        /// </summary>
        /// <param name="source">The source of the error (class name, method, etc.)</param>
        /// <param name="message">Message describing the error</param>
        /// <param name="severity">The severity level of the error</param>
        /// <param name="shouldDisplayToUser">Whether to show a message to the user</param>
        void LogError(string source, string message, ErrorSeverity severity = ErrorSeverity.Warning, bool shouldDisplayToUser = true);
        
        /// <summary>
        /// Display a notification to the user
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="severity">The severity level affecting how the notification appears</param>
        /// <returns>Task that completes when the notification is displayed</returns>
        Task ShowNotificationAsync(string message, ErrorSeverity severity = ErrorSeverity.Information);
        
        /// <summary>
        /// Get user-friendly error message for common exceptions
        /// </summary>
        /// <param name="ex">The exception to get a message for</param>
        /// <returns>User-friendly error message</returns>
        string GetUserFriendlyErrorMessage(Exception ex);
    }
    
    /// <summary>
    /// Severity levels for error handling
    /// </summary>
    public enum ErrorSeverity
    {
        /// <summary>Information that doesn't indicate an error</summary>
        Information,
        
        /// <summary>Warning that doesn't prevent the application from functioning</summary>
        Warning,
        
        /// <summary>Error that affects some functionality but doesn't crash the app</summary>
        Error,
        
        /// <summary>Critical error that prevents the application from functioning</summary>
        Critical
    }
}
