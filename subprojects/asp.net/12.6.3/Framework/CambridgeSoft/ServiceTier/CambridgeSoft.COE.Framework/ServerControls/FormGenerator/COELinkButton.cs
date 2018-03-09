using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Collections;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a LinkButton control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COELinkButton class accepts every LinkButton property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default text?</item>
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
    ///       &lt;defaultValue&gt;Resolve This Compound&lt;/defaultValue&gt;
    ///       &lt;bindingExpression/&gt;
    ///       &lt;serverEvents&gt;
    ///         &lt;event eventName="Click" handlerName="ResolveLinkButton_Click"/&gt;
    ///       &lt;/serverEvents&gt;
    ///       &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
    ///         &lt;ID&gt;ResolveLinkButton&lt;/ID&gt;
    ///         &lt;/fieldConfig&gt;
    ///       &lt;/configInfo&gt;
    ///       &lt;displayInfo&gt;
    ///         &lt;top&gt;325px&lt;/top&gt;
    ///         &lt;left&gt;100px&lt;/left&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELinkButton&lt;/type&gt;
    ///       &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    ///   &lt;/formElements&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Text property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COELinkButton runat=server></{0}:COELinkButton>")]
    public class COELinkButton : LinkButton, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEFullDatasource
    {

        #region Variables
        string _defaultLink = string.Empty;
        private Label _lit = new Label();
        private object _datasource;
        private Dictionary<string, string> _params = new Dictionary<string, string>();
        #endregion

        #region ICOEGenerableControl
        /// <summary>
        /// <para>Gets the Text of the link button.</para>
        /// </summary>
        /// <returns>A string with the link button's text</returns>
        public object GetData()
        {
            return this.Text;
        }

        /// <summary>
        /// <para>Sets the link button's text.</para>
        /// </summary>
        /// <param name="data">A string with the desired link button's text</param>
        public void PutData(object data)
        {
            if(!(data is int))
                this.Text = data.ToString();
            this.SetCommandArg();
        }

        /// <summary>
        /// Read params from xml snipet and load them into a list
        /// </summary>
        /// <param name="paramsText"></param>
        private void LoadParams(XmlNode paramsXml, XmlNamespaceManager manager)
        {
            if (paramsXml != null)
            {
                foreach (XmlNode param in paramsXml.SelectNodes("./COE:param", manager))
                {
                    if (param.Attributes["databindingexpression"] != null && param.Attributes["formattedText"] != null)
                        _params.Add(param.Attributes["formattedText"].Value, param.Attributes["databindingexpression"].Value);
                }
            }
        }

        private void SetCommandArg()
        {
            object dataItemValue;
            try
            {
                //If we have to replace something and the datasource is the full datasource...
                if (_datasource != null)
                {
                    COEDataBinder dataBinder = new COEDataBinder(_datasource);
                    foreach (KeyValuePair<string, string> param in _params)
                    {
                        dataItemValue = dataBinder.RetrieveProperty(param.Value);
                        if (dataItemValue != null)
                        {
                            if (!this.CommandName.Contains(dataItemValue.ToString()))
                            {
                                this.CommandName += dataItemValue.ToString();
                                this.CommandArgument += dataItemValue.ToString();
                                //this.CommandArgument += string.Format(param.Key, dataItemValue); //Todo: find a better and more explicit way to set the params
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
        ///   &lt;ID&gt;ResolveLinkButton&lt;/ID&gt;
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {
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

            XmlNode txt = xmlData.SelectSingleNode("//COE:Text", manager);
            if (txt != null && txt.InnerText.Length > 0)
            {
                this.Text = txt.InnerText;
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

            XmlNode commandArgs = xmlData.SelectSingleNode("//COE:CommandArguments", manager);
            if (commandArgs != null && !string.IsNullOrEmpty(commandArgs.InnerXml))
                this.LoadParams(commandArgs, manager);

            XmlNode causesValidation = xmlData.SelectSingleNode("//COE:CausesValidation", manager);
            if (causesValidation != null && !string.IsNullOrEmpty(causesValidation.InnerXml))
                this.CausesValidation = causesValidation.InnerXml.ToUpper() == "TRUE" ? true : false;
        }

        /// <summary>
        /// <para>Sets the default Text.</para>
        /// </summary>
        public string DefaultValue
        {
            get { return _defaultLink; }
            set { _defaultLink = value; }
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
            base.Render(writer);
        }
        #endregion

        #region ICOEFullDatasource Members

        public object FullDatasource
        {
            set { _datasource = value; }
            get { return _datasource; }
        }

        #endregion
    }
}
