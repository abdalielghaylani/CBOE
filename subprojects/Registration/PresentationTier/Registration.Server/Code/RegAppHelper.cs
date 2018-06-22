using System.Collections.Generic;
using System.Text.RegularExpressions;
using CambridgeSoft.COE.Framework.Common;
using PerkinElmer.COE.Registration.Server.Models;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class RegAppHelper
    {
        public static List<SettingData> RetrieveSettings()
        {
            var settingList = new List<SettingData>();
            var currentApplicationName = RegUtilities.GetApplicationName();
            var appConfigSettings = FrameworkUtils.GetAppConfigSettings(currentApplicationName, true);
            var groups = appConfigSettings.SettingsGroup;
            foreach (var group in groups)
            {
                var settings = group.Settings;
                foreach (var setting in settings)
                {
                    if (!setting.IsHidden.ToLower().Equals(bool.TrueString.ToLower()))
                    {
                        if (setting.IsHidden.ToLower().Equals(bool.TrueString.ToLower()))
                            continue;

                        if ((setting.IsAdmin.Equals(bool.TrueString) && CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetManageConfigurationSettings())
                        || !setting.IsAdmin.Equals(bool.TrueString))
                        {
                            settingList.Add(new SettingData(group, setting));
                        }
                    }  
                }
            }
            return settingList;
        }

        public static string ExtractHtmlInnerText(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
                return string.Empty;

            // Match any Html tag (opening or closing tags) 
            // followed by any successive whitespaces
            // consider the Html text as a single line
            Regex regex = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);
            // replace all html tags (and consequtive whitespaces) by spaces
            // trim the first and last space
            return regex.Replace(htmlText, " ").Trim();
        }

        /// <summary>
        /// Transform the given XML to pretty printed format
        /// </summary>
        /// <param name="xmlInput">xmlInput is a string that contains xml</param>
        /// <returns></returns>
        public static string TransformToPrettyPrintXML(string xmlInput)
        {
            StringBuilder xsltInput = new StringBuilder();
            xsltInput.Append("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            xsltInput.Append("<xsl:output omit-xml-declaration=\"yes\" indent=\"yes\"/>");
            xsltInput.Append("<xsl:template match=\"node()|@*\">");
            xsltInput.Append("<xsl:copy>");
            xsltInput.Append("<xsl:apply-templates select=\"node()|@*\"/>");
            xsltInput.Append("</xsl:copy>");
            xsltInput.Append("</xsl:template>");
            xsltInput.Append("</xsl:stylesheet>");

            string output = string.Empty;
            using (StringReader srt = new StringReader(xsltInput.ToString())) 
            using (StringReader sri = new StringReader(xmlInput))
            {
                using (XmlReader xrt = XmlReader.Create(srt))
                using (XmlReader xri = XmlReader.Create(sri))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    xslt.Load(xrt);
                    using (StringWriter sw = new StringWriter())
                    using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                    {
                        xslt.Transform(xri, xwo);
                        output = sw.ToString();
                    }
                }
            }
            return output;
        }
    }
}