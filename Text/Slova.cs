using System;
using System.Collections;
using System.Collections.Generic;

namespace Daliboris.Text
{
	public partial class Slova {
	    private Nastaveni mnstNastaveni = new Nastaveni();
		private SortedList sl = new SortedList();
		internal static char[] cachMezery = null;
		
		static Slova(){
			cachMezery  = new char[] { '\u0020', '\u00A0', '\u1680', '\u180E', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200A', '\u200B', '\u200C', '\u200D', '\u202F', '\u205F', '\u2060', '\u3000', '\uFEFF' };
		}

		public Slova() { }
		public Slova(Nastaveni nstNastaveni) { 
			if(nstNastaveni != null)
				this.Settings = nstNastaveni;
		}

		public Nastaveni Settings {
			get { return mnstNastaveni; }
			set { mnstNastaveni = value; }
		}
		public virtual void Add(Slovo objSlovo) { sl.Add(objSlovo.OriginalniPodoba, objSlovo); }
		public virtual void Remove(string OriginalniPodoba) { sl.Remove(OriginalniPodoba); }

		public char[] MezeryVychozi {
			get { return cachMezery; }
		}

		public virtual void Replace(Slovo objSlovo) {
			this.Remove(objSlovo.OriginalniPodoba);
			this.Add(objSlovo);
		}

		public bool Existuje(Slovo objSlovo) {
			Slovo tg = (Slovo)sl[objSlovo.OriginalniPodoba];
			return (tg != null);
		}
		public bool Existuje(int Index) {
			Slovo tg = (Slovo)sl[Index];
			return (tg != null);
		}
		public bool Existuje(string OriginalniPodoba) {
			Slovo tg = (Slovo)sl[OriginalniPodoba];
			return (tg != null);
		}
		public Slovo this[int Index] {
			get { return ((Slovo)sl.GetByIndex(Index)); }
		}

		public Slovo this[string OriginalniPodoba] {
			get { return ((Slovo)sl[OriginalniPodoba]); }
		}
		public int Count {
			get { return (sl.Count); }
		}

		public static string[] RozdelitTextNaSlova(string strText, Nastaveni nstNastaveni) {

			char[] achTecka = new char[] { '.' };
			char[] achDelimitatory = new char[nstNastaveni.Mezery.Length + nstNastaveni.DelimitatorySlov.Length];
			int iDelim = 0;
			for (int i = 0; i < nstNastaveni.Mezery.Length; i++) {
				achDelimitatory[iDelim++] = nstNastaveni.Mezery[i];
			}
			for (int i = 0; i < nstNastaveni.DelimitatorySlov.Length; i++) {
				achDelimitatory[iDelim++] = nstNastaveni.DelimitatorySlov[i];
			}

			List<string> glstText = new List<string>(strText.Split(achDelimitatory, StringSplitOptions.RemoveEmptyEntries));

			if (!nstNastaveni.PonechatInterpunkci) {
				OdstranitParovouInterpunkci(nstNastaveni, glstText);
			}

			if (!nstNastaveni.PonechatInterpunkci) {
				for (int i = 0; i < glstText.Count; i++) {
					if (glstText[i].IndexOfAny(nstNastaveni.PocatecniInterpunkce) > -1 || glstText[i].IndexOfAny(nstNastaveni.KoncovaInterpunkce) > -1) {
						bool bZmeneno = true;
						while (bZmeneno) {
							bZmeneno = false;
							if (glstText[i].Length > 0) {
								if (glstText[i][0].ToString().IndexOfAny(nstNastaveni.PocatecniInterpunkce) > -1 || glstText[i].IndexOfAny(nstNastaveni.KoncovaInterpunkce, glstText[i].Length - 1) > -1) {
									bZmeneno = true;
									glstText[i] = glstText[i].TrimStart(nstNastaveni.PocatecniInterpunkce);
									glstText[i] = glstText[i].TrimEnd(nstNastaveni.KoncovaInterpunkce);
								}
							}
						}
					}
				}
			}

            if(!nstNastaveni.PonechatInterpunkci)
                OdstranitParovouInterpunkci(nstNastaveni, glstText);

			if (nstNastaveni.OdstranitTecku)
				for (int i = 0; i < glstText.Count; i++) {
					glstText[i] = glstText[i].TrimEnd(achTecka);
				}

			if (nstNastaveni.ZnakyKOdstraneni != null) {

				for (int i = 0; i < glstText.Count; i++) {
					while (glstText[i].IndexOfAny(nstNastaveni.ZnakyKOdstraneni) > -1) {
						glstText[i] = glstText[i].Remove(glstText[i].IndexOfAny(nstNastaveni.ZnakyKOdstraneni), 1);
					}
				}
			}
			if(nstNastaveni.OdstranitPrazdnaSlova)
			    while (glstText.Contains(""))
			    {
                    glstText.Remove("");        
			    }

            if (nstNastaveni.OdstranitTecku)
                for (int i = 0; i < glstText.Count; i++)
                {
                    glstText[i] = glstText[i].TrimEnd(achTecka);
                }

			return glstText.ToArray();

		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nstNastaveni"></param>
        /// <param name="glstText"></param>
        /// <remarks>Pravidla pro odstranění párové interpunkce: pokud výraz obsahuje obě položky páru <example>(ve)dení</example>, interpunkce se neodstraní; pokud obsahuje pouze jednu položku <example>(ve</example>, interpunkce se odstraní.</remarks>
        /// <remarks>Pokud výraz obsahuje obě položky páru na začátku a na konci <example>„dievky“</example>, obojí se odstraní.</remarks>
	    private static void OdstranitParovouInterpunkci(Nastaveni nstNastaveni, List<string> glstText)
	    {
            //odstraní párovou interpunkci ze začátku a z konce výrazu
	        for (int i = 0; i < glstText.Count; i++)
	        {
	            if (glstText[i].IndexOfAny(nstNastaveni.ParovaInterpunkce) > -1)
	            {
	                bool bZmeneno = true;
	                while (bZmeneno)
	                {
	                    bZmeneno = false;
	                    for (int j = 0; j < nstNastaveni.ParovaInterpunkce.Length; j += 2)
	                    {
	                        if (glstText[i].Length > 0 && glstText[i][0] == nstNastaveni.ParovaInterpunkce[j] &&
	                            glstText[i][glstText[i].Length - 1] == nstNastaveni.ParovaInterpunkce[j + 1])
	                        {
	                            glstText[i] = glstText[i].Substring(1, glstText[i].Length - 2);
	                            bZmeneno = true;
	                            if (glstText[i].Length == 0)
	                                bZmeneno = false;
	                        }
	                    }
	                }
	            }
	        }

            //odstraní párovou interpunkci ve výrazu, pokud se vyskytuje jenom jednou
            for (int i = 0; i < glstText.Count; i++)
            {
                if (glstText[i].IndexOfAny(nstNastaveni.ParovaInterpunkce) > -1)
                {
                    bool bZmeneno = true;
                    while (bZmeneno)
                    {
                        bZmeneno = false;
                        for (int j = 0; j < nstNastaveni.ParovaInterpunkce.Length; j += 2)
                        {
                            // (gen
                            if (glstText[i].Length > 0 && glstText[i][0] == nstNastaveni.ParovaInterpunkce[j] &&
                                glstText[i].IndexOf(nstNastaveni.ParovaInterpunkce[j + 1], 1) == -1)
                            {
                                glstText[i] = glstText[i].Substring(1, glstText[i].Length - 1);
                                bZmeneno = true;
                                if (glstText[i].Length == 0)
                                    bZmeneno = false;
                            }

                            //gen.)
                            if (glstText[i].Length > 0 && glstText[i].IndexOf(nstNastaveni.ParovaInterpunkce[j + 1], 1) > -1
                                && glstText[i][0] != nstNastaveni.ParovaInterpunkce[j] && glstText[i].IndexOf(nstNastaveni.ParovaInterpunkce[j], 1) == -1)
                            {
                                glstText[i] = glstText[i].Remove(glstText[i].IndexOf(nstNastaveni.ParovaInterpunkce[j + 1], 1), 1);
                                bZmeneno = true;
                                if (glstText[i].Length == 0)
                                    bZmeneno = false;
                            }

                        }
                    }
                }
            }


	    }

	    public static string[] RozdelitTextNaSlova(string strText, bool blnOdstranitTecku, bool blnPonechatInterpunkci, string strZnakyKOdstraneni) {
			//char[] achMezery = new char[] { ' ', '\u00A0', '\t' };
			//char[] achParove = new char[] { '(', ')', '[', ']', '{', '}' };

			Nastaveni nst = new Nastaveni();
			if (string.IsNullOrEmpty(strZnakyKOdstraneni))
				nst.ZnakyKOdstraneni = null;
			else
				nst.ZnakyKOdstraneni = strZnakyKOdstraneni.ToCharArray();
			nst.OdstranitTecku = blnOdstranitTecku;
			nst.PonechatInterpunkci = blnPonechatInterpunkci;

			return RozdelitTextNaSlova(strText, nst);


			/*
			string[] aText = RozdelitTextNaSlova(strText, blnOdstranitTecku, blnPonechatInterpunkci);
			if (String.IsNullOrEmpty(strZnakyKOdstraneni))
				return aText;
			char[] chZnakyKOdstraneni = strZnakyKOdstraneni.ToCharArray();
			for (int i = 0; i < aText.Length; i++) {
				while (aText[i].IndexOfAny(chZnakyKOdstraneni) > -1) {
					aText[i] = aText[i].Remove(aText[i].IndexOfAny(chZnakyKOdstraneni), 1);
					}
				}
			return aText;
			*/
		}
		/// <summary>
		/// Rozdělí text na slova
		/// </summary>
		/// <param name="strText">Text, který se má rozdělit na slova</param>
		/// <param name="strDelimitatorySlov">Delimitátory slov kromě mezer</param>
		/// <param name="blnOdstranitTecku">Zdá se má odstranit tečka</param>
		/// <param name="blnPonechatInterpunkci">Zda se má ponechat koncová a počáteční interpunkce</param>
		/// <param name="strZnakyKOdstraneni">Znaky, které se z textu odstraní ještě před rozdělením na slova</param>
		/// <returns>Pole slov</returns>
		public static string[] RozdelitTextNaSlova(string strText, string strDelimitatorySlov, bool blnOdstranitTecku,
		                                           bool blnPonechatInterpunkci, string strZnakyKOdstraneni) {

			Nastaveni nst = new Nastaveni();
			if (string.IsNullOrEmpty(strZnakyKOdstraneni))
				nst.ZnakyKOdstraneni = new char[] { };
			else
				nst.ZnakyKOdstraneni = strZnakyKOdstraneni.ToCharArray();
			
			if (string.IsNullOrEmpty(strDelimitatorySlov))
				nst.DelimitatorySlov = new char[] {};
			else
				nst.DelimitatorySlov = strDelimitatorySlov.ToCharArray();

			nst.OdstranitTecku = blnOdstranitTecku;
			nst.PonechatInterpunkci = blnPonechatInterpunkci;

			return RozdelitTextNaSlova(strText, nst);

			/*
			string sMezery =  " \u00A0\t" + (strDelimitatorySlov ?? String.Empty);

			char[] achMezery = sMezery.ToCharArray();
			char[] achParove = new char[] { '(', ')', '[', ']', '{', '}' };
			char[] achPocatecni = new char[] { '\u206E', '\u201A', '\u201E', '\u2026' }; //tj. spodní jednoduchá a dvojitá uvozovka, trojtečka
			char[] achKoncove = new char[] { '\u201B', '!', '?', ',', ';', ':', '\u2026', '\u201C' }; //tj. horní jednoduchá a dvojitá uvozovka a trojtečka


			string[] aText = RozdelitTextNaSlova(strText, achMezery, achParove, achPocatecni, achKoncove, blnOdstranitTecku, blnPonechatInterpunkci);


			if (String.IsNullOrEmpty(strZnakyKOdstraneni))
				return aText;
			char[] achZnakyKOdstraneni = strZnakyKOdstraneni.ToCharArray();
			for (int i = 0; i < aText.Length; i++) {
				while (aText[i].IndexOfAny(achZnakyKOdstraneni) > -1) {
					aText[i] = aText[i].Remove(aText[i].IndexOfAny(achZnakyKOdstraneni), 1);
				}
			}
			return aText;
			 */
		                                           }

		public static string[] RozdelitTextNaSlova(string strText, char[] achMezery, char[] achParove, char[] achPocatecni, char[] achKoncove,
		                                           bool blnOdstranitTecku, bool blnPonechatInterpunkci) {

			Nastaveni nst = new Nastaveni();
			nst.Mezery = achMezery;
			nst.ParovaInterpunkce = achParove;
			nst.PocatecniInterpunkce = achPocatecni;
			nst.KoncovaInterpunkce = achKoncove;
			nst.OdstranitTecku = blnOdstranitTecku;
			nst.PonechatInterpunkci = blnPonechatInterpunkci;

			return RozdelitTextNaSlova(strText, nst);

			/*
			char[] achTecka = new char[] { '.' };


			string[] aText = strText.Split(achMezery);
			if (!blnPonechatInterpunkci) {

				for (int i = 0; i < aText.Length; i++) {
					if (aText[i].IndexOfAny(achParove) > -1) {
						bool bZmeneno = true;
						while (bZmeneno) {
							bZmeneno = false;
							for (int j = 0; j < achParove.Length; j += 2) {
								if (aText[i][0] == achParove[j] && aText[i][aText[i].Length - 1] == achParove[j + 1]) {
									aText[i] = aText[i].Substring(1, aText[i].Length - 2);
									bZmeneno = true;
								}
							}
						}
					}
				}
			}

			if (!blnPonechatInterpunkci) {
				for (int i = 0; i < aText.Length; i++) {
					if (aText[i].IndexOfAny(achPocatecni) > -1 || aText[i].IndexOfAny(achPocatecni) > -1) {
						bool bZmeneno = true;
						while (bZmeneno) {
							bZmeneno = false;
							if(aText[i].Length > 0){
								if (aText[i][0].ToString().IndexOfAny(achPocatecni) > -1 || aText[i].IndexOfAny(achKoncove, aText[i].Length - 1) > -1) {
									bZmeneno = true;
									aText[i] = aText[i].TrimStart(achPocatecni);
									aText[i] = aText[i].TrimEnd(achKoncove);
								}
							}
						}
					}
				}
			}
			if (blnOdstranitTecku)
				for (int i = 0; i < aText.Length; i++) {
					aText[i] = aText[i].TrimEnd(achTecka);
				}
			//projít slova, jestli se nejedná o zkratku (zvl., Doc., m. ap.), pokud ne, odstranit tečku

			return (aText);
			*/
		                                           }

		public static string[] RozdelitTextNaSlova(string strText, bool blnOdstranitTecku, bool blnPonechatInterpunkci) {
			//201C = koncová dvojitá uvozovka
			//2026 = trojtečka
			//201F = koncová dvojitá uvozovka (nejspíš nečeská) - double-heigh-reversed-9 quotation mark
			//201B = koncová jednoduchá uvozovka
			//00A0 = pevná mezera

			Nastaveni nst = new Nastaveni();
			nst.OdstranitTecku = blnOdstranitTecku;
			nst.PonechatInterpunkci = blnPonechatInterpunkci;

			return RozdelitTextNaSlova(strText, nst);
			/*
			char[] achMezery = new char[] { ' ', '\u00A0', '\t' };
			char[] achParove = new char[] { '(', ')', '[', ']', '{', '}' };
			char[] achPocatecni = new char[] { '\u206E', '\u201A', '\u201E', '\u2026' }; //tj. spodní jednoduchá a dvojitá uvozovka, trojtečka
			char[] achKoncove = new char[] { '\u201B', '!', '?', ',', ';', ':', '\u2026', '\u201C' }; //tj. horní jednoduchá a dvojitá uvozovka a trojtečka

			return RozdelitTextNaSlova(strText, achMezery, achParove, achPocatecni, achKoncove, blnOdstranitTecku, blnPonechatInterpunkci);
			*/

			//if (blnOdstranitTecku) {
			//  strText = strText.Replace('.', ' ');
			//}


		}

		public static string[] RozdelitTextNaSlova(string strText, bool blnOdstranitTecku) {
			return RozdelitTextNaSlova(strText, blnOdstranitTecku, false);
		}

		public static string[] RozdelitTextNaSlova(string strText) {
			return RozdelitTextNaSlova(strText, false);
		}
	}
}