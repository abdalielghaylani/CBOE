using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.DataLoader.UserControls.Forms;

namespace CambridgeSoft.COE.DataLoader.UserControls
{
    public partial class DataMapper : UserControl
    {

        #region fields

        private InputDataForm _inputDataForm;

        private Dictionary<int, string[]> _registryComboBoxDictionary;

        private Dictionary<int, string[]> _componentComboBoxDictionary;

        private Dictionary<int, string[]> _batchComboBoxDictionary;

        private Dictionary<int, string[]> _batchComponentComboBoxDictionary;

        private string[] _defaultComboBoxItems = new string[] { "None" };

        #endregion

        #region properties

        public InputDataForm InputDataForm
        {
            get
            {
                if (this._inputDataForm == null)
                {
                    this._inputDataForm = new InputDataForm();
                }
                return this._inputDataForm;
            }
        }

        public Dictionary<int, string[]> RegistryComboBoxDictionary
        {
            get
            {
                if (this._registryComboBoxDictionary == null)
                {
                    this._registryComboBoxDictionary = new Dictionary<int, string[]>(3);
                    for (int i = 0; i < 3; i++)
                    {
                        this._registryComboBoxDictionary.Add(i, this._defaultComboBoxItems);
                    }
                }
                return this._registryComboBoxDictionary;
            }
            set
            {
                this._registryComboBoxDictionary = value;
            }
        }

        public Dictionary<int, string[]> ComponentComboBoxDictionary
        {
            get
            {
                if (this._componentComboBoxDictionary == null)
                {
                    this._componentComboBoxDictionary = new Dictionary<int, string[]>(3);
                    for (int i = 0; i < 3; i++)
                    {
                        this._componentComboBoxDictionary.Add(i, this._defaultComboBoxItems);
                    }
                }
                return this._componentComboBoxDictionary;
            }
            set
            {
                this._componentComboBoxDictionary = value;
            }
        }

        public Dictionary<int, string[]> BatchComboBoxDictionary
        {
            get
            {
                if (this._batchComboBoxDictionary == null)
                {
                    this._batchComboBoxDictionary = new Dictionary<int, string[]>(12);
                    for (int i = 0; i < 12; i++)
                    {
                        this._batchComboBoxDictionary.Add(i, this._defaultComboBoxItems);
                    }
                }
                return this._batchComboBoxDictionary;
            }
            set
            {
                this._batchComboBoxDictionary = value;
            }
        }

        public Dictionary<int, string[]> BatchComponentComboBoxDictionary
        {
            get
            {
                return this._batchComponentComboBoxDictionary;
            }
            set
            {
                this._batchComponentComboBoxDictionary = value;
            }
        }

        #endregion

        #region constructors

        public DataMapper()
        {
            InitializeComponent();
            this.InitControlsSetting();
        }

        #endregion

        #region methods

        private void InitControlsSetting()
        {
            //event
            this._configurationTabControl.Selected
                += new TabControlEventHandler(ConfigurationTabControl_Selected);
            this.InputDataForm.Save += new InputDataFormSaveEventHandler(RegistryInputDataForm_Save);
            this.InputDataForm.Save += new InputDataFormSaveEventHandler(ComponentInputDataForm_Save);
            this.InputDataForm.Save += new InputDataFormSaveEventHandler(BatchInputDataForm_Save);
            this.InputDataForm.Save += new InputDataFormSaveEventHandler(BatchComponentInputDataForm_Save);

            //operations
            this.InitDefaultDisplayDataGridView();
        }

        private void LoadDataGridView(SelectedDataview sd)
        {
            switch (sd)
            {
                case SelectedDataview.Registry:
                    if (this._registryDataGridView.Rows.Count != 3)
                    {
                        this.InitRegistryDataGridView();
                    }
                    break;
                case SelectedDataview.Component:
                    if (this._componentDataGridView.Rows.Count != 3)
                    {
                        this.InitComponentDataGridView();
                    }
                    break;
                case SelectedDataview.Batch:
                    if (this._batchDataGridView.Rows.Count != 12)
                    {
                        this.InitBatchDataGridView();
                    }
                    break;
                case SelectedDataview.BatchComponent:
                    this.InitBatchComponentDataGridView();
                    break;
            }
        }

        private void InitDefaultDisplayDataGridView()
        {
            this.InitRegistryDataGridView();
        }

        private void InitRegistryDataGridView()
        {
            this._registryDataGridView.Rows.Clear();
            this._registryDataGridView.Rows.Add(3);
            this._registryDataGridView.AllowUserToAddRows = false;

            this._registryDataGridView.Rows[0].Cells[0].Value = "Projects";
            this._registryDataGridView.Rows[1].Cells[0].Value = "Prefix";
            this._registryDataGridView.Rows[2].Cells[0].Value = "Registry Comments";
            this._registryDataGridView.Rows[0].Cells[2].Value = "String";
            this._registryDataGridView.Rows[1].Cells[2].Value = "String";
            this._registryDataGridView.Rows[2].Cells[2].Value = "String";

            this._registryDFColumn.ReadOnly = true;
            this._registryTColumn.ReadOnly = true;
            this._registryValueColumn.ReadOnly = true;

            this._registryDataGridView.EditingControlShowing +=
                new DataGridViewEditingControlShowingEventHandler(DataGridView_EditingControlShowing);
        }

        private void InitComponentDataGridView()
        {
            this._componentDataGridView.Rows.Clear();
            this._componentDataGridView.Rows.Add(3);
            this._componentDataGridView.AllowUserToAddRows = false;

            this._componentDataGridView.Rows[0].Cells[0].Value = "Structure";
            this._componentDataGridView.Rows[1].Cells[0].Value = "Component Comments";
            this._componentDataGridView.Rows[2].Cells[0].Value = "Stereochemistry Comments";
            this._componentDataGridView.Rows[0].Cells[2].Value = "Binary";
            this._componentDataGridView.Rows[1].Cells[2].Value = "String";
            this._componentDataGridView.Rows[2].Cells[2].Value = "String";

            this._componentDFColumn.ReadOnly = true;
            this._componentTColumn.ReadOnly = true;
            this._componentValueColumn.ReadOnly = true;

            this._componentDataGridView.EditingControlShowing +=
                new DataGridViewEditingControlShowingEventHandler(DataGridView_EditingControlShowing);
        }

        private void InitBatchDataGridView()
        {
            this._batchDataGridView.Rows.Clear();
            this._batchDataGridView.Rows.Add(12);
            this._batchDataGridView.AllowUserToAddRows = false;

            this._batchDataGridView.Rows[0].Cells[0].Value = "Scientist";
            this._batchDataGridView.Rows[1].Cells[0].Value = "Synthesis Date";
            this._batchDataGridView.Rows[2].Cells[0].Value = "Notebook Reference";
            this._batchDataGridView.Rows[3].Cells[0].Value = "Amount";
            this._batchDataGridView.Rows[4].Cells[0].Value = "Units";
            this._batchDataGridView.Rows[5].Cells[0].Value = "Appearance";
            this._batchDataGridView.Rows[6].Cells[0].Value = "Purity";
            this._batchDataGridView.Rows[7].Cells[0].Value = "Purity Comments";
            this._batchDataGridView.Rows[8].Cells[0].Value = "Sample ID";
            this._batchDataGridView.Rows[9].Cells[0].Value = "Solubility";
            this._batchDataGridView.Rows[10].Cells[0].Value = "Batch Comments";
            this._batchDataGridView.Rows[11].Cells[0].Value = "Storage Requirements Warnings";

            this._batchDataGridView.Rows[0].Cells[2].Value = "String";
            this._batchDataGridView.Rows[1].Cells[2].Value = "Date";
            this._batchDataGridView.Rows[2].Cells[2].Value = "String";
            this._batchDataGridView.Rows[3].Cells[2].Value = "Decimal";
            this._batchDataGridView.Rows[4].Cells[2].Value = "String";
            this._batchDataGridView.Rows[5].Cells[2].Value = "String";
            this._batchDataGridView.Rows[6].Cells[2].Value = "Decimal";
            this._batchDataGridView.Rows[7].Cells[2].Value = "String";
            this._batchDataGridView.Rows[8].Cells[2].Value = "String";
            this._batchDataGridView.Rows[9].Cells[2].Value = "String";
            this._batchDataGridView.Rows[10].Cells[2].Value = "String";
            this._batchDataGridView.Rows[11].Cells[2].Value = "String";

            this._batchDFColumn.ReadOnly = true;
            this._batchTColumn.ReadOnly = true;
            this._batchValueColumn.ReadOnly = true;

            this._batchDataGridView.EditingControlShowing +=
                new DataGridViewEditingControlShowingEventHandler(DataGridView_EditingControlShowing);
        }

        private void InitBatchComponentDataGridView()
        { }

        private string[] GetInputDataFormInputValue(int selectedIndex, DataGridView dataGridView)
        {
            string[] result = null;

            if (dataGridView.Name == this._registryDataGridView.Name)
            {
                result = this._registryComboBoxDictionary[selectedIndex];
            }
            else if (dataGridView.Name == this._componentDataGridView.Name)
            {
                result = this._componentComboBoxDictionary[selectedIndex];
            }
            else if (dataGridView.Name == this._batchDataGridView.Name)
            {
                result = this._batchComboBoxDictionary[selectedIndex];
            }
            else if (dataGridView.Name == this._batchComponentDataGridView.Name)
            {
                result = this._batchComponentComboBoxDictionary[selectedIndex];
            }

            return result;
        }

        private void SetDataGridViewValueColumn(InputDataFormSaveEventArgs e)
        {
            DataGridView currentDataGridView = e.DataGridView;
            int rowIndex = e.RowIndex;
            string value = e.Value;
            currentDataGridView.Rows[rowIndex].Cells[3].Value = value;
        }

        #endregion

        #region event handlers

        private void ConfigurationTabControl_Selected(object sender, TabControlEventArgs e)
        {
            TabPage tp = e.TabPage;
            if (tp.Name == "_registryTabPage")
                this.LoadDataGridView(SelectedDataview.Registry);
            else if (tp.Name == "_componentTabPage")
                this.LoadDataGridView(SelectedDataview.Component);
            else if (tp.Name == "_batchTabPage")
                this.LoadDataGridView(SelectedDataview.Batch);
            else if (tp.Name == "_batchComponentTabPage")
                this.LoadDataGridView(SelectedDataview.BatchComponent);
        }

        private void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewComboBoxEditingControl cb = e.Control as DataGridViewComboBoxEditingControl;

            if (cb != null)
            {
                cb.SelectedIndexChanged -= new EventHandler(DataGridViewComboBoxEditingControl_SelectedIndexChanged);

                cb.SelectedIndexChanged += new EventHandler(DataGridViewComboBoxEditingControl_SelectedIndexChanged);
            }
        }

        private void DataGridViewComboBoxEditingControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridViewComboBoxEditingControl cb = sender as DataGridViewComboBoxEditingControl;

            if (cb != null)
            {
                //setting InputDataForm propeties
                this.InputDataForm.CurrentDataGridViewRowIndex = cb.EditingControlRowIndex;
                this.InputDataForm.CurrentDataGridView = cb.EditingControlDataGridView;
                if (cb.SelectedIndex == 0)
                {
                    this.InputDataForm.ShowingControl = InputDataFormShowingControl.ComboBox;
                }
                else
                {
                    this.InputDataForm.ShowingControl = InputDataFormShowingControl.TextBox;
                }
                this.InputDataForm.InputValue =
                    this.GetInputDataFormInputValue(cb.EditingControlRowIndex, cb.EditingControlDataGridView);

                this.InputDataForm.ShowDialog();
            }
        }

        private void RegistryInputDataForm_Save(object sender, InputDataFormSaveEventArgs e)
        {
            this.SetDataGridViewValueColumn(e);
        }

        private void ComponentInputDataForm_Save(object sender, InputDataFormSaveEventArgs e)
        {
            this.SetDataGridViewValueColumn(e);
        }

        private void BatchInputDataForm_Save(object sender, InputDataFormSaveEventArgs e)
        {
            this.SetDataGridViewValueColumn(e);
        }

        private void BatchComponentInputDataForm_Save(object sender, InputDataFormSaveEventArgs e)
        {
            this.SetDataGridViewValueColumn(e);
        }

        #endregion

        #region enum

        public enum SelectedDataview
        {
            Registry,
            Component,
            Batch,
            BatchComponent
        }

        #endregion

    }
}
