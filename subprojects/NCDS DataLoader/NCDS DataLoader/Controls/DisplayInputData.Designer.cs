namespace CambridgeSoft.NCDS_DataLoader.Controls
{
    partial class DisplayInputData
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this._BatchMarkbutton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this._Inversebutton = new System.Windows.Forms.Button();
            this._ExportButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(25, 60);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(750, 290);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.Sorted += new System.EventHandler(this.dataGridView1_Sorted);
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(22, 353);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "\"Structure\" does not show.";
            // 
            // _BatchMarkbutton
            // 
            this._BatchMarkbutton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._BatchMarkbutton.Location = new System.Drawing.Point(25, 393);
            this._BatchMarkbutton.Name = "_BatchMarkbutton";
            this._BatchMarkbutton.Size = new System.Drawing.Size(75, 29);
            this._BatchMarkbutton.TabIndex = 17;
            this._BatchMarkbutton.Text = "Batch Mark";
            this._BatchMarkbutton.UseVisualStyleBackColor = false;
            this._BatchMarkbutton.Click += new System.EventHandler(this._BatchMarkbutton_Click);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SelectAllButton.Location = new System.Drawing.Point(113, 393);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(75, 29);
            this.SelectAllButton.TabIndex = 17;
            this.SelectAllButton.Text = "Select All";
            this.SelectAllButton.UseVisualStyleBackColor = false;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // _Inversebutton
            // 
            this._Inversebutton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._Inversebutton.Location = new System.Drawing.Point(201, 393);
            this._Inversebutton.Name = "_Inversebutton";
            this._Inversebutton.Size = new System.Drawing.Size(75, 29);
            this._Inversebutton.TabIndex = 17;
            this._Inversebutton.Text = "Inverse";
            this._Inversebutton.UseVisualStyleBackColor = false;
            this._Inversebutton.Click += new System.EventHandler(this._Inversebutton_Click);
            // 
            // _ExportButton
            // 
            this._ExportButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._ExportButton.Location = new System.Drawing.Point(618, 393);
            this._ExportButton.Name = "_ExportButton";
            this._ExportButton.Size = new System.Drawing.Size(75, 29);
            this._ExportButton.TabIndex = 17;
            this._ExportButton.Text = "Export to";
            this._ExportButton.UseVisualStyleBackColor = false;
            this._ExportButton.Click += new System.EventHandler(this._ExportButton_Click);
            // 
            // DisplayInputData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ExportButton);
            this.Controls.Add(this._Inversebutton);
            this.Controls.Add(this.SelectAllButton);
            this.Controls.Add(this._BatchMarkbutton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DisplayInputData";
            this.Size = new System.Drawing.Size(800, 500);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _BatchMarkbutton;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button _Inversebutton;
        private System.Windows.Forms.Button _ExportButton;
    }
}
