namespace CambridgeSoft.DataLoaderGUI.Controls
{
    partial class ModeSelection
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
            this._AllCountLabel = new System.Windows.Forms.Label();
            this._DuplicateLabel = new System.Windows.Forms.Label();
            this._UniqueLabel = new System.Windows.Forms.Label();
            this._DuplicateCountLabel = new System.Windows.Forms.Label();
            this._UniqueCountLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this._AllRadioButton = new System.Windows.Forms.RadioButton();
            this._UniqueRadioButton = new System.Windows.Forms.RadioButton();
            this._DuplicateRadioButton = new System.Windows.Forms.RadioButton();
            this._ImportButton = new System.Windows.Forms.Button();
            this._ReviewButton = new System.Windows.Forms.Button();
            this._ExportButton = new System.Windows.Forms.Button();
            this._CommentLabel = new System.Windows.Forms.Label();
            this.reviewlabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _AllCountLabel
            // 
            this._AllCountLabel.AutoSize = true;
            this._AllCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._AllCountLabel.Location = new System.Drawing.Point(55, 33);
            this._AllCountLabel.Name = "_AllCountLabel";
            this._AllCountLabel.Size = new System.Drawing.Size(176, 20);
            this._AllCountLabel.TabIndex = 0;
            this._AllCountLabel.Text = "{0} records checked. {1}";
            // 
            // _DuplicateLabel
            // 
            this._DuplicateLabel.AutoSize = true;
            this._DuplicateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._DuplicateLabel.Location = new System.Drawing.Point(55, 77);
            this._DuplicateLabel.Name = "_DuplicateLabel";
            this._DuplicateLabel.Size = new System.Drawing.Size(88, 20);
            this._DuplicateLabel.TabIndex = 0;
            this._DuplicateLabel.Text = "Duplicate : ";
            // 
            // _UniqueLabel
            // 
            this._UniqueLabel.AutoSize = true;
            this._UniqueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._UniqueLabel.Location = new System.Drawing.Point(55, 123);
            this._UniqueLabel.Name = "_UniqueLabel";
            this._UniqueLabel.Size = new System.Drawing.Size(72, 20);
            this._UniqueLabel.TabIndex = 0;
            this._UniqueLabel.Text = "Unique : ";
            // 
            // _DuplicateCountLabel
            // 
            this._DuplicateCountLabel.AutoSize = true;
            this._DuplicateCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._DuplicateCountLabel.Location = new System.Drawing.Point(170, 77);
            this._DuplicateCountLabel.Name = "_DuplicateCountLabel";
            this._DuplicateCountLabel.Size = new System.Drawing.Size(18, 20);
            this._DuplicateCountLabel.TabIndex = 0;
            this._DuplicateCountLabel.Text = "0";
            // 
            // _UniqueCountLabel
            // 
            this._UniqueCountLabel.AutoSize = true;
            this._UniqueCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._UniqueCountLabel.Location = new System.Drawing.Point(170, 123);
            this._UniqueCountLabel.Name = "_UniqueCountLabel";
            this._UniqueCountLabel.Size = new System.Drawing.Size(18, 20);
            this._UniqueCountLabel.TabIndex = 0;
            this._UniqueCountLabel.Text = "0";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._AllRadioButton);
            this.panel1.Controls.Add(this._UniqueRadioButton);
            this.panel1.Controls.Add(this._DuplicateRadioButton);
            this.panel1.Location = new System.Drawing.Point(59, 175);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 43);
            this.panel1.TabIndex = 1;
            // 
            // _AllRadioButton
            // 
            this._AllRadioButton.AutoSize = true;
            this._AllRadioButton.Location = new System.Drawing.Point(219, 12);
            this._AllRadioButton.Name = "_AllRadioButton";
            this._AllRadioButton.Size = new System.Drawing.Size(36, 17);
            this._AllRadioButton.TabIndex = 2;
            this._AllRadioButton.TabStop = true;
            this._AllRadioButton.Text = "All";
            this._AllRadioButton.UseVisualStyleBackColor = true;
            // 
            // _UniqueRadioButton
            // 
            this._UniqueRadioButton.AutoSize = true;
            this._UniqueRadioButton.Location = new System.Drawing.Point(119, 12);
            this._UniqueRadioButton.Name = "_UniqueRadioButton";
            this._UniqueRadioButton.Size = new System.Drawing.Size(59, 17);
            this._UniqueRadioButton.TabIndex = 2;
            this._UniqueRadioButton.TabStop = true;
            this._UniqueRadioButton.Text = "Unique";
            this._UniqueRadioButton.UseVisualStyleBackColor = true;
            // 
            // _DuplicateRadioButton
            // 
            this._DuplicateRadioButton.AutoSize = true;
            this._DuplicateRadioButton.Location = new System.Drawing.Point(19, 12);
            this._DuplicateRadioButton.Name = "_DuplicateRadioButton";
            this._DuplicateRadioButton.Size = new System.Drawing.Size(70, 17);
            this._DuplicateRadioButton.TabIndex = 2;
            this._DuplicateRadioButton.TabStop = true;
            this._DuplicateRadioButton.Text = "Duplicate";
            this._DuplicateRadioButton.UseVisualStyleBackColor = true;
            // 
            // _ImportButton
            // 
            this._ImportButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._ImportButton.Location = new System.Drawing.Point(59, 243);
            this._ImportButton.Name = "_ImportButton";
            this._ImportButton.Size = new System.Drawing.Size(104, 29);
            this._ImportButton.TabIndex = 2;
            this._ImportButton.Text = "Import";
            this._ImportButton.UseVisualStyleBackColor = false;
            this._ImportButton.Click += new System.EventHandler(this.AcceptButton_Click);
            // 
            // _ReviewButton
            // 
            this._ReviewButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._ReviewButton.Location = new System.Drawing.Point(59, 278);
            this._ReviewButton.Name = "_ReviewButton";
            this._ReviewButton.Size = new System.Drawing.Size(104, 29);
            this._ReviewButton.TabIndex = 2;
            this._ReviewButton.Text = "Review";
            this._ReviewButton.UseVisualStyleBackColor = false;
            this._ReviewButton.Click += new System.EventHandler(this._ReviewButton_Click);
            // 
            // _ExportButton
            // 
            this._ExportButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._ExportButton.Location = new System.Drawing.Point(59, 313);
            this._ExportButton.Name = "_ExportButton";
            this._ExportButton.Size = new System.Drawing.Size(104, 29);
            this._ExportButton.TabIndex = 2;
            this._ExportButton.Text = "Export to";
            this._ExportButton.UseVisualStyleBackColor = false;
            this._ExportButton.Click += new System.EventHandler(this._ExportButton_Click);
            // 
            // _CommentLabel
            // 
            this._CommentLabel.AutoSize = true;
            this._CommentLabel.Location = new System.Drawing.Point(193, 278);
            this._CommentLabel.Name = "_CommentLabel";
            this._CommentLabel.Size = new System.Drawing.Size(221, 26);
            this._CommentLabel.TabIndex = 3;
            this._CommentLabel.Text = "Exceed the maximum number of shows {0}\r\nand only the first {0} records will be di" +
                "splayed.\r\n";
            // 
            // reviewlabel
            // 
            this.reviewlabel.AutoSize = true;
            this.reviewlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reviewlabel.Location = new System.Drawing.Point(193, 316);
            this.reviewlabel.Name = "reviewlabel";
            this.reviewlabel.Size = new System.Drawing.Size(192, 13);
            this.reviewlabel.TabIndex = 4;
            this.reviewlabel.Text = "Loading the file,please waiting...";
            this.reviewlabel.Visible = false;
            // 
            // ModeSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.reviewlabel);
            this.Controls.Add(this._CommentLabel);
            this.Controls.Add(this._ExportButton);
            this.Controls.Add(this._ReviewButton);
            this.Controls.Add(this._ImportButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._UniqueCountLabel);
            this.Controls.Add(this._UniqueLabel);
            this.Controls.Add(this._DuplicateCountLabel);
            this.Controls.Add(this._DuplicateLabel);
            this.Controls.Add(this._AllCountLabel);
            this.Name = "ModeSelection";
            this.Size = new System.Drawing.Size(672, 428);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label _AllCountLabel;
        private System.Windows.Forms.Label _DuplicateLabel;
        private System.Windows.Forms.Label _UniqueLabel;
        private System.Windows.Forms.Label _DuplicateCountLabel;
        private System.Windows.Forms.Label _UniqueCountLabel;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton _UniqueRadioButton;
        private System.Windows.Forms.RadioButton _DuplicateRadioButton;
        public System.Windows.Forms.Button _ImportButton;
        public System.Windows.Forms.Button _ReviewButton;
        public System.Windows.Forms.Button _ExportButton;
        private System.Windows.Forms.Label _CommentLabel;
        private System.Windows.Forms.RadioButton _AllRadioButton;
        public System.Windows.Forms.Label reviewlabel;
    }
}
