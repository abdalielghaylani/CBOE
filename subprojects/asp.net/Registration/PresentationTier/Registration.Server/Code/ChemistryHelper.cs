using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class ChemistryHelper
    {
        private const string cdxMimeType = "chemical/x-cdx";
        private const string cdxmlMimeType = "text/xml";

        public static string Convert(string fromType, string toType, string fromData)
        {
            // CacheableChemdrawControl may dispose automatically when timeout elapses.
            // In other words, it does not need to be disposed of explicitly in this code.
            // In order to support multiple versions of ChemDrawCtl, all calls to ChemDrawCtl are done dynamically.
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            dynamic cacheableChemDrawCtl = (object)CambridgeSoft.COE.Framework.Caching.CacheableChemdrawControl.GetCachedChemdrawControl(assemblyName);
            dynamic chemDrawCtl = cacheableChemDrawCtl.Control;
            dynamic cdObjects = chemDrawCtl.Objects;
            cdObjects.Clear();
            chemDrawCtl.DataEncoded = true;
            chemDrawCtl.set_Data(fromType, fromData);
            return chemDrawCtl.get_Data(toType);
        }

        public static string ConvertToCdxml(string data)
        {
            return Convert(cdxMimeType, cdxmlMimeType, data);
        }

        public static string ConvertToCdx(string data)
        {
            return Convert(cdxmlMimeType, cdxMimeType, data);
        }

        private static void ConvertStructuresToCdxml(XmlElement element)
        {
            if (element == null) return;
            foreach (var childElement in element.ChildNodes)
            {
                if (childElement is XmlElement)
                    ConvertStructuresToCdxml(childElement as XmlElement);
            }
            var textData = element.InnerText;
            if (!string.IsNullOrEmpty(textData) && textData.StartsWith("VmpD"))
            {
                var converted = Convert(cdxMimeType, cdxmlMimeType, textData);
                element.InnerText = converted.Replace("\r\n", " ");
            }
        }

        private static void ConvertStructuresToCdx(XmlElement element)
        {
            if (element == null) return;
            foreach (var childElement in element.ChildNodes)
            {
                if (childElement is XmlElement)
                    ConvertStructuresToCdx(childElement as XmlElement);
            }
            var textData = element.InnerText;
            if (!string.IsNullOrEmpty(textData) && textData.StartsWith("<CDXML "))
            {
                var converted = Convert(cdxmlMimeType, cdxMimeType, textData);
                element.InnerText = converted.Replace("\r\n", " ");
            }
        }

        public static XmlDocument ConvertStructuresToCdxml(XmlDocument doc)
        {
            ConvertStructuresToCdxml(doc.DocumentElement);
            return doc;
        }

        public static XmlDocument ConvertStructuresToCdx(XmlDocument doc)
        {
            ConvertStructuresToCdx(doc.DocumentElement);
            return doc;
        }
    }
}