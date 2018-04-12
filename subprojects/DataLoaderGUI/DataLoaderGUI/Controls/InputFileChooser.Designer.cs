namespace CambridgeSoft.DataLoaderGUI.Controls
{
    partial class InputFileChooser
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
            this._HeaderComboBox = new System.Windows.Forms.ComboBox();
            this._WorkSheetComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this._UserNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._PasswordTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._LoginGroupBox = new System.Windows.Forms.GroupBox();
            this._BrowseButton = new System.Windows.Forms.Button();
            this._InputFileTextBox = new System.Windows.Forms.TextBox();
            this._InputFileLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._DelimiterTextBox = new System.Windows.Forms.TextBox();
            this._SplitFileButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.StartAtRecordTextBox = new System.Windows.Forms.TextBox();
            this.EndAtRecordTextBox = new System.Windows.Forms.TextBox();
            this.RegisterKnownRegCheckBox = new System.Windows.Forms.CheckBox();
            this._LoginGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _HeaderComboBox
            // 
            this._HeaderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._HeaderComboBox.Enabled = false;
            this._HeaderComboBox.FormattingEnabled = true;
            this._HeaderComboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._HeaderComboBox.Location = new System.Drawing.Point(365, 239);
            this._HeaderComboBox.Name = "_HeaderComboBox";
            this._HeaderComboBox.Size = new System.Drawing.Size(49, 21);
            this._HeaderComboBox.TabIndex = 9;
            // 
            // _WorkSheetComboBox
            // 
            this._WorkSheetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._WorkSheetComboBox.Enabled = false;
            this._WorkSheetComboBox.FormattingEnabled = true;
            this._WorkSheetComboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._WorkSheetComboBox.Location = new System.Drawing.Point(365, 209);
            this._WorkSheetComboBox.Name = "_WorkSheetComboBox";
            this._WorkSheetComboBox.Size = new System.Drawing.Size(145, 21);
            this._WorkSheetComboBox.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(299, 242);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Header:";
            // 
            // _UserNameTextBox
            // 
            this._UserNameTextBox.Location = new System.Drawing.Point(76, 24);
            this._UserNameTextBox.Name = "_UserNameTextBox";
            this._UserNameTextBox.Size = new System.Drawing.Size(168, 20);
            this._UserNameTextBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(299, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Worksheet:";
            // 
            // _PasswordTextBox
            // 
            this._PasswordTextBox.Location = new System.Drawing.Point(76, 54);
            this._PasswordTextBox.Name = "_PasswordTextBox";
            this._PasswordTextBox.PasswordChar = '*';
            this._PasswordTextBox.Size = new System.Drawing.Size(168, 20);
            this._PasswordTextBox.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(6, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Password:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(6, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "User Name:";
            // 
            // _LoginGroupBox
            // 
            this._LoginGroupBox.Controls.Add(this._PasswordTextBox);
            this._LoginGroupBox.Controls.Add(this._UserNameTextBox);
            this._LoginGroupBox.Controls.Add(this.label6);
            this._LoginGroupBox.Controls.Add(this.label5);
            this._LoginGroupBox.Location = new System.Drawing.Point(15, 201);
            this._LoginGroupBox.Name = "_LoginGroupBox";
            this._LoginGroupBox.Size = new System.Drawing.Size(260, 100);
            this._LoginGroupBox.TabIndex = 5;
            this._LoginGroupBox.TabStop = false;
            this._LoginGroupBox.Text = "Login";
            // 
            // _BrowseButton
            // 
            this._BrowseButton.BackColor = System.Drawing.Color.LightGray;
            this._BrowseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._BrowseButton.Location = new System.Drawing.Point(520, 101);
            this._BrowseButton.Name = "_BrowseButton";
            this._BrowseButton.Size = new System.Drawing.Size(82, 29);
            this._BrowseButton.TabIndex = 2;
            this._BrowseButton.Text = "Browse";
            this._BrowseButton.UseVisualStyleBackColor = false;
            // 
            // _InputFileTextBox
            // 
            this._InputFileTextBox.Location = new System.Drawing.Point(70, 105);
            this._InputFileTextBox.Name = "_InputFileTextBox";
            this._InputFileTextBox.Size = new System.Drawing.Size(442, 20);
            this._InputFileTextBox.TabIndex = 1;
            // 
            // _InputFileLabel
            // 
            this._InputFileLabel.AutoSize = true;
            this._InputFileLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._InputFileLabel.Location = new System.Drawing.Point(12, 109);
            this._InputFileLabel.Name = "_InputFileLabel";
            this._InputFileLabel.Size = new System.Drawing.Size(50, 13);
            this._InputFileLabel.TabIndex = 17;
            this._InputFileLabel.Text = "Input File";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(299, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Delimiter:";
            // 
            // _DelimiterTextBox
            // 
            this._DelimiterTextBox.Enabled = false;
            this._DelimiterTextBox.Location = new System.Drawing.Point(365, 270);
            this._DelimiterTextBox.Name = "_DelimiterTextBox";
            this._DelimiterTextBox.Size = new System.Drawing.Size(100, 20);
            this._DelimiterTextBox.TabIndex = 10;
            // 
            // _SplitFileButton
            // 
            this._SplitFileButton.AutoSize = true;
            this._SplitFileButton.BackColor = System.Drawing.Color.LightGray;
            this._SplitFileButton.Image = global::CambridgeSoft.DataLoaderGUI.Properties.Resources.document_split;
            this._SplitFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._SplitFileButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._SplitFileButton.Location = new System.Drawing.Point(564, 309);
            this._SplitFileButton.Name = "_SplitFileButton";
            this._SplitFileButton.Size = new System.Drawing.Size(82, 29);
            this._SplitFileButton.TabIndex = 11;
            this._SplitFileButton.Text = "Split File";
            this._SplitFileButton.UseVisualStyleBackColor = false;
            this._SplitFileButton.Click += new System.EventHandler(this._SplitFileButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(70, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Start at Record:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(325, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "End at Record:";
            // 
            // StartAtRecordTextBox
            // 
            this.StartAtRecordTextBox.Location = new System.Drawing.Point(160, 141);
            this.StartAtRecordTextBox.MaxLength = 9;
            this.StartAtRecordTextBox.Name = "StartAtRecordTextBox";
            this.StartAtRecordTextBox.Size = new System.Drawing.Size(100, 20);
            this.StartAtRecordTextBox.TabIndex = 3;
            this.StartAtRecordTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // EndAtRecordTextBox
            // 
            this.EndAtRecordTextBox.Location = new System.Drawing.Point(412, 141);
            this.EndAtRecordTextBox.MaxLength = 9;
            this.EndAtRecordTextBox.Name = "EndAtRecordTextBox";
            this.EndAtRecordTextBox.Size = new System.Drawing.Size(100, 20);
            this.EndAtRecordTextBox.TabIndex = 4;
            this.EndAtRecordTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // RegisterKnownRegCheckBox
            // 
            this.RegisterKnownRegCheckBox.AutoSize = true;
            this.RegisterKnownRegCheckBox.Location = new System.Drawing.Point(309, 296);
            this.RegisterKnownRegCheckBox.Name = "RegisterKnownRegCheckBox";
            this.RegisterKnownRegCheckBox.Size = new System.Drawing.Size(201, 17);
            this.RegisterKnownRegCheckBox.TabIndex = 18;
            this.RegisterKnownRegCheckBox.Text = "Register with known RegNum values";
            this.RegisterKnownRegCheckBox.UseVisualStyleBackColor = true;
            // 
            // InputFileChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RegisterKnownRegCheckBox);
            this.Controls.Add(this.EndAtRecordTextBox);
            this.Controls.Add(this.StartAtRecordTextBox);
            this.Controls.Add(this._DelimiterTextBox);
            this.Controls.Add(this._HeaderComboBox);
            this.Controls.Add(this._WorkSheetComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._LoginGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._SplitFileButton);
            this.Controls.Add(this._BrowseButton);
            this.Controls.Add(this._InputFileTextBox);
            this.Controls.Add(this._InputFileLabel);
            this.Name = "InputFileChooser";
            this.Size = new System.Drawing.Size(672, 428);
            this._LoginGroupBox.ResumeLayout(false);
            this._LoginGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _HeaderComboBox;
        private System.Windows.Forms.ComboBox _WorkSheetComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _UserNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _PasswordTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox _LoginGroupBox;
        private System.Windows.Forms.Button _BrowseButton;
        private System.Windows.Forms.TextBox _InputFileTextBox;
        private System.Windows.Forms.Label _InputFileLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _DelimiterTextBox;
        private System.Windows.Forms.Button _SplitFileButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox StartAtRecordTextBox;
        private System.Windows.Forms.TextBox EndAtRecordTextBox;
        private System.Windows.Forms.CheckBox RegisterKnownRegCheckBox;
    }
}
