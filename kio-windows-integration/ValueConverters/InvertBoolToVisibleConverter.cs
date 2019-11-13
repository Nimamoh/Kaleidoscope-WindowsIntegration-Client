using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace kio_windows_integration.ValueConverters
{
    /// <summary>
    /// true => <see cref="Visibility.Collapsed"/>
    /// false => <see cref="Visibility.Visible"/>
    /// 
    /// <see cref="Visibility.Visible"/> => false
    /// <see cref="Visibility.Hidden"/> => true
    /// <see cref="Visibility.Collapsed"/> => true
    /// </summary>
    public class InvertBoolToVisibleConverter
        : BaseValueConverter<bool, Visibility>
    {
        private static readonly BoolToVisibleConverter Converter = new BoolToVisibleConverter();

        public override Visibility Convert(bool value, object parameter, CultureInfo culture)
        {
            return Converter.Convert(!value, parameter, culture);
        }

        public override bool ConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            var result = Converter.ConvertBack(value, parameter, culture);
            return !result;
        }
    }
}