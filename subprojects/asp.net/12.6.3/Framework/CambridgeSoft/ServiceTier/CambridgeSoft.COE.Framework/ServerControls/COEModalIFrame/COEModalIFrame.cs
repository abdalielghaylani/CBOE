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
    /// <summary>
    /// <para>
    /// This class implements a YUI Modal Panel which embeds an iframe control.
    /// </para>
    /// <para>
    /// The purpose of this control is to be able to embed other apps such as CBV or Reg into an iframe and stay on the same screen context (since is amodal screen).
    /// Inside the modal body, the developer has to implement ways to return back to the main application (such as hyperlinks). 
    /// The control provides three main section:
    /// </para>
    /// <para>
        /// <list type="bullet">
            ///<item>Header: A title for the windows</item>
            ///<item>Body: the iframe container where the content of the embedded app is shown</item>
            ///<item>Footer: Another text to explain or add details to the screen</item>
        ///</list> 
    /// </para>
    /// <para>
    /// <b>Usage (example):</b>
    /// </para>
    /// <para>
    ///     In your aspx page add the following:
    /// &lt;%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls" TagPrefix="cc2" %&gt;  
    /// &lt;cc2:COEModalIFrame ID="ModalYUI" runat="server" BodyURL="/ChemSource/Forms/Search/MaterialsSearch.aspx?mt=1" HeaderText="Add additional material"  Width="600" Height="800" /&gt;    
    /// One more thing you must do is to attach the opening action of the modal panel with a control. i.e: when clicking in a hyperlink:
    ///     this.ModalYUI.ControlIDToBind = this.AddMatLink.ClientID;
    /// </para>
    /// </summary>
    [DefaultProperty("BodyURL")]
    [ToolboxData("<{0}:COEModalIframe runat=server></{0}:COEModalIframe>")]
    [Description("Displays a modal panel control that renders an iframe inside.")]
    public class COEModalIFrame : WebControl
    {
        #region Variables

        private Page _currentPage;
        private MasterPage _currentMasterPage;
        private bool _hideFooter = true;
        
        #endregion

        #region Constants
        
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        private const string _spanFormat = "<span class=\"{0}\">{1}</span>";
        private const string _iframeFormat = @"<iframe frameborder=""0"" width=""{0}px"" height=""{1}px"" src=""{2}""></iframe>";
        private const string _defPaneWidth = "800";
        private const string _defPaneHeight = "600";
        private const string _defaultCloseFunctionName = "_CloseModalIframe";
        private const string _defaultOpenFunctionName = "_OpenModalIframe";
        private const string _defaultModalSettings = "visible:true,modal:true,fixedcenter:true,close:true,zindex:100,constraintoviewport:true,draggable:true";
        private const string PREFIX_V = "MIC_";
        
        #endregion

        #region Properties

        /// <summary>
        /// Text for the header of the container
        /// </summary>
        [
        Bindable(true),
        Description("Text of the header of the panel container"),
        Category("Default"),
        DefaultValue(""),
        ]
        public string HeaderText
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "HeaderText"] != value)
                    ViewState[PREFIX_V + "HeaderText"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "HeaderText"] == null ? string.Empty : ViewState[PREFIX_V + "HeaderText"].ToString();
            }
        }

        /// <summary>
        /// URL of the page to embed inside the pane container
        /// </summary>
        [
        Bindable(true),
        Description("URL of the iframe to display in the body"),
        Category("Default"),
        DefaultValue(""),
        ]
        public string BodyURL
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "BodyURL"] != value)
                    ViewState[PREFIX_V + "BodyURL"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "BodyURL"] == null ? string.Empty : ViewState[PREFIX_V + "BodyURL"].ToString();
            }
        }

        /// <summary>
        /// Text to display in the footer of the container
        /// </summary>
        [
        Bindable(true),
        Description("Text to display in the footer of the container"),
        Category("Default"),
        DefaultValue(""),
        ]
        public string FooterText
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "FooterText"] != value)
                    ViewState[PREFIX_V + "FooterText"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "FooterText"] == null ? string.Empty : ViewState[PREFIX_V + "FooterText"].ToString();
            }
        }

        /// <summary>
        /// CSS Class to apply to the header text
        /// </summary>
        [
        Bindable(true),
        Description("Header text CSS Class name"),
        Category("Appearance"),
        DefaultValue(""),
        ]
        public string HeaderTextClass
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "HeaderTextClass"] != value)
                    ViewState[PREFIX_V + "HeaderTextClass"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "HeaderTextClass"] == null ? string.Empty : ViewState[PREFIX_V + "HeaderTextClass"].ToString();
            }
        }

        /// <summary>
        /// CSS Classs to apply to the footer text
        /// </summary>
        [
        Bindable(true),
        Description("Footer text CSS Class name"),
        Category("Appearance"),
        DefaultValue(""),
        ]
        public string FooterTextClass
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "FooterTextClass"] != value)
                    ViewState[PREFIX_V + "FooterTextClass"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "FooterTextClass"] == null ? string.Empty : ViewState[PREFIX_V + "FooterTextClass"].ToString();
            }
        }

        /// <summary>
        /// Sets a value indicating whether [hide footer].
        /// </summary>
        /// <value><c>true</c> if [hide footer]; otherwise, <c>false</c>.</value>
        /// <remarks>By default is false (the footer is shown)</remarks>
        [
        Bindable(true),
        Description("Hide footer text"),
        Category("Default"),
        DefaultValue("true"),
        ]
        public bool HideFooter
        {
            set
            {
                if (!_hideFooter.Equals(value))
                    _hideFooter = value;
            }
        }

        /// <summary>
        /// Width of the iframe container (content)
        /// </summary>
        [
        Bindable(true),
        Description("Width of the body pane container"),
        Category("Appearance"),
        DefaultValue(""),
        ]
        public string PaneInsideWidth
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "IframeWidth"] != value)
                    ViewState[PREFIX_V + "IframeWidth"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "IframeWidth"] == null ? _defPaneWidth : ViewState[PREFIX_V + "IframeWidth"].ToString();
            }
        }

        /// <summary>
        /// Height of the iframe container (content)
        /// </summary>
        [
        Bindable(true),
        Description("Height of the body pane container"),
        Category("Appearance"),
        DefaultValue(""),
        ]
        public string PaneInsideHeight
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "IframeHeight"] != value)
                    ViewState[PREFIX_V + "IframeHeight"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "IframeHeight"] == null ? _defPaneHeight : ViewState[PREFIX_V + "IframeHeight"].ToString();
            }
        }

        /// <summary>
        /// Window closing js method name
        /// </summary>
        //public string CloseJSMethodName
        //{
        //    get { return this.ID + _defaultCloseFunctionName; }
        //}

        ///// <summary>
        ///// Windo opening js method name
        ///// </summary>
        //public string OpenJSMethodName
        //{
        //    get { return this.ID + _defaultOpenFunctionName; }
        //}

        /// <summary>
        /// YUI Panel default settings
        /// </summary>
        /// <remarks> Check YAHOO.widget.Panel</remarks>
        [
        Bindable(true),
        Description("Configuration settings (YAHOO.widget.Panel) for the modal panel"),
        Category("Appearance"),
        ]
        public string ModalPanelSettings
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "ModalSettings"] != value)
                    ViewState[PREFIX_V + "ModalSettings"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "ModalSettings"] == null ? _defaultModalSettings : ViewState[PREFIX_V + "ModalSettings"].ToString();
            }
        }

        /// <summary>
        /// Client control ID to bind and open the panel on click
        /// </summary>
        [
        Bindable(true),
        Description("Client control ID to bind and open the panel on click"),
        Category("Default"),
        DefaultValue(""),
        ]
        public string ControlIDToBind
        {
            set
            {
                this.EnsureChildControls();
                if (ViewState[PREFIX_V + "ControlIDToBind"] != value)
                    ViewState[PREFIX_V + "ControlIDToBind"] = value;
            }
            get
            {
                this.EnsureChildControls();
                return ViewState[PREFIX_V + "ControlIDToBind"] == null ? string.Empty : ViewState[PREFIX_V + "ControlIDToBind"].ToString();
            }
        }
    
        #endregion

        #region Control Life-Cycle

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            //Now crete the control but do not show it (that is done calling the open JS method
            //It's implemented to load the iframe after the user clicks, but could be changed so we load the pane and just change the visibility of the pane.
            //The disadvantage is that it takes some time to load "heavy" apps such as CBV.
            //I.E  function " + this.OpenJSMethodName + @"(){ YAHOO.COEModIframeCtrl." + this.ClientID + @".show(); return;}              

            string script = @"
                <script type='text/javascript'>
                YAHOO.namespace('COEModIframeCtrl');
                function InitModalIframeControl_" + this.ClientID + @"(urlOverride, headerTextOverride, manageChemDrawVisibility) {
                    YAHOO.COEModIframeCtrl." + this.ClientID + @" = new YAHOO.widget.Panel('ModalPanel', {" + this.ModalPanelSettings + @"} );
                    if(typeof(headerTextOverride) == 'string')
                        YAHOO.COEModIframeCtrl." + this.ClientID + @".setHeader(headerTextOverride);
                    else    
                        YAHOO.COEModIframeCtrl." + this.ClientID + @".setHeader('" + this.SetHeaderText() + @"');

                    if(typeof(urlOverride) == 'string')
                        YAHOO.COEModIframeCtrl." + this.ClientID + @".setBody('" + string.Format(@"<iframe frameborder=""0"" width=""{0}px"" height=""{1}px"" src=""{2}""></iframe>", this.PaneInsideWidth, this.PaneInsideHeight, "' + urlOverride + '") + @"');
                    else
                        YAHOO.COEModIframeCtrl." + this.ClientID + @".setBody('" + this.SetBodyText() + @"');";
            if (!this._hideFooter)
                script += @"YAHOO.COEModIframeCtrl." + this.ClientID + @".setBody('" + this.SetFooterText() + @"');";
            script += @"YAHOO.COEModIframeCtrl." + this.ClientID + @".render(document.body); 

                    YAHOO.COEModIframeCtrl." + this.ClientID + @".beforeHideEvent.subscribe(function () {
                           document.getElementById('ChemBioPluginContainer').removeAttribute('style');
                       });

                    if((typeof(manageChemDrawVisibility) == 'boolean') && manageChemDrawVisibility)
                    {
                        YAHOO.COEModIframeCtrl." + this.ClientID + @".hideEvent.subscribe(function() {
                            if(typeof(ShowChemDraws) == 'function') ShowChemDraws();
                        });
                        YAHOO.COEModIframeCtrl." + this.ClientID + @".showEvent.subscribe(function() {
                            if(typeof(HideChemDraws) == 'function') HideChemDraws();
                        });
                    }

                    YAHOO.COEModIframeCtrl." + this.ClientID + @".show();
                }
                function CloseCOEModalIframe() { YAHOO.COEModIframeCtrl." + this.ClientID + @".hide(); }";
            if(!string.IsNullOrEmpty(this.ControlIDToBind))
                script += @"YAHOO.util.Event.addListener(""" + this.ControlIDToBind + @""", 'click', InitModalIframeControl_" + this.ClientID + ");";
            script += "</script>";
            writer.Write(script);
        }

        /// <summary>
        /// Sets the footer text.
        /// </summary>
        /// <returns></returns>
        internal virtual string SetFooterText()
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(this.FooterText))
                retVal += !string.IsNullOrEmpty(this.FooterTextClass) ? string.Format(_spanFormat, this.FooterTextClass, this.FooterText) : this.FooterText;
            return retVal;
        }

        /// <summary>
        /// Sets the header text and cssclass
        /// </summary>
        /// <returns></returns>
        internal virtual string SetHeaderText()
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
        internal virtual string SetBodyText()
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(this.BodyURL))
                retVal += string.Format(_iframeFormat, this.PaneInsideWidth, this.PaneInsideHeight, this.BodyURL);
            return retVal; 
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
        }

        #endregion
    }
}
