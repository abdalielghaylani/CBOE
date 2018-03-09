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
using System.Collections;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// COE Web Control to work with Dates.
    /// </summary>
    [ToolboxData("<{0}:COEWebDateEdit runat=server></{0}:COEWebDateEdit>")]
    public class COEWebDateEdit : WebDateTimeEdit, ICOEGenerableControl, ICOELabelable, ICOECultureable
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
                        {   //Using COEDatabinder find the object property given a bindingExpression in the current object
                            CambridgeSoft.COE.Framework.Controls.COEDataMapper.COEDataBinder dataBinder =
                                new CambridgeSoft.COE.Framework.Controls.COEDataMapper.COEDataBinder(this);
                            dataBinder.SetProperty(bindingExpression, configSetting.InnerText);
                        }
                        catch(Exception ex)
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
                 if (this.LabelStyles != null)
                {
                    IEnumerator styles = this.LabelStyles.Keys.GetEnumerator();
                    while (styles.MoveNext())
                    {
                        string key = (string)styles.Current;
                        _lit.Style.Add(key, (string)this.LabelStyles[key]);
                    }
                }
				_lit.Text = this.Label;
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
