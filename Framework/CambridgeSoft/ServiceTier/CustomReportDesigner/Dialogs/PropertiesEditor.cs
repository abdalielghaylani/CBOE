using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    public partial class PropertiesEditor : UserControl
    {
        #region variables
        private ResultsCriteria.IResultsCriteriaBase _criterium;
        #endregion

        #region Properties
        public ResultsCriteria.IResultsCriteriaBase Criterium
        {
            get
            {
                UnBindCriterium();
                return _criterium;
            }
            set
            {
                _criterium = value;

                if (_criterium != null)
                    this.DisplayCriterium(_criterium);
            }
        }
        #endregion

        #region Methods
        private void HandleException(Exception exception)
        {
            this.StatusLabel.Text = exception.Message;
        }

        private void UnBindCriterium()
        {
            if (_criterium != null)
            {
                COEDataBinder databinder = new COEDataBinder(_criterium);

                foreach (DataGridViewRow currentRow in this.PropertiesDataGridView.Rows)
                {
                    if (currentRow.Cells[0].Value.ToString() != "Type")
                        databinder.SetProperty(currentRow.Cells[0].Value.ToString(), currentRow.Cells[1].Value);
                }
            }
        }

        private void DisplayCriterium(ResultsCriteria.IResultsCriteriaBase _criterium)
        {
            PropertiesDataGridView.Rows.Clear();


            List<Type> resultsCriteriaTypes = new List<Type>();
            foreach (Type currenType in typeof(ResultsCriteria.IResultsCriteriaBase).Assembly.GetExportedTypes())
            {
                if (currenType.IsSubclassOf(typeof(ResultsCriteria.IResultsCriteriaBase)))
                {
                    resultsCriteriaTypes.Add(currenType);
                }
            }

            SetProperty("Type", _criterium.GetType().FullName, resultsCriteriaTypes, "Name", "FullName");

            PropertyInfo[] propertiesInfo = _criterium.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertiesInfo)
            {
                SetProperty(propertyInfo.Name, propertyInfo.GetValue(_criterium, null), !propertyInfo.CanWrite);
            }
        }

        private void SetProperty(string nombre, object value, IList dataSource, string displayMember, string valueMember)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Cells.Add(CreateTextBoxCell(nombre, true));
            row.Cells.Add(CreateComboBoxCell(value, dataSource, displayMember, valueMember));
            this.PropertiesDataGridView.Rows.Add(row);

            row.Cells[0].ReadOnly = true;
            row.Cells[1].ReadOnly = false;
        }

        private DataGridViewRow GetPropertyRow(string nombre)
        {
            foreach (DataGridViewRow currentRow in PropertiesDataGridView.Rows)
            {
                if (currentRow.Cells[0].Value.Equals(nombre))
                    return currentRow;
            }

            return null;
        }

        private void SetProperty(DataGridViewRow propertyRow, object value)
        {
            if (propertyRow.Cells.Count > 1)
                propertyRow.Cells[1].Value = value;
            else
            {
                if (value != null && value.GetType().IsSubclassOf(typeof(Enum)))
                {
                    List<string> namesList = new List<string>(Enum.GetNames(value.GetType()));
                    propertyRow.Cells.Add(CreateComboBoxCell(value, namesList, null, null));
                }
                else if (value != null && value is bool)
                {
                    List<bool> booleanValues = new List<bool>(new bool[] { true, false });
                    propertyRow.Cells.Add(CreateComboBoxCell(value, booleanValues, null, null));
                }
                else
                {
                    propertyRow.Cells.Add(CreateTextBoxCell(value, false));
                }
            }
        }

        private void SetProperty(string nombre, object value, bool isReadOnly)
        {
            //If Property exists, replace
            if (GetPropertyRow(nombre) != null)
                SetProperty(GetPropertyRow(nombre), value);
            else
            {
                //Else create a new row
                DataGridViewRow propertyRow = new DataGridViewRow();
                propertyRow.Cells.Add(CreateTextBoxCell(nombre, true));

                SetProperty(propertyRow, value);

                this.PropertiesDataGridView.Rows.Add(propertyRow);
                propertyRow.Cells[0].ReadOnly = true;
                propertyRow.Cells[1].ReadOnly = isReadOnly;
            }
        }

        private DataGridViewComboBoxCell CreateComboBoxCell(object value, IList dataSource, string displayMember, string valueMember)
        {
            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
            comboBoxCell.DataSource = dataSource;

            if (!string.IsNullOrEmpty(valueMember))
                comboBoxCell.ValueMember = valueMember;

            if (!string.IsNullOrEmpty(displayMember))
                comboBoxCell.DisplayMember = displayMember;

            comboBoxCell.ValueType = value.GetType();

            comboBoxCell.Value = value;

            return comboBoxCell;
        }

        private DataGridViewTextBoxCell CreateTextBoxCell(object value, bool isReadOnly)
        {
            DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
            textBoxCell.Value = value;
            //textBoxCell.ValueType = value.GetType();
            //textBoxCell.ReadOnly = isReadOnly;

            return textBoxCell;
        }

        public PropertiesEditor()
        {
            InitializeComponent();
        }

        private void PropertiesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            this.PropertiesDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = e.Exception.Message;
        }

        private void PropertiesDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (0 <= e.RowIndex && e.RowIndex < this.PropertiesDataGridView.Rows.Count)
                this.CellValueChanged(e.RowIndex);
        }

        private void CellValueChanged(int rowIndex)
        {
            DataGridViewRow currentRow = PropertiesDataGridView.Rows[rowIndex];

            if (currentRow != null && currentRow.Cells[0].Value.ToString() == "Type")
            {
                if (!_criterium.GetType().FullName.Equals(currentRow.Cells[1].Value.ToString()))
                {
                    ResultsCriteria.IResultsCriteriaBase oldCriterium = this._criterium;

                    Criterium = this.CreateCriteriumInstance(currentRow.Cells[1].Value.ToString());

                    PropertyInfo[] propertiesInfo = oldCriterium.GetType().GetProperties();

                    foreach (PropertyInfo propertyInfo in propertiesInfo)
                    {
                        if (GetPropertyRow(propertyInfo.Name) != null)
                            SetProperty(propertyInfo.Name, propertyInfo.GetValue(oldCriterium, null), !propertyInfo.CanWrite);
                    }
                }
            }
        }

        private ResultsCriteria.IResultsCriteriaBase CreateCriteriumInstance(string fullName)
        {
            Type CriteriumType = typeof(ResultsCriteria.IResultsCriteriaBase).Assembly.GetType(fullName);
            return (ResultsCriteria.IResultsCriteriaBase)CriteriumType.GetConstructor(new Type[] { }).Invoke(new object[] { });
        }
        #endregion
    }
}
