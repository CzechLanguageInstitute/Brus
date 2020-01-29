using System.Xml.Schema;
using System.Xml.Serialization;

namespace Daliboris.Statistiky
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute("j",AnonymousType = true,
        Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd",
        IsNullable = false)]
    public class JazykovyJev
    {
        /// <remarks/>
        [XmlElement("o")]
        public object Obsah { get; set; }
        

        /// <remarks/>
        [XmlArrayItem("d", IsNullable = false)]
        public Detaily[] Detaily { get; set; }


        /// <remarks/>
        public Lokace LokaceJevu { get; set; }


        /// <remarks/>
        [XmlAttribute("n")]
        public string NazevJevu { get; set; }
        

        /// <remarks/>
        [XmlAttribute("p", Form = XmlSchemaForm.Unqualified)]
        // Počet výskytů jednotlivých charakteristik (jevů, detailů)
        public int PocetCharakteristik { get; set; }
        

        /// <remarks/>
        /// Retrográdní podoba textu (názvu); používá se většinou jenom u slova
        [XmlAttribute("r")]
        public string RetrográdníPodobaTextu { get; set; }
        
    }
}