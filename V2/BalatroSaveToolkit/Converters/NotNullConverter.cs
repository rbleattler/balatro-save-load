using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BalatroSaveToolkit.Converters
{
    /// <summary>
    /// Converts a null/not-null value to a boolean.
    /// </summary>
    public class NotNullConverter : IValueConverter
    {
        /// <summary>
        /// Converts a null check to a boolean value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
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
