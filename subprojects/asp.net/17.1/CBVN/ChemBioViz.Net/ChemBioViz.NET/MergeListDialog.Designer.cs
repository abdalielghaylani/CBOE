namespace ChemBioViz.NET
{
    partial class MergeListDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeListDialog));
            this.OKultraButton = new Infragistics.Win.Misc.UltraButton();
            this.CancelUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.subtractFromRadioButton = new System.Windows.Forms.RadioButton();
            this.unionRadioButton = new System.Windows.Forms.RadioButton();
            this.subtractRadioButton = new System.Windows.Forms.RadioButton();
            this.intersectRadioButton = new System.Windows.Forms.RadioButton();
            this.q2Label = new System.Windows.Forms.Label();
            this.q1Label = new System.Windows.Forms.Label();
            this.List2Label = new System.Windows.Forms.Label();
            this.List1Label = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            this.SuspendLayout();
            // 
            // OKultraButton
            // 
            this.OKultraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKultraButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKultraButton.Location = new System.Drawing.Point(247, 226);
            this.OKultraButton.Name = "OKultraButton";
            this.OKultraButton.Size = new System.Drawing.Size(81, 23);
            this.OKultraButton.TabIndex = 4;
            this.OKultraButton.Text = "OK";
            this.OKultraButton.Click += new System.EventHandler(this.OKultraButton_Click);
            // 
            // CancelUltraButton
            // 
            this.CancelUltraButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelUltraButton.Location = new System.Drawing.Point(334, 226);
            this.CancelUltraButton.Name = "CancelUltraButton";
            this.CancelUltraButton.Size = new System.Drawing.Size(81, 23);
            this.CancelUltraButton.TabIndex = 5;
            this.CancelUltraButton.Text = "Cancel";
            this.CancelUltraButton.Click += new System.EventHandler(this.CancelUltraButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.subtractFromRadioButton);
            this.groupBox1.Controls.Add(this.unionRadioButton);
            this.groupBox1.Controls.Add(this.subtractRadioButton);
            this.groupBox1.Controls.Add(this.intersectRadioButton);
            this.groupBox1.Controls.Add(this.q2Label);
            this.groupBox1.Controls.Add(this.q1Label);
            this.groupBox1.Controls.Add(this.List2Label);
            this.groupBox1.Controls.Add(this.List1Label);
            this.groupBox1.Controls.Add(this.pictureBox6);
            this.groupBox1.Controls.Add(this.pictureBox5);
            this.groupBox1.Controls.Add(this.pictureBox4);
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.pictureBox8);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 208);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // subtractFromRadioButton
            // 
            this.subtractFromRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.subtractFromRadioButton.AutoSize = true;
            this.subtractFromRadioButton.Location = new System.Drawing.Point(242, 145);
            this.subtractFromRadioButton.Name = "subtractFromRadioButton";
            this.subtractFromRadioButton.Size = new System.Drawing.Size(136, 17);
            this.subtractFromRadioButton.TabIndex = 3;
            this.subtractFromRadioButton.TabStop = true;
            this.subtractFromRadioButton.Text = "Subtract from [ L2 - L1 ]";
            this.subtractFromRadioButton.UseVisualStyleBackColor = true;
            // 
            // unionRadioButton
            // 
            this.unionRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.unionRadioButton.AutoSize = true;
            this.unionRadioButton.Location = new System.Drawing.Point(242, 112);
            this.unionRadioButton.Name = "unionRadioButton";
            this.unionRadioButton.Size = new System.Drawing.Size(104, 17);
            this.unionRadioButton.TabIndex = 2;
            this.unionRadioButton.TabStop = true;
            this.unionRadioButton.Text = "Union [ L1 + L2 ]";
            this.unionRadioButton.UseVisualStyleBackColor = true;
            // 
            // subtractRadioButton
            // 
            this.subtractRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.subtractRadioButton.AutoSize = true;
            this.subtractRadioButton.Location = new System.Drawing.Point(242, 77);
            this.subtractRadioButton.Name = "subtractRadioButton";
            this.subtractRadioButton.Size = new System.Drawing.Size(113, 17);
            this.subtractRadioButton.TabIndex = 1;
            this.subtractRadioButton.TabStop = true;
            this.subtractRadioButton.Text = "Subtract [ L1 - L2 ]";
            this.subtractRadioButton.UseVisualStyleBackColor = true;
            // 
            // intersectRadioButton
            // 
            this.intersectRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.intersectRadioButton.AutoSize = true;
            this.intersectRadioButton.Location = new System.Drawing.Point(242, 43);
            this.intersectRadioButton.Name = "intersectRadioButton";
            this.intersectRadioButton.Size = new System.Drawing.Size(129, 17);
            this.intersectRadioButton.TabIndex = 0;
            this.intersectRadioButton.TabStop = true;
            this.intersectRadioButton.Text = "Intersect [ L1 and L2 ]";
            this.intersectRadioButton.UseVisualStyleBackColor = true;
            // 
            // q2Label
            // 
            this.q2Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.q2Label.Location = new System.Drawing.Point(43, 144);
            this.q2Label.Name = "q2Label";
            this.q2Label.Size = new System.Drawing.Size(153, 60);
            this.q2Label.TabIndex = 18;
            this.q2Label.Text = "Q";
            // 
            // q1Label
            // 
            this.q1Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.q1Label.Location = new System.Drawing.Point(43, 65);
            this.q1Label.Name = "q1Label";
            this.q1Label.Size = new System.Drawing.Size(153, 60);
            this.q1Label.TabIndex = 17;
            this.q1Label.Text = "Q";
            // 
            // List2Label
            // 
            this.List2Label.AutoSize = true;
            this.List2Label.Location = new System.Drawing.Point(43, 125);
            this.List2Label.Name = "List2Label";
            this.List2Label.Size = new System.Drawing.Size(22, 13);
            this.List2Label.TabIndex = 15;
            this.List2Label.Text = "L2:";
            // 
            // List1Label
            // 
            this.List1Label.AutoSize = true;
            this.List1Label.Location = new System.Drawing.Point(43, 47);
            this.List1Label.Name = "List1Label";
            this.List1Label.Size = new System.Drawing.Size(22, 13);
            this.List1Label.TabIndex = 14;
            this.List1Label.Text = "L1:";
            // 
            // pictureBox6
            // 
            this.pictureBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox6.Image = global::ChemBioViz.NET.Properties.Resources.rl4;
            this.pictureBox6.Location = new System.Drawing.Point(202, 144);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(32, 32);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox6.TabIndex = 13;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox5.Image = global::ChemBioViz.NET.Properties.Resources.rl6;
            this.pictureBox5.Location = new System.Drawing.Point(202, 109);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(32, 32);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox5.TabIndex = 12;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox4.Image = global::ChemBioViz.NET.Properties.Resources.rl5;
            this.pictureBox4.Location = new System.Drawing.Point(202, 73);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox4.TabIndex = 11;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3.Image = global::ChemBioViz.NET.Properties.Resources.rl2;
            this.pictureBox3.Location = new System.Drawing.Point(202, 37);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 10;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ChemBioViz.NET.Properties.Resources.rl3;
            this.pictureBox1.Location = new System.Drawing.Point(5, 117);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::ChemBioViz.NET.Properties.Resources.rl1;
            this.pictureBox8.Location = new System.Drawing.Point(5, 39);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(32, 32);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox8.TabIndex = 7;
            this.pictureBox8.TabStop = false;
            // 
            // MergeListDialog
            // 
            this.AcceptButton = this.OKultraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelUltraButton;
            this.ClientSize = new System.Drawing.Size(441, 261);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CancelUltraButton);
            this.Controls.Add(this.OKultraButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MergeListDialog";
            this.Text = "Merge Lists";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton OKultraButton;
        private Infragistics.Win.Misc.UltraButton CancelUltraButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.Label List1Label;
        private System.Windows.Forms.Label List2Label;
        private System.Windows.Forms.Label q1Label;
        private System.Windows.Forms.Label q2Label;
        private System.Windows.Forms.RadioButton subtractRadioButton;
        private System.Windows.Forms.RadioButton intersectRadioButton;
        private System.Windows.Forms.RadioButton subtractFromRadioButton;
        private System.Windows.Forms.RadioButton unionRadioButton;
    }
}