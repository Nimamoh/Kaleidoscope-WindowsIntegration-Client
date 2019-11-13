using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace kio_windows_integration.ValueConverters
{
    /// <summary>
    /// null => <see cref="Visibility.Collapsed"/>
    /// "" => <see cref="Visibility.Collapsed"/>
    /// <see cref="bool.FalseString"/> => <see cref="Visibility.Collapsed"/>
    /// "any string" => <see cref="Visibility.Visible"/>
    ///
    /// <see cref="Visibility.Visible"/> => <see cref="bool.TrueString"/>
    /// <see cref="Visibility.Collapsed"/> => <see cref="bool.FalseString"/>
    /// <see cref="Visibility.Hidden"/> => <see cref="bool.FalseString"/>
    /// </summary>
    public class StringToVisibleConverter
        : BaseValueConverter<string, Visibility>
    {
        private static bool StringToBool(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            bool result; if (bool.TryParse(input, out result))
                return result;
            return true;
        }

        public override Visibility Convert(string value, object parameter, CultureInfo culture)
        {
            return (StringToBool(value)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public override string ConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Visibility.Visible:
                    return Boolean.TrueString;
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    return Boolean.FalseString;
            }

            return Boolean.FalseString;
        }
    }
}