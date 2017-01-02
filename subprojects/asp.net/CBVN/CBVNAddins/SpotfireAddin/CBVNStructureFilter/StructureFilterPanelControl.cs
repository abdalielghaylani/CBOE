// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPanel.cs" company="PerkinElmer Inc.">
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

using System.Linq;

namespace CBVNStructureFilter
{
    using System;
    using System.Collections.Generic;
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Application.Filters;
    using Spotfire.Dxp.Data;
    using Spotfire.Dxp.Framework.ApplicationModel;
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.Threading;
    //using CBVNSFCoreChemistryCLR;
    using FilterThread;
    using System.Windows.Forms;
    using Properties;

    internal class StructureFilterPanelControl : StructureFilterPanelControlBase
    {
        #region Constants and Fields

        private const int MaxQueueSize = 20;
        
        /// <summary>
        /// A dictionary that maps pages with panel controls - each page has it's own panel control
        /// </summary>
        private static readonly Dictionary<Page, StructureFilterPanelControl> PagePanelControls = new Dictionary<Page, StructureFilterPanelControl>();

        private readonly Document _document;
        private readonly Page _page;
        private ExternalEventManager _eventManager;
        private readonly StructureFilterModel _filterModel;
        private readonly ApplicationThread _applicationThread;
        private WorkDispatcher<StructureFilterWorkItem> _dispatcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of class <see cref="StructureFilterPanelControl"/>
        /// given the panel object.
        /// </summary>
        /// <remarks>This constructor is mandatory.</remarks>
        public StructureFilterPanelControl(StructureFilterPanel panel)
            : base(panel.StructureFilterModel, panel.StructureFilterModel, panel.StructureFilterModel)
        {
            _document = panel.Context.GetAncestor<Document>();

            _filterModel = panel.StructureFilterModel;

            _page = _document != null ? _document.ActivePageReference : null;

            // Store the ApplicationThread so we can use it to invoke back to the UI thread.
            try
            {
                _applicationThread = _filterModel.Context.GetService<ApplicationThread>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            CreateWorkDispatcher();

            DeclareExternalEventHandlers();

            if (_page != null)
            {
                PagePanelControls.Add(_page, this);
            }

            if (_filterModel.DataTableReference != null &&
                !DataTableInfoMgr.ContainsKey(_filterModel.DataTableReference.Id))
            {
                DataTableInfoMgr.Add(_filterModel.DataTableReference.Id, _document,
                                     _filterModel.ValidDataTable());
            }
            SetPanelState();
            FillPreviousSearchesCombo();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (_filterModel != null && _filterModel.DataTableReference != null &&
                DataTableInfoMgr.GetSearchState(_filterModel.DataTableReference.Id) ==
                DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy)
            {
                CancelOperation();
            }
            if (disposing)
            {
                if (_eventManager != null)
                {
                    _eventManager.Dispose();
                    _eventManager = null;
                }

                if (_dispatcher != null)
                {
                    _dispatcher.Dispose();
                    _dispatcher = null;
                }

                PagePanelControls.Remove(_page);
            }

            base.Dispose(disposing);
        }

        #endregion
        
        #region Methods

        internal static StructureFilterPanelControl GetPanelControl(Page page)
        {
            if (page == null)
            {
                return null;
            }

            return PagePanelControls.ContainsKey(page) ? PagePanelControls[page] : null;
        }

        protected override void CancelOperation()
        {
            if (_dispatcher == null || _filterModel == null || !_filterModel.IsAttached ||
                _filterModel.DataTableReference == null)
            {
                return;
            }

            // Cancel current operation
            var items = _dispatcher.GetItems();
            foreach (var structureFilterWorkItem in items.Where(structureFilterWorkItem => structureFilterWorkItem.DataTableId == _filterModel.DataTableReference.Id))
            {
                _dispatcher.Cancel(structureFilterWorkItem);
                DataTableInfoMgr.SetSearchState(_filterModel.DataTableReference.Id, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                SetPanelState();
                return;
            }
            DataTableInfoMgr.CancelCurrentOperation(_filterModel.DataTableReference.Id);
        }

        private void CreateWorkDispatcher()
        {
            // Get the WorkManager
            WorkManager manager = null;
            try
            {
                manager = _filterModel.Context.GetService<WorkManager>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            if (manager == null)
            {
                return;
            }

            // Create the dispatcher and store it as a member
            manager.CreateDispatcher(new StructureFilterWorkerFactory(), _filterModel, out _dispatcher);
            _dispatcher.MaxQueueSize = MaxQueueSize;
            _dispatcher.AutoCancel = false;

            // When we update the model, we will get a callback when it has changed.
            _dispatcher.AddWorkerModelChangedHandler(Dispatcher_WorkerModelChanged);
        }

        private void DeclareExternalEventHandlers()
        {
            _eventManager = new ExternalEventManager();

            if (!_filterModel.IsAttached)
            {
                return;
            }
            _eventManager.AddEventHandler(
                OnRefreshChanged,
                Trigger.CreateMutablePropertyTrigger<DataTable>(_filterModel, StructureFilterModel.PropertyNames.DataTableReference, DataTable.PropertyNames.Refreshing));
            _eventManager.AddEventHandler(
               OnPageChanged,
               Trigger.CreatePropertyTrigger(_document, Document.PropertyNames.ActivePageReference));
        }

        private bool CheckForChanges(StructureFilterModel newModel)
        {
            // only care about filtersettings changes or data table changes
            if (newModel.DataTableReference == null)
            {
                // without a data table there is nothing to dispatch
                return false;
            }
            if (!DataTableInfoMgr.ContainsKey(newModel.DataTableReference.Id))
            {
                DataTableInfoMgr.Add(newModel.DataTableReference.Id, _document, newModel.ValidDataTable());
                return true;
            }
            
            var lastSettings = DataTableInfoMgr.GetLastDispatched(newModel);
            if (lastSettings == null)
            {
                return true;
            }
            if (lastSettings.IsChanged(newModel.StructureFilterSettings))
            {
                return true;
            }

            if (newModel.DataColumnReference != null)
            {
                if (DataTableInfoMgr.GetLastDispatchedColumnName(newModel) != newModel.DataColumnReference.Name)
                {
                    return true;
                }
            }
            else if (!string.IsNullOrEmpty(DataTableInfoMgr.GetLastDispatchedColumnName(newModel)))
            {
                return true;
            }

            return false;
        }

        private void Dispatcher_WorkerModelChanged(WorkDispatcher workDispatcher, DocumentNode model)
        {
            var checkModel = model as StructureFilterModel;

            // update UI to reflect changes in model
            UpdateUIFromModelChanged(checkModel, checkModel, checkModel);

            if (!CheckForChanges(checkModel))
            {
                return;
            }

            if (checkModel != null && string.IsNullOrEmpty(checkModel.StructureString))
            {
                return;
            }

            SendWorkItems(workDispatcher as WorkDispatcher<StructureFilterWorkItem>, checkModel);
        }

        private void FilterResultHandler(StructureFilterWorkItem item)
        {
            // Make sure the result is handled on the UI thread.
            // Using ApplicationThread.InvokeAsynchronously instead of
            // this.BeginInvoke avoids problems when this UI is disposed.
            // We check if the form is disposed on the UI thread.
            _applicationThread.InvokeAsynchronously(() => FilterResultHandlerOnUIThread(item));

            // Note that we must call ReturnItem when we're done with it.
            // But here we do it from the UI thread.
        }
        
        private void RGDResultHandler(StructureFilterWorkItem item)
        {
            _applicationThread.InvokeAsynchronously(() => RGDResultHandlerOnUIThread(item));
        }

        private static void CancelDuplicateQueueItems(WorkDispatcher<StructureFilterWorkItem> workDispatcher,
                                               StructureFilterModel sendModel)
        {
            var items = workDispatcher.GetItems();
            foreach (var structureFilterWorkItem in items.Where(structureFilterWorkItem => structureFilterWorkItem.DataTableId == sendModel.DataTableReference.Id))
            {
                workDispatcher.Cancel(structureFilterWorkItem);
            }
        }

        private void SendWorkItems(WorkDispatcher<StructureFilterWorkItem> workDispatcher, StructureFilterModel sendModel)
        {
            if (!sendModel.ValidDataTable())
            {
                return;
            }

            //var coreChemistryClr = DataTableInfoMgr.GetCoreChemistry(sendModel.DataTableReference.Id);
            //if (!coreChemistryClr.SettingsChanged(sendModel.StructureFilterSettings, sendModel.DataTableReference,
            //                                      sendModel.DataColumnReference))
            //{
            //    return;
            //}
            
            if (BusyTimerEnabled && TimerTable != sendModel.DataTableReference.Id)
            {
                // if another table had a timer, mark it busy now
                DataTableInfoMgr.SetSearchState(TimerTable, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy);
            }

            var indexingRequired = false;//coreChemistryClr.DataChanged(sendModel.DataTableReference, sendModel.DataColumnReference);
            if (UsingBusyTimer(indexingRequired, sendModel))
            {
                DataTableInfoMgr.SetSearchState(sendModel.DataTableReference.Id, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy);
            }

            DataTableInfoMgr.SetLastDispatched(sendModel);
            TimerTable = sendModel.DataTableReference.Id;
            ProgressTable = sendModel.DataTableReference.Id;
            SetPanelState();

            if (!UsingBusyTimer(indexingRequired, sendModel))
            {
                BusyTimerEnabled = false; // this resets the count if it was already running
                BusyTimerEnabled = true;
            }

            CancelDuplicateQueueItems(workDispatcher, sendModel);
            //coreChemistryClr.CancelCurrentOperation = false;
            if (sendModel.StructureFilterSettings.FilterMode == StructureFilterSettings.FilterModeEnum.SubStructure &&
                sendModel.StructureFilterSettings.RGroupDecomposition)
            {
                // Do the R-Group decomposition
                workDispatcher.AddLast(
                    new StructureFilterWorkItem(StructureFilterWorkItem.WorkerOperation.ExecuteRGD,
                                                sendModel.StructureFilterSettings, 
                                                RGDResultHandler, indexingRequired, sendModel.DataTableReference,
                                                sendModel.DataColumnReference));
            }
            else
            {
                // Do the regular structure filter
                workDispatcher.AddLast(
                    new StructureFilterWorkItem(StructureFilterWorkItem.WorkerOperation.ExecuteFilter,
                                                sendModel.StructureFilterSettings, 
                                                FilterResultHandler, indexingRequired, sendModel.DataTableReference,
                                                sendModel.DataColumnReference));
            }
        }

        private bool UsingBusyTimer(bool indexingRequired, StructureFilterModel sendModel)
        {
            return indexingRequired || sendModel.StructureFilterSettings.RGroupDecomposition;
        }

        private void FilterResultHandlerOnUIThread(StructureFilterWorkItem item)
        {
            try
            {
                if (item.IsCanceled)// || item.CoreChemistryClr.CancelCurrentOperation)
                {
                    DisableTimer1();
                    DataTableInfoMgr.SetSearchState(item.DataTableId,
                                                    DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                    //item.CoreChemistryClr.CancelCurrentOperation = false;
                    return; // The finally clause below will still be executed and call ReturnItem...
                }
                if (IsDisposed)
                {
                    return; // The finally clause below will still be executed and call ReturnItem...
                }

                DataTableInfoMgr.SetSearchState(item.DataTableId,
                                                DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                DisableTimer1();

                if (item.DataTableId != Guid.Empty && item.Result != null &&
                    !string.IsNullOrEmpty(item.FilterSettings.FilterStructure))
                {
                    ProcessFilterResults(item);
                    UpdateSavedSearchesList(item);
                    Presenter.FilterModel.UpdateSavedSearchList = true;
                }
                else
                {
                    Presenter.ClearFilter();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                // We must return the item when we're done with it...
                item.ReturnItem();

                // note the current model's data table may not be the same one from the event so this may not do anything
                SetPanelState();
                //if (item.CoreChemistryClr.Redo)
                //{
                //    item.CoreChemistryClr.Redo = false;
                //    // fire filtering again
                //    SendWorkItems(_dispatcher, _filterModel);
                //}

                Presenter.FilterModel.UpdateSavedSearchList = false;
            }
        }

        private void RGDResultHandlerOnUIThread(StructureFilterWorkItem item)
        {
            if (item.Operation == StructureFilterWorkItem.WorkerOperation.ExecuteFilter)
            {
                FilterResultHandlerOnUIThread(item);
                return;
            }

            try
            {
                if (item.IsCanceled)// || item.CoreChemistryClr.CancelCurrentOperation)
                {
                    DisableTimer1();
                    DataTableInfoMgr.SetSearchState(item.DataTableId, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                    //item.CoreChemistryClr.CancelCurrentOperation = false;
                    return; // The finally clause below will still be executed and call ReturnItem...
                }
                if (IsDisposed)
                {
                    return; // The finally clause below will still be executed and call ReturnItem...
                }

                DataTableInfoMgr.SetSearchState(item.DataTableId, DataTableInfoMgr.DataTableInfo.SearchStateEnum.Enabled);
                DisableTimer1();

                if (item.Result != null && item.Result.Length > 0)
                {
                    ProcessFilterResults(item);
                }

                //if ((item.RGroupResult != null) && !item.RGroupResult.Cancelled && !item.RGroupResult.Error)
                //{
                //    // Add the virtual columns
                //    TableOperations.AddRGroupDecompositionColumnsToDocument(_document, item.RGroupResult, item.DataColumnName, item.DataTableId, item.FilterSettings.RGroupNicknames);

                //    if (item.RGroupResult.NumberOfGroups > 0)
                //    {
                //        UpdateSavedSearchesList(item);
                //        Presenter.FilterModel.UpdateSavedSearchList = true;
                //    }
                //}
                //else if ((item.RGroupResult != null) && (item.RGroupResult.Error))
                //{
                //    _filterModel.SetRGroupDecomposition(false, false);
                //    MessageBox.Show(item.RGroupResult.ErrorMessage, Resources.RGD, MessageBoxButtons.OK);
                //}
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                // We must return the item when we're done with it...
                item.ReturnItem();

                // note the current model may not be what was acted on, so this may not chagne things
                SetPanelState();
                //if (item.CoreChemistryClr.Redo)
                //{
                //    item.CoreChemistryClr.Redo = false;
                //    // fire filtering again
                //    SendWorkItems(_dispatcher, _filterModel);
                //}
                Presenter.FilterModel.UpdateSavedSearchList = false;
            }
        }

        private static void SetFilter(DataColumn dataColumn, DataTable dataTable, string trueString, string falseString)
        {
            // Use active filtering scheme  
            var document = dataColumn.Context.GetAncestor<Document>();
            var fs = document.ActivePageReference.FilterPanel.FilteringSchemeReference;

            // use filter collection for the correct table  
            var fc = fs[dataTable];
            if (fc == null)
            {
                return;
            }

            if (!fc.Contains(dataColumn.Name))
            {
                // this is an error if the filter does not have the added tag column
                // TODO: check if this case occurs when changing active scheme while search is executing
                Log.Error("Filter does not contain tag column");
                return;
            }
            // Find the filter corresponding to the column you want to manipulate.  
            var filter = fc[dataColumn.Name];

            var myFilter = filter.As<CheckBoxFilter>();
            myFilter.Uncheck(falseString);
            myFilter.Check(trueString);
        }

        private void OnPageChanged()
        {
            if (_filterModel.DataTableReference == null)
            {
                return;
            }
            ProgressTable = _filterModel.DataTableReference.Id;
            ProgressMethod();
            InitializeSearchControls();
            SetPanelState();
        }

        private void OnRefreshChanged()
        {
            if (!_document.ActivePageReference.Equals(_page))
            {
                return;
            }
            
            if (_filterModel.DataTableReference == null || _filterModel.DataColumnReference == null)
            {
                return;
            }

            SetPanelState();
            ProgressTable = _filterModel.DataTableReference.Id;
            ProgressMethod();

            //var coreChemistryClr = DataTableInfoMgr.GetCoreChemistry(_filterModel.DataTableReference.Id);

            ////Note: When data table is reloaded when indexing is in process, coreChemistryClr.LoadedDataTable will be null at this point when Refressing is null (ref LD-1213)
            //if (coreChemistryClr == null)
            //{
            //    return;
            //}
   
            //// when a table is reloaded first Refreshing is true then it changes to false
            //// other cases this event fires just with false
            //if (!_filterModel.DataTableReference.Refreshing)
            //{
            //    if (coreChemistryClr.ReloadStarted)
            //    {
            //        if (DataTableInfoMgr.GetSearchState(_filterModel.DataTableReference.Id) ==
            //            DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy)
            //        {
            //            OnDataTableReloadedWhileBusy(coreChemistryClr);
            //        }
            //        else
            //        {
            //            OnDataTableReloadedNotBusy(coreChemistryClr);                     
            //        }
            //    }
            //}
            //else
            //{
            //    coreChemistryClr.ReloadStarted = true;
            //}
        }

        //private void OnDataTableReloadedWhileBusy(CoreChemistryCLR coreChemistryClr)
        //{
        //    CancelOperation();
        //    coreChemistryClr.ClearLoaded();
        //    coreChemistryClr.Redo = true;
        //    coreChemistryClr.ReloadStarted = false;
        //}

        //private void OnDataTableReloadedNotBusy(CoreChemistryCLR coreChemistryClr)
        //{
        //    coreChemistryClr.ClearLoaded();
        //    coreChemistryClr.ReloadStarted = false;
        //    SendWorkItems(_dispatcher, _filterModel); 
        //}

        private void ProcessFilterResults(StructureFilterWorkItem item)
        {
            var dataManager = _document.Context.GetService<DataManager>();
            var dataTable = TableOperations.GetDataTable(dataManager, item.DataTableId);
            if (dataTable == null)
            {
                return;
            }

            var dataColumn = TableOperations.GetTagsColumn(dataTable);
            if (dataColumn == null)
            {
                TableOperations.AddTagsColumn(dataTable);
                dataColumn = TableOperations.GetTagsColumn(dataTable);
            }

            var set = new IndexSet(dataTable.RowCount, false);
            for (var pos = 0; pos < dataTable.RowCount && pos < item.Result.Length; pos++)
            {
                if (item.Result[pos])
                {
                    set.AddIndex(pos);
                }
            }

            var tagsColumn = dataColumn.As<TagsColumn>();
            tagsColumn.Tag(tagsColumn.TagValues[0], new RowSelection(set));
            tagsColumn.Tag(tagsColumn.TagValues[1], new RowSelection(set.Not()));

            // Next we setup the filter so only the correct rows are shown...
            SetFilter(dataColumn, dataTable, tagsColumn.TagValues[0], tagsColumn.TagValues[1]);
        }

        private void UpdateSavedSearchesList(StructureFilterWorkItem item)
        {
            var savedSearchSettings = new SavedSearchSettings(item.FilterSettings.FilterMode,
                                                                                      item.FilterSettings
                                                                                          .SimularityPercent,
                                                                                      item.FilterSettings
                                                                                          .RGroupDecomposition,
                                                                                      item.FilterSettings
                                                                                          .FilterStructure,
                                                                                      item.FilterSettings
                                                                                          .RGroupNicknames)
            {
                CDXStructureData = Presenter.CdxStructure,
                Name = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                //RGDStructure = item.RGroupResult == null ? null : item.RGroupResult.TemplateStructure,
                //RGDCDXStructureData = item.RGroupResult == null ? null : item.RGroupResult.CDXStructure,
                StructureColumn = item.DataColumnName
            };
            DataTableInfoMgr.AddSavedSearch(item.DataTableId, savedSearchSettings);
        }

        #endregion
    }
}


