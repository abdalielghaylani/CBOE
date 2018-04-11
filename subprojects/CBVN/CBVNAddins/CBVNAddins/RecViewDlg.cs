using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ChemBioViz.NET;
using FormDBLib;
using CBVUtilities;
using ChemControls;

namespace CBVNAddins
{
    public partial class RecViewDlg : Form
    {
        private ChemBioVizForm m_form;

        //---------------------------------------------------------------------
        public RecViewDlg(ChemBioVizForm form)
        {
            m_form = form;
            InitializeComponent();

            form.FormClosing += new FormClosingEventHandler(form_FormClosing);
            form.RecordChanged += new ChemBioVizForm.RecordChangedEventHandler(form_RecordChanged);
        }
        //---------------------------------------------------------------------
        void form_RecordChanged(object sender, RecordChangedEventArgs e)
        {
            int currRec = m_form.Pager.CurrRowInPage;
            dataGridView1.Rows[currRec].Selected = true;
        }
        //---------------------------------------------------------------------
        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        private void RecViewDlg_Load(object sender, EventArgs e)
        {
            DataSet dataSet = m_form.Pager.CurrDataSet;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                this.dataGridView1.DataSource = dataSet.Tables[0];
            }
        }
        //---------------------------------------------------------------------
        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            //Coverity BUg Fix : CID 12911 
            if (dgv != null && dgv.SelectedRows.Count > 0)
            {
                int selIndex = dgv.SelectedRows[0].Index;
                m_form.DoMove(Pager.MoveType.kmGotoPageRow, selIndex);
            }
        }
        //---------------------------------------------------------------------
    }
}
