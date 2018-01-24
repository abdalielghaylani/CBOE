using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace CambridgeSoft.COE.Registration.Core
{
    class BaseUtilities
    {
        /// <summary>
        /// If an object and its tree is entirely Serializable, this mechanism will convert
        /// that object into its XML representation.
        /// </summary>
        /// <param name="target">the Object to convett to xml</param>
        /// <returns>an xml string representing the Serializable Object instance</returns>
        internal static string ToXml(object target)
        {
            string buf = null;

            Type t = target.GetType();
            XmlSerializer xs = new XmlSerializer(t);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            writerSettings.Indent = true;
            StringWriter stringWriter = new StringWriter();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
            {
                xs.Serialize(xmlWriter, target, ns);
            }
            buf = stringWriter.ToString();

            return buf;
        }

    }
}
