using System;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.CustomReportDesigner.Properties;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    public partial class DataBaseSaveDialog : DataBaseOpenDialog
    {
        #region Properties
        public string NewRecordName
        {
            get
            {
                return NameTextBox.Text;
            }
            set
            {
                NameTextBox.Text = value;
            }
        }

        public string NewRecordDescription
        {
            get
            {
                return DescriptionTextBox.Text;
            }
            set
            {
                DescriptionTextBox.Text = value;
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public DataBaseSaveDialog()
        {
            InitializeComponent();
            this.AvailableRecordsDataGridView.SelectionChanged += new EventHandler(AvailableRecordsDataGridView_SelectionChanged);
        }
        public DataBaseSaveDialog(string caption)
            : this()
        {
            this.Text = caption;
        }
        #endregion

        #region Event Handlers
        void AvailableRecordsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.NewRecordName = this.NewRecordDescription = string.Empty;
        }
        
        private void AcceptUIButton_Click(object sender, EventArgs e)
        {
            if (this.AvailableRecordsDataGridView.SelectedRows.Count > 0 && string.IsNullOrEmpty(this.DescriptionTextBox.Text) && string.IsNullOrEmpty(this.NameTextBox.Text))
            {
                if (MessageBox.Show(Resources.DuplicatedRecordMessage, Resources.ConfirmOverwriting, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (this.RetrieveSelectedId())
                        this.DialogResult = DialogResult.OK;
                }
                else
                    this.DialogResult = DialogResult.None;
            }
            else
            {
                this.SelectedId = 0;
                DialogResult = DialogResult.OK;
            }
        }
        #endregion
        #endregion
    }
}
