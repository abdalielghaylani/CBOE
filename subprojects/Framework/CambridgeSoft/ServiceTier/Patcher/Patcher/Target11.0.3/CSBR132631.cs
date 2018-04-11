using System.Collections.Generic;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR - 132631: Numerical range searches cannot be performed on Registration 11.0.2
    /// </summary>
	class CSBR132631 : BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            List<string> formElementsToModify = new List<string>(new string[] { "AMOUNT", "PURITY" });
            List<COEFormHelper> coeFormHelpers = new List<COEFormHelper>();
            // 'Search Registry' and 'Search Temporary'
            coeFormHelpers.Add(new COEFormHelper("4002", "0"));
            coeFormHelpers.Add(new COEFormHelper("4003", "2"));

            foreach (XmlDocument doc in forms)
            {
                foreach (COEFormHelper coeFormHelper in coeFormHelpers)
                {
                    string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;

                    if (coeFormHelper.FormGroupId == id)
                    {
                        XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                        manager.AddNamespace("COE", "COE.FormGroup");

                        XmlNode coeForm = doc.SelectSingleNode(string.Format("//COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='{0}']", coeFormHelper.COEFormId), manager);
                        if (coeForm == null)
                        {
                            messages.Add(string.Format("The expected form '{0}' was not found", coeFormHelper.COEFormId));
                            errorsInPatch = true;
                        }
                        else
                        {
                            foreach (string formElement in formElementsToModify)
                            {
                                XmlNode validationRuleListToModify = coeForm.SelectSingleNode(string.Format("//COE:layoutInfo/COE:formElement[@name='{0}']/COE:validationRuleList", formElement), manager);

                                if (validationRuleListToModify == null)
                                {
                                    messages.Add("The expected validationRuleList was not found");
                                    errorsInPatch = true;
                                }
                                else
                                {
                                    validationRuleListToModify.InnerXml = string.Empty;
                                }
                            }
                        }
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR132631 was successfully patched");
            }
            else
                messages.Add("CSBR132631 was patched with errors");
            return messages;
        }
    }

    class COEFormHelper
    {
        public string FormGroupId = string.Empty;
        public string COEFormId = string.Empty;

        public COEFormHelper(string formGroupId, string coeFormId)
        {
            FormGroupId = formGroupId;
            COEFormId = coeFormId;
        }
    }
}
