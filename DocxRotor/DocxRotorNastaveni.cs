﻿using System.Collections.Generic;

namespace Daliboris.Statistiky.Word
{
	/// <summary>
	/// Nastavení pro extrakci informací z dokumentu Docx
	/// </summary>
	public class DocxRotorNastaveni {
		/// <summary>
		/// Zda se do zpracování mají zahrnout poznámky pod čarou
		/// </summary>
		public bool ZahrnoutPoznamkyPodCarou { get; set; }

		/// <summary>
		/// Zda se má do výstupů
		/// </summary>
		public bool ZahrnoutTextOdstavce { get; set; }
		public bool ZahrnoutTextZnakovychStylu { get; set; }
		public bool OdstranitPocatecniAKoncoveMezery { get; set; }
	    public bool OdstranitTeckuUSlov { get; set; }
		/// <summary>
		/// Obsahuje seznam stylů, které zachycují lokaci (paginaci, foliaci) originálního textu.
		/// </summary>
		public List<string> StylyLokace { get; set; } 

		public DocxRotorNastaveni() {
			ZahrnoutTextOdstavce = true;
			ZahrnoutTextZnakovychStylu = true;
			OdstranitPocatecniAKoncoveMezery = false;
			ZahrnoutPoznamkyPodCarou = false;
		    OdstranitTeckuUSlov = true;
			StylyLokace = new List<string>();
		}
	}
}