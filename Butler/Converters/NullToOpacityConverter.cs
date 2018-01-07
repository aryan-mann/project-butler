using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Butler.Converters {

    public class NullToOpacityConverter: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }

    }

}
