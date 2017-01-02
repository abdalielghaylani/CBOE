namespace ChemBioViz.NET
{
    partial class SortDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SortDialog));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fieldLabel = new System.Windows.Forms.Label();
            this.Descending1 = new System.Windows.Forms.RadioButton();
            this.Ascending1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Descending2 = new System.Windows.Forms.RadioButton();
            this.Ascending2 = new System.Windows.Forms.RadioButton();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.OKbutton = new System.Windows.Forms.Button();
            this.SDCancelButton = new System.Windows.Forms.Button();
            this.currListLabel = new System.Windows.Forms.Label();
            this.alwaysSortCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(14, 38);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(256, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.fieldLabel);
            this.groupBox1.Controls.Add(this.Descending1);
            this.groupBox1.Controls.Add(this.Ascending1);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sort by";
            // 
            // fieldLabel
            // 
            this.fieldLabel.AutoSize = true;
            this.fieldLabel.Location = new System.Drawing.Point(10, 20);
            this.fieldLabel.Name = "fieldLabel";
            this.fieldLabel.Size = new System.Drawing.Size(29, 13);
            this.fieldLabel.TabIndex = 6;
            this.fieldLabel.Text = "Field";
            // 
            // Descending1
            // 
            this.Descending1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Descending1.AutoSize = true;
            this.Descending1.Location = new System.Drawing.Point(294, 43);
            this.Descending1.Name = "Descending1";
            this.Descending1.Size = new System.Drawing.Size(82, 17);
            this.Descending1.TabIndex = 3;
            this.Descending1.TabStop = true;
            this.Descending1.Text = "Descending";
            this.Descending1.UseVisualStyleBackColor = true;
            // 
            // Ascending1
            // 
            this.Ascending1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Ascending1.AutoSize = true;
            this.Ascending1.Location = new System.Drawing.Point(294, 19);
            this.Ascending1.Name = "Ascending1";
            this.Ascending1.Size = new System.Drawing.Size(75, 17);
            this.Ascending1.TabIndex = 2;
            this.Ascending1.TabStop = true;
            this.Ascending1.Text = "Ascending";
            this.Ascending1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.Descending2);
            this.groupBox2.Controls.Add(this.Ascending2);
            this.groupBox2.Location = new System.Drawing.Point(12, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(396, 71);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Then by";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Field";
            // 
            // Descending2
            // 
            this.Descending2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Descending2.AutoSize = true;
            this.Descending2.Location = new System.Drawing.Point(294, 43);
            this.Descending2.Name = "Descending2";
            this.Descending2.Size = new System.Drawing.Size(82, 17);
            this.Descending2.TabIndex = 6;
            this.Descending2.TabStop = true;
            this.Descending2.Text = "Descending";
            this.Descending2.UseVisualStyleBackColor = true;
            // 
            // Ascending2
            // 
            this.Ascending2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Ascending2.AutoSize = true;
            this.Ascending2.Location = new System.Drawing.Point(294, 19);
            this.Ascending2.Name = "Ascending2";
            this.Ascending2.Size = new System.Drawing.Size(75, 17);
            this.Ascending2.TabIndex = 5;
            this.Ascending2.TabStop = true;
            this.Ascending2.Text = "Ascending";
            this.Ascending2.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(25, 127);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(256, 21);
            this.comboBox2.TabIndex = 4;
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.Location = new System.Drawing.Point(252, 183);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(75, 23);
            this.OKbutton.TabIndex = 7;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // SDCancelButton
            // 
            this.SDCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SDCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.SDCancelButton.Location = new System.Drawing.Point(333, 183);
            this.SDCancelButton.Name = "SDCancelButton";
            this.SDCancelButton.Size = new System.Drawing.Size(75, 23);
            this.SDCancelButton.TabIndex = 8;
            this.SDCancelButton.Text = "Cancel";
            this.SDCancelButton.UseVisualStyleBackColor = true;
            this.SDCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // currListLabel
            // 
            this.currListLabel.AutoSize = true;
            this.currListLabel.Location = new System.Drawing.Point(25, 173);
            this.currListLabel.Name = "currListLabel";
            this.currListLabel.Size = new System.Drawing.Size(76, 13);
            this.currListLabel.TabIndex = 9;
            this.currListLabel.Text = "Current list: Q2";
            // 
            // alwaysSortCheckBox
            // 
            this.alwaysSortCheckBox.AutoSize = true;
            this.alwaysSortCheckBox.Location = new System.Drawing.Point(28, 189);
            this.alwaysSortCheckBox.Name = "alwaysSortCheckBox";
            this.alwaysSortCheckBox.Size = new System.Drawing.Size(150, 17);
            this.alwaysSortCheckBox.TabIndex = 10;
            this.alwaysSortCheckBox.Text = "always sort list in this order";
            this.alwaysSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // SortDialog
            // 
            this.AcceptButton = this.OKbutton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.SDCancelButton;
            this.ClientSize = new System.Drawing.Size(422, 227);
            this.Controls.Add(this.alwaysSortCheckBox);
            this.Controls.Add(this.currListLabel);
            this.Controls.Add(this.SDCancelButton);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 265);
            this.MinimumSize = new System.Drawing.Size(385, 265);
            this.Name = "SortDialog";
            this.Text = "Sort";
            this.Load += new System.EventHandler(this.SortDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Button SDCancelButton;
        private System.Windows.Forms.RadioButton Descending1;
        private System.Windows.Forms.RadioButton Ascending1;
        private System.Windows.Forms.RadioButton Descending2;
        private System.Windows.Forms.RadioButton Ascending2;
        private System.Windows.Forms.Label fieldLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currListLabel;
        private System.Windows.Forms.CheckBox alwaysSortCheckBox;
    }
}