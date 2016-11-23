using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  changelist # 441393 .
    /// </summary>
    class CSBR156748 : BugFixBaseCommand
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

            

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");



                if (id == "4014" || id == "4010" || id == "4011" || id == "4012" || id == "4013" || id == "4015")
                {
                    LoopCount = 1;

                    string[] modes = new string[] { "COE:addMode", "COE:editMode", "COE:viewMode" };
                    foreach (string mode in modes)
                    {
                        _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='1003']/" + mode + "/COE:formElement[@name='']/COE:configInfo/COE:fieldConfig/COE:tables/COE:table/COE:Columns/COE:Column[@name='Component']"; // Path to check the Rootnode before patcher update.

                        XmlNode rootNode = doc.SelectSingleNode(_coeFormPath, manager);
                        #region Formxml Validations
                        if (rootNode == null)
                        {
                            errorsInPatch = true;
                            messages.Add("Component column is not available to update patch for form [" + id + "] for mode " + mode + ".");
                            continue;
                        }
                        # endregion

                        #region Formxml changes

                        XmlNode idNode = doc.SelectSingleNode(_coeFormPath + "/COE:formElement/COE:Id", manager);
                        XmlNode bindingExpression = doc.SelectSingleNode(_coeFormPath + "/COE:formElement/COE:bindingExpression", manager);
                        //ID
                        if (string.IsNullOrEmpty(idNode.InnerText))
                        {
                            // update value
                            idNode.InnerText = "DisplayKey";
                            messages.Add("Id node updated successfully for form [" + id + "] for mode " + mode + ".");
                        }
                        else if (idNode.InnerText.ToUpper() != "DISPLAYKEY")
                        {
                            // update value
                            idNode.InnerText = "DisplayKey";
                            messages.Add("Id node updated successfully for form [" + id + "] for mode " + mode + ".");
                        }
                        else
                            messages.Add("Id node already updated for form [" + id + "] for mode " + mode + ".");

                        //BINDINGEXPRESSION
                        if (string.IsNullOrEmpty(bindingExpression.InnerText))
                        {
                            // update value
                            bindingExpression.InnerText = "DisplayKey";
                            messages.Add("BindingExpression node updated successfully for form [" + id + "].");
                        }
                        else if (bindingExpression.InnerText.ToUpper() != "DISPLAYKEY")
                        {
                            // update value
                            bindingExpression.InnerText = "DisplayKey";
                            messages.Add("BindingExpression node updated successfully for form [" + id + "] for mode " + mode + ".");
                        }
                        else
                            messages.Add("BindingExpression node already updated for form [" + id + "] for mode " + mode + ".");
                        #endregion

                    }
                   
                }
                #region Validate Loopcount
                if (LoopCount == 6)
                {
                    LoopCount = -6;
                    break;
                }
                #endregion

                
            }
            if (!errorsInPatch)
                messages.Add("CSBR156748 was successfully patched");
            else
                messages.Add("CSBR156748 was patched with partial update");
            return messages;
        }
    }
}
