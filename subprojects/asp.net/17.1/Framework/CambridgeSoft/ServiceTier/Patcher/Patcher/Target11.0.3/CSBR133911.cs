using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-133911: "User is not able to add a document link from docmanager to registration"
    /// 
    /// 1) Login with cssadmin(Permission to docmanager) to registration.
    /// 2) Select a registered compound.
    /// 3) Click on add doc link and try to attach a document from docmanager.
    /// Bug: User gets logged off while clicking on the link.
    /// </summary>
	class CSBR133911 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix.
        /// 
        /// You need to run the latest db patcher (11.0.3 or higher), then look for the docmanager form (id=8) in form 4012.
        /// Replace the docManager form (id=8) with the one in the default configuration.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            //&amp;ticket=|_ticket_|
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            foreach (XmlDocument doc in forms)
            {

                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if (id == "4012")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNode addDocsLinkFormElement = doc.SelectSingleNode("//COE:coeForm[@id='8']/COE:viewMode/COE:formElement[@name='AddDocsLink']/COE:configInfo/COE:fieldConfig", manager);
                    if (addDocsLinkFormElement != null)
                    {
                        XmlNode textNode = addDocsLinkFormElement.SelectSingleNode("./COE:Text", manager);
                        if (textNode != null && textNode.InnerText.ToLower().Trim() == "add document")
                        {
                            textNode.InnerText = "Add Document Link";
                            messages.Add("Add Document Link text was succesfully updated");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Add Document link text was already changed");
                        }

                        XmlNode hrefNode = addDocsLinkFormElement.SelectSingleNode("./COE:HRef", manager);
                        if (hrefNode != null && hrefNode.InnerText.Contains("&ticket=|_ticket_|"))
                        {
                            hrefNode.InnerText = hrefNode.InnerText.Replace("&ticket=|_ticket_|", string.Empty);
                            messages.Add("Ticket was successfuly removed from href in AddDocsLink form element");
                        }
                        else
                        {
                            errorsInPatch = true;
                            messages.Add("Add Document link url was already changed");
                        }
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Add Document Link form element was not found on form 4012");
                    }

                    XmlNode docGridRidFormElement = doc.SelectSingleNode("//COE:coeForm[@id='8']/COE:viewMode/COE:formElement[@name='DocGrid']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns/COE:Column[@name='RID']", manager);
                    if (docGridRidFormElement != null)
                    {
                        docGridRidFormElement.Attributes["name"].Value = "DelID";
                        docGridRidFormElement.Attributes["formatText"].Value = "<a href='/docmanager/docmanager/externallinks/processLinks.asp?maction=deleteconfirm&docid={0}&LinkType=CHEMREGREGNUMBER&extLinkID=|_StartBindingExp_RegNumber_EndsBindingExp_|&extAppName=Chem_Reg&useReload=false' target='_blank' >Delete </a>";
                        docGridRidFormElement.FirstChild.Attributes.Append(doc.CreateAttribute("name"));
                        docGridRidFormElement.FirstChild.Attributes["name"].Value = "DeleteId";
                        docGridRidFormElement.FirstChild.SelectSingleNode("./COE:bindingExpression", manager).InnerText = "DOCID";
                        messages.Add("Rid column was successfully changed for displaying doc manager links properly on form 4012");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Document grid has no longer a column named RID on form 4012");
                    }
                    XmlNode docGridDocIdFormElement = doc.SelectSingleNode("//COE:coeForm[@id='8']/COE:viewMode/COE:formElement[@name='DocGrid']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns/COE:Column[@name='DOCID']", manager);
                    if (docGridDocIdFormElement != null)
                    {
                        docGridDocIdFormElement.Attributes["formatText"].Value = "<a href='/docmanager/default.asp?formgroup=base_form_group&dbname=docmanager&formmode_override=edit&dataaction=query_string&field_type=integer&full_field_name=docmgr_documents.docid&field_value={0}' target='_blank' >View</a>";
                        messages.Add("DocId column was successfully changed for displaying doc manager links properly on form 4012");
                    }
                    else
                    {
                        errorsInPatch = true;
                        messages.Add("Document grid has no column named DOCID on form 4012");
                    }
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("CSBR133911 was successfully patched");
            }
            else
                messages.Add("CSBR133911 was patched with errors");
            return messages;

        }
    }
}
