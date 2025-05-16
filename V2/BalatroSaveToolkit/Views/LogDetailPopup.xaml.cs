using CommunityToolkit.Maui.Views;
using BalatroSaveToolkit.Models;

namespace BalatroSaveToolkit.Views
{
    public partial class LogDetailPopup : Popup
    {
        private ActivityLogItem _logItem;

        public LogDetailPopup(ActivityLogItem logItem)
        {
            InitializeComponent();
            _logItem = logItem;
            BindingContext = _logItem;

            // Add a property for binding visibility
            if (BindingContext is ActivityLogItem item)
            {
                item.GetType().GetProperty("HasDetails")?.SetValue(item, !string.IsNullOrEmpty(item.Details));
            }
        }

        private void OnDismissButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        private async void OnCopyButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Format the log entry nicely for the clipboard
                string formattedLog =
                    $"Timestamp: {_logItem.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\n" +
                    $"Level: {_logItem.Level}\n" +
                    $"Source: {_logItem.Source}\n" +
                    $"Message: {_logItem.Message}\n";

                if (!string.IsNullOrEmpty(_logItem.Details))
                {
                    formattedLog += $"Details: {_logItem.Details}\n";
                }

                // Copy to clipboard
                await Clipboard.Default.SetTextAsync(formattedLog);

                // Get error handling service and show notification
                var errorHandler = IPlatformApplication.Current?.Services?.GetService<Services.Interfaces.IErrorHandlingService>();
                if (errorHandler != null)
                {
                    await errorHandler.ShowNotificationAsync("Log entry copied to clipboard", Services.Interfaces.ErrorSeverity.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying to clipboard: {ex.Message}");
            }
        }
    }
}
