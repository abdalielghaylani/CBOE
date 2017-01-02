using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  changelist # 441319 .
    /// </summary>
    class CSBR157371 : BugFixBaseCommand
    {

        #region Variable
        int _loopCount = 0;
        #endregion

        #region Private Property
        private int LoopCount
        {
            get
            {
                return _loopCount;
            }
            set
            {
                _loopCount = _loopCount + value;
            }
        }
        #endregion

        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;
            string _coeFormPath = string.Empty;
            _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1002']/{0}/COE:formElement[@name='CREATION_DATE']"; // Path to check the Rootnode before patcher update.

            string[] formModes = new string[] { "COE:addMode", "COE:editMode"};

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");

                if (id == "4010" || id == "4011" || id == "4012")
                {
                    LoopCount = 1;
                    foreach (string mode in formModes)
                    {
                        string _formPath = string.Format(_coeFormPath, mode);
                        XmlNode rootNode = doc.SelectSingleNode(_formPath, manager);

                        #region Formxml Validations

                        if (rootNode == null)
                        {
                            errorsInPatch = true;
                            messages.Add("CREATION_DATE property is not available to update patch for form [" + id + "] for mode "+ mode +".");
                            continue;
                        }
                        # endregion

                        #region Formxml changes
                        XmlNode InsertAfterNode = doc.SelectSingleNode(_formPath + "/COE:configInfo/COE:fieldConfig/COE:Section508Compliant", manager);
                        XmlNode InsertBeforeNode = doc.SelectSingleNode(_formPath + "/COE:configInfo/COE:fieldConfig/COE:DefaultDate", manager);
                        XmlNode cssCalenderClass = doc.SelectSingleNode(_formPath + "/COE:configInfo/COE:fieldConfig/COE:CSSCalenderClass", manager);

                        // add node if CSS Node doesn't exists
                        if (cssCalenderClass == null)
                        {
                            cssCalenderClass = doc.CreateNode(XmlNodeType.Element, "CSSCalenderClass", "COE.FormGroup");
                            // adding value
                            cssCalenderClass.InnerText = "CalenderClass";
                            if (InsertAfterNode != null)
                            {
                                InsertAfterNode.ParentNode.InsertAfter(cssCalenderClass, InsertAfterNode);
                            }
                            else if (InsertBeforeNode != null)
                            {
                                InsertBeforeNode.ParentNode.InsertBefore(cssCalenderClass, InsertBeforeNode);
                            }
                            else
                            {
                                rootNode.SelectSingleNode("COE:configInfo/COE:fieldConfig", manager).AppendChild(cssCalenderClass);
                            }
                            messages.Add("CSSCalenderClass node added successfully for form [" + id + "].");
                        }
                        else if (string.IsNullOrEmpty(cssCalenderClass.InnerText))
                        {
                            // update value
                            cssCalenderClass.InnerText = "CalenderClass";
                            messages.Add("CSSCalenderClass node added successfully for form [" + id + "].");
                        }
                        else if (cssCalenderClass.InnerText.ToUpper() != "CALENDERCLASS")
                        {
                            // update value
                            cssCalenderClass.InnerText = "CalenderClass";
                            messages.Add("CSSCalenderClass node updated successfully for form [" + id + "].");
                        }
                        else
                        {
                            messages.Add("CSSCalenderClass node is already updated for form [" + id + "].");
                        }
                        #endregion
                    }
                }

                #region Validate Loopcount
                if (LoopCount == 3)
                {
                    LoopCount = -1;
                    break;
                }
                #endregion
            }
            if (!errorsInPatch)
                messages.Add("CSBR157371 was successfully patched");
            else
                messages.Add("CSBR157371 was patched with errors");
            return messages;
        }
    }
}
