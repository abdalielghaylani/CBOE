namespace ChemBioVizExcelAddIn
{
    partial class frmMaxHit
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMaxPrompt = new System.Windows.Forms.TextBox();
            this.txtMaxRecord = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtMaxStructureWidth = new System.Windows.Forms.TextBox();
            this.txtMaxStructureHeight = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbSearchHit = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMaxStructures = new System.Windows.Forms.TextBox();
            this.tbStructureDisp = new System.Windows.Forms.TabPage();
            this.pnlCBVOptions = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tbSearchHit.SuspendLayout();
            this.tbStructureDisp.SuspendLayout();
            this.pnlCBVOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum number of record to prompt:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Location = new System.Drawing.Point(6, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Maximum number of record to return:";
            // 
            // txtMaxPrompt
            // 
            this.txtMaxPrompt.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxPrompt.Location = new System.Drawing.Point(275, 12);
            this.txtMaxPrompt.Name = "txtMaxPrompt";
            this.txtMaxPrompt.Size = new System.Drawing.Size(125, 21);
            this.txtMaxPrompt.TabIndex = 0;
            this.txtMaxPrompt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMaxRecord
            // 
            this.txtMaxRecord.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxRecord.Location = new System.Drawing.Point(275, 39);
            this.txtMaxRecord.Name = "txtMaxRecord";
            this.txtMaxRecord.Size = new System.Drawing.Size(125, 21);
            this.txtMaxRecord.TabIndex = 1;
            this.txtMaxRecord.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btnOK.Location = new System.Drawing.Point(279, 128);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btnCancel.Location = new System.Drawing.Point(367, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtMaxStructureWidth
            // 
            this.txtMaxStructureWidth.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxStructureWidth.Location = new System.Drawing.Point(275, 39);
            this.txtMaxStructureWidth.MaxLength = 3;
            this.txtMaxStructureWidth.Name = "txtMaxStructureWidth";
            this.txtMaxStructureWidth.Size = new System.Drawing.Size(125, 21);
            this.txtMaxStructureWidth.TabIndex = 7;
            this.txtMaxStructureWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMaxStructureHeight
            // 
            this.txtMaxStructureHeight.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxStructureHeight.Location = new System.Drawing.Point(275, 12);
            this.txtMaxStructureHeight.MaxLength = 3;
            this.txtMaxStructureHeight.Name = "txtMaxStructureHeight";
            this.txtMaxStructureHeight.Size = new System.Drawing.Size(125, 21);
            this.txtMaxStructureHeight.TabIndex = 6;
            this.txtMaxStructureHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Location = new System.Drawing.Point(6, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(257, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Maximum Structure Width:";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label4.Location = new System.Drawing.Point(6, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "Maximum Structure Height:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbSearchHit);
            this.tabControl1.Controls.Add(this.tbStructureDisp);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(440, 122);
            this.tabControl1.TabIndex = 8;
            // 
            // tbSearchHit
            // 
            this.tbSearchHit.Controls.Add(this.label5);
            this.tbSearchHit.Controls.Add(this.txtMaxStructures);
            this.tbSearchHit.Controls.Add(this.label1);
            this.tbSearchHit.Controls.Add(this.label2);
            this.tbSearchHit.Controls.Add(this.txtMaxPrompt);
            this.tbSearchHit.Controls.Add(this.txtMaxRecord);
            this.tbSearchHit.Location = new System.Drawing.Point(4, 24);
            this.tbSearchHit.Name = "tbSearchHit";
            this.tbSearchHit.Padding = new System.Windows.Forms.Padding(3);
            this.tbSearchHit.Size = new System.Drawing.Size(432, 94);
            this.tbSearchHit.TabIndex = 0;
            this.tbSearchHit.Text = "Search Hit Prompts";
            this.tbSearchHit.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label5.Location = new System.Drawing.Point(6, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(257, 23);
            this.label5.TabIndex = 2;
            this.label5.Text = "Maximum hits to show structures:";
            // 
            // txtMaxStructures
            // 
            this.txtMaxStructures.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxStructures.Location = new System.Drawing.Point(275, 66);
            this.txtMaxStructures.Name = "txtMaxStructures";
            this.txtMaxStructures.Size = new System.Drawing.Size(125, 21);
            this.txtMaxStructures.TabIndex = 3;
            this.txtMaxStructures.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbStructureDisp
            // 
            this.tbStructureDisp.Controls.Add(this.label4);
            this.tbStructureDisp.Controls.Add(this.txtMaxStructureWidth);
            this.tbStructureDisp.Controls.Add(this.label3);
            this.tbStructureDisp.Controls.Add(this.txtMaxStructureHeight);
            this.tbStructureDisp.Location = new System.Drawing.Point(4, 24);
            this.tbStructureDisp.Name = "tbStructureDisp";
            this.tbStructureDisp.Padding = new System.Windows.Forms.Padding(3);
            this.tbStructureDisp.Size = new System.Drawing.Size(432, 94);
            this.tbStructureDisp.TabIndex = 1;
            this.tbStructureDisp.Text = "Structure Display Options";
            this.tbStructureDisp.UseVisualStyleBackColor = true;
            // 
            // pnlCBVOptions
            // 
            this.pnlCBVOptions.Controls.Add(this.tabControl1);
            this.pnlCBVOptions.Controls.Add(this.btnOK);
            this.pnlCBVOptions.Controls.Add(this.btnCancel);
            this.pnlCBVOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCBVOptions.Location = new System.Drawing.Point(0, 0);
            this.pnlCBVOptions.Name = "pnlCBVOptions";
            this.pnlCBVOptions.Size = new System.Drawing.Size(440, 158);
            this.pnlCBVOptions.TabIndex = 9;
            // 
            // frmMaxHit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 158);
            this.Controls.Add(this.pnlCBVOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMaxHit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChemBioViz Options";
            this.tabControl1.ResumeLayout(false);
            this.tbSearchHit.ResumeLayout(false);
            this.tbSearchHit.PerformLayout();
            this.tbStructureDisp.ResumeLayout(false);
            this.tbStructureDisp.PerformLayout();
            this.pnlCBVOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMaxPrompt;
        private System.Windows.Forms.TextBox txtMaxRecord;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtMaxStructureWidth;
        private System.Windows.Forms.TextBox txtMaxStructureHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbSearchHit;
        private System.Windows.Forms.TabPage tbStructureDisp;
        private System.Windows.Forms.Panel pnlCBVOptions;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxStructures;
    }
}