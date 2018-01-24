using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.WebUI.WebDataInput;
using System.Globalization;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Collections;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// COE Web Control to work with currency fields (E.g $120.00).
    /// </summary>
    /// <summary>
    /// <para>
    /// This class implements a Currency TextBox control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COECurrencyEdit class accepts every WebCurrencyEdit(Infragistics) property to be set, but as ICOEGenerable control it also provides
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
    ///                     &lt;formElement&gt;
    ///                        &lt;Id&gt;AM_Total&lt;/Id&gt;
    ///                        &lt;bindingExpression&gt;Total&lt;/bindingExpression&gt;
    ///                        &lt;configInfo&gt;
    ///                          &lt;fieldConfig&gt;
    ///                            &lt;configSetting bindingExpression="this.ID"&gt;UniqueID_CurrencyEditor&lt;/configSetting&gt;
    ///                            &lt;configSetting bindingExpression="this.CssClass"&gt;COECurrencyEdit&lt;/configSetting&gt;
    ///                            &lt;configSetting bindingExpression="this.ReadOnly"&gt;true&lt;/configSetting&gt;
    ///                          &lt;/fieldConfig&gt;
    ///                        &lt;/configInfo&gt;
    ///                        &lt;displayInfo&gt;
    ///                          &lt;type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COECurrencyEdit&lt;/type>
    ///                        &lt;/displayInfo&gt;
    ///                      &lt;/formElement&gt;
    /// </code>
    /// </summary>
    [ToolboxData("<{0}:COECurrencyEdit runat=server></{0}:COECurrencyEdit>")]
    public class COECurrencyEdit : WebCurrencyEdit, ICOEGenerableControl, ICOELabelable, ICOECultureable
    {
        #region Variables

        private string _defaultValue = string.Empty;
        private Label _lit = new Label();

        #endregion

        #region ICOEGenerableControl Members

        /// <summary>
        /// Sets the CSS label class.
        /// </summary>
        /// <value>The CSS label class.</value>
        public string CSSLabelClass
        {
            set { _lit.CssClass = value; }
        }

        /// <summary>
        /// <para>Gets the value of the control.</para>
        /// </summary>
        /// <returns>A string with the control's value.</returns>
        public object GetData()
        {
            EnsureChildControls();
            return this.Value;
        }

        /// <summary>
        /// Sets the control's value.
        /// </summary>
        /// <param name="data">A string with the desired value of the control.</param>
        public void PutData(object data)
        {
            this.Value = data.ToString();
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;configSetting bindingExpression="this.CSSLabelClass"&gt;COELabel&lt;/configSetting&gt;
        ///   &lt;configSetting bindingExpression="this.SpinButtons.Display"&gt;OnRight&lt;/configSetting&gt;
        /// &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);
            foreach (XmlNode configSetting in xmlData.SelectNodes("//COE:configSetting", manager))
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
        }

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


        #endregion

        #region Control Lyfe cycle

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

        #region ICOECultureable Members

        /// <summary>
        /// Sets the culture info for the underlying control.
        /// <see cref="CultureInfo"/>
        /// </summary>
        /// <value></value>
        public System.Globalization.CultureInfo DisplayCulture
        {
            set
            {
                try { base.Culture = value; }
                catch { base.Culture = CultureInfo.CurrentUICulture; }
            }
        }

        #endregion
    }
}

