using System;
using System.Globalization;
using System.Windows.Data;

namespace kio_windows_integration.ValueConverters
{
    public abstract class BaseValueConverter<T, TR> : IValueConverter
    {
        public abstract TR Convert(T value, object parameter, CultureInfo culture);

        public abstract T ConvertBack(TR value, object parameter, CultureInfo culture);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var casted = (T) value;
            return this.Convert(casted, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var casted = (TR) value;
            return this.ConvertBack(casted, parameter, culture);
        }


        #region Helpers

        private class FromLamdbas<TU, TUr> : BaseValueConverter<TU, TUr>
        {
            public Func<TU, TUr> Converter { get; set; }
            public Func<TUr, TU> BackConverter { get; set; }

            public override TUr Convert(TU value, object parameter, CultureInfo culture)
            {
                return Converter.Invoke(value);
            }

            public override TU ConvertBack(TUr value, object parameter, CultureInfo culture)
            {
                return BackConverter.Invoke(value);
            }
        }

        /// <summary>
        /// Make a <see cref="IValueConverter"/> using converters passed in parameter
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="backConverter"></param>
        public static BaseValueConverter<T, TR> Make(
            Func<T, TR> converter,
            Func<TR, T> backConverter
        )
        {
            return new FromLamdbas<T, TR>
            {
                Converter = converter,
                BackConverter = backConverter
            };
        }

        #endregion
    }
}