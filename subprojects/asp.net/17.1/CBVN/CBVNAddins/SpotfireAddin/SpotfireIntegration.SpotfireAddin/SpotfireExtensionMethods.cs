using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Visuals;
using Spotfire.Dxp.Data;
using SpotfireIntegration.Common;
using SpotfireIntegration.SpotfireAddin.Properties;
using Spotfire.Dxp.Framework.ApplicationModel;
using System.Reflection;
using Spotfire.Dxp.Framework.DocumentModel;
using CambridgeSoft.COE.Framework.COESearchService;
using FormWizard;
using CambridgeSoft.COE.Framework.COEHitListService;
using COEServiceLib;
using FormDBLib;
using Spotfire.Dxp.Framework.Preferences;

namespace SpotfireIntegration.SpotfireAddin
{
    internal static class SpotfireExtensionMethods
    {
        private static bool preventRecursion = false;
        private static string pubFormName = string.Empty;

        #region AnalysisApplication methods
        //CSBR:151920 formName parameter is added to LoadCOETables to set Document Title.
        internal static void LoadCOETables(this AnalysisApplication application, COEHitList hitList,
string baseTableName, string formName, bool filterChildHits)
        {
            if (hitList.ResultsCriteria.Tables.Count == 0)
            {
                throw new Exception("The results criteria is empty.");
            }
            COEService service = application.GetService<COEService>();
            service.IsFromCBVN = true;
            service.CBVNFormName = formName;
            service.CBVNHitList = hitList;
            //COEDataView dataView = service.GetDataViewBO(hitList, filterChildHits).COEDataView;
            COEDataView dataView = service.LoadDataViewBO(hitList).COEDataView;
            ResultsCriteria.ResultsCriteriaTable baseRcTable = hitList.ResultsCriteria.Tables[0];
            if (baseRcTable.Id != dataView.Basetable)
            {
                throw new Exception("The ChemBioViz form was not generated from the base table of the dataview.");
            }
            else
            {
                //Loading Max Row settings from Spotfire Preferences
                CustomPreferences theCustomPreferences = null;
                PreferenceManager thePreferenceManager = application.GetService<PreferenceManager>();
                if (thePreferenceManager.PreferenceExists<CustomPreferences>())
                {
                    theCustomPreferences = thePreferenceManager.GetPreference<CustomPreferences>();
                }
                else
                {
                    theCustomPreferences = new CustomPreferences();
                }

                if (hitList.NumHits > theCustomPreferences.NumberOfMaxRows)
                {
                    //Checking the base table row limit
                    System.Windows.Forms.MessageBox.Show(String.Format(FormWizard.Properties.Resources.Max_Row_Limit_Message, hitList.NumHits, dataView.Basetable, theCustomPreferences.NumberOfMaxRows), FormWizard.Properties.Resources.FORM_TITLE);
                    return;
                }
                //Differed to next release
                //else
                //{
                //    //Checking the child tables row limit
                //    if (service.CheckChildMaxRow(hitList.SearchCriteria, hitList.ResultsCriteria, dataView, theCustomPreferences.NumberOfMaxRows, filterChildHits, FormWizard.Properties.Resources.Max_Row_Limit_Message, FormWizard.Properties.Resources.FORM_TITLE))
                //        return;
                //}
            }

            string baseTableAlias = dataView.Tables.getById(dataView.Basetable).Alias;
            //CSBR:151920 formName parameter is added to SpotfireCOETableDataSource constructor to set Document Title.
            SpotfireCOETableDataSource baseDataSource = new SpotfireCOETableDataSource(hitList, baseTableAlias, true, formName, filterChildHits);
            pubFormName = formName;//formName parameter is made public to use at MergeCOEChildTables            
            DataTable baseDataTable = null;
            if (application.Document == null)
            {
                application.Open(baseDataSource);
                baseDataTable = application.Document.Data.Tables.DefaultTableReference;
            }

            // If we did not just open a new document, baseDataTable will still be null.
            if (baseDataTable == null)
            {
                // CSBR-153130: no transaction is used in this case because for some reason it sometimes
                // causes a crash when the existing document contains multiple pages / visualizations.
                baseDataTable = LoadCOETablesIntoExistingDocument(application, dataView, baseDataSource, hitList, true);
                // Begin synchronizing selected rows with ChemBioViz.NET.
                CBVNMarkingTool.SynchronizeMarking(application.Document);
            }
            else
            {
                application.Document.Transactions.ExecuteTransaction(delegate
                {
                    baseDataTable.Name = formName;
                    // Auto-configure the new document and remove the automatically generated cover page.
                    application.Document.AutoConfigure();
                    foreach (Page page in application.Document.Pages)
                    {
                        if (page.Title == "Cover Page")
                        {
                            application.Document.Pages.Remove(page);
                            break;
                        }
                    }
                    // Begin synchronizing selected rows with ChemBioViz.NET.
                    CBVNMarkingTool.SynchronizeMarking(application.Document);
                });
            }

        }

        private static DataTable LoadCOETablesIntoExistingDocument(this AnalysisApplication application, COEDataView dataView, SpotfireCOETableDataSource dataSource, bool addNewVisualization, COEHitList hitList, bool isDataviewChanged)
        {
            DataTable oldBaseTable = application.Document.Data.Tables.GetCOEBaseTable();
            int oldBaseTableID = 0;
            if (oldBaseTable != null)
            {
                oldBaseTableID = (int)oldBaseTable.Properties.GetProperty(Properties.Resources.COETableID_PropertyName);
            }

            Dictionary<int, IList<Visual>> affectedVisuals = new Dictionary<int, IList<Visual>>();
            // If there are already COE tables open in the document, remove them first.
            foreach (DataTable dataTable in new List<DataTable>(application.Document.Data.Tables))
            {
                if (dataTable.Properties.HasPropertyValue(Properties.Resources.COETableID_PropertyName))
                {
                    int tableID = (int)dataTable.Properties.GetProperty(Properties.Resources.COETableID_PropertyName);
                    // Record each visual that uses this table, so that we can try to reuse them later.
                    foreach (Page page in application.Document.Pages.ToArray())
                    {
                        foreach (Visual visual in page.Visuals.ToArray())
                        {
                            Visualization visualization = visual.As<Visualization>();
                            if (visualization != null && visualization.Data.DataTableReference == dataTable)
                            {
                                IList<Visual> tableVisuals;
                                if (!affectedVisuals.TryGetValue(tableID, out tableVisuals))
                                {
                                    tableVisuals = new List<Visual>();
                                    affectedVisuals[tableID] = tableVisuals;
                                }
                                tableVisuals.Add(visual);
                            }
                        }

                    }

                    //if dataview is changed then remove all tables from collection and add again
                    if (isDataviewChanged)
                    {
                        application.Document.Data.Tables.Remove(dataTable);
                    }
                    //if (oldBaseTableID == tableID)
                    //{
                    //    application.Document.Data.Tables.Remove(dataTable);
                    //}
                }
            }

            DataTable baseDataTable = null;
            // Existing COE tables have been removed.  Now add the base table.  This will also trigger
            // merging in the child tables.
            if (isDataviewChanged)
            {
                baseDataTable = application.Document.Data.Tables.Add(dataSource.TableName, dataSource);
            }
            else
            {
                baseDataTable = application.Document.Data.Tables.GetCOEBaseTable();
                if (baseDataTable != null)
                {
                    COEService service = application.GetService<COEService>();
                    COEDataView _dataview = service.GetDataView(dataView.DataViewID);
                    CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTable dataViewTable = _dataview.Tables.getById(_dataview.Basetable);
                    baseDataTable.Name = dataViewTable.Alias;

                    try
                    {
                        baseDataTable.ReplaceData(dataSource);
                        baseDataTable.Properties.SetProperty(Resources.FilterChildHits_PropertyName, dataSource.FilterChildHits);
                    }
                    catch (Spotfire.Dxp.Framework.ApplicationModel.ProgressCanceledException)
                    {
                        //Do nothing if user cancel the progress dialog.
                    }
                    catch (Spotfire.Dxp.Framework.ApplicationModel.PromptCanceledException)
                    {
                        //Do nothing if user cancel the match column window.
                    }
                    application.Document.Transactions.ExecuteTransaction(delegate
                    {
                        application.MergeCOEChildTables(baseDataTable, hitList);
                    });
                }
            }

            // Try to preserve configured visuals for any previously existing tables.
            bool needBaseTableVisual = true;
            foreach (int tableID in affectedVisuals.Keys)
            {
                DataTable dataTable = application.Document.Data.Tables.GetCOEDataTable(dataView.DataViewID, tableID);
                if (dataTable != null)
                {
                    foreach (Visual visual in affectedVisuals[tableID])
                    {
                        visual.As<Visualization>().Data.DataTableReference = dataTable;
                        if (visual.TypeId.Name == "Spotfire.Table")
                            visual.AutoConfigure();
                    }
                }
                else
                {
                    // If the table is no longer present, remove its visuals, and the containing pages if they
                    // are consequently empty.  If the base table is in this group, recreate the first visual
                    // found for it, keeping the same type identifier and page.
                    foreach (Visual visual in affectedVisuals[tableID])
                    {
                        Page page = visual.Context.GetAncestor<Page>();
                        TypeIdentifier typeId = visual.TypeId;
                        page.Visuals.Remove(visual);
                        if (needBaseTableVisual && oldBaseTable != null && oldBaseTableID == tableID)
                        {
                            Visual newVisual = page.Visuals.AddNew(typeId);
                            newVisual.As<Visualization>().Data.DataTableReference = baseDataTable;
                            newVisual.AutoConfigure();
                            needBaseTableVisual = false;
                        }
                        if (page.Visuals.Count == 0)
                        {
                            application.Document.Pages.Remove(page);
                        }
                    }
                }
            }

            if (needBaseTableVisual && addNewVisualization)
            {
                // Since we don't have access to the default visualization user preference, create a scatter plot.
                Page page = application.Document.Pages.AddNew(baseDataTable.Name);
                page.AutoConfigure(null);
                Visual visual = page.Visuals.AddNew(VisualTypeIdentifiers.ScatterPlot);
                visual.As<Visualization>().Data.DataTableReference = baseDataTable;
                visual.AutoConfigure();
            }

            return baseDataTable;
        }

        private static DataTable LoadCOETablesIntoExistingDocument(this AnalysisApplication application, COEDataView dataView, SpotfireCOETableDataSource dataSource, COEHitList hitList, bool isDataviewChanged)
        {
            return LoadCOETablesIntoExistingDocument(application, dataView, dataSource, true, hitList, isDataviewChanged);
        }
        //CSBR-167455 CSBR-167458 CSBR-167349 fixed
        private static void ColMovedLoadCOETablesIntoExistingDocument(this AnalysisApplication application, COEDataView dataView, SpotfireCOETableDataSource dataSource)
        {
            IList<Visual> tableVisuals;
            DataTable oldBaseTable = application.Document.Data.Tables.GetCOEBaseTable();
            int oldBaseTableID = 0;
            if (oldBaseTable != null)
            {
                oldBaseTableID = (int)oldBaseTable.Properties.GetProperty(Properties.Resources.COETableID_PropertyName);
            }

            Dictionary<int, IList<Visual>> affectedVisuals = new Dictionary<int, IList<Visual>>();
            // If there are already COE tables open in the document, remove them first.
            foreach (DataTable dataTable in new List<DataTable>(application.Document.Data.Tables))
            {
                if (dataTable.Properties.HasPropertyValue(Properties.Resources.COETableID_PropertyName))
                {
                    int tableID = (int)dataTable.Properties.GetProperty(Properties.Resources.COETableID_PropertyName);
                    // Record each visual that uses this table, so that we can try to reuse them later.
                    foreach (Page page in application.Document.Pages.ToArray())
                    {
                        foreach (Visual visual in page.Visuals.ToArray())
                        {
                            Visualization visualization = visual.As<Visualization>();
                            if (visualization != null && visualization.Data.DataTableReference == dataTable)
                            {
                                if (!affectedVisuals.TryGetValue(tableID, out tableVisuals))
                                {
                                    tableVisuals = new List<Visual>();
                                    affectedVisuals[tableID] = tableVisuals;
                                }
                                tableVisuals.Add(visual);
                            }
                        }
                    }
                    //application.Document.Data.Tables.Remove(dataTable);
                }
            }

            // Existing COE tables have been removed.  Now add the base table.  This will also trigger
            // merging in the child tables.
            //DataTable baseDataTable = application.Document.Data.Tables.Add(dataSource.Name, dataSource);

            DataTable baseDataTable = application.Document.Data.Tables.GetCOEBaseTable();
            if (baseDataTable != null)
            {
                application.Document.Transactions.ExecuteTransaction(delegate
                {
                    application.MergeCOEChildTables(baseDataTable);
                });
            }


            // Try to preserve configured visuals for any previously existing tables.
            foreach (int tableID in affectedVisuals.Keys)
            {
                DataTable dataTable = application.Document.Data.Tables.GetCOEDataTable(dataView.DataViewID, tableID);
                if (dataTable != null)
                {
                    foreach (Visual visual in affectedVisuals[tableID])
                    {
                        visual.As<Visualization>().Data.DataTableReference = dataTable;
                        Page page = visual.Context.GetAncestor<Page>();
                        TypeIdentifier typeId = visual.TypeId;
                        page.Visuals.Remove(visual);
                        if (typeId != null)
                        {
                            Visual newVisual = page.Visuals.AddNew(typeId);
                            newVisual.As<Visualization>().Data.DataTableReference = baseDataTable;
                            newVisual.AutoConfigure();
                        }

                    }
                }

            }
            // Begin synchronizing selected rows with ChemBioViz.NET.
            CBVNMarkingTool.SynchronizeMarking(application.Document);

        }

        internal static void MergeCOEChildTables(this AnalysisApplication application, DataTable baseDataTable)
        {
            MergeCOEChildTables(application, baseDataTable, GetCOEHitList(baseDataTable));
        }

        internal static void MergeCOEChildTables(this AnalysisApplication application, DataTable baseDataTable, COEHitList hitList)
        {
            if (preventRecursion)
            {
                return;
            }
            try
            {
                preventRecursion = true;
                DataManager dataManager = application.Document.Data;
                COEService service = application.GetService<COEService>();
                COEDataView dataView = service.LoadDataViewBO(hitList).COEDataView;

                ProgressService.CurrentProgress.CheckCancel();

                // Create a map of COE table IDs to Spotfire DataTables for use in setting relationships.
                Dictionary<int, DataTable> tableMap = new Dictionary<int, DataTable>();
                // And a map of COE field IDs to ResultsCriteria aliases.
                Dictionary<int, string> fieldAliasMap = new Dictionary<int, string>();

                int baseTableID = dataView.Basetable;
                tableMap.Add(baseTableID, baseDataTable);

                // Create data flows for the child tables.
                foreach (ResultsCriteria.ResultsCriteriaTable rct in hitList.ResultsCriteria.Tables)
                {
                    // Cache the ResultsCriteria alias for each field for linking up relationships.
                    foreach (ResultsCriteria.IResultsCriteriaBase fieldBase in rct.Criterias)
                    {
                        if (fieldBase is ResultsCriteria.Field)
                        {
                            ResultsCriteria.Field field = (ResultsCriteria.Field)fieldBase;
                            try
                            {
                                fieldAliasMap.Add(field.Id, field.Alias);
                            }
                            catch (ArgumentException)
                            {
                                Debug.WriteLine(string.Format("Tried to record a duplicate alias for field {0}", field.Id));
                            }
                        }
                    }

                    if (rct.Id == baseTableID)
                    {
                        continue;
                    }

                    // Find the table in the data view by ID.
                    CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTable dataViewTable = dataView.Tables.getById(rct.Id);

                    // Add a child table
                    try
                    {
                        //CSBR:151920 formName parameter is added to SpotfireCOETableDataSource constructor to set Document Title.

                        SpotfireCOETableDataSource childDataSource = new SpotfireCOETableDataSource(hitList, dataViewTable.Alias, false, rct.Id, pubFormName, baseDataTable.IsFilterChildHits());

                        DataFlowBuilder childDataFlowBuilder = new DataFlowBuilder(childDataSource, application.ImportContext);
                        //childDataFlowBuilder.AddTransformation(new COEMetadataTransformation(hitList, dataViewTable.Id));

                        // Check whether this table already exists.
                        DataTable dataTable = GetCOEDataTable(dataManager.Tables, dataView.DataViewID, dataViewTable.Id);
                        if (dataTable != null)
                        {
                            dataTable.ReplaceData(childDataFlowBuilder.Build());
                            dataTable.Name = dataViewTable.Alias;
                        }
                        else
                        {
                            if (dataManager.Tables.Contains(dataViewTable.Alias))
                            {
                                dataTable = dataManager.Tables[dataViewTable.Alias];
                                dataTable.ReplaceData(childDataFlowBuilder.Build());
                            }
                            else
                            {
                                dataTable = dataManager.Tables.Add(dataViewTable.Alias, childDataFlowBuilder.Build());
                            }
                        }
                        
                        tableMap.Add(rct.Id, dataTable);
                        ProgressService.CurrentProgress.CheckCancel();
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage.ShowDialog(FormWizard.Properties.Resources.FORM_TITLE, ex.Message, ex.ToString());
                    }
                }

                // Remove tables not included in the results criteria.
                foreach (DataTable dataTable in new List<DataTable>(dataManager.Tables))
                {
                    if (dataTable.Properties.HasPropertyValue(Resources.COEDataViewID_PropertyName) &&
                        dataTable.Properties.HasPropertyValue(Resources.COETableID_PropertyName) &&
                        (int)dataTable.Properties.GetProperty(Resources.COEDataViewID_PropertyName) == hitList.DataViewID)
                    {
                        int tableID = (int)dataTable.Properties.GetProperty(Resources.COETableID_PropertyName);
                        if (!tableMap.ContainsKey(tableID))
                        {
                            dataManager.Tables.Remove(dataTable);
                        }
                    }
                }

                // Add the relationships
                foreach (CambridgeSoft.COE.Framework.Common.COEDataView.Relationship rel in dataView.Relationships)
                {
                    DataTable parentDataTable, childDataTable;
                    if (!tableMap.TryGetValue(rel.Parent, out parentDataTable) ||
                        !tableMap.TryGetValue(rel.Child, out childDataTable))
                    {
                        continue;
                    }

                    DataRelation existingRelation = dataManager.Relations.FindRelation(parentDataTable, childDataTable);
                    if (existingRelation != null)
                    {
                        dataManager.Relations.Remove(existingRelation);
                    }

                    string parentFieldAlias;
                    if (!fieldAliasMap.TryGetValue(rel.ParentKey, out parentFieldAlias) || parentFieldAlias == null)
                    {
                        // If the ResultsCriteria didn't contain an alias, fall back on the DataView alias.
                        CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTable parentDataViewTable = dataView.Tables.getById(rel.Parent);
                        CambridgeSoft.COE.Framework.Common.COEDataView.Field parentField = parentDataViewTable.Fields.getById(rel.ParentKey);
                        parentFieldAlias = parentField.Alias;
                    }
                    DataColumn parentDataColumn;
                    if (!parentDataTable.Columns.TryGetValue(parentFieldAlias, out parentDataColumn))
                    {
                        continue;
                    }

                    string childFieldAlias;
                    if (!fieldAliasMap.TryGetValue(rel.ChildKey, out childFieldAlias) || childFieldAlias == null)
                    {
                        // If the ResultsCriteria didn't contain an alias, fall back on the DataView alias.
                        CambridgeSoft.COE.Framework.Common.COEDataView.DataViewTable childDataViewTable = dataView.Tables.getById(rel.Child);
                        CambridgeSoft.COE.Framework.Common.COEDataView.Field childField = childDataViewTable.Fields.getById(rel.ChildKey);
                        childFieldAlias = childField.Alias;
                    }
                    DataColumn childDataColumn;
                    if (!childDataTable.Columns.TryGetValue(childFieldAlias, out childDataColumn))
                    {
                        continue;
                    }

                    string relation = string.Format("{0}.{1} = {2}.{3}",
                        parentDataTable.NameEscapedForExpression, parentDataColumn.NameEscapedForExpression,
                        childDataTable.NameEscapedForExpression, childDataColumn.NameEscapedForExpression);

                    dataManager.Relations.Add(parentDataTable, childDataTable, relation);
                }
            }
            finally
            {
                preventRecursion = false;

            }
        }

        /// <summary>
        /// Update the COE Spotfire data table with a new hitlist without changing the results criteria.
        /// </summary>
        internal static void UpdateSpotfireDataTable(this AnalysisApplication application, int hitListID, HitListType hitListType, int numHits)
        {
            DataTable dataTable = application.Document.Data.Tables.GetCOEBaseTable();
            int dataViewID = (int)dataTable.Properties.GetProperty(Resources.COEDataViewID_PropertyName);
            string resultsCriteriaXml = (string)dataTable.Properties.GetProperty(Resources.COEResultsCriteria_PropertyName);
            COEHitList hitList = new COEHitList(hitListID, hitListType, numHits, dataViewID, resultsCriteriaXml);
            UpdateSpotfireDataTable(application, hitList, false);
        }

        internal static void UpdateSpotfireDataTable(this AnalysisApplication application, string sNewResultsCriteria)
        {
            // given new RC, attach to hitlist, then update as usual
            DataTable dataTable = application.Document.Data.Tables.GetCOEBaseTable();
            COEHitList oldHitList = GetCOEHitList(dataTable);
            COEHitList newHitList = new COEHitList(oldHitList.HitListID, oldHitList.HitListType, oldHitList.NumHits, oldHitList.DataViewID, sNewResultsCriteria);
            UpdateSpotfireDataTable(application, newHitList, false);
        }

        internal static void UpdateSpotfireDataTable(this AnalysisApplication application, ResultsCriteria newResultsCriteria)
        {
            // given new RC, attach to hitlist, then update as usual
            DataTable dataTable = application.Document.Data.Tables.GetCOEBaseTable();
            COEHitList oldHitList = GetCOEHitList(dataTable);
            COEHitList newHitList = new COEHitList(oldHitList.HitListID, oldHitList.HitListType, oldHitList.NumHits, oldHitList.DataViewID, newResultsCriteria);
            UpdateSpotfireDataTable(application, newHitList, false);
        }

        internal static void UpdateSpotfireDataTable(this AnalysisApplication application, COEHitList hitList, bool filterChildHits)
        {
            Document document = application.Document;
            DataTable dataTable = document.Data.Tables.GetCOEBaseTable();

            // first make sure the hitlist or the results criteria are actually changing
            object currentHitListID = dataTable.Properties.GetProperty(Resources.COEHitListID_PropertyName);
            if (currentHitListID != null && (currentHitListID is int) && (int)currentHitListID == hitList.HitListID)
            {
                string currentResultCriteriaXml = dataTable.Properties.GetProperty(Resources.COEResultsCriteria_PropertyName) as string;
                string currentSearchCriteriaXml = dataTable.Properties.GetProperty(Resources.COESearchCriteria_PropertyName) as string;
                if ((CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(hitList.ResultsCriteriaXML) == CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(currentResultCriteriaXml)) &&
                    (CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(hitList.SearchCriteriaXml) == CambridgeSoft.COE.Framework.Common.Utilities.GetMD5Hash(currentSearchCriteriaXml)))
                    return;
            }

            int tableID = (int)dataTable.Properties.GetProperty(Resources.COETableID_PropertyName);
            SpotfireCOETableDataSource baseDataSource = new SpotfireCOETableDataSource(hitList, dataTable.Name, true, tableID, filterChildHits);

            application.Document.Transactions.ExecuteTransaction(delegate
            {
                // Load the revised data.
                ProgressService.CurrentProgress.CheckCancel();
                ProgressService.CurrentProgress.ExecuteSubtask("Loading base table");
                ColumnsChangedResult columnsChangedResult = dataTable.ReplaceData(baseDataSource);
                // TODO: Do something with the columnsChangedResult. //CSBR-167455 CSBR-167458 CSBR-167349 fixed
                if (!columnsChangedResult.ColumnMismatch && columnsChangedResult.InvalidatedColumns.Count == 0 && columnsChangedResult.InvalidatedHierarchies.Count == 0 && columnsChangedResult.NameChanges.Count == 0 && !columnsChangedResult.NoMatchingColumns)
                {
                    SpotfireCOETableDataSource colMovedBaseDataSource = new SpotfireCOETableDataSource(hitList, dataTable.Name, true, pubFormName, filterChildHits);
                    COEService service = application.GetService<COEService>();
                    COEDataView dataView = service.GetDataViewBO(hitList, false).COEDataView;
                    ColMovedLoadCOETablesIntoExistingDocument(application, dataView, colMovedBaseDataSource);
                }
                else
                {
                    ProgressService.CurrentProgress.CheckCancel();
                    ProgressService.CurrentProgress.ExecuteSubtask("Loading child tables");
                    application.MergeCOEChildTables(dataTable, hitList);
                    ProgressService.CurrentProgress.CheckCancel();
                    // If the number of DataTables changed, make sure the legend item
                    // on each visualization is either displayed or not as well.
                    foreach (Page page in application.Document.Pages)
                    {
                        foreach (Visual visual in page.Visuals)
                        {
                            Visualization visualization = visual.As<Visualization>();
                            if (visualization != null)
                            {
                                visualization.Data.DataTableLegendItem.Visible = application.Document.Data.Tables.Count > 1;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// It will update the spotfire datatable.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="newHitList"></param>
        /// <param name="hitListID"></param>
        /// <param name="oldDataViewID"></param>
        /// <returns>return status of the spotfire datatable update status true/false</returns>
        internal static bool UpdateSpotfireDataTable(this AnalysisApplication application, COEHitList newHitList, int hitListID, int oldDataViewID, bool filterChildHits)
        {
            COEService service = application.GetService<COEService>();
            bool isDataviewChanged = false;

            if (application.Document != null && IsDummyAnalysisDocument(application)) // no document is loaded in the analysis or it was the dummy document that was loaded 
            {
                // This closes the dummy document
                    application.Close();
            }
            if (oldDataViewID == 0)
            {
                UpdateSpotfireDataTable(application, newHitList, filterChildHits);
                return true;
            }

            if (oldDataViewID != newHitList.DataViewID)
            {
                isDataviewChanged = true;
            }

            //COEDataView dataView = service.GetDataViewBO(newHitList, filterChildHits).COEDataView;
            COEDataView dataView = service.LoadDataViewBO(newHitList).COEDataView;
            ResultsCriteria.ResultsCriteriaTable baseRcTable = newHitList.ResultsCriteria.Tables[0];
            if (baseRcTable.Id != dataView.Basetable)
            {
                throw new Exception("The ChemBioViz form was not generated from the base table of the dataview.");
            }

            string baseTableAlias = dataView.Tables.getById(dataView.Basetable).Alias;
            //CSBR:151920 formName parameter is added to SpotfireCOETableDataSource constructor to set Document Title.
            string formName = dataView.Name; // "New form";
            SpotfireCOETableDataSource baseDataSource = new SpotfireCOETableDataSource(newHitList, baseTableAlias, true, formName, hitListID, filterChildHits);
            pubFormName = formName;//formName parameter is made public to use at MergeCOEChildTables 
            DataTable baseDataTable = null;
            if (application.Document == null)
            {
                application.Open(baseDataSource);
                application.Document.Data.Tables.DefaultTableReference.Name = baseTableAlias;
                baseDataTable = application.Document.Data.Tables.DefaultTableReference;
            }
            else
                baseDataTable = application.LoadCOETablesIntoExistingDocument(dataView, baseDataSource, false, newHitList, isDataviewChanged);

            application.Document.Transactions.ExecuteTransaction(delegate
            {
                //baseDataTable.Name = formName;
                // Auto-configure the new document and remove the automatically generated cover page.
                application.Document.AutoConfigure();
                if (application.Document.ActivePageReference.Title.Equals("Page"))
                {
                    application.Document.ActivePageReference.Title = formName; //Setting DataView name as Page title when Page title value is "Page"
                }
                foreach (Page page in application.Document.Pages)
                {
                    if (page.Title == "Cover Page")
                    {
                        application.Document.Pages.Remove(page);
                        break;
                    }
                }
            });
            return true;
        }

        /// <summary>
        /// Sets the datatable metadata property with the Search Criteria fields order value and Filter Child Hits
        /// </summary>
        /// <param name="application">Spotfire analysis application object in use</param>
        /// <param name="searchFieldsOrder">Search criteria fields order collection value for storing in datatable property</param>
        internal static void UpdateDataTableProperties(this AnalysisApplication application, SearchFields searchFieldsOrder, bool filterChildHits)
        {
            Document document = application.Document;
            DataTable dataTable = document.Data.Tables.GetCOEBaseTable();

            if (dataTable != null)
            {
                SearchCriteriaFieldOrder fldsOrder = new SearchCriteriaFieldOrder();
                fldsOrder.SearchFieldsCollection = searchFieldsOrder;
                //set search criteria field order value in xml format to datatable property
                dataTable.Properties.SetProperty(Resources.COESearchCriteriaFieldOrder_PropertyName, fldsOrder.ToString());
                dataTable.Properties.SetProperty(Resources.FilterChildHits_PropertyName, filterChildHits);
            }
        }

        #endregion

        #region DataTableCollection methods

        internal static DataTable GetCOEDataTable(this DataTableCollection dataTables, int dataViewID, int tableId)
        {
            foreach (DataTable dataTable in dataTables)
            {
                if (dataTable.Properties.HasPropertyValue(Resources.COEDataViewID_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COETableID_PropertyName) &&
                    dataViewID == (int)dataTable.Properties.GetProperty(Resources.COEDataViewID_PropertyName) &&
                    tableId == (int)dataTable.Properties.GetProperty(Resources.COETableID_PropertyName))
                {
                    return dataTable;
                }
            }
            return null;
        }

        internal static DataTable GetCOEBaseTable(this DataTableCollection dataTables)
        {
            foreach (DataTable dataTable in dataTables)
            {
                if (dataTable.Properties.HasPropertyValue(Resources.COEDataView_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COEDataViewID_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COETableID_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COEResultsCriteria_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COEHitListID_PropertyName) &&
                    dataTable.Properties.HasPropertyValue(Resources.COEHitListType_PropertyName))
                {
                    string dataViewStr = (string)dataTable.Properties.GetProperty(Resources.COEDataView_PropertyName);
                    XmlDocument dataViewDoc = new XmlDocument();
                    try
                    {
                        dataViewDoc.LoadXml(dataViewStr);
                    }
                    catch (XmlException)
                    {
                        continue;
                    }
                    COEDataView dataView = new COEDataView(dataViewDoc);
                    if (dataView.Basetable == (int)dataTable.Properties.GetProperty(Resources.COETableID_PropertyName))
                    {
                        return dataTable;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if the datatable is dummy data table set by datalytix tool
        /// </summary>
        /// <param name="dataTables">DataTableCollection to check for dummy table existence</param>
        /// <returns>returns true if dummy table found; otherwise false</returns>
        internal static bool IsDummyAsBaseTable(this DataTableCollection dataTables)
        {
            foreach (DataTable dataTable in dataTables)
            {
                if (dataTable.Name.Equals("dummy", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region DataTable methods

        internal static COEHitList GetCOEHitList(this DataTable baseDataTable)
        {
            // Load all the hitlist data.
            int hitListID = (int)baseDataTable.Properties.GetProperty(Resources.COEHitListID_PropertyName);
            HitListType hitListType = (HitListType)baseDataTable.Properties.GetProperty(Resources.COEHitListType_PropertyName);
            int numHits = baseDataTable.RowCount;
            int dataViewID = (int)baseDataTable.Properties.GetProperty(Resources.COEDataViewID_PropertyName);
            ResultsCriteria resultsCriteria = GetCOEResultsCriteria(baseDataTable);
            //load the search criteria object from saved property
            SearchCriteria searchCriteria = GetCOESearchCriteria(baseDataTable);
            //SelectDataForm.TableSelectionMethod tableSelectionMethod = (SelectDataForm.TableSelectionMethod)baseDataTable.Properties.GetProperty(Resources.COEHitListType_PropertyName);
            return new COEHitList(hitListID, hitListType, numHits, dataViewID, resultsCriteria, searchCriteria);
        }

        internal static ResultsCriteria GetCOEResultsCriteria(this DataTable baseDataTable)
        {
            // Load the results criteria.
            string resultsCriteriaStr = (string)baseDataTable.Properties.GetProperty(Resources.COEResultsCriteria_PropertyName);
            XmlDocument resultsCriteriaDoc = new XmlDocument();
            resultsCriteriaDoc.LoadXml(resultsCriteriaStr);
            return new ResultsCriteria(resultsCriteriaDoc);
        }

        internal static SearchCriteria GetCOESearchCriteria(this DataTable baseDataTable)
        {
            // Load the search criteria.
            string searchCriteriaStr = (string)baseDataTable.Properties.GetProperty(Resources.COESearchCriteria_PropertyName);
            XmlDocument searchCriteriaDoc = new XmlDocument();
            searchCriteriaDoc.LoadXml(searchCriteriaStr);
            return new SearchCriteria(searchCriteriaDoc);
        }

        /// <summary>
        /// Gets the search criteria fields order saved in datatable property
        /// </summary>
        /// <param name="baseDataTable">the datatable which holds the search fields order property</param>
        /// <returns>returns the search criteria fields order object</returns>
        internal static SearchCriteriaFieldOrder GetSearchCriteriaFieldsOrder(this DataTable baseDataTable)
        {
            // Load the search criteria fields order from property.
            string searchCriteriaFieldsOrderStr = (string)baseDataTable.Properties.GetProperty(Resources.COESearchCriteriaFieldOrder_PropertyName);
            return new SearchCriteriaFieldOrder(searchCriteriaFieldsOrderStr);
        }

        /// <summary>
        /// Indicates whether Filter Child table rows based on query or not
        /// </summary>
        /// <param name="baseDataTable"></param>
        /// <returns>true/false</returns>
        internal static bool IsFilterChildHits(this DataTable baseDataTable)
        {
            // Load the search criteria fields order from property.
            return (bool)baseDataTable.Properties.GetProperty(Resources.FilterChildHits_PropertyName);
        }

        #endregion

        internal static bool IsResultCriteriaUpdated(this COEHitList hitList, ResultsCriteria resultCriteriaToCompare)
        {
            bool result = false;
            if (hitList.ResultsCriteriaXML.GetHashCode() != resultCriteriaToCompare.ToString().GetHashCode())
                result = true;
            return result;
        }

        internal static bool IsSearchCriteriaUpdated(this COEHitList hitList, SearchCriteria searchCriteriaToCompare)
        {
            bool result = false;
            if (hitList.SearchCriteria.ToString().GetHashCode() != searchCriteriaToCompare.ToString().GetHashCode())
                result = true;
            return result;
        }

        /// <summary>
        /// Checks if the current document is the DummyAnalysis document
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        private static bool IsDummyAnalysisDocument(AnalysisApplication application)
        {
            bool rc = false;
            if (application.Document != null && application.Document.Data.Tables.DefaultTableReference.Name == "dummy")
                rc = true;
            return rc;
        }

        /// <summary>
        /// Gets the connection information associated with the document from the document property
        /// </summary>
        /// <param name="application">analysis application instance</param>
        /// <returns>returns persisted object of connection information</returns>
        internal static CBOEConnectionPersistance GetConnInfo(this AnalysisApplication application)
        {
            CBOEConnectionPersistance connInfo = null;
            if (application.Document != null)
            {
                string cboeConnInfo = (string)application.Document.Properties[Resources.COEConnectionInfo_PropertyName];
                if (!string.IsNullOrEmpty(cboeConnInfo))
                {
                    connInfo = new CBOEConnectionPersistance(cboeConnInfo);
                }
            }
            return connInfo;
        }

        /// <summary>
        /// Sets the connection information in the document property
        /// </summary>
        /// <param name="application">analsyis applciation instance</param>
        /// <param name="currentMRUEntry">mru entry object that is currently in use</param>
        internal static void SetConnInfo(this AnalysisApplication application, MRUEntry currentMRUEntry)
        {
            CBOEConnectionPersistance connInfo = null;
            if (application.Document != null)
            {
                connInfo = new CBOEConnectionPersistance();
                connInfo.SetConnInfo(currentMRUEntry);
                application.Document.Properties[Resources.COEConnectionInfo_PropertyName] = connInfo.StrConnInfo;
            }
        }

        /// <summary>
        /// Sets the custom preference values to COEPreferenceSettings class from CustomPreferenceManager
        /// </summary>
        /// <param name="application">Spotfire analysis application instance</param>
        /// <param name="coeService">COEService instance</param>
        internal static void SetUserCustomPreferences(this AnalysisApplication application, COEService coeService)
        {
            CustomPreferenceManager theCustomPreferenceManager = CustomPreferenceManager.Instance(application);
            coeService.TheCOEPreferenceSettings.CompressData = theCustomPreferenceManager.IsCompressData;
            coeService.TheCOEPreferenceSettings.MaxRows = theCustomPreferenceManager.NumberOfMaxRow;
            coeService.TheCOEPreferenceSettings.PagingSize = theCustomPreferenceManager.PagingSize;
            coeService.TheCOEPreferenceSettings.UseRemoting = theCustomPreferenceManager.IsUseRemoting;
        }
    }

}
