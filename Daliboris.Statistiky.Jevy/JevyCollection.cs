using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky {
	//možné operátory k přetížení: *, +, -, /, &, |, %
	[TypedCollection(typeof(Jev))]
	[DebuggerDisplay("{ID}, {Identifikator}, {Pocet}")]
	public class Jevy : ICollection<Jev>, IDictionary<string, Jev>, IJevyZdroje {
		//private ReversibleSortedList<string, int> mjvJevy = new ReversibleSortedList<string, int>();
		//private Dictionary<string, object> mobjObsah = new Dictionary<string, object>();

		private readonly RazeniJevu mrjRazeniJevu = new RazeniJevu();

		private readonly SortedDictionary<string, Jev> mgdJevy = new SortedDictionary<string, Jev>(new RazeniJevu());
		private Zdroj mzdrZdroj = new Zdroj();
		private DateTime mdtPosledniZmena;
		//private int mintPocet;
		private string mstrPopis;
		private string mstrIdentifikator;
		private string mstrJazyk;
		private TypJevu mtpTyp = TypJevu.Ostatni;
		private object mobjSyncRoot = new object();
		private Type finalType = typeof(Jev);


		#region Veřejně přístupné vlastnosti




		public CompareOptions ZpusobRazeni {
			get { return mrjRazeniJevu.ZpusobRazeni; }
		}

		public string ID {
			get { return GetID(mstrJazyk, mstrIdentifikator); }
		}


		public TypJevu Typ {
			get { return mtpTyp; }
			set { mtpTyp = value; }
		}

		public string Jazyk {
			get { return mstrJazyk; }
			set { mstrJazyk = value; }
		}

		public string Identifikator {
			get { return mstrIdentifikator; }
			set { mstrIdentifikator = value; }
		}

		public string Popis {
			get { return mstrPopis; }
			set { mstrPopis = value; }
		}

		public int Pocet { get { return mgdJevy.Count; } }

		public  int  SoucetJevu()
		{
			int i = 0;
			foreach (Jev jv in this)
			{
				i += jv.Pocet;
			}
			return i;
		}

		public Zdroj Zdroj {
			get { return (mzdrZdroj); }
			set { mzdrZdroj = value; }
		}

		public DateTime PosledniZmena {
			get { return (mdtPosledniZmena); }
			set { mdtPosledniZmena = value; }
		}
		#endregion

		#region Konstruktory
		public Jevy() { }

		public Jevy(RazeniJevu rjRazeni) {
			mgdJevy = new SortedDictionary<string, Jev>(rjRazeni);
		}

		public Jevy(TypJevu tpTyp) {
			mtpTyp = tpTyp;
		}
		public Jevy(TypJevu tpTyp, Zdroj zdZdroj) {
			mtpTyp = tpTyp;
			mzdrZdroj = zdZdroj;
		}
		public Jevy(Zdroj zdZdroj) {
			mzdrZdroj = zdZdroj;
		}
		public Jevy(TypJevu tpTyp, string strCestaKeZdroji)
			: this(strCestaKeZdroji) {
			mtpTyp = tpTyp;
		}
		public Jevy(string strCestaKeZdroji) {
			mzdrZdroj = new Zdroj(strCestaKeZdroji);
		}
		public Jevy(string strCestaKeZdroji, string strJazyk, string strIdentifikator)
			: this(strCestaKeZdroji) {
			mstrJazyk = strJazyk;
			mstrIdentifikator = strIdentifikator;

		}

		public Jevy(TypJevu tpTyp, string strCestaKeZdroji, string strJazyk, string strIdentifikator)
			: this(strCestaKeZdroji, strJazyk, strIdentifikator) {
			mtpTyp = tpTyp;
		}

		public Jevy(TypJevu tpTyp, string strCestaKeZdroji, string strJazyk, string strIdentifikator, string popis)
			: this(tpTyp, strCestaKeZdroji, strJazyk, strIdentifikator)
		{
			Popis = popis;
		}

		#endregion


		public static string GetID(string strJazyk, string strIdentifikator) {
			if (strJazyk == null)
				return strIdentifikator;
			return strIdentifikator + "$" + strJazyk;
		}

		public Jev this[string identifikator] {
			get { return mgdJevy.ContainsKey(identifikator) ? mgdJevy[identifikator] : null; }
		}


		#region ICollection<Jev> Members

		public Jev Append(string strNazev) {
			return Append(mstrJazyk, strNazev, null, null, 1);
		}


		public Jev Append(string strJazyk, string strNazev, object objObsah, string strRetrograd, int intPocet) {
			Jev jv = new Jev(strJazyk, strNazev, objObsah, strRetrograd, intPocet);
			Add(jv);
			jv.Pocet = mgdJevy[strNazev].Pocet;

			/*
			if (mgdJevy.ContainsKey(strNazev)) {
				mgdJevy[strNazev].Pocet++;
				jv.Pocet = mgdJevy[strNazev].Pocet;
			}
			else {
				mgdJevy.Add(strNazev, jv);
			}
			*/
			return jv;

		}
		public void Add(Jev item) {
			if (mgdJevy.ContainsKey(item.Nazev)) {
				mgdJevy[item.Nazev].Pocet += item.Pocet;
				mgdJevy[item.Nazev].Kontexty.AddRange(item.Kontexty);
			}
			else {
				mgdJevy.Add(item.Nazev, item);
			}
		}

		public void Clear() {
			mgdJevy.Clear();
		}

		public bool Contains(Jev item) {
			return mgdJevy.ContainsKey(item.Nazev);
		}

		public void CopyTo(Jev[] array, int arrayIndex) {
			mgdJevy.Values.CopyTo(array, arrayIndex);
		}


		public int Count {
			get { return mgdJevy.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Jev item) {
			return mgdJevy.Remove(item.Nazev);
		}

		#endregion

		#region IEnumerable<Jev> Members

		public IEnumerator<Jev> GetEnumerator() {
			return mgdJevy.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion

		public IEnumerable<Jev> Seradit(SortDescription srdPopisRazeni) {
			List<Jev> lsJevy = new List<Jev>(this);
			if (srdPopisRazeni.PropertyName == "Nazev") {
				if (srdPopisRazeni.Direction == ListSortDirection.Ascending)
					lsJevy.Sort(delegate(Jev x, Jev y) { return x.Nazev.CompareTo(y.Nazev); });
				else
					lsJevy.Sort(delegate(Jev x, Jev y) { return -x.Nazev.CompareTo(y.Nazev); });
			}
			else if (srdPopisRazeni.PropertyName == "Pocet") {
				if (srdPopisRazeni.Direction == ListSortDirection.Ascending)
					lsJevy.Sort(delegate(Jev x, Jev y) { return x.Pocet.CompareTo(y.Pocet); });
				else
					lsJevy.Sort(delegate(Jev x, Jev y) { return -x.Pocet.CompareTo(y.Pocet); });
			}
			return lsJevy;
		}

/*
		IEnumerable<IJev> Seradit(SortDescription srdPopisRazeni) {
			return (IEnumerable<IJev>)this.Seradit(srdPopisRazeni);
		}
*/

		#region IDictionary<string,Jev> Members

		public void Add(string key, Jev value) {
			mgdJevy.Add(key, value);
		}

		bool IDictionary<string, Jev>.ContainsKey(string key) {
			return mgdJevy.ContainsKey(key);
		}

		ICollection<string> IDictionary<string, Jev>.Keys {
			get { return mgdJevy.Keys; }
		}

		bool IDictionary<string, Jev>.Remove(string key) {
			return mgdJevy.Remove(key);
		}

		public bool TryGetValue(string key, out Jev value) {
			return mgdJevy.TryGetValue(key, out value);
		}

		ICollection<Jev> IDictionary<string, Jev>.Values {
			get { return mgdJevy.Values; }
		}

		Jev IDictionary<string, Jev>.this[string key] {
			get {
				return mgdJevy[key];
			}
			set {
				mgdJevy[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<string,Jev>> Members

		public void Add(KeyValuePair<string, Jev> item) {
			mgdJevy.Add(item.Key, item.Value);
		}

		public bool Contains(KeyValuePair<string, Jev> item) {
			return mgdJevy.ContainsKey(item.Key);
		}

		public void CopyTo(KeyValuePair<string, Jev>[] array, int arrayIndex) {
			//KeyValuePair<string, Jev> kvp = new KeyValuePair<string, Jev>();
			//for (int i = 0; i < mgdJevy.Count; i++) {

			//}

			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<string, Jev> item) {
			return mgdJevy.Remove(item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,Jev>> Members

		IEnumerator<KeyValuePair<string, Jev>> IEnumerable<KeyValuePair<string, Jev>>.GetEnumerator() {
			throw new NotImplementedException();
		}

		#endregion

		#region Vlastní operátory
		public static Jevy operator +(Jevy jvJevy1, Jevy jvJevy2) {
			Jevy jvs = jvJevy1;
			foreach (Jev jv in jvJevy2) {
				jvs.Add(jv);
			}
			return jvs;
		}

		public static Jevy operator -(Jevy jvJevy1, Jevy jvJevy2) {
			Jevy jvs = jvJevy1;
			foreach (Jev jv in jvJevy2) {
				if (jvs.Contains(jv)) {
					if (jvs[jv.Nazev].Pocet == jv.Pocet) {
						jvs.Remove(jv);
					}
					else {
						jvs[jv.Nazev].Pocet -= jv.Pocet;
					}
				}
			}
			return jvs;
		}
		/// <summary>
		///Vrátí seznam jevů, které se nevyskytují v druhé kolekci
		/// </summary>
		/// <param name="jvJevy1">první kolekce jevů</param>
		/// <param name="jvJevy2">druhá kolekce jevů</param>
		/// <returns></returns>
		public static Jevy operator /(Jevy jvJevy1, Jevy jvJevy2) {
			Jevy jvs = new Jevy(jvJevy1.Typ, jvJevy1.Zdroj);
			foreach (Jev jv in jvJevy2) {
				if (!jvJevy1.Contains(jv)) {
					jv.Pocet = 1;
					jvs.Append(jv.Jazyk, jv.Nazev, jv.Obsah, jv.Retrograd, 1);
				}
			}
			return jvs;
		}

		#endregion

		#region IBindingList Members

		private ListChangedEventArgs resetEvent = new ListChangedEventArgs(ListChangedType.Reset, -1);
		private ListChangedEventHandler onListChanged;

		protected virtual void OnListChanged(ListChangedEventArgs ev) {
			if (onListChanged != null) {
				onListChanged(this, ev);
			}
		}

		event ListChangedEventHandler IBindingList.ListChanged {
			add {
				onListChanged += value;
			}
			remove {
				onListChanged -= value;
			}
		}

		bool IBindingList.AllowEdit {
			get { return true; }
		}

		bool IBindingList.AllowNew {
			get { return true; }
		}

		bool IBindingList.AllowRemove {
			get { return true; }
		}

		bool IBindingList.SupportsChangeNotification {
			get { return false; }
		}


		bool IBindingList.SupportsSearching {
			get { return true; }
		}

		bool IBindingList.SupportsSorting {
			get { return true; }
		}

		object IBindingList.AddNew() {
			return finalType.GetConstructor(new Type[] { }).Invoke(null);
		}

		private bool isSorted = false;

		bool IBindingList.IsSorted {
			get { return isSorted; }
		}

		private ListSortDirection listSortDirection = ListSortDirection.Ascending;

		ListSortDirection IBindingList.SortDirection {
			get { return listSortDirection; }
		}

		PropertyDescriptor sortProperty = null;

		PropertyDescriptor IBindingList.SortProperty {
			get { return sortProperty; }
		}

		void IBindingList.AddIndex(PropertyDescriptor property) {
			isSorted = true;
			sortProperty = property;
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			isSorted = true;
			sortProperty = property;
			listSortDirection = direction;

			ArrayList a = new ArrayList();

			//////this.Sort(new ObjectPropertyComparer(property.Name));
			//////if (direction == ListSortDirection.Descending) this.Reverse();
		}

		int IBindingList.Find(PropertyDescriptor property, object key) {
			foreach (object o in this) {
				if (Match(finalType.GetProperty(property.Name).GetValue(o, null), key))
					return this.IndexOf(o);
			}
			return -1;
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			sortProperty = null;
		}

		void IBindingList.RemoveSort() {
			isSorted = false;
			sortProperty = null;
		}

		#endregion

		#region IList Members

		public int Add(object value) {
			//return this.Add(value as Jev);
			Jev jv = value as Jev;
			int iPocet = mgdJevy.Count;
			Add(jv);
			return IndexOf(jv);
		}

		public bool Contains(object value) {
			return this.Contains(value as Jev);
		}

		public int IndexOf(object value) {
			int i = -1;
			Jev jv = value as Jev;
			if (jv == null)
				return i;
			foreach (Jev j in this) {
				i++;
				if (j == jv)
					return i;
			}
			return -1;

		}

		public void Insert(int index, object value) {
			throw new NotImplementedException();
		}

		public bool IsFixedSize {
			get { return false; }
		}

		public void Remove(object value) {
			Jev jv = value as Jev;
			if (jv == null)
				return;
			mgdJevy.Remove(jv.Nazev);
		}

		public void RemoveAt(int index) {
			int i = -1;
			foreach (Jev j in this) {
				i++;
				if (i == index) {
					Remove(j);
					return;
				}

			}
		}

		public object this[int index] {
			get {
				int i = 0;
				if (mgdJevy.Count > index) {
					foreach (Jev jv in this) {
						if (i == index)
							return jv;
						i++;
					}
					return null;
				}
				else
					return null;

			}
			set {
				int i = 0;

				foreach (Jev jv in this) {
					if (i == index) {
						//jv = value;
						return;
					}
					i++;
				}
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index) {
			this.CopyTo((Jev[])array, index);
		}

		public bool IsSynchronized {
			get { return true; }
		}

		public object SyncRoot {
			get { return mobjSyncRoot; }
		}

		#endregion

		#region ITypedList
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			ArrayList input = null;
			ArrayList output = new ArrayList();

			if (listAccessors != null && listAccessors.Length > 0) {
				// if an listAccessors is suppled, we return the 
				// properties for the LAST one - dont ask me why - 
				// I found it in the sourse code for 
				// DataView.ITypedList.GetItemProperties using a 
				// decompiler

				PropertyDescriptor childProperty = listAccessors[listAccessors.Length - 1];

				Type t = null;

				foreach (Attribute a in childProperty.Attributes) {
					if (a is TypedCollectionAttribute) {
						t = ((TypedCollectionAttribute)a).CollectionType;
						break;
					}
				}

				if (t != null)
					input = new ArrayList(TypeDescriptor.GetProperties(t));
			}
			else {
				input = new ArrayList(TypeDescriptor.GetProperties(finalType));
			}

			return GetPropertyDescriptorCollection(input);
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			string name = "";

			if (listAccessors != null) {
				foreach (PropertyDescriptor p in listAccessors) {
					name += p.PropertyType.Name + "_";
				}
				name = name.TrimEnd('_');
			}
			else
				name = this.GetType().Name;

			return name;
		}
		#endregion

		#region Helper functions

		protected PropertyDescriptorCollection GetPropertyDescriptorCollection(ArrayList properties) {
			if (properties == null || properties.Count == 0)
				return new PropertyDescriptorCollection(null);

			ArrayList output = new ArrayList();

			foreach (PropertyDescriptor p in properties) {
				if (p.Attributes.Matches(new Attribute[] { new BindableAttribute(false) }))
					continue;

				if (p.PropertyType.Namespace == "System.Data.SqlTypes") {
					// create the base type property descriptor
					output.Add(SqlPropertyDescriptor.GetProperty(p.Name, p.PropertyType));
				}
				else {
					output.Add(p);
				}
			}
			return new PropertyDescriptorCollection((PropertyDescriptor[])output.ToArray(typeof(PropertyDescriptor)));
		}

		protected bool Match(object data, object searchValue) {
			// handle nulls
			if (data == null || searchValue == null) {
				return (bool)(data == searchValue);
			}

			// if its a string, our comparisons should be 
			// case insensitive.
			bool IsString = (bool)(data is string);


			// bit of validation b4 we start...
			if (data.GetType() != searchValue.GetType())
				throw new ArgumentException("Objects must be of the same type");

			if (!(data.GetType().IsValueType || data is string))
				throw new ArgumentException("Objects must be a value type");



			/*
			 * Less than zero a is less than b. 
			 * Zero a equals b. 
			 * Greater than zero a is greater than b. 
			 */

			if (IsString) {
				string stringData = ((string)data).ToLower(CultureInfo.CurrentCulture);
				string stringMatch = ((string)searchValue).ToLower(CultureInfo.CurrentCulture);

				return (bool)(stringData == stringMatch);
			}
			else {
				return (bool)(Comparer.Default.Compare(data, searchValue) == 0);
			}
		}

		#endregion

		IZdroj IJevyZdroje.Zdroj {
			get { return (IZdroj)Zdroj; }
			set {
				if (value == null)
					Zdroj = null;
				else
					Zdroj = new Zdroj(value.CelaCesta);
			}
		}

		#region IJevyZdroje Members

		IJev IJevyZdroje.Append(string strNazev) {
			return (IJev) this.Append(strNazev);
		}

		IJev IJevyZdroje.Append(string strJazyk, string strNazev, object objObsah, string strRetrograd, int intPocet) {
			return (IJev) this.Append(strJazyk, strNazev, objObsah, strRetrograd, intPocet);
		}


		//IEnumerable<IJev> IJevyZdroje.Seradit(SortDescription srdPopisRazeni) {
		//  return (IEnumerable<IJev>)this.Seradit(srdPopisRazeni);
		//}

		#endregion

		#region ICollection<IJev> Members

		public void Add(IJev item) {
			this.Add(new Jev(null, item.Nazev, item.Obsah, item.Pocet) );
		}

		public bool Contains(IJev item) {
			return this.Contains(new Jev(null, item.Nazev, item.Obsah, item.Pocet));
		}

		public void CopyTo(IJev[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public bool Remove(IJev item) {
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<IJev> Members

		IEnumerator<IJev> IEnumerable<IJev>.GetEnumerator() {
			return (IEnumerator<IJev>)this.GetEnumerator();
		}

		#endregion

		#region IDictionary<string,IJev> Members

		public void Add(string key, IJev value) {
			this.Add(key, value);
		}

		bool IDictionary<string, IJev>.ContainsKey(string key) {
			return this.Contains(key);
		}

		ICollection<string> IDictionary<string, IJev>.Keys {
			get { throw new NotImplementedException(); }
		}

		bool IDictionary<string, IJev>.Remove(string key) {
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out IJev value) {
			throw new NotImplementedException();
		}

		ICollection<IJev> IDictionary<string, IJev>.Values {
			get { throw new NotImplementedException(); }
		}

		IJev IDictionary<string, IJev>.this[string key] {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion

		#region ICollection<KeyValuePair<string,IJev>> Members

		public void Add(KeyValuePair<string, IJev> item) {
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<string, IJev> item) {
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<string, IJev>[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<string, IJev> item) {
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,IJev>> Members

		IEnumerator<KeyValuePair<string, IJev>> IEnumerable<KeyValuePair<string, IJev>>.GetEnumerator() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
