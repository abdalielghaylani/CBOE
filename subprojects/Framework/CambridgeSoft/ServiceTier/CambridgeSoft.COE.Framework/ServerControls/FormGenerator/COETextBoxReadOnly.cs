using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections;
using System.Drawing;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This is the read only version of <see cref="COETextEdit"/>.
    /// </para>
    /// <para>
    /// The COETextBoxReadOnly class accepts every Infragistics' WebTextEdit property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default value of the text box?</item>
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
    ///       &lt;CSSClass&gt;COETextBoxView&lt;/CSSClass&gt;
    ///       &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
    ///       &lt;ID&gt;OpticalRotationTextBox&lt;/ID&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;top&gt;220px&lt;/top&gt;
    ///       &lt;left&gt;15px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COETextBoxReadOnly&lt;/type&gt;
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
    [ToolboxData("<{0}:COETextBoxReadOnly runat=server></{0}:COETextBoxReadOnly>")]
    public class COETextBoxReadOnly : Infragistics.WebUI.WebDataInput.WebTextEdit, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEDesignable, ICOEHelpContainer
    {//Contains ReadOnly property. So, can be used as a Display Control.
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        #endregion

        #region Variables
        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private string _mask;
        private System.Web.UI.WebControls.Image _helpImage = null;
        #endregion

        #region Constructor
        public COETextBoxReadOnly() {
            this.ReadOnly = true;
            this.EnableViewState = false;
        }

        #endregion

        #region COEGenerableControl Members
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

        public string Mask
        {
            get { return _mask; }
            set {_mask = value; }
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
            if (data.ToString().Length > 0 && this.Mask != null)
            {               
                try
                {
                    if (Mask.Contains("#"))
                    {
                        double doubleData = double.Parse(data.ToString());
                        this.Value = string.Format("{0:" + this.Mask + "}", doubleData);
                    }
                    else 
                        this.Value = string.Format("{0:" + this.Mask + "}", data);                    
                }
                catch (FormatException)
                {
                    this.Value = "Invalid mask format";
                }
            }else
                this.Value = data.ToString();
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COETextBoxView&lt;/CSSClass&gt;
        ///   &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
        ///   &lt;ID&gt;OpticalRotationTextBox&lt;/ID&gt;
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

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
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
            {
                this.MaxLength = int.Parse(maxLength.InnerText);
            }

            XmlNode mask = xmlData.SelectSingleNode("//COE:Mask", manager);
            if (mask != null && mask.InnerText.Length > 0)
            {
                this.Mask = mask.InnerText;
            }

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if (width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode password = xmlData.SelectSingleNode("//COE:PassWord", manager);
            if (password != null && password.InnerText.Length > 0)
            {
                this.PasswordMode = (((password.InnerText).Trim()).ToLower()).Equals("true") ? true : false;
            }
            XmlNode read = xmlData.SelectSingleNode("//COE:ReadOnly", manager);
            if (read != null && read.InnerText.Length > 0)
            {
                this.ReadOnly = (((read.InnerText).Trim()).ToLower()).Equals("true") ? true : false;
            }
            XmlNode toolTip = xmlData.SelectSingleNode("//COE:ToolTip", manager);
            if (toolTip != null && toolTip.InnerText.Length > 0)
            {
                this.ToolTip = toolTip.InnerText;
            }
            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if(labelStyle != null && labelStyle.InnerText.Length > 0) {
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++) {
                    if(styles[i].Length > 0) {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }
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

        #region ICOERequireable Members
        /// <summary>
        /// Gets or sets if the control is required to have a value.
        /// </summary>
        public bool Required
        {
            get
            {
                if (ViewState["Required"] != null)
                    return (bool)ViewState["Required"];
                else
                    return false;
            }
            set
            {
                ViewState["Required"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable css style
        /// </summary>
        public string RequiredStyle
        {
            get
            {
                if (ViewState[Constants.RequiredStyle_VS] != null)
                    return (string)ViewState[Constants.RequiredStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[Constants.RequiredStyle_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable label's css style
        /// </summary>
        public string RequiredLabelStyle
        {
            get
            {
                if (ViewState[Constants.RequiredLabelStyle_VS] != null)
                    return (string)ViewState[Constants.RequiredLabelStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[Constants.RequiredLabelStyle_VS] = value;
            }
        }

        #endregion


        #region Life Cycle Events

        protected override void OnDataBinding(EventArgs e)
        {            
            base.OnDataBinding(e);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.Text = this.Label;
                _lit.RenderControl(writer);
            }
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            base.Render(writer);
            if(this.ShowHelp)
            {
                _helpImage = new System.Web.UI.WebControls.Image();
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
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if(this.ShowHelp)
                this.AddYUIScripts();
        }
        #endregion

        #region ICOEDesignable Members

        public XmlNode GetConfigInfo() {
            /*
             * <fieldConfig>
             *  <Style>border-color:MaxLength;</Style>
             *  <MaxLength></MaxLength>
             *  <Height></Height>
             *  <Width></Width>
             *  <TextMode></TextMode>
             *  <ID>32</ID>
             * </fieldConfig>
             */
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";

            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            if(this.Style.Count > 0) {
                XmlNode style = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Style", xmlns);
                style.InnerText = this.Style.Value;
                xmlData.FirstChild.AppendChild(style);
            }
            if(this.MaxLength > 0) {
                XmlNode maxLength = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "MaxLength", xmlns);
                maxLength.InnerText = this.MaxLength.ToString();
                xmlData.FirstChild.AppendChild(maxLength);
            }

            if (this.Mask != null && this.Mask.Length >0)
            {
                XmlNode mask = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Mask", xmlns);
                mask.InnerText = this.Mask;
                xmlData.FirstChild.AppendChild(mask);
            }

            if(this.Width != Unit.Empty) {
                XmlNode width = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Width", xmlns);
                width.InnerText = this.Width.ToString();
                xmlData.FirstChild.AppendChild(width);
            }

            if(this.Height != Unit.Empty) {
                XmlNode height = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Height", xmlns);
                height.InnerText = this.Height.ToString();
                xmlData.FirstChild.AppendChild(height);
            }

            XmlNode password = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "PassWord", xmlns);
            password.InnerText = this.PasswordMode.ToString().ToLower();
            xmlData.FirstChild.AppendChild(password);


            XmlNode read = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ReadOnly", xmlns);
            read.InnerText = this.ReadOnly.ToString().ToLower();
            xmlData.FirstChild.AppendChild(read);

            if(!string.IsNullOrEmpty(this.ToolTip)) {
                XmlNode toolTip = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "ToolTip", xmlns);
                toolTip.InnerText = this.ToolTip;
                xmlData.FirstChild.AppendChild(toolTip);
            }

            if(string.IsNullOrEmpty(this.CssClass)) {
                XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
                cssClass.InnerText = this.CssClass;
                xmlData.FirstChild.AppendChild(cssClass);
            }

            if(!string.IsNullOrEmpty(_lit.CssClass)) {
                XmlNode cssLabelClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns);
                cssLabelClass.InnerText = _lit.CssClass;
                xmlData.FirstChild.AppendChild(cssLabelClass);
            }

            if(!string.IsNullOrEmpty(_lit.Style.Value)) {
                XmlNode labelStyle = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "LabelStyle", xmlns);
                labelStyle.InnerText = _lit.Style.Value;
                xmlData.FirstChild.AppendChild(labelStyle);
            }

            return xmlData.FirstChild;
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
    }
}
