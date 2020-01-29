namespace Daliboris.Statistiky.Word
{
    public partial class WordService
    {
        private class Usek
        {
            public string Lokace { get; set; }
            public string Styl { get; set; }
            public string Text { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public Usek(string styl, string text)
            {
                Styl = styl;
                Text = text;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public Usek(string styl, string text, string lokace)
                : this(styl, text)
            {
                Lokace = lokace;
            }
        }
    }
}