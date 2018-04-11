using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FormDBLib;

namespace ChemBioViz.NET
{
    public partial class MergeListDialog : Form
    {
        #region Variables
        private Query m_q1;
        private Query m_q2;
        private MergeQuery.LogicChoice m_choice;
        #endregion

        #region Properties
        public MergeQuery.LogicChoice LogicChoice
        {
            get { return m_choice; }
        }
        #endregion

        #region Constructors
        public MergeListDialog()
        {
            InitializeComponent();
        }

        public MergeListDialog(Query q1, Query q2)
        {
            InitializeComponent();
            m_q1 = q1;
            m_q2 = q2;
            this.ConfigureDialog();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configures MergeList Dialog
        /// </summary>
        private void ConfigureDialog() 
        {
            this.CenterToParent();
            intersectRadioButton.Checked = true;
            q1Label.Text = GetQueryText(m_q1.Name, m_q1.Description, m_q1.NumHits);
            q2Label.Text = GetQueryText(m_q2.Name, m_q2.Description, m_q2.NumHits);
        }
        /// <summary>
        /// Gets the formatted text for queries on tree. 
        /// </summary>
        /// <param name="qName"></param>
        /// <param name="qDescription"></param>
        /// <param name="qNumHits"></param>
        /// <returns></returns>
        private string GetQueryText(string qName, string qDescription, int qNumHits)
        {
            // name must be followed by colon
            return string.Format("{0}: {1} [{2:D}]", qName, qDescription, qNumHits);
        }
        /// <summary>
        /// Sets member variable based on radio choice
        /// </summary>
        private void SetLogicChoice()
        {
            if (intersectRadioButton.Checked)
                m_choice = MergeQuery.LogicChoice.kmIntersect;
            else if (subtractRadioButton.Checked)
                m_choice = MergeQuery.LogicChoice.kmSubtract;
            else if (unionRadioButton.Checked)
                m_choice = MergeQuery.LogicChoice.kmUnion;
            else
                m_choice = MergeQuery.LogicChoice.kmSubtractFrom;
        }
        #endregion

        #region Events
        /// <summary>
        /// Handles the Click event for OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKultraButton_Click(object sender, EventArgs e)
        {
            SetLogicChoice();
            this.Close();
        }
        /// <summary>
        /// Handles the Click event for Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelUltraButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
