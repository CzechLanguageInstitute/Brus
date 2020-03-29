using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Daliboris.Statistiky
{
    [XmlRoot(ElementName = "uk")]
    public class Ukazy : Collection<Ukaz>, IEqualityComparer<Ukaz>, IComparer<Ukaz>
    {
        //private List<Ukaz> mlsNeserazene;
        private string mstrNazev;

        /// <summary>
        /// Název pro kolekci položek
        /// </summary>
        /// 
        [XmlAttribute(AttributeName = "n")]
        public string Nazev
        {
            get { return mstrNazev; }
            set { mstrNazev = value; }
        }

        public double Avg
        {
            get
            {
                if (this.Items.Count == 0)
                    return 0;
                return Sum / Items.Count;
                //return base.Items.Average<Ukaz>(p => p.Pocet); 
            }
        }

        public int Max
        {
            get
            {
                if (this.Items.Count == 0)
                    return 0;
                int iMax = 0;
                foreach (Ukaz uk in Items)
                {
                    if (iMax > uk.Pocet)
                        iMax = uk.Pocet;
                }

                return iMax;
                //return base.Items.Max<Ukaz>(p => p.Pocet);
            }
        }

        public int Min
        {
            get
            {
                if (this.Items.Count == 0)
                    return 0;
                int iMin = 0;
                foreach (Ukaz uk in Items)
                {
                    if (iMin < uk.Pocet)
                        iMin = uk.Pocet;
                }

                return iMin;
                //return base.Items.Min<Ukaz>(p => p.Pocet); }
            }
        }

        public int Sum
        {
            get
            {
                if (this.Items.Count == 0)
                    return 0;
                int iSum = 0;
                foreach (Ukaz uk in Items)
                {
                    iSum += uk.Pocet;
                }

                return iSum;

                //return base.Items.Sum<Ukaz>(p => p.Pocet); 
            }
        }
        //public int Count {
        //   get { return base.Items.Count; }
        //}


        #region Konstruktory

        public Ukazy()
        {
        }

        public Ukazy(string strNazev)
        {
            mstrNazev = strNazev;
        }

        public Ukazy(IList<Ukaz> list)
            : base(list)
        {
        }

        public Ukazy(string strNazev, IList<Ukaz> list)
            : base(list)
        {
            mstrNazev = strNazev;
        }

        #endregion

        /// <summary>
        /// Pøidá novou položku do kolekce; pokud položka existuje, zvýší u níc poèet.
        /// </summary>
        /// <param name="item">Položka oznaèující nìjaký jev.</param>
        public virtual new void Add(Ukaz item)
        {
            if (base.Contains(item))
            {
                Ukaz uk = base[base.IndexOf(item)];
                uk.Pocet += item.Pocet;
            }
            else
            {
                base.Add(item);
            }
        }

        //public virtual bool Contains(Ukaz item)
        //{
        //   return base.Contains(item);
        //}


        #region IEqualityComparer<Ukaz> Members

        /// <summary>
        /// Porovná dva objekty typu Ukaz, zda jsou si rovny
        /// </summary>
        /// <param name="x">Objekt porovnání na levé stranì</param>
        /// <param name="y">Objekt porovnání na pravé stranì</param>
        /// <returns>True, pokud jsou si objekty rovny, false, pokud se objekty liší</returns>
        public bool Equals(Ukaz x, Ukaz y)
        {
            if ((object) x == null)
            {
                if ((object) y == null)
                    return true;
                else
                    return false;
            }
            else if ((object) y == null)
            {
                return false;
            }
            else
            {
                return x.Equals(y);
            }
        }

        /// <summary>
        /// Vrací hešový kód pro konkrétní objekt
        /// </summary>
        /// <param name="obj">Objekt typu Ukaz</param>
        /// <returns>Generovaný hešový kód.</returns>
        public int GetHashCode(Ukaz obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        #region IComparer<Ukaz> Members

        /// <summary>
        /// Porovná dva objekty typu Ukaz a urèí, který z nich je vìtší nebo menší
        /// </summary>
        /// <param name="x">Objekt porovnání na levé stranì</param>
        /// <param name="y">Objekt porovnání na pravé stranì</param>
        /// <returns>Vrátí 0, pokud jsou si objekty rovny; 1, pokud je objekt x menší než objekt y; -1 pokud je objekt y menší než objekt x.</returns>
        public int Compare(Ukaz x, Ukaz y)
        {
            return ((IComparable) x).CompareTo(y);
        }

        #endregion

        #region Øazení

        public void Sort()
        {
            this.Sort(new UkazComparer());
        }

        public void Sort(PoleRazeni przPoleRazeni, SmerRazeni srzSmerRazeni)
        {
            this.Sort(new UkazComparer(przPoleRazeni, srzSmerRazeni));
        }

        public void Sort(IComparer<Ukaz> comparer)
        {
            List<Ukaz> lsUkaz = (List<Ukaz>) base.Items;
            lsUkaz.Sort(comparer);
            for (int i = 0; i < lsUkaz.Count - 1; i++)
            {
                base.SetItem(i, lsUkaz[i]);
            }
        }

        public IList<Ukaz> NajdiVNazvu(string strText)
        {
            UkazFilter fu = new UkazFilter(true, strText);
            return FindAll(fu.ObsahujeText);
        }

        public IList<Ukaz> FindAll(Predicate<Ukaz> match)
        {
            List<Ukaz> mlsNeserazene = (List<Ukaz>) base.Items;
            return mlsNeserazene.FindAll(match);
        }

        #endregion
    }
}