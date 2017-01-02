using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-116596: Submit new record: The system is allowing the user to enter the same project multiple times
    /// 
    /// Steps:
    /// 1) Login as t5_85
    /// 2) Go to Submit new record
    /// 3) Select a project from projects grid at registry level
    /// 4) Select the same project again
    /// 5) Draw a structure
    /// 6) Click on Submit or Register
    /// 
    /// BUG: The system should not allow a user to enter the same project twice. See attached screenshot for details.
    /// </summary>
	public class CSBR116596 : BugFixBaseCommand
	{
        /// <summary>
        /// Manuals steps to fix:
        /// Open customize forms in registration and edit forms 4010 and 4012 by editing the coeForm with id=4, looking for the formElement
        /// named Projects, which is a grid. 
        /// Look for a Column with name=ProjectID, and modify it to be name=ID
        /// 
        /// Do the same for coeform 4011 but looking at coeForm=1002, and finaly edit the form 4013 and do the same but for coeForm id=2.
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
                XmlNode coeForm = null;
                if (id == "4010" || id == "4012")
                {
                    coeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']", manager);
                }
                else if(id == "4011")
                {
                    coeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']", manager);
                }
                else if (id == "4013")
                {
                    coeForm = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']", manager);
                }

                if (coeForm != null)
                {
                    XmlNodeList projectsFormElement = coeForm.SelectNodes(".//COE:formElement[@name='Projects']", manager);
                    if (projectsFormElement == null || projectsFormElement.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("ERROR: formElement Projects was not found on dataviewid=" + id + ", coeForm id=" + coeForm.Attributes["id"].Value);
                    }
                    else
                    {
                        foreach (XmlNode element in projectsFormElement)
                        {
                            XmlNode column = element.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns/COE:Column[@name='ProjectID']", manager);
                            if (column != null)
                            {
                                column.Attributes["name"].Value = "ID";
                                messages.Add("Column name changed to ID in dataviewid=" + id + ", coeForm id=" + coeForm.Attributes["id"].Value);
                            }
                            else
                            {
                                if (element.SelectSingleNode("./COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns/COE:Column[@name='ID']", manager) != null)
                                    messages.Add("WARNING: column with name='ProjectID' was not found and column with name='ID' was, form probably was already patched.");
                                else
                                {
                                    messages.Add("ERROR: column with name='ProjectID' was not found.");
                                    errorsInPatch = true;
                                }
                                
                            }
                        }
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("CSBR116596 was successfully patched");
            }
            else
                messages.Add("CSBR116596 was patched with errors");
            return messages;
        }
	}
}
