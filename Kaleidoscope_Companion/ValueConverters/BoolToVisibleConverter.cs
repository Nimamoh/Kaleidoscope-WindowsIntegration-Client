using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace kaleidoscope_companion.ValueConverters
{
    /// <summary>
    /// true => <see cref="Visibility.Visible"/>
    /// false => <see cref="Visibility.Collapsed"/>
    /// 
    /// <see cref="Visibility.Visible"/> => true
    /// <see cref="Visibility.Hidden"/> => false
    /// <see cref="Visibility.Collapsed"/> => false
    /// </summary>
    public class BoolToVisibleConverter
        : BaseValueConverter<bool, Visibility>
    {
        public override Visibility Convert(bool value, object parameter, CultureInfo culture)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public override bool ConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    return false;
            }

            return false;
        }
    }
}