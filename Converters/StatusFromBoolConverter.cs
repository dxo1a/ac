using System;
using System.Globalization;
using System.Windows.Data;

namespace ac.Converters
{
    public class StatusFromBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool statusValue = (bool)value;

            switch (statusValue)
            {
                case true:
                    return "Готово";
                case false:
                    return "Не готово";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
