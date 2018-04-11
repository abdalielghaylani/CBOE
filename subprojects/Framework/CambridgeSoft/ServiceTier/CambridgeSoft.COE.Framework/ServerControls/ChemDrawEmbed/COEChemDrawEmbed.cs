using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web;

namespace CambridgeSoft.COE.Framework.Controls.ChemDraw
{     
    /// <summary>
    /// <para>
    /// This class is used to render an ChemDraw activex, which basically allows to draw and analyze chemical structures, as well as 
    /// data formatting.
    /// This server controls has plenty of the functionality running on client side, but that means that ChemDraw activex <b>must</b> be
    /// installed on the client machine.
    /// </para>
    /// <para>
    /// Other important consideration is that the chemdraw needs to be activated to a PRO license in order to be fully functional. The NET
    /// version of it may cause the control to behave in an unexpected manner.
    /// </para>
    /// <para><b>Example:</b></para>
    /// <para><i>Adding the control in the html source is as easy as follows:</i></para>
    /// <code lang="HTML/.NET">
    /// &lt;%@ Page Language="C#" ...  %&gt;
    /// &lt;%@ Register Assembly="CambridgeSoft.COE.Framework"
    /// Namespace="CambridgeSoft.COE.Framework.Controls.ChemDraw" TagPrefix="cc1" %&gt;
    /// 
    /// ...
    /// 
    /// &lt;cc1:COEChemDrawEmbed id="COEChemDrawEmbedControl" runat="server"&gt;&lt;/cc1:COEChemDrawEmbed&gt;
    /// 
    /// ...
    /// </code>
    /// 
    /// <para><i>And to include the control programatically is as simple as:</i></para>
    /// <code lang="C#">
    /// COEChemDrawEmbed ChemDrawControl = new COEChemDrawEmbed();
    /// ChemDrawControl.ID = "COEChemDrawEmbedControl";
    /// ChemDrawControl.InlineData = "Some base64 structure representation";
    /// </code>
    /// </summary>
    [DefaultProperty("Src")]
    [ToolboxData("<{0}:COEChemDrawEmbed runat=server></{0}:COEChemDrawEmbed>")]
    [Description("Displays the ChemOfice plugin application.")]
    [ToolboxBitmap(typeof(COEChemDrawEmbed), "ChemDrawEmbed.bmp")]
    public class COEChemDrawEmbed : CompositeControl
    {

        #region Public Properties
        /// <summary>
        /// Gets or sets the mime type of the data passed to the control. <seealso cref="MimeTypes"/>
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(MimeTypes.xcdx)]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Localizable(true)]
        [Description("The mime type of the data passed to the control")]
        public MimeTypes MimeType
        {
            get
            {
                object mt = ViewState["MimeType"];
                if(mt != null)
                {
                    return (MimeTypes)ViewState["MimeType"];
                }

                return MimeTypes.xcdx;
            }

            set
            {
                ViewState["MimeType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the chem draw source to display
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        [Description("The chem draw source to display")]
        public string Src
        {
            get
            {
                string s = (string) ViewState["Src"];
                return ((s == null)? string.Empty : s);
            }

            set
            {
                ViewState["Src"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the control.
        /// </summary>
        [Bindable(true)]
        [Category("Misc")]
        [DefaultValue("")]
        [Browsable(true)]
        [Description("Gets or sets the name of the control.")]
        public string Name
        {
            get
            {
                string s = (string) ViewState["Name"];
                return ((s == null)? string.Empty : s) ;
            }

            set
            {
                ViewState["Name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the control is in mode View only.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Browsable(true)]
        [Description("Gets or sets if the control is in mode View only.")]
        public bool ViewOnly
        {
            get
            {
                bool b = (bool) (ViewState["ViewOnly"] == null? false : ViewState["ViewOnly"]);
                return b;
            }

            set
            {
                ViewState["ViewOnly"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the docking reference id.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Browsable(true)]
        [Description("Gets or sets the docking reference id.")]
        public string DockingReferenceID
        {
            get
            {
                string s = (string) ViewState["DockingReferenceID"];
                return ((s == null)? string.Empty : s);
            }

            set
            {
                ViewState["DockingReferenceID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the data URL.
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Browsable(true)]
        [Description("Gets or sets the data URL.")]
        public string DataURL
        {
            get
            {
                string s = (string) ViewState["DataURL"];
                return ((s == null)? string.Empty : s);
            }

            set
            {
                ViewState["DataURL"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the data inline.
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DefaultValue("")]
        [Browsable(true)]
        [Description("Gets or sets the data inline.")]
        public string InlineData
        {
            get
            {
                string s = (string) ViewState["InlineData"];
                return ((s == null)? string.Empty : s);
            }

            set
            {
                ViewState["InlineData"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the control should shrink to fit.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Browsable(true)]
        [Description("Gets or sets if the control should shrink to fit.")]
        public bool ShrinkToFit
        {
            get
            {
                bool b = (bool) (ViewState["ShrinkToFit"] == null? false: ViewState["ShrinkToFit"]);
                return b;
            }

            set
            {
                ViewState["ShrinkToFit"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the control shouldnt be catched.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Browsable(true)]
        [Description("Gets or sets if the control shouldnt be catched.")]
        public bool DontCache
        {
            get
            {
                bool b = (bool) (ViewState["DontCache"] == null? false : ViewState["DontCache"]);
                return b;
            }

            set
            {
                ViewState["DontCache"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the control can be edited out of place.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Browsable(true)]
        [Description("Gets or sets if the control can be edited out of place.")]
        public bool EditOutOfPlace
        {
            get
            {
                bool b = (bool) (ViewState["EditOutOfPlace"] == null? true : ViewState["EditOutOfPlace"]);
                return b;
            }

            set
            {
                ViewState["EditOutOfPlace"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if the tools should be shown when the control is visible.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Browsable(true)]
        [Description("Gets or sets if the tools should be shown when the control is visible.")]
        public bool ShowToolsWhenVisible
        {
            get
            {
                bool b = (bool) (ViewState["ShowToolsWhenVisible"] == null? true : ViewState["ShowToolsWhenVisible"]);
                return b;
            }
            set
            {
                ViewState["ShowToolsWhenVisible"] = value;
            }
        }

        public string OutputData {
            get
            {
                try
                {
                    EnsureChildControls();

                    return _outputData.Value;
                }
                catch (Exception e)
                {
                    return _nullOutput;
                }
            }
        }
        #endregion

        #region Private Properties
        private string PluginDownloadURL
        {
            get
            {
                string url = string.Empty;

                if(Page != null)
                {
                    if(Page.Session["PluginDownloadURL"] != null)
                        url = (string) Page.Session["PluginDownloadURL"];
                }
                else
                {
                    if(((Page) HttpContext.Current.CurrentHandler).Session["PluginDownloadURL"] != null)
                        url = (string) ((Page) HttpContext.Current.CurrentHandler).Session["PluginDownloadURL"];
                }

                return url;
            }
        }
        #endregion

        #region Overriden Properties
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        #region Hidden Properties (not browsables in property editor)
        [Bindable(false)]
        [Browsable(false)]
        public override FontInfo Font
        {
            get
            {
                return base.Font;
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        #endregion
        #endregion

        #region Variables
        private string _errorMessage = string.Empty;
        private HtmlInputHidden _outputData;
        #endregion

        #region Constants
        private const string _mimeType = "chemical/x-cdx";
        private const string _chemdrawjs = "chemdraw";
        private const string _cdlibjs = "cdlib";
        private const string _changeinlinejs = "changeInlines";
        private const string _chemofficeenterprisejs = "ChemOfficeEnterprise";
        private const string _nullOutput = "null";
        private const string CHEMDRAWFOLDER = "/COECommonResources/ChemDraw/";
        #endregion

        #region Constructors
        public COEChemDrawEmbed()
        {
            this.Src = string.Empty;
            this.ViewOnly = false;
            this.DataURL = string.Empty;
            this.DockingReferenceID = string.Empty;
            this.DontCache = false;
            this.EditOutOfPlace = false;
            this.ShowToolsWhenVisible = true;
            this.ShrinkToFit = false;
            this.Width = new Unit("200px");
            this.Height = new Unit("200px");
            this.InlineData = string.Empty;
            this.MimeType = MimeTypes.xcdx;
            
        }
        #endregion

        #region Overriden Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            _outputData = new HtmlInputHidden();
            _outputData.ID = "Output";
            _outputData.Value = _nullOutput;
            this.Controls.Clear();
            this.Controls.Add(_outputData);
        }
        protected override void OnPreRender(EventArgs e)
        {
            int currentUsing = 0;

            // Coverity Fix CID : 11823 : link object was declared but not used.

            //System.Web.UI.HtmlControls.HtmlGenericControl link = new System.Web.UI.HtmlControls.HtmlGenericControl("script");

            string url = CHEMDRAWFOLDER + "chemdraw.js";

            if(!Page.ClientScript.IsClientScriptBlockRegistered(typeof(COEChemDrawEmbed), "chemdrawDownloadURL") && !string.IsNullOrEmpty(this.PluginDownloadURL))
                Page.ClientScript.RegisterClientScriptBlock(typeof(COEChemDrawEmbed), "chemdrawDownloadURL", "CD_AUTODOWNLOAD_ACTIVEX = CD_AUTODOWNLOAD_PLUGIN = '" + this.PluginDownloadURL + "';", true);

            if(!Page.ClientScript.IsClientScriptIncludeRegistered(_chemdrawjs))
            {
                Page.ClientScript.RegisterClientScriptInclude(_chemdrawjs, url);
            }
            
            url = CHEMDRAWFOLDER + "ChemOfficeEnterprise.js";
            if(!Page.ClientScript.IsClientScriptIncludeRegistered(_chemofficeenterprisejs)) {
                Page.ClientScript.RegisterClientScriptInclude(_chemofficeenterprisejs, url);
            }

            if(!Page.ClientScript.IsOnSubmitStatementRegistered(_changeinlinejs)) {
                Page.ClientScript.RegisterOnSubmitStatement(typeof(string), _changeinlinejs,
                    @"
                    if (typeof(cd_objectArray)!='undefined' && typeof(cd_objectArray)!='unknown' && cd_objectArray) {
                        for(i = 0; i < cd_objectArray.length; i++) {
                            if(document.getElementById(cd_objectArray[i] + '_Output') != null){
                                if(!cd_getSpecificObject(cd_objectArray[i]).viewOnly)
                                    document.getElementById(cd_objectArray[i] + '_Output').value = cd_getData(cd_objectArray[i], 'chemical/x-cdx');
                            }
                        }
                    }
                    ");
            }

            System.Web.HttpBrowserCapabilities caps = Page.Request.Browser;

            // CURRENT SETTING:
            //    ActiveX Control (1) - IE 5.5 or higher versions
            //    old Plugin      (2) - IE 5.0 or lower versions, Netscape 4.x or lower versions
            //    new Plugin      (3) - Netscape 6.0 or higher versions
            string strAgent = Page.Request.UserAgent;
        //  Page.Response.Write("strAgent = " + strAgent + "<br>");
            

            if (caps.Browser.ToLower().IndexOf("ie") >= 0)
            {
                if (caps.MajorVersion == 5 && caps.MinorVersion >= 5 || caps.MajorVersion > 5)
                {
                    currentUsing = 1;
                }
                else
                {
                    currentUsing = 2;
                }
            }
            else if ((caps.Browser.ToLower().IndexOf("ie") == -1) && ((caps.Browser.ToLower().IndexOf("firefox") < 0 && caps.Browser.ToLower().IndexOf("netscape") < 0 && caps.Browser.ToLower().IndexOf("chrome") < 0 && caps.Browser.ToLower().IndexOf("applemac-safari")< 0 )))
            {
                if (strAgent.Contains("like Gecko") && strAgent.Contains("Trident") && caps.MajorVersion == 5 && caps.MinorVersion >= 5 || caps.MajorVersion > 5)
                
                {
                    currentUsing = 1;
                }
                else
                {
                    currentUsing = 2;
                }
            }
            else if (caps.Browser.ToLower().IndexOf("firefox") >= 0 || caps.Browser.ToLower().IndexOf("netscape") >= 0 )
            {
                if(caps.MajorVersion <= 4)
                {
                    currentUsing = 2;
                }
                else if(caps.MajorVersion >= 6)
                {
                    currentUsing = 3;
                }
            }
            else if (caps.Browser.ToLower().IndexOf("applemac-safari") >= 0)
            {
                
               if(caps.MajorVersion >= 5)
                {
                    currentUsing = 3;
                }
            }
            else if (caps.Browser.ToLower().IndexOf("mozilla") >= 0)
            {

                if (caps.MajorVersion >= 5)
                {
                    currentUsing = 3;
                }
            }

            if(currentUsing == 1)
            {
                url = CHEMDRAWFOLDER + "cdlib_ie.js";
                if(!Page.ClientScript.IsClientScriptIncludeRegistered(_cdlibjs))
                {
                    Page.ClientScript.RegisterClientScriptInclude(_cdlibjs, url);
                }
            }
            else if(currentUsing == 2 || currentUsing == 3)
            {
                url = CHEMDRAWFOLDER + "cdlib_ns.js";
                if(!Page.ClientScript.IsClientScriptIncludeRegistered(_cdlibjs))
                {
                    Page.ClientScript.RegisterClientScriptInclude(_cdlibjs, url);
                }
            }
            else
            {
                _errorMessage = "Your Browser is not supported";
            }
           

            base.OnPreRender(e);
        }
        
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if(this.Name == string.Empty)
                this.Name = ClientID;
            if(CssClass != string.Empty)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            if(!BorderColor.IsEmpty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, BorderColor.ToKnownColor().ToString());
            if(BorderStyle != BorderStyle.NotSet)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, BorderStyle.ToString());
            if(!BorderWidth.IsEmpty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, BorderWidth.ToString());

            writer.RenderBeginTag(this.TagKey);
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if(_errorMessage == string.Empty)
            {
                string dataUrl = string.Empty;

                if(DataURL != string.Empty)
                {
                    dataUrl = "'" + DataURL + "'";
                    InlineData = string.Empty;
                } else {
                    dataUrl = "''";
                }

                if (OutputData != _nullOutput/* && OutputData != string.Empty*/)
                {
                    InlineData = OutputData;
                    this.MimeType = MimeTypes.cdx;
                }

                if(InlineData != string.Empty && MimeType != MimeTypes.xml)
                {
                    dataUrl = "data:" + GetString(MimeType);
                    if(MimeType == MimeTypes.xcdx || MimeType == MimeTypes.cdx)
                        dataUrl += ";base64";

                    dataUrl += "," + InlineData.Trim();
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "Inline");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, Name + "Inline");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, dataUrl);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                    dataUrl = "document.getElementById('" + ClientID + "Inline').value";
                }
                
                
                if(MimeType == MimeTypes.xml)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute("language", "javascript");
                    writer.RenderBeginTag(HtmlTextWriterTag.Script);
                    writer.Write("cd_insertObject('" + _mimeType + "','" + Width + 
                    "','" + Height + "','" + Name + "','" + Src + "'," + ViewOnly.ToString().ToLower() + "," + ShrinkToFit.ToString().ToLower() + 
                    "," + dataUrl + "," + DontCache.ToString().ToLower() + ",'" + DockingReferenceID + "'," + EditOutOfPlace.ToString().ToLower() + ")");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.AddAttribute("language", "javascript");
                    writer.RenderBeginTag(HtmlTextWriterTag.Script);
                    writer.Write("var xml" + Name + " = \"" + NormalizeXML(this.InlineData) + "\"; cd_putData('" + Name + "', '" + GetString(MimeType) + "', xml" + Name + ");");
                    writer.RenderEndTag();
                }
                else
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute("language", "javascript");
                    writer.RenderBeginTag(HtmlTextWriterTag.Script);
                    writer.Write("cd_insertObject('" + _mimeType + "','" + Width + 
                    "','" + Height + "','" + Name + "','" + Src + "'," + ViewOnly.ToString().ToLower() + "," + ShrinkToFit.ToString().ToLower() + 
                    "," + dataUrl + "," + DontCache.ToString().ToLower() + ",'" + DockingReferenceID + "'," + EditOutOfPlace.ToString().ToLower() + ")");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }
                
                RenderChildren(writer);
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "solid");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "red");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "2px");
                writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(_errorMessage);
                writer.RenderEndTag();
            }
        }
        #endregion

        #region Private Methods
        private string NormalizeXML(string xml)
        {
            string result = xml.Trim();
            result = result.Replace("\"", "'");
            result = result.Replace("\r", "");
            result = result.Replace("\n", "");
            result = result.Replace("  ", " ");
            if(result.IndexOf("  ") >= 0)
                result = NormalizeXML(result);
            return result;
        }

        private string GetString(MimeTypes mimeType)
        {
            switch(mimeType)
            {
                case MimeTypes.cdx:
                case MimeTypes.xcdx:
                    return "chemical/x-cdx";
                case MimeTypes.xml:
                    return "text/xml";
                case MimeTypes.xchemdraw:
                    return "chemical/x-chemdraw";
                case MimeTypes.mdlmolfile:
                case MimeTypes.xmdlmolfile:
                    return "chemical/x-mdl-molfile";
                case MimeTypes.mdltgf:
                case MimeTypes.xmdltgf:
                    return "chemical/x-mdl-tgf";
                case MimeTypes.mdlrxn:
                case MimeTypes.xmdlrxn:
                    return "chemical/x-mdl-rxn";
                case MimeTypes.daylightsmiles:
                case MimeTypes.xdaylightsmiles:
                case MimeTypes.smiles:
                case MimeTypes.xsmiles:
                    return "chemical/x-daylight-smiles";
                case MimeTypes.mdlisis:
                case MimeTypes.xmdlisis:
                    return "chemical/x-mdl-isis";
                case MimeTypes.xquestelf1:
                    return "chemical/x-questel-f1";
                case MimeTypes.xquestelf1query:
                    return "chemical/x-questel-f1-query";
                case MimeTypes.msimolfile:
                case MimeTypes.xmsimolfile:
                    return "chemical/x-msi-molfile";
                case MimeTypes.smd:
                case MimeTypes.xsmd:
                    return "chemical/x-smd";
                case MimeTypes.ct:
                case MimeTypes.xct:
                    return "chemical/x-ct";
                case MimeTypes.cml:
                case MimeTypes.xcml:
                    return "chemical/x-cml";
                case MimeTypes.xname:
                    return "chemical/x-name";
                default:
                    return "";

            }
        }
        #endregion
    }

    #region MimeTypes enumeration
    /// <summary>
    /// Supported mime types by chem draw plugin.
    /// </summary>
    public enum MimeTypes {
        /// <summary>
        /// chemical/x-cdx
        /// </summary>
        xcdx,
        /// <summary>
        /// chemical/cdx
        /// </summary>
        cdx,
        /// <summary>
        /// text/xml
        /// </summary>
        xml,
        /// <summary>
        /// chemical/x-chemdraw
        /// </summary>
        xchemdraw,
        /// <summary>
        /// chemical/x-mdl-molfile
        /// </summary>
        xmdlmolfile,
        /// <summary>
        /// chemical/mdl-molfile
        /// </summary>
        mdlmolfile,
        /// <summary>
        /// chemical/x-mdl-tgf
        /// </summary>
        xmdltgf,
        /// <summary>
        /// chemical/mdl-tgf
        /// </summary>
        mdltgf,
        /// <summary>
        /// chemical/x-mdl-rxn
        /// </summary>
        xmdlrxn,
        /// <summary>
        /// chemical/mdl-rxn
        /// </summary>
        mdlrxn,
        /// <summary>
        /// chemical/x-daylight-smiles
        /// </summary>
        xdaylightsmiles,
        /// <summary>
        /// chemical/daylight-smiles
        /// </summary>
        daylightsmiles,
        /// <summary>
        /// chemical/x-smiles
        /// </summary>
        xsmiles,
        /// <summary>
        /// chemical/smiles
        /// </summary>
        smiles,
        /// <summary>
        /// chemical/x-mdl-isis
        /// </summary>
        xmdlisis,
        /// <summary>
        /// chemical/mdl-isis
        /// </summary>
        mdlisis,
        /// <summary>
        /// chemical/x-questel-f1
        /// </summary>
        xquestelf1,
        /// <summary>
        /// chemical/x-questel-f1-query
        /// </summary>
        xquestelf1query,
        /// <summary>
        /// chemical/x-msi-molfile
        /// </summary>
        xmsimolfile,
        /// <summary>
        /// chemical/msi-molfile
        /// </summary>
        msimolfile,
        /// <summary>
        /// chemical/x-smd
        /// </summary>
        xsmd,
        /// <summary>
        /// chemical/smd
        /// </summary>
        smd,
        /// <summary>
        /// chemical/x-ct
        /// </summary>
        xct,
        /// <summary>
        /// chemical/ct
        /// </summary>
        ct,
        /// <summary>
        /// chemical/x-cml
        /// </summary>
        xcml,
        /// <summary>
        /// chemical/cml
        /// </summary>
        cml,
        /// <summary>
        /// chemical/x-name
        /// </summary>
        xname
    }
    #endregion
}

