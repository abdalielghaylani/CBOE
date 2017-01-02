// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuTool.cs" company="PerkinElmer Inc.">
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

using System.IO;
using System.Linq;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Application.Tools;
using Spotfire.Dxp.Application.Visuals;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Framework.DocumentModel;
using CBVNStructureFilter;

namespace CBVNStructureFilter
{
    internal class MenuToolUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchType"></param>
        /// <param name="molecule"></param>
        internal static void ExecuteFilter(DocumentNode context, StructureFilterSettings.FilterModeEnum searchType, object molecule)
        {
            var panel = ShowPanel(context);

            if (panel == null || molecule == null)
            {
                return;
            }

            // Get the molecule string
            var structure = GetMoleculeString(molecule);

            // Update the GUI and execute the filter
            panel.SetFilterSettings(searchType, structure, false, false);
        }

        internal static int StructureColumnCount(DataColumnCollection columns)
        {
            return columns.Count(FilterUtilities.IsStructureColumn);
        }

        internal static int SelectedRowCount(VisualizationData data)
        {
            if (data == null || data.MarkingReference == null || data.DataTableReference == null)
            {
                return 0;
            }

            var selection = data.MarkingReference.GetSelection(data.DataTableReference);
            return selection.IncludedRowCount;
        }

        internal static StructureFilterPanel ShowPanel(DocumentNode context)
        {
            // Check if the panel is already displayed
            var doc = context.Context.GetAncestor<Document>();
            var panel = GetStructureFilterPanel(context);
            if (panel == null)
            {
                // Display the panel
                panel = doc.ActivePageReference.Panels.AddNew<StructureFilterPanel>();
            }
            else
            {
                // Ensure the panel is visible
               panel.Visible = true;
            }
            return panel;
        }

        internal static bool IsRGroupApplied(DocumentNode context)
        {
            var panel = GetStructureFilterPanel(context);
            return panel != null && panel.StructureFilterModel.RGroupDecomposition;
        }

        internal static bool IsBusy(VisualizationData data)
        {
            if (!DataTableInfoMgr.ContainsKey(data.DataTableReference.Id))
            {
                return false;
            }

            var state =
                DataTableInfoMgr.GetSearchState(data.DataTableReference.Id);
            return state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Busy ||
                   state == DataTableInfoMgr.DataTableInfo.SearchStateEnum.Editting;
        }

        internal static StructureFilterPanel GetStructureFilterPanel(DocumentNode context)
        {
            var doc = context.Context.GetAncestor<Document>();
            if (!doc.ActivePageReference.Panels.OfType<StructureFilterPanel>().Any())
            {
                return null;
            }

            var panels = doc.ActivePageReference.Panels.OfType<StructureFilterPanel>();
            return panels.First();
        }

        internal static string GetMoleculeString(object value)
        {
            var o = value as BinaryLargeObject;
            if (o == null)
            {
                return value.ToString();
            }
            var binaryLargeObject = o;
            using (StreamReader sr = new StreamReader(binaryLargeObject.GetByteStream()))
            {
                string returnValue = sr.ReadToEnd();
                return returnValue;
            }
        }
    }

    //internal abstract class TablePlotMenuTool : CustomTool<TablePlotCellContext>
    //{
    //    protected TablePlotMenuTool(string name) 
    //        : base(name, ChemistryLicense.Functions.StructureFilterFunction)
    //    {}

    //    protected override bool IsVisibleCore(TablePlotCellContext context)
    //    {
    //        return MenuToolUtilities.StructureColumnCount(context.TablePlot.Data.DataTableReference.Columns) > 0;
    //    }
    //}

    //internal class TablePlotFilterMenuTool : TablePlotMenuTool
    //{
    //    protected StructureFilterSettings.FilterModeEnum SearchType;

    //    public TablePlotFilterMenuTool(string name)
    //        : base(name)
    //    {}

    //    protected override bool IsEnabledCore(TablePlotCellContext context)
    //    {
    //        if (context.TablePlot == null || context.TablePlot.Data == null || context.TableColumn.DataColumn == null)
    //        {
    //            return false;
    //        }

    //        // Return true if R-Group decomposition isn't applied, we're not filtering/indexing/editing, and the user right-clicked on the structure column
    //        return !MenuToolUtilities.IsRGroupApplied(context.TablePlot) &&
    //               !MenuToolUtilities.IsBusy(context.TablePlot.Data) &&
    //               FilterUtilities.IsStructureColumn(context.TableColumn.DataColumn);
    //    }

    //    protected override void ExecuteCore(TablePlotCellContext context)
    //    {
    //        if (context.DataValue.HasValidValue)
    //        {
    //            MenuToolUtilities.ExecuteFilter(context.TablePlot, SearchType, context.DataValue.ValidValue);
    //        }
    //    }
    //}

    //internal class ScatterPlotMenuTool : CustomTool<ScatterPlot>
    //{
    //    protected StructureFilterSettings.FilterModeEnum SearchType;

    //    public ScatterPlotMenuTool(string name)
    //        : base(name, ChemistryLicense.Functions.StructureFilterFunction)
    //    {
    //    }

    //    protected override bool IsVisibleCore(ScatterPlot context)
    //    {
    //        if (context != null)
    //        {
    //            return MenuToolUtilities.StructureColumnCount(context.Data.DataTableReference.Columns) > 0;
    //        }

    //        return false;
    //    }

    //    protected override bool IsEnabledCore(ScatterPlot context)
    //    {
    //        // Check if R-Group decomposition is applied
    //        if (MenuToolUtilities.IsRGroupApplied(context))
    //        {
    //            return false;
    //        }

    //        if (context == null)
    //        {
    //            return false;
    //        }

    //        // Are we currently indexing/filtering the table or editing the structure
    //        if (MenuToolUtilities.IsBusy(context.Data))
    //        {
    //            return false;
    //        }

    //        // The filter menu should be enabled if 1 and only 1 row is selected
    //        return MenuToolUtilities.SelectedRowCount(context.Data) == 1;
    //    }

    //    protected override void ExecuteCore(ScatterPlot context)
    //    {
    //        var panel = MenuToolUtilities.ShowPanel(context);
    //        if (panel == null)
    //        {
    //            return;
    //        }
    //        var molecule = GetMoleculeData(panel);
    //        MenuToolUtilities.ExecuteFilter(context, SearchType, molecule);
    //    }

    //    private static object GetMoleculeData(StructureFilterPanel panel)
    //    {
    //        if (panel.DataColumnReference == null)
    //        {
    //            return null;
    //        }

    //        // Get the selected molecule data
    //        var cursor = new DataValueCursor[1];
    //        if (panel.DataColumnReference.RowValues.DataType == DataType.Binary)
    //        {
    //            cursor[0] = DataValueCursor.Create<BinaryLargeObject>(panel.DataColumnReference);
    //        }
    //        else
    //        {
    //            cursor[0] = DataValueCursor.CreateFormatted(panel.DataColumnReference);
    //        }
    //        var doc = panel.Context.GetAncestor<Document>();
    //        var selection = doc.ActiveMarkingSelectionReference;
    //        var dataRow = doc.ActiveDataTableReference.GetRows(selection.GetSelection(doc.ActiveDataTableReference).AsIndexSet(), cursor);
    //        return panel.DataColumnReference.RowValues.GetValue(dataRow.First().Index).ValidValue;
    //    }
    //}

    //// Each menu item in the context menu has to be implemented as a separate tool
    //// so we have three tools that are all derived from the same base class where
    //// all the work is done
    //internal class FullStructureSearchTableMenuTool : TablePlotFilterMenuTool
    //{
    //    public FullStructureSearchTableMenuTool()
    //        : base(Properties.Resources.ExactSearch)
    //    {
    //        SearchType = StructureFilterSettings.FilterModeEnum.FullStructure;
    //    }
    //}

    //internal class FullStructureSearchScatterMenuTool : ScatterPlotMenuTool
    //{
    //     public FullStructureSearchScatterMenuTool()
    //         : base(Properties.Resources.ExactSearch)
    //     {
    //         SearchType = StructureFilterSettings.FilterModeEnum.FullStructure;
    //     }
    //}

    //internal class SubstructureSearchTableMenuTool : TablePlotFilterMenuTool
    //{
    //    public SubstructureSearchTableMenuTool()
    //        : base(Properties.Resources.SubStructureSearch)
    //    {
    //        SearchType = StructureFilterSettings.FilterModeEnum.SubStructure;
    //    }
    //}

    //internal class SubstructureSearchScatterMenuTool : ScatterPlotMenuTool
    //{
    //    public SubstructureSearchScatterMenuTool()
    //        : base(Properties.Resources.SubStructureSearch)
    //    {
    //        SearchType = StructureFilterSettings.FilterModeEnum.SubStructure;
    //    }
    //}

    //internal class SimilaritySearchTableMenuTool : TablePlotFilterMenuTool
    //{
    //    public SimilaritySearchTableMenuTool()
    //        : base(Properties.Resources.SimilaritySearch)
    //    {
    //        SearchType = StructureFilterSettings.FilterModeEnum.Simularity;
    //    }
    //}

    //internal class SimilaritySearchScatterMenuTool : ScatterPlotMenuTool
    //{
    //    public SimilaritySearchScatterMenuTool()
    //        : base(Properties.Resources.SimilaritySearch)
    //    {
    //        SearchType = StructureFilterSettings.FilterModeEnum.Simularity;
    //    }
    //}

    //internal class RGroupMenuTool : TablePlotMenuTool
    //{
    //    public RGroupMenuTool()
    //        : base(Properties.Resources.RGD)
    //    { }

    //    protected override bool IsEnabledCore(TablePlotCellContext context)
    //    {
    //        if (context.TablePlot.Data == null || context.TableColumn == null || context.TableColumn.DataColumn == null)
    //        {
    //            return false;
    //        }

    //        // Return true if we're not filtering/indexing/editing, and the user right-clicked on the structure column
    //        return !MenuToolUtilities.IsBusy(context.TablePlot.Data) &&
    //               FilterUtilities.IsStructureColumn(context.TableColumn.DataColumn);
    //    }

    //    protected override void ExecuteCore(TablePlotCellContext context)
    //    {
    //        if (context.DataValue.HasValidValue)
    //        {
    //            ExecuteRGroup(context.TablePlot, context.DataValue.ValidValue);
    //        }
    //    }

    //    private static void ExecuteRGroup(DocumentNode context, object molecule)
    //    {
    //        var panel = MenuToolUtilities.ShowPanel(context);

    //        if (panel == null || molecule == null)
    //        {
    //            return;
    //        }

    //        // Get the molecule string
    //        var structure = MenuToolUtilities.GetMoleculeString(molecule);

    //        // Update the GUI and execute the filter
    //        panel.SetFilterSettings(StructureFilterSettings.FilterModeEnum.SubStructure, structure, true, false);

    //    }
    //}



}
