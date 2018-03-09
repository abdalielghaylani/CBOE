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
using CambridgeSoft.COE.Framework.Controls.ChemDraw;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;

[assembly: TagPrefix("CambridgeSoft.COE.Framework.Controls.COEFormGenerator", "COECntrl")]
namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ParseChildren(false),
    PersistChildren(true),
    ToolboxData("<{0}:COEFragments runat=server></{0}:COEFragments>")]
    public class COEFragments : CompositeControl, ICOEGenerableControl, ICOELabelable, ICOEReadOnly
    {
        #region Controls

        private UltraWebGrid _grid = new UltraWebGrid("UltraWebGridControl");
        private Infragistics.WebUI.WebCombo.WebCombo _fragmentTypesCombo = new Infragistics.WebUI.WebCombo.WebCombo("FragmentTypesControl");
        private Infragistics.WebUI.WebCombo.WebCombo _fragmentNamesCombo = new Infragistics.WebUI.WebCombo.WebCombo("FragmentNamesControl");
        private DataTable _data = new DataTable("OutputTable");
        private Label _title = new Label();
        private string _invalidCellText = "The Equivalent value must be a positive number. Please retype the value "; //Default Text - Overwritten by Config
        private int _columnsCount = 0;
        private bool _typesColVisible = true;
        private bool _namesColVisible = true;
        private bool _mwColVisible = true;
        private bool _formulaColVisible = true;
        public int _index = -1;
        private Hashtable _settings = new Hashtable();

        private LinkButton _deleteLinkButton = new LinkButton();
        private LinkButton _addLinkButton = new LinkButton();
        private string _removeButtonCSSClass = string.Empty;
        private string _addButtonCSSClass = string.Empty;
        private bool _addNewRowVisibility = true;
        private HtmlTableCell _deleteCell = new HtmlTableCell();
        private COEEditControl _editControl = COEEditControl.NotSet;
        #endregion

        #region Variables

        private string _defaultValue;
        private Label _lit = new Label();
        ScriptManager _scManager;
        XmlDocument _xmlData;
        XmlNamespaceManager _manager;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");
        
        /// <summary>
        /// Main Column Keys
        /// </summary>
 
        private enum ColumnKeys
        {
            FragmentId,
            FragmentType,
            FragmentName,
            Equivalent,
            Structure,
            MW,
            Formula,
            Code,
        }

        private enum SubColumnKeys
        {
            ID,
            Name,
        }


        /// <summary>
        /// Config keys
        /// </summary>
        private enum ConfigSetting
        {
            AddFragmentButtonText,
            TypesColTitle,
            TypesColWidth,
            TypesColVisible,
            NamesColTitle,
            NamesColWidth,
            NamesColVisible,
            MWColTitle,
            MWColWidth,
            MWColVisible,
            MWMask,
            FormulaColTitle,
            FormulaColWidth,
            FormulaColVisible,
            CodeColTitle,
            CodeColWidth,
            CodeColVisible,
            StructureColTitle,
            StructureColWidth,
            StructureColVisible,
            InvalidEquivalentValidationText,
            ReadOnly,
            ReadOnlyMode,
            DefaultEmptyRows,
            HeaderStyleCSS,
            HeaderHorizontalAlign,
            AddButtonCSS,
            RowAlternateStyleCSS,
            RowStyleCSS,
            SelectedRowStyleCSS,
        }

        #endregion

        #region Properties
        
        private bool _allowDelete;
        public bool AllowDelete
        {
            get
            {
                return _allowDelete;
            }
            set
            {
                if(value)
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
        /// Index of the control when inside a repeater control.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// UltraWebGrid control
        /// </summary>
        private UltraWebGrid Grid
        {
            get
            {
                EnsureChildControls();
                return _grid;
            }
            set { _grid = value; }
        }

        /// <summary>
        /// Infragistics DropDown control to select Fragment Type.
        /// </summary>
        public Infragistics.WebUI.WebCombo.WebCombo TypesCombo
        {
            get
            {
                EnsureChildControls();
                return _fragmentTypesCombo;
            }
            set { _fragmentTypesCombo = value; }
        }

        /// <summary>
        /// Infragistics DropDown control to select a fragment.
        /// </summary>
        public Infragistics.WebUI.WebCombo.WebCombo NamesCombos
        {
            get
            {
                EnsureChildControls();
                return _fragmentNamesCombo;
            }
            set { _fragmentNamesCombo = value; }
        }

        /// <summary>
        /// Read Only mode (same table but not in edit mode)
        /// </summary>
        public bool ReadOnly
        {
            get { return ViewState["RO"] == null ? false : (bool)ViewState["RO"]; }
            set { ViewState["RO"] = value; }
        }

        private string DeleteRowScript
        {
            get {
                return @"igtbl_deleteSelRows('" + _grid.ClientID + @"'); return false; ";
            }
        }

        private string AddRowScript
        {
            get
            {
                return @"igtbl_addNew('" + _grid.ClientID + @"', 0, true, true); return false;";
            }
        }

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
        
        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public COEFragments()
        {
        }

        /// <summary>
        /// Constructor called by the Repeater control
        /// </summary>
        /// <param name="data"></param>
        public COEFragments(string data): this()
        {
            this.LoadFromXml(data);
        }

        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return this.UnBind();
        }

        public void PutData(object data)
        {
            this._fragmentTypesCombo.DataBind();
            this._fragmentNamesCombo.DataBind();
            this._grid.DataSource = data;
            this._grid.DataBind();
            this.HideEmptyRows();
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

        #region Methods


        private void HideEmptyRows() 
        {
            if (this.Page.IsPostBack && this._grid.Rows.Count > 1)
            {                
                foreach (UltraGridRow row in this._grid.Rows)
                {
                    int fragmentCode;
                    string codeValue=row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).Value.ToString();
                    int.TryParse(codeValue, out fragmentCode);
                    if (fragmentCode <= 0 && string.IsNullOrEmpty(codeValue))
                        row.Hidden = true;
                }
            }
        }

        /// <summary>
        /// Returns a DataTable containing the entered info into the grid.
        /// </summary>
        /// <returns>DataTable with the entered info</returns>
        private object UnBind()
        {
            string fragIDColName = "FragmentID"; //It must match the property name in the object.
            string fragEquivColName = "Equivalents";
            string fragMWColName = "MW";
            string fragFormulaColName = "Formula";
            _data.Columns.Add(fragIDColName, typeof(System.Int32));
            _data.Columns.Add(fragEquivColName, typeof(System.Single));
            _data.Columns.Add(fragMWColName, typeof(System.String));
            _data.Columns.Add(fragFormulaColName, typeof(System.String));
            int tempFragmentID = 0;
            float tempEquivalent = 0;
            string tempMW = string.Empty;
            string tempFormula = string.Empty;
            foreach (UltraGridRow row in this._grid.Rows)
            {
                try
                {
                    DataRow newRow = _data.NewRow();
                    if (!string.IsNullOrEmpty(row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).GetText()))
                    {
                        if (int.TryParse(row.Cells.FromKey(ColumnKeys.FragmentId.ToString()).GetText(), out tempFragmentID)) 
                            newRow[fragIDColName] = tempFragmentID;
                        else if (int.TryParse(row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).GetText(), out tempFragmentID))
                            newRow[fragIDColName] = tempFragmentID;
                        else
                        {
                            //Try getting it given the text. - Be aware of uniqueness.
                            if (int.TryParse(row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).Tag.ToString(), out tempFragmentID))
                                newRow[fragIDColName] = tempFragmentID;
                            else
                                throw new Exception("Fragments: Invalid entered FragmentID");
                        }

                        if (newRow[fragIDColName] != null && ((int)newRow[fragIDColName]) > 0)
                        {
                            _data.Rows.Add(newRow);
                            if (Single.TryParse(row.Cells.FromKey(ColumnKeys.Equivalent.ToString()).GetText(), out tempEquivalent))
                                newRow[fragEquivColName] = tempEquivalent;
                            if (!string.IsNullOrEmpty(row.Cells.FromKey(ColumnKeys.MW.ToString()).GetText()))
                                newRow[fragMWColName] = row.Cells.FromKey(ColumnKeys.MW.ToString()).GetText();
                            if (!string.IsNullOrEmpty(row.Cells.FromKey(ColumnKeys.Formula.ToString()).GetText()))
                                newRow[fragFormulaColName] = row.Cells.FromKey(ColumnKeys.Formula.ToString()).GetText();
                            else
                                throw new Exception("Fragments: Invalid entered Equivalent");

                            string rowState = row.Cells.FromKey("RowState") != null && row.Cells.FromKey("RowState").Value != null ? row.Cells.FromKey("RowState").Value.ToString() : DataChanged.Unchanged.ToString();
                            switch((DataChanged) Enum.Parse(typeof(DataChanged), rowState))
                            {
                                case DataChanged.Deleted:
                                    newRow.AcceptChanges();
                                    newRow.Delete();
                                    break;
                                case DataChanged.Added:
                                    break;
                                case DataChanged.Modified:
                                    newRow.AcceptChanges();
                                    newRow.SetModified();
                                    break;
                                //DataChanged.Unchanged
                                default:
                                    newRow.AcceptChanges();
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return (object)_data;
        }

        /// <summary>
        /// Sets grid configuration settings
        /// </summary>
        /// <param name="xmlData">Configuration xml snippet</param>
        private void SetGridSettings(XmlNode xmlData)
        {
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

            XmlNode cssLabelClass = xmlData.SelectSingleNode("//COE:CSSLabelClass", _manager);
            if (cssLabelClass != null && cssLabelClass.InnerText.Length > 0)
                _lit.CssClass = cssLabelClass.InnerText;

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
                        _lit.Style.Add(styleId, styleValue);
                    }
                }
            }

            XmlNode id = _xmlData.SelectSingleNode("//COE:Id", _manager);
            if (id != null && id.InnerText.Length > 0)
                this.ID = id.InnerText;

            XmlNode fragmentTypeDSId = xmlData.SelectSingleNode("//COE:FragmentTypesDataSourceID", _manager);
            if (fragmentTypeDSId != null && fragmentTypeDSId.InnerText.Length > 0)
                this._fragmentTypesCombo.DataSourceID = fragmentTypeDSId.InnerText;

            XmlNode fragmentNamesDSId = xmlData.SelectSingleNode("//COE:FragmentNamesDataSourceID", _manager);
            if (fragmentNamesDSId != null && fragmentNamesDSId.InnerText.Length > 0)
                this._fragmentNamesCombo.DataSourceID = fragmentNamesDSId.InnerText;

            XmlNode addFragmentText = xmlData.SelectSingleNode("//COE:" + ConfigSetting.AddFragmentButtonText.ToString(), _manager);
            if (addFragmentText != null && addFragmentText.InnerText.Length > 0)
                _settings.Add(ConfigSetting.AddFragmentButtonText, addFragmentText.InnerText);

            XmlNode removeRowTitle = _xmlData.SelectSingleNode("//COE:DeleteFragmentButtonText", _manager);
            if(removeRowTitle != null && removeRowTitle.InnerText.Length > 0)
                this._deleteLinkButton.Text = removeRowTitle.InnerText;
            else
                this._deleteLinkButton.Text = "Delete";


            XmlNode invalidEquivalentText = xmlData.SelectSingleNode("//COE:" + ConfigSetting.InvalidEquivalentValidationText.ToString(), _manager);
            if (invalidEquivalentText != null && invalidEquivalentText.InnerText.Length > 0)
                _invalidCellText = invalidEquivalentText.InnerText;

            XmlNode readOnlyMode = xmlData.SelectSingleNode("//COE:" + ConfigSetting.ReadOnlyMode.ToString(), _manager);
            if (readOnlyMode != null && readOnlyMode.InnerText.Length > 0)
                this.ReadOnly = bool.Parse(readOnlyMode.InnerText);

            XmlNode readOnly = xmlData.SelectSingleNode("//COE:" + ConfigSetting.ReadOnly.ToString(), _manager);
            if (readOnly != null && readOnly.InnerText.Length > 0)
                this._grid.DisplayLayout.ReadOnly = (Infragistics.WebUI.UltraWebGrid.ReadOnly)Enum.Parse(typeof(Infragistics.WebUI.UltraWebGrid.ReadOnly), readOnly.InnerText);
            
            //if (_settings.ContainsKey(ConfigSetting.ReadOnly))
            //    this._grid.DisplayLayout.ReadOnly = (Infragistics.WebUI.UltraWebGrid.ReadOnly)_settings[ConfigSetting.ReadOnly];

            XmlNode typesColTitle = xmlData.SelectSingleNode("//COE:" + ConfigSetting.TypesColTitle.ToString(), _manager);
            if (typesColTitle != null && typesColTitle.InnerText.Length > 0)
                _settings.Add(ConfigSetting.TypesColTitle, typesColTitle.InnerText);

            XmlNode typesColWidth = xmlData.SelectSingleNode("//COE:" + ConfigSetting.TypesColWidth.ToString(), _manager);
            if (typesColWidth != null && typesColWidth.InnerText.Length > 0)
                _settings.Add(ConfigSetting.TypesColWidth, new Unit(typesColWidth.InnerText));

            XmlNode typesColVisible = xmlData.SelectSingleNode("//COE:" + ConfigSetting.TypesColVisible.ToString(), _manager);
            if (typesColVisible != null && typesColVisible.InnerText.Length > 0)
                _settings.Add(ConfigSetting.TypesColVisible, bool.Parse(typesColVisible.InnerText));

            XmlNode namesColTitle = xmlData.SelectSingleNode("//COE:" + ConfigSetting.NamesColTitle.ToString(), _manager);
            if (namesColTitle != null && namesColTitle.InnerText.Length > 0)
                _settings.Add(ColumnKeys.FragmentName, namesColTitle.InnerText);

            XmlNode namesColWidth = xmlData.SelectSingleNode("//COE:" + ConfigSetting.NamesColWidth.ToString(), _manager);
            if (namesColWidth != null && namesColWidth.InnerText.Length > 0)
                _settings.Add(ColumnKeys.FragmentName, new Unit(namesColWidth.InnerText));

            XmlNode namesColVisible = xmlData.SelectSingleNode("//COE:" + ConfigSetting.NamesColVisible.ToString(), _manager);
            if (namesColVisible != null && namesColVisible.InnerText.Length > 0)
                _settings.Add(ConfigSetting.NamesColVisible, bool.Parse(namesColVisible.InnerText));

            XmlNode mwColTitle = xmlData.SelectSingleNode("//COE:" + ConfigSetting.MWColTitle.ToString(), _manager);
            if (mwColTitle != null && mwColTitle.InnerText.Length > 0)
                _settings.Add(ConfigSetting.MWColTitle, mwColTitle.InnerText);

            XmlNode mwColWidth = xmlData.SelectSingleNode("//COE:" + ConfigSetting.MWColTitle.ToString(), _manager);
            if (mwColWidth != null && mwColWidth.InnerText.Length > 0)
                _settings.Add(ConfigSetting.MWColTitle, new Unit(mwColWidth.InnerText));

            XmlNode mwColVisible = xmlData.SelectSingleNode("//COE:" + ConfigSetting.MWColVisible.ToString(), _manager);
            if (mwColVisible != null && mwColVisible.InnerText.Length > 0)
                _settings.Add(ConfigSetting.MWColVisible, bool.Parse(mwColVisible.InnerText));

            XmlNode mwMask = xmlData.SelectSingleNode("//COE:" + ConfigSetting.MWMask.ToString(), _manager);
            if (mwMask != null && !string.IsNullOrEmpty(mwMask.InnerText))
                if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.MW.ToString()))
                    this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.MW.ToString()).Format = mwMask.InnerText;

            XmlNode formulaColTitle = xmlData.SelectSingleNode("//COE:" + ConfigSetting.FormulaColTitle.ToString(), _manager);
            if (formulaColTitle != null && formulaColTitle.InnerText.Length > 0)
                _settings.Add(ConfigSetting.FormulaColTitle, formulaColTitle.InnerText);

            XmlNode formulaColWidth = xmlData.SelectSingleNode("//COE:" + ConfigSetting.FormulaColWidth.ToString(), _manager);
            if (formulaColWidth != null && formulaColWidth.InnerText.Length > 0)
                _settings.Add(ConfigSetting.FormulaColWidth, new Unit(formulaColWidth.InnerText));

            XmlNode formulaColVisible = xmlData.SelectSingleNode("//COE:" + ConfigSetting.FormulaColVisible.ToString(), _manager);
            if (formulaColVisible != null && formulaColVisible.InnerText.Length > 0)
                _settings.Add(ConfigSetting.FormulaColVisible, bool.Parse(formulaColVisible.InnerText));

            //XmlNode invalidEquivalentText = xmlData.SelectSingleNode("//COE:" + ConfigSetting.InvalidEquivalentValidationText.ToString(), _manager);
            //if (invalidEquivalentText != null && invalidEquivalentText.InnerText.Length > 0)
            //    _settings.Add(ConfigSetting.InvalidEquivalentValidationText, invalidEquivalentText.InnerText);

            //XmlNode readOnly = xmlData.SelectSingleNode("//COE:" + ConfigSetting.ReadOnly.ToString(), _manager);
            //if (readOnly != null && readOnly.InnerText.Length > 0)
            //    _settings.Add(ConfigSetting.ReadOnly, (ReadOnly)Enum.Parse(typeof(Infragistics.WebUI.UltraWebGrid.ReadOnly), readOnly.InnerText));

            XmlNode codeColTitle = xmlData.SelectSingleNode("//COE:" + ConfigSetting.CodeColTitle.ToString(), _manager);
            if (codeColTitle != null && codeColTitle.InnerText.Length > 0)
                _settings.Add(ConfigSetting.CodeColTitle, codeColTitle.InnerText);

            XmlNode codeColWidth = xmlData.SelectSingleNode("//COE:" + ConfigSetting.CodeColWidth.ToString(), _manager);
            if (codeColWidth != null && codeColWidth.InnerText.Length > 0)
                _settings.Add(ConfigSetting.CodeColWidth, new Unit(codeColWidth.InnerText));

            XmlNode codeColVisible = xmlData.SelectSingleNode("//COE:" + ConfigSetting.CodeColVisible.ToString(), _manager);
            if (codeColVisible != null && codeColVisible.InnerText.Length > 0)
                _settings.Add(ConfigSetting.CodeColVisible, bool.Parse(codeColVisible.InnerText));

            XmlNode structureColTitle = xmlData.SelectSingleNode("//COE:" + ConfigSetting.StructureColTitle.ToString(), _manager);
            if (structureColTitle != null && structureColTitle.InnerText.Length > 0)
                _settings.Add(ConfigSetting.StructureColTitle, structureColTitle.InnerText);

            XmlNode structureColWidth = xmlData.SelectSingleNode("//COE:" + ConfigSetting.StructureColWidth.ToString(), _manager);
            if (structureColWidth != null && structureColWidth.InnerText.Length > 0)
                _settings.Add(ConfigSetting.StructureColWidth, new Unit(structureColWidth.InnerText));

            XmlNode structureColVisible = xmlData.SelectSingleNode("//COE:" + ConfigSetting.StructureColVisible.ToString(), _manager);
            if (structureColVisible != null && structureColVisible.InnerText.Length > 0)
                _settings.Add(ConfigSetting.StructureColVisible, bool.Parse(structureColVisible.InnerText));

            XmlNode defaultEmptyRows = _xmlData.SelectSingleNode("//COE:DefaultEmptyRows", _manager);
            if (defaultEmptyRows != null && defaultEmptyRows.InnerText.Length > 0)
                _settings.Add(ConfigSetting.DefaultEmptyRows, Convert.ToInt32(defaultEmptyRows.InnerText));

            XmlNode headerStyleCSS = _xmlData.SelectSingleNode("//COE:" + ConfigSetting.HeaderStyleCSS, _manager);
            if (headerStyleCSS != null && headerStyleCSS.InnerText.Length > 0)
                _settings.Add(ConfigSetting.HeaderStyleCSS, headerStyleCSS.InnerText);

            XmlNode headerHAlign = _xmlData.SelectSingleNode("//COE:" + ConfigSetting.HeaderHorizontalAlign , _manager);
            if (headerHAlign != null && headerHAlign.InnerText.Length > 0)
                if (Enum.IsDefined(typeof(HorizontalAlign), headerHAlign.InnerText))
                    _settings.Add(ConfigSetting.HeaderHorizontalAlign, (HorizontalAlign)Enum.Parse(typeof(HorizontalAlign), headerHAlign.InnerText));

            XmlNode addButtonStyleCSS = _xmlData.SelectSingleNode("//COE:" + ConfigSetting.AddButtonCSS, _manager);
            if (addButtonStyleCSS != null && addButtonStyleCSS.InnerText.Length > 0)
                _settings.Add(ConfigSetting.AddButtonCSS, addButtonStyleCSS.InnerText);
            
            XmlNode removeButtonStyleCSS = _xmlData.SelectSingleNode("//COE:RemoveButtonCSS", _manager);
            if(removeButtonStyleCSS != null && removeButtonStyleCSS.InnerText.Length > 0)
                _deleteLinkButton.CssClass = _deleteCell.Attributes["class"] = removeButtonStyleCSS.InnerText;

            XmlNode allowDeletingRows = _xmlData.SelectSingleNode("//COE:AllowDeletingRows", _manager);
            if(allowDeletingRows != null && allowDeletingRows.InnerText.Length > 0)
                this.AllowDelete = bool.Parse(allowDeletingRows.InnerText);
            else
                this.AllowDelete = true;

            XmlNode rowAltStyleCSS = _xmlData.SelectSingleNode("//COE:" + ConfigSetting.RowAlternateStyleCSS, _manager);
            if (rowAltStyleCSS != null && rowAltStyleCSS.InnerText.Length > 0)
                _settings.Add(ConfigSetting.RowAlternateStyleCSS, rowAltStyleCSS.InnerText);

            XmlNode rowStyleCSS = _xmlData.SelectSingleNode("//COE:" + ConfigSetting.RowStyleCSS , _manager);
            if (rowStyleCSS != null && rowStyleCSS.InnerText.Length > 0)
                _settings.Add(ConfigSetting.RowStyleCSS, rowStyleCSS.InnerText);

            XmlNode selectedRowStyleCSS = _xmlData.SelectSingleNode("//COE:" + ConfigSetting.SelectedRowStyleCSS, _manager);
            if (selectedRowStyleCSS != null && selectedRowStyleCSS.InnerText.Length > 0)
                _settings.Add(ConfigSetting.SelectedRowStyleCSS, selectedRowStyleCSS.InnerText);
                
        }

       public void LoadFromXml(string xmlDataAsString)
        {
            _xmlData = new XmlDocument();
            _xmlData.LoadXml(xmlDataAsString);

            _manager = new XmlNamespaceManager(_xmlData.NameTable);
            _manager.AddNamespace("COE", _xmlData.DocumentElement.NamespaceURI);

            XmlNode managerNode = _xmlData.SelectSingleNode("//COE:GridConfigSettings", _manager); //Coverity Fix CID 21812 ASV
            if (managerNode != null)
                this.SetGridSettings(managerNode);
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

        private string GetNamesComboTextByFragmentID(int id, string key)
        {
            string retVal = string.Empty;
            try
            {
                if (id > 0 && !string.IsNullOrEmpty(key))
                {
                    retVal = this._fragmentNamesCombo.Rows[this._fragmentNamesCombo.Columns.FromKey("FragmentID").Find(id.ToString(),true,true).Row.Index].Cells.FromKey(key).Text;
                }
            }
            catch (Exception)
            {
            }
            return retVal;
        }

        /// <summary>
        /// Returns the FragmentTypesName given a FragmentTypeID.
        /// Mostly used when unbinding
        /// </summary>
        /// <param name="id">Fragment Type ID</param>
        /// <returns>Fragment Type name</returns>
        private string GetTypesComboTextByFragTypeId(int id)
        {
            string retVal = string.Empty;
            try
            {
                if (id > 0)
                {
                    retVal = this._fragmentTypesCombo.Rows[this._fragmentTypesCombo.Columns.FromKey("Key").Find(id.ToString(),true,true).Row.Index].Cells.FromKey("Value").Text;
                }
            }
            catch (Exception)
            {
            }
            return retVal;
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
                if(!this.ReadOnly)
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
                  
                    htmlTable.ID = "DeleteTable";
                    htmlTable.Rows.Add(new HtmlTableRow());
                    htmlTable.Rows[1].VAlign = "top";
                    htmlTable.Rows[1].Cells.Add(new HtmlTableCell());
                    htmlTable.Rows[1].Cells[0].Style.Value = "padding-right: 9px;";
                    htmlTable.Rows[1].Cells[0].VAlign = "middle";

                    htmlTable.Rows[1].Cells.Add(this._deleteCell);
                    _deleteLinkButton.Style.Value = "background-image:none;margin-left:0px;padding-left:0px;";
                    htmlTable.Rows[1].Cells[1].Controls.Add(_deleteLinkButton);
                    this.Controls.Add(htmlTable);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                base.CreateChildControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            this.Init += new EventHandler(COEFragments_Init);
            this._grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            this._grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            base.OnInit(e);
        }

        void COEFragments_Init(object sender, EventArgs e)
        {
            this._grid.DisplayLayout.AutoGenerateColumns = false;
            this._grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            this._grid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;
            this._grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            //this.ID = "COEFragmentsControl";
            this.InitializeLayout();
            //this._grid.DisplayLayout.NoDataMessage = string.Empty;
            this._grid.DisplayLayout.NoDataMessage = "No Fragments Info";

            _deleteLinkButton.ID = "DeleteLinkButton";
            _addLinkButton.ID = "AddLinkButton";
        }

        /// <summary>
        /// Set default values in case the datasource is not null or empty
        /// Called as many time as fragments in the datasource 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            COEDataBinder dataBinder = new COEDataBinder(e.Data);
            object equivalentsItemValue = null;
            object fragmentIDItemValue = null;
            equivalentsItemValue = dataBinder.RetrieveProperty("Equivalents");
            fragmentIDItemValue = dataBinder.RetrieveProperty("FragmentID");
            //Find values using the NamesCombo as a datasource for the missing values.
            e.Row.Cells.FromKey(ColumnKeys.FragmentId.ToString()).Value = (int)fragmentIDItemValue > 0 ? ((int)fragmentIDItemValue).ToString() : "0";
            e.Row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).Value = e.Row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).Tag = (int)fragmentIDItemValue > 0 ? ((int)fragmentIDItemValue).ToString() : "0"; // Stores primary key first in Tag
            //e.Row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).Text = this.GetNamesComboTextByFragmentID((int)fragmentIDItemValue, "Description");
            e.Row.Cells.FromKey(ColumnKeys.FragmentName.ToString()).Text = this.GetNamesComboTextByFragmentID((int)fragmentIDItemValue, "Code");
            
            string fragmentTypeIdString = this.GetNamesComboTextByFragmentID((int)fragmentIDItemValue, "FragmentTypeId");
            e.Row.Cells.FromKey(ColumnKeys.FragmentType.ToString()).Value = fragmentTypeIdString;
            e.Row.Cells.FromKey(ColumnKeys.FragmentType.ToString()).Text = fragmentTypeIdString == string.Empty ? string.Empty : this.GetTypesComboTextByFragTypeId(Convert.ToInt32(fragmentTypeIdString));
            e.Row.Cells.FromKey(ColumnKeys.Equivalent.ToString()).Text = (float)equivalentsItemValue > 0 ? ((float)equivalentsItemValue).ToString() : "";
            e.Row.Cells.FromKey(ColumnKeys.MW.ToString()).Text = this.GetNamesComboTextByFragmentID((int)fragmentIDItemValue, "MW");
            e.Row.Cells.FromKey(ColumnKeys.Formula.ToString()).Text = this.GetNamesComboTextByFragmentID((int)fragmentIDItemValue, "Formula");
            e.Row.Cells.FromKey(ColumnKeys.Code.ToString()).Text = this.GetNamesComboTextByFragmentID((int)fragmentIDItemValue, "Description");
            e.Row.Style.CssClass = _settings[ConfigSetting.RowStyleCSS].ToString();
        }

        void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            this.InitializeLayout();
        }

        private void InitializeLayout()
        {
            
            //Add by default a Row.
            if (this._grid.Rows.Count == 0)
                this._grid.DisplayLayout.Rows.Add();
            //Customized Columns
            this._grid.DisplayLayout.AutoGenerateColumns = false;

            _addLinkButton.Text = "Add Fragment"; //Default text then overwritten by configuration
            _addButtonCSSClass = _settings[ConfigSetting.AddButtonCSS].ToString();
                     
            this._grid.Rows[0].Style.CssClass = _settings[ConfigSetting.RowStyleCSS].ToString();
            #region Create default Columns

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentType.ToString()))
            {
                UltraGridColumn fragmentTypeCol = new UltraGridColumn(true);
                fragmentTypeCol.Key = fragmentTypeCol.Footer.Key = fragmentTypeCol.Header.Key = ColumnKeys.FragmentType.ToString();
                fragmentTypeCol.Header.Caption = "Fragment Types";
                fragmentTypeCol.EditorControlID = this._fragmentTypesCombo.ID;
                fragmentTypeCol.Type = ColumnType.Custom;
                fragmentTypeCol.Width = new Unit("120px");
                this._grid.DisplayLayout.Bands[0].Columns.Add(fragmentTypeCol);
                _columnsCount++;
            }

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentName.ToString()))
            {
                UltraGridColumn fragmentNameCol = new UltraGridColumn(true);
                fragmentNameCol.Key = fragmentNameCol.Footer.Key = fragmentNameCol.Header.Key = ColumnKeys.FragmentName.ToString();
                fragmentNameCol.Header.Caption = "Code";
                fragmentNameCol.EditorControlID = this._fragmentNamesCombo.ID;
                fragmentNameCol.Type = ColumnType.Custom;
                fragmentNameCol.Width = new Unit("60px");
                this._grid.DisplayLayout.Bands[0].Columns.Add(fragmentNameCol);
                _columnsCount++;
            }

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Equivalent.ToString()))
            {
                UltraGridColumn equivalentsCol = new UltraGridColumn(true);
                equivalentsCol.Key = equivalentsCol.Header.Key = equivalentsCol.Footer.Key = ColumnKeys.Equivalent.ToString();
                equivalentsCol.Header.Caption = "Equivalent";
                equivalentsCol.Type = ColumnType.NotSet;
                equivalentsCol.AllowUpdate = AllowUpdate.Yes;
                equivalentsCol.IsBound = false;
                equivalentsCol.Width = new Unit("80px");
                equivalentsCol.Format = "##.#####";
                this._grid.DisplayLayout.Bands[0].Columns.Add(equivalentsCol);
                _columnsCount++;
            }

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.MW.ToString()))
            {
                UltraGridColumn mwCol = new UltraGridColumn(true);
                mwCol.Key = ColumnKeys.MW.ToString();
                mwCol.Header.Caption = "MW";
                mwCol.Type = ColumnType.Custom;
                mwCol.AllowUpdate = AllowUpdate.No;
                mwCol.Width = new Unit("70px");
                mwCol.DataType = "System.Double";
                this._grid.DisplayLayout.Bands[0].Columns.Add(mwCol);
                _columnsCount++;
            }

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Formula.ToString()))
            {
                UltraGridColumn formulaCol = new UltraGridColumn(true);
                formulaCol.Key = ColumnKeys.Formula.ToString();
                formulaCol.Header.Caption = "Formula";
                formulaCol.Type = ColumnType.Custom;
                formulaCol.AllowUpdate = AllowUpdate.No;
                formulaCol.Width = new Unit("70px");
                this._grid.DisplayLayout.Bands[0].Columns.Add(formulaCol);
                _columnsCount++;
            }

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Code.ToString()))
            {
                UltraGridColumn codeCol = new UltraGridColumn(true);
                codeCol.Key = ColumnKeys.Code.ToString();
                codeCol.Header.Caption = "Description";
                codeCol.Type = ColumnType.Custom;
                codeCol.AllowUpdate = AllowUpdate.No;
                codeCol.Width = new Unit("150px");
                this._grid.DisplayLayout.Bands[0].Columns.Add(codeCol);
            }

            if (!this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentId.ToString()))
            {
                UltraGridColumn fragmentId = new UltraGridColumn(true);
                fragmentId.Key = ColumnKeys.FragmentId.ToString();
                fragmentId.Header.Caption = "FragmentId";
                fragmentId.Type = ColumnType.NotSet;
                fragmentId.AllowUpdate = AllowUpdate.No;
                fragmentId.Width = new Unit("1px");
                fragmentId.Hidden = true;
                this._grid.DisplayLayout.Bands[0].Columns.Add(fragmentId);
                _columnsCount++;
            }

            if(_grid.Columns.FromKey("RowState") == null)
            {
                UltraGridColumn column = new UltraGridColumn();
                column.Key = "RowState";
                column.IsBound = false;
                column.Header.Title = "RowState";
                column.Hidden = true;
                _grid.Columns.Add(column);
                column.DefaultValue = DataChanged.Added;
                _columnsCount++;
            }

           #endregion

            #region Create WebCombos that are part of Grid Cells

            if (this.FindControl(this._fragmentTypesCombo.ID) == null)
                this.ReCreateTypesCombo();
            else if (this._fragmentTypesCombo.DataValueField == null)
            {
                this._fragmentTypesCombo.Columns.Clear();
                this.ReCreateTypesCombo();
            }

            if (this.FindControl(this._fragmentNamesCombo.ID) == null)
                this.ReCreateNamesCombo();
            if (this._fragmentNamesCombo.DataValueField == null)
            {
                this._fragmentNamesCombo.Columns.Clear();
                this.ReCreateNamesCombo();
            }
            #endregion
        }

        private void ReCreateTypesCombo()
        {
            if (_fragmentTypesCombo.Columns.Count == 0)
            {
                _fragmentTypesCombo.DataValueField = "Key";
                _fragmentTypesCombo.DataTextField = "Value";
                _fragmentTypesCombo.DropDownLayout.ColWidthDefault = Unit.Empty;
                _fragmentTypesCombo.DropDownLayout.DropdownHeight = Unit.Empty;
                _fragmentTypesCombo.DropDownLayout.DropdownWidth = Unit.Pixel(120);
                _fragmentTypesCombo.DropDownLayout.RowSelectors = RowSelectors.No;
                _fragmentTypesCombo.DropDownLayout.ColHeadersVisible = ShowMarginInfo.No;
                _fragmentTypesCombo.DropDownLayout.GridLines = UltraGridLines.Horizontal;
                _fragmentTypesCombo.DropDownLayout.FrameStyle.BorderWidth = Unit.Pixel(1);
                _fragmentTypesCombo.DropDownLayout.FrameStyle.BorderColor = _fragmentTypesCombo.DropDownLayout.RowStyle.BorderColor;
                _fragmentTypesCombo.DropDownLayout.SelectedRowStyle.ForeColor = System.Drawing.Color.White;
                _fragmentTypesCombo.DropDownLayout.SelectedRowStyle.BackColor = System.Drawing.Color.Blue;
                _fragmentTypesCombo.DropDownLayout.FrameStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Default;
                _fragmentTypesCombo.ComboTypeAhead = Infragistics.WebUI.WebCombo.TypeAhead.Extended;
            }

            if (!_fragmentTypesCombo.Columns.Exists("Key"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "ID";
                col.BaseColumnName = col.Key = "Key";
                col.Hidden = true;
                _fragmentTypesCombo.Columns.Add(col);
            }

            if (!_fragmentTypesCombo.Columns.Exists("Value"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Fragment Type";
                col.BaseColumnName = col.Key = "Value";
                _fragmentTypesCombo.Columns.Add(col);
            }

            foreach (UltraGridColumn col in _fragmentTypesCombo.Columns)
            {
                col.IsBound = true;
                col.AllowUpdate = AllowUpdate.No;
                col.CellStyle.Wrap = true;  // suppress nobr tags
            }
        }

        private void ReCreateNamesCombo()
        {
            if (_fragmentNamesCombo.Columns.Count == 0)
            {
                _fragmentNamesCombo.DataValueField = "FragmentId";
                _fragmentNamesCombo.DataTextField = "Code";
                _fragmentNamesCombo.DropDownLayout.AutoGenerateColumns = false;
                _fragmentTypesCombo.DropDownLayout.ColWidthDefault = Unit.Empty;
                _fragmentNamesCombo.DropDownLayout.DropdownHeight = Unit.Pixel(260);
                _fragmentNamesCombo.DropDownLayout.DropdownWidth = Unit.Empty;
                _fragmentNamesCombo.DropDownLayout.RowSelectors = RowSelectors.No;
                //_fragmentNamesCombo.DropDownLayout.ColHeadersVisible = ShowMarginInfo.No;
                _fragmentNamesCombo.DropDownLayout.GridLines = UltraGridLines.Horizontal;
                //_fragmentNamesCombo.DropDownLayout.StationaryMargins = StationaryMargins.Header;  // turning this on would be nice, but breaks autosizing of columns
                _fragmentNamesCombo.DropDownLayout.FrameStyle.TextOverflow = TextOverflow.Ellipsis;
                _fragmentNamesCombo.DropDownLayout.FrameStyle.BorderWidth = Unit.Pixel(1);
                _fragmentNamesCombo.DropDownLayout.FrameStyle.BorderColor = _fragmentNamesCombo.DropDownLayout.RowStyle.BorderColor;
                _fragmentNamesCombo.DropDownLayout.SelectedRowStyle.ForeColor = System.Drawing.Color.White;
                _fragmentNamesCombo.DropDownLayout.SelectedRowStyle.BackColor = System.Drawing.Color.Blue;
                _fragmentNamesCombo.DropDownLayout.FrameStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
                _fragmentNamesCombo.InitializeRow += new Infragistics.WebUI.WebCombo.InitializeRowEventHandler(_fragmentNamesCombo_InitializeRow);
                _fragmentNamesCombo.ComboTypeAhead = Infragistics.WebUI.WebCombo.TypeAhead.Extended;
            }

            if (!_fragmentNamesCombo.Columns.Exists("FragmentId"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Fragment Id";
                col.BaseColumnName = col.Key = "FragmentId";
                col.Hidden = true;
                _fragmentNamesCombo.Columns.Add(col);
            }

            if (!_fragmentNamesCombo.Columns.Exists("Code"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Code";
                col.BaseColumnName = col.Key = "Code";
                _fragmentNamesCombo.Columns.Add(col);
            }

            if (!_fragmentNamesCombo.Columns.Exists("Structure"))
            {
                TemplatedColumn col = new TemplatedColumn(true);
                col.Header.Caption = "Structure";
                col.BaseColumnName = col.Key = "Structure.Value";
                col.CellTemplate = new TemplatedChemDraw();
                col.CellButtonDisplay = CellButtonDisplay.Always;
                _fragmentNamesCombo.Columns.Add(col);
            }

            if (!_fragmentNamesCombo.Columns.Exists("Description"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Description";
                col.BaseColumnName = col.Key = "Description";
                _fragmentNamesCombo.Columns.Add(col);
            }

            if (!_fragmentNamesCombo.Columns.Exists("FragmentTypeId"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Fragment Type Id";
                col.BaseColumnName = col.Key = "FragmentTypeId";
                col.Hidden = true;
                _fragmentNamesCombo.Columns.Add(col);
            }

            if (!_fragmentNamesCombo.Columns.Exists("MW"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Mol Weight";
                col.BaseColumnName = col.Key = "MW";
                _fragmentNamesCombo.Columns.Add(col);
            }

            if (!_fragmentNamesCombo.Columns.Exists("Formula"))
            {
                UltraGridColumn col = new UltraGridColumn(true);
                col.Header.Caption = "Mol Formula";
                col.BaseColumnName = col.Key = "Formula";
                _fragmentNamesCombo.Columns.Add(col);
            }

            foreach (UltraGridColumn col in _fragmentTypesCombo.Columns)
            {
                col.IsBound = true;
                col.CellStyle.Wrap = true;  // suppress nobr tags
            }
        }

        /// <summary>
        /// Create Rows and pay special attention on those columns with "special" binding expressions (not supported by Infragistics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _fragmentNamesCombo_InitializeRow(object sender, RowEventArgs e)
        {
            COEDataBinder dataBinder = new COEDataBinder(e.Data);
            if (dataBinder != null)
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    if (IsAComplexBindingExp(cell.Key))
                    {
                        //Temp try/catch until changes in DB to retunr the strucuture inside the fragment object.
                        cell.Value = dataBinder.RetrieveProperty(cell.Key);
                        //try
                        //{
                        //    cell.Value = dataBinder.RetrieveProperty(cell.Key);
                        //}
                        //catch (Exception) { }
                    }
                }
            }
        }

        /// <summary>
        /// Check for complex binding expression
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsAComplexBindingExp(string key)
        {
            return key.Contains(".");
        }

        private void CreateControlHierarchy()
        {
            //Grid costmetic Settings
            this.GridDefaultSettings();
            if (this.FindControl(this._grid.ID) == null)
            {
                if (this._grid.Rows.Count == 0)
                    this._grid.Rows.Add();
                this.Controls.Add(this._grid);
            }
            if (this.FindControl(_fragmentTypesCombo.ID) == null)
                this.Controls.Add(_fragmentTypesCombo);
            if (this.FindControl(_fragmentNamesCombo.ID) == null)
                this.Controls.Add(_fragmentNamesCombo);
        }

        /// <summary>
        /// Default settings of the shown grid. Most of them can be changed by the Config xml
        /// </summary>
        private void GridDefaultSettings()
        {
            this._grid.EnableViewState = _fragmentNamesCombo.EnableViewState = _fragmentTypesCombo.EnableViewState = true;

            this._grid.DisplayLayout.CellClickActionDefault = CellClickAction.Edit;
            //this._grid.DisplayLayout.AllowDeleteDefault = AllowDelete.Yes;
            this._grid.DisplayLayout.AllowAddNewDefault = AllowAddNew.Yes;
            this._grid.DisplayLayout.AllowUpdateDefault = AllowUpdate.Yes;
            this._grid.DisplayLayout.EditCellStyleDefault.BorderColor = System.Drawing.Color.Gray;
            this._grid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;
            this._grid.DisplayLayout.RowHeightDefault = new Unit("22px");
            this._grid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
            this._grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.OnServer;
            this._grid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.SortMulti;
            this._grid.DisplayLayout.BorderCollapseDefault = BorderCollapse.Collapse; 
            this._grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            this._grid.DisplayLayout.AllowRowNumberingDefault = RowNumbering.ByBandLevel;
            this._grid.DisplayLayout.EnableClientSideRenumbering = true;
            this._grid.DisplayLayout.Name = this._grid.ID;
                                
            this._grid.DisplayLayout.FilterOptionsDefault.AllString = "(All)";
            this._grid.DisplayLayout.FilterOptionsDefault.NonEmptyString = "(NonEmpty)";
            this._grid.DisplayLayout.FilterOptionsDefault.EmptyString = "(Empty)";

            this._grid.DisplayLayout.ClientSideEvents.BeforeEnterEditModeHandler = "AddFragments_BeforeEnterEditModeHandler";
            this._grid.DisplayLayout.ClientSideEvents.AfterExitEditModeHandler = "AddFragments_BeforeExitEnterEditModeHandler";            
            this._grid.DisplayLayout.ClientSideEvents.AfterCellUpdateHandler = "CleanCells_AfterCellUpdateHandler";

            /* TODO Review settins below to remove them or not*/
            this._grid.DisplayLayout.GridLinesDefault = UltraGridLines.Both;
            this._grid.DisplayLayout.AddNewBox.ButtonStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
            this._grid.DisplayLayout.GroupByBox.Style.BorderColor = System.Drawing.Color.Gray;
            this._grid.DisplayLayout.GroupByBox.Style.BackColor = System.Drawing.Color.Blue;
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.COEReadOnly == COEEditControl.ReadOnly)
            {
                this.DisableEditFeatures();
                this.Enabled = false;
                foreach (UltraGridRow uRow in this.Grid.Rows)
                {
                    uRow.Activated = false;
                    uRow.Selected = false;
                    foreach (UltraGridCell uCell in uRow.Cells)
                    {
                        uCell.AllowEditing = AllowEditing.No;
                        uCell.Activated = false;
                    }
                }

            }
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

        protected override void OnPreRender(EventArgs e)
        {

            string addIdToGridsArray = @"
            if(typeof(gridsArray) != 'undefined')
                gridsArray[gridsArray.length] = '" + this.Grid.ClientID + "';";

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID + "AddGridId", addIdToGridsArray, true);

            string clearWebGrids = @"
            function clearWebGrids()
            {            
	            for(i = 0; i < gridsArray.length; i++)
	            {
		            var grid = igtbl_getGridById(gridsArray[i]);
                    for(j = 0; j < grid.Rows.length; j++)
			            {
				            var row = grid.Rows.getRow(j);            
				            var index = 2;
				            var cel = row.getCell(index);
				            while(cel != null)
				            {
					            cel.setValue('');
					            cel=row.getCell(index++);
				            }            
                        }
	            }
            }";


            StringBuilder filteringJS = new StringBuilder();
            filteringJS.Append("<script>");
            //Here the filtering is done 
            filteringJS.Append("function AddFragments_BeforeEnterEditModeHandler(gridName, cellId){");
            filteringJS.Append("var NamesCombo = igcmbo_getComboById(\"" + _fragmentNamesCombo.ClientID + "\");");
            filteringJS.Append("var cell=igtbl_getCellById(cellId);");
            filteringJS.Append("var row=cell.Row;");
            filteringJS.Append("var column=cell.Column;");
            filteringJS.Append("if(column.Key==\"" + ColumnKeys.FragmentName.ToString() + "\"){");
            filteringJS.Append("var fragmentTypeId=GetFragmentTypeId(row.getCell(0).getValue(true));");
            filteringJS.Append("var comboRows = NamesCombo.getGrid().Rows;");
            filteringJS.Append("var ids = GetSelectedFragmentsId(gridName);");
            filteringJS.Append("for(j = 0; j < comboRows.length ; j++){");
            filteringJS.Append("if( (comboRows.getRow(j).getCellFromKey(\"FragmentTypeId\").getValue() == fragmentTypeId) && (ids.indexOf(comboRows.getRow(j).getCellFromKey(\"FragmentId\").getValue() + '|') < 0 )){");
            filteringJS.Append("comboRows.getRow(j).Element.style.display = 'inline';}"); //Show it
            filteringJS.Append("else{comboRows.getRow(j).Element.style.display = 'none';}"); //Hide it
            filteringJS.Append("}");//Close For
            filteringJS.Append("}");//Close If 

            filteringJS.Append("}");//Close Function
            filteringJS.Append("function GetFragmentTypeId(item) {");
            filteringJS.Append("var TypesCombo = igcmbo_getComboById(\"" + _fragmentTypesCombo.ClientID + "\");");
            filteringJS.Append("var comboRows = TypesCombo.getGrid().Rows;");
            filteringJS.Append("for(j = 0; j < comboRows.length ; j++){");
            filteringJS.Append("if(comboRows.getRow(j).getCellFromKey('Key').getValue() == item || comboRows.getRow(j).getCellFromKey('Value').getValue() == item)");
            filteringJS.Append("return comboRows.getRow(j).getCellFromKey('Key').getValue();");
            filteringJS.Append("}");
            filteringJS.Append("return item;");
            filteringJS.Append("}");

            filteringJS.Append("function GetSelectedFragmentsId(gridId){");
            filteringJS.Append("var listOfIds = '';");
            filteringJS.Append("var grid = igtbl_getGridById(gridId);");
            filteringJS.Append("for(i = 0; i < grid.Rows.length - 1; i++){");
            filteringJS.Append("var fragId = grid.Rows.getRow(i).getCellFromKey(\"FragmentName\").getValue();");
            filteringJS.Append("listOfIds = listOfIds + fragId + '|';");
            filteringJS.Append("}"); //Close FOR
            filteringJS.Append("return listOfIds;");
            filteringJS.Append("}");//Close Function

            //Show the MW and Formula in the cells after selected a Fragment.
            filteringJS.Append("function AddFragments_BeforeExitEnterEditModeHandler(gridName, cellId){");
            filteringJS.Append("var NamesCombo = igcmbo_getComboById(\"" + _fragmentNamesCombo.ClientID + "\");");
            filteringJS.Append("var cell=igtbl_getCellById(cellId);");
            filteringJS.Append("var row=cell.Row;");
            filteringJS.Append(" var column=cell.Column;");
            filteringJS.Append("if(column.Key==\"" + ColumnKeys.FragmentName.ToString() + "\"){");
            filteringJS.Append("var comboRows = NamesCombo.getGrid().Rows;");
            filteringJS.Append("var mw = comboRows.getRow(NamesCombo.getSelectedIndex()).getCellFromKey(\"MW\").getValue();");
            filteringJS.Append("var formula = comboRows.getRow(NamesCombo.getSelectedIndex()).getCellFromKey(\"Formula\").getValue();");
            filteringJS.Append("var description = comboRows.getRow(NamesCombo.getSelectedIndex()).getCellFromKey(\"Description\").getValue();");
            filteringJS.Append("var FragID = comboRows.getRow(NamesCombo.getSelectedIndex()).getCellFromKey(\"FragmentId\").getValue();");
            filteringJS.Append("if(mw) row.getCell(3).setValue(mw);");
            filteringJS.Append("if(formula) row.getCell(4).setValue(formula);");
            filteringJS.Append("if(description) row.getCell(5).setValue(description);");
            filteringJS.Append("if(FragID) row.getCell(6).setValue(FragID);");   
            filteringJS.Append("row.getCell(2).setValue('1.00');row.getCell(2).setSelected();row.getCell(2).activate();");
            filteringJS.Append("return 0;}"); //End If

            //Equivalent field validation.
            filteringJS.Append("if(cell.Column.Key==\"" + ColumnKeys.Equivalent.ToString() + "\"){");
            filteringJS.Append("var input = cell.getValue();");                                    
            filteringJS.Append("if(input == null || input == ''){");
            filteringJS.Append("input = 0.00;");
            filteringJS.Append("}");
            filteringJS.Append("if(!isNaN(input)) {");
            filteringJS.Append("if(parseFloat(input) <= 0.00)");
            filteringJS.Append("{cell.setSelected();cell.activate();alert('" + _invalidCellText + "');row.getCell(2).setValue('1.00');arguments.IsValid = false;return 1;}"); //TODO: Get text form Config
            filteringJS.Append("}");
            filteringJS.Append("else {cell.setSelected();cell.activate();alert('" + _invalidCellText + "');row.getCell(2).setValue('1.00');arguments.IsValid = false;return 1;}");
            filteringJS.Append("arguments.IsValid = true;return 0;}");

            filteringJS.Append("}");
            filteringJS.Append("</script>");

            if (!this.Page.ClientScript.IsStartupScriptRegistered("FragmentsFilter"))
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "FragmentsFilter", filteringJS.ToString());

            string clean = @"function CleanCells_AfterCellUpdateHandler(grid, cellId)
                            {
                                var cell=igtbl_getCellById(cellId);
                                var row=cell.Row;
                                var column=cell.Column;
                                if(column.Key == '" + ColumnKeys.FragmentType.ToString() + @"')
                                {
                                    row.getCell(1).setValue('');
                                    row.getCell(2).setValue('');
                                    row.getCell(3).setValue('');
                                    row.getCell(4).setValue('');
                                    row.getCell(5).setValue('');
                                }
                             }";

            if (!this.Page.ClientScript.IsStartupScriptRegistered("CleanCells"))
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "CleanCells", clean, true);


            this.SetCosmeticSettings();
            
            //In ViewOnly mode, no need to show the combos and "Add Fragments" button.
            if (this.ReadOnly)
            {
                this.DisableEditFeatures();
            }
            else
            {
                //Create empty rows as set by configuration. Just a cosmetic thing.
                if (_settings.ContainsKey(ConfigSetting.DefaultEmptyRows))
                {
                    if (this._grid.DisplayLayout.Rows.Count < (int)_settings[ConfigSetting.DefaultEmptyRows])
                    {
                        for (int i = 0; i < (int)_settings[ConfigSetting.DefaultEmptyRows]; i++)
                            this._grid.DisplayLayout.Rows.Add();
                    }
                }

                //Found that the UniqueID changes in edit mode, basically because the combos controls are inside cells.
                if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentType.ToString()))
                    this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentType.ToString()).EditorControlID = this._fragmentTypesCombo.UniqueID;
                if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentName.ToString()))
                    this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentName.ToString()).EditorControlID = this._fragmentNamesCombo.UniqueID;
            }
            
            //When this control is inside of a repeater, the ids of the EditorControl (in this case the dropdowns) change, but we have it harcoded.
            //I'm not sure why this is not changing as it's with the this.ID (Composite control id). The good thing is that is needed just one EditorControl 
            // to edit several rows (always in the same column)
            if (this.Index > 0)
            {
                this.Controls.Remove(this._fragmentNamesCombo);
                this.Controls.Remove(this._fragmentTypesCombo);
            }

            string beforeRowDeleteHandler = @"
function BeforeRowDeletedHandler(gridName, rowId) 
{
           var row = igtbl_getRowById(rowId);
           row.getCell(row.cells.length - 1).setValue('" + DataChanged.Deleted.ToString() + @"');
           row.Element.style.display = 'none';

// true cancels the deletion;           
return true; 
}
";

            if(!Page.ClientScript.IsClientScriptBlockRegistered(typeof(string), "BeforeRowDeletedHandler"))
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "BeforeRowDeletedHandler", beforeRowDeleteHandler, true);


            _grid.DisplayLayout.ClientSideEvents.BeforeRowDeletedHandler = "BeforeRowDeletedHandler";

            _deleteCell.Attributes["onClick"] = _deleteLinkButton.OnClientClick = DeleteRowScript;
            _addLinkButton.OnClientClick = "return false;";
         }

        private void DisableEditFeatures()
        {
            this._fragmentTypesCombo.Visible = false;
            this._fragmentNamesCombo.Visible = false;
            this._grid.DisplayLayout.AddNewBox.Hidden = true;
            this._grid.DisplayLayout.AllowAddNewDefault = AllowAddNew.No;

            if(this.FindControl("DeleteTable") != null)
                this.FindControl("DeleteTable").Visible = false;

            if(_grid.Columns.FromKey("RowState") != null)
                _grid.Columns.Remove(_grid.Columns.FromKey("RowState"));
        }

        private void SetCosmeticSettings()
        {
           if(this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentType.ToString()) &&  _settings.ContainsKey(ConfigSetting.TypesColTitle))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentType.ToString()).Header.Caption = (string)_settings[ConfigSetting.TypesColTitle];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentType.ToString()) && _settings.ContainsKey(ConfigSetting.TypesColWidth))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentType.ToString()).Width = (Unit)_settings[ConfigSetting.TypesColWidth];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentType.ToString()) && _settings.ContainsKey(ConfigSetting.TypesColVisible))
                if (!(bool)_settings[ConfigSetting.TypesColVisible])
                    this._grid.DisplayLayout.Bands[0].Columns.Remove(this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentType.ToString()));

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentName.ToString()) && _settings.ContainsKey(ConfigSetting.NamesColTitle))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentName.ToString()).Header.Caption = (string)_settings[ConfigSetting.NamesColTitle];
                                                        
            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentName.ToString()) && _settings.ContainsKey(ConfigSetting.NamesColWidth))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentName.ToString()).Width = (Unit)_settings[ConfigSetting.NamesColWidth];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.FragmentName.ToString()) && _settings.ContainsKey(ConfigSetting.NamesColVisible))
                if (!(bool)_settings[ConfigSetting.TypesColVisible])
                    this._grid.DisplayLayout.Bands[0].Columns.Remove(this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.FragmentName.ToString()));
                
            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.MW.ToString()) && _settings.ContainsKey(ConfigSetting.MWColTitle))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.MW.ToString()).Header.Caption = (string)_settings[ConfigSetting.MWColTitle];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.MW.ToString()) && _settings.ContainsKey(ConfigSetting.MWColWidth))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.MW.ToString()).Width = (Unit)_settings[ConfigSetting.MWColWidth];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.MW.ToString()) && _settings.ContainsKey(ConfigSetting.MWColVisible))
                if (!(bool)_settings[ConfigSetting.TypesColVisible])
                    this._grid.DisplayLayout.Bands[0].Columns.Remove(this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.MW.ToString()));

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Formula.ToString()) && _settings.ContainsKey(ConfigSetting.FormulaColTitle))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Formula.ToString()).Header.Caption = (string)_settings[ConfigSetting.FormulaColTitle];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Formula.ToString()) && _settings.ContainsKey(ConfigSetting.FormulaColWidth))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Formula.ToString()).Width = (Unit)_settings[ConfigSetting.FormulaColWidth];
            
            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Formula.ToString()) && _settings.ContainsKey(ConfigSetting.FormulaColVisible))
                if (!(bool)_settings[ConfigSetting.TypesColVisible])
                    this._grid.DisplayLayout.Bands[0].Columns.Remove(this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Formula.ToString()));

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Code.ToString()) && _settings.ContainsKey(ConfigSetting.FormulaColTitle))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Code.ToString()).Header.Caption = (string)_settings[ConfigSetting.FormulaColTitle];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Code.ToString()) && _settings.ContainsKey(ConfigSetting.CodeColWidth))
                this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Code.ToString()).Width = (Unit)_settings[ConfigSetting.CodeColWidth];

            if (this._grid.DisplayLayout.Bands[0].Columns.Exists(ColumnKeys.Code.ToString()) && _settings.ContainsKey(ConfigSetting.CodeColVisible))
                if (!(bool)_settings[ConfigSetting.CodeColVisible])
                    this._grid.DisplayLayout.Bands[0].Columns.Remove(this._grid.DisplayLayout.Bands[0].Columns.FromKey(ColumnKeys.Code.ToString()));

            if (this._fragmentNamesCombo.Columns.Exists("Structure.Value") && _settings.ContainsKey(ConfigSetting.StructureColVisible))
                if (!(bool)_settings[ConfigSetting.StructureColVisible])
                    this._fragmentNamesCombo.Columns.Remove(this._fragmentNamesCombo.Columns.FromKey("Structure.Value"));

            if (_settings.Contains(ConfigSetting.HeaderStyleCSS))
            {
                this._grid.DisplayLayout.HeaderStyleDefault.CssClass = _settings[ConfigSetting.HeaderStyleCSS].ToString();
                this._fragmentTypesCombo.DropDownLayout.HeaderStyle.CssClass = _settings[ConfigSetting.HeaderStyleCSS].ToString();
                this._fragmentNamesCombo.DropDownLayout.HeaderStyle.CssClass = _settings[ConfigSetting.HeaderStyleCSS].ToString();
            }

            if (_settings.Contains(ConfigSetting.HeaderHorizontalAlign))
                this._grid.DisplayLayout.HeaderStyleDefault.HorizontalAlign = (HorizontalAlign)_settings[ConfigSetting.HeaderHorizontalAlign];

            if (_settings.Contains(ConfigSetting.AddButtonCSS))
                this._grid.DisplayLayout.AddNewBox.ButtonStyle.CssClass = _settings[ConfigSetting.AddButtonCSS].ToString();

            if (_settings.Contains(ConfigSetting.RowAlternateStyleCSS))
            {
                this._grid.DisplayLayout.RowAlternateStyleDefault.CssClass = _settings[ConfigSetting.RowAlternateStyleCSS].ToString();
                this._fragmentTypesCombo.DropDownLayout.RowAlternateStyle.CssClass = _settings[ConfigSetting.RowAlternateStyleCSS].ToString();
                this._fragmentNamesCombo.DropDownLayout.RowAlternateStyle.CssClass = _settings[ConfigSetting.RowAlternateStyleCSS].ToString();
            }

            if (_settings.Contains(ConfigSetting.RowStyleCSS))
            {
                this._grid.DisplayLayout.RowStyleDefault.CssClass = _settings[ConfigSetting.RowStyleCSS].ToString();
                this._fragmentTypesCombo.DropDownLayout.RowStyle.CssClass = _settings[ConfigSetting.RowStyleCSS].ToString();
                this._fragmentNamesCombo.DropDownLayout.RowStyle.CssClass = _settings[ConfigSetting.RowStyleCSS].ToString();
            }

            if (_settings.Contains(ConfigSetting.SelectedRowStyleCSS))
            {
                this._grid.DisplayLayout.SelectedRowStyleDefault.CssClass = _settings[ConfigSetting.SelectedRowStyleCSS].ToString();
                this._fragmentTypesCombo.DropDownLayout.SelectedRowStyle.CssClass = _settings[ConfigSetting.SelectedRowStyleCSS].ToString();
                this._fragmentNamesCombo.DropDownLayout.SelectedRowStyle.CssClass = _settings[ConfigSetting.SelectedRowStyleCSS].ToString();
            }

        }

        #endregion

        #region ITemplate
        public class TemplatedChemDraw : ITemplate
        {
            private string _inlineData = null;
            public CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed CoeChemControl = null;
            public string InlineData
            {
                get { return _inlineData; }
                set { _inlineData = value; }
            }
            public TemplatedChemDraw()
            {
            }
            public TemplatedChemDraw(string inlineData)
            {
                _inlineData = inlineData;
            }

            #region ITemplate Members

            public void InstantiateIn(Control container)
            {
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                Infragistics.WebUI.UltraWebGrid.CellItem controlItem = (Infragistics.WebUI.UltraWebGrid.CellItem)container;
                CoeChemControl = new CambridgeSoft.COE.Framework.Controls.ChemDraw.COEChemDrawEmbed();
                CoeChemControl.ID = "ChemDraw";
                CoeChemControl.Height = controlItem.Cell.Style.Height = new Unit("100px");
                CoeChemControl.Width = controlItem.Cell.Style.Width = new Unit("100px");
                CoeChemControl.EnableViewState = true;
                CoeChemControl.ViewOnly = true;
                CoeChemControl.InlineData = (string)controlItem.Value;
                container.DataBinding += new EventHandler(container_DataBinding);
                controlItem.Controls.Add(CoeChemControl);
                _coeLog.LogEnd(methodSignature);
            }

            //Called when the cell inside has an "special" binding expression, which is handled manually. See InitializeRow of the Combo.
            void container_DataBinding(object sender, EventArgs e)
            {
                if (((CellItem)sender).HasControls())
                {
                    if (((CellItem)sender).Controls[0] is COEChemDrawEmbed && ((CellItem)sender).Cell.Value != null)
                        ((COEChemDrawEmbed)((CellItem)sender).Controls[0]).InlineData = (string)((CellItem)sender).Cell.Value;
                }
            }
            #endregion

        }
        #endregion
     }

    /// <summary>
    /// Repeater to show as many COEFragments controls as Batchcomponents. 
    /// </summary>
    /// <remarks>Implemented a DataList because repeater is not a WebControl (avoiding changes in COEFormGenerator)</remarks>
    [ParseChildren(false),
    PersistChildren(true),
    ToolboxData("<{0}:COEFragmentsRepeater runat=server></{0}:COEFragmentsRepeater>")]
    public class COEFragmentsRepeater : DataList, ICOEGenerableControl
    {
        #region Variables
        private string _data = string.Empty;
        private object _datasource;
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return this.UnBind();
        }

        public void PutData(object data)
        {
            this.DataSource = data;
            //this.EnsureDataBound();
            this.DataBind();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode id = xmlData.SelectSingleNode("//COE:Id", manager);
            if (id != null && id.InnerText.Length > 0)
                this.ID = id.InnerText;

            _data = xmlDataAsString;
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

        public COEFragmentsRepeater()
        {
            this.ID = "COEFragmentsRepeater";
        }

        public COEFragmentsRepeater(string data)
            : this()
        {
            this.LoadFromXml(data);
        }

        #region Misc Methods

        private object UnBind()
        {
            Hashtable retVal = new Hashtable();
            foreach (DataListItem control in this.Items)
            {
                if (control.HasControls())
                {
                    if (control.Controls[0] is COEFragments)
                    {
                        Control hiddenID = control.FindControl("ComponentDataHiddenField");
                        if (hiddenID != null)
                            retVal.Add("BatchComponentList[" + control.ItemIndex + "].BatchComponentFragmentList", ((ICOEGenerableControl)control.Controls[0]).GetData());
                    }
                }
            }
            return (object)retVal;
        }

        #endregion

        #region Control Events

        protected override void OnInit(EventArgs e)
        {
            this.ItemTemplate = new CompiledTemplateBuilder(new BuildTemplateMethod(this.buildItemTemplate));
            //Add COEFragment control as an item template of the datalist.
            this.ItemDataBound += new DataListItemEventHandler(COEFragmentsRepeater_ItemDataBound);
            base.OnInit(e);
        }

        private void buildItemTemplate(Control _ctrl)
        {
            IParserAccessor accessor = _ctrl;
            // Coverity Fix CID - 11827
            using(COEFragments fragments = new COEFragments(_data))
                accessor.AddParsedSubObject(fragments);
        }

        void COEFragmentsRepeater_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            //For each BatchComponent, create a COEFragmentList.
            if (e.Item.Controls[0] is COEFragments)
            {

                COEDataBinder dataBinder = new COEDataBinder(e.Item.DataItem);
                object batchComponentFragmentList = null;
                batchComponentFragmentList = dataBinder.RetrieveProperty("BatchComponentFragmentList");
                
                //Set the label text that is shown below the control.
                Label details = new Label();
                object batchID = dataBinder.RetrieveProperty("BatchID");
                object orderIndex = dataBinder.RetrieveProperty("OrderIndex");
                details.Text = string.Format("BatchID: {0} - Component: {1}", batchID.ToString(), orderIndex.ToString());
                e.Item.Controls.Add(details);
                
                ((COEFragments)e.Item.Controls[0]).Index = e.Item.ItemIndex;
                if (batchComponentFragmentList != null)
                    ((ICOEGenerableControl)e.Item.Controls[0]).PutData(batchComponentFragmentList);

                //We need to identify the COEFragment at this level, that's why its added a control to have an ID
                //For now, DisplayKey is the only way to identify a BatchComponent at any time (temp, registered, etc)
                HiddenField componentDataHiddenField = new HiddenField();
                componentDataHiddenField.ID = "ComponentDataHiddenField";
                object componentIDValue = null;
                componentIDValue = dataBinder.RetrieveProperty("DisplayKey");
                if (componentIDValue != null)
                    componentDataHiddenField.Value = (string)componentIDValue;
                e.Item.Controls.Add(componentDataHiddenField);
            }
        }

        protected override void CreateChildControls()
        {
            this.CreateControlHierarchy(true);
            base.CreateChildControls();
        }

        //protected override void Render(HtmlTextWriter writer)
        //{            
        //    base.Render(writer);
        //}

        //protected override void OnPreRender(EventArgs e)
        //{
        //    Label test = new Label();
        //    test.Text = "Some test";
        //    this.Controls.Add(test)
        //    base.OnPreRender(e);
        //}

        #endregion

    }


    /// <summary>
    /// Repeater to show as many COEFragments controls as Batchcomponents. 
    /// </summary>
    /// <remarks>Implemented a DataList because repeater is not a WebControl (avoiding changes in COEFormGenerator)</remarks>
    [ParseChildren(false),
    PersistChildren(true),
    ToolboxData("<{0}:COEBatchComponentFragmentsRepeater runat=server></{0}:COEBatchComponentFragmentsRepeater>")]
    public class COEBatchComponentFragmentsRepeater : DataList, ICOEGenerableControl
    {
        #region Variables
        private string _data = string.Empty;
        private object _datasource;
        private readonly string _hiddenControlId = "BindExpDataHiddenField";
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            return this.UnBind();
        }

        public void PutData(object data)
        {
            this.DataSource = data;
            //this.EnsureDataBound();
            this.DataBind();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlData.NameTable);
            manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);

            XmlNode id = xmlData.SelectSingleNode("//COE:Id", manager);
            if (id != null && id.InnerText.Length > 0)
                this.ID = id.InnerText;

            _data = xmlDataAsString;
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

        public COEBatchComponentFragmentsRepeater()
        {
            this.ID = "COEBatchComponentFragmentsRepeater";
        }

        #region Misc Methods

        private object UnBind()
        {
            Hashtable retVal = new Hashtable();
            foreach (DataListItem control in this.Items)
            {
                if (control.HasControls())
                {
                    if (control.Controls[0] is COEFragments)
                    {
                        Control hiddenBindingExp = control.FindControl(_hiddenControlId);
                        if (hiddenBindingExp != null && hiddenBindingExp is HiddenField)
                            retVal.Add(((HiddenField)hiddenBindingExp).Value, ((ICOEGenerableControl)control.Controls[0]).GetData());
                    }
                }
            }
            return (object)retVal;
        }

        #endregion

        #region Control Events

        protected override void OnInit(EventArgs e)
        {
            this.ItemTemplate = new CompiledTemplateBuilder(new BuildTemplateMethod(this.buildItemTemplate));
            //Add COEFragment control as an item template of the datalist.
            this.ItemDataBound += new DataListItemEventHandler(COEBatchComponentFragmentsRepeater_ItemDataBound);
            base.OnInit(e);
        }

        private void buildItemTemplate(Control _ctrl)
        {
            IParserAccessor accessor = _ctrl;
            // Coverity Fix CID - 11826
            using (COEFragmentsRepeater fragmentsRepeater = new COEFragmentsRepeater(_data))
                accessor.AddParsedSubObject(fragmentsRepeater);
        }

        void COEBatchComponentFragmentsRepeater_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.Controls[0] is COEFragmentsRepeater)
            {
                COEDataBinder dataBinder = new COEDataBinder(e.Item.DataItem);
                object batch = null;
                batch = dataBinder.RetrieveProperty("BatchComponentList");
                HiddenField bindingExpHiddenField = new HiddenField();
                bindingExpHiddenField.ID = _hiddenControlId;
                if (batch != null)
                {
                    ((ICOEGenerableControl)e.Item.Controls[0]).PutData(batch);
                    bindingExpHiddenField.Value = string.Format("Batch[{0}]", e.Item.ItemIndex);
                }
                e.Item.Controls.Add(bindingExpHiddenField);
            }
        }

        protected override void CreateChildControls()
        {
            this.CreateControlHierarchy(true);
            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        #endregion

    }
}

