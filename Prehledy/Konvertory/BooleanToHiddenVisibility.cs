using System;
using System.Windows;
using System.Windows.Data;

namespace Daliboris.Statistiky.UI.WPF.Konvertory
{
	[ValueConversion(typeof(Boolean), typeof(Visibility))]
	public class BooleanToHiddenVisibility : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Visibility rv = Visibility.Visible;
			try {
				bool x = bool.Parse(value.ToString());
				rv = x ? Visibility.Visible : Visibility.Collapsed;
			}
			catch {
			}
			return rv;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}

	}
}