using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LongListApp.Converters
{
    public class GreaterThanZeroToOpacityConverter : IValueConverter
    {
        private const double OpacityForEmpty = 0.666;
        private const double OpacityForNotEmpty = 1.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return OpacityForEmpty;

            try
            {
                return ((int)value) > 0 ? OpacityForNotEmpty : OpacityForEmpty;
            }
            catch (InvalidCastException)
            {
                return OpacityForNotEmpty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
