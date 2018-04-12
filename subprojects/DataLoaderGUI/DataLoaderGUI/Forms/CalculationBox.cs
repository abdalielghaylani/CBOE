using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Calculation.Parser;
using CambridgeSoft.DataLoaderGUI.Controls;

namespace CambridgeSoft.DataLoaderGUI.Forms
{
    /// <summary>
    /// Form to enter and edit calculations
    /// </summary>
    public partial class CalculationBox : Form
    {
        #region data
        // data members
        private bool _CenteredToOwner = false;
        private Point _Origin = new Point(-1, -1);
        private readonly System.Windows.Forms.RichTextBox _Formula;
        private readonly System.Windows.Forms.RichTextBox _Status;
        private readonly System.Windows.Forms.Button _FunctionsButton;
        private readonly System.Windows.Forms.Button _FieldsButton;
        private readonly System.Windows.Forms.Button _AcceptButton;
        private readonly System.Windows.Forms.Button _CancelButton;
        private readonly CalculationParser _Parser = null;
        private object _ReturnType;
        private string _strParserError;
        private int _nParserErrorColumn;
        private int _cParserErrorColumns;
        #endregion

        #region properties
        /// <summary>
        /// Get / set formula Text
        /// </summary>
        public string FormulaText
        {
            get
            {
                return _Formula.Text;
            }
            set
            {
                _Formula.Text = value;
                return;
            }
        } // FormulaText

        /// <summary>
        /// Get / set formula Rtf
        /// </summary>
        public string FormulaRtf
        {
            get
            {
                return _Formula.Rtf;
            }
            set
            {
                _Formula.Rtf = value;
                return;
            }
        } // Formula

        private CalculationParser Parser
        {
            get
            {
                return _Parser;
            }
        } // Parser

        /// <summary>
        /// Get / set forumla return type
        /// </summary>
        public object ReturnType
        {
            get
            {
                return _ReturnType;
            }
            set
            {
                _ReturnType = value;
                return;
            }
        } // ReturnType

        #endregion

        #region constructors

        /// <summary>
        /// Calculation constructor
        /// </summary>
        public CalculationBox(ref CalculationParser rParser)
        {
            _Parser = rParser;
            InitializeComponent();
            BackColor = UIBase.LightGray;
            FormBorderStyle = FormBorderStyle.Sizable;
            SuspendLayout();

            _Formula = UIBase.GetRichTextBox();
            _Formula.Font = new Font(FontFamily.GenericMonospace, _Formula.Font.SizeInPoints);
            Controls.Add(_Formula);

            _Status = UIBase.GetRichTextBox();
            _Status.Enabled = false;
            Controls.Add(_Status);

            _FunctionsButton = UIBase.GetButton(UIBase.ButtonType.BlueF);
            Controls.Add(_FunctionsButton);

            _FieldsButton = UIBase.GetButton(UIBase.ButtonType.BlueBook);
            Controls.Add(_FieldsButton);

            AcceptButton = _AcceptButton = UIBase.GetButton(UIBase.ButtonType.Accept);
            Controls.Add(_AcceptButton);
            CancelButton = _CancelButton = UIBase.GetButton(UIBase.ButtonType.Cancel);
            Controls.Add(_CancelButton);

            _Formula.TextChanged += new EventHandler(Formula_TextChanged);
            _FunctionsButton.Click += new EventHandler(FunctionsButton_Click);
            _FieldsButton.Click += new EventHandler(FieldsButton_Click);
            _AcceptButton.Click += new EventHandler(AcceptButton_Click);
            _CancelButton.Click += new EventHandler(CancelButton_Click);
            FormClosed += new FormClosedEventHandler(Calculation_FormClosed);
            Layout += new LayoutEventHandler(Calculation_Layout);
            Shown += new EventHandler(Calculation_Shown);

            ResumeLayout(false);
            PerformLayout();
            return;
        } // Calculation()

        #endregion
        
        #region event handlers

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculationBox_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DL;
        }

        void Calculation_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Origin.X = Left;
            _Origin.Y = Top;
            Parser.CalculationRemove("CalculationBox");
            return;
        } // Calculation_FormClosed

        void Calculation_Shown(object sender, EventArgs e)
        {
            _Formula.Tag = FormulaRtf;    // Save incoming formula
            if (_CenteredToOwner)
            {
                Left = _Origin.X;
                Top = _Origin.Y;
            }
            else
            {
                _CenteredToOwner = true;
                Width = Owner.ClientSize.Width;
                Height = _CancelButton.Bounds.Bottom;
                Point ptScreen = new Point(Owner.Left + (Owner.Width - Width) / 2, Owner.Top + (Owner.Height - Height) / 2);
                Left = ptScreen.X;
                Top = ptScreen.Y;
                _Origin.X = Left;
                _Origin.Y = Top;
            }
            if (_cParserErrorColumns > 0)
            {
                _Formula.Select(_nParserErrorColumn, _cParserErrorColumns);
            }
            else
            {
                _Formula.Select(_Formula.Text.Length, 0);
            }
            _Formula.Focus();
            return;
        } // Calculation_Shown()

        private void Calculation_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && ((e.AffectedProperty == "Bounds") || (e.AffectedProperty == "Visible")))
            {
                // Vertical
                int y = 0;
                _Formula.Top = y;
                y += _Formula.Height;
                y += UIBase.ExtraPadding.Bottom;
                _Status.Top = y;
                y += _Status.Height;
                y += UIBase.ExtraPadding.Bottom;

                _FunctionsButton.Top = y;
                _FieldsButton.Top = y;
                _AcceptButton.Top = y;
                _CancelButton.Top = y;
                y += _AcceptButton.Height;

                // Horizontal
                int x = 0;
                int maxX = -1;
                if (maxX < x) maxX = x;
                x = 0;
                _Formula.Left = x;
                _Formula.Width = ClientSize.Width;
                x += _Formula.Width;
                if (maxX < x) maxX = x;
                x = 0;
                _Status.Left = x;
                _Status.Width = ClientSize.Width;
                if (maxX < x) maxX = x;
                x = 0;
                _FunctionsButton.Left = x;
                x += _FunctionsButton.Width;
                _FieldsButton.Left = x;
                x += _FieldsButton.Width;
                x += UIBase.ExtraPadding.Right;
                _AcceptButton.Left = x;
                x += _AcceptButton.Width;
                _CancelButton.Left = x;
                x += _CancelButton.Width;
                x += _CancelButton.Height; // For gripper
                if (maxX < x) maxX = x;

                _Formula.Width = maxX;
                _Status.Width = maxX;
                x = maxX;
                x -= _CancelButton.Height; // For gripper
                x -= _CancelButton.Width;
                _CancelButton.Left = x;
                x -= _AcceptButton.Width;
                _AcceptButton.Left = x;

                if ((ClientSize.Width < maxX) || (ClientSize.Height < y)) ClientSize = new Size(maxX, y);

            }
            return;
        } // Calculation_Layout()

        void AcceptButton_Click(object sender, EventArgs e)
        {
            Close();
            return;
        }  // AcceptButton_Click()

        void CancelButton_Click(object sender, EventArgs e)
        {
            FormulaRtf = (string)_Formula.Tag;    // Restore incoming formula
            return;
        }  // CancelButton_Click()

        void FieldsButton_Click(object sender, EventArgs e)
        {
            string xmlFields = Parser.Fields;
            FieldChooser oFieldChooser = new FieldChooser();
            oFieldChooser.Fields = xmlFields;
            DialogResult dr = oFieldChooser.ShowDialog(this);
            string strField = oFieldChooser.Field;
            if (strField != string.Empty)
            {
                _Formula.SelectedText = strField;
                if (_cParserErrorColumns > 0)
                {
                    _Formula.Select(_nParserErrorColumn, _cParserErrorColumns);
                }
            }
            _Formula.Focus();
            return;
        }  // FieldsButton_Click

        void FunctionsButton_Click(object sender, EventArgs e)
        {
            string xmlFunctions = Parser.Functions;
            FunctionChooser oFunctionChooser = new FunctionChooser();
            oFunctionChooser.Functions = xmlFunctions;
            DialogResult dr = oFunctionChooser.ShowDialog(this);
            string strFunction = oFunctionChooser.Function;
            if (strFunction != string.Empty)
            {
                _Formula.SelectedText = strFunction;
                if (_cParserErrorColumns > 0)
                {
                    _Formula.Select(_nParserErrorColumn, _cParserErrorColumns);
                }
            }
            _Formula.Focus();
            return;
        }  // FunctionsButton_Click

        void Formula_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            string strSource = rtb.Text;
            bool bFailed = false;
            if (strSource != string.Empty)
            {
                bFailed = Parser.CalculationAdd("CalculationBox", ReturnType, strSource);
                if (bFailed)
                {
                    _strParserError = Parser.Error;
                    _nParserErrorColumn = Parser.ErrorColumn;
                    _cParserErrorColumns = Parser.ErrorColumns;
                }
                else
                {
                    _strParserError = "The expression is valid";
                    _nParserErrorColumn = -1;
                    _cParserErrorColumns = -1;
                }
            }
            else
            {
                Parser.CalculationRemove("CalculationBox");
                _strParserError = "The expression is empty";
                _nParserErrorColumn = -1;
                _cParserErrorColumns = -1;
            } // if (strSource != string.Empty)
            _Status.Text = _strParserError;
            return;
        } // Formula_TextChanged()

        #endregion

    } // class Calculation
}