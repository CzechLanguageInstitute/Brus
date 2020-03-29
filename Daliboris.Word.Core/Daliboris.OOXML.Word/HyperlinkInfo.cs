using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Daliboris.OOXML.Word
{
    public class HyperlinkInfo
    {
        public string Id { get; set; }
        public string Target { get; set; }
        public string TargetMode { get; set; }

        public HyperlinkInfo(string id, string target, string targetMode)
        {
            Id = id;
            Target = target;
            TargetMode = targetMode;
        }

        public HyperlinkInfo()
        {
        }

        public static Dictionary<string, HyperlinkInfo> ReadHyperlinksInfo(string file)
        {
            XDocument relations = XDocument.Load(file);
            XNamespace ns = "http://schemas.openxmlformats.org/package/2006/relationships";
            Dictionary < string, HyperlinkInfo> info =
                (from rel in
                relations.Descendants(ns + "Relationship")
                    .Where(
                        r =>
                            r.Attribute("Type").Value ==
                            "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink")
                select new HyperlinkInfo(rel.Attribute("Id").Value, rel.Attribute("Target").Value, rel.Attribute("TargetMode").Value)).ToDictionary(k => k.Id, v => v);
            return info;
        }
    }

    
}