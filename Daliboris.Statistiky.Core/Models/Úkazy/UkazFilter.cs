using System;

namespace Daliboris.Statistiky
{
    public class UkazFilter
    {
        public enum Operator
        {
            JeRovno,
            JeVetsi,
            JeVetsiRovno,
            JeMensi,
            JeMensiRovno,
            NeniRovno
        }

        bool mblnNerozlisovatVelikostPismen = false;
        Operator mopOperator = Operator.JeRovno;

        public UkazFilter(bool bNerozlisovatVelikostPismen)
        {
            this.NerozlisovatVelikostPismen = bNerozlisovatVelikostPismen;
        }

        public UkazFilter(bool bNerozlisovatVelikostPismen, string strText)
        {
            this.NerozlisovatVelikostPismen = bNerozlisovatVelikostPismen;
            this.Text = strText;
        }

        public UkazFilter(Operator opOperator, int intPocet)
        {
            this.Porovnani = opOperator;
            this.Pocet = intPocet;
        }


        public bool NerozlisovatVelikostPismen
        {
            get { return mblnNerozlisovatVelikostPismen; }
            set { mblnNerozlisovatVelikostPismen = value; }
        }

        public string Text { get; set; }
        public string Nazev { get; set; }

        public string Specifikace { get; set; }

        //public string Retrograd { get; set; }
        public int Pocet { get; set; }

        public Operator Porovnani
        {
            get { return mopOperator; }
            set { mopOperator = value; }
        }


        public static bool ZacinaVelkymPismenem(Ukaz uk)
        {
            if (String.IsNullOrEmpty(uk.Nazev))
                return false;
            return Char.IsLetter(uk.Nazev, 0) && Char.IsUpper(uk.Nazev, 0);
        }

        public static bool ZacinaMalymPismenem(Ukaz uk)
        {
            if (String.IsNullOrEmpty(uk.Nazev))
                return false;
            return Char.IsLetter(uk.Nazev, 0) && Char.IsLower(uk.Nazev, 0);
        }

        public static bool NezacinaPismenem(Ukaz uk)
        {
            if (String.IsNullOrEmpty(uk.Nazev))
                return false;
            return !Char.IsLetter(uk.Nazev, 0);
        }


        public bool OdpovidaPocet(Ukaz uk)
        {
            bool bOdpovidaPodmince = false;
            if (uk == null)
                return bOdpovidaPodmince;

            switch (this.Porovnani)
            {
                case Operator.JeRovno:
                    bOdpovidaPodmince = (uk.Pocet == this.Pocet);
                    break;
                case Operator.JeVetsi:
                    bOdpovidaPodmince = (uk.Pocet > this.Pocet);
                    break;
                case Operator.JeVetsiRovno:
                    bOdpovidaPodmince = (uk.Pocet >= this.Pocet);
                    break;
                case Operator.JeMensi:
                    bOdpovidaPodmince = (uk.Pocet < this.Pocet);
                    break;
                case Operator.JeMensiRovno:
                    bOdpovidaPodmince = (uk.Pocet <= this.Pocet);
                    break;
                case Operator.NeniRovno:
                    bOdpovidaPodmince = (uk.Pocet != this.Pocet);
                    break;
                default:
                    break;
            }

            return bOdpovidaPodmince;
        }


        public bool ObsahujeText(Ukaz uk)
        {
            const char mcchHraniceSlova = '\u2038';
            bool bObsahuje = false;
            if (uk.Nazev == null) return bObsahuje;


            string sText = this.NerozlisovatVelikostPismen ? Text.ToLower() : Text;
            if (sText == null) return bObsahuje;

            if (Text.Length == 0) return true;
            string sSrovnani = this.NerozlisovatVelikostPismen ? uk.Nazev.ToLower() : uk.Nazev;

            if (sText[0] == mcchHraniceSlova)
            {
                sText = sText.Substring(1);
                return sSrovnani.StartsWith(sText);
            }

            if (sText[sText.Length - 1] == mcchHraniceSlova)
            {
                sText = sText.Substring(0, sText.Length - 1);
                return sSrovnani.EndsWith(sText);
            }

            return sSrovnani.Contains(sText);
        }
    }
}