using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE2580 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
        List<string> messages = new List<string>();
            bool errorInPatch = false;

            #region Dataview changes
            foreach (XmlDocument currentDataview in dataviews)
            {
                string id = currentDataview.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : currentDataview.DocumentElement.Attributes["dataviewid"].Value;
                if (id.Equals("4003"))
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(currentDataview.NameTable);
                    manager.AddNamespace("COE", "COE.COEDataView");
                    XmlNodeList tablePermanentcompund = currentDataview.SelectNodes("//COE:table[@name='VW_BATCH']/COE:fields[@id]", manager);                  

                    XmlNode tablePermanentcompundNode = currentDataview.SelectSingleNode("//COE:table[@name='VW_BATCH']", manager);
                    XmlNode fieldAMOUNTUNITS = currentDataview.SelectSingleNode("//COE:table[@name='VW_BATCH']/COE:fields[@name='AMOUNT_UNITS']", manager);
                   
                    // For Table 9 in 4003 dataview xml
                    if (tablePermanentcompundNode != null)
                    {
                        
                        if (tablePermanentcompundNode.Attributes["name"].Value == "VW_BATCH")
                        {
                            foreach(XmlNode fieldNode in tablePermanentcompund)
                            {
                            #region iteration                            

                                XmlAttribute visibleAttribute = null;
                                if (fieldAMOUNTUNITS != null && fieldNode.Attributes["name"].Value == "AMOUNT_UNITS")
                                {
                                    XmlAttribute lookupFieldId = fieldNode.Attributes["lookupFieldId"];
                                    XmlAttribute lookupDisplayFieldId = fieldNode.Attributes["lookupDisplayFieldId"];
                                    if (lookupFieldId == null)
                                    {
                                        visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("lookupFieldId");
                                        fieldNode.Attributes.Append(visibleAttribute);
                                        fieldNode.Attributes["lookupFieldId"].Value = "1600";
                                        messages.Add("lookupFieldId Attribute is successfully added to field AMOUNT_UNITS");
                                    }
                                    else if (lookupFieldId.Value == "1100")
                                    {
                                        fieldNode.Attributes["lookupFieldId"].Value = "1600";
                                        messages.Add("lookupFieldId Attribute is Updated for field AMOUNT_UNITS");
                                    }
                                    else
                                    {
                                        messages.Add("lookupFieldId Attribute is already Updated for field AMOUNT_UNITS");
                                    }
                                    if (lookupDisplayFieldId == null)
                                    {
                                        visibleAttribute = fieldNode.OwnerDocument.CreateAttribute("lookupDisplayFieldId");
                                        fieldNode.Attributes.Append(visibleAttribute);
                                        fieldNode.Attributes["lookupDisplayFieldId"].Value = "1601";
                                        messages.Add("lookupDisplayFieldId Attribute is Successfully added to field AMOUNT_UNITS");
                                    }
                                    else if (lookupDisplayFieldId.Value == "1101")
                                    {
                                        fieldNode.Attributes["lookupDisplayFieldId"].Value = "1601";
                                        messages.Add("lookupDisplayFieldId Attribute is Updated for field AMOUNT_UNITS");
                                    }
                                    else
                                    {
                                        messages.Add("lookupDisplayFieldId Attribute is already Updated for field AMOUNT_UNITS");
                                    }
                                }
                            

                            #endregion
                             }
                        }
                        else
                        {
                            errorInPatch = true;
                            messages.Add(" Error: Table attribute VW_BATCH  was not found in 4003 Dataview ");
                        }
                    }
                    else
                    {
                        errorInPatch = true;
                        messages.Add("Error: Table attribute VW_BATCH  was not found in 4003 Dataview ");

                    }
               }
             }

            #endregion

            if (!errorInPatch)
                messages.Add("4003 Dataview was successfully updated for Upgrade machine.");
            else
                messages.Add("Error: 4003 Dataview was not successfully updated for Upgrade machine.");

            return messages;

        }
        #region Private Method
        private void createNewAttribute(string attributeName, string attributeValue, ref XmlNode refnode)
        {
            XmlAttribute attributes = refnode.OwnerDocument.CreateAttribute(attributeName);
            refnode.Attributes.Append(attributes);
            refnode.Attributes[attributeName].Value = attributeValue;
        }

        #endregion

	}
}
