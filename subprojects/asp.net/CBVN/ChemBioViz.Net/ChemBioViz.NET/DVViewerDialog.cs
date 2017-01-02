using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace ChemBioViz.NET
{
    public partial class DVViewerDialog : Form
    {
        #region Properties
        public UltraGrid UltraGrid
        {
            get { return this.ultraGrid1; }
        }
        #endregion

        #region Constructors
        public DVViewerDialog(ChemBioVizForm form)
        {
            m_form = form;
            InitializeComponent();

            ultraGrid1.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
            ultraGrid1.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            ultraGrid1.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;

        }
        private ChemBioVizForm m_form;
        #endregion

        #region Events
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void refreshDVbutton_click(object sender, EventArgs e)
        {
            m_form.RefreshDataViews();
        }
        #endregion
    }
}
