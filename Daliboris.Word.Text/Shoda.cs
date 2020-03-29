using System;
using System.Collections.Generic;

namespace Daliboris.Word.Text
{
    public class Shoda : IShoda
    {
        private string mstrPredchozi;
        private string mstrNasledujici;
        private string mstrAktualni;
        private string mstrZacatek;
        private string mstrStred;
        private string mstrKonec;
        private bool mblnRetrograd;
        private StringComparison mscpZpusobPorovnani = StringComparison.CurrentCulture;


        #region IShoda Members

        public StringComparison ZpusobPorovnani
        {
            get { return mscpZpusobPorovnani; }
            set { mscpZpusobPorovnani = value; }
        }

        public string Predchozi
        {
            get { return mstrPredchozi; }
            set { mstrPredchozi = value; }
        }

        public string Nasledujici
        {
            get { return mstrNasledujici; }
            set { mstrNasledujici = value; }
        }

        public string Aktualni
        {
            get { return mstrAktualni; }
            set { mstrAktualni = value; }
        }

        public string Zacatek
        {
            get { return mstrZacatek; }
            set { mstrZacatek = value; }
        }

        public string Stred
        {
            get { return mstrStred; }
            set { mstrStred = value; }
        }

        public string Konec
        {
            get { return mstrKonec; }
            set { mstrKonec = value; }
        }

        public bool Retrograd
        {
            get { return mblnRetrograd; }
            set { mblnRetrograd = value; }
        }

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}", mstrZacatek, mstrStred, mstrKonec);
        }

        public Shoda()
        {
        }

        public Shoda(string strPredchozi, string strAktualni, string strNasledujici, bool blnRetrograd,
            StringComparison scpZpusobPorovnani)
        {
            mstrAktualni = strAktualni;
            mstrPredchozi = strPredchozi;
            mstrNasledujici = strNasledujici;
            mscpZpusobPorovnani = scpZpusobPorovnani;
            UrciShodu();
        }

        public Shoda(string strPredchozi, string strAktualni, string strNasledujici,
            StringComparison scpZpusobPorovnani)
            : this(strPredchozi, strAktualni, strNasledujici, false, scpZpusobPorovnani)
        {
        }

        public Shoda(string strPredchozi, string strAktualni, string strNasledujici, bool blnRetrograd)
            : this(strPredchozi, strAktualni, strNasledujici, blnRetrograd, StringComparison.CurrentCulture)
        {
        }

        public Shoda(string strPredchozi, string strAktualni, string strNasledujici)
            : this(strPredchozi, strAktualni, strNasledujici, false)
        {
        }

        private void UrciShodu()
        {
            int iPredchozi = Shoda.PocetShodnychZnaku(mstrPredchozi, mstrAktualni, mblnRetrograd, mscpZpusobPorovnani);
            int iNasledujici =
                Shoda.PocetShodnychZnaku(mstrAktualni, mstrNasledujici, mblnRetrograd, mscpZpusobPorovnani);
            if (!mblnRetrograd)
            {
                if (iPredchozi >= iNasledujici)
                {
                    mstrZacatek = mstrAktualni.Substring(0, iPredchozi);
                    mstrKonec = mstrAktualni.Substring(iPredchozi);
                }
                else
                {
                    mstrZacatek = mstrAktualni.Substring(0, iPredchozi);
                    mstrStred = mstrAktualni.Substring(iPredchozi, iNasledujici - iPredchozi);
                    mstrKonec = mstrAktualni.Substring(iNasledujici);
                }
            }
            else
            {
                if (iNasledujici >= iPredchozi)
                {
                    mstrKonec = mstrAktualni.Substring(iNasledujici);
                    mstrZacatek = mstrAktualni.Substring(0, iPredchozi);
                }
            }
        }

        public static IList<Shoda> UrciShodu(string strSeznamSlov, string strOddelovac, bool blnRetrograd,
            StringComparison scpZpusobPorovnani)
        {
            string[] asSeznam = strSeznamSlov.Split(new string[] {strOddelovac}, StringSplitOptions.RemoveEmptyEntries);
            int iDelka = asSeznam.Length;
            List<Shoda> lstShoda = new List<Shoda>(iDelka);
            for (int i = 0; i < iDelka; i++)
            {
                if (i == 0)
                {
                    lstShoda.Add(new Shoda(null, asSeznam[i], asSeznam[i + 1], blnRetrograd, scpZpusobPorovnani));
                }
                else if (i < iDelka - 1)
                {
                    lstShoda.Add(new Shoda(asSeznam[i - 1], asSeznam[i], asSeznam[i + 1], blnRetrograd,
                        scpZpusobPorovnani));
                }
                else
                {
                    lstShoda.Add(new Shoda(asSeznam[i - 1], asSeznam[i], null, blnRetrograd, scpZpusobPorovnani));
                }
            }

            return (IList<Shoda>) lstShoda;
        }

        public static IList<Shoda> UrciShodu(string strSeznamSlov, string strOddelovac, bool blnRetrograd)
        {
            return UrciShodu(strSeznamSlov, strOddelovac, blnRetrograd, StringComparison.CurrentCulture);
        }

        public static IList<Shoda> UrciShodu(string strSeznamSlov, string strOddelovac)
        {
            return UrciShodu(strSeznamSlov, strOddelovac, false);
        }

        public static IList<Shoda> UrciShodu(string strSeznamSlov, string strOddelovac,
            StringComparison scpZpusobPorovnani)
        {
            return UrciShodu(strSeznamSlov, strOddelovac, false, scpZpusobPorovnani);
        }

        public static Shoda UrciShodu(string strPredchozi, string strAktualni, string strNasledujici, bool blnRetrograd)
        {
            Shoda shd = new Shoda(strPredchozi, strAktualni, strNasledujici, blnRetrograd);
            return shd;
        }

        /// <summary>
        /// Určuje počet shodných znaků ve dvou slovech od začátku slova se zřetelem na velikost písmen.
        /// </summary>
        /// <param name="strX">První slovo na porovnání</param>
        /// <param name="strY">Druhé slovo na porovnání</param>
        /// <returns>Vrací počet shodných znaků počítáno odpředu</returns>
        public static int PocetShodnychZnaku(string strX, string strY)
        {
            return PocetShodnychZnaku(strX, strY, StringComparison.CurrentCulture);
        }

        public static int PocetShodnychZnaku(string strX, string strY, StringComparison ZpusobPorovnani)
        {
            return PocetShodnychZnaku(strX, strY, false, ZpusobPorovnani);
        }

        public static int PocetShodnychZnaku(string strX, string strY, bool blnOdzadu)
        {
            return PocetShodnychZnaku(strX, strY, blnOdzadu, StringComparison.CurrentCulture);
        }

        public static int PocetShodnychZnaku(string strX, string strY, bool blnOdzadu, StringComparison ZpusobPorovnani)
        {
            int iPocet = 0;
            if (strX == null || strY == null)
                return iPocet;
            if (String.Equals(strX, strY, ZpusobPorovnani))
                return strY.Length;
            int iNejdelsi = Math.Min(strX.Length, strY.Length);
            if (blnOdzadu)
            {
                for (int i = iNejdelsi - 1; i >= 0; i--)
                {
                    if (!String.Equals(strX[i].ToString(), strY[i].ToString(), ZpusobPorovnani))
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < iNejdelsi; i++)
                {
                    if (!String.Equals(strX[i].ToString(), strY[i].ToString(), ZpusobPorovnani))
                        return i;
                }
            }

            return iNejdelsi;
        }

        #endregion
    }
}