namespace Daliboris.Statistiky.Word
{
    public partial class WordService
    {
        private class UsekZnakovehoStylu
        {
            public string Text { get; set; }
            public string Styl { get; set; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public UsekZnakovehoStylu(string text, string styl)
            {
                Text = text;
                Styl = styl;
            }
        }
    }
}