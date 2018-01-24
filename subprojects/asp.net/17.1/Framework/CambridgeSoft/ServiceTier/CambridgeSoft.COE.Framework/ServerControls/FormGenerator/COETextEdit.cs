using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using System.Collections;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements an Infragistics' WebTextEdit control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COETextEdit class accepts every Infragistics' WebTextEdit property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default value of the text edit?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Width, Height, ID...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;label&gt;Optical Rotation&lt;/label&gt;
    ///     &lt;bindingExpression&gt;PropertyList[@Name='OPTICALROTATION' | Value]&lt;/bindingExpression&gt;
    ///     &lt;validationRuleList&gt;
    ///       &lt;validationRule validationRuleName="textLength" errorMessage="The length must be between 0 and 200"&gt;
    ///         &lt;params&gt;
    ///           &lt;param name="min" value="0"/&gt;
    ///           &lt;param name="max" value="200"/&gt;
    ///         &lt;/params&gt;
    ///       &lt;/validationRule&gt;
    ///     &lt;/validationRuleList&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;CSSClass&gt;COETextEdit&lt;/CSSClass&gt;
    ///       &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
    ///       &lt;ID&gt;OpticalRotationTextEdit&lt;/ID&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;top&gt;220px&lt;/top&gt;
    ///       &lt;left&gt;15px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextEdit&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Value property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COETextEdit runat=server></{0}:COETextEdit>")]
    public class COETextEdit : Infragistics.WebUI.WebDataInput.WebTextEdit, ICOEGenerableControl, ICOEHelpContainer, ICOELabelable, ICOEFileUpload
    {//Contains ReadOnly property. So, can be used as a Display Control.
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        #endregion

        #region Variables
        private string _defaultValue = string.Empty;
        private Image _helpImage = null;
        private Label _lit = new Label();
        private string _mask = string.Empty;
        private string _uploadCtrolSet = string.Empty;
        #endregion

        #region ICOEGenerableControl Members
        /// <summary>
        /// Allows to set the default value.
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
            }
        }

        /// <summary>
        /// <para>Gets the value of the control.</para>
        /// </summary>
        /// <returns>A string with the control's value.</returns>
        public object GetData()
        {
            return this.Value;
        }

        /// <summary>
        /// Sets the control's value.
        /// </summary>
        /// <param name="data">A string with the desired value of the control.</param>
        public void PutData(object data)
        {
            // Avoid parsing data strings of zero length
            if (data.ToString().Length > 0 && !string.IsNullOrEmpty(_mask))
            {
                try
                {
                    Double toMask;
                    Double.TryParse(data.ToString(), out toMask);
                    string formatString = String.Format(_mask, toMask);
                    this.Text = formatString;
                }
                catch
                {
                    this.Value = "Invalid mask format";
                }
            }
            else
                this.Value = data.ToString();
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COETextEdit&lt;/CSSClass&gt;
        ///   &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
        ///   &lt;ID&gt;OpticalRotationTextEdit&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            /*
             * <fieldConfig>
             *  <Style>border-color:blue;</Style>
             *  <MaxLength></MaxLength>
             *  <Height></Height>
             *  <Width></Width>
             *  <TextMode></TextMode>
             *  <ID>32</ID>
             * </fieldConfig>
             */
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if (style != null && style.InnerText.Length > 0)
            {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode maxLength = xmlData.SelectSingleNode("//COE:MaxLength", manager);
            if (maxLength != null && maxLength.InnerText.Length > 0)
                this.MaxLength = int.Parse(maxLength.InnerText);
           
            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
                this.Width = new Unit(width.InnerText);
           
            XmlNode mask = xmlData.SelectSingleNode("//COE:Mask", manager);
            if (mask != null && mask.InnerText.Length > 0)
                _mask = mask.InnerText;
           
            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
                this.Height = new Unit(height.InnerText);
           
            XmlNode password = xmlData.SelectSingleNode("//COE:PassWord", manager);
            if (password != null && password.InnerText.Length > 0)
                this.PasswordMode = ((((string)password.InnerText).Trim()).ToLower()).Equals("true") ? true : false;
           
            XmlNode display = xmlData.SelectSingleNode("//COE:Display", manager);
            if (display != null && display.InnerText.Length > 0)
                this.ReadOnly = ((((string)display.InnerText).Trim()).ToLower()).Equals("true") ? true : false;
            
            XmlNode toolTip = xmlData.SelectSingleNode("//COE:ToolTip", manager);
            if (toolTip != null && toolTip.InnerText.Length > 0)
                this.ToolTip = (string)toolTip.InnerText;

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;
           
            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            foreach (XmlNode configSetting in xmlData.SelectNodes("//COE:fieldConfig/COE:configSetting", manager))
            {
                if (configSetting != null && configSetting.InnerText.Length > 0)
                {
                    string bindingExpression = configSetting.Attributes["bindingExpression"].Value;
                    if (!string.IsNullOrEmpty(bindingExpression))
                    {
                        try
                        {   //Using COEDatabinder to find the object property given a bindingExpression in the current object
                            COEDataBinder dataBinder = new COEDataBinder(this);
                            dataBinder.SetProperty(bindingExpression, configSetting.InnerText);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            XmlNode uploadCtrolSet = xmlData.SelectSingleNode("//COE:UploadControlSettings", manager);
            if (uploadCtrolSet != null && uploadCtrolSet.InnerXml.Length > 0)
                _uploadCtrolSet = uploadCtrolSet.OuterXml;
            
        }
        #endregion

        #region Life Cycle Events
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                _lit.Text = this.Label;
                if (this.LabelStyles != null)
                {
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        _lit.Style.Add(key, (string)this.LabelStyles[key]);
                    }
                }
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.RenderControl(writer);
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            base.Render(writer);
            if(this.ShowHelp)
            {
                _helpImage = new Image();
                _helpImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ICOEGenerableControl), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.help.png");
                _helpImage.ID = this.ClientID + "HelpImage";
                _helpImage.Style.Add(HtmlTextWriterStyle.MarginLeft, "2px");
                _helpImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                _helpImage.Attributes.Add("onclick", "showHide(YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip, " + _helpImage.ClientID + ");");
                _helpImage.RenderControl(writer);
                string yahooScript = "<script language='javascript' type='text/javascript'>YAHOO.namespace('COEFormGenerator');" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip = new YAHOO.widget.Tooltip(\"" + _helpImage.ClientID + "Tooltip\", " +
                                     "{ context:\"" + _helpImage.ClientID + "\", text:\"" + FormatText(this.HelpText) + "\"," +
                                     "showdelay: 20000," +
                                     "hidedelay: 20000," +
                                     "visible:false});" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip.isToBeShown = true;" +
                                     "</script>";
                writer.Write(yahooScript);
            }
            if (this.IsFileUpload && !string.IsNullOrEmpty(this.PageComunicationProvider)) 
            {
                if (this.PageComunicationProvider.ToUpper() == CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.GUIShellProvider && 
                    !string.IsNullOrEmpty(this.FileUploadBindingExpression))
                {
                    //if (this.Parent is COEWebGridUltra)
                    //    ((COEWebGridUltra)this.Parent).Grid.DisplayLayout.ClientSideEvents.CellClickHandler = "DisplayUploadControl";
                    
                    //Custom provider but could be added others.
                    using (UploadControl upload = new UploadControl(this.ID, this.ClientID, this.FileUploadBindingExpression, _uploadCtrolSet)) //Coverity Fix CID : 19053
                    {
                        upload.RenderControl(writer);
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if(this.ShowHelp)
                this.AddYUIScripts();
        }
        #endregion

        #region ICOEHelpContainer Members
        public bool ShowHelp
        {
            get
            {
                if(ViewState["ShowHelp"] != null)
                    return (bool) ViewState["ShowHelp"];
                else
                    return false;
            }
            set
            {
                ViewState["ShowHelp"] = value;
            }
        }

        public string HelpText
        {
            get
            {
                if(ViewState["HelpText"] != null)
                    return (string) ViewState["HelpText"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["HelpText"] = value;
            }
        }
        #endregion

        #region Private Methods
        private void AddYUIScripts()
        {
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);

            CambridgeSoft.COE.Framework.Common.FrameworkUtils.AddYUICSSReference(this.Page,
                CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_CSS.CONTAINER);

            string showHideJS = @"
                function showHide(tooltip, contextid)
                {
                    var originalshowdelay = tooltip.cfg.getProperty('showdelay');
                    if(tooltip.isToBeShown)
                    {
                        tooltip.isToBeShown = false;
                        tooltip.cfg.setProperty('showdelay', 0);
                        tooltip.doShow(null, document.getElementById(contextid));
                    }
                    else
                    {
                        tooltip.isToBeShown = true;
                        tooltip.hide();
                    }

                    tooltip.cfg.setProperty('showdelay', originalshowdelay);
                }";
            if(!Page.ClientScript.IsStartupScriptRegistered(typeof(ICOEGenerableControl), "showHideJS"))
            {
                Page.ClientScript.RegisterStartupScript(typeof(ICOEGenerableControl), "showHideJS", showHideJS, true);
            }
        }

        private string FormatText(string txt)
        {
            return "<span style='font-size:9px;'>" + Page.Server.HtmlEncode(txt) + "</span>";
        }
        #endregion

        #region ICOELabelable Members
        /// <summary>
        /// Gets or sets the Control's label.
        /// </summary>
        public string Label
        {
            get
            {
                if (ViewState[Constants.Label_VS] != null)
                    return (string)ViewState[Constants.Label_VS];
                else
                    return string.Empty;
            }
            set
            {
                ViewState[Constants.Label_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// </summary>
        public Dictionary<string, string> LabelStyles
        {
            get
            {
                if (ViewState[Constants.LabelStyles_VS] != null)
                    return (Dictionary<string, string>)ViewState[Constants.LabelStyles_VS];
                else
                    return null;
            }
            set
            {
                ViewState[Constants.LabelStyles_VS] = value;
            }
        }


        #endregion

        #region ICOEFileUpload Members

        /// <summary>
        /// Gets or sets a value indicating whether this instance is file upload.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is file upload; otherwise, <c>false</c>.
        /// </value>
        public bool IsFileUpload
        {
            get
            {
                return ViewState["IsFileUpload"] != null ? (bool)ViewState["IsFileUpload"] : false;
            }
            set
            {
                ViewState["IsFileUpload"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the page comunication provider.
        /// </summary>
        /// <value>The page comunication provider.</value>
        /// <example>GUIShell</example>
        public string PageComunicationProvider
        {
            get
            {
                return ViewState["PageComunicationProvider"] != null ? ViewState["PageComunicationProvider"].ToString() : string.Empty;
            }
            set
            {
                ViewState["PageComunicationProvider"] = value;
            }
        }

        public string FileUploadBindingExpression
        {
            get
            {
                return ViewState["FileUploadBindingExpression"] != null ? ViewState["FileUploadBindingExpression"].ToString() : string.Empty;
            }
            set
            {
                ViewState["FileUploadBindingExpression"] = value;
            }
        }

        #endregion
    }
}
