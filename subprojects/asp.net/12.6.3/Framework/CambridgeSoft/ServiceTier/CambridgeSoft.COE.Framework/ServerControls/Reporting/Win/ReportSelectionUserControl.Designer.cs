namespace CambridgeSoft.COE.Framework.ServerControls.Reporting.Win
{
    partial class COEReportSelector
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AvailableReportsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CreateLinkLabel = new System.Windows.Forms.LinkLabel();
            this.DeleteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // AvailableReportsComboBox
            // 
            this.AvailableReportsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AvailableReportsComboBox.FormattingEnabled = true;
            this.AvailableReportsComboBox.Location = new System.Drawing.Point(3, 14);
            this.AvailableReportsComboBox.Name = "AvailableReportsComboBox";
            this.AvailableReportsComboBox.Size = new System.Drawing.Size(459, 21);
            this.AvailableReportsComboBox.TabIndex = 0;
            this.AvailableReportsComboBox.SelectedIndexChanged += new System.EventHandler(this.AvailableReportsComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pick a report to preview";
            // 
            // CreateLinkLabel
            // 
            this.CreateLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateLinkLabel.AutoSize = true;
            this.CreateLinkLabel.Location = new System.Drawing.Point(468, 17);
            this.CreateLinkLabel.Name = "CreateLinkLabel";
            this.CreateLinkLabel.Size = new System.Drawing.Size(47, 13);
            this.CreateLinkLabel.TabIndex = 2;
            this.CreateLinkLabel.TabStop = true;
            this.CreateLinkLabel.Text = "Create...";
            this.CreateLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CreateLinkLabel_LinkClicked);
            // 
            // DeleteLinkLabel
            // 
            this.DeleteLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteLinkLabel.AutoSize = true;
            this.DeleteLinkLabel.Location = new System.Drawing.Point(521, 17);
            this.DeleteLinkLabel.Name = "DeleteLinkLabel";
            this.DeleteLinkLabel.Size = new System.Drawing.Size(47, 13);
            this.DeleteLinkLabel.TabIndex = 3;
            this.DeleteLinkLabel.TabStop = true;
            this.DeleteLinkLabel.Text = "Delete...";
            this.DeleteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DeleteLinkLabel_LinkClicked);
            // 
            // COEReportSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DeleteLinkLabel);
            this.Controls.Add(this.CreateLinkLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AvailableReportsComboBox);
            this.Name = "COEReportSelector";
            this.Size = new System.Drawing.Size(571, 38);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.COEReportSelector_Paint);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.COEReportSelector_Layout);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox AvailableReportsComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel CreateLinkLabel;
        private System.Windows.Forms.LinkLabel DeleteLinkLabel;
    }
}
