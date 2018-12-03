using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    class CBOE8349 : BugFixBaseCommand
	{
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath_View = string.Empty;
            string _coeFormPath_Add= string.Empty;
            string _coeFormPath_Edit = string.Empty;

            _coeFormPath_View = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:viewMode/COE:formElement[@name='BaseFragmentStructure']"; // Path to check the Rootnode before patcher update.
            _coeFormPath_Add = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='BaseFragmentStructure']"; // Path to check the Rootnode before patcher update.
            _coeFormPath_Edit = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='BaseFragmentStructure']"; // Path to check the Rootnode before patcher update.
            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4012")
                {
                    XmlNode rootNodeView = doc.SelectSingleNode(_coeFormPath_View, manager);
                    XmlNode rootNodeAdd = doc.SelectSingleNode(_coeFormPath_Add, manager);
                    XmlNode rootNodeEdit = doc.SelectSingleNode(_coeFormPath_Edit, manager);
                    #region Form 4012 xml bindingExpression
                    if (rootNodeView == null)
                    {
                        errorsInPatch = true;
                        messages.Add("BaseFragmentStructure property is not available to update patch for form [" + id + "] in viewMode.");
                        break;
                    }
                    else
                    {
                        XmlNode bindingExpression = rootNodeView.SelectSingleNode("COE:bindingExpression", manager);
                        if (bindingExpression == null)
                        {
                            bindingExpression = rootNodeView.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                            bindingExpression.InnerText = "Compound.BaseFragment.Structure.Value";
                            rootNodeView.AppendChild(bindingExpression);
                            messages.Add("The bindingExpression is patched successfully in form [4012] viewMode ");
                        }
                        else if (bindingExpression.InnerText == null)
                        {
                            bindingExpression.InnerText = "Compound.BaseFragment.Structure.Value";
                            rootNodeView.AppendChild(bindingExpression);
                            messages.Add("The bindingExpression inner text is pacthed successfully in form [4012] viewMode ");
                        }
                        else
                        {
                            messages.Add("The bindingExpression already exists with inner text in form [4012] viewMode ");
                        }

                    }

                    if (rootNodeAdd == null)
                    {
                        errorsInPatch = true;
                        messages.Add("BaseFragmentStructure property is not available to update patch for form [" + id + "] in addMode.");
                        break;
                    }
                    else
                    {
                        XmlNode bindingExpression = rootNodeAdd.SelectSingleNode("COE:bindingExpression", manager);
                        if (bindingExpression == null)
                        {
                            bindingExpression = rootNodeAdd.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                            bindingExpression.InnerText = "Compound.BaseFragment.Structure.Value";
                            rootNodeAdd.AppendChild(bindingExpression);
                            messages.Add("The bindingExpression is patched successfully in form [4012] addMode ");
                        }
                        else if (bindingExpression.InnerText == null)
                        {
                            bindingExpression.InnerText = "Compound.BaseFragment.Structure.Value";
                            rootNodeAdd.AppendChild(bindingExpression);
                            messages.Add("The bindingExpression inner text is pacthed successfully in form [4012] addMode ");
                        }
                        else
                        {
                            messages.Add("The bindingExpression already exists with inner text in form [4012] addMode ");
                        }

                    }

                    if (rootNodeEdit == null)
                    {
                        errorsInPatch = true;
                        messages.Add("BaseFragmentStructure property is not available to update patch for form [" + id + "] in editMode.");
                        break;
                    }
                    else
                    {
                        XmlNode bindingExpression = rootNodeEdit.SelectSingleNode("COE:bindingExpression", manager);
                        if (bindingExpression == null)
                        {
                            bindingExpression = rootNodeEdit.OwnerDocument.CreateNode(XmlNodeType.Element, "bindingExpression", "COE.FormGroup");
                            bindingExpression.InnerText = "Compound.BaseFragment.Structure.Value";
                            rootNodeEdit.AppendChild(bindingExpression);
                            messages.Add("The bindingExpression is patched successfully in form [4012] editMode ");
                        }
                        else if (bindingExpression.InnerText == null)
                        {
                            bindingExpression.InnerText = "Compound.BaseFragment.Structure.Value";
                            rootNodeEdit.AppendChild(bindingExpression);
                            messages.Add("The bindingExpression inner text is pacthed successfully in form [4012] editMode ");
                        }
                        else
                        {
                            messages.Add("The bindingExpression already exists with inner text in form [4012] editMode ");
                        }

                    }

                    # endregion

                }

            }
            if (!errorsInPatch)
                messages.Add("CBOE8349 was successfully patched");
            else
                messages.Add("CBOE8349 was patched with errors");
            return messages;
        }        
	}
}
