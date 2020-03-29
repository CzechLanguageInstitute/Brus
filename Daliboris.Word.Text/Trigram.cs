using System;
using Daliboris.Text;

namespace Daliboris.Word.Text
{
    public class Trigram : IGram
    {
        private char[] machTrigramy = new char[3];

        public Trigram()
        {
        }

        public Trigram(char chPrvni, char chDruhy, char chTreti)
        {
            machTrigramy[0] = chPrvni;
            machTrigramy[1] = chDruhy;
            machTrigramy[2] = chTreti;
        }


        #region IGram Members

        public char[] Znaky
        {
            get { return machTrigramy; }
            set
            {
                if (value.Length != 3)
                    throw new ArgumentException("Hodnota neobsahuje tři prvky");
                machTrigramy = value;
            }
        }

        public override string ToString()
        {
            return machTrigramy[0].ToString() + machTrigramy[1].ToString() + machTrigramy[3].ToString();
        }

        #endregion
    }
}