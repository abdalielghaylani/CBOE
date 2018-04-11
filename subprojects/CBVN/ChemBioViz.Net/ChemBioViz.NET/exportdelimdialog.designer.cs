namespace ChemBioViz.NET
{
    partial class ExportDelimDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDelimDialog));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.filenameTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.withHdrCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.commaRadioButton = new System.Windows.Forms.RadioButton();
            this.tabRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.selectallBtn = new System.Windows.Forms.Button();
            this.fieldsListBox = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.expRecordsLabel = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.filenameTextBox);
            this.groupBox2.Controls.Add(this.browseButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 36);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 53);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output File";
            // 
            // filenameTextBox
            // 
            this.filenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filenameTextBox.Location = new System.Drawing.Point(6, 19);
            this.filenameTextBox.Name = "filenameTextBox";
            this.filenameTextBox.Size = new System.Drawing.Size(307, 20);
            this.filenameTextBox.TabIndex = 0;
            this.filenameTextBox.TextChanged += new System.EventHandler(this.filenameTextBox_TextChanged);
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(319, 17);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AllowDrop = true;
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(337, 233);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(256, 233);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "Go";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.withHdrCheckBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.commaRadioButton);
            this.groupBox1.Controls.Add(this.tabRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 95);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(153, 133);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // withHdrCheckBox
            // 
            this.withHdrCheckBox.AutoSize = true;
            this.withHdrCheckBox.Location = new System.Drawing.Point(14, 89);
            this.withHdrCheckBox.Name = "withHdrCheckBox";
            this.withHdrCheckBox.Size = new System.Drawing.Size(117, 17);
            this.withHdrCheckBox.TabIndex = 7;
            this.withHdrCheckBox.Text = "Include header row";
            this.withHdrCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Delimiter";
            // 
            // commaRadioButton
            // 
            this.commaRadioButton.AutoSize = true;
            this.commaRadioButton.Location = new System.Drawing.Point(73, 29);
            this.commaRadioButton.Name = "commaRadioButton";
            this.commaRadioButton.Size = new System.Drawing.Size(60, 17);
            this.commaRadioButton.TabIndex = 2;
            this.commaRadioButton.TabStop = true;
            this.commaRadioButton.Text = "Comma";
            this.commaRadioButton.UseVisualStyleBackColor = true;
            // 
            // tabRadioButton
            // 
            this.tabRadioButton.AutoSize = true;
            this.tabRadioButton.Location = new System.Drawing.Point(73, 52);
            this.tabRadioButton.Name = "tabRadioButton";
            this.tabRadioButton.Size = new System.Drawing.Size(44, 17);
            this.tabRadioButton.TabIndex = 3;
            this.tabRadioButton.TabStop = true;
            this.tabRadioButton.Text = "Tab";
            this.tabRadioButton.UseVisualStyleBackColor = true;
            this.tabRadioButton.CheckedChanged += new System.EventHandler(this.tabRadioButton_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.selectallBtn);
            this.groupBox3.Controls.Add(this.fieldsListBox);
            this.groupBox3.Location = new System.Drawing.Point(171, 96);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(241, 132);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Fields";
            // 
            // selectallBtn
            // 
            this.selectallBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectallBtn.Location = new System.Drawing.Point(158, 7);
            this.selectallBtn.Name = "selectallBtn";
            this.selectallBtn.Size = new System.Drawing.Size(76, 21);
            this.selectallBtn.TabIndex = 1;
            this.selectallBtn.Text = "Unselect All";
            this.selectallBtn.UseVisualStyleBackColor = true;
            this.selectallBtn.Click += new System.EventHandler(this.selectallBtn_Click);
            // 
            // fieldsListBox
            // 
            this.fieldsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldsListBox.CheckBoxes = true;
            this.fieldsListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.fieldsListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.fieldsListBox.Location = new System.Drawing.Point(6, 30);
            this.fieldsListBox.Name = "fieldsListBox";
            this.fieldsListBox.Size = new System.Drawing.Size(229, 96);
            this.fieldsListBox.TabIndex = 0;
            this.fieldsListBox.UseCompatibleStateImageBehavior = false;
            this.fieldsListBox.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Field Name";
            this.columnHeader1.Width = 900;
            // 
            // expRecordsLabel
            // 
            this.expRecordsLabel.AutoSize = true;
            this.expRecordsLabel.Location = new System.Drawing.Point(176, 20);
            this.expRecordsLabel.Name = "expRecordsLabel";
            this.expRecordsLabel.Size = new System.Drawing.Size(127, 13);
            this.expRecordsLabel.TabIndex = 2;
            this.expRecordsLabel.Text = "Records to export: 99999";
            // 
            // ExportDelimDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(424, 265);
            this.Controls.Add(this.expRecordsLabel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(432, 280);
            this.Name = "ExportDelimDialog";
            this.Text = "Delimited Text Export";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox filenameTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton commaRadioButton;
        private System.Windows.Forms.RadioButton tabRadioButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView fieldsListBox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox withHdrCheckBox;
        private System.Windows.Forms.Label expRecordsLabel;
        private System.Windows.Forms.Button selectallBtn;
    }
}