using System;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace Daliboris.Word.Text
{
    /// <summary>
    /// Třída pro uchovávání skupin o zvoleném počtu znaků.
    /// Výchozí počet znaků je 2, tj. digram.
    /// </summary>
    [XmlRoot(ElementName = "ng")]
    public class NGram : IGram
    {
        private char[] machNgramy = new char[2];
        private byte mbtPocetZnaku = 2;

        /// <summary>
        /// Implicitní konstruktor
        /// </summary>
        public NGram()
        {
        }


        /// <summary>
        /// Konstruktor, který umožňuje definovat počet počítaných prvků
        /// </summary>
        /// <param name="btPocetZnaku">Počet prvků v rámci jedné n-tice</param>
        public NGram(byte btPocetZnaku)
        {
            PocetZnaku = btPocetZnaku;
        }

        public NGram(string strZnaky)
        {
            if (strZnaky.Length > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("Text překročil maximální povolednou délku.");
            PocetZnaku = (byte) strZnaky.Length;
            machNgramy = strZnaky.ToCharArray();
        }

        #region IGram Members

        [XmlAttribute(AttributeName = "p")]
        public byte PocetZnaku
        {
            get { return mbtPocetZnaku; }
            set
            {
                mbtPocetZnaku = value;
                machNgramy = new char[mbtPocetZnaku];
            }
        }

        [XmlArray(ElementName = "chars")]
        public char[] Znaky
        {
            get { return machNgramy; }
            set
            {
                if (value.Length != PocetZnaku)
                    throw new ArgumentException("Hodnota neobsahuje odpovídající počet prvků (" +
                                                mbtPocetZnaku.ToString() + ").");
                machNgramy = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(mbtPocetZnaku);
            foreach (char ch in machNgramy)
            {
                sb.Append(ch);
            }

            return sb.ToString();
        }

        #endregion

        public override bool Equals(object obj)
        {
            return machNgramy.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public class NGramy : CollectionBase
    {
        private const string cstrHraniceSlova = " ";

        //private const char mcchHraniceSlova = '\u2058'; //= FOUR DOT PUNCTUATION
        private const char cmcchHraniceSlova = '\u2038'; //= CARET
        private char mchHraniceSlova = cmcchHraniceSlova;
        private byte mbtPocetZnaku = 2;

        public char HraniceSlova
        {
            get { return mchHraniceSlova; }
            set { mchHraniceSlova = value; }
        }

        public byte PocetZnaku
        {
            get { return mbtPocetZnaku; }
            set { mbtPocetZnaku = value; }
        }

        public NGramy()
        {
        }

        public NGramy(byte btPocetZnaku)
        {
            PocetZnaku = btPocetZnaku;
        }

        public NGramy(byte btPocetZnaku, char chHraniceSlova) : this(btPocetZnaku)
        {
            mchHraniceSlova = chHraniceSlova;
        }

        public static char HraniceSlovaVychozi
        {
            get { return cmcchHraniceSlova; }
        }

        public static NGramy ZpracujNGramy(string strText, byte btPocetZnaku)
        {
            return ZpracujNGramy(strText, btPocetZnaku, cmcchHraniceSlova);
        }

        public static NGramy ZpracujNGramy(string strText, byte btPocetZnaku, char chHraniceSlova)
        {
            NGramy ngZnaky = new NGramy(btPocetZnaku);
            int iDelka = strText.Length;
            if (iDelka < btPocetZnaku)
                return ngZnaky;

            for (int i = -1; i < iDelka; i++)
            {
                NGram ng;
                if (i == -1)
                {
                    ng = new NGram(chHraniceSlova.ToString() + strText.Substring(i + 1, btPocetZnaku - 1));
                }
                else if (i == iDelka - btPocetZnaku + 1)
                {
                    ng = new NGram(strText.Substring(i) + chHraniceSlova.ToString());
                    ngZnaky.List.Add(ng);
                    return ngZnaky;
                }
                else
                {
                    ng = new NGram(strText.Substring(i, btPocetZnaku));
                }

                ngZnaky.List.Add(ng);
            }

            return ngZnaky;
        }
    }
}