using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Web.UI.HtmlControls;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls
{
    /// <summary>
    /// Control to upload files (works with GUIShell variables)
    /// </summary>
    [DefaultProperty("BodyText")]
    [ToolboxData("<{0}:UploadControl runat=server></{0}:UploadControl>")]
    [Description("Displays an upload control.")]
    public class UploadControl : WebControl
    {
        #region Variables

        private bool _hidden = true;
        string _parentClientID = string.Empty;
        string _bindingExpression = string.Empty;
        string _xmlConfig = string.Empty;
        private string _titleBindExp = string.Empty;
        private string _descBindExp = string.Empty;
        private string _titleTextCSSClass= string.Empty;
        private string _titleTextBoxCSSClass= string.Empty;
        private string _descTextCSSClass= string.Empty;
        private string _descTextBoxCSSClass = string.Empty;
        private string _displayMethodName = string.Empty;

        #endregion

        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        private const string _spanFormat = "<span class=\"{0}\">{1}</span>";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UploadControl"/> is hidden.
        /// </summary>
        /// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        /// <summary>
        /// Panel Control Identifier
        /// </summary>
        public string PanelID
        {
            get { return ViewState["EC_PanelID"] != null ? ViewState["EC_PanelID"].ToString() : string.Empty; }
            set
            {
                if (ViewState["EC_PanelID"] != value)
                    ViewState["EC_PanelID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        /// <value>The header text.</value>
        public string HeaderText
        {
            set
            {
                if (ViewState["EC_HeaderText"] != value)
                    ViewState["EC_HeaderText"] = value;
            }
            get
            {
                return ViewState["EC_HeaderText"] == null ? string.Empty : ViewState["EC_HeaderText"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        /// <value>The body text.</value>
        public string BodyText
        {
            set
            {
                if (ViewState["EC_BodyText"] != value)
                    ViewState["EC_BodyText"] = value;
            }
            get
            {
                return ViewState["EC_BodyText"] == null ? string.Empty : ViewState["EC_BodyText"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the footer text.
        /// </summary>
        /// <value>The footer text.</value>
        public string FooterText
        {
            set
            {
                if (ViewState["EC_FooterText"] != value)
                    ViewState["EC_FooterText"] = value;
            }
            get
            {
                return ViewState["EC_FooterText"] == null ? string.Empty : ViewState["EC_FooterText"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the header text class.
        /// </summary>
        /// <value>The header text class.</value>
        public string HeaderTextClass
        {
            set
            {
                if (ViewState["EC_HeaderTextClass"] != value)
                    ViewState["EC_HeaderTextClass"] = value;
            }
            get
            {
                return ViewState["EC_HeaderTextClass"] == null ? string.Empty : ViewState["EC_HeaderTextClass"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the footer text class.
        /// </summary>
        /// <value>The footer text class.</value>
        public string FooterTextClass
        {
            set
            {
                if (ViewState["EC_FooterTextClass"] != value)
                    ViewState["EC_FooterTextClass"] = value;
            }
            get
            {
                return ViewState["EC_FooterTextClass"] == null ? string.Empty : ViewState["EC_FooterTextClass"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the left corner class.
        /// </summary>
        /// <value>The left corner class.</value>
        public string LeftCornerClass
        {
            set
            {
                if (ViewState["EC_LeftCornerClass"] != value)
                    ViewState["EC_LeftCornerClass"] = value;
            }
            get
            {
                return ViewState["EC_LeftCornerClass"] == null ? string.Empty : ViewState["EC_LeftCornerClass"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the right corner class.
        /// </summary>
        /// <value>The right corner class.</value>
        public string RightCornerClass
        {
            set
            {
                if (ViewState["EC_RightCornerClass"] != value)
                    ViewState["EC_RightCornerClass"] = value;
            }
            get
            {
                return ViewState["EC_RightCornerClass"] == null ? string.Empty : ViewState["EC_RightCornerClass"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        /// <value>The title text.</value>
        public string TitleText
        {
            set
            {
                if (ViewState["EC_TitleText"] != value)
                    ViewState["EC_TitleText"] = value;
            }
            get
            {
                return ViewState["EC_TitleText"] == null ? string.Empty : ViewState["EC_TitleText"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets the description text.
        /// </summary>
        /// <value>The description text.</value>
        public string DescriptionText
        {
            set
            {
                if (ViewState["EC_DescriptionText"] != value)
                    ViewState["EC_DescriptionText"] = value;
            }
            get
            {
                return ViewState["EC_DescriptionText"] == null ? string.Empty : ViewState["EC_DescriptionText"].ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
        public bool ShowTitle
        {
            set
            {
                ViewState["EC_ShowTitle"] = value;
            }
            get
            {
                return ViewState["EC_ShowTitle"] == null ? true: (bool)ViewState["EC_ShowTitle"];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show description].
        /// </summary>
        /// <value><c>true</c> if [show description]; otherwise, <c>false</c>.</value>
        public bool ShowDescription
        {
            set
            {
                ViewState["EC_ShowDescription"] = value;
            }
            get
            {
                return ViewState["EC_ShowDescription"] == null ? true : (bool)ViewState["EC_ShowDescription"];
            }
        }

        /// <summary>
        /// Sets the title binding expression.
        /// </summary>
        /// <value>The title binding expression.</value>
        public string TitleBindingExpression
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _titleBindExp = value;
            }
        }

        /// <summary>
        /// Sets the description binding expression.
        /// </summary>
        /// <value>The description binding expression.</value>
        public string DescriptionBindingExpression
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _descBindExp = value;
            }
        }

        /// <summary>
        /// Sets the title text CSS class.
        /// </summary>
        /// <value>The title text CSS class.</value>
        public string TitleTextCSSClass
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _titleTextCSSClass = value;
            }
        }

        /// <summary>
        /// Sets the title text box CSS class.
        /// </summary>
        /// <value>The title text box CSS class.</value>
        public string TitleTextBoxCSSClass
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _titleTextBoxCSSClass = value;
            }
        }

        /// <summary>
        /// Sets the description text CSS class.
        /// </summary>
        /// <value>The description text CSS class.</value>
        public string DescriptionTextCSSClass
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _descTextCSSClass = value;
            }
        }

        /// <summary>
        /// Sets the description text box CSS class.
        /// </summary>
        /// <value>The description text box CSS class.</value>
        public string DescriptionTextBoxCSSClass
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _descTextBoxCSSClass = value;
            }
        }

        /// <summary>
        /// Sets the display name of the method.
        /// </summary>
        /// <value>The display name of the method.</value>
        public string DisplayMethodName
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _displayMethodName = value;
            }
        }

        #endregion

        #region Control Life-Cycle

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadControl"/> class.
        /// </summary>
        /// <param name="controlID">The control ID.</param>
        /// <param name="parentClientID">The parent client ID.</param>
        /// <param name="bindExp">Binding expression for the control</param>
        /// <param name="xmlConfig">Config XML for the control</param>
        public UploadControl(string controlID, string parentClientID, string bindExp, string xmlConfig)
        {
            this.ID = controlID + "UploadControl";
            _parentClientID = parentClientID;
            _bindingExpression = bindExp;
            if (!string.IsNullOrEmpty(xmlConfig))
                this.SetConfigSettings(xmlConfig);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string methodName = this.ID;
            string script = @"
                <script type='text/javascript'>
                YAHOO.namespace('COEUploadCtrl');
                function InitUploadControl() {
                    YAHOO.COEUploadCtrl." + methodName + @" = new YAHOO.widget.Panel('" + methodName + @"', { width:'400px',visible:true,draggable:true,close:true,modal:true,constraintoviewport:true,zIndex:100,context:['" + _parentClientID + @"','tl','br']} );
                    YAHOO.COEUploadCtrl." + methodName + @".setHeader('" + this.HeaderText + @"');
                    YAHOO.COEUploadCtrl." + methodName + @".setBody('" + this.SetBodyText() + @"');
                    YAHOO.COEUploadCtrl." + methodName + @".setFooter('" + this.FooterText + @"');
                    YAHOO.COEUploadCtrl." + methodName + @".render(document.body.form); 
                    YAHOO.COEUploadCtrl." + methodName + @".hide();
                    YAHOO.COEUploadCtrl." + methodName + @".beforeShowEvent.subscribe(ChangeFormSettings);
                    YAHOO.COEUploadCtrl." + methodName + @".beforeShowEvent.subscribe(HideChemDraws);
                    YAHOO.COEUploadCtrl." + methodName + @".beforeHideEvent.subscribe(DefaultFormSettings);
                    YAHOO.COEUploadCtrl." + methodName + @".beforeHideEvent.subscribe(ShowChemDraws);
                }
                function DisplayUploadControl_" + this.ID + @"(){   var Xpos = (YAHOO.util.Dom.getViewportWidth() / 2 ) - 200;
                                                                    var Ypos = (YAHOO.util.Dom.getViewportHeight() / 2 ) - 100;
                                                                    YAHOO.COEUploadCtrl." + this.ID + @".cfg.setProperty('x',Xpos);
                                                                    YAHOO.COEUploadCtrl." + this.ID + @".cfg.setProperty('y',Ypos);
                                                                    YAHOO.COEUploadCtrl." + methodName + @".show();}
                YAHOO.util.Event.addListener(window, 'load', InitUploadControl);";
//            string script = @"
//                <script type='text/javascript'>
//                YAHOO.namespace('COEUploadCtrl');
//                function InitUploadControl() {
//                    var Xpos = (YAHOO.util.Dom.getViewportWidth() / 2 ) - 200;
//                    var Ypos = (YAHOO.util.Dom.getViewportHeight() / 2 ) - 100;
//                    YAHOO.COEUploadCtrl." + methodName + @" = new YAHOO.widget.Panel('" + methodName + @"', { width:'400px',visible:true,draggable:true,close:true,modal:true,constraintoviewport:true,zIndex:100,x:Xpos,y:Ypos} );
//                    YAHOO.COEUploadCtrl." + methodName + @".setHeader('" + this.HeaderText + @"');
//                    YAHOO.COEUploadCtrl." + methodName + @".setBody('" + this.SetBodyText() + @"');
//                    YAHOO.COEUploadCtrl." + methodName + @".setFooter('" + this.FooterText + @"');
//                    YAHOO.COEUploadCtrl." + methodName + @".render(document.body.form); 
//                    YAHOO.COEUploadCtrl." + methodName + @".hide();
//                    YAHOO.COEUploadCtrl." + methodName + @".beforeShowEvent.subscribe(ChangeFormSettings);
//                    YAHOO.COEUploadCtrl." + methodName + @".beforeShowEvent.subscribe(HideChemDraws);
//                    YAHOO.COEUploadCtrl." + methodName + @".beforeHideEvent.subscribe(DefaultFormSettings);
//                    YAHOO.COEUploadCtrl." + methodName + @".beforeHideEvent.subscribe(ShowChemDraws);
//                }
//                function DisplayUploadControl_" + this.ID + @"(){YAHOO.COEUploadCtrl." + methodName + @".show();}
//                YAHOO.util.Event.addListener(window, 'load', InitUploadControl);";

            script += @"function ChangeFormSettings() { 
                            document.forms[0].encoding = 'multipart/form-data';
                        }";
            script += @"function DefaultFormSettings() { 
                            document.forms[0].encoding = 'application/x-www-form-urlencoded';
                        }";
            script += FrameworkUtils.GetHideChemDrawsJSCode() + FrameworkUtils.GetShowChemDrawsJSCode();
            /*
            script += @"function " + methodName + @"SetVarsValues() { 
                        if(document.getElementById('" + this.ClientID + @"UploadFileTitleTxt') != null)
            }";

            if (!string.IsNullOrEmpty(this.GetHiddenControlID(GUIShellTypes.FileUploadTitleCtrlID)) &&
                !string.IsNullOrEmpty(this.GetHiddenControlID(GUIShellTypes.FileUploadDescriptionCtrlID)) &&
                !string.IsNullOrEmpty(this.GetHiddenControlID(GUIShellTypes.FileUploadBindingExpression)))
                script += @"function " + methodName + @"SetVarsValues() { 
           
                            if(document.getElementById('" + this.ClientID + @"UploadFileTitleTxt') != null)
                                if(YAHOO.lang.isString(document.getElementById('" + this.ClientID + @"UploadFileTitleTxt').value))
                                    document.getElementById('" + this.GetHiddenControlID(GUIShellTypes.FileUploadTitleCtrlID) + @"').value = YAHOO.lang.trim(document.getElementById('" + this.ClientID + @"UploadFileTitleTxt').value);
                            if(YAHOO.lang.isValue(document.getElementById('" + this.ClientID + @"UploadFileDescriptionTxt')))
                                if(YAHOO.lang.isString(document.getElementById('" + this.ClientID + @"UploadFileDescriptionTxt').value))
                                    document.getElementById('" + this.GetHiddenControlID(GUIShellTypes.FileUploadDescriptionCtrlID) + @"').value = YAHOO.lang.trim(document.getElementById('" + this.ClientID + @"UploadFileDescriptionTxt').value);
                            
                            }";
            */
            script += "</script>";
             
            writer.Write(script);
        }

        /// <summary>
        /// Gets the hidden control ID.
        /// </summary>
        /// <param name="ctrlID">The Controld ID.</param>
        /// <returns>The found control ID</returns>
        /// <remarks>Relies on GUIShell Controls added in that class</remarks>
        private string GetHiddenControlID(string ctrlID)
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(ctrlID))
                if (this.Context.CurrentHandler != null)
                    if (((Page)this.Context.CurrentHandler).Master.FindControl(GUIShellTypes.MainFormID) != null)
                        if (((Page)this.Context.CurrentHandler).Master.FindControl(GUIShellTypes.MainFormID).FindControl(ctrlID) != null)
                            retVal = ((Page)this.Context.CurrentHandler).Master.FindControl(GUIShellTypes.MainFormID).FindControl(ctrlID).ClientID;
            return retVal;
        }

        /// <summary>
        /// Cleans the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string CleanString(string text)
        {
            return text.Length > 400 ? this.RemoveInvalidChars(text.Substring(0, 400)) + "..." : this.RemoveInvalidChars(text);
        }

        /// <summary>
        /// Removes the invalid chars.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string RemoveInvalidChars(string input)
        {
            return input.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("'", "");
        }

        /// <summary>
        /// Sets the config settings.
        /// </summary>
        /// <param name="xmlDataAsString">The XML data as string.</param>
        private void SetConfigSettings(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode titleBE = xmlData.SelectSingleNode("//COE:TitleBindingExpression", manager);
            if (titleBE != null && titleBE.InnerText.Length > 0)
                _titleBindExp = titleBE.InnerText;

            XmlNode descBE = xmlData.SelectSingleNode("//COE:DescriptionBindingExpression", manager);
            if (descBE != null && descBE.InnerText.Length > 0)
                _descBindExp = descBE.InnerText;

            foreach (XmlNode configSetting in xmlData.SelectNodes("//COE:configSetting", manager))
            {
                if (configSetting != null && configSetting.InnerText.Length > 0)
                {
                    string bindingExpression = configSetting.Attributes["bindingExpression"].Value;
                    if (!string.IsNullOrEmpty(bindingExpression))
                    {
                        try
                        {   //Using COEDatabinder to find an object property given a bindingExpression in the current object.
                            CambridgeSoft.COE.Framework.Controls.COEDataMapper.COEDataBinder dataBinder =
                                new CambridgeSoft.COE.Framework.Controls.COEDataMapper.COEDataBinder(this);
                            dataBinder.SetProperty(bindingExpression, configSetting.InnerText);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the body text.
        /// </summary>
        /// <returns></returns>
        private string SetBodyText()
        {
            string retVal = "<input type=\"file\" class=\"COEUploadControlTxt\" name=\"uploadfile\"/><br>";
            if(this.ShowTitle)
                retVal +=  "<span class=\"" + _titleTextCSSClass  + "\">" + this.TitleText + "</span><input type=\"text\" id=\"" + this.ClientID + "UploadFileTitleTxt\" name=\"Title\" runat=\"server\" class=\"" + _titleTextBoxCSSClass + "\"></textarea><br>";
            if(this.ShowDescription)
                retVal += "<span class=\"" + _descTextCSSClass + "\">" + this.DescriptionText + "</span><textarea id=\"" + this.ClientID + "UploadFileDescriptionTxt\" name=\"Title\" rows=\"5\" cols=\"30\" runat=\"server\" class=\"" + _descTextBoxCSSClass + "\"></textarea><br>";
            retVal += "<input type=\"Submit\" alt\"Upload\" value=\"Upload!\" />";
            return retVal;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);

            if (this.Page.Header != null)
                CambridgeSoft.COE.Framework.Common.FrameworkUtils.AddYUICSSReference(this.Page,
                CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_CSS.CONTAINER);

            if (this.Page.Master != null)
            {
                if (this.Page.Master.FindControl(GUIShellTypes.MainBodyID) != null)
                    if (((HtmlGenericControl)this.Page.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes["class"] == null)
                        ((HtmlGenericControl)this.Page.Master.FindControl(GUIShellTypes.MainBodyID)).Attributes.Add("class", "yui-skin-sam");
            }

        }

        #endregion

    }
}
