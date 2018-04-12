namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    partial class LogSummaryView
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
            this.dgvMessages = new System.Windows.Forms.DataGridView();
            this.chkError = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblInstruction = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMessages
            // 
            this.dgvMessages.AllowUserToAddRows = false;
            this.dgvMessages.AllowUserToDeleteRows = false;
            this.dgvMessages.AllowUserToOrderColumns = true;
            this.dgvMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMessages.Location = new System.Drawing.Point(3, 120);
            this.dgvMessages.Name = "dgvMessages";
            this.dgvMessages.ReadOnly = true;
            this.dgvMessages.Size = new System.Drawing.Size(457, 387);
            this.dgvMessages.TabIndex = 3;
            // 
            // chkError
            // 
            this.chkError.AutoSize = true;
            this.chkError.Location = new System.Drawing.Point(3, 97);
            this.chkError.Name = "chkError";
            this.chkError.Size = new System.Drawing.Size(101, 17);
            this.chkError.TabIndex = 1;
            this.chkError.Text = "View only Errors";
            this.chkError.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(457, 74);
            this.textBox1.TabIndex = 4;
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Location = new System.Drawing.Point(121, 98);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(88, 13);
            this.lblInstruction.TabIndex = 5;
            this.lblInstruction.Text = "[placeholder text]";
            // 
            // LogSummaryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.chkError);
            this.Controls.Add(this.dgvMessages);
            this.Name = "LogSummaryView";
            this.Size = new System.Drawing.Size(463, 510);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMessages;
        private System.Windows.Forms.CheckBox chkError;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblInstruction;
    }
}
