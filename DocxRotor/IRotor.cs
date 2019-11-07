using System;
namespace Daliboris.Statistiky.Word
{
	interface IRotor
	{
		event DocxRotor.Progress Prubeh;
		DocxRotorNastaveni Settings { get; set; }
		Daliboris.Statistiky.SkupinaJevu SkupinaJevu { get; }
		string Soubor { get; set; }
		void Zpracuj();
	}
}
