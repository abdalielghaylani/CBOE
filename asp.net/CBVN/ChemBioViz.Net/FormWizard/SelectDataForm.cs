using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using System.Resources;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEHitListService;
using SpotfireIntegration.Common;
using COEServiceLib;
using Spotfire.Dxp.Framework.ApplicationModel;
using System.Data;
using System.ComponentModel;

namespace FormWizard
{
    public partial class SelectDataForm : WizardPage
    {

        #region Private Fields

        /// <summary>
        /// The list of available DataViews the user can select from.
        /// </summary>
        private IEnumerable<COEDataViewBO> _dataViewBOList;

        /// <summary>
        /// The list of available Strcuture Fields the user can select from.
        /// </summary>
        private List<ComboInfo> _structureFieldsList = new List<ComboInfo>();

        /// <summary>
        /// The currently selected DataView.
        /// </summary>
        private COEDataViewBO _dataViewBO;

        //internal for accessing it from the usercontrol.
        internal COEDataViewManagerBO _dataViewManagerBO;

        private IDictionary<int, int> _tableParents;

        /// <summary>
        /// The list of currently selected tables.
        /// </summary>
        private IList<TablePanel> _tablePanels = new List<TablePanel>();

        private FieldPanel _contextualField;

        private bool _changingDataView = false;

        private Label _cueLabel;

        /// <summary>
        /// The list of currently selected tables on Query criteria.
        /// </summary>
        private IList<TablePanel> _tablePanelsOnQC = new List<TablePanel>();

        Panel activePanel = null;
        private StructureFieldCriteria structureFieldCriteria1;

        SearchFields fieldsOrder;

        private string ldInstalltionPath = string.Empty;

        ResultsCriteria _oldResultsCriteria;
        SearchCriteria _oldSearchCriteria;

        private bool isDataViewSelectionChanged = false;

        private WaitForm _waitForm;
        Spotfire.Dxp.Application.AnalysisApplication _theAnalysisApplication;
        int iMaxRow;

        #endregion

        BackgroundWorker worker;

        #region Constructors

        public SelectDataForm()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        public SelectDataForm(Spotfire.Dxp.Application.AnalysisApplication theAnalysisApplication)
            : this()
        {
            this._theAnalysisApplication = theAnalysisApplication;
            base.PreviousButton = this.cancelButton;
            IntializeHelpSettings();
            //base.NextButton = this.nextButton;
            UpdateSearchButton();
            includeFieldsComboBox.SelectedIndex = 0;
            //SetWatermark(searchTextBox, "Search Tables and Fields");
            resultCriteriaTabPage.Text = FormWizard.Properties.Resources.RESULT_CRITERIA_TAB_DISPLAY_NAME;
            queryCriteriaTabPage.Text = FormWizard.Properties.Resources.QUERY_CRITERIA_TAB_DISPLAY_NAME;
            //set textx for clear form link lable
            clearQueryFormLinkLabel.Text = FormWizard.Properties.Resources.ClearFormLinkLabelText;
            clearResultDefinitionLinkLabel.Text = FormWizard.Properties.Resources.ClearFormLinkLabelText;

            searchCriteriaTabControl.SelectedIndex = 0;

            this.structureFieldCriteria1 = new StructureFieldCriteria(theAnalysisApplication);
            this.ultraExpandableGroupBoxPanel2.Controls.Add(this.structureFieldCriteria1);
            // 
            // structureFieldCriteria1
            // 
            this.structureFieldCriteria1.AutoSize = true;
            this.structureFieldCriteria1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.structureFieldCriteria1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structureFieldCriteria1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.structureFieldCriteria1.Location = new System.Drawing.Point(0, 0);
            this.structureFieldCriteria1.Name = "structureFieldCriteria1";
            this.structureFieldCriteria1.Size = new System.Drawing.Size(258, 490);
            this.structureFieldCriteria1.TabIndex = 0;

            COEServiceHelper coeServiceHelper = COEServiceHelper.Instance(theAnalysisApplication);
        }

        #endregion

        #region Properties

        public COEHitList coeHitList { get; set; }

        public int HitListInfoId { get; set; }

        /// <summary>
        /// Gets or sets the server framework version
        /// </summary>
        public Version ServerVersion { get; set; }

        /// <summary>
        /// Gets or sets the list of available DataViews the user can select from.
        /// </summary>
        public IEnumerable<COEDataViewBO> availableDataViews
        {
            get
            {
                return this._dataViewBOList;
            }
            set
            {
                this._dataViewBOList = value;
                this.dataViewComboBox.Items.Clear();
                foreach (COEDataViewBO dataView in this._dataViewBOList)
                {
                    this.dataViewComboBox.Items.Add(dataView);
                }
                this.dataViewComboBox.SelectedItem = null;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected DataView.
        /// </summary>
        public COEDataViewBO dataViewBO
        {
            get
            {
                return this._dataViewBO;
            }
            set
            {
                this._dataViewBO = value;
                this._changingDataView = true;
                try
                {
                    if (this._dataViewBO != null)
                    {
                        //set DV object in COEService to use in the Datalytix as a local copy
                        //this code executes when DV is selected from drop down box, from Browse dataview form and Refresh button click
                        COEServiceHelper.COEService.TheCOEDataviewBO = this._dataViewBO;
                        if (this._dataViewBO.DataViewManager == null)
                        {
                            this._dataViewBO.DataViewManager = COEServiceHelper.COEService.GetDataViewManagerBO(this._dataViewBO.COEDataView);
                            //this._dataViewBO.DataViewManager = COEDataViewManagerBO.NewManager(this._dataViewBO.COEDataView);
                        }
                        this._dataViewManagerBO = this._dataViewBO.DataViewManager;
                        try
                        {
                            this.dataViewComboBox.SelectedItem = value;
                        }
                        catch (Exception)
                        {
                            this.dataViewComboBox.SelectedItem = null;
                        }
                        this._tableParents = new Dictionary<int, int>();
                        foreach (RelationshipBO relationship in this._dataViewManagerBO.Relationships)
                        {
                            //check if the key already exists in the collection of table parents then do not add again.
                            if (!this._tableParents.ContainsKey(relationship.Child))
                            {
                                this._tableParents.Add(relationship.Child, relationship.Parent);
                            }
                        }
                    }
                    else
                    {
                        this._dataViewManagerBO = null;
                        this.dataViewComboBox.SelectedItem = null;
                        this._tableParents = null;
                    }
                }
                finally
                {
                    this._changingDataView = false;
                }
                //Refresh the treeview in user control.
                this.selectDataview1.RebuildTreeView(_dataViewManagerBO);
                //this.FillHitListCombo();
                ClearSelections();
            }
        }

        /// <summary>
        /// Gets or sets whether the user can change the selected DataView.
        /// </summary>
        public Boolean AllowChangingDataView
        {
            get
            {
                return this.dataViewComboBox.Enabled;
            }
            set
            {
                this.dataViewComboBox.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the search criteria fields collection
        /// </summary>
        public SearchFields FieldsOrder
        {
            get { return fieldsOrder; }
            set { fieldsOrder = value; }
        }


        SearchCriteria.SearchCriteriaItem BuildSearchCriteria(QueryCriteriaFieldContext queryCriteriaFiedContext, QueryFieldPanel queryFieldPanel)// QueryCriteriaOperator queryCriteriaOperator, string searchValue1, string searchValue2)
        {
            QueryCriteriaOperator queryCriteriaOperator = queryFieldPanel.QueryCriteriaOperator;
            //CID:20630
            if (queryCriteriaOperator == null)
            {
                return null;
            }
            string searchValue1 = queryFieldPanel.SearchValue1;
            string searchValue2 = queryFieldPanel.SearchValue2;
            SearchCriteria.SearchCriteriaItem searchCriteriaItem = null;
            SearchCriteria.ISearchCriteriaBase searchCriteriaBase = null;

            // get the field criteria type value
            Type searchCriteriaType = queryCriteriaFiedContext.fieldCriteriaType;

            //initialize a default instance of the search criteria object
            searchCriteriaBase = (SearchCriteria.ISearchCriteriaBase)searchCriteriaType.GetConstructor(Type.EmptyTypes).Invoke(null);

            if (searchCriteriaBase != null)
            {
                if (searchCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria))
                {
                    //set implementation value to default CsCartridge
                    searchCriteriaType.GetProperty("Implementation").SetValue(searchCriteriaBase, "CsCartridge", null);
                    if (queryFieldPanel.ValidateControls())
                    {
                        searchCriteriaBase.Value = queryFieldPanel.MolWeightSearchValue;
                    }
                }
                else if (searchCriteriaType == typeof(SearchCriteria.CSFormulaCriteria))
                {
                    searchCriteriaType.GetProperty("Implementation").SetValue(searchCriteriaBase, "CsCartridge", null);
                    switch (queryCriteriaOperator.ToString())
                    {
                        case "equal":
                            searchCriteriaBase.Value = searchValue1.Trim().StartsWith("=") ? searchValue1.Trim() : searchValue1.PadLeft(searchValue1.Length + 1, '=');
                            break;
                        case "contains":
                            searchCriteriaBase.Value = searchValue1;
                            break;
                    }
                }
                else
                {
                    //set the operator property value of search criteria object
                    searchCriteriaType.GetProperty("Operator").SetValue(searchCriteriaBase, queryCriteriaOperator.coeOperator, null);
                    //set other properties of the search criteria object from form values
                    StringBuilder searchValue = new StringBuilder();
                    searchValue.Append(searchValue1);
                    if (!string.IsNullOrEmpty(searchValue2))
                    {
                        searchValue.Append(" - " + searchValue2);
                    }
                    searchCriteriaBase.Value = searchValue.ToString();
                }

                //access table and field id from field context to get the correct id values
                searchCriteriaItem = new SearchCriteria.SearchCriteriaItem();
                searchCriteriaItem.TableId = queryCriteriaFiedContext.tableId;
                searchCriteriaItem.FieldId = queryCriteriaFiedContext.fieldId;
                searchCriteriaItem.Criterium = searchCriteriaBase;
                searchCriteriaItem.ToString();
            }

            return searchCriteriaItem;
        }

        /// <summary>
        /// Gets or sets the search criteria object represented by the selected query criteria fields on query form
        /// </summary>
        public SearchCriteria SearchCriteria
        {
            get
            {
                SearchCriteria searchCriteria = new SearchCriteria();
                fieldsOrder = new SearchFields();
                foreach (TablePanel tablePanel in this.selectedTablesOnQC)
                {
                    foreach (FieldPanel fieldPanel in tablePanel.fields)
                    {
                        //assign to local variable to avoid typecasting
                        QueryFieldPanel queryFieldPanel = (QueryFieldPanel)fieldPanel;
                        SearchCriteria.SearchCriteriaItem searchCriteriaItem = new SearchCriteria.SearchCriteriaItem();
                        QueryCriteriaFieldContext queryCriteriaFiedContext = (QueryCriteriaFieldContext)fieldPanel.fieldContext;
                        SearchCriteria.ISearchCriteriaBase searchCriteriaBase = queryCriteriaFiedContext.fieldQueryCriteria;
                        //CID:20629
                        string strQueryCriteriaOperator = string.Empty;
                        QueryCriteriaOperator queryCriteriaOperator = queryFieldPanel.QueryCriteriaOperator;
                        if (queryCriteriaOperator != null)
                        {
                            strQueryCriteriaOperator = queryCriteriaOperator.coeOperator.ToString();
                        }

                        //add search criteria field to collection for maintaining fields order on query tab
                        fieldsOrder.AddSearchField(tablePanel.tableBO.ID, fieldPanel.fieldBO.ID, queryCriteriaFiedContext.tableId, fieldPanel.fieldContext.fieldCriteriaType, queryFieldPanel.MolWeightSearchValue, strQueryCriteriaOperator);

                        if (!string.IsNullOrEmpty(queryFieldPanel.SearchValue1) && queryFieldPanel.QueryCriteriaOperator != null)
                        {
                            searchCriteriaItem = BuildSearchCriteria(queryCriteriaFiedContext, queryFieldPanel); // queryFieldPanel.QueryCriteriaOperator, queryFieldPanel.SearchValue1, queryFieldPanel.SearchValue2);

                            if (searchCriteriaItem != null)
                            {
                                //If field having the lookup then it will find the lookup tables and then search criteria will be generated accordingly.
                                searchCriteriaItem.SearchLookupByID = false;// we are not searching the lookup field by its Id
                                searchCriteria.Items.Add(searchCriteriaItem);
                            }
                        }
                    }
                }

                #region Remembering all the structure fields which are in Structure Combo box

                foreach (ComboInfo item in _structureFieldsList)
                {
                    ComboInfo cmbInfo = item.Key as ComboInfo;
                    if (cmbInfo != null)
                    {
                        fieldsOrder.AddSearchField(Convert.ToInt32(cmbInfo.Key.ToString()), Convert.ToInt32(cmbInfo.Value.ToString()), Convert.ToInt32(cmbInfo.Key.ToString()), typeof(StructureFieldCriteria), string.Empty, string.Empty);
                    }
                }

                #endregion

                if (structureFieldCriteria1.SFCriteria != null)
                {
                    //if structure criteria is specified then create a new structure search criteria object and add to collection
                    ComboInfo _comboInfo = structureFieldCriteria1.SelectedStructureField;
                    SearchCriteria.SearchCriteriaItem searchCriteriaItem = new SearchCriteria.SearchCriteriaItem();
                    searchCriteriaItem.TableId = Convert.ToInt32(Convert.ToString((_comboInfo.Key as ComboInfo).Key));
                    searchCriteriaItem.FieldId = Convert.ToInt32(Convert.ToString((_comboInfo.Key as ComboInfo).Value));
                    //If field having the lookup then it will find the lookup tables and then search criteria will be generated accordingly.
                    searchCriteriaItem.SearchLookupByID = false;
                    searchCriteriaItem.Criterium = structureFieldCriteria1.SFCriteria;

                    searchCriteria.Items.Add(searchCriteriaItem);
                }
                return searchCriteria;
            }
            set
            {
                // Set the form state based on an existing ResultsCriteria.
                ClearSelections(true, this.selectedQCFieldsPanel);

                //store old search criteria
                _oldSearchCriteria = value;
                TablePanel tablePanel = null;

                #region for search criteria item is Structure criteria

                if (fieldsOrder != null && fieldsOrder.SearchFieldCollection.Count > 0)
                {
                    List<SearchField> _structureFieldList = fieldsOrder.SearchFieldCollection.FindAll(e => e.FieldCriteriaType.Trim().Equals(typeof(StructureFieldCriteria).ToString().Trim()));

                    foreach (SearchField searchField in _structureFieldList)
                    {
                        int fieldId = 0;
                        TableBO tableBO = null;

                        fieldId = searchField.FieldId;
                        //get the owner tableBo of the field
                        tableBO = this._dataViewManagerBO.Tables.GetTableByFieldId(fieldId);

                        if (tableBO == null)
                        {
                            continue;
                        }

                        //get FieldBo object of the field
                        FieldBO fieldBO = tableBO.Fields.GetField(fieldId);
                        FieldBO lookupField = this._dataViewManagerBO.Tables.GetField(fieldBO.LookupDisplayFieldId);

                        if ((fieldBO.LookupDisplayFieldId <= 0 && (fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))
                            || (fieldBO.LookupDisplayFieldId > 0 && lookupField != null && (lookupField.Alias == "Structure" ||
                            lookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)))
                        {
                            if (fieldBO.Visible)
                            {
                                //Adding structure field if not exist in _structureFieldsList list but exists in Search criteria
                                bool _fieldsNeedsToADD = true;
                                ComboInfo cmbItem = new ComboInfo() { Key = tableBO.ID, Value = fieldBO.ID };
                                foreach (ComboInfo item in _structureFieldsList)
                                {
                                    ComboInfo cmbInfo = item.Key as ComboInfo;
                                    if (cmbInfo != null)
                                    {
                                        if ((cmbInfo.Key.Equals(cmbItem.Key)) && cmbInfo.Value.Equals(cmbItem.Value))
                                        {
                                            _fieldsNeedsToADD = false;
                                            break;
                                        }
                                    }
                                }

                                if (_fieldsNeedsToADD)
                                {
                                    ComboInfo structField = new ComboInfo() { Key = cmbItem, Value = (!string.IsNullOrEmpty(fieldBO.Alias) ? string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldBO.Alias, (!string.IsNullOrEmpty(tableBO.Alias) ? tableBO.Alias : tableBO.Name)) : string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldBO.Name, (!string.IsNullOrEmpty(tableBO.Alias) ? tableBO.Alias : tableBO.Name))) };
                                    _structureFieldsList.Add(structField);
                                }
                            }
                        }
                    }
                }

                //check if structure search item is present then set value in structure searc panel
                if (_structureFieldsList != null && _structureFieldsList.Count > 0)
                {
                    this.structureFieldCriteria1.FillAvailableStructureFields(_structureFieldsList);
                }

                foreach (SearchCriteria.SearchCriteriaItem searchCriteriaItem in value.Items)
                {
                    FieldBO fieldBO = null;
                    TableBO tableBO = this._dataViewManagerBO.Tables.GetTable(searchCriteriaItem.TableId);
                    if (tableBO == null)
                    {
                        continue;
                    }
                    FieldBO theFieldBO = tableBO.Fields.FirstOrDefault(e => e.ID == searchCriteriaItem.FieldId);
                    if (theFieldBO == null)
                    {
                        continue;
                    }
                    else if (!theFieldBO.Visible)
                    {
                        continue;
                    }

                    if (searchCriteriaItem.Criterium is SearchCriteria.StructureCriteria)
                    {
                        fieldBO = tableBO.Fields.GetField(searchCriteriaItem.FieldId);
                        if (fieldBO != null)
                        {
                            ComboInfo _comboInfo = new ComboInfo()
                            {
                                Key = (new ComboInfo() { Key = searchCriteriaItem.TableId, Value = fieldBO.ID }),
                                Value = (!string.IsNullOrEmpty(fieldBO.Alias) ? string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldBO.Alias, (!string.IsNullOrEmpty(tableBO.Alias) ? tableBO.Alias : tableBO.Name)) : string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldBO.Name, (!string.IsNullOrEmpty(tableBO.Alias) ? tableBO.Alias : tableBO.Name)))
                            };

                            structureFieldCriteria1.SelectedStructureField = _comboInfo;
                            structureFieldCriteria1.SFCriteria = searchCriteriaItem.Criterium;
                            structureFieldCriteria1.SetStructureCriteria();
                            ultraExpandableGroupBox2.Expanded = true;
                            break;
                        }
                    }
                }

                #endregion


                if (fieldsOrder != null && fieldsOrder.SearchFieldCollection.Count > 0)
                {
                    //loop over the search criteria field s collection and add to the QueryTab
                    foreach (SearchField searchField in fieldsOrder.SearchFieldCollection)
                    {
                        if (searchField.FieldCriteriaType == typeof(StructureFieldCriteria).ToString())
                        {
                            continue;
                        }

                        int fieldId = 0;
                        TableBO ownerTableBO = null;
                        //get tableBo object with parent table id
                        TableBO tableBo = this._dataViewManagerBO.Tables.GetTable(searchField.ParentTableId);
                        if (tableBo == null)
                        {
                            continue;
                        }

                        fieldId = searchField.FieldId;
                        //get the owner tableBo of the field
                        ownerTableBO = this._dataViewManagerBO.Tables.GetTableByFieldId(fieldId);

                        if (ownerTableBO == null)
                        {
                            continue;
                        }

                        FieldBO theFieldBO = ownerTableBO.Fields.FirstOrDefault(e => e.ID == fieldId);
                        if (theFieldBO == null)
                        {
                            continue;
                        }
                        else if (!theFieldBO.Visible)
                        {
                            continue;
                        }

                        //add table panel on the query tab
                        tablePanel = AppendTableInternal(tableBo, TableSelectionMethod.None, this.selectedQCFieldsPanel);
                        if (tablePanel == null)
                        {
                            continue;
                        }

                        //get FieldBo object of the field
                        FieldBO fieldBo = ownerTableBO.Fields.GetField(fieldId);
                        FieldBO lookupField = this._dataViewManagerBO.Tables.GetField(fieldBo.LookupDisplayFieldId);

                        //Find the SearchCriteria item that matches field id and table id fro search criteria item collection
                        SearchCriteria.SearchCriteriaItem searchCriteriaItem = (SearchCriteria.SearchCriteriaItem)value.Items.FirstOrDefault(delegate(SearchCriteria.SearchExpression itm)
                        {
                            //CID:20215, 20644
                            SearchCriteria.SearchCriteriaItem searchItem = itm as SearchCriteria.SearchCriteriaItem;
                            if (searchItem == null)
                            {
                                return false;
                            }
                            if (searchItem.FieldId == fieldId && searchItem.TableId == ownerTableBO.ID && searchItem.Criterium.GetType().ToString() == searchField.FieldCriteriaType)
                            {
                                return true;
                            }
                            return false;
                        });

                        FieldPanel fieldPanel = null;

                        //if search criteria item not found then add default entry for field context otherwise add the new field context and set the properties
                        if (searchCriteriaItem == null)
                        {
                            FieldContext fieldContext = null;
                            if (searchField.FieldCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria).ToString())
                            {
                                fieldContext = new QueryCriteriaFieldContext(ownerTableBO, fieldBo, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.MolWeight));
                            }
                            else if (searchField.FieldCriteriaType == typeof(SearchCriteria.CSFormulaCriteria).ToString())
                            {
                                fieldContext = new QueryCriteriaFieldContext(ownerTableBO, fieldBo, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.Formula));
                            }
                            else
                            {
                                fieldContext = new QueryCriteriaFieldContext(ownerTableBO, fieldBo, _dataViewManagerBO.DataViewId, lookupField);
                            }
                            fieldPanel = tablePanel.InsertField(fieldContext, tablePanel.fields.Count);
                        }
                        else
                        {
                            //for other search criteria types
                            SearchCriteria.ISearchCriteriaBase criteria = searchCriteriaItem.Criterium;
                            SearchCriteria.COEOperators coeOperator = CambridgeSoft.COE.Framework.Common.SearchCriteria.COEOperators.EQUAL;

                            string searchCriteriaValue = string.Empty;

                            FieldContext fieldContext = null;
                            if (searchField.FieldCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria).ToString())
                            {
                                fieldContext = new QueryCriteriaFieldContext(ownerTableBO, fieldBo, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.MolWeight));
                            }
                            else if (searchField.FieldCriteriaType == typeof(SearchCriteria.CSFormulaCriteria).ToString())
                            {
                                fieldContext = new QueryCriteriaFieldContext(ownerTableBO, fieldBo, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.Formula));
                            }
                            else
                            {
                                fieldContext = new QueryCriteriaFieldContext(ownerTableBO, fieldBo, _dataViewManagerBO.DataViewId, lookupField, criteria);
                            }

                            fieldPanel = tablePanel.InsertField(fieldContext, tablePanel.fields.Count);

                            if (fieldPanel != null)
                            {
                                string lkpStruct = string.Empty;
                                if (fieldContext.LookupField != null)
                                    lkpStruct = fieldContext.fieldBO.Alias + "-";
                                fieldPanel.IsNew = false;

                                if (fieldBo.Alias != null && !fieldPanel.Alias.Equals(lkpStruct + "Mol Weight") && !fieldPanel.Alias.Equals(lkpStruct + "Formula"))
                                {
                                    fieldPanel.Alias = fieldBo.Alias;
                                }
                                searchCriteriaValue = criteria.Value;
                            }

                            //check for the type of the search criteria object to get the operator values
                            if (criteria is SearchCriteria.CSMolWeightCriteria)
                            {
                                coeOperator = CambridgeSoft.COE.Framework.Common.SearchCriteria.COEOperators.EQUAL;

                                if (fieldPanel != null)
                                {
                                    //AssignFieldPanelValues(criteria, fieldPanel, coeOperator, searchCriteriaValue);
                                    AssignMolWeightFieldPanelValues(criteria, fieldPanel, searchCriteriaValue, searchField);
                                }
                                continue;
                            }
                            else if (criteria is SearchCriteria.CSFormulaCriteria)
                            {
                                Enum.TryParse<SearchCriteria.COEOperators>(searchField.CoeOperator, out coeOperator);

                                if (fieldPanel != null)
                                {
                                    AssignFieldPanelValues(criteria, fieldPanel, coeOperator, searchCriteriaValue);
                                }
                                continue;
                            }
                            else
                            {
                                if (criteria is SearchCriteria.TextCriteria)
                                {
                                    coeOperator = ((SearchCriteria.TextCriteria)criteria).Operator;
                                }
                                else if (criteria is SearchCriteria.NumericalCriteria)
                                {
                                    coeOperator = ((SearchCriteria.NumericalCriteria)criteria).Operator;
                                }
                                else if (criteria is SearchCriteria.DateCriteria)
                                {
                                    coeOperator = ((SearchCriteria.DateCriteria)criteria).Operator;
                                }

                                if (fieldPanel != null)
                                {
                                    AssignFieldPanelValues(criteria, fieldPanel, coeOperator, searchCriteriaValue);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //populate defualt fields when the form is opened for first time after coming from CBVN
                    PopulateDefaultSearchFields();
                }
            }
        }

        /// <summary>
        /// Method to populate search criteria fields based on table selection method when launched for first time after CBVN connection
        /// </summary>
        private void PopulateDefaultSearchFields()
        {
            PrependTable(this._dataViewManagerBO.Tables.GetTable(this._dataViewManagerBO.BaseTableId),
                        tableSelectionMethod, this.selectedQCFieldsPanel);
        }

        /// <summary>
        /// Assign mol weight while retriving the form
        /// </summary>
        /// <param name="criteria">Search criteria object</param>
        /// <param name="fieldPanel">Field panel object</param>
        /// <param name="searchCriteriaValue">search criteria value</param>
        /// <param name="molWtSearchField">mol weight search field</param>
        private void AssignMolWeightFieldPanelValues(SearchCriteria.ISearchCriteriaBase criteria, FieldPanel fieldPanel, string searchCriteriaValue, SearchField molWtSearchField)
        {
            QueryFieldPanel queryFieldPanel = (QueryFieldPanel)fieldPanel;
            SearchCriteria.COEOperators coeOperator;
            QueryCriteriaOperator queryCriteriaOperator = null;
            if (criteria is SearchCriteria.CSMolWeightCriteria)
            {
                if (Enum.TryParse<SearchCriteria.COEOperators>(molWtSearchField.CoeOperator, out coeOperator))
                {
                    queryCriteriaOperator = GetQueryCriteriaOperators<MolWeightQueryCriteriaOperator>(coeOperator);
                    queryFieldPanel.AssignMolWeightCriteriaDetails(molWtSearchField.FieldValue);
                    queryFieldPanel.QueryCriteriaOperator = queryCriteriaOperator;
                }
            }
        }

        private void AssignFieldPanelValues(SearchCriteria.ISearchCriteriaBase criteria, FieldPanel fieldPanel, SearchCriteria.COEOperators coeOperator, string searchCriteriaValue)
        {
            QueryFieldPanel queryFieldPanel = (QueryFieldPanel)fieldPanel;
            QueryCriteriaOperator queryCriteriaOperator = null;
            if (criteria is SearchCriteria.CSFormulaCriteria)
            {
                queryCriteriaOperator = GetQueryCriteriaOperators<FormulaQueryCriteriaOperator>(coeOperator);
                if (coeOperator == CambridgeSoft.COE.Framework.Common.SearchCriteria.COEOperators.EQUAL)
                {
                    searchCriteriaValue = searchCriteriaValue.Trim('=');
                }
                queryFieldPanel.SearchValue1 = searchCriteriaValue;
            }
            else
            {
                queryCriteriaOperator = GetQueryCriteriaOperators<QueryCriteriaOperator>(coeOperator);

                //get the criteria values in array and assign to individual value
                string[] searchValues = ParseSearchCriteriaValues(searchCriteriaValue);
                if (searchValues.Length == 2)
                {
                    queryFieldPanel.SearchValue1 = searchValues[0];
                    queryFieldPanel.SearchValue2 = searchValues[1];
                }
                else
                {
                    queryFieldPanel.SearchValue1 = searchValues[0];
                }
            }
            queryFieldPanel.QueryCriteriaOperator = queryCriteriaOperator;
        }

        private QueryCriteriaOperator GetQueryCriteriaOperators<T>(SearchCriteria.COEOperators coeOperator)
        {
            var molWeightQueryCriteriaOperators = QueryCriteriaOperators.Where(p => p.GetType() == typeof(T));
            foreach (var queryCriteriaOperator in molWeightQueryCriteriaOperators)
            {
                if (queryCriteriaOperator.coeOperator == coeOperator)
                {
                    return queryCriteriaOperator;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse the search value string for Numeric and Date criteria to check if range values are specified, then separate those in two values
        /// </summary>
        /// <param name="searchValue">input value present in search criteria containing range value separator</param>
        /// <returns>returns separate values of search criteria in a string array</returns>
        string[] ParseSearchCriteriaValues(string searchValue)
        {
            //replace the range separator with @ symbol to avoid split function separating values with space in case of date-time values.
            searchValue = searchValue.Replace(" - ", "@");
            string[] searchValues = searchValue.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            return searchValues;
        }

        /// <summary>
        /// Gets or sets the ResultsCriteria represented by the table and field selections.
        /// </summary>
        public ResultsCriteria resultsCriteria
        {
            get
            {
                // Build a ResultsCriteria from the wizard state.
                ResultsCriteria resultsCriteria = new ResultsCriteria();
                foreach (TablePanel tablePanel in this.selectedTables)
                {
                    ResultsCriteria.ResultsCriteriaTable table = new ResultsCriteria.ResultsCriteriaTable(tablePanel.tableBO.ID);
                    int sortIndex = 1;
                    foreach (FieldPanel fieldPanel in tablePanel.fields)
                    {
                        ResultsFieldPanel resultsFieldPanel = (ResultsFieldPanel)fieldPanel;
                        ResultsCriteria.IResultsCriteriaBase criteria = ((ResultCriteriaFieldContext)fieldPanel.fieldContext).fieldResultsCriteria;
                        if (criteria == null)
                        {
                            Type criteriaType = ((ResultCriteriaFieldContext)fieldPanel.fieldContext).fieldCriteriaType;
                            criteria = (ResultsCriteria.IResultsCriteriaBase)criteriaType.GetConstructor(Type.EmptyTypes).Invoke(null);
                            criteriaType.GetProperty("Id").SetValue(criteria, fieldPanel.fieldBO.ID, null);
                        }

                        if (resultsFieldPanel.Aggregation != null)
                        {
                            ResultsCriteria.AggregateFunction aggregate = new ResultsCriteria.AggregateFunction();
                            aggregate.Parameters = new List<ResultsCriteria.IResultsCriteriaBase>(1);
                            aggregate.Parameters.Add(criteria);
                            criteria = aggregate;
                            aggregate.FunctionName = resultsFieldPanel.Aggregation.Function;
                        }
                        criteria.Alias = resultsFieldPanel.Alias;
                        if (resultsFieldPanel.SortDirection.HasValue)
                        {
                            criteria.Direction = resultsFieldPanel.SortDirection.Value;
                            criteria.OrderById = sortIndex++;
                        }
                        table.Criterias.Add(criteria);
                    }
                    resultsCriteria.Add(table);
                }
                return resultsCriteria;
            }
            set
            {
                // Set the form state based on an existing ResultsCriteria.
                ClearSelections(true, this.selectedFieldsPanel);
                //store old results criteria
                _oldResultsCriteria = value;

                foreach (ResultsCriteria.ResultsCriteriaTable table in value.Tables)
                {
                    TableBO tableBO = this._dataViewManagerBO.Tables.GetTable(table.Id);
                    if (tableBO == null)
                    {
                        continue;
                    }


                    //temporary TableSelectionMethod is set to Default fields but needs to be read from document properties
                    TablePanel tablePanel = AppendTableInternal(tableBO, TableSelectionMethod.None, this.selectedFieldsPanel);
                    if (tablePanel == null)
                    {
                        continue;
                    }
                    foreach (ResultsCriteria.IResultsCriteriaBase criteria in table.Criterias)
                    {
                        int fieldId = 0;
                        TableBO ownerTableBO = null;
                        AggregationFunction aggregationFunction = null;
                        ResultsCriteria.IResultsCriteriaBase deaggregatedCriteria = criteria;
                        if (criteria is ResultsCriteria.Field)
                        {
                            fieldId = ((ResultsCriteria.Field)criteria).Id;
                            ownerTableBO = tableBO;
                        }
                        else if (criteria is ResultsCriteria.MolWeight)
                        {
                            fieldId = ((ResultsCriteria.MolWeight)criteria).Id;
                            ownerTableBO = tableBO;
                        }
                        else if (criteria is ResultsCriteria.Formula)
                        {
                            fieldId = ((ResultsCriteria.Formula)criteria).Id;
                            ownerTableBO = tableBO;
                        }
                        else if (criteria is ResultsCriteria.HighlightedStructure)
                        {
                            fieldId = ((ResultsCriteria.HighlightedStructure)criteria).Id;
                            ownerTableBO = tableBO;
                        }
                        else if (criteria is ResultsCriteria.AggregateFunction)
                        {
                            deaggregatedCriteria = ((ResultsCriteria.AggregateFunction)criteria).Parameters[0];
                            ResultsCriteria.Field field = deaggregatedCriteria as ResultsCriteria.Field;
                            if (field != null)
                            {
                                fieldId = field.Id;
                                ownerTableBO = this._dataViewManagerBO.Tables.GetTableByFieldId(fieldId);
                            }
                            string aggregation = ((ResultsCriteria.AggregateFunction)criteria).FunctionName.ToUpper();
                            foreach (AggregationFunction aggFunc in AggregationFunctions)
                            {
                                if (aggregation.Equals(aggFunc.Function, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    aggregationFunction = aggFunc;
                                    break;
                                }
                            }
                        }
                        if (ownerTableBO == null)
                        {
                            continue;
                        }
                        FieldBO fieldBO = ownerTableBO.Fields.GetField(fieldId);
                        if (fieldBO == null)
                        {
                            continue;
                        }
                        else if (!fieldBO.Visible)
                        {
                            continue;
                        }
                        FieldBO lookupField = null;
                        if (fieldBO.LookupDisplayFieldId > 0 && _dataViewManagerBO != null)
                        {
                            lookupField = _dataViewManagerBO.Tables.GetField(fieldBO.LookupDisplayFieldId);
                        }
                        FieldContext fieldContext = new ResultCriteriaFieldContext(ownerTableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, deaggregatedCriteria);
                        FieldPanel fieldPanel = tablePanel.InsertField(fieldContext, tablePanel.fields.Count);
                        fieldPanel.IsNew = false;
                        if (!string.IsNullOrEmpty(criteria.Alias))
                        {
                            fieldPanel.Alias = criteria.Alias;//To remember result criteria field alias names always
                        }
                        if (criteria.OrderById > 0)
                        {
                            ((ResultsFieldPanel)fieldPanel).SortDirection = criteria.Direction;
                        }
                        if (aggregationFunction != null)
                        {
                            ((ResultsFieldPanel)fieldPanel).Aggregation = aggregationFunction;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of the tables currently selected in the ResultsCriteria.
        /// </summary>
        private IEnumerable<TablePanel> selectedTables
        {
            get
            {
                return this._tablePanels;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected TableSelectionMethod for the form.
        /// </summary>
        public TableSelectionMethod tableSelectionMethod
        {
            get
            {
                return (TableSelectionMethod)includeFieldsComboBox.SelectedIndex;
            }
            set
            {
                includeFieldsComboBox.SelectedIndex = (int)value;
            }
        }

        public bool SearchOverPrevHitList { get; set; }

        /// <summary>
        /// To store and get the hitlist result row count for set the hitlist label.
        /// </summary>
        public int CBVNHitListRowCount { get; set; }

        public string CBVNFormName { get; set; }

        protected override bool IsValid
        {
            get
            {
                if (!base.IsValid)
                {
                    return false;
                }

                this.errorProvider1.Clear();
                //check result criteria has at least one field selected for display
                bool result = ValidateResultCriterias();

                if (result)
                {
                    result = ValidateQueryCriterias();

                    if (result)
                    {
                        // CSBR-153065: Check for conflicts between base table column names and child table names.
                        TablePanel baseTable = this._tablePanels[0];
                        HashSet<string> baseTableColumnNames = new HashSet<string>(
                            from f in baseTable.fields
                            select f.Alias);
                        baseTableColumnNames.IntersectWith(
                            from t in this.selectedTables.Skip(1)
                            select t.tableBO.Alias);
                        if (baseTableColumnNames.Count > 0)
                        {
                            MessageBox.Show(String.Format(FormWizard.Properties.Resources.SearchCriteria_Field_Validation, baseTableColumnNames.First()),
                                            FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            result = false;
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Validates the result criteria fields
        /// </summary>
        /// <returns>returns boolean value</returns>
        private bool ValidateResultCriterias()
        {
            bool result = false;
            if (this.selectedTables.Count() == 0)
            {
                result = false;
            }
            else
            {
                foreach (TablePanel tablePanel in this.selectedTables)
                {
                    if (tablePanel.fields.Count > 0)
                    {
                        result = true;
                        break;
                    }
                }
            }

            if (!result)
            {
                MessageBox.Show(FormWizard.Properties.Resources.ResultCriteria_Validation, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        /// <summary>
        /// Validates the query criteria has corect values in input controls
        /// </summary>
        /// <returns>returns boolean value for valid query criterias</returns>
        bool ValidateQueryCriterias()
        {
            bool result = true;
            foreach (TablePanel tablePanel in this.selectedTablesOnQC)
            {
                foreach (FieldPanel fieldPanel in tablePanel.fields)
                {
                    if (!fieldPanel.ValidateControls())
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets an enumeration of the tables currently selected in the QueryCriteria.
        /// </summary>
        private IEnumerable<TablePanel> selectedTablesOnQC
        {
            get
            {
                return this._tablePanelsOnQC;
            }
        }

        /// <summary>
        /// Gets the current active field panel
        /// </summary>
        private Panel ActivePanel
        {
            get
            {
                if (activePanel == null)
                {
                    searchCriteriaTabControl.SelectedIndex = 0;
                    activePanel = selectedQCFieldsPanel;
                }
                return activePanel;
            }
        }

        public ErrorProvider TheErrorProvider
        {
            get
            {
                return errorProvider1;
            }
        }

        /// <summary>
        /// Get selected HitList object
        /// </summary>
        public COEHitListBO SelectedHitList
        {
            get
            {
                COEHitListBO theCOEHitListBO = null;
                //if (hitListComboBox.SelectedIndex > 0 && hitListComboBox.SelectedItem != null)
                //{
                //    if ((hitListComboBox.SelectedItem as ComboInfo) != null)
                //    {
                //        theCOEHitListBO = ((hitListComboBox.SelectedItem as ComboInfo).Key as COEHitListBO);
                //    }
                //}
                return theCOEHitListBO;
            }
        }

        /// <summary>
        /// It will return the ReRun Last Query value based on the search button option selected
        /// </summary>
        public bool IsRunLastQuery { get; set; }

        public bool IsResultCriteriaModified { get; set; }

        public bool IsSearchCriteriaModified { get; set; }

        /// <summary>
        /// Get/Set Lead Discovery installed path
        /// </summary>
        public string LDInstallationPath
        {
            get
            {
                return this.ldInstalltionPath;
            }
            set
            {
                this.ldInstalltionPath = value;
            }
        }

        /// <summary>
        /// Spotfire preferences max row limit
        /// </summary>
        public int MaxRows
        {
            get
            {
                return iMaxRow;
            }
            set
            {
                iMaxRow = value;
            }
        }

        public bool FilterChildHits
        {
            get
            {
                return filterChildHitsCheckBox.Checked;
            }
            set
            {
                filterChildHitsCheckBox.Checked = value;
            }
        }

        /// <summary>
        /// Sets the visibility of LogOff link based on custom preference value. Visible when Http is used and invisible when remoting is used
        /// </summary>
        public bool IsLogOffLinkVisible
        {
            set
            {
                logOutLinkLabel.Visible = value;
            }
        }
        #endregion

        #region Public Methods

        public enum TableSelectionMethod
        {
            DefaultFields,
            AllFields,
            UniqueKeyAndStructure,
            None
        }

        /// <summary>
        /// Prepends the table to the front of the selected tables using the currently selected TableSelectionMethod.
        /// </summary>
        /// <param name="tableBO">The table to prepend</param>
        /// <param name="panel">The table panel currently active to which the fields will be added</param>
        public void PrependTable(TableBO tableBO, Panel panel)
        {
            PrependTableInternal(tableBO, panel);
        }

        /// <summary>
        /// Prepends the table to the front of the selected tables using the specified method.
        /// </summary>
        /// <param name="tableBO">The table to prepend</param>
        /// <param name="method">Whether to insert the default fields, all fields, or only unique keys and structure</param>
        /// <param name="panel">The table panel currently active to which the fields will be added</param>
        public void PrependTable(TableBO tableBO, TableSelectionMethod method, Panel panel)
        {
            PrependTableInternal(tableBO, method, panel);
        }

        /// <summary>
        /// Appends the table to the end of the selected tables using the currently selected TableSelectionMethod.
        /// </summary>
        /// <param name="tableBO">The table to append</param>
        /// <param name="panel">The table panel currently active to which the fields will be added</param>
        public void AppendTable(TableBO tableBO, Panel panel)
        {
            AppendTableInternal(tableBO, panel);
        }

        /// <summary>
        /// Appends the table to the end of the selected tables list using the specified method.
        /// </summary>
        /// <param name="tableBO">The table to append</param>
        /// <param name="method">Whether to insert the default fields, all fields, or only unique keys and structure</param>
        /// <param name="panel">The table panel currently active to which the fields will be added</param>
        public void AppendTable(TableBO tableBO, TableSelectionMethod method, Panel panel)
        {
            AppendTableInternal(tableBO, method, panel);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Clears all currently selected tables and fields from the form.
        /// Then re-adds the base table, if a dataview is selected.
        /// </summary>
        private void ClearSelections()
        {
            ClearSelections(false, this.selectedFieldsPanel);
            ClearSelections(false, this.selectedQCFieldsPanel);
        }

        /// <summary>
        /// Clears the selection of fields
        /// </summary>
        /// <param name="clearAll">boolean value to specify to clear all selections</param>
        /// <param name="panel">The table panel currently active</param>
        private void ClearSelections(bool clearAll, Panel panel)
        {
            if (panel == this.selectedFieldsPanel)
            {
                foreach (TablePanel tablePanel in this.selectedTables)
                {
                    this.selectedFieldsPanel.Controls.Remove(tablePanel);
                }
                this._tablePanels.Clear();
                if (!clearAll && this._dataViewManagerBO != null)
                {
                    PrependTable(this._dataViewManagerBO.Tables.GetTable(this._dataViewManagerBO.BaseTableId),
                        tableSelectionMethod, this.selectedFieldsPanel);
                }
            }
            else
            {
                #region Query Criteria

                foreach (TablePanel tablePanel in this.selectedTablesOnQC)
                {
                    this.selectedQCFieldsPanel.Controls.Remove(tablePanel);
                }
                this._tablePanelsOnQC.Clear();
                if (!clearAll && this._dataViewManagerBO != null)
                {
                    PrependTable(this._dataViewManagerBO.Tables.GetTable(this._dataViewManagerBO.BaseTableId),
                        tableSelectionMethod, this.selectedQCFieldsPanel);
                }

                #endregion
            }
        }

        /// <summary>
        /// Prepends the table to the front of the selected tables using the currently selected TableSelectionMethod.
        /// </summary>
        /// <param name="tableBO">The table to prepend</param>
        /// <param name="panel">Current active table panel</param>
        /// <returns>The panel containing the prepended table</returns>
        private TablePanel PrependTableInternal(TableBO tableBO, Panel panel)
        {
            return PrependTableInternal(tableBO, this.tableSelectionMethod, panel);
        }

        /// <summary>
        /// Prepends the table to the front of the selected tables using the specified method.
        /// </summary>
        /// <param name="tableBO">The table to prepend</param>
        /// <param name="method">Whether to insert the default fields, all fields, or only unique keys and structure</param>
        /// <param name="panel">Current active table panel</param>
        /// <returns>The panel containing the prepended table</returns>
        private TablePanel PrependTableInternal(TableBO tableBO, TableSelectionMethod method, Panel panel)
        {
            return InsertTable(tableBO, method, 0, panel);
        }

        /// <summary>
        /// Appends the table to the end of the selected tables using the currently selected TableSelectionMethod.
        /// </summary>
        /// <param name="tableBO">The table to append</param>
        /// <param name="panel">Current active table panel</param>
        /// <returns>The panel containing the appended table</returns>
        private TablePanel AppendTableInternal(TableBO tableBO, Panel panel)
        {
            return AppendTableInternal(tableBO, this.tableSelectionMethod, panel);
        }

        /// <summary>
        /// Appends the table to the end of the selected tables list using the specified method.
        /// </summary>
        /// <param name="tableBO">The table to append</param>
        /// <param name="method">Whether to insert the default fields, all fields, or only unique keys and structure</param>
        /// <param name="panel">Current active table panel</param>
        /// <returns>The panel containing the appended table</returns>
        private TablePanel AppendTableInternal(TableBO tableBO, TableSelectionMethod method, Panel panel)
        {
            if (panel == selectedFieldsPanel)
                return InsertTable(tableBO, method, this._tablePanels.Count, panel);
            else
                return InsertTable(tableBO, method, this._tablePanelsOnQC.Count, panel);
            //return InsertTable(tableBO, method, this._tablePanels.Count, panel);
        }

        /// <summary>
        /// Appends the table to the end of the selected tables list using the specified method.
        /// internal for accessing it from the usercontrol.
        /// </summary>
        /// <param name="tableBO">The table to append</param>
        /// <param name="method">Whether to insert the default fields, all fields, or only unique keys and structure</param>
        /// <returns>The panel containing the appended table</returns>
        internal TablePanel AppendTableInternal(TableBO tableBO, TableSelectionMethod method)
        {
            //get the active panel and then append the table fields
            Panel panel = this.ActivePanel;
            if (panel == selectedFieldsPanel)
                return InsertTable(tableBO, method, this._tablePanels.Count, panel);
            else
                return InsertTable(tableBO, method, this._tablePanelsOnQC.Count, panel);
        }

        /// <summary>
        /// Inserts the table into the selected tables list before the beforeTable, using the specified method.
        /// </summary>
        /// <param name="tableBO">The table to insert</param>
        /// <param name="method">Whether to insert the default fields, all fields, or only unique keys and structure</param>
        /// <param name="tableIndex">The index to insert the table at</param>
        private TablePanel InsertTable(TableBO tableBO, TableSelectionMethod method, int tableIndex, Panel panel)
        {
            // Don't add a panel if it already exists.
            TablePanel tablePanelOld = this.GetTablePanel(tableBO, panel);
            if (tablePanelOld != null)
            {
                return tablePanelOld;
            }

            //this.selectedFieldsPanel.Controls.Add(tablePanel); AM
            //tablePanel.Width = this.selectedFieldsPanel.Width; AM

            IList<TablePanel> tablePanelList;
            if (panel == selectedQCFieldsPanel)
            {
                tablePanelList = this._tablePanelsOnQC;
            }
            else
            {
                tablePanelList = this._tablePanels;
                //check if parent table of current table is added on the panel and add a one
                TryAddParentTable(tableBO, method, panel, tablePanelList, ref tableIndex);
            }

            // Create a collapsible TablePanel container to put the FlowLayoutPanel in.
            TablePanel tablePanel = new TablePanel(tableBO);

            //set adding table to true to notify that field duplication can be ignored
            tablePanel.IsAddingTable = true;

            panel.Controls.Add(tablePanel);
            tablePanel.Width = panel.Width;

            int fieldIndex = 0;
            foreach (FieldContext fieldContext in GetFieldsForTable(tableBO, method, panel))
            {
                bool isFieldAdded;
                tablePanel.InsertField(fieldContext, fieldIndex, out isFieldAdded);
                //if field is added then only increase the index value; otherwise insert the field at same location
                if (isFieldAdded)
                {
                    fieldIndex++;
                }
            }

            // Force the base table to the front.
            if (tableBO.ID == this._dataViewBO.COEDataView.Basetable)
            {
                tableIndex = 0;
            }
            //else if (tablePanelList.Count > tableIndex) // && tablePanelList[tableIndex].tableBO.ID == this._dataViewBO.COEDataView.Basetable)
            //{
            //    tableIndex = tablePanelList.Count;
            //}

            // Add the TablePanel to the list of panels.
            tablePanelList.Insert(tableIndex, tablePanel);
            //set adding table to false after adding the table panel
            tablePanel.IsAddingTable = false;
            LayoutTables(panel);
            tablePanel.SizeChanged += new EventHandler(tablePanel_SizeChanged);
            return tablePanel;
        }

        /// <summary>
        /// Adds the parent table on the selected panel for currently selected table
        /// </summary>
        /// <param name="tableBO">tableBO object of current table</param>
        /// <param name="method">table selection method selected</param>
        /// <param name="panel">panel on which table is added</param>
        /// <param name="tablePanelList">list of table ids added on the panel</param>
        private void TryAddParentTable(TableBO tableBO, TableSelectionMethod method, Panel panel, IList<TablePanel> tablePanelList, ref int tableIndex)
        {
            int parentTableId;
            if (tablePanelList.Count > 0 && !IsParentTableAdded(tableBO.ID, tablePanelList, out parentTableId))
            {
                TableBO parentTableBO = this._dataViewManagerBO.Tables.GetTable(parentTableId);
                InsertTable(parentTableBO, method, tableIndex, panel);
                tableIndex++;
            }
        }

        /// <summary>
        /// Adds the parent table on the selected panel for currently selected table
        /// </summary>
        /// <param name="tableBO">tableBO object of current table</param>
        /// <param name="method">table selection method selected</param>
        /// <param name="panel">panel on which table is added</param>
        /// <param name="tablePanelList">list of table ids added on the panel</param>
        private void TryAddParentTable(TableBO tableBO, TableSelectionMethod method, Panel panel, IList<TablePanel> tablePanelList)
        {
            int parentTableId;
            if (tablePanelList.Count > 0 && !IsParentTableAdded(tableBO.ID, tablePanelList, out parentTableId))
            {
                TableBO parentTableBO = this._dataViewManagerBO.Tables.GetTable(parentTableId);
                InsertTable(parentTableBO, method, tablePanelList.Count, panel);
            }
        }

        /// <summary>
        /// Check whether parent table is added on the panel
        /// </summary>
        /// <param name="movingTableId">table id to move on the results/query panel</param>
        /// <param name="tablePanelList">collection of table ids added to the selected panel</param>
        /// <param name="parentTableId">parent table id of the current table</param>
        /// <returns>returns true if the parent table is already added on panel; otherwise false</returns>
        private bool IsParentTableAdded(int movingTableId, IList<TablePanel> tablePanelList, out int parentTableId)
        {
            bool isParentTablePresent = false;
            parentTableId = 0;
            COEDataView.Relationship tableRelationship = this._dataViewBO.COEDataView.Relationships.Find(r => r.Child == movingTableId);
            if (tableRelationship != null)
            {
                if (tablePanelList.Any(tp => tp.tableBO.ID == tableRelationship.Parent))
                {
                    isParentTablePresent = true;
                }
                parentTableId = tableRelationship.Parent;
            }
            return isParentTablePresent;
        }

        /// <summary>
        /// Removes a table from the selected tables list.
        /// internal for accessing it from the usercontrol.
        /// </summary>
        /// <param name="tablePanel">The TablePanel to remove</param>
        internal void RemoveTable(TablePanel tablePanel)
        {
            Panel panel;
            IList<TablePanel> tablePanelList;
            if (searchCriteriaTabControl.SelectedIndex == 0)
            {
                panel = this.selectedQCFieldsPanel;
                tablePanelList = this._tablePanelsOnQC;
            }
            else
            {
                panel = this.selectedFieldsPanel;
                tablePanelList = this._tablePanels;
            }

            panel.Controls.Remove(tablePanel);
            tablePanelList.Remove(tablePanel);
            LayoutTables(panel);
        }

        /// <summary>
        /// Removes a field from the selected tables list.
        /// </summary>
        /// <param name="fieldPanel">The FieldPanel to remove</param>
        internal void RemoveField(FieldPanel fieldPanel)
        {
            TablePanel tablePanel = fieldPanel.tablePanel;
            tablePanel.RemoveField(fieldPanel);
        }

        private IEnumerable<string> GetCategories()
        {
            HashSet<string> categories = new HashSet<string>();
            foreach (TableBO tableBO in this._dataViewManagerBO.Tables)
            {
                if (tableBO.Tags != null)
                {
                    foreach (string tag in tableBO.Tags)
                    {
                        categories.Add(tag);
                    }
                }
            }
            return categories;
        }

        /// <summary>
        /// Looks up the appropriate index into the field type ImageList for a given field.
        /// </summary>
        /// <param name="fieldBO">The field to look up</param>
        /// <param name="lookupField">lookup field object</param>
        /// <returns>The appropriate index into the field type ImageList for the field</returns>
        internal int GetImageIndex(FieldBO fieldBO, FieldBO lookupField)
        {
            if (fieldBO.LookupDisplayFieldId != -1 && lookupField != null)
            {
                return GetImageIndex(lookupField, null);
            }
            else if (fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
            {
                return 1;
            }
            else
            {
                switch (fieldBO.DataType)
                {
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Date:
                        return 2;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Boolean:
                        return 3;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Integer:
                        return 4;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Real:
                        return 5;
                    case CambridgeSoft.COE.Framework.Common.COEDataView.AbstractTypes.Text:
                        return 6;
                    default:
                        break;
                }
            }
            return 0;
        }

        /// <summary>
        /// Finds the selected table displayed at a given point.
        /// </summary>
        /// <param name="p">The point in screen coordinates</param>
        /// <param name="panel">Current active table panel</param>
        /// <returns>The TablePanel displayed at the given point, if any</returns>
        private TablePanel GetTableUnderFromScreenPoint(Point p, Panel panel)
        {
            Point clientPos = panel.PointToClient(p);
            return panel.GetChildAtPoint(clientPos) as TablePanel;
        }

        /// <summary>
        /// Gets an enumeration of the fields to automatically select for a given table using a given TableSelectionMethod.
        /// </summary>
        /// <param name="tableBO">The table being selected</param>
        /// <param name="method">The TableSelectionMethod to employ</param>
        /// <param name="panel">Current active table panel</param>
        /// <returns>An enumeration of fields to select</returns>
        private IEnumerable<FieldContext> GetFieldsForTable(TableBO tableBO, TableSelectionMethod method, Panel panel)
        {
            foreach (FieldBO fieldBO in tableBO.Fields)
            {
                if (fieldBO.Visible)
                {
                    bool yielded = false;
                    bool useFieldForDefaultQuery = false;

                    if (ServerVersion != null && new Version(ServerVersion.ToString(2)) > new Version(12, 5))
                    {
                        //check if the fieldBO object has IsDefaultQuery property then assign its value to local boolean variable
                        System.Reflection.PropertyInfo isDefaultQueryProperty = fieldBO.GetType().GetProperty("IsDefaultQuery");
                        if (isDefaultQueryProperty != null)
                        {
                            useFieldForDefaultQuery = (bool)isDefaultQueryProperty.GetValue(fieldBO, null);
                        }
                    }
                    else
                    {
                        //if property is not available then use IsDefault property for value setting (to support 12.5.2)
                        useFieldForDefaultQuery = fieldBO.IsDefault;
                    }

                    //do only for result criteria panel
                    if (panel == this.selectedFieldsPanel && !fieldBO.Visible)
                    {
                        continue;
                    }

                    FieldBO lookupField = null;
                    if (fieldBO.LookupDisplayFieldId > 0 && _dataViewManagerBO != null)
                    {
                        lookupField = _dataViewManagerBO.Tables.GetField(fieldBO.LookupDisplayFieldId);
                    }

                    if (fieldBO.LookupDisplayFieldId > 0 && (fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))
                    {
                        //We should not allow invalid lookup fields.
                        continue;
                    }

                    if (panel == this.selectedQCFieldsPanel)
                    {
                        //Adding the structure field, which is not having lookup
                        //Adding structure lookup fields
                        if ((fieldBO.LookupDisplayFieldId <= 0 && (fieldBO.Alias == "Structure" || fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)) || (fieldBO.LookupDisplayFieldId > 0 && lookupField != null && (lookupField.Alias == "Structure" || lookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)))
                        {
                            if (fieldBO.Visible)
                            {
                                bool _fieldsNeedsToADD = true;
                                ComboInfo cmbItem = new ComboInfo() { Key = tableBO.ID, Value = fieldBO.ID };
                                foreach (ComboInfo item in _structureFieldsList)
                                {
                                    ComboInfo cmbInfo = item.Key as ComboInfo;
                                    if (cmbInfo != null)
                                    {
                                        if ((cmbInfo.Key.Equals(cmbItem.Key)) && cmbInfo.Value.Equals(cmbItem.Value))
                                        {
                                            _fieldsNeedsToADD = false;
                                            break;
                                        }
                                    }
                                }

                                if (_fieldsNeedsToADD)
                                {
                                    ComboInfo structField = new ComboInfo() { Key = cmbItem, Value = (!string.IsNullOrEmpty(fieldBO.Alias) ? string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldBO.Alias, (!string.IsNullOrEmpty(tableBO.Alias) ? tableBO.Alias : tableBO.Name)) : string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldBO.Name, (!string.IsNullOrEmpty(tableBO.Alias) ? tableBO.Alias : tableBO.Name))) };
                                    _structureFieldsList.Add(structField);
                                }
                            }
                            continue;
                        }
                    }

                    switch (method)
                    {
                        case TableSelectionMethod.AllFields:
                            if (panel == this.selectedFieldsPanel)
                                yield return new ResultCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);
                            else
                                yield return new QueryCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);
                            yielded = true;
                            break;
                        case TableSelectionMethod.DefaultFields:
                            if (panel == this.selectedFieldsPanel)
                            {
                                if (fieldBO.IsDefault)
                                {
                                    yield return new ResultCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);
                                }
                                else
                                    continue;
                            }
                            else
                            {
                                if (useFieldForDefaultQuery)
                                    yield return new QueryCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);
                                else
                                    continue;
                            }
                            yielded = true;
                            break;
                        case TableSelectionMethod.UniqueKeyAndStructure:
                            if (tableBO.PrimaryKey == fieldBO.ID ||
                                fieldBO.IsUniqueKey ||
                                fieldBO.Alias == "Structure" ||
                                fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE || fieldBO.LookupFieldId != -1) // Fixed 162720 To Add the lookup field the LookupFieldId must be not -1. As per the Note #9 in CSBR i ahve fixed it.
                            {
                                if (panel == this.selectedFieldsPanel)
                                    yield return new ResultCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);
                                else
                                    yield return new QueryCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField);
                                yielded = true;
                            }
                            break;
                        default:
                            break;
                    }

                    if (yielded && ((fieldBO.Alias == "Structure" ||
                                    fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE) || (fieldBO.LookupDisplayFieldId != -1 && lookupField != null && (lookupField.Alias == "Structure" ||
                                    lookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))) && panel == this.selectedFieldsPanel)
                    {
                        // For structure fields, yield the Formula and Mol Weight fields also.
                        yield return new ResultCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.Formula));
                        yield return new ResultCriteriaFieldContext(tableBO, fieldBO, _dataViewManagerBO.DataViewId, lookupField, typeof(ResultsCriteria.MolWeight));
                    }
                }
            }
        }

        private void MergeFields(TableBO tableBO, TableSelectionMethod method, Panel panel)
        {
            TablePanel tablePanel = this.GetTablePanel(tableBO, panel);
            if (tablePanel != null)
            {
                foreach (FieldContext fieldContext in GetFieldsForTable(tableBO, method, panel))
                {
                    if (!tablePanel.ContainsField(fieldContext))
                    {
                        tablePanel.AppendField(fieldContext);
                    }
                }
            }
        }

        /// <summary>
        /// Append the field to the table panel
        /// internal fdor accessing this method from user control
        /// Drag and drop the fields from tree view to table panel will add the field 
        /// </summary>
        /// <param name="fieldContext"></param>
        internal void AppendField(FieldContext fieldContext)
        {
            TablePanel tablePanel = AppendTablePanel(fieldContext);

            FieldContext activePanelFieldContext = GetFieldContextForActivePanel(fieldContext);
            //the activepanelFieldContext will be null for Structure type of field, then show a warning message to user
            if (activePanelFieldContext == null)
            {
                return;
            }
            tablePanel.AppendField(activePanelFieldContext);
        }

        /// <summary>
        /// Mehtod to get the field context from tree field context for current active field panel
        /// </summary>
        /// <param name="fieldContext">The tree node field context which needs to be trasnformed into result or query criteria field context type</param>
        /// <returns>The converted field context depending on the current active field panel</returns>
        FieldContext GetFieldContextForActivePanel(FieldContext fieldContext)
        {
            if (fieldContext.fieldBO.LookupDisplayFieldId > 0 && (fieldContext.fieldBO.Alias == "Structure" || fieldContext.fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))
            {
                //We should not allow invalid lookup fields.
                MessageBox.Show(string.Format(FormWizard.Properties.Resources.LookupField_Validation, fieldContext.fieldBO.Alias), FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
            if (this.ActivePanel == selectedFieldsPanel)
            {
                return new SelectDataForm.ResultCriteriaFieldContext(fieldContext.tableBO, fieldContext.fieldBO, _dataViewManagerBO.DataViewId, fieldContext.LookupField, fieldContext.fieldCriteriaType);
            }
            else
            {
                if (fieldContext.fieldCriteriaType == typeof(ResultsCriteria.MolWeight) || fieldContext.fieldCriteriaType == typeof(ResultsCriteria.Formula) ||
                    fieldContext.fieldCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria) || fieldContext.fieldCriteriaType == typeof(SearchCriteria.CSFormulaCriteria))
                {
                    return new SelectDataForm.QueryCriteriaFieldContext(fieldContext.tableBO, fieldContext.fieldBO, _dataViewManagerBO.DataViewId, fieldContext.LookupField, fieldContext.fieldCriteriaType);
                }
                else if (fieldContext.fieldBO.Alias == "Structure" || fieldContext.fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE)
                {
                    if (fieldContext.fieldBO.LookupDisplayFieldId <= 0)
                    {
                        AddStructureField(fieldContext);
                        AppendTablePanel(fieldContext);
                    }
                    return null;
                }
                else if (fieldContext.fieldBO.LookupDisplayFieldId > 0 && fieldContext.LookupField != null && (fieldContext.LookupField.Alias == "Structure" || fieldContext.LookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))
                {
                    AddStructureField(fieldContext);
                    AppendTablePanel(fieldContext);
                    return null;
                }
                else
                {
                    return new SelectDataForm.QueryCriteriaFieldContext(fieldContext.tableBO, fieldContext.fieldBO, _dataViewManagerBO.DataViewId, fieldContext.LookupField);
                }
            }
        }

        private TablePanel AppendTablePanel(FieldContext fieldContext)
        {
            Panel panel = this.ActivePanel;
            // Add the table panel if it is not already present.
            TablePanel tablePanel = this.GetTablePanel(fieldContext.tableBO, panel);
            if (tablePanel == null)
            {
                //check for parent table added on the panel
                if (panel == this.selectedFieldsPanel)
                {
                    IList<TablePanel> tablePanelList;
                    tablePanelList = this._tablePanels;
                    TryAddParentTable(fieldContext.tableBO, this.tableSelectionMethod, panel, tablePanelList);
                }
                tablePanel = this.AppendTableInternal(fieldContext.tableBO, TableSelectionMethod.None, panel);
            }
            return tablePanel;
        }

        /// <summary>
        /// Add/select the structure field in structure search criteria panel and open up the panel
        /// </summary>
        /// <param name="fieldContext">FieldCOntext object containing information about the fieldBO object</param>
        private void AddStructureField(FieldContext fieldContext)
        {
            ComboInfo _comboInfo = new ComboInfo()
            {
                Key = (new ComboInfo() { Key = fieldContext.tableId, Value = fieldContext.fieldId }),
                Value = (!string.IsNullOrEmpty(fieldContext.fieldBO.Alias) ? string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldContext.fieldBO.Alias, (!string.IsNullOrEmpty(fieldContext.tableBO.Alias) ? fieldContext.tableBO.Alias : fieldContext.tableBO.Name)) : string.Format(FormWizard.Properties.Resources.Structure_Field_Display_Format, fieldContext.fieldBO.Name, (!string.IsNullOrEmpty(fieldContext.tableBO.Alias) ? fieldContext.tableBO.Alias : fieldContext.tableBO.Name)))
            };

            AddStructureField(_comboInfo);
        }

        /// <summary>
        /// Add/select the structure field in structure search criteria panel and open up the panel
        /// </summary>
        /// <param name="structField"></param>
        private void AddStructureField(ComboInfo structField)
        {
            //add structure field on structure search criteria panel and set in drop down box.
            //check if LD is installed on client machine
            CBVNStructureFilter.GeneralClass.LDInstallationPath = this.LDInstallationPath;
            if (string.IsNullOrEmpty(this.LDInstallationPath))
            {
                MessageBox.Show(FormWizard.Properties.Resources.LD_NOT_INSTALLED, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ultraExpandableGroupBox2.Expanded = false;
            }
            else if (CBVNStructureFilter.StructureFilterPanelControlBase.IsRendererInstalled(this._theAnalysisApplication) <= 0)
            {
                MessageBox.Show(FormWizard.Properties.Resources.Renderer_Not_Licensed, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ultraExpandableGroupBox2.Expanded = false;
            }
            else
            {
                structureFieldCriteria1.SelectedStructureField = structField;
                ultraExpandableGroupBox2.Expanded = true;
            }
        }

        private void LayoutTables(Panel panel)
        {
            IEnumerable<TablePanel> theTablePanel = GetCurrentTabInformation(panel);

            Point topLeft = new Point(0, panel.AutoScrollPosition.Y);
            foreach (TablePanel tablePanel in theTablePanel)
            {
                tablePanel.Location = topLeft;
                topLeft = new Point(0, tablePanel.Bottom + 4);
            }
        }

        private IEnumerable<TablePanel> GetCurrentTabInformation(Panel panel)
        {
            IEnumerable<TablePanel> theTablePanel;
            if (panel == this.selectedFieldsPanel)
            {
                theTablePanel = this.selectedTables;
            }
            else
            {
                theTablePanel = this.selectedTablesOnQC;
            }
            return theTablePanel;
        }

        internal TablePanel GetTablePanel(TableBO tableBO)
        {
            Panel panel = this.ActivePanel;
            IEnumerable<TablePanel> theTablePanel = GetCurrentTabInformation(panel);
            foreach (TablePanel tablePanel in theTablePanel)
            {
                if (tablePanel.tableBO.ID == tableBO.ID)
                {
                    return tablePanel;
                }
            }
            return null;
        }

        internal TablePanel GetTablePanel(TableBO tableBO, Panel panel)
        {
            IEnumerable<TablePanel> theTablePanel = GetCurrentTabInformation(panel);
            foreach (TablePanel tablePanel in theTablePanel)
            {
                if (tablePanel.tableBO.ID == tableBO.ID)
                {
                    return tablePanel;
                }
            }
            return null;
        }

        ///// <summary>
        ///// Filling Saved and Temparary HitList for the selected DataView in HitList dropdown box
        ///// </summary>
        //private void FillHitListCombo()
        //{
        //    try
        //    {
        //        this.hitListComboBox.DisplayMember = "Value";
        //        this.hitListComboBox.ValueMember = "Key";
        //        this.hitListComboBox.Items.Clear();

        //        this.hitListComboBox.Items.Add(new ComboInfo()
        //        {
        //            Key = "--Select HitList--",
        //            Value = "--Select HitList--"
        //        });

        //        this.hitListComboBox.SelectedIndex = 0;

        //        COEHitListBOList theSavedCOEHitListBOList = COEHitListBOList.GetSavedHitListList(this.dataViewBO.DatabaseName, COEUser.Name, this.dataViewBO.ID);
        //        foreach (COEHitListBO theCOEHitListBO in theSavedCOEHitListBOList)
        //        {
        //            this.hitListComboBox.Items.Add(
        //            new ComboInfo()
        //                     {
        //                         Key = theCOEHitListBO,
        //                         Value = theCOEHitListBO.Name + " - " + theCOEHitListBO.Description + " - " + theCOEHitListBO.DateCreated + " (" + theCOEHitListBO.NumHits + ")"
        //                     });
        //        }

        //        COEHitListBOList theTempCOEHitListBOList = COEHitListBOList.GetRecentHitLists(this.dataViewBO.DatabaseName, COEUser.Name, this.dataViewBO.ID, 1000);
        //        foreach (COEHitListBO theCOEHitListBO in theTempCOEHitListBOList)
        //        {
        //            this.hitListComboBox.Items.Add(
        //            new ComboInfo()
        //            {
        //                Key = theCOEHitListBO,
        //                Value = theCOEHitListBO.Name + " - " + theCOEHitListBO.Description + " - " + theCOEHitListBO.DateCreated + " (" + theCOEHitListBO.NumHits + ")"
        //            });
        //        }

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Place dropdown Image for the search button
        /// </summary>
        private void UpdateSearchButton()
        {
            this.nextButton.Image = (Image)FormWizard.Properties.Resources.ArrowDown;
            this.nextButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
        }

        /// <summary>
        /// Set the hitlist label visible and invisible.
        /// </summary>
        /// <param name="isVisible"></param>
        private void SetHitListLabel(bool isVisible)
        {
            HitListLabel.Visible = isVisible;
            SearchOverPrevHitList = isVisible;
            ClearHitlistlinkLabel.Visible = isVisible;
        }

        /// <summary>
        /// Method checks for result and query criteria modification to set the local properties. 
        /// The properties will determine the default action of the Search button
        /// </summary>
        void CheckCriteriaModification()
        {
            IsResultCriteriaModified = false;
            IsSearchCriteriaModified = false;

            if (_oldResultsCriteria == null || (_oldResultsCriteria.ToString().GetHashCode() != resultsCriteria.ToString().GetHashCode()))
            {
                IsResultCriteriaModified = true;
            }

            if (_oldSearchCriteria == null)
            {
                IsSearchCriteriaModified = true;
            }
            else if (this.isDataViewSelectionChanged)
            {
                IsSearchCriteriaModified = true;
                IsResultCriteriaModified = true;
                //set value of SearchOverPrevHitList to false only after dataview is changed by user
                SearchOverPrevHitList = false;
                COEServiceHelper.COEService.IsFromCBVN = false;
                COEServiceHelper.COEService.CBVNHitList = null;
            }
            else if (_oldSearchCriteria.ToString().GetHashCode() != SearchCriteria.ToString().GetHashCode())
            {
                IsSearchCriteriaModified = true;
            }
        }

        /// <summary>
        /// It will validate the hitlist and decides to perform next action for loading tables.
        /// </summary>
        /// <param name="isRunLastQuery"></param>
        private void PerformSearch(bool isRunLastQuery)
        {
            IsRunLastQuery = isRunLastQuery;
            if (!CheckValidDataView())
            {
                MessageBox.Show(string.Format("Invalid Dataview {0} : Can not query on the selected dataview.{1}Table(s) with same name or alias present more than once.", this.dataViewBO.COEDataView.Name, Environment.NewLine), FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            worker.RunWorkerAsync();
            ShowWaitForm(FormWizard.Properties.Resources.Process_Wait);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_waitForm != null && !_waitForm.IsDisposed)
                _waitForm.Close();
            this.BringToFront();
            COEHitList coeHl = e.Result as COEHitList;
            if (coeHl != null)
            {
                coeHitList = coeHl;
                base.NextButton = this.nextButton;
                base.NextButton.PerformClick();
            }
        }

        private void PerformOnUIThread(MethodInvoker invoker)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(invoker);
            }
            else
            {
                invoker();
            }
        }

        private void ShowWarningMessage(string message)
        {
            PerformOnUIThread(new MethodInvoker(delegate()
            {
                if (_waitForm != null && !_waitForm.IsDisposed)
                    _waitForm.Close();
            }));

            PerformOnUIThread(new MethodInvoker(delegate()
            {
                MessageBox.Show(message, FormWizard.Properties.Resources.FORM_TITLE, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }));
        }

        private void ShowErrorMessageDialog(string message, string stackTrace)
        {
            PerformOnUIThread(new MethodInvoker(delegate()
            {
                if (_waitForm != null && !_waitForm.IsDisposed)
                    _waitForm.Close();
            }));

            PerformOnUIThread(new MethodInvoker(delegate()
            {
                ErrorMessage.ShowDialog(FormWizard.Properties.Resources.FORM_TITLE, message, stackTrace);
            }));
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                COEHitList coeHl = GenerateHitList();
                e.Result = coeHl;
            }
            catch (MaxRowSettingsExceededException maxRowEx)
            {
                ShowWarningMessage(maxRowEx.Message);
            }
            catch (System.Net.WebException wEx)
            {
                ShowErrorMessageDialog(FormWizard.Properties.Resources.TimeOut_Exception_Message, wEx.ToString());
            }
            catch (System.Runtime.Serialization.SerializationException sEx)
            {
                ShowErrorMessageDialog(FormWizard.Properties.Resources.TimeOut_Exception_Message, sEx.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Active Server Pages error") || ex.GetType().FullName.Equals("System.Web.Services.Protocols.SoapException"))
                {
                    ShowErrorMessageDialog(FormWizard.Properties.Resources.TimeOut_Exception_Message, ex.ToString());
                }
                else if (ex.Message == FormWizard.Properties.Resources.Molecular_Formula_Validation || ex.Message.Contains("System.Xml.XmlException"))
                {
                    ShowErrorMessageDialog(FormWizard.Properties.Resources.Molecular_Formula_Validation, ex.ToString());
                }
                else if (ex.Message.Contains("ORA-29400: empty query structure, no search data") || ex.Message.Contains("ORA-29400: query structure could not be interpreted"))
                {
                    ShowErrorMessageDialog(FormWizard.Properties.Resources.INVALID_QUERY_STRUCTURE, ex.ToString());
                }
                else if (ex.Message.Contains("System.ArgumentException: An item with the same key has already been added"))
                {
                    ShowErrorMessageDialog(FormWizard.Properties.Resources.StructureLookupField_SearchError, ex.ToString());
                }
                else if (ex.GetBaseException() != null && ex.GetBaseException() is System.Data.Common.DbException)
                {
                    string errorMessage = ((System.Data.Common.DbException)ex.GetBaseException()).Message;
                    string[] errorMessages = errorMessage.Split(':');
                    string newError = errorMessages[1].Trim();
                    newError = newError.Substring(0, 1).ToUpper() + newError.Substring(1, newError.Length - 1);
                    ShowErrorMessageDialog(newError, ex.ToString());
                }
                else
                {
                    COEDataViewBOList dataViewBOList = COEServiceHelper.COEService.GetDataViews();
                    if (dataViewBOList == null || dataViewBOList.Count == 0)
                    {
                        ClearForm();
                        ShowErrorMessageDialog(FormWizard.Properties.Resources.No_Dataviews, ex.ToString());
                    }
                    else
                    {
                        if (this.dataViewBO != null && !dataViewBOList.Contains(this.dataViewBO))
                        {
                            this.availableDataViews = dataViewBOList.ToArray();
                            ShowErrorMessageDialog(FormWizard.Properties.Resources.Dataview_NotFound, ex.ToString());
                            dataViewComboBox.SelectedIndex = 0;
                        }
                        else if (!(this.dataViewBO != null && dataViewBOList.Contains(this.dataViewBO) && dataViewBOList.Where(x => x.ID == this.dataViewBO.ID).FirstOrDefault().COEDataView.ToString().Equals((this.dataViewBO.COEDataView.ToString()))))
                        {
                            ShowErrorMessageDialog(FormWizard.Properties.Resources.Daview_Modification_Error, ex.ToString());
                        }
                        else
                            ShowErrorMessageDialog(FormWizard.Properties.Resources.Invalid_Daview, ex.ToString());
                    }
                }
            }
        }

        bool CheckValidDataView()
        {
            bool isValid = true;
            CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTableList tableList = this.dataViewBO.COEDataView.Tables;
            if (tableList != null && tableList.Count > 0)
            {
                //get all tables with Alias value
                var z = tableList.Where(t => t.Alias != string.Empty);
                if (z != null)
                {
                    //first check for duplicates of Alias
                    var v = z.GroupBy(t => t.Alias).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                    if (v != null && v.Count > 0)
                    {
                        isValid = false;
                    }
                    else
                    {
                        //get all tables with Alias value as empty
                        var y = tableList.Where(t => t.Alias == string.Empty);
                        if (y != null)
                        {
                            //check for duplicate names
                            var w = y.GroupBy(t => t.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                            if (w != null && w.Count > 0)
                            {
                                isValid = false;
                            }
                            else
                            {
                                //if no alias and name duplicate found then check for Name with alias duplicates
                                foreach (var a in y)
                                {
                                    var b = tableList.FirstOrDefault(t => t.Alias == a.Name);
                                    if (b != null)
                                    {
                                        isValid = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Displaying the wait dialog for long process.
        /// </summary>
        /// <param name="message"></param>
        protected void ShowWaitForm(string message)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                // don't display more than one wait form at a time
                if (_waitForm != null && !_waitForm.IsDisposed)
                {
                    return;
                }

                _waitForm = new WaitForm();
                _waitForm.SetMessage(message); // "Loading data. Please wait..."
                _waitForm.StartPosition = FormStartPosition.CenterScreen;
                _waitForm.Cursor = Cursors.WaitCursor;
                _waitForm.ShowDialog(this);
                _waitForm.Cursor = Cursors.Default;
            }));
        }

        /// <summary>
        /// Displaying the wait dialog for information.
        /// </summary>
        /// <param name="message"></param>
        protected void ShowWaitForm(string message, bool isModal)
        {
            // don't display more than one wait form at a time
            if (_waitForm != null && !_waitForm.IsDisposed)
            {
                return;
            }

            _waitForm = new WaitForm();
            _waitForm.SetMessage(message); // "Loading data. Please wait..."
            _waitForm.StartPosition = FormStartPosition.CenterScreen;
            if (isModal)
                _waitForm.ShowDialog(this);
            else
            {
                _waitForm.Show(this);
                _waitForm.Refresh();
                System.Threading.Thread.Sleep(1000);
                if (_waitForm != null && !_waitForm.IsDisposed)
                {
                    _waitForm.Close();
                    return;
                }
            }
        }

        /// <summary>
        /// It will validate the row count and return the Hitlist.
        /// </summary>
        /// <returns></returns>
        private COEHitList GenerateHitList()
        {
            COEService service = COEServiceHelper.COEService;
            COEHitList newHitList = null;
            HitListInfo hitListInfo = null;
            int oldDataViewID = 0;
            int dataViewID = _dataViewBO.COEDataView.DataViewID;
            string baseTableAlias = this._dataViewBO.COEDataView.Tables.getById(this._dataViewManagerBO.BaseTableId).Alias;

            // Analysis is already open and it's not the dummy analysis      
            if (service == null)
            {
                return null;
            }

            if (coeHitList != null)
            {
                oldDataViewID = coeHitList.DataViewID;

                if (IsRunLastQuery)
                {
                    if (service.CBVNHitList == null)
                    {
                        newHitList = new COEHitList(coeHitList.HitListID, coeHitList.HitListType, coeHitList.NumHits, coeHitList.DataViewID, resultsCriteria, coeHitList.SearchCriteria);
                        HitListInfoId = coeHitList.HitListID;
                        MaxRowSettingsExceeds(service, coeHitList.NumHits, baseTableAlias);
                    }
                    else
                    {
                        newHitList = new COEHitList(service.CBVNHitList.HitListID, service.CBVNHitList.HitListType, service.CBVNHitList.NumHits, service.CBVNHitList.DataViewID, resultsCriteria, this.SearchCriteria);
                    }
                }
                else if (coeHitList.DataViewID != dataViewID || IsSearchCriteriaModified)
                {
                    //Replace Alert moved to before calling the method. 
                    if (service.CBVNHitList == null)
                    {
                        newHitList = new COEHitList(CambridgeSoft.COE.Framework.HitListType.TEMP, dataViewID, resultsCriteria, this.SearchCriteria);
                        hitListInfo = service.GetHitListInfo(newHitList.DataViewID, SearchCriteria);
                        //CID:20297
                        if (hitListInfo == null)
                        {
                            return null;
                        }
                        HitListInfoId = hitListInfo.HitListID;
                    }
                    else
                    {
                        newHitList = new COEHitList(service.CBVNHitList.HitListID, service.CBVNHitList.HitListType, service.CBVNHitList.NumHits, service.CBVNHitList.DataViewID, resultsCriteria, this.SearchCriteria);
                    }
                }
                else
                {
                    if (IsResultCriteriaModified)
                    {
                        if (service.CBVNHitList == null)
                        {
                            newHitList = new COEHitList(CambridgeSoft.COE.Framework.HitListType.TEMP, dataViewID, resultsCriteria, this.SearchCriteria);
                            hitListInfo = service.GetHitListInfo(newHitList.DataViewID, SearchCriteria);
                            if (hitListInfo == null)
                            {
                                return null;
                            }
                            HitListInfoId = hitListInfo.HitListID;
                        }
                        else
                        {
                            newHitList = new COEHitList(service.CBVNHitList.HitListID, service.CBVNHitList.HitListType, service.CBVNHitList.NumHits, service.CBVNHitList.DataViewID, resultsCriteria, service.CBVNHitList.SearchCriteria);
                            return newHitList;
                        }
                    }
                    else
                    {
                        if (service.CBVNHitList != null)
                        {
                            newHitList = new COEHitList(service.CBVNHitList.HitListID, service.CBVNHitList.HitListType, service.CBVNHitList.NumHits, service.CBVNHitList.DataViewID, resultsCriteria, service.CBVNHitList.SearchCriteria);
                            return newHitList;
                        }
                        else
                        {
                            newHitList = new COEHitList(coeHitList.HitListID, coeHitList.HitListType, coeHitList.NumHits, coeHitList.DataViewID, resultsCriteria, coeHitList.SearchCriteria);
                            hitListInfo = service.GetHitListInfo(newHitList.DataViewID, coeHitList.SearchCriteria);
                            //CID:20297
                            if (hitListInfo == null)
                            {
                                return null;
                            }
                            HitListInfoId = hitListInfo.HitListID;
                        }
                    }
                }
            }
            else
            {
                if (service.CBVNHitList == null)
                {
                    newHitList = new COEHitList(CambridgeSoft.COE.Framework.HitListType.TEMP,
                        dataViewID, resultsCriteria, SearchCriteria);
                    hitListInfo = service.GetHitListInfo(newHitList.DataViewID, SearchCriteria);
                    //CID:20297
                    if (hitListInfo == null)
                    {
                        return null;
                    }
                    HitListInfoId = hitListInfo.HitListID;
                }
                else
                {
                    hitListInfo = service.GetHitListInfo(service.CBVNHitList.DataViewID, SearchCriteria);
                    //CID:20297
                    if (hitListInfo == null)
                    {
                        return null;
                    }
                    MaxRowSettingsExceeds(service, hitListInfo.RecordCount, baseTableAlias);

                    newHitList = new COEHitList(service.CBVNHitList.HitListID, service.CBVNHitList.HitListType, service.CBVNHitList.NumHits, service.CBVNHitList.DataViewID, resultsCriteria, SearchCriteria);
                    HitListInfoId = service.CBVNHitList.HitListID;
                }
            }
            //for first time loading Datalytix
            if (newHitList != null && hitListInfo != null)
            {
                MaxRowSettingsExceeds(service, hitListInfo.RecordCount, baseTableAlias);
            }
            return newHitList;
        }

        /// <summary>
        /// Validating HitList record count against Max Row settings in Spotfire Preferences
        /// </summary>
        /// <param name="totalRecordsInHitList">Total records in HitList</param>
        /// <param name="tableName">Table name</param>
        /// <returns>True/False</returns>
        private void MaxRowSettingsExceeds(COEService service, int totalRecordsInHitList, string tableName)
        {
            if (totalRecordsInHitList <= 0)
            {
                COEDataViewBOList dataViewBOList = COEServiceHelper.COEService.GetDataViews();

                //There will be a chance of modifying the dataview leads no hits.
                if (!(dataViewBOList != null && this.dataViewBO != null && dataViewBOList.Contains(this.dataViewBO) && dataViewBOList.Where(x => x.ID == this.dataViewBO.ID).FirstOrDefault().COEDataView.ToString().Equals((this.dataViewBO.COEDataView.ToString()))))
                    throw new MaxRowSettingsExceededException(FormWizard.Properties.Resources.Daview_Modification_Error);
                else
                    throw new MaxRowSettingsExceededException(FormWizard.Properties.Resources.No_Hit_Message);
            }
            else
            {
                if (totalRecordsInHitList > this.MaxRows)
                    throw new MaxRowSettingsExceededException(String.Format(FormWizard.Properties.Resources.Max_Row_Limit_Message, totalRecordsInHitList, tableName, this.MaxRows));
                //else
                //{
                //    //Differed to next release
                //    //if (service.CheckChildMaxRow(this.SearchCriteria, this.resultsCriteria, this._dataViewBO.COEDataView, this.MaxRows, filterChildHitsCheckBox.Checked, FormWizard.Properties.Resources.Max_Row_Limit_Message, FormWizard.Properties.Resources.FORM_TITLE))
                //    //{
                //    //    isMaxRowLimitExceed = true;
                //    //}
                //}
            }
        }

        /// <summary>
        /// //For no dataviews. (Clean machine or dataviews might be deleted using DVM)
        /// It will celar the form: dataviews, search and result criterias and disable search and browse buttons
        /// </summary>
        private void ClearForm()
        {
            dataViewComboBox.Items.Clear();
            dataViewComboBox.Text = string.Empty;
            this.dataViewBO = null;
            dataviewPopupButton.Enabled = false;
            nextButton.Enabled = false;
            this.SearchCriteria.Items.Clear();
            this.resultsCriteria.Tables.Clear();
        }

        #endregion

        #region Drag and Drop

        /// <summary>
        /// Updates a drag operation as the mouse pointer moves over the selectedFieldsPanel.
        /// </summary>
        private void selectedFieldsPanel_DragOver(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetDataPresent(typeof(List<TableBO>)) ||
                  e.Data.GetDataPresent(typeof(List<FieldContext>)) ||
                  e.Data.GetDataPresent(typeof(TablePanel)) ||
                  e.Data.GetDataPresent(typeof(FieldPanel)) ||
                  e.Data.GetDataPresent(typeof(ResultsFieldPanel)) ||
                  e.Data.GetDataPresent(typeof(QueryFieldPanel))))
            {
                e.Effect = DragDropEffects.None;
                HideDragDropCue();
                return;
            }

            if ((e.KeyState & 4) == 4 && (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
                HideDragDropCue();
                return;
            }

            if (e.Data.GetDataPresent(typeof(List<TableBO>)) ||
                  e.Data.GetDataPresent(typeof(TablePanel)))
            {
                ShowDragDropCue(GetTableDropIndex(new Point(e.X, e.Y)));
            }
            else
            {
                IEnumerable<FieldContext> fieldContexts = null;
                if (e.Data.GetDataPresent(typeof(List<FieldContext>)))
                {
                    fieldContexts = (List<FieldContext>)e.Data.GetData(typeof(List<FieldContext>));
                }
                else
                {
                    FieldPanel fieldPanel = null;
                    if (e.Data.GetDataPresent(typeof(ResultsFieldPanel)))
                    {
                        fieldPanel = (FieldPanel)e.Data.GetData(typeof(ResultsFieldPanel));
                    }
                    else if (e.Data.GetDataPresent(typeof(QueryFieldPanel)))
                    {
                        fieldPanel = (FieldPanel)e.Data.GetData(typeof(QueryFieldPanel));
                    }
                    //FieldPanel fieldPanel = (FieldPanel)e.Data.GetData(typeof(FieldPanel));
                    if (fieldPanel != null)
                        fieldContexts = new FieldContext[] { fieldPanel.fieldContext };
                }
                if (fieldContexts != null)
                {
                    TablePanel tableUnder;
                    int index = GetFieldDropIndex(new Point(e.X, e.Y), out tableUnder, this.ActivePanel);// this.selectedFieldsPanel);
                    //check if table under drop is null then add table first by displaying cue just below the table
                    if (tableUnder == null && index == -1)
                    {
                        ShowDragDropCue(GetTableDropIndex(new Point(e.X, e.Y)));
                    }
                    else if (tableUnder != null && fieldContexts.All(fc => CanAcceptField(tableUnder, fc)))
                    {
                        ShowDragDropCue(tableUnder, index);
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                        HideDragDropCue();
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the passed-in TablePanel can accept the passed-in FieldContext to be added to it.
        /// </summary>
        /// <param name="tablePanel">The accepting TablePanel</param>
        /// <param name="fieldContext">The FieldContext to be added to the TablePanel</param>
        /// <returns>true iff the table of the TablePanel is an ancestor of the table of the FieldContext</returns>
        private bool CanAcceptField(TablePanel tablePanel, FieldContext fieldContext)
        {
            int parentID = tablePanel.tableBO.ID;
            int childID = fieldContext.tableBO.ID;
            while (true)
            {
                if (parentID == childID)
                {
                    return true;
                }
                else if (!this._tableParents.TryGetValue(childID, out childID))
                {
                    return false;
                }
            }
        }

        void selectedFieldsPanel_DragLeave(object sender, System.EventArgs e)
        {
            HideDragDropCue();
        }

        private int GetTableDropIndex(Point p)
        {
            if (ActivePanel == selectedFieldsPanel)
            {
                for (int i = 0; i < this._tablePanels.Count; ++i)
                {
                    TablePanel tablePanel = this._tablePanels[i];
                    if (p.Y < tablePanel.PointToScreen(new Point(0, 0)).Y)
                    {
                        return i;
                    }
                }
                return this._tablePanels.Count;
            }
            else
            {
                for (int i = 0; i < this._tablePanelsOnQC.Count; ++i)
                {
                    TablePanel tablePanel = this._tablePanelsOnQC[i];
                    if (p.Y < tablePanel.PointToScreen(new Point(0, 0)).Y)
                    {
                        return i;
                    }
                }
                return this._tablePanelsOnQC.Count;
            }
        }

        private int GetFieldDropIndex(Point p, out TablePanel tableUnder, Panel panel)
        {
            tableUnder = this.GetTableUnderFromScreenPoint(p, panel);
            if (tableUnder == null)
            {
                return -1;
            }
            if (tableUnder.Expanded)
            {
                for (int i = 0; i < tableUnder.fields.Count; ++i)
                {
                    FieldPanel fieldPanel = tableUnder.fields[i];
                    if (p.Y < fieldPanel.PointToScreen(new Point(0, 0)).Y)
                    {
                        return i;
                    }
                }
            }
            return tableUnder.fields.Count;
        }


        /// <summary>
        /// Completes a drag and drop operation onto the selectedFieldsPanel.
        /// </summary>
        private void selectedFieldsPanel_DragDrop(object sender, DragEventArgs e)
        {
            HideDragDropCue();
            if (e.Data.GetDataPresent(typeof(List<FieldContext>)))
            {
                List<FieldContext> fieldContexts = (List<FieldContext>)e.Data.GetData(typeof(List<FieldContext>));
                TablePanel tableUnder;
                int insertIndex = this.GetFieldDropIndex(new Point(e.X, e.Y), out tableUnder, this.ActivePanel);

                //if table insert index is -1 then first add the table object to the UI
                if (tableUnder == null || insertIndex == -1)
                {
                    int tableIndex = this.GetTableDropIndex(new Point(e.X, e.Y));

                    //loop through the fields and add with a new table on UI
                    foreach (FieldContext fieldContext in fieldContexts)
                    {
                        FieldContext fldContext = GetFieldContextForActivePanel(fieldContext);
                        //coverity fix
                        if (fldContext != null)
                        {
                            InsertTable(fldContext.tableBO, TableSelectionMethod.None, tableIndex, this.ActivePanel);
                            AppendField(fldContext);
                        }
                    }
                }
                else if (tableUnder != null)
                {
                    foreach (FieldContext fieldContext in fieldContexts)
                    {
                        //the fieldContext will be of type TreeFieldContext and needs to be converted to appropriate type according to active panel
                        FieldContext fldContext = GetFieldContextForActivePanel(fieldContext);
                        if (fldContext != null)
                        {
                            if (fldContext.tableBO.ID != tableUnder.tableBO.ID)
                            {
                                if (fldContext.fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE || (fldContext.fieldBO.LookupDisplayFieldId != -1 && fldContext.LookupField != null && fldContext.LookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))
                                    MessageBox.Show(FormWizard.Properties.Resources.ResultCriteria_Structure_Validation, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                else
                                    tableUnder.InsertField(fldContext, insertIndex++);
                            }
                            else
                                tableUnder.InsertField(fldContext, insertIndex++);
                        }
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(FieldPanel)))
            {
                FieldPanel movingField = (FieldPanel)e.Data.GetData(typeof(FieldPanel));
                TablePanel tableUnder;
                int insertIndex = this.GetFieldDropIndex(new Point(e.X, e.Y), out tableUnder, this.ActivePanel);
                if (tableUnder != null)
                {
                    if (tableUnder == movingField.tablePanel && e.Effect == DragDropEffects.Move)
                    {
                        // Move within the same TablePanel.
                        int movingIndex = tableUnder.fields.IndexOf(movingField);
                        if (movingIndex < insertIndex)
                        {
                            // Account for removing the field from the table before reinserting it.
                            --insertIndex;
                        }
                        if (movingIndex == insertIndex)
                        {
                            // Do nothing.
                            return;
                        }
                        tableUnder.MoveField(movingField, insertIndex);
                    }
                    else
                    {
                        // Insert into different table or copy within the same table
                        if (e.Effect == DragDropEffects.Move)
                        {
                            movingField.tablePanel.RemoveField(movingField);
                        }
                        else
                        {
                            movingField = new FieldPanel(movingField);
                        }
                        tableUnder.InsertFieldPanel(movingField, insertIndex);
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(ResultsFieldPanel)))
            {
                FieldPanel movingField = (FieldPanel)e.Data.GetData(typeof(ResultsFieldPanel));
                //for structure fields do not allow drag-drop from child to parent table
                if (movingField.fieldContext.fieldBO.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE || (movingField.fieldContext.fieldBO.LookupDisplayFieldId != -1 && movingField.fieldContext.LookupField != null && movingField.fieldContext.LookupField.IndexType == COEDataView.IndexTypes.CS_CARTRIDGE))
                {
                    MessageBox.Show(FormWizard.Properties.Resources.ResultCriteria_Structure_Validation, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                TablePanel tableUnder;
                int insertIndex = this.GetFieldDropIndex(new Point(e.X, e.Y), out tableUnder, this.ActivePanel);
                if (tableUnder != null)
                {
                    if (tableUnder == movingField.tablePanel && e.Effect == DragDropEffects.Move)
                    {
                        // Move within the same TablePanel.
                        int movingIndex = tableUnder.fields.IndexOf(movingField);
                        if (movingIndex < insertIndex)
                        {
                            // Account for removing the field from the table before reinserting it.
                            --insertIndex;
                        }
                        if (movingIndex == insertIndex)
                        {
                            // Do nothing.
                            return;
                        }
                        tableUnder.MoveField(movingField, insertIndex);
                    }
                    else
                    {
                        // Insert into different table or copy within the same table
                        if (e.Effect == DragDropEffects.Move)
                        {
                            movingField.tablePanel.RemoveField(movingField);
                        }
                        else
                        {
                            movingField = new FieldPanel(movingField);
                        }
                        tableUnder.InsertFieldPanel(movingField, insertIndex);
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(QueryFieldPanel)))
            {
                FieldPanel movingField = (FieldPanel)e.Data.GetData(typeof(QueryFieldPanel));
                TablePanel tableUnder;
                int insertIndex = this.GetFieldDropIndex(new Point(e.X, e.Y), out tableUnder, this.ActivePanel);
                if (tableUnder != null)
                {
                    if (tableUnder == movingField.tablePanel && e.Effect == DragDropEffects.Move)
                    {
                        // Move within the same TablePanel.
                        int movingIndex = tableUnder.fields.IndexOf(movingField);
                        if (movingIndex < insertIndex)
                        {
                            // Account for removing the field from the table before reinserting it.
                            --insertIndex;
                        }
                        if (movingIndex == insertIndex)
                        {
                            // Do nothing.
                            return;
                        }
                        tableUnder.MoveField(movingField, insertIndex);
                    }
                    else
                    {
                        // Insert into different table or copy within the same table
                        if (e.Effect == DragDropEffects.Move)
                        {
                            movingField.tablePanel.RemoveField(movingField);
                        }
                        else
                        {
                            movingField = new FieldPanel(movingField);
                        }
                        tableUnder.InsertFieldPanel(movingField, insertIndex);
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(List<TableBO>)))
            {
                List<TableBO> tableBOs = (List<TableBO>)e.Data.GetData(typeof(List<TableBO>));
                int insertIndex = this.GetTableDropIndex(new Point(e.X, e.Y));

                foreach (TableBO tableBO in tableBOs)
                {
                    InsertTable(tableBO, this.tableSelectionMethod, insertIndex, this.ActivePanel);
                    //do not open the structure panel while adding full table on query form.
                    foreach (ComboInfo structField in _structureFieldsList)
                    {
                        ComboInfo tableInfo = structField.Key as ComboInfo;
                        if (tableInfo != null)
                        {
                            if ((int)tableInfo.Key == tableBO.ID)
                            {
                                AddStructureField(structField);
                            }
                        }
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(TablePanel)))
            {
                TablePanel movingTable = (TablePanel)e.Data.GetData(typeof(TablePanel));
                int tableIndex = this.GetTableDropIndex(new Point(e.X, e.Y));
                TablePanel tableUnder;
                if (tableIndex < this._tablePanels.Count)
                {
                    tableUnder = this._tablePanels[tableIndex];
                }
                else
                {
                    tableUnder = null;
                }

                // Force the base table to the front.
                if (movingTable.tableBO.ID == this._dataViewBO.COEDataView.Basetable)
                {
                    tableIndex = 0;
                    tableUnder = this._tablePanels[tableIndex];
                }
                else if (tableUnder != null && tableUnder.tableBO.ID == this._dataViewBO.COEDataView.Basetable)
                {
                    ++tableIndex;
                    tableUnder = this._tablePanels[tableIndex];
                }

                int oldIndex = this._tablePanels.IndexOf(movingTable);
                if (tableIndex > oldIndex)
                {
                    // Account for the table being removed before it is inserted.
                    --tableIndex;
                }

                if (tableIndex == oldIndex)
                {
                    // Do nothing.
                    return;
                }
                else
                {
                    this._tablePanels.Remove(movingTable);
                    this._tablePanels.Insert(tableIndex, movingTable);
                    LayoutTables(this.ActivePanel); // this.selectedFieldsPanel);
                }
            }
        }


        private Label cueLabel
        {
            get
            {
                if (this._cueLabel == null)
                {
                    this._cueLabel = new Label();
                    this._cueLabel.Visible = false;
                    this._cueLabel.BackColor = Color.Red;
                    this._cueLabel.Height = 1;
                    this.Controls.Add(this._cueLabel);
                }
                return this._cueLabel;
            }
        }

        private void ShowDragDropCue(int index)
        {
            if (ActivePanel == selectedFieldsPanel)
            {
                if (this._tablePanels.Count == 0)
                {
                    HideDragDropCue();
                    return;
                }
                if (index < this._tablePanels.Count)
                {
                    TablePanel tableAfter = this._tablePanels[index];
                    ShowDragDropCueAt(tableAfter, -2);
                }
                else
                {
                    TablePanel lastTable = this._tablePanels[this._tablePanels.Count - 1];
                    ShowDragDropCueAt(lastTable, lastTable.Height + 2);
                }
            }
            else
            {
                if (this._tablePanelsOnQC.Count == 0)
                {
                    HideDragDropCue();
                    return;
                }
                if (index < this._tablePanelsOnQC.Count)
                {
                    TablePanel tableAfter = this._tablePanelsOnQC[index];
                    ShowDragDropCueAt(tableAfter, -2);
                }
                else
                {
                    TablePanel lastTable = this._tablePanelsOnQC[this._tablePanelsOnQC.Count - 1];
                    ShowDragDropCueAt(lastTable, lastTable.Height + 2);
                }
            }
        }

        private void ShowDragDropCue(TablePanel tablePanel, int index)
        {
            if (tablePanel.fields.Count == 0 || !tablePanel.Expanded)
            {
                HideDragDropCue();
                return;
            }
            if (index < tablePanel.fields.Count)
            {
                FieldPanel fieldAfter = tablePanel.fields[index];
                ShowDragDropCueAt(fieldAfter, 0);
            }
            else
            {
                FieldPanel lastField = tablePanel.fields[tablePanel.fields.Count - 1];
                ShowDragDropCueAt(lastField, lastField.Height + 1);
            }
        }

        private void ShowDragDropCueAt(Control control, int yOffset)
        {
            this.cueLabel.Location = this.PointToClient(control.PointToScreen(new Point(0, yOffset)));
            this.cueLabel.Width = control.Width;
            this.cueLabel.BringToFront();
            this.cueLabel.Visible = true;
        }

        private void HideDragDropCue()
        {
            this.cueLabel.Visible = false;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates the DataView when the DataView selection is changed.
        /// </summary>
        private void dataViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _structureFieldsList.Clear();
            this.selectDataview1.ClearSearchText();
            if (!this._changingDataView)  // Ignore if the selection was changed programmatically.
            {
                COEDataViewBO dataViewBO = dataViewComboBox.SelectedItem as COEDataViewBO;
                if (dataViewBO != null)
                {
                    this.dataViewBO = dataViewBO;
                    this.structureFieldCriteria1.FillAvailableStructureFields(_structureFieldsList);
                    this.structureFieldCriteria1.ClearStructureFilter();
                }
                if (!this.isDataViewSelectionChanged)
                {
                    this.isDataViewSelectionChanged = true;
                }
            }
            SetHitListLabel(false);
        }
        /// It will append the fields to the table panel based on the selection mode like default fields, all fields or unique/structured columns
        /// internal for accessingit from user control.
        /// <param name="tableBO"></param>
        /// <param name="tsm"></param>
        internal void AppendTableFields(TableBO tableBO, TableSelectionMethod tsm)
        {
            TablePanel tablePanel = AppendTableInternal(tableBO, tsm);
            if (tablePanel != null)
            {
                // The table is already present; just merge in the fields.
                MergeFields(tableBO, tsm, ActivePanel);
            }
        }

        private void selectedTableContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Don't allow the base table to be removed.
            TablePanel tablePanel = (TablePanel)selectedTableContextMenuStrip.SourceControl;
            //Coverity Bug Fix  CID 13115 
            this.removeTableToolStripMenuItem.Enabled = (tablePanel != null) ? (tablePanel.tableBO.ID != this._dataViewManagerBO.BaseTableId) : false;
        }

        private void removeTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TablePanel tablePanel = (TablePanel)selectedTableContextMenuStrip.SourceControl;
            //coverity fix
            if(tablePanel != null)
                RemoveTable(tablePanel);
        }

        private void selectedFieldContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _contextualField = (FieldPanel)selectedFieldContextMenuStrip.SourceControl;
            //Coverity Bug Fix : CID 13114 
            if (_contextualField != null)
            {
                //code moved to the field panel
                _contextualField.SetContextMenuStripItems();
            }
        }

        private void renameFieldToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            // Don't allow the user to rename the field to something that already exists in the same table.
            string newAlias = this.renameFieldToolStripTextBox.Text;
            if (this._contextualField.tablePanel.fields.Any(f => f.Alias == newAlias && f != this._contextualField))
            {
                this.renameFieldToolStripTextBox.ForeColor = Color.Red;
            }
            else
            {
                this.renameFieldToolStripTextBox.ForeColor = Color.Black;
                _contextualField.Alias = this.renameFieldToolStripTextBox.Text;
            }
        }

        private void removeFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveField(_contextualField);
        }

        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextualField.GetType() == typeof(ResultsFieldPanel))
            {
                ((ResultsFieldPanel)_contextualField).SortDirection = ResultsCriteria.SortDirection.ASC;
            }
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextualField.GetType() == typeof(ResultsFieldPanel))
            {
                ((ResultsFieldPanel)_contextualField).SortDirection = ResultsCriteria.SortDirection.DESC;
            }
        }

        private void sortNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextualField.GetType() == typeof(ResultsFieldPanel))
            {
                ((ResultsFieldPanel)_contextualField).SortDirection = null;
            }
        }

        private void tablePanel_SizeChanged(object sender, EventArgs e)
        {
            if (searchCriteriaTabControl.SelectedIndex == 0)
            {
                LayoutTables(this.selectedQCFieldsPanel);
            }
            else
            {
                LayoutTables(this.selectedFieldsPanel);
            }
        }

        private void dataviewPopupButton_Click(object sender, EventArgs e)
        {
            SelectDataviewForm sdf = new SelectDataviewForm();
            // use ModulesService to map the resourceName to an absolute path to the default help file.
            string absolutePathToHelpFile = this._theAnalysisApplication.GetService<ModulesService>().GetResourcePath(FormWizard.Properties.Resources.AdavncedDataviewSearchHelp);
            if (!string.IsNullOrEmpty(absolutePathToHelpFile))
            {
                sdf.HelpFilePath = absolutePathToHelpFile;
            }
            sdf.FillDataviews(availableDataViews);
            if (dataViewComboBox.SelectedItem != null)
            {
                sdf.DataviewBO = (dataViewComboBox.SelectedItem as COEDataViewBO);
            }
            DialogResult dlgResult = sdf.ShowDialog(this);
            if (dlgResult == System.Windows.Forms.DialogResult.OK)
                dataViewComboBox.SelectedItem = sdf._selectedDataviewBO;
        }

        private void nextButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //call method to set properties that determines the default action of Search button
                CheckCriteriaModification();

                if (!IsSearchCriteriaModified)//nothing is modified or result criteria modified
                {
                    UpdateCurrentResultstoolStripMenuItem.Enabled = true;
                    UpdateCurrentResultstoolStripMenuItem.Font = new Font(UpdateCurrentResultstoolStripMenuItem.Font, FontStyle.Bold);//Hilight Default item
                    ReplaceAllDatatoolStripMenuItem.Enabled = true;
                    ReplaceAllDatatoolStripMenuItem.Font = new Font(ReplaceAllDatatoolStripMenuItem.Font, FontStyle.Regular);
                }
                else
                {
                    UpdateCurrentResultstoolStripMenuItem.Enabled = false; //query criteria modified or dataview changed.
                    UpdateCurrentResultstoolStripMenuItem.Font = new Font(UpdateCurrentResultstoolStripMenuItem.Font, FontStyle.Regular);
                    ReplaceAllDatatoolStripMenuItem.Enabled = true;
                    ReplaceAllDatatoolStripMenuItem.Font = new Font(ReplaceAllDatatoolStripMenuItem.Font, FontStyle.Bold);//Hilight Default item
                }
                if (e.X > 55)//show option when clcik on dropdown image area
                {
                    this.SearchContextMenuStrip.Show(nextButton, new Point(0, nextButton.Height));
                }
                else
                {
                    //performing default actions with out dropdown options when click on search text area (Not dropdown image area)
                    if (!IsSearchCriteriaModified)
                        UpdateCurrentResultstoolStripMenuItem_Click(UpdateCurrentResultstoolStripMenuItem, null);//Default Action
                    else
                        ReplaceAllDatatoolStripMenuItem_Click(ReplaceAllDatatoolStripMenuItem, null);//Default Action

                }
            }
        }

        private void UpdateCurrentResultstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
                PerformSearch(true);
        }

        private void ReplaceAllDatatoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
                PerformSearch(false);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            if (_waitForm != null && !_waitForm.IsDisposed)
                _waitForm.Close();
        }

        private void searchCriteriaTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchCriteriaTabControl.SelectedTab == searchCriteriaTabControl.TabPages["queryCriteriaTabPage"])
            {
                activePanel = selectedQCFieldsPanel;

                if (_tablePanelsOnQC.Count > 0)
                {
                    (_tablePanelsOnQC[0] as Infragistics.Win.Misc.UltraExpandableGroupBox).Expanded = false;
                    (_tablePanelsOnQC[0] as Infragistics.Win.Misc.UltraExpandableGroupBox).Expanded = true;
                }
            }
            else if (searchCriteriaTabControl.SelectedTab == searchCriteriaTabControl.TabPages["resultCriteriaTabPage"])
            {
                activePanel = selectedFieldsPanel;

                if (_tablePanels.Count > 0)
                {
                    (_tablePanels[0] as Infragistics.Win.Misc.UltraExpandableGroupBox).Expanded = false;
                    (_tablePanels[0] as Infragistics.Win.Misc.UltraExpandableGroupBox).Expanded = true;
                }
            }
        }

        private void ultraExpandableGroupBox2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.LDInstallationPath))
                {
                    CBVNStructureFilter.GeneralClass.LDInstallationPath = this.LDInstallationPath;
                    if (CBVNStructureFilter.StructureFilterPanelControlBase.IsRendererInstalled(this._theAnalysisApplication) <= 0)
                    {
                        MessageBox.Show(FormWizard.Properties.Resources.Renderer_Not_Licensed, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ultraExpandableGroupBox2.Expanded = false;
                    }
                }
                else
                {
                    MessageBox.Show(FormWizard.Properties.Resources.LD_NOT_INSTALLED, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ultraExpandableGroupBox2.Expanded = false;
                }
            }
            catch
            {
            }
        }

        private void ClearHitlistlinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetHitListLabel(false);
            if (COEServiceHelper.COEService != null)
            {
                COEServiceHelper.COEService.IsFromCBVN = false;
                COEServiceHelper.COEService.CBVNHitList = null;
            }

            //After clearing CBVN HitList, Result Criteria is re-binding from DataView. Because Result criteria from CBVN and on Datalytix are mismatch. 
            //To fix the mismatch issue, it is require to clear and rebind Result criteria from DataView definition.
            ClearSelections(false, this.selectedFieldsPanel);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            COEDataViewBOList dataViewBOList = COEServiceHelper.COEService.GetDataViews();
            //For no dataviews
            if (dataViewBOList == null || dataViewBOList.Count == 0)
            {
                ClearForm();
                MessageBox.Show(FormWizard.Properties.Resources.No_Dataviews, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (this.dataViewBO == null || (dataViewBOList.Contains(this.dataViewBO) && dataViewBOList.Where(x => x.ID == this.dataViewBO.ID).FirstOrDefault().COEDataView.ToString().Equals((this.dataViewBO.COEDataView.ToString()))))
                {
                    this.availableDataViews = dataViewBOList.ToArray();
                    if (this.dataViewBO != null)
                    {
                        //refresh dataviews with out warning as selected dataview not modified.
                        this._changingDataView = true;
                        this.dataViewComboBox.SelectedItem = this.dataViewBO;
                        this._changingDataView = false;
                    }
                    else
                    {
                        //If refresh clicks then there are no dataviews then the buttons will be disabled
                        //And then again refresh if we get the dataviews back then selecting the first dataview default.
                        this.dataViewComboBox.SelectedIndex = 0;
                        dataviewPopupButton.Enabled = true;
                        nextButton.Enabled = true;
                    }
                    ShowWaitForm(FormWizard.Properties.Resources.Refresh_Dataviews, false);
                }
                else if (this.dataViewBO != null && !dataViewBOList.Contains(this.dataViewBO))
                {
                    //selected dataview deleted or does not have permissions
                    this.availableDataViews = dataViewBOList.ToArray();
                    MessageBox.Show(FormWizard.Properties.Resources.Dataview_NotFound, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    dataViewComboBox.SelectedIndex = 0;
                }
                else
                {
                    //Selected dataview modified
                    if (MessageBox.Show(FormWizard.Properties.Resources.DataView_Modification_Alert, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.availableDataViews = dataViewBOList.ToArray();
                        foreach (COEDataViewBO dv in this.availableDataViews)
                        {
                            if (dv.ID == this.dataViewBO.ID)
                            {
                                this.dataViewComboBox.SelectedItem = dv;
                                break;
                            }
                        }
                        ShowWaitForm(FormWizard.Properties.Resources.Refresh_Dataviews, false);
                    }
                    else
                    {
                        ShowWaitForm(FormWizard.Properties.Resources.Refresh_Dataviews_Cancel, false);
                    }
                }
            }
        }

        private void SelectDataForm_Load(object sender, EventArgs e)
        {
            //set form title as Datalytix for TIBCO Spotfire®
            this.Text = string.Format(FormWizard.Properties.Resources.Form_Title_Format, FormWizard.Properties.Resources.FORM_TITLE, COEServiceHelper.COEService.ServerName, COEServiceHelper.COEService.UserName);

            if (COEServiceHelper.COEService.IsFromCBVN)
            {
                this.CBVNHitListRowCount = COEServiceHelper.COEService.CBVNHitList.NumHits;
                this.CBVNFormName = COEServiceHelper.COEService.CBVNFormName;
                if (!string.IsNullOrEmpty(this.CBVNFormName) && this.CBVNHitListRowCount > 0)
                {
                    HitListLabel.Text = string.Format(FormWizard.Properties.Resources.HITLIST_INFO, this.CBVNHitListRowCount, this.CBVNFormName);
                    SetHitListLabel(true);
                }
                else
                {
                    SetHitListLabel(false);
                }
            }

            if (this.structureFieldCriteria1.theStructureFilterPanelControlBase == null)
            {
                this.structureFieldCriteria1.FillAvailableStructureFields(_structureFieldsList);
            }
        }

        /// <summary>
        /// Clears the controls on the result definition form and reset it to default state
        /// </summary>
        /// <param name="sender">button from tool strip menu</param>
        /// <param name="e">event arguments</param>
        private void clearResultDefinitionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(FormWizard.Properties.Resources.ClearResultDefinitionMsg, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                //clear result tab panels
                ClearSelections(false, this.selectedFieldsPanel);
            }
        }

        /// <summary>
        /// Clears the controls on the query form and reset it to default state
        /// </summary>
        /// <param name="sender">button from tool strip menu</param>
        /// <param name="e">event arguments</param>
        private void clearQueryFormLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(FormWizard.Properties.Resources.ClearQueryMsg, FormWizard.Properties.Resources.FORM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                //clear structure panel
                this.structureFieldCriteria1.ClearStructureFilter();
                //clear query tab panels
                ClearSelections(false, this.selectedQCFieldsPanel);
            }
        }

        private void dataViewComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cmbBox = sender as ComboBox;
            if (cmbBox != null)
            {
                if (cmbBox.DroppedDown)
                    cmbBox.DroppedDown = false;
            }
        }

        #endregion

        #region FieldContextClasses

        /// <summary>
        /// Field context class to represent the result criteria field context
        /// </summary>
        internal class ResultCriteriaFieldContext : FieldContext
        {
            #region Variables
            public ResultsCriteria.IResultsCriteriaBase fieldResultsCriteria;
            #endregion

            #region Constructor

            public ResultCriteriaFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField)
                : base(tableBO, fieldBO, dataviewId, lookupField)
            {
                this.fieldCriteriaType = typeof(ResultsCriteria.Field);
            }

            public ResultCriteriaFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField, Type fieldResultsCriteriaType)
                : base(tableBO, fieldBO, dataviewId, lookupField, fieldResultsCriteriaType)
            {
                //check if fieldREsultsCriteria type is null then set it to default type ResultsCriteria.Field
                if (fieldResultsCriteriaType == null)
                    fieldResultsCriteriaType = typeof(ResultsCriteria.Field);
                this.fieldCriteriaType = fieldResultsCriteriaType;
            }

            public ResultCriteriaFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField, ResultsCriteria.IResultsCriteriaBase fieldResultsCriteria)
                : this(tableBO, fieldBO, dataviewId, lookupField, fieldResultsCriteria.GetType())
            {
                this.fieldResultsCriteria = fieldResultsCriteria;
            }

            #endregion
        }

        /// <summary>
        /// Field context class to represent the query criteria field context
        /// </summary>
        internal class QueryCriteriaFieldContext : FieldContext
        {
            #region Valriables
            public SearchCriteria.ISearchCriteriaBase fieldQueryCriteria;
            #endregion

            #region Constructor

            public QueryCriteriaFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField)
                : base(tableBO, fieldBO, dataviewId, lookupField)
            {
                //TODO : depending on the fieldBo.DataType we need to add specific search criteria object and set the type accordingly
                if (fieldBO.LookupDisplayFieldId > 0 && lookupField != null)
                    this.fieldCriteriaType = GetFieldCriteriaType(lookupField.DataType);
                else
                    this.fieldCriteriaType = GetFieldCriteriaType(fieldBO.DataType);
            }

            public QueryCriteriaFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField, Type fieldQueryCriteriaType)
                : base(tableBO, fieldBO, dataviewId, lookupField, fieldQueryCriteriaType)
            {
                if (fieldQueryCriteriaType == typeof(ResultsCriteria.MolWeight))
                {
                    this.fieldCriteriaType = typeof(SearchCriteria.CSMolWeightCriteria);
                }
                else if (fieldQueryCriteriaType == typeof(ResultsCriteria.Formula))
                {
                    this.fieldCriteriaType = typeof(SearchCriteria.CSFormulaCriteria);
                }
                else
                {
                    this.fieldCriteriaType = fieldQueryCriteriaType;
                }
            }

            public QueryCriteriaFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField, SearchCriteria.ISearchCriteriaBase fieldQueryCriteria)
                : this(tableBO, fieldBO, dataviewId, lookupField, fieldQueryCriteria.GetType())
            {
                this.fieldQueryCriteria = fieldQueryCriteria;
            }

            private Type GetFieldCriteriaType(COEDataView.AbstractTypes fieldType)
            {
                switch (fieldType)
                {
                    case COEDataView.AbstractTypes.Date:
                        return this.fieldCriteriaType = typeof(SearchCriteria.DateCriteria);
                    case COEDataView.AbstractTypes.Integer:
                    case COEDataView.AbstractTypes.Real:
                        return this.fieldCriteriaType = typeof(SearchCriteria.NumericalCriteria);
                    case COEDataView.AbstractTypes.Text:
                    default:
                        return this.fieldCriteriaType = typeof(SearchCriteria.TextCriteria);
                }
            }

            #endregion
        }

        /// <summary>
        /// Field context class to represent the tree node context
        /// </summary>
        internal class TreeNodeFieldContext : FieldContext
        {
            #region Constructor
            public TreeNodeFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField)
                : base(tableBO, fieldBO, dataviewId, lookupField)
            {
            }

            public TreeNodeFieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField, Type fieldCriteriaType)
                : base(tableBO, fieldBO, dataviewId, lookupField, fieldCriteriaType)
            {
            }
            #endregion

            /// <summary>
            /// Enum to maintain the field context type
            /// </summary>
            public enum FieldContextType
            {
                ResultCriteria,
                QueryCriteria
            }
        }

        /// <summary>
        /// Field context class to maintain the table and field BO objects
        /// </summary>
        internal abstract class FieldContext
        {
            #region Variables
            public TableBO tableBO;
            public FieldBO fieldBO;
            public Type fieldCriteriaType;
            private int _dataviewId = -1;
            private int _lookupFieldTableBOId = -1;
            private FieldBO _lookupField = null;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the table id corresponding to the field
            /// </summary>
            public int tableId
            {
                get
                {
                    if (tableBO != null)
                    {
                        return tableBO.ID;
                    }
                    return -1;
                }
            }

            /// <summary>
            /// Gets the field id
            /// </summary>
            public int fieldId
            {
                get
                {
                    if (fieldBO != null)
                    {
                        return fieldBO.ID;
                    }
                    return -1;
                }
            }
            public int DataviewId
            {
                get
                {
                    return _dataviewId;
                }
                set
                {
                    _dataviewId = value;
                }
            }

            public FieldBO LookupField
            {
                get
                {
                    return _lookupField;
                }
                set
                {
                    _lookupField = value;
                    GetLookupTableBOId();
                }
            }

            /// <summary>
            /// Gets the lookup field tableBO id 
            /// </summary>
            public int LookupFieldTableBOId
            {
                get
                {
                    return _lookupFieldTableBOId;
                }
            }

            #endregion

            #region Constructor
            protected FieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField)
            {
                this.tableBO = tableBO;
                this.fieldBO = fieldBO;
                this._dataviewId = dataviewId;
                this.LookupField = lookupField;
            }

            protected FieldContext(TableBO tableBO, FieldBO fieldBO, int dataviewId, FieldBO lookupField, Type fieldCriteriaType)
                : this(tableBO, fieldBO, dataviewId, lookupField)
            {
                this.fieldCriteriaType = fieldCriteriaType;
            }
            #endregion

            /// <summary>
            /// Gets the lookup field table id from dataview
            /// </summary>
            void GetLookupTableBOId()
            {
                int lookUpFieldId = -1;
                if (_lookupField != null)
                {
                    lookUpFieldId = _lookupField.ID;
                    var lookupFieldTableBO = COEServiceHelper.COEService.TheCOEDataviewBO.DataViewManager.Tables.GetTableByFieldId(lookUpFieldId);
                    if(lookupFieldTableBO != null)
                        _lookupFieldTableBOId = lookupFieldTableBO.ID;
                }
            }
        }

        #endregion FieldContextClasses

        internal class AggregationFunction
        {
            public readonly string DisplayName;
            public readonly string Function;
            public readonly COEDataView.AbstractTypes[] AbstractTypes;

            public AggregationFunction(string displayName, string function, COEDataView.AbstractTypes[] abstractTypes)
            {
                DisplayName = displayName;
                Function = function;
                AbstractTypes = abstractTypes;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        internal static readonly AggregationFunction[] AggregationFunctions = new AggregationFunction[] {
            new AggregationFunction(Properties.Resources.AVERAGE_DISPLAY_NAME, Properties.Resources.AVERAGE_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.COUNT_DISPLAY_NAME, Properties.Resources.COUNT_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Boolean, COEDataView.AbstractTypes.Date,
                                                 COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real,
                                                 COEDataView.AbstractTypes.Text}),
            //removed cumulative sample standard deviation and variance calculation functions from aggregations as per CBVS-988
            //new AggregationFunction(Properties.Resources.CUM_SAMP_STDDEV_DISPLAY_NAME, Properties.Resources.CUM_SAMP_STDDEV_FUNCTION,
            //    new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            //new AggregationFunction(Properties.Resources.CUM_SAMP_VARIANCE_DISPLAY_NAME, Properties.Resources.CUM_SAMP_VARIANCE_FUNCTION,
            //    new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.MAX_DISPLAY_NAME, Properties.Resources.MAX_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Date, COEDataView.AbstractTypes.Integer,
                                                 COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.MEDIAN_DISPLAY_NAME, Properties.Resources.MEDIAN_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Date, COEDataView.AbstractTypes.Integer,
                                                 COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.MIN_DISPLAY_NAME, Properties.Resources.MIN_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Date, COEDataView.AbstractTypes.Integer,
                                                 COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.POP_STDDEV_DISPLAY_NAME, Properties.Resources.POP_STDDEV_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.POP_VARIANCE_DISPLAY_NAME, Properties.Resources.POP_VARIANCE_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.STDDEV_DISPLAY_NAME, Properties.Resources.STDDEV_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.SUM_DISPLAY_NAME, Properties.Resources.SUM_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
            new AggregationFunction(Properties.Resources.VARIANCE_DISPLAY_NAME, Properties.Resources.VARIANCE_FUNCTION,
                new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real}),
        };


        internal static IEnumerable<AggregationFunction> GetAggregationFunctions(FieldPanel fieldPanel)
        {
            COEDataView.AbstractTypes dataType = fieldPanel.fieldContext.fieldBO.DataType;
            if (fieldPanel.fieldContext.fieldBO.LookupDisplayFieldId > 0 && fieldPanel.fieldContext.LookupField != null)
                dataType = fieldPanel.fieldContext.LookupField.DataType;
            return AggregationFunctions.Where(F => F.AbstractTypes.Contains(dataType));
        }

        #region enum classes for operators

        /// <summary>
        /// Class to hold the coeoperator value for various datatypes
        /// </summary>
        internal class QueryCriteriaOperator
        {
            string displayOperatorName;
            public readonly SearchCriteria.COEOperators coeOperator;
            public readonly COEDataView.AbstractTypes[] AbstractTypes;
            public string queryOperatorType = string.Empty;

            public QueryCriteriaOperator(string operatorDisplay, SearchCriteria.COEOperators coeOperator, COEDataView.AbstractTypes[] abstractTypes)
            {
                this.displayOperatorName = operatorDisplay;
                this.coeOperator = coeOperator;
                this.AbstractTypes = abstractTypes;
            }

            public override string ToString()
            {
                return displayOperatorName;
            }

        }

        internal class MolWeightQueryCriteriaOperator : QueryCriteriaOperator
        {
            public MolWeightQueryCriteriaOperator(string operatorDisplay, SearchCriteria.COEOperators coeOperator, COEDataView.AbstractTypes[] abstractTypes) :
                base(operatorDisplay, coeOperator, abstractTypes)
            {
                queryOperatorType = "MOLWEIGHT";
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        internal class FormulaQueryCriteriaOperator : QueryCriteriaOperator
        {
            public FormulaQueryCriteriaOperator(string operatorDisplay, SearchCriteria.COEOperators coeOperator, COEDataView.AbstractTypes[] abstractTypes) :
                base(operatorDisplay, coeOperator, abstractTypes)
            {
                queryOperatorType = "FORMULA";
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        internal static readonly QueryCriteriaOperator[] QueryCriteriaOperators = new QueryCriteriaOperator[] {
            new QueryCriteriaOperator(FormWizard.Properties.Resources.EQUAL_OPERAOTR, SearchCriteria.COEOperators.EQUAL, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real, 
                    COEDataView.AbstractTypes.Text, COEDataView.AbstractTypes.Date, COEDataView.AbstractTypes.Boolean }),
            new QueryCriteriaOperator(FormWizard.Properties.Resources.GREATERTHAN_OPERATOR, SearchCriteria.COEOperators.GT, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real, 
                    COEDataView.AbstractTypes.Date }),
            new QueryCriteriaOperator(FormWizard.Properties.Resources.GREATERTHANEQUAL_OPERATOR, SearchCriteria.COEOperators.GTE, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real, 
                    COEDataView.AbstractTypes.Date }),  
            new QueryCriteriaOperator(FormWizard.Properties.Resources.LESSTHAN_OPERATOR, SearchCriteria.COEOperators.LT, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real, 
                    COEDataView.AbstractTypes.Date }),
            new QueryCriteriaOperator(FormWizard.Properties.Resources.LESSTHANEQUAL_OPERATOR, SearchCriteria.COEOperators.LTE, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real, 
                    COEDataView.AbstractTypes.Date }),
            new QueryCriteriaOperator(FormWizard.Properties.Resources.NOTEQUAL_OPERATOR, SearchCriteria.COEOperators.NOTEQUAL, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real, 
                    COEDataView.AbstractTypes.Text, COEDataView.AbstractTypes.Boolean }),
            new QueryCriteriaOperator(FormWizard.Properties.Resources.CONTAINS_OPERATOR, SearchCriteria.COEOperators.CONTAINS, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Text }),
            new QueryCriteriaOperator(FormWizard.Properties.Resources.BETWEEN_OPERATOR, SearchCriteria.COEOperators.EQUAL, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Integer, COEDataView.AbstractTypes.Real,
                    COEDataView.AbstractTypes.Date, COEDataView.AbstractTypes.Boolean}),
            new MolWeightQueryCriteriaOperator(FormWizard.Properties.Resources.EQUAL_OPERAOTR, SearchCriteria.COEOperators.EQUAL, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Real}),
            new MolWeightQueryCriteriaOperator(FormWizard.Properties.Resources.GREATERTHAN_OPERATOR, SearchCriteria.COEOperators.GT, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Real}),
            new MolWeightQueryCriteriaOperator(FormWizard.Properties.Resources.LESSTHAN_OPERATOR, SearchCriteria.COEOperators.LT, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Real}),
            new MolWeightQueryCriteriaOperator(FormWizard.Properties.Resources.GREATERTHANEQUAL_OPERATOR, SearchCriteria.COEOperators.GTE, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Real}),
            new MolWeightQueryCriteriaOperator(FormWizard.Properties.Resources.LESSTHANEQUAL_OPERATOR, SearchCriteria.COEOperators.LTE, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Real}),
            new MolWeightQueryCriteriaOperator(FormWizard.Properties.Resources.BETWEEN_OPERATOR, SearchCriteria.COEOperators.EQUAL, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Real}),

            new FormulaQueryCriteriaOperator(FormWizard.Properties.Resources.EQUAL_OPERAOTR, SearchCriteria.COEOperators.EQUAL, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Text}),
            new FormulaQueryCriteriaOperator(FormWizard.Properties.Resources.CONTAINS_OPERATOR, SearchCriteria.COEOperators.CONTAINS, new COEDataView.AbstractTypes[] {COEDataView.AbstractTypes.Text})
        };

        /// <summary>
        /// Method to get the query criteria operators for specified field panel
        /// </summary>
        /// <param name="fieldPanel">Current active field panel</param>
        /// <returns>Returns the query criteria operator collection</returns>
        internal static IEnumerable<QueryCriteriaOperator> GetQueryCriteriaOperators(FieldPanel fieldPanel)
        {
            if (fieldPanel.fieldContext.fieldCriteriaType == typeof(SearchCriteria.CSMolWeightCriteria))
            {
                return QueryCriteriaOperators.Where(F => F.queryOperatorType.Equals("MOLWEIGHT", StringComparison.OrdinalIgnoreCase));
            }
            else if (fieldPanel.fieldContext.fieldCriteriaType == typeof(SearchCriteria.CSFormulaCriteria))
            {
                return QueryCriteriaOperators.Where(F => F.queryOperatorType.Equals("FORMULA", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                COEDataView.AbstractTypes dataType = fieldPanel.fieldContext.fieldBO.DataType;
                if (fieldPanel.fieldContext.fieldBO.LookupDisplayFieldId > 0 && fieldPanel.fieldContext.LookupField != null)
                    dataType = fieldPanel.fieldContext.LookupField.DataType;
                //check for empty queryOperatorType to get correct list of operators
                return QueryCriteriaOperators.Where(F => F.AbstractTypes.Contains(dataType) && F.queryOperatorType == string.Empty);
            }
        }

        #endregion

        private void logOutLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            COEService.LogOut();
            this.Close();
        }

        /// <summary>
        /// Intialize and setup the help for Datalytix
        /// </summary>
        void IntializeHelpSettings()
        {
            #region uncomment this once the help document is added to .pkdesc file and help is received
            // use ModulesService to map the resourceName to an absolute path to the default help file.
            string absolutePathToHelpFile = this._theAnalysisApplication.GetService<ModulesService>().GetResourcePath(FormWizard.Properties.Resources.HelpResourceName);

            if (!string.IsNullOrEmpty(absolutePathToHelpFile))
            {
                this.helpProvider.HelpNamespace = absolutePathToHelpFile;
                this.helpProvider.SetHelpKeyword(this, FormWizard.Properties.Resources.HelpDefaultTopicId);
            }
            #endregion uncomment this once the help document is added to .pkdesc file and help is received
        }

        /// <summary>
        /// Raise the help button click event
        /// </summary>
        /// <param name="sender">help button</param>
        /// <param name="e">event arguments</param>
        private void SelectDataForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(this, this.helpProvider.HelpNamespace);
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(this, this.helpProvider.HelpNamespace);
        }
    }

    internal class COEServiceHelper
    {
        static COEService _COEService = null;

        private static volatile COEServiceHelper coeServiceHelper;
        private static object syncLockObject = new object();

        public static COEService COEService
        {
            get
            {
                return _COEService;
            }
        }

        private COEServiceHelper(Spotfire.Dxp.Application.AnalysisApplication theAnalysisApplication)
        {
            _COEService = theAnalysisApplication.GetService<COEService>();
        }

        public static COEServiceHelper Instance(Spotfire.Dxp.Application.AnalysisApplication theAnalysisApplication)
        {
            lock (syncLockObject)
            {
                if (coeServiceHelper == null)
                {
                    coeServiceHelper = new COEServiceHelper(theAnalysisApplication);
                }
                return coeServiceHelper;
            }
        }
    }

    internal class MaxRowSettingsExceededException : Exception
    {
        public MaxRowSettingsExceededException()
            : base()
        {

        }

        public MaxRowSettingsExceededException(string message)
            : base(message)
        {

        }

        public MaxRowSettingsExceededException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
