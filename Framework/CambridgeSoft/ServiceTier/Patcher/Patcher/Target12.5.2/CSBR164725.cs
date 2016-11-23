using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// patcher for  CSBR-164725.
    /// </summary>
    class CSBR164725 : BugFixBaseCommand
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

                if (id == "4012")
                {
                    //update the Container grid
                    _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='9']/COE:viewMode";
                    XmlNode rootNodeViewModeContainer = doc.SelectSingleNode(_coeFormPath, manager);
                    if (rootNodeViewModeContainer != null)
                    {
                        XmlNode rootNodeContainer = rootNodeViewModeContainer.SelectSingleNode("COE:formElement[@name='ContainersGrid']/COE:configInfo/COE:fieldConfig", manager);
                        XmlNode newChildContainer = rootNodeContainer.OwnerDocument.CreateNode(XmlNodeType.Element, "ViewEnabled", doc.DocumentElement.NamespaceURI);
                        newChildContainer.InnerText = "true";
                        rootNodeContainer.AppendChild(newChildContainer);
                    }
                    //Update the Docmanager grid also
                    _coeFormPath = "//COE:detailsForms/COE:detailsForm[@id='0']/COE:coeForms/COE:coeForm[@id='8']/COE:viewMode";
                    XmlNode rootNodeViewModeDoc = doc.SelectSingleNode(_coeFormPath, manager);
                    if (rootNodeViewModeDoc != null)
                    {
                        XmlNode rootNodeDoc = rootNodeViewModeDoc.SelectSingleNode("COE:formElement[@name='DocGrid']/COE:configInfo/COE:fieldConfig", manager);
                        XmlNode newChildDoc = rootNodeDoc.OwnerDocument.CreateNode(XmlNodeType.Element, "ViewEnabled", doc.DocumentElement.NamespaceURI);
                        newChildDoc.InnerText = "true";
                        rootNodeDoc.AppendChild(newChildDoc);
                    }
                }
                
            }
            if (!errorsInPatch)
                messages.Add("CSBR164725 was successfully patched");
            else
                messages.Add("CSBR164725 was patched with partial update");
            return messages;
        }
    }
}
