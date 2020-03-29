using System;
using System.Globalization;
using System.Net.Mime;
using System.Xml.Linq;
using Daliboris.OOXML.Word;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Daliboris.Statistiky.Core.Models.Jevy.XML;

namespace Daliboris.Statistiky.Word
{
    public partial class WordService
    {
        private bool jePoznamkaPodCarou;
        private int _poradiOdstavce;
        private string _souborDocx;
        private string _aktualniLokace;
        private DocxReader _reader;
        private WordSettings _nastaveni = new WordSettings();
        private SkupinaJevu _aktualniSkupinaJevu;
        private SkupinaJevu _jevyAktualnihoOdstavce;
        private List<Usek> _usekyAktulanihoOdstravceInContinuo;
        private TextRunEventArgs _predchoziUsekZnakovehoStylu = null;
        private string _textOdstavce;
        private Dictionary<string, int> _styly;
        private StringBuilder _textOdstavceSb;
        private int _nejdelsiOdstavec = 10;

        public delegate void Progress(object sender, ProgressEventArgs ev);

        public event Progress Prubeh;
        private ParagraphEventArgs _aktualniOdstavecEventArgs;
        private Stack<ParagraphEventArgs> _aktualniOdstavecStack;

        public WordSettings Nastaveni
        {
            get { return _nastaveni; }
            set { _nastaveni = value; }
        }


        public string SouborDocx
        {
            get { return _souborDocx; }
            set { _souborDocx = value; }
        }

        public List<string> ObsahDokumentu { get; set; }

        public Dictionary<string, int> StylyDokumentu
        {
            get { return _styly; }
            set { _styly = value; }
        }

        #region Konstruktory

        public WordService()
        {
        }

        public WordService(WordSettings settings)
        {
            Nastaveni = settings;
        }

        public WordService(string file, WordSettings settings)
        {
            SouborDocx = file;
            Nastaveni = settings;
        }

        public WordService(string file)
        {
            SouborDocx = file;
        }

        #endregion


        #region Události generované procházením dokumentem

        private void mdxrReader_ZacatekDokumentu(object sender, EventArgs ev)
        {
            _predchoziUsekZnakovehoStylu = null;
            _aktualniSkupinaJevu = new SkupinaJevu();
            _aktualniOdstavecStack = new Stack<ParagraphEventArgs>(2);
            ObsahDokumentu = new List<string>(10000);
            _poradiOdstavce = 0;
            _styly = new Dictionary<string, int>(60);
        }


        private void mdxrReader_KonecDokumentu(object sender, EventArgs ev)
        {
            //
        }


        private void mdxrReader_ZacatekOdstavce(object sender, ParagraphEventArgs ev)
        {
            _predchoziUsekZnakovehoStylu = null;
            _jevyAktualnihoOdstavce = new SkupinaJevu();
            _usekyAktulanihoOdstravceInContinuo = new List<Usek>(40);

            _aktualniOdstavecStack.Push(ev);
            _aktualniOdstavecEventArgs = _aktualniOdstavecStack.Peek();
            if (_textOdstavceSb != null)
                _nejdelsiOdstavec = _textOdstavceSb.Length > _nejdelsiOdstavec
                    ? _textOdstavceSb.Length
                    : _nejdelsiOdstavec;
            _textOdstavce = null;
            _textOdstavceSb = new StringBuilder(_nejdelsiOdstavec);
        }


        private void mdxrReader_KonecOdstavce(object sender, ParagraphEventArgs ev)
        {
            _predchoziUsekZnakovehoStylu = null;

            if (!_nastaveni.ZahrnoutTextOdstavce)
                return;
            string sId = Jevy.GetID(ev.Style.Language, ev.Style.ID);
            if (_textOdstavce == null)
                _textOdstavce = "\r\n";
            if (_textOdstavceSb.Length == 0)
                _textOdstavceSb.AppendLine();
            //Jev jv = new Jev(ev.Style.Language, mstrTextOdstavce, null);
            string sText = _nastaveni.OdstranitPocatecniAKoncoveMezery
                ? _textOdstavceSb.ToString().Trim()
                : _textOdstavceSb.ToString();

            StringBuilder jevySb = new StringBuilder(sText.Length);

            if (!_styly.ContainsKey(ev.Style.UIName))
                _styly.Add(ev.Style.UIName, _styly.Count + 1);
            int stylOdstavce = _styly[ev.Style.UIName];

            //jevySb.Append(String.Format("<p i=\"{0}\" s=\"{1}\">", ++poradiOdstavce, ev.Style.UIName));
            //jevySb.Append(String.Format("<p l=\"{0}\" i=\"{1}\" s=\"{2}\">", _aktualniLokace, ++poradiOdstavce, stylOdstavce));

            XElement root = new XElement("p",
                new XAttribute("l", _aktualniLokace ?? "0"),
                new XAttribute("i", ++_poradiOdstavce),
                new XAttribute("s", stylOdstavce)
            );


            char[] xmlEscape = new[] {'&', '<', '>'};

            foreach (Usek jev in _usekyAktulanihoOdstravceInContinuo)
            {
                if (!_styly.ContainsKey(jev.Styl))
                    _styly.Add(jev.Styl, _styly.Count + 1);
                int stylZnaku = _styly[jev.Styl];

                //jevySb.Append(String.Format("<r s=\"{0}\">{1}</r>", jev.Styl, jev.Text));
                string text = jev.Text;
                if (text.IndexOfAny(xmlEscape) > -1)
                    text = jev.Text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                //jevySb.Append(String.Format("<r l=\"{0}\" s=\"{1}\">{2}</r>", jev.Lokace, stylZnaku, text));
                //jevySb.Append(String.Format("<r s=\"{0}\">{1}</r>", stylZnaku, text));

                root.Add(new XElement("r",
                    new XAttribute("s", stylZnaku),
                    new XText(text)
                ));
            }

            //jevySb.Append("</p>");

            XDocument document = new XDocument();
            document.Add(root);

            string textOdstavce = jevySb.ToString();
            //ObsahDokumentu.Add(textOdstavce);
            textOdstavce = document.ToString(SaveOptions.DisableFormatting);
            ObsahDokumentu.Add(textOdstavce);

            foreach (Jevy jevy in _jevyAktualnihoOdstavce)
            {
                Jevy tmpJevy = GetJevy(jevy.Jazyk, jevy.Identifikator, jevy.Popis, jevy.Druh);
                foreach (Jev jev in jevy)
                {
                    //jev.Kontexty.Add(sText);
                    //jev.Kontexty.Add(textOdstavce);
                    jev.Kontexty.Add(_poradiOdstavce.ToString(CultureInfo.InvariantCulture));
                    //jev.Kontexty.Add(poradiOdstavce.ToString(CultureInfo.InvariantCulture));
                    tmpJevy.Add(jev);
                }
            }


            Jev jv = new Jev(ev.Style.Language, sText, null);
            Jevy jvs = GetJevy(ev.Style.Language, ev.Style.ID, ev.Style.UIName, TypJevu.Odstavce);
            jvs.Add(jv);
            _aktualniSkupinaJevu[sId] = jvs;
            if (_aktualniOdstavecStack.Count == 0)
                _aktualniOdstavecEventArgs = null;
            else
                _aktualniOdstavecEventArgs = _aktualniOdstavecStack.Pop();
        }


        /// <summary>
        ///  Regular expression built for C# on: st, III 9, 2016, 12:03:57 dop.
        ///  Using Expresso Version: 3.0.3634, http://www.ultrapico.com
        ///  
        ///  A description of the regular expression:
        ///  
        ///  First or last character in a word
        ///  Any character in this class: [ab]
        ///  Any character in this class: [12], zero or one repetitions
        ///  First or last character in a word
        ///  
        ///
        /// </summary>
        private static Regex abRegex = new Regex(
            "^[ab][12]?\\b",
            RegexOptions.CultureInvariant
            | RegexOptions.Compiled
        );

        private static Regex slashAbRegex = new Regex(
            "/[ab][12]?\\b",
            RegexOptions.CultureInvariant
            | RegexOptions.Compiled
        );

        private void mdxrReader_ZnakovyStyl(object sender, TextRunEventArgs ev)
        {
            //ZnakovyStylImpl(ev);
            ZnakovyStylImplNew(ev);
        }

        private void ZnakovyStylImplNew(TextRunEventArgs ev)
        {
            if (_nastaveni.ZahrnoutTextOdstavce)
            {
                _textOdstavceSb.Append(ev.Text);
            }

            //mstrTextOdstavce += ev.Text;
            if (_nastaveni.ZahrnoutTextZnakovychStylu)
            {
                if (_predchoziUsekZnakovehoStylu == null)
                {
                    _predchoziUsekZnakovehoStylu = ev;
                }
                else
                {
                    if (ev.Style.ID == _predchoziUsekZnakovehoStylu.Style.ID)
                    {
                        _predchoziUsekZnakovehoStylu.Text += ev.Text;
                    }

                    else
                    {
                        ZpracovatZnakovyStylJakoJev(_predchoziUsekZnakovehoStylu);
                        _predchoziUsekZnakovehoStylu = ev;
                    }
                }
            }
        }

        private void ZpracovatZnakovyStylJakoJev(TextRunEventArgs ev)
        {
            //TODO Za jakých okonlností k tomu dochází?
            if (ev == null) return;

            if (ev.Text == null)
                ev.Text = String.Empty;
            string sText = _nastaveni.OdstranitPocatecniAKoncoveMezery ? ev.Text.Trim() : ev.Text;

            if (Nastaveni.StylyLokace.Contains(ev.Style.UIName))
            {
                if (_aktualniLokace == null)
                    _aktualniLokace = sText;
                else if (abRegex.IsMatch(sText) && slashAbRegex.IsMatch(_aktualniLokace))
                    _aktualniLokace = _aktualniLokace.Substring(0, _aktualniLokace.LastIndexOf("/") + 1) + sText;
                else
                    _aktualniLokace = sText;
            }

            //pokud se odstraňovaly počáteční a koncové mezery a text je prázdný, znamená to, že text obsahoval pouze mezery
            //v tom případě se do výsledků takový text zahrne (půjde o mezery, tabulátory)
            if (_nastaveni.OdstranitPocatecniAKoncoveMezery && sText.Length == 0 && ev.Text != null)
                sText = ev.Text;

            Jev jv = new Jev(ev.Language, sText, null);

            Jevy jvs = GetJevy(ev.Language, ev.Style.ID, ev.Style.UIName, TypJevu.Useky);
            //jvs.Add(jv);

            Jevy jevyOdstavce = GetJevy(_jevyAktualnihoOdstavce, _souborDocx,
                ev.Language, ev.Style.ID, ev.Style.UIName, TypJevu.Useky);
            jevyOdstavce.Add(jv);

            _usekyAktulanihoOdstravceInContinuo.Add(new Usek(ev.Style.UIName, ev.Text, _aktualniLokace));


            //jevyAktualnihoOdstavce.Add(jevyOdstavce);

            if (_nastaveni.ZahrnoutTextOdstavce)
            {
                string sOdstavecZnak = _aktualniOdstavecEventArgs.Style.ID + "+" + ev.Style.ID;
                string sOdstavecZnakID = Jevy.GetID(ev.Language, sOdstavecZnak);

                if (!_aktualniSkupinaJevu.ContainsID((sOdstavecZnakID)))
                {
                    jvs = new Jevy(TypJevu.Useky, _souborDocx, ev.Language, sOdstavecZnak);
                    jvs.Popis = _aktualniOdstavecEventArgs.Style.UIName + " + " + ev.Style.UIName;
                    _aktualniSkupinaJevu.Add(jvs);
                }

                _aktualniSkupinaJevu[sOdstavecZnakID].Add(jv.Clone());
            }
        }

        private void ZnakovyStylImpl(TextRunEventArgs ev)
        {
            if (_nastaveni.ZahrnoutTextOdstavce)
            {
                _textOdstavceSb.Append(ev.Text);
            }

            //mstrTextOdstavce += ev.Text;
            if (_nastaveni.ZahrnoutTextZnakovychStylu)
            {
                if (ev.Text == null)
                    ev.Text = String.Empty;
                string sText = _nastaveni.OdstranitPocatecniAKoncoveMezery ? ev.Text.Trim() : ev.Text;

                if (Nastaveni.StylyLokace.Contains(ev.Style.UIName))
                {
                    if (_aktualniLokace == null)
                        _aktualniLokace = sText;
                    else if (abRegex.IsMatch(sText) && slashAbRegex.IsMatch(_aktualniLokace))
                        _aktualniLokace = _aktualniLokace.Substring(0, _aktualniLokace.LastIndexOf("/") + 1) + sText;
                    else
                        _aktualniLokace = sText;
                }

                //pokud se odstraňovaly počáteční a koncové mezery a text je prázdný, znamená to, že text obsahoval pouze mezery
                //v tom případě se do výsledků takový text zahrne (půjde o mezery, tabulátory)
                if (_nastaveni.OdstranitPocatecniAKoncoveMezery && sText.Length == 0 && ev.Text != null)
                    sText = ev.Text;

                Jev jv = new Jev(ev.Language, sText, null);

                Jevy jvs = GetJevy(ev.Language, ev.Style.ID, ev.Style.UIName, TypJevu.Useky);
                //jvs.Add(jv);

                Jevy jevyOdstavce = GetJevy(_jevyAktualnihoOdstavce, _souborDocx,
                    ev.Language, ev.Style.ID, ev.Style.UIName, TypJevu.Useky);
                jevyOdstavce.Add(jv);

                _usekyAktulanihoOdstravceInContinuo.Add(new Usek(ev.Style.UIName, ev.Text, _aktualniLokace));


                //jevyAktualnihoOdstavce.Add(jevyOdstavce);

                if (_nastaveni.ZahrnoutTextOdstavce)
                {
                    string sOdstavecZnak = _aktualniOdstavecEventArgs.Style.ID + "+" + ev.Style.ID;
                    string sOdstavecZnakID = Jevy.GetID(ev.Language, sOdstavecZnak);

                    if (!_aktualniSkupinaJevu.ContainsID((sOdstavecZnakID)))
                    {
                        jvs = new Jevy(TypJevu.Useky, _souborDocx, ev.Language, sOdstavecZnak);
                        jvs.Popis = _aktualniOdstavecEventArgs.Style.UIName + " + " + ev.Style.UIName;
                        _aktualniSkupinaJevu.Add(jvs);
                    }

                    _aktualniSkupinaJevu[sOdstavecZnakID].Add(jv.Clone());
                }
            }
        }


        private void mdxrReader_ZacatekTabulky(object sender, TableEventArgs ev)
        {
            ZpracovatZnakovyStylJakoJevNaKonciUseku();
            //throw new NotImplementedException();
        }

        private void ZpracovatZnakovyStylJakoJevNaKonciUseku()
        {
            ZpracovatZnakovyStylJakoJev(_predchoziUsekZnakovehoStylu);
            _predchoziUsekZnakovehoStylu = null;
        }

        private void mdxrReader_ZacatekRadku(object sender, TableRowEventArgs ev)
        {
            ZpracovatZnakovyStylJakoJevNaKonciUseku();
            //throw new NotImplementedException();
        }

        private void mdxrReader_ZacatekBunky(object sender, TableCellEventArgs ev)
        {
            //throw new NotImplementedException();
            ZpracovatZnakovyStylJakoJevNaKonciUseku();
        }

        private void mdxrReader_Prubeh(object sender, ProgressEventArgs ev)
        {
            Progress handler = Prubeh;
            if (handler != null)
                handler(this, ev);
        }

        private void mdxrReader_Pismo(object sender, FontEventArgs ev)
        {
            //throw new NotImplementedException();
        }

        void mdxrReader_KonecPoznamkyPodCarou(object sender, FootnoteEventArgs ev)
        {
            ZpracovatZnakovyStylJakoJevNaKonciUseku();
            jePoznamkaPodCarou = false;
        }

        void mdxrReader_ZacatekPoznamkyPodCarou(object sender, FootnoteEventArgs ev)
        {
            ZpracovatZnakovyStylJakoJevNaKonciUseku();
            jePoznamkaPodCarou = true;
        }

        #endregion


        private Jevy GetJevy(string strLanguage, string strStyleID, string strStyleUIName, TypJevu tpTyp)
        {
            string sID = Jevy.GetID(strLanguage, strStyleID);
            Jevy jvs;
            if (!_aktualniSkupinaJevu.ContainsID(sID))
            {
                jvs = new Jevy(tpTyp, _souborDocx, strLanguage, strStyleID);
                jvs.Popis = strStyleUIName;
                _aktualniSkupinaJevu.Add(jvs);
                ConsoleColor color = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Yellow;

                Console.WriteLine("Vytvořena nová skupina jevů {0}, {1}, {2}", jvs.Popis, jvs.Jazyk, strStyleID);

                Console.BackgroundColor = color;
            }

            return _aktualniSkupinaJevu[sID];
        }

        private static Jevy GetJevy(SkupinaJevu skupina, string souborDocx,
            string strLanguage, string styleId, string styleUiName, TypJevu typJevu)
        {
            string sId = Jevy.GetID(strLanguage, styleId);
            if (!skupina.ContainsID(sId))
            {
                var jvs = new Jevy(typJevu, souborDocx, strLanguage, styleId, styleUiName);
                skupina.Add(jvs);
            }

            return skupina[sId];
        }

        public SkupinaJevu ZpracujDocx()
        {
            DocxReaderSettings readerSettings = new DocxReaderSettings();
            readerSettings.SkipFootnotesAndEndnotesContent = !Nastaveni.ZahrnoutPoznamkyPodCarou;

            _reader = new DocxReader(_souborDocx, readerSettings);

            _reader.ZacatekDokumentu += mdxrReader_ZacatekDokumentu;
            _reader.KonecDokumentu += mdxrReader_KonecDokumentu;

            _reader.ZacatekOdstavce += mdxrReader_ZacatekOdstavce;
            _reader.KonecOdstavce += mdxrReader_KonecOdstavce;
            _reader.ZnakovyStyl += mdxrReader_ZnakovyStyl;
            _reader.ZacatekTabulky += mdxrReader_ZacatekTabulky;
            _reader.ZacatekRadku += mdxrReader_ZacatekRadku;
            _reader.ZacatekBunky += mdxrReader_ZacatekBunky;
            _reader.ZacatekPoznamkyPodCarou += mdxrReader_ZacatekPoznamkyPodCarou;
            _reader.KonecPoznamkyPodCarou += mdxrReader_KonecPoznamkyPodCarou;
            _reader.Pismo += mdxrReader_Pismo;

            _reader.Prubeh += mdxrReader_Prubeh;
            _reader.Read();
            _reader = null;

            return _aktualniSkupinaJevu;
        }
    }
}