using System;
using System.Collections.Generic;
using System.IO;

namespace Daliboris.Text
{
	public class WordNGram
	{
		private List<string> _words;

		#region Constructors

		public WordNGram()
		{
			NumberOfWords = 2;
		}

		public WordNGram(byte numberOfWords)
		{
			NumberOfWords = numberOfWords;
		}


		public WordNGram(byte numberOfWords, string text)
		{
			NumberOfWords = numberOfWords;
			string[] words = Slova.RozdelitTextNaSlova(text);
			Words = new List<string>(words);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Default is 2</remarks>
		public byte NumberOfWords { get; set; }


		public List<string> Words
		{
			get { return _words; }
			set
			{
				if (value.Count > NumberOfWords)
				{
					
				}
				_words = new List<string>();
				foreach (string s in value)
				{
					_words.Add(s);
				}
			}
		}

		#endregion

		#region Properties

		#endregion

		#region Methods

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return string.Join(" ", Words.ToArray());
		}

		#endregion

		#endregion

		#region Helpers

		public static List<WordNGram> SplitString(byte numberOfWords, string text)
		{
			List<WordNGram> list = new List<WordNGram>();
			string[] words = Slova.RozdelitTextNaSlova(text, true, false, "\"'--");
			for (int i = 0; i < words.Length; i++)
			{
				WordNGram wordNGram = new WordNGram(numberOfWords);
				wordNGram.Words = new List<string>(numberOfWords);
				for (int j = 0; j < numberOfWords; j++)
				{
					if((i + j) < words.Length) 
						wordNGram.Words.Add(words[i + j]);
				}

				list.Add(wordNGram);
			}
			return list;
		}

		#endregion


		 
	}
}