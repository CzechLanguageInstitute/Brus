namespace Daliboris.Text
{
    
        /// <summary>
        /// Nastavení umožňující ovlivnit vymezení slov
        /// </summary>
        public class Nastaveni {
            private char[] machMezery = Slova.cachMezery;

            private char[] machParovaInterpunkce = new char[] { '(', ')', '[', ']', '{', '}' };

            public char[] ParovaInterpunkce {
                get { return machParovaInterpunkce; }
                set { machParovaInterpunkce = value; }
            }

            public char[] Mezery {
                get { return machMezery; }
                set { machMezery = value; }
            }
            private char[] machPocatecniInterpunkce = new char[] { '\u206E', '\u201A', '\u201E', '\u2026' }; //tj. spodní jednoduchá a dvojitá uvozovka, trojtečka

            public char[] PocatecniInterpunkce {
                get { return machPocatecniInterpunkce; }
                set { machPocatecniInterpunkce = value; }
            }
            private char[] machKoncovaInterpunkce = new char[] { '\u201B', '!', '?', ',', ';', ':', '\u2026', '\u201C' }; //tj. horní jednoduchá a dvojitá uvozovka a trojtečka

            public char[] KoncovaInterpunkce {
                get { return machKoncovaInterpunkce; }
                set { machKoncovaInterpunkce = value; }
            }

            private char[] machZnakyKOdstraneni = new char[] { };

            public char[] ZnakyKOdstraneni {
                get { return machZnakyKOdstraneni; }
                set { machZnakyKOdstraneni = value; }
            }
            private char[] machDelimitatorySlov = new char[] {};

            public char[] DelimitatorySlov {
                get { return machDelimitatorySlov; }
                set { machDelimitatorySlov = value; }
            }
            private bool mblnOdstranitTecku;

            public bool OdstranitTecku {
                get { return mblnOdstranitTecku; }
                set { mblnOdstranitTecku = value; }
            }
            private bool mblnPonechatInterpunkci = true;

            public bool PonechatInterpunkci {
                get { return mblnPonechatInterpunkci; }
                set { mblnPonechatInterpunkci = value; }
            }

            /// <summary>
            /// Odstraní z výsledného seznamu prázdné řetězce
            /// </summary>
            public bool OdstranitPrazdnaSlova { get; set; }

        }
    }