namespace FormDBLib
{
    partial class PromptWithRadio
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
            this.userStringTextBox = new System.Windows.Forms.TextBox();
            this.OKUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.CancelUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.messageLabel = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // userStringTextBox
            // 
            this.userStringTextBox.Location = new System.Drawing.Point(12, 35);
            this.userStringTextBox.Name = "userStringTextBox";
            this.userStringTextBox.Size = new System.Drawing.Size(310, 20);
            this.userStringTextBox.TabIndex = 0;
            // 
            // OKUltraButton
            // 
            this.OKUltraButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKUltraButton.Location = new System.Drawing.Point(166, 99);
            this.OKUltraButton.Name = "OKUltraButton";
            this.OKUltraButton.Size = new System.Drawing.Size(75, 23);
            this.OKUltraButton.TabIndex = 3;
            this.OKUltraButton.Text = "OK";
            this.OKUltraButton.Click += new System.EventHandler(this.OKUltraButton_Click);
            // 
            // CancelUltraButton
            // 
            this.CancelUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelUltraButton.Location = new System.Drawing.Point(247, 99);
            this.CancelUltraButton.Name = "CancelUltraButton";
            this.CancelUltraButton.Size = new System.Drawing.Size(75, 23);
            this.CancelUltraButton.TabIndex = 4;
            this.CancelUltraButton.Text = "Cancel";
            this.CancelUltraButton.Click += new System.EventHandler(this.CancelUltraButton_Click);
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(9, 19);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(63, 13);
            this.messageLabel.TabIndex = 10;
            this.messageLabel.Text = "Enter string:";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(32, 65);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(85, 17);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "radioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Click += new System.EventHandler(this.RadioButton1_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(32, 88);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(85, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "radioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Click += new System.EventHandler(this.RadioButton2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(166, 66);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // PromptWithRadio
            // 
            this.AcceptButton = this.OKUltraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelUltraButton;
            this.ClientSize = new System.Drawing.Size(334, 140);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.userStringTextBox);
            this.Controls.Add(this.OKUltraButton);
            this.Controls.Add(this.CancelUltraButton);
            this.Controls.Add(this.messageLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximumSize = new System.Drawing.Size(900, 168);
            this.MinimumSize = new System.Drawing.Size(300, 168);
            this.Name = "PromptWithRadio";
            this.Text = "ChemBioViz";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userStringTextBox;
        private Infragistics.Win.Misc.UltraButton OKUltraButton;
        private Infragistics.Win.Misc.UltraButton CancelUltraButton;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}