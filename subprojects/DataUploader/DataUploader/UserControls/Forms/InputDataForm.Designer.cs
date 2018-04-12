namespace CambridgeSoft.COE.DataLoader.UserControls.Forms
{
    partial class InputDataForm
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
            this._valueComboBox = new System.Windows.Forms.ComboBox();
            this._valueTextBox = new System.Windows.Forms.TextBox();
            this._constantValueLabel = new System.Windows.Forms.Label();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._inputFieldValueLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _valueComboBox
            // 
            this._valueComboBox.FormattingEnabled = true;
            this._valueComboBox.Location = new System.Drawing.Point(54, 42);
            this._valueComboBox.Name = "_valueComboBox";
            this._valueComboBox.Size = new System.Drawing.Size(162, 21);
            this._valueComboBox.TabIndex = 0;
            // 
            // _valueTextBox
            // 
            this._valueTextBox.Location = new System.Drawing.Point(54, 43);
            this._valueTextBox.Name = "_valueTextBox";
            this._valueTextBox.Size = new System.Drawing.Size(162, 20);
            this._valueTextBox.TabIndex = 1;
            // 
            // _constantValueLabel
            // 
            this._constantValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._constantValueLabel.Location = new System.Drawing.Point(51, 13);
            this._constantValueLabel.Name = "_constantValueLabel";
            this._constantValueLabel.Size = new System.Drawing.Size(165, 13);
            this._constantValueLabel.TabIndex = 2;
            this._constantValueLabel.Text = "Please input value:";
            // 
            // _saveButton
            // 
            this._saveButton.Location = new System.Drawing.Point(54, 82);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(75, 23);
            this._saveButton.TabIndex = 3;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(141, 82);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _inputFieldValueLabel
            // 
            this._inputFieldValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._inputFieldValueLabel.Location = new System.Drawing.Point(51, 13);
            this._inputFieldValueLabel.Name = "_inputFieldValueLabel";
            this._inputFieldValueLabel.Size = new System.Drawing.Size(165, 13);
            this._inputFieldValueLabel.TabIndex = 4;
            this._inputFieldValueLabel.Text = "Please select the value:";
            // 
            // InputDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(265, 121);
            this.Controls.Add(this._inputFieldValueLabel);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._saveButton);
            this.Controls.Add(this._constantValueLabel);
            this.Controls.Add(this._valueTextBox);
            this.Controls.Add(this._valueComboBox);
            this.Name = "InputDataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputDataForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _valueComboBox;
        private System.Windows.Forms.TextBox _valueTextBox;
        private System.Windows.Forms.Label _constantValueLabel;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _inputFieldValueLabel;
    }
}