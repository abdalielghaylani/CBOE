using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-133571: When RLS = Batch: Registry Level projects were available in the Projects dropdown in Duplicates Components Information screen.
    /// 
    /// Steps to Reproduce:
    /// 
    /// 1. Login to CBOE application
    /// 2. Click on Main form of registration
    /// 3. Click on Duplicate records count.
    /// 4. Click on new query link in the Duplicates components information screen.
    /// 5. View the List of projects available in the Projects dropdown.
    /// 
    /// Bug: Type: Registry Projects were available in the list of projects.
    /// 
    /// Expected Result: The Batch/All Projects should be available in the list of projects.
    /// 
    /// 
    /// CSBR-133563: Inactive Projects were available in the Projects dropdown of Duplicates components information form.
    /// 
    /// Steps to Reproduce:
    /// 
    /// 1. Login to CBOE application
    /// 2. Click on Main form of registration
    /// 3. Click on Duplicate records count.
    /// 4. Click on new query link in the Duplicates components information screen.
    /// 5. View the List of projects available in the Projects dropdown.
    /// 
    /// Bug: Inactive Projects were available in the list of projects.
    /// 
    /// Expected Result: The inactive proects should be not available in the list of projects.
    /// </summary>
    class CSBR133571_133563 : BugFixBaseCommand
    {
        /// <summary>
        /// Edit formgroup 4019 and look for the form element named REGISTRY_PROJECT and change
        /// 
        /// &lt;dropDownItemsSelect&gt;SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT&lt;/dropDownItemsSelect&gt;
        /// 
        /// with
        /// 
        /// &lt;dropDownItemsSelect&gt;SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A') AND ACTIVE = 'T'&lt;/dropDownItemsSelect&gt;
        /// </summary>        
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4019")
                {
                    XmlNode layoutInfo = doc.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo", manager);

                    if (layoutInfo == null)
                    {
                        messages.Add("ERROR - QUERY MODE: Could not find layoutInfo from queryForm id='0' -> coeForm id='0'");
                        errorsInPatch = true;
                    }
                    else
                    {
                        XmlNode regProject = layoutInfo.SelectSingleNode("COE:formElement[@name='REGISTRY_PROJECT']", manager);
                        if (regProject == null)
                        {
                            errorsInPatch = true;
                            messages.Add("QUERY MODE: formElement 'REGISTRY_PROJECT' does not exist in query form.");
                        }
                        else
                        {
                            XmlNode dropdownItemsSelect = regProject.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                            if (dropdownItemsSelect != null && dropdownItemsSelect.InnerText.ToUpper().Trim() == "SELECT PROJECTID AS KEY, NAME AS VALUE FROM REGDB.VW_PROJECT")
                            {
                                dropdownItemsSelect.InnerText = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='R' OR TYPE='A') AND ACTIVE = 'T'";
                                messages.Add("Successfully updated formElement Project at registry level in duplicates search form with the proper drop down item select");
                            }
                            else
                                messages.Add("DropDownListItemsSelect did not have the expected value and was not modified.");
                        }
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR-133571 and CSBR-133563 were successfully patched");
            }
            else
                messages.Add("CSBR-133571 and CSBR-133563 were patched with errors");
            return messages;
        }
    }
}
