using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.COESearchService;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using CambridgeSoft.COE.Framework.COELoggingService;
using System.Reflection;
using CambridgeSoft.COE.Framework.COEHitListService;

namespace CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService
{
    [Serializable()]
    public class GenericBO : BusinessBase<GenericBO>
    {

        #region Variables
        [NonSerialized, NotUndoable]
        private DAL _COEChemBioVizDAL = null;
        [NonSerialized, NotUndoable]
        private DALFactory _dalFactory = new DALFactory();
        [NonSerialized]
        private const string _serviceName = "COEChemBioViz";
        [NonSerialized]
        private string _specialFolder = COEConfigurationBO.ConfigurationBaseFilePath +
                                                 @"SimulationFolder\Framework\";
        [NonSerialized]
        COELog _log = COELog.GetSingleton(_serviceName);

        private List<string> _changedProperties = new List<string>();

        private string _appName = string.Empty; //TODO: Allow an application to set this value

        private int _id;
        private DataSet _dataset;
        private SearchCriteria _searchCriteria;
        private byte[] _previousSearchCriteriaMD5;
        private OrderByCriteria _orderByCriteria;
        private byte[] _previousOrderByCriteriaMD5;
        private ResultsCriteria _resultsCriteria;
        private byte[] _previousResultsCriteriaMD5;
        private int _dataViewId;
        private COEDataView _dataView;
        private byte[] _previousDataViewMD5;
        private int _previousDataViewId;
        private PagingInfo _pagingInfo;
        private byte[] _previousPagingInfoMD5;
        private HitListInfo _lastSearchHitlist;
        private HitListInfo _hitListInfo;
        private HitListInfo _hitListToRestore;
        private HitListInfo _hitlistToRefine;
        private int _currentRecordIndex;
        private int _pageSize;
        private int _actualRecordCount;
        private int _totalRecordCount;
        private int _maxRecordCount;
        COESearch _searchService;
        private DataSet _filteredDataSet;
        private bool _returnPartialHitlist = false;
        private int _commitSize = -1;
        private int _sessionSearchCount;
        private int _formGroupID;
        private bool _saveQueryHistory;
        private bool _browse;
        private SearchCriteria _recoveredSearchCriteria;

        [NonSerialized]
        private List<SearchCriteria> _originalSearchCriterias = new List<SearchCriteria>();
        [NonSerialized]
        private List<ResultsCriteria> _originalDetailsResultsCriterias = new List<ResultsCriteria>();
        [NonSerialized]
        private List<ResultsCriteria> _originalListsResultsCriterias = new List<ResultsCriteria>();

        private FormGroup.CurrentFormEnum _currentFormType;
        private int _currentFormIndex;
        private int _databaseRecordCount;
        private int _markedHitsMax;
        private int _expireHitlistHistoryDays;
        private int _expireQueryHistoryDays;

        private bool _allowFullScan;
        private bool _keepRecordCountSynchronized = false;

        private string _markedColumnName;
        #endregion

        #region Business Properties
        public int MarkedHitsMax
        {
            set
            {
                _markedHitsMax = value;
            }
        }

        public int ExpireHitlistHistoryDays
        {
            set
            {
                _expireHitlistHistoryDays = value;
            }
        }

        public int ExpireQueryHistoryDays
        {
            set
            {
                _expireQueryHistoryDays = value;
            }
        }
        public bool Browse
        {
            get { return _browse; }
            set
            {
                if(_browse != value)
                {
                    _browse = value;
                    PropertyHasChanged();
                }
            }
        }

        public bool AllowFullScan
        {
            get
            {
                return _allowFullScan;
            }
            set
            {
                _allowFullScan = value;
            }
        }

        public int HitListID
        {
            get
            {
                int retVal = -1;
                if (_hitListInfo != null)
                {
                    if (_hitListInfo.HitListID > 0)
                        retVal = _hitListInfo.HitListID;
                }
                return retVal;
            }
        }

        /// <summary>
        /// Stores the last performed search hitlist.
        /// </summary>
        public HitListInfo LastSearchHitList
        {
            get 
            {
                return _lastSearchHitlist;
            }
        }

        public HitListInfo CurrentHitList
        {
            get
            {
                return _hitListInfo;
            }
            set
            {
                _hitListInfo = value;
            }
        }

        public HitListInfo HitListToRefine
        {
            get
            {
                return _hitlistToRefine;
            }
            set
            {
                _hitlistToRefine = value;
            }
        }

        public bool ReturnPartialHitlist
        {
            get
            {
                return _returnPartialHitlist;
            }
            set
            {
                _returnPartialHitlist = value;
            }
        }

        public bool SaveQueryHistory
        {
            get
            {
                return _saveQueryHistory;
            }
            set
            {
                _saveQueryHistory = value;
            }
        }

        public int CommitSize
        {
            get
            {
                return _commitSize;
            }
            set
            {
                _commitSize = value;
            }
        }

        [System.ComponentModel.DataObjectField(true, true)]
        public int ID
        {
            get
            {
                CanReadProperty(true);
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Page Size [1...n]
        /// </summary>
        public int PageSize
        {
            get
            {
                CanReadProperty(true);
                return _pageSize;
            }
            set
            {
                CanWriteProperty(true);

                if (value < 1)
                    value = 1;

                if (_pageSize != value)
                {
                    _pageSize = value;
                    PropertyHasChanged();
                    AdjustPagingInfo();
                }
            }
        }
        /// <summary>
        /// Number of records in current page. May not coincide with PageSize.
        /// </summary>
        public int RecordsInPage
        {
            get
            {
                CanReadProperty(true);
                EnsureSearch();
                return _actualRecordCount;
            }
        }
        /// <summary>
        /// Total number of records fetched so far.
        /// </summary>
        public int TotalRecordCount
        {
            get
            {
                CanReadProperty(true);
                EnsureSearch();

                return _totalRecordCount;
            }
        }
        /// <summary>
        /// Upper limit or records to be fetched.
        /// </summary>
        public int MaxRecordCount
        {
            set
            {
                CanWriteProperty(true);

                if (value != _maxRecordCount)
                {
                    _maxRecordCount = value;
                    PropertyHasChanged();
                }
            }
            get
            {
                CanReadProperty(true);
                return _maxRecordCount;
            }
        }


        /// <summary>
        /// Current Record Index [1...n]
        /// </summary>
        public int CurrentRecordIndex
        {
            get
            {
                CanReadProperty(true);
                EnsureSearch();
                return _currentRecordIndex;
            }
            set
            {
                CanWriteProperty(true);

                if (value < 1)
                    value = 1;
                else if (value >= _totalRecordCount)
                    value = _totalRecordCount;

                if (_currentRecordIndex != value)
                {
                    _currentRecordIndex = value;
                    PropertyHasChanged();
                    AdjustPagingInfo();
                }
            }
        }

        public DataSet Dataset
        {
            get
            {
                CanReadProperty(true);

                EnsureSearch();
                return _dataset;
            }
            set
            {
                CanWriteProperty(true);
                _dataset = value;
                PropertyHasChanged();
            }
        }

        public int SessionSearchCount
        {
            get
            {
                return _sessionSearchCount;
            }
        }

        private void EnsureSearch()
        {
            this.Search();
        }

        public SearchCriteria SearchCriteria
        {
            get
            {
                CanReadProperty(true);
                return _searchCriteria;
            }

            set
            {
                CanWriteProperty(true);

                if (value == null)
                    value = new SearchCriteria();

                if (!AreEqualXml(_searchCriteria.ToString(), value.ToString()))
                {
                    _searchCriteria = value;
                    PropertyHasChanged();
                }
            }
        }

        public OrderByCriteria OrderByCriteria
        {
            get
            {
                CanReadProperty(true);
                return _orderByCriteria;
            }

            set
            {
                CanWriteProperty(true);

                if(value == null)
                    value = new OrderByCriteria();

                if(!AreEqualXml(_orderByCriteria.ToString(), value.ToString()))
                {
                    _orderByCriteria = value;
                    PropertyHasChanged();
                }
            }
        }

        public ResultsCriteria ResultsCriteria
        {
            get
            {
                CanReadProperty(true);
                return _resultsCriteria;
            }
            set
            {
                CanWriteProperty(true);

                if (value == null)
                    value = new ResultsCriteria();

                if (!AreEqualXml(_resultsCriteria.ToString(), value.ToString()))
                {
                    _resultsCriteria = value;
                    GetMarkedColumnName();
                    PropertyHasChanged();
                }
            }
        }

        public string GetMarkedColumnName()
        {
            foreach (ResultsCriteria.ResultsCriteriaTable table in _resultsCriteria.Tables)
            {
                foreach (ResultsCriteria.IResultsCriteriaBase criteria in table.Criterias)
                {
                    if (criteria is ResultsCriteria.Marked)
                    {
                        if (!string.IsNullOrEmpty(((ResultsCriteria.Marked)criteria).Alias))
                        {
                            _markedColumnName = ((ResultsCriteria.Marked)criteria).Alias;
                        }
                        else
                            _markedColumnName = "Marked";
                    }
                }
            }

            return _markedColumnName;
        }

        public COEDataView DataView
        {
            get
            {
                CanReadProperty(true);
                return _dataView;
            }
            set
            {
                CanWriteProperty(true);

                if (value == null)
                    value = new COEDataView();

                if (!AreEqualXml(_dataView.ToString(), value.ToString()))
                {
                    _dataView = value;
                    _dataViewId = -1;
                    _databaseRecordCount = GetDatabaseRecordCount();
                    PropertyHasChanged();
                }
            }
        }

        public int DataViewId
        {
            get
            {
                CanReadProperty(true);
                if (_dataViewId < 0 && this.DataView != null)
                    _dataViewId = this.DataView.DataViewID;

                return _dataViewId;
            }
            set
            {
                CanWriteProperty(true);

                if (_dataViewId != value)
                {
                    _dataViewId = value;
                    _dataView = null;
                    PropertyHasChanged();
                }
            }
        }

        public int FormGroupID
        {
            get
            {
                return _formGroupID;
            }
        }

        public PagingInfo PagingInfo
        {
            get
            {
                CanReadProperty(true);
                return _pagingInfo;
            }
            set
            {
                CanWriteProperty(true);

                if (value == null)
                    value = new PagingInfo();

                if (!AreEqualXml(_pagingInfo.ToString(), value.ToString()))
                {
                    _pagingInfo = value;
                    PropertyHasChanged();
                }
            }
        }

        private bool _createNewHitList = true;
        public bool CreateNewHitList
        {
            get
            {
                return _createNewHitList;
            }
            set
            {
                _createNewHitList = value;
            }
        }

        public HitListInfo HitListToRestore
        {
            set
            {
                if (_hitListToRestore != value)
                {
                    _hitListToRestore = value;
                    _totalRecordCount = value.RecordCount;
                    EnsureSearch();
                    PropertyHasChanged();
                }
            }
        }

        public bool KeepRecordCountSyncrhonized
        {
            set
            {
                _keepRecordCountSynchronized = value;
            }
        }

        public string ApplicationName
        {
            get
            {
                CanReadProperty(true);
                return _appName;
            }
            set
            {
                CanWriteProperty(true);

                if (value == null)
                    value = string.Empty;

                if (_appName != value)
                {
                    _appName = value;
                    PropertyHasChanged();
                }
            }
        }

        private COESearch SearchService
        {
            get
            {
                if (_searchService == null)
                    _searchService = new COESearch();

                return _searchService;
            }
        }

        public COEHitListBO MarkedHitList
        {
            get
            {
                return COEHitListBO.GetMarkedHitList(this.DataView.Database, COEUser.Name, this.DataView.DataViewID);
            }
        }

        public int MarkedHitListID
        {
            get
            {
                return MarkedHitList.ID;
            }
        }

        public DataSet FilteredDataSet
        {
            get
            {
                EnsureSearch();
                EnsureChildTables();
                return this._filteredDataSet;
            }
        }

        /// <summary>
        /// Requires search includes Requires GetHitlist, so those properties verified in RequiresGetHitList should not be duplicated here.
        /// </summary>
        private bool RequiresDoSearch
        {
            get
            {
                return this.RequiresGetHitList ||
                       this.RequiresGetData;
            }
        }

        private bool RequiresGetHitList
        {
            get
            {
                
                if(_hitListToRestore != null)
                    return false;
                else if (_keepRecordCountSynchronized)
                    return true;
                else
                    return !_browse && (_hitListInfo == null ||
                        !CompareByteArray(_previousSearchCriteriaMD5, ComputeXMLMD5(this.IsSearchCriteriaEmpty(_searchCriteria) ? null : _searchCriteria.ToString())) ||
                        (UseDataViewId && _previousDataViewId != _dataViewId) ||
                        (!UseDataViewId && (_previousDataViewMD5 == null || !CompareByteArray(_previousDataViewMD5, ComputeXMLMD5(_dataView.ToString())))));
            }
        }

        private bool RequiresGetData
        {
            get
            {
                return _dataset == null || _keepRecordCountSynchronized ||
                        _previousResultsCriteriaMD5 == null || !CompareByteArray(_previousResultsCriteriaMD5, ComputeXMLMD5(_resultsCriteria.ToString())) ||
                        _previousPagingInfoMD5 == null || !CompareByteArray(_previousPagingInfoMD5, ComputeXMLMD5(_pagingInfo.ToString())) ||
                        _changedProperties.Contains("HitListToRestore") || _changedProperties.Contains("Browse") ||
                        !CompareByteArray(_previousOrderByCriteriaMD5, ComputeXMLMD5(_orderByCriteria.ToString()));
            }
        }

        private bool RequiresFilterChildTables
        {
            get
            {
                return _filteredDataSet == null ||
                        _changedProperties.Contains("Dataset") ||
                        _previousPagingInfoMD5 == null || !CompareByteArray(_previousPagingInfoMD5, ComputeXMLMD5(_pagingInfo.ToString())) ||
                        _changedProperties.Contains("CurrentRecordIndex");
            }
        }

        public bool RequiresRefreshingHitList
        {
            get
            {
                return this.ReturnPartialHitlist && !RequiresGetHitList && _hitListInfo.CurrentRecordCount != _hitListInfo.RecordCount;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return _actualRecordCount < _pagingInfo.End - _pagingInfo.Start;
            }
        }

        public FormGroup.CurrentFormEnum CurrentFormType
        {
            get
            {
                return _currentFormType;
            }
            set
            {
                _currentFormType = value;
                UpdateCurrentResultsCriteria();
            }
        }

        public int CurrentFormIndex
        {
            get
            {
                return _currentFormIndex;
            }
            set
            {
                _currentFormIndex = value;
                UpdateCurrentResultsCriteria();
            }
        }


        public int DatabaseRecordCount
        {
            get 
            {
                if(_keepRecordCountSynchronized)
                    _databaseRecordCount = this.GetDatabaseRecordCount();
                return _databaseRecordCount; 
            }
        }
        #endregion

        #region Business Methods
        protected override object GetIdValue()
        {
            return _id;
        }

        private void EnsureChildTables()
        {
            try
            {
                if (this.RequiresFilterChildTables)
                {
                    this._filteredDataSet = new DataSet();

                    //Add Parent table to Tables List.

                    DataTable parentTable = _dataset.Tables[0].Clone();
                    _filteredDataSet.Tables.Add(parentTable);

                    _filteredDataSet.Tables[0].ImportRow(_dataset.Tables[0].Rows[this.CurrentRecordIndex - this.PagingInfo.Start]);

                    for (int childTableindex = 1; childTableindex < _dataset.Tables.Count; childTableindex++)
                    {
                        DataTable currentChildTable = _dataset.Tables[childTableindex].Clone();

                        _filteredDataSet.Tables.Add(currentChildTable);

                        DataRow[] childTableRows = GetChildTableRows(childTableindex);

                        //if null the relation doesn't exist.
                        if (childTableRows != null)
                        {
                            foreach (DataRow currentRow in childTableRows)
                            {
                                DataRow currentChildTableRow = currentChildTable.NewRow();
                                currentChildTableRow.ItemArray = currentRow.ItemArray;
                                currentChildTable.Rows.Add(currentChildTableRow);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private DataRow[] GetChildTableRows(int childTableIndex)
        {
            foreach (DataRelation currentRelation in this._dataset.Relations)
                if (currentRelation.ChildTable.TableName == _dataset.Tables[childTableIndex].TableName)
                {
                    int parentRowIndex = Math.Max(0, _currentRecordIndex - _pagingInfo.Start);
                    if (parentRowIndex < currentRelation.ParentTable.Rows.Count)
                    {
                        DataRow parentRow = currentRelation.ParentTable.Rows[parentRowIndex];
                        DataRow[] rows = parentRow.GetChildRows(currentRelation);
                        return rows;
                    }
                    else
                        return new DataRow[0];
                }

            return null;
        }


        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (!_changedProperties.Contains(propertyName))
                _changedProperties.Add(propertyName);
        }

        private void AdjustPagingInfo()
        {
            if (_pagingInfo.Start > _currentRecordIndex || _currentRecordIndex >= _pagingInfo.End || _changedProperties.Contains("PageSize"))
            {
                int currentPageIndex = (_currentRecordIndex - 1) / _pageSize;

                _pagingInfo.Start = (currentPageIndex * _pageSize) + 1;
                _pagingInfo.End = _pagingInfo.Start + _pageSize;
                _pagingInfo.RecordCount = _pageSize;
                PropertyHasChanged("PagingInfo");
            }
        }

        private bool CompareByteArray(byte[] operand1, byte[] operand2)
        {
            //if both are the same reference or null return true.
            if (operand1 == operand2)
                return true;

            if(operand1 == null || operand2 == null)
                return false;

            if (operand1.Length == operand2.Length)
                for (int index = 0; index < operand1.Length; index++)
                    if (operand1[index] != operand2[index])
                        return false;

            return true;
        }

        private byte[] ComputeXMLMD5(string value)
        {
            if (value != null)
            {
                // "Normalize" the xml.
                XmlDocument document = new XmlDocument();
                document.LoadXml(value);

                return CambridgeSoft.COE.Framework.Common.Utilities.ComputeHash(document.OuterXml);
            }
            return null;
        }


        private bool AreEqualXml(string operand1, string operand2)
        {
            XmlDocument originalSearchCriteria = new XmlDocument();
            originalSearchCriteria.LoadXml(operand1);

            XmlDocument newSearchCriteria = new XmlDocument();
            newSearchCriteria.LoadXml(operand2);

            return originalSearchCriteria.OuterXml == newSearchCriteria.OuterXml;
        }

        private bool IsSearchCriteriaEmpty(SearchCriteria searchCriteria)
        {
            bool isEmpty = true;
            foreach (SearchCriteria.SearchExpression expr in searchCriteria.Items)
            {
                if (expr is SearchCriteria.SearchCriteriaItem)
                {
                    SearchCriteria.SearchCriteriaItem item = expr as SearchCriteria.SearchCriteriaItem;

                    if (!string.IsNullOrEmpty(item.Criterium.Value))
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }
            return isEmpty;
        }

        private SearchCriteria RemoveUnusedFieldsFromSearchCriteria(SearchCriteria originalSearchCriteria)
        {
            SearchCriteria searchCriteria = new SearchCriteria();

            foreach (SearchCriteria.SearchExpression expr in originalSearchCriteria.Items)
            {
                if (expr is SearchCriteria.SearchCriteriaItem)
                {
                    SearchCriteria.SearchCriteriaItem item = expr as SearchCriteria.SearchCriteriaItem;

                    if (!string.IsNullOrEmpty(item.Criterium.Value))
                    {
                        searchCriteria.Items.Add(item);
                    }
                }
            }


            return searchCriteria;
        }

        public void DoSearch()
        {
            if (RequiresDoSearch)
            {
                SearchResponse response = SearchService.DoSearch(RemoveUnusedFieldsFromSearchCriteria(_searchCriteria),
                                                            _resultsCriteria.Clone(),
                                                            _pagingInfo,
                                                            _dataView,
                                                            "NO", 
                                                            _orderByCriteria);
                this.Dataset = response.ResultsDataSet;

                _previousSearchCriteriaMD5 = ComputeXMLMD5(_searchCriteria.ToString());
                _previousResultsCriteriaMD5 = ComputeXMLMD5(_resultsCriteria.ToString());
                _previousOrderByCriteriaMD5 = ComputeXMLMD5(_orderByCriteria.ToString());

                if (UseDataViewId)
                    _previousDataViewId = _dataViewId;
                else
                    _previousDataViewMD5 = ComputeXMLMD5(_dataView.ToString());

                _previousPagingInfoMD5 = ComputeXMLMD5(_pagingInfo.ToString());

                _changedProperties.Remove("SearchCriteria");
                _changedProperties.Remove("DataView");
                _changedProperties.Remove("ResultsCriteria");
                _changedProperties.Remove("PagingInfo");
                _changedProperties.Remove("Dataset");
                _changedProperties.Remove("OrderByCriteria");
            }
        }

        [Transactional(TransactionalTypes.Manual)]
        public void Search()
        {
            if (_recoveredSearchCriteria != null)
            {
                ReplaceSearchCriteriaWithRecovered();
            }

            if(IsSearchCriteriaEmpty(this.SearchCriteria) && !AllowFullScan && _hitListToRestore == null && RequiresGetHitList)
                throw new Exception("You must specify at least one search criterium");
            if(!IsSearchCriteriaEmpty(this.SearchCriteria))
            {
                if(ReturnPartialHitlist)
                    this.GetPartialHitList();
                else
                    this.GetHitList();
            }

            if (_hitListToRestore != null)
            {
                if (this.MarkedHitListID == _hitListToRestore.HitListID && _hitListToRestore.HitListID != _hitListInfo.HitListID && _createNewHitList)
                {
                    COEHitListBO recentHL = COEHitListOperationManager.UnionHitLists(_hitListToRestore, new HitListInfo(), this.DataViewId);
                    _hitListToRestore = _hitListInfo = recentHL.HitListInfo;
                }
                else
                    _hitListInfo = _hitListToRestore;
            }
            else if(IsSearchCriteriaEmpty(this.SearchCriteria))
            {
                //Do a Browse instead of using hitlist
                if(_hitListInfo == null)
                    _hitListInfo = new HitListInfo();

                if(Browse)
                    _hitListInfo.HitListID = 0;
            }

            GetData(_hitListInfo);

            if (_keepRecordCountSynchronized)
                _keepRecordCountSynchronized = false;
        }

        private bool UseDataViewId
        {
            get
            {
                return _dataViewId >= 0;
            }
        }

        public HitListInfo GetHitList()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";

            _log.LogStart(methodSignature);
            if (RequiresGetHitList)
            {
                SearchCriteria sc = RemoveUnusedFieldsFromSearchCriteria(_searchCriteria);
                if(UseDataViewId)
                    _hitListInfo = SearchService.GetHitList(sc, _dataViewId, _hitlistToRefine);
                else
                    _hitListInfo = SearchService.GetHitList(sc, _dataView, _hitlistToRefine);

                _lastSearchHitlist = _hitListInfo;

                UpdateSearchCriteriaFormGroup(sc.SearchCriteriaID);

                _totalRecordCount = _hitListInfo.RecordCount;
                _currentRecordIndex = 1;

                //Dataset = null;
                _filteredDataSet = null;

                _previousSearchCriteriaMD5 = ComputeXMLMD5(_searchCriteria.ToString());

                if (UseDataViewId)
                    _previousDataViewId = _dataViewId;
                else
                    _previousDataViewMD5 = ComputeXMLMD5(_dataView.ToString());

                _changedProperties.Remove("SearchCriteria");
                _changedProperties.Remove("DataView");
                _sessionSearchCount++;
            }
            else
                _log.Log(methodSignature + "does not require get hitlist");

            _log.LogEnd(methodSignature);

            return _hitListInfo;
        }

        public int RefreshHitList()
        {
            if (RequiresRefreshingHitList)
            {
                if (RequiresGetHitList)
                    this.GetPartialHitList();

                _hitListInfo = SearchService.GetHitListProgress(_hitListInfo, _dataView);
                _totalRecordCount = _hitListInfo.CurrentRecordCount;

                return _totalRecordCount;
            }

            return _totalRecordCount;
        }

        public HitListInfo GetPartialHitList()
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            try
            {
                _log.LogStart(methodSignature);

                if (RequiresGetHitList)
                {
                    SearchService.MaxRecordCount = _maxRecordCount;

                    SearchCriteria sc = RemoveUnusedFieldsFromSearchCriteria(_searchCriteria);
                    if (CommitSize > 0)
                        _hitListInfo = SearchService.GetPartialHitList(sc, _dataView, CommitSize, _hitlistToRefine);
                    else
                        _hitListInfo = SearchService.GetPartialHitList(sc, _dataView, _hitlistToRefine);

                    _lastSearchHitlist = _hitListInfo;

                    UpdateSearchCriteriaFormGroup(sc.SearchCriteriaID);

                    _totalRecordCount = _hitListInfo.CurrentRecordCount;
                    _currentRecordIndex = 1;

                    Dataset = null;
                    _filteredDataSet = null;

                    _previousSearchCriteriaMD5 = ComputeXMLMD5(_searchCriteria.ToString());

                    if (UseDataViewId)
                        _previousDataViewId = _dataViewId;
                    else
                        _previousDataViewMD5 = ComputeXMLMD5(_dataView.ToString());

                    _changedProperties.Remove("SearchCriteria");
                    _changedProperties.Remove("DataView");
                    _sessionSearchCount++;
                }

                return _hitListInfo;
            }
            finally
            {
                _log.LogEnd(methodSignature);
            }
        }

        public DataSet GetData(HitListInfo hitList)
        {
            string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
            try
            {
                _log.LogStart(methodSignature);

                if (this.RequiresGetData && _resultsCriteria.Tables.Count > 0)
                {                    

                    PagingInfo pagingInfo = new PagingInfo();
                    pagingInfo.GetFromXML(_pagingInfo.ToString());
                    pagingInfo.HitListID = hitList.HitListID;
                    pagingInfo.HitListType = hitList.HitListType;
                    pagingInfo.AddToRecent = CreateNewHitList;

                    if (UseDataViewId)
                        Dataset = SearchService.GetData(_resultsCriteria.Clone(), pagingInfo, _dataViewId, "NO", _orderByCriteria);
                    else
                        Dataset = SearchService.GetData(_resultsCriteria.Clone(), pagingInfo, _dataView, "NO", _orderByCriteria);

                    if(_browse)
                    {
                        _totalRecordCount = SearchService.GetFastRecordCount(_dataView);
                        _hitListInfo.IsExactRecordCount = false;
                    }

                    _actualRecordCount = pagingInfo.RecordCount;

                    _previousResultsCriteriaMD5 = ComputeXMLMD5(_resultsCriteria.ToString());
                    _previousOrderByCriteriaMD5 = ComputeXMLMD5(_orderByCriteria.ToString());

                    if (UseDataViewId)
                        _previousDataViewId = _dataViewId;
                    else
                        _previousDataViewMD5 = ComputeXMLMD5(_dataView.ToString());

                    _previousPagingInfoMD5 = ComputeXMLMD5(_pagingInfo.ToString());

                    _changedProperties.Remove("OrderByCriteria");
                    _changedProperties.Remove("ResultsCriteria");
                    _changedProperties.Remove("PagingInfo");
                    _changedProperties.Remove("DataView");
                    _changedProperties.Remove("Dataset");
                    _changedProperties.Remove("HitListToRestore");
                    _changedProperties.Remove("Browse");
                    _changedProperties.Remove("PageSize");
					if(_keepRecordCountSynchronized)
						_databaseRecordCount = this.GetDatabaseRecordCount();
                }
                else
                    _log.Log(methodSignature + "does not require GetData");
                
                return _dataset;
            }
            finally
            {
                _log.LogEnd(methodSignature);
            }
        }

        public void MarkHits(int[] hitIDs)
        {
            throw new Exception("Unimplemented method exception");
        }

        public void UnMarkHits(int[] hitsIDs)
        {
            throw new Exception("Unimplemented method exception");
        }

        public bool MarkHit(int hitID, string columnIDBindingExpression)
        {
            if(MarkedHitList.NumHits < _markedHitsMax)
            {
                columnIDBindingExpression = BindingExpressionToColumnName(columnIDBindingExpression);
                MarkedHitList.MarkHit(hitID);
                _dataset.Tables[0].Select(columnIDBindingExpression + "=" + hitID)[0][_markedColumnName] = 1;
                return true;
            }
            else
                return false;
        }

        public void UnMarkHit(int hitID, string columnIDBindingExpression)
        {
            columnIDBindingExpression = BindingExpressionToColumnName(columnIDBindingExpression);
            MarkedHitList.UnMarkHit(hitID);
            if(_dataset.Tables[0].Select(columnIDBindingExpression + "=" + hitID).Length > 0)
                _dataset.Tables[0].Select(columnIDBindingExpression + "=" + hitID)[0][_markedColumnName] = 0;
        }

        public void UnMarkAllHits()
        {
            try
            {
                MarkedHitList.UnMarkAllHits();
                for (int i = 0; i < this.Dataset.Tables[0].Rows.Count; i++)
                {
                    this.Dataset.Tables[0].Rows[i][_markedColumnName] = 0;
                }
            }
            catch 
            {
                throw;
            }
        }

        public void MarkAllHits()
        {
            try
            {
                if(_hitListInfo.HitListID != MarkedHitList.HitListInfo.HitListID)
                {
                    int recordsAffected = MarkedHitList.MarkAllHits(_hitListInfo.HitListID, MarkedHitList.HitListInfo.HitListID, _hitListInfo.HitListType, _markedHitsMax - MarkedHitList.NumHits);
                    for(int i = 0; i < this.Dataset.Tables[0].Rows.Count && recordsAffected > 0; i++)
                    {
                        if(this.Dataset.Tables[0].Rows[i][_markedColumnName].ToString() == "0")
                        {
                            this.Dataset.Tables[0].Rows[i][_markedColumnName] = 1;
                            recordsAffected--;
                        }
                    }
                }
            }
            catch 
            {
                throw;
            }
        }      

        private string BindingExpressionToColumnName(string columnIDBindingExpression)
        {
            return columnIDBindingExpression.Replace("this['", string.Empty).Replace("']", string.Empty);
        }

        public int GetMarkedCount()
        {
            return this.MarkedHitList.NumHits;
        }

        private void UpdateSearchCriteriaFormGroup(int searchCriteriaID)
        {
            if (_saveQueryHistory)
            {
                COESearchCriteriaBO searchCriteriaBO = COESearchCriteriaBO.Get(SearchCriteriaType.TEMP, searchCriteriaID);
                searchCriteriaBO.FormGroup = _formGroupID;
                searchCriteriaBO.Update();
            }
        }

        private void FillOriginalSearchCriterias(FormGroup formGroup)
        {
            _originalSearchCriterias.Clear();

            for (int i = 0; i < formGroup.QueryForms.Displays.Count; i++)
            {
                FormGroup.QueryDisplay queryForm = (FormGroup.QueryDisplay)formGroup.QueryForms.Displays[i];
                _originalSearchCriterias.Add(SearchFormGroupAdapter.GetSearchCriteria(queryForm));
            }
        }
        private void FillDataView(int dataViewID)
        {
            _dataView = SearchFormGroupAdapter.GetDataView(dataViewID);
        }

        private void FillOriginalResultsCriterias(FormGroup formGroup)
        {
            _originalDetailsResultsCriterias.Clear();

            for (int i = 0; i < formGroup.DetailsForms.Displays.Count; i++)
            {
                FormGroup.DetailsDisplay detailForm = (FormGroup.DetailsDisplay)formGroup.DetailsForms.Displays[i];
                _originalDetailsResultsCriterias.Add(SearchFormGroupAdapter.GetResultsCriteria(detailForm));
            }

            _originalListsResultsCriterias.Clear();

            for (int i = 0; i < formGroup.ListForms.Displays.Count; i++)
            {
                FormGroup.ListDisplay listForm = (FormGroup.ListDisplay)formGroup.ListForms.Displays[i];
                _originalListsResultsCriterias.Add(SearchFormGroupAdapter.GetResultsCriteria(listForm));
            }
        }

        public List<string> RecoverSearchCriteria(int searchCriteriaIDToLoad, SearchCriteriaType type)
        {
            List<string> errorMessages = new List<string>();
            COESearchCriteriaBO scbo = COESearchCriteriaBO.Get(type, searchCriteriaIDToLoad);
            SearchCriteria newSearchCriteria = new SearchCriteria();
            newSearchCriteria.GetFromXML(_originalSearchCriterias[CurrentFormIndex].ToString());

            foreach (SearchCriteria.SearchCriteriaItem boItem in scbo.SearchCriteria.Items)
            {
                if (boItem.ID != 0)
                {
                    int j = 0;
                    bool recovered = false;
                    foreach (SearchCriteria.SearchCriteriaItem formItem in _originalSearchCriterias[CurrentFormIndex].Items)
                    {

                        if (boItem.FieldId == formItem.FieldId && boItem.TableId == formItem.TableId && boItem.Criterium.GetType() == formItem.Criterium.GetType())
                        {
                            newSearchCriteria.Items[j] = boItem;
                            recovered = true;
                        }
                        j++;

                    }

                    if (!recovered)
                        errorMessages.Add("The searchcriteria for tableid " + boItem.TableId + "  and fieldid " + boItem.FieldId + " cannot be set.");
                }
            }
            this.SearchCriteria = newSearchCriteria;
            _recoveredSearchCriteria = newSearchCriteria;
            return errorMessages;
        }

        private void ReplaceSearchCriteriaWithRecovered()
        {

            SearchCriteria newSearchCriteria = new SearchCriteria();
            newSearchCriteria.GetFromXML(this.SearchCriteria.ToString());

            foreach (SearchCriteria.SearchCriteriaItem boItem in _recoveredSearchCriteria.Items)
            {
                if (boItem.ID != 0)
                {
                    int j = 0;
                    foreach (SearchCriteria.SearchCriteriaItem formItem in this.SearchCriteria.Items)
                    {

                        if (boItem.FieldId == formItem.FieldId && boItem.TableId == formItem.TableId && boItem.Criterium.GetType() == formItem.Criterium.GetType())
                        {
                            newSearchCriteria.Items[j] = boItem;
                            ((SearchCriteria.SearchCriteriaItem)newSearchCriteria.Items[j]).Criterium.Value = formItem.Criterium.Value;
                        }
                        j++;
                    }
                }
            }

            this.SearchCriteria = newSearchCriteria;
            _recoveredSearchCriteria = null;
        }

        public void CleanSearchCriteria()
        {
            if(CurrentFormIndex >= _originalSearchCriterias.Count)
                this.SearchCriteria = _originalSearchCriterias[_originalSearchCriterias.Count - 1];
            else
                this.SearchCriteria = _originalSearchCriterias[CurrentFormIndex];
        }

        private void UpdateCurrentResultsCriteria()
        {
            switch (CurrentFormType)
            {
                case FormGroup.CurrentFormEnum.DetailForm:
                    if(CurrentFormIndex < _originalDetailsResultsCriterias.Count)
                        this.ResultsCriteria = _originalDetailsResultsCriterias[CurrentFormIndex];
                    else
                        this.ResultsCriteria = _originalDetailsResultsCriterias[_originalDetailsResultsCriterias.Count - 1];
                    break;
                case FormGroup.CurrentFormEnum.ListForm:
                    if(CurrentFormIndex < _originalListsResultsCriterias.Count)
                        this.ResultsCriteria = _originalListsResultsCriterias[CurrentFormIndex];
                    else
                        this.ResultsCriteria = _originalListsResultsCriterias[_originalListsResultsCriterias.Count - 1];
                    break;
            }
        }

        private int GetDatabaseRecordCount()
        {
            return this.SearchService.GetFastRecordCount(this.DataView);
        }

        public string UserNameExists(string name, int hits)
        {            
            string username = SearchService.GetUserName(name, hits);
            return username;
        }

        public int GetHits(int ID)
        {
            int hits = SearchService.GetHitsPerPage(ID);
            return hits;
        }
        #endregion

        #region Factory Methods

        private GenericBO()
        {
            _lastSearchHitlist = new HitListInfo();
            _lastSearchHitlist.HitListType = CambridgeSoft.COE.Framework.HitListType.TEMP;
            _lastSearchHitlist.HitListID = -1;

            this._dataViewId = -1;
            this._dataView = new COEDataView();
            this._searchCriteria = new SearchCriteria();
            this._resultsCriteria = new ResultsCriteria();
            this._pagingInfo = new PagingInfo();
            this._orderByCriteria = new OrderByCriteria();

            this._pagingInfo.Start = _pagingInfo.End = 1;
            this._pageSize = 1;
            this.CurrentRecordIndex = 1;
        }

        public void RefreshDatabaseRecordCount()
        {
            this.SearchService.RefreshDatabaseRecordCount(this.DataView);
        }

        public static GenericBO NewGenericBO()
        {
            return DataPortal.Create<GenericBO>();
        }

        public static GenericBO GetGenericBO(ResultsCriteria resultsCriteria, SearchCriteria searchCriteria, COEDataView dataView, PagingInfo pagingInfo)
        {
            if (!CanGetObject())
            {
                throw new System.Security.SecurityException("User not authorized to view a RegistryRecord");
            }

            GenericBO result = null;

            try
            {
                result = DataPortal.Fetch<GenericBO>(new COESearchManagerCriteria(resultsCriteria, searchCriteria, dataView, pagingInfo));
            }
            catch (DataPortalException exception)
            {
                throw exception;
                //result = null;
            }
            //result.MarkOld();
            //result.MarkClean();
            return result;
        }

        public static GenericBO GetGenericBO(string applicationName, int formGroupID)
        {
            if (!CanGetObject())
            {
                throw new System.Security.SecurityException("User not authorized to view a RegistryRecord");
            }

            GenericBO result = null;

            try
            {
                result = DataPortal.Fetch<GenericBO>(new Criteria(applicationName, formGroupID));
            }
            catch (DataPortalException exception)
            {
                throw exception;
                //result = null;
            }
            //result.MarkOld();
            //result.MarkClean();
            return result;
        }

        public static GenericBO GetGenericBO(string applicationName, FormGroup formGroup)
        {
            if (!CanGetObject())
            {
                throw new System.Security.SecurityException("User not authorized to view the Business Object");
            }

            GenericBO result = null;

            try
            {
                result = DataPortal.Fetch<GenericBO>(new FormCriteria(applicationName, formGroup));
            }
            catch (DataPortalException exception)
            {
                throw exception;
                //result = null;
            }
            //result.MarkOld();
            //result.MarkClean();
            return result;
        }
        #endregion

        #region Authorization Rules
        protected override void AddAuthorizationRules()
        {
            AuthorizationRules.AllowWrite(
              "GenericBO", "ADD_IDENTIFIER");
        }

        public static bool CanAddObject()
        {
            //return Csla.ApplicationContext.User.IsInRole("multiCompoundRegistryRecord");
            return true;
        }

        public static bool CanGetObject()
        {
            return true;
        }

        public static bool CanDeleteObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("multiCompoundRegistryRecord");
        }

        public static bool CanEditObject()
        {
            return true;
            //return Csla.ApplicationContext.User.IsInRole("multiCompoundRegistryRecord");
        }
        #endregion

        #region Criteria
        [Serializable()]
        private class COESearchManagerCriteria
        {
            private PagingInfo _pagingInfo;
            private SearchCriteria _searchCriteria;
            private ResultsCriteria _resultsCriteria;
            private COEDataView _dataView;

            public PagingInfo PagingInfo
            {
                get { return _pagingInfo; }
            }

            public SearchCriteria SearchCriteria
            {
                get { return _searchCriteria; }
            }

            public ResultsCriteria ResultsCriteria
            {
                get { return _resultsCriteria; }
            }

            public COEDataView DataView
            {
                get { return _dataView; }
            }

            public COESearchManagerCriteria(ResultsCriteria resultsCriteria, SearchCriteria searchCriteria, COEDataView dataview, PagingInfo pagingInfo)
            {
                _resultsCriteria = resultsCriteria;
                _pagingInfo = pagingInfo;
                _searchCriteria = searchCriteria;
                _dataView = dataview;
            }
        }

        [Serializable()]
        private class Criteria
        {
            private string _applicationName;
            private int _formGroupID;

            public string ApplicationName
            {
                get { return _applicationName; }
            }

            public int FormGroupID
            {
                get { return _formGroupID; }
            }

            public Criteria(string applicationName, int formGroupID)
            {
                _applicationName = applicationName;
                _formGroupID = formGroupID;
            }
        }

        [Serializable()]
        private class FormCriteria
        {
            private string _applicationName;
            private FormGroup _formGroup;

            public string ApplicationName
            {
                get { return _applicationName; }
            }

            public FormGroup FormGroup
            {
                get { return _formGroup; }
            }

            public FormCriteria(string applicationName, FormGroup formGroup)
            {
                _applicationName = applicationName;
                _formGroup = formGroup;
            }
        }
        #endregion

        #region DataAccess
        [RunLocal]
        private void DataPortal_Fetch(COESearchManagerCriteria criteria)
        {
            _resultsCriteria = criteria.ResultsCriteria;
            _searchCriteria = criteria.SearchCriteria;
            _pagingInfo = criteria.PagingInfo;
            _dataView = criteria.DataView;
            _databaseRecordCount = GetDatabaseRecordCount();
        }

        [RunLocal]
        private void DataPortal_Fetch(Criteria criteria)
        {
            this.ApplicationName = criteria.ApplicationName;
            _formGroupID = criteria.FormGroupID;
        }

        [RunLocal]
        private void DataPortal_Fetch(FormCriteria criteria)
        {
            this.ApplicationName = criteria.ApplicationName;
            _formGroupID = criteria.FormGroup.Id;
            FillOriginalSearchCriterias(criteria.FormGroup);
            FillDataView(criteria.FormGroup.DataViewId);
            FillOriginalResultsCriterias(criteria.FormGroup);
            _databaseRecordCount = GetDatabaseRecordCount();
        }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Insert() { }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_Update() { }

        [Transactional(TransactionalTypes.Manual)]
        protected override void DataPortal_DeleteSelf() { }

        [Transactional(TransactionalTypes.Manual)]
        private void DataPortal_Delete(Criteria criteria) { }
        #endregion

        #region DALLoader
        private void LoadDAL()
        {
            string databaseName = CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetDatabaseNameFromAppName(_appName);
            if (_dalFactory == null) { _dalFactory = new DALFactory(); }
            _dalFactory.GetDAL<DAL>(ref _COEChemBioVizDAL, _serviceName, databaseName, true);
        }
        #endregion

    }
}
