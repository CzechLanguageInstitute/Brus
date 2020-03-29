using System;
using System.Windows.Data;

namespace Daliboris.Statistiky.UI.WPF.Converters {
	[ValueConversion(typeof(char), typeof(int))]
	[ValueConversion(typeof(char), typeof(string))]
	public class Character2Kod : IValueConverter {

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			char ch = '\0';
			if (value is char)
				ch = (char)value;
			else if (value is string) {
				string s = (string)value;
				ch = s[0];
			}
			if (ch == '\0')
				return 0;
			if (parameter == null)
				return (int)ch;
			
			switch (parameter.ToString()) {
				case "hex":
					string sHex = String.Format("{0:X4}", (int)ch);
					return sHex;
				case "kategorie":
					string sPopis = Char.GetUnicodeCategory(ch).ToString("g");
					return sPopis;
				default:
					return (int)ch;
			}


		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
