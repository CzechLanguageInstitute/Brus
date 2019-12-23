using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using Daliboris.Statistiky.Rozhrani.Jevy;


//using System.Linq;

namespace Daliboris.Statistiky
{

	[AttributeUsage(AttributeTargets.All)]
	public class TypedCollectionAttribute : Attribute
	{
		private Type UnderlyingType;

		public TypedCollectionAttribute(Type underlyingType)
		{
			UnderlyingType = underlyingType;
		}

		public Type CollectionType
		{
			get { return UnderlyingType; }
		}
	}



	public class Detail
	{
		private int mintPocet;
		private string mstrPopis;
		private string mstrIdentifikator;

		public string Identifikator
		{
			get { return mstrIdentifikator; }
			set { mstrIdentifikator = value; }
		}

		public string Popis
		{
			get { return mstrPopis; }
			set { mstrPopis = value; }
		}

		public int Pocet
		{
			get { return mintPocet; }
			set { mintPocet = value; }
		}

	}
	/*
	 public class Detaily : ICollection<Detail> { 
	 }
	*/


	public class Jev<T> : IJev<T>
	{
		private string mstrNazev;
		private int mintPocet = 1;
		private T mgenObsah;

		public Jev() { }
		public Jev(string strNazev) { mstrNazev = strNazev; }


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

		public string Nazev
		{
			get { return mstrNazev; }
			set { mstrNazev = value; }
		}
		public int Pocet
		{
			get { return mintPocet; }
			set { mintPocet = value; }
		}

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

	/*
	public class ObJevy : ObservableCollection<Jev>
	{
		public ObJevy() : base() { }
		public ObJevy(IEnumerable<Jev> collection) : base(collection) { }
		public ObJevy(IList<Jev> list) : base(list) { }

	}
	*/
}