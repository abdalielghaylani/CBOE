namespace CambridgeSoft.DataLoaderGUI.Controls
{
    partial class ImportOption
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
            this._ImportTempRadioButton = new System.Windows.Forms.RadioButton();
            this._ImportRegDupNoneRadioButton = new System.Windows.Forms.RadioButton();
            this._ImportRegDupAsTempRadioButton = new System.Windows.Forms.RadioButton();
            this._ImportRegDupAsCreateNewRadioButton = new System.Windows.Forms.RadioButton();
            this._ImportRegDupAsCreateNewBatchRadioButton = new System.Windows.Forms.RadioButton();
            this._ResultLabel = new System.Windows.Forms.Label();
            this._OptionPanel = new System.Windows.Forms.Panel();
            this._ImportAnotherFile = new System.Windows.Forms.Button();
            this._ResultRichTextBox = new System.Windows.Forms.RichTextBox();
            this._OptionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ImportTempRadioButton
            // 
            this._ImportTempRadioButton.AutoSize = true;
            this._ImportTempRadioButton.Location = new System.Drawing.Point(22, 12);
            this._ImportTempRadioButton.Name = "_ImportTempRadioButton";
            this._ImportTempRadioButton.Size = new System.Drawing.Size(84, 17);
            this._ImportTempRadioButton.TabIndex = 0;
            this._ImportTempRadioButton.TabStop = true;
            this._ImportTempRadioButton.Text = "Import Temp";
            this._ImportTempRadioButton.UseVisualStyleBackColor = true;
            // 
            // _ImportRegDupNoneRadioButton
            // 
            this._ImportRegDupNoneRadioButton.AutoSize = true;
            this._ImportRegDupNoneRadioButton.Location = new System.Drawing.Point(22, 49);
            this._ImportRegDupNoneRadioButton.Name = "_ImportRegDupNoneRadioButton";
            this._ImportRegDupNoneRadioButton.Size = new System.Drawing.Size(120, 17);
            this._ImportRegDupNoneRadioButton.TabIndex = 0;
            this._ImportRegDupNoneRadioButton.TabStop = true;
            this._ImportRegDupNoneRadioButton.Text = "ImportRegDupNone";
            this._ImportRegDupNoneRadioButton.UseVisualStyleBackColor = true;
            // 
            // _ImportRegDupAsTempRadioButton
            // 
            this._ImportRegDupAsTempRadioButton.AutoSize = true;
            this._ImportRegDupAsTempRadioButton.Location = new System.Drawing.Point(22, 86);
            this._ImportRegDupAsTempRadioButton.Name = "_ImportRegDupAsTempRadioButton";
            this._ImportRegDupAsTempRadioButton.Size = new System.Drawing.Size(133, 17);
            this._ImportRegDupAsTempRadioButton.TabIndex = 0;
            this._ImportRegDupAsTempRadioButton.TabStop = true;
            this._ImportRegDupAsTempRadioButton.Text = "ImportRegDupAsTemp";
            this._ImportRegDupAsTempRadioButton.UseVisualStyleBackColor = true;
            // 
            // _ImportRegDupAsCreateNewRadioButton
            // 
            this._ImportRegDupAsCreateNewRadioButton.AutoSize = true;
            this._ImportRegDupAsCreateNewRadioButton.Location = new System.Drawing.Point(22, 123);
            this._ImportRegDupAsCreateNewRadioButton.Name = "_ImportRegDupAsCreateNewRadioButton";
            this._ImportRegDupAsCreateNewRadioButton.Size = new System.Drawing.Size(159, 17);
            this._ImportRegDupAsCreateNewRadioButton.TabIndex = 0;
            this._ImportRegDupAsCreateNewRadioButton.TabStop = true;
            this._ImportRegDupAsCreateNewRadioButton.Text = "ImportRegDupAsCreateNew";
            this._ImportRegDupAsCreateNewRadioButton.UseVisualStyleBackColor = true;
            // 
            // _ImportRegDupAsCreateNewBatchRadioButton
            // 
            this._ImportRegDupAsCreateNewBatchRadioButton.AutoSize = true;
            this._ImportRegDupAsCreateNewBatchRadioButton.Location = new System.Drawing.Point(22, 160);
            this._ImportRegDupAsCreateNewBatchRadioButton.Name = "_ImportRegDupAsCreateNewBatchRadioButton";
            this._ImportRegDupAsCreateNewBatchRadioButton.Size = new System.Drawing.Size(156, 17);
            this._ImportRegDupAsCreateNewBatchRadioButton.TabIndex = 0;
            this._ImportRegDupAsCreateNewBatchRadioButton.TabStop = true;
            this._ImportRegDupAsCreateNewBatchRadioButton.Text = "ImportRegDupAsNewBatch";
            this._ImportRegDupAsCreateNewBatchRadioButton.UseVisualStyleBackColor = true;
            // 
            // _ResultLabel
            // 
            this._ResultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ResultLabel.Location = new System.Drawing.Point(60, 299);
            this._ResultLabel.Name = "_ResultLabel";
            this._ResultLabel.Size = new System.Drawing.Size(565, 55);
            this._ResultLabel.TabIndex = 1;
            this._ResultLabel.Text = "Result";
            // 
            // _OptionPanel
            // 
            this._OptionPanel.Controls.Add(this._ImportRegDupAsCreateNewRadioButton);
            this._OptionPanel.Controls.Add(this._ImportTempRadioButton);
            this._OptionPanel.Controls.Add(this._ImportRegDupAsCreateNewBatchRadioButton);
            this._OptionPanel.Controls.Add(this._ImportRegDupNoneRadioButton);
            this._OptionPanel.Controls.Add(this._ImportRegDupAsTempRadioButton);
            this._OptionPanel.Location = new System.Drawing.Point(42, 80);
            this._OptionPanel.Name = "_OptionPanel";
            this._OptionPanel.Size = new System.Drawing.Size(293, 189);
            this._OptionPanel.TabIndex = 2;
            // 
            // _ImportAnotherFile
            // 
            this._ImportAnotherFile.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._ImportAnotherFile.Location = new System.Drawing.Point(429, 80);
            this._ImportAnotherFile.Name = "_ImportAnotherFile";
            this._ImportAnotherFile.Size = new System.Drawing.Size(136, 29);
            this._ImportAnotherFile.TabIndex = 3;
            this._ImportAnotherFile.Text = "Import another file\r\n";
            this._ImportAnotherFile.UseVisualStyleBackColor = false;
            this._ImportAnotherFile.Click += new System.EventHandler(this._ImportAnotherFile_Click);
            // 
            // _ResultRichTextBox
            // 
            this._ResultRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._ResultRichTextBox.Location = new System.Drawing.Point(42, 298);
            this._ResultRichTextBox.Name = "_ResultRichTextBox";
            this._ResultRichTextBox.ReadOnly = true;
            this._ResultRichTextBox.Size = new System.Drawing.Size(293, 96);
            this._ResultRichTextBox.TabIndex = 4;
            this._ResultRichTextBox.Text = "";
            // 
            // ImportOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ResultRichTextBox);
            this.Controls.Add(this._ImportAnotherFile);
            this.Controls.Add(this._OptionPanel);
            this.Controls.Add(this._ResultLabel);
            this.Name = "ImportOption";
            this.Size = new System.Drawing.Size(672, 428);
            this._OptionPanel.ResumeLayout(false);
            this._OptionPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton _ImportTempRadioButton;
        private System.Windows.Forms.RadioButton _ImportRegDupNoneRadioButton;
        private System.Windows.Forms.RadioButton _ImportRegDupAsTempRadioButton;
        private System.Windows.Forms.RadioButton _ImportRegDupAsCreateNewRadioButton;
        private System.Windows.Forms.RadioButton _ImportRegDupAsCreateNewBatchRadioButton;
        private System.Windows.Forms.Label _ResultLabel;
        private System.Windows.Forms.Panel _OptionPanel;
        private System.Windows.Forms.Button _ImportAnotherFile;
        private System.Windows.Forms.RichTextBox _ResultRichTextBox;
    }
}
