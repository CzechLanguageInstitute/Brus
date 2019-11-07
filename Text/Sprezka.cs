namespace Daliboris.Text
{
	public class Sprezka : TypografickyPrvek {
		private Grafem[] grGrafemy;
		public Sprezka(string Text) : base(Text) { }
		public Sprezka(string Text, int PoradiRazeni) : base(Text, PoradiRazeni) { }
		public Grafem[] Grafemy { get { return grGrafemy; } }
	}
}