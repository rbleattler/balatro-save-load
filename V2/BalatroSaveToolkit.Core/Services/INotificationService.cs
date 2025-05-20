using System;
using System.Threading.Tasks;

namespace BalatroSaveToolkit.Core.Services
{
    /// <summary>
    /// Service interface for displaying notifications to the user.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Shows an informational notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        void ShowInformation(string title, string message, int durationMs = 3000);

        /// <summary>
        /// Shows a success notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        void ShowSuccess(string title, string message, int durationMs = 3000);

        /// <summary>
        /// Shows a warning notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        void ShowWarning(string title, string message, int durationMs = 3000);

        /// <summary>
        /// Shows an error notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        void ShowError(string title, string message, int durationMs = 3000);

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <returns>True if confirmed, false otherwise.</returns>
        Task<bool> ShowConfirmationAsync(string title, string message);
    }
}
