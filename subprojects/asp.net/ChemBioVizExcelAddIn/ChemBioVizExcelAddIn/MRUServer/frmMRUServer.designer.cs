namespace ChemBioVizExcelAddIn
{
    partial class MRUServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MRUServer));
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlTier = new System.Windows.Forms.Panel();
            this.rbo3Tier = new System.Windows.Forms.RadioButton();
            this.rbo2Tier = new System.Windows.Forms.RadioButton();
            this.cbxSSL = new System.Windows.Forms.CheckBox();
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
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(14, 78);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(359, 22);
            this.txtInput.TabIndex = 0;
            this.txtInput.Text = "<Middle-tier server name>";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(247, 106);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(58, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(320, 106);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(53, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlTier
            // 
            this.pnlTier.Controls.Add(this.rbo3Tier);
            this.pnlTier.Controls.Add(this.rbo2Tier);
            this.pnlTier.Location = new System.Drawing.Point(14, 17);
            this.pnlTier.Name = "pnlTier";
            this.pnlTier.Size = new System.Drawing.Size(152, 26);
            this.pnlTier.TabIndex = 7;
            // 
            // rbo3Tier
            // 
            this.rbo3Tier.AutoSize = true;
            this.rbo3Tier.Checked = true;
            this.rbo3Tier.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbo3Tier.Location = new System.Drawing.Point(13, 3);
            this.rbo3Tier.Name = "rbo3Tier";
            this.rbo3Tier.Size = new System.Drawing.Size(56, 19);
            this.rbo3Tier.TabIndex = 2;
            this.rbo3Tier.TabStop = true;
            this.rbo3Tier.Text = "3-Tier";
            this.rbo3Tier.UseVisualStyleBackColor = true;
            // 
            // rbo2Tier
            // 
            this.rbo2Tier.AutoSize = true;
            this.rbo2Tier.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbo2Tier.Location = new System.Drawing.Point(86, 3);
            this.rbo2Tier.Name = "rbo2Tier";
            this.rbo2Tier.Size = new System.Drawing.Size(56, 19);
            this.rbo2Tier.TabIndex = 1;
            this.rbo2Tier.Text = "2-Tier";
            this.rbo2Tier.UseVisualStyleBackColor = true;
            // 
            // cbxSSL
            // 
            this.cbxSSL.Location = new System.Drawing.Point(302, 17);
            this.cbxSSL.Name = "cbxSSL";
            this.cbxSSL.Size = new System.Drawing.Size(71, 17);
            this.cbxSSL.TabIndex = 8;
            this.cbxSSL.Text = "Use SSL";
            this.cbxSSL.UseVisualStyleBackColor = true;
            // 
            // MRUServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 140);
            this.Controls.Add(this.cbxSSL);
            this.Controls.Add(this.pnlTier);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInput);
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
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlTier;
        private System.Windows.Forms.RadioButton rbo3Tier;
        private System.Windows.Forms.RadioButton rbo2Tier;
        private System.Windows.Forms.CheckBox cbxSSL;
    }
}