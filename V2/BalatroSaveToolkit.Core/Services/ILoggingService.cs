using System;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Service interface for logging operations.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Logs a debug message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Debug(string message, Exception exception);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Info(string message);

        /// <summary>
        /// Logs an information message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Info(string message, Exception exception);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Warning(string message);

        /// <summary>
        /// Logs a warning message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Warning(string message, Exception exception);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message);

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void Error(string message, Exception exception);
    }
}
