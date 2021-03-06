﻿namespace CBVNStructureFilter
{
    partial class StructureFilterPanelControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            this.rendererToolStripMenuItem = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonConfigure = new System.Windows.Forms.ToolStripButton();
            this.searchTypePanel = new System.Windows.Forms.Panel();
            this.searchOption = new System.Windows.Forms.ComboBox();
            this.minLabel = new System.Windows.Forms.Label();
            this.percent = new System.Windows.Forms.Label();
            this.percentLabel = new System.Windows.Forms.TextBox();
            this.rGroupDecomposition = new System.Windows.Forms.CheckBox();
            this.simPercent = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.progressLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.rgdPanel = new System.Windows.Forms.Panel();
            this.MoleculeImage = new System.Windows.Forms.PictureBox();
            this.noStructureColumnsLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.helpProvider = new System.Windows.Forms.HelpProvider();
            this.toolStrip1.SuspendLayout();
            this.searchTypePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.simPercent)).BeginInit();
            this.panel2.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.rgdPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MoleculeImage)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEdit,
            this.toolStripButtonClear,
            this.rendererToolStripMenuItem,
            this.toolStripButtonConfigure});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(191, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.Image = global::Spotfire.Dxp.LeadDiscovery.StructureFilter.Properties.Resources.edit1;
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(29, 22);
            this.toolStripButtonEdit.ToolTipText = "Edit filter";
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClear.Image = global::Spotfire.Dxp.LeadDiscovery.StructureFilter.Properties.Resources.clear;
            this.toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClear.Text = "toolStripButton1";
            this.toolStripButtonClear.ToolTipText = "Clear filter";
            this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
            // 
            // rendererToolStripMenuItem
            // 
            this.rendererToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rendererToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.settingsToolStripMenuItem});
            this.rendererToolStripMenuItem.Image = global::Spotfire.Dxp.LeadDiscovery.StructureFilter.Properties.Resources.renderer2;
            this.rendererToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rendererToolStripMenuItem.Name = "rendererToolStripMenuItem";
            this.rendererToolStripMenuItem.Size = new System.Drawing.Size(29, 22);
            this.rendererToolStripMenuItem.Text = "Renderer";
            this.rendererToolStripMenuItem.ToolTipText = "Renderer settings";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.settingsToolStripMenuItem.Text = "Settings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripButtonConfigure
            // 
            this.toolStripButtonConfigure.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConfigure.Image = global::Spotfire.Dxp.LeadDiscovery.StructureFilter.Properties.Resources.configure;
            this.toolStripButtonConfigure.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConfigure.Name = "toolStripButtonConfigure";
            this.toolStripButtonConfigure.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonConfigure.Text = "Configure";
            this.toolStripButtonConfigure.ToolTipText = "Configure source";
            this.toolStripButtonConfigure.Click += new System.EventHandler(this.toolStripButtonConfigure_Click);
            // 
            // searchTypePanel
            // 
            this.searchTypePanel.AutoSize = true;
            this.searchTypePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.searchTypePanel.Controls.Add(this.searchOption);
            this.searchTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTypePanel.Location = new System.Drawing.Point(3, 0);
            this.searchTypePanel.Name = "searchTypePanel";
            this.searchTypePanel.Size = new System.Drawing.Size(177, 24);
            this.searchTypePanel.TabIndex = 5;
            this.searchTypePanel.Click += new System.EventHandler(this.searchTypePanel_Click);
            // 
            // searchOption
            // 
            this.searchOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchOption.FormattingEnabled = true;
            this.searchOption.Location = new System.Drawing.Point(0, 0);
            this.searchOption.Name = "searchOption";
            this.searchOption.Size = new System.Drawing.Size(177, 21);
            this.searchOption.TabIndex = 5;
            this.searchOption.Click += new System.EventHandler(this.searchOption_Click);
            // 
            // minLabel
            // 
            this.minLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minLabel.AutoSize = true;
            this.minLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLabel.Location = new System.Drawing.Point(137, 7);
            this.minLabel.Name = "minLabel";
            this.minLabel.Size = new System.Drawing.Size(37, 13);
            this.minLabel.TabIndex = 11;
            this.minLabel.Text = "100 %";
            this.minLabel.Visible = false;
            this.minLabel.Click += new System.EventHandler(this.minLabel_Click);
            // 
            // percent
            // 
            this.percent.AutoSize = true;
            this.percent.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percent.Location = new System.Drawing.Point(21, 7);
            this.percent.Name = "percent";
            this.percent.Size = new System.Drawing.Size(15, 13);
            this.percent.TabIndex = 10;
            this.percent.Text = "%";
            this.percent.Visible = false;
            this.percent.Click += new System.EventHandler(this.percentLabel_Click);
            // 
            // percentLabel
            // 
            this.percentLabel.BackColor = System.Drawing.SystemColors.Window;
            this.percentLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.percentLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.percentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentLabel.Location = new System.Drawing.Point(3, 8);
            this.percentLabel.MaxLength = 3;
            this.percentLabel.Name = "percentLabel";
            this.percentLabel.ReadOnly = true;
            this.percentLabel.Size = new System.Drawing.Size(17, 12);
            this.percentLabel.TabIndex = 9;
            this.percentLabel.Text = "90";
            this.percentLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.percentLabel.Visible = false;
            this.percentLabel.Click += new System.EventHandler(this.percentLabel_Click);
            this.percentLabel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.percentLabel_KeyPress);
            this.percentLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.percentLabel_MouseDoubleClick);
            // 
            // rGroupDecomposition
            // 
            this.rGroupDecomposition.AutoSize = true;
            this.rGroupDecomposition.Enabled = false;
            this.rGroupDecomposition.Location = new System.Drawing.Point(0, 3);
            this.rGroupDecomposition.Name = "rGroupDecomposition";
            this.rGroupDecomposition.Size = new System.Drawing.Size(139, 17);
            this.rGroupDecomposition.TabIndex = 8;
            this.rGroupDecomposition.Text = "R-Group Decomposition";
            this.rGroupDecomposition.UseVisualStyleBackColor = true;
            this.rGroupDecomposition.Visible = false;
            this.rGroupDecomposition.CheckedChanged += new System.EventHandler(this.rGroupDecomposition_CheckedChanged);
            this.rGroupDecomposition.Click += new System.EventHandler(this.rGroupDecomposition_Click);
            // 
            // simPercent
            // 
            this.simPercent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.simPercent.Location = new System.Drawing.Point(3, 20);
            this.simPercent.Maximum = 100;
            this.simPercent.Name = "simPercent";
            this.simPercent.Size = new System.Drawing.Size(162, 42);
            this.simPercent.TabIndex = 6;
            this.simPercent.TickFrequency = 5;
            this.simPercent.Visible = false;
            this.simPercent.Scroll += new System.EventHandler(this.simPercent_Scroll);
            this.simPercent.ValueChanged += new System.EventHandler(this.simPercent_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.Panel1);
            this.panel2.Controls.Add(this.rgdPanel);
            this.panel2.Controls.Add(this.MoleculeImage);
            this.panel2.Controls.Add(this.searchTypePanel);
            this.panel2.Controls.Add(this.noStructureColumnsLabel);
            this.panel2.Location = new System.Drawing.Point(0, 26);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.panel2.Size = new System.Drawing.Size(183, 261);
            this.panel2.TabIndex = 6;
            // 
            // Panel1
            // 
            this.Panel1.AutoSize = true;
            this.Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.progressLabel);
            this.Panel1.Controls.Add(this.button1);
            this.Panel1.Controls.Add(this.progressBar1);
            this.Panel1.Location = new System.Drawing.Point(9, 52);
            this.Panel1.Name = "Panel1";
            this.Panel1.Padding = new System.Windows.Forms.Padding(4, 0, 4, 4);
            this.Panel1.Size = new System.Drawing.Size(164, 47);
            this.Panel1.TabIndex = 8;
            this.Panel1.Visible = false;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(10, 4);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(13, 13);
            this.progressLabel.TabIndex = 4;
            this.progressLabel.Text = "5";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Image = global::Spotfire.Dxp.LeadDiscovery.StructureFilter.Properties.Resources.cancel;
            this.button1.Location = new System.Drawing.Point(140, 21);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(18, 18);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 22);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(130, 16);
            this.progressBar1.TabIndex = 0;
            // 
            // rgdPanel
            // 
            this.rgdPanel.AutoSize = true;
            this.rgdPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rgdPanel.Controls.Add(this.minLabel);
            this.rgdPanel.Controls.Add(this.percent);
            this.rgdPanel.Controls.Add(this.percentLabel);
            this.rgdPanel.Controls.Add(this.rGroupDecomposition);
            this.rgdPanel.Controls.Add(this.simPercent);
            this.rgdPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.rgdPanel.Location = new System.Drawing.Point(3, 154);
            this.rgdPanel.Name = "rgdPanel";
            this.rgdPanel.Size = new System.Drawing.Size(177, 65);
            this.rgdPanel.TabIndex = 7;
            this.rgdPanel.Click += new System.EventHandler(this.rgdPanel_Click);
            // 
            // MoleculeImage
            // 
            this.MoleculeImage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.MoleculeImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MoleculeImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.MoleculeImage.Location = new System.Drawing.Point(3, 24);
            this.MoleculeImage.Name = "MoleculeImage";
            this.MoleculeImage.Size = new System.Drawing.Size(177, 130);
            this.MoleculeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MoleculeImage.TabIndex = 6;
            this.MoleculeImage.TabStop = false;
            this.MoleculeImage.SizeChanged += new System.EventHandler(this.MoleculeImage_SizeChanged);
            this.MoleculeImage.Click += new System.EventHandler(this.MoleculeImage_Click);
            this.MoleculeImage.DoubleClick += new System.EventHandler(this.MoleculeImage_DoubleClick);
            // 
            // noStructureColumnsLabel
            // 
            this.noStructureColumnsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.noStructureColumnsLabel.Location = new System.Drawing.Point(3, 0);
            this.noStructureColumnsLabel.Name = "noStructureColumnsLabel";
            this.noStructureColumnsLabel.Padding = new System.Windows.Forms.Padding(5);
            this.noStructureColumnsLabel.Size = new System.Drawing.Size(177, 261);
            this.noStructureColumnsLabel.TabIndex = 6;
            this.noStructureColumnsLabel.Text = "noStructureColumnsLabel";
            this.noStructureColumnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.noStructureColumnsLabel.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // StructureFilterPanelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel2);
            this.Name = "StructureFilterPanelControl";
            this.Size = new System.Drawing.Size(191, 292);
            this.Click += new System.EventHandler(this.StructureFilterPanelControl_Click);
            this.Leave += new System.EventHandler(this.StructureFilterPanelControl_Leave);
            this.Resize += new System.EventHandler(this.StructureFilterPanelControl_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.searchTypePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.simPercent)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.rgdPanel.ResumeLayout(false);
            this.rgdPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MoleculeImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton rendererToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonConfigure;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.Panel searchTypePanel;
        private System.Windows.Forms.TrackBar simPercent;
        private System.Windows.Forms.ComboBox searchOption;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox rGroupDecomposition;
        private System.Windows.Forms.TextBox percentLabel;
        private System.Windows.Forms.Label percent;
        private System.Windows.Forms.PictureBox MoleculeImage;
        private System.Windows.Forms.Label minLabel;
        private System.Windows.Forms.Panel Panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Panel rgdPanel;
        private System.Windows.Forms.Label noStructureColumnsLabel;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.HelpProvider helpProvider;
    }
}
