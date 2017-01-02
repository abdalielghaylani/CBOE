using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.CustomReportDesigner.Properties;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    public partial class DataBaseOpenDialog : Form
    {
        #region Variables
        private int _selectedId;
        private string _idColumName = "ID";
        #endregion

        #region Properties
        public int SelectedId
        {
            get { return _selectedId; }
            set { _selectedId = value; }
        }

        public string IdColumnName
        {
            get
            {
                return _idColumName;
            }
            set
            {
                _idColumName = value;
            }
        }

        public object DataSource
        {
            get
            {
                return AvailableRecordsDataGridView.DataSource;
            }
            set
            {
                AvailableRecordsDataGridView.DataSource = value;
                AvailableRecordsDataGridView.ClearSelection();
            }
        }

        public string DataMember
        {
            get
            {
                return AvailableRecordsDataGridView.DataMember;
            }
            set
            {
                //DataViewsDataGridView.AutoGenerateColumns = false;
                AvailableRecordsDataGridView.DataMember = value;
            }
        }

        #endregion

        #region Methods
        #region Constructors

        public DataBaseOpenDialog()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        public DataBaseOpenDialog(string dialogCaption)
            : this()
        {
            try
            {
                this.Text = dialogCaption;
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
        #endregion

        #region Event Handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!RetrieveSelectedId())
                    this.ShowErrorMessage(Resources.SelectARowWarning);
                else
                    this.DialogResult = DialogResult.OK;

            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
        private void DataViewsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RetrieveSelectedId();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
        #endregion

        #region internal Methods
        protected void HandleException(Exception exception)
        {
            this.ShowErrorMessage(exception.Message);
        }

        protected void ShowErrorMessage(string message)
        {
            this.ErrorsTextBox.Text = message;
            this.ErrorsTextBox.Visible = true;
        }

        protected bool RetrieveSelectedId()
        {
            if (AvailableRecordsDataGridView.SelectedRows.Count > 0)
            {
                SelectedId = int.Parse(AvailableRecordsDataGridView.SelectedRows[0].Cells[IdColumnName].Value.ToString());
                return true;
            }

            return false;
        }
        #endregion
        #endregion
    }
}
