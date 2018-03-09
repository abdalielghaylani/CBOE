using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Collections;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// COE Radio Button List control.
    /// </summary>
    /// <summary>
    /// <para>
    /// This class implements a Radio button list control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COECurrencyEdit class accepts every RadioButtonList property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: Value to be displayed by default</item>
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
    ///                     &lt;formElement name="MSDSRadioButton"&gt;
    ///                        &lt;label&gt;Can generate own MSDS&lt;/label&gt;
    ///                        &lt;bindingExpression&gt;this.GenerateMSDS&lt;/bindingExpression&gt;
    ///                        &lt;Id&gt;AM_MSDSRadioButton&lt;/Id&gt;
    ///                        &lt;displayInfo&gt;
    ///                          &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COERadioButtonList&lt;/type&gt;
    ///                        &lt;/displayInfo&gt;
    ///                        &lt;configInfo&gt;
    ///                          &lt;fieldConfig&gt;
    ///                            &lt;Width&gt;400px&lt;/Width&gt;
    ///                            &lt;CSSClass&gt;COEBoolean SameLine&lt;/CSSClass&gt;
    ///                            &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
    ///                            &lt;ReadOnly&gt;true&lt;/ReadOnly&gt;
    ///                            &lt;LayoutStyle&gt;Flow&lt;/LayoutStyle&gt;
    ///                            &lt;ListItems&gt;
    ///                              &lt;Item text="Yes" value="true" /&gt;
    ///                              &lt;Item text="No" value="false" /&gt;
    ///                            &lt;/ListItems&gt;
    ///                            &lt;DefaultSelectedValue&gt;false&lt;/DefaultSelectedValue&gt;
    ///                          &lt;/fieldConfig&gt;
    ///                        &lt;/configInfo&gt;
    ///                      &lt;/formElement&gt;
    /// </code>
    /// </summary>
    
    [ToolboxData("<{0}:COERadioButtonList runat=server></{0}:COERadioButtonList>")]
    public class COERadioButtonList : RadioButtonList, ICOEGenerableControl, ICOERequireable, ICOELabelable
    {
        #region Variables

        private string _defaultValue = string.Empty;
        private Label _lit = new Label();
        private string _defaultSelectedValue = string.Empty; 
        private bool _readOnly = false;

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

        public object GetData()
        {
            return !string.IsNullOrEmpty(this.SelectedValue) ? this.SelectedValue : string.Empty;
        }

        public void PutData(object data)
        {
            if (data.GetType() == typeof(string))
            {
                ListItem lstItem = this.Items.FindByValue(data.ToString()); //Coverity Fix CID 13988 ASV
                if (lstItem != null)
                    this.SelectedValue = lstItem.Value;
            }
            if (data.GetType() == typeof(bool))
            {
                foreach (ListItem item in this.Items)
                {
                    if (item.Value.ToLower() == data.ToString().ToLower())
                    {
                        this.SelectedValue = item.Value;
                        break;
                    }
                }
            }
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

            XmlNode id = xmlData.SelectSingleNode("//COE:ID", manager);
            if (id != null && id.InnerText.Length > 0)
            {
                this.ID = id.InnerText;
            }

            //coverity fix
            XmlNode text = xmlData.SelectSingleNode("//COE:Text", manager);
            if (text != null && ID.Length > 0)
            {
                this.Text = text.InnerText;
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode layoutStyle = xmlData.SelectSingleNode("//COE:LayoutStyle", manager);
            if (layoutStyle != null && layoutStyle.InnerText.Length > 0)
                switch (layoutStyle.InnerText.ToUpper())
                {
                    case "FLOW": this.RepeatLayout = RepeatLayout.Flow; break;
                    case "TABLE": this.RepeatLayout = RepeatLayout.Table; break;
                }


            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode defaultSelectedValueNode = xmlData.SelectSingleNode("//COE:DefaultSelectedValue", manager);
            if (defaultSelectedValueNode != null && defaultSelectedValueNode.InnerText.Length > 0)
                _defaultSelectedValue = defaultSelectedValueNode.InnerText;

            XmlNode listItemsNode = xmlData.SelectSingleNode("//COE:ListItems", manager);
            if (listItemsNode != null && !string.IsNullOrEmpty(listItemsNode.InnerXml))
                this.LoadParams(listItemsNode, manager);

            XmlNode readOnlyNode = xmlData.SelectSingleNode("//COE:ReadOnly", manager);
            if (readOnlyNode != null && readOnlyNode.InnerText.Length > 0)
                _readOnly = bool.Parse(readOnlyNode.InnerText);

        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (_readOnly)
                this.Enabled = !_readOnly;

            this.SetDefaultSelectedValue();

            if (!string.IsNullOrEmpty(this.Label))
            {
                _lit.Text = this.Label;
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.ConfigureLabelStyles(this.RequiredLabelStyle, this, _lit);
                _lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                _lit.RenderControl(writer);
                this.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            if (!string.IsNullOrEmpty(this.RequiredStyle))
            {
                CambridgeSoft.COE.Framework.ServerControls.Utilities.LabelStylesParser.SetRequiredControlStyles(this.RequiredStyle, this);
            }
            base.Render(writer);
        }

        /// <summary>
        /// Sets the default selected value.
        /// </summary>
        /// <remarks>Checks if there an already selected value</remarks>
        private void SetDefaultSelectedValue()
        {
            if (!string.IsNullOrEmpty(_defaultSelectedValue) && string.IsNullOrEmpty(this.SelectedValue))
            {
                ListItem lstItem = this.Items.FindByValue(_defaultSelectedValue); //Coverity Fix CID 13989 ASV
                if (lstItem != null)
                    this.SelectedValue = lstItem.Value;
            }
        }

        /// <summary>
        /// Read params from xml snipet and load them into a list
        /// </summary>
        /// <param name="paramsText"></param>
        private void LoadParams(XmlNode paramsXml, XmlNamespaceManager manager)
        {
            if (paramsXml != null)
            {
                foreach (XmlNode param in paramsXml.SelectNodes("./COE:Item", manager))
                {
                    if (param.Attributes["text"] != null && param.Attributes["value"] != null)
                    {
                        if (!string.IsNullOrEmpty(param.Attributes["text"].Value) && !string.IsNullOrEmpty(param.Attributes["value"].Value))
                        {
                            ListItem item = new ListItem(param.Attributes["text"].Value, param.Attributes["value"].Value);
                            if (param.Attributes["onclick"] != null)
                                item.Attributes.Add("onclick", param.Attributes["onclick"].Value);
                            this.Items.Add(item);
                        }
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
        /// Gets or sets the label's css class.
        /// </summary>
        public string LabelCSSClass
        {
            get
            {
                if (ViewState[Constants.CSSClass_VS] != null)
                    return (string)ViewState[Constants.CSSClass_VS];
                else
                    return this.CssClass;
            }
            set
            {
                ViewState[Constants.CSSClass_VS] = value;
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
                if (ViewState[CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.RequiredStyle_VS] != null)
                    return (string)ViewState[CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.RequiredStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.RequiredStyle_VS] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ICOERequireable label's css style
        /// </summary>
        public string RequiredLabelStyle
        {
            get
            {
                if (ViewState[CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.RequiredLabelStyle_VS] != null)
                    return (string)ViewState[CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.RequiredLabelStyle_VS];
                else
                    return String.Empty;
            }
            set
            {
                ViewState[CambridgeSoft.COE.Framework.Controls.COEFormGenerator.Constants.RequiredLabelStyle_VS] = value;
            }
        }

        #endregion


        internal class Constants
        {
            #region ViewState Constants
            public const string Label_VS = "Label";
            public const string CSSClass_VS = "CSSClass";
            public const string LabelStyles_VS = "LabelStyles";
            #endregion
        }
    }
}
