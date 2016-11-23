using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-125310:
    /// 
    /// 'Identifiers' do not have to be uniquely keyed: that is, if a user wants to assign multiple 'Synonym' identifers to a compound, he/she should be able to,
    /// </summary>
	public class CSBR125310 : BugFixBaseCommand
	{
        /// <summary>
        /// This were the manual steps to fix:
        /// In Forms 4011 and 4012
        /// 1.	Search for the following string: &lt;formElement name="Identifiers"&gt;.
        /// 2.	If that section exists, check if it has this line at the end of the section by searching for:: &lt;Event name="BeforeEnterEdit"&gt;{CustomJS_FilterByUnique(IdentifierID)}&lt;/Event&gt;.
        /// 3.	If it does, replace that line with the following one: &lt;!-- &lt;Event name="BeforeEnterEdit"&gt;{CustomJS_FilterByUnique(IdentifierID)}&lt;/Event&gt;.
        /// 4.	Search for all occurrence and modify them all. (there are three entries in the default COE11LR configuration)
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
            foreach(XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if(id == "4011" || id == "4012")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNodeList identifiers = doc.SelectNodes("//COE:formElement[@name='Identifiers']", manager);
                    if(identifiers != null && identifiers.Count > 0)
                    {
                        bool errorsInForm = false;
                        foreach(XmlNode identifier in identifiers)
                        {
                            XmlNode beforeEnterEditNode = identifier.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ClientSideEvents/COE:Event[@name='BeforeEnterEdit']", manager);
                            if(beforeEnterEditNode != null)
                            {
                                if(beforeEnterEditNode.InnerText == "{CustomJS_FilterByUnique(IdentifierID)}")
                                {
                                    beforeEnterEditNode.ParentNode.ReplaceChild(doc.CreateComment(beforeEnterEditNode.OuterXml), beforeEnterEditNode);
                                    messages.Add("Successfully commented out BeforeEnterEdit event");
                                }
                                else
                                {
                                    messages.Add("Found Event BeforeEnterEdit but had a different value than expected. Did the customer modify it by himself?. Please review carefully form id: " + id);
                                    errorsInForm = true;
                                }
                            }
                        }
                        errorsInPatch |= errorsInForm;
                    }

                    XmlNodeList identifierList = doc.SelectNodes("//COE:formElement[@name='Identifier']", manager);
                    if((identifierList == null || identifierList.Count == 0) && (identifiers == null || identifiers.Count == 0))
                    {
                        messages.Add("No identifier found in form " + id);
                        errorsInPatch = true;
                    }
                    else if(identifierList != null && identifierList.Count > 0)
                    {
                        bool errorsInForm = false;
                        foreach(XmlNode identifier in identifierList)
                        {
                            XmlNode beforeEnterEditNode = identifier.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:ClientSideEvents/COE:Event[@name='BeforeEnterEdit']", manager);
                            if(beforeEnterEditNode != null)
                            {
                                if(beforeEnterEditNode.InnerText == "{CustomJS_FilterByUnique(IdentifierID)}")
                                {
                                    beforeEnterEditNode.ParentNode.ReplaceChild(doc.CreateComment(beforeEnterEditNode.OuterXml), beforeEnterEditNode);
                                    messages.Add("Successfully commented out BeforeEnterEdit event");
                                }
                                else
                                {
                                    messages.Add("Found Event BeforeEnterEdit but had a different value than expected. Did the customer modify it by himself?. Please review carefully form id: " + id);
                                    errorsInForm = true;
                                }
                            }
                        }
                        errorsInPatch |= errorsInForm;
                    }
                }
            }
            if (!errorsInPatch)
            {
                if (messages.Count == 0)
                {
                    messages.Add("There is no Identifier with BeforeEnterEdit event. No action was required");
                    messages.Add("CSBR125310 was patched with errors");
                }
                else
                    messages.Add("CSBR125310 was successfully patched");
            }
            else
                messages.Add("CSBR125310 was patched with errors");
            return messages;
        }
	}
}
