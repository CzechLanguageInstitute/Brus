using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky {
	/// <summary>
	/// Jednotlivý jev
	/// </summary>
	/// 

	[DebuggerDisplay("{Jazyk}, {Nazev}, {Pocet}")]
	public class Jev : IJev, ICloneable {

		#region Interní proměnné
		private string mstrNazev;
		private int mintPocet = 1;
		private object mobjObsah;
		private string mstrRetrograd;
		private string mstrJazyk;
		private List<string> _kontexty = new List<string>();

		#endregion

		#region Konstruktory
		public Jev() { }
		public Jev(string strNazev) {
			mstrNazev = strNazev;
		}

		public Jev(string strJazyk, string strNazev) {
			mstrNazev = strNazev;
			mstrJazyk = strJazyk;
		}

		public Jev(string strJazyk, string strNazev, string strRetrograd)
			: this(strJazyk, strNazev) {
			mstrRetrograd = strRetrograd;
		}
		public Jev(string strJazyk, string strNazev, string strRetrograd, int intPocet)
			: this(strJazyk, strNazev, strRetrograd) {
			mintPocet = intPocet;
		}
		public Jev(string strJazyk, string strNazev, int intPocet)
			: this(strJazyk, strNazev) {
			mintPocet = intPocet;
		}

		public Jev(string strJazyk, string strNazev, object objObsah)
			: this(strJazyk, strNazev) {
			mobjObsah = objObsah;
		}

		public Jev(string strJazyk, string strNazev, object objObsah, int intPocet)
			: this(strJazyk, strNazev, objObsah) {
			mintPocet = intPocet;
		}

		public Jev(string strJazyk, string strNazev, object objObsah, string strRetrograd)
			: this(strJazyk, strNazev, objObsah) {
			mstrRetrograd = strRetrograd;
		}

		public Jev(string strJazyk, string strNazev, object objObsah, string strRetrograd, int intPocet)
			: this(strJazyk, strNazev, objObsah, strRetrograd) {
			mintPocet = intPocet;
		}
		#endregion

		#region Veřejně přístupné vlastnosti
		public string Jazyk {
			get { return mstrJazyk; }
			set { mstrJazyk = value; }
		}

		public string Nazev {
			get { return mstrNazev; }
			set { mstrNazev = value; }
		}
		public string Retrograd {
			get { return mstrRetrograd; }
			set { mstrRetrograd = value; }
		}
		public int Pocet {
			get { return mintPocet; }
			set { mintPocet = value; }
		}

		public List<string> Kontexty
		{
			get { return _kontexty; }
			set { _kontexty = value; }
		}

		public object Obsah {
			get { return mobjObsah; }
			set { mobjObsah = value; }
		}

		#endregion

		#region IJev Members

		string IJev.Nazev {
			get { return mstrNazev; }
			set { mstrNazev = value; }
		}

		int IJev.Pocet {
			get { return mintPocet; }
			set { mintPocet = value; }
		}
		object IJev.Obsah {
			get { return mobjObsah; }
			set { mobjObsah = value; }
		}

		#endregion

		public override bool Equals(object obj) {
			Jev jv = obj as Jev;
			if (jv == null)
				return false;
			return String.Equals(mstrNazev, jv.Nazev);
		}

		public override int GetHashCode() {
			//return base.GetHashCode();
			int iHashCode = 0;
			if (mstrNazev != null)
				iHashCode = mstrNazev.GetHashCode();
			return iHashCode;
		}
		public static bool operator ==(Jev x, Jev y) {
			if ((object)x == null)
				return ((object)y == null);
			return x.Equals(y);
		}
		public static bool operator !=(Jev x, Jev y) {
			return !(x == y);
		}


		#region ICloneable Members

		object ICloneable.Clone() {
			return this.Clone();
		}

		#endregion

		public virtual Jev Clone() {
			Jev jv = this.MemberwiseClone() as Jev;
			if (mobjObsah != null) {
				using (Stream objectStream = new MemoryStream()) {
					IFormatter formatter = new BinaryFormatter();
					formatter.Serialize(objectStream, mobjObsah);
					objectStream.Seek(0, SeekOrigin.Begin);
					jv.Obsah = (object)formatter.Deserialize(objectStream);
				}
			}
			return jv;

		}

	}
}
