using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using BalatroSaveToolkit.Core.Services;
using Splat;

namespace BalatroSaveToolkit.Services.Notifications
{
    /// <summary>
    /// Implementation of the notification service.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly WindowNotificationManager? _notificationManager;
        private readonly ILoggingService? _loggingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="hostWindow">The host window.</param>
        public NotificationService(Window? hostWindow = null)
        {
            _loggingService = Locator.Current.GetService<ILoggingService>();

            if (hostWindow != null)
            {
                _notificationManager = new WindowNotificationManager(hostWindow)
                {
                    Position = NotificationPosition.BottomRight,
                    MaxItems = 3,
                };
            }
        }

        /// <summary>
        /// Sets the host window for notifications.
        /// </summary>
        /// <param name="hostWindow">The host window.</param>
        public void SetHostWindow(Window hostWindow)
        {
            if (_notificationManager == null && hostWindow != null)
            {
                var notificationManager = new WindowNotificationManager(hostWindow)
                {
                    Position = NotificationPosition.BottomRight,
                    MaxItems = 3,
                };

                // We need this hack to set the readonly field
                typeof(NotificationService)
                    .GetField("_notificationManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(this, notificationManager);
            }
        }

        /// <summary>
        /// Shows an informational notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowInformation(string title, string message, int durationMs = 3000)
        {
            _loggingService?.Info($"Notification: {title} - {message}");
            _notificationManager?.Show(new Notification(title, message, NotificationType.Information, TimeSpan.FromMilliseconds(durationMs)));
        }

        /// <summary>
        /// Shows a success notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowSuccess(string title, string message, int durationMs = 3000)
        {
            _loggingService?.Info($"Success: {title} - {message}");
            _notificationManager?.Show(new Notification(title, message, NotificationType.Success, TimeSpan.FromMilliseconds(durationMs)));
        }

        /// <summary>
        /// Shows a warning notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowWarning(string title, string message, int durationMs = 3000)
        {
            _loggingService?.Warning($"Warning: {title} - {message}");
            _notificationManager?.Show(new Notification(title, message, NotificationType.Warning, TimeSpan.FromMilliseconds(durationMs)));
        }

        /// <summary>
        /// Shows an error notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="durationMs">The duration in milliseconds to show the notification for.</param>
        public void ShowError(string title, string message, int durationMs = 3000)
        {
            _loggingService?.Error($"Error: {title} - {message}");
            _notificationManager?.Show(new Notification(title, message, NotificationType.Error, TimeSpan.FromMilliseconds(durationMs)));
        }

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <returns>True if confirmed, false otherwise.</returns>
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            _loggingService?.Info($"Confirmation dialog: {title} - {message}");

            var window = Locator.Current.GetService<Window>();
            if (window == null)
            {
                return false;
            }

            var dialog = new TaskDialog
            {
                Title = title,
                Content = new TextBlock { Text = message },
                Buttons = {
                    new TaskDialogButton { Text = "Yes", IsDefault = true },
                    new TaskDialogButton { Text = "No", IsCancel = true }
                }
            };

            var result = await dialog.ShowAsync();
            return result?.Text == "Yes";
        }
    }
}
