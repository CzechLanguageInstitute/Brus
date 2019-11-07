namespace Daliboris.Statistiky.Rozhrani.Jevy
{
	/// <summary>
	/// Výčet různých typů jevů; určení typu slouží k jejeich dalšímu zpracování (např. rozdělení úseku na slova a na znaky)
	/// </summary>
	public enum TypJevu {
		/// <summary>
		/// Ostatní, blíže nespecifikovaný typ jevu
		/// </summary>
		Ostatni,

		/// <summary>
		/// Delší úsek textu, např. text označný jedním znakovým stylem
		/// </summary>
		Useky,

		/// <summary>
		/// Celé odstavce textu
		/// </summary>
		Odstavce,

		/// <summary>
		/// Jednotlivá slova
		/// </summary>
		Slova,

		/// <summary>
		/// Jednotlivé znaky
		/// </summary>
		Znaky,

		/// <summary>
		/// Ngramy, tj. skupiny dvou a více znaků
		/// </summary>
		NGramy,

		/// <summary>
		/// Značky XML (jejich názvy)
		/// </summary>
		Tagy,

		/// <summary>
		/// Atributy XML
		/// </summary>
		Atributy,

		/// <summary>
		///Hodnoty atributů nebo obsah značek XML
		/// </summary>
		Hodnoty
	}
}