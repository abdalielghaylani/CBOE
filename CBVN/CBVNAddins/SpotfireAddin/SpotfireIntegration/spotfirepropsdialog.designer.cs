namespace SpotfireIntegration
{
    partial class SpotfirePropsDialog
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
            this.AutoRefreshCheckBox = new System.Windows.Forms.CheckBox();
            this.WarnRefreshCheckBox = new System.Windows.Forms.CheckBox();
            this.WarnRebuildCheckBox = new System.Windows.Forms.CheckBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.maxRowsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.maxRowsUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // AutoRefreshCheckBox
            // 
            this.AutoRefreshCheckBox.AutoSize = true;
            this.AutoRefreshCheckBox.Location = new System.Drawing.Point(25, 27);
            this.AutoRefreshCheckBox.Name = "AutoRefreshCheckBox";
            this.AutoRefreshCheckBox.Size = new System.Drawing.Size(234, 17);
            this.AutoRefreshCheckBox.TabIndex = 0;
            this.AutoRefreshCheckBox.Text = "Auto-refresh on change of ChemBioViz hitlist";
            this.AutoRefreshCheckBox.UseVisualStyleBackColor = true;
            // 
            // WarnRefreshCheckBox
            // 
            this.WarnRefreshCheckBox.AutoSize = true;
            this.WarnRefreshCheckBox.Checked = true;
            this.WarnRefreshCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarnRefreshCheckBox.Location = new System.Drawing.Point(25, 50);
            this.WarnRefreshCheckBox.Name = "WarnRefreshCheckBox";
            this.WarnRefreshCheckBox.Size = new System.Drawing.Size(213, 17);
            this.WarnRefreshCheckBox.TabIndex = 1;
            this.WarnRefreshCheckBox.Text = "Ask before modifying Spotfire data table";
            this.WarnRefreshCheckBox.UseVisualStyleBackColor = true;
            // 
            // WarnRebuildCheckBox
            // 
            this.WarnRebuildCheckBox.AutoSize = true;
            this.WarnRebuildCheckBox.Checked = true;
            this.WarnRebuildCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarnRebuildCheckBox.Location = new System.Drawing.Point(25, 73);
            this.WarnRebuildCheckBox.Name = "WarnRebuildCheckBox";
            this.WarnRebuildCheckBox.Size = new System.Drawing.Size(192, 17);
            this.WarnRebuildCheckBox.TabIndex = 2;
            this.WarnRebuildCheckBox.Text = "Ask before closing Spotfire analysis";
            this.WarnRebuildCheckBox.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(157, 167);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 3;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(238, 167);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // maxRowsUpDown
            // 
            this.maxRowsUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxRowsUpDown.Location = new System.Drawing.Point(192, 116);
            this.maxRowsUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.maxRowsUpDown.Name = "maxRowsUpDown";
            this.maxRowsUpDown.Size = new System.Drawing.Size(79, 20);
            this.maxRowsUpDown.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Maximum records in data table:";
            // 
            // SpotfirePropsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 202);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maxRowsUpDown);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.WarnRebuildCheckBox);
            this.Controls.Add(this.WarnRefreshCheckBox);
            this.Controls.Add(this.AutoRefreshCheckBox);
            this.Name = "SpotfirePropsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spotfire Addin Preferences";
            ((System.ComponentModel.ISupportInitialize)(this.maxRowsUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox AutoRefreshCheckBox;
        private System.Windows.Forms.CheckBox WarnRefreshCheckBox;
        private System.Windows.Forms.CheckBox WarnRebuildCheckBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.NumericUpDown maxRowsUpDown;
        private System.Windows.Forms.Label label1;
    }
}