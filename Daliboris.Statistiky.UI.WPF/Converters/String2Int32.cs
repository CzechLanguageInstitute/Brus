using System;
using System.Windows.Data;

namespace Daliboris.Statistiky.UI.WPF.Converters {
	[ValueConversion(typeof(String), typeof(Int32))]
	public class String2Int32 :  IValueConverter {
		#region IValueConverter - členové

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int i;
			string s = value as string;
			Int32.TryParse(s, out i);
			return i;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int i = Int32.MinValue;
			try
			{
				i = (int) value;
			}
			catch
			{
				
			}
			return i.ToString();
			
		}

		#endregion
	}
}
