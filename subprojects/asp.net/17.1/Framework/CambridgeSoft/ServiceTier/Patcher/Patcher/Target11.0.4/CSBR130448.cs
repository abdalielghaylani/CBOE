using System;
using System.Collections.Generic;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-130448: Prefix is not nullable for Component ID ( Compound ID)
    /// 
    /// Steps:
    /// 1.Log in as regadmin
    /// 2.go to "Customize Registration"/Manage customizable tables/selecdt SEQUENCES table/edit Registry or Compound sequence
    /// 3.set PREFIX field as empty
    /// 4.click Update button
    /// 
    /// Expected Behavior:
    /// Prefix for Registry or Compound should be able to be set Null
    /// </summary>
	class CSBR130448:BugFixBaseCommand
	{
        /// <summary>
        /// Update client validation rules for SEQUENCES table editor, which is configured in the COEFrameworkConfig file
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            //Update PREFIX field
            string prefix_XPath = "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='PREFIX']";
            XmlNode prefix = frameworkConfig.SelectSingleNode(prefix_XPath);
            if (prefix != null)
            {
                try
                {
                    //remove requiredField validation rule
                    XmlNode prefix_ValidationRule_RequiredField = frameworkConfig.SelectSingleNode(prefix_XPath + "/validationRule/add[@name='requiredField']");
                    if (prefix_ValidationRule_RequiredField != null)
                    {
                        prefix_ValidationRule_RequiredField.ParentNode.RemoveChild(prefix_ValidationRule_RequiredField);
                        messages.Add("Remove requiredField validation rule of PREFIX field in SEQUENCES talbe editor.");
                    }

                    //Update textLength validation rule
                    XmlNode prefix_ValidationRule_TextLength = frameworkConfig.SelectSingleNode(prefix_XPath + "/validationRule/add[@name='textLength']");
                    if (prefix_ValidationRule_TextLength != null)
                    {
                        //error message
                        prefix_ValidationRule_TextLength.Attributes["errorMessage"].Value = "The length must be between 0 and 150.";
                        //parameter
                        foreach (XmlNode param in prefix_ValidationRule_TextLength.FirstChild.ChildNodes)
                        {
                            if (param.Attributes["name"].Value == "min")
                                param.Attributes["value"].Value = "0";
                        }
                        messages.Add("Update textLength validation rule of PREFIX field in SEQUENCES talbe editor.");
                    }
                }
                catch(Exception e)
                {
                    errorsInPatch = true;
                    messages.Add(e.Message);
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Cannot find PREFIX field in SEQUENCES table editor");
            }

            //update PREFIXDELIMITER field
            string prefixDelimiter_XPath = "/configuration/coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='PREFIXDELIMITER']";
            XmlNode prefixDelimiter = frameworkConfig.SelectSingleNode(prefix_XPath);
            if (prefixDelimiter != null)
            {
                try
                {
                    //remove requiredField validation rule
                    XmlNode prefixDelimiter_ValidationRule_RequiredField = frameworkConfig.SelectSingleNode(prefixDelimiter_XPath + "/validationRule/add[@name='requiredField']");
                    if (prefixDelimiter_ValidationRule_RequiredField != null)
                    {
                        prefixDelimiter_ValidationRule_RequiredField.ParentNode.RemoveChild(prefixDelimiter_ValidationRule_RequiredField);
                        messages.Add("Remove requiredField validation rule of PREFIXDELIMITER field in SEQUENCES talbe editor.");
                    }
                }
                catch (Exception e)
                {
                    errorsInPatch = true;
                    messages.Add(e.Message);
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Cannot find PREFIXDELIMITER field in SEQUENCES table editor");
            }

            if (errorsInPatch)
                messages.Add("CSBR130488 was patched with errors");
            else
                messages.Add("CSBR130488 was successfully patched");

            return messages;
        }
	}
}
