using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace RegistrationAdmin.Services.MSUnitTests.Helpers
{
    class XMLDataLoader
    {
        const string xmlFileLocation = @"Registration.MSUnitTests\Registration.Admin\XmlData\";
        const string configurationSettingsFileName = @"Registration.MSUnitTests\Xml_Files\ConfigurationSettings.xml";


        public static XmlDocument LoadXmlDocument(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                XmlDocument doc = new XmlDocument();
                string xmlfilename = GetXMLFilePath() + xmlFileLocation + fileName;
                doc.Load(xmlfilename);
                return doc;
            }
            return null;
        }

        public static XmlDocument LoadConfigurationSettings()
        {
            if (!string.IsNullOrEmpty(configurationSettingsFileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(GetXMLFilePath() + configurationSettingsFileName);
                return doc;
            }
            return null;
        }

        static string GetXMLFilePath()
        {
            string filePath = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("TestResults", StringComparison.OrdinalIgnoreCase));
            return filePath;
        }

        public static XmlNodeList PrepareXMLNodeList(XmlDocument doc)
        {
            if (doc != null)
            {
                XmlNodeList xmlNodeList = doc.DocumentElement.ChildNodes;
                return xmlNodeList;
            }
            return null;
        }
    }
}
