using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Fix for Grid's alignment in submit records after upgrdation of infragistics
    /// </summary>
    class CSBR158405: BugFixBaseCommand
    {

        #region Variable
        string _coeFormPath = string.Empty;
        string _coePath = string.Empty;
        XmlNode _rootNode = null;
        #endregion
               
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews,List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
                List<string> messages = new List<string>();
                foreach (XmlDocument doc in forms)
                 {
                     string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;  
                     if (id == "4010" || id == "4011" || id == "4012")
                     {
                         XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                         manager.AddNamespace("COE", "COE.FormGroup");

                         //Creating cssClass attribute to Registry Projects grid
                         if (id == "4010")
                         {
                             _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:addMode/COE:formElement[@name='Projects']/COE:displayInfo";
                             _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                             createWidthAttribute(manager, "ProjectsPanWidthStyleCSS");
                             messages.Add("Added cssClass attribute to Projects Grid in add mode");
                         }

                          _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Projects']/COE:displayInfo";
                         _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                         createWidthAttribute(manager, "ProjectsPanWidthStyleCSS");
                         messages.Add("Added cssClass attribute to Projects Grid in edit mode");

                         //Creating cssClass attribute to Registry Identifier grid
                         if (id == "4010")
                         {
                             _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:addMode/COE:formElement[@name='Identifiers']/COE:displayInfo";
                             _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                             createWidthAttribute(manager, "IdentifiersPanWidthStyleCSS");
                             messages.Add("Added cssClass attribute to Identifers Grid in add mode");
                         }

                         if (id == "4010")
                         {
                             _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='IdentifierList']/COE:displayInfo";
                         }
                         else if(id=="4011")
                         {
                             _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Identifiers']/COE:displayInfo";
                         }
                         else if (id == "4012")
                         {
                             _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:editMode/COE:formElement[@name='Identifier']/COE:displayInfo";
                         }
                         _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                         createWidthAttribute(manager, "IdentifiersPanWidthStyleCSS");
                         messages.Add("Added cssClass attribute to Identifers Grid in edit mode");


                         //Creating cssClass attribute to Structure Identifier grid
                         _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:addMode/COE:formElement[@name='Identifiers']/COE:displayInfo";
                         _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                         createWidthAttribute(manager, "IdentifiersPanWidthStyleCSS");
                         messages.Add("Added cssClass attribute to Structure Identifers Grid in add mode");

                         _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1']/COE:editMode/COE:formElement[@name='Identifiers']/COE:displayInfo";
                         _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                         createWidthAttribute(manager, "IdentifiersPanWidthStyleCSS");
                         messages.Add("Added cssClass attribute to Structure Identifers Grid in edit mode");

                         //Creating cssClass attribute to Component Identifier grid
                         _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:addMode/COE:formElement[@name='Identifiers']/COE:displayInfo";
                         _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                         createWidthAttribute(manager, "IdentifiersPanWidthStyleCSS");
                         messages.Add("Added cssClass attribute to Component Identifers Grid in add mode");

                         _coePath = "//COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1001']/COE:editMode/COE:formElement[@name='Identifiers']/COE:displayInfo";
                         _rootNode = doc.SelectSingleNode(_coePath, manager); // 
                         createWidthAttribute(manager, "IdentifiersPanWidthStyleCSS");
                         messages.Add("Added cssClass attribute to Component Identifers Grid in edit mode"); 
                    }
                 }
                return messages;
        }
        /// <summary>
        /// This method will create a cssClass attribute and sets the innertext
        /// </summary>
        /// <param name="manager">root Namespace</param>
        /// <param name="value"> innertext</param>
        private void createWidthAttribute(XmlNamespaceManager manager, string value)
        {

            if (_rootNode.SelectSingleNode("COE:cssClass", manager) == null)
            {
                XmlNode width = _rootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "cssClass", "COE.FormGroup");
                width.InnerText = value; // ;
                XmlNode insertAfter = _rootNode.SelectSingleNode("COE:type", manager);
                if (insertAfter != null)
                  _rootNode.InsertAfter(width, insertAfter);
                else
                    _rootNode.AppendChild(width);
            }
        }

      }
    }

