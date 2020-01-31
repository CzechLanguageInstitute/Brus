using System;

namespace Daliboris.Word.Text
{
    interface IShoda
    {
        string Predchozi { get; set; }
        string Nasledujici { get; set; }
        string Aktualni { get; set; }
        string Zacatek { get; set; }
        string Stred { get; set; }
        string Konec { get; set; }
        bool Retrograd { get; set; }
        StringComparison ZpusobPorovnani { get; set; }
    }
}