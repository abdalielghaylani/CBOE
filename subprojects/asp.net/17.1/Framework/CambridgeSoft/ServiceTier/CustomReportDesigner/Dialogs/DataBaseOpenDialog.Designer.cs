namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    partial class DataBaseOpenDialog
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
            this.CancelUIButton = new System.Windows.Forms.Button();
            this.AcceptUIButton = new System.Windows.Forms.Button();
            this.AvailableRecordsDataGridView = new System.Windows.Forms.DataGridView();
            this.ErrorsTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.AvailableRecordsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CancelUIButton
            // 
            this.CancelUIButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelUIButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelUIButton.Location = new System.Drawing.Point(939, 291);
            this.CancelUIButton.Name = "CancelUIButton";
            this.CancelUIButton.Size = new System.Drawing.Size(75, 23);
            this.CancelUIButton.TabIndex = 0;
            this.CancelUIButton.Text = "Cancel";
            this.CancelUIButton.UseVisualStyleBackColor = true;
            // 
            // AcceptUIButton
            // 
            this.AcceptUIButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AcceptUIButton.Location = new System.Drawing.Point(939, 262);
            this.AcceptUIButton.Name = "AcceptUIButton";
            this.AcceptUIButton.Size = new System.Drawing.Size(75, 23);
            this.AcceptUIButton.TabIndex = 1;
            this.AcceptUIButton.Text = "Accept";
            this.AcceptUIButton.UseVisualStyleBackColor = true;
            this.AcceptUIButton.Click += new System.EventHandler(this.AcceptButton_Click);
            // 
            // AvailableRecordsDataGridView
            // 
            this.AvailableRecordsDataGridView.AllowUserToAddRows = false;
            this.AvailableRecordsDataGridView.AllowUserToDeleteRows = false;
            this.AvailableRecordsDataGridView.AllowUserToResizeRows = false;
            this.AvailableRecordsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AvailableRecordsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.AvailableRecordsDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.AvailableRecordsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AvailableRecordsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.AvailableRecordsDataGridView.Location = new System.Drawing.Point(12, 2);
            this.AvailableRecordsDataGridView.MultiSelect = false;
            this.AvailableRecordsDataGridView.Name = "AvailableRecordsDataGridView";
            this.AvailableRecordsDataGridView.ReadOnly = true;
            this.AvailableRecordsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.AvailableRecordsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AvailableRecordsDataGridView.ShowEditingIcon = false;
            this.AvailableRecordsDataGridView.Size = new System.Drawing.Size(1002, 254);
            this.AvailableRecordsDataGridView.TabIndex = 2;
            this.AvailableRecordsDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataViewsDataGridView_CellDoubleClick);
            // 
            // ErrorsTextBox
            // 
            this.ErrorsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorsTextBox.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ErrorsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ErrorsTextBox.ForeColor = System.Drawing.Color.Red;
            this.ErrorsTextBox.Location = new System.Drawing.Point(12, 262);
            this.ErrorsTextBox.Multiline = true;
            this.ErrorsTextBox.Name = "ErrorsTextBox";
            this.ErrorsTextBox.Size = new System.Drawing.Size(915, 52);
            this.ErrorsTextBox.TabIndex = 3;
            this.ErrorsTextBox.Visible = false;
            // 
            // DataBaseOpenDialog
            // 
            this.AcceptButton = this.AcceptUIButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelUIButton;
            this.ClientSize = new System.Drawing.Size(1026, 326);
            this.Controls.Add(this.ErrorsTextBox);
            this.Controls.Add(this.AvailableRecordsDataGridView);
            this.Controls.Add(this.AcceptUIButton);
            this.Controls.Add(this.CancelUIButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataBaseOpenDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DataBaseOpenDialog";
            ((System.ComponentModel.ISupportInitialize)(this.AvailableRecordsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button CancelUIButton;
        protected System.Windows.Forms.Button AcceptUIButton;
        protected System.Windows.Forms.TextBox ErrorsTextBox;
        protected System.Windows.Forms.DataGridView AvailableRecordsDataGridView;
    }
}