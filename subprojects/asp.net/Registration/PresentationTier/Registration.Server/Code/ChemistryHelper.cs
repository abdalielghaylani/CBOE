using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class ChemistryHelper
    {
        private const string cdxMimeType = "chemical/x-cdx";
        private const string cdxmlMimeType = "text/xml";

        public static string ConvertAndName(string fromType, string toType, string fromData, ref string name, bool returnEmptyWhenEmptyStructure = false, bool useCachedControl = false)
        {
            if (!useCachedControl)
            {
                return COEChemDrawConverterUtils.ConvertStructure(fromData, fromType, toType);
            }

            // CacheableChemdrawControl may dispose automatically when timeout elapses.
            // In other words, it does not need to be disposed of explicitly in this code.
            // In order to support multiple versions of ChemDrawCtl, all calls to ChemDrawCtl are done dynamically.
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            dynamic cacheableChemDrawCtl = (object)CambridgeSoft.COE.Framework.Caching.CacheableChemdrawControl.GetCachedChemdrawControl(assemblyName);
            dynamic chemDrawCtl = cacheableChemDrawCtl.Control;
            dynamic chemDrawObjects = chemDrawCtl.Objects;
            chemDrawObjects.Clear();
            chemDrawCtl.DataEncoded = true;
            chemDrawCtl.set_Data(fromType, fromData);
            if (name != null)
                name = chemDrawCtl.get_Data("chemical/x-name");
            if (returnEmptyWhenEmptyStructure && string.IsNullOrEmpty(chemDrawObjects.Formula)) return string.Empty;
            return chemDrawCtl.get_Data(toType);
        }

        public static string Convert(string fromType, string toType, string fromData, bool returnEmptyWhenEmptyStructure = false, bool useCachedControl = false)
        {
            string name = null;
            return ConvertAndName(fromType, toType, fromData, ref name, returnEmptyWhenEmptyStructure, useCachedControl);
        }

        public static string ConvertToCdxml(string data, bool returnEmptyWhenEmptyStructure = false)
        {
            return Convert(cdxMimeType, cdxmlMimeType, data, returnEmptyWhenEmptyStructure, true);
        }

        public static string ConvertToCdxmlAndName(string data, ref string name, bool returnEmptyWhenEmptyStructure = false)
        {
            return ConvertAndName(cdxMimeType, cdxmlMimeType, data, ref name, returnEmptyWhenEmptyStructure);
        }

        public static string ConvertToCdx(string data, bool returnEmptyWhenEmptyStructure = false)
        {
            return Convert(cdxmlMimeType, cdxMimeType, data, returnEmptyWhenEmptyStructure);
        }

        public static string ConvertToCdxAndName(string data, ref string name, bool returnEmptyWhenEmptyStructure = false)
        {
            return ConvertAndName(cdxmlMimeType, cdxMimeType, data, ref name, returnEmptyWhenEmptyStructure);
        }

        private static void ConvertStructuresToCdxml(XmlElement element, bool returnEmptyWhenEmptyStructure = false)
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
                var converted = Convert(cdxMimeType, cdxmlMimeType, textData, returnEmptyWhenEmptyStructure);
                element.InnerText = converted.Replace("\r\n", " ");
            }
        }

        private static void ConvertStructuresToCdx(XmlElement element, bool returnEmptyWhenEmptyStructure = false, bool useCachedControl = false)
        {
            if (element == null) return;
            foreach (var childElement in element.ChildNodes)
            {
                if (childElement is XmlElement)
                    ConvertStructuresToCdx(childElement as XmlElement);
            }

            var textData = element.InnerText;
            if (!string.IsNullOrEmpty(textData) && (textData.StartsWith("<?xml ") || textData.StartsWith("<CDXML ")))
            {
                var converted = Convert(cdxmlMimeType, cdxMimeType, textData, returnEmptyWhenEmptyStructure, useCachedControl);
                element.InnerText = converted.Replace("\r\n", " ");
            }
        }

        public static XmlDocument ConvertStructuresToCdxml(XmlDocument doc, bool returnEmptyWhenEmptyStructure = false)
        {
            ConvertStructuresToCdxml(doc.DocumentElement, returnEmptyWhenEmptyStructure);
            return doc;
        }

        public static XmlDocument ConvertStructuresToCdx(XmlDocument doc, bool returnEmptyWhenEmptyStructure = false, bool useCachedControl = false)
        {
            ConvertStructuresToCdx(doc.DocumentElement, returnEmptyWhenEmptyStructure, useCachedControl);
            return doc;
        }
    }
}