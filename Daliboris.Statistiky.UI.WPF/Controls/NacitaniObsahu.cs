namespace Daliboris.Statistiky.UI.WPF.Controls
{
    public class NacitaniObsahu
    {
        public string Soubor { get; set; }
        public string IdentifikatorRelace { get; set; }

        public NacitaniObsahu(string strSoubor, string strIdentifikatorRelace)
        {
            Soubor = strSoubor;
            IdentifikatorRelace = strIdentifikatorRelace;
        }
    }
}