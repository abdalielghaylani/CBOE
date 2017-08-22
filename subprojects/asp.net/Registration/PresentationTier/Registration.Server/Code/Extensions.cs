using CambridgeSoft.COE.Registration.Services.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class Extensions
    {
        public static void UpdateFromXmlEx(this RegistryRecord record, string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var rootNode = doc.DocumentElement;

            // RegistryRecord itself only update properties that are allowed to be updated not auto-generated fields like person created
            var matchingChild = rootNode.SelectSingleNode("SubmissionComments");
            if (matchingChild != null && !string.IsNullOrEmpty(matchingChild.InnerText) && (record.SubmissionComments != matchingChild.InnerText))
                record.SubmissionComments = matchingChild.InnerText;

            record.PropertyList.UpdateFromXmlEx(rootNode.SelectSingleNode("PropertyList"));
            record.ProjectList.UpdateFromXmlEx(rootNode.SelectSingleNode("ProjectList"));
            record.IdentifierList.UpdateFromXmlEx(rootNode.SelectSingleNode("IdentifierList"));
            record.BatchList.UpdateFromXmlEx(rootNode.SelectSingleNode("BatchList"));
            record.ComponentList.UpdateFromXmlEx(rootNode.SelectSingleNode("ComponentList"));
        }

        public static void UpdateFromXmlEx(this PropertyList list, XmlNode dataNode)
        {
            if (dataNode == null) return;

        }

        public static void UpdateFromXmlEx(this ProjectList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
        }

        public static void UpdateFromXmlEx(this IdentifierList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
        }

        public static void UpdateFromXmlEx(this BatchList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
        }

        public static void UpdateFromXmlEx(this ComponentList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
        }
    }
}