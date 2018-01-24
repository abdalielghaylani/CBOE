// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterModel.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451.
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace CBVNStructureFilter
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;    
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Data;
    using Spotfire.Dxp.Framework.ApplicationModel;
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.Persistence;    
    using Spotfire.Dxp.Framework;
    using log4net;
    using Spotfire.Dxp.Application.Visuals;
    using CBVNStructureFilterSupport.Framework;    

    [Serializable]
    [PersistenceVersion(9, 1)]
    internal class StructureFilterModel : DocumentNode, IStructureEditModel, IStructureSearchModel,
                                          IStructureFilterModel
    {
        [DllImport("user32.dll")]
        protected static extern IntPtr GetForegroundWindow();

        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureFilterModel));

        private readonly UndoableCrossReferenceProperty<DataTable> _dataTableReference;
        private readonly UndoableCrossReferenceProperty<DataColumn> _dataColumnReference;
        private readonly UndoableProperty<string> _showHydrogens;
        private readonly UndoableProperty<StructureFilterSettings> _filterSettings;
        private readonly UndoableProperty<string> _cdxStructure;
        private readonly UndoableProperty<string> _rGroupStructure;
        private readonly UndoableProperty<StructureStringType> _rGroupStructureType;
        private readonly UndoableProperty<bool> _rGroupNickNames;
        private readonly UndoableProperty<bool> _savedSearchList;

        public const int DefaultPercentSimilarity = 90;

        public const StructureFilterSettings.FilterModeEnum DefaultSearchType =
            StructureFilterSettings.FilterModeEnum.SubStructure;

        public const StructureFilterSettings.SimularityModeEnum DefaultSimilarityMode =
            StructureFilterSettings.SimularityModeEnum.GreaterThan;

        private const string DefaultShowHydrogens = "Off";
        public const string DefaultStructureString = "";

        #endregion

        #region Constructors and Destructors

        internal StructureFilterModel()
        {
            CreateProperty(PropertyNames.DataTableReference, out _dataTableReference, null);
            CreateProperty(PropertyNames.DataColumnReference, out _dataColumnReference, null);
            CreateProperty(PropertyNames.ShowHydrogens, out _showHydrogens, DefaultShowHydrogens);
            CreateProperty(PropertyNames.FilterSettings, out _filterSettings, null);
            CreateProperty(PropertyNames.CDXStructure, out _cdxStructure, String.Empty);
            CreateProperty(PropertyNames.RGroupStructure, out _rGroupStructure, String.Empty);
            CreateProperty(PropertyNames.RGroupStructureType, out _rGroupStructureType, StructureStringType.Unknown);
            CreateProperty(PropertyNames.RGroupNickNames, out _rGroupNickNames, false);
            CreateProperty(PropertyNames.SavedSearchList, out _savedSearchList, false);
        }

        protected StructureFilterModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DeserializeProperty(info, context, PropertyNames.DataTableReference, out _dataTableReference);
            DeserializeProperty(info, context, PropertyNames.DataColumnReference, out _dataColumnReference);
            DeserializeProperty(info, context, PropertyNames.ShowHydrogens, out _showHydrogens);
            DeserializeProperty(info, context, PropertyNames.FilterSettings, out _filterSettings);
            DeserializeProperty(info, context, PropertyNames.CDXStructure, out _cdxStructure);
            DeserializeProperty(info, context, PropertyNames.RGroupStructure, out _rGroupStructure);
            DeserializeProperty(info, context, PropertyNames.RGroupStructureType, out _rGroupStructureType);

            var persistedVersion = GetPersistedVersion(info, context, typeof(StructureFilterModel));
            var ver91OrNewer = new Version(persistedVersion.MajorVersion, persistedVersion.MinorVersion) >=
                               new Version(9, 1);
            if (ver91OrNewer)
            {
                DeserializeProperty(info, context, PropertyNames.RGroupNickNames, out _rGroupNickNames);
                DeserializeProperty(info, context, PropertyNames.SavedSearchList, out _savedSearchList);
            }
        }

        #endregion

        #region Properties

        internal DataTable DataTableReference
        {
            get
            {
                try
                {
                    return _dataTableReference.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
        }

        public Guid ModelId
        {
            get
            {
                try
                {
                    return _dataTableReference.Value != null ? _dataTableReference.Value.Id : Guid.Empty;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return Guid.Empty;
                }
            }
        }

        internal DataColumn DataColumnReference
        {
            get
            {
                try
                {
                    return _dataColumnReference.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
            set
            {
                _dataColumnReference.Value = value;
                SetTableMetaDataProperty(Identifiers.StructureFilterDataColumnName,
                                         value != null ? value.Name : String.Empty);
            }
        }

        public INodeContext NodeContext
        {
            get
            {
                return Context;
            }
        }

        public StructureFilterSettings.FilterModeEnum SearchType
        {
            get
            {
                try
                {
                    return _filterSettings.Value.FilterMode;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return DefaultSearchType;
                }
            }
            set
            {
                SetTableMetaDataProperty(Identifiers.StructureFilterSearchType, (int)value);
            }
        }

        public bool RGroupDecomposition
        {
            get
            {
                try
                {
                    return _filterSettings.Value.RGroupDecomposition;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return false;
                }
            }
        }

        public int PercentSimilarity
        {
            get
            {
                try
                {
                    return _filterSettings.Value.SimularityPercent;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return DefaultPercentSimilarity;
                }
            }
            set
            {
                SetTableMetaDataProperty(Identifiers.StructureFilterPercentSimilarity, value);
            }
        }

        internal StructureFilterSettings.SimularityModeEnum SimilarityMode
        {
            get
            {
                try
                {
                    return _filterSettings.Value.SimularityMode;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return DefaultSimilarityMode;
                }
            }
            set
            {
                SetTableMetaDataProperty(Identifiers.StructureFilterSimilarityMode, (int)value);
            }
        }

        public string ShowHydrogens
        {
            get
            {
                try
                {
                    return _showHydrogens.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return DefaultShowHydrogens;
                }
            }
            set
            {
                _showHydrogens.Value = value;
            }
        }

        public string StructureString
        {
            get
            {
                try
                {
                    return _filterSettings.Value.FilterStructure;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
            set
            {
                throw new InvalidOperationException("Use SetFilterSettings to set this property");
            }
        }

        public StructureFilterSettings StructureFilterSettings
        {
            get
            {
                try
                {
                    return _filterSettings.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
        }

        public string MimeType
        {
            get
            {
                try
                {
                    if (DataColumnReference == null)
                    {
                        return null;
                    }
                    return DataColumnReference.Properties.ContentType;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
        }

        public byte[] CdxStructure
        {
            get
            {
                try
                {
                    return _cdxStructure.Value == null ? null : Convert.FromBase64String(_cdxStructure.Value);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
            set
            {
                throw new InvalidOperationException("Use SetFilterSettings to set this property");
            }
        }

        public string RGroupStructure
        {
            get
            {
                try
                {
                    return _rGroupStructure.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return null;
                }
            }
            set
            {
                SetTableMetaDataProperty(Identifiers.RGroupStructure, value ?? String.Empty);
            }
        }

        public StructureStringType RGroupStructureType
        {
            get
            {
                try
                {
                    return _rGroupStructureType.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return StructureStringType.Unknown;
                }
            }
            set
            {
                SetTableMetaDataProperty(Identifiers.RGroupStructureType, value);
            }
        }

        public bool NickNames
        {
            get
            {
                try
                {
                    return _rGroupNickNames.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return false;
                }
            }
            set
            {
                //SetTableMetaDataProperty(Identifiers.RGroupNickNames, value);
            }
        }

        public bool UpdateSavedSearchList
        {
            get
            {
                try
                {
                    return _savedSearchList.Value;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return false;
                }
            }
            set
            {
               // SetTableMetaDataProperty(Identifiers.UpdateSavedSearchList, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>Implements ISerializable.</summary>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            SerializeProperty(info, context, _dataTableReference);
            SerializeProperty(info, context, _dataColumnReference);
            SerializeProperty(info, context, _showHydrogens);
            SerializeProperty(info, context, _filterSettings);
            SerializeProperty(info, context, _cdxStructure);
            SerializeProperty(info, context, _rGroupStructure);
            SerializeProperty(info, context, _rGroupStructureType);
            SerializeProperty(info, context, _rGroupNickNames);
            SerializeProperty(info, context, _savedSearchList);
        }

        /// <summary>
        /// This method is called only the first time the node is added to the document.
        /// </summary>
        protected override void OnConfigure()
        {
            base.OnConfigure();

            _dataTableReference.Value = Context.GetAncestor<Document>().ActiveDataTableReference;

            RefreshDataPersistedInTableProperties();
            SetDataColumn();
        }

        public void ExecuteTransaction(Executor executor)
        {
            Transactions.ExecuteTransaction(executor);
        }

        public ModulesService GetModulesService()
        {
            return Context.GetService<ModulesService>();
        }

        private DataManager TryGetDataManager()
        {
            DataManager dataManager = null;
            try
            {
                dataManager = Context.GetService<DataManager>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return dataManager;
        }

        protected override void DeclareInternalEventHandlers(InternalEventManager eventManager)
        {
            base.DeclareInternalEventHandlers(eventManager);

            AddActiveDataTableChangeEventHandler(eventManager);
            AddFilterSettingsPropertyChangeEventHandler(eventManager);
            AddDataTablePropertyChangeEventHandler(eventManager);
            AddDataTableColumnPropertyChangeEventHandler(eventManager);
            AddDataTableColumnChangeEventHandler(eventManager);
            AddDataTableRowCountEventHandler(eventManager);
        }

        #region AddInternalEventHandlers

        private void AddActiveDataTableChangeEventHandler(InternalEventManager eventManager)
        {
            eventManager.AddEventHandler(
                delegate
                {
                    OnActiveDataTableChanged();

                },
                Trigger.CreatePropertyTrigger(Context.GetAncestor<Document>(),
                                              Document.PropertyNames.ActiveDataTableReference));
        }

        private void AddFilterSettingsPropertyChangeEventHandler(InternalEventManager eventManager)
        {
            Trigger filterSettingsChanges = Trigger.CreatePropertyTrigger(this,
                                                                          PropertyNames.FilterSettings);
            eventManager.AddEventHandler(delegate { OnFilterSettingsPropertyChanged(); }, filterSettingsChanges);
        }

        private void AddDataTablePropertyChangeEventHandler(InternalEventManager eventManager)
        {
            Trigger dataTablePropertyChanges = Trigger.CreateMutablePropertyTrigger(
                this,
                PropertyNames.DataTableReference,
                delegate(DataTable table)
                {
                    if (table == null)
                    {
                        return Trigger.NeverTrigger;
                    }

                    return Trigger.CreatePropertyTrigger(table, DataTable.PropertyNames.Properties);
                });

            eventManager.AddEventHandler(delegate { OnDataTablePropertyChanged(); },
                                         dataTablePropertyChanges);
        }

        private void AddDataTableColumnPropertyChangeEventHandler(InternalEventManager eventManager)
        {
            //trigger for column property changes in the active data table (this may affect the structure filter)
            Trigger columnPropertyChanges = Trigger.CreateMutablePropertyTrigger(
                this,
                PropertyNames.DataTableReference,
                delegate(DataTable table)
                {
                    if (table == null)
                    {
                        return Trigger.NeverTrigger;
                    }
                    return Trigger.CreateSubTreeTrigger(table.Columns, typeof(DataColumn),
                                                        DataColumn.PropertyNames.Properties);
                });

            eventManager.AddEventHandler(delegate { OnDataTableColumnPropertyChanged(); }, columnPropertyChanges);
        }

        private void AddDataTableColumnChangeEventHandler(InternalEventManager eventManager)
        {
            Trigger dataTableChanges = Trigger.CreateMutablePropertyTrigger(
                this,
                PropertyNames.DataTableReference,
                delegate(DataTable table)
                {
                    if (table == null)
                    {
                        return Trigger.NeverTrigger;
                    }

                    return Trigger.CreatePropertyTrigger(table.Columns, DataColumnCollection.PropertyNames.Items);
                });

            eventManager.AddEventHandler(delegate { OnDataTableColumnsChanged(); }, dataTableChanges);
        }

        private void AddDataTableRowCountEventHandler(InternalEventManager eventManager)
        {
            Trigger dataRowsChanges = Trigger.CreateMutablePropertyTrigger(
                this,
                PropertyNames.DataTableReference,
                delegate(DataTable table)
                {
                    if (table == null)
                    {
                        return Trigger.NeverTrigger;
                    }

                    return Trigger.CreatePropertyTrigger(table, DataTable.PropertyNames.RowCount);
                });

            //eventManager.AddEventHandler(delegate { OnRowCountChanged(); }, dataRowsChanges);
        }

        #endregion

        #region InternalEventHandlers

        private void OnActiveDataTableChanged()
        {
            //update the data table reference
            _dataTableReference.Value = Context.GetAncestor<Document>().ActiveDataTableReference;

            RefreshDataPersistedInTableProperties();
            //set data column containing structures if not set yet on the dataset (must do before initializing the filter settings)
            if (_dataTableReference.Value != null && DataColumnReference == null)
            {
                SetDataColumn();
            }
        }

        private void OnFilterSettingsPropertyChanged()
        {
            if (DataColumnReference == null)
            {
                SetDataColumn();
            }

            if (_filterSettings.Value != null) return;

            _filterSettings.Value = CreateDefaultFilterSettings();
            if (ValidDataTable())
            {
                //set table metadata properties
                SetDefaultTableMetaDataProperties();
            }
        }

        private void OnDataTablePropertyChanged()
        {
            RefreshDataPersistedInTableProperties();
        }

        private void OnDataTableColumnPropertyChanged()
        {
            SetDataColumn();
        }

        private void OnDataTableColumnsChanged()
        {
            if (_dataTableReference.Value != null && _dataColumnReference.Value != null)
            {
                if (!_dataTableReference.Value.Columns.Contains(_dataColumnReference.Value.Name))
                {
                    DataColumnReference = null;
                }
            }
        }

        private void OnRowCountChanged()
        {
            //CoreChemistryCLR clr = DataTableInfoMgr.GetCoreChemistry(this);

            ////Note: this event handler is called when the data table changes
            //if (!TableIndexed(clr) || !NumberOfRowsChanged(clr))
            //{
            //    return;
            //}

            if (_dataTableReference != null && _dataTableReference.Value != null)
            {
                DataTableInfoMgr.CancelCurrentOperation(_dataTableReference.Value.Id);
            }
            DataTableInfoMgr.ClearLastDispatched(this);
            // do not clear data when loading a document
            //if (clr != null && !clr.IgnoreRowChange)
            //{
            //    clr.ClearLoaded();
            //}
            RefreshDataPersistedInTableProperties();
        }

        //private bool NumberOfRowsChanged(CoreChemistryCLR clr)
        //{
        //    if (clr == null)
        //    {
        //        return false;
        //    }
        //    return clr.NumberOfRowsIndexed() != DataTableReference.RowCount;
        //}

        //private bool TableIndexed(CoreChemistryCLR clr)
        //{
        //    if (clr == null)
        //    {
        //        return false;
        //    }
        //    return clr.HaveData();
        //}

        #endregion

        private void RegisterTableProperties()
        {
            RegisterTableProperty<string>(Identifiers.StructureFilterDataColumnName, DataType.String, null);
            RegisterTableProperty(Identifiers.StructureFilterSearchType, DataType.Integer, (int)DefaultSearchType);
            RegisterTableProperty(Identifiers.StructureFilterPercentSimilarity, DataType.Integer,
                                  DefaultPercentSimilarity);
            RegisterTableProperty(Identifiers.StructureFilterSimilarityMode, DataType.Integer,
                                  (int)DefaultSimilarityMode);
            RegisterTableProperty(Identifiers.StructureFilterStructureString, DataType.String, DefaultStructureString);
            RegisterTableProperty(Identifiers.StructureMatchColumn, DataType.String, String.Empty);
            RegisterTableProperty(Identifiers.CDXStructureData, DataType.String, String.Empty);
            RegisterTableProperty(Identifiers.StructureFilterRGroup, DataType.Boolean, false);
            RegisterTableProperty(Identifiers.RGroupStructure, DataType.String, String.Empty);
            RegisterTableProperty(Identifiers.RGroupStructureType, DataType.Integer, StructureStringType.Unknown);
            RegisterTableProperty(Identifiers.NumberRGroups, DataType.Integer, 0);
            //RegisterTableProperty(Identifiers.RGroupNickNames, DataType.Boolean, false);
            //RegisterTableProperty(Identifiers.UpdateSavedSearchList, DataType.Boolean, false);
            //RegisterTableProperty(Identifiers.StructureFilterAlignment, DataType.Boolean, false);
        }

        private void RegisterTableProperty<T>(string metadataPropertyName, DataType propertyType, T defaultPropertyValue)
        {
            DataManager dataManager = TryGetDataManager();

            if (dataManager == null)
            {
                return;
            }

            if (!dataManager.Properties.ContainsProperty(
                DataPropertyClass.Table, metadataPropertyName))
            {
                //register table property name in the data manager property registry
                DataProperty p =
                    DataProperty.CreateCustomPrototype(
                        metadataPropertyName,
                        defaultPropertyValue,
                        propertyType,
                        DataPropertyAttributes.IsPersistent
                        );
                dataManager.Properties.AddProperty(DataPropertyClass.Table, p);
            }
        }

        private void SetDefaultTableMetaDataProperties()
        {
            SetTableMetaDataProperty(Identifiers.StructureFilterSearchType, (int)DefaultSearchType);
            SetTableMetaDataProperty(Identifiers.StructureFilterSimilarityMode, (int)DefaultSimilarityMode);
            SetTableMetaDataProperty(Identifiers.StructureFilterPercentSimilarity, DefaultPercentSimilarity);
            SetTableMetaDataProperty(Identifiers.StructureFilterStructureString, DefaultStructureString);
            SetTableMetaDataProperty(Identifiers.StructureFilterRGroup, false);
            SetTableMetaDataProperty(Identifiers.RGroupStructure, String.Empty);
            SetTableMetaDataProperty(Identifiers.CDXStructureData, String.Empty);
            //SetTableMetaDataProperty(Identifiers.RGroupNickNames, false);
            //SetTableMetaDataProperty(Identifiers.UpdateSavedSearchList, false);
        }

        private void SetTableMetaDataProperty<T>(string metadataPropertyName, T propertyValue)
        {
            if (DataTableReference != null)
            {
                try
                {
                    //set property on table
                    DataTableReference.Properties.SetProperty(metadataPropertyName, propertyValue);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public bool ValidDataTable()
        {
            return _dataTableReference != null && _dataColumnReference != null && _dataTableReference.Value != null &&
                   _dataColumnReference.Value != null;
        }

        internal StructureFilterSettings CreateDefaultFilterSettings()
        {
            return new StructureFilterSettings(
                String.Empty,
                DefaultSearchType,
                false,
                DefaultSimilarityMode,
                DefaultPercentSimilarity,
                false,
                DefaultStructureString,
                false);
        }

        public void SetRGroupDecomposition(bool enabled, bool nickNames)
        {
            //ExecuteTransaction(delegate
            //    {
            //        SetTableMetaDataProperty(Identifiers.StructureFilterRGroup, enabled);
            //        SetTableMetaDataProperty(Identifiers.RGroupNickNames, nickNames);
            //    });
        }

        public void RemoveRGroupDecompositionColumnsFromDocument()
        {
            if (DataTableReference == null)
            {
                return;
            }
            Document document = DataTableReference.Context.GetService<Document>();
            //TableOperations.RemoveRGroupDecompositionColumnsFromDocument(document, DataTableReference);
        }

        public void SetStructureAlignment(bool enabled)
        {
            //SetTableMetaDataProperty(Identifiers.StructureFilterAlignment, enabled);

            //Document document = DataTableReference.Context.GetService<Document>();
            //if (document != null && document.ActiveVisualReference.As<TablePlot>() != null && _dataColumnReference.Value != null)
            //{
            //    StructureColumnRendererModel structureColumnRendererModel = TableOperations.GetStructureColumnRenderer(document.ActiveVisualReference.As<TablePlot>(), _dataColumnReference.Value.Name);
            //    if (structureColumnRendererModel != null)
            //    {
            //        if (enabled)
            //        {
            //            structureColumnRendererModel.StructureAlignmentData = _cdxStructure.Value;
            //        }
            //        else
            //        {
            //            structureColumnRendererModel.StructureAlignmentData = "";
            //        }
            //    }
            //}
        }


        public void ShowStructureSourceConfigDialog()
        {
            // Get the active window (needed when launched by the right-click menu)
            //var wnd = new Win32Window(GetForegroundWindow());
            //var sourceConfig = new StructureSourceConfiguration(this);
            //sourceConfig.ShowDialog(wnd);
        }

        public string GetDataColumnReference()
        {
            return DataColumnReference.Name;
        }

        private void RefreshDataPersistedInTableProperties()
        {
            DataColumn currDataColumn = null;
            StructureFilterSettings.FilterModeEnum currSearchType = DefaultSearchType;
            StructureFilterSettings.SimularityModeEnum currSimilarityMode = DefaultSimilarityMode;
            int currPercentSimilarity = DefaultPercentSimilarity;
            string currCDXStructureData = String.Empty;
            string currStructureString = DefaultStructureString;
            string currDataColumnName = null;
            bool currRGroupDecomposition = false;
            string currRGroupStructure = null;
            StructureStringType currRGroupStructureType = StructureStringType.Unknown;
            bool currRGroupNickNames = false;
            bool currSavedSearchList = false;

            if (DataTableReference != null)
            {
                RegisterTableProperties();

                GetDataColumnPropertiesFromTableProperties(out currDataColumn, out currDataColumnName);
                currSearchType = GetSearchTypeFromTableProperty();
                currPercentSimilarity = GetPercentSimilarityFromTableProperty();
                currSimilarityMode = GetSimilarityModeFromTableProperty();
                currCDXStructureData = GetCDXStructureDataFromTableProperty();
                currStructureString = GetStructureStringFromTableProperty(currCDXStructureData);
                currRGroupDecomposition = GetRGroupDecompFromTableProperty();
                currRGroupStructure = GetRGroupStructureFromTableProperty();
                currRGroupStructureType = GetRGroupStructureTypeFromTableProperty();
                currRGroupNickNames = GetRGroupNickNamesFromTableProperty();
                currSavedSearchList = GetSavedSearchListFromTableProperty();
            }
            
            ExecuteTransaction(delegate
                {
                    _rGroupStructure.Value = currRGroupStructure;
                    _rGroupStructureType.Value = currRGroupStructureType;
                    _rGroupNickNames.Value = currRGroupNickNames;
                    _dataColumnReference.Value = currDataColumn;
                    _cdxStructure.Value = currCDXStructureData;
                    _filterSettings.Value = new StructureFilterSettings(currDataColumnName,
                                                                       currSearchType, true, currSimilarityMode,
                                                                       currPercentSimilarity, currRGroupDecomposition,
                                                                       currStructureString, currRGroupNickNames);
                    _savedSearchList.Value = currSavedSearchList;
                });
        }

        #region GetIndividualTableProperties

        private void GetDataColumnPropertiesFromTableProperties(out DataColumn dataColumn, out string dataColumnName)
        {
            dataColumn = null;
            dataColumnName =
                    DataTableReference.Properties.GetProperty(
                        Identifiers.StructureFilterDataColumnName) as string;
            if (!string.IsNullOrEmpty(dataColumnName))
            {
                if (DataTableReference.Columns.Contains(dataColumnName))
                {
                    dataColumn = DataTableReference.Columns[dataColumnName];
                }
                else
                {
                    dataColumnName = null;
                }
            }
        }

        private StructureFilterSettings.FilterModeEnum GetSearchTypeFromTableProperty()
        {
            StructureFilterSettings.FilterModeEnum searchType;
            int? searchTypeTableProperty = DataTableReference.Properties.GetProperty(
                    Identifiers.StructureFilterSearchType) as int?;
            if (searchTypeTableProperty == null ||
                !Enum.IsDefined(typeof(StructureFilterSettings.FilterModeEnum), (int)searchTypeTableProperty))
            {
                searchType = DefaultSearchType;
            }
            else
            {
                searchType = (StructureFilterSettings.FilterModeEnum)searchTypeTableProperty;
            }
            return searchType;
        }

        private int GetPercentSimilarityFromTableProperty()
        {
            int? percentSimilarityTableProperty = DataTableReference.Properties.GetProperty(
                    Identifiers.StructureFilterPercentSimilarity) as int?;
            return percentSimilarityTableProperty ?? DefaultPercentSimilarity;
        }

        private StructureFilterSettings.SimularityModeEnum GetSimilarityModeFromTableProperty()
        {
            StructureFilterSettings.SimularityModeEnum similarityMode;
            int? similarityModeTableProperty = DataTableReference.Properties.GetProperty(
                            Identifiers.StructureFilterSimilarityMode) as int?;
            if (similarityModeTableProperty == null || !Enum.IsDefined(typeof(StructureFilterSettings.SimularityModeEnum), (int)similarityModeTableProperty))
            {
                similarityMode = DefaultSimilarityMode;
            }
            else
            {
                similarityMode = (StructureFilterSettings.SimularityModeEnum)similarityModeTableProperty;
            }
            return similarityMode;
        }

        private string GetCDXStructureDataFromTableProperty()
        {
            return DataTableReference.Properties.GetProperty(Identifiers.CDXStructureData) as string;
        }

        private string GetStructureStringFromTableProperty(string cdxStructureData)
        {
            string structureStringTableProperty = DataTableReference.Properties.GetProperty(Identifiers.StructureFilterStructureString) as string;
            string structureString = structureStringTableProperty ?? DefaultStructureString;
            if (!string.IsNullOrEmpty(cdxStructureData))
            {
                structureString = cdxStructureData;
            }
            return structureString;
        }

        private bool GetRGroupDecompFromTableProperty()
        {
            bool? rGroupDecoposition = DataTableReference.Properties.GetProperty(Identifiers.StructureFilterRGroup) as bool?;
            return rGroupDecoposition != null && rGroupDecoposition.Value;
        }

        private string GetRGroupStructureFromTableProperty()
        {
            return DataTableReference.Properties.GetProperty(Identifiers.RGroupStructure) as string;

        }

        private StructureStringType GetRGroupStructureTypeFromTableProperty()
        {
            int? rgst = DataTableReference.Properties.GetProperty(Identifiers.RGroupStructureType) as int?;
            return (StructureStringType)(rgst ?? (int)StructureStringType.Unknown);
        }

        private bool GetRGroupNickNamesFromTableProperty()
        {
            //bool? rGroupNickNames = DataTableReference.Properties.GetProperty(Identifiers.RGroupNickNames) as bool?;
            //return rGroupNickNames != null && rGroupNickNames.Value;
            return false;
        }

        private string GetStructureMatchColumnFromTableProperty()
        {
            return DataTableReference.Properties.GetProperty(Identifiers.StructureMatchColumn) as string;
        }

        private bool GetSavedSearchListFromTableProperty()
        {
            //bool? savedSearchList = DataTableReference.Properties.GetProperty(Identifiers.UpdateSavedSearchList) as bool?;
            //return savedSearchList != null && savedSearchList.Value;
            return false;
        }

        #endregion

        public void ClearCurrentFilter()
        {
            DataManager dataManager = TryGetDataManager();
            if (dataManager == null)
            {
                return;
            }
            DataMarkingSelection markingSelection = dataManager.Markings.DefaultMarkingReference;
            RowSelection rowsMarked = markingSelection.GetSelection(DataTableReference);

            ExecuteTransaction(delegate
            {
                RemoveFilterColumn();

                // Reset the filter options to the defaults
                _cdxStructure.Value = null;
                _filterSettings.Value = CreateDefaultFilterSettings();

                //set table metadata properties
                SetDefaultTableMetaDataProperties();
            });

            markingSelection.SetSelection(rowsMarked, DataTableReference);
        }

        private void RemoveFilterColumn()
        {
            // Remove the structure match tag column and corresponding table metadata property
            var doc = Context.GetAncestor<Document>();
            var matchCol = (string)doc.ActiveDataTableReference.Properties.GetProperty(Identifiers.StructureMatchColumn);
            if (!String.IsNullOrEmpty(matchCol))
            {
                if (doc.ActiveDataTableReference.Columns.Contains(matchCol))
                {
                    doc.ActiveDataTableReference.Columns.Remove(matchCol);
                }
                DataTableReference.Properties.SetProperty(Identifiers.StructureMatchColumn, String.Empty);
            }
        }

        /// <summary>
        /// Set the Data Column that contains the structure data for the structure filter.
        /// </summary>
        public void SetDataColumn()
        {
            if (DataTableReference == null)
            {
                return;
            }

            //see if table has structure column defined already
            var existingDataColumnName = DataTableReference.Properties.GetProperty(Identifiers.StructureFilterDataColumnName) as string;
            if (!String.IsNullOrEmpty(existingDataColumnName) && DataTableReference.Columns.Contains(existingDataColumnName))
            {
                DataColumnReference = DataTableReference.Columns[existingDataColumnName];
            }
            else
            {
                // find the structure column in the dataset
                foreach (DataColumn column in DataTableReference.Columns)
                {
                    if (FilterUtilities.IsStructureColumn(column))
                    {
                        DataColumnReference = column;
                        return;
                    }
                }

                //there are no structure columns
                DataColumnReference = null;
                if (!String.IsNullOrEmpty(existingDataColumnName))
                {
                    RemoveFilterColumn();
                    RemoveRGroupDecompositionColumnsFromDocument();
                }
            }
        }

        public void SetFilterSettings(StructureFilterSettings.FilterModeEnum searchType, string structure)
        {
            string structureString;
            byte[] cdxStructure;

            if (MimeType == Identifiers.CDXContentType)
            {
                cdxStructure = Encoding.UTF8.GetBytes(structure);
                structureString = FilterUtilities.ConvertCdxToMol(structure);
            }
            else if (MimeType == Identifiers.CDXMLContentType)
            {
                cdxStructure = Encoding.UTF8.GetBytes(structure);
                structureString = FilterUtilities.ConvertCdxToMol(Convert.ToBase64String(cdxStructure));
            }
            else
            {
                structureString = structure;
                cdxStructure = null;
            }

            SetFilterSettings(searchType, structureString, cdxStructure);
        }
        
        public void SetFilterSettings(StructureFilterSettings.FilterModeEnum searchType, string structure, byte[] cdx)
        {
            ExecuteTransaction(delegate
            {
                if (string.IsNullOrEmpty(structure) && (cdx == null || cdx.Length == 0))
                {
                    ClearCurrentFilter();
                }
                else
                {
                    SetTableMetaDataProperty(Identifiers.StructureFilterSearchType, (int)searchType);
                    SetTableMetaDataProperty(Identifiers.StructureFilterStructureString, structure);
                    SetTableMetaDataProperty(Identifiers.CDXStructureData,
                                             cdx == null ? null : Convert.ToBase64String(cdx));
                }
            });
        }

        public void SetFilterSettings(SavedSearchSettings settings)
        {
            ExecuteTransaction(delegate
            {
                if (string.IsNullOrEmpty(settings.FilterStructure) && (settings.CDXStructureData == null || settings.CDXStructureData.Length == 0))
                {
                    ClearCurrentFilter();
                }
                else
                {
                    SetTableMetaDataProperty(Identifiers.StructureFilterSearchType, (int)settings.FilterMode);
                    SetTableMetaDataProperty(Identifiers.StructureFilterStructureString, settings.FilterStructure);
                    SetTableMetaDataProperty(Identifiers.CDXStructureData,
                                             settings.CDXStructureData == null ? null : Convert.ToBase64String(settings.CDXStructureData));
                    SetTableMetaDataProperty(Identifiers.StructureFilterPercentSimilarity, settings.SimularityPercent);
                    SetTableMetaDataProperty(Identifiers.StructureFilterSimilarityMode, (int)settings.SimularityMode);
                    SetTableMetaDataProperty(Identifiers.StructureFilterRGroup, settings.RGroupDecomposition);
                    //SetTableMetaDataProperty(Identifiers.RGroupNickNames, settings.RGroupNicknames);
                    SetTableMetaDataProperty(Identifiers.StructureFilterDataColumnName, settings.StructureColumn);
                }
            });
        }

        internal void ApplyFilter(DataTable dataTable)
        {
            // use this instance of the model to invoke filtering on the specific data table
            DataTable dataOriginal = DataTableReference;
            // note the row count change event should not erase saved data, so it must be ignored
            //CoreChemistryCLR clr = DataTableInfoMgr.GetCoreChemistry(dataTable.Id);
            _dataTableReference.Value = null;
            //clr.IgnoreRowChange = true;
            _dataTableReference.Value = dataTable;
            //clr.IgnoreRowChange = false;
            _dataTableReference.Value = dataOriginal;
        }

        public bool HasStructureString()
        {
            return !string.IsNullOrEmpty(StructureString);
        }

        internal bool HasStructureMatchColumn()
        {
            return !String.IsNullOrEmpty(GetStructureMatchColumnFromTableProperty());
        }

        #endregion

        public new abstract class PropertyNames : DocumentNode.PropertyNames
        {
            #region Constants and Fields

            internal static readonly PropertyName DataTableReference = CreatePropertyName("DataTableReference");
            internal static readonly PropertyName DataColumnReference = CreatePropertyName("DataColumnReference");
            internal static readonly PropertyName ShowHydrogens = CreatePropertyName("ShowHydrogens");
            internal static readonly PropertyName FilterSettings = CreatePropertyName("FilterSettings");
            internal static readonly PropertyName CDXStructure = CreatePropertyName("CDXStructure");
            internal static readonly PropertyName RGroup = CreatePropertyName("RGroup");
            internal static readonly PropertyName RGroupStructure = CreatePropertyName("RGroupStructure");
            internal static readonly PropertyName RGroupStructureType = CreatePropertyName("RGroupStructureType");
            internal static readonly PropertyName RGroupNickNames = CreatePropertyName("RGroupNickNames");
            internal static readonly PropertyName SavedSearchList = CreatePropertyName("SavedSearchList");

            #endregion
        }
    }
}
