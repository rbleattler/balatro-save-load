using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace BalatroSaveToolkit.Converters
{
    /// <summary>
    /// Converts a boolean value to a brush color.
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a brush color.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRunning)
            {
                return isRunning ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Gray);
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
