using System;
using System.IO;
using System.Xml.Serialization;
using Daliboris.Pomucky.Xml;
using System.Collections;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky
{
    public class JevyService
    {
        public const string csNamespace = "http://www.daliboris.cz/schemata/statistiky.xsd";
        private const char cchOddelovacPoli = '\u001d';

        private string mstrSouborUlozeni;

        public string SouborUlozeni
        {
            get { return mstrSouborUlozeni; }
            set { mstrSouborUlozeni = value; }
        }

        public static Jevy NactiZeSouboruXml(string strSoubor)
        {
            JevyService sp = new JevyService();
            return sp.NactiZeSouboru(strSoubor);
        }

        public Jevy NactiZeSouboru(string strSoubor)
        {
            mstrSouborUlozeni = strSoubor;
            return NactiZeSouboru();
        }

        public Jevy NactiZeSouboru()
        {
            var jazykovéJevy = (JazykovéJevy) SavingTools.NacistZXml(mstrSouborUlozeni, typeof(JazykovéJevy));

            var jvJevy = new Jevy();
            string sJazyk = jazykovéJevy.Jazyk;
            jvJevy.Popis = jazykovéJevy.Popis;
            jvJevy.PosledniZmena = jazykovéJevy.Vytvoreno;
            jvJevy.Zdroj.CelaCesta = jazykovéJevy.Zdroj;
            jvJevy.Jazyk = sJazyk;
            jvJevy.Druh = (TypJevu) jazykovéJevy.TypJevu;

            if (jazykovéJevy.Value != null)
            {
                foreach (var jevyXml in jazykovéJevy.Value)
                {
                    var jev = new Jev(sJazyk, jevyXml.NazevJevu, jevyXml.Obsah, jevyXml.RetrográdníPodobaTextu, jevyXml.PocetCharakteristik);
                    jvJevy.Add(jev);
                }
            }

            return jvJevy;
        }

        private void UlozDoSouboru(IJevyZdroje jvJevy)
        {
            var jazykovéJevy = new JazykovéJevy();
            jazykovéJevy.Zdroj = jvJevy.Zdroj.CelaCesta;
            if (jazykovéJevy.Vytvoreno == DateTime.MinValue)
                jazykovéJevy.Vytvoreno = DateTime.Now;
            jazykovéJevy.Popis = jvJevy.Popis;
            jazykovéJevy.Pocet = jvJevy.Pocet;
            jazykovéJevy.Identifikator = jvJevy.Identifikator;
            jazykovéJevy.Jazyk = jvJevy.Jazyk;
            jazykovéJevy.TypJevu = (TypJevu) jvJevy.Druh;
            if (jvJevy.Pocet > 0)
            {
                int i = 0;
                JazykovyJev[] jv = new JazykovyJev[jvJevy.Pocet];
                //foreach (ITextovyJev jj in (ICollection) jvJevy) {
                foreach (Jev jj in (ICollection) jvJevy)
                {
                    JazykovyJev jazykovyJev = new JazykovyJev();
                    jazykovyJev.NazevJevu = jj.Nazev;
                    jazykovyJev.PocetCharakteristik = jj.Pocet;
                    if (jj.Obsah != null)
                        jazykovyJev.Obsah = jj.Obsah;
                    if (jj.Retrograd != null)
                        jazykovyJev.RetrográdníPodobaTextu = jj.Retrograd;
                    jv[i++] = jazykovyJev;
                }

                jazykovéJevy.Value = jv;
            }

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();

            xsn.Add(String.Empty, "http://www.daliboris.cz/schemata/statistiky.xsd");
            xsn.Add("q", "http://microsoft.com/wsdl/types/");
            xsn.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            SavingTools.UlozitDoXml(jazykovéJevy, mstrSouborUlozeni, xsn);
        }

        public static Jevy NactiJevy(Stream stream)
        {
            var jevyXml = new Jevy();
            using (StreamReader sr = new StreamReader(stream))
            {
                {
                    string sRadek = null;
                    while ((sRadek = sr.ReadLine()) != null)
                    {
                        var asRadek = sRadek.Split(new char[] {cchOddelovacPoli});
                        jevyXml.Add(new Jev(null, asRadek[0], Int32.Parse(asRadek[1])));
                    }
                }
            }

            return jevyXml;
        }


        public static void UlozDoSouboru(Jevy jvJevy, string strSoubor, FormatUlozeni fmtFormat)
        {
            UlozDoSouboru(jvJevy, strSoubor, fmtFormat, cchOddelovacPoli);
        }

        public static void UlozDoSouboru(Jevy jvJevy, string strSoubor, FormatUlozeni fmtFormat, char oddelovacPoli)
        {
            if (fmtFormat == FormatUlozeni.Xml)
                UlozDoSouboruXml(jvJevy, strSoubor);
            if (fmtFormat == FormatUlozeni.Text)
                UlozDoSouboruTxt(jvJevy, strSoubor, oddelovacPoli);
        }

        private static void UlozDoSouboruTxt(Jevy jvJevy, string strSoubor, char oddelovacPoli)
        {
            using (StreamWriter sw = new StreamWriter(strSoubor))
            {
                foreach (Jev jv in jvJevy)
                {
                    //TODO Doplnit odkaz na Daliboris.Pomucky.Texty a konstantu oddělovače
                    //TODO Ukládata i informace o jazyce? Promyslet ztrátu data ve srovnání s formátem Xml
                    sw.WriteLine("{1}{0}{2}{0}{3}", oddelovacPoli, jv.Nazev, jv.Pocet, jv.Retrograd);
                }
            }
        }

        private static void UlozDoSouboruXml(Jevy jvJevy, string strSoubor)
        {
            JazykovéJevy jvs = new JazykovéJevy();
            jvs.Zdroj = jvJevy.Zdroj.CelaCesta;
            if (jvs.Vytvoreno == DateTime.MinValue)
                jvs.Vytvoreno = DateTime.Now;
            jvs.Popis = jvJevy.Popis;
            jvs.Pocet = jvJevy.Pocet;
            jvs.Identifikator = jvJevy.Identifikator;
            jvs.Jazyk = jvJevy.Jazyk;
            jvs.TypJevu = (TypJevu) jvJevy.Druh;
            if (jvJevy.Pocet > 0)
            {
                int i = 0;
                var jv = new JazykovyJev[jvJevy.Pocet];
                //foreach (ITextovyJev jj in (ICollection) jvJevy) {
                foreach (Jev jj in (ICollection) jvJevy)
                {
                    var jazykovyJev = new JazykovyJev();
                    jazykovyJev.NazevJevu = jj.Nazev;
                    jazykovyJev.PocetCharakteristik = jj.Pocet;
                    if (jj.Obsah != null)
                        jazykovyJev.Obsah = jj.Obsah;
                    if (jj.Retrograd != null)
                        jazykovyJev.RetrográdníPodobaTextu = jj.Retrograd;
                    int pocetKontextu = jj.Kontexty.Count;
                    if (jj.Kontexty != null && pocetKontextu > 0)

                    {
                        jazykovyJev.LokaceJevu = new Lokace();
                        jazykovyJev.LokaceJevu.LokaceVyskytu = new LokaceVyskytu[pocetKontextu];
                        jazykovyJev.LokaceJevu.PočetVýskytů = pocetKontextu;
                        for (int k = 0; k < pocetKontextu; k++)
                        {
                            jazykovyJev.LokaceJevu.LokaceVyskytu[k] = new LokaceVyskytu();
                            jazykovyJev.LokaceJevu.LokaceVyskytu[k].Value = jj.Kontexty[k];
                        }
                    }
                    jv[i++] = jazykovyJev;
                }
                jvs.Value = jv;
            }

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(String.Empty, "http://www.daliboris.cz/schemata/statistiky.xsd");
            xsn.Add("q", "http://microsoft.com/wsdl/types/");
            xsn.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            SavingTools.UlozitDoXml(jvs, strSoubor, xsn);
        }

        public void UlozDoSouboru(Jevy jvJevy, string strSoubor)
        {
            mstrSouborUlozeni = strSoubor;
            UlozDoSouboru(jvJevy);
        }

        public void UlozDoSouboru(Jevy jvJevy)
        {
            UlozDoSouboru((IJevyZdroje) jvJevy);
        }
    }

    public enum FormatUlozeni
    {
        Xml,
        Text
    }
}