namespace CBVNEmbed
{
    partial class Form1
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
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.formTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.queryTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ServerLabel = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.chemDataGrid1 = new ChemControls.ChemDataGrid();
            this.chemDraw1 = new ChemControls.ChemDraw();
            ((System.ComponentModel.ISupportInitialize)(this.chemDataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(467, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // loginTextBox
            // 
            this.loginTextBox.Location = new System.Drawing.Point(84, 23);
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(194, 20);
            this.loginTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Login";
            // 
            // formTextBox
            // 
            this.formTextBox.Location = new System.Drawing.Point(84, 71);
            this.formTextBox.Name = "formTextBox";
            this.formTextBox.Size = new System.Drawing.Size(194, 20);
            this.formTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Form";
            // 
            // queryTextBox
            // 
            this.queryTextBox.Location = new System.Drawing.Point(84, 95);
            this.queryTextBox.Name = "queryTextBox";
            this.queryTextBox.Size = new System.Drawing.Size(194, 20);
            this.queryTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Query";
            // 
            // ServerLabel
            // 
            this.ServerLabel.AutoSize = true;
            this.ServerLabel.Location = new System.Drawing.Point(24, 50);
            this.ServerLabel.Name = "ServerLabel";
            this.ServerLabel.Size = new System.Drawing.Size(38, 13);
            this.ServerLabel.TabIndex = 6;
            this.ServerLabel.Text = "Server";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(84, 47);
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(194, 20);
            this.serverTextBox.TabIndex = 5;
            // 
            // chemDataGrid1
            // 
            this.chemDataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chemDataGrid1.CellArrangementOptimization = false;
            this.chemDataGrid1.ChemDataProvider = null;
            this.chemDataGrid1.DisplayCustomizeButton = false;
            appearance30.BackColor = System.Drawing.SystemColors.Window;
            appearance30.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.chemDataGrid1.DisplayLayout.Appearance = appearance30;
            this.chemDataGrid1.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.chemDataGrid1.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance27.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance27.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance27.BorderColor = System.Drawing.SystemColors.Window;
            this.chemDataGrid1.DisplayLayout.GroupByBox.Appearance = appearance27;
            appearance28.ForeColor = System.Drawing.SystemColors.GrayText;
            this.chemDataGrid1.DisplayLayout.GroupByBox.BandLabelAppearance = appearance28;
            this.chemDataGrid1.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.chemDataGrid1.DisplayLayout.GroupByBox.Hidden = true;
            appearance29.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance29.BackColor2 = System.Drawing.SystemColors.Control;
            appearance29.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance29.ForeColor = System.Drawing.SystemColors.GrayText;
            this.chemDataGrid1.DisplayLayout.GroupByBox.PromptAppearance = appearance29;
            this.chemDataGrid1.DisplayLayout.MaxColScrollRegions = 1;
            this.chemDataGrid1.DisplayLayout.MaxRowScrollRegions = 1;
            appearance38.BackColor = System.Drawing.SystemColors.Window;
            appearance38.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chemDataGrid1.DisplayLayout.Override.ActiveCellAppearance = appearance38;
            appearance33.BackColor = System.Drawing.SystemColors.Highlight;
            appearance33.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.chemDataGrid1.DisplayLayout.Override.ActiveRowAppearance = appearance33;
            this.chemDataGrid1.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.chemDataGrid1.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.chemDataGrid1.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.chemDataGrid1.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.chemDataGrid1.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.chemDataGrid1.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance32.BackColor = System.Drawing.SystemColors.Window;
            this.chemDataGrid1.DisplayLayout.Override.CardAreaAppearance = appearance32;
            appearance31.BackColor = System.Drawing.Color.White;
            appearance31.BorderColor = System.Drawing.Color.Silver;
            appearance31.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.chemDataGrid1.DisplayLayout.Override.CellAppearance = appearance31;
            this.chemDataGrid1.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.chemDataGrid1.DisplayLayout.Override.CellPadding = 0;
            appearance35.BackColor = System.Drawing.SystemColors.Control;
            appearance35.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance35.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance35.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance35.BorderColor = System.Drawing.SystemColors.Window;
            this.chemDataGrid1.DisplayLayout.Override.GroupByRowAppearance = appearance35;
            appearance37.TextHAlignAsString = "Left";
            this.chemDataGrid1.DisplayLayout.Override.HeaderAppearance = appearance37;
            this.chemDataGrid1.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.chemDataGrid1.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            appearance36.BorderColor = System.Drawing.Color.Silver;
            this.chemDataGrid1.DisplayLayout.Override.RowAppearance = appearance36;
            this.chemDataGrid1.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance34.BackColor = System.Drawing.SystemColors.ControlLight;
            this.chemDataGrid1.DisplayLayout.Override.TemplateAddRowAppearance = appearance34;
            this.chemDataGrid1.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.chemDataGrid1.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.chemDataGrid1.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.chemDataGrid1.HasChemDrawColumn = false;
            this.chemDataGrid1.LabelsInHeader = true;
            this.chemDataGrid1.Location = new System.Drawing.Point(12, 133);
            this.chemDataGrid1.Name = "chemDataGrid1";
            this.chemDataGrid1.Size = new System.Drawing.Size(555, 251);
            this.chemDataGrid1.TabIndex = 8;
            this.chemDataGrid1.Text = "chemDataGrid1";
            // 
            // chemDraw1
            // 
            this.chemDraw1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chemDraw1.Base64 = resources.GetString("chemDraw1.Base64");
            this.chemDraw1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chemDraw1.ChemDrawDocument = new byte[] {
        ((byte)(86)),
        ((byte)(106)),
        ((byte)(67)),
        ((byte)(68)),
        ((byte)(48)),
        ((byte)(49)),
        ((byte)(48)),
        ((byte)(48)),
        ((byte)(4)),
        ((byte)(3)),
        ((byte)(2)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(128)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(0)),
        ((byte)(15)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(67)),
        ((byte)(104)),
        ((byte)(101)),
        ((byte)(109)),
        ((byte)(68)),
        ((byte)(114)),
        ((byte)(97)),
        ((byte)(119)),
        ((byte)(32)),
        ((byte)(49)),
        ((byte)(50)),
        ((byte)(46)),
        ((byte)(48)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(19)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(85)),
        ((byte)(110)),
        ((byte)(116)),
        ((byte)(105)),
        ((byte)(116)),
        ((byte)(108)),
        ((byte)(101)),
        ((byte)(100)),
        ((byte)(32)),
        ((byte)(68)),
        ((byte)(111)),
        ((byte)(99)),
        ((byte)(117)),
        ((byte)(109)),
        ((byte)(101)),
        ((byte)(110)),
        ((byte)(116)),
        ((byte)(4)),
        ((byte)(2)),
        ((byte)(16)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(9)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(2)),
        ((byte)(9)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(84)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(64)),
        ((byte)(131)),
        ((byte)(0)),
        ((byte)(13)),
        ((byte)(8)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(8)),
        ((byte)(7)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(58)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(59)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(69)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(60)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(12)),
        ((byte)(6)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(15)),
        ((byte)(6)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(13)),
        ((byte)(6)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(66)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(67)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(68)),
        ((byte)(4)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(10)),
        ((byte)(8)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(0)),
        ((byte)(96)),
        ((byte)(0)),
        ((byte)(200)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(0)),
        ((byte)(11)),
        ((byte)(8)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(240)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(0)),
        ((byte)(9)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(51)),
        ((byte)(179)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(8)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(7)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(6)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(5)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(30)),
        ((byte)(0)),
        ((byte)(4)),
        ((byte)(8)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(120)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(120)),
        ((byte)(0)),
        ((byte)(35)),
        ((byte)(8)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(5)),
        ((byte)(12)),
        ((byte)(8)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(40)),
        ((byte)(8)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(41)),
        ((byte)(8)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(42)),
        ((byte)(8)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(2)),
        ((byte)(8)),
        ((byte)(16)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(3)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(2)),
        ((byte)(3)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(50)),
        ((byte)(0)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(255)),
        ((byte)(255)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(3)),
        ((byte)(0)),
        ((byte)(228)),
        ((byte)(4)),
        ((byte)(5)),
        ((byte)(0)),
        ((byte)(65)),
        ((byte)(114)),
        ((byte)(105)),
        ((byte)(97)),
        ((byte)(108)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(228)),
        ((byte)(4)),
        ((byte)(15)),
        ((byte)(0)),
        ((byte)(84)),
        ((byte)(105)),
        ((byte)(109)),
        ((byte)(101)),
        ((byte)(115)),
        ((byte)(32)),
        ((byte)(78)),
        ((byte)(101)),
        ((byte)(119)),
        ((byte)(32)),
        ((byte)(82)),
        ((byte)(111)),
        ((byte)(109)),
        ((byte)(97)),
        ((byte)(110)),
        ((byte)(1)),
        ((byte)(128)),
        ((byte)(3)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(4)),
        ((byte)(2)),
        ((byte)(16)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(208)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(28)),
        ((byte)(2)),
        ((byte)(22)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(24)),
        ((byte)(8)),
        ((byte)(4)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(36)),
        ((byte)(0)),
        ((byte)(25)),
        ((byte)(8)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(16)),
        ((byte)(8)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(15)),
        ((byte)(8)),
        ((byte)(2)),
        ((byte)(0)),
        ((byte)(1)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0)),
        ((byte)(0))};
            this.chemDraw1.Location = new System.Drawing.Point(284, 13);
            this.chemDraw1.Name = "chemDraw1";
            this.chemDraw1.ReadOnly = false;
            this.chemDraw1.Size = new System.Drawing.Size(177, 114);
            this.chemDraw1.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 397);
            this.Controls.Add(this.chemDraw1);
            this.Controls.Add(this.chemDataGrid1);
            this.Controls.Add(this.ServerLabel);
            this.Controls.Add(this.serverTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.queryTextBox);
            this.Controls.Add(this.formTextBox);
            this.Controls.Add(this.loginTextBox);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "ChemBioViz Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.chemDataGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox formTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox queryTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ServerLabel;
        private System.Windows.Forms.TextBox serverTextBox;
        private ChemControls.ChemDataGrid chemDataGrid1;
        private ChemControls.ChemDraw chemDraw1;
    }
}

