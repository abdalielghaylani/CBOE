namespace CambridgeSoft.COE.DataLoader.UserControls
{
    partial class ImportFilePicker
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
            this._fileNameTextBox = new System.Windows.Forms.TextBox();
            this._chooseFileButton = new System.Windows.Forms.Button();
            this._selectExtentionNameComboBox = new System.Windows.Forms.ComboBox();
            this._selectSheetLabel = new System.Windows.Forms.Label();
            this._selectSheetComboBox = new System.Windows.Forms.ComboBox();
            this._checkBoxTwo = new System.Windows.Forms.CheckBox();
            this._checkBoxOne = new System.Windows.Forms.CheckBox();
            this._csvConfigGroupBox = new System.Windows.Forms.GroupBox();
            this._sdfConfigGroupBox = new System.Windows.Forms.GroupBox();
            this._csvConfigGroupBox.SuspendLayout();
            this._sdfConfigGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _fileNameTextBox
            // 
            this._fileNameTextBox.BackColor = System.Drawing.Color.White;
            this._fileNameTextBox.ForeColor = System.Drawing.Color.Black;
            this._fileNameTextBox.Location = new System.Drawing.Point(25, 25);
            this._fileNameTextBox.Margin = new System.Windows.Forms.Padding(10);
            this._fileNameTextBox.Name = "_fileNameTextBox";
            this._fileNameTextBox.Size = new System.Drawing.Size(360, 20);
            this._fileNameTextBox.TabIndex = 0;
            // 
            // _chooseFileButton
            // 
            this._chooseFileButton.BackColor = System.Drawing.Color.White;
            this._chooseFileButton.ForeColor = System.Drawing.Color.Black;
            this._chooseFileButton.Location = new System.Drawing.Point(405, 25);
            this._chooseFileButton.Margin = new System.Windows.Forms.Padding(10);
            this._chooseFileButton.Name = "_chooseFileButton";
            this._chooseFileButton.Size = new System.Drawing.Size(70, 21);
            this._chooseFileButton.TabIndex = 1;
            this._chooseFileButton.Text = "choose file";
            this._chooseFileButton.UseVisualStyleBackColor = false;
            // 
            // _selectExtentionNameComboBox
            // 
            this._selectExtentionNameComboBox.BackColor = System.Drawing.Color.White;
            this._selectExtentionNameComboBox.ForeColor = System.Drawing.Color.Black;
            this._selectExtentionNameComboBox.FormattingEnabled = true;
            this._selectExtentionNameComboBox.Items.AddRange(new object[] {
            "sdf",
            "csv;tsv"});
            this._selectExtentionNameComboBox.Location = new System.Drawing.Point(25, 65);
            this._selectExtentionNameComboBox.Margin = new System.Windows.Forms.Padding(10);
            this._selectExtentionNameComboBox.Name = "_selectExtentionNameComboBox";
            this._selectExtentionNameComboBox.Size = new System.Drawing.Size(360, 21);
            this._selectExtentionNameComboBox.TabIndex = 2;
            // 
            // _selectSheetLabel
            // 
            this._selectSheetLabel.AutoSize = true;
            this._selectSheetLabel.Location = new System.Drawing.Point(13, 19);
            this._selectSheetLabel.Margin = new System.Windows.Forms.Padding(10, 3, 3, 0);
            this._selectSheetLabel.Name = "_selectSheetLabel";
            this._selectSheetLabel.Size = new System.Drawing.Size(73, 13);
            this._selectSheetLabel.TabIndex = 1;
            this._selectSheetLabel.Text = "select a sheet";
            // 
            // _selectSheetComboBox
            // 
            this._selectSheetComboBox.FormattingEnabled = true;
            this._selectSheetComboBox.Location = new System.Drawing.Point(99, 16);
            this._selectSheetComboBox.Margin = new System.Windows.Forms.Padding(10, 3, 10, 10);
            this._selectSheetComboBox.Name = "_selectSheetComboBox";
            this._selectSheetComboBox.Size = new System.Drawing.Size(189, 21);
            this._selectSheetComboBox.TabIndex = 2;
            // 
            // _checkBoxTwo
            // 
            this._checkBoxTwo.AutoSize = true;
            this._checkBoxTwo.Location = new System.Drawing.Point(13, 42);
            this._checkBoxTwo.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this._checkBoxTwo.Name = "_checkBoxTwo";
            this._checkBoxTwo.Size = new System.Drawing.Size(118, 17);
            this._checkBoxTwo.TabIndex = 0;
            this._checkBoxTwo.Text = "separator character";
            this._checkBoxTwo.UseVisualStyleBackColor = true;
            // 
            // _checkBoxOne
            // 
            this._checkBoxOne.AutoSize = true;
            this._checkBoxOne.Location = new System.Drawing.Point(13, 19);
            this._checkBoxOne.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this._checkBoxOne.Name = "_checkBoxOne";
            this._checkBoxOne.Size = new System.Drawing.Size(124, 17);
            this._checkBoxOne.TabIndex = 0;
            this._checkBoxOne.Text = "file has a row header";
            this._checkBoxOne.UseVisualStyleBackColor = true;
            // 
            // _csvConfigGroupBox
            // 
            this._csvConfigGroupBox.Controls.Add(this._checkBoxTwo);
            this._csvConfigGroupBox.Controls.Add(this._checkBoxOne);
            this._csvConfigGroupBox.Location = new System.Drawing.Point(25, 106);
            this._csvConfigGroupBox.Margin = new System.Windows.Forms.Padding(10);
            this._csvConfigGroupBox.Name = "_csvConfigGroupBox";
            this._csvConfigGroupBox.Size = new System.Drawing.Size(358, 119);
            this._csvConfigGroupBox.TabIndex = 3;
            this._csvConfigGroupBox.TabStop = false;
            // 
            // _sdfConfigGroupBox
            // 
            this._sdfConfigGroupBox.Controls.Add(this._selectSheetLabel);
            this._sdfConfigGroupBox.Controls.Add(this._selectSheetComboBox);
            this._sdfConfigGroupBox.Location = new System.Drawing.Point(25, 106);
            this._sdfConfigGroupBox.Margin = new System.Windows.Forms.Padding(10);
            this._sdfConfigGroupBox.Name = "_sdfConfigGroupBox";
            this._sdfConfigGroupBox.Size = new System.Drawing.Size(358, 119);
            this._sdfConfigGroupBox.TabIndex = 3;
            this._sdfConfigGroupBox.TabStop = false;
            // 
            // ImportFilePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._csvConfigGroupBox);
            this.Controls.Add(this._sdfConfigGroupBox);
            this.Controls.Add(this._selectExtentionNameComboBox);
            this.Controls.Add(this._chooseFileButton);
            this.Controls.Add(this._fileNameTextBox);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ImportFilePicker";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.Size = new System.Drawing.Size(500, 250);
            this._csvConfigGroupBox.ResumeLayout(false);
            this._csvConfigGroupBox.PerformLayout();
            this._sdfConfigGroupBox.ResumeLayout(false);
            this._sdfConfigGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _fileNameTextBox;
        private System.Windows.Forms.Button _chooseFileButton;
        private System.Windows.Forms.ComboBox _selectExtentionNameComboBox;
        private System.Windows.Forms.Label _selectSheetLabel;
        private System.Windows.Forms.ComboBox _selectSheetComboBox;
        private System.Windows.Forms.CheckBox _checkBoxTwo;
        private System.Windows.Forms.CheckBox _checkBoxOne;
        private System.Windows.Forms.GroupBox _csvConfigGroupBox;
        private System.Windows.Forms.GroupBox _sdfConfigGroupBox;
    }
}
