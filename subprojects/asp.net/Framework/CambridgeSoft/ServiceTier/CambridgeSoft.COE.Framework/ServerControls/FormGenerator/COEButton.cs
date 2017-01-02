using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// COE Button web control.
    /// </summary>
    /// <summary>
    /// <para>
    /// This class implements a button control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COECurrencyEdit class accepts every Button property to be set, but as ICOEGenerable control it also provides
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
    ///        &lt;formElement name="AdvSearchLink"&gt;
    ///          &lt;showHelp>false&lt;/showHelp&gt;
    ///          &lt;isFileUpload>false&lt;/isFileUpload&gt;
    ///          &lt;pageComunicationProvider /&gt;
    ///          &lt;fileUploadBindingExpression /&gt;
    ///          &lt;showMoreDetails>false&lt;/showMoreDetails&gt;
    ///          &lt;helpText /&gt;
    ///          &lt;defaultValue&gt;/ChemSource/Forms/Search/MaterialsSearch.aspx&lt;/defaultValue&gt;
    ///          &lt;bindingExpression /&gt;
    ///          &lt;Id>VM_AdvSearchButton&lt;/Id&gt;
    ///          &lt;displayInfo&gt;
    ///            &lt;type>CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEButton&lt;/type&gt;
    ///            &lt;visible>true&lt;/visible&gt;
    ///          &lt;/displayInfo&gt;
    ///          &lt;validationRuleList /&gt;
    ///          &lt;serverEvents&gt;
    ///            &lt;event handlerName="AdvSearchMaterial_Click" eventName="Click" /&gt;
    ///          &lt;/serverEvents&gt;
    ///          &lt;clientEvents /&gt;
    ///          &lt;configInfo&gt;
    ///            &lt;fieldConfig&gt;
    ///              &lt;configSetting bindingExpression="this.Text"&gt;Structure/Advanced Search&lt;/configSetting&gt;
    ///              &lt;configSetting bindingExpression="this.CssClass"&gt;COEAdvSearchButton&lt;/configSetting&gt;
    ///              &lt;configSetting bindingExpression="this.CommandName"&gt;/ChemSource/Forms/Search/MaterialsSearch.aspx&lt;/configSetting&gt;
    ///            &lt;/fieldConfig&gt;
    ///          &lt;/configInfo&gt;
    ///          &lt;dataSource /&gt;
    ///          &lt;dataSourceId /&gt;
    ///          &lt;displayData /&gt;
    ///        &lt;/formElement&gt;
    /// </code>
    /// </summary>
    [ToolboxData("<{0}:COEButton runat=server></{0}:COEButton>")]
    public class COEButton : Button, ICOEGenerableControl, ICOELabelable
    {

        #region Variables

        private string _defaultValue = string.Empty;
        private Label _lit = new Label();

        #endregion

        #region Properties

        /// <summary>
        /// Sets the CSS label class.
        /// </summary>
        /// <value>The CSS label class.</value>
        public string CSSLabelClass
        {
            set { _lit.CssClass = value; }
        }

        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return null;
        }

        public void PutData(object data)
        {
            ;
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

        #region ICOELabelable Members

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
    }
}
