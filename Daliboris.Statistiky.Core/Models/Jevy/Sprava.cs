using System;
using System.IO;
using System.Xml.Serialization;
using Daliboris.Pomucky.Xml;
using System.Collections;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky {
	public class Sprava {
		public const string csNamespace = "http://www.daliboris.cz/schemata/statistiky.xsd";
		private const char cchOddelovacPoli = '\u001d';

		private string mstrSouborUlozeni;

		public string SouborUlozeni {
			get { return mstrSouborUlozeni; }
			set { mstrSouborUlozeni = value; }
		}



		public static Jevy NactiZeSouboruXml(string strSoubor) {
		 Sprava sp = new Sprava();
		 return sp.NactiZeSouboru(strSoubor);
		}
		public Jevy NactiZeSouboru(string strSoubor) {
			mstrSouborUlozeni = strSoubor;
			return NactiZeSouboru();
		}

		public Jevy NactiZeSouboru() {
			jevy jvs;
			jvs = (jevy)Daliboris.Pomucky.Xml.Serializace.NacistZXml(mstrSouborUlozeni, typeof(jevy));
			string sJazyk = jvs.jazyk;

			Jevy jvJevy = new Jevy();
			jvJevy.Popis = jvs.popis;
			jvJevy.PosledniZmena = jvs.vytvoreno;
			jvJevy.Zdroj.CelaCesta = jvs.zdroj;
			jvJevy.Jazyk = sJazyk;
			jvJevy.Typ = (TypJevu)jvs.typ;
			if (jvs.j != null) {
				foreach (j jv in jvs.j) {
					Jev jev = new Jev(sJazyk, jv.n, jv.o, jv.r, jv.p);
					jvJevy.Add(jev);
				}
			}
			return jvJevy;
		}

		public void UlozDoSouboru(IJevyZdroje jvJevy, string strSoubor) {
			mstrSouborUlozeni = strSoubor;
			UlozDoSouboru(jvJevy);
		}
		public void UlozDoSouboru(IJevy jvJevy, string strSoubor) {
			mstrSouborUlozeni = strSoubor;
			//UlozDoSouboru(jvJevy);
		}

		private void UlozDoSouboru(IJevyZdroje jvJevy) {
			jevy jvs = new jevy();
			jvs.zdroj = jvJevy.Zdroj.CelaCesta;
			if (jvs.vytvoreno == DateTime.MinValue)
				jvs.vytvoreno = DateTime.Now;
			jvs.popis = jvJevy.Popis;
			jvs.pocet = jvJevy.Pocet;
			jvs.identifikator = jvJevy.Identifikator;
			jvs.jazyk = jvJevy.Jazyk;
			jvs.typ = (jevyTyp)jvJevy.Typ;
			if (jvJevy.Pocet > 0) {
				int i = 0;
				j[] jv = new j[jvJevy.Pocet];
				//foreach (ITextovyJev jj in (ICollection) jvJevy) {
				foreach (Jev jj in (ICollection) jvJevy) {
					j j = new j();
					j.n = jj.Nazev;
					j.p = jj.Pocet;
					if (jj.Obsah != null)
						j.o = jj.Obsah;
					if (jj.Retrograd != null)
						j.r = jj.Retrograd;
					jv[i++] = j;
				}
				jvs.j = jv;
			}
			XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
			xsn.Add(String.Empty, "http://www.daliboris.cz/schemata/statistiky.xsd");
			xsn.Add("q", "http://microsoft.com/wsdl/types/");
			xsn.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			Daliboris.Pomucky.Xml.Serializace.UlozitDoXml(jvs, mstrSouborUlozeni, xsn);

		}

		public static Jevy NactiJevy(Stream stream)
		{
			Jevy jv = new Jevy();
			using (StreamReader sr = new StreamReader(stream))
			{
				{
					string sRadek = null;
					while ((sRadek = sr.ReadLine()) != null)
					{
						string[] asRadek = sRadek.Split(new char[] {cchOddelovacPoli});
						jv.Add(new Jev(null, asRadek[0], Int32.Parse(asRadek[1])));
					}
				}
			}
			return jv;
		}


	    public static void UlozDoSouboru(Jevy jvJevy, string  strSoubor, FormatUlozeni fmtFormat)
	    {
	        UlozDoSouboru(jvJevy, strSoubor, fmtFormat, cchOddelovacPoli);
	    }

	    public static void UlozDoSouboru(Jevy jvJevy, string strSoubor, FormatUlozeni fmtFormat, char oddelovacPoli)
		{
			if(fmtFormat == FormatUlozeni.Xml)
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
			jevy jvs = new jevy();
			jvs.zdroj = jvJevy.Zdroj.CelaCesta;
			if (jvs.vytvoreno == DateTime.MinValue)
				jvs.vytvoreno = DateTime.Now;
			jvs.popis = jvJevy.Popis;
			jvs.pocet = jvJevy.Pocet;
			jvs.identifikator = jvJevy.Identifikator;
			jvs.jazyk = jvJevy.Jazyk;
			jvs.typ = (jevyTyp)jvJevy.Typ;
			if (jvJevy.Pocet > 0) {
				int i = 0;
				j[] jv = new j[jvJevy.Pocet];
				//foreach (ITextovyJev jj in (ICollection) jvJevy) {
				foreach (Jev jj in (ICollection)jvJevy) {
					j j = new j();
					j.n = jj.Nazev;
					j.p = jj.Pocet;
					if (jj.Obsah != null)
						j.o = jj.Obsah;
					if (jj.Retrograd != null)
						j.r = jj.Retrograd;
					int pocetKontextu = jj.Kontexty.Count;
					if (jj.Kontexty != null && pocetKontextu > 0)

					{
						j.lokace = new lokace();
						j.lokace.l = new lokaceL[pocetKontextu];
						j.lokace.p = pocetKontextu;
						for (int k = 0; k < pocetKontextu; k++)
						{
							j.lokace.l[k] = new lokaceL();
							j.lokace.l[k].l = jj.Kontexty[k];
						}
					}

					jv[i++] = j;
				}
				jvs.j = jv;
			}
			XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
			xsn.Add(String.Empty, "http://www.daliboris.cz/schemata/statistiky.xsd");
			xsn.Add("q", "http://microsoft.com/wsdl/types/");
			xsn.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			Daliboris.Pomucky.Xml.Serializace.UlozitDoXml(jvs, strSoubor, xsn);
		}

		public void UlozDoSouboru(Jevy jvJevy, string strSoubor) {
			mstrSouborUlozeni = strSoubor;
			UlozDoSouboru(jvJevy);
		}
		public void UlozDoSouboru(Jevy jvJevy) {
			UlozDoSouboru((IJevyZdroje)jvJevy);
		}
	}

	public enum FormatUlozeni
	{
		Xml,
		Text
	}
}
