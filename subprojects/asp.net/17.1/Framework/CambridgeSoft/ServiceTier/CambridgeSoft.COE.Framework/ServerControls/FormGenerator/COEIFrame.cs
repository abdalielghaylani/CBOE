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
    /// This class implements an IFrame control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEIFrame class accepts URLParams value as a COELink does. It supports binding data values to its
    /// src parameter so the IFrame contents can be parameterized according to the current record
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>configInfo: Attributes like HRef, Target, URLParams, CSSClass, Style, Width, Height, ID...</item>
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
    ///       &lt;configInfo&gt;
    ///         &lt;fieldConfig&gt;
    ///         &lt;CSSClass&gt;COEIFrame&lt;/CSSClass&gt;
    ///         &lt;HRef&gt;/Path/toIFrameContent/Content.aspx?BindedField={0}&lt;/HRef&gt;
	/// 		&lt;Target&gt;_parent&lt;/Target&gt;
    ///         &lt;URLParams&lt;
    ///         &lt;param databindingexpression="BindedField" formattedText="BindedField={0}" /&gt;
    ///         &lt;/URLParams&gt;
    ///         &lt;/fieldConfig&gt;
    ///       &lt;/configInfo&gt;
    ///       &lt;displayInfo&gt;
    ///         &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEIFrame&lt;/type&gt;
    ///       &lt;/displayInfo&gt;
    ///     &lt;/formElement&gt;
    ///   &lt;/formElements&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation GetData reffers to the _urlParamName attribute.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEIFrame runat=server></{0}:COEIFrame>")]
    public class COEIFrame : WebControl, ICOEGenerableControl, ICOEFullDatasource 
    {
        #region Variables
        private string _hRefFormat = "{0}";
        private object _datasource;
        private Boolean _hideIfEmptyData;
        private string _urlParamName;
        private Dictionary<string, string> _params = new Dictionary<string, string>();
        #endregion

        #region ICOEGenerableControl
        /// <summary>
        /// <para>Gets the HRef of the link.</para>
        /// </summary>
        /// <returns>A string with the link's HRef</returns>
        public object GetData()
        {
            return this._urlParamName;
        }

        /// <summary>
        /// <para>Sets the link's HRef and text</para>
        /// </summary>
        /// <param name="data">A string with a concat of the text and url to display</param>
        public void PutData(object data)
        {
            this._urlParamName  = this.FormatURL(data.ToString());
            this.Attributes["src"] = this._urlParamName;
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

            node = xmlData.SelectSingleNode("//COE:HRef", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerXml))
                this._hRefFormat = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:Name", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerXml))
                this.Attributes["name"] = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:FrameBorder", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerXml))
                this.Attributes["frameborder"] = node.InnerText;

            node = xmlData.SelectSingleNode("//COE:CSSClass", manager);
            if (node != null && !string.IsNullOrEmpty(node.InnerText))
                this.Attributes["class"] = node.InnerText;

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
            get { return !string.IsNullOrEmpty(this._urlParamName) ? this._urlParamName : this._hRefFormat; }
            set { }
        }

        #endregion

        #region ICOEFullDatasource Members

        public object FullDatasource
        {
            set { _datasource = value; }
            get { return _datasource; }
        }

        #endregion

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Iframe;
            }
        }

    }
}