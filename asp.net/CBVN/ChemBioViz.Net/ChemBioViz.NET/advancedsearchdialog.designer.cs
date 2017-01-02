namespace ChemBioViz.NET
{
    partial class AdvancedSearchDialog
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSearchDialog));
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mustHitRxnCtrCheckBox = new System.Windows.Forms.CheckBox();
            this.hitAnyChargeCarbonCheckBox = new System.Windows.Forms.CheckBox();
            this.hitAnyChargeHeteroCheckBox = new System.Windows.Forms.CheckBox();
            this.extraFragsCheckBox = new System.Windows.Forms.CheckBox();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.matchStereochemistryCheckBox = new System.Windows.Forms.CheckBox();
            this.doubleBondGroupBox = new System.Windows.Forms.GroupBox();
            this.anyDoubleBondStereoButton = new System.Windows.Forms.RadioButton();
            this.sameDoubleBondStereoButton = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.tetrahedralStereoGroupBox = new System.Windows.Forms.GroupBox();
            this.anyTetStereoButton = new System.Windows.Forms.RadioButton();
            this.eitherTetStereoButton = new System.Windows.Forms.RadioButton();
            this.sameTetStereoButton = new System.Windows.Forms.RadioButton();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.ultraTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.CancelUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.OKUltraButton = new Infragistics.Win.Misc.UltraButton();
            this.ultraTabPageControl1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.ultraTabPageControl4.SuspendLayout();
            this.doubleBondGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            this.tetrahedralStereoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl)).BeginInit();
            this.ultraTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.groupBox3);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(370, 287);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.mustHitRxnCtrCheckBox);
            this.groupBox3.Controls.Add(this.hitAnyChargeCarbonCheckBox);
            this.groupBox3.Controls.Add(this.hitAnyChargeHeteroCheckBox);
            this.groupBox3.Controls.Add(this.extraFragsCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(11, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(345, 132);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Options";
            // 
            // mustHitRxnCtrCheckBox
            // 
            this.mustHitRxnCtrCheckBox.AutoSize = true;
            this.mustHitRxnCtrCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mustHitRxnCtrCheckBox.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.ReactionCenter;
            this.mustHitRxnCtrCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ChemBioViz.NET.SearchOptionsSettings.Default, "ReactionCenter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.mustHitRxnCtrCheckBox.Location = new System.Drawing.Point(15, 62);
            this.mustHitRxnCtrCheckBox.Name = "mustHitRxnCtrCheckBox";
            this.mustHitRxnCtrCheckBox.Size = new System.Drawing.Size(211, 17);
            this.mustHitRxnCtrCheckBox.TabIndex = 2;
            this.mustHitRxnCtrCheckBox.Text = "Reaction query must hit reaction center";
            this.mustHitRxnCtrCheckBox.UseVisualStyleBackColor = false;
            // 
            // hitAnyChargeCarbonCheckBox
            // 
            this.hitAnyChargeCarbonCheckBox.AutoSize = true;
            this.hitAnyChargeCarbonCheckBox.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.HitAnyChargeCarbon;
            this.hitAnyChargeCarbonCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ChemBioViz.NET.SearchOptionsSettings.Default, "HitAnyChargeCarbon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hitAnyChargeCarbonCheckBox.Location = new System.Drawing.Point(215, 39);
            this.hitAnyChargeCarbonCheckBox.Name = "hitAnyChargeCarbonCheckBox";
            this.hitAnyChargeCarbonCheckBox.Size = new System.Drawing.Size(76, 17);
            this.hitAnyChargeCarbonCheckBox.TabIndex = 1;
            this.hitAnyChargeCarbonCheckBox.Text = "On carbon";
            this.hitAnyChargeCarbonCheckBox.UseVisualStyleBackColor = true;
            // 
            // hitAnyChargeHeteroCheckBox
            // 
            this.hitAnyChargeHeteroCheckBox.AutoSize = true;
            this.hitAnyChargeHeteroCheckBox.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.HitAnyChargeHetero;
            this.hitAnyChargeHeteroCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ChemBioViz.NET.SearchOptionsSettings.Default, "HitAnyChargeHetero", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hitAnyChargeHeteroCheckBox.Location = new System.Drawing.Point(15, 39);
            this.hitAnyChargeHeteroCheckBox.Name = "hitAnyChargeHeteroCheckBox";
            this.hitAnyChargeHeteroCheckBox.Size = new System.Drawing.Size(166, 17);
            this.hitAnyChargeHeteroCheckBox.TabIndex = 0;
            this.hitAnyChargeHeteroCheckBox.Text = "Hit any charge on heteroatom";
            this.hitAnyChargeHeteroCheckBox.UseVisualStyleBackColor = true;
            // 
            // extraFragsCheckBox
            // 
            this.extraFragsCheckBox.AutoSize = true;
            this.extraFragsCheckBox.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.PermitExtraneousFragments;
            this.extraFragsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ChemBioViz.NET.SearchOptionsSettings.Default, "PermitExtraneousFragments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.extraFragsCheckBox.Location = new System.Drawing.Point(15, 85);
            this.extraFragsCheckBox.Name = "extraFragsCheckBox";
            this.extraFragsCheckBox.Size = new System.Drawing.Size(329, 17);
            this.extraFragsCheckBox.TabIndex = 3;
            this.extraFragsCheckBox.Text = "Permit extraneous fragments in full structure or reaction searches";
            this.extraFragsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.matchStereochemistryCheckBox);
            this.ultraTabPageControl4.Controls.Add(this.doubleBondGroupBox);
            this.ultraTabPageControl4.Controls.Add(this.tetrahedralStereoGroupBox);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(370, 287);
            // 
            // matchStereochemistryCheckBox
            // 
            this.matchStereochemistryCheckBox.AutoSize = true;
            this.matchStereochemistryCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.matchStereochemistryCheckBox.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.MatchStereochemistry;
            this.matchStereochemistryCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ChemBioViz.NET.SearchOptionsSettings.Default, "MatchStereochemistry", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.matchStereochemistryCheckBox.Location = new System.Drawing.Point(139, 11);
            this.matchStereochemistryCheckBox.Name = "matchStereochemistryCheckBox";
            this.matchStereochemistryCheckBox.Size = new System.Drawing.Size(132, 17);
            this.matchStereochemistryCheckBox.TabIndex = 5;
            this.matchStereochemistryCheckBox.Text = "Match stereochemistry";
            this.matchStereochemistryCheckBox.UseVisualStyleBackColor = false;
            this.matchStereochemistryCheckBox.CheckedChanged += new System.EventHandler(this.matchStereochemistryCheckBox_CheckedChanged);
            // 
            // doubleBondGroupBox
            // 
            this.doubleBondGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.doubleBondGroupBox.Controls.Add(this.anyDoubleBondStereoButton);
            this.doubleBondGroupBox.Controls.Add(this.sameDoubleBondStereoButton);
            this.doubleBondGroupBox.Controls.Add(this.pictureBox1);
            this.doubleBondGroupBox.Controls.Add(this.pictureBox15);
            this.doubleBondGroupBox.Controls.Add(this.pictureBox6);
            this.doubleBondGroupBox.Controls.Add(this.pictureBox5);
            this.doubleBondGroupBox.Controls.Add(this.pictureBox11);
            this.doubleBondGroupBox.Location = new System.Drawing.Point(11, 175);
            this.doubleBondGroupBox.Name = "doubleBondGroupBox";
            this.doubleBondGroupBox.Size = new System.Drawing.Size(345, 97);
            this.doubleBondGroupBox.TabIndex = 6;
            this.doubleBondGroupBox.TabStop = false;
            this.doubleBondGroupBox.Text = "Double bond hits:";
            // 
            // anyDoubleBondStereoButton
            // 
            this.anyDoubleBondStereoButton.AutoSize = true;
            this.anyDoubleBondStereoButton.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.AnyDoubleBondStereo;
            this.anyDoubleBondStereoButton.Location = new System.Drawing.Point(128, 60);
            this.anyDoubleBondStereoButton.Name = "anyDoubleBondStereoButton";
            this.anyDoubleBondStereoButton.Size = new System.Drawing.Size(43, 17);
            this.anyDoubleBondStereoButton.TabIndex = 10;
            this.anyDoubleBondStereoButton.TabStop = true;
            this.anyDoubleBondStereoButton.Text = "Any";
            this.anyDoubleBondStereoButton.UseVisualStyleBackColor = true;
            // 
            // sameDoubleBondStereoButton
            // 
            this.sameDoubleBondStereoButton.AutoSize = true;
            this.sameDoubleBondStereoButton.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.SameDoubleBondStereo;
            this.sameDoubleBondStereoButton.Location = new System.Drawing.Point(128, 28);
            this.sameDoubleBondStereoButton.Name = "sameDoubleBondStereoButton";
            this.sameDoubleBondStereoButton.Size = new System.Drawing.Size(52, 17);
            this.sameDoubleBondStereoButton.TabIndex = 9;
            this.sameDoubleBondStereoButton.TabStop = true;
            this.sameDoubleBondStereoButton.Text = "Same";
            this.sameDoubleBondStereoButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ChemBioViz.NET.Properties.Resources.std_either;
            this.pictureBox1.Location = new System.Drawing.Point(271, 59);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 25);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox15
            // 
            this.pictureBox15.Image = global::ChemBioViz.NET.Properties.Resources.std_cis;
            this.pictureBox15.Location = new System.Drawing.Point(195, 59);
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.Size = new System.Drawing.Size(32, 25);
            this.pictureBox15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox15.TabIndex = 0;
            this.pictureBox15.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = global::ChemBioViz.NET.Properties.Resources.std_cis;
            this.pictureBox6.Location = new System.Drawing.Point(195, 23);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(32, 25);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox6.TabIndex = 0;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::ChemBioViz.NET.Properties.Resources.std_trans;
            this.pictureBox5.Location = new System.Drawing.Point(233, 59);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(32, 25);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox5.TabIndex = 0;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox11
            // 
            this.pictureBox11.Image = global::ChemBioViz.NET.Properties.Resources.std_cis;
            this.pictureBox11.Location = new System.Drawing.Point(53, 19);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Size = new System.Drawing.Size(32, 25);
            this.pictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox11.TabIndex = 0;
            this.pictureBox11.TabStop = false;
            // 
            // tetrahedralStereoGroupBox
            // 
            this.tetrahedralStereoGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.tetrahedralStereoGroupBox.Controls.Add(this.anyTetStereoButton);
            this.tetrahedralStereoGroupBox.Controls.Add(this.eitherTetStereoButton);
            this.tetrahedralStereoGroupBox.Controls.Add(this.sameTetStereoButton);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox10);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox13);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox12);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox8);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox14);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox2);
            this.tetrahedralStereoGroupBox.Controls.Add(this.pictureBox3);
            this.tetrahedralStereoGroupBox.Location = new System.Drawing.Point(11, 34);
            this.tetrahedralStereoGroupBox.Name = "tetrahedralStereoGroupBox";
            this.tetrahedralStereoGroupBox.Size = new System.Drawing.Size(345, 135);
            this.tetrahedralStereoGroupBox.TabIndex = 2;
            this.tetrahedralStereoGroupBox.TabStop = false;
            this.tetrahedralStereoGroupBox.Text = "Tetrahedral stereo center hits:";
            // 
            // anyTetStereoButton
            // 
            this.anyTetStereoButton.AutoSize = true;
            this.anyTetStereoButton.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.AnyTetStereo;
            this.anyTetStereoButton.Location = new System.Drawing.Point(128, 97);
            this.anyTetStereoButton.Name = "anyTetStereoButton";
            this.anyTetStereoButton.Size = new System.Drawing.Size(43, 17);
            this.anyTetStereoButton.TabIndex = 8;
            this.anyTetStereoButton.TabStop = true;
            this.anyTetStereoButton.Text = "Any";
            this.anyTetStereoButton.UseVisualStyleBackColor = true;
            // 
            // eitherTetStereoButton
            // 
            this.eitherTetStereoButton.AutoSize = true;
            this.eitherTetStereoButton.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.EitherTetStereo;
            this.eitherTetStereoButton.Location = new System.Drawing.Point(128, 62);
            this.eitherTetStereoButton.Name = "eitherTetStereoButton";
            this.eitherTetStereoButton.Size = new System.Drawing.Size(52, 17);
            this.eitherTetStereoButton.TabIndex = 7;
            this.eitherTetStereoButton.TabStop = true;
            this.eitherTetStereoButton.Text = "Either";
            this.eitherTetStereoButton.UseVisualStyleBackColor = true;
            // 
            // sameTetStereoButton
            // 
            this.sameTetStereoButton.AutoSize = true;
            this.sameTetStereoButton.Checked = global::ChemBioViz.NET.SearchOptionsSettings.Default.SameTetStereo;
            this.sameTetStereoButton.Location = new System.Drawing.Point(128, 27);
            this.sameTetStereoButton.Name = "sameTetStereoButton";
            this.sameTetStereoButton.Size = new System.Drawing.Size(52, 17);
            this.sameTetStereoButton.TabIndex = 6;
            this.sameTetStereoButton.TabStop = true;
            this.sameTetStereoButton.Text = "Same";
            this.sameTetStereoButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::ChemBioViz.NET.Properties.Resources.st_up;
            this.pictureBox10.Location = new System.Drawing.Point(53, 19);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(32, 29);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox10.TabIndex = 0;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox13
            // 
            this.pictureBox13.Image = global::ChemBioViz.NET.Properties.Resources.st_up;
            this.pictureBox13.Location = new System.Drawing.Point(195, 89);
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.Size = new System.Drawing.Size(32, 29);
            this.pictureBox13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox13.TabIndex = 0;
            this.pictureBox13.TabStop = false;
            // 
            // pictureBox12
            // 
            this.pictureBox12.Image = global::ChemBioViz.NET.Properties.Resources.st_up;
            this.pictureBox12.Location = new System.Drawing.Point(195, 54);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Size = new System.Drawing.Size(32, 29);
            this.pictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox12.TabIndex = 0;
            this.pictureBox12.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::ChemBioViz.NET.Properties.Resources.st_up;
            this.pictureBox8.Location = new System.Drawing.Point(195, 19);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(32, 29);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox8.TabIndex = 0;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox14
            // 
            this.pictureBox14.Image = global::ChemBioViz.NET.Properties.Resources.st_down;
            this.pictureBox14.Location = new System.Drawing.Point(233, 89);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Size = new System.Drawing.Size(32, 29);
            this.pictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox14.TabIndex = 0;
            this.pictureBox14.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ChemBioViz.NET.Properties.Resources.st_any;
            this.pictureBox2.Location = new System.Drawing.Point(271, 89);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 29);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ChemBioViz.NET.Properties.Resources.st_down;
            this.pictureBox3.Location = new System.Drawing.Point(233, 54);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 29);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // ultraTabControl
            // 
            this.ultraTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl.Controls.Add(this.ultraTabPageControl4);
            this.ultraTabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraTabControl.Location = new System.Drawing.Point(0, 0);
            this.ultraTabControl.Margin = new System.Windows.Forms.Padding(8);
            this.ultraTabControl.Name = "ultraTabControl";
            this.ultraTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl.Size = new System.Drawing.Size(374, 313);
            this.ultraTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.PropertyPage;
            this.ultraTabControl.TabIndex = 4;
            this.ultraTabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            ultraTab1.Key = "General";
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "General";
            ultraTab4.Key = "Stereochemistry";
            ultraTab4.TabPage = this.ultraTabPageControl4;
            ultraTab4.Text = "Stereochemistry";
            ultraTab4.ToolTipText = "Apply style to the application.";
            this.ultraTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab4});
            this.ultraTabControl.UseHotTracking = Infragistics.Win.DefaultableBoolean.False;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(370, 287);
            // 
            // CancelUltraButton
            // 
            this.CancelUltraButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelUltraButton.Location = new System.Drawing.Point(246, 320);
            this.CancelUltraButton.Name = "CancelUltraButton";
            this.CancelUltraButton.Size = new System.Drawing.Size(75, 23);
            this.CancelUltraButton.TabIndex = 12;
            this.CancelUltraButton.Text = "Cancel";
            this.CancelUltraButton.Click += new System.EventHandler(this.CancelUltraButton_Click);
            // 
            // OKUltraButton
            // 
            this.OKUltraButton.Location = new System.Drawing.Point(164, 320);
            this.OKUltraButton.Name = "OKUltraButton";
            this.OKUltraButton.Size = new System.Drawing.Size(75, 23);
            this.OKUltraButton.TabIndex = 11;
            this.OKUltraButton.Text = "OK";
            this.OKUltraButton.Click += new System.EventHandler(this.OKUltraButton_Click);
            // 
            // AdvancedSearchDialog
            // 
            this.AcceptButton = this.OKUltraButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelUltraButton;
            this.ClientSize = new System.Drawing.Size(374, 357);
            this.Controls.Add(this.CancelUltraButton);
            this.Controls.Add(this.OKUltraButton);
            this.Controls.Add(this.ultraTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AdvancedSearchDialog";
            this.Text = "Advanced Search Preferences";
            this.ultraTabPageControl1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ultraTabPageControl4.ResumeLayout(false);
            this.ultraTabPageControl4.PerformLayout();
            this.doubleBondGroupBox.ResumeLayout(false);
            this.doubleBondGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            this.tetrahedralStereoGroupBox.ResumeLayout(false);
            this.tetrahedralStereoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl)).EndInit();
            this.ultraTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox doubleBondGroupBox;
        private System.Windows.Forms.PictureBox pictureBox15;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.GroupBox tetrahedralStereoGroupBox;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox13;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.PictureBox pictureBox14;
        private System.Windows.Forms.CheckBox mustHitRxnCtrCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox hitAnyChargeCarbonCheckBox;
        private System.Windows.Forms.CheckBox hitAnyChargeHeteroCheckBox;
        private System.Windows.Forms.CheckBox extraFragsCheckBox;
        private System.Windows.Forms.CheckBox matchStereochemistryCheckBox;
        private Infragistics.Win.Misc.UltraButton CancelUltraButton;
        private Infragistics.Win.Misc.UltraButton OKUltraButton;
        private System.Windows.Forms.RadioButton anyTetStereoButton;
        private System.Windows.Forms.RadioButton eitherTetStereoButton;
        private System.Windows.Forms.RadioButton sameTetStereoButton;
        private System.Windows.Forms.RadioButton anyDoubleBondStereoButton;
        private System.Windows.Forms.RadioButton sameDoubleBondStereoButton;
    }
}