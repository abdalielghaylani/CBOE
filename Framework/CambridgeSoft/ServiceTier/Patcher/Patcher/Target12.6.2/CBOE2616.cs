using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CBOE2616 : BugFixBaseCommand
	{
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string batchProjectSQL = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A')";
            string activeFilter = " AND (ACTIVE = 'T' OR ACTIVE = 'F')";
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4002")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    XmlNode batchProject = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='BATCH_PROJECT']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    if (batchProject == null)
                    {
                        errorsInPatch = true;
                        messages.Add("The dropDownItemsSelect on BATCH_PROJECT item is missing in form id= '" + id + "'");
                    }
                    else
                    {
                        if (batchProject.InnerText.ToUpper().Trim().Equals((batchProjectSQL + activeFilter).ToUpper().Trim()))
                        {
                            messages.Add("WARNING: The dropDownItemsSelect on BATCH_PROJECT form element on COEFormGroup id='" + id + "' was already patched - skipping.");
                        }
                        else 
                        {
                            batchProject.InnerText = batchProjectSQL + activeFilter;
                            messages.Add("The dropDownItemsSelect on BATCH_PROJECT form element on COEFormGroup id='" + id + "' was successfully updated"); 
                        }                        
                    }
                }
            }
            if (!errorsInPatch)
                messages.Add("CBOE2616 was successfully patched");
            else
                messages.Add("CBOE2616 was patched with errors");
            return messages;
        }


	}
}
