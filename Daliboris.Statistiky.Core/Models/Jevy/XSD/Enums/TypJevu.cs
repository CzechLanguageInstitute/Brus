using System.Xml.Serialization;

namespace Daliboris.Statistiky
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [XmlTypeAttribute(TypeName = "jevyTyp", AnonymousType = true, Namespace = "http://www.daliboris.cz/schemata/statistiky.xsd")]
    public enum TypJevu
    {
        /// <remarks/>
        [XmlEnum(Name = "ostatni")]
        Ostatni,

        /// <remarks/>
        [XmlEnum(Name = "useky")]
        Useky,

        [XmlEnum(Name = "odstavce")]
        Odstavce,

        [XmlEnum(Name = "slova")]
        Slova,

        [XmlEnum(Name = "znaky")]
        Znaky,

        [XmlEnum(Name = "ngramy")]
        NGramy,

        [XmlEnum(Name = "tagy")]
        Tagy,

        [XmlEnum(Name = "atributy")]
        Atributy,

        [XmlEnum(Name = "hodnoty")]
        Hodnoty,
    }
}