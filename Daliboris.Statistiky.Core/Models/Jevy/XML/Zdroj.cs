using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky
{
    public class Zdroj : IZdroj
    {
        //Doplněno: zachovat označení zdroje, nemusí jít jenom o soubor, ale i databázi ap.
        public string Oznaceni { get; set; }
        public string Slozka { get; set; }
        public string Soubor { get; set; }

        public string CelaCesta
        {
            get { return Slozka + Soubor; }
            set
            {
                Oznaceni = value;
                if (value.LastIndexOf('\\') > 0)
                {
                    Slozka = value.Substring(0, value.LastIndexOf('\\'));
                    Soubor = value.Substring(value.LastIndexOf('\\') + 1);
                }
            }
        }

        public Zdroj()
        {
        }

        public Zdroj(string sCelaCesta)
        {
            CelaCesta = sCelaCesta;
        }

        public Zdroj(string sSlozka, string sSoubor)
        {
            Slozka = sSlozka;
            Soubor = sSoubor;
        }
    }
}