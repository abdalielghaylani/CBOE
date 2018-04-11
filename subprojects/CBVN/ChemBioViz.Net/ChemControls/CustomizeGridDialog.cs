using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

namespace ChemControls
{
    public partial class CustomizeGridDialog : Form
    {
        ChemDataGrid grid;
        //bool cardview;

        public ChemDataGrid ChemDataGrid { get { return grid; } set { grid = value; } }

        public CustomizeGridDialog()
        {
            InitializeComponent();
            grid = null;
            //cardview = false;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            StandardView.Checked = ! grid.DisplayLayout.Bands[0].CardView;
            CardView.Checked = grid.DisplayLayout.Bands[0].CardView;

            this.cellsHorizontal.Checked = ! grid.CellArrangementOptimization;
            this.cellsOptimized.Checked = grid.CellArrangementOptimization;

            this.labelStyleSeparate.Checked = grid.DisplayLayout.Bands[0].RowLayoutLabelStyle == RowLayoutLabelStyle.Separate;
            this.labelStyleWithCells.Checked = grid.DisplayLayout.Bands[0].RowLayoutLabelStyle == RowLayoutLabelStyle.WithCellData;

            this.FilteringEnabled.Checked = grid.DisplayLayout.Bands[0].Override.AllowRowFiltering == Infragistics.Win.DefaultableBoolean.True;
            this.FilteringDisabled.Checked = grid.DisplayLayout.Bands[0].Override.AllowRowFiltering == Infragistics.Win.DefaultableBoolean.False;

            List<string> childBandNames = grid.GetChildTableNames();
            string childBandPreferred = grid.PreferredChildTable;

            foreach (string s in childBandNames)
                childTableNamesCombo.Items.Add(s);

            this.allChildTablesVertical.Checked = childBandPreferred == null;
            this.oneChildTableHorizontal.Checked = childBandPreferred != null;

            childTableNamesCombo.Text = childBandPreferred != null ? childBandPreferred : "";
        }

        private void StandardView_CheckedChanged(object sender, EventArgs e)
        {
            if (grid == null)
                return;

            grid.DisplayLayout.Bands[0].CardView = false;
        }

        private void CardView_CheckedChanged(object sender, EventArgs e)
        {
            if (grid == null)
                return;

            grid.DisplayLayout.Bands[0].CardView = true;
        }

        private void cellsHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            grid.CellArrangementOptimization = false;
        }

        private void cellsOptimized_CheckedChanged(object sender, EventArgs e)
        {
            grid.CellArrangementOptimization = true;
        }

        private void labelStyleSeparate_CheckedChanged(object sender, EventArgs e)
        {
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                band.RowLayoutLabelStyle = RowLayoutLabelStyle.Separate;
        }

        private void labelStyleWidthCells_CheckedChanged(object sender, EventArgs e)
        {
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                band.RowLayoutLabelStyle = RowLayoutLabelStyle.WithCellData;
                band.RowLayoutLabelPosition = LabelPosition.Top;

                band.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
                band.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
                band.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;

                band.Override.RowSpacingBefore = 0;
                band.Override.RowSpacingAfter = 0;

                band.Override.HeaderAppearance.BackColor = Color.LightGray;
                band.Override.HeaderAppearance.BackColor2 = Color.LightGray;

                band.Override.HeaderAppearance.BorderColor = Color.White;
                band.Override.HeaderAppearance.BorderColor2 = Color.White;
                band.Override.HeaderAppearance.BorderColor3DBase = Color.White;

                band.Override.HeaderStyle = Infragistics.Win.HeaderStyle.XPThemed;  // to have a flat appearance for the labels

                band.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Etched;

                int n = band.RowLayouts.Count;

                foreach (UltraGridColumn c in band.Columns)
                {

                    c.RowLayoutColumnInfo.LabelInsets.Left = 2;
                    c.RowLayoutColumnInfo.LabelInsets.Top = 2;
                    c.RowLayoutColumnInfo.LabelInsets.Right = 2;
                    c.RowLayoutColumnInfo.LabelInsets.Bottom = 2;

                    c.RowLayoutColumnInfo.CellInsets.Left = 2;
                    c.RowLayoutColumnInfo.CellInsets.Top = 2;
                    c.RowLayoutColumnInfo.CellInsets.Right = 5;
                    c.RowLayoutColumnInfo.CellInsets.Bottom = 2;
                
                }
            }

            grid.DisplayLayout.Override.CellClickAction = CellClickAction.CellSelect;

        }

        private void FilteringEnabled_CheckedChanged(object sender, EventArgs e)
        {
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                band.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
        }

        private void FilteringDisabled_CheckedChanged(object sender, EventArgs e)
        {
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                band.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
        }

        private void allChildTablesVertical_CheckedChanged(object sender, EventArgs e)
        {
            grid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.Vertical;
            grid.PreferChildTable(null);
        }

        private void oneChildTableHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            grid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.Horizontal;
            grid.PreferChildTable(childTableNamesCombo.Text);
        }
    }
}
