using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Statistiky.Core.Models.Jevy.XML;

namespace Daliboris.Statistiky {
	public class Prehled 
	{
		public string Popis { get; set; }

		public Zdroj Zdroj { get; set; }

		public DateTime PosledniZmena { get; set; }

		public string Identifikator { get; set; }

		public SkupinaJevu SkupinaJevu { get; set; } = new SkupinaJevu();
	}
}
