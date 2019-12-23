using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Statistiky {
	class MetadataBalicku {

		/// <summary>
		/// Použije se jako vlastnost r:id
		/// </summary>
		public string RelaceId { get; set; }

		/// <summary>
		/// Název, pojmenování prvku
		/// </summary>
		public string Nazev { get; set; }

		/// <summary>
		/// Identifikátor pvku v rámci dané oblasti
		/// </summary>
		public string Identifikator { get; set; }
	}
}
