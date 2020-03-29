using System.Xml.Serialization;
using Daliboris.Statistiky.Core.Models.Jevy.XSD;

namespace Daliboris.Statistiky
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "detaily",AnonymousType = true,
        Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd",
        IsNullable = false)]
    public class Detaily
    {
        /// <remarks/>
        [XmlElement("d")]
        public Detail[] Value { get; set; }
    }
}