namespace ChemBioVizExcelAddIn
{
    partial class frmDataviewSchema
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
            this.tvDataview = new System.Windows.Forms.TreeView();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDvname = new System.Windows.Forms.Label();
            this.lblDVlabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvDataview
            // 
            this.tvDataview.CheckBoxes = true;
            this.tvDataview.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvDataview.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tvDataview.Location = new System.Drawing.Point(9, 57);
            this.tvDataview.Name = "tvDataview";
            this.tvDataview.Size = new System.Drawing.Size(417, 400);
            this.tvDataview.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(368, 463);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(58, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDvname);
            this.panel1.Controls.Add(this.lblDVlabel);
            this.panel1.Controls.Add(this.tvDataview);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Location = new System.Drawing.Point(3, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 493);
            this.panel1.TabIndex = 3;
            // 
            // lblDvname
            // 
            this.lblDvname.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDvname.Location = new System.Drawing.Point(131, 24);
            this.lblDvname.Name = "lblDvname";
            this.lblDvname.Size = new System.Drawing.Size(281, 30);
            this.lblDvname.TabIndex = 4;
            // 
            // lblDVlabel
            // 
            this.lblDVlabel.AutoSize = true;
            this.lblDVlabel.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDVlabel.Location = new System.Drawing.Point(21, 24);
            this.lblDVlabel.Name = "lblDVlabel";
            this.lblDVlabel.Size = new System.Drawing.Size(93, 15);
            this.lblDVlabel.TabIndex = 3;
            this.lblDVlabel.Text = "Dataview Name:";
            // 
            // frmDataviewSchema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 507);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmDataviewSchema";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dataview";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvDataview;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblDvname;
        private System.Windows.Forms.Label lblDVlabel;
    }
}