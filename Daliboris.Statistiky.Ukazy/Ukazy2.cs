using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Daliboris.Statistiky {
	public class Ukazy2 : ObservableCollection<Ukaz>, IBindingList {

		private class Razeni {

			private bool mblnPodporujeRazeni = true;
			private bool mblnJeSerazen = false;
			private ListSortDirection mlsdSmer = ListSortDirection.Ascending;
			private PropertyDescriptor mprdCoRadit = null;

			public Razeni() { }
			public Razeni(bool blnPodporujeRazeni) {
				mblnPodporujeRazeni = blnPodporujeRazeni;
			}
			public Razeni(PropertyDescriptor prdCoRadit, ListSortDirection lsdSmer) {
				this.CoRadit = prdCoRadit;
				this.Smer = lsdSmer;
			}


			public ListSortDirection Smer {
				get { return mlsdSmer; }
				set { mlsdSmer = value; }
			}
			public PropertyDescriptor CoRadit {
				get { return mprdCoRadit; }
				set { mprdCoRadit = value; }
			}
			public bool JeSerazen {
				get { return mblnJeSerazen; }
				set { mblnJeSerazen = value; }
			}
			public bool PodporujeRazeni {
				get { return mblnPodporujeRazeni; }
				set { mblnPodporujeRazeni = value; }
			}

		}


		private bool mblnAllowEdit = true;
		private bool mblnAllowNew = true;
		private bool mblnAllowRemove = true;

		public bool mblnSupportsChangeNotification = true;
		public bool mblnSupportsSorting = true;
		public bool mblnSupportsSearching = true;

		private Razeni mrzRazeni = new Razeni();

		//private BindingList<Ukaz> mbdlPlnySeznam = new BindingList<Ukaz>();

		private List<PropertyDescriptor> mglstPropertyDescriptors = new List<PropertyDescriptor>();
		private List<Ukaz> mglsNeserazene = new List<Ukaz>();
		private List<Ukaz> mglsSerazene;



		private ListChangedEventArgs resetEvent = new ListChangedEventArgs(ListChangedType.Reset, -1);
		private ListChangedEventHandler onListChanged;


		protected virtual void OnListChanged(ListChangedEventArgs ev) {
			if (onListChanged != null) {
				onListChanged(this, ev);
			}
		}

		event ListChangedEventHandler IBindingList.ListChanged {
			add { onListChanged += value; }
			remove { onListChanged -= value; }
		}

		public Ukazy2() : base() { }

		public Ukaz Append(string strNazev) {
			return Append(strNazev, null, null, null, 1);
		}

		public Ukaz Append(string strNazev, string strSpecifikace, string strRetrograd, object objObsah, int intPocet)
		{
			//Ukaz jv = new Ukaz(strNazev, strSpecifikace, strRetrograd, objObsah, intPocet);
			Ukaz jv = new Ukaz(strNazev, strSpecifikace, objObsah, intPocet);
			Add(jv);
			jv.Pocet = mglsNeserazene[mglsNeserazene.IndexOf(jv)].Pocet;
			return jv;
		}


		public new void Add(Ukaz item) {
			int iExistujici = mglsNeserazene.IndexOf(item);
			if (iExistujici == -1) {
				mglsNeserazene.Add(item);
				base.Add(item);
				PriZmeneSeznamu(new ListChangedEventArgs(ListChangedType.ItemAdded, mglsNeserazene.Count - 1));
			}
			else {
				//mglsNeserazene[iExistujici].Pocet += item.Pocet;
				base[iExistujici].Pocet += item.Pocet;
				PriZmeneSeznamu(new ListChangedEventArgs(ListChangedType.ItemChanged, iExistujici));
			}
		}

		private void PriZmeneSeznamu(ListChangedEventArgs ev) {
			if (onListChanged != null)
				onListChanged(this, ev);
		}

		#region IBindingList Members

		public void AddIndex(PropertyDescriptor property) {
			mglstPropertyDescriptors.Add(property);
		}

		public object AddNew() {
			//throw new NotImplementedException();
			if (!mblnAllowNew)
				throw new NotSupportedException();
			Ukaz uk = new Ukaz();
			mglsNeserazene.Add(uk);
			return uk;
		}

		public bool AllowEdit {
			get { return mblnAllowEdit; }
		}

		public bool AllowNew {
			get { return mblnAllowNew; }
		}

		public bool AllowRemove {
			get { return mblnAllowNew; }
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			mrzRazeni.CoRadit = property;
			mrzRazeni.Smer = direction;
			//mglsSerazene = new List<Ukaz>(mglsNeserazene);
			PouzijFiltr();
			//mglsSerazene.Sort(new UkazComparer<Ukaz>(mrzRazeni.CoRadit.Name));
			//if (mrzRazeni.Smer == ListSortDirection.Descending)
			//  mglsSerazene.Reverse();
			//throw new NotImplementedException();
		}

		public int Find(PropertyDescriptor property, object key) {
			//mglsNeserazene.Find(new Predicate<Ukaz> { });
			throw new NotImplementedException();
		}

		public bool IsSorted {
			get { return mrzRazeni.JeSerazen; }
		}

		public event ListChangedEventHandler ListChanged;

		public void RemoveIndex(PropertyDescriptor property) {
			mglstPropertyDescriptors.Remove(property);
		}

		public void RemoveSort() {
			mrzRazeni.JeSerazen = false;
			PriZmeneSeznamu(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public ListSortDirection SortDirection {
			get { return mrzRazeni.Smer; }
		}

		public PropertyDescriptor SortProperty {
			get { return mrzRazeni.CoRadit; }
		}

		public bool SupportsChangeNotification {
			get { return mblnSupportsChangeNotification; }
		}

		public bool SupportsSearching {
			get { return mblnSupportsSearching; }
		}

		public bool SupportsSorting {
			get { return mblnSupportsSorting; }
		}

		#endregion


		public void Filtruj(string strVlastnost, object objHodnota) {
			UkazFilter fuk = new UkazFilter(true, objHodnota.ToString());
			mglsSerazene.FindAll(fuk.ObsahujeText);
			PriZmeneSeznamu(new ListChangedEventArgs(ListChangedType.Reset, -1));
			base.Clear();

		}

		private void PouzijFiltr() {
			mglsSerazene = new List<Ukaz>(mglsNeserazene);

			UkazFilter fuk = new UkazFilter(true, mglstPropertyDescriptors[0].Name);
			mglsSerazene.FindAll(fuk.ObsahujeText);
			PriZmeneSeznamu(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
	}
}
