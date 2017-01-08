using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI;
using System.Web;
using System.Collections;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.ServerControls.Utilities;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Reflection;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This control is used to display the legacy salt/batch suffix value for a Batch when the Regsequence less than some limit 
    /// This class implements a TextBox control in Singleline mode that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COETextBox class accepts every TextBox property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default text of the textbox?</item>
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
    ///     &lt;bindingExpression&gt;PropertyList[@Name='SALTANDBATCHSUFFIX' | Value]&lt;/bindingExpression&gt;
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
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.SaltAndBacthSuffixTextBox&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Text property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COETextBox runat=server></{0}:COETextBox>")]
    public class SaltAndBacthSuffixTextBox : TextBox, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEDesignable, ICOEHelpContainer, ICOEReadOnly
    {
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        private const string DEFAULT_CSSCLASS = "FETextBox";
        private const string DEFAULT_CSSCLASS_VIEWONLY = "FETextBoxViewMode";
        private const string DEFAULT_LABEL_CSSCLASS = "FELabel";
        private const string Label_VS = "Label";
        private const string LabelStyles_VS = "LabelStyles";
        private const string RequiredStyle_VS = "RequiredStyle";
        private const string RequiredLabelStyle_VS = "RequiredLabelStyle";

        #endregion

        #region Variables
        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private string _mask;
        private Image _helpImage = null;
        private string _controlId = string.Empty;
        int _iRegSequenceLimit = 0;
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region COEGenerableControl Members

        public string Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }

        public int RegSequenceLimit
        {
            get { return _iRegSequenceLimit; }
            set { _iRegSequenceLimit = value; }
        }

        public string ControlId
        {
            get { return _controlId; }
            set { _controlId = value; }
        }

        /// <summary>
        /// Allows to set the default text.
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
        /// <para>Gets the Text of the control.</para>
        /// </summary>
        /// <returns>A string with the control's text.</returns>
        public virtual object GetData()
        {
            return this.Text;
        }

        /// <summary>
        /// Sets the control's text.
        /// </summary>
        /// <param name="data">A string with the desired text of the control.</param>
        public virtual void PutData(object data)
        {
            // Avoid parsing data strings of zero length
            if (data.ToString().Length > 0 && this.Mask != null)
            {
                try
                {
                    Double toMask;
                    Double.TryParse(data.ToString(), out toMask);
                    string formatString = String.Format(this.Mask, toMask);
                    //this.Value = data.ToString();
                    this.Text = formatString;
                }
                catch
                {
                    this.Text = "Invalid mask format";
                }
            }
            else
            {
                this.Text = data.ToString();
            }
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

            XmlNode readOnly = xmlData.SelectSingleNode("//COE:ReadOnly", manager);
            if (readOnly != null && readOnly.InnerText.Length > 0)
            {
                this.ReadOnly = bool.Parse(readOnly.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode textMode = xmlData.SelectSingleNode("//COE:TextMode", manager);
            if (textMode != null && textMode.InnerText.Length > 0)
            {
                switch (textMode.InnerText.ToLower())
                {
                    case "multiline":
                        this.TextMode = TextBoxMode.MultiLine;
                        break;
                    case "password":
                        this.TextMode = TextBoxMode.Password;
                        break;
                    default:
                        this.TextMode = TextBoxMode.SingleLine;
                        break;
                }
                XmlNode controlidNode = xmlData.SelectSingleNode("//COE:ControlId", manager);
                if (controlidNode != null && controlidNode.InnerText.Length > 0)
                {
                    this.ControlId = controlidNode.InnerText;
                }

            }

            XmlNode cssRegSequence = xmlData.SelectSingleNode("//COE:LegacyFieldDisplayLimit", manager);
            if (cssRegSequence != null && cssRegSequence.InnerText.Length > 0)
            {
                this.RegSequenceLimit = int.Parse(cssRegSequence.InnerText);
                HttpContext.Current.Session.Add("LegacyFieldDisplayLimit", RegSequenceLimit);
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if (labelStyle != null && labelStyle.InnerText.Length > 0)
            {
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }
        }
        #endregion

        #region ICOEDesignable Members
        public XmlNode GetConfigInfo()
        {
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";

            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));


            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            if (this.Style.Count > 0)
            {
                XmlNode style = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Style", xmlns);
                style.InnerText = this.Style.Value;
                xmlData.FirstChild.AppendChild(style);
            }
            if (this.MaxLength > 0)
            {
                XmlNode maxLength = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "MaxLength", xmlns);
                maxLength.InnerText = this.MaxLength.ToString();
                xmlData.FirstChild.AppendChild(maxLength);
            }

            if (this.Mask != null && this.Mask.Length > 0)
            {
                XmlNode mask = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Mask", xmlns);
                mask.InnerText = this.Mask;
                xmlData.FirstChild.AppendChild(mask);
            }

            if (this.Width != Unit.Empty)
            {
                XmlNode width = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Width", xmlns);
                width.InnerText = this.Width.ToString();
                xmlData.FirstChild.AppendChild(width);
            }

            if (this.Height != Unit.Empty)
            {
                XmlNode height = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Height", xmlns);
                height.InnerText = this.Height.ToString();
                xmlData.FirstChild.AppendChild(height);
            }

            XmlNode textMode = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "TextMode", xmlns);
            textMode.InnerText = this.TextMode.ToString();
            xmlData.FirstChild.AppendChild(textMode);

            //Ensure there is a default CSSClass value for this form-field (be it read-only or editable) and its label
            if (string.IsNullOrEmpty(this.CssClass) && this.ReadOnly != true)
                this.CssClass = DEFAULT_CSSCLASS;
            else if (string.IsNullOrEmpty(this.CssClass) && this.ReadOnly == true)
                this.CssClass = DEFAULT_CSSCLASS_VIEWONLY;
            XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
            cssClass.InnerText = this.CssClass;
            xmlData.FirstChild.AppendChild(cssClass);

            if (string.IsNullOrEmpty(_lit.CssClass))
                _lit.CssClass = DEFAULT_LABEL_CSSCLASS;
            XmlNode cssLabelClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns);
            cssLabelClass.InnerText = _lit.CssClass;
            xmlData.FirstChild.AppendChild(cssLabelClass);

            if (!string.IsNullOrEmpty(_lit.Style.Value))
            {
                XmlNode labelStyle = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "LabelStyle", xmlns);
                labelStyle.InnerText = _lit.Style.Value;
                xmlData.FirstChild.AppendChild(labelStyle);
            }

            return xmlData.FirstChild;
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
                if (ViewState[Label_VS] != null)
                    return (string)ViewState[Label_VS];
                else
                    return string.Empty;
            }
            set
            {
                ViewState[Label_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Control's label CSS styles attributes.
        /// </summary>
        public Dictionary<string, string> LabelStyles
        {
            get
            {
                if (ViewState[LabelStyles_VS] != null)
                    return (Dictionary<string, string>)ViewState[LabelStyles_VS];
                else
                    return null;
            }
            set
            {
                ViewState[LabelStyles_VS] = value;
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
                if (ViewState[RequiredStyle_VS] != null)
                    return (string)ViewState[RequiredStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[RequiredStyle_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable label's css style
        /// </summary>
        public string RequiredLabelStyle
        {
            get
            {
                if (ViewState[RequiredLabelStyle_VS] != null)
                    return (string)ViewState[RequiredLabelStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[RequiredLabelStyle_VS] = value;
            }
        }

        #endregion

        #region Life Cycle Events
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.ReadOnly = true;
            }
            if (!string.IsNullOrEmpty(this.Label))
            {
                _lit.Text = this.Label;
                //CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.Style.Add("float", "left");  // CSBR-120344 fix, new line added
                _lit.RenderControl(writer);
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            //if (!string.IsNullOrEmpty(this.RequiredStyle))
            //{
            //    CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            //}

            // CSBR-120344 fix
            if (this.ShowHelp)
            {
                Label lblHelp = new Label();
                lblHelp.Style.Add("float", "left");  // CSBR-120344 fix, new line added           
                _helpImage = new Image();
                _helpImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ICOEGenerableControl), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.help.png");
                _helpImage.ID = this.ClientID + "HelpImage";
                _helpImage.Style.Add(HtmlTextWriterStyle.MarginLeft, "2px");
                _helpImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                _helpImage.Attributes.Add("onclick", "showHide(YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip, " + _helpImage.ClientID + ");");
                lblHelp.Controls.Add(_helpImage);
                //_helpImage.RenderControl(writer);
                lblHelp.RenderControl(writer);
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

            base.Render(writer);

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            //setting the visibility of the control based onthe Registration sequence and the Limit value.
            bool bControlVisibility = false;
            if (HttpContext.Current.Session[PerkinElmer.CBOE.Registration.Client.Constants.MultiCompoundObject_Session] != null)
            {
                try
                {
                    RegistryRecord registryRecord = (RegistryRecord)(HttpContext.Current.Session[PerkinElmer.CBOE.Registration.Client.Constants.MultiCompoundObject_Session]);
                    if (registryRecord.RegNum != "")
                    {
                        string sregnum = registryRecord.RegNum;
                        if (registryRecord.RegNumber.Sequence.Prefix != "")
                            sregnum = sregnum.Replace(registryRecord.RegNumber.Sequence.Prefix, "");
                        int iRegSequence = int.Parse(registryRecord.RegNum);
                        if (iRegSequence > this.RegSequenceLimit)
                            bControlVisibility = false;
                        else
                            bControlVisibility = true;
                    }
                }
                catch (Exception ex) { }
            }
            this.Visible = bControlVisibility;
            this.Parent.Visible = bControlVisibility;
            if (this.ShowHelp)
                this.AddYUIScripts();
        }
        #endregion

        #region ICOEHelpContainer Members
        public bool ShowHelp
        {
            get
            {
                if (ViewState["ShowHelp"] != null)
                    return (bool)ViewState["ShowHelp"];
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
                if (ViewState["HelpText"] != null)
                    return (string)ViewState["HelpText"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["HelpText"] = value;
            }
        }
        #endregion

        #region ICOEReadOnly Members
        /// <summary>
        /// EditControl Property implementation.
        /// </summary>
        public COEEditControl COEReadOnly
        {
            get
            {
                return _editControl;
            }
            set
            {
                _editControl = value;
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
            if (!Page.ClientScript.IsStartupScriptRegistered(typeof(ICOEGenerableControl), "showHideJS"))
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
