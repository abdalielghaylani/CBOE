using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.Controls
{
    [DefaultProperty("BodyText")]
    [ToolboxData("<{0}:ErrorControl runat=server></{0}:ErrorControl>")]
    [Description("Displays an error control.")]
    public class ErrorControl : WebControl
    {
        #region Variables

        private Page _currentPage;
        private MasterPage _currentMasterPage;
        private string _email = string.Empty;
        private string _fullDescription = string.Empty;
        private string _emailSubject = string.Empty;
        private bool _hidden = true;
        private bool _goback = false;
        private bool _hideFooter = false;
        //0806modified
        private string _clipBoardMessage = string.Empty;
        //0806modified
        private bool _showAllMessage = false;
        #endregion

        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        private const string _mailtoFormat = "<a href=\"mailto:{0}?subject={2}&body={1}\">Send report</a>";
        private const string _spanFormat = "<span class=\"{0}\">{1}</span>";
        private const string FULLDESCID = "fulldesc";
        private const string IMAGESRC = "CambridgeSoft.COE.Framework.ServerControls.ErrorControl.Images.Warning_h.png";

        #endregion

        #region Properties

        /// <summary>
        /// If it is true then it will show the whole contents of meesage even if message length is greater than 400
        /// </summary>
        public bool ShowAllMessage
        {
            get { return _showAllMessage; }
            set { _showAllMessage = value; }
        }

        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool GoBackLink
        {
            set { _goback = value; }
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

        public string BodyTextClass
        {
            set
            {
                if (ViewState["EC_BodyTextClass"] != value)
                    ViewState["EC_BodyTextClass"] = value;
            }
            get
            {
                return ViewState["EC_BodyTextClass"] == null ? string.Empty : ViewState["EC_BodyTextClass"].ToString();
            }
        }

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
        /// Email account to send reports
        /// </summary>
        public string EmailAccount
        {
            set
            {
                if (_email != value)
                    _email = value;
            }
        }

        /// <summary>
        /// Email subject
        /// </summary>
        public string EmailSubject
        {
            set
            {
                if (_emailSubject != value)
                    _emailSubject = value;
            }
        }

        /// <summary>
        /// Full error message description
        /// </summary>
        public string ErrorDescription
        {
            set
            {
                if (_fullDescription != value)
                    _fullDescription = value;
            }
        }

        /// <summary>
        /// Sets a value indicating whether [hide footer].
        /// </summary>
        /// <value><c>true</c> if [hide footer]; otherwise, <c>false</c>.</value>
        /// <remarks>By default is false (the footer is shown)</remarks>
        public bool HideFooter
        {
            set
            {
                if (!_hideFooter.Equals(value))
                    _hideFooter = value;
            }
        }

        //0806modified
        /// <summary>
        /// The message set into clipboard
        /// </summary>
        public string ClipBoardMessage
        {
            get
            {
                _clipBoardMessage = _clipBoardMessage.Replace("\r", "&#92;r");
                _clipBoardMessage = _clipBoardMessage.Replace("\n", "&#92;n");
                _clipBoardMessage = _clipBoardMessage.Replace("\t", "&#92;t");
                return _clipBoardMessage;
            }
            set
            {
                this._clipBoardMessage = value;
            }
        }
        //0806modified

        #endregion

        #region Control Life-Cycle

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            if (!this.Hidden)
            {
                Panel errorPanel = new Panel();

                // Fix Coverity: CID-28946 Resource Leak
                try
                {
                    errorPanel.ID = this.ID + "ErrorPanel";

                    Panel header = new Panel();
                    errorPanel.Controls.Add(header);
                    header.CssClass = "hd";
                    header.Style.Add(HtmlTextWriterStyle.FontSize, "100%");
                    Literal headerLiteral = new Literal();
                    headerLiteral.Text = this.SetHeaderText();
                    header.Controls.Add(headerLiteral);

                    Panel body = new Panel();
                    errorPanel.Controls.Add(body);
                    body.CssClass = "bd";
                    Literal bodyLiteral = new Literal();
                    bodyLiteral.Text = this.SetBodyText();
                    body.Controls.Add(bodyLiteral);

                    Panel footer = new Panel();
                    errorPanel.Controls.Add(footer);
                    footer.CssClass = "ft";
                    footer.Style.Add(HtmlTextWriterStyle.FontSize, "100%");
                    if (!_hideFooter)
                    {
                        Literal footerLiteral = new Literal();
                        footerLiteral.Text = this.SetFooterText();
                        footer.Controls.Add(footerLiteral);
                    }
                    //autofillheight: 'body'
                    //this.Controls.Add(errorPanel);
                    errorPanel.RenderControl(writer);

                    string script = @"
                <script type='text/javascript'>
                YAHOO.namespace('COEErrorCtrl');
                function InitErrorControl() {
                    var Xpos = (YAHOO.util.Dom.getViewportWidth() / 2 ) - 200;
                    var Ypos = 150;
                    YAHOO.COEErrorCtrl." + this.ClientID + @" = new YAHOO.widget.Panel('" + errorPanel.ClientID + @"', { width:'402px',visible:true, draggable:true,close:true,modal:true,constraintoviewport:true,zIndex:100,x:Xpos,y:Ypos} );
                    YAHOO.COEErrorCtrl." + this.ClientID + @".render(); 
                    YAHOO.COEErrorCtrl." + this.ClientID + @".show();
                    YAHOO.COEErrorCtrl." + this.ClientID + @".beforeHideEvent.subscribe(ShowChemDraws);}
                YAHOO.util.Event.addListener(window, 'load', InitErrorControl);
                YAHOO.util.Event.addListener(window, 'load', HideChemDraws);";
                    script += "YAHOO.COEErrorCtrl.tt1 = new YAHOO.widget.Tooltip(\"tt1\", { preventoverlap:false,constraintoviewport:true,iframe:true,context:\"" + FULLDESCID + "\" });";

                    script += @"function ParseStr(str) 
{ 
if(str!=null && str.length>0) 
{ 
if(str.indexOf('&#92;f')!=-1) 
str=str.replace(/&#92;f/ig, '\f'); 
if(str.indexOf('&#92;n')!=-1) 
str=str.replace(/&#92;n/ig, '\n'); 
if(str.indexOf('&#92;r')!=-1) 
str=str.replace(/&#92;r/ig, '\r'); 
if(str.indexOf('&#92;t')!=-1) 
str=str.replace(/&#92;t/ig, '\t'); 
} 
return str; 
} ";

                    script += "function CopyToClipboard() { window.clipboardData.setData('Text', ParseStr(\"" + this.ClipBoardMessage.Replace('\"', '\'') + "\"));}";
                    script += @"function HideChemDraws() {
                            if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
                                for(i = 0; i < cd_objectArray.length; i++) {
                                    cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'hidden';
                                }
                            }
                        }";
                    script += @"function ShowChemDraws() {
                            if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
                                for(i = 0; i < cd_objectArray.length; i++) {
                                    cd_getSpecificObject(cd_objectArray[i]).style.visibility = 'visible';
                                }
                            }
                        }";
                    script += "</script>";
                    writer.Write(script);
                }
                finally
                {
                    errorPanel.Dispose();
                }
            }
        }

        /// <summary>
        /// Sets the footer text.
        /// </summary>
        /// <returns></returns>
        private string SetFooterText()
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(_email))
                retVal += string.Format(_mailtoFormat, _email, this.CleanString(_fullDescription), _emailSubject);
            if (!string.IsNullOrEmpty(_fullDescription))
            {
                //Need a way to change the z-index.
                retVal += " | <a id=\"" + FULLDESCID + "\" style=\"cursor:pointer;\" title=\"" + this.CleanString(_fullDescription) + "\">Full Description</a>";
                retVal += " | <a id=\"CopyClipBoard\" style=\"cursor:pointer;\" onclick=\"CopyToClipboard();\">Copy to Clipboard</a>";
            }
            if (_goback)
                retVal += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id=\"GoBack\" style=\"cursor:pointer;\" onclick=\"history.back(-1);\">Go back</a>";

            return retVal;
        }

        private string CleanString(string text)
        {
            if (!ShowAllMessage)
                return text.Length > 400 ? this.RemoveInvalidChars(text.Substring(0, 400)) + "..." : this.RemoveInvalidChars(text);
            else // it will show the whole message
                return text;
        }

        private string RemoveInvalidChars(string input)
        {
            return input.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("'", "");
        }

        /// <summary>
        /// Sets the header text and cssclass
        /// </summary>
        /// <returns></returns>
        private string SetHeaderText()
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(this.HeaderText))
                retVal += !string.IsNullOrEmpty(this.HeaderTextClass) ? string.Format(_spanFormat, this.HeaderTextClass, this.HeaderText) : this.HeaderText;
            return retVal;
        }

        /// <summary>
        /// Sets the body text.
        /// </summary>
        /// <returns></returns>
        public virtual string SetBodyText()
        {
            string retVal = "<img alt=\"Warning\" id=\"warningImage\" src=\"" + Page.ClientScript.GetWebResourceUrl(this.GetType(), IMAGESRC) + "\" />&nbsp;&nbsp;";
            if (!string.IsNullOrEmpty(this.HeaderText))
                retVal += !string.IsNullOrEmpty(this.BodyTextClass) ? string.Format(_spanFormat, this.BodyTextClass, this.CleanString(this.BodyText)) : this.CleanString(this.BodyText);
            return retVal;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.RegisterYUIScript(this.Page,
                                                                    CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.RegisterYUIScript(this.Page,
                                                                    CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_JS.DRAGDROPMIN);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.RegisterYUIScript(this.Page,
                                                                    CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_JS.CONTAINERMIN);

            if (this.Page.Header != null)
                FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.CONTAINER);


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
