using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wazniak_forever.Converters
{
    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var time = (int)value;
            var mins = Math.Floor((double)(time / 60));
            var secs = time - mins * 60;
            if (time < 59)
            {
                return "00:" + ((time < 10) ? "0" + time : time.ToString());
            }
            else
            {
                return ((mins < 10) ? "0" + mins : mins.ToString()) + ":" + ((secs < 10) ? "0" + secs : secs.ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
