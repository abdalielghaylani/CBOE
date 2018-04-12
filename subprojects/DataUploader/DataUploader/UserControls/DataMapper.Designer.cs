namespace CambridgeSoft.COE.DataLoader.UserControls
{
    partial class DataMapper
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
            this._configurationTabControl = new System.Windows.Forms.TabControl();
            this._registryTabPage = new System.Windows.Forms.TabPage();
            this._registryDataGridView = new System.Windows.Forms.DataGridView();
            this._componentTabPage = new System.Windows.Forms.TabPage();
            this._componentDataGridView = new System.Windows.Forms.DataGridView();
            this._batchTabPage = new System.Windows.Forms.TabPage();
            this._batchDataGridView = new System.Windows.Forms.DataGridView();
            this._batchComponentTabPage = new System.Windows.Forms.TabPage();
            this._batchComponentDataGridView = new System.Windows.Forms.DataGridView();
            this._registryDFColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._registryVFColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._registryTColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._registryValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._batchComponentDFColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._batchComponentVFColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._batchComponentTColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._batchComponentValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._batchDFColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._batchVFColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._batchTColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._batchValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._componentDFColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._componentVFColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._componentTColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._componentValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._configurationTabControl.SuspendLayout();
            this._registryTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._registryDataGridView)).BeginInit();
            this._componentTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._componentDataGridView)).BeginInit();
            this._batchTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._batchDataGridView)).BeginInit();
            this._batchComponentTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._batchComponentDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _configurationTabControl
            // 
            this._configurationTabControl.Controls.Add(this._registryTabPage);
            this._configurationTabControl.Controls.Add(this._componentTabPage);
            this._configurationTabControl.Controls.Add(this._batchTabPage);
            this._configurationTabControl.Controls.Add(this._batchComponentTabPage);
            this._configurationTabControl.Location = new System.Drawing.Point(8, 8);
            this._configurationTabControl.Name = "_configurationTabControl";
            this._configurationTabControl.SelectedIndex = 0;
            this._configurationTabControl.Size = new System.Drawing.Size(484, 364);
            this._configurationTabControl.TabIndex = 0;
            // 
            // _registryTabPage
            // 
            this._registryTabPage.BackColor = System.Drawing.Color.White;
            this._registryTabPage.Controls.Add(this._registryDataGridView);
            this._registryTabPage.Location = new System.Drawing.Point(4, 22);
            this._registryTabPage.Name = "_registryTabPage";
            this._registryTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._registryTabPage.Size = new System.Drawing.Size(476, 338);
            this._registryTabPage.TabIndex = 0;
            this._registryTabPage.Text = "Registry";
            this._registryTabPage.UseVisualStyleBackColor = true;
            // 
            // _registryDataGridView
            // 
            this._registryDataGridView.AllowUserToResizeRows = false;
            this._registryDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._registryDataGridView.BackgroundColor = System.Drawing.SystemColors.InactiveCaptionText;
            this._registryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._registryDFColumn,
            this._registryVFColumn,
            this._registryTColumn,
            this._registryValueColumn});
            this._registryDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._registryDataGridView.Location = new System.Drawing.Point(3, 3);
            this._registryDataGridView.Name = "_registryDataGridView";
            this._registryDataGridView.RowHeadersVisible = false;
            this._registryDataGridView.RowHeadersWidth = 56;
            this._registryDataGridView.Size = new System.Drawing.Size(470, 332);
            this._registryDataGridView.TabIndex = 0;
            // 
            // _componentTabPage
            // 
            this._componentTabPage.BackColor = System.Drawing.Color.White;
            this._componentTabPage.Controls.Add(this._componentDataGridView);
            this._componentTabPage.Location = new System.Drawing.Point(4, 22);
            this._componentTabPage.Name = "_componentTabPage";
            this._componentTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._componentTabPage.Size = new System.Drawing.Size(476, 338);
            this._componentTabPage.TabIndex = 1;
            this._componentTabPage.Text = "Component";
            this._componentTabPage.UseVisualStyleBackColor = true;
            // 
            // _componentDataGridView
            // 
            this._componentDataGridView.AllowUserToResizeRows = false;
            this._componentDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._componentDataGridView.BackgroundColor = System.Drawing.SystemColors.InactiveCaptionText;
            this._componentDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._componentDFColumn,
            this._componentVFColumn,
            this._componentTColumn,
            this._componentValueColumn});
            this._componentDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._componentDataGridView.Location = new System.Drawing.Point(3, 3);
            this._componentDataGridView.Name = "_componentDataGridView";
            this._componentDataGridView.RowHeadersVisible = false;
            this._componentDataGridView.Size = new System.Drawing.Size(470, 332);
            this._componentDataGridView.TabIndex = 1;
            // 
            // _batchTabPage
            // 
            this._batchTabPage.Controls.Add(this._batchDataGridView);
            this._batchTabPage.Location = new System.Drawing.Point(4, 22);
            this._batchTabPage.Name = "_batchTabPage";
            this._batchTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._batchTabPage.Size = new System.Drawing.Size(476, 338);
            this._batchTabPage.TabIndex = 2;
            this._batchTabPage.Text = "Batch";
            this._batchTabPage.UseVisualStyleBackColor = true;
            // 
            // _batchDataGridView
            // 
            this._batchDataGridView.AllowUserToResizeRows = false;
            this._batchDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._batchDataGridView.BackgroundColor = System.Drawing.SystemColors.InactiveCaptionText;
            this._batchDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._batchDFColumn,
            this._batchVFColumn,
            this._batchTColumn,
            this._batchValueColumn});
            this._batchDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._batchDataGridView.Location = new System.Drawing.Point(3, 3);
            this._batchDataGridView.Name = "_batchDataGridView";
            this._batchDataGridView.RowHeadersVisible = false;
            this._batchDataGridView.Size = new System.Drawing.Size(470, 332);
            this._batchDataGridView.TabIndex = 1;
            // 
            // _batchComponentTabPage
            // 
            this._batchComponentTabPage.Controls.Add(this._batchComponentDataGridView);
            this._batchComponentTabPage.Location = new System.Drawing.Point(4, 22);
            this._batchComponentTabPage.Name = "_batchComponentTabPage";
            this._batchComponentTabPage.Size = new System.Drawing.Size(476, 338);
            this._batchComponentTabPage.TabIndex = 3;
            this._batchComponentTabPage.Text = "Batch Component";
            this._batchComponentTabPage.UseVisualStyleBackColor = true;
            // 
            // _batchComponentDataGridView
            // 
            this._batchComponentDataGridView.AllowUserToResizeRows = false;
            this._batchComponentDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._batchComponentDataGridView.BackgroundColor = System.Drawing.SystemColors.InactiveCaptionText;
            this._batchComponentDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._batchComponentDFColumn,
            this._batchComponentVFColumn,
            this._batchComponentTColumn,
            this._batchComponentValueColumn});
            this._batchComponentDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._batchComponentDataGridView.Location = new System.Drawing.Point(0, 0);
            this._batchComponentDataGridView.Name = "_batchComponentDataGridView";
            this._batchComponentDataGridView.RowHeadersVisible = false;
            this._batchComponentDataGridView.Size = new System.Drawing.Size(476, 338);
            this._batchComponentDataGridView.TabIndex = 2;
            // 
            // _registryDFColumn
            // 
            this._registryDFColumn.FillWeight = 96.7216F;
            this._registryDFColumn.HeaderText = "Destination Field";
            this._registryDFColumn.Name = "_registryDFColumn";
            // 
            // _registryVFColumn
            // 
            this._registryVFColumn.FillWeight = 69.56053F;
            this._registryVFColumn.HeaderText = "Value From";
            this._registryVFColumn.Items.AddRange(new object[] {
            "Input field",
            "Constant"});
            this._registryVFColumn.Name = "_registryVFColumn";
            // 
            // _registryTColumn
            // 
            this._registryTColumn.FillWeight = 50.00225F;
            this._registryTColumn.HeaderText = "Type";
            this._registryTColumn.Name = "_registryTColumn";
            // 
            // _registryValueColumn
            // 
            this._registryValueColumn.FillWeight = 175.2421F;
            this._registryValueColumn.HeaderText = "Value";
            this._registryValueColumn.Name = "_registryValueColumn";
            // 
            // _batchComponentDFColumn
            // 
            this._batchComponentDFColumn.FillWeight = 113.0742F;
            this._batchComponentDFColumn.HeaderText = "Destination Field";
            this._batchComponentDFColumn.Name = "_batchComponentDFColumn";
            // 
            // _batchComponentVFColumn
            // 
            this._batchComponentVFColumn.FillWeight = 73.28988F;
            this._batchComponentVFColumn.HeaderText = "Value From";
            this._batchComponentVFColumn.Items.AddRange(new object[] {
            "Imput field",
            "Constant"});
            this._batchComponentVFColumn.Name = "_batchComponentVFColumn";
            // 
            // _batchComponentTColumn
            // 
            this._batchComponentTColumn.FillWeight = 50.76142F;
            this._batchComponentTColumn.HeaderText = "Type";
            this._batchComponentTColumn.Name = "_batchComponentTColumn";
            // 
            // _batchComponentValueColumn
            // 
            this._batchComponentValueColumn.FillWeight = 162.8745F;
            this._batchComponentValueColumn.HeaderText = "Value";
            this._batchComponentValueColumn.Name = "_batchComponentValueColumn";
            // 
            // _batchDFColumn
            // 
            this._batchDFColumn.FillWeight = 106.7079F;
            this._batchDFColumn.HeaderText = "Destination Field";
            this._batchDFColumn.Name = "_batchDFColumn";
            // 
            // _batchVFColumn
            // 
            this._batchVFColumn.FillWeight = 72.62617F;
            this._batchVFColumn.HeaderText = "Value From";
            this._batchVFColumn.Items.AddRange(new object[] {
            "Input field",
            "Constant"});
            this._batchVFColumn.Name = "_batchVFColumn";
            // 
            // _batchTColumn
            // 
            this._batchTColumn.FillWeight = 50.76143F;
            this._batchTColumn.HeaderText = "Type";
            this._batchTColumn.Name = "_batchTColumn";
            // 
            // _batchValueColumn
            // 
            this._batchValueColumn.FillWeight = 169.9046F;
            this._batchValueColumn.HeaderText = "Value";
            this._batchValueColumn.Name = "_batchValueColumn";
            // 
            // _componentDFColumn
            // 
            this._componentDFColumn.FillWeight = 104.7558F;
            this._componentDFColumn.HeaderText = "Destination Field";
            this._componentDFColumn.Name = "_componentDFColumn";
            // 
            // _componentVFColumn
            // 
            this._componentVFColumn.FillWeight = 71.7381F;
            this._componentVFColumn.HeaderText = "Value From";
            this._componentVFColumn.Items.AddRange(new object[] {
            "Input field",
            "Constant"});
            this._componentVFColumn.Name = "_componentVFColumn";
            // 
            // _componentTColumn
            // 
            this._componentTColumn.FillWeight = 50.76141F;
            this._componentTColumn.HeaderText = "Type";
            this._componentTColumn.Name = "_componentTColumn";
            // 
            // _componentValueColumn
            // 
            this._componentValueColumn.FillWeight = 172.7447F;
            this._componentValueColumn.HeaderText = "Value";
            this._componentValueColumn.Name = "_componentValueColumn";
            // 
            // DataMapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._configurationTabControl);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "DataMapper";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(500, 380);
            this._configurationTabControl.ResumeLayout(false);
            this._registryTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._registryDataGridView)).EndInit();
            this._componentTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._componentDataGridView)).EndInit();
            this._batchTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._batchDataGridView)).EndInit();
            this._batchComponentTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._batchComponentDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl _configurationTabControl;
        private System.Windows.Forms.TabPage _registryTabPage;
        private System.Windows.Forms.TabPage _componentTabPage;
        private System.Windows.Forms.DataGridView _registryDataGridView;
        private System.Windows.Forms.TabPage _batchTabPage;
        private System.Windows.Forms.TabPage _batchComponentTabPage;
        private System.Windows.Forms.DataGridView _componentDataGridView;
        private System.Windows.Forms.DataGridView _batchDataGridView;
        private System.Windows.Forms.DataGridView _batchComponentDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn _registryDFColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn _registryVFColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _registryTColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _registryValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _batchComponentDFColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn _batchComponentVFColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _batchComponentTColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _batchComponentValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _componentDFColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn _componentVFColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _componentTColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _componentValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _batchDFColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn _batchVFColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _batchTColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _batchValueColumn;
    }
}
