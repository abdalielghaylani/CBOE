namespace CambridgeSoft.DataLoaderGUI.Controls
{
    partial class ImportAndDuplicateCheck
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AllRadioButton = new System.Windows.Forms.RadioButton();
            this.SelectedRecordsRadioButton = new System.Windows.Forms.RadioButton();
            this.InvalidRecordsRadioButton = new System.Windows.Forms.RadioButton();
            this.DuplicateRadioButton = new System.Windows.Forms.RadioButton();
            this.UniqueRadioButton = new System.Windows.Forms.RadioButton();
            this.ReviewButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.ImportExportGroupBox = new System.Windows.Forms.GroupBox();
            this.ImportOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ImportRegDupAsTempRadioButton = new System.Windows.Forms.RadioButton();
            this.ImportRegDupNoneRadioButton = new System.Windows.Forms.RadioButton();
            this.ImportRegDupAsCreateNewBatchRadioButton = new System.Windows.Forms.RadioButton();
            this.ImportRegDupAsCreateNewRadioButton = new System.Windows.Forms.RadioButton();
            this.ImportButton = new System.Windows.Forms.Button();
            this.ImportTempRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.InvalidCountLabel = new System.Windows.Forms.Label();
            this.UniqueCountLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._UniqueLabel = new System.Windows.Forms.Label();
            this.ScanButton = new System.Windows.Forms.Button();
            this.TotalCountLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DuplicateCountLabel = new System.Windows.Forms.Label();
            this._DuplicateLabel = new System.Windows.Forms.Label();
            this.ImportFormToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.BeginImportButton = new System.Windows.Forms.Button();
            this.SaveMappingFileButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.ImportExportGroupBox.SuspendLayout();
            this.ImportOptionsGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AllRadioButton);
            this.panel1.Controls.Add(this.SelectedRecordsRadioButton);
            this.panel1.Controls.Add(this.InvalidRecordsRadioButton);
            this.panel1.Controls.Add(this.DuplicateRadioButton);
            this.panel1.Controls.Add(this.UniqueRadioButton);
            this.panel1.Location = new System.Drawing.Point(40, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 30);
            this.panel1.TabIndex = 7;
            this.ImportFormToolTip.SetToolTip(this.panel1, "Perform a duplicate check scan to enable this option");
            // 
            // AllRadioButton
            // 
            this.AllRadioButton.AutoSize = true;
            this.AllRadioButton.Location = new System.Drawing.Point(18, 7);
            this.AllRadioButton.Name = "AllRadioButton";
            this.AllRadioButton.Size = new System.Drawing.Size(79, 17);
            this.AllRadioButton.TabIndex = 1;
            this.AllRadioButton.TabStop = true;
            this.AllRadioButton.Text = "&All Records";
            this.ImportFormToolTip.SetToolTip(this.AllRadioButton, "Select to work with all records in the source file");
            this.AllRadioButton.UseVisualStyleBackColor = true;
            // 
            // SelectedRecordsRadioButton
            // 
            this.SelectedRecordsRadioButton.AutoSize = true;
            this.SelectedRecordsRadioButton.Enabled = false;
            this.SelectedRecordsRadioButton.Location = new System.Drawing.Point(336, 7);
            this.SelectedRecordsRadioButton.Name = "SelectedRecordsRadioButton";
            this.SelectedRecordsRadioButton.Size = new System.Drawing.Size(110, 17);
            this.SelectedRecordsRadioButton.TabIndex = 4;
            this.SelectedRecordsRadioButton.TabStop = true;
            this.SelectedRecordsRadioButton.Text = "&Selected Records";
            this.SelectedRecordsRadioButton.UseVisualStyleBackColor = true;
            // 
            // InvalidRecordsRadioButton
            // 
            this.InvalidRecordsRadioButton.AutoSize = true;
            this.InvalidRecordsRadioButton.Enabled = false;
            this.InvalidRecordsRadioButton.Location = new System.Drawing.Point(454, 7);
            this.InvalidRecordsRadioButton.Name = "InvalidRecordsRadioButton";
            this.InvalidRecordsRadioButton.Size = new System.Drawing.Size(99, 17);
            this.InvalidRecordsRadioButton.TabIndex = 5;
            this.InvalidRecordsRadioButton.TabStop = true;
            this.InvalidRecordsRadioButton.Text = "&Invalid Records";
            this.InvalidRecordsRadioButton.UseVisualStyleBackColor = true;
            // 
            // DuplicateRadioButton
            // 
            this.DuplicateRadioButton.AutoSize = true;
            this.DuplicateRadioButton.Enabled = false;
            this.DuplicateRadioButton.Location = new System.Drawing.Point(215, 7);
            this.DuplicateRadioButton.Name = "DuplicateRadioButton";
            this.DuplicateRadioButton.Size = new System.Drawing.Size(113, 17);
            this.DuplicateRadioButton.TabIndex = 3;
            this.DuplicateRadioButton.TabStop = true;
            this.DuplicateRadioButton.Text = "&Duplicate Records";
            this.ImportFormToolTip.SetToolTip(this.DuplicateRadioButton, "Perform a duplicate check scan to enable this option");
            this.DuplicateRadioButton.UseVisualStyleBackColor = true;
            // 
            // UniqueRadioButton
            // 
            this.UniqueRadioButton.AutoSize = true;
            this.UniqueRadioButton.Enabled = false;
            this.UniqueRadioButton.Location = new System.Drawing.Point(105, 7);
            this.UniqueRadioButton.Name = "UniqueRadioButton";
            this.UniqueRadioButton.Size = new System.Drawing.Size(102, 17);
            this.UniqueRadioButton.TabIndex = 2;
            this.UniqueRadioButton.TabStop = true;
            this.UniqueRadioButton.Text = "&Unique Records";
            this.ImportFormToolTip.SetToolTip(this.UniqueRadioButton, "Perform a duplicate check scan to enable this option");
            this.UniqueRadioButton.UseVisualStyleBackColor = true;
            // 
            // ReviewButton
            // 
            this.ReviewButton.AutoSize = true;
            this.ReviewButton.BackColor = System.Drawing.Color.LightGray;
            this.ReviewButton.Image = global::CambridgeSoft.DataLoaderGUI.Properties.Resources.Preview;
            this.ReviewButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ReviewButton.Location = new System.Drawing.Point(532, 179);
            this.ReviewButton.Name = "ReviewButton";
            this.ReviewButton.Size = new System.Drawing.Size(100, 30);
            this.ReviewButton.TabIndex = 13;
            this.ReviewButton.Text = "&View";
            this.ReviewButton.UseVisualStyleBackColor = false;
            this.ReviewButton.Click += new System.EventHandler(this.ReviewButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.AutoSize = true;
            this.ExportButton.BackColor = System.Drawing.Color.LightGray;
            this.ExportButton.Image = global::CambridgeSoft.DataLoaderGUI.Properties.Resources.Export_Data;
            this.ExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ExportButton.Location = new System.Drawing.Point(425, 179);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(100, 30);
            this.ExportButton.TabIndex = 12;
            this.ExportButton.Text = "&Export";
            this.ImportFormToolTip.SetToolTip(this.ExportButton, "Export the selected set of records");
            this.ExportButton.UseVisualStyleBackColor = false;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // ImportExportGroupBox
            // 
            this.ImportExportGroupBox.Controls.Add(this.ImportOptionsGroupBox);
            this.ImportExportGroupBox.Controls.Add(this.ReviewButton);
            this.ImportExportGroupBox.Controls.Add(this.panel1);
            this.ImportExportGroupBox.Controls.Add(this.ExportButton);
            this.ImportExportGroupBox.Location = new System.Drawing.Point(100, 150);
            this.ImportExportGroupBox.Name = "ImportExportGroupBox";
            this.ImportExportGroupBox.Size = new System.Drawing.Size(650, 215);
            this.ImportExportGroupBox.TabIndex = 39;
            this.ImportExportGroupBox.TabStop = false;
            this.ImportExportGroupBox.Text = "Import";
            // 
            // ImportOptionsGroupBox
            // 
            this.ImportOptionsGroupBox.Controls.Add(this.label2);
            this.ImportOptionsGroupBox.Controls.Add(this.label4);
            this.ImportOptionsGroupBox.Controls.Add(this.ImportRegDupAsTempRadioButton);
            this.ImportOptionsGroupBox.Controls.Add(this.ImportRegDupNoneRadioButton);
            this.ImportOptionsGroupBox.Controls.Add(this.ImportRegDupAsCreateNewBatchRadioButton);
            this.ImportOptionsGroupBox.Controls.Add(this.ImportRegDupAsCreateNewRadioButton);
            this.ImportOptionsGroupBox.Controls.Add(this.ImportButton);
            this.ImportOptionsGroupBox.Controls.Add(this.ImportTempRadioButton);
            this.ImportOptionsGroupBox.Location = new System.Drawing.Point(10, 45);
            this.ImportOptionsGroupBox.Name = "ImportOptionsGroupBox";
            this.ImportOptionsGroupBox.Size = new System.Drawing.Size(630, 125);
            this.ImportOptionsGroupBox.TabIndex = 43;
            this.ImportOptionsGroupBox.TabStop = false;
            this.ImportOptionsGroupBox.Text = "Import Options";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Temporary Registration";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Final Registration";
            // 
            // ImportRegDupAsTempRadioButton
            // 
            this.ImportRegDupAsTempRadioButton.AutoSize = true;
            this.ImportRegDupAsTempRadioButton.Location = new System.Drawing.Point(346, 55);
            this.ImportRegDupAsTempRadioButton.Name = "ImportRegDupAsTempRadioButton";
            this.ImportRegDupAsTempRadioButton.Size = new System.Drawing.Size(171, 17);
            this.ImportRegDupAsTempRadioButton.TabIndex = 8;
            this.ImportRegDupAsTempRadioButton.TabStop = true;
            this.ImportRegDupAsTempRadioButton.Text = "Duplicates as temporary record";
            this.ImportRegDupAsTempRadioButton.UseVisualStyleBackColor = true;
            // 
            // ImportRegDupNoneRadioButton
            // 
            this.ImportRegDupNoneRadioButton.AutoSize = true;
            this.ImportRegDupNoneRadioButton.Location = new System.Drawing.Point(346, 80);
            this.ImportRegDupNoneRadioButton.Name = "ImportRegDupNoneRadioButton";
            this.ImportRegDupNoneRadioButton.Size = new System.Drawing.Size(122, 17);
            this.ImportRegDupNoneRadioButton.TabIndex = 10;
            this.ImportRegDupNoneRadioButton.TabStop = true;
            this.ImportRegDupNoneRadioButton.Text = "Duplicates no action";
            this.ImportRegDupNoneRadioButton.UseVisualStyleBackColor = true;
            // 
            // ImportRegDupAsCreateNewBatchRadioButton
            // 
            this.ImportRegDupAsCreateNewBatchRadioButton.AutoSize = true;
            this.ImportRegDupAsCreateNewBatchRadioButton.Location = new System.Drawing.Point(180, 53);
            this.ImportRegDupAsCreateNewBatchRadioButton.Name = "ImportRegDupAsCreateNewBatchRadioButton";
            this.ImportRegDupAsCreateNewBatchRadioButton.Size = new System.Drawing.Size(142, 17);
            this.ImportRegDupAsCreateNewBatchRadioButton.TabIndex = 7;
            this.ImportRegDupAsCreateNewBatchRadioButton.TabStop = true;
            this.ImportRegDupAsCreateNewBatchRadioButton.Text = "Duplicates as new batch";
            this.ImportRegDupAsCreateNewBatchRadioButton.UseVisualStyleBackColor = true;
            // 
            // ImportRegDupAsCreateNewRadioButton
            // 
            this.ImportRegDupAsCreateNewRadioButton.AutoSize = true;
            this.ImportRegDupAsCreateNewRadioButton.Location = new System.Drawing.Point(180, 78);
            this.ImportRegDupAsCreateNewRadioButton.Name = "ImportRegDupAsCreateNewRadioButton";
            this.ImportRegDupAsCreateNewRadioButton.Size = new System.Drawing.Size(145, 17);
            this.ImportRegDupAsCreateNewRadioButton.TabIndex = 9;
            this.ImportRegDupAsCreateNewRadioButton.TabStop = true;
            this.ImportRegDupAsCreateNewRadioButton.Text = "Duplicates as new record";
            this.ImportRegDupAsCreateNewRadioButton.UseVisualStyleBackColor = true;
            // 
            // ImportButton
            // 
            this.ImportButton.AutoSize = true;
            this.ImportButton.BackColor = System.Drawing.Color.LightGray;
            this.ImportButton.Image = global::CambridgeSoft.DataLoaderGUI.Properties.Resources.Import_Data;
            this.ImportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ImportButton.Location = new System.Drawing.Point(522, 89);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(100, 30);
            this.ImportButton.TabIndex = 11;
            this.ImportButton.Text = "&Import";
            this.ImportFormToolTip.SetToolTip(this.ImportButton, "Import the selected set of records");
            this.ImportButton.UseVisualStyleBackColor = false;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // ImportTempRadioButton
            // 
            this.ImportTempRadioButton.AutoSize = true;
            this.ImportTempRadioButton.Location = new System.Drawing.Point(180, 18);
            this.ImportTempRadioButton.Name = "ImportTempRadioButton";
            this.ImportTempRadioButton.Size = new System.Drawing.Size(106, 17);
            this.ImportTempRadioButton.TabIndex = 6;
            this.ImportTempRadioButton.TabStop = true;
            this.ImportTempRadioButton.Text = "Submit for review";
            this.ImportTempRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.InvalidCountLabel);
            this.groupBox2.Controls.Add(this.UniqueCountLabel);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this._UniqueLabel);
            this.groupBox2.Controls.Add(this.ScanButton);
            this.groupBox2.Controls.Add(this.TotalCountLabel);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.DuplicateCountLabel);
            this.groupBox2.Controls.Add(this._DuplicateLabel);
            this.groupBox2.Location = new System.Drawing.Point(100, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(650, 100);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Duplicate Check";
            // 
            // InvalidCountLabel
            // 
            this.InvalidCountLabel.AutoSize = true;
            this.InvalidCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InvalidCountLabel.Location = new System.Drawing.Point(439, 20);
            this.InvalidCountLabel.Name = "InvalidCountLabel";
            this.InvalidCountLabel.Size = new System.Drawing.Size(15, 16);
            this.InvalidCountLabel.TabIndex = 6;
            this.InvalidCountLabel.Text = "0";
            // 
            // UniqueCountLabel
            // 
            this.UniqueCountLabel.AutoSize = true;
            this.UniqueCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UniqueCountLabel.Location = new System.Drawing.Point(439, 50);
            this.UniqueCountLabel.Name = "UniqueCountLabel";
            this.UniqueCountLabel.Size = new System.Drawing.Size(15, 16);
            this.UniqueCountLabel.TabIndex = 6;
            this.UniqueCountLabel.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(320, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Invalid Records : ";
            // 
            // _UniqueLabel
            // 
            this._UniqueLabel.AutoSize = true;
            this._UniqueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._UniqueLabel.Location = new System.Drawing.Point(320, 50);
            this._UniqueLabel.Name = "_UniqueLabel";
            this._UniqueLabel.Size = new System.Drawing.Size(60, 16);
            this._UniqueLabel.TabIndex = 5;
            this._UniqueLabel.Text = "Unique : ";
            // 
            // ScanButton
            // 
            this.ScanButton.AutoSize = true;
            this.ScanButton.BackColor = System.Drawing.Color.LightGray;
            this.ScanButton.Image = global::CambridgeSoft.DataLoaderGUI.Properties.Resources.Preview;
            this.ScanButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ScanButton.Location = new System.Drawing.Point(532, 60);
            this.ScanButton.Name = "ScanButton";
            this.ScanButton.Size = new System.Drawing.Size(100, 30);
            this.ScanButton.TabIndex = 0;
            this.ScanButton.Text = "&Scan";
            this.ScanButton.UseVisualStyleBackColor = false;
            this.ScanButton.Click += new System.EventHandler(this.ScanButton_Click);
            // 
            // TotalCountLabel
            // 
            this.TotalCountLabel.AutoSize = true;
            this.TotalCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalCountLabel.Location = new System.Drawing.Point(131, 20);
            this.TotalCountLabel.Name = "TotalCountLabel";
            this.TotalCountLabel.Size = new System.Drawing.Size(15, 16);
            this.TotalCountLabel.TabIndex = 2;
            this.TotalCountLabel.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Total Records :";
            // 
            // DuplicateCountLabel
            // 
            this.DuplicateCountLabel.AutoSize = true;
            this.DuplicateCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DuplicateCountLabel.Location = new System.Drawing.Point(131, 50);
            this.DuplicateCountLabel.Name = "DuplicateCountLabel";
            this.DuplicateCountLabel.Size = new System.Drawing.Size(15, 16);
            this.DuplicateCountLabel.TabIndex = 2;
            this.DuplicateCountLabel.Text = "0";
            // 
            // _DuplicateLabel
            // 
            this._DuplicateLabel.AutoSize = true;
            this._DuplicateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._DuplicateLabel.Location = new System.Drawing.Point(20, 50);
            this._DuplicateLabel.Name = "_DuplicateLabel";
            this._DuplicateLabel.Size = new System.Drawing.Size(74, 16);
            this._DuplicateLabel.TabIndex = 3;
            this._DuplicateLabel.Text = "Duplicate : ";
            // 
            // ImportFormToolTip
            // 
            this.ImportFormToolTip.ShowAlways = true;
            // 
            // BeginImportButton
            // 
            this.BeginImportButton.BackColor = System.Drawing.Color.LightGray;
            this.BeginImportButton.Image = global::CambridgeSoft.DataLoaderGUI.Properties.Resources.Import_Data;
            this.BeginImportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BeginImportButton.Location = new System.Drawing.Point(228, 380);
            this.BeginImportButton.Name = "BeginImportButton";
            this.BeginImportButton.Size = new System.Drawing.Size(120, 26);
            this.BeginImportButton.TabIndex = 15;
            this.BeginImportButton.Text = "Import Another File";
            this.BeginImportButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BeginImportButton.UseVisualStyleBackColor = false;
            this.BeginImportButton.Click += new System.EventHandler(this.BeginImportButton_Click);
            // 
            // SaveMappingFileButton
            // 
            this.SaveMappingFileButton.BackColor = System.Drawing.Color.LightGray;
            this.SaveMappingFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SaveMappingFileButton.Location = new System.Drawing.Point(100, 380);
            this.SaveMappingFileButton.Name = "SaveMappingFileButton";
            this.SaveMappingFileButton.Size = new System.Drawing.Size(120, 26);
            this.SaveMappingFileButton.TabIndex = 14;
            this.SaveMappingFileButton.Text = "Save Mapping File";
            this.SaveMappingFileButton.UseVisualStyleBackColor = false;
            this.SaveMappingFileButton.Click += new System.EventHandler(this.SaveMappingFileButton_Click);
            // 
            // ImportAndDuplicateCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ImportExportGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.SaveMappingFileButton);
            this.Controls.Add(this.BeginImportButton);
            this.Name = "ImportAndDuplicateCheck";
            this.Size = new System.Drawing.Size(850, 420);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ImportExportGroupBox.ResumeLayout(false);
            this.ImportExportGroupBox.PerformLayout();
            this.ImportOptionsGroupBox.ResumeLayout(false);
            this.ImportOptionsGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.GroupBox ImportExportGroupBox;
        public System.Windows.Forms.Button ReviewButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label InvalidCountLabel;
        private System.Windows.Forms.Label UniqueCountLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label _UniqueLabel;
        private System.Windows.Forms.Label TotalCountLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DuplicateCountLabel;
        private System.Windows.Forms.Label _DuplicateLabel;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton AllRadioButton;
        private System.Windows.Forms.RadioButton DuplicateRadioButton;
        public System.Windows.Forms.Button ScanButton;
        private System.Windows.Forms.ToolTip ImportFormToolTip;
        public System.Windows.Forms.Button BeginImportButton;
        private System.Windows.Forms.RadioButton UniqueRadioButton;
        private System.Windows.Forms.GroupBox ImportOptionsGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton ImportRegDupAsTempRadioButton;
        private System.Windows.Forms.RadioButton ImportRegDupNoneRadioButton;
        private System.Windows.Forms.RadioButton ImportRegDupAsCreateNewBatchRadioButton;
        private System.Windows.Forms.RadioButton ImportRegDupAsCreateNewRadioButton;
        public System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.RadioButton ImportTempRadioButton;
        internal System.Windows.Forms.RadioButton SelectedRecordsRadioButton;
        public System.Windows.Forms.Button SaveMappingFileButton;
        private System.Windows.Forms.RadioButton InvalidRecordsRadioButton;
    }
}
