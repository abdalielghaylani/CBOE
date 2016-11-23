using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.ComponentModel;
using System.Web.UI.WebControls;
using CambridgeSoft.COE.Framework.Controls.COEWebGrid;
using Infragistics.WebUI.UltraWebGrid;
using System.Data;
using System.Collections;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.Common.Messaging;
using Infragistics.WebUI.Shared;
using System.Collections.Specialized;
using System.Globalization;
using CambridgeSoft.COE.Framework.GUIShell;
using CambridgeSoft.COE.Framework.Common;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ParseChildren(false),
    PersistChildren(true),
    ToolboxData("<{0}:COEWebGridUltra runat=server></{0}:COECOEWebGridUltra>")]
    public class COEWebGridUltra : CompositeControl, ICOEGenerableControl, ICOELabelable, ICOECultureable, ICOEGrid, ICOEReadOnly
    {
        #region Controls

        private UltraWebGrid _grid = new UltraWebGrid("UltraWebGridControl");
        //private DataTable _data = new DataTable("OutputTable");

        private LinkButton _deleteLinkButton = new LinkButton();
        private LinkButton _addLinkButton = new LinkButton();
        private string _removeButtonCSSClass = string.Empty;
        private string _addButtonCSSClass = string.Empty;
        #endregion

        #region Variables

        private string _defaultValue;
        private Label _lit = new Label();
        XmlDocument _xmlData;
        private bool _readOnly = false;
        private bool _viewEnabled = false;
        private XmlNodeList _xmlTablesDef;
        private XmlNodeList _xmlDefaultRows;
        private XmlNodeList _xmlClientSideEventsConfig;
        private string _rowStyleCSS;
        XmlNamespaceManager _manager = null;
        private string _eventNameFormat = "{0}_{1}";
        private string _deleteColID = String.Empty;
        private string _jsFunctionFormat = "function {0}(gridName, cellId) {{ {1} }}";
        //CSBR: 125954.  _jsRowFunctionFormat will take GridName and rowId as parameters
        private string _jsRowFunctionFormat = "function {0}(gridName, rowId) {{ {1} }}";
        private string _jsCellFunctionFormat = "function {0}(gridName, cellId, button) {{ {1} }}";
        private string _jsFunctionFormatSimple = "function {0}() {{ {1} }}";
        private int _columnsCount = 0;

        private Hashtable _jsEvents = new Hashtable();
        private CultureInfo _culture;
        private int _emptyDefaultRows = 0;
        private Behavior _behavior = Behavior.NotSet;
        private readonly string ENDBINDINGEXP = "_EndsBindingExp_|";
        private readonly string STARTSBINDINGEXP = "|_StartBindingExp_";
        private CambridgeSoft.COE.Framework.COESearchService.DAL _searchDAL = null;
        private CambridgeSoft.COE.Framework.Common.DALFactory _dalFactory = new DALFactory();
        private bool _enableClientSideNumbering = true;
        private bool _addNewRowVisibility = true;
        private bool _headerWrap = false;
        private int _requiredRowsNumber;
        private string[] _requiredColumnIndex;
        private string _requiredRowsNumberMessage;
        private RowSelectors _rowSelectorsDefault = RowSelectors.No;
        private COEEditControl _editControl = COEEditControl.NotSet;
        private string _serviceName = "COESearch";
        //private string _databaseName = "COEDB";
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        /// <summary>
        /// Config keys
        /// </summary>
        private enum ConfigSetting
        {
            AddButtonText,
            ReadOnly,
            ReadOnlyMode,
        }

        private enum ClientSideEvents
        {
            BeforeEnterEdit,
            AfterExitEdit,
            OnFormSubmit,
            CustomValidation,
            CellClickHandler,
            AfterRowInsert,
        }

        private enum JS
        {
            CustomJS_FilterByUnique,
            CustomJS_Remove,
            CustomJS_ValidateRows,
            CustomJS_RequiredRow,
            CustomJS_CustomRowsValidation,
            //CSBR:125954
            CustomJS_SetDefaultValueForCOEDropDownListUltra
        }

        private enum SupportedColTypes
        {
            STRING,
            DATETIME,
        }

        #endregion

        #region Properties

        /// <summary>
        /// UltraWebGrid control
        /// </summary>
        public UltraWebGrid Grid
        {
            get
            {
                EnsureChildControls();
                return _grid;
            }
            set { _grid = value; }
        }

        /// <summary>
        /// Status of the Grid
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
            }
        }

        public bool ViewEnabled
        {
            get
            {
                return _viewEnabled;
            }
            set
            {
                _viewEnabled = value;
            }
        }
        private bool _allowDelete;
        public bool AllowDelete
        {
            get
            {
                return _allowDelete;
            }
            set
            {
                if (value)
                {
                    _grid.DisplayLayout.AllowDeleteDefault = Infragistics.WebUI.UltraWebGrid.AllowDelete.Yes;
                    _grid.DisplayLayout.SelectTypeRowDefault = SelectType.Extended;
                    this._deleteLinkButton.Visible = true;
                }
                else
                {
                    _grid.DisplayLayout.AllowDeleteDefault = Infragistics.WebUI.UltraWebGrid.AllowDelete.No;
                    _grid.DisplayLayout.SelectTypeRowDefault = SelectType.None;
                    this._deleteLinkButton.Visible = false;
                }
                _allowDelete = value;
            }
        }

        /// <summary>
        /// Default behaviors for a better management of the table. (privileges, etc)
        /// </summary>
        private enum Behavior
        {
            DocMgr,
            NotSet,
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

        private string DeleteRowScript
        {
            get
            {
                return @"igtbl_deleteSelRows('" + _grid.ClientID + @"'); return false;";
            }
        }

        private string AddRowScript
        {
            get
            {
                return @"igtbl_addNew('" + _grid.ClientID + @"', 0, true, true); return false;";
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public COEWebGridUltra()
        {
            _grid.DisplayLayout.AutoGenerateColumns = false;
            _grid.DisplayLayout.AllowAddNewDefault = AllowAddNew.Yes;
            _grid.DisplayLayout.AllowUpdateDefault = AllowUpdate.Yes;
            _grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            _grid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;
            _grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;

            _deleteLinkButton.ID = "DeleteLinkButton";
            _addLinkButton.ID = "AddLinkButton";
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
           return this.UnBind();
        }

        public void PutData(object data)
        {
            if (data is IList || data is DataTable)
            {
                int counter = data is IList ? ((IList)data).Count : ((DataTable)data).Rows.Count;
                //We don't want to bind to an empty list, but we have to do it in Edit/Add mode.
                if (counter > 0 || !this.ReadOnly)
                {
                    _grid.DataSource = data;

                    if (_xmlTablesDef.Count > 0 && _xmlTablesDef[0].Attributes["name"] != null && !string.IsNullOrEmpty(_xmlTablesDef[0].Attributes["name"].Value))
                        _grid.DataMember = _xmlTablesDef[0].Attributes["name"].Value;

                    _grid.DataBind();
                }
            }
            else if (data is string)
            {
                if (string.IsNullOrEmpty((string)data))
                    _grid.DataBind();//Just Bind saying no data to display
                else
                    throw new Exception("COEWebGridUltra doesn't know howto bind to a nonempty string");
            }
            else
                throw new Exception("COEWebGridUltra doesn't support the given datasource type");
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get Columns information according config xml settings
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="columnName"></param>
        /// <param name="columnBindingExpression"></param>
        private void GetColumnInfo(XmlNode xmlNode, out string columnName, out string columnBindingExpression)
        {
            columnName = string.Empty;
            columnBindingExpression = string.Empty;

            if (xmlNode !=null && xmlNode.Attributes["name"] != null && !string.IsNullOrEmpty(xmlNode.Attributes["name"].Value))
                columnName = xmlNode.Attributes["name"].Value;

            XmlNode managerNode = xmlNode.SelectSingleNode("./COE:formElement", _manager); //Coverity Fix CID 13143 ASV
            if (managerNode != null && !string.IsNullOrEmpty(managerNode.OuterXml) )
            {
                FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(managerNode.OuterXml);
                if (formElement != null)
                {
                    if (!string.IsNullOrEmpty(formElement.Name)) //Name refers to a datatable column, so modify the binding expression.
                        columnBindingExpression = string.Format("this['{0}']", formElement.Name);
                    else if (!string.IsNullOrEmpty(formElement.BindingExpression))
                        columnBindingExpression = formElement.BindingExpression;
                }
                else
                {
                    if (!string.IsNullOrEmpty(columnName))
                        columnBindingExpression = columnName;
                }
            }
            if (string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(columnBindingExpression))
                columnName = columnBindingExpression;
            else if (string.IsNullOrEmpty(columnBindingExpression))
                columnBindingExpression = columnName;
        }

        private string GetColumnName(XmlNode xmlNode)
        {
            string retVal = string.Empty;
            if (xmlNode != null && xmlNode.Attributes["name"] != null && !string.IsNullOrEmpty(xmlNode.Attributes["name"].Value)) //Coverity Fix CID 13143 ASV
                retVal = xmlNode.Attributes["name"].Value;
            return retVal;
        }

        /// <summary>
        /// Returns a DataTable containing the entered info into the grid.
        /// </summary>
        /// <returns>DataTable with the entered info</returns>
        private object UnBind()
        {
            
            DataTable result = new DataTable();
            //Coverity Fix CID 13139 ASV
            XmlNodeList managerNodes = null;
            if (_xmlTablesDef != null && _xmlTablesDef.Count > 0) 
                managerNodes = _xmlTablesDef[0].SelectNodes("./COE:Columns/COE:Column", _manager);

            if (managerNodes != null)
            {
                foreach (XmlNode column in managerNodes)
                {
                    bool unbind = true;
                    string columnName = string.Empty, columnBindingExpression = string.Empty;
                    GetColumnInfo(column, out columnName, out columnBindingExpression);
                    if (column.Attributes["unbind"] != null)
                        unbind = bool.Parse(column.Attributes["unbind"].Value);
                    if (!result.Columns.Contains(columnBindingExpression) && unbind)
                        result.Columns.Add(columnBindingExpression);
                }
            }
            foreach (UltraGridRow currentRow in _grid.Rows)
            {
                DataRow currentDataRow = result.NewRow();
                bool validRow = false;

                foreach (UltraGridCell currentCell in currentRow.Cells)
                {
                    if (currentCell.Value != null)
                    {
                        if (!(currentCell.Value is DBNull))
                        {
                            //Now check if the cell has an inner editorcontrolid, so we need to unbind also the DataText field.
                            if (currentCell.Column.Tag is ColSettings && currentCell.Column.ValueList.ValueListItems.Count > 0)
                            {
                                //Check if the cell value is really the Key
                                string txt = this.GetTextByValueKey(currentCell.Value, currentCell.Column.ValueList.ValueListItems);
                                string key = string.Empty;
                                if (string.IsNullOrEmpty(txt)) //Something is wrong, so lets try getting the key given the "text"
                                    key = this.GetKeyByValueKey(currentCell.Value, currentCell.Column.ValueList.ValueListItems);
                                currentDataRow[currentCell.Column.Key] = string.IsNullOrEmpty(txt) ? key : currentCell.Value;
                                if (currentDataRow.Table.Columns[((ColSettings)currentCell.Column.Tag).ValueField] != null)
                                    currentDataRow[((ColSettings)currentCell.Column.Tag).ValueField] = this.GetTextByValueKey(string.IsNullOrEmpty(txt) ? key : currentCell.Value, currentCell.Column.ValueList.ValueListItems);
                            }
                            else
                            {
                                if (currentDataRow.Table.Columns.Contains(currentCell.Column.Key))
                                    currentDataRow[currentCell.Column.Key] = currentCell.Value;
                            }
                        }
                    }
                }

                if (IsAValidRow(currentDataRow))
                {
                    result.Rows.Add(currentDataRow);

                    string rowState = currentRow.Cells.FromKey("RowState") != null && currentRow.Cells.FromKey("RowState").Value != null ? currentRow.Cells.FromKey("RowState").Value.ToString() : DataChanged.Unchanged.ToString();

                    switch ((DataChanged)Enum.Parse(typeof(DataChanged), rowState))
                    {
                        case DataChanged.Deleted:
                            currentDataRow.AcceptChanges();
                            currentDataRow.Delete();
                            break;
                        case DataChanged.Added:
                            currentDataRow.AcceptChanges();
                            currentDataRow.SetAdded();
                            if (currentRow.Cells.FromKey("RowState").Value != null)
                                currentRow.Cells.FromKey("RowState").Value = null; // Set to null value because RowState column always holds the action performed in GUI [Row Added, Row Deleted, RowChanged]. 
                            break;
                        case DataChanged.Modified:
                            currentDataRow.AcceptChanges();
                            currentDataRow.SetModified();
                            break;
                        //DataChanged.Unchanged
                        default:
                            currentDataRow.AcceptChanges();
                            break;
                    }
                    //result.Rows.Remove(currentDataRow);
                }
            }
            return (object)result;
        }

        /// <summary>
        /// Validates that there is no null cells in the row, so we avoid returning empty/invalid data rows.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsAValidRow(DataRow row)
        {
            bool retVal = true;
            foreach (object obj in row.ItemArray)
            {
                if (obj is System.DBNull)
                {
                    retVal = false;
                    break;
                }
            }
            return retVal;
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            _xmlData = new XmlDocument();
            _xmlData.LoadXml(xmlDataAsString);

            _manager = new XmlNamespaceManager(_xmlData.NameTable);
            _manager.AddNamespace("COE", _xmlData.DocumentElement.NamespaceURI);

            //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
            XmlNode style = _xmlData.SelectSingleNode("//COE:Style", _manager);
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

            XmlNode width = _xmlData.SelectSingleNode("//COE:Width", _manager);
            if (width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode height = _xmlData.SelectSingleNode("//COE:Height", _manager);
            if (height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode cssClass = _xmlData.SelectSingleNode("//COE:CSSClass", _manager);
            if (cssClass != null && cssClass.InnerText.Length > 0)
                this.CssClass = cssClass.InnerText;

            XmlNode cssLabelClass = _xmlData.SelectSingleNode("//COE:CSSLabelClass", _manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

            XmlNode labelStyle = _xmlData.SelectSingleNode("//COE:LabelStyle", _manager);
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
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }

            //XmlNode id = _xmlData.SelectSingleNode("//COE:Id", _manager);
            //if (id != null && id.InnerText.Length > 0)
            //    this.ID = id.InnerText;

            XmlNode addRowTitle = _xmlData.SelectSingleNode("//COE:AddRowTitle", _manager);
            if (addRowTitle != null && addRowTitle.InnerText.Length > 0)
                _addLinkButton.Text = addRowTitle.InnerText;

            XmlNode removeRowTitle = _xmlData.SelectSingleNode("//COE:RemoveRowTitle", _manager);
            if (removeRowTitle != null && removeRowTitle.InnerText.Length > 0)
                _deleteLinkButton.Text = removeRowTitle.InnerText;

            XmlNode readOnly = _xmlData.SelectSingleNode("//COE:ReadOnly", _manager);
            if (readOnly != null && readOnly.InnerText.Length > 0)
                bool.TryParse(readOnly.InnerText, out _readOnly);

            XmlNode viewEnabled = _xmlData.SelectSingleNode("//COE:ViewEnabled", _manager);
            if (viewEnabled != null && viewEnabled.InnerText.Length > 0)
                bool.TryParse(viewEnabled.InnerText, out _viewEnabled);

            _xmlTablesDef = _xmlData.SelectNodes("//COE:table", _manager);
            _xmlDefaultRows = _xmlData.SelectNodes("//COE:DefaultRows", _manager);

            XmlNode clientSideEvents = _xmlData.SelectSingleNode("//COE:ClientSideEvents", _manager);
            if (clientSideEvents != null)
            {
                _xmlClientSideEventsConfig = clientSideEvents.ChildNodes;

                //perform defensive check for node type in case of XML comments
                foreach (XmlNode node in clientSideEvents.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        XmlElement el = (XmlElement)node;
                        if (Enum.IsDefined(typeof(ClientSideEvents), el.Attributes["name"].Value))
                            _jsEvents.Add(Enum.Parse(typeof(ClientSideEvents), el.Attributes["name"].Value), el.InnerText);
                    }
                }
            }

            //Number of rows to display empty in the grid.
            XmlNode defaultEmptyRows = _xmlData.SelectSingleNode("//COE:DefaultEmptyRows", _manager);
            if (defaultEmptyRows != null && defaultEmptyRows.InnerText.Length > 0)
                _emptyDefaultRows = Convert.ToInt32(defaultEmptyRows.InnerText);

            XmlNode noDataMessage = _xmlData.SelectSingleNode("//COE:NoDataMessage", _manager);
            if (noDataMessage != null && noDataMessage.InnerText.Length > 0)
                _grid.DisplayLayout.NoDataMessage = noDataMessage.InnerText;
            else
                _grid.DisplayLayout.NoDataMessage = string.Empty;

            XmlNode headerStyleCSS = _xmlData.SelectSingleNode("//COE:HeaderStyleCSS", _manager);
            if (headerStyleCSS != null && headerStyleCSS.InnerText.Length > 0)
                _grid.Bands[0].HeaderStyle.CssClass = headerStyleCSS.InnerText;

            XmlNode headerHAlign = _xmlData.SelectSingleNode("//COE:HeaderHorizontalAlign", _manager);
            if (headerHAlign != null && headerHAlign.InnerText.Length > 0)
                if (Enum.IsDefined(typeof(HorizontalAlign), headerHAlign.InnerText))
                    _grid.DisplayLayout.HeaderStyleDefault.HorizontalAlign = (HorizontalAlign)Enum.Parse(typeof(HorizontalAlign), headerHAlign.InnerText);

            XmlNode addButtonStyleCSS = _xmlData.SelectSingleNode("//COE:AddButtonCSS", _manager);
            if (addButtonStyleCSS != null && addButtonStyleCSS.InnerText.Length > 0)
               _addButtonCSSClass = addButtonStyleCSS.InnerText;

            XmlNode removeButtonStyleCSS = _xmlData.SelectSingleNode("//COE:RemoveButtonCSS", _manager);
            if (removeButtonStyleCSS != null && removeButtonStyleCSS.InnerText.Length > 0)
                _removeButtonCSSClass = removeButtonStyleCSS.InnerText;

            XmlNode rowAltStyleCSS = _xmlData.SelectSingleNode("//COE:RowAlternateStyleCSS", _manager);
            if (rowAltStyleCSS != null && rowAltStyleCSS.InnerText.Length > 0)
                _grid.DisplayLayout.RowAlternateStyleDefault.CssClass = rowAltStyleCSS.InnerText;

            XmlNode rowStyleCSS = _xmlData.SelectSingleNode("//COE:RowStyleCSS", _manager);
            if (rowStyleCSS != null && rowStyleCSS.InnerText.Length > 0)
            {
                _rowStyleCSS = rowStyleCSS.InnerText;
                _grid.DisplayLayout.RowStyleDefault.CssClass = rowStyleCSS.InnerText;
            }

            rowStyleCSS = _xmlData.SelectSingleNode("//COE:AllowDeletingRows", _manager);
            if (rowStyleCSS != null && rowStyleCSS.InnerText.Length > 0)
                this.AllowDelete = bool.Parse(rowStyleCSS.InnerText);
            else
                this.AllowDelete = true;

            XmlNode _defBehavior = _xmlData.SelectSingleNode("//COE:DefaultBehavior", _manager);
            if (_defBehavior != null && _defBehavior.InnerText.Length > 0)
                if (Enum.IsDefined(typeof(Behavior), _defBehavior.InnerText))
                    _behavior = (Behavior)Enum.Parse(typeof(Behavior), _defBehavior.InnerText);

            XmlNode selectedRowAltStyleCSS = _xmlData.SelectSingleNode("//COE:SelectedRowStyleCSS", _manager);
            if (selectedRowAltStyleCSS != null && selectedRowAltStyleCSS.InnerText.Length > 0)
                _grid.DisplayLayout.SelectedRowStyleDefault.CssClass = selectedRowAltStyleCSS.InnerText;

            XmlNode enableClientSideNumbering = _xmlData.SelectSingleNode("//COE:EnableClientSideNumbering", _manager);
            if (enableClientSideNumbering != null && enableClientSideNumbering.InnerText.Length > 0)
                if (enableClientSideNumbering.InnerText.ToUpper() == bool.FalseString.ToUpper())
                    _enableClientSideNumbering = false;

            XmlNode addNewRowVisibility = _xmlData.SelectSingleNode("//COE:AddNewRowVisibility", _manager);
            if (addNewRowVisibility != null && addNewRowVisibility.InnerText.Length > 0)
                if (addNewRowVisibility.InnerText.ToUpper() == bool.FalseString.ToUpper())
                    _addNewRowVisibility = false;

            XmlNode headerWrap = _xmlData.SelectSingleNode("//COE:HeaderWrap", _manager);
            if (headerWrap != null && headerWrap.InnerText.Length > 0)
                if (headerWrap.InnerText.ToUpper() == bool.TrueString.ToUpper())
                    _headerWrap = true;

            /*
            XmlNode dropdownItemsSelect = _xmlData.SelectSingleNode("//COE:DropDownItemsSelect", _manager);
            if (dropdownItemsSelect != null && dropdownItemsSelect.InnerText.Length > 0)
            {
                if (dropdownItemsSelect.InnerText.Trim().Substring(0, 6).ToUpper() != "SELECT")
                {
                    throw new Exception("An invalid select statement was specified for a drop down box");
                }
                if (_searchDAL == null) { LoadDAL(ConnStringType.PROXY); }
                System.Data.DataSet ds = _searchDAL.ExecuteDataSet(dropdownItemsSelect.InnerText.Trim());

            }
             * */

            XmlNode _rowSelectorDefRow = _xmlData.SelectSingleNode("//COE:RowSelectorsDefault", _manager);
            if (_rowSelectorDefRow != null && _rowSelectorDefRow.InnerText.Length > 0)
                if (Enum.IsDefined(typeof(RowSelectors), _rowSelectorDefRow.InnerText))
                    _grid.DisplayLayout.RowSelectorsDefault = (RowSelectors)Enum.Parse(typeof(RowSelectors), _rowSelectorDefRow.InnerText);

            XmlNode _rowSelectorDefCssRow = _xmlData.SelectSingleNode("//COE:RowSelectorsCssClass", _manager);
            if (_rowSelectorDefCssRow != null && _rowSelectorDefCssRow.InnerText.Length > 0)
                    _grid.DisplayLayout.RowSelectorStyleDefault.CssClass = _rowSelectorDefCssRow.InnerText;
 
            XmlNode requiredRowsNumberNode = _xmlData.SelectSingleNode("//COE:RequiredRowsNumber", _manager);
            if (requiredRowsNumberNode != null && requiredRowsNumberNode.InnerText.Length > 0)
                _requiredRowsNumber = int.Parse(requiredRowsNumberNode.InnerText);

            XmlNode requiredRowsNumberMessageNode = _xmlData.SelectSingleNode("//COE:RequiredRowsNumberMessage", _manager);
            if (requiredRowsNumberMessageNode != null && requiredRowsNumberMessageNode.InnerText.Length > 0)
                _requiredRowsNumberMessage = requiredRowsNumberMessageNode.InnerText;

            XmlNode requiredColumnIndexNode = _xmlData.SelectSingleNode("//COE:RequiredColumnIndex", _manager);
            if (requiredColumnIndexNode != null && requiredColumnIndexNode.InnerText.Length > 0)
                _requiredColumnIndex = requiredColumnIndexNode.InnerText.Split('|');

            ///Generic configuration settings for the control. You can only set public properties (but also those that belong to the control - inherited)
            ///Format (e.g):  <configSetting bindingExpression="this.CSSClass">MyCustomCSSClass</configSetting>
            XmlNodeList managerNodes = _xmlData.SelectNodes("/COE:configInfo/COE:fieldConfig/COE:configSetting", _manager); //Coverity Fix CID 13139 ASV
            if (managerNodes !=null && managerNodes.Count>0)
                foreach (XmlNode configSetting in managerNodes)
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
        /// Load the DAL
        /// </summary>
        private void LoadDAL(ConnStringType connStringType)
        {
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            this._dalFactory.GetDAL<CambridgeSoft.COE.Framework.COESearchService.DAL>(ref this._searchDAL, this._serviceName, Resources.SecurityDatabaseName, true);
        }

        public string DefaultValue
        {
            get
            {
                return "";
            }
            set
            {
                ;
            }
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Create all control.
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                this.Controls.Clear();
                this.CreateControlHierarchy();
                //this.ChildControlsCreated = true;
                if (!this.ReadOnly)
                {
                    HtmlTable htmlTable = new HtmlTable();
                    
                    htmlTable.Rows.Add(new HtmlTableRow());
                    htmlTable.Style.Add(HtmlTextWriterStyle.MarginTop, "5px");
                    htmlTable.Rows[0].VAlign = "top";
                    htmlTable.Rows[0].Cells.Add(new HtmlTableCell());
                    htmlTable.Rows[0].Cells[0].Style.Value = "padding-right: 9px;";
                    htmlTable.Rows[0].Cells[0].VAlign = "middle";

                    htmlTable.Rows[0].Cells.Add(new HtmlTableCell());
                    htmlTable.Rows[0].Cells[1].Attributes["class"] = this._addButtonCSSClass;
                    htmlTable.Rows[0].Cells[1].Attributes["onClick"] = this.AddRowScript;
                    _addLinkButton.CssClass = _addButtonCSSClass;
                    _addLinkButton.Style.Value = "background-image:none;margin-left:0px;padding-left:0px;";
                    htmlTable.Rows[0].Cells[1].Controls.Add(_addLinkButton);
                    htmlTable.Rows[0].Visible = _addNewRowVisibility;

                    htmlTable.Rows.Add(new HtmlTableRow());
                    htmlTable.Rows[1].VAlign = "top";
                    htmlTable.Rows[1].Cells.Add(new HtmlTableCell());
                    htmlTable.Rows[1].Cells[0].Style.Value = "padding-right: 9px;";
                    htmlTable.Rows[1].Cells[0].VAlign = "middle";

                    htmlTable.Rows[1].Cells.Add(new HtmlTableCell());
                    htmlTable.Rows[1].Cells[1].Attributes["class"] = this._removeButtonCSSClass;
                    htmlTable.Rows[1].Cells[1].Attributes["onClick"] = this.DeleteRowScript;
                    _deleteLinkButton.CssClass = this._removeButtonCSSClass;
                    _deleteLinkButton.Style.Value = "background-image:none;margin-left:0px;padding-left:0px;";
                    htmlTable.Rows[1].Cells[1].Controls.Add(_deleteLinkButton);
                    this.Controls.Add(htmlTable);

                }

                base.CreateChildControls();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            _grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            _grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
        
            //if(this.ReadOnly)
                this._grid.PreRender += new EventHandler(Grid_PreRender);
            base.OnInit(e);
        }

        void Grid_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i].ID != null && this.Controls[i].ID != this._grid.ID)
                    this.Controls[i].ID = this.Controls[i].Parent.ID + this.Controls[i].ID;
            }
            //Also need to change the EditorControlID references.
            //Remarks: Could cause issues with custom js written inside the inner controls, but haven't found such errors so far (just guessing).
            
            
                for (int j = 0; j < _grid.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(_grid.Columns[j].EditorControlID))
                        _grid.Columns[j].EditorControlID = this.ID + _grid.Columns[j].EditorControlID; //this.ID = his.Controls[i].Parent.ID (above)
                }
            
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            //Bind each cell to its datasource.
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Column.IsBound)
                {
                    COEDataBinder dataBinder = new COEDataBinder(e.Data);
                    if (dataBinder != null)
                    {
                        string key = cell.Key;
                        cell.Value = cell.Tag = dataBinder.RetrieveProperty(key);
                        if (cell.Column.Tag != null)
                        {
                            if (cell.Column.Tag is ColSettings)
                            {
                                string tempText = cell.Text;
                                cell.Column.DataType = "System.String"; //Forced since when there is a editorcontrol embeded with type int, it could cause issues of missmatch datatypes. 
                                //if (!string.IsNullOrEmpty(((ColSettings)cell.Column.Tag).ValueField))
                                //    cell.Text = (string)dataBinder.RetrieveProperty(((ColSettings)cell.Column.Tag).ValueField);
                                
                                //verify column type and default text is -1 or 0
                                if ((cell.Column.Type == ColumnType.Custom || cell.Column.Type == ColumnType.DropDownList)  && (cell.Text.Equals("-1") || cell.Text.Equals("0")))
                                {
                                    bool IsValidListItem = false;
                                    //If the column valueList contains datasource then the column should be the list type like dropdown.
                                    if (cell.Column.ValueList.DataSource != null || !string.IsNullOrEmpty(cell.Column.ValueList.DataSourceID))
                                    {
                                        if (cell.Column.ValueList.DataSource != null)
                                        {
                                            DataTable dt = (DataTable)(cell.Column.ValueList.DataSource);
                                            foreach(DataRow dr in dt.Rows)
                                            {
                                                //get the valuelist value
                                                string str = dr[cell.Column.ValueList.DisplayMember].ToString();
                                                if (str.Equals("-1") || str.Equals("0"))
                                                {
                                                    //If the valueList having -1 or 0 then no change
                                                    IsValidListItem = true;
                                                    break;
                                                }
                                            }
                                        }
                                        //If the valueList not having -1 or 0 then change the default text as empty.
                                        if (!IsValidListItem)
                                            cell.Text = string.Empty;
                                    }
                                    
                                }
                                if (!string.IsNullOrEmpty(((ColSettings)cell.Column.Tag).FormatExp))
                                {
                                    switch (((ColSettings)cell.Column.Tag).ColType)
                                    {
                                        case SupportedColTypes.DATETIME:
                                            DateTime tempDate = new DateTime();
                                            tempDate = DateTime.MinValue;
                                            if (DateTime.TryParse(tempText, out tempDate))
                                                cell.Text = string.Format(_culture, ((ColSettings)cell.Column.Tag).FormatExp, tempDate);
                                            break;
                                        default:
                                            string preFormat = this.ReplaceReservedWords(((ColSettings)cell.Column.Tag).FormatExp, dataBinder);
                                            cell.Text = string.Format(preFormat, tempText);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            e.Row.Style.CssClass = _rowStyleCSS;
        }

        /// <summary>
        /// Replaces a reserved word for one inside the datasource.
        /// </summary>
        /// <param name="text">tect containing reserved words</param>
        /// <param name="dataBinder">databinder</param>
        /// <returns>replaced string</returns>
        private string ReplaceReservedWords(string text, COEDataBinder dataBinder)
        {
            string retVal = text;
            string ticketKey = "|_ticket_|";
            string ticketValue = this.Page.Request.Cookies["COESSO"] != null ? this.Page.Request.Cookies["COESSO"].Value : string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.Contains(ticketKey) && !string.IsNullOrEmpty(ticketValue))
                    retVal = retVal.Replace(ticketKey, ticketValue);
            }

            //Now the binding expressions. It just supports one extra binding expression, but it could be improved
            if (this.ContainsBindingExpression(text) && dataBinder != null)
            {
                int counter = this.BindingExpressionCount(text);
                for (int i = 0; i < counter; i++)
                {
                    string bindExp = text.Substring(text.IndexOf(STARTSBINDINGEXP) + STARTSBINDINGEXP.Length, text.IndexOf(ENDBINDINGEXP) - (text.IndexOf(STARTSBINDINGEXP) + STARTSBINDINGEXP.Length));
                    object tempObj = ReplaceBindingExpression(dataBinder, bindExp);
                    if (tempObj is string || tempObj is int)
                    {
                        text = retVal = retVal.Replace(STARTSBINDINGEXP + bindExp + ENDBINDINGEXP, tempObj.ToString());
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Replaces a text for a given binding expression.
        /// </summary>
        /// <param name="dataBinder">databinder object</param>
        /// <param name="key">Text to be replaced</param>
        /// <returns>Clean string with replaced keys</returns>
        private object ReplaceBindingExpression(COEDataBinder dataBinder, string key)
        {
            object retVal = null;
            if (!string.IsNullOrEmpty(key))
                retVal = dataBinder.RetrieveProperty(key);
            return retVal;
        }

        /// <summary>
        /// Checks for binding expressions start and end.
        /// </summary>
        /// <param name="key">key string</param>
        /// <returns>boolean indicating the result</returns>
        private bool ContainsBindingExpression(string key)
        {
            return key.Contains(STARTSBINDINGEXP) && key.Contains(ENDBINDINGEXP);
        }

        /// <summary>
        /// Bindings the expression count.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private int BindingExpressionCount(string key)
        {
            return System.Text.RegularExpressions.Regex.Matches(key, STARTSBINDINGEXP.Substring(1)).Count;
        }

        private void InitializeLayout()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
            _columnsCount = 0;

            _grid.DisplayLayout.AutoGenerateColumns = false;

            if (string.IsNullOrEmpty(_addLinkButton.Text))
                _addLinkButton.Text = "Add"; //Default text then overwritten by configuration
            _grid.Bands[0].Key = "Add";

            _grid.DisplayLayout.CellClickActionDefault = CellClickAction.Edit;

            _grid.DisplayLayout.AllowAddNewDefault = AllowAddNew.Yes;
            _grid.DisplayLayout.AllowUpdateDefault = AllowUpdate.Yes;
            _grid.DisplayLayout.EditCellStyleDefault.BorderColor = System.Drawing.Color.Gray;
            _grid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;
            _grid.DisplayLayout.RowHeightDefault = new Unit("22px");
            _grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.OnServer;
            _grid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.SortMulti;
            _grid.DisplayLayout.BorderCollapseDefault = BorderCollapse.Collapse;
            _grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            _grid.DisplayLayout.AllowRowNumberingDefault = RowNumbering.ByBandLevel;
            _grid.DisplayLayout.EnableClientSideRenumbering = true;
            _grid.DisplayLayout.Name = _grid.ID;

            #region Create default Columns

            if (_grid.DataSource != null || !this.ReadOnly)
            {
                //Create Columns.
                XmlNodeList managerNodes = null;
                if (_xmlTablesDef != null && _xmlTablesDef.Count > 0 && _xmlTablesDef[0] != null) //Coverity Fix CID 13143 ASV
                    managerNodes = _xmlTablesDef[0].SelectNodes("./COE:Columns/COE:Column", _manager);
                if (managerNodes != null)
                {
                    foreach (XmlNode column in managerNodes)
                    {
                        string columnName, columnBindingExpression, errorMsg;
                        string dataTextValueField = string.Empty;
                        string formatText = string.Empty;
                        bool visible = true;
                        string headerText = string.Empty;
                        ColumnType colType = ColumnType.NotSet;
                        string colCastType = string.Empty;
                        string dataSourceID = string.Empty;
                        string dataType = string.Empty;
                        string defaultValue = string.Empty;
                        string width = string.Empty;
                        string cellClass = string.Empty;
                        AllowUpdate allowUpdate = AllowUpdate.Yes;

                        //Used when we show something as a text, but the value to unbind behind is other.
                        if (column.Attributes["dataTextValueField"] != null)
                            dataTextValueField = column.Attributes["dataTextValueField"].Value;

                        if (column.Attributes["visible"] != null)
                            visible = bool.Parse(column.Attributes["visible"].Value);

                        if (column.Attributes["headerText"] != null)
                            headerText = column.Attributes["headerText"].Value;

                        if (column.Attributes["colCastType"] != null)
                            colCastType = column.Attributes["colCastType"].Value;

                        if (column.Attributes["defaultValue"] != null)
                            defaultValue = column.Attributes["defaultValue"].Value;

                        if (column.Attributes["formatText"] != null)
                            formatText = column.Attributes["formatText"].Value;

                        if (column.Attributes["dataSourceID"] != null)
                            dataSourceID = column.Attributes["dataSourceID"].Value;

                        if (column.Attributes["dataSource"] != null)
                            dataSourceID = column.Attributes["dataSource"].Value;

                        //Width of the column - needs to be a valid Unit.
                        if (column.Attributes["width"] != null)
                            width = column.Attributes["width"].Value;

                        //Displays the column in edit mode or readonly - Check Infragistics AllowUpdate enum for more options
                        if (column.Attributes["allowUpdate"] != null)
                        {
                            if (Enum.IsDefined(typeof(AllowUpdate), column.Attributes["allowUpdate"].Value))
                                allowUpdate = (AllowUpdate)Enum.Parse(typeof(AllowUpdate), column.Attributes["allowUpdate"].Value);
                        }

                        if (column.Attributes["columnType"] != null)
                        {
                            if (Enum.IsDefined(typeof(ColumnType), column.Attributes["columnType"].Value))
                                colType = (ColumnType)Enum.Parse(typeof(ColumnType), column.Attributes["columnType"].Value);
                        }

                        if (column.Attributes["dataType"] != null)
                            dataType = column.Attributes["dataType"].Value;

                        if (column.Attributes["cellClass"] != null)
                            cellClass = column.Attributes["cellClass"].Value;


                        switch (colType)
                        {
                            case ColumnType.NotSet:
                                GetColumnInfo(column, out columnName, out columnBindingExpression);
                                XmlNode managerNode = column.SelectSingleNode("./COE:formElement", _manager); //Coverity Fix CID 13143 ASV
                                if (managerNode != null && !string.IsNullOrEmpty(managerNode.OuterXml))
                                {
                                    ICOEGenerableControl formElement = COEFormGenerator.GetCOEGenerableControl(managerNode.OuterXml, out errorMsg);

                                    if (!_grid.Columns.Exists(columnName))
                                    {
                                        UltraGridColumn col = new UltraGridColumn(true);
                                        this.SetColumnSettings(ref col, columnName, columnBindingExpression, ColumnType.Custom, headerText, !visible, true, ((Control)formElement).UniqueID, dataTextValueField, allowUpdate != AllowUpdate.Yes ? allowUpdate : AllowUpdate.Yes, formatText, colCastType, dataSourceID, dataType, defaultValue, width, cellClass);
                                        
                                        _grid.Columns.Add(col);
                                    }
                                    _columnsCount++;

                                    
                                    //Editor controls must be part of the container control (composite). If you included them in a cell, they will not be rendered clientside.
                                    //This is because the grid knows how to do this creating a copy of the control (that's why you need to add it just once)
                                    if (this.FindControl(((Control)formElement).ID) == null)
                                        this.Controls.Add((Control)formElement);
                                    if (formElement is ICOECultureable) //Set culture of inner COE Controls (e.g. maybe a inner COEDateEdit)
                                        ((ICOECultureable)formElement).DisplayCulture = _culture;

                                }
                                break;
                            case ColumnType.HyperLink:
                                columnName = this.GetColumnName(column);
                                if (!_grid.Columns.Exists(columnName))
                                {
                                    UltraGridColumn col = new UltraGridColumn(true);
                                    this.SetColumnSettings(ref col, columnName, columnName, ColumnType.HyperLink, headerText, !visible, true, string.Empty, string.Empty, AllowUpdate.No, formatText, colCastType, dataSourceID, dataType, width, cellClass);
                                    col.CellButtonDisplay = CellButtonDisplay.Always;
                                    _grid.Columns.Add(col);
                                }
                                _columnsCount++;
                                break;
                            case ColumnType.Custom:
                                columnName = this.GetColumnName(column);
                                if (!_grid.Columns.Exists(columnName))
                                {
                                    UltraGridColumn col = new UltraGridColumn(true);
                                    this.SetColumnSettings(ref col, columnName, columnName, ColumnType.NotSet, headerText, !visible, true, string.Empty, string.Empty, AllowUpdate.No, formatText, colCastType, dataSourceID, dataType, defaultValue, width, cellClass);
                                    _grid.Columns.Add(col);
                                    col.DefaultValue = defaultValue;
                                }
                                _columnsCount++;

                                break;
                            case ColumnType.CheckBox:
                                columnName = this.GetColumnName(column);
                                if (!_grid.Columns.Exists(columnName))
                                {
                                    UltraGridColumn col = new UltraGridColumn(true);
                                    this.SetColumnSettings(ref col, columnName, columnName, ColumnType.CheckBox, headerText, !visible, true, string.Empty, string.Empty, allowUpdate != AllowUpdate.Yes ? allowUpdate : AllowUpdate.Yes, formatText, colCastType, dataSourceID, dataType, width, cellClass);
                                    _grid.Columns.Add(col);
                                }
                                _columnsCount++;
                                break;
                        }
                    }
                }

                if (_grid.Columns.FromKey("RowState") == null)
                {
                    UltraGridColumn column = new UltraGridColumn();
                    column.Key = "RowState";
                    column.IsBound = false;
                    column.Header.Title = "RowState";
                    column.Hidden = true;

                    _grid.Columns.Add(column);
                    column.DefaultValue = DataChanged.Added;
                }
                _columnsCount++;
            }
            _coeLog.LogEnd(methodSignature);
            #endregion
        }

        

        [Serializable]
        private class ColSettings
        {
            private string _valueField = string.Empty;
            private string _formatExp = string.Empty;
            private SupportedColTypes _colCastType = SupportedColTypes.STRING;
            private object _defaultValue = null;

            public string ValueField { get { return _valueField; } }
            public string FormatExp { get { return _formatExp; } }
            public SupportedColTypes ColType { get { return _colCastType; } }
            public object DefaultValue { get { return _defaultValue; } }

            public static ColSettings NewColSettings(string valueField, string formatTextExpression, string colType)
            {
                return new ColSettings(valueField, formatTextExpression, colType);
            }

            public static ColSettings NewColSettings(string valueField, string formatTextExpression, string colType, string defValue)
            {
                return new ColSettings(valueField, formatTextExpression, colType, defValue);
            }

            public static ColSettings NewColSettings(string txt)
            {
                return new ColSettings(txt);
            }

            private ColSettings(string valueField, string formatTextExpression, string colType)
            {
                _valueField = valueField;
                _formatExp = formatTextExpression;
                if (Enum.IsDefined(typeof(SupportedColTypes), colType))
                    _colCastType = (SupportedColTypes)Enum.Parse(typeof(SupportedColTypes), colType);
            }

            private ColSettings(string txt)
            {
                _defaultValue = txt;
            }

            private ColSettings(string valueField, string formatTextExpression, string colType, string defValue) : this(defValue)
            {
                _valueField = valueField;
                _formatExp = formatTextExpression;
                if (Enum.IsDefined(typeof(SupportedColTypes), colType))
                    _colCastType = (SupportedColTypes)Enum.Parse(typeof(SupportedColTypes), colType);
            }
        }


        /// <summary>
        /// Sets the seetings of the column to add to the current grid
        /// </summary>
        /// <param name="col"></param>
        /// <param name="columnName"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="headerCaption"></param>
        /// <param name="hidden"></param>
        /// <param name="bound"></param>
        /// <param name="editorControlID"></param>
        /// <param name="valueField"></param>
        /// <param name="allowUpdate"></param>
        /// <param name="formatText"></param>
        /// <param name="colCastType"></param>
        /// <param name="dataSourceId"></param>
        /// <param name="dataType"></param>
        private void SetColumnSettings(ref UltraGridColumn col, string columnName, string key, ColumnType type, string headerCaption, bool hidden, bool bound, string editorControlID, string valueField, AllowUpdate allowUpdate, string formatText, string colCastType, string dataSource, string dataType, string width, string cellClass)
        {
            col.Key = col.Header.Key = col.Footer.Key = col.BaseColumnName = key;
            col.Type = type;
            col.Header.Caption = string.IsNullOrEmpty(headerCaption) ? columnName : headerCaption;
            col.Hidden = hidden;
            col.IsBound = bound;
            col.Width = new Unit(width);
            if (!string.IsNullOrEmpty(cellClass))
                col.CellStyle.CssClass = cellClass;

            if (!string.IsNullOrEmpty(editorControlID))
                col.EditorControlID = editorControlID;

            if (!string.IsNullOrEmpty(dataSource))
            {
                if (this.IsValidSelectSQL(dataSource))
                {
                    System.Data.DataSet ds = this.RetrieveDataSource(dataSource);
                    if (ds != null)
                    {
                        col.ValueList.DataSource = ds.Tables[0];
                        col.ValueList.DisplayMember = valueField.ToUpper(); //Needs upper case since the datable headers are all in caps
                        col.ValueList.ValueMember = col.ValueList.Key = key.ToUpper();
                    }
                }
                else
                {
                    col.ValueList.DataSourceID = dataSource;
                    col.ValueList.DisplayMember = valueField;
                    col.ValueList.ValueMember = col.ValueList.Key = key;
                }
                col.ValueList.DisplayStyle = ValueListDisplayStyle.DisplayText;
            }

            if (col.Tag == null)
                col.Tag = ColSettings.NewColSettings(valueField, formatText, colCastType);
            col.AllowUpdate = allowUpdate;
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
        private DataSet RetrieveDataSource(string sql)
        {
            System.Data.DataSet ds = null;
            try
            {
                if (_searchDAL == null) { LoadDAL(ConnStringType.PROXY); }
                // Coverity Fix CID - 11655
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

        private void SetColumnSettings(ref UltraGridColumn col, string columnName, string key, ColumnType type, string headerCaption, bool hidden, bool bound, string editorControlID, string valueField, AllowUpdate allowUpdate, string formatText, string colCastType, string dataSourceId, string dataType, string defaultValue, string width, string cellClass)
        {
            col.Tag = ColSettings.NewColSettings(valueField, formatText, colCastType, defaultValue);
            //col.Tag = type == ColumnType.NotSet ? ColSettings.NewColSettings(defaultValue) : ColSettings.NewColSettings(valueField, formatText, colCastType);
            //if(!string.IsNullOrEmpty(defaultValue))
            //    col.DefaultValue = defaultValue;
            this.SetColumnSettings(ref col, columnName, key, type, headerCaption, hidden, bound, editorControlID, valueField, allowUpdate, formatText, colCastType, dataSourceId, dataType, width, cellClass);
        }

        void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            this.InitializeLayout();
        }

        private void CreateControlHierarchy()
        {
            //Grid costmetic Settings
            this.GridDefaultSettings();
            if (this.FindControl(_grid.ID) == null)
                this.Controls.Add(_grid);
        }

        /// <summary>
        /// Default settings of the shown grid. Most of them can be changed by the Config xml
        /// </summary>
        private void GridDefaultSettings()
        {
            _grid.DisplayLayout.CellClickActionDefault = CellClickAction.Edit;
            _grid.DisplayLayout.AllowAddNewDefault = AllowAddNew.Yes;
            _grid.DisplayLayout.AllowUpdateDefault = AllowUpdate.Yes;
            _grid.DisplayLayout.EditCellStyleDefault.BorderColor = System.Drawing.Color.Gray;
            _grid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;
            _grid.DisplayLayout.RowHeightDefault = new Unit("22px");
            //_grid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
            _grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.OnServer;
            _grid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.SortMulti;
            _grid.DisplayLayout.BorderCollapseDefault = BorderCollapse.Collapse;
            _grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            _grid.DisplayLayout.AllowRowNumberingDefault = RowNumbering.ByBandLevel;
            _grid.DisplayLayout.EnableClientSideRenumbering = true;
            _grid.DisplayLayout.Name = _grid.ID;

            _grid.DisplayLayout.FilterOptionsDefault.AllString = "(All)";
            _grid.DisplayLayout.FilterOptionsDefault.NonEmptyString = "(NonEmpty)";
            _grid.DisplayLayout.FilterOptionsDefault.EmptyString = "(Empty)";

            /* TODO Review settins below to remove them or not*/
            _grid.DisplayLayout.GridLinesDefault = UltraGridLines.Both;
            _grid.DisplayLayout.GroupByBox.Style.BorderColor = System.Drawing.Color.Gray;
            _grid.DisplayLayout.GroupByBox.Style.BackColor = System.Drawing.Color.Blue;

            //Client Side Events.
            if (_jsEvents.Contains(ClientSideEvents.AfterExitEdit))
                _grid.DisplayLayout.ClientSideEvents.AfterExitEditModeHandler = string.Format(this._eventNameFormat, this.ID, ClientSideEvents.AfterExitEdit.ToString());

            if (_jsEvents.Contains(ClientSideEvents.BeforeEnterEdit))
                _grid.DisplayLayout.ClientSideEvents.BeforeEnterEditModeHandler = string.Format(this._eventNameFormat, this.ID, ClientSideEvents.BeforeEnterEdit.ToString());

            if (_jsEvents.Contains(ClientSideEvents.CellClickHandler))
                _grid.DisplayLayout.ClientSideEvents.CellClickHandler = string.Format(this._eventNameFormat, this.ID, ClientSideEvents.CellClickHandler.ToString());
        
            if (_jsEvents.Contains(ClientSideEvents.AfterRowInsert))
                _grid.DisplayLayout.ClientSideEvents.AfterRowInsertHandler = string.Format(this._eventNameFormat, this.ID, ClientSideEvents.AfterRowInsert.ToString());
        
            }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.DisableEditFeatures();

            }
            if (!string.IsNullOrEmpty(this.Label))
            {
                //_lit.Style.Add(HtmlTextWriterStyle.Display, "block");
                //this.Style.Add(HtmlTextWriterStyle.Display, "block");
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
                _lit.RenderControl(writer);
            }
            base.Render(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {

            string addIdToGridsArray = @"
            if(typeof(gridsArray) !== 'undefined')
                gridsArray[gridsArray.length] = '" + this.Grid.ClientID + "';";

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "AddGridId", addIdToGridsArray, true);

            string clearWebGrids = @"
            function clearWebGrids()
            {            
	            for(i = 0; i < gridsArray.length; i++)
	            {
		            var grid = igtbl_getGridById(gridsArray[i]);
                    if(grid != null)
                    {
                        for(j = 0; j < grid.Rows.length; j++)
		                {
			                var row = grid.Rows.getRow(j);
		                    var index = 0;
                            // DataKey is being used to store from which column it is no longer a default value
                            if(row.getDataKey() != null)
                            {
                                index = row.getDataKey();
                            }
		                    var cel = row.getCell(index);
		                    while(cel != null)
		                    {
			                    cel.setValue('');
			                    cel=row.getCell(index++);
		                    }
                        }
                    }
	            }
            }";

            if (!Page.ClientScript.IsStartupScriptRegistered("ClearWebGrids"))
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ClearWebGrids", clearWebGrids, true);


            if (_grid.DataSource == null && string.IsNullOrEmpty(_grid.DataSourceID))
            {
                GridDefaultSettings();
                InitializeLayout();
            }
            this.CheckColumnsValueLists(); //Need to add ValueList to column in case the configuration contains defaul rows to add.
            if (this.ReadOnly)
            {
                this.DisableEditFeatures();
                this.Enabled = false;
            }
            else
            {
                this.AddDefaultRows(); //Add default rows.
                //Create empty rows as set by configuration. Just a cosmetic thing.
                if (_grid.DisplayLayout.Rows.Count < _emptyDefaultRows)
                {
                    for (int i = _grid.DisplayLayout.Rows.Count; i < _emptyDefaultRows; i++)
                        _grid.DisplayLayout.Rows.Add();
                }
                this.ChechColumnsDefaultValues();
            }

            if (this.ViewEnabled)
            {
                this.Enabled = true;
            }

            //Modify values show test in cells. This must be done becase there are two different things
            //One is the value (thing to unbind) and the other is the text (thing to display to the user)
            this.ModifyCellTexts();
            this.SetJscript(); //Add Jscripts.
            if (_behavior == Behavior.DocMgr)
            {
                if (!this.UserHasPrivileges(Privileges.Delete))
                {
                    if (_grid.Columns.Exists(_deleteColID))
                        _grid.Columns.FromKey(_deleteColID).Hidden = true;
                }
            }
            this.OverWriteDisplaySettings();

            //Workaround that removes extra created columns in the control. This is just a workaround until we find the real cause of this.
            this.CleanUpColumns();

            if (this._requiredRowsNumber > 0)
            {
                CustomValidator rowNumberValidator = new CustomValidator();
                rowNumberValidator.ID = this.ClientID + "RowNumberValidator";
                rowNumberValidator.ClientValidationFunction = this.ClientID + "_checkRowNumber";
                if (string.IsNullOrEmpty(_requiredRowsNumberMessage))
                    rowNumberValidator.Text = "Please insert " + this._requiredRowsNumber + " " + this.Label + " at least";
                else
                    rowNumberValidator.Text = this._requiredRowsNumberMessage;
                rowNumberValidator.EnableClientScript = true;
                this.Controls.Add(rowNumberValidator);
            }
        }

        private void OverWriteDisplaySettings()
        {
            if (!_enableClientSideNumbering)
            {
                _grid.DisplayLayout.EnableClientSideRenumbering = false;
                _grid.DisplayLayout.AllowRowNumberingDefault = RowNumbering.None;
            }
            else
                _grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes; //By default is no, but it's required to hold numbering.
            
            //In case we don't display numbering but we need the selector column for removing rows.
            if (_rowSelectorsDefault != RowSelectors.No)//Default Value.
                _grid.DisplayLayout.RowSelectorsDefault = _rowSelectorsDefault;

            if (_headerWrap)
                _grid.DisplayLayout.HeaderStyleDefault.Wrap = true;
        }

        private void ChechColumnsDefaultValues()
        {
            foreach (UltraGridColumn col in _grid.Columns)
            {
                if (col.Tag is ColSettings)
                {
                    if (((ColSettings)col.Tag).DefaultValue != null)
                    {
                        foreach (UltraGridRow row in _grid.Rows)
                        {
                                if (_rowStyleCSS != null)
                                    row.Style.CssClass = _rowStyleCSS;
                                if (row.Cells.FromKey(col.Key) != null)
                                if (row.Cells.FromKey(col.Key).Value == null && 
                                    !string.IsNullOrEmpty(((ColSettings)col.Tag).DefaultValue.ToString()))
                                    row.Cells.FromKey(col.Key).Value = ((ColSettings)col.Tag).DefaultValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add default rows from configuration.
        /// </summary>
        private void AddDefaultRows()
        {
            string key = string.Empty;
            string dataValue = string.Empty;
            string dataText = string.Empty;
            AllowEditing allEdit = AllowEditing.NotSet;
            if (_xmlDefaultRows[0] != null)
            {
                foreach (XmlElement el in _xmlDefaultRows[0].ChildNodes)
                {
                    key = el.FirstChild.Attributes["bindingExpression"].Value;
                    dataValue = el.FirstChild.Attributes["dataValue"].Value;
                    dataText = el.FirstChild.Attributes["dataText"].Value;
                    if (el.FirstChild.Attributes["allowEditing"] != null)
                        if (!string.IsNullOrEmpty(el.FirstChild.Attributes["allowEditing"].Value))
                            if (Enum.IsDefined(typeof(AllowEditing), el.FirstChild.Attributes["allowEditing"].Value))
                                allEdit = (AllowEditing)Enum.Parse(typeof(AllowEditing), el.FirstChild.Attributes["allowEditing"].Value);

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(dataValue) && !this.AlreadyExistsRow(key, dataValue)
                        && _grid.Columns.FromKey(key).ValueList.ValueListItems.Count > 0) //Check to don't duplicate rows after postbacks
                    {
                        if (this.ValuesExists(_grid.Columns.FromKey(key).ValueList.ValueListItems, dataValue))
                        {
                            _grid.DisplayLayout.Rows.Add(); //add new row.
                            UltraGridRow row = _grid.DisplayLayout.Rows[_grid.DisplayLayout.Rows.Count - 1]; //latest added row.
                            if (_rowStyleCSS != null)
                                row.Style.CssClass = _rowStyleCSS;
                            if (row.Cells.FromKey(key) != null)
                            {
                                row.Cells.FromKey(key).Value = this.FormatText(dataText, dataValue, row.Cells.FromKey(key)); //Find the right text.
                            }

                            if (allEdit != AllowEditing.NotSet)
                                row.Cells.FromKey(key).AllowEditing = allEdit;
                            if (this.ReadOnly)
                                row.DataKey = row.Index;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Check if a value exist so we can add default rows
        /// </summary>
        /// <param name="items">List of items to check</param>
        /// <param name="value">Value to find in the collection</param>
        /// <returns></returns>
        private bool ValuesExists(ValueListItemsCollection items, string value)
        {
            bool retVal = false;
            foreach (ValueListItem item in items)
            {
                if (item.DataValue.ToString() == value)
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Methods that checks if the row already exists so we don't duplicate rows.
        /// </summary>
        /// <param name="key">Column Key</param>
        /// <param name="value">Value to check</param>
        /// <returns>Boolean indicating the result</returns>
        /// <remarks>This methods should be called after we added the rows from the datasource (those could be duplicated)</remarks>
        private bool AlreadyExistsRow(string key, string value)
        {
            bool retVal = false;
            foreach (UltraGridRow row in _grid.Rows)
            {
                    if (row.Cells.FromKey(key) != null)
                    if (row.Cells.FromKey(key).Value != null)
                        retVal = row.Cells.FromKey(key).Value.ToString().Equals(this.GetTextByValueKey(value, row.Cells.FromKey(key).Column.ValueList.ValueListItems))
                                || row.Cells.FromKey(key).Value.ToString().Equals(value);
                if (retVal)
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Returns the real text to display given a Keyword
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        private object FormatText(string text, string value, UltraGridCell cell)
        {
            object retVal = string.Empty;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value) && cell != null && !string.IsNullOrEmpty(cell.Column.Key) && cell.Column.Tag is ColSettings)
            {
                if (text.Contains("Custom_FindFromEditor"))
                    retVal = this.GetText(cell.Column.EditorControlID, cell.Column.Key, value, ((ColSettings)cell.Column.Tag).ValueField);
            }
            return retVal;
        }

        /// <summary>
        /// Check and force binding if the Column doesn't have added the valuelist required after post-back 
        /// </summary>
        private void CheckColumnsValueLists()
        {
            foreach (UltraGridColumn col in _grid.Columns)
            {
                if (!string.IsNullOrEmpty(col.ValueList.DataSourceID) || col.ValueList.DataSource != null)
                    col.ValueList.DataBind();
            }
        }

        /// <summary>
        /// Returns the text to display given a key looking at the ValueList associated to the column
        /// </summary>
        /// <param name="value">Value to search</param>
        /// <param name="list">List of values to look into - Datasource</param>
        /// <returns>Found text to display</returns>
        private string GetTextByValueKey(object value, ValueListItemsCollection list)
        {
            string retVal = string.Empty;
            foreach (ValueListItem item in list)
            {
                if (item.DataValue.ToString().Equals(value.ToString()))
                {
                    retVal = item.DisplayText;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Returns the key given a display text looking at the ValueList associated to the column
        /// </summary>
        /// <param name="value">Text to search</param>
        /// <param name="list">List of values to look into - Datasource</param>
        /// <returns>Found key</returns>
        private string GetKeyByValueKey(object value, ValueListItemsCollection list)
        {
            string retVal = string.Empty;
            foreach (ValueListItem item in list)
            {
                if (item.DisplayText.Equals(value))
                {
                    retVal = item.DataValue.ToString();
                    break;
                }
            }
            return retVal;
        }


        /// <summary>
        /// Removed extra added columns during postbacks
        /// </summary>
        private void CleanUpColumns()
        {
            for (int i = _grid.Columns.Count - 1; i >= _columnsCount; i--)
            {
                _grid.Columns.RemoveAt(i);
            }
        }

        /// <summary>
        /// Check in the COEPageControlSettings var if the user has privileges to do the current action.
        ///</summary>
        ///<param name="userPrivileges">Current privilege to check</param>
        ///<returns>A boolean value indicating if the user has the asked privilege</returns>
        private bool UserHasPrivileges(Privileges userPrivileges)
        {
            bool retVal = true;
            string[] columnsToDisable = null;
            if (System.Web.HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()] != null)
            {
                //Read the COEControlSettings Session var
                COEPageControlSettingsService.ControlList ctrls = ((COEPageControlSettingsService.ControlList)System.Web.HttpContext.Current.Session[GUIShellTypes.COEPageSettings + COEAppName.Get().ToString()]).GetByPageID(this.Page.ToString());
                // Coverity Fix CID - 10509 (from local server)
                if (ctrls != null && ctrls.Count > 0)
                {
                    foreach (COEPageControlSettingsService.Control ctrl in ctrls)
                    {
                        if (ctrl.ID.Contains(userPrivileges.ToString()) && ctrl.ID.Contains("DocMgr"))
                        {
                            retVal = false;
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds jsctips calls.
        /// </summary>
        private void SetJscript()
        {
            if (_requiredRowsNumber > 0)
            {
                string validatorMessage;
                if (string.IsNullOrEmpty(_requiredRowsNumberMessage))
                    validatorMessage = "Please insert " + this._requiredRowsNumber + " " + this.Label + " at least";
                else
                    validatorMessage = this._requiredRowsNumberMessage;
                string scriptCheckRowNumber = @"function " + this.ClientID + @"_checkRowNumber(sender, args)
                                {   
                                    var grid = igtbl_getGridById('" + this._grid.ClientID + @"');
                                    var deletedRows = 0;                                                                             
                                    for(i=0; i<grid.Rows.length; i++)
                                    {
                                        var row = grid.Rows.getRow(i);
                                        var cell = row.getCell(row.cells.length - 1);                                                                             
                                        if(cell.Value == '" + DataChanged.Deleted.ToString() + @"')
                                        {
                                            deletedRows++;    
                                        }                                                                                                                                                   
                                    }
                                    if((grid.Rows.length - deletedRows) < " + this._requiredRowsNumber + @")
                                    {
                                        sender.innerText = '" + validatorMessage + @"';                
                                        args.IsValid = false;                                                                                                       
                                    }
                                    else if(" + this.ClientID + @"_checkEmptyValues(sender))
                                    {
                                        args.IsValid = false;                                                                                                        
                                    }
                                    else
                                    {
                                        args.IsValid = true;                                                                                                        
                                    }
                                }";

                if (!this.Page.ClientScript.IsStartupScriptRegistered(this.ClientID + "_checkRowNumber"))
                    this.Page.ClientScript.RegisterStartupScript(Page.GetType(), this.ClientID + "_checkRowNumber", scriptCheckRowNumber, true);

                StringBuilder requiredColumnScript = new StringBuilder();
                requiredColumnScript.Append("function " + this.ClientID + "_checkEmptyValues(validator){");
                requiredColumnScript.Append("var emptyValues = false;");
                requiredColumnScript.Append("validator.innerText = '';");
                requiredColumnScript.Append("var grid = igtbl_getGridById('" + this._grid.ClientID + "');");
                requiredColumnScript.Append("var requiredColumns = new Array();");
                if (this._requiredColumnIndex != null)
                {
                    for (int i = 0; i < this._requiredColumnIndex.Length; i++)
                        requiredColumnScript.Append("requiredColumns[" + i + "] = '" + _requiredColumnIndex[i] + "';");
                }
                requiredColumnScript.Append("if(requiredColumns.length > 0){");
                requiredColumnScript.Append("for(j=0; j<requiredColumns.length; j++){");
                requiredColumnScript.Append("for(k=0; k<grid.Rows.length; k++){");
                requiredColumnScript.Append("var cell = grid.Rows.getRow(k).getCell(requiredColumns[j]);");
                requiredColumnScript.Append("if(cell.getValue() == null){");
                requiredColumnScript.Append("validator.innerText = cell.Column.HeaderText + ' value can´t be null or empty.';");
                requiredColumnScript.Append("emptyValues = true;");
                requiredColumnScript.Append("}");
                requiredColumnScript.Append("}");
                requiredColumnScript.Append("}");
                requiredColumnScript.Append("}");
                requiredColumnScript.Append("return emptyValues;");
                requiredColumnScript.Append("}");

                if (!this.Page.ClientScript.IsStartupScriptRegistered(this.ClientID + "_checkEmptyValues"))
                    this.Page.ClientScript.RegisterStartupScript(Page.GetType(), this.ClientID + "_checkEmptyValues", requiredColumnScript.ToString(), true);
            }

            if (_jsEvents.Count > 0)
            {
                StringBuilder jscriptCode = new StringBuilder();
                jscriptCode.Append("<script>");
                foreach (ClientSideEvents key in _jsEvents.Keys)
                {
                    string functionName = string.Format(_eventNameFormat, this.ID, key.ToString().Trim());
                    string jsMethodCode = _jsEvents[key].ToString().Trim();
                    this.CheckIfReservedMethod(ref jsMethodCode);


                    switch (key)
                    {
                        case ClientSideEvents.AfterExitEdit:
                            jsMethodCode = jsMethodCode.Replace("()", "(cellId)");
                            jscriptCode.Append(string.Format(_jsFunctionFormat, functionName, jsMethodCode));
                            _grid.DisplayLayout.ClientSideEvents.AfterExitEditModeHandler = functionName;
                            break;
                        case ClientSideEvents.BeforeEnterEdit:
                            jscriptCode.Append(string.Format(_jsFunctionFormat, functionName, jsMethodCode));
                            _grid.DisplayLayout.ClientSideEvents.BeforeEnterEditModeHandler = functionName;
                            break;
                        case ClientSideEvents.OnFormSubmit:
                            jscriptCode.Append(string.Format(_jsFunctionFormatSimple, functionName, jsMethodCode));
                            this.Page.ClientScript.RegisterOnSubmitStatement(typeof(string), "COEWebGridUltraCode_" + this.ID, "return " + functionName + "();");
                            break;
                        case ClientSideEvents.CellClickHandler:
                            jscriptCode.Append(string.Format(_jsCellFunctionFormat, functionName, jsMethodCode));
                            _grid.DisplayLayout.ClientSideEvents.CellClickHandler = functionName;
                            break;
                        case ClientSideEvents.AfterRowInsert:
                            //CSBR: 125954.  _jsRowFunctionFormat which takes GridName and rowId as parameters
                            jscriptCode.Append(string.Format(_jsRowFunctionFormat, functionName, jsMethodCode));
                            _grid.DisplayLayout.ClientSideEvents.AfterRowInsertHandler = functionName;
                            break;
                    }
                }

                jscriptCode.Append("</script>");
                if (!this.Page.ClientScript.IsStartupScriptRegistered("COEWebGridUltraCode_" + this.ID))
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "COEWebGridUltraCode_" + this.ID, jscriptCode.ToString());
            }

            string beforeRowDeleteHandler = @"
function BeforeRowDeletedHandler(gridName, rowId) 
{
           var row = igtbl_getRowById(rowId);
           if (row.getCell(0).getEditable() =='no') return true; //not allowing the user to delete the row if the cells are noneditable.
           row.getCell(row.cells.length - 1).setValue('" + DataChanged.Deleted.ToString() + @"');
           row.Element.style.display = 'none';

// true cancels the deletion;           
return true; 
}
";
            
            if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(string), "BeforeRowDeletedHandler"))
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "BeforeRowDeletedHandler", beforeRowDeleteHandler, true);


            _grid.DisplayLayout.ClientSideEvents.BeforeRowDeletedHandler = "BeforeRowDeletedHandler";
            _deleteLinkButton.OnClientClick = "return false;";
            _addLinkButton.OnClientClick = "return false;";
            
        }

        /// <summary>
        /// Checks if the entered js is a reserved work, so it can replace the code accordinly
        /// </summary>
        /// <param name="js">JS text</param>
        private void CheckIfReservedMethod(ref string js)
        {
            bool reservedJs = false;
            foreach (string jscript in Enum.GetNames(typeof(JS)))
            {
                if (js.Contains(jscript))
                {
                    reservedJs = true;
                    break;
                }
            }

            if (reservedJs)
            {
                if(js.Contains(JS.CustomJS_FilterByUnique.ToString()))
                    js = GetFilterByUnique(js.Substring(JS.CustomJS_FilterByUnique.ToString().Length + 2, js.IndexOf(")}") - (JS.CustomJS_FilterByUnique.ToString().Length + 2)));
                 //CSBR:125954
                //if CustomJS_SetDefaultValueForCOEDropDownListUltra string is found in JSMethodeCode 
                //i.e Client side script then it will call the DefaultValueForCOEDropDownListUltra method 
                //Parameter's are Identifierkey, Namekey.
                //Identifierkey will provide cell key of a newly added row.
                // NameKey will provide cell key of the first row of dropdown list  
                //rowIndex: indexof the row to retrive
                else if (js.Contains(JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString()))
                {
                    string identifierkey=js.Substring(JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString().Length + 2, js.IndexOf(",") - (JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString().Length + 2));
                    string namekey = js.Substring(JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString().Length + 15, js.IndexOf(")}") - (JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString().Length + 17));
                    string rowindex = js.Substring(JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString().Length + 20, js.IndexOf(")}") - (JS.CustomJS_SetDefaultValueForCOEDropDownListUltra.ToString().Length + 20));
                    js = DefaultValueForCOEDropDownListUltra(identifierkey, namekey, rowindex);
                }
                else if(js.Contains(JS.CustomJS_ValidateRows.ToString()))
                    js = GetValidateRowsScripts();
                else if(js.Contains(JS.CustomJS_CustomRowsValidation.ToString()))
                    AddCustomRowsValidator();
            }
        }

        /// <summary>
        /// Loop throught the xml params and write the validation jscript
        /// </summary>
        /// <returns>A valid js code to be called by the validator.</returns>
        private string GetValidateRowsScripts()
        {
            StringBuilder validationJS = new StringBuilder();
            StringBuilder caseJS = new StringBuilder();
            string parentColKey = string.Empty;
            string childColKey = string.Empty;
            string errorText = string.Empty;
            string caseFormat = "case '{0}': valid={1}(currentChildValue);errorTxt='{2}';break;";
            if (_xmlClientSideEventsConfig.Count > 0)
            {
                foreach (XmlElement el in _xmlClientSideEventsConfig)
                {
                    if (el.Attributes["name"].Value.ToUpper() == ClientSideEvents.OnFormSubmit.ToString().ToUpper())
                    {
                        foreach (XmlNode paramsNode in el.ChildNodes)
                        {
                            if (paramsNode is XmlElement)
                            {
                                parentColKey = paramsNode.Attributes["parentColKey"].Value;
                                childColKey = paramsNode.Attributes["childColKey"].Value;
                                foreach (XmlNode param in paramsNode.ChildNodes) //Param nodes
                                    caseJS.Append(string.Format(caseFormat, param.Attributes["parentColValue"].Value, param.Attributes["validationMethod"].Value, param.Attributes["errorMessage"].Value));
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(caseJS.ToString()) && !string.IsNullOrEmpty(parentColKey) && !string.IsNullOrEmpty(childColKey))
            {
                //Now we write the xml.
                validationJS.Append("var grid = igtbl_getGridById('" + _grid.ClientID + "');");
                validationJS.Append("var retVal = true; var valid = true; var errorTxt = 'Text Undefined';"); //hardcoded value will be overwritten by config settings
                validationJS.Append("if(grid != null)");
                validationJS.Append("{");
                validationJS.Append("for(j = 0; j < grid.Rows.length; j++)");
                validationJS.Append("{");
                validationJS.Append("if(grid.Rows.getRow(j).Element.style.display != 'none')");
                validationJS.Append("{");
                validationJS.Append("var currentParentValue = grid.Rows.getRow(j).getCellFromKey(\"" + parentColKey + "\").getValue();");
                validationJS.Append("var currentChildValue = grid.Rows.getRow(j).getCellFromKey(\"" + childColKey + "\").getValue();");
                validationJS.Append("if(currentParentValue != null && currentChildValue != null)");
                validationJS.Append("{");
                validationJS.Append("switch(currentParentValue)");
                validationJS.Append("{");
                validationJS.Append(caseJS.ToString());
                validationJS.Append("}");
                validationJS.Append("retVal = retVal && valid;");
                validationJS.Append("if(!retVal){ alert(errorTxt);break;}");
                validationJS.Append("}");
                validationJS.Append("}");
                validationJS.Append("}");
                validationJS.Append("}");
                validationJS.Append("return retVal;");
            }
            return validationJS.ToString();
        }

        private void AddCustomRowsValidator()
        {
            StringBuilder validationJS = new StringBuilder();
            StringBuilder caseJS = new StringBuilder();
            string parentColKey = string.Empty;
            string childColKey = string.Empty;
            string errorText = string.Empty;
            string caseFormat = "case '{0}': valid={1}(currentChildValue);errorTxt='{2}';break;";
            if (_xmlClientSideEventsConfig.Count > 0)
            {
                //perform defensive check for node type in case of XML comments
                foreach (XmlNode node in _xmlClientSideEventsConfig)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        XmlElement el = (XmlElement)node;
                        if (el.Attributes["name"].Value.ToUpper() == ClientSideEvents.CustomValidation.ToString().ToUpper())
                        {
                            foreach (XmlNode paramsNode in el.ChildNodes)
                            {
                                if (paramsNode is XmlElement)
                                {
                                    parentColKey = paramsNode.Attributes["parentColKey"].Value;
                                    childColKey = paramsNode.Attributes["childColKey"].Value;
                                    foreach (XmlNode param in paramsNode.ChildNodes) //Param nodes
                                        caseJS.Append(string.Format(caseFormat, param.Attributes["parentColValue"].Value, param.Attributes["validationMethod"].Value, param.Attributes["errorMessage"].Value));
                                }
                            }
                        }
                    }
                }

                ////old code
                //foreach (XmlElement el in _xmlClientSideEventsConfig)
                //{
                //    if (el.Attributes["name"].Value.ToUpper() == ClientSideEvents.CustomValidation.ToString().ToUpper())
                //    {
                //        foreach (XmlNode paramsNode in el.ChildNodes)
                //        {
                //            if (paramsNode is XmlElement)
                //            {
                //                parentColKey = paramsNode.Attributes["parentColKey"].Value;
                //                childColKey = paramsNode.Attributes["childColKey"].Value;
                //                foreach (XmlNode param in paramsNode.ChildNodes) //Param nodes
                //                    caseJS.Append(string.Format(caseFormat, param.Attributes["parentColValue"].Value, param.Attributes["validationMethod"].Value, param.Attributes["errorMessage"].Value));
                //            }
                //        }
                //    }
                //}
            }
            if (!string.IsNullOrEmpty(caseJS.ToString()) && !string.IsNullOrEmpty(parentColKey) && !string.IsNullOrEmpty(childColKey))
            {
                //Now we write the xml.
                validationJS.Append("function UltraWebGridCustomValidations" + _grid.ClientID + "(source, args) {");
                validationJS.Append("try {");
                validationJS.Append("var grid = igtbl_getGridById('" + _grid.ClientID + "');}");
                validationJS.Append("catch (err) {}");
                validationJS.Append("var retVal = true; var valid = true; var errorTxt = 'Text Undefined';"); //hardcoded value will be overwritten by config settings
                validationJS.Append("if(grid != null)");
                validationJS.Append("{");
                validationJS.Append("for(j = 0; j < grid.Rows.length; j++)");
                validationJS.Append("{");
                validationJS.Append("if(grid.Rows.getRow(j).Element.style.display != 'none')");
                validationJS.Append("{");
                validationJS.Append("var currentParentValue = grid.Rows.getRow(j).getCellFromKey(\"" + parentColKey + "\").getValue();");
                validationJS.Append("var currentChildValue = grid.Rows.getRow(j).getCellFromKey(\"" + childColKey + "\").getValue();");
                validationJS.Append("if(currentParentValue != null && currentChildValue != null)");
                validationJS.Append("{");
                validationJS.Append("switch(currentParentValue)");
                validationJS.Append("{");
                validationJS.Append(caseJS.ToString());
                validationJS.Append("}");
                validationJS.Append("retVal = retVal && valid;");
                validationJS.Append("if(!retVal){ args.IsValid = false; source.errormessage = errorTxt; break;}");
                validationJS.Append("}");
                validationJS.Append("}");
                validationJS.Append("}");
                validationJS.Append("}");
                validationJS.Append("args.IsValid = retVal;");
                validationJS.Append("}");
                if(!Page.ClientScript.IsStartupScriptRegistered("UltraWebGridCustomValidations" + _grid.ClientID))
                    Page.ClientScript.RegisterStartupScript(typeof(COEWebGridUltra), "UltraWebGridCustomValidations" + _grid.ClientID, validationJS.ToString(), true);

                CustomValidator validator = new CustomValidator();
                validator.ClientValidationFunction = "UltraWebGridCustomValidations" +_grid.ClientID;
                validator.EnableClientScript = true;
                this.Controls.Add(validator);
            }
        }

        /// <summary>
        /// Filters the rows checking the other selected values to have unique ones.
        /// </summary>
        /// <param name="key">js code</param>
        /// <returns>Full working js code</returns>
        private string GetFilterByUnique(string key)
        {
            StringBuilder filteringJS = new StringBuilder();
            filteringJS.Append("if(igtbl_getCellById(cellId).Row.getIndex() > 0 || igtbl_getGridById(gridName).Rows.length > 0)");
            filteringJS.Append("{");
            filteringJS.Append("var editorControlId = igtbl_getCellById(cellId).Column.EditorControlID;");
            filteringJS.Append("var combo = igcmbo_getComboById(editorControlId);");
            filteringJS.Append("var numRowsInCombo = combo.getGrid().Rows.length;");
            filteringJS.Append("var currentRowIndex = igtbl_getCellById(cellId).Row.getIndex();");
            filteringJS.Append("var numRowsInGrid = igtbl_getGridById(gridName).Rows.length;");
            filteringJS.Append("var currentSelValue = igtbl_getCellById(cellId).getValue();");
            filteringJS.Append("var splitter = '|';");
            filteringJS.Append("var selectedValues = '';");
            filteringJS.Append("for(j = 0; j < numRowsInGrid; j++)");
            filteringJS.Append("{");
            filteringJS.Append("var selectedValue = igtbl_getGridById(gridName).Rows.getRow(j).getCellFromKey(\"" + key + "\").getValue();");
            //CSBR:129586.
            //Here the logic implemented is to checking the display style of the row element 
            //which is made to 'none' in beforerowdeletehandler event.
            //it works fine wihtout effecting any other webgrids. as it specific to projects dropdownlist
            filteringJS.Append("var currentStyle = igtbl_getGridById(gridName).Rows.getRow(j).Element.style.display;");
            filteringJS.Append("if (selectedValue != currentSelValue & selectedValue != null & currentStyle!='none') selectedValues += splitter + selectedValue + splitter;");
            filteringJS.Append("}");//close for
            filteringJS.Append("for(i=0;i < numRowsInCombo; i++)");
            filteringJS.Append("{");
            filteringJS.Append("var foundCell = combo.getGrid().Rows.getRow(i).getCellFromKey(\"" + key + "\");");
            filteringJS.Append("if(selectedValues.indexOf(splitter + foundCell.getValue() + splitter) > -1) { foundCell.Row.Element.style.display = 'none'; }");
            filteringJS.Append("else {foundCell.Row.Element.style.display = 'inline';}");
            filteringJS.Append("}");//close for
            filteringJS.Append("}");//close if
            return filteringJS.ToString();
        }
        /// <summary>
        /// CSBR:125954
        /// This method embeds the client side script to AfterrowInsert event of of UltraWebGrid
        /// </summary>
        /// <returns></returns>
        private string DefaultValueForCOEDropDownListUltra(string identifierkey, string namekey, string rowIndex)
        {
            StringBuilder filteringJS = new StringBuilder();
            filteringJS.Append("var row = igtbl_getRowById(rowId);");
            filteringJS.Append("var cell=row.getCellFromKey(\"" + identifierkey + "\");");
            filteringJS.Append("var editorControlId = cell.Column.EditorControlID;");
            filteringJS.Append("var combo = igcmbo_getComboById(editorControlId);");
            filteringJS.Append("if(combo != null);");
            filteringJS.Append("{");
            filteringJS.Append("if(combo.getGrid().Rows.length >0)");
            filteringJS.Append("{");
            filteringJS.Append("var value=combo.grid.Rows.rows[" + rowIndex + "].getCellFromKey(\"" + namekey + "\").getValue();");
            filteringJS.Append("row.getCellFromKey(\"" + identifierkey + "\").setValue(value);");
            filteringJS.Append("}");
            filteringJS.Append("}");
            return filteringJS.ToString();
        }
        /// <summary>
        /// Set the text value according the value.
        /// This is for cases as when you have a dropdown and you want to display some text but hold a value behind.
        /// </summary>
        private void ModifyCellTexts()
        {
            foreach (UltraGridRow row in _grid.Rows)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.Column.EditorControlID) && cell.Column.Tag != null)
                    {
                        if (IsASupportedControl(cell.Column.EditorControlID) && cell.Value != null && cell.Column.Tag is ColSettings)
                            cell.Text = this.GetText(cell.Column.EditorControlID, cell.Key, cell.Value, ((ColSettings)cell.Column.Tag).ValueField);
                    }
                }
            }
        }

        /// <summary>
        /// Get the text of a given cell given the value of each
        /// </summary>
        /// <param name="editControlID">ControlID in charge to edit the cell text/value</param>
        /// <param name="key">Column key to search the value</param>
        /// <param name="value">Cell object containing the value to search</param>
        /// <param name="textField">Column Text key to get given an value key</param>
        /// <returns></returns>
        private string GetText(string editControlID, string key, object value, string textField)
        {
            string retVal = value.ToString();
            if (!string.IsNullOrEmpty(editControlID))
            {
                if (this.FindControl(editControlID) is COEDropDownListUltra)
                {
                    if (this.FindControl(editControlID) != null)
                        if (((COEDropDownListUltra)this.FindControl(editControlID)).Columns.FromKey(key).Find(value.ToString(),false,true) != null)
                            if (((COEDropDownListUltra)this.FindControl(editControlID)).Columns.FromKey(key).Find(value.ToString(),false,true).Row.Cells.FromKey(textField) != null)
                                retVal = ((COEDropDownListUltra)this.FindControl(editControlID)).Columns.FromKey(key).Find(value.ToString(),false,true).Row.Cells.FromKey(textField).Value.ToString();
                }
            }
            return retVal;
        }

        /// <summary>
        /// Check if the control is supported to search for it text
        /// </summary>
        /// <param name="editorControlID">Control ID</param>
        /// <returns></returns>
        private bool IsASupportedControl(string editorControlID)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(editorControlID))
            {
                if (this.FindControl(editorControlID) is COEDropDownListUltra)
                    retVal = true;
                else if (this.FindControl(editorControlID) is COETextEdit)
                    retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Disable fatures when is in read only mode
        /// </summary>
        private void DisableEditFeatures()
        {
            _grid.DisplayLayout.ReadOnly = Infragistics.WebUI.UltraWebGrid.ReadOnly.LevelZero;
            _grid.DisplayLayout.AddNewBox.Hidden = true;
            _grid.DisplayLayout.AllowAddNewDefault = AllowAddNew.No;
            //_deleteButton.Visible = false;


            if (_grid.Columns.FromKey("RowState") != null)
                _grid.Columns.Remove(_grid.Columns.FromKey("RowState"));

            this.EnableViewState = false;
        }
        /// <summary>
        /// Function to make the cells non editable if the it has value.
        /// </summary>
        public void SetCellsNonEditable()
        {
            foreach (UltraGridRow uRow in this.Grid.Rows)
            {
                uRow.Activated = false;
                foreach (UltraGridCell uCell in uRow.Cells)
                {
                    if (!string.IsNullOrEmpty(uCell.Text))
                    {
                        uCell.AllowEditing = AllowEditing.No;
                        uCell.Activated = false;
                    }
                }
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

        #region ICOECultureable Members

        public CultureInfo DisplayCulture
        {
            set { _culture = value; }
        }

        #endregion

        #region ICOEGrid Members
        public void SetColumnVisibility(string key, bool visibility)
        {
            if (_grid != null)
            {
                if (_grid.Columns.FromKey(key) != null)
                    _grid.Columns.FromKey(key).Hidden = !visibility;
            }
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


    }
}

