using Daliboris.Word.Text;

namespace Daliboris.Text
{
    /// <summary>
    /// Nejmenší jednotka textu. Stavební jednotka z hlediska počítačového.
    /// </summary>
    public class Znak : IPocitatelny
    {
        int mintPocet;
        readonly char mchZnak;

        /// <summary>
        /// Vytváří novou instanci.
        /// </summary>
        /// <param name="Znak">Unicodový znak, který je představitelem třídy.</param>
        public Znak(char Znak)
        {
            mintPocet = 1;
            mchZnak = Znak;
        }

        /// <summary>
        /// Vrací znak.
        /// </summary>
        public char Text
        {
            get { return mchZnak; }
        }

        /// <summary>
        /// Nastavuje/vrací počet výskytů daného znaku.
        /// </summary>
        public int Pocet
        {
            get { return mintPocet; }
            set { mintPocet = value; }
        }

        /// <summary>
        /// Vrací hodnotu znaku v podobě celého čísla.
        /// </summary>
        public int Unicode
        {
            get { return (int) mchZnak; }
        }
    }
}