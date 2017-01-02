namespace FormWizard
{
    partial class SelectDataviewForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataViewComboBox = new System.Windows.Forms.ComboBox();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.dvDateCreatedLabel = new System.Windows.Forms.Label();
            this.dvUserLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dvDescLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dvNameLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IncludeChildTablescheckBox = new System.Windows.Forms.CheckBox();
            this.tagNameRadioButton = new System.Windows.Forms.RadioButton();
            this.tableNameRadioButton = new System.Windows.Forms.RadioButton();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.retrieveAllButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.selectDataview1 = new FormWizard.SelectDataview();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.helpButton = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(496, 524);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 25);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(587, 524);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Dataview";
            // 
            // dataViewComboBox
            // 
            this.dataViewComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.dataViewComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.dataViewComboBox.DisplayMember = "Name";
            this.dataViewComboBox.Location = new System.Drawing.Point(10, 50);
            this.dataViewComboBox.Name = "dataViewComboBox";
            this.dataViewComboBox.Size = new System.Drawing.Size(400, 22);
            this.dataViewComboBox.TabIndex = 8;
            this.dataViewComboBox.SelectedIndexChanged += new System.EventHandler(this.dataViewComboBox_SelectedIndexChanged);
            this.dataViewComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataViewComboBox_KeyDown);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.dvDateCreatedLabel);
            this.ultraGroupBox1.Controls.Add(this.dvUserLabel);
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Controls.Add(this.label8);
            this.ultraGroupBox1.Controls.Add(this.label6);
            this.ultraGroupBox1.Controls.Add(this.dataViewComboBox);
            this.ultraGroupBox1.Controls.Add(this.dvDescLabel);
            this.ultraGroupBox1.Controls.Add(this.label4);
            this.ultraGroupBox1.Controls.Add(this.dvNameLabel);
            this.ultraGroupBox1.Controls.Add(this.label2);
            this.ultraGroupBox1.Location = new System.Drawing.Point(220, 165);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(460, 183);
            this.ultraGroupBox1.TabIndex = 8;
            this.ultraGroupBox1.Text = "Dataview Summary";
            // 
            // dvDateCreatedLabel
            // 
            this.dvDateCreatedLabel.AutoSize = true;
            this.dvDateCreatedLabel.Location = new System.Drawing.Point(107, 157);
            this.dvDateCreatedLabel.Name = "dvDateCreatedLabel";
            this.dvDateCreatedLabel.Size = new System.Drawing.Size(79, 14);
            this.dvDateCreatedLabel.TabIndex = 0;
            this.dvDateCreatedLabel.Text = "dvDateCreated";
            // 
            // dvUserLabel
            // 
            this.dvUserLabel.AutoSize = true;
            this.dvUserLabel.Location = new System.Drawing.Point(107, 135);
            this.dvUserLabel.Name = "dvUserLabel";
            this.dvUserLabel.Size = new System.Drawing.Size(42, 14);
            this.dvUserLabel.TabIndex = 0;
            this.dvUserLabel.Text = "dvUser";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 14);
            this.label8.TabIndex = 0;
            this.label8.Text = "Date Created:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 14);
            this.label6.TabIndex = 0;
            this.label6.Text = "User:";
            // 
            // dvDescLabel
            // 
            this.dvDescLabel.AutoSize = true;
            this.dvDescLabel.Location = new System.Drawing.Point(107, 112);
            this.dvDescLabel.Name = "dvDescLabel";
            this.dvDescLabel.Size = new System.Drawing.Size(44, 14);
            this.dvDescLabel.TabIndex = 0;
            this.dvDescLabel.Text = "dvDesc";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 14);
            this.label4.TabIndex = 0;
            this.label4.Text = "Description:";
            // 
            // dvNameLabel
            // 
            this.dvNameLabel.AutoSize = true;
            this.dvNameLabel.Location = new System.Drawing.Point(107, 89);
            this.dvNameLabel.Name = "dvNameLabel";
            this.dvNameLabel.Size = new System.Drawing.Size(46, 14);
            this.dvNameLabel.TabIndex = 0;
            this.dvNameLabel.Text = "dvName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name:";
            // 
            // IncludeChildTablescheckBox
            // 
            this.IncludeChildTablescheckBox.AutoSize = true;
            this.IncludeChildTablescheckBox.Location = new System.Drawing.Point(10, 54);
            this.IncludeChildTablescheckBox.Name = "IncludeChildTablescheckBox";
            this.IncludeChildTablescheckBox.Size = new System.Drawing.Size(122, 17);
            this.IncludeChildTablescheckBox.TabIndex = 4;
            this.IncludeChildTablescheckBox.Text = "Include Child Tables";
            this.IncludeChildTablescheckBox.UseVisualStyleBackColor = true;
            this.IncludeChildTablescheckBox.Visible = false;
            // 
            // tagNameRadioButton
            // 
            this.tagNameRadioButton.AutoSize = true;
            this.tagNameRadioButton.Location = new System.Drawing.Point(10, 27);
            this.tagNameRadioButton.Name = "tagNameRadioButton";
            this.tagNameRadioButton.Size = new System.Drawing.Size(75, 17);
            this.tagNameRadioButton.TabIndex = 2;
            this.tagNameRadioButton.Text = "Tag Name";
            this.tagNameRadioButton.UseVisualStyleBackColor = true;
            this.tagNameRadioButton.CheckedChanged += new System.EventHandler(this.searchOptionRadioButton_CheckedChanged);
            // 
            // tableNameRadioButton
            // 
            this.tableNameRadioButton.AutoSize = true;
            this.tableNameRadioButton.Checked = true;
            this.tableNameRadioButton.Location = new System.Drawing.Point(98, 27);
            this.tableNameRadioButton.Name = "tableNameRadioButton";
            this.tableNameRadioButton.Size = new System.Drawing.Size(83, 17);
            this.tableNameRadioButton.TabIndex = 3;
            this.tableNameRadioButton.TabStop = true;
            this.tableNameRadioButton.Text = "Table Name";
            this.tableNameRadioButton.UseVisualStyleBackColor = true;
            this.tableNameRadioButton.CheckedChanged += new System.EventHandler(this.searchOptionRadioButton_CheckedChanged);
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.retrieveAllButton);
            this.ultraGroupBox2.Controls.Add(this.searchButton);
            this.ultraGroupBox2.Controls.Add(this.searchTextBox);
            this.ultraGroupBox2.Controls.Add(this.IncludeChildTablescheckBox);
            this.ultraGroupBox2.Controls.Add(this.tableNameRadioButton);
            this.ultraGroupBox2.Controls.Add(this.tagNameRadioButton);
            this.ultraGroupBox2.Location = new System.Drawing.Point(221, 5);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(460, 151);
            this.ultraGroupBox2.TabIndex = 2;
            this.ultraGroupBox2.Text = "Advance Search";
            // 
            // retrieveAllButton
            // 
            this.retrieveAllButton.Location = new System.Drawing.Point(366, 111);
            this.retrieveAllButton.Name = "retrieveAllButton";
            this.retrieveAllButton.Size = new System.Drawing.Size(75, 25);
            this.retrieveAllButton.TabIndex = 7;
            this.retrieveAllButton.Text = "Retrieve &All";
            this.retrieveAllButton.UseVisualStyleBackColor = true;
            this.retrieveAllButton.Click += new System.EventHandler(this.retrieveAllButton_Click);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(275, 111);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 25);
            this.searchButton.TabIndex = 6;
            this.searchButton.Text = "&Find";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.searchTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.searchTextBox.Location = new System.Drawing.Point(10, 81);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(400, 20);
            this.searchTextBox.TabIndex = 5;
            // 
            // selectDataview1
            // 
            this.selectDataview1.Cursor = System.Windows.Forms.Cursors.Default;
            this.selectDataview1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectDataview1.Location = new System.Drawing.Point(5, 0);
            this.selectDataview1.Name = "selectDataview1";
            this.selectDataview1.Size = new System.Drawing.Size(210, 549);
            this.selectDataview1.TabIndex = 0;
            this.selectDataview1.tableFilter = null;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(220, 524);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 25);
            this.helpButton.TabIndex = 11;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            this.helpButton.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.helpButton_HelpRequested);
            // 
            // SelectDataviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(692, 563);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.ultraGroupBox2);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.selectDataview1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.HelpButton = true;
            this.Name = "SelectDataviewForm";
            this.helpProvider1.SetShowHelp(this, false);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Dataview";
            this.Load += new System.EventHandler(this.SelectDataviewForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SelectDataview selectDataview1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox dataViewComboBox;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.Label dvDateCreatedLabel;
        private System.Windows.Forms.Label dvUserLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label dvDescLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label dvNameLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox IncludeChildTablescheckBox;
        private System.Windows.Forms.RadioButton tagNameRadioButton;
        private System.Windows.Forms.RadioButton tableNameRadioButton;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private System.Windows.Forms.Button retrieveAllButton;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}