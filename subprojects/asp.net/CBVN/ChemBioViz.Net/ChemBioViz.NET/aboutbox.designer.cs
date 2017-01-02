namespace ChemBioViz.NET
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.institutionalLinkLabel = new System.Windows.Forms.LinkLabel();
            this.OKUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.trademarkLabel = new System.Windows.Forms.Label();
            this.mCSLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.versionCreationTimeLabel = new System.Windows.Forms.Label();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.mSupportLabel = new System.Windows.Forms.Label();
            this.productNameLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.institutionalSupportLinkLabel = new System.Windows.Forms.LinkLabel();
            this.version = new System.Windows.Forms.Label();
            this.ordersAndInfoText = new System.Windows.Forms.Label();
            this.ordersAndInfoLabel = new System.Windows.Forms.Label();
            this.corporateAddressLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.mSupportText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mCSLogoPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // institutionalLinkLabel
            // 
            this.institutionalLinkLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.institutionalLinkLabel, 2);
            this.institutionalLinkLabel.ForeColor = System.Drawing.Color.Black;
            this.institutionalLinkLabel.LinkColor = System.Drawing.Color.Navy;
            this.institutionalLinkLabel.Location = new System.Drawing.Point(102, 179);
            this.institutionalLinkLabel.Name = "institutionalLinkLabel";
            this.institutionalLinkLabel.Size = new System.Drawing.Size(123, 13);
            this.institutionalLinkLabel.TabIndex = 2;
            this.institutionalLinkLabel.TabStop = true;
            this.institutionalLinkLabel.Text = "www.cambridgesoft.com";
            this.institutionalLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.institutionalLinkLabel_LinkClicked);
            // 
            // OKUltraButton
            // 
            this.OKUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKUltraButton.Location = new System.Drawing.Point(9, 368);
            this.OKUltraButton.Name = "OKUltraButton";
            this.OKUltraButton.Size = new System.Drawing.Size(75, 23);
            this.OKUltraButton.TabIndex = 0;
            this.OKUltraButton.Text = "&OK";
            this.OKUltraButton.Click += new System.EventHandler(this.OKUltraButton_Click);
            // 
            // trademarkLabel
            // 
            this.trademarkLabel.AutoSize = true;
            this.trademarkLabel.BackColor = System.Drawing.Color.Transparent;
            this.trademarkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trademarkLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.trademarkLabel.Location = new System.Drawing.Point(230, 6);
            this.trademarkLabel.Name = "trademarkLabel";
            this.trademarkLabel.Size = new System.Drawing.Size(17, 16);
            this.trademarkLabel.TabIndex = 31;
            this.trademarkLabel.Text = "™";
            // 
            // mCSLogoPictureBox
            // 
            this.mCSLogoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCSLogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("mCSLogoPictureBox.Image")));
            this.mCSLogoPictureBox.Location = new System.Drawing.Point(177, 342);
            this.mCSLogoPictureBox.Name = "mCSLogoPictureBox";
            this.mCSLogoPictureBox.Size = new System.Drawing.Size(216, 49);
            this.mCSLogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.mCSLogoPictureBox.TabIndex = 33;
            this.mCSLogoPictureBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.versionCreationTimeLabel, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.mSupportLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.productNameLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.descriptionLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.copyrightLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.institutionalSupportLinkLabel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.institutionalLinkLabel, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.version, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.ordersAndInfoText, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.ordersAndInfoLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.corporateAddressLabel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.versionLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.mSupportText, 1, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(384, 325);
            this.tableLayoutPanel1.TabIndex = 32;
            // 
            // versionCreationTimeLabel
            // 
            this.versionCreationTimeLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.versionCreationTimeLabel, 2);
            this.versionCreationTimeLabel.ForeColor = System.Drawing.Color.Black;
            this.versionCreationTimeLabel.Location = new System.Drawing.Point(102, 296);
            this.versionCreationTimeLabel.Name = "versionCreationTimeLabel";
            this.versionCreationTimeLabel.Size = new System.Drawing.Size(28, 13);
            this.versionCreationTimeLabel.TabIndex = 34;
            this.versionCreationTimeLabel.Text = "date";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = global::ChemBioViz.NET.Properties.Resources.CBV;
            this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel1.SetRowSpan(this.logoPictureBox, 4);
            this.logoPictureBox.Size = new System.Drawing.Size(92, 50);
            this.logoPictureBox.TabIndex = 29;
            this.logoPictureBox.TabStop = false;
            // 
            // mSupportLabel
            // 
            this.mSupportLabel.AutoSize = true;
            this.mSupportLabel.ForeColor = System.Drawing.Color.Black;
            this.mSupportLabel.Location = new System.Drawing.Point(3, 114);
            this.mSupportLabel.Name = "mSupportLabel";
            this.mSupportLabel.Size = new System.Drawing.Size(62, 13);
            this.mSupportLabel.TabIndex = 2;
            this.mSupportLabel.Text = "SUPPORT:";
            // 
            // productNameLabel
            // 
            this.productNameLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.productNameLabel, 2);
            this.productNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.productNameLabel.Location = new System.Drawing.Point(102, 0);
            this.productNameLabel.Name = "productNameLabel";
            this.productNameLabel.Size = new System.Drawing.Size(114, 24);
            this.productNameLabel.TabIndex = 27;
            this.productNameLabel.Text = "ChemBioViz";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.descriptionLabel, 2);
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.descriptionLabel.Location = new System.Drawing.Point(102, 24);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(272, 26);
            this.descriptionLabel.TabIndex = 28;
            this.descriptionLabel.Text = "Desktop to Enterprise Knowledge Management\r\n\r\n";
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.ForeColor = System.Drawing.Color.Black;
            this.copyrightLabel.Location = new System.Drawing.Point(102, 50);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(70, 13);
            this.copyrightLabel.TabIndex = 0;
            this.copyrightLabel.Text = "© 1999-2013";
            // 
            // institutionalSupportLinkLabel
            // 
            this.institutionalSupportLinkLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.institutionalSupportLinkLabel, 2);
            this.institutionalSupportLinkLabel.ForeColor = System.Drawing.Color.Black;
            this.institutionalSupportLinkLabel.LinkColor = System.Drawing.Color.Navy;
            this.institutionalSupportLinkLabel.Location = new System.Drawing.Point(102, 114);
            this.institutionalSupportLinkLabel.Name = "institutionalSupportLinkLabel";
            this.institutionalSupportLinkLabel.Size = new System.Drawing.Size(168, 13);
            this.institutionalSupportLinkLabel.TabIndex = 33;
            this.institutionalSupportLinkLabel.TabStop = true;
            this.institutionalSupportLinkLabel.Text = "www.cambridgesoft.com/support/";
            this.institutionalSupportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.institutionalSupportLinkLabel_LinkClicked);
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.version, 2);
            this.version.ForeColor = System.Drawing.Color.Black;
            this.version.Location = new System.Drawing.Point(102, 283);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(46, 13);
            this.version.TabIndex = 32;
            this.version.Text = "11.0.0.1";
            // 
            // ordersAndInfoText
            // 
            this.ordersAndInfoText.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.ordersAndInfoText, 2);
            this.ordersAndInfoText.ForeColor = System.Drawing.Color.Black;
            this.ordersAndInfoText.Location = new System.Drawing.Point(102, 192);
            this.ordersAndInfoText.Name = "ordersAndInfoText";
            this.ordersAndInfoText.Size = new System.Drawing.Size(180, 91);
            this.ordersAndInfoText.TabIndex = 1;
            this.ordersAndInfoText.Text = "EMAIL: info@cambridgesoft.com\r\nFAX: 617-588-9390\r\nTEL: 800-315-7300 or 617-588-93" +
                "00\r\n\r\n\r\n\r\n\r\n";
            // 
            // ordersAndInfoLabel
            // 
            this.ordersAndInfoLabel.AutoSize = true;
            this.ordersAndInfoLabel.ForeColor = System.Drawing.Color.Black;
            this.ordersAndInfoLabel.Location = new System.Drawing.Point(3, 179);
            this.ordersAndInfoLabel.Name = "ordersAndInfoLabel";
            this.ordersAndInfoLabel.Size = new System.Drawing.Size(93, 13);
            this.ordersAndInfoLabel.TabIndex = 3;
            this.ordersAndInfoLabel.Text = "ORDERS && INFO:";
            // 
            // corporateAddressLabel
            // 
            this.corporateAddressLabel.AutoSize = true;
            this.corporateAddressLabel.ForeColor = System.Drawing.Color.Black;
            this.corporateAddressLabel.Location = new System.Drawing.Point(178, 50);
            this.corporateAddressLabel.Name = "corporateAddressLabel";
            this.tableLayoutPanel1.SetRowSpan(this.corporateAddressLabel, 2);
            this.corporateAddressLabel.Size = new System.Drawing.Size(143, 64);
            this.corporateAddressLabel.TabIndex = 31;
            this.corporateAddressLabel.Text = "CambridgeSoft Corporation\r\n100 CambridgePark Drive\r\nCambridge, MA  02140  USA\r\n\r\n" +
                "\r\n";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.ForeColor = System.Drawing.Color.Black;
            this.versionLabel.Location = new System.Drawing.Point(3, 283);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(58, 13);
            this.versionLabel.TabIndex = 35;
            this.versionLabel.Text = "VERSION:";
            // 
            // mSupportText
            // 
            this.mSupportText.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.mSupportText, 2);
            this.mSupportText.ForeColor = System.Drawing.Color.Black;
            this.mSupportText.Location = new System.Drawing.Point(102, 127);
            this.mSupportText.Name = "mSupportText";
            this.mSupportText.Size = new System.Drawing.Size(180, 52);
            this.mSupportText.TabIndex = 36;
            this.mSupportText.Text = "EMAIL: support@cambridgesoft.com\r\nFAX: 617-588-9360\r\nTEL: 617-588-9100\r\n\r\n";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.OKUltraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.OKUltraButton;
            this.ClientSize = new System.Drawing.Size(399, 403);
            this.Controls.Add(this.trademarkLabel);
            this.Controls.Add(this.mCSLogoPictureBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.OKUltraButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AboutBox";
            this.Text = "About ChemBioViz";
            ((System.ComponentModel.ISupportInitialize)(this.mCSLogoPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.LinkLabel institutionalLinkLabel;
        private Infragistics.Win.Misc.UltraButton OKUltraButton;
        private System.Windows.Forms.Label trademarkLabel;
        private System.Windows.Forms.PictureBox mCSLogoPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label ordersAndInfoLabel;
        private System.Windows.Forms.Label mSupportLabel;
        private System.Windows.Forms.Label productNameLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label ordersAndInfoText;
        private System.Windows.Forms.Label corporateAddressLabel;
        private System.Windows.Forms.LinkLabel institutionalSupportLinkLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label mSupportText;
        private System.Windows.Forms.Label versionCreationTimeLabel;
	}
}
