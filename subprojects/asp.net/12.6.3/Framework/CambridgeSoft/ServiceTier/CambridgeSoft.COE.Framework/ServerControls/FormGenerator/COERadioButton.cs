using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a RadioButton control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COERadioButton class accepts every RadioButton property to be set, but as ICOEGenerable control it also provides
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
    ///         <item>configInfo: Additional attributes like GroupName, CSSClass, Style, Width, Height, ID...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;defaultValue&gt;true&lt;/defaultValue&gt;
    ///     &lt;bindingExpression&gt;Compound.BaseFragment.Structure.UseNormalizedStructure&lt;/bindingExpression&gt;
    ///     &lt;label&gt;Use normalized structure&lt;/label&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;GroupName&gt;StructureSelectionGroup&lt;/GroupName&gt;
    ///       &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
    ///       &lt;ID&gt;UseNormalizedStructureRadio&lt;/ID&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;top&gt;310px&lt;/top&gt;
    ///       &lt;left&gt;100px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COERadioButton&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Checked property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COERadioButton runat=server></{0}:COERadioButton>")]
    public class COERadioButton : RadioButton, ICOEGenerableControl, ICOERequireable, ICOELabelable {
        
        #region Variables
        private string _defaultValue = string.Empty;
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
            return this.Checked;
        }

        /// <summary>
        /// Sets if it should be checked.
        /// </summary>
        /// <param name="data">A bool or a parseable string that sets if this should be checked or not.</param>
        public void PutData(object data)
        {
            if (data.GetType() == typeof(string))
                this.Checked = ((((string)data).Trim()).ToLower()).Equals("true") ? true : false;
            else
                throw new Exception("Incompatible types.");
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;GroupName&gt;StructureSelectionGroup&lt;/GroupName&gt;
        ///   &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
        ///   &lt;ID&gt;UseNormalizedStructureRadio&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
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
             *  <GroupName></GroupName>
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
            XmlNode grpName = xmlData.SelectSingleNode("//COE:GroupName", manager);
            if (grpName != null && grpName.InnerText.Length > 0)
            {
                this.GroupName  = grpName.InnerText;
            }
            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if(cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                this.LabelAttributes.Add("class", cssLabelClass.InnerText);

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", manager);
            if(labelStyle != null && labelStyle.InnerText.Length > 0) {
                this.LabelAttributes.Add("style", string.Empty);
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for(int i = 0; i < styles.Length; i++) {
                    if(styles[i].Length > 0) {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        this.LabelAttributes["style"] += styleId + ":" + styleValue + ";";
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

        #region Life Cycle Events
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetLabelStyles(this.RequiredLabelStyle, this);
                if (this.LabelStyles != null)
                {
                    string styleString = string.Empty;
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        styleString += key + ":" + (string)this.LabelStyles[key] + ";";
                    }
                    // TODO: Avoid overwriting LabelAttributes property
                    this.LabelAttributes.Add("style", styleString);
                }
            }
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            base.Render(writer);
        }
        #endregion
    }
}
