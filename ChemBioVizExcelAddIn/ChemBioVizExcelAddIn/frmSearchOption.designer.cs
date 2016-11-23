namespace ChemBioVizExcelAddIn
{
    partial class frmSearchOption
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
            this.tabSearchPreferences = new System.Windows.Forms.TabControl();
            this.tabSearchGeneral = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSimilarSearchThld = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cbxReactionCenter = new System.Windows.Forms.CheckBox();
            this.cbxChargeCarbon = new System.Windows.Forms.CheckBox();
            this.cbxTautomeric = new System.Windows.Forms.CheckBox();
            this.cbxFragmentsOverlap = new System.Windows.Forms.CheckBox();
            this.cbxPermitExtraneousFragmentsIfRXN = new System.Windows.Forms.CheckBox();
            this.cbxPermitExtraneousFragments = new System.Windows.Forms.CheckBox();
            this.cbxChargeHeteroAtom = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.c = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.tabSearchStereo = new System.Windows.Forms.TabPage();
            this.gpbDoubleBond = new System.Windows.Forms.GroupBox();
            this.rbtnDBSame = new System.Windows.Forms.RadioButton();
            this.rbtnDBAny = new System.Windows.Forms.RadioButton();
            this.cbxTHSThickBond = new System.Windows.Forms.CheckBox();
            this.gpbTHDStereo = new System.Windows.Forms.GroupBox();
            this.rbtnTHSEither = new System.Windows.Forms.RadioButton();
            this.rbtnTHSSame = new System.Windows.Forms.RadioButton();
            this.rbtnTHSAny = new System.Windows.Forms.RadioButton();
            this.cbxMatchStereoChem = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSearchPreferences.SuspendLayout();
            this.tabSearchGeneral.SuspendLayout();
            this.tabSearchStereo.SuspendLayout();
            this.gpbDoubleBond.SuspendLayout();
            this.gpbTHDStereo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSearchPreferences
            // 
            this.tabSearchPreferences.Controls.Add(this.tabSearchGeneral);
            this.tabSearchPreferences.Controls.Add(this.tabSearchStereo);
            this.tabSearchPreferences.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabSearchPreferences.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.tabSearchPreferences.Location = new System.Drawing.Point(0, 0);
            this.tabSearchPreferences.Name = "tabSearchPreferences";
            this.tabSearchPreferences.Padding = new System.Drawing.Point(58, 2);
            this.tabSearchPreferences.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabSearchPreferences.SelectedIndex = 0;
            this.tabSearchPreferences.Size = new System.Drawing.Size(370, 322);
            this.tabSearchPreferences.TabIndex = 0;
            // 
            // tabSearchGeneral
            // 
            this.tabSearchGeneral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabSearchGeneral.Controls.Add(this.label3);
            this.tabSearchGeneral.Controls.Add(this.txtSimilarSearchThld);
            this.tabSearchGeneral.Controls.Add(this.label18);
            this.tabSearchGeneral.Controls.Add(this.cbxReactionCenter);
            this.tabSearchGeneral.Controls.Add(this.cbxChargeCarbon);
            this.tabSearchGeneral.Controls.Add(this.cbxTautomeric);
            this.tabSearchGeneral.Controls.Add(this.cbxFragmentsOverlap);
            this.tabSearchGeneral.Controls.Add(this.cbxPermitExtraneousFragmentsIfRXN);
            this.tabSearchGeneral.Controls.Add(this.cbxPermitExtraneousFragments);
            this.tabSearchGeneral.Controls.Add(this.cbxChargeHeteroAtom);
            this.tabSearchGeneral.Controls.Add(this.label11);
            this.tabSearchGeneral.Controls.Add(this.label12);
            this.tabSearchGeneral.Controls.Add(this.label13);
            this.tabSearchGeneral.Controls.Add(this.c);
            this.tabSearchGeneral.Controls.Add(this.label15);
            this.tabSearchGeneral.Controls.Add(this.label16);
            this.tabSearchGeneral.Controls.Add(this.label17);
            this.tabSearchGeneral.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold);
            this.tabSearchGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabSearchGeneral.Name = "tabSearchGeneral";
            this.tabSearchGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabSearchGeneral.Size = new System.Drawing.Size(362, 296);
            this.tabSearchGeneral.TabIndex = 0;
            this.tabSearchGeneral.Text = "General";
            this.tabSearchGeneral.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(301, 276);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 17);
            this.label3.TabIndex = 27;
            this.label3.Text = "%";
            // 
            // txtSimilarSearchThld
            // 
            this.txtSimilarSearchThld.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSimilarSearchThld.Location = new System.Drawing.Point(304, 251);
            this.txtSimilarSearchThld.Name = "txtSimilarSearchThld";
            this.txtSimilarSearchThld.Size = new System.Drawing.Size(38, 22);
            this.txtSimilarSearchThld.TabIndex = 26;
            this.txtSimilarSearchThld.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSimilarSearchThld_KeyPress);
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(22, 259);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(237, 14);
            this.label18.TabIndex = 25;
            this.label18.Text = "Similarity search (20-100%)";
            // 
            // cbxReactionCenter
            // 
            this.cbxReactionCenter.AutoSize = true;
            this.cbxReactionCenter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxReactionCenter.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxReactionCenter.Location = new System.Drawing.Point(304, 48);
            this.cbxReactionCenter.Name = "cbxReactionCenter";
            this.cbxReactionCenter.Size = new System.Drawing.Size(15, 14);
            this.cbxReactionCenter.TabIndex = 24;
            this.cbxReactionCenter.UseVisualStyleBackColor = true;
            // 
            // cbxChargeCarbon
            // 
            this.cbxChargeCarbon.AutoSize = true;
            this.cbxChargeCarbon.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxChargeCarbon.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxChargeCarbon.Location = new System.Drawing.Point(304, 79);
            this.cbxChargeCarbon.Name = "cbxChargeCarbon";
            this.cbxChargeCarbon.Size = new System.Drawing.Size(15, 14);
            this.cbxChargeCarbon.TabIndex = 23;
            this.cbxChargeCarbon.UseVisualStyleBackColor = true;
            // 
            // cbxTautomeric
            // 
            this.cbxTautomeric.AutoSize = true;
            this.cbxTautomeric.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxTautomeric.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxTautomeric.Location = new System.Drawing.Point(304, 228);
            this.cbxTautomeric.Name = "cbxTautomeric";
            this.cbxTautomeric.Size = new System.Drawing.Size(15, 14);
            this.cbxTautomeric.TabIndex = 20;
            this.cbxTautomeric.UseVisualStyleBackColor = true;
            // 
            // cbxFragmentsOverlap
            // 
            this.cbxFragmentsOverlap.AutoSize = true;
            this.cbxFragmentsOverlap.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxFragmentsOverlap.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxFragmentsOverlap.Location = new System.Drawing.Point(304, 195);
            this.cbxFragmentsOverlap.Name = "cbxFragmentsOverlap";
            this.cbxFragmentsOverlap.Size = new System.Drawing.Size(15, 14);
            this.cbxFragmentsOverlap.TabIndex = 19;
            this.cbxFragmentsOverlap.UseVisualStyleBackColor = true;
            // 
            // cbxPermitExtraneousFragmentsIfRXN
            // 
            this.cbxPermitExtraneousFragmentsIfRXN.AutoSize = true;
            this.cbxPermitExtraneousFragmentsIfRXN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxPermitExtraneousFragmentsIfRXN.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxPermitExtraneousFragmentsIfRXN.Location = new System.Drawing.Point(304, 151);
            this.cbxPermitExtraneousFragmentsIfRXN.Name = "cbxPermitExtraneousFragmentsIfRXN";
            this.cbxPermitExtraneousFragmentsIfRXN.Size = new System.Drawing.Size(15, 14);
            this.cbxPermitExtraneousFragmentsIfRXN.TabIndex = 18;
            this.cbxPermitExtraneousFragmentsIfRXN.UseVisualStyleBackColor = true;
            // 
            // cbxPermitExtraneousFragments
            // 
            this.cbxPermitExtraneousFragments.AutoSize = true;
            this.cbxPermitExtraneousFragments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxPermitExtraneousFragments.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxPermitExtraneousFragments.Location = new System.Drawing.Point(304, 107);
            this.cbxPermitExtraneousFragments.Name = "cbxPermitExtraneousFragments";
            this.cbxPermitExtraneousFragments.Size = new System.Drawing.Size(15, 14);
            this.cbxPermitExtraneousFragments.TabIndex = 17;
            this.cbxPermitExtraneousFragments.UseVisualStyleBackColor = true;
            // 
            // cbxChargeHeteroAtom
            // 
            this.cbxChargeHeteroAtom.AutoSize = true;
            this.cbxChargeHeteroAtom.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxChargeHeteroAtom.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxChargeHeteroAtom.Location = new System.Drawing.Point(304, 21);
            this.cbxChargeHeteroAtom.Name = "cbxChargeHeteroAtom";
            this.cbxChargeHeteroAtom.Size = new System.Drawing.Size(15, 14);
            this.cbxChargeHeteroAtom.TabIndex = 16;
            this.cbxChargeHeteroAtom.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(22, 227);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(233, 15);
            this.label11.TabIndex = 14;
            this.label11.Text = "Tautomeric";
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(22, 150);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(233, 38);
            this.label12.TabIndex = 13;
            this.label12.Text = "Permit extraneous fragements in Reaction Full Structure (exact) Searches";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(22, 195);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(233, 22);
            this.label13.TabIndex = 12;
            this.label13.Text = "Query fragments can overlap in target";
            // 
            // c
            // 
            this.c.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c.Location = new System.Drawing.Point(22, 106);
            this.c.Name = "c";
            this.c.Size = new System.Drawing.Size(233, 31);
            this.c.TabIndex = 11;
            this.c.Text = "Permit extraneous fragements in Full Structure (exact) Searches";
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(22, 78);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(233, 17);
            this.label15.TabIndex = 10;
            this.label15.Text = "Hit any charge on carbon";
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(22, 47);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(233, 15);
            this.label16.TabIndex = 9;
            this.label16.Text = "Reaction query must hit reaction center";
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(22, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(233, 16);
            this.label17.TabIndex = 8;
            this.label17.Text = "Hit any charge of heteroatom";
            // 
            // tabSearchStereo
            // 
            this.tabSearchStereo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabSearchStereo.Controls.Add(this.gpbDoubleBond);
            this.tabSearchStereo.Controls.Add(this.cbxTHSThickBond);
            this.tabSearchStereo.Controls.Add(this.gpbTHDStereo);
            this.tabSearchStereo.Controls.Add(this.cbxMatchStereoChem);
            this.tabSearchStereo.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.tabSearchStereo.Location = new System.Drawing.Point(4, 22);
            this.tabSearchStereo.Name = "tabSearchStereo";
            this.tabSearchStereo.Padding = new System.Windows.Forms.Padding(3);
            this.tabSearchStereo.Size = new System.Drawing.Size(362, 296);
            this.tabSearchStereo.TabIndex = 1;
            this.tabSearchStereo.Text = "Stereochemistry";
            this.tabSearchStereo.UseVisualStyleBackColor = true;
            // 
            // gpbDoubleBond
            // 
            this.gpbDoubleBond.Controls.Add(this.rbtnDBSame);
            this.gpbDoubleBond.Controls.Add(this.rbtnDBAny);
            this.gpbDoubleBond.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gpbDoubleBond.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gpbDoubleBond.Location = new System.Drawing.Point(20, 192);
            this.gpbDoubleBond.Name = "gpbDoubleBond";
            this.gpbDoubleBond.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gpbDoubleBond.Size = new System.Drawing.Size(182, 94);
            this.gpbDoubleBond.TabIndex = 33;
            this.gpbDoubleBond.TabStop = false;
            this.gpbDoubleBond.Text = "Double bond hits:";
            // 
            // rbtnDBSame
            // 
            this.rbtnDBSame.AutoSize = true;
            this.rbtnDBSame.Location = new System.Drawing.Point(28, 21);
            this.rbtnDBSame.Name = "rbtnDBSame";
            this.rbtnDBSame.Size = new System.Drawing.Size(52, 19);
            this.rbtnDBSame.TabIndex = 28;
            this.rbtnDBSame.TabStop = true;
            this.rbtnDBSame.Text = "same";
            this.rbtnDBSame.UseVisualStyleBackColor = true;
            // 
            // rbtnDBAny
            // 
            this.rbtnDBAny.AutoSize = true;
            this.rbtnDBAny.Location = new System.Drawing.Point(28, 64);
            this.rbtnDBAny.Name = "rbtnDBAny";
            this.rbtnDBAny.Size = new System.Drawing.Size(45, 19);
            this.rbtnDBAny.TabIndex = 29;
            this.rbtnDBAny.TabStop = true;
            this.rbtnDBAny.Text = "any";
            this.rbtnDBAny.UseVisualStyleBackColor = true;
            // 
            // cbxTHSThickBond
            // 
            this.cbxTHSThickBond.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbxTHSThickBond.Location = new System.Drawing.Point(208, 126);
            this.cbxTHSThickBond.Name = "cbxTHSThickBond";
            this.cbxTHSThickBond.Size = new System.Drawing.Size(150, 43);
            this.cbxTHSThickBond.TabIndex = 31;
            this.cbxTHSThickBond.Text = "Thick bonds represent relative streochemistry";
            this.cbxTHSThickBond.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cbxTHSThickBond.UseVisualStyleBackColor = true;
            // 
            // gpbTHDStereo
            // 
            this.gpbTHDStereo.Controls.Add(this.rbtnTHSEither);
            this.gpbTHDStereo.Controls.Add(this.rbtnTHSSame);
            this.gpbTHDStereo.Controls.Add(this.rbtnTHSAny);
            this.gpbTHDStereo.Location = new System.Drawing.Point(20, 42);
            this.gpbTHDStereo.Name = "gpbTHDStereo";
            this.gpbTHDStereo.Size = new System.Drawing.Size(182, 128);
            this.gpbTHDStereo.TabIndex = 32;
            this.gpbTHDStereo.TabStop = false;
            this.gpbTHDStereo.Text = "Tetrahedral stereo center hits:";
            // 
            // rbtnTHSEither
            // 
            this.rbtnTHSEither.AutoSize = true;
            this.rbtnTHSEither.Location = new System.Drawing.Point(28, 62);
            this.rbtnTHSEither.Name = "rbtnTHSEither";
            this.rbtnTHSEither.Size = new System.Drawing.Size(55, 19);
            this.rbtnTHSEither.TabIndex = 26;
            this.rbtnTHSEither.TabStop = true;
            this.rbtnTHSEither.Text = "either";
            this.rbtnTHSEither.UseVisualStyleBackColor = true;
            // 
            // rbtnTHSSame
            // 
            this.rbtnTHSSame.AutoSize = true;
            this.rbtnTHSSame.Location = new System.Drawing.Point(28, 23);
            this.rbtnTHSSame.Name = "rbtnTHSSame";
            this.rbtnTHSSame.Size = new System.Drawing.Size(52, 19);
            this.rbtnTHSSame.TabIndex = 25;
            this.rbtnTHSSame.TabStop = true;
            this.rbtnTHSSame.Text = "same";
            this.rbtnTHSSame.UseVisualStyleBackColor = true;
            // 
            // rbtnTHSAny
            // 
            this.rbtnTHSAny.AutoSize = true;
            this.rbtnTHSAny.Location = new System.Drawing.Point(28, 98);
            this.rbtnTHSAny.Name = "rbtnTHSAny";
            this.rbtnTHSAny.Size = new System.Drawing.Size(45, 19);
            this.rbtnTHSAny.TabIndex = 27;
            this.rbtnTHSAny.TabStop = true;
            this.rbtnTHSAny.Text = "any";
            this.rbtnTHSAny.UseVisualStyleBackColor = true;
            // 
            // cbxMatchStereoChem
            // 
            this.cbxMatchStereoChem.AutoSize = true;
            this.cbxMatchStereoChem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbxMatchStereoChem.Location = new System.Drawing.Point(111, 12);
            this.cbxMatchStereoChem.Name = "cbxMatchStereoChem";
            this.cbxMatchStereoChem.Size = new System.Drawing.Size(150, 19);
            this.cbxMatchStereoChem.TabIndex = 24;
            this.cbxMatchStereoChem.Text = "Match Stereochemistry";
            this.cbxMatchStereoChem.UseVisualStyleBackColor = true;
            this.cbxMatchStereoChem.CheckedChanged += new System.EventHandler(this.cbxMatchStereoChem_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(370, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please select your search options.";
            // 
            // frmSearchOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 344);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabSearchPreferences);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmSearchOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Preferences";
            this.Load += new System.EventHandler(this.frmSearchOption_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSearchOption_FormClosed);
            this.tabSearchPreferences.ResumeLayout(false);
            this.tabSearchGeneral.ResumeLayout(false);
            this.tabSearchGeneral.PerformLayout();
            this.tabSearchStereo.ResumeLayout(false);
            this.tabSearchStereo.PerformLayout();
            this.gpbDoubleBond.ResumeLayout(false);
            this.gpbDoubleBond.PerformLayout();
            this.gpbTHDStereo.ResumeLayout(false);
            this.gpbTHDStereo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabSearchPreferences;
        private System.Windows.Forms.TabPage tabSearchGeneral;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxChargeHeteroAtom;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label c;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox cbxReactionCenter;
        private System.Windows.Forms.CheckBox cbxChargeCarbon;
        private System.Windows.Forms.CheckBox cbxTautomeric;
        private System.Windows.Forms.CheckBox cbxFragmentsOverlap;
        private System.Windows.Forms.CheckBox cbxPermitExtraneousFragmentsIfRXN;
        private System.Windows.Forms.CheckBox cbxPermitExtraneousFragments;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtSimilarSearchThld;
        private System.Windows.Forms.TabPage tabSearchStereo;
        private System.Windows.Forms.CheckBox cbxTHSThickBond;
        private System.Windows.Forms.RadioButton rbtnDBAny;
        private System.Windows.Forms.RadioButton rbtnDBSame;
        private System.Windows.Forms.RadioButton rbtnTHSAny;
        private System.Windows.Forms.RadioButton rbtnTHSEither;
        private System.Windows.Forms.RadioButton rbtnTHSSame;
        private System.Windows.Forms.CheckBox cbxMatchStereoChem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gpbDoubleBond;
        private System.Windows.Forms.GroupBox gpbTHDStereo;

    }
}

