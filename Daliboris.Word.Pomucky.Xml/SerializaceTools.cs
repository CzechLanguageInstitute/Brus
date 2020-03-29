using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Daliboris.Pomucky.Xml
{
    public static class SavingTools
    {
        public static void UlozitDoXml(object objekt, string strSoubor, Encoding encKodovani, bool blnOdsazeni, XmlSerializerNamespaces xsnJmenneProstory)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.CloseOutput = true;
            xmlWriterSettings.Encoding = encKodovani;
            xmlWriterSettings.IndentChars = " ";
            xmlWriterSettings.Indent = blnOdsazeni;
            var xmlWriter = XmlWriter.Create(strSoubor, xmlWriterSettings);
            if (xsnJmenneProstory == null)
                xsnJmenneProstory = new XmlSerializerNamespaces();
            
            //xsn.Add(String.Empty, String.Empty);
            //xsn.Add(String.Empty, csNamespace);

            var xmlSerializer = new XmlSerializer(objekt.GetType());
            xmlSerializer.Serialize(xmlWriter, objekt, xsnJmenneProstory);
            xmlWriter.Close();
            xmlWriter = null;
            xmlSerializer = null;
        }


        public static void UlozitDoXml(object objekt, string strSoubor,
            string strVychoziNamespace, Encoding encKodovani, bool blnOdsazeni)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = encKodovani;
            xws.IndentChars = " ";
            xws.Indent = blnOdsazeni;
            XmlSerializer xs = null;
            XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();

            if (strVychoziNamespace != null)
            {
                xs = new XmlSerializer(objekt.GetType(), strVychoziNamespace);
            }
            else
            {
                xs = new XmlSerializer(objekt.GetType());
            }

            xs.Serialize(xw, objekt, xsn);
            xw.Close();
            xw = null;
            xs = null;
        }

        public static void UlozitDoXml(object objekt, string strSoubor, XmlSerializerNamespaces xsnJmenneProstory)
        {
            UlozitDoXml(objekt, strSoubor, Encoding.UTF8, false, xsnJmenneProstory);
        }

        public static void UlozitDoXml(object objekt, string strSoubor)
        {
            UlozitDoXml(objekt, strSoubor, null, Encoding.UTF8, false);
        }

        public static void UlozitDoXml(object objekt, string strSoubor, bool blnOdsazeni)
        {
            UlozitDoXml(objekt, strSoubor, null, Encoding.UTF8, false);
        }

        public static object NacistZXml(string strSoubor, string strVychoziNamespace, Type typ)
        {
            if (!File.Exists(strSoubor))
            {
                throw new FileNotFoundException("Uvedený soubor '" + strSoubor + "' neexistuje");
            }

            XmlSerializer xs = null;
            if (strVychoziNamespace != null)
                xs = new XmlSerializer(typ, strVychoziNamespace);
            else
                xs = new XmlSerializer(typ);
            FileStream fs = new FileStream(strSoubor, FileMode.Open, FileAccess.Read);
            XmlReader reader = XmlReader.Create(fs);
            object objekt = xs.Deserialize(reader);
            fs.Close();
            return objekt;
        }

        public static object NacistZXml(string strSoubor, Type typ)
        {
            return NacistZXml(strSoubor, null, typ);
        }
    }
}