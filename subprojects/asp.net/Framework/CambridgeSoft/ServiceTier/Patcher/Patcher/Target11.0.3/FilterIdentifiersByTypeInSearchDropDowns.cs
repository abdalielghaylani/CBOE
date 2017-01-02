using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// IDENTIFIERTYPE form element in search forms is not filtering based on its type, and thus showing 'Registry type' Identifiers at batch level
    /// </summary>
	class FilterIdentifiersByTypeInSearchDropDowns : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix are:
        /// 
        /// Edit formgroups 4003, 4006 and 4019. Look for the formElement with name IDENTIFIERTYPE in query mode and ensure the filters accordingly.
        /// That is:  WHERE (TYPE ='{0}' OR TYPE='A') AND ACTIVE = 'T'
        /// Where {0} must be replaced with R for registry, B for batches and C for Components
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4003" || id == "4019" || id == "4006")
                {
                    XmlNodeList identifierTypeList = doc.SelectNodes("//COE:queryForm//COE:formElement[@name='IDENTIFIERTYPE']", manager);

                    if (identifierTypeList == null || identifierTypeList.Count == 0)
                    {
                        messages.Add(string.Format("Could not find identifiertype form elements on formgroup id='{0}'", id));
                        errorsInPatch = true;
                    }
                    else
                    {
                        foreach(XmlNode identifierType in identifierTypeList)
                        {
                            char identifierLetter = 'B';

                            switch (identifierType.ParentNode.ParentNode.Attributes["id"].Value)
                            {
                                case "0":
                                    identifierLetter = 'R';
                                    break;
                                case "1":
                                    identifierLetter = 'C';
                                    break;
                                case "4":
                                    identifierLetter = 'B';
                                    break;
                            }

                            XmlNode dropdownItemsSelect = identifierType.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect", manager);
                            if (dropdownItemsSelect != null && dropdownItemsSelect.InnerText.ToUpper().Trim() == "SELECT ID AS KEY, NAME AS VALUE FROM REGDB.VW_IDENTIFIERTYPE")
                            {
                                dropdownItemsSelect.InnerText = string.Format("SELECT ID as key, NAME as value FROM REGDB.VW_IDENTIFIERTYPE WHERE (TYPE ='{0}' OR TYPE='A') AND ACTIVE = 'T'", identifierLetter);
                                messages.Add("Successfully updated formElement Project at registry level in duplicates search form with the proper drop down item select");
                            }
                            else
                                messages.Add(string.Format("DropDownListItemsSelect did not have the expected value and was not modified on formgroup id='{0}'", id));
                        }
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("FilterIdentifiersByTypeInSearchDropDowns were successfully patched");
            }
            else
                messages.Add("FilterIdentifiersByTypeInSearchDropDowns were patched with errors");
            return messages;
        }
	}
}
