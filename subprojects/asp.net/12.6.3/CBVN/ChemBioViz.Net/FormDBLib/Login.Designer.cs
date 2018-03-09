namespace FormDBLib
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label1 = new System.Windows.Forms.Label();
            this.userName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.cancelUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.OKUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.mruCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.savePasswd = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User name:";
            // 
            // userName
            // 
            this.userName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userName.Location = new System.Drawing.Point(91, 12);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(236, 20);
            this.userName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // password
            // 
            this.password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.password.Location = new System.Drawing.Point(91, 38);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(236, 20);
            this.password.TabIndex = 1;
            // 
            // cancelUltraButton
            // 
            this.cancelUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelUltraButton.Location = new System.Drawing.Point(372, 92);
            this.cancelUltraButton.Name = "cancelUltraButton";
            this.cancelUltraButton.Size = new System.Drawing.Size(75, 23);
            this.cancelUltraButton.TabIndex = 5;
            this.cancelUltraButton.Text = "Cancel";
            this.cancelUltraButton.Click += new System.EventHandler(this.cancelUltraButton_Click);
            // 
            // OKUltraButton
            // 
            this.OKUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKUltraButton.Location = new System.Drawing.Point(372, 63);
            this.OKUltraButton.Name = "OKUltraButton";
            this.OKUltraButton.Size = new System.Drawing.Size(75, 23);
            this.OKUltraButton.TabIndex = 4;
            this.OKUltraButton.Text = "OK";
            this.OKUltraButton.Click += new System.EventHandler(this.OKUltraButton_Click);
            // 
            // mruCombo
            // 
            this.mruCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mruCombo.FormattingEnabled = true;
            this.mruCombo.Location = new System.Drawing.Point(91, 77);
            this.mruCombo.Name = "mruCombo";
            this.mruCombo.Size = new System.Drawing.Size(236, 21);
            this.mruCombo.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Server:";
            // 
            // savePasswd
            // 
            this.savePasswd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.savePasswd.AutoSize = true;
            this.savePasswd.Location = new System.Drawing.Point(348, 15);
            this.savePasswd.Name = "savePasswd";
            this.savePasswd.Size = new System.Drawing.Size(99, 17);
            this.savePasswd.TabIndex = 3;
            this.savePasswd.Text = "Save password";
            this.savePasswd.UseVisualStyleBackColor = true;
            // 
            // Login
            // 
            this.AcceptButton = this.OKUltraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelUltraButton;
            this.ClientSize = new System.Drawing.Size(468, 129);
            this.Controls.Add(this.savePasswd);
            this.Controls.Add(this.mruCombo);
            this.Controls.Add(this.OKUltraButton);
            this.Controls.Add(this.cancelUltraButton);
            this.Controls.Add(this.password);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 161);
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox userName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox password;
        private Infragistics.Win.Misc.UltraButton cancelUltraButton;
        private Infragistics.Win.Misc.UltraButton OKUltraButton;
        private System.Windows.Forms.ComboBox mruCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox savePasswd;
    }
}