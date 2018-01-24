using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CSBR148015 : BugFixBaseCommand
    {
        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;

            foreach (XmlDocument doc in forms)
            {
               string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
               XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
               manager.AddNamespace("COE", "COE.FormGroup");
               if (id == "4011")
                 {
                  _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:viewMode/COE:formElement[@name='Identifiers']/COE:configInfo/COE:fieldConfig"; // Path to check the Rootnode before patcher update.
                  XmlNode selectedNode = doc.SelectSingleNode(_coeFormPath, manager);

                   #region Formxml Validations
                   
                     if (selectedNode == null)
                       {
                          errorsInPatch = true;
                          messages.Add("Patcher was not added.");
                          break;
                      }
               # endregion

                   #region Formxml changes:
                      XmlNode InsertAfterNode = doc.SelectSingleNode(_coeFormPath + "/COE:RemoveRowTitle", manager);
                      XmlNode InsertBeforeNode = doc.SelectSingleNode(_coeFormPath + "/COE:DefaultEmptyRows", manager);
                      XmlNode ReadOnly = doc.SelectSingleNode(_coeFormPath + "/COE:ReadOnly", manager);

                      // add node if ReadOnly Node doesn't exists
                      if (ReadOnly == null)
                      {
                          ReadOnly = doc.CreateNode(XmlNodeType.Element, "ReadOnly", "COE.FormGroup");
                          // adding value
                          ReadOnly.InnerText = "true";
                          if (InsertAfterNode != null)
                          {
                              selectedNode.InsertAfter(ReadOnly, InsertAfterNode);
                          }
                          else if (InsertBeforeNode != null)
                          {
                              selectedNode.InsertBefore(ReadOnly, InsertBeforeNode);
                          }
                          else
                          {
                              selectedNode.AppendChild(ReadOnly);
                          }
                          messages.Add("ReadOnly node added.");
                       }
                       else if (string.IsNullOrEmpty(ReadOnly.InnerText))
                       {
                          // update value
                          ReadOnly.InnerText = "true";
                          messages.Add("ReadOnly node updated.");
                       }
                       else if (ReadOnly.InnerText.ToUpper() == "FALSE")
                       {
                         // update value
                          ReadOnly.InnerText = "true";
                          messages.Add("ReadOnly node updated.");
                       }
                       else
                       {
                          messages.Add("ReadOnly node already set to true.");
                       }
                      #endregion 
                   break;
                  } 
             }
             if (!errorsInPatch)
             {
                 messages.Add("ReadOnly node was successfully patched");
             }
             else
                 messages.Add("ReadOnly node was patched with errors");

             return messages;
        }
    }
}
