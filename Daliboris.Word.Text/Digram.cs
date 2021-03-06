﻿using System;

namespace Daliboris.Word.Text
{
    public class Digram : IGram
    {
        private char[] machDigramy = new char[2];

        public Digram()
        {
        }

        public Digram(char chPrvni, char chDruhy)
        {
            machDigramy[0] = chPrvni;
            machDigramy[1] = chDruhy;
        }

        public char[] Znaky
        {
            get { return machDigramy; }
            set
            {
                if (value.Length != 2)
                    throw new ArgumentException("Hodnota neobsahuje dva prvky");
                machDigramy = value;
            }
        }

        public override string ToString()
        {
            return machDigramy[0].ToString() + machDigramy[1].ToString();
        }
    }
}