using System.Collections.Generic;
using System.Globalization;

namespace Daliboris.Statistiky
{
	public class RazeniJevu : IComparer<string> 
	{
		public CompareOptions ZpusobRazeni { get; } = CompareOptions.StringSort;

		public int Compare(string x, string y) {
			return string.Compare(x, y, CultureInfo.CurrentCulture, ZpusobRazeni);
		}
	}
}