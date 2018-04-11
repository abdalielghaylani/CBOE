namespace ChemBioVizExcelAddIn
{
    partial class frmSheetProperties
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
            this.tabSheetProperties = new System.Windows.Forms.TabControl();
            this.tbSheet = new System.Windows.Forms.TabPage();
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLoginUser = new System.Windows.Forms.Label();
            this.lblSheetName = new System.Windows.Forms.Label();
            this.cbxSSL = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblConnmode = new System.Windows.Forms.Label();
            this.lblDispSheetName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblDispCreatedOn = new System.Windows.Forms.Label();
            this.lblServername = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSnam = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDispLogPath = new System.Windows.Forms.Label();
            this.lblDispModifiedBy = new System.Windows.Forms.Label();
            this.lblLogPath = new System.Windows.Forms.Label();
            this.lblDispSheetCreatedBy = new System.Windows.Forms.Label();
            this.lblDispDataview = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ttSheetProp = new System.Windows.Forms.ToolTip(this.components);
            this.tabSheetProperties.SuspendLayout();
            this.tbSheet.SuspendLayout();
            this.pnlProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSheetProperties
            // 
            this.tabSheetProperties.Controls.Add(this.tbSheet);
            this.tabSheetProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSheetProperties.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSheetProperties.Location = new System.Drawing.Point(0, 0);
            this.tabSheetProperties.Name = "tabSheetProperties";
            this.tabSheetProperties.SelectedIndex = 0;
            this.tabSheetProperties.Size = new System.Drawing.Size(431, 433);
            this.tabSheetProperties.TabIndex = 0;
            // 
            // tbSheet
            // 
            this.tbSheet.Controls.Add(this.pnlProperties);
            this.tbSheet.Location = new System.Drawing.Point(4, 24);
            this.tbSheet.Name = "tbSheet";
            this.tbSheet.Padding = new System.Windows.Forms.Padding(3);
            this.tbSheet.Size = new System.Drawing.Size(423, 405);
            this.tbSheet.TabIndex = 0;
            this.tbSheet.Text = "Properties";
            this.tbSheet.UseVisualStyleBackColor = true;
            // 
            // pnlProperties
            // 
            this.pnlProperties.AutoScroll = true;
            this.pnlProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlProperties.Controls.Add(this.label3);
            this.pnlProperties.Controls.Add(this.lblLoginUser);
            this.pnlProperties.Controls.Add(this.lblSheetName);
            this.pnlProperties.Controls.Add(this.cbxSSL);
            this.pnlProperties.Controls.Add(this.label1);
            this.pnlProperties.Controls.Add(this.lblConnmode);
            this.pnlProperties.Controls.Add(this.lblDispSheetName);
            this.pnlProperties.Controls.Add(this.label7);
            this.pnlProperties.Controls.Add(this.lblDispCreatedOn);
            this.pnlProperties.Controls.Add(this.lblServername);
            this.pnlProperties.Controls.Add(this.label5);
            this.pnlProperties.Controls.Add(this.lblSnam);
            this.pnlProperties.Controls.Add(this.label2);
            this.pnlProperties.Controls.Add(this.lblDispLogPath);
            this.pnlProperties.Controls.Add(this.lblDispModifiedBy);
            this.pnlProperties.Controls.Add(this.lblLogPath);
            this.pnlProperties.Controls.Add(this.lblDispSheetCreatedBy);
            this.pnlProperties.Controls.Add(this.lblDispDataview);
            this.pnlProperties.Controls.Add(this.label8);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProperties.Location = new System.Drawing.Point(3, 3);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(417, 399);
            this.pnlProperties.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 22;
            this.label3.Text = "Login User:";
            // 
            // lblLoginUser
            // 
            this.lblLoginUser.AutoSize = true;
            this.lblLoginUser.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoginUser.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblLoginUser.Location = new System.Drawing.Point(166, 53);
            this.lblLoginUser.Name = "lblLoginUser";
            this.lblLoginUser.Size = new System.Drawing.Size(19, 15);
            this.lblLoginUser.TabIndex = 23;
            this.lblLoginUser.Text = "$$";
            // 
            // lblSheetName
            // 
            this.lblSheetName.AutoSize = true;
            this.lblSheetName.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSheetName.Location = new System.Drawing.Point(13, 16);
            this.lblSheetName.Name = "lblSheetName";
            this.lblSheetName.Size = new System.Drawing.Size(73, 15);
            this.lblSheetName.TabIndex = 0;
            this.lblSheetName.Text = "Sheet Name:";
            // 
            // cbxSSL
            // 
            this.cbxSSL.AutoSize = true;
            this.cbxSSL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxSSL.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxSSL.Location = new System.Drawing.Point(338, 168);
            this.cbxSSL.Name = "cbxSSL";
            this.cbxSSL.Size = new System.Drawing.Size(47, 19);
            this.cbxSSL.TabIndex = 21;
            this.cbxSSL.Text = "SSL";
            this.cbxSSL.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sheet Created On:";
            // 
            // lblConnmode
            // 
            this.lblConnmode.AutoSize = true;
            this.lblConnmode.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnmode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnmode.Location = new System.Drawing.Point(165, 130);
            this.lblConnmode.Name = "lblConnmode";
            this.lblConnmode.Size = new System.Drawing.Size(19, 15);
            this.lblConnmode.TabIndex = 19;
            this.lblConnmode.Tag = "";
            this.lblConnmode.Text = "$$";
            // 
            // lblDispSheetName
            // 
            this.lblDispSheetName.AutoSize = true;
            this.lblDispSheetName.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispSheetName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDispSheetName.Location = new System.Drawing.Point(166, 16);
            this.lblDispSheetName.Name = "lblDispSheetName";
            this.lblDispSheetName.Size = new System.Drawing.Size(19, 15);
            this.lblDispSheetName.TabIndex = 6;
            this.lblDispSheetName.Text = "$$";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 130);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 15);
            this.label7.TabIndex = 18;
            this.label7.Text = "Connection Mode:";
            // 
            // lblDispCreatedOn
            // 
            this.lblDispCreatedOn.AutoSize = true;
            this.lblDispCreatedOn.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispCreatedOn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDispCreatedOn.Location = new System.Drawing.Point(165, 209);
            this.lblDispCreatedOn.Name = "lblDispCreatedOn";
            this.lblDispCreatedOn.Size = new System.Drawing.Size(19, 15);
            this.lblDispCreatedOn.TabIndex = 7;
            this.lblDispCreatedOn.Tag = "";
            this.lblDispCreatedOn.Text = "$$";
            // 
            // lblServername
            // 
            this.lblServername.AutoSize = true;
            this.lblServername.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServername.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblServername.Location = new System.Drawing.Point(165, 169);
            this.lblServername.Name = "lblServername";
            this.lblServername.Size = new System.Drawing.Size(19, 15);
            this.lblServername.TabIndex = 17;
            this.lblServername.Tag = "";
            this.lblServername.Text = "$$";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 247);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Sheet Created By:";
            // 
            // lblSnam
            // 
            this.lblSnam.AutoSize = true;
            this.lblSnam.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSnam.Location = new System.Drawing.Point(13, 169);
            this.lblSnam.Name = "lblSnam";
            this.lblSnam.Size = new System.Drawing.Size(120, 15);
            this.lblSnam.TabIndex = 16;
            this.lblSnam.Text = "Server/Service Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 290);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Sheet Modified By:";
            // 
            // lblDispLogPath
            // 
            this.lblDispLogPath.AutoSize = true;
            this.lblDispLogPath.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispLogPath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDispLogPath.Location = new System.Drawing.Point(165, 335);
            this.lblDispLogPath.Name = "lblDispLogPath";
            this.lblDispLogPath.Size = new System.Drawing.Size(19, 15);
            this.lblDispLogPath.TabIndex = 15;
            this.lblDispLogPath.Tag = "";
            this.lblDispLogPath.Text = "$$";
            // 
            // lblDispModifiedBy
            // 
            this.lblDispModifiedBy.AutoSize = true;
            this.lblDispModifiedBy.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispModifiedBy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDispModifiedBy.Location = new System.Drawing.Point(165, 290);
            this.lblDispModifiedBy.Name = "lblDispModifiedBy";
            this.lblDispModifiedBy.Size = new System.Drawing.Size(19, 15);
            this.lblDispModifiedBy.TabIndex = 10;
            this.lblDispModifiedBy.Tag = "";
            this.lblDispModifiedBy.Text = "$$";
            // 
            // lblLogPath
            // 
            this.lblLogPath.AutoSize = true;
            this.lblLogPath.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogPath.Location = new System.Drawing.Point(13, 335);
            this.lblLogPath.Name = "lblLogPath";
            this.lblLogPath.Size = new System.Drawing.Size(58, 15);
            this.lblLogPath.TabIndex = 14;
            this.lblLogPath.Text = "Log Path:";
            // 
            // lblDispSheetCreatedBy
            // 
            this.lblDispSheetCreatedBy.AutoSize = true;
            this.lblDispSheetCreatedBy.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispSheetCreatedBy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDispSheetCreatedBy.Location = new System.Drawing.Point(166, 247);
            this.lblDispSheetCreatedBy.Name = "lblDispSheetCreatedBy";
            this.lblDispSheetCreatedBy.Size = new System.Drawing.Size(19, 15);
            this.lblDispSheetCreatedBy.TabIndex = 11;
            this.lblDispSheetCreatedBy.Tag = "";
            this.lblDispSheetCreatedBy.Text = "$$";
            // 
            // lblDispDataview
            // 
            this.lblDispDataview.AutoSize = true;
            this.lblDispDataview.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDispDataview.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDispDataview.Location = new System.Drawing.Point(166, 90);
            this.lblDispDataview.Name = "lblDispDataview";
            this.lblDispDataview.Size = new System.Drawing.Size(19, 15);
            this.lblDispDataview.TabIndex = 13;
            this.lblDispDataview.Tag = "";
            this.lblDispDataview.Text = "$$";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "Current Dataview:";
            // 
            // frmSheetProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 433);
            this.Controls.Add(this.tabSheetProperties);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmSheetProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CBV Sheet Properties";
            this.tabSheetProperties.ResumeLayout(false);
            this.tbSheet.ResumeLayout(false);
            this.pnlProperties.ResumeLayout(false);
            this.pnlProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSheetProperties;
        private System.Windows.Forms.TabPage tbSheet;
        private System.Windows.Forms.CheckBox cbxSSL;
        private System.Windows.Forms.Label lblConnmode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblServername;
        private System.Windows.Forms.Label lblSnam;
        private System.Windows.Forms.Label lblDispLogPath;
        private System.Windows.Forms.Label lblLogPath;
        private System.Windows.Forms.Label lblDispDataview;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDispSheetCreatedBy;
        private System.Windows.Forms.Label lblDispModifiedBy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDispCreatedOn;
        private System.Windows.Forms.Label lblDispSheetName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSheetName;
        private System.Windows.Forms.ToolTip ttSheetProp;
        private System.Windows.Forms.Panel pnlProperties;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLoginUser;

    }
}