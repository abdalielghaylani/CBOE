using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 133757: RLS=Off, the temp search didn' take effect for the registry project query condition
    /// </summary>
    class CSBR133757 : BugFixBaseCommand
    {

        /// <summary>
        /// Steps to fix manually:
        /// 1 - Open dataview 4002 and add de following table:
        ///&lt;table id="7" name="VW_TEMPORARYREGNUMBERSPROJECT" alias="VW_TEMPORARYREGNUMBERSPROJECT" database="REGDB" primaryKey="700"&gt;
        ///	&lt;fields id="700" name="ID" dataType="INTEGER" alias="ID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        ///	&lt;fields id="701" name="TEMPBATCHID" dataType="INTEGER" alias="TEMPBATCHID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        ///	&lt;fields id="702" name="PROJECTID" dataType="INTEGER" lookupFieldId="500" lookupDisplayFieldId="501" alias="PROJECTID" indexType="NONE" mimeType="NONE" visible="1"/&gt;
        ///&lt;/table&gt;
        ///
        /// 2 - In the same dataview add the relationship: &lt;relationship child="7" parent="1" childkey="701" parentkey="100" jointype="INNER"/&gt;
        /// 3 - Open the COEFormGroup 4002 and search the searchCriteria id 30 definition and change fieldId with 702 and table id with 7
        /// </summary>        
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4002")
                {
                    XmlNode searchCriteriaToModify = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='REGISTRY_PROJECT']/COE:searchCriteriaItem", manager);

                    if (searchCriteriaToModify == null)
                    {
                        messages.Add("The expected searchCriteriaItem was not found");
                        errorsInPatch = true;
                    }
                    else
                    {
                        searchCriteriaToModify.Attributes["fieldid"].Value = "702";
                        searchCriteriaToModify.Attributes["tableid"].Value = "7";
                        messages.Add("The searchCriteriaItem was successfully updated");
                    }
                }
            }

            foreach (XmlDocument doc2 in dataviews)
            {
                string dataviewid = doc2.DocumentElement.Attributes["dataviewid"] == null ? string.Empty : doc2.DocumentElement.Attributes["dataviewid"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc2.NameTable);
                manager.AddNamespace("COE", "COE.COEDataView");

                if (dataviewid == "4002")
                {
                    XmlNode tables = doc2.SelectSingleNode("//COE:tables", manager);
                    if (tables.SelectSingleNode("//COE:table[@id='7']", manager) == null)
                    {
                        tables.AppendChild(doc2.CreateNode(XmlNodeType.Element, "table", doc2.DocumentElement.NamespaceURI));
                        tables.LastChild.Attributes.Append(doc2.CreateAttribute("id"));
                        tables.LastChild.Attributes["id"].Value = "7";
                        tables.LastChild.Attributes.Append(doc2.CreateAttribute("name"));
                        tables.LastChild.Attributes["name"].Value = "VW_TEMPORARYREGNUMBERSPROJECT";
                        tables.LastChild.Attributes.Append(doc2.CreateAttribute("alias"));
                        tables.LastChild.Attributes["alias"].Value = "VW_TEMPORARYREGNUMBERSPROJECT";
                        tables.LastChild.Attributes.Append(doc2.CreateAttribute("database"));
                        tables.LastChild.Attributes["database"].Value = "REGDB";
                        tables.LastChild.Attributes.Append(doc2.CreateAttribute("primaryKey"));
                        tables.LastChild.Attributes["primaryKey"].Value = "700";
                        tables.LastChild.InnerXml = @"<fields id='700' name='ID' dataType='INTEGER' alias='ID' indexType='NONE' mimeType='NONE' visible='1'/><fields id='701' name='TEMPBATCHID' dataType='INTEGER' alias='TEMPBATCHID' indexType='NONE' mimeType='NONE' visible='1'/><fields id='702' name='PROJECTID' dataType='INTEGER' lookupFieldId='500' lookupDisplayFieldId='501' alias='PROJECTID' indexType='NONE' mimeType='NONE' visible='1'/>";

                        messages.Add("The table id = '7' was successfully added");
                    }
                    else
                    {
                        messages.Add("The table id='7' was not added because it was already pressent");
                        errorsInPatch = true;
                    }

                    XmlNode relationships = doc2.SelectSingleNode("//COE:relationships", manager);
                    if (relationships.SelectSingleNode("//COE:relationship[@child='7' and @parent='1']", manager) == null)
                    {
                        relationships.AppendChild(doc2.CreateNode(XmlNodeType.Element, "relationship", doc2.DocumentElement.NamespaceURI));
                        relationships.LastChild.Attributes.Append(doc2.CreateAttribute("child"));
                        relationships.LastChild.Attributes["child"].Value = "7";
                        relationships.LastChild.Attributes.Append(doc2.CreateAttribute("parent"));
                        relationships.LastChild.Attributes["parent"].Value = "1";
                        relationships.LastChild.Attributes.Append(doc2.CreateAttribute("childkey"));
                        relationships.LastChild.Attributes["childkey"].Value = "701";
                        relationships.LastChild.Attributes.Append(doc2.CreateAttribute("parentkey"));
                        relationships.LastChild.Attributes["parentkey"].Value = "100";
                        relationships.LastChild.Attributes.Append(doc2.CreateAttribute("jointype"));
                        relationships.LastChild.Attributes["jointype"].Value = "INNER";

                        messages.Add("The relatioship between table id = '7' and tables id ='1' was successfully added");
                    }
                    else
                    {
                        messages.Add("The relationship for table id='7' was not added because it was already pressent");
                        errorsInPatch = true;
                    }

                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR133757 was successfully patched");
            }
            else
                messages.Add("CSBR133757 was patched with errors");

            return messages;
        }
    }
}
