namespace ChemBioViz.NET
{
    partial class SaveDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveDialog));
            this.nameLabel = new System.Windows.Forms.Label();
            this.optionsLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.commentsLabel = new System.Windows.Forms.Label();
            this.commentsTextBox = new System.Windows.Forms.TextBox();
            this.CancelUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.OKUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.optionsRadioList = new ChemBioViz.NET.RadioList();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 18);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(38, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name:";
            // 
            // optionsLabel
            // 
            this.optionsLabel.AutoSize = true;
            this.optionsLabel.Location = new System.Drawing.Point(12, 118);
            this.optionsLabel.Name = "optionsLabel";
            this.optionsLabel.Size = new System.Drawing.Size(51, 13);
            this.optionsLabel.TabIndex = 1;
            this.optionsLabel.Text = "Location:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.Location = new System.Drawing.Point(85, 18);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(277, 20);
            this.nameTextBox.TabIndex = 0;
            // 
            // commentsLabel
            // 
            this.commentsLabel.AutoSize = true;
            this.commentsLabel.Location = new System.Drawing.Point(12, 60);
            this.commentsLabel.Name = "commentsLabel";
            this.commentsLabel.Size = new System.Drawing.Size(59, 13);
            this.commentsLabel.TabIndex = 7;
            this.commentsLabel.Text = "Comments:";
            // 
            // commentsTextBox
            // 
            this.commentsTextBox.AcceptsReturn = true;
            this.commentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commentsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commentsTextBox.Location = new System.Drawing.Point(85, 53);
            this.commentsTextBox.Multiline = true;
            this.commentsTextBox.Name = "commentsTextBox";
            this.commentsTextBox.Size = new System.Drawing.Size(277, 50);
            this.commentsTextBox.TabIndex = 1;
            // 
            // CancelUltraButton
            // 
            this.CancelUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelUltraButton.Location = new System.Drawing.Point(197, 165);
            this.CancelUltraButton.Name = "CancelUltraButton";
            this.CancelUltraButton.Size = new System.Drawing.Size(75, 23);
            this.CancelUltraButton.TabIndex = 4;
            this.CancelUltraButton.Text = "Cancel";
            this.CancelUltraButton.Click += new System.EventHandler(this.CancelUltraButton_Click);
            // 
            // OKUltraButton
            // 
            this.OKUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKUltraButton.Location = new System.Drawing.Point(285, 165);
            this.OKUltraButton.Name = "OKUltraButton";
            this.OKUltraButton.Size = new System.Drawing.Size(75, 23);
            this.OKUltraButton.TabIndex = 3;
            this.OKUltraButton.Text = "OK";
            this.OKUltraButton.Click += new System.EventHandler(this.OKUltraButton_Click);
            // 
            // optionsRadioList
            // 
            this.optionsRadioList.BackColor = System.Drawing.SystemColors.Window;
            this.optionsRadioList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.optionsRadioList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.optionsRadioList.FormattingEnabled = true;
            this.optionsRadioList.ItemHeight = 16;
            this.optionsRadioList.Location = new System.Drawing.Point(85, 118);
            this.optionsRadioList.Name = "optionsRadioList";
            this.optionsRadioList.Size = new System.Drawing.Size(76, 48);
            this.optionsRadioList.TabIndex = 2;
            this.optionsRadioList.SelectedIndexChanged += new System.EventHandler(this.formOptionsRadioList_SelectedIndexChanged);
            // 
            // SaveDialog
            // 
            this.AcceptButton = this.OKUltraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelUltraButton;
            this.ClientSize = new System.Drawing.Size(376, 198);
            this.Controls.Add(this.OKUltraButton);
            this.Controls.Add(this.CancelUltraButton);
            this.Controls.Add(this.commentsTextBox);
            this.Controls.Add(this.commentsLabel);
            this.Controls.Add(this.optionsRadioList);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.optionsLabel);
            this.Controls.Add(this.nameLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 232);
            this.MinimumSize = new System.Drawing.Size(300, 232);
            this.Name = "SaveDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save As";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label optionsLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label commentsLabel;
        private System.Windows.Forms.TextBox commentsTextBox;
        private RadioList optionsRadioList;
        private Infragistics.Win.Misc.UltraButton CancelUltraButton;
        private Infragistics.Win.Misc.UltraButton OKUltraButton;
    }
}
