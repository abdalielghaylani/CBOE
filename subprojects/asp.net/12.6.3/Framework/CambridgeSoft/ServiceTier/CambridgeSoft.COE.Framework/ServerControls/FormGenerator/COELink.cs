using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using System.Collections.Specialized;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Web;

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
    /// ImageURL is optional if it is present the View Text is shown as alternative text for the image.
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
    ///         &lt;ImageURL&gt;/ImageFolder/linkImage.png&lt;/ImageURL&gt;
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
    public class COELink : HyperLink, ICOEGenerableControl, ICOELabelable, ICOERequireable, ICOEFullDatasource, ICOEDesignable, ICOEReadOnly
    {
        #region Properties
        public string PopupOptions
        {
            get { return (string)ViewState["PopupOptions"]; }
            set { ViewState["PopupOptions"] = value; }
        }

        public string ClientClick
        {
            get { return ViewState["ClientClick"] == null ? string.Empty : (string)ViewState["ClientClick"]; }
            set { ViewState["ClientClick"] = value; }
        }
        #endregion

        #region Variables
        private string _hRefFormat = "{0}";
        private object _datasource;
        private Boolean _hideIfEmptyData;
        private Dictionary<string, string> _params = new Dictionary<string, string>();
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region ICOEGenerableControl
        /// <summary>
        /// <para>Gets the HRef of the link.</para>
        /// </summary>
        /// <returns>A string with the link's HRef</returns>
        public object GetData()
        {
            return this.NavigateUrl;
        }

        /// <summary>
        /// <para>Sets the link's HRef.</para>
        /// </summary>
        /// <param name="data">A string with the desired link button's text</param>
        public void PutData(object data)
        {
            if (_hideIfEmptyData && string.IsNullOrEmpty(data.ToString()))
                this.Enabled = false;
            else
                this.NavigateUrl = this.FormatURL(data.ToString());
            if (string.IsNullOrEmpty(this.Text))
                this.Text = data.ToString();
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
            XmlNode node = xmlData.SelectSingleNode("//COE:Style", manager);
            if (node != null && node.InnerText.Length > 0)
            {
                string[] styles = node.InnerText.Split(new char[1] { ';' });
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

            node = xmlData.SelectSingleNode("//COE:Text", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.Text = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:HRef", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this._hRefFormat = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.Attributes["class"] = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:Target", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.Target = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:PopupOptions", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.PopupOptions = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:URLParams", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerXml))
                this.LoadParams(node, manager);

            node = xmlData.SelectSingleNode("//COE:HideIfEmptyData", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this._hideIfEmptyData = bool.Parse(node.InnerText);

            node = xmlData.SelectSingleNode("//COE:ImageURL", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.ImageUrl = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:ClientClick", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.ClientClick = node.InnerText;
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

        /// <summary>
        /// Format the URL replacing some given parameters
        /// </summary>
        /// <param name="data">Formatted string</param>
        /// <returns>A clean url string</returns>
        private string FormatURL(string data)
        {
            string retVal = string.Empty;
            object dataItemValue;
            retVal = string.Format(_hRefFormat, data);
            string serverName = HttpContext.Current.Request.Url.Authority;
            string protocol = HttpContext.Current.Request.Url.Scheme;

            Page page = (this.Page == null ? ((Page)System.Web.HttpContext.Current.CurrentHandler) : this.Page);

            string ticket = page.Request.Cookies["COESSO"] != null ? page.Request.Cookies["COESSO"].Value : string.Empty;
            //Reserved words
            string serverNameKey = "|_serverName_|";
            string protocolKey = "|_protocol_|";
            string ticketKey = "|_ticket_|";

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
                            retVal += "&" + string.Format(param.Key, dataItemValue);
                    }
                }

                //Now replace reserved words as protocol and serverName.
                if (retVal.Contains(serverNameKey))
                    retVal = retVal.Replace(serverNameKey, serverName);
                if (retVal.Contains(protocolKey))
                    retVal = retVal.Replace(protocolKey, protocol);
                if (retVal.Contains(ticketKey) && !string.IsNullOrEmpty(ticket))
                    retVal = retVal.Replace(ticketKey, ticket);
                retVal = CorrectUrlFormat(retVal);
            }
            catch (Exception ex)
            { }
            return retVal;
        }


        /// <summary>
        /// Validate URL string to check protocol.Formats string to AbsoluteUri
        /// </summary>
        /// <param name="retVal">Un constructed URL string</param>
        private string CorrectUrlFormat(string retVal)
        {
            try
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    bool isFile = (retVal.StartsWith("//") || retVal.StartsWith("\\") || retVal.StartsWith("/") || retVal.StartsWith(@"\")) ? true : false;
                    string validFilePathKey = "\\";
                    string invalidvalidFilePathKey = "//";

                    if (isFile)
                        retVal = retVal.Replace(invalidvalidFilePathKey, validFilePathKey);
                    else if (!Uri.IsWellFormedUriString(retVal, UriKind.Absolute))  // Check with URI Builder to Append protocol;
                        retVal = (string)new UriBuilder(retVal).Uri.AbsoluteUri;
                }
            }
            catch (Exception ex)
            { }
            return retVal;
        }

        /// <summary>
        /// <para>Sets the default Text.</para>
        /// </summary>
        public string DefaultValue
        {
            get { return !string.IsNullOrEmpty(this.NavigateUrl) ? this.NavigateUrl : this._hRefFormat; }
            set { }
        }
        #endregion

        #region ICOELabelable Members
        /// <summary>
        /// Gets or sets the Control's label.
        /// </summary
        public string Label
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Text))
                    return this.Text;
                else
                    return string.Empty;
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

        #region Overriden Methods
        protected override void OnPreRender(EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.PopupOptions))
            {
                this.Attributes.Add("onclick", ClientClick + " window.open(this.href, '" + this.Target + "','" + this.PopupOptions + "'); return false;");
            }
            else if (!string.IsNullOrEmpty(ClientClick))
                this.Attributes.Add("onclick", ClientClick);

            base.OnPreRender(e);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            //setting the link to disabled if readonly is set.
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.Enabled = false;
                if (this.Attributes["onclick"] != null)
                    this.Attributes["onclick"] = "return false;";
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

            if (string.IsNullOrEmpty(this.CssClass))
            {
                XmlNode cssClass = xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "CSSClass", xmlns);
                cssClass.InnerText = this.CssClass;
                xmlData.FirstChild.AppendChild(cssClass);
            }

            return xmlData.FirstChild;
        }

        #endregion
    }
}
