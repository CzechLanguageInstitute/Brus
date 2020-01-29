using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Daliboris.Statistiky.Core.Services
{
    /// <summary>
    /// The usage of the XmlSerializerCache should be pretty straightforward:
    /// XmlSerializer serializer = XmlSerializerCache.GetXmlSerializer(typeof(Order), "http://tempuri.org/");
    /// serializer.Serialize(stream, order);
    /// </summary>
    public class XmlSerializerCache
    {
        #region fields

        private static Hashtable table;

        #endregion // fields

        #region constructors

        static XmlSerializerCache()
        {
            XmlSerializerCache.table = new Hashtable();
        }

        private XmlSerializerCache()
        {
        }

        #endregion // constructors

        public static XmlSerializer GetXmlSerializer(Type type, string defaultNamespace)
        {
            XmlSerializer serializer;
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // Make it thread safe
            lock (XmlSerializerCache.table.SyncRoot)
            {
                string typeName;
                if ((defaultNamespace == null) || (defaultNamespace.Length == 0))
                {
                    typeName = type.FullName + "#";
                }
                else
                {
                    typeName = type.FullName + "#" + defaultNamespace;
                }

                // Try to get the serializer from cache
                object obj = XmlSerializerCache.table[typeName];
                if (obj == null)
                {
                    // We don't have it, create a new instance
                    obj = new XmlSerializer(type, defaultNamespace);
                    XmlSerializerCache.table.Add(typeName, obj);
                }

                serializer = obj as XmlSerializer;
            }

            return serializer;
        }
    }


    public class XmlSerializace<T>
    {
        #region Deserializace

        public static T Deserialize(string xmlFilePath, string defaultNamespace)
        {
            using (FileStream stream = new FileStream(xmlFilePath, FileMode.Open))
            {
                return Deserialize(stream, defaultNamespace);
            }
        }

        public static T Deserialize(string xmlFilePath)
        {
            return Deserialize(xmlFilePath, null);
        }

        public static T Deserialize(Stream xmlFileStream, string defaultNamespace)
        {
            if (defaultNamespace == null)
                return (T) Serializer(typeof(T)).Deserialize(xmlFileStream);
            else
                return (T) Serializer(typeof(T), defaultNamespace).Deserialize(xmlFileStream);
        }

        public static T Deserialize(Stream xmlFileStream)
        {
            return Deserialize(xmlFileStream, null);
        }

        public static T Deserialize(TextReader textReader)
        {
            return (T) Serializer(typeof(T)).Deserialize(textReader);
        }

        public static T Deserialize(XmlReader xmlReader)
        {
            return (T) Serializer(typeof(T)).Deserialize(xmlReader);
        }

        public static T Deserialize(XmlReader xmlReader, string encodingStyle)
        {
            return (T) Serializer(typeof(T)).Deserialize(xmlReader, encodingStyle);
        }

        public static T Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
        {
            return (T) Serializer(typeof(T)).Deserialize(xmlReader, events);
        }

        public static T Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events)
        {
            return (T) Serializer(typeof(T)).Deserialize(xmlReader, encodingStyle, events);
        }

        private static XmlSerializer _Serializer = null;

        private static XmlSerializer Serializer(Type t)
        {
            if (_Serializer == null)
                _Serializer = new XmlSerializer(t);
            return _Serializer;
        }

        private static XmlSerializer Serializer(Type t, string defaultNamespace)
        {
            if (_Serializer == null)
                _Serializer = new XmlSerializer(t, defaultNamespace);
            return _Serializer;
        }

        private static XmlWriterSettings _XmlWriterSettings = null;

        public static XmlWriterSettings DefaultXmlWriterSettings
        {
            get
            {
                if (_XmlWriterSettings == null)
                {
                    _XmlWriterSettings = new XmlWriterSettings();
                    _XmlWriterSettings.Encoding = System.Text.Encoding.UTF8;
                    _XmlWriterSettings.Indent = true;
                    _XmlWriterSettings.IndentChars = " ";
                    _XmlWriterSettings.CloseOutput = true;
                    return _XmlWriterSettings;
                }
                else
                    return _XmlWriterSettings;
            }
        }

        #endregion

        #region Serializace

        public static void Serialize(string strPath, T o, string defaultNamespace)
        {
            XmlWriter xw = XmlWriter.Create(strPath, DefaultXmlWriterSettings);
            Serialize(xw, o, defaultNamespace);
            xw.Close();
        }

        public static void Serialize(string strPath, T o)
        {
            XmlWriter xw = XmlWriter.Create(strPath, DefaultXmlWriterSettings);
            Serialize(xw, o);
            xw.Close();
        }

        public static void Serialize(Stream stream, T o)
        {
            Serializer(typeof(T)).Serialize(stream, o);
        }

        public static void Serialize(Stream stream, T o, string defaultNamespace)
        {
            XmlSerializerNamespaces xsnNamespaces = new XmlSerializerNamespaces();
            xsnNamespaces.Add("", defaultNamespace);
            Serializer(typeof(T)).Serialize(stream, o, xsnNamespaces);
        }

        public static void Serialize(Stream stream, T o, XmlSerializerNamespaces namespaces)
        {
            Serializer(typeof(T)).Serialize(stream, o, namespaces);
        }

        public static void Serialize(TextWriter textWriter, T o)
        {
            Serializer(typeof(T), null).Serialize(textWriter, o);
        }

        public static void Serialize(TextWriter textWriter, T o, string defaultNamespace)
        {
            Serializer(typeof(T), defaultNamespace).Serialize(textWriter, o);
        }

        public static void Serialize(TextWriter textWriter, T o, XmlSerializerNamespaces namespaces)
        {
            Serializer(typeof(T)).Serialize(textWriter, o, namespaces);
        }

        public static void Serialize(XmlWriter xmlWriter, T o, string defaultNamespace)
        {
            Serializer(typeof(T), defaultNamespace).Serialize(xmlWriter, o);
        }

        public static void Serialize(XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces)
        {
            Serializer(typeof(T)).Serialize(xmlWriter, o, namespaces);
        }

        public static void Serialize(XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, string encodingStyle)
        {
            Serializer(typeof(T)).Serialize(xmlWriter, o, namespaces, encodingStyle);
        }

        public static void Serialize(XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, string encodingStyle,
            string id)
        {
            Serializer(typeof(T)).Serialize(xmlWriter, o, namespaces, encodingStyle, id);
        }

        public static void Serialize(XmlWriter xmlWriter, T o)
        {
            Serializer(typeof(T)).Serialize(xmlWriter, o);
        }

        #endregion
    }
}