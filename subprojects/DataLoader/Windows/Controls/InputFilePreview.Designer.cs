namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    partial class InputFilePreview
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
            this.dgvPreviewSet = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dgvRowDetails = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreviewSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRowDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPreviewSet
            // 
            this.dgvPreviewSet.AllowUserToAddRows = false;
            this.dgvPreviewSet.AllowUserToDeleteRows = false;
            this.dgvPreviewSet.AllowUserToOrderColumns = true;
            this.dgvPreviewSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPreviewSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreviewSet.Location = new System.Drawing.Point(3, 87);
            this.dgvPreviewSet.Name = "dgvPreviewSet";
            this.dgvPreviewSet.ReadOnly = true;
            this.dgvPreviewSet.Size = new System.Drawing.Size(457, 152);
            this.dgvPreviewSet.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(457, 78);
            this.textBox1.TabIndex = 5;
            // 
            // dgvRowDetails
            // 
            this.dgvRowDetails.AllowUserToAddRows = false;
            this.dgvRowDetails.AllowUserToDeleteRows = false;
            this.dgvRowDetails.AllowUserToOrderColumns = true;
            this.dgvRowDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRowDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRowDetails.Location = new System.Drawing.Point(3, 245);
            this.dgvRowDetails.Name = "dgvRowDetails";
            this.dgvRowDetails.ReadOnly = true;
            this.dgvRowDetails.Size = new System.Drawing.Size(457, 262);
            this.dgvRowDetails.TabIndex = 6;
            // 
            // InputFilePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvRowDetails);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dgvPreviewSet);
            this.Name = "InputFilePreview";
            this.Size = new System.Drawing.Size(463, 510);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreviewSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRowDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPreviewSet;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dgvRowDetails;
    }
}
