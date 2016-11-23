using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    //EN-5480 The Date Created and Synthesis Date Size in Registry Compound Selector needs to be increased

    public class EN5480 : BugFixBaseCommand
	{
        
        XmlDocument formDoc = new XmlDocument();
        XmlNamespaceManager manager;
        string nameSpaceURI = "COE.FormGroup";
        
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorInPatch = false;
            manager = new XmlNamespaceManager(formDoc.NameTable);
            manager.AddNamespace("COE", "COE.FormGroup");
            string coeFormPath;
            #region FromBO Updates
            try
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    formDoc = (XmlDocument)(forms[i]);
                    string id = formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : formDoc.DocumentElement.Attributes["id"].Value;
                    if (id == "4006")
                    {

                        coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='DATECREATED']";
                        XmlNode selectMixtureDateNode = formDoc.SelectSingleNode(coeFormPath, manager);
                        if (selectMixtureDateNode != null)
                        {
                            UpdateDateFieldWidth(selectMixtureDateNode, coeFormPath);
                            UpdateDateFieldCalenderCalss(selectMixtureDateNode, coeFormPath);
                        }

                        coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='DATECREATED']";
                        XmlNode selectBatchDateCreatedNode = formDoc.SelectSingleNode(coeFormPath, manager);
                        if (selectBatchDateCreatedNode != null)
                        {
                            UpdateDateFieldWidth(selectBatchDateCreatedNode, coeFormPath);
                            UpdateDateFieldCalenderCalss(selectBatchDateCreatedNode, coeFormPath);
                        }


                        coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='CREATION_DATE']";
                        XmlNode selectBatchSynthesisDateNode = formDoc.SelectSingleNode(coeFormPath, manager);
                        if (selectBatchSynthesisDateNode != null)
                        {
                            UpdateDateFieldWidth(selectBatchSynthesisDateNode, coeFormPath);
                            UpdateDateFieldCalenderCalss(selectBatchSynthesisDateNode, coeFormPath);
                        }
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                errorInPatch = true;
                messages.Add(ex.Message);
            }

            #endregion
            if (!errorInPatch)
                messages.Add("Date fields updated successfully in ELN search registry form.");
            else
                messages.Add("Error while updating date fields in ELN search registry form.");
            return messages;
        }

        private void UpdateDateFieldWidth(XmlNode node,string formPath)
        {
            if (node != null)
            {
                XmlNode fieldConfigNode = node.SelectSingleNode("COE:configInfo/COE:fieldConfig",manager);
                if (fieldConfigNode!=null)
                {
                    XmlNode widthNode = fieldConfigNode.SelectSingleNode("COE:Width", manager);
                    if (widthNode == null)
                    {
                        widthNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Width", fieldConfigNode.NamespaceURI);
                        widthNode.InnerXml = "100%";
                        fieldConfigNode.AppendChild(widthNode);
                    }
                    else
                        widthNode.InnerXml = "100%";
                }

            }
        }
        private void UpdateDateFieldCalenderCalss(XmlNode node, string formPath)
        {
            if (node != null)
            {
                XmlNode fieldConfigNode = node.SelectSingleNode("COE:configInfo/COE:fieldConfig", manager);
                if (fieldConfigNode != null)
                {
                    XmlNode CSSCalenderClassNode = fieldConfigNode.SelectSingleNode("COE:CSSCalenderClass", manager);
                    if (CSSCalenderClassNode == null)
                    {
                        CSSCalenderClassNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "CSSCalenderClass", fieldConfigNode.NamespaceURI);
                        CSSCalenderClassNode.InnerXml = "CalenderClass";
                        fieldConfigNode.AppendChild(CSSCalenderClassNode);
                    }
                    else
                        CSSCalenderClassNode.InnerXml = "CalenderClass";
                }

            }
        }
	}
}
