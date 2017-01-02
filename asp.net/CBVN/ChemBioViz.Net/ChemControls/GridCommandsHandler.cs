using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;

namespace ChemControls
{
    public class GridCommandsHandler
    {
        public void UpdateGridView(UltraToolbarsManager manager, ChemDataGrid grid)
        {
            StateButtonTool tool = manager.Tools["GridView"] as StateButtonTool;
            //Coverity Bug Fix CID 11424  
            if (tool != null)
                tool.Checked = !grid.DisplayLayout.Bands[0].CardView;
        }

        public void UpdateCardView(UltraToolbarsManager manager, ChemDataGrid grid)
        {
            StateButtonTool tool = manager.Tools["CardView"] as StateButtonTool;
            //Coverity Bug Fix CID 11423 
            if (tool != null)
                tool.Checked = grid.DisplayLayout.Bands[0].CardView;
        }

        public void ExecuteGridView(ChemDataGrid grid) { grid.DisplayLayout.Bands[0].CardView = false; }
        public void ExecuteCardView(ChemDataGrid grid) { grid.DisplayLayout.Bands[0].CardView = true; }

        public void UpdateLabelsInHeaders(UltraToolbarsManager manager, ChemDataGrid grid)
        {
            StateButtonTool tool = manager.Tools["LabelsInHeaders"] as StateButtonTool;
            //Coverity Bug Fix CID 11426 
            if (tool != null)
                tool.Checked = grid.DisplayLayout.Bands[0].RowLayoutLabelStyle == RowLayoutLabelStyle.Separate;
        }

        public void UpdateLabelsInCells(UltraToolbarsManager manager, ChemDataGrid grid)
        {
            StateButtonTool tool = manager.Tools["LabelsInCells"] as StateButtonTool;
            //Coverity Bug Fix CID 11425  
            if (tool != null)
                tool.Checked = grid.DisplayLayout.Bands[0].RowLayoutLabelStyle == RowLayoutLabelStyle.WithCellData;
        }

        public void ExecuteLabelsInHeaders(ChemDataGrid grid)
        {
            grid.DisplayLayout.Bands[0].RowLayoutLabelStyle = RowLayoutLabelStyle.Separate;
            grid.DisplayLayout.Bands[0].UseRowLayout = true;
        }

        public void ExecuteLabelsInCells(ChemDataGrid grid)
        {
            grid.DisplayLayout.Bands[0].RowLayoutLabelStyle = RowLayoutLabelStyle.WithCellData;
            grid.DisplayLayout.Bands[0].UseRowLayout = true;
        }
    }
}
