using System;
using System.IO;

namespace Daliboris.Word.Text
{
    public class Soubor
    {
        private string sCesta = null;
        Slova slvSlova = new Slova();
        string sChyby = null;

        public Soubor(string Soubor)
        {
            sCesta = Soubor;
        }

        public bool Analyzovat()
        {
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = new FileStream(sCesta, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs, System.Text.Encoding.GetEncoding("Unicode"));

                sr = File.OpenText(sCesta);
                char[] chSeparatory = new Char[]
                {
                    ',', ' ', '(', ')', '.', '[', ']', '"', '-', '–', '!', '%', '/', ':', ';', '?', '+', '=', '…', '$',
                    '×', '†', '“', '„', '\t'
                };
                string sRadek;

                while ((sRadek = sr.ReadLine()) != null)
                {
                    string[] arSlova = sRadek.Split(chSeparatory);
                    foreach (string s in arSlova)
                    {
                        if (slvSlova.Existuje(s))
                            slvSlova[s].Pocet++;
                        else
                            slvSlova.Add(new Slovo(s));
                    }
                }
            }
            catch (Exception ev)
            {
                sChyby = ev.ToString();
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (fs != null)
                    fs.Close();
            }

            return (sChyby == null);
        }

        public Slova Slova
        {
            get { return slvSlova; }
        }

        public string Chyby
        {
            get { return sChyby; }
        }
    }
}