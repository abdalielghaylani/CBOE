using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE8611 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string coeRootPath = string.Empty;
            string coeDataViewPath = string.Empty;
            #region DataView
            foreach (XmlDocument dataviewDoc in dataviews)
            {
                string id = dataviewDoc.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : dataviewDoc.DocumentElement.Attributes["dataviewid"].Value;

                if (id == "4003")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(dataviewDoc.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    coeRootPath = "//COE:tables";
                    coeDataViewPath = "//COE:table[@id='16']";  // Travel to root node
                    XmlNode rootNode = dataviewDoc.SelectSingleNode(coeRootPath, manager);
                    XmlNode tableVwUnit = rootNode.SelectSingleNode(coeDataViewPath, manager);

                    string vw_Unit_Xml = @"<table id=""16"" name=""VW_UNIT"" alias=""VW_UNIT"" database=""REGDB"" primaryKey=""1600"">
      <tags/>
      <fields id=""1600"" name=""ID"" dataType=""INTEGER"" alias=""UNIT_ID"" indexType=""NONE"" mimeType=""NONE"" visible=""true""/>
      <fields id=""1601"" name=""UNIT"" dataType=""TEXT"" alias=""UNIT_VALUE"" indexType=""NONE"" mimeType=""NONE"" visible=""true""/>
    </table>";
                    if (tableVwUnit != null)
                    {
                        messages.Add("VW_UNIT table already exists in 4003 Dataview");
                        errorsInPatch = true;
                    }
                    else
                    {
                        rootNode.InnerXml += vw_Unit_Xml;
                        messages.Add("VW_UNIT table with ID 16 is appended to 4003 Dataview successfully");
                    }
                }
            }

            #endregion

            if (!errorsInPatch)
                messages.Add("Patch was successfully applied.");
            else
                messages.Add("Patch was not applied as the table already exists");
            return messages;

        }
    }
}
