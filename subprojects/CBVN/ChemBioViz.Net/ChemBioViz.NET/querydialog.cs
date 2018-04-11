using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FormDBLib;
using CBVUtilities;

namespace ChemBioViz.NET
{
    public partial class QueryDialog : Form
    {
        #region Variables
        private Query m_query;
        #endregion

        #region Constructors
        public QueryDialog(Query q)
        {
            InitializeComponent();
            m_query = q;
            CenterToParent();
        }
        #endregion

        #region Events
        private void QueryDialog_Load(object sender, EventArgs e)
        {
            // transfer query properties to dialog
            //Coverity Bug Fix 
            if (m_query != null)
            {
                ChemBioVizForm form = m_query.QueryCollection.Form as ChemBioVizForm;
                nameBox.Text = m_query.Name;
                descrBox.Text = m_query.Description;
                idTextBox.Text = CBVUtil.IntToStr(m_query.ID);
                if (m_query is RetrieveAllQuery)
                    idTextBox.Text = "RA";

                sortTextBox.Text = m_query.DefaultSortString;
                this.curSortString.Text = (form != null && form.CurrQuery != null) ? form.CurrQuery.SortString : string.Empty;

                hitsBox.Text = CBVUtil.IntToStr(m_query.NumHits);
                if (m_query.IsMergeQuery)
                {
                    merge1Box.Text = (m_query as MergeQuery).Query1.Name;
                    merge2Box.Text = (m_query as MergeQuery).Query2.Name;
                }
                if (m_query.HitListID != 0)
                    hitlistIDBox.Text = CBVUtil.IntToStr(m_query.HitListID);

                savedcheckBox.Checked = m_query.IsSaved;
                mergecheckBox.Checked = m_query.IsMergeQuery;
                reruncheckBox.Checked = false;
                discardcheckBox.Checked = m_query.IsFlagged(Query.QueryFlag.kfDiscard);
                restoreLcheckBox.Checked = m_query.CanRestoreHitlist;
                restoreQcheckBox.Checked = false;
                //Coverity Bug Fix CID 13001 
                FormViewControl formView = null;
                formView = form != null ? form.GetQueryFormView(m_query) : null;
                if (formView != null)
                {
                    reruncheckBox.Checked = m_query.CanRerun(formView);
                    restoreQcheckBox.Checked = m_query.CanRestoreToForm(formView);
                }

                int queryIDOnOpen = m_query.QueryCollection.QueryOnOpen;
                runOnOpencheckBox.Checked = m_query.ID == queryIDOnOpen;
            }
        }
        //---------------------------------------------------------------------
        private void OKbutton_Click(object sender, EventArgs e)
        {
            // OK: transfer dialog values to query
            m_query.Name = nameBox.Text;
            m_query.Description = descrBox.Text;
            m_query.Flag(Query.QueryFlag.kfDiscard, discardcheckBox.Checked);
            m_query.DefaultSortString = sortTextBox.Text;   // CSBR-142590

            if (runOnOpencheckBox.Checked)
                m_query.QueryCollection.QueryOnOpen = m_query.ID;
            else if (m_query.ID == m_query.QueryCollection.QueryOnOpen)
                m_query.QueryCollection.QueryOnOpen = 0;

            DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            // Cancel: no change of query
            DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        private void sortBrowseButton_Click(object sender, EventArgs e)
        {
            ChemBioVizForm form = m_query.QueryCollection.Form as ChemBioVizForm;
            SortDialog dialog = new SortDialog(form, sortTextBox.Text);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.Cancel)
                sortTextBox.Text = dialog.SortString;
        }
        #endregion
    }
}
