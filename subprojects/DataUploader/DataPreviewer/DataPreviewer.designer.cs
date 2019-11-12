namespace DataPreviewer
{
    partial class DataPreviewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPreviewer));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsBtn_SetFolder = new System.Windows.Forms.ToolStripButton();
            this.tsCbo_Files = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtn_Start = new System.Windows.Forms.ToolStripButton();
            this.tsBtn_Cancel = new System.Windows.Forms.ToolStripButton();
            this.tsBtn_Export = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtn_CountAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsBtn_ParseSpecific = new System.Windows.Forms.ToolStripButton();
            this.tsText_Indices = new System.Windows.Forms.ToolStripTextBox();
            this.tsBtn_ExportSpecific = new System.Windows.Forms.ToolStripButton();
            this.dgvParsedData = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showObjectDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recreateSDRecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.rtbParsedStructure = new System.Windows.Forms.RichTextBox();
            this.splitVertical = new System.Windows.Forms.SplitContainer();
            this.splitHRight = new System.Windows.Forms.SplitContainer();
            this.axChemDrawCtl2 = new AxChemDrawControl19.AxChemDrawCtl();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParsedData)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.splitVertical.Panel1.SuspendLayout();
            this.splitVertical.Panel2.SuspendLayout();
            this.splitVertical.SuspendLayout();
            this.splitHRight.Panel1.SuspendLayout();
            this.splitHRight.Panel2.SuspendLayout();
            this.splitHRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axChemDrawCtl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusProgress,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 360);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(889, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusProgress
            // 
            this.statusProgress.Name = "statusProgress";
            this.statusProgress.Size = new System.Drawing.Size(200, 16);
            // 
            // statusLabel
            // 
            this.statusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(94, 17);
            this.statusLabel.Text = "Choose an action:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtn_SetFolder,
            this.tsCbo_Files,
            this.toolStripSeparator3,
            this.tsBtn_Start,
            this.tsBtn_Cancel,
            this.tsBtn_Export,
            this.toolStripSeparator2,
            this.tsBtn_CountAll,
            this.toolStripSeparator1,
            this.tsBtn_ParseSpecific,
            this.tsText_Indices,
            this.tsBtn_ExportSpecific});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(889, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsBtn_SetFolder
            // 
            this.tsBtn_SetFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_SetFolder.Name = "tsBtn_SetFolder";
            this.tsBtn_SetFolder.Size = new System.Drawing.Size(78, 22);
            this.tsBtn_SetFolder.Text = "Choose folder";
            this.tsBtn_SetFolder.Click += new System.EventHandler(this.tsBtn_SetFolder_Click);
            // 
            // tsCbo_Files
            // 
            this.tsCbo_Files.Name = "tsCbo_Files";
            this.tsCbo_Files.Size = new System.Drawing.Size(250, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtn_Start
            // 
            this.tsBtn_Start.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_Start.Name = "tsBtn_Start";
            this.tsBtn_Start.Size = new System.Drawing.Size(35, 22);
            this.tsBtn_Start.Text = "&Start";
            this.tsBtn_Start.Click += new System.EventHandler(this.tsBtn_Start_Click);
            // 
            // tsBtn_Cancel
            // 
            this.tsBtn_Cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_Cancel.Name = "tsBtn_Cancel";
            this.tsBtn_Cancel.Size = new System.Drawing.Size(43, 22);
            this.tsBtn_Cancel.Text = "&Cancel";
            this.tsBtn_Cancel.Click += new System.EventHandler(this.tsBtn_Cancel_Click);
            // 
            // tsBtn_Export
            // 
            this.tsBtn_Export.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtn_Export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_Export.Name = "tsBtn_Export";
            this.tsBtn_Export.Size = new System.Drawing.Size(81, 22);
            this.tsBtn_Export.Text = "&Export (to file)";
            this.tsBtn_Export.Click += new System.EventHandler(this.tsBtn_Export_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtn_CountAll
            // 
            this.tsBtn_CountAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtn_CountAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_CountAll.Name = "tsBtn_CountAll";
            this.tsBtn_CountAll.Size = new System.Drawing.Size(104, 22);
            this.tsBtn_CountAll.Text = "Count &total records";
            this.tsBtn_CountAll.Click += new System.EventHandler(this.tsBtn_CountAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsBtn_ParseSpecific
            // 
            this.tsBtn_ParseSpecific.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtn_ParseSpecific.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_ParseSpecific.Name = "tsBtn_ParseSpecific";
            this.tsBtn_ParseSpecific.Size = new System.Drawing.Size(111, 22);
            this.tsBtn_ParseSpecific.Text = "Parse speci&fic indices";
            this.tsBtn_ParseSpecific.Click += new System.EventHandler(this.tsBtn_ParseSpecific_Click);
            // 
            // tsText_Indices
            // 
            this.tsText_Indices.Name = "tsText_Indices";
            this.tsText_Indices.Size = new System.Drawing.Size(65, 25);
            // 
            // tsBtn_ExportSpecific
            // 
            this.tsBtn_ExportSpecific.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtn_ExportSpecific.Image = ((System.Drawing.Image)(resources.GetObject("tsBtn_ExportSpecific.Image")));
            this.tsBtn_ExportSpecific.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtn_ExportSpecific.Name = "tsBtn_ExportSpecific";
            this.tsBtn_ExportSpecific.Size = new System.Drawing.Size(105, 17);
            this.tsBtn_ExportSpecific.Text = "Export (to textbox)";
            this.tsBtn_ExportSpecific.Click += new System.EventHandler(this.tsBtn_ExportSpecific_Click);
            // 
            // dgvParsedData
            // 
            this.dgvParsedData.AllowUserToAddRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.LightCyan;
            this.dgvParsedData.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvParsedData.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvParsedData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParsedData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvParsedData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParsedData.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvParsedData.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvParsedData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParsedData.Location = new System.Drawing.Point(0, 0);
            this.dgvParsedData.Name = "dgvParsedData";
            this.dgvParsedData.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParsedData.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewCellStyle10.Format = "E2";
            dataGridViewCellStyle10.NullValue = null;
            this.dgvParsedData.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvParsedData.Size = new System.Drawing.Size(580, 335);
            this.dgvParsedData.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showRecordToolStripMenuItem,
            this.showObjectDataToolStripMenuItem,
            this.recreateSDRecToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(171, 70);
            // 
            // showRecordToolStripMenuItem
            // 
            this.showRecordToolStripMenuItem.Name = "showRecordToolStripMenuItem";
            this.showRecordToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.showRecordToolStripMenuItem.Text = "Show record data";
            this.showRecordToolStripMenuItem.Click += new System.EventHandler(this.showRecordToolStripMenuItem_Click);
            // 
            // showObjectDataToolStripMenuItem
            // 
            this.showObjectDataToolStripMenuItem.Name = "showObjectDataToolStripMenuItem";
            this.showObjectDataToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.showObjectDataToolStripMenuItem.Text = "Show object data";
            this.showObjectDataToolStripMenuItem.Click += new System.EventHandler(this.showObjectDataToolStripMenuItem_Click);
            // 
            // recreateSDRecToolStripMenuItem
            // 
            this.recreateSDRecToolStripMenuItem.Name = "recreateSDRecToolStripMenuItem";
            this.recreateSDRecToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.recreateSDRecToolStripMenuItem.Text = "Recreate SD Rec";
            this.recreateSDRecToolStripMenuItem.Click += new System.EventHandler(this.recreateSDRecToolStripMenuItem_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // rtbParsedStructure
            // 
            this.rtbParsedStructure.BackColor = System.Drawing.Color.White;
            this.rtbParsedStructure.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbParsedStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbParsedStructure.Location = new System.Drawing.Point(0, 0);
            this.rtbParsedStructure.Name = "rtbParsedStructure";
            this.rtbParsedStructure.ReadOnly = true;
            this.rtbParsedStructure.Size = new System.Drawing.Size(305, 89);
            this.rtbParsedStructure.TabIndex = 4;
            this.rtbParsedStructure.Text = "";
            // 
            // splitVertical
            // 
            this.splitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitVertical.Location = new System.Drawing.Point(0, 25);
            this.splitVertical.Name = "splitVertical";
            // 
            // splitVertical.Panel1
            // 
            this.splitVertical.Panel1.Controls.Add(this.dgvParsedData);
            // 
            // splitVertical.Panel2
            // 
            this.splitVertical.Panel2.Controls.Add(this.splitHRight);
            this.splitVertical.Size = new System.Drawing.Size(889, 335);
            this.splitVertical.SplitterDistance = 580;
            this.splitVertical.TabIndex = 5;
            // 
            // splitHRight
            // 
            this.splitHRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitHRight.Location = new System.Drawing.Point(0, 0);
            this.splitHRight.Name = "splitHRight";
            this.splitHRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitHRight.Panel1
            // 
            this.splitHRight.Panel1.Controls.Add(this.axChemDrawCtl2);
            // 
            // splitHRight.Panel2
            // 
            this.splitHRight.Panel2.Controls.Add(this.rtbParsedStructure);
            this.splitHRight.Size = new System.Drawing.Size(305, 335);
            this.splitHRight.SplitterDistance = 242;
            this.splitHRight.TabIndex = 5;
            // 
            // axChemDrawCtl2
            // 
            this.axChemDrawCtl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axChemDrawCtl2.Enabled = true;
            this.axChemDrawCtl2.Location = new System.Drawing.Point(0, 0);
            this.axChemDrawCtl2.Name = "axChemDrawCtl2";
            this.axChemDrawCtl2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axChemDrawCtl2.OcxState")));
            this.axChemDrawCtl2.Size = new System.Drawing.Size(305, 242);
            this.axChemDrawCtl2.TabIndex = 0;
            // 
            // DataPreviewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 382);
            this.Controls.Add(this.splitVertical);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "DataPreviewer";
            this.Text = "SDFile Previewer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParsedData)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitVertical.Panel1.ResumeLayout(false);
            this.splitVertical.Panel2.ResumeLayout(false);
            this.splitVertical.ResumeLayout(false);
            this.splitHRight.Panel1.ResumeLayout(false);
            this.splitHRight.Panel2.ResumeLayout(false);
            this.splitHRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axChemDrawCtl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsBtn_Start;
        private System.Windows.Forms.DataGridView dgvParsedData;
        private System.Windows.Forms.ToolStripButton tsBtn_Cancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.RichTextBox rtbParsedStructure;
        private System.Windows.Forms.SplitContainer splitVertical;
        private System.Windows.Forms.SplitContainer splitHRight;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showObjectDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsBtn_CountAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsBtn_ParseSpecific;
        private System.Windows.Forms.ToolStripMenuItem recreateSDRecToolStripMenuItem;
        private AxChemDrawControl19.AxChemDrawCtl axChemDrawCtl2;
        private System.Windows.Forms.ToolStripTextBox tsText_Indices;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox tsCbo_Files;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsBtn_SetFolder;
        private System.Windows.Forms.ToolStripButton tsBtn_ExportSpecific;
        private System.Windows.Forms.ToolStripButton tsBtn_Export;
    }
}