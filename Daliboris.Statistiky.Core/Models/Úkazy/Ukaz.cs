using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky 
{
	[XmlRoot(ElementName="u")]
	public class Ukaz : IJev, INotifyPropertyChanged, IComparable{

		#region Interní proměnné
		private string mstrNazev;
		private int mintPocet = 1;
		private object mobjObsah;
		//private string mstrRetrograd;
		private string mstrSpecifikace;
		private Type mtpTypHodnoty;
		private List<string> _kontexty;

		#endregion

		#region Konstruktory
		public Ukaz() { _kontexty = new List<string>(); }
		public Ukaz(string strNazev) : this() {
			this.Nazev = strNazev;
		}

		public Ukaz(string strNazev, int intPocet)
			: this(strNazev) {
			this.Pocet = intPocet;
		}

		public Ukaz(string strNazev, string strSpecifikace)
			: this(strNazev) {
			this.Specifikace = strSpecifikace;
		}
		public Ukaz(string strNazev, string strSpecifikace, int intPocet)
			: this(strNazev, strSpecifikace) {
			this.Pocet = intPocet;
		}

		public Ukaz(string strNazev, object objObsah)
			: this(strNazev) {
			this.Obsah = objObsah;
		}

		public Ukaz(string strNazev, string strSpecifikace, object objObsah)
			: this(strNazev, strSpecifikace) {
			this.Obsah = objObsah;
		}

		public Ukaz(string strNazev, string strSpecifikace, object objObsah, int intPocet)
			: this(strNazev, strSpecifikace, objObsah) {
			this.Pocet = intPocet;
		}

		public Ukaz(object objObsah) : this()
		{
			this.Obsah = objObsah;
		}

		public Ukaz(object objObsah, int intPocet)
			: this(objObsah)
		{
			this.Pocet = intPocet;
		}
		public Ukaz(object objObsah, string strSpecifikace)
			: this(objObsah)
		{
			this.Specifikace = strSpecifikace;
		}
		public Ukaz(object objObsah, string strSpecifikace, int intPocet)
			: this(objObsah, intPocet) {
				this.Specifikace = strSpecifikace;
		}
		#endregion

		#region Veřejně přístupné vlastnosti

		protected virtual void Changed(string propertyName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		/// <summary>
		/// Specifikace úkazu, např. jazyk textu. Spolu s názvem tvoří jedinečný identifikátor úkazu.
		/// </summary>
		[XmlAttribute(AttributeName="s")]
		public string Specifikace {
			get { return mstrSpecifikace; }
			set {
				if (mstrSpecifikace != value) {
					mstrSpecifikace = value;
					Changed("Specifikace");
				}
			}
		}
		/// <summary>
		/// Název úkazu, jeho viditelný identifikátor. Spolu se specifikací tvoří jedinečný identifikátor úkazu.
		/// </summary>
		[XmlAttribute(AttributeName = "n")]
		public string Nazev {
			get { return mstrNazev; }
			set {
				if (mstrNazev != value) {
					mstrNazev = value;
					Changed("Nazev");
				}
			}
		}
		/*
		/// <summary>
		/// Retrográdní podoba názvu. (Patří do této obecné třídy? Například znak nebo číslo nebude mít retrográdní podobu.
		/// </summary>
		[XmlAttribute(AttributeName = "r")]
		public string Retrograd {
			get { return mstrRetrograd; }
			set {
				if (mstrRetrograd != value) {
					mstrRetrograd = value;
					Changed("Retrograd");
				}
			}
		}
		*/
		/// <summary>
		/// Počet úkazů.
		/// </summary>
		[XmlAttribute(AttributeName = "p")]
		public int Pocet {
			get { return mintPocet; }
			set {
				if (mintPocet != value) {
					mintPocet = value;
					Changed("Pocet");
				}
			}
		}

		[XmlArray(ElementName = "cont")]
		[XmlArrayItem(ElementName = "c")]
		public List<string> Kontexty
		{
			get { return _kontexty; }
			set { _kontexty = value; }
		}

		/*
		/// <summary>
		/// Typ evidované hodnoty. Může jít např. o text, číslo, datum nebo objekt určité třídy.
		/// </summary>
		[XmlIgnore]
		public Type TypHodnoty {
			get { return mtpTypHodnoty; }
			set {
				if (mtpTypHodnoty != value) {
					mtpTypHodnoty = value;
					Changed("TypHodnoty");
				}
			}
		}
		*/

		/// <summary>
		/// Reprezentace evidovaného úkazu v jeho objektové podobě.
		/// </summary>
		[XmlElement(ElementName = "o")]
		public object Obsah {
			get { return mobjObsah; }
			set {
				if (mobjObsah != value) {
					if (value is String) {
						this.Nazev = mobjObsah.ToString();
					}
					else
					{
						if (mstrNazev == null)
							this.Nazev = value.ToString();
						mobjObsah = value;
						Changed("Obsah");
					}
				}
			}
		}

		#endregion

		#region IJev Members

		string IJev.Nazev {
			get { return mstrNazev; }
			set { this.Nazev = value; }
		}
		int IJev.Pocet {
			get { return mintPocet; }
			set { this.Pocet = value; }
		}
		object IJev.Obsah {
			get { return mobjObsah; }
			set { this.Obsah = value; }
		}

		#endregion

		internal string Identifikator {
			get { return mstrSpecifikace ?? "" + mstrNazev ?? ""; }
		}

		#region Operátory
		/// <summary>
		/// Ekvivalence dvou objektů nastává tehdy, když je shodný název a specifikace.
		/// Počet výskytů, retrográd ani obsah se neporovnávají
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			if (obj is Ukaz) {
				return ((((Ukaz)obj).Nazev == this.Nazev) && (((Ukaz)obj).Specifikace == this.Specifikace));
			}
			else
				return false;
		}
		public override int GetHashCode() {
			//return base.GetHashCode();
			int iHashCode = 0;
			if (this.Nazev != null)
				iHashCode ^= this.Nazev.GetHashCode();
			if (this.Specifikace != null)
				iHashCode ^= this.Specifikace.GetHashCode();
			return iHashCode;
		}
		public static bool operator ==(Ukaz x, Ukaz y) {
			if ((object)x == null)
				return ((object)y == null);
			return x.Equals(y);
		}
		public static bool operator !=(Ukaz x, Ukaz y) {
			return !(x == y);
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (null != PropertyChanged) {
				PropertyChanged(this, e);
			}
		}

		#endregion

		#region IComparable Members

		//TODO: co když budu chtít porovnávat podle počtu, nikoli podle názvu?
		public int CompareTo(object obj) {
			if (obj is Ukaz) {
				Ukaz uk = (Ukaz)obj;
				return this.Identifikator.CompareTo(uk.Identifikator);
			}
			else {
				throw new ArgumentException("Porovnávaný objekt není typu Ukaz");
			}
		}

		#endregion

	}

}

