namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    partial class DataBaseSaveDialog
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
            this.NewRecordGroupBox = new System.Windows.Forms.GroupBox();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NewRecordGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelUIButton
            // 
            this.CancelUIButton.Location = new System.Drawing.Point(940, 355);
            // 
            // AcceptUIButton
            // 
            this.AcceptUIButton.Location = new System.Drawing.Point(939, 324);
            this.AcceptUIButton.Click += new System.EventHandler(this.AcceptUIButton_Click);
            // 
            // ErrorsTextBox
            // 
            this.ErrorsTextBox.Size = new System.Drawing.Size(1002, 42);
            // 
            // NewRecordGroupBox
            // 
            this.NewRecordGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NewRecordGroupBox.Controls.Add(this.DescriptionTextBox);
            this.NewRecordGroupBox.Controls.Add(this.DescriptionLabel);
            this.NewRecordGroupBox.Controls.Add(this.NameTextBox);
            this.NewRecordGroupBox.Controls.Add(this.NameLabel);
            this.NewRecordGroupBox.Location = new System.Drawing.Point(12, 310);
            this.NewRecordGroupBox.Name = "NewRecordGroupBox";
            this.NewRecordGroupBox.Size = new System.Drawing.Size(909, 80);
            this.NewRecordGroupBox.TabIndex = 4;
            this.NewRecordGroupBox.TabStop = false;
            this.NewRecordGroupBox.Text = "New Record";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Location = new System.Drawing.Point(82, 45);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(432, 20);
            this.DescriptionTextBox.TabIndex = 3;
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Location = new System.Drawing.Point(9, 45);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(63, 13);
            this.DescriptionLabel.TabIndex = 2;
            this.DescriptionLabel.Text = "Description:";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(82, 19);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(432, 20);
            this.NameTextBox.TabIndex = 1;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(9, 19);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(38, 13);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Name:";
            // 
            // DataBaseSaveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(1027, 402);
            this.Controls.Add(this.NewRecordGroupBox);
            this.Name = "DataBaseSaveDialog";
            this.Controls.SetChildIndex(this.ErrorsTextBox, 0);
            this.Controls.SetChildIndex(this.CancelUIButton, 0);
            this.Controls.SetChildIndex(this.AcceptUIButton, 0);
            this.Controls.SetChildIndex(this.NewRecordGroupBox, 0);
            this.NewRecordGroupBox.ResumeLayout(false);
            this.NewRecordGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox NewRecordGroupBox;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label NameLabel;

    }
}