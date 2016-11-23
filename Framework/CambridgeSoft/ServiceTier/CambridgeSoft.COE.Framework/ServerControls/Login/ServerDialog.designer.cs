namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    partial class ServerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerDialog));
            this.lblHeader = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.pnlTier = new System.Windows.Forms.Panel();
            this.ThreeTierRadioButton = new System.Windows.Forms.RadioButton();
            this.TwoTierRadioButton = new System.Windows.Forms.RadioButton();
            this.SSLCheckBox = new System.Windows.Forms.CheckBox();
            this.pnlTier.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(12, 57);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(330, 15);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Enter middle tier server name";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTextBox.Location = new System.Drawing.Point(14, 78);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(359, 22);
            this.NameTextBox.TabIndex = 0;
            this.NameTextBox.Text = "<Middle-tier server name>";
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OKButton.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(247, 106);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(58, 24);
            this.OKButton.TabIndex = 3;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(320, 106);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(53, 24);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // pnlTier
            // 
            this.pnlTier.Controls.Add(this.ThreeTierRadioButton);
            this.pnlTier.Controls.Add(this.TwoTierRadioButton);
            this.pnlTier.Location = new System.Drawing.Point(14, 17);
            this.pnlTier.Name = "pnlTier";
            this.pnlTier.Size = new System.Drawing.Size(152, 26);
            this.pnlTier.TabIndex = 7;
            // 
            // ThreeTierRadioButton
            // 
            this.ThreeTierRadioButton.AutoSize = true;
            this.ThreeTierRadioButton.Checked = true;
            this.ThreeTierRadioButton.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ThreeTierRadioButton.Location = new System.Drawing.Point(13, 3);
            this.ThreeTierRadioButton.Name = "ThreeTierRadioButton";
            this.ThreeTierRadioButton.Size = new System.Drawing.Size(56, 19);
            this.ThreeTierRadioButton.TabIndex = 2;
            this.ThreeTierRadioButton.TabStop = true;
            this.ThreeTierRadioButton.Text = "3-Tier";
            this.ThreeTierRadioButton.UseVisualStyleBackColor = true;
            // 
            // TwoTierRadioButton
            // 
            this.TwoTierRadioButton.AutoSize = true;
            this.TwoTierRadioButton.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TwoTierRadioButton.Location = new System.Drawing.Point(86, 3);
            this.TwoTierRadioButton.Name = "TwoTierRadioButton";
            this.TwoTierRadioButton.Size = new System.Drawing.Size(56, 19);
            this.TwoTierRadioButton.TabIndex = 1;
            this.TwoTierRadioButton.Text = "2-Tier";
            this.TwoTierRadioButton.UseVisualStyleBackColor = true;
            // 
            // SSLCheckBox
            // 
            this.SSLCheckBox.Location = new System.Drawing.Point(302, 17);
            this.SSLCheckBox.Name = "SSLCheckBox";
            this.SSLCheckBox.Size = new System.Drawing.Size(71, 17);
            this.SSLCheckBox.TabIndex = 8;
            this.SSLCheckBox.Text = "Use SSL";
            this.SSLCheckBox.UseVisualStyleBackColor = true;
            // 
            // MRUServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 140);
            this.Controls.Add(this.SSLCheckBox);
            this.Controls.Add(this.pnlTier);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.lblHeader);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MRUServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.pnlTier.ResumeLayout(false);
            this.pnlTier.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Panel pnlTier;
        private System.Windows.Forms.RadioButton ThreeTierRadioButton;
        private System.Windows.Forms.RadioButton TwoTierRadioButton;
        private System.Windows.Forms.CheckBox SSLCheckBox;
    }
}