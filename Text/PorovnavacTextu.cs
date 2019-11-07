using System;
using System.Collections.Generic;
using System.Collections;

namespace Daliboris.Text {
	/// <summary>
	/// Porovnávač textových řetězců rozlišující velikost písmen
	/// </summary>
	public class PorovnavacTextu : IComparer<string> {
		private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();
		#region IComparer<string> Members

		public int Compare(string x, string y) {
			int i = cic.Compare(x, y);
			if (i == 0) {
				return String.CompareOrdinal(x, y);
			}
			else
				return i;

		}

		#endregion
	}
}
