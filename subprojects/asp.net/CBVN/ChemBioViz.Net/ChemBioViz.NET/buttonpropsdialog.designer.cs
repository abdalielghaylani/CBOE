namespace ChemBioViz.NET
{
    partial class ButtonPropsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonPropsDialog));
            this.argTextBox = new System.Windows.Forms.TextBox();
            this.insertButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.variableComboBox = new System.Windows.Forms.ComboBox();
            this.dataFieldComboBox = new System.Windows.Forms.ComboBox();
            this.currFieldTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.variableRadioButton = new System.Windows.Forms.RadioButton();
            this.dataFieldRadioButton = new System.Windows.Forms.RadioButton();
            this.currFieldRadioButton = new System.Windows.Forms.RadioButton();
            this.actionComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.descrTextBox = new System.Windows.Forms.TextBox();
            this.displayTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OKbutton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tooltipTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // argTextBox
            // 
            this.argTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.argTextBox.Location = new System.Drawing.Point(20, 98);
            this.argTextBox.Multiline = true;
            this.argTextBox.Name = "argTextBox";
            this.argTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.argTextBox.Size = new System.Drawing.Size(375, 38);
            this.argTextBox.TabIndex = 1;
            this.argTextBox.Leave += new System.EventHandler(this.argTextBox_Leave);
            this.argTextBox.Enter += new System.EventHandler(this.argTextBox_Enter);
            // 
            // insertButton
            // 
            this.insertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.insertButton.Location = new System.Drawing.Point(275, 33);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(75, 23);
            this.insertButton.TabIndex = 9;
            this.insertButton.Text = "Insert";
            this.insertButton.UseVisualStyleBackColor = true;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.variableComboBox);
            this.groupBox1.Controls.Add(this.dataFieldComboBox);
            this.groupBox1.Controls.Add(this.currFieldTextBox);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.variableRadioButton);
            this.groupBox1.Controls.Add(this.dataFieldRadioButton);
            this.groupBox1.Controls.Add(this.currFieldRadioButton);
            this.groupBox1.Controls.Add(this.insertButton);
            this.groupBox1.Location = new System.Drawing.Point(20, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(375, 110);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Variables";
            // 
            // variableComboBox
            // 
            this.variableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.variableComboBox.FormattingEnabled = true;
            this.variableComboBox.Location = new System.Drawing.Point(106, 73);
            this.variableComboBox.Name = "variableComboBox";
            this.variableComboBox.Size = new System.Drawing.Size(143, 21);
            this.variableComboBox.TabIndex = 8;
            this.variableComboBox.SelectedIndexChanged += new System.EventHandler(this.variableComboBox_SelectedIndexChanged);
            // 
            // dataFieldComboBox
            // 
            this.dataFieldComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataFieldComboBox.FormattingEnabled = true;
            this.dataFieldComboBox.Location = new System.Drawing.Point(106, 46);
            this.dataFieldComboBox.Name = "dataFieldComboBox";
            this.dataFieldComboBox.Size = new System.Drawing.Size(143, 21);
            this.dataFieldComboBox.TabIndex = 6;
            this.dataFieldComboBox.SelectedIndexChanged += new System.EventHandler(this.dataFieldComboBox_SelectedIndexChanged);
            // 
            // currFieldTextBox
            // 
            this.currFieldTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.currFieldTextBox.Location = new System.Drawing.Point(106, 19);
            this.currFieldTextBox.Name = "currFieldTextBox";
            this.currFieldTextBox.ReadOnly = true;
            this.currFieldTextBox.Size = new System.Drawing.Size(143, 20);
            this.currFieldTextBox.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(256, 65);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(117, 29);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "Click to insert selected item at text cursor.";
            // 
            // variableRadioButton
            // 
            this.variableRadioButton.AutoSize = true;
            this.variableRadioButton.Location = new System.Drawing.Point(12, 75);
            this.variableRadioButton.Name = "variableRadioButton";
            this.variableRadioButton.Size = new System.Drawing.Size(63, 17);
            this.variableRadioButton.TabIndex = 7;
            this.variableRadioButton.TabStop = true;
            this.variableRadioButton.Text = "Variable";
            this.variableRadioButton.UseVisualStyleBackColor = true;
            this.variableRadioButton.Click += new System.EventHandler(this.variableRadioButton_Click);
            // 
            // dataFieldRadioButton
            // 
            this.dataFieldRadioButton.AutoSize = true;
            this.dataFieldRadioButton.Location = new System.Drawing.Point(12, 48);
            this.dataFieldRadioButton.Name = "dataFieldRadioButton";
            this.dataFieldRadioButton.Size = new System.Drawing.Size(70, 17);
            this.dataFieldRadioButton.TabIndex = 5;
            this.dataFieldRadioButton.TabStop = true;
            this.dataFieldRadioButton.Text = "Data field";
            this.dataFieldRadioButton.UseVisualStyleBackColor = true;
            this.dataFieldRadioButton.Click += new System.EventHandler(this.dataFieldRadioButton_Click);
            // 
            // currFieldRadioButton
            // 
            this.currFieldRadioButton.AutoSize = true;
            this.currFieldRadioButton.Location = new System.Drawing.Point(12, 21);
            this.currFieldRadioButton.Name = "currFieldRadioButton";
            this.currFieldRadioButton.Size = new System.Drawing.Size(78, 17);
            this.currFieldRadioButton.TabIndex = 3;
            this.currFieldRadioButton.TabStop = true;
            this.currFieldRadioButton.Text = "Bound field";
            this.currFieldRadioButton.UseVisualStyleBackColor = true;
            this.currFieldRadioButton.Click += new System.EventHandler(this.currFieldRadioButton_Click);
            // 
            // actionComboBox
            // 
            this.actionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.actionComboBox.FormattingEnabled = true;
            this.actionComboBox.Location = new System.Drawing.Point(223, 6);
            this.actionComboBox.Name = "actionComboBox";
            this.actionComboBox.Size = new System.Drawing.Size(172, 21);
            this.actionComboBox.TabIndex = 0;
            this.actionComboBox.SelectedIndexChanged += new System.EventHandler(this.actionComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Argument";
            // 
            // descrTextBox
            // 
            this.descrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descrTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descrTextBox.Location = new System.Drawing.Point(20, 33);
            this.descrTextBox.Multiline = true;
            this.descrTextBox.Name = "descrTextBox";
            this.descrTextBox.ReadOnly = true;
            this.descrTextBox.Size = new System.Drawing.Size(375, 46);
            this.descrTextBox.TabIndex = 6;
            this.descrTextBox.Text = "Description";
            // 
            // displayTextBox
            // 
            this.displayTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.displayTextBox.Location = new System.Drawing.Point(20, 155);
            this.displayTextBox.Name = "displayTextBox";
            this.displayTextBox.Size = new System.Drawing.Size(375, 20);
            this.displayTextBox.TabIndex = 2;
            this.displayTextBox.Leave += new System.EventHandler(this.displayTextBox_Leave);
            this.displayTextBox.Enter += new System.EventHandler(this.displayTextBox_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Button Label";
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.Location = new System.Drawing.Point(239, 356);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(75, 23);
            this.OKbutton.TabIndex = 11;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(320, 356);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(191, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Action to occur when button is clicked:";
            // 
            // tooltipTextBox
            // 
            this.tooltipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tooltipTextBox.Location = new System.Drawing.Point(20, 194);
            this.tooltipTextBox.Multiline = true;
            this.tooltipTextBox.Name = "tooltipTextBox";
            this.tooltipTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tooltipTextBox.Size = new System.Drawing.Size(375, 38);
            this.tooltipTextBox.TabIndex = 2;
            this.tooltipTextBox.Leave += new System.EventHandler(this.tooltipTextBox_Leave);
            this.tooltipTextBox.Enter += new System.EventHandler(this.tooltipTextBox_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Tooltip Text";
            // 
            // ButtonPropsDialog
            // 
            this.AcceptButton = this.OKbutton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(414, 392);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.descrTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.actionComboBox);
            this.Controls.Add(this.tooltipTextBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.displayTextBox);
            this.Controls.Add(this.argTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 430);
            this.MinimumSize = new System.Drawing.Size(430, 430);
            this.Name = "ButtonPropsDialog";
            this.Text = "CBV Button Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TextBox argTextBox;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox variableComboBox;
        private System.Windows.Forms.ComboBox dataFieldComboBox;
        private System.Windows.Forms.TextBox currFieldTextBox;
        private System.Windows.Forms.RadioButton variableRadioButton;
        private System.Windows.Forms.RadioButton dataFieldRadioButton;
        private System.Windows.Forms.RadioButton currFieldRadioButton;
        private System.Windows.Forms.ComboBox actionComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox descrTextBox;
        private System.Windows.Forms.TextBox displayTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox tooltipTextBox;
        private System.Windows.Forms.Label label1;
    }
}