using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls;
using CambridgeSoft.COE.Framework.Controls.ChemDraw;
using System.Xml;
using System.ComponentModel;
using System.Web;
using CambridgeSoft.COE.Framework.COEChemDrawConverterService;
using System.Web.UI.HtmlControls;
using System.Collections;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a <see cref="COEChemDrawEmbed"/> control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEChemDraw class accepts every COEChemDraw property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default base64?</item>
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
    /// &lt;formElement&gt;
    ///     &lt;defaultValue/&gt;
    ///     &lt;bindingExpression&gt;Compound.BaseFragment.Structure.Value&lt;/bindingExpression&gt;
    ///     &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COEChemDraw&lt;/CSSClass&gt;
    ///         &lt;ID&gt;BaseFragmentStructure&lt;/ID&gt;
    ///         &lt;Height&gt;300px&lt;/Height&gt;
    ///         &lt;Width&gt;300px&lt;/Width&gt;
    ///         &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///         &lt;height&gt;300px&lt;/height&gt;
    ///         &lt;width&gt;300px&lt;/width&gt;
    ///         &lt;top&gt;12px&lt;/top&gt;
    ///         &lt;left&gt;15px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDraw&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    /// &lt;/formElement>
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to its base64 representation.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEChemDraw runat=server></{0}:COEChemDraw>")]
    [ValidationPropertyAttribute("Value")]
    public class COEChemDraw : CompositeControl, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEReadOnly
    {
        #region Variables & Constants
        private string _defaultValue = string.Empty;
        private COEChemDrawEmbed _cdEmbed = null;
        private HtmlImage _image = null;
        private Label _lit = new Label();
        private string _structure = string.Empty;
        private string _configString = "<Empty />";
        private const string DEFAULT_LABEL_CSSCLASS = "FEStructure";
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region Constructor
        public COEChemDraw() {
            _cdEmbed = new COEChemDrawEmbed();
            _image = new HtmlImage();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets if the control is going to be rendered as image.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Browsable(true)]
        [Description("Gets or sets if the control will be render as an image.")]
        public bool DisplayAsImage {
            get {
                bool b = (bool) (ViewState["DisplayAsImage"] == null ? false : ViewState["DisplayAsImage"]);
                return b;
            }

            set {
                ViewState["DisplayAsImage"] = value;
            }
        }

        protected Control control {
            get {
                if(DisplayAsImage)
                    return _image;
                else
                    return _cdEmbed;
            }
        }

        protected CssStyleCollection ControlStyle {
            get {
                CssStyleCollection style = null;
                if(!DisplayAsImage)
                    style = _cdEmbed.Style;
                else
                    style = _image.Style;
                return style;
            }
        }
        public override Unit Height {
            get {
                Unit height = new Unit();
                if(!DisplayAsImage)
                    height = _cdEmbed.Height;
                else
                    height = new Unit(_image.Height);
                return height;
            }
            set {
                if(!DisplayAsImage)
                    _cdEmbed.Height = value;
                else
                    _image.Height = (int) value.Value;
            }
        }

        public override Unit Width
        {
            get {
                Unit width = new Unit();
                if(!DisplayAsImage)
                    width = _cdEmbed.Width;
                else
                    width = new Unit(_image.Width);
                return width;
            }
            set {
                if(!DisplayAsImage)
                    _cdEmbed.Width = value;
                else
                    _image.Width = (int) value.Value;
            }
        }

        protected string ControlCssClass {
            set {
                if(!DisplayAsImage)
                    _cdEmbed.CssClass = value;
                else
                    _image.Attributes.Add("class", value);
            }
        }

        public override string ClientID {
            get {
                return control.ClientID;
            }
        }

        public override string ID {
            get {
                return control.ID;
            }
            set {
                base.ID = value + "Container";
                control.ID = value;
            }
        }
        #endregion

        #region COEGenerableControl Members
        /// <summary>
        /// Allows to set which is the default base64.
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

        public object Value
        {
            get {
                return GetData();
            }
        }

        /// <summary>
        /// Gets the base64 representation of the control.
        /// </summary>
        /// <returns>A string with its base64 representation.</returns>
        public object GetData()
        {
            if(control is COEChemDrawEmbed)
                _structure = ((COEChemDrawEmbed) control).OutputData;
            return _structure;
        }

        /// <summary>
        /// Sets the base64 representation of the control.
        /// </summary>
        /// <param name="data">A string with the desired base64 representation.</param>
        public void PutData(object data)
        {
            if(data is DBNull)
                _structure = string.Empty;
            else if(data.GetType() == typeof(string)) {
                _structure = (string) data;
            } else
                throw new Exception("Incompatible types.");

            if (DisplayAsImage)
            {
                _image.Src = COEChemDrawConverter.GetStructureResource(_structure, "image/gif", Height, Width);
            }
            else
            {
                if (_structure.StartsWith("<?xml"))
                {
                    _cdEmbed.MimeType = MimeTypes.xml;
                }
                _cdEmbed.InlineData = _structure;
            }
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///     &lt;configInfo&gt;
        ///         &lt;fieldConfig&gt;
        ///         &lt;CSSClass&gt;COEChemDraw&lt;/CSSClass&gt;
        ///         &lt;ID&gt;BaseFragmentStructure&lt;/ID&gt;
        ///         &lt;Height&gt;300px&lt;/Height&gt;
        ///         &lt;Width&gt;300px&lt;/Width&gt;
        ///         &lt;/fieldConfig&gt;
        ///     &lt;/configInfo&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public virtual void LoadFromXml(string xmlDataAsString)
        {
            /*
             * <fieldConfig>
             *  <Style>border-color:blue;</Style>
             *  <Height></Height>
             *  <Width></Width>
             *  <ID>32</ID>
             * </fieldConfig>
             */
            _configString = xmlDataAsString;
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

        #region Life Cycle Events
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            try {
                if(Page.Session["isCDP"] != null)
                    DisplayAsImage = !(bool) Page.Session["isCDP"];
            } catch(Exception) {
                DisplayAsImage = false;
            }
            LoadFromXml();

            this.Controls.Add(control);
        }
        protected override void Render(HtmlTextWriter writer) {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this._cdEmbed.ViewOnly = true;
            }
            if(!string.IsNullOrEmpty(this.Label)) {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Text = this.Label + "<br>";
                _lit.RenderControl(writer);
            }
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            RenderContents(writer);
        }
        #endregion

        #region Private Methods
        private void LoadFromXml() {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(_configString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode diplayAsImage = xmlData.SelectSingleNode("//COE:DisplayAsImage", manager);
            if(diplayAsImage != null && diplayAsImage.InnerText.Length > 0) {
                DisplayAsImage = bool.Parse(diplayAsImage.InnerText);
            
            }
            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", manager);
            if(style != null && style.InnerText.Length > 0) {
                string[] styles = style.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++) {
                    if(styles[i].Length > 0) {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        
                        ControlStyle.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);

            //Ensure there is a default CSSClass value for this form-field
            if (cssClass != null && string.IsNullOrEmpty(cssClass.InnerText))
                ControlCssClass = DEFAULT_LABEL_CSSCLASS;

            if(cssClass != null && cssClass.InnerText.Length > 0)
                ControlCssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if(cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
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
            
            XmlNode width = xmlData.SelectSingleNode("//COE:Width", manager);
            if(width != null && width.InnerText.Length > 0) {
                Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", manager);
            if(height != null && height.InnerText.Length > 0) {
                Height = new Unit(height.InnerText);
            }

            XmlNode inlineData = xmlData.SelectSingleNode("//COE:InlineData", manager);
            if(inlineData != null && inlineData.InnerText.Length > 0) {
                string str = inlineData.InnerText;
                this.PutData(str);
            }

            if(!DisplayAsImage) {
                

                XmlNode mimeType = xmlData.SelectSingleNode("//COE:MimeType", manager);
                if(mimeType != null && mimeType.InnerText.Length > 0) {
                    ((COEChemDrawEmbed) control).MimeType = (MimeTypes) Enum.Parse(typeof(MimeTypes), mimeType.InnerText);
                }

                XmlNode viewOnly = xmlData.SelectSingleNode("//COE:ViewOnly", manager);
                if(viewOnly != null && viewOnly.InnerText.Length > 0) {
                    ((COEChemDrawEmbed) control).ViewOnly = bool.Parse(viewOnly.InnerText);
                }

                XmlNode name = xmlData.SelectSingleNode("//COE:Name", manager);
                if(name != null && name.InnerText.Length > 0) {
                    ((COEChemDrawEmbed) control).Name = name.InnerText;
                }
            }
        }
        #endregion
    }
}
