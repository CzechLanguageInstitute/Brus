using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky
{
    /// <summary>
    /// Metadata o jevu, které se využijí při sestavování balíčku OPC
    /// </summary>
    class MetadataOJevu
    {

        /// <summary>
        /// Typ jevu
        /// </summary>
        TypJevu Typ { get; set; }

        /// <summary>
        /// Pořadí jevu v rámci balíčku
        /// </summary>
        public int Poradi { get; set; }

        /// <summary>
        /// Zdroj (soubor, databáze), z něhož přehled jevů pochází
        /// </summary>
        public string Zdroj { get; set; }

        /// <summary>
        /// Popis jevů
        /// </summary>
        public string Popis { get; set; }

        /// <summary>
        /// Jazyk jevů
        /// </summary>
        public string Jazyk { get; set; }



        /// <summary>
        /// Datum a čas, kdy byl seznam jevů vytvořen
        /// </summary>
        public DateTime PosledniZmena { get; set; }

        /// <summary>
        /// Identifikátor v rámci balíčku
        /// </summary>
        public string Identifikator { get; set; }

        private Pocty mpocetUseku = new Pocty();

        /// <summary>
        /// Počet jevů
        /// </summary>
        public Pocty PocetUseku
        {
            get { return mpocetUseku; }
            set { mpocetUseku = value; }
        }

        private Pocty mpocetSlov = new Pocty();
        public Pocty PocetSlov
        {
            get { return mpocetSlov; }
            set { mpocetSlov = value; }
        }

        private Pocty mpocetZnaku = new Pocty();
        public Pocty PocetZnaku
        {
            get { return mpocetZnaku; }
            set { mpocetZnaku = value; }
        }

        private Pocty mpocetTrigramu = new Pocty();
        public Pocty PocetTrigramu
        {
            get { return mpocetTrigramu; }
            set { mpocetTrigramu = value; }
        }

        private Pocty mpocetDigramu = new Pocty();
        public Pocty PocetDigramu
        {
            get { return mpocetDigramu; }
            set { mpocetDigramu = value; }
        }

        private MetadataBalicku mdtMetadataBalicku = new MetadataBalicku();
        public MetadataBalicku MetadataBalicku
        {
            get
            {
                return mdtMetadataBalicku;
            }
            set
            {
                mdtMetadataBalicku = value;
            }
        }

        private MetadataBalicku mtdZnaky = new MetadataBalicku();
        public MetadataBalicku Znaky
        {
            get { return mtdZnaky; }
            set { mtdZnaky = value; }
        }

        private MetadataBalicku mtdSlova = new MetadataBalicku();
        public MetadataBalicku Slova
        {
            get { return mtdSlova; }
            set { mtdSlova = value; }
        }

        private MetadataBalicku mtdUseky = new MetadataBalicku();
        public MetadataBalicku Useky
        {
            get { return mtdUseky; }
            set { mtdUseky = value; }
        }

        private MetadataBalicku mtdDigramy = new MetadataBalicku();
        public MetadataBalicku Digramy
        {
            get { return mtdDigramy; }
            set { mtdDigramy = value; }
        }

        private MetadataBalicku mtdTrigramy = new MetadataBalicku();
        public MetadataBalicku Trigramy
        {
            get { return mtdTrigramy; }
            set { mtdTrigramy = value; }
        }


        private List<MetadataOJevu> mglsmdjPodrizeneJevy = new List<MetadataOJevu>();
        public List<MetadataOJevu> PodrizeneJevy
        {
            get
            {
                return mglsmdjPodrizeneJevy;
            }
            set
            {
                mglsmdjPodrizeneJevy = value;
            }
        }

        public static XmlDocument JakoXmlDocument(MetadataOJevu mtdMetadata)
        {
            XmlDocument xd = new XmlDocument();

            XmlNamespaceManager xnmsp = new XmlNamespaceManager(xd.NameTable);
            xnmsp.AddNamespace("r", JmenneProstory.Relationship);
            xnmsp.AddNamespace("", JmenneProstory.Statistiky);

            XmlNode xn = xd.CreateElement("styl");

            XmlAttribute xa = xd.CreateAttribute("xmlns");
            xa.Value = JmenneProstory.Statistiky;
            xn.Attributes.Append(xa);

            xa = xd.CreateAttribute("xmlns:r");
            xa.Value = JmenneProstory.Relationship;
            xn.Attributes.Append(xa);


            xa = xd.CreateAttribute("nazev");
            xa.Value = mtdMetadata.Popis;
            xn.Attributes.Append(xa);

            xa = xd.CreateAttribute("zdroj");
            xa.Value = mtdMetadata.Zdroj;
            xn.Attributes.Append(xa);

            xa = xd.CreateAttribute("posledniZmena");
            xa.Value = mtdMetadata.PosledniZmena.ToString();
            xn.Attributes.Append(xa);
            /*
            xa = xd.CreateAttribute("useku");
            xa.Value = mtdMetadata.Pocet.ToString();
            xn.Attributes.Append(xa);

            xa = xd.CreateAttribute("slov");
            xa.Value = mtdMetadata.PocetSlov.ToString();
            xn.Attributes.Append(xa);

            xa = xd.CreateAttribute("znaku");
            xa.Value = mtdMetadata.PocetZnaku.ToString();
            xn.Attributes.Append(xa);
            */

            XmlNode xno = xd.CreateElement("obsah");
            XmlNode x = VytvoritTagZMetadat(xd, mtdMetadata.Useky, "useky", "úseky", mtdMetadata.PocetUseku);
            xno.AppendChild(x);

            x = VytvoritTagZMetadat(xd, mtdMetadata.Slova, "slova", "slova", mtdMetadata.PocetSlov);
            xno.AppendChild(x);

            x = VytvoritTagZMetadat(xd, mtdMetadata.Znaky, "znaky", "znaky", mtdMetadata.PocetZnaku);
            xno.AppendChild(x);

            x = VytvoritTagZMetadat(xd, mtdMetadata.Digramy, "digramy", "digramy", mtdMetadata.PocetZnaku);
            xno.AppendChild(x);

            x = VytvoritTagZMetadat(xd, mtdMetadata.Trigramy, "trigramy", "trigramy", mtdMetadata.PocetZnaku);
            xno.AppendChild(x);

            xn.AppendChild(xno);

            xd.AppendChild(xn);

            return xd;

        }

        public static XmlNode VytvoritTagZMetadat(XmlDocument xd, MetadataBalicku mtdMetadata, string nazevPrvku, string strPojmenovani, Pocty pcPocty)
        {
            XmlAttribute xa;
            XmlNode xnu = xd.CreateElement(nazevPrvku);
            xa = xd.CreateAttribute("name");
            xa.Value = strPojmenovani;
            xnu.Attributes.Append(xa);

            xa = xd.CreateAttribute(nazevPrvku + "Id");
            xa.Value = nazevPrvku; //TODO Jak vyřešit příponu?
            xnu.Attributes.Append(xa);

            xa = xd.CreateAttribute("jedinecnych");
            xa.Value = pcPocty.Jedinecnych.ToString();
            xnu.Attributes.Append(xa);


            xa = xd.CreateAttribute("pocet");
            xa.Value = pcPocty.Celkem.ToString();
            xnu.Attributes.Append(xa);

            //TODO  Rozlišit celkový počet a počet jedinečných (což je teď)

            xa = xd.CreateAttribute("r", "id", JmenneProstory.Relationship);
            xa.Value = mtdMetadata.RelaceId;
            xnu.Attributes.Append(xa);

            return xnu;
        }

        public static XmlNode VytvoritTagZMetadatProPrehled(XmlDocument xd, MetadataBalicku mtdMetadata, string nazevPrvku, string strPojmenovani,
            Pocty pcUseky, Pocty pcSlova, Pocty pcZnaky, Pocty pcDigramy, Pocty pcTrigramy)
        {
            XmlAttribute xa;
            XmlNode xnu = xd.CreateElement(nazevPrvku);
            xa = xd.CreateAttribute("nazev");
            xa.Value = strPojmenovani;
            xnu.Attributes.Append(xa);

            //TODO  Rozlišit celkový počet a počet jedinečných (což je teď)

            //xa = xd.CreateAttribute("r", "id", JmenneProstory.Relationship);
            xa = xd.CreateAttribute("id");
            xa.Value = mtdMetadata.RelaceId;
            xnu.Attributes.Append(xa);

            XmlNode xno = xd.CreateElement("obsah");
            VytvoritPolozkuObsahu(xno, pcUseky, "useky");
            VytvoritPolozkuObsahu(xno, pcSlova, "slova");
            VytvoritPolozkuObsahu(xno, pcZnaky, "znaky");
            VytvoritPolozkuObsahu(xno, pcDigramy, "digramy");
            VytvoritPolozkuObsahu(xno, pcTrigramy, "trigramy");


            xnu.AppendChild(xno);

            return xnu;
        }

        private static void VytvoritPolozkuObsahu(XmlNode xno, Pocty pcPocty, string strNazev)
        {
            XmlDocument xd = xno.OwnerDocument;
            XmlAttribute xa;
            XmlNode x = xd.CreateElement(strNazev);

            xa = xd.CreateAttribute("jedinecne");
            xa.Value = pcPocty.Jedinecnych.ToString();
            x.Attributes.Append(xa);

            xa = xd.CreateAttribute("celkem");
            xa.Value = pcPocty.Celkem.ToString();
            x.Attributes.Append(xa);
            xno.AppendChild(x);
        }
    }
}
