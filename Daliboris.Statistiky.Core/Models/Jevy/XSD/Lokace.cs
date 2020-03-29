using System.Xml.Serialization;

namespace Daliboris.Statistiky
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(TypeName = "lokace", AnonymousType = true,
        Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd")]
    [XmlRootAttribute(Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd",
        IsNullable = false)]
    // Lokace, seznam likací, kde se jev nachází
    public class Lokace
    {
    
        /// <remarks/>
        [XmlElement("l", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LokaceVyskytu[] LokaceVyskytu { get; set; }
        
        /// <remarks/>
        [XmlAttribute("p", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        // Počet výskytů jednotlivých charakteristik (jevů, detailů)
        public int PočetVýskytů { get; set; }

    }
}