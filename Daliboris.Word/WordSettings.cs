using System.Collections.Generic;

namespace Daliboris.Statistiky.Word
{
    /// <summary>
    /// Nastavení pro extrakci informací z dokumentu Docx
    /// </summary>
    public class WordSettings
    {
        public bool ZahrnoutPoznamkyPodCarou { get; set; }
        public bool ZahrnoutTextOdstavce { get; set; }
        public bool ZahrnoutTextZnakovychStylu { get; set; }
        public bool OdstranitPocatecniAKoncoveMezery { get; set; }
        public bool OdstranitTeckuUSlov { get; set; }

        /// <summary>
        /// Obsahuje seznam stylů, které zachycují lokaci (paginaci, foliaci) originálního textu.
        /// </summary>
        public List<string> StylyLokace { get; set; }

        public WordSettings()
        {
            ZahrnoutTextOdstavce = true;
            ZahrnoutTextZnakovychStylu = true;
            OdstranitPocatecniAKoncoveMezery = true;
            ZahrnoutPoznamkyPodCarou = false;
            OdstranitTeckuUSlov = true;
            StylyLokace = new List<string>();
        }
    }
}