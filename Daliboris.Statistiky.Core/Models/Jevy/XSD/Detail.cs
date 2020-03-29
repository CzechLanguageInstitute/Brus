using System.Xml.Serialization;

namespace Daliboris.Statistiky.Core.Models.Jevy.XSD
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(TypeName = "d", AnonymousType = true,
        Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd")]
    [XmlRoot(Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd",
        IsNullable = false)]
    public class Detail
    {
        /// <remarks/>
        [XmlAttribute("pocet", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Pocet { get; set; }

        /// <remarks/>
        [XmlAttribute("popis")]
        public string Popis { get; set; }


        /// <remarks/>
        [XmlAttribute("identifikator")]
        public string Identifikator { get; set; }
    }
}