using System.Diagnostics;
using BalatroSaveToolkit.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace BalatroSaveToolkit.Services.Implementations
{
    /// <summary>
    /// Implementation of IErrorHandlingService for consistent error handling
    /// </summary>
    public class ErrorHandlingService : IErrorHandlingService
    {
        private readonly ILogService _logService;

        public ErrorHandlingService(ILogService logService)
        {
            _logService = logService;
        }

        /// <inheritdoc />
        public void HandleException(Exception ex, string source, string message, bool shouldDisplayToUser = true)
        {
            HandleException(ex, source, message, ErrorSeverity.Warning, shouldDisplayToUser);
        }

        /// <inheritdoc />
        public void HandleException(Exception ex, string source, string message, ErrorSeverity severity, bool shouldDisplayToUser = true)
        {
            // Log the exception with details
            string fullMessage = $"{message} - {ex.Message}";
            string details = $"Exception: {ex.GetType().Name} | Stack Trace: {ex.StackTrace}";

            // Log based on severity
            switch (severity)
            {
                case ErrorSeverity.Information:
                    _logService.LogInfo(source, fullMessage, details);
                    break;
                case ErrorSeverity.Warning:
                    _logService.LogWarning(source, fullMessage, details);
                    break;
                case ErrorSeverity.Error:
                    _logService.LogError(source, fullMessage, details);
                    break;
                case ErrorSeverity.Critical:
                    _logService.LogCritical(source, fullMessage, details);
                    break;
            }

            // Always output to debug console for development purposes
            Debug.WriteLine($"[{severity}] {source}: {fullMessage}");

            // Show notification to user if required
            if (shouldDisplayToUser)
            {
                // Get user-friendly message
                string userMessage = GetUserFriendlyErrorMessage(ex);
                if (string.IsNullOrEmpty(userMessage))
                {
                    userMessage = message;
                }

                // Show toast asynchronously (fire and forget)
                _ = ShowNotificationAsync(userMessage, severity);
            }
        }

        /// <inheritdoc />
        public void LogError(string source, string message, ErrorSeverity severity = ErrorSeverity.Warning, bool shouldDisplayToUser = true)
        {
            // Log based on severity
            switch (severity)
            {
                case ErrorSeverity.Information:
                    _logService.LogInfo(source, message);
                    break;
                case ErrorSeverity.Warning:
                    _logService.LogWarning(source, message);
                    break;
                case ErrorSeverity.Error:
                    _logService.LogError(source, message);
                    break;
                case ErrorSeverity.Critical:
                    _logService.LogCritical(source, message);
                    break;
            }

            // Always output to debug console for development purposes
            Debug.WriteLine($"[{severity}] {source}: {message}");

            // Show notification to user if required
            if (shouldDisplayToUser)
            {
                // Show toast asynchronously (fire and forget)
                _ = ShowNotificationAsync(message, severity);
            }
        }

        /// <inheritdoc />
        public async Task ShowNotificationAsync(string message, ErrorSeverity severity = ErrorSeverity.Information)
        {
            try
            {
                // Configure toast based on severity
                ToastDuration duration = severity >= ErrorSeverity.Error ? ToastDuration.Long : ToastDuration.Short;
                double fontSize = 14;
                  // Map severity to an appropriate color
                Microsoft.Maui.Graphics.Color backgroundColor = severity switch
                {
                    ErrorSeverity.Information => Microsoft.Maui.Graphics.Colors.DarkBlue,
                    ErrorSeverity.Warning => Microsoft.Maui.Graphics.Colors.DarkOrange,
                    ErrorSeverity.Error => Microsoft.Maui.Graphics.Colors.DarkRed,
                    ErrorSeverity.Critical => Microsoft.Maui.Graphics.Colors.Red,
                    _ => Microsoft.Maui.Graphics.Colors.Gray
                };

                // Create and show the toast
                var toast = Toast.Make(message, duration, fontSize);
                await toast.Show();
            }
            catch (Exception ex)
            {
                // If toast fails, we don't want to recursively show more toasts
                Debug.WriteLine($"Error showing notification: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public string GetUserFriendlyErrorMessage(Exception ex)
        {
            // Map common exceptions to user-friendly messages
            return ex switch
            {
                FileNotFoundException => "The requested file was not found. It may have been moved or deleted.",
                UnauthorizedAccessException => "You don't have permission to access this file or directory.",
                IOException => "There was an issue accessing the file. It may be in use by another program.",
                OutOfMemoryException => "The application ran out of memory. Try closing other applications.",
                ArgumentException => "Invalid input was provided to the operation.",
                NullReferenceException => "A required value was missing.",
                FormatException => "The data format is incorrect.",
                TimeoutException => "The operation timed out. Please try again.",
                // Add more mappings as needed

                // Default case
                _ => string.Empty // Return empty to use the caller-provided message
            };
        }
    }
}
