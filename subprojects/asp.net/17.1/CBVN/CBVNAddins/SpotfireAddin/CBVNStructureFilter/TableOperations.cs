// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableOperations.cs" company="PerkinElmer Inc.">
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
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Application.Visuals;
    using Spotfire.Dxp.Data;
    using Spotfire.Dxp.Data.VirtualColumns;
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework;
    using log4net;
    using Properties;
    using CBVNStructureFilterSupport.Framework;        

    internal class TableOperations
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureFilterModel));
        
        #endregion

        #region Methods

        private delegate void TablePlotOperation(TablePlot tableVis);

        private static void ActOnEachTablePlotForDataTable(Document document, TablePlotOperation operation, Guid tableId)
        {
            foreach (var documentPage in document.Pages)
            {
                foreach (var visual in documentPage.Visuals)
                {
                    if (visual.TypeId != VisualTypeIdentifiers.Table)
                    {
                        continue;
                    }
                    var tableVis = visual.As<TablePlot>();
                    if (tableVis.Data.DataTableReference.Id == tableId)
                    {
                        operation.Invoke(tableVis);
                    }
                }
            }
        }

        //internal static void RemoveRGroupDecompositionColumnsFromDocument(Document document, DataTable dataTable)
        //{
        //    // Clear the R-group properties
        //    var rGroupData =
        //        new RGroupDecompositionData(0, String.Empty, null, null);

        //    SetRGDResults(document, rGroupData, dataTable.Id, false, (bool)dataTable.Properties.GetProperty(Identifiers.RGroupNickNames));
        //    ActOnEachTablePlotForDataTable(document, RemoveRGroupDecompositionColumns, dataTable.Id);
        //}

        //private static void RemoveRGroupDecompositionColumns(TablePlot tableVis)
        //{
        //    var doc = tableVis.Context.GetAncestor<Document>();
        //    doc.Transactions.ExecuteTransaction(
        //        delegate
        //        {
        //            foreach (var producer in tableVis.VirtualColumnProducers)
        //            {
        //                if (producer.TypeId == StructureFilterVirtualColumnIdentifiers.RGDVirtualColumnIdentifier)
        //                {

        //                    tableVis.VirtualColumnProducers.Remove(producer);
        //                    break;
        //                }
        //            }
        //        });
        //}

        //private static void SetRGDResults(Document document, RGroupDecompositionData rGroupData, Guid dataTableId, bool enabled, bool nickNames)
        //{
        //    DataManager dataManager = document.Context.GetService<DataManager>();
        //    DataTable dataTable = GetDataTable(dataManager, dataTableId);
        //    if (dataTable != null)
        //    {
        //        // store values in Data table for use by other visualizations
        //        document.Transactions.ExecuteTransaction(delegate
        //        {
        //            dataTable.Properties.SetProperty(Identifiers.NumberRGroups, rGroupData.NumberOfGroups);
        //            dataTable.Properties.SetProperty(Identifiers.RGroupStructure,
        //                                             rGroupData.StructureType == StructureStringType.CDX
        //                                                 ? Convert.ToBase64String(rGroupData.CDXStructure)
        //                                                 : rGroupData.TemplateStructure);
        //            dataTable.Properties.SetProperty(Identifiers.RGroupStructureType, (int)rGroupData.StructureType);
        //            dataTable.Properties.SetProperty(Identifiers.StructureFilterRGroup, enabled);
        //            dataTable.Properties.SetProperty(Identifiers.RGroupNickNames, nickNames);
        //        });
        //    }
        //}

        //internal static void AddRGroupDecompositionColumnsToDocument(Document document, RGroupDecompositionData rGroupData, string dataColumnName, Guid dataTableId, bool nickNames)
        //{
        //    if (rGroupData.NumberOfGroups == 0)
        //    {
        //        Log.Warn("R-Group Decomposition found no groups");
        //        MessageBox.Show(Resources.RGDNoGroups, Resources.RGD, MessageBoxButtons.OK);
        //        return;
        //    }

        //    if (dataColumnName == null)
        //    {
        //        Log.Error("No Id column defined for R-Group Decomposition");
        //        return;
        //    }

        //    SetRGDResults(document, rGroupData, dataTableId, true, nickNames);

        //    ActOnEachTablePlotForDataTable(document,
        //                                   delegate(TablePlot tableVis)
        //                                   {
        //                                       //AddRGroupDecompositionColumnsToTablePlot(document, tableVis,
        //                                       //                                         rGroupData, dataColumnName,
        //                                       //                                         dataTableId, nickNames);
        //                                   }, dataTableId);
        //}

        //private static void AddRGroupDecompositionColumnsToTablePlot(Document document, TablePlot tableVis, RGroupDecompositionData rGroupData, string dataColumnName, Guid id, bool nickNames)
        //{
        //    DataManager dataManager = document.Context.GetService<DataManager>();
        //    RemoveRGroupDecompositionColumns(tableVis);
        //    DataTable dataTable = GetDataTable(dataManager, id);
        //    if (dataTable != null)
        //    {
        //        DataColumn dataColumn;
        //        if (dataTable.Columns.TryGetValue(dataColumnName, out dataColumn))
        //        {
        //            document.Transactions.ExecuteTransaction(
        //                delegate
        //                {
        //                    VirtualColumnProducer producer =
        //                        tableVis.VirtualColumnProducers.AddNew(
        //                            StructureFilterVirtualColumnIdentifiers.RGDVirtualColumnIdentifier);
        //                    var rgdProducer = (RGDVirtualColumnProducer)producer;

        //                    rgdProducer.NumberRGroups = rGroupData.NumberOfGroups;
        //                    rgdProducer.DataTableId = id;
        //                    rgdProducer.StringType = rGroupData.StructureType;
        //                    rgdProducer.SelectedInput = new DataColumnSignature(dataColumn);
        //                    rgdProducer.NickNames = nickNames;

        //                    TypeIdentifier rendererId = GetStructureColumnRendererId(tableVis, dataColumn);
                            
        //                    foreach (VirtualColumn col in producer.Columns)
        //                    {
        //                        if (!tableVis.AutoAddNewColumns)
        //                        {
        //                            tableVis.TableColumns.Add(col, rendererId);
        //                        }
        //                        else
        //                        {
        //                            tableVis.TableColumns.SetValueRenderer(col, rendererId);
        //                        }
        //                    }

        //                });
        //        }
        //    }
        //}

        private static TypeIdentifier GetStructureColumnRendererId(TablePlot tableVis, DataColumn dataColumn)
        {
            TableColumn structureCol;
            TypeIdentifier rendererId = ChemDrawRendererIdentifiers.ChemDrawRenderer;
            if (tableVis.TableColumns.TryGetTableColumn(dataColumn,
                                                        out structureCol))
            {
                rendererId = structureCol.ValueRendererSettings.TypeId;
            }

            return rendererId;
        }

        public static StructureColumnRendererModel GetStructureColumnRenderer(TablePlot table, string dataColumnName)
        {
            foreach (TableColumn column in table.TableColumns)
            {
                if (column.DataColumn != null && column.Name.Equals(dataColumnName))
                {

                    StructureColumnRendererModel columnRendererModel = column.ValueRendererSettings as StructureColumnRendererModel;
                    return columnRendererModel;

                }
            }
            return null;
        }

        internal static DataTable GetDataTable(DataManager dataManager, Guid id)
        {
            DataTable dataTable;
            dataManager.Tables.TryGetValue(id, out dataTable);
            return dataTable;
        }

        internal static DataColumn GetTagsColumn(DataTable dataTable)
        {
            if (dataTable == null)
            {
                return null;
            }
            var tagColumnName = (string)dataTable.Properties.GetProperty(Identifiers.StructureMatchColumn);
            if (!String.IsNullOrEmpty(tagColumnName) && dataTable.Columns.Contains(tagColumnName))
            {
                return dataTable.Columns[tagColumnName];
            }
            return null;
        }

        internal static void AddTagsColumn(DataTable dataTable)
        {
            if (dataTable == null)
            {
                return;
            }
            if (GetTagsColumn(dataTable) != null)
            {
                return;
            }

            //add tags column
            string tagsColumnName = GenerateTagsColumnName(dataTable);
            var tagValues = new List<string>();
            tagValues.Add(InvariantResources.MatchTrue);
            tagValues.Add(InvariantResources.MatchFalse);
            dataTable.Columns.AddTagsColumn(tagsColumnName, tagValues);
            dataTable.Properties.SetProperty(Identifiers.StructureMatchColumn, tagsColumnName);
        }

        private static string GenerateTagsColumnName(DataTable dataTable)
        {
            return dataTable.Columns.CreateUniqueName(InvariantResources.StructureTagColumn);
        }

        #endregion
    }
}
