using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Statistiky {
	/// <summary>
	/// Formát, ve kterém budou (při ukládání) nebo jsou při načítání) uloženy seznamy s obsahem
	/// </summary>
 public	enum FormatUlozeniSeznamu {

	 /// <summary>
	 /// Seznamy budou uloženy ve formátu XML
	 /// </summary>
	 Xml,

	 /// <summary>
	 /// Seznamy budou uloženy v textovém formátu s oddělovači polí a hodnot
	 /// </summary>
	 Text
	}
}
