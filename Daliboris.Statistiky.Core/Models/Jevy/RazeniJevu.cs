using System.Collections.Generic;
using System.Globalization;

namespace Daliboris.Statistiky
{
	public class RazeniJevu : IComparer<string> {
		private readonly CompareOptions mcoZpusobRazeni = CompareOptions.StringSort;

		public CompareOptions ZpusobRazeni {
			get { return mcoZpusobRazeni; }
		}

		public RazeniJevu() {}
		public RazeniJevu(CompareOptions coZpusobRazeni) {
			mcoZpusobRazeni = coZpusobRazeni;
		}

		#region IComparer<string> Members

		public int Compare(string x, string y) {
			return string.Compare(x, y, CultureInfo.CurrentCulture, mcoZpusobRazeni);
		}

		#endregion
	}
}