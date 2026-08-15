using SmartLedger.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartLedger.Converters
{
    public class StatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZahlungsStatus status)
            {
                return status switch
                {
                    ZahlungsStatus.BestaetigtManuell => new SolidColorBrush(Color.FromRgb(16, 137, 62)),   // grün
                    ZahlungsStatus.VorschlagKI => new SolidColorBrush(Color.FromRgb(37, 99, 235)),          // blau
                    _ => new SolidColorBrush(Color.FromRgb(58, 58, 58))                                     // grau (offen)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}