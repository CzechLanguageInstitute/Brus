using System;
using System.Globalization;
using System.Text;

namespace Daliboris.Text
{
	public class Slovo : IPocitatelny {
		private int intPocet = 1;
		private string strOriginalniPodoba;
		//private string strTypografickaPodoba;
		//private string strTextovaPodoba;
		//private TypografickyPrvek[] tgPrvek;
		public Slovo(string OriginalniPodoba) {
			strOriginalniPodoba = OriginalniPodoba;
		}
		public int Pocet {
			get { return intPocet; }
			set { intPocet = value; }
		}
		public string OriginalniPodoba { get { return strOriginalniPodoba; } }


		public static string Retrograd(string text, bool vypustitZavorky) {
			StringBuilder sb = new StringBuilder(text.Length);

			for (int i = text.Length - 1; i >= 0; i--) {
				switch (text[i]) {
					case ')':
						if (!vypustitZavorky)
							sb.Append('(');
						break;
					case '(':
						if (!vypustitZavorky)
							sb.Append(')');
						break;
					default:
						sb.Append(text[i]);
						break;
				}

			}
			if (text.ToLower().Contains("ch")) {
				sb.Replace("hc", "ch");
				sb.Replace("hC", "Ch");
				sb.Replace("Hc", "cH");
				sb.Replace("HC", "CH");
			}


			return sb.ToString();
		}

        public static string ZjistiZakonceni(string text)
        {
            string zakonceni;
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length <= 1)
                return text;
            if (text.Length >= 2 && String.Compare(text, text.Length - 2, "ch", 0, 2, true) == 0)
                zakonceni = "ch";
            else
            {
                zakonceni = text.Substring(text.Length - 1, 1);
            }
            return zakonceni;
        }

		public static string Retrograd(string text) {
			return Retrograd(text, false);
		}
		public static string Retrograd(string text, CultureInfo jazyk) {
			return Retrograd(text);
		}
	}
}