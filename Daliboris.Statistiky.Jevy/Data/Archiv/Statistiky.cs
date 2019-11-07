using System;
//using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.ComponentModel;
using Daliboris.Pomucky.Xml;

namespace Daliboris.Statistiky
{

    public static class Sprava
    {
        public const string csNamespace = "http://www.daliboris.cz/schemata/statistiky.xsd";

        public static void UlozitDoXml(object objekt, string strSoubor) {
			  XmlWriterSettings xws = new XmlWriterSettings();
			  xws.CloseOutput = true;
			  xws.Encoding = System.Text.Encoding.UTF8;
			  xws.IndentChars = " ";
			  xws.Indent = true;

			  XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
			  //XmlSerializer xs = new XmlSerializer(objekt.GetType(), csNamespace);
			  XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
			  xsn.Add(String.Empty, String.Empty);
			  xsn.Add(String.Empty, csNamespace);
			  XmlSerializer xs = new XmlSerializer(objekt.GetType());

			  xs.Serialize(xw, objekt, xsn);
			  xw.Close();
			  xw = null;
			  xs = null;
		  }

		  public static object NacistZXml(string strSoubor, Type typ) {
			  if (!File.Exists(strSoubor)) {
				  throw new FileNotFoundException("Uvedený soubor '" + strSoubor + "' neexistuje");
			  }
			  XmlSerializer xs = new XmlSerializer(typ, csNamespace);
			  FileStream fs = new FileStream(strSoubor, FileMode.Open, FileAccess.Read);
			  XmlReader reader = XmlReader.Create(fs);
			  object objekt = xs.Deserialize(reader);
			  fs.Close();
			  return objekt;
		  }

        public static void UlozitDoXml(Jevy jvJevy, string strSoubor)
        {
            UlozitDoXml(jvJevy, strSoubor, false);
        }
        public static void UlozitDoXml(Jevy jvJevy, string strSoubor, bool blnRozlisitZdroj)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = System.Text.Encoding.UTF8;
            xws.IndentChars = " ";
            xws.Indent = true;

            XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
            XmlSerializer xs = new XmlSerializer(typeof(Jevy), csNamespace);

            xs.Serialize(xw, jvJevy);
            xw.Close();
            xw = null;
            xs = null;
        }
        public static Jevy NacistZXml(string strSoubor)
        {
            if (!File.Exists(strSoubor))
            {
                throw new FileNotFoundException("Uvedený soubor '" + strSoubor + "' neexistuje");
            }
            XmlSerializer xs = new XmlSerializer(typeof(Jevy), csNamespace);
            FileStream fs = new FileStream(strSoubor, FileMode.Open, FileAccess.Read);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.
            Jevy jv;

            // Use the Deserialize method to restore the object's state.
            jv = (Jevy)xs.Deserialize(reader);
            fs.Close();
            return jv;

        }
    }

    /// <summary>
    /// Rozhraní pro jev, jehož obsah má stanovený typ
    /// </summary>
    /// <typeparam name="T">formát obsahu</typeparam>
    public interface IJev<T>
    {
        string Nazev { get; set; }
        T Obsah { get; set; }
        int Pocet { get; set; }
    }

    public interface IJev
    {
        string Nazev { get; set; }
        object Obsah { get; set; }
        int Pocet { get; set; }
    }

    public interface ILokalizovanyJev : IJev
    {
        ArrayList Vyskyty { get; set; }
    }

    [XmlRoot("jev")]
    public class Jev<T> : IJev<T>
    {
        private string mstrNazev;
        private int mintPocet = 1;
        private T mgenObsah;

        public Jev() { }
        public Jev(string strNazev)
        { mstrNazev = strNazev; }


        public Jev(string strNazev, int intPocet)
            : this(strNazev)
        {
            mintPocet = intPocet;
        }

        public Jev(string strNazev, T objObsah)
        {
            mstrNazev = strNazev;
            mgenObsah = objObsah;
        }

        public Jev(string strNazev, T objObsah, int intPocet)
            : this(strNazev, objObsah)
        {
            mintPocet = intPocet;
        }

        [XmlAttribute(AttributeName = "nazev")]
        public string Nazev
        {
            get { return mstrNazev; }
            set { mstrNazev = value; }
        }
        [XmlAttribute(AttributeName = "pocet")]
        public int Pocet
        {
            get { return mintPocet; }
            set { mintPocet = value; }
        }

        [XmlAttribute(AttributeName = "obsah")]
        public T Obsah
        {
            get { return mgenObsah; }
            set { mgenObsah = value; }
        }




        #region IJev<T> Members

        string IJev<T>.Nazev
        {
            get { return mstrNazev; }
            set { mstrNazev = value; }
        }

        T IJev<T>.Obsah
        {
            get { return mgenObsah; }
            set { mgenObsah = value; }
        }

        int IJev<T>.Pocet
        {
            get { return mintPocet; }
            set { mintPocet = value; }
        }

        #endregion
    }
    
    [Serializable]
    [XmlRootAttribute("jev")]
    public class Jev : IJev
    {
        private string mstrNazev;
        private int mintPocet = 1;
        private object mobjObsah = null;


        public Jev() { }
        public Jev(string strNazev): this(strNazev, (object) strNazev)
        {       }


        public Jev(string strNazev, int intPocet)
            : this(strNazev)
        {
            mintPocet = intPocet;
        }

        public Jev(string strNazev, object objObsah)
        {
            mstrNazev = strNazev;
            mobjObsah = objObsah;
        }

        public Jev(string strNazev, object objObsah, int intPocet):this(strNazev, objObsah)
        {
            mintPocet = intPocet;
        }

        [XmlAttribute(AttributeName = "nazev")]
        public string Nazev
        {
            get { return mstrNazev; }
            set { mstrNazev = value; }
        }
        [XmlAttribute(AttributeName = "pocet")]
        public int Pocet
        {
            get { return mintPocet; }
            set { mintPocet = value; }
        }

        [XmlAttribute(AttributeName = "obsah")]
        public string Obsah
        {
            get { return mobjObsah.ToString(); }
            set { mobjObsah = value; }
        }

        #region IJev Members

        string IJev.Nazev
        {
            get { return mstrNazev; }
            set { mstrNazev = value; }
        }

        int IJev.Pocet
        {
            get { return mintPocet; }
            set { mintPocet = value; }
        }
        object IJev.Obsah
        {
            get { return mobjObsah; }
            set { mobjObsah = value; }
        }

        #endregion
    }


    [XmlRoot("jevy")]
    public class Jevy : XmlSerializace<Jevy>, ICollection<Jev>, IDictionary<string, int>
    {
        private ReversibleSortedList<string, int> mjvJevy = new ReversibleSortedList<string, int>();
        private Dictionary<string, object> mobjObsah = new Dictionary<string, object>();
        private string mstrZdroj;
        private DateTime mdtPosledniZmena;
        private string mstrAutor;

        [XmlElement(ElementName = "zdroj")]
        public string Zdroj
        {
            get { return (mstrZdroj); }
            set { mstrZdroj = value; }
        }

        [XmlElement(ElementName = "poseldniZmena")]
        public DateTime PosledniZmena
        {
            get { return (mdtPosledniZmena); }
            set { mdtPosledniZmena = value; }
        }

        [XmlElement(ElementName = "autor")]
        public string Autor
        {
            get { return (mstrAutor); }
            set { mstrAutor = value; }
        }

        public Jevy() { }
        public Jevy(string sZdroj)
        {
            Zdroj = sZdroj;
        }

        #region ICollection<Jev> Members

        public Jev Append(string strNazev)
        {

            Jev jv = new Jev(strNazev);
            if (mjvJevy.ContainsKey(strNazev))
            {
                mjvJevy[strNazev]++;
                jv.Pocet = mjvJevy[strNazev];
            }
            else
            {

                mjvJevy.Add(strNazev, jv.Pocet);
                mobjObsah.Add(strNazev, strNazev);
            }
            return jv;
        }

        public void Add(Jev item)
        {
            if (mjvJevy.ContainsKey(item.Nazev))
            {
                mjvJevy[item.Nazev] += item.Pocet;
            }
            else
            {
                mjvJevy.Add(item.Nazev, item.Pocet);
                mobjObsah.Add(item.Nazev, item.Obsah);
            }
        }

        public void Clear()
        {
            mjvJevy.Clear();
        }

        public bool Contains(Jev item)
        {
            return mjvJevy.ContainsKey(item.Nazev);
        }

        public void CopyTo(Jev[] array, int arrayIndex)
        {
            ArrayList al = new ArrayList(mjvJevy);
            //al.CopyTo(array, arrayIndex);
            int iIndex = -1;
            foreach (Jev jv in this)
            {
                iIndex++;
                array[iIndex] = jv;
            }
            //((IDictionary)mjvJevy).CopyTo(array, arrayIndex);
            //List<Jev> lJevy = new List<Jev>(mjvJevy.Count);

            ////throw new NotImplementedException();
            //List<Jev> lJevy = new List<Jev>(mjvJevy);
            //array = lJevy.ToArray();
        }


        public int Count
        {
            get { return mjvJevy.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Jev item)
        {
            mobjObsah.Remove(item.Nazev);
            return mjvJevy.Remove(item.Nazev);
        }

        #endregion

        #region IEnumerable<Jev> Members

        public IEnumerator<Jev> GetEnumerator()
        {
            foreach (string sNazev in mjvJevy.Keys)
            {
                yield return new Jev(sNazev, mobjObsah[sNazev], mjvJevy[sNazev]);
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mjvJevy.GetEnumerator();
        }

        #endregion

        public void Seradit(ListSortDirection lsdSmerRazeni)
        {
            mjvJevy.Comparer.SortDirection = lsdSmerRazeni;
            mjvJevy.Sort();
        }


        #region IDictionary<string,int> Members

        void IDictionary<string, int>.Add(string key, int value)
        {
            mjvJevy.Add(key, value);
        }

        bool IDictionary<string, int>.ContainsKey(string key)
        {
            return (mjvJevy.ContainsKey(key));
        }

        ICollection<string> IDictionary<string, int>.Keys
        {
            get { return mjvJevy.Keys; }
        }

        bool IDictionary<string, int>.Remove(string key)
        {
            mobjObsah.Remove(key);
            return (mjvJevy.Remove(key));
        }

        bool IDictionary<string, int>.TryGetValue(string key, out int value)
        {
            return (mjvJevy.TryGetValue(key, out value));
        }

        ICollection<int> IDictionary<string, int>.Values
        {
            get { return mjvJevy.Values; }
        }

        int IDictionary<string, int>.this[string key]
        {
            get
            {
                return mjvJevy[key];
            }
            set
            {
                mjvJevy[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,int>> Members

        void ICollection<KeyValuePair<string, int>>.Add(KeyValuePair<string, int> item)
        {
            mjvJevy.Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<string, int>>.Clear()
        {
            mjvJevy.Clear();
        }

        bool ICollection<KeyValuePair<string, int>>.Contains(KeyValuePair<string, int> item)
        {
            //dodělat
            return mjvJevy.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<string, int>>.CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
        {

            //mjvJevy.Keys.CopyTo(array, arrayIndex);
            throw new NotImplementedException();
        }

        int ICollection<KeyValuePair<string, int>>.Count
        {
            get { return mjvJevy.Count; }
        }

        bool ICollection<KeyValuePair<string, int>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, int>>.Remove(KeyValuePair<string, int> item)
        {
            return (mjvJevy.Remove(item.Key));
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,int>> Members

        IEnumerator<KeyValuePair<string, int>> IEnumerable<KeyValuePair<string, int>>.GetEnumerator()
        {
            return mjvJevy.GetEnumerator();
        }

        #endregion
    }


}