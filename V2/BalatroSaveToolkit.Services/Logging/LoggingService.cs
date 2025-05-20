using System;
using System.Diagnostics;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Services.Logging
{
    /// <summary>
    /// Implementation of the logging service.
    /// </summary>
    public class LoggingService : ILoggingService
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] {message}");
        }

        /// <summary>
        /// Logs a debug message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Debug(string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] {message} - Exception: {exception}");
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] {message}");
        }

        /// <summary>
        /// Logs an information message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Info(string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] {message} - Exception: {exception}");
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] {message}");
        }

        /// <summary>
        /// Logs a warning message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Warning(string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] {message} - Exception: {exception}");
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}");
        }

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Error(string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message} - Exception: {exception}");
        }
    }
}
