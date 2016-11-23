namespace ChemBioViz.NET
{
    partial class CBVChartPanel
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
            this.components = new System.ComponentModel.Container();
            this.linkLabelFilter = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxXAgg = new System.Windows.Forms.ComboBox();
            this.comboBoxXFld = new System.Windows.Forms.ComboBox();
            this.comboBoxX = new System.Windows.Forms.ComboBox();
            this.linkLabelProps = new System.Windows.Forms.LinkLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxYAgg = new System.Windows.Forms.ComboBox();
            this.comboBoxYFld = new System.Windows.Forms.ComboBox();
            this.comboBoxY = new System.Windows.Forms.ComboBox();
            this.linkLabelApply = new System.Windows.Forms.LinkLabel();
            this.linkLabelRevert = new System.Windows.Forms.LinkLabel();
            this.cBVChartControlBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cBVChartControlBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabelFilter
            // 
            this.linkLabelFilter.AutoSize = true;
            this.linkLabelFilter.Location = new System.Drawing.Point(18, 173);
            this.linkLabelFilter.Name = "linkLabelFilter";
            this.linkLabelFilter.Size = new System.Drawing.Size(51, 13);
            this.linkLabelFilter.TabIndex = 9;
            this.linkLabelFilter.TabStop = true;
            this.linkLabelFilter.Text = "Add Filter";
            this.linkLabelFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFilter_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.comboBoxXAgg);
            this.groupBox1.Controls.Add(this.comboBoxXFld);
            this.groupBox1.Controls.Add(this.comboBoxX);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(167, 76);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "X Axis";
            // 
            // comboBoxXAgg
            // 
            this.comboBoxXAgg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxXAgg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxXAgg.FormattingEnabled = true;
            this.comboBoxXAgg.Items.AddRange(new object[] {
            "",
            "MIN",
            "MAX",
            "AVG"});
            this.comboBoxXAgg.Location = new System.Drawing.Point(110, 46);
            this.comboBoxXAgg.Name = "comboBoxXAgg";
            this.comboBoxXAgg.Size = new System.Drawing.Size(48, 21);
            this.comboBoxXAgg.TabIndex = 1;
            this.comboBoxXAgg.SelectedIndexChanged += new System.EventHandler(this.any_SelectedIndexChanged);
            // 
            // comboBoxXFld
            // 
            this.comboBoxXFld.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxXFld.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxXFld.FormattingEnabled = true;
            this.comboBoxXFld.Location = new System.Drawing.Point(9, 46);
            this.comboBoxXFld.Name = "comboBoxXFld";
            this.comboBoxXFld.Size = new System.Drawing.Size(95, 21);
            this.comboBoxXFld.TabIndex = 1;
            this.comboBoxXFld.SelectedIndexChanged += new System.EventHandler(this.any_SelectedIndexChanged);
            this.comboBoxXFld.DropDown += new System.EventHandler(this.comboBoxXFld_DropDown);
            // 
            // comboBoxX
            // 
            this.comboBoxX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxX.FormattingEnabled = true;
            this.comboBoxX.Location = new System.Drawing.Point(9, 19);
            this.comboBoxX.Name = "comboBoxX";
            this.comboBoxX.Size = new System.Drawing.Size(149, 21);
            this.comboBoxX.TabIndex = 0;
            this.comboBoxX.SelectedIndexChanged += new System.EventHandler(this.comboBoxX_SelectedIndexChanged);
            // 
            // linkLabelProps
            // 
            this.linkLabelProps.AutoSize = true;
            this.linkLabelProps.Location = new System.Drawing.Point(125, 173);
            this.linkLabelProps.Name = "linkLabelProps";
            this.linkLabelProps.Size = new System.Drawing.Size(54, 13);
            this.linkLabelProps.TabIndex = 9;
            this.linkLabelProps.TabStop = true;
            this.linkLabelProps.Text = "Settings...";
            this.linkLabelProps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelProps_LinkClicked);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.comboBoxYAgg);
            this.groupBox2.Controls.Add(this.comboBoxYFld);
            this.groupBox2.Controls.Add(this.comboBoxY);
            this.groupBox2.Location = new System.Drawing.Point(12, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(167, 76);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Y Axis";
            // 
            // comboBoxYAgg
            // 
            this.comboBoxYAgg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxYAgg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxYAgg.FormattingEnabled = true;
            this.comboBoxYAgg.Items.AddRange(new object[] {
            "",
            "MIN",
            "MAX",
            "AVG"});
            this.comboBoxYAgg.Location = new System.Drawing.Point(110, 46);
            this.comboBoxYAgg.Name = "comboBoxYAgg";
            this.comboBoxYAgg.Size = new System.Drawing.Size(48, 21);
            this.comboBoxYAgg.TabIndex = 1;
            this.comboBoxYAgg.SelectedIndexChanged += new System.EventHandler(this.any_SelectedIndexChanged);
            // 
            // comboBoxYFld
            // 
            this.comboBoxYFld.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxYFld.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxYFld.FormattingEnabled = true;
            this.comboBoxYFld.Location = new System.Drawing.Point(9, 46);
            this.comboBoxYFld.Name = "comboBoxYFld";
            this.comboBoxYFld.Size = new System.Drawing.Size(95, 21);
            this.comboBoxYFld.TabIndex = 1;
            this.comboBoxYFld.SelectedIndexChanged += new System.EventHandler(this.any_SelectedIndexChanged);
            this.comboBoxYFld.DropDown += new System.EventHandler(this.comboBoxYFld_DropDown);
            // 
            // comboBoxY
            // 
            this.comboBoxY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxY.FormattingEnabled = true;
            this.comboBoxY.Location = new System.Drawing.Point(9, 19);
            this.comboBoxY.Name = "comboBoxY";
            this.comboBoxY.Size = new System.Drawing.Size(149, 21);
            this.comboBoxY.TabIndex = 0;
            this.comboBoxY.SelectedIndexChanged += new System.EventHandler(this.comboBoxY_SelectedIndexChanged);
            // 
            // linkLabelApply
            // 
            this.linkLabelApply.AutoSize = true;
            this.linkLabelApply.Location = new System.Drawing.Point(77, 173);
            this.linkLabelApply.Name = "linkLabelApply";
            this.linkLabelApply.Size = new System.Drawing.Size(33, 13);
            this.linkLabelApply.TabIndex = 9;
            this.linkLabelApply.TabStop = true;
            this.linkLabelApply.Text = "Apply";
            this.linkLabelApply.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelApply_LinkClicked);
            // 
            // linkLabelRevert
            // 
            this.linkLabelRevert.AutoSize = true;
            this.linkLabelRevert.Location = new System.Drawing.Point(85, 189);
            this.linkLabelRevert.Name = "linkLabelRevert";
            this.linkLabelRevert.Size = new System.Drawing.Size(39, 13);
            this.linkLabelRevert.TabIndex = 9;
            this.linkLabelRevert.TabStop = true;
            this.linkLabelRevert.Text = "Revert";
            this.linkLabelRevert.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRevert_LinkClicked);
            // 
            // cBVChartControlBindingSource
            // 
            this.cBVChartControlBindingSource.DataSource = typeof(CBVControls.CBVChartControl);
            // 
            // CBVChartPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(191, 215);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.linkLabelProps);
            this.Controls.Add(this.linkLabelRevert);
            this.Controls.Add(this.linkLabelApply);
            this.Controls.Add(this.linkLabelFilter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CBVChartPanel";
            this.Text = "CBVChartPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cBVChartControlBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelFilter;
        private System.Windows.Forms.BindingSource cBVChartControlBindingSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxXFld;
        private System.Windows.Forms.ComboBox comboBoxX;
        private System.Windows.Forms.LinkLabel linkLabelProps;
        private System.Windows.Forms.ComboBox comboBoxXAgg;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBoxYAgg;
        private System.Windows.Forms.ComboBox comboBoxYFld;
        private System.Windows.Forms.ComboBox comboBoxY;
        private System.Windows.Forms.LinkLabel linkLabelApply;
        private System.Windows.Forms.LinkLabel linkLabelRevert;
    }
}