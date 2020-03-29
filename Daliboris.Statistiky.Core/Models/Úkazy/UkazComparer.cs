using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Reflection;
using System.Globalization;

namespace Daliboris.Statistiky
{
    public enum PoleRazeni
    {
        Nazev,
        Pocet,
        Obsah,
        Specifikace
    }

    public enum SmerRazeni
    {
        Vzestupne,
        Sestupne
    }

    public class UkazComparer : Comparer<Ukaz>
    {
        private PoleRazeni meprPoleRazeni = PoleRazeni.Nazev;
        private SmerRazeni mesrSmerRazeni = SmerRazeni.Vzestupne;
        private CompareOptions mcoZpusobRazeniTextu = CompareOptions.StringSort;

        #region Konstruktory

        public UkazComparer()
        {
        }

        public UkazComparer(PoleRazeni eprPoleRazeni) :
            this(eprPoleRazeni, SmerRazeni.Vzestupne)
        {
        }

        public UkazComparer(SmerRazeni esrSmerRazeni)
            : this(PoleRazeni.Nazev, esrSmerRazeni)
        {
        }


        public UkazComparer(PoleRazeni eprPoleRazeni, SmerRazeni esrSmerRazeni) :
            this(eprPoleRazeni, esrSmerRazeni, CompareOptions.StringSort)
        {
        }


        public UkazComparer(CompareOptions coZpusobRazeniTextu) :
            this(PoleRazeni.Nazev, SmerRazeni.Vzestupne, coZpusobRazeniTextu)
        {
        }

        public UkazComparer(PoleRazeni eprPoleRazeni, SmerRazeni esrSmerRazeni, CompareOptions coZpusobRazeniTextu)
        {
            ZpusobRazeniTextu = coZpusobRazeniTextu;
            PoleRazeni = eprPoleRazeni;
            SmerRazeni = esrSmerRazeni;
        }

        #endregion

        #region Veřejné vlastnosti

        public PoleRazeni PoleRazeni
        {
            get { return meprPoleRazeni; }
            set { meprPoleRazeni = value; }
        }

        public SmerRazeni SmerRazeni
        {
            get { return mesrSmerRazeni; }
            set { mesrSmerRazeni = value; }
        }

        public CompareOptions ZpusobRazeniTextu
        {
            get { return mcoZpusobRazeniTextu; }
            set { mcoZpusobRazeniTextu = value; }
        }

        #endregion

        public override int Compare(Ukaz x, Ukaz y)
        {
            int iSmerRazeni = (mesrSmerRazeni == SmerRazeni.Vzestupne ? 1 : -1);

            if (x != null && y == null)
                return 1 * iSmerRazeni;

            if (x == null && y != null)
                return -1 * iSmerRazeni;

            if (x == null && y == null)
                return 0 * iSmerRazeni;

            //Nejprve je potřeba porovnat specifikace; přednost má nespecifikovaná položka před specikovanou

            if (x.Specifikace != null && y.Specifikace == null)
                return 1 * iSmerRazeni;
            if (x.Specifikace == null && y.Specifikace != null)
                return -1 * iSmerRazeni;


            string strPoleRazeni = Enum.GetName(typeof(PoleRazeni), meprPoleRazeni);
            bool blnPosledniPorovnani = false;

            Porovnani:
            object a = x.GetType().GetProperty(strPoleRazeni).GetValue(x, null);
            object b = y.GetType().GetProperty(strPoleRazeni).GetValue(y, null);


            if (a != null && b == null)
                return 1 * iSmerRazeni;

            if (a == null && b != null)
                return -1 * iSmerRazeni;

            if (a == null && b == null)
                return 0 * iSmerRazeni;

            int iPorovnani = 0;
            if (a is string)
                iPorovnani = string.Compare(a.ToString(), b.ToString(), CultureInfo.CurrentCulture,
                    mcoZpusobRazeniTextu);
            else
                iPorovnani = (((IComparable) a).CompareTo(b));
            //shodují se vlastnosti, je potřeba porovnat vlastnost následující
            if (!blnPosledniPorovnani && iPorovnani == 0)
            {
                if (meprPoleRazeni == PoleRazeni.Pocet)
                    strPoleRazeni = "Nazev";
                else
                    strPoleRazeni = "Pocet";
                iSmerRazeni = 1;
                blnPosledniPorovnani = true;
                goto Porovnani;
            }

            return iPorovnani * iSmerRazeni;
        }
    }
}

/*
 * 
 * 
 * 
 * public enum SortOrder
{
Ascending,
Descending
}

public class Sorter<T> : IComparer<T>
{
string _SortString = String.Empty;
public string SortString
{
get { return _SortString.Trim(); }
set { _SortString = value; }
}

public Sorter() { }

public Sorter(string sortstring)
{
_SortString = sortstring;
}

#region IComparer<T> Members

public int Compare(T x, T y)
{
int result = 0;

if (!string.IsNullOrEmpty(SortString))
{
Type t = typeof(T);

Comparer c = Comparer.DefaultInvariant;
System.Reflection.PropertyInfo pi;

foreach (string expr in SortString.Split(new char[] {','}))
{
SortOrder dir = SortOrder.Ascending;
string field;

if (expr.EndsWith(" DESC"))
{
field = expr.Replace(" DESC", String.Empty).Trim();
dir = SortOrder.Descending;
}
else {
field = expr.Replace(" ASC", String.Empty).Trim();
}
pi = t.GetProperty(field);
if (pi != null)
{
result = c.Compare(pi.GetValue(x, null), pi.GetValue(y, null));
if (dir.Equals(SortOrder.Descending))
{
result = -result;
}
if (result != 0)
{
break;
}
}
}
return result;
}
return result;
}
*/