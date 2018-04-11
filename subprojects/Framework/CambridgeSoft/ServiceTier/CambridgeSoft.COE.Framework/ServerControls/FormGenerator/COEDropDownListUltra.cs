using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.WebUI.WebCombo;
using Infragistics.WebUI.UltraWebGrid;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Properties;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements an Infragistics' WebCombo control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEDropDownListUltra class accepts every Infragistics' WebCombo property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default value of the text edit?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>label: What is its label?</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Width, Height, ID, etc. 
    ///                 This control supports to configure its properties in a generic way (Review the xml sample below - configSetting tag)</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;label&gt;Sample Label&lt;/label&gt;
    ///     &lt;bindingExpression&gt;PropertyList[@Name='SAMPLE' | Value]&lt;/bindingExpression&gt;
    ///     &lt;validationRuleList&gt;
    ///       &lt;validationRule validationRuleName="textLength" errorMessage="The length must be between 0 and 200"&gt;
    ///         &lt;params&gt;
    ///           &lt;param name="min" value="0"/&gt;
    ///           &lt;param name="max" value="200"/&gt;
    ///         &lt;/params&gt;
    ///       &lt;/validationRule&gt;
    ///     &lt;/validationRuleList&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///       &lt;CSSLabelClass&gt;COELabel&lt;/CSSLabelClass&gt;
    ///       &lt;ID&gt;OpticalRotationTextEdit&lt;/ID&gt;
    ///       &lt;configSetting bindingExpression="this.CssClass"&gt;COEMyCustomStyle&lt;/configSetting&gt;
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;top&gt;220px&lt;/top&gt;
    ///       &lt;left&gt;15px&lt;/left&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEDropDownListUltra&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value", GetData and PutData Methods reffer to the Value property.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEDropDownListUltra runat=server></{0}:COEDropDownListUltra>")]
    public class COEDropDownListUltra : WebCombo, ICOEGenerableControl, ICOEHelpContainer
    {
        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        #endregion

        #region Variables
        private string _defaultValue;
        private XmlNodeList _xmlColumnsDef;
        XmlNamespaceManager _manager = null;
        private Image _helpImage = null;
        private CambridgeSoft.COE.Framework.COESearchService.DAL _searchDAL = null;
        private CambridgeSoft.COE.Framework.Common.DALFactory _dalFactory = new DALFactory();
        private string _serviceName = "COESearch";
        #endregion

        #region ICOEGenerableControl Members
        /// <summary>
        /// Gets the selected value as string.
        /// </summary>
        /// <returns>A string with the selected value.</returns>
        public object GetData()
        {
            EnsureDataBound();
            // Coverity Fix CID - 11747,10558 (from local server)
            UltraGridCell selectedCell = this.SelectedCell;
            if (selectedCell != null)
                return selectedCell.Value;
            else
                return null;
        }

        /// <summary>
        /// Sets the selected value.
        /// </summary>
        /// <param name="data">A string with the desired selected value.</param>
        public void PutData(object data)
        {
            //EnsureDataBound();

            //if (data != null && data.ToString() != "")
            //    this.SelectedValue = data.ToString();
            this.DataBind();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            _manager = new XmlNamespaceManager(xmlData.NameTable);
            _manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = xmlData.SelectSingleNode("//COE:Style", _manager);
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

            XmlNode dataTextFieldNode = xmlData.SelectSingleNode("//COE:DataTextField", _manager);
            if (dataTextFieldNode != null && !string.IsNullOrEmpty(dataTextFieldNode.InnerText))
                this.DataTextField = dataTextFieldNode.InnerText;

            XmlNode dataValueFieldNode = xmlData.SelectSingleNode("//COE:DataValueField", _manager);
            if (dataValueFieldNode != null && !string.IsNullOrEmpty(dataValueFieldNode.InnerText))
                this.DataValueField = dataValueFieldNode.InnerText;

            //We read either the DataSourceID or the DataSource (in that order)
            XmlNode dsID = xmlData.SelectSingleNode("//COE:DataSourceID", _manager);
            if (dsID != null && !string.IsNullOrEmpty(dsID.InnerText))
            {
                this.DataSourceID = dsID.InnerText;
            }
            else
            {
                System.Data.DataSet ds = null;
                XmlNode dsValue = xmlData.SelectSingleNode("//COE:DataSource", _manager);
                if (dsValue != null && !string.IsNullOrEmpty(dsValue.InnerText))
                {
                    if (this.IsValidSelectSQL(dsValue.InnerText))
                    {
                        ds = this.RetrieveDataSource(dsValue.InnerText);
                        if (ds.Tables[0].Rows.Count > 0) //No need to databind if the ds is empty
                        {
                            this.DataSource = ds.Tables[0];
                            this.DataBind();
                        }
                    }
                }
            }

            XmlNode width = xmlData.SelectSingleNode("//COE:Width", _manager);
            if (width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = xmlData.SelectSingleNode("//COE:Height", _manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode cssClass = xmlData.SelectSingleNode("//COE:CSSClass", _manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode labelStyle = xmlData.SelectSingleNode("//COE:LabelStyle", _manager);
            if (labelStyle != null && labelStyle.InnerText.Length > 0)
            {
                string[] styles = labelStyle.InnerText.Split(new char[1] { ';' });
                for (int i = 0; i < styles.Length; i++)
                {
                    if (styles[i].Length > 0)
                    {
                        string[] styleDef = styles[i].Split(new char[1] { ':' });
                        string styleId = styleDef[0].Trim();
                        string styleValue = styleDef[1].Trim();
                        //_lit.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode id = xmlData.SelectSingleNode("//COE:ID", _manager);
            if (id != null && id.InnerText.Length > 0)
                this.ID = id.InnerText;

            _xmlColumnsDef = xmlData.SelectNodes("//COE:Columns", _manager);

            ///Generic configuration settings for the control. You can only set public properties (but also those that belong to the control - inherited)
            ///Format (e.g):  <configSetting bindingExpression="this.CSSClass">MyCustomCSSClass</configSetting>
            foreach (XmlNode configSetting in xmlData.SelectNodes("//COE:fieldConfig/COE:configSetting", _manager))
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
        /// Determines whether [is valid select SQL] [the specified SQL].
        /// </summary>
        /// <param name="sql">The SQL statement to check.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid select SQL] [the specified SQL]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>In case the datasource is a select statement - we check for the select word + an space (avoid datasource that could start with select word)</remarks>
        private bool IsValidSelectSQL(string sql)
        {
            return sql.Trim().Substring(0, 6).ToUpper() == "SELECT" && sql.Split(' ').Length > 1;
        }

        /// <summary>
        /// Retrieves the data source.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        private System.Data.DataSet RetrieveDataSource(string sql)
        {
            System.Data.DataSet ds = null;
            try
            {
                if (_searchDAL == null) { LoadDAL(ConnStringType.PROXY); }
                // Coverity Fix CID - 11652
                if (_searchDAL != null)
                    ds = _searchDAL.ExecuteDataSet(sql);
                else
                    throw new System.Security.SecurityException(string.Format(Resources.Culture, Resources.NullObjectError, "DAL"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        /// <summary>
        /// Load the DAL
        /// </summary>
        private void LoadDAL(ConnStringType connStringType)
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            this._dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this._searchDAL, this._serviceName, Resources.SecurityDatabaseName, true);
        }

        /// <summary>
        /// Allows to set the default selected value.
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

        #region Control Events

        protected override void OnInit(EventArgs e)
        {
            this.Init += new EventHandler(COEDropDownListUltra_Init);
            this.ComboTypeAhead = TypeAhead.Extended;
            base.OnInit(e);
        }

        void COEDropDownListUltra_Init(object sender, EventArgs e)
        {
            if (_xmlColumnsDef.Count > 0)
            {
                //If there a re custom columns created, do not auto generate columns, just display the chosen ones.
                if (_xmlColumnsDef[0].SelectNodes("./COE:Column", _manager) != null)
                {
                    this.Columns.Clear();
                    this.DropDownLayout.AutoGenerateColumns = false;
                }
                //It wasn't tested without columns definition, but I would say that it should show all the public properties of the datasource.
                foreach (XmlNode column in _xmlColumnsDef[0].SelectNodes("./COE:Column", _manager))
                {
                    string key = column.Attributes["key"].Value;
                    string title = column.Attributes["title"].Value;
                    string value = string.Empty;
                    string dataType = string.Empty;
                    bool visible = true;
                    if (column.Attributes["visible"] != null)
                        visible = bool.Parse(column.Attributes["visible"].Value);
                    if (column.Attributes["dataType"] != null)
                        dataType = column.Attributes["dataType"].Value;
                    //if (column.Attributes["value"] != null)
                    //    value = column.Attributes["value"].Value;

                    if (!this.Columns.Exists(key))
                    {
                        UltraGridColumn col = new UltraGridColumn(true);
                        col.Key = col.Header.Key = col.Footer.Key = col.BaseColumnName = key;
                        col.Header.Caption = title;
                        col.IsBound = true;
                        col.Hidden = !visible;
                        if (!string.IsNullOrEmpty(dataType))
                            col.DataType = dataType;
                        col.ValueList.DisplayStyle = ValueListDisplayStyle.DisplayText;
                        //col.Tag = value;
                        this.Columns.Add(col);
                    }
                }
            }
        }

        //void COEDropDownListUltra_InitializeRow(object sender, RowEventArgs e)
        //{
        //    COEDataBinder dataBinder = new COEDataBinder(e.Data);
        //    if (dataBinder != null)
        //    {
        //        foreach (UltraGridCell cell in e.Row.Cells)
        //        {
        //            if (!string.IsNullOrEmpty(cell.Key))
        //            {
        //                cell.Value = dataBinder.RetrieveProperty(cell.Key);
        //                cell.Text = dataBinder.RetrieveProperty(this.DataTextField.ToString()) as string;
        //            }
        //        }
        //    }
        //}

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
            if(this.ShowHelp)
            {
                _helpImage = new Image();
                _helpImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(ICOEGenerableControl), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.help.png");
                _helpImage.ID = this.ClientID + "HelpImage";
                _helpImage.Style.Add(HtmlTextWriterStyle.MarginLeft, "2px");
                _helpImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                _helpImage.Attributes.Add("onclick", "showHide(YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip, " + _helpImage.ClientID + ");");
                _helpImage.RenderControl(writer);
                string yahooScript = "<script language='javascript' type='text/javascript'>YAHOO.namespace('COEFormGenerator');" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip = new YAHOO.widget.Tooltip(\"" + _helpImage.ClientID + "Tooltip\", " +
                                     "{ context:\"" + _helpImage.ClientID + "\", text:\"" + FormatText(this.HelpText) + "\"," +
                                     "showdelay: 20000," +
                                     "hidedelay: 20000," +
                                     "visible:false});" +
                                     "YAHOO.COEFormGenerator." + _helpImage.ClientID + "Tooltip.isToBeShown = true;" +
                                     "</script>";
                writer.Write(yahooScript); 
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if(this.ShowHelp)
                this.AddYUIScripts();
        }
        #endregion

        #region ICOEHelpContainer Members
        public bool ShowHelp
        {
            get
            {
                if(ViewState["ShowHelp"] != null)
                    return (bool) ViewState["ShowHelp"];
                else
                    return false;
            }
            set
            {
                ViewState["ShowHelp"] = value;
            }
        }

        public string HelpText
        {
            get
            {
                if(ViewState["HelpText"] != null)
                    return (string) ViewState["HelpText"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["HelpText"] = value;
            }
        }
        #endregion

        #region Private Methods
        private void AddYUIScripts()
        {
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);
            FrameworkUtils.AddYUICSSReference(this.Page, FrameworkConstants.YUI_CSS.CONTAINER);

            string showHideJS = @"
                function showHide(tooltip, contextid)
                {
                    var originalshowdelay = tooltip.cfg.getProperty('showdelay');
                    if(tooltip.isToBeShown)
                    {
                        tooltip.isToBeShown = false;
                        tooltip.cfg.setProperty('showdelay', 0);
                        tooltip.doShow(null, document.getElementById(contextid));
                    }
                    else
                    {
                        tooltip.isToBeShown = true;
                        tooltip.hide();
                    }

                    tooltip.cfg.setProperty('showdelay', originalshowdelay);
                }";
            if(!Page.ClientScript.IsStartupScriptRegistered(typeof(ICOEGenerableControl), "showHideJS"))
            {
                Page.ClientScript.RegisterStartupScript(typeof(ICOEGenerableControl), "showHideJS", showHideJS, true);
            }
        }

        private string FormatText(string txt)
        {
            return "<span style='font-size:9px;'>" + Page.Server.HtmlEncode(txt) + "</span>";
        }
        #endregion
    }

}

