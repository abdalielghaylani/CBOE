using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FormWizard
{
    public class ToolStripTextBoxWithLabel : ToolStripControlHost
    {
        #region Variable

        /// <summary>
        /// Event handler required to notify the TextBox value get changed
        /// </summary>
        public event EventHandler MaxRowValueChanged;

        readonly ControlPanel theControlPanel;

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set Max Rows
        /// </summary>
        public int Max_Rows
        {
            get
            {
                return theControlPanel.Max_Rows;
            }
            set
            {
                theControlPanel.Max_Rows = value;
            }
        }

        #endregion

        #region Constructor

        public ToolStripTextBoxWithLabel(string labelText)
            : base(new ControlPanel(labelText))
        {
            theControlPanel = (ControlPanel)base.Control;
            theControlPanel.TextBoxValueChanged += new EventHandler(ToolStripTextBoxWithLabel_TextBoxValueChanged);
        }

        #endregion

        #region Events

        void ToolStripTextBoxWithLabel_TextBoxValueChanged(object sender, EventArgs e)
        {
            if (MaxRowValueChanged != null)
            {
                MaxRowValueChanged(sender, e);
            }
        }

        #endregion
    }

    public class ControlPanel : Panel
    {
        #region Variables

        /// <summary>
        /// Varaible holds an object of TextBox class
        /// </summary>
        TextBox theTextBox = new TextBox();

        /// <summary>
        /// Variable holds an object of Label class
        /// </summary>
        Label theLabel = new Label();

        /// <summary>
        /// Event handler required to notify that TextBox value get changed
        /// </summary>
        public event EventHandler TextBoxValueChanged;

        #endregion

        #region Properties

        public int Max_Rows
        {
            get
            {
                int iValue = 0;
                if (!string.IsNullOrEmpty(theTextBox.Text))
                {
                    iValue = Convert.ToInt32(theTextBox.Text.ToString());
                }
                return iValue;
            }
            set
            {
                theTextBox.Text = value.ToString();
            }
        }

        #endregion

        #region Cosntructor

        public ControlPanel(string labelText)
        {
            this.Height = 20;
            this.Width = 103;
            this.Dock = DockStyle.Fill;

            theLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            theLabel.Text = labelText;
            theLabel.TextAlign = ContentAlignment.BottomLeft;
            theLabel.AutoSize = true;
            theLabel.Height = this.Height;
            theLabel.Location = new Point(0, 3);
            theLabel.Parent = this;

            theTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            theTextBox.Location = new Point(theLabel.Right + 3, 0);
            theTextBox.Width = 40;
            theLabel.Height = this.Height;
            theTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            theTextBox.Margin = new Padding(3, 3, 3, 3);
            theTextBox.Parent = this;
            theTextBox.MaxLength = 5;
            theTextBox.KeyPress += new KeyPressEventHandler(theTextBox_KeyPress);
            theTextBox.TextChanged += new EventHandler(theTextBox_TextChanged);
        }

        #endregion

        #region Events

        void theTextBox_TextChanged(object sender, EventArgs e)
        {
            if (TextBoxValueChanged != null)
            {
                TextBoxValueChanged(sender, e);
            }
        }

        void theTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        #endregion
    }
}
