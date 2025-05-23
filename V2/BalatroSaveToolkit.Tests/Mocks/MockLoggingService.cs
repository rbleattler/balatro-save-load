using System;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of ILoggingService for testing.
    /// </summary>
    public class MockLoggingService : ILoggingService
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Debug(string message)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs a debug message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Debug(string message, Exception exception)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(string message)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs an information message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Info(string message, Exception exception)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warning(string message)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs a warning message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Warning(string message, Exception exception)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message)
        {
            // Mock implementation - no actual logging
        }

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        public void Error(string message, Exception exception)
        {
            // Mock implementation - no actual logging
        }
    }
}
