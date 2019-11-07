using System;
using System.Collections.Generic;
using System.Text;

namespace Daliboris.Text {
 public	class Retezec {

		/// <summary>
		/// Rozdělí vstupní řetězec na jednotlivé části, včetně mezer
		/// </summary>
		/// <param name="strText"></param>
		/// <returns></returns>
		public static string[] RozdelitNaCasti(string strText)
		{
			List<string> glsCasti = new List<string>(strText.Length);
			StringBuilder sb = new StringBuilder(strText.Length);
			foreach (char c in strText)
			{
				if (c == ' ')
				{
					if (sb.Length > 0)
						glsCasti.Add(sb.ToString());
					glsCasti.Add(c.ToString());
					glsCasti = new List<string>(strText.Length);
				}
				else
				{
					glsCasti.Add(c.ToString());
				}
			}

			return glsCasti.ToArray();
		}

	}
}
