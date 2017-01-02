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
using System.Web;
using CambridgeSoft.COE.Framework.GUIShell;
using System.Web.UI.HtmlControls;
using System.Collections;
[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements an infragistics' UltraWebGrid that gets the doc manager (v9) documents associated to a registration record 
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
    /// In this implementation "Default Value", and PutData Methods reffer to the registry number, and GetData reffers to the documents
    /// associated to that registration number.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEDocManager runat=server></{0}:COEDocManagerClassic")]
    public class COEDocManagerClassic : Infragistics.WebUI.UltraWebGrid.UltraWebGrid, ICOEGenerableControl, ICOERequireable, ICOELabelable
    {
        #region Variables

        private string _defaultValue = string.Empty;
        DataTable infoDataTable = new DataTable("DocManagerLinksTable");
        private string _sourceServer = HttpContext.Current.Request.Url.Authority;
        private string _targetURL = String.Empty;
        private string _regNumberID = String.Empty;
        private string _linkType = String.Empty;
        private string _returnType = String.Empty;
        private string _linkColumn = String.Empty;
        private string _linkTarget = "_blank";
        public string _deleteItemText = "Delete Me";
        private string _protocol = "http";
        private bool _showLogo = false;
        private bool _showsubmitdate = false;
        private string _emptyGridMessage = String.Empty;
        private string _cssLinkClass = String.Empty;
        private string _addDocsURL = String.Empty;
        private List<XmlNode> _columnsToDisplay = new List<XmlNode>();
        private string _addLinkID = "AddDocManagerClassic";
        HtmlAnchor AddLink = new HtmlAnchor();
        private Label _lit = new Label();

        /// <summary>
        /// Types of returned info from server.
        /// </summary>
        private enum ReturnTypes
        {
            xml,
            String,
        }

        /// <summary>
        /// Link Types for DocManagerClassic
        /// </summary>
        private enum LinkTypes
        {
            CHEMREGREGNUMBER,
        }

        /// <summary>
        /// Supported Error Status from Server Response.
        /// </summary>
        private enum ErrorStatus
        {
            ServerNotFound,
            NoDataToDisplay,
            NoPrivileges,
        }

        /// <summary>
        /// Supported privileges for the current user.
        /// </summary>
        private enum Privileges
        {
            Delete,
            Add,
            Browse,
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
        /// Full Server URL (for using inside the code)
        /// </summary>
        private string ServerURL
        {
            get { return this._protocol + "://" + this._sourceServer; }
        }

        /// <summary>
        /// Ticket text to add to some URL for requests
        /// </summary>
        private string Ticket
        {
            get 
            {
                if (Page.Request.Cookies["COESSO"] != null)
                    return "&ticket=" + Page.Request.Cookies["COESSO"].Value;
                else
                    return null;
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

        public COEDocManagerClassic()
        {
            //All of these are cosmetic settings
            this.ID = "DocManagerClassicControl";
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
            this.DisplayLayout.GroupByBox.Hidden = true;
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
        private void SetDataTableColumnsText()
        {
            foreach (XmlNode propDescriptor in _columnsToDisplay)
            {
                infoDataTable.Columns.Add(propDescriptor.Attributes["key"].Value);
            }
        }

        /// <summary>
        /// Here we try to get the info from a source Url.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBinding(EventArgs e)
        {
            XmlDocument infoXmlDoc;
            infoXmlDoc = this.GetResponseFromURL(this.SetURL());
            if (infoXmlDoc != null)
            {
                if (infoXmlDoc.ChildNodes[0].ChildNodes.Count <= 0)
                    this.HandleError(ErrorStatus.NoDataToDisplay);
                else
                {
                    this.SetDataTableColumnsText();
                    this.PopulateDataTableFromXML(infoXmlDoc);
                    this.DataSource = infoDataTable;
                }
            }
            else
                this.HandleError(ErrorStatus.ServerNotFound);

            base.OnDataBinding(e);
        }

        /// <summary>
        /// In case of error, display the correct error message.
        /// </summary>
        /// <param name="currentStatus">Status error to inspect</param>
        private void HandleError(ErrorStatus currentStatus)
        {
            switch (currentStatus)
            {
                //case ErrorStatus.NoDataToDisplay: 
                //        this.DisplayLayout.NoDataMessage = "No Data to Display"; 
                //        break;
                case ErrorStatus.ServerNotFound: 
                    this.DisplayLayout.NoDataMessage = "No server found"; 
                    break;
                case ErrorStatus.NoPrivileges:
                    this.DisplayLayout.NoDataMessage = "No privileges to browse this control";
                    break;
            }
        }

        /// <summary>
        /// Method to form the DocManagerDocument URL
        /// </summary>
        /// <returns> A url to the specific related docs</returns>
        private string SetURL()
        {
            string url = this.ServerURL + this._targetURL;
            url += "?ReturnType=" + this.ReturnType.ToString();
            url += "&showlogo=" + this.ShowLogo.ToString().ToLower();
            url += "&LinkType=" + this.LinkType.ToString();
            url += "&showsubmitdate=" + this.ShowSubmitDate.ToString().ToLower();
            url += "&extLinkID=" + this._regNumberID;
            if(!string.IsNullOrEmpty(this.Ticket))
                url += this.Ticket;
            return url;
        }

        /// <summary>
        /// Fill the Dt with the information of the XMLdoc
        /// </summary>
        /// <param name="currentXmlDoc">Source of info</param>
        /// <remarks>I(Ulises) didn't find a better way to do this, but I think it must be possible to do this with just one call, like ReadXml...</remarks>
        private void PopulateDataTableFromXML(XmlDocument currentXmlDoc)
        {
            foreach (XmlElement currentNode in currentXmlDoc.ChildNodes[0].ChildNodes)
            {
                string startLink = "<a target=\"" + this._linkTarget + "\" href=\"";
                string middleLink = "\">";
                string endLink = "</a>";
                string docLink = string.Empty;
                DataRow currentDataRow = infoDataTable.NewRow();
                for (int i = 0; i < currentNode.ChildNodes.Count; i++)
                {
                    if (currentNode.ChildNodes[i].Name.ToLower() == "doclink")
                        docLink = this.ServerURL + currentNode.ChildNodes[i].InnerText + this.Ticket;
                    else if (currentNode.ChildNodes[i].Name.ToLower() == "docname")
                        currentDataRow[i] = startLink + docLink + middleLink + currentNode.ChildNodes[i].InnerText + endLink;
                    else if (currentNode.ChildNodes[i].Name.ToLower() == "deleteurl")
                    {
                        string url = this.ServerURL + currentNode.ChildNodes[i].InnerText + this.Ticket;
                        string value = this._deleteItemText;
                        currentDataRow[i] = startLink + url + middleLink + value + endLink;
                    }
                    else
                        currentDataRow[i] = currentNode.ChildNodes[i].InnerText;
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
            //string xmlResponseToTest;
            //xmlResponseToTest = "<externallinks>";
            //    xmlResponseToTest += "<externallink>";
            //        xmlResponseToTest += "<doctitle>Col1</doctitle>";
            //        xmlResponseToTest += "<doctitle2>http://www.google.com</doctitle2>";
            //        xmlResponseToTest += "<doctitle3>Col1</doctitle3>";
            //        xmlResponseToTest += "<doctitle4>Col1</doctitle4>";
            //        xmlResponseToTest += "<doctitle5>Col1</doctitle5>";
            //    xmlResponseToTest += "</externallink>";
            //xmlResponseToTest += "</externallinks>";
            XmlDocument xmldoc = new XmlDocument();
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Timeout = 10000;     // 10 secs
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                    Encoding enc = Encoding.GetEncoding(1252);
                    StreamReader responseStream = new StreamReader(webResponse.GetResponseStream(), enc);
                    string xmlResponse = responseStream.ReadToEnd();
                    responseStream.Close();
                    responseStream.Close();
                    xmldoc.Load(new StringReader(xmlResponse));
                }
                catch
                {
                    xmldoc = null;
                }
            }
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

            XmlNode cssLinkClass = xmlData.SelectSingleNode("//COE:CSSLinkClass", manager);
            if (cssLinkClass != null && cssLinkClass.InnerText.Length > 0)
                this._cssLinkClass = cssLinkClass.InnerText;

            XmlNode sourceServer = xmlData.SelectSingleNode("//COE:AppServerName", manager);
            if (sourceServer != null && sourceServer.InnerText.Length > 0)
                this._sourceServer = sourceServer.InnerText;

            XmlNode protocol = xmlData.SelectSingleNode("//COE:Protocol", manager);
            if (protocol != null && protocol.InnerText.Length > 0)
                this._protocol = protocol.InnerText;

            XmlNode addDocsURL = xmlData.SelectSingleNode("//COE:AddDocsURL", manager);
            if (addDocsURL != null && addDocsURL.InnerText.Length > 0)
                this._addDocsURL = HttpUtility.UrlDecode(addDocsURL.InnerText);

            XmlNode targetURL = xmlData.SelectSingleNode("//COE:TargetURL", manager);
            if (targetURL != null && targetURL.InnerText.Length > 0)
                this._targetURL = HttpUtility.UrlDecode(targetURL.InnerText);

            XmlNode returnType = xmlData.SelectSingleNode("//COE:ReturnType", manager);
            if (returnType != null && returnType.InnerText.Length > 0)
                this._returnType = returnType.InnerText;

            XmlNode linkType = xmlData.SelectSingleNode("//COE:LinkType", manager);
            if (linkType != null && linkType.InnerText.Length > 0)
                this._linkType = linkType.InnerText;

            XmlNode linkTarget = xmlData.SelectSingleNode("//COE:LinkTarget", manager);
            if (linkTarget != null && linkTarget.InnerText.Length > 0)
                this._linkTarget = linkTarget.InnerText;

            XmlNode deleteItemText = xmlData.SelectSingleNode("//COE:DeleteItemText", manager);
            if (deleteItemText != null && deleteItemText.InnerText.Length > 0)
                this._deleteItemText = deleteItemText.InnerText;

            XmlNode showLogo = xmlData.SelectSingleNode("//COE:ShowLogo", manager);
            if (showLogo != null && showLogo.InnerText.Length > 0)
                bool.TryParse(showLogo.InnerText, out this._showLogo);

            XmlNode showSubmitDate = xmlData.SelectSingleNode("//COE:ShowSubmitDate", manager);
            if (showSubmitDate != null && showSubmitDate.InnerText.Length > 0)
                bool.TryParse(showSubmitDate.InnerText, out this._showsubmitdate);

            XmlNode columns = xmlData.SelectSingleNode("//COE:Columns", manager);
            //coverity fix
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

            XmlNode emptyGridMessage = xmlData.SelectSingleNode("//COE:EmptyGridMessage", manager);
            if (emptyGridMessage != null && emptyGridMessage.InnerText.Length > 0)
                this.DisplayLayout.NoDataMessage = emptyGridMessage.InnerText;
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
            //if (UserHasPrivileges(Privileges.Browse))
            //{
                this.DataBind();
                this.CosmeticSetUp();
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

                //if (UserHasPrivileges(Privileges.Add))
                //{
                    AddLink.InnerText = "Add Link";
                    AddLink.ID = this._addLinkID;
                    AddLink.HRef = this.ServerURL + this._addDocsURL + this._regNumberID + this.Ticket;

                    AddLink.Target = this._linkTarget;
                    AddLink.Attributes.Add("Class", this._cssLinkClass);
                    AddLink.RenderControl(writer);
                //}
                //else
                //    this.AddLink.Visible = false; //Privileges settings

                //Privileges settings
                //if (!UserHasPrivileges(Privileges.Delete))
                //    this.RemoveColumn("Delete");
            //}
            //else
            //    this.HandleError(ErrorStatus.NoPrivileges);

            base.Render(writer);
        }

        /// <summary>
        /// Cosmetic Setup (Set column properties... width, etc)
        /// </summary>
        private void CosmeticSetUp()
        {
            foreach (XmlNode propDescriptor in _columnsToDisplay)
            {
                int columnWidth = 0;
                string pixelPrefix = "px";
                if (this.Columns.Exists(propDescriptor.Attributes["key"].Value) && !string.IsNullOrEmpty(propDescriptor.Attributes["width"].Value))
                {
                    string rawWidth = propDescriptor.Attributes["width"].Value;
                    //Pixels support for now -> TODO : Percentage.
                    if (rawWidth.Contains(pixelPrefix))
                    {
                        string widthInt = rawWidth.Remove(rawWidth.IndexOf(pixelPrefix,StringComparison.Ordinal),pixelPrefix.Length);
                        int.TryParse(widthInt, out columnWidth);
                        this.Columns.FromKey(propDescriptor.Attributes["key"].Value).Width = Unit.Pixel(columnWidth);
                    }
                }
            }
            this.RemoveColumn("Link");
        }

        /// <summary>
        /// Check in the COEPageControlSettings var if the user has privileges to do the current action.
        /// </summary>
        /// <param name="userPrivileges">Current privilege to check</param>
        /// <returns>A boolean value indicating if the user has the asked privilege</returns>
        //private bool UserHasPrivileges(Privileges userPrivileges)
        //{
        //    bool retVal = true;
        //    string[] columnsToDisable = null;
        //    if (Page.Session["COEPageSettings"] != null)
        //    {
        //        //Read the COEControlSettings Session var
        //        columnsToDisable = ((COEPageControlsSettings)Page.Session["COEPageSettings"]).GetControlsToDisableByPage(this.Page.ToString());
        //        if (columnsToDisable != null)
        //        {
        //            foreach (string currentColumn in columnsToDisable)
        //            {
        //                if (currentColumn.Contains(userPrivileges.ToString()) && currentColumn.Contains("DocManagerClassic"))
        //                {
        //                    retVal = false;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return retVal;
        //}

        /// <summary>
        /// Method to remove a column from the grid Control (if exists)
        /// </summary>
        /// <param name="columnKey">The column key ID</param>
        private void RemoveColumn(string columnKey)
        {
            if (this.Columns.Exists(columnKey))
                this.Columns.Remove(this.Columns.FromKey(columnKey));
        }

        #endregion
    }
}

