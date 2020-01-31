using Daliboris.Text;

namespace Daliboris.Word.Text
{
    /// <summary>
    /// Abstraktní třída typografických prvků v textu.
    /// </summary>
    public class TypografickyPrvek : IPocitatelny
    {
        string strText;
        int intPocet;
        int intPoradiRazeni;
        Znak[] znkZnaky;

        /// <summary>
        /// Vytváří instanci třídy TypografickýPrvek.
        /// </summary>
        /// <param name="Text">Text, který reprezentuje daný prvek.</param>
        public TypografickyPrvek(string Text)
        {
            strText = Text;
            intPocet = 1;
            znkZnaky = new Znak[strText.Length];
            char[] pole = strText.ToCharArray();
            for (int i = 0; i < pole.Length; i++)
                znkZnaky[i] = new Znak(pole[i]);
            intPoradiRazeni =
                -1; //= (int) Thread.CurrentThread.CurrentCulture.CompareInfo.GetSortKey(strText).ToString() ;
        }

        /// <summary>
        /// Vytváří instanci třídy TypografickýPrvek.
        /// </summary>
        /// <param name="Text">Text, který reprezentuje daný prvek.</param>
        /// <param name="PoradiRazeni">Uživatelsky definované pořadí řazení prvku v rámci skupiny.</param>
        public TypografickyPrvek(string Text, int PoradiRazeni)
        {
            strText = Text;
            intPocet = 1;
            znkZnaky = new Znak[strText.Length - 1];
            char[] pole = strText.ToCharArray();
            for (int i = 0; i < pole.Length; i++)
                znkZnaky[i] = new Znak(pole[i]);
            //znkZnaky[] = strText.ToCharArray();
            intPoradiRazeni = PoradiRazeni;
        }

        /// <summary>
        /// Vrací textovou podobu typografického prvku.
        /// </summary>
        public string Text
        {
            get { return strText; }
        }

        /// <summary>
        /// Vrací počet výskytů prvku v textu.
        /// </summary>
        public int Pocet
        {
            get { return intPocet; }
            set { intPocet = value; }
        }

        /// <summary>
        /// Nastavuje/vrací pořadí prvku v rámci individuálního řazení.
        /// </summary>
        public int PoradiRazeni
        {
            get { return intPoradiRazeni; }
            set { intPoradiRazeni = value; }
        }

        /// <summary>
        /// Vrací pole znaků, které tvoří daný typografický prvek.
        /// </summary>
        public Znak[] Znaky
        {
            get { return znkZnaky; }
        }
    }
}