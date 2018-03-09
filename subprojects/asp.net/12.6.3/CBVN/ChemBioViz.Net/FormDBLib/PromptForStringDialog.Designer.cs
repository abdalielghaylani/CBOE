namespace FormDBLib
{
    partial class PromptForStringDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromptForStringDialog));
            this.messageLabel = new System.Windows.Forms.Label();
            this.CancelUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.OKUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.userStringTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(9, 19);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(63, 13);
            this.messageLabel.TabIndex = 6;
            this.messageLabel.Text = "Enter string:";
            // 
            // CancelUltraButton
            // 
            this.CancelUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelUltraButton.Location = new System.Drawing.Point(246, 76);
            this.CancelUltraButton.Name = "CancelUltraButton";
            this.CancelUltraButton.Size = new System.Drawing.Size(75, 23);
            this.CancelUltraButton.TabIndex = 2;
            this.CancelUltraButton.Text = "Cancel";
            this.CancelUltraButton.Click += new System.EventHandler(this.CancelUltraButton_Click);
            // 
            // OKUltraButton
            // 
            this.OKUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKUltraButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKUltraButton.Location = new System.Drawing.Point(165, 76);
            this.OKUltraButton.Name = "OKUltraButton";
            this.OKUltraButton.Size = new System.Drawing.Size(75, 23);
            this.OKUltraButton.TabIndex = 1;
            this.OKUltraButton.Text = "OK";
            this.OKUltraButton.Click += new System.EventHandler(this.OKUltraButton_Click);
            // 
            // userStringTextBox
            // 
            this.userStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userStringTextBox.Location = new System.Drawing.Point(12, 35);
            this.userStringTextBox.Name = "userStringTextBox";
            this.userStringTextBox.Size = new System.Drawing.Size(309, 20);
            this.userStringTextBox.TabIndex = 0;
            // 
            // PromptForStringDialog
            // 
            this.AcceptButton = this.OKUltraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelUltraButton;
            this.ClientSize = new System.Drawing.Size(333, 111);
            this.Controls.Add(this.userStringTextBox);
            this.Controls.Add(this.OKUltraButton);
            this.Controls.Add(this.CancelUltraButton);
            this.Controls.Add(this.messageLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 145);
            this.MinimumSize = new System.Drawing.Size(200, 145);
            this.Name = "PromptForStringDialog";
            this.Text = "ChemBioViz";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label messageLabel;
        private Infragistics.Win.Misc.UltraButton CancelUltraButton;
        private Infragistics.Win.Misc.UltraButton OKUltraButton;
        private System.Windows.Forms.TextBox userStringTextBox;
    }
}