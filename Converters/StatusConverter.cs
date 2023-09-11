using System;
using System.Globalization;
using System.Windows.Data;

namespace ac.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int rstsValue = (int)value;

            switch (rstsValue)
            {
                case 1:
                    return "Подтверждено";
                case 2:
                    return "В работе";
                case 3:
                    return "Просрочено";
                case 4:
                    return "Изготовлено";
                case 5:
                    return "Отложено";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string ConvertThis(object value)
        {
            int rstsValue = (int)value;

            switch (rstsValue)
            {
                case 1:
                    return "Подтверждено";
                case 2:
                    return "В работе";
                case 3:
                    return "Просрочено";
                case 4:
                    return "Изготовлено";
                case 5:
                    return "Отложено";
                default:
                    return "";
            }
        }
    }
}
