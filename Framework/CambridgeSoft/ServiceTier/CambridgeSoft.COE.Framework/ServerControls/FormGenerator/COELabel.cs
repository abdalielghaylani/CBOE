using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Collections;
using System.Web.UI;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>
    /// This class implements a Label control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COELabel class accepts every Label property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default text of the label?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Columns, column's datafield and headertext...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;defaultValue&gt;Compound Information&lt;/defaultValue&gt;
    ///     &lt;bindingExpression/&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;CSSClass&gt;COELabelTitle&lt;/CSSClass&gt;
    ///       &lt;ID&gt;CompoundTitleLabel&lt;/ID&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;top&gt;-20px&lt;/top&gt;
    ///       &lt;left&gt;0px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELabel&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the "Text" of the Label.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COELabel runat=server></{0}:COELabel>")]
    public class COELabel : Label, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEDesignable, ICOEFileUpload {

        #region Variables
        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private string _uploadCtrolSet = string.Empty;
        private string _mask;
        #endregion

        public string Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }


        #region COEGenerableControl Members
        /// <summary>
        /// <para>Allows to set the default Text for the control.</para>
        /// </summary>
        public string DefaultValue {
            get {
                return _defaultValue;
            }
            set {
                _defaultValue = value;
            }
        }

        /// <summary>
        /// <para>Gets the Text of the control.</para>
        /// </summary>
        /// <returns>A string with the control's text.</returns>
        public object GetData() {
            return this.Text;
        }

        /// <summary>
        /// Sets the control's text.
        /// </summary>
        /// <param name="data">A string with the desired text of the control.</param>
        public void PutData(object data) {
            // Avoid parsing data strings of zero length
            if (data.ToString().Length > 0 && this.Mask != null)
            {
                try
                {
                    if (Mask.Contains("#"))
                    {
                        double doubleData = double.Parse(data.ToString());
                        this.Text = string.Format("{0:" + this.Mask + "}", doubleData);
                    }
                    else 
                        this.Text = string.Format("{0:" + this.Mask + "}", data);
                }
                catch(FormatException)
                {
                    this.Text = "Invalid mask format";
                }
            }
            else
                this.Text = data.ToString();
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COELabelTitle&lt;/CSSClass&gt;
        ///   &lt;ID&gt;CompoundTitleLabel&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString) {
            /*
             * <fieldConfig>
             *  <Style>border-color:blue;</Style>
             *  <Height></Height>
             *  <Width></Width>
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

            XmlNode uploadCtrolSet = xmlData.SelectSingleNode("//COE:UploadControlSettings", manager);
            if (uploadCtrolSet != null && uploadCtrolSet.InnerXml.Length > 0)
                _uploadCtrolSet = uploadCtrolSet.OuterXml;

            XmlNode mask = xmlData.SelectSingleNode("//COE:Mask", manager);
            if (mask != null && mask.InnerText.Length > 0)
            {
                this.Mask = mask.InnerText;
            }
        }
        #endregion

        #region ICOEDesignable Members

        public XmlNode GetConfigInfo() {
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

            if (this.IsFileUpload && !string.IsNullOrEmpty(this.PageComunicationProvider)) 
            {
                if (this.PageComunicationProvider.ToUpper() == CambridgeSoft.COE.Framework.GUIShell.GUIShellTypes.GUIShellProvider && 
                    !string.IsNullOrEmpty(this.FileUploadBindingExpression))
                {
                    //Custom provider but could be added others.
                    using (UploadControl upload = new UploadControl(this.ID, this.ClientID, this.FileUploadBindingExpression, _uploadCtrolSet))     //Coverity Fix CID : 19052
                    {
                        upload.RenderControl(writer);
                    }
                }
            }
            base.Render(writer);
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
