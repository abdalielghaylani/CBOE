using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    public partial class ResultsCriteriaEditor : Form
    {
        #region Variables
        private ResultsCriteria _resultsCriteria;
        private COEDataView _dataView;
        #endregion

        #region Properties
        public ResultsCriteria ResultsCriteria
        {
            get 
            {
                this.UnbindResultsCriteria();
                return _resultsCriteria;
            }
            set 
            {
                _resultsCriteria = ResultsCriteria.GetResultsCriteria(value.ToString());
                DisplayAvailableCriteria();
            }
        }
        #endregion

        #region Constructors
        public ResultsCriteriaEditor(ResultsCriteria resultsCriteria, COEDataView dataView)
        {
            InitializeComponent();
            _dataView = dataView;
            ResultsCriteria = resultsCriteria;
        }
        #endregion

        #region Events
        private void ResultsCriteriaEditor_Load(object sender, EventArgs e)
        {
            //this.DisplayAvailableCriteria();
        }

        private void AvailableCriteriumsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.AvailableCriteriumsTreeView.SelectedNode.Tag != null && this.SelectedCriteriumPropertiesEditor.Criterium != null)
                this.AvailableCriteriumsTreeView.SelectedNode.Tag = this.SelectedCriteriumPropertiesEditor.Criterium;

            if (e.Node.Tag != null)
            {
                this.SelectedCriteriumPropertiesEditor.Criterium = (ResultsCriteria.IResultsCriteriaBase)e.Node.Tag;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this.AvailableCriteriumsTreeView.SelectedNode.Tag != null && this.SelectedCriteriumPropertiesEditor.Criterium != null)
                this.AvailableCriteriumsTreeView.SelectedNode.Tag = this.SelectedCriteriumPropertiesEditor.Criterium;

            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Private Methods
        
        private void UnbindResultsCriteria()
        {
            foreach (ResultsCriteria.ResultsCriteriaTable currentTable in _resultsCriteria.Tables)
            {
                TreeNode currentTableNode = AvailableCriteriumsTreeView.Nodes[_dataView.Tables.getById(currentTable.Id).Alias];
                
                foreach (TreeNode currentCriteriumNode in currentTableNode.Nodes)
                {
                    if(currentTable[currentCriteriumNode.Name] != null)
                        currentTable[currentCriteriumNode.Name] = (ResultsCriteria.IResultsCriteriaBase)currentCriteriumNode.Tag;
                    //else new node, so add it.
                }

            }
        }

        private void DisplayAvailableCriteria()
        {
            AvailableCriteriumsTreeView.Nodes.Clear();

            foreach (ResultsCriteria.ResultsCriteriaTable currentTable in _resultsCriteria.Tables)
            {
                TreeNode currentTableNode = new TreeNode();
                currentTableNode.Name = currentTableNode.Text = _dataView.Tables.getById(currentTable.Id).Alias;

                foreach (ResultsCriteria.IResultsCriteriaBase currentCriterium in currentTable.Criterias)
                {
                    TreeNode currentCriteriumNode = new TreeNode();
                    currentCriteriumNode.Name = currentCriteriumNode.Text = currentCriterium.Alias;
                    currentCriteriumNode.Tag = currentCriterium;
                    currentTableNode.Nodes.Add(currentCriteriumNode);
                }

                AvailableCriteriumsTreeView.Nodes.Add(currentTableNode);
            }
        }
        #endregion
    }
}
