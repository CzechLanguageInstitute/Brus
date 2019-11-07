using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Daliboris.Statistiky.Rozhrani.Jevy {
	public interface IJevyZdroje : IJevy {
		//int Add(object value);
		//void Add(IJev item);
		//void Add(System.Collections.Generic.KeyValuePair<string, IJev> item);
		//void Add(string key, IJev value);
		IJev Append(string strNazev);
		IJev Append(string strJazyk, string strNazev, object objObsah, string strRetrograd, int intPocet);
		//void Clear();
		//bool Contains(System.Collections.Generic.KeyValuePair<string, IJev> item);
		//bool Contains(IJev item);
		//bool Contains(object value);
		//void CopyTo(System.Collections.Generic.KeyValuePair<string, IJev>[] array, int arrayIndex);
		//void CopyTo(Array array, int index);
		//void CopyTo(IJev[] array, int arrayIndex);
		//int Count { get; }
		//System.Collections.Generic.IEnumerator<IJev> GetEnumerator();
		string ID { get; }
		string Identifikator { get; set; }
		//int IndexOf(object value);
		//void Insert(int index, object value);
		//bool IsFixedSize { get; }
		//bool IsReadOnly { get; }
		//bool IsSynchronized { get; }
		string Jazyk { get; set; }
		int Pocet { get; }
		string Popis { get; set; }
		DateTime PosledniZmena { get; set; }
		//bool Remove(IJev item);
		//void Remove(object value);
		//bool Remove(System.Collections.Generic.KeyValuePair<string, IJev> item);
		//void RemoveAt(int index);
		//IEnumerable<IJev> Seradit(SortDescription srdPopisRazeni);
		//object SyncRoot { get; }
		//IJev this[string identifikator] { get; }
		//object this[int index] { get; set; }
		//bool TryGetValue(string key, out IJev value);
		TypJevu Typ { get; set; }
		IZdroj Zdroj { get; set; }
		System.Globalization.CompareOptions ZpusobRazeni { get; }
	}
}
