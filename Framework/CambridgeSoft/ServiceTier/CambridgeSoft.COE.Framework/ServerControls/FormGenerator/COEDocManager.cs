using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using System.Xml.XPath;
using System.Data;
using System.Collections;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements an infragistics' UltraWebGrid that gets the doc manager (v11) documents associated to a registration record 
    /// and may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEDocManager class accepts every UltraWebGrid property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What value would be the default reg number?</item>
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
    ///     &lt;label&gt;Doc Manager associated documents:&lt;/label&gt;
    ///     &lt;bindingExpression&gt;RegNumber.RegNum&lt;/bindingExpression&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
    ///       &lt;CSSLabelClass&gt;COELabelTitle&lt;/CSSLabelClass&gt;
    ///       &lt;CSSLinkClass&gt;COELabelTitle&lt;/CSSLinkClass&gt;
    ///       &lt;Protocol&gt;http&lt;/Protocol&gt;
    ///       &lt;TargetURL&gt;%2Fdocmanager%2Fdocmanager%2Fexternallinks%2FgetDocumentsNoGUI.asp&lt;/TargetURL&gt;
    ///       &lt;AddDocsURL&gt;%2Fdocmanager%2Fdefault.asp%3Fdataaction%3Ddb%26formgroup%3Dbase_form_group%26dbname%3Ddocmanager%26extAppName%3DChem_Reg%26LinkType%3DCHEMREGREGNUMBER%26linkfieldname%3DROOT_NUMBER%26showselect%3Dtrue%26extlinkid%3D&lt;/AddDocsURL&gt;
    ///       &lt;ReturnType&gt;xml&lt;/ReturnType&gt;
    ///       &lt;LinkType&gt;CHEMREGREGNUMBER&lt;/LinkType&gt;
    ///       &lt;LinkTarget&gt;_blank&lt;/LinkTarget&gt;
    ///       &lt;DeleteItemText&gt;Delete&lt;/DeleteItemText&gt;
    ///       &lt;ShowLogo&gt;false&lt;/ShowLogo&gt;
    ///       &lt;EmptyGridMessage&gt;No documents links currently exists.&lt;/EmptyGridMessage&gt;
    ///       &lt;ShowSubmitDate&gt;true&lt;/ShowSubmitDate&gt;
    ///       &lt;Columns&gt;
    ///       &lt;Column key='Name' width='80px'/&gt;
    ///       &lt;Column key='Link' width='0px'/&gt;
    ///       &lt;Column key='DocName' width='250px'/&gt;
    ///       &lt;Column key='Owner' width='80px'/&gt;
    ///       &lt;Column key='Date' type='Button' width='200px'/&gt;
    ///       &lt;Column key='Delete' width='100px'/&gt;
    ///       &lt;/Columns&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;top&gt;325px&lt;/top&gt;
    ///       &lt;left&gt;5px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDocManagerClassic&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// <b>The xml examples are based on the "Classic" implementation of DocManager, because version 11 of DocManager is still unavailable.</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", and PutData Methods reffer to the registry number, and GetData reffers to the documents
    /// associated to that registration number.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEDocManager runat=server></{0}:COEDocManager")]
    public class COEDocManager : Infragistics.WebUI.UltraWebGrid.UltraWebGrid, ICOEGenerableControl, ICOERequireable, ICOELabelable
    {
        #region Variables

        private string _defaultValue = string.Empty;
        DataTable infoDataTable = new DataTable("DoxManagerLinks");
        private string _sourceServer;
        private string _regNumberID = String.Empty;
        private string _linkType = String.Empty;
        private string _returnType = String.Empty;
        private string _linkColumn = String.Empty;
        private bool _showLogo = false;
        private bool _showsubmitdate = false;
        private List<XmlNode> _columnsToDisplay = new List<XmlNode>();
        private Label _lit = new Label();

        /// <summary>
        /// Enumeration for return types. Allowed values are:
        /// <list type="bullet">
        ///   <item>Xml</item>
        ///   <item>String</item>
        /// </list>
        /// </summary>
        private enum ReturnTypes
        {
            xml,
            String,
        }

        /// <summary>
        /// Ling types enumeration.
        /// </summary>
        private enum LinkTypes
        {
            CHEMREGREGNUMBER,
        }

        #endregion

        #region Properties
        /// <summary>
        /// The LinkType, currently Chem registry number is the only allowed.
        /// </summary>
        private LinkTypes LinkType
        {
            get
            {
                LinkTypes currentLinkType = LinkTypes.CHEMREGREGNUMBER;

                switch (this._linkType)
                {
                    case "CHEMREGREGNUMBER":
                        currentLinkType = LinkTypes.CHEMREGREGNUMBER;
                        break;
                }

                return currentLinkType;
            }
        }

        /// <summary>
        /// The return type, that may be xml or string.
        /// </summary>
        private ReturnTypes ReturnType
        {
            get
            {
                ReturnTypes currentReturnType = ReturnTypes.xml;

                switch (this._returnType)
                {
                    case "xml":
                        currentReturnType = ReturnTypes.xml;
                        break;
                    case "String":
                        currentReturnType = ReturnTypes.String;
                        break;
                }
                return currentReturnType;
            }
        }

        /// <summary>
        /// Would the logo be shown?
        /// </summary>
        private bool ShowLogo
        {
            get { return this._showLogo; }
        }

        /// <summary>
        /// Would the submission date be shown?
        /// </summary>
        private bool ShowSubmitDate
        {
            get { return this._showsubmitdate; }
        }

        #endregion

        #region Constructor

        public COEDocManager()
        {
            //All of these are cosmetic settings
            this.DisplayLayout.AllowColSizingDefault = Infragistics.WebUI.UltraWebGrid.AllowSizing.Free;
            this.DisplayLayout.AllowColumnMovingDefault = Infragistics.WebUI.UltraWebGrid.AllowColumnMoving.OnServer;
            this.DisplayLayout.AllowSortingDefault = Infragistics.WebUI.UltraWebGrid.AllowSorting.OnClient;
            this.DisplayLayout.BorderCollapseDefault = Infragistics.WebUI.UltraWebGrid.BorderCollapse.Separate;
            this.DisplayLayout.HeaderClickActionDefault = Infragistics.WebUI.UltraWebGrid.HeaderClickAction.SortSingle;
            this.DisplayLayout.RowHeightDefault = new Unit("20px");
            this.DisplayLayout.RowSelectorsDefault = Infragistics.WebUI.UltraWebGrid.RowSelectors.No;
            this.DisplayLayout.SelectTypeRowDefault = Infragistics.WebUI.UltraWebGrid.SelectType.Extended;
            this.DisplayLayout.TableLayout = Infragistics.WebUI.UltraWebGrid.TableLayout.Fixed;
            this.DisplayLayout.ViewType = Infragistics.WebUI.UltraWebGrid.ViewType.OutlookGroupBy;
            this.DisplayLayout.AutoGenerateColumns = true;
            this.DisplayLayout.GroupByBox.Style.CssClass = "BandLabel";
        }

        #endregion

        #region COEGenerableControl Members
        /// <summary>
        /// Gets or sets the default registry number.
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
        /// Gets the documents associated to the previously set registry number.
        /// </summary>
        /// <returns>A DataTable with the associated links to the documents found.</returns>
        public object GetData()
        {
            return this.infoDataTable;
        }

        /// <summary>
        /// Sets the registry number that we are looking documents for.
        /// </summary>
        /// <param name="data">A string with the registry number</param>
        public void PutData(object data)
        {
            _regNumberID = data.ToString();
        }



        /// <summary>
        /// Method to read from the url response and set the columns title text
        /// </summary>
        /// <param name="xmlDoc">The xml source of information</param>
        private void SetDataTableColumnsText(XmlDocument xmlDoc)
        {
            //foreach (XmlElement currentNode in xmlDoc.ChildNodes[0].ChildNodes[0].ChildNodes)
            //    infoDataTable.Columns.Add(currentNode.Name); 
            foreach (XmlNode propDescriptor in _columnsToDisplay)
            {
                infoDataTable.Columns.Add(propDescriptor.Attributes["headerText"].Value);
            }
        }

        /// <summary>
        /// Here we try to get the info from a source Url.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnDataBinding(EventArgs e)
        {
            XmlDocument infoXmlDoc = this.GetResponseFromURL(this.SetURL());
            this.SetDataTableColumnsText(infoXmlDoc);
            this.PopulateDataTableFromXML(infoXmlDoc);
            this.DataSource = infoDataTable;
            base.OnDataBinding(e);
        }

        /// <summary>
        /// Sets the url to the docmanager application.
        /// </summary>
        /// <returns>The url</returns>
        private string SetURL()
        {
            string url = this._sourceServer;
            url += "?ReturnType=" + this.ReturnType.ToString();
            url += "&showlogo=" + this.ShowLogo.ToString().ToLower();
            url += "&LinkType=" + this.LinkType.ToString();
            url += "&showsubmitdate=" + this.ShowSubmitDate.ToString().ToLower();
            url += "&extLinkID=" + this._regNumberID;
            url += "&csusername=doc_admin&csuserid=doc_admin";
            return url;
        }

        /// <summary>
        /// Fill the Dt with the information of the XMLdoc
        /// </summary>
        /// <param name="currentXmlDoc">Source of info</param>
        /// <remarks>I didn't (Ulises) find a better way to do this, but I think it must be possible to do this with just one call, like ReadXml...</remarks>
        private void PopulateDataTableFromXML(XmlDocument currentXmlDoc)
        {
            foreach (XmlElement currentNode in currentXmlDoc.ChildNodes)
            {
                DataRow currentDataRow = infoDataTable.NewRow();
                string startLink = "<a href='";
                string middleLink = "'>";
                string endLink = "</a>";
                for (int i = 0; i < currentNode.ChildNodes[0].ChildNodes.Count; i++)
                {
                    if (currentNode.ChildNodes[0].ChildNodes[i].InnerText.Contains("http") || currentNode.ChildNodes[0].ChildNodes[i].InnerText.Contains("../"))
                        currentDataRow[i] = startLink + currentNode.ChildNodes[0].ChildNodes[i].InnerText + middleLink + currentNode.ChildNodes[0].ChildNodes[i].InnerText + endLink;
                    else
                        currentDataRow[i] = currentNode.ChildNodes[0].ChildNodes[i].InnerText;
                }
                infoDataTable.Rows.Add(currentDataRow);
            }
        }

        /// <summary>
        /// Request a URL for an XML response object
        /// </summary>
        /// <param name="url">The server address</param>
        /// <returns>An xmlDoc with the response</returns>
        private XmlDocument GetResponseFromURL(string url)
        {
            string xmlResponseToTest;// = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            xmlResponseToTest = "<externallinks>";
                xmlResponseToTest += "<externallink>";
                    xmlResponseToTest += "<doctitle>Col1</doctitle>";
                    xmlResponseToTest += "<doctitle2>http://www.google.com</doctitle2>";
                    xmlResponseToTest += "<doctitle3>Col1</doctitle3>";
                    xmlResponseToTest += "<doctitle4>Col1</doctitle4>";
                    xmlResponseToTest += "<doctitle5>Col1</doctitle5>";
                xmlResponseToTest += "</externallink>";
            xmlResponseToTest += "</externallinks>";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 10000;     // 10 secs
            //httpWRequest.UserAgent = "Code Sample Web Client";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            Encoding enc = Encoding.GetEncoding(1252);
            StreamReader responseStream = new StreamReader(webResponse.GetResponseStream(), enc);
            string xmlResponse = responseStream.ReadToEnd();
            responseStream.Close();
            responseStream.Close();
            XmlDocument xmldoc = new XmlDocument();
            if(!xmlResponse.Contains("doctitle"))
                xmldoc.Load(new StringReader(xmlResponseToTest));
            else
                xmldoc.Load(new StringReader(xmlResponse));
            return xmldoc;
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///   &lt;CSSClass&gt;COELabelView&lt;/CSSClass&gt;
        ///   &lt;CSSLabelClass&gt;COELabelTitle&lt;/CSSLabelClass&gt;
        ///   &lt;CSSLinkClass&gt;COELabelTitle&lt;/CSSLinkClass&gt;
        ///   &lt;Protocol&gt;http&lt;/Protocol&gt;
        ///   &lt;TargetURL&gt;%2Fdocmanager%2Fdocmanager%2Fexternallinks%2FgetDocumentsNoGUI.asp&lt;/TargetURL&gt;
        ///   &lt;AddDocsURL&gt;%2Fdocmanager%2Fdefault.asp%3Fdataaction%3Ddb%26formgroup%3Dbase_form_group%26dbname%3Ddocmanager%26extAppName%3DChem_Reg%26LinkType%3DCHEMREGREGNUMBER%26linkfieldname%3DROOT_NUMBER%26showselect%3Dtrue%26extlinkid%3D&lt;/AddDocsURL&gt;
        ///   &lt;ReturnType&gt;xml&lt;/ReturnType&gt;
        ///   &lt;LinkType&gt;CHEMREGREGNUMBER&lt;/LinkType&gt;
        ///   &lt;LinkTarget&gt;_blank&lt;/LinkTarget&gt;
        ///   &lt;DeleteItemText&gt;Delete&lt;/DeleteItemText&gt;
        ///   &lt;ShowLogo&gt;false&lt;/ShowLogo&gt;
        ///   &lt;EmptyGridMessage&gt;No documents links currently exists.&lt;/EmptyGridMessage&gt;
        ///   &lt;ShowSubmitDate&gt;true&lt;/ShowSubmitDate&gt;
        ///   &lt;Columns&gt;
        ///   &lt;Column key='Name' width='80px'/&gt;
        ///   &lt;Column key='Link' width='0px'/&gt;
        ///   &lt;Column key='DocName' width='250px'/&gt;
        ///   &lt;Column key='Owner' width='80px'/&gt;
        ///   &lt;Column key='Date' type='Button' width='200px'/&gt;
        ///   &lt;Column key='Delete' width='100px'/&gt;
        ///   &lt;/Columns&gt;
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
             *  <TextMode></TextMode>
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

            XmlNode sourceServer = xmlData.SelectSingleNode("//COE:SourceServer", manager);
            if (sourceServer != null && sourceServer.InnerText.Length > 0)
                this._sourceServer = sourceServer.InnerText;

            XmlNode returnType = xmlData.SelectSingleNode("//COE:ReturnType", manager);
            if (returnType != null && returnType.InnerText.Length > 0)
                this._returnType = returnType.InnerText;

            XmlNode linkType = xmlData.SelectSingleNode("//COE:LinkType", manager);
            if (linkType != null && linkType.InnerText.Length > 0)
                this._linkType = linkType.InnerText;

            XmlNode showLogo = xmlData.SelectSingleNode("//COE:ShowLogo", manager);
            if (showLogo != null && showLogo.InnerText.Length > 0)
                bool.TryParse(showLogo.InnerText, out this._showLogo);

            XmlNode showSubmitDate = xmlData.SelectSingleNode("//COE:ShowSubmitDate", manager);
            if (showSubmitDate != null && showSubmitDate.InnerText.Length > 0)
                bool.TryParse(showSubmitDate.InnerText, out this._showsubmitdate);

            XmlNode columns = xmlData.SelectSingleNode("//COE:Columns", manager);
            // Coverity Fix CID - 13135
            if (columns != null)
            {
                XmlNodeList columnList = columns.SelectNodes("Column");
                if (columnList != null)
                {
                    foreach (XmlNode col in columnList)
                    {
                        _columnsToDisplay.Add(col);
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
            this.DataBind();
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
    }
}

