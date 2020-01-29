using System;
using System.Xml.Serialization;

namespace Daliboris.Statistiky
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true,
        Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd", TypeName = "jevy")]
    [XmlRoot(Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd",
        IsNullable = false)]
    // Přehled zaznamenaných jevů
    public class JazykovéJevy
    {

        /// <remarks/>
        [XmlElement("j")]
        public JazykovyJev[] Value { get; set; }
        
        /// <remarks/>
        [XmlAttribute("pocet", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Pocet { get; set; }

        /// <remarks/>
        [XmlAttribute("popis")]
        public string Popis { get; set; }

        /// <remarks/>
        /// Identifikátor (např. styl) použitý pro identifikaci jevu
        [XmlAttribute("identifikator")]
        public string Identifikator { get; set; }

        /// <remarks/>
        [XmlAttribute("vytvoreno")]
        public DateTime Vytvoreno { get; set; }
        
        /// <remarks/>
        [XmlAttribute("zdroj")]
        public string Zdroj { get; set; }
        
        /// <remarks/>
        [XmlAttribute("jazyk", DataType = "language")]
        public string Jazyk { get; set; }

        /// <remarks/>
        [XmlAttribute("typ")]
        public TypJevu TypJevu { get; set; }

    }
}