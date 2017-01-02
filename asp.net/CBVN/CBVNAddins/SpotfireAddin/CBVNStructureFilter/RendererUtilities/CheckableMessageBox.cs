// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckableMessageBox.cs" company="PerkinElmer Inc.">
//   Copyright © 2012 PerkinElmer Inc. 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace CBVNStructureFilter
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Custom MessageBox with Checkbox
    /// </summary>
    public partial class CheckableMessageBox : Form
    {

        #region Enumerations

        /// <summary>
        /// MessageBox Button enum
        /// </summary>
        public enum MessageBoxButtons
        {
            /// <summary>
            /// display Ok button only
            /// </summary>
            OkOnly, 
            /// <summary>
            /// display Ok and Cancel buttons
            /// </summary>
            OkCancel, 
            /// <summary>
            /// display Yes and No buttons
            /// </summary>
            YesNo, 
            /// <summary>
            /// display Yes, No and Cancel buttons
            /// </summary>
            YesNoCancel
        }

        /// <summary>
        /// MessageBox Icons enum
        /// </summary>
        public enum MessageBoxIcons
        {
            /// <summary>
            /// "/i\" Warning icon
            /// </summary>
            Warning,
            /// <summary>
            /// "(X)" Error icon
            /// </summary>
            Error,
            /// <summary>
            /// "(!)" Information icon
            /// </summary>
            Information,
            /// <summary>
            /// "(?)" Question icon
            /// </summary>
            Question
        }

        #endregion


        #region Constructors and Destructors

        internal CheckableMessageBox(string message,
                            string caption,
                            string checkboxText,
                            MessageBoxButtons messageboxButtons,
                            MessageBoxIcons messageboxIcons)
        {
            InitializeComponent();

            this.Text = caption;
            this.lblMessage.Text = message;
            this.chkOption.Text = checkboxText;

            // change the status of buttons
            switch (messageboxButtons)
            {
                case MessageBoxButtons.OkOnly:
                    this.btnYes.Visible = false;
                    this.btnNo.Visible = false;
                    this.btnCancel.Visible = false;
                    this.AcceptButton = this.btnOK;
                    break;
                case MessageBoxButtons.OkCancel:
                    this.btnYes.Visible = false;
                    this.btnNo.Visible = false;
                    this.AcceptButton = this.btnOK;
                    this.CancelButton = this.btnCancel;
                    break;
                case MessageBoxButtons.YesNo:
                    this.btnOK.Visible = false;
                    this.btnCancel.Visible = false;
                    this.AcceptButton = this.btnYes;
                    this.CancelButton = this.btnNo;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    this.btnOK.Visible = false;
                    this.AcceptButton = this.btnYes;
                    this.CancelButton = this.btnCancel;
                    break;
            }


            // show the icon for massage box
            switch (messageboxIcons)
            {
                case MessageBoxIcons.Warning:
                    this.pbxIcon.Image = Properties.Resources.Warning1;
                    break;
                case MessageBoxIcons.Error:
                    this.pbxIcon.Image = Properties.Resources.ErrorImage;
                    break;
                case MessageBoxIcons.Question:
                    this.pbxIcon.Image = Properties.Resources.Question;
                    break;
                case MessageBoxIcons.Information:
                    this.pbxIcon.Image = Properties.Resources.Information;
                    break;
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// show the massage box with one checkbox
        /// </summary>
        /// <param name="message">the message to show</param>
        /// <param name="caption">the caption of message box</param>
        /// <param name="checkboxText">the text of checkbox</param>
        /// <param name="messageboxButtons">the buttons of message box</param>
        /// <param name="messageboxIcons">the icon of message box</param>
        /// <param name="checkboxState">the default state of checkbox, this parameter is referenced</param>
        /// <returns>return one DialogResult</returns>
        public static DialogResult Show(string message,
                                                string caption,
                                                string checkboxText,
                                                MessageBoxButtons messageboxButtons,
                                                MessageBoxIcons messageboxIcons,
                                                ref bool checkboxState)
        {
            CheckableMessageBox messageBox = new CheckableMessageBox(message, caption, checkboxText, messageboxButtons, messageboxIcons);
            return messageBox.ShowCheckableDialog(ref checkboxState);

        }

        /// <summary>
        /// show this message dialog as model
        /// </summary>
        /// <param name="checkboxState">reference value, the state of checkbox control</param>
        /// <returns></returns>
        internal DialogResult ShowCheckableDialog(ref bool checkboxState)
        {
            this.chkOption.Checked = checkboxState;
            DialogResult dialogResult = this.ShowDialog();
            checkboxState = this.chkOption.Checked;

            return dialogResult;
        }

        #endregion


        #region Event handlers

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        #endregion

    }
}
