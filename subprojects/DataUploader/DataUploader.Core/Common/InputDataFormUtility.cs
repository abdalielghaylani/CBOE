using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.UserControls.Forms
{
    public enum InputDataFormShowingControl
    {
        ComboBox,
        TextBox
    }

    public delegate void InputDataFormSaveEventHandler(object sender, InputDataFormSaveEventArgs e);

    public class InputDataFormSaveEventArgs : EventArgs
    {
        private DataGridView _dataGridView;

        public DataGridView DataGridView
        {
            get
            {
                return this._dataGridView;
            }
        }

        private int _rowIndex;

        public int RowIndex
        {
            get
            {
                return this._rowIndex;
            }
        }

        private string _value;

        public string Value
        {
            get
            {
                return this._value;
            }
        }

        public InputDataFormSaveEventArgs(DataGridView dataGridView, int rowIndex, string value)
        {
            this._dataGridView = dataGridView;
            this._rowIndex = rowIndex;
            this._value = value;
        }
    }
}
