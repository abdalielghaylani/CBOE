using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 133194: Disable project shows at the project name dropdown list in Search Temp and Search Registry section.
    /// </summary>
    class CSBR133194 : BugFixBaseCommand
    {
        /// <summary>
        /// Steps to fix manually:
        /// 1 - Open COEFormGroups 4002 and 4003.
        /// 2 - Search the FormElements REGISTRY_PROJECT and BATCH_PROJECT in both form groups.
        /// 3 - Add at the end of the dropDownItemsSelect the SQL filter 'AND ACTIVE = 'T''.
        /// </summary>        
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            string registryProjectSQL = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A')";
            string batchProjectSQL = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A')";
            string activeFilter = " AND ACTIVE = 'T'";

            foreach (XmlDocument doc in forms)
            {
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4003" || id == "4002")
                {
                    XmlNode registryProjectsDDItemsSelect = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='REGISTRY_PROJECT']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    if (registryProjectsDDItemsSelect.InnerText.ToUpper().Trim().Equals((registryProjectSQL + activeFilter).Trim().ToUpper()))
                    {
                        messages.Add("WARNING: The dropDownItemsSelect on REGISTRY_PROJECT form element on COEFormGroup id='" + id + "' was already patched - skipping.");
                    }
                    else if (registryProjectsDDItemsSelect.InnerText.Trim().ToUpper() != registryProjectSQL.Trim().ToUpper())
                    {
                        messages.Add("The dropDownItemsSelect on REGISTRY_PROJECT form element on COEFormGroup id='" + id + "' doesn´t have the expected value and was not changed");
                        errorsInPatch = true;
                    }
                    else
                    {
                        registryProjectsDDItemsSelect.InnerText = registryProjectsDDItemsSelect.InnerText + activeFilter;
                        messages.Add("The dropDownItemsSelect on REGISTRY_PROJECT form element on COEFormGroup id='" + id + "' was successfully updated");
                    }

                    XmlNode batchProjectsDDItemsSelect;
                    if (id == "4002")
                    {
                        batchProjectsDDItemsSelect = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='BATCH_PROJECT']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    }
                    else
                    {
                        batchProjectsDDItemsSelect = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='BATCH_PROJECT']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    }

                    if (batchProjectsDDItemsSelect.InnerText.ToUpper().Trim().Equals((batchProjectSQL + activeFilter).ToUpper().Trim()))
                    {
                        messages.Add("WARNING: The dropDownItemsSelect on BATCH_PROJECT form element on COEFormGroup id='" + id + "' was already patched - skipping.");
                    }
                    else if (batchProjectsDDItemsSelect.InnerText.Trim().ToUpper() != batchProjectSQL.Trim().ToUpper())
                    {
                        messages.Add("The dropDownItemsSelect on BATCH_PROJECT form element on COEFormGroup id='" + id + "' doesn´t have the expected value and was not changed");
                        errorsInPatch = true;
                    }
                    else
                    {
                        batchProjectsDDItemsSelect.InnerText = batchProjectsDDItemsSelect.InnerText + activeFilter;
                        messages.Add("The dropDownItemsSelect on BATCH_PROJECT form element on COEFormGroup id='" + id + "' was successfully updated");
                    }
                }

            }

            if (!errorsInPatch)
                messages.Add("CSBR133194 was successfully patched");
            else
                messages.Add("CSBR133194 was patched with errors");
            return messages;
        }
    }
}
