using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.UserControls.Forms
{
    public partial class InputDataForm : Form
    {

        #region fields

        private int _currentDataGirdViewRowIndex = -1;

        private DataGridView _currentDataGridView;

        private InputDataFormShowingControl _showingControl = InputDataFormShowingControl.ComboBox;

        private string[] _inputValue = new string[] { "none for right now" };

        private string _outputValue = string.Empty;

        #endregion

        #region properties

        public int CurrentDataGridViewRowIndex
        {
            get
            {
                return this._currentDataGirdViewRowIndex;
            }
            set
            {
                this._currentDataGirdViewRowIndex = value;
            }
        }

        public DataGridView CurrentDataGridView
        {
            get
            {
                return this._currentDataGridView;
            }
            set
            {
                this._currentDataGridView = value;
            }
        }

        public InputDataFormShowingControl ShowingControl
        {
            get
            {
                return this._showingControl;
            }
            set
            {
                this._showingControl = value;
                this.DisplaySetting();
            }
        }

        public string[] InputValue
        {
            get
            {
                return this._inputValue;
            }
            set
            {
                this._inputValue = value;
                this.DisplaySetting();
            }
        }

        public string OutputValue
        {
            get
            {
                return this._outputValue;
            }
        }

        #endregion

        #region events

        public event InputDataFormSaveEventHandler Save;

        #endregion

        #region constructors

        public InputDataForm()
        {
            this.InitializeComponent();
            this.InitControlsSetting();
        }

        #endregion

        #region methods

        private void InitControlsSetting()
        {
            //events
            this._saveButton.Click += new EventHandler(SaveButton_Click);
            this._cancelButton.Click += new EventHandler(CancelButton_Click);

            //operation
            this.SuspendLayout();
            this.DisplaySetting();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void DisplaySetting()
        {
            if (this.ShowingControl == InputDataFormShowingControl.ComboBox)
            {
                this._valueComboBox.Items.Clear();
                this._valueComboBox.Items.AddRange(this.InputValue);
                this._inputFieldValueLabel.BringToFront();
                this._valueComboBox.BringToFront();
            }
            else if (this.ShowingControl == InputDataFormShowingControl.TextBox)
            {
                this._valueTextBox.Text = string.Empty;
                this._constantValueLabel.BringToFront();
                this._valueTextBox.BringToFront();
            }
        }

        private void OnSave(InputDataFormSaveEventArgs args)
        {
            if (this.Save != null)
            {
                this.Save(this, args);
            }
        }

        private void SetOutputValue()
        {
            if (this.ShowingControl == InputDataFormShowingControl.ComboBox)
            {
                this._outputValue = this._valueComboBox.Text;
            }
            else
            {
                this._outputValue = this._valueTextBox.Text;
            }
        }

        #endregion

        #region event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            this.SetOutputValue();
            InputDataFormSaveEventArgs args =
                new InputDataFormSaveEventArgs(this.CurrentDataGridView, this.CurrentDataGridViewRowIndex, this.OutputValue);
            this.OnSave(args);
            this.Hide();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        #endregion

    }
}
