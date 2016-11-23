using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Patch to update missing xml configuration in 4006.xml from upgrade [1210_1250].
    /// </summary>
    class EN3365: BugFixBaseCommand
    {

        #region Variables
        XmlNode _parentNode;
        XmlNode _newNode;
        XmlNode _insertBefore;
        XmlNode _insertAfter;
        string _newElementPath = string.Empty;
        string _newElementAttributes = string.Empty;
        string _nameSpaceURI = string.Empty;
        const string PREFIX = "COE:";
        bool _errorsInPatch = false;
        XmlDocument _formDoc = new XmlDocument();
        XmlNamespaceManager _manager;
        string _innerText = string.Empty;
        string _innerXml = string.Empty;
        Dictionary<string, string> _listAttributes = new Dictionary<string, string>();
        #endregion


        #region Property
        private string InnerText
        {
            get
            {
                return _innerText;
            }
            set
            {
                _innerText = value;
            }
        }

        private string NewElementAttributes
        {
            get
            {
                return _newElementAttributes;
            }
            set
            {
                _newElementAttributes = value;
            }
        }

        private string InnerXml
        {
            get
            {
                return _innerXml;
            }
            set
            {
                _innerXml = value;
            }
        }

        private Dictionary<string, string> ListAttributes
        {
            get
            {
                return _listAttributes;
            }
        }

        #endregion


        public override List<string> Fix(List<System.Xml.XmlDocument> forms, List<System.Xml.XmlDocument> dataviews, List<System.Xml.XmlDocument> configurations, System.Xml.XmlDocument objectConfig, System.Xml.XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            string coeFormPath = string.Empty;
            string status = string.Empty;

            #region GetFormDoc & Namespace
            for (int i = 0; i < forms.Count; i++) // Loop through List with for
            {
                _formDoc = (XmlDocument)(forms[i]);
                string id = _formDoc.DocumentElement.Attributes["id"] == null ? string.Empty : _formDoc.DocumentElement.Attributes["id"].Value;
                if (id == "4006")
                {
                    
                    #region manager
                    _manager = new XmlNamespaceManager(_formDoc.NameTable);
                    _manager.AddNamespace("COE", "COE.FormGroup");
                    _nameSpaceURI = "COE.FormGroup";
                    #endregion

                    #region DATECREATED Registry level
                    coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']/COE:layoutInfo/COE:formElement[@name='DATECREATED']";
                    status = ReplaceNullDateText(coeFormPath,"DATECREATED");
                    messages.Add(status);
                    #endregion

                    #region DATECREATED Batch level
                    coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='DATECREATED']";
                    status = ReplaceNullDateText(coeFormPath, "DATECREATED");
                    messages.Add(status);
                    #endregion

                    #region DATECREATED Batch level
                    coeFormPath = "//COE:queryForms/COE:queryForm[@id='0']/COE:coeForms/COE:coeForm[@id='2']/COE:layoutInfo/COE:formElement[@name='CREATION_DATE']";
                    status = ReplaceNullDateText(coeFormPath, "CREATION_DATE");
                    messages.Add(status);
                    #endregion

                    break;
                }
            }
            

            #endregion

            if (!_errorsInPatch)
                messages.Add("EN3365 ISSUE was fixed successfully .");
            else
                messages.Add("EN3365 ISSUE was fixed with partial update.");
            return messages;
        }

        #region Private Function & Method
        
        private string ReplaceNullDateText(string coeFormPath, string dateElement)
        {
            try
            {
                XmlNode dateLabelNode;
                _parentNode = _formDoc.SelectSingleNode(coeFormPath, _manager);
                if (_parentNode != null)
                {

                    dateLabelNode = _parentNode.SelectSingleNode("COE:configInfo/COE:fieldConfig/COE:NullDateLabel",_manager);
                    if (dateLabelNode != null)
                    {
                        dateLabelNode.InnerText = "";
                        return "Form[4006]: " + dateElement + " was fixed successfully.";
                    }
                    else
                    {
                        _errorsInPatch = true;
                        return "Form[4006]: " + dateElement + " was not updated due to errors.";
                    }
                }
                else
                {
                    _errorsInPatch = true;
                    return "Form[4006]: " + dateElement + " was not updated due to errors.";
                }
            }
            catch (Exception ex)
            { _errorsInPatch = true; return ex.Message; }
        }

        #endregion
    }
}
