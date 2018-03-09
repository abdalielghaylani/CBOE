namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    partial class LoginDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginDialog));
            this.TextBoxPassword = new System.Windows.Forms.TextBox();
            this.TextBoxUserName = new System.Windows.Forms.TextBox();
            this.LabelPassword = new System.Windows.Forms.Label();
            this.LabelUserName = new System.Windows.Forms.Label();
            this.LabelMessege = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.StatusToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.ServerComboBox = new System.Windows.Forms.ComboBox();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.SavePasswordCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBoxPassword
            // 
            this.TextBoxPassword.Location = new System.Drawing.Point(168, 38);
            this.TextBoxPassword.Name = "TextBoxPassword";
            this.TextBoxPassword.Size = new System.Drawing.Size(257, 20);
            this.TextBoxPassword.TabIndex = 1;
            this.TextBoxPassword.UseSystemPasswordChar = true;
            // 
            // TextBoxUserName
            // 
            this.TextBoxUserName.Location = new System.Drawing.Point(168, 12);
            this.TextBoxUserName.Name = "TextBoxUserName";
            this.TextBoxUserName.Size = new System.Drawing.Size(257, 20);
            this.TextBoxUserName.TabIndex = 0;
            // 
            // LabelPassword
            // 
            this.LabelPassword.AutoSize = true;
            this.LabelPassword.Location = new System.Drawing.Point(86, 41);
            this.LabelPassword.Name = "LabelPassword";
            this.LabelPassword.Size = new System.Drawing.Size(56, 13);
            this.LabelPassword.TabIndex = 0;
            this.LabelPassword.Text = "Password:";
            this.LabelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelUserName
            // 
            this.LabelUserName.AutoSize = true;
            this.LabelUserName.Location = new System.Drawing.Point(86, 15);
            this.LabelUserName.Name = "LabelUserName";
            this.LabelUserName.Size = new System.Drawing.Size(63, 13);
            this.LabelUserName.TabIndex = 0;
            this.LabelUserName.Text = "User Name:";
            this.LabelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelMessege
            // 
            this.LabelMessege.AutoSize = true;
            this.LabelMessege.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMessege.ForeColor = System.Drawing.Color.Red;
            this.LabelMessege.Location = new System.Drawing.Point(12, 9);
            this.LabelMessege.Name = "LabelMessege";
            this.LabelMessege.Size = new System.Drawing.Size(0, 15);
            this.LabelMessege.TabIndex = 1;
            this.LabelMessege.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(350, 120);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 5;
            this.ButtonCancel.Text = "&Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOK
            // 
            this.ButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOK.Location = new System.Drawing.Point(269, 120);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 4;
            this.ButtonOK.Text = "&Log In";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(86, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Server:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerComboBox
            // 
            this.ServerComboBox.FormattingEnabled = true;
            this.ServerComboBox.Items.AddRange(new object[] {
            "localhost",
            "add..."});
            this.ServerComboBox.Location = new System.Drawing.Point(168, 65);
            this.ServerComboBox.Name = "ServerComboBox";
            this.ServerComboBox.Size = new System.Drawing.Size(257, 21);
            this.ServerComboBox.TabIndex = 2;
            this.ServerComboBox.SelectionChangeCommitted += new System.EventHandler(this.ServerComboBox_SelectionChangeCommitted);
            this.ServerComboBox.SelectedIndexChanged += new System.EventHandler(this.ServerComboBox_SelectedIndexChanged);
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("LogoPictureBox.Image")));
            this.LogoPictureBox.Location = new System.Drawing.Point(12, 15);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(52, 56);
            this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LogoPictureBox.TabIndex = 6;
            this.LogoPictureBox.TabStop = false;
            // 
            // SavePasswordCheckBox
            // 
            this.SavePasswordCheckBox.AutoSize = true;
            this.SavePasswordCheckBox.Location = new System.Drawing.Point(168, 92);
            this.SavePasswordCheckBox.Name = "SavePasswordCheckBox";
            this.SavePasswordCheckBox.Size = new System.Drawing.Size(99, 17);
            this.SavePasswordCheckBox.TabIndex = 3;
            this.SavePasswordCheckBox.Text = "Save password";
            this.SavePasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // LoginDialog
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(437, 155);
            this.Controls.Add(this.SavePasswordCheckBox);
            this.Controls.Add(this.LogoPictureBox);
            this.Controls.Add(this.ServerComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxPassword);
            this.Controls.Add(this.TextBoxUserName);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.LabelPassword);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.LabelUserName);
            this.Controls.Add(this.LabelMessege);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label LabelPassword;
        internal System.Windows.Forms.Label LabelUserName;
        internal System.Windows.Forms.TextBox TextBoxPassword;
        internal System.Windows.Forms.TextBox TextBoxUserName;
        internal System.Windows.Forms.Label LabelMessege;
        internal System.Windows.Forms.Button ButtonCancel;
        internal System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.ToolTip StatusToolTip;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ServerComboBox;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.CheckBox SavePasswordCheckBox;

    }
}
