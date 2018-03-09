using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update missing xml configuration in 4006.xml.
    /// </summary>
    class EN4247 : BugFixBaseCommand
    {

        #region Variables
        XmlNode _parentNode;
        string _nameSpaceURI = string.Empty;
        bool _errorsInPatch = false;
        XmlDocument _formDoc = new XmlDocument();
        XmlNamespaceManager _manager;
        #endregion


        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            string coeFormPath = string.Empty;

            #region GetFormDoc & Namespace
            for (int i = 0; i < forms.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(forms[i]);
                string id = _formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["id"].Value;
                if (id == "4003")
                    break;
            }
            #endregion
                    
            #region manager
            _manager = new XmlNamespaceManager(_formDoc.NameTable);
            _manager.AddNamespace("COE", "COE.FormGroup");
            _nameSpaceURI = "COE.FormGroup";
            #endregion

            coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='BATCH_PROJECT']/COE:configInfo/COE:fieldConfig/COE:dropDownItemsSelect";
            _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
            if (_parentNode != null)
            {
                _parentNode.InnerText = "SELECT PROJECTID as key, NAME as value FROM REGDB.VW_PROJECT WHERE (TYPE ='B' OR TYPE='A') AND (ACTIVE = 'T' OR ACTIVE = 'F')";
                messages.Add("Form[4003]: dropDownItemsSelect node was updated successfully.");
            }
            else
            {
                _errorsInPatch = true;
                messages.Add("Form[4003]: Not able to update BATCH_PROJECT form element as its not available.");
            }

            if (!_errorsInPatch)
                messages.Add("EN4247 ISSUE was fixed successfully .");
            else
                messages.Add("EN4247 ISSUE was not fixed due to errors.");
            return messages;
        }
    }
}
