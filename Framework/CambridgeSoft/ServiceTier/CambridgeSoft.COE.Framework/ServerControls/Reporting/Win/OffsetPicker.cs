using System;
using System.Windows.Forms;

namespace CambridgeSoft.COE.Framework.ServerControls.Reporting.Win
{
    public partial class OffsetPicker : Form
    {
        #region Variables
        private Button _selectedButton;
        private int _numberRows;
        private int _numberColumns;
        #endregion

        #region Properties
        public int NumberOfRows
        {
            get
            {
                return _numberRows;
            }
            set
            {
                _numberRows = value;
            }
        }

        public int NumberOfColumns
        {
            get
            {
                return _numberColumns;
            }
            set
            {
                _numberColumns = value;
            }
        }

        public int SelectedRow
        {
            get
            {
                if (_selectedButton != null)
                    return int.Parse(_selectedButton.Name.Substring(_selectedButton.Name.IndexOf("_") + 1, _selectedButton.Name.Length - (_selectedButton.Name.LastIndexOf("_") + 1)));

                return 0;
            }
            set
            {
                SelectOffset(value, SelectedColumn);
            }
        }

        public int SelectedColumn
        {
            get
            {
                if (_selectedButton != null)
                    return int.Parse(_selectedButton.Name.Substring(_selectedButton.Name.LastIndexOf("_") + 1));

                return 0;
            }
            set
            {
                SelectOffset(SelectedRow, value);
            }
        }
        #endregion

        #region Methods
        #region Constructors
        public OffsetPicker()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void OffsetPicker_Load(object sender, EventArgs e)
        {
            CreateChildControls();
        }

        void button_Click(object sender, EventArgs e)
        {
            this.SelectOffset((Button)sender);
            DialogResult = DialogResult.OK;
        }
        #endregion

        #region Private Methods
        private void CreateChildControls()
        {
            int buttonHeight = this.ClientRectangle.Height / NumberOfRows;
            int buttonWidth = this.ClientRectangle.Width / NumberOfColumns;

            for (int currentRow = 0; currentRow < NumberOfRows; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < NumberOfColumns; currentColumn++)
                {
                    Button button = new Button();
                    button.Name = string.Format("cell_{0}_{1}", currentRow, currentColumn);
                    this.Controls.Add(button);

                    button.Text = string.Format("({0}, {1})", currentRow, currentColumn);
                    button.ForeColor = System.Drawing.SystemColors.GrayText;

                    button.Height = buttonHeight - 2;
                    button.Width = buttonWidth - 2;
                    button.Top = currentRow * buttonHeight + 1;
                    button.Left = currentColumn * buttonWidth + 1;
                    button.FlatStyle = FlatStyle.Flat;
                    button.Click += new EventHandler(button_Click);
                }
            }
        }
        private void SelectOffset(Button button)
        {
            EnsureChildControls();

            if (_selectedButton != null)
            {
                _selectedButton.BackColor = System.Drawing.SystemColors.Control;
                _selectedButton.ForeColor = System.Drawing.SystemColors.GrayText;
            }

            _selectedButton = button;
            _selectedButton.BackColor = System.Drawing.SystemColors.ControlDark;
            _selectedButton.ForeColor = System.Drawing.SystemColors.ControlText;
        }

        private void SelectOffset(int row, int column)
        {
            EnsureChildControls();

            int index = column + row * NumberOfColumns;

            if (index < this.Controls.Count)
                SelectOffset((Button)this.Controls[index]);
        }

        private void EnsureChildControls()
        {
            if (this.Controls.Count == 0)
                this.CreateChildControls();
        }

        #endregion
        #endregion
    }
}
