using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Statistiky {
	public class Prehled {
		private SkupinaJevu mskjJevy = new SkupinaJevu();
		private Zdroj mzdrZdroj;
		private DateTime mdtPosledniZmena;
		private string mstrPopis;
		private string mstrIdentifikator;

		public string Popis {
			get { return mstrPopis; }
			set { mstrPopis = value; }
		}
		public Zdroj Zdroj {
			get { return (mzdrZdroj); }
			set { mzdrZdroj = value; }
		}

		public DateTime PosledniZmena {
			get { return (mdtPosledniZmena); }
			set { mdtPosledniZmena = value; }
		}

		public string Identifikator {
			get { return mstrIdentifikator; }
			set { mstrIdentifikator = value; }
		}
		public SkupinaJevu SkupinaJevu
		{
			get { return mskjJevy; }
			set { mskjJevy = value; }
		}
	}
}
