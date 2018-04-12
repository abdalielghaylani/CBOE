namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
	partial class AboutBox
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
            this.institutionalLinkLabel = new System.Windows.Forms.LinkLabel();
            this.okButton = new System.Windows.Forms.Button();
            this.mCSLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.lblBuildDate = new System.Windows.Forms.Label();
            this.mSupportLabel = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.institutionalSupportLinkLabel = new System.Windows.Forms.LinkLabel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.ordersAndInfoText = new System.Windows.Forms.Label();
            this.ordersAndInfoLabel = new System.Windows.Forms.Label();
            this.corporateAddressLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.mSupportText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mCSLogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // institutionalLinkLabel
            // 
            this.institutionalLinkLabel.AutoSize = true;
            this.institutionalLinkLabel.ForeColor = System.Drawing.Color.Black;
            this.institutionalLinkLabel.LinkColor = System.Drawing.Color.Navy;
            this.institutionalLinkLabel.Location = new System.Drawing.Point(151, 139);
            this.institutionalLinkLabel.Name = "institutionalLinkLabel";
            this.institutionalLinkLabel.Size = new System.Drawing.Size(123, 13);
            this.institutionalLinkLabel.TabIndex = 2;
            this.institutionalLinkLabel.TabStop = true;
            this.institutionalLinkLabel.Text = "www.cambridgesoft.com";
            this.institutionalLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.institutionalLinkLabel_LinkClicked);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(9, 290);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // mCSLogoPictureBox
            // 
            this.mCSLogoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCSLogoPictureBox.Image = global::CambridgeSoft.COE.DataLoader.Properties.Resources.DataLoader_02;
            this.mCSLogoPictureBox.InitialImage = global::CambridgeSoft.COE.DataLoader.Properties.Resources.Data_Loader;
            this.mCSLogoPictureBox.Location = new System.Drawing.Point(174, 250);
            this.mCSLogoPictureBox.Name = "mCSLogoPictureBox";
            this.mCSLogoPictureBox.Size = new System.Drawing.Size(191, 63);
            this.mCSLogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.mCSLogoPictureBox.TabIndex = 33;
            this.mCSLogoPictureBox.TabStop = false;
            // 
            // lblBuildDate
            // 
            this.lblBuildDate.AutoSize = true;
            this.lblBuildDate.ForeColor = System.Drawing.Color.Black;
            this.lblBuildDate.Location = new System.Drawing.Point(215, 199);
            this.lblBuildDate.Name = "lblBuildDate";
            this.lblBuildDate.Size = new System.Drawing.Size(59, 13);
            this.lblBuildDate.TabIndex = 34;
            this.lblBuildDate.Text = "[build date]";
            // 
            // mSupportLabel
            // 
            this.mSupportLabel.AutoSize = true;
            this.mSupportLabel.ForeColor = System.Drawing.Color.Black;
            this.mSupportLabel.Location = new System.Drawing.Point(39, 79);
            this.mSupportLabel.Name = "mSupportLabel";
            this.mSupportLabel.Size = new System.Drawing.Size(62, 13);
            this.mSupportLabel.TabIndex = 2;
            this.mSupportLabel.Text = "SUPPORT:";
            // 
            // lblProductName
            // 
            this.lblProductName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblProductName.Location = new System.Drawing.Point(5, 9);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(368, 24);
            this.lblProductName.TabIndex = 1;
            this.lblProductName.Text = "[product name]";
            this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblCopyright.Location = new System.Drawing.Point(6, 43);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(367, 15);
            this.lblCopyright.TabIndex = 28;
            this.lblCopyright.Text = "[copyright]";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // institutionalSupportLinkLabel
            // 
            this.institutionalSupportLinkLabel.AutoSize = true;
            this.institutionalSupportLinkLabel.ForeColor = System.Drawing.Color.Black;
            this.institutionalSupportLinkLabel.LinkColor = System.Drawing.Color.Navy;
            this.institutionalSupportLinkLabel.Location = new System.Drawing.Point(151, 79);
            this.institutionalSupportLinkLabel.Name = "institutionalSupportLinkLabel";
            this.institutionalSupportLinkLabel.Size = new System.Drawing.Size(168, 13);
            this.institutionalSupportLinkLabel.TabIndex = 33;
            this.institutionalSupportLinkLabel.TabStop = true;
            this.institutionalSupportLinkLabel.Text = "www.cambridgesoft.com/support/";
            this.institutionalSupportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.institutionalSupportLinkLabel_LinkClicked);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.ForeColor = System.Drawing.Color.Black;
            this.lblVersion.Location = new System.Drawing.Point(151, 199);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(47, 13);
            this.lblVersion.TabIndex = 32;
            this.lblVersion.Text = "[version]";
            // 
            // ordersAndInfoText
            // 
            this.ordersAndInfoText.AutoSize = true;
            this.ordersAndInfoText.ForeColor = System.Drawing.Color.Black;
            this.ordersAndInfoText.Location = new System.Drawing.Point(151, 152);
            this.ordersAndInfoText.Name = "ordersAndInfoText";
            this.ordersAndInfoText.Size = new System.Drawing.Size(180, 39);
            this.ordersAndInfoText.TabIndex = 1;
            this.ordersAndInfoText.Text = "EMAIL: info@cambridgesoft.com\r\nFAX: 617-588-9390\r\nTEL: 800-315-7300 or 617-588-93" +
                "00";
            // 
            // ordersAndInfoLabel
            // 
            this.ordersAndInfoLabel.AutoSize = true;
            this.ordersAndInfoLabel.ForeColor = System.Drawing.Color.Black;
            this.ordersAndInfoLabel.Location = new System.Drawing.Point(39, 139);
            this.ordersAndInfoLabel.Name = "ordersAndInfoLabel";
            this.ordersAndInfoLabel.Size = new System.Drawing.Size(93, 13);
            this.ordersAndInfoLabel.TabIndex = 3;
            this.ordersAndInfoLabel.Text = "ORDERS && INFO:";
            // 
            // corporateAddressLabel
            // 
            this.corporateAddressLabel.AutoSize = true;
            this.corporateAddressLabel.ForeColor = System.Drawing.Color.Black;
            this.corporateAddressLabel.Location = new System.Drawing.Point(9, 228);
            this.corporateAddressLabel.Name = "corporateAddressLabel";
            this.corporateAddressLabel.Size = new System.Drawing.Size(157, 52);
            this.corporateAddressLabel.TabIndex = 31;
            this.corporateAddressLabel.Text = "CambridgeSoft Corporation,\r\na subsidiary of PerkinElmer, Inc.\r\n940 Winter Street " +
                "| Waltham,\r\nMA 02451";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.ForeColor = System.Drawing.Color.Black;
            this.versionLabel.Location = new System.Drawing.Point(39, 199);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(58, 13);
            this.versionLabel.TabIndex = 35;
            this.versionLabel.Text = "VERSION:";
            // 
            // mSupportText
            // 
            this.mSupportText.AutoSize = true;
            this.mSupportText.ForeColor = System.Drawing.Color.Black;
            this.mSupportText.Location = new System.Drawing.Point(151, 92);
            this.mSupportText.Name = "mSupportText";
            this.mSupportText.Size = new System.Drawing.Size(180, 39);
            this.mSupportText.TabIndex = 36;
            this.mSupportText.Text = "EMAIL: support@cambridgesoft.com\r\nFAX: 617-588-9360\r\nTEL: 617-588-9100";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(377, 325);
            this.Controls.Add(this.lblProductName);
            this.Controls.Add(this.lblBuildDate);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.institutionalLinkLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.ordersAndInfoText);
            this.Controls.Add(this.institutionalSupportLinkLabel);
            this.Controls.Add(this.mSupportLabel);
            this.Controls.Add(this.ordersAndInfoLabel);
            this.Controls.Add(this.mCSLogoPictureBox);
            this.Controls.Add(this.mSupportText);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.corporateAddressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AboutBox";
            this.Text = "title]";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.mCSLogoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.LinkLabel institutionalLinkLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox mCSLogoPictureBox;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label ordersAndInfoLabel;
        private System.Windows.Forms.Label mSupportLabel;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label ordersAndInfoText;
        private System.Windows.Forms.Label corporateAddressLabel;
        private System.Windows.Forms.LinkLabel institutionalSupportLinkLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label mSupportText;
        private System.Windows.Forms.Label lblBuildDate;
	}
}
