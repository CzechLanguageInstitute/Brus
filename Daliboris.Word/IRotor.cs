using System;
using Daliboris.Statistiky.Core.Models.Jevy.XML;

namespace Daliboris.Statistiky.Word
{
	interface IRotor
	{
		event WordService.Progress Prubeh;
		WordSettings Settings { get; set; }
		SkupinaJevu SkupinaJevu { get; }
		string Soubor { get; set; }
		void Zpracuj();
	}
}
