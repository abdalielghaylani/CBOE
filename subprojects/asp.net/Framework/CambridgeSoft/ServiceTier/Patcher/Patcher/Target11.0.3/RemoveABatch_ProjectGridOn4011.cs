using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Project grid should be removed from form 4 in COEForm 4011.
    /// </summary>
	public class RemoveABatch_ProjectGridOn4011 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Goto customize registration and edit form 4011.
        /// Look in edit and view modes for formElements named "Projects" and remove them.
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
                if (id == "4011")
                {
                    XmlNode editModeProjectsF44011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:editMode/COE:formElement[@name='Projects']", manager);
                    if (editModeProjectsF44011 != null)
                    {
                        editModeProjectsF44011.ParentNode.RemoveChild(editModeProjectsF44011);
                        messages.Add("Projects form element succesfully deleted from edit mode of form 4 in coeform 4011");
                    }
                    else
                    {
                        messages.Add("WARNING: Projects form element was not found on edit mode for form 4 in coeform 4011 - skipped");
                    }

                    XmlNode viewModeProjectsF44011 = doc.SelectSingleNode("//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='4']/COE:viewMode/COE:formElement[@name='Projects']", manager);
                    if (viewModeProjectsF44011 != null)
                    {
                        viewModeProjectsF44011.ParentNode.RemoveChild(viewModeProjectsF44011);
                        messages.Add("Projects form element succesfully deleted from view mode of form 4 in coeform 4011");
                    }
                    else
                    {
                        messages.Add("WARNING: Projects form element was not found on view mode for form 4 in coeform 4011 - skipped");
                    }
                }
            }

            if (!errorsInPatch)
            {
                messages.Add("RemoveABatch_ProjectGridOn4011 was successfully patched");
            }
            else
                messages.Add("RemoveABatch_ProjectGridOn4011 was patched with errors");
            return messages;
        }
    }
}
