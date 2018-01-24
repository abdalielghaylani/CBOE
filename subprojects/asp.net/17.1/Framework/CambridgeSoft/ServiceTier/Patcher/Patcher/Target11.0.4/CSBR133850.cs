using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// The dropdown items for Units are not sorted in a case-insensitive way.
    /// Change ORDER BY to upper(UNIT) ASC to fix this bug.
    /// </summary>
	public class CSBR133850 : BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            string selectSQL = "SELECT ID as key,UNIT as value FROM REGDB.VW_Unit ORDER BY upper(UNIT) ASC";

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4010")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");

                    XmlNode unitsDropdownSelectAddMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:addMode/COE:formElement[@name='AMOUNT_UNITS']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    if (unitsDropdownSelectAddMode == null)
                    {
                        errorsInPatch = true;
                        messages.Add("Cannot find AMOUNT_UNITS element's dropDownItemsSelect information");
                    }
                    else
                    {
                        unitsDropdownSelectAddMode.InnerText = selectSQL;
                    }

                    XmlNode unitsDropdownSelectEditMode = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/COE:editMode/COE:formElement[@name='AMOUNT_UNITS']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                    if (unitsDropdownSelectEditMode == null)
                    {
                        errorsInPatch = true;
                        messages.Add("Cannot find AMOUNT_UNITS element's dropDownItemsSelect information");
                    }
                    else
                    {
                        unitsDropdownSelectEditMode.InnerText = selectSQL;
                    }
                }
            }

            if (!errorsInPatch)
                messages.Add("CSBR133850 was successfully patched");
            else
                messages.Add("CSBR133850 was patched with errors");

            return messages;
        }
    }
}
