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
    public partial class SortDialog : Form
    {
        #region Variables
        private List<String> m_fieldNames1, m_fieldNames2;
        private String m_fieldName1, m_fieldName2;
        private bool m_isAscending1, m_isAscending2;
        private ChemBioVizForm m_form;
        #endregion

        #region Properties
        public String FieldName1
        {
            get { return m_fieldName1; }
            set { m_fieldName1 = value; }
        }
        //---------------------------------------------------------------------
        public String FieldName2
        {
            get { return m_fieldName2; }
            set { m_fieldName2 = value; }
        }
        //---------------------------------------------------------------------
        public bool IsAscending1
        {
            get { return m_isAscending1; }
            set { m_isAscending1 = value; }
        }
        //---------------------------------------------------------------------
        public bool IsAscending2
        {
            get { return m_isAscending2; }
            set { m_isAscending2 = value; }
        }
        //---------------------------------------------------------------------
        public String SortString
        {
            get
            {
                String s = String.Empty;
                SortData sData = new SortData();
                if (!String.IsNullOrEmpty(m_fieldName1))
                    sData.Add(new SortField(m_fieldName1, m_isAscending1));
                if (!String.IsNullOrEmpty(m_fieldName2))    // allows adding fld2 without fld1
                    sData.Add(new SortField(m_fieldName2, m_isAscending2));
                s = SortData.SortDataToString(sData);
                return s;
            }
            set
            {
                String sortStr = value;
                m_isAscending1 = m_isAscending2 = true;
                m_fieldName1 = m_fieldName2 = String.Empty;
                if (!String.IsNullOrEmpty(sortStr))
                {
                    SortData sData = SortData.StringToSortData(sortStr);
                    if (sData.Count > 0)
                    {
                        m_fieldName1 = sData[0].m_fieldName;
                        m_isAscending1 = sData[0].m_bAscending;
                    }
                    if (sData.Count > 1)
                    {
                        m_fieldName2 = sData[1].m_fieldName;
                        m_isAscending2 = sData[1].m_bAscending;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Constructors
        public SortDialog(ChemBioVizForm form, String sortStr)
        {
            m_form = form;
            InitializeComponent();
            SortString = sortStr;
        }
        #endregion

        #region Events
        private void SortDialog_Load(object sender, EventArgs e)
        {
            // fill combos with field names
            ChemBioVizForm form = m_form; //  this.Owner as ChemBioVizForm;
            this.currListLabel.Text = String.Format("Current list: {0}", "none");
            Query qCurr = null;

            if (form != null)
            {
                m_fieldNames1 = form.FormDbMgr.GetFieldList(false /*struct*/, true /*blank first*/, false /*numeric*/, 0, false /*full rc*/);
                //m_fieldNames1 = form.FormDbMgr.GetFieldNames();     // includes subform fields
                m_fieldNames2 = new List<String>(m_fieldNames1);

                // TO DO: allow subform fields; allow more than two

                comboBox1.DataSource = m_fieldNames1;
                comboBox1.SelectedItem = m_fieldName1;
                comboBox2.DataSource = m_fieldNames2;
                comboBox2.SelectedItem = m_fieldName2;

                qCurr = m_form.CurrQuery;
                this.currListLabel.Text = String.Format("Current list: {0}", (qCurr == null) ? "none" : qCurr.Name);
            }
            this.Ascending1.Checked = m_isAscending1;
            this.Descending1.Checked = !m_isAscending1;
            this.Ascending2.Checked = m_isAscending2;
            this.Descending2.Checked = !m_isAscending2;
            //Coverity Bug Fix CID 13003 
            bool bIsThisTheDefaultQuery = qCurr != null && !String.IsNullOrEmpty(qCurr.DefaultSortString)
                                        && CBVUtil.Eqstrs(qCurr.DefaultSortString, SortString);
            this.alwaysSortCheckBox.Checked = bIsThisTheDefaultQuery;

            CheckButtons();
            this.CenterToParent();
        }
        //---------------------------------------------------------------------
        private void OKbutton_Click(object sender, EventArgs e)
        {
            m_fieldName1 = (comboBox1.SelectedItem != null) ? comboBox1.SelectedItem.ToString() : String.Empty;
            m_fieldName2 = (comboBox2.SelectedItem != null) ? comboBox2.SelectedItem.ToString() : String.Empty;
            m_isAscending1 = Ascending1.Checked;
            m_isAscending2 = Ascending2.Checked;

            // if box is unchecked, and this is the default sort, then clear it
            if (m_form.CurrQuery != null && !alwaysSortCheckBox.Checked &&
                CBVUtil.Eqstrs(m_form.CurrQuery.DefaultSortString, SortString))
                m_form.CurrQuery.DefaultSortString = String.Empty;
            else if (m_form.CurrQuery != null && alwaysSortCheckBox.Checked)
                m_form.CurrQuery.DefaultSortString = SortString;
            DialogResult = DialogResult.OK;
        }
        //---------------------------------------------------------------------
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        //---------------------------------------------------------------------
        private void CheckButtons()
        {
            // dim fld2 and checkbox if no fld1 chosen
            bool bFld1OK = !String.IsNullOrEmpty(comboBox1.SelectedItem as String);
            comboBox2.Enabled = bFld1OK;
            Ascending2.Enabled = bFld1OK;
            alwaysSortCheckBox.Enabled = bFld1OK;
        }
        //---------------------------------------------------------------------
        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            CheckButtons();
        }
        //---------------------------------------------------------------------
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.alwaysSortCheckBox.Checked = false;    // clear when change of field
            CheckButtons();
        }
        //---------------------------------------------------------------------
        #endregion

    }
}
