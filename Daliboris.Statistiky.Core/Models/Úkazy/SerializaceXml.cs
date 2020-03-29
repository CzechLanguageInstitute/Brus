using System.Xml.Serialization;
using System.Text;
using System.Xml;

namespace Daliboris.Statistiky
{
    public static class SerializaceXml
    {
        public static void Serializuj(Ukaz item, string strSoubor)
        {
            Encoding encKodovani = Encoding.UTF8;
            bool blnOdsazeni = false;
            XmlSerializerNamespaces xsnJmenneProstory = new XmlSerializerNamespaces();

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = encKodovani;
            xws.Indent = blnOdsazeni;
            XmlSerializer xs = null;
            XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
            if (xsnJmenneProstory == null)
                xsnJmenneProstory = new XmlSerializerNamespaces();

            xs = new XmlSerializer(item.GetType());
            xs.Serialize(xw, item, xsnJmenneProstory);
            xw.Close();
            xw = null;
            xs = null;
        }

        public static void Serializuj(Ukazy item, string strSoubor)
        {
            Encoding encKodovani = Encoding.UTF8;
            bool blnOdsazeni = false;
            XmlSerializerNamespaces xsnJmenneProstory = new XmlSerializerNamespaces();

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = encKodovani;
            xws.IndentChars = " ";
            xws.Indent = blnOdsazeni;
            XmlSerializer xs = null;
            XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
            if (xsnJmenneProstory == null)
                xsnJmenneProstory = new XmlSerializerNamespaces();

            xs = new XmlSerializer(item.GetType());
            xs.Serialize(xw, item, xsnJmenneProstory);
            xw.Close();
            xw = null;
            xs = null;
        }

        public static Ukazy Deserializuj(string strSoubor)
        {
            Encoding encKodovani = Encoding.UTF8;
            bool blnOdsazeni = false;
            XmlSerializerNamespaces xsnJmenneProstory = new XmlSerializerNamespaces();

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = encKodovani;
            xws.IndentChars = " ";
            xws.Indent = blnOdsazeni;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(strSoubor, encKodovani))
            {
                using (XmlTextReader xr = new XmlTextReader(sr))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Ukazy));
                    Ukazy ukz = (Ukazy) xs.Deserialize(xr);
                    return ukz;
                }
            }
        }
    }
}