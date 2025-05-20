using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BalatroSaveToolkit.Converters
{
    /// <summary>
    /// Converts a boolean game running status to a display string.
    /// </summary>
    public class GameStatusConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a status string.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRunning)
            {
                return isRunning ? "Running" : "Not Running";
            }

            return "Unknown";
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
