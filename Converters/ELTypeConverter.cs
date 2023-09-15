using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ac.Converters
{
    public class ELTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int rstsValue = (int)value;

                switch (rstsValue)
                {
                    case 1:
                        return "(Не менее)";
                    case 2:
                        return "(Не более)";
                    case 3:
                        return "(Диапазон)";
                    case 4:
                        return "(Контроль)";
                    case 5:
                        return "(Не использовать)";
                    case 6:
                        return "(Равно)";
                    default:
                        return "";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
