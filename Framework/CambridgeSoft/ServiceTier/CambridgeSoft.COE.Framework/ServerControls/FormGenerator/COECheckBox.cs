using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a CheckBox control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COECheckBox class accepts every CheckBox property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: Would be checked by default?</item>
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
    ///     &lt;formElement&gt;
    ///       &lt;defaultValue&gt;true&lt;/defaultValue&gt;
    ///       &lt;bindingExpression&gt;Compound.BaseFragment.Structure.UseNormalizedStructure&lt;/bindingExpression&gt;
    ///       &lt;label&gt;Use normalized structure&lt;/label&gt;
    ///       &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
    ///         &lt;ID&gt;UseNormalizedStructureCheck&lt;/ID&gt;
    ///         &lt;/fieldConfig&gt;
    ///       &lt;/configInfo&gt;
    ///       &lt;displayInfo&gt;
    ///         &lt;top&gt;310px&lt;/top&gt;
    ///         &lt;left&gt;100px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECheckBox&lt;/type&gt;
    ///       &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Checked property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COECheckBox runat=server></{0}:COECheckBox>")]
    public class COECheckBox : CheckBox, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEDesignable, ICOEReadOnly
    {
        #region Variables

        private string _defaultValue = string.Empty;
        private string _trueString = string.Empty;
        private string _falseString = string.Empty;
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region COEGenerableControl Members
        /// <summary>
        /// Allows to set if it is checked or not by default.
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
        /// Gets if this checkbox is currently checked.
        /// </summary>
        /// <returns>A bool value that shows if it is checked or not.</returns>
        public object GetData()
        {
            if (this.Checked)
                return _trueString;
            else
            {
                if (_falseString.ToLower() == "null")
                    return null;
                
                return _falseString;
            }
        }

        /// <summary>
        /// Sets if it should be checked.
        /// </summary>
        /// <param name="data">A bool or a parseable string that sets if this should be checked or not.</param>
        public void PutData(object data)
        {

            if (data.ToString() == _trueString)
                data = "True";
            if(data.ToString() == _falseString || data.ToString() == "")
                data = "False";
            if(data.GetType() == typeof(bool))
                this.Checked = (bool) data;
            else if(data.GetType() == typeof(string))
                this.Checked = bool.Parse(data as string);
            else if(data is System.DBNull)
                this.Checked = false;
            else
                throw new Exception("Incompatible types.");
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///     &lt;fieldConfig&gt;
        ///         &lt;Style&gt;border-color:blue;&lt;/Style&gt;
        ///         &lt;Height&gt;&lt;/Height&gt;
        ///         &lt;Width&gt;&lt;/Width&gt;
        ///         &lt;Text&gt;&lt;/Text&gt;
        ///         &lt;CssClass&gt;&lt;/CssClass&gt;
        ///         &lt;CSSLabelClass&gt;&lt;/CSSLabelClass&gt;
        ///         &lt;ID&gt;32&lt;/ID&gt;
        ///     &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
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

            XmlNode text = xmlData.SelectSingleNode("//COE:Text", manager);
            if (text != null && text.InnerText.Length > 0)
            {
                this.Text = text.InnerText;
            }

            XmlNode trueString = xmlData.SelectSingleNode("//COE:TrueString", manager);
            if (trueString != null && trueString.InnerText.Length > 0)
                _trueString = trueString.InnerText;
            else
                _trueString = "T";

            XmlNode falseString = xmlData.SelectSingleNode("//COE:FalseString", manager);
            if (falseString != null && falseString.InnerText.Length > 0)
                _falseString = falseString.InnerText;
            else
                _falseString = "F";

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                this.LabelAttributes["class"] = cssLabelClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if(labelStyle != null && labelStyle.InnerText.Length > 0) {
                this.LabelAttributes["style"] = labelStyle.InnerText;
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

            if(!string.IsNullOrEmpty(this.Text)) {
                XmlNode text = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "Text", xmlns);
                text.InnerText = this.Text;
                xmlData.FirstChild.AppendChild(text);
            }

            if(string.IsNullOrEmpty(this.CssClass)) {
                XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
                cssClass.InnerText = this.CssClass;
                xmlData.FirstChild.AppendChild(cssClass);
            }

            if(!string.IsNullOrEmpty(this.LabelAttributes["class"])) {
                XmlNode cssLabelClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSLabelClass", xmlns);
                cssLabelClass.InnerText = this.LabelAttributes["class"];
                xmlData.FirstChild.AppendChild(cssLabelClass);
            }

            if(!string.IsNullOrEmpty(this.LabelAttributes["style"])) {
                XmlNode labelStyle = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "LabelStyle", xmlns);
                labelStyle.InnerText = this.LabelAttributes["style"];
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
                return this.Text;
            }
            set
            {
                this.Text = value;
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
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            
            if (!string.IsNullOrEmpty(this.Label))
            {
                if (this.LabelStyles != null)
                {
                    string styleString = string.Empty;
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        styleString+=key+":"+(string)this.LabelStyles[key]+";";
                    }
                    // TODO: Avoid overwriting LabelAttributes property
                    this.LabelAttributes.Add("style", styleString);
                }
            }
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.Enabled = false;
            }
            base.Render(writer);
        }
        #endregion

    }
}
