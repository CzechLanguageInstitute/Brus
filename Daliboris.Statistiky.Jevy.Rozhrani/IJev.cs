using System.Collections.Generic;

namespace Daliboris.Statistiky.Rozhrani.Jevy {
	/// <summary>
	/// Rozhraní pro jev, jehož obsah má stanovený typ
	/// </summary>
	/// <typeparam name="T">formát obsahu</typeparam>
	public interface IJev<T> {
		string Nazev { get; set; }
		T Obsah { get; set; }
		int Pocet { get; set; }
	}

	/// <summary>
	/// Rozhraní pro jeden jev
	/// </summary>
	public interface IJev {
		string Nazev { get; set; }
		object Obsah { get; set; }
		int Pocet { get; set; }
		List<string> Kontexty { get; set; } 
	}
}
