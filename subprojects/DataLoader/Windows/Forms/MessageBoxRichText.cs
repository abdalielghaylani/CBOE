using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.DataLoader.Windows.Controls;

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
    /// <summary>
    /// Similar to a MessageBox but a RichText control
    /// </summary>
    public partial class MessageBoxRichText : Form
    {
        #region data
        // data members
        private readonly System.Windows.Forms.RichTextBox _RichTextBox;
        private readonly System.Windows.Forms.Button _CopyButton;
        private readonly System.Windows.Forms.Button _AcceptButton;
        #endregion

        #region properties
        /// <summary>
        /// Set MessageText
        /// </summary>
        public string MessageText
        {
            get
            {
                return _RichTextBox.Text;
            }
            set
            {
                _RichTextBox.Text = value;
                return;
            }
        } // Text
        #endregion

        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public MessageBoxRichText(string vstrCaption)
        {
            InitializeComponent();
            BackColor = UIBase.LightGray;
            FormBorderStyle = FormBorderStyle.Sizable;
            Text = vstrCaption;
            SuspendLayout();
            _RichTextBox = UIBase.GetRichTextBox();
            //_RichTextBox.Enabled = false; // Unfortunately this will disable the scroll bars as well
            _RichTextBox.Multiline = true;
            _RichTextBox.ReadOnly = true;
            _RichTextBox.WordWrap = false;
            Controls.Add(_RichTextBox);
            _RichTextBox.GotFocus +=new EventHandler(RichTextBox_GotFocus);

            _CopyButton = UIBase.GetButton(UIBase.ButtonType.Copy);
            Controls.Add(_CopyButton);
            _CopyButton.Click += new EventHandler(CopyButton_Click);

            AcceptButton = _AcceptButton = UIBase.GetButton(UIBase.ButtonType.Accept);
            Controls.Add(_AcceptButton);
            _AcceptButton.Click += new EventHandler(AcceptButton_Click);

            Layout += new LayoutEventHandler(MessageBoxRichText_Layout);
            ResumeLayout(false);
            PerformLayout();
            return;
        } // MessageBoxRichText()
        #endregion

        #region events

        void AcceptButton_Click(object sender, EventArgs e)
        {
            Close();
            return;
        }  // AcceptButton_Click()

        void CopyButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(MessageText.Replace("\n", "\r\n"));
            return;
        }  // CopyButton_Click()

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageBoxRichText_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DL;
        }

        private void MessageBoxRichText_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && ((e.AffectedProperty == "Bounds") || (e.AffectedProperty == "Visible")))
            {
                // Vertical
                int bottom = ClientSize.Height;
                int top = 0;
                _RichTextBox.Top = top;
                bottom -= _AcceptButton.Height;
                _CopyButton.Top = bottom;
                _AcceptButton.Top = bottom;
                bottom -= UIBase.ExtraPadding.Top;
                _RichTextBox.Height = bottom - top;
                // Horizontal
                int right = ClientSize.Width;
                int left = 0;
                _RichTextBox.Left = left;
                _RichTextBox.Width = right;
                right -= _AcceptButton.Height;
                right -= _AcceptButton.Width;
                _CopyButton.Left = left;
                _AcceptButton.Left = right;
            }
            return;
        } // MessageBoxRichText_Layout()

        void RichTextBox_GotFocus(object sender, EventArgs e)
        {
            _AcceptButton.Focus();
            return;
        }         

        #endregion

    } // class MessageBoxRichText
}