using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BalatroSaveToolkit.Converters {
    /// <summary>
    /// Converter that returns "Game Stats" when the boolean value is true,
    /// or "Raw Content" when the boolean value is false
    /// </summary>
    class ViewModeConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is bool isGameStats) { return isGameStats ? "Game Stats" : "Raw Content"; }

            return "Raw Content";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is string stringValue) {
                return stringValue.Equals("Game Stats", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}