// -----------------------------------------------------------------------
// <copyright file="NunitUtil.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CambridgeSoft.COE.Framework.Common.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NunitUtil
    {
        public static void AddInstanceTag(string xmlPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            var instanceNodes = document.DocumentElement.SelectNodes("/configuration/coeConfiguration/instances");
            if (instanceNodes.Count == 0)
            {
                var configurationNode = document.DocumentElement.SelectSingleNode("/configuration/coeConfiguration");
                if (configurationNode != null)
                {
                    XmlElement node = (XmlElement)document.CreateElement("instances");
                    node.InnerXml = "<add id=\"bf124610-4d77-47ce-966f-ac2a365c58a7\" name=\"MAIN\" dbmsType=\"ORACLE\" databaseGlobalUser=\"COEUSER\" isCBOEInstance=\"true\" useProxy=\"true\" hostName=\"localhost\" port=\"1521\" sid=\"orcl\"><sqlGeneratorData><add name=\"CSORACLECARTRIDGE\" schema=\"CSCARTRIDGE\" tempQueriesTableName=\"TempQueries\" molFileFormat=\"V2000\" /></sqlGeneratorData></add>";
                    configurationNode.AppendChild(node);
                }
            }
            document.Save(xmlPath);
        }
    }
}
