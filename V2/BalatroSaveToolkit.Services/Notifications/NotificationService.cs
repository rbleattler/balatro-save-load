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
        }        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <returns>True if confirmed, false otherwise.</returns>
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            _loggingService?.Info($"Confirmation dialog: {title} - {message}");            var window = Locator.Current.GetService<Window>();
            if (window == null)
            {
                return false;
            }

            // Create a simple confirmation dialog using Window API
            var dialog = new Window
            {
                Title = title,
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel()
            };

            var contentPanel = (StackPanel)dialog.Content!;
            contentPanel.Children.Add(new TextBlock { Text = message, Margin = new Avalonia.Thickness(10) });

            var buttonPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Avalonia.Thickness(10)
            };

            var yesButton = new Button { Content = "Yes", Margin = new Avalonia.Thickness(5, 0) };
            yesButton.Tag = true;

            var noButton = new Button { Content = "No", Margin = new Avalonia.Thickness(5, 0) };
            noButton.Tag = false;

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);
            contentPanel.Children.Add(buttonPanel);            // Set up button event handlers
            var dialogResult = new TaskCompletionSource<bool>();

            yesButton.Click += (s, e) =>
            {
                dialogResult.SetResult(true);
                dialog.Close();
            };

            noButton.Click += (s, e) =>
            {
                dialogResult.SetResult(false);
                dialog.Close();
            };

            // Show dialog
            await dialog.ShowDialog(window).ConfigureAwait(false);
            return await dialogResult.Task.ConfigureAwait(false);
        }
    }
}
