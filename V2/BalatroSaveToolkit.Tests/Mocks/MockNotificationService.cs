using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BalatroSaveToolkit.Core.Services;

namespace BalatroSaveToolkit.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of INotificationService for testing.
    /// </summary>
    public class MockNotificationService : INotificationService
    {
        /// <summary>
        /// Gets the list of shown notifications.
        /// </summary>
        public List<(string Title, string Message, string Type)> Notifications { get; } = new();

        /// <summary>
        /// Gets or sets whether confirmation dialogs should return true.
        /// </summary>
        public bool ConfirmationResult { get; set; } = true;

        /// <summary>
        /// Shows an informational notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowInformation(string title, string message, int durationMs = 3000)
        {
            Notifications.Add((title, message, "Information"));
        }

        /// <summary>
        /// Shows a success notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowSuccess(string title, string message, int durationMs = 3000)
        {
            Notifications.Add((title, message, "Success"));
        }

        /// <summary>
        /// Shows a warning notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowWarning(string title, string message, int durationMs = 3000)
        {
            Notifications.Add((title, message, "Warning"));
        }

        /// <summary>
        /// Shows an error notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowError(string title, string message, int durationMs = 3000)
        {
            Notifications.Add((title, message, "Error"));
        }

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <returns>True if confirmed, false otherwise.</returns>
        public Task<bool> ShowConfirmationAsync(string title, string message)
        {
            Notifications.Add((title, message, "Confirmation"));
            return Task.FromResult(ConfirmationResult);
        }
    }
}
