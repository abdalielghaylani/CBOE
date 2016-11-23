using System;
using System.Xml;
using System.Collections.Generic;

using CambridgeSoft.COE.Patcher.Utilities;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// 1. In coeForm 1, remove CHEM_NAME_AUTOGEN form element;
    /// 2. In resultsCriteria, remove CHEM_NAME_AUTOGEN field from table 2.
    /// </summary>
    public class MigCOEFormGroup4002 : BugFixBaseCommand
    {
        private XmlDocument coeFormGroup4002 = null;

        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            coeFormGroup4002 = PatcherUtility.GetCoeFormGroupById(forms, "4002");
            if (coeFormGroup4002 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeFormGroup 4002");
            }
            else
            {
                PatcherUtility.SetXmlNamespaceManager(coeFormGroup4002);

                RemoveCNAFormElement(ref errorsInPatch, messages);
                RemoveCNAField(ref errorsInPatch, messages);
            }

            if (!errorsInPatch)
                messages.Add("COE form group 4002 was successfully migrated");
            else
                messages.Add("Failed to migrate COE form group 4002");

            return messages;
        }

        private void RemoveCNAField(ref bool errorsInPatch, List<string> messages)
        {
            PatcherUtility.XmlNamespaceManager.AddNamespace("ResultsCriteria", "COE.ResultsCriteria");
            XmlNode resultsCriteriaNode = coeFormGroup4002.SelectSingleNode("//COE:listForms/COE:listForm[@id='0']/ResultsCriteria:resultsCriteria", PatcherUtility.XmlNamespaceManager);

            if (resultsCriteriaNode == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find resultsCriteria node");
            }
            else
            {
                XmlNode CNAField = resultsCriteriaNode.SelectSingleNode("//ResultsCriteria:tables/ResultsCriteria:table[@id='2']/ResultsCriteria:field[@alias='CHEM_NAME_AUTOGEN']", PatcherUtility.XmlNamespaceManager);

                if (CNAField != null)
                    CNAField.ParentNode.RemoveChild(CNAField);
            }
        }

        private void RemoveCNAFormElement(ref bool errorsInPatch, List<string> messages)
        {
            XmlNode coeForm1 = coeFormGroup4002.SelectSingleNode("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']", PatcherUtility.XmlNamespaceManager);
            if (coeForm1 == null)
            {
                errorsInPatch = true;
                messages.Add("ERROR: Couldn't find coeForm 1");
            }
            else
            {
                XmlNode CNAFormElement = coeForm1.SelectSingleNode("./COE:layoutInfo/COE:formElement[@name='CHEM_NAME_AUTOGEN']", PatcherUtility.XmlNamespaceManager);
                if (CNAFormElement == null)
                {
                    errorsInPatch = true;
                    messages.Add("ERROR: Couldn't find CHEM_NAME_AUTOGEN element in coeForm 1");
                }
                else
                {
                    CNAFormElement.ParentNode.RemoveChild(CNAFormElement);
                }
            }
        }
    }
}
