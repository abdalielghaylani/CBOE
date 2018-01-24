namespace COELogViewer
{
    partial class COELogViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(COELogViewerForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.OpenButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshButton = new System.Windows.Forms.ToolStripButton();
            this.ColumnNamesComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.LogDataGridView = new System.Windows.Forms.DataGridView();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.LogEntriesStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.FilterClauseTextCombo = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogDataGridView)).BeginInit();
            this.StatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenButton,
            this.RefreshButton,
            this.ColumnNamesComboBox,
            this.FilterClauseTextCombo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(666, 36);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // OpenButton
            // 
            this.OpenButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenButton.Image")));
            this.OpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(57, 33);
            this.OpenButton.Text = "Open Log";
            this.OpenButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Image = ((System.Drawing.Image)(resources.GetObject("RefreshButton.Image")));
            this.RefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(49, 33);
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // ColumnNamesComboBox
            // 
            this.ColumnNamesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnNamesComboBox.DropDownWidth = 200;
            this.ColumnNamesComboBox.Items.AddRange(new object[] {
            "Choose a column:"});
            this.ColumnNamesComboBox.Name = "ColumnNamesComboBox";
            this.ColumnNamesComboBox.Size = new System.Drawing.Size(121, 36);
            this.ColumnNamesComboBox.SelectedIndexChanged += new System.EventHandler(this.ColumnNamesComboBox_SelectedIndexChanged);
            // 
            // LogDataGridView
            // 
            this.LogDataGridView.AllowUserToAddRows = false;
            this.LogDataGridView.AllowUserToDeleteRows = false;
            this.LogDataGridView.AllowUserToOrderColumns = true;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.LogDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.LogDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LogDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.LogDataGridView.DefaultCellStyle = dataGridViewCellStyle8;
            this.LogDataGridView.Location = new System.Drawing.Point(0, 36);
            this.LogDataGridView.Name = "LogDataGridView";
            this.LogDataGridView.ReadOnly = true;
            this.LogDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.LogDataGridView.Size = new System.Drawing.Size(666, 385);
            this.LogDataGridView.TabIndex = 1;
            this.LogDataGridView.VirtualMode = true;
            this.LogDataGridView.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.LogDataGridView_CellMouseEnter);
            this.LogDataGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.LogDataGridView_CellMouseDoubleClick);
            this.LogDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.LogDataGridView_CellEnter);
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LogEntriesStatusLabel});
            this.StatusBar.Location = new System.Drawing.Point(0, 424);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(666, 22);
            this.StatusBar.TabIndex = 2;
            this.StatusBar.Text = "StatusBar";
            // 
            // LogEntriesStatusLabel
            // 
            this.LogEntriesStatusLabel.Name = "LogEntriesStatusLabel";
            this.LogEntriesStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // FilterClauseTextCombo
            // 
            this.FilterClauseTextCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FilterClauseTextCombo.DropDownWidth = 300;
            this.FilterClauseTextCombo.Name = "FilterClauseTextCombo";
            this.FilterClauseTextCombo.Size = new System.Drawing.Size(100, 36);
            this.FilterClauseTextCombo.TextChanged += new System.EventHandler(this.FilterClauseTextBox_TextChanged);
            // 
            // COELogViewerForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 446);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.LogDataGridView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "COELogViewerForm";
            this.Text = "COELogViewerForm";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.COELogViewerForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.COELogViewerForm_DragEnter);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogDataGridView)).EndInit();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton OpenButton;
        private System.Windows.Forms.ToolStripButton RefreshButton;
        private System.Windows.Forms.DataGridView LogDataGridView;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel LogEntriesStatusLabel;
        private System.Windows.Forms.ToolStripComboBox ColumnNamesComboBox;
        private System.Windows.Forms.ToolStripComboBox FilterClauseTextCombo;


    }
}