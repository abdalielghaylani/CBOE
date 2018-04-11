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
using Utilities;

namespace ChemBioViz.NET
{
    public partial class ConnectionsDialog : Form
    {
        #region Variables
        private ChemBioVizForm m_parentForm;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public ConnectionsDialog(ChemBioVizForm form)
        {
            InitializeComponent();
            this.CenterToParent();

            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.Columns[0].Width = 120;
            listView1.Columns[1].Width = 240;

            this.m_parentForm = form;
        }
        #endregion

        #region Methods
        public void AddItem(String propname, String propvalue)
        {
            ListViewItem item = new ListViewItem(propname);
            item.SubItems.Add(propvalue);
            listView1.Items.Add(item);
        }
        #endregion

        #region Events
        private void showDataViewsbutton_Click(object sender, EventArgs e)
        {
            DVViewerDialog dlg = new DVViewerDialog(m_parentForm);
            FormUtil.DataviewsToGrid(m_parentForm, dlg.UltraGrid);
            dlg.Show();
        }
        #endregion

    }
}
