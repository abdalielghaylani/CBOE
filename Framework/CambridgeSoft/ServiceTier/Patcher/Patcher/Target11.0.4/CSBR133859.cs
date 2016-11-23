using System;
using System.Collections.Generic;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// The dropdown items for Scientists should not filter out those inactive ones.
    /// </summary>
    public class CSBR133859 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            string selectSQL = "SELECT PERSON_ID as key,USER_ID as value FROM COEDB.PEOPLE";

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4010")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    XmlNode scientistDropdownSelectAddMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:addMode/COE:formElement[@name='SCIENTIST_ID']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    if (scientistDropdownSelectAddMode == null)
                    {
                        errorsInPatch = true;
                        messages.Add("Cannot find SCIENTIST_ID element's dropDownItemsSelect information");
                    }
                    else
                    {
                        scientistDropdownSelectAddMode.InnerText = selectSQL;
                    }

                    XmlNode scientistDropdownSelectEditMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='SCIENTIST_ID']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    if (scientistDropdownSelectEditMode == null)
                    {
                        errorsInPatch = true;
                        messages.Add("Cannot find AMOUNT_UNITS element's dropDownItemsSelect information");
                    }
                    else
                    {
                        scientistDropdownSelectEditMode.InnerText = selectSQL;
                    }
                }
            }

            if (!errorsInPatch)
                messages.Add("CSBR133859 was successfully patched");
            else
                messages.Add("CSBR133859 was patched with errors");

            return messages;
        }
    }
}
