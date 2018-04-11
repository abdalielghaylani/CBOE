namespace GenerateFilesFromChemDraw
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
			this.btnBrowseGet = new System.Windows.Forms.Button();
			this.txtGetFilesPath = new System.Windows.Forms.TextBox();
			this.lblInput = new System.Windows.Forms.Label();
			this.tabOptions = new System.Windows.Forms.TabControl();
			this.tabGOO = new System.Windows.Forms.TabPage();
			this.btnBrowsePut = new System.Windows.Forms.Button();
			this.Label5 = new System.Windows.Forms.Label();
			this.txtRes = new System.Windows.Forms.TextBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.lblRes = new System.Windows.Forms.Label();
			this.chkCreateFiles = new System.Windows.Forms.CheckBox();
			this.txtType = new System.Windows.Forms.TextBox();
			this.txtPutFilePath = new System.Windows.Forms.TextBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.lblType = new System.Windows.Forms.Label();
			this.lblOutput = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.tabAOO = new System.Windows.Forms.TabPage();
			this.btnBrowseSD = new System.Windows.Forms.Button();
			this.txtSetDoc = new System.Windows.Forms.TextBox();
			this.txtAtomFT = new System.Windows.Forms.TextBox();
			this.txtAtomFS = new System.Windows.Forms.TextBox();
			this.chkUseSetDoc = new System.Windows.Forms.CheckBox();
			this.Label7 = new System.Windows.Forms.Label();
			this.chkChangeFT = new System.Windows.Forms.CheckBox();
			this.Label6 = new System.Windows.Forms.Label();
			this.chkChangeFS = new System.Windows.Forms.CheckBox();
			this.lblFontSize = new System.Windows.Forms.Label();
			this.tabDBInput = new System.Windows.Forms.TabPage();
			this.chkEmtStructure = new System.Windows.Forms.CheckBox();
			this.lblEnterQuery = new System.Windows.Forms.Label();
			this.lblOR = new System.Windows.Forms.Label();
			this.lblBuildQuery = new System.Windows.Forms.Label();
			this.txtQryBox = new System.Windows.Forms.TextBox();
			this.chkCurrQry = new System.Windows.Forms.CheckBox();
			this.txttheStructure = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.txtoutputID = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.txtoutTable = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtConnStr2 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tabSDFStructGrab = new System.Windows.Forms.TabPage();
			this.lblSdfDirectoryMessage = new System.Windows.Forms.Label();
			this.lblSdfProcess = new System.Windows.Forms.Label();
			this.cmbSdfSelect = new System.Windows.Forms.ComboBox();
			this.chkSdfAppExistTable = new System.Windows.Forms.CheckBox();
			this.txtSdfTableName = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.txtSdfIDField = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.btnBrowseSdfFile = new System.Windows.Forms.Button();
			this.txtInputSDF = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.txtSdfAccessDB = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.tabDBOptions = new System.Windows.Forms.TabPage();
			this.Label3 = new System.Windows.Forms.Label();
			this.btnBrowseDB = new System.Windows.Forms.Button();
			this.lblInsTblNm = new System.Windows.Forms.Label();
			this.txtMFtbl = new System.Windows.Forms.TextBox();
			this.txtMWMFtbl = new System.Windows.Forms.TextBox();
			this.txtMWtbl = new System.Windows.Forms.TextBox();
			this.txtConnString = new System.Windows.Forms.TextBox();
			this.chkMFs = new System.Windows.Forms.CheckBox();
			this.Label10 = new System.Windows.Forms.Label();
			this.chkmwandmf = new System.Windows.Forms.CheckBox();
			this.Label9 = new System.Windows.Forms.Label();
			this.chkMWs = new System.Windows.Forms.CheckBox();
			this.chkDoDBOps = new System.Windows.Forms.CheckBox();
			this.lblProcessing = new System.Windows.Forms.Label();
			this.lblGifName = new System.Windows.Forms.Label();
			this.cmdCreateFiles = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.lblTabMessage = new System.Windows.Forms.Label();
			this.cmbProcessTab = new System.Windows.Forms.ComboBox();
			this.tabOptions.SuspendLayout();
			this.tabGOO.SuspendLayout();
			this.tabAOO.SuspendLayout();
			this.tabDBInput.SuspendLayout();
			this.tabSDFStructGrab.SuspendLayout();
			this.tabDBOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnBrowseGet
			// 
			this.btnBrowseGet.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowseGet.Location = new System.Drawing.Point(468, 32);
			this.btnBrowseGet.Name = "btnBrowseGet";
			this.btnBrowseGet.Size = new System.Drawing.Size(60, 24);
			this.btnBrowseGet.TabIndex = 1;
			this.btnBrowseGet.Text = "Browse";
			this.btnBrowseGet.Click += new System.EventHandler(this.btnBrowseGet_Click);
			// 
			// txtGetFilesPath
			// 
			this.txtGetFilesPath.AcceptsReturn = true;
			this.txtGetFilesPath.BackColor = System.Drawing.SystemColors.Window;
			this.txtGetFilesPath.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtGetFilesPath.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtGetFilesPath.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtGetFilesPath.Location = new System.Drawing.Point(164, 32);
			this.txtGetFilesPath.MaxLength = 0;
			this.txtGetFilesPath.Name = "txtGetFilesPath";
			this.txtGetFilesPath.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtGetFilesPath.Size = new System.Drawing.Size(297, 22);
			this.txtGetFilesPath.TabIndex = 0;
			this.txtGetFilesPath.Text = "c:\\test";
			// 
			// lblInput
			// 
			this.lblInput.BackColor = System.Drawing.SystemColors.Control;
			this.lblInput.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblInput.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInput.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblInput.Location = new System.Drawing.Point(69, 32);
			this.lblInput.Name = "lblInput";
			this.lblInput.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblInput.Size = new System.Drawing.Size(96, 17);
			this.lblInput.TabIndex = 42;
			this.lblInput.Text = "Input Directory";
			// 
			// tabOptions
			// 
			this.tabOptions.Controls.Add(this.tabGOO);
			this.tabOptions.Controls.Add(this.tabAOO);
			this.tabOptions.Controls.Add(this.tabDBInput);
			this.tabOptions.Controls.Add(this.tabSDFStructGrab);
			this.tabOptions.Controls.Add(this.tabDBOptions);
			this.tabOptions.Location = new System.Drawing.Point(12, 113);
			this.tabOptions.Name = "tabOptions";
			this.tabOptions.SelectedIndex = 0;
			this.tabOptions.Size = new System.Drawing.Size(803, 313);
			this.tabOptions.TabIndex = 44;
			this.tabOptions.SelectedIndexChanged += new System.EventHandler(this.tabOptions_SelectedIndexChanged);
			// 
			// tabGOO
			// 
			this.tabGOO.Controls.Add(this.btnBrowsePut);
			this.tabGOO.Controls.Add(this.Label5);
			this.tabGOO.Controls.Add(this.txtRes);
			this.tabGOO.Controls.Add(this.Label2);
			this.tabGOO.Controls.Add(this.lblRes);
			this.tabGOO.Controls.Add(this.chkCreateFiles);
			this.tabGOO.Controls.Add(this.txtType);
			this.tabGOO.Controls.Add(this.txtPutFilePath);
			this.tabGOO.Controls.Add(this.Label1);
			this.tabGOO.Controls.Add(this.lblType);
			this.tabGOO.Controls.Add(this.lblOutput);
			this.tabGOO.Controls.Add(this.Label4);
			this.tabGOO.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabGOO.Location = new System.Drawing.Point(4, 22);
			this.tabGOO.Name = "tabGOO";
			this.tabGOO.Size = new System.Drawing.Size(795, 287);
			this.tabGOO.TabIndex = 2;
			this.tabGOO.Text = "General Output Options";
			this.tabGOO.UseVisualStyleBackColor = true;
			// 
			// btnBrowsePut
			// 
			this.btnBrowsePut.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowsePut.Location = new System.Drawing.Point(440, 40);
			this.btnBrowsePut.Name = "btnBrowsePut";
			this.btnBrowsePut.Size = new System.Drawing.Size(80, 24);
			this.btnBrowsePut.TabIndex = 33;
			this.btnBrowsePut.Text = "Browse";
			this.btnBrowsePut.Click += new System.EventHandler(this.btnBrowsePut_Click);
			// 
			// Label5
			// 
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label5.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label5.Location = new System.Drawing.Point(200, 112);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(248, 24);
			this.Label5.TabIndex = 16;
			this.Label5.Text = "Chemical extensions: mol, cdx, cdxml";
			// 
			// txtRes
			// 
			this.txtRes.AcceptsReturn = true;
			this.txtRes.BackColor = System.Drawing.SystemColors.Window;
			this.txtRes.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtRes.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtRes.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtRes.Location = new System.Drawing.Point(120, 152);
			this.txtRes.MaxLength = 0;
			this.txtRes.Name = "txtRes";
			this.txtRes.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtRes.Size = new System.Drawing.Size(57, 22);
			this.txtRes.TabIndex = 30;
			this.txtRes.Text = "72";
			// 
			// Label2
			// 
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label2.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label2.Location = new System.Drawing.Point(200, 152);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(161, 25);
			this.Label2.TabIndex = 32;
			this.Label2.Text = "ignored for cases where resolution is not relevant";
			// 
			// lblRes
			// 
			this.lblRes.BackColor = System.Drawing.SystemColors.Control;
			this.lblRes.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblRes.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblRes.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblRes.Location = new System.Drawing.Point(21, 152);
			this.lblRes.Name = "lblRes";
			this.lblRes.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblRes.Size = new System.Drawing.Size(72, 22);
			this.lblRes.TabIndex = 31;
			this.lblRes.Text = "Resolution";
			// 
			// chkCreateFiles
			// 
			this.chkCreateFiles.Checked = true;
			this.chkCreateFiles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkCreateFiles.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkCreateFiles.Location = new System.Drawing.Point(24, 8);
			this.chkCreateFiles.Name = "chkCreateFiles";
			this.chkCreateFiles.Size = new System.Drawing.Size(273, 16);
			this.chkCreateFiles.TabIndex = 28;
			this.chkCreateFiles.Text = "Create Files (only uncheck for db operations)";
			// 
			// txtType
			// 
			this.txtType.AcceptsReturn = true;
			this.txtType.BackColor = System.Drawing.SystemColors.Window;
			this.txtType.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtType.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtType.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtType.Location = new System.Drawing.Point(120, 104);
			this.txtType.MaxLength = 0;
			this.txtType.Name = "txtType";
			this.txtType.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtType.Size = new System.Drawing.Size(57, 22);
			this.txtType.TabIndex = 15;
			this.txtType.Text = "gif";
			// 
			// txtPutFilePath
			// 
			this.txtPutFilePath.AcceptsReturn = true;
			this.txtPutFilePath.BackColor = System.Drawing.SystemColors.Window;
			this.txtPutFilePath.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtPutFilePath.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPutFilePath.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtPutFilePath.Location = new System.Drawing.Point(120, 40);
			this.txtPutFilePath.MaxLength = 0;
			this.txtPutFilePath.Name = "txtPutFilePath";
			this.txtPutFilePath.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtPutFilePath.Size = new System.Drawing.Size(297, 22);
			this.txtPutFilePath.TabIndex = 13;
			this.txtPutFilePath.Text = "c:\\";
			// 
			// Label1
			// 
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label1.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(200, 96);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(264, 24);
			this.Label1.TabIndex = 17;
			this.Label1.Text = "Image extensions: gif, png, wmf, emf, eps, bmp, tif";
			// 
			// lblType
			// 
			this.lblType.BackColor = System.Drawing.SystemColors.Control;
			this.lblType.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblType.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblType.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblType.Location = new System.Drawing.Point(24, 104);
			this.lblType.Name = "lblType";
			this.lblType.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblType.Size = new System.Drawing.Size(88, 17);
			this.lblType.TabIndex = 16;
			this.lblType.Text = "Output File Type";
			// 
			// lblOutput
			// 
			this.lblOutput.BackColor = System.Drawing.SystemColors.Control;
			this.lblOutput.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblOutput.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOutput.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblOutput.Location = new System.Drawing.Point(24, 40);
			this.lblOutput.Name = "lblOutput";
			this.lblOutput.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblOutput.Size = new System.Drawing.Size(88, 17);
			this.lblOutput.TabIndex = 14;
			this.lblOutput.Text = "Output Directory";
			// 
			// Label4
			// 
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label4.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label4.Location = new System.Drawing.Point(120, 64);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(177, 16);
			this.Label4.TabIndex = 15;
			this.Label4.Text = "Path must end in \\";
			// 
			// tabAOO
			// 
			this.tabAOO.Controls.Add(this.btnBrowseSD);
			this.tabAOO.Controls.Add(this.txtSetDoc);
			this.tabAOO.Controls.Add(this.txtAtomFT);
			this.tabAOO.Controls.Add(this.txtAtomFS);
			this.tabAOO.Controls.Add(this.chkUseSetDoc);
			this.tabAOO.Controls.Add(this.Label7);
			this.tabAOO.Controls.Add(this.chkChangeFT);
			this.tabAOO.Controls.Add(this.Label6);
			this.tabAOO.Controls.Add(this.chkChangeFS);
			this.tabAOO.Controls.Add(this.lblFontSize);
			this.tabAOO.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabAOO.Location = new System.Drawing.Point(4, 22);
			this.tabAOO.Name = "tabAOO";
			this.tabAOO.Size = new System.Drawing.Size(795, 287);
			this.tabAOO.TabIndex = 0;
			this.tabAOO.Text = "Advanced Output Options";
			this.tabAOO.UseVisualStyleBackColor = true;
			// 
			// btnBrowseSD
			// 
			this.btnBrowseSD.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowseSD.Location = new System.Drawing.Point(632, 48);
			this.btnBrowseSD.Name = "btnBrowseSD";
			this.btnBrowseSD.Size = new System.Drawing.Size(56, 24);
			this.btnBrowseSD.TabIndex = 40;
			this.btnBrowseSD.Text = "Browse";
			// 
			// txtSetDoc
			// 
			this.txtSetDoc.AcceptsReturn = true;
			this.txtSetDoc.BackColor = System.Drawing.SystemColors.Window;
			this.txtSetDoc.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtSetDoc.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSetDoc.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtSetDoc.Location = new System.Drawing.Point(8, 48);
			this.txtSetDoc.MaxLength = 0;
			this.txtSetDoc.Name = "txtSetDoc";
			this.txtSetDoc.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtSetDoc.Size = new System.Drawing.Size(608, 22);
			this.txtSetDoc.TabIndex = 37;
			this.txtSetDoc.Text = "C:\\Program Files\\CambridgeSoft\\ChemOffice2005\\ChemDraw\\ChemDraw Items\\ACS Documen" +
				"t 1996.cds";
			// 
			// txtAtomFT
			// 
			this.txtAtomFT.AcceptsReturn = true;
			this.txtAtomFT.BackColor = System.Drawing.SystemColors.Window;
			this.txtAtomFT.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtAtomFT.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAtomFT.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtAtomFT.Location = new System.Drawing.Point(128, 168);
			this.txtAtomFT.MaxLength = 0;
			this.txtAtomFT.Name = "txtAtomFT";
			this.txtAtomFT.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtAtomFT.Size = new System.Drawing.Size(144, 22);
			this.txtAtomFT.TabIndex = 34;
			this.txtAtomFT.Text = "Arial";
			// 
			// txtAtomFS
			// 
			this.txtAtomFS.AcceptsReturn = true;
			this.txtAtomFS.BackColor = System.Drawing.SystemColors.Window;
			this.txtAtomFS.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtAtomFS.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAtomFS.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtAtomFS.Location = new System.Drawing.Point(128, 128);
			this.txtAtomFS.MaxLength = 0;
			this.txtAtomFS.Name = "txtAtomFS";
			this.txtAtomFS.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtAtomFS.Size = new System.Drawing.Size(40, 22);
			this.txtAtomFS.TabIndex = 31;
			this.txtAtomFS.Text = "10";
			// 
			// chkUseSetDoc
			// 
			this.chkUseSetDoc.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkUseSetDoc.Location = new System.Drawing.Point(136, 32);
			this.chkUseSetDoc.Name = "chkUseSetDoc";
			this.chkUseSetDoc.Size = new System.Drawing.Size(16, 16);
			this.chkUseSetDoc.TabIndex = 39;
			// 
			// Label7
			// 
			this.Label7.BackColor = System.Drawing.SystemColors.Control;
			this.Label7.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label7.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label7.Location = new System.Drawing.Point(8, 32);
			this.Label7.Name = "Label7";
			this.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label7.Size = new System.Drawing.Size(128, 17);
			this.Label7.TabIndex = 38;
			this.Label7.Text = "Apply Settings Document";
			// 
			// chkChangeFT
			// 
			this.chkChangeFT.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkChangeFT.Location = new System.Drawing.Point(112, 168);
			this.chkChangeFT.Name = "chkChangeFT";
			this.chkChangeFT.Size = new System.Drawing.Size(16, 16);
			this.chkChangeFT.TabIndex = 36;
			// 
			// Label6
			// 
			this.Label6.BackColor = System.Drawing.SystemColors.Control;
			this.Label6.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label6.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label6.Location = new System.Drawing.Point(24, 168);
			this.Label6.Name = "Label6";
			this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label6.Size = new System.Drawing.Size(88, 17);
			this.Label6.TabIndex = 35;
			this.Label6.Text = "Atom Font Type";
			// 
			// chkChangeFS
			// 
			this.chkChangeFS.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkChangeFS.Location = new System.Drawing.Point(112, 128);
			this.chkChangeFS.Name = "chkChangeFS";
			this.chkChangeFS.Size = new System.Drawing.Size(16, 16);
			this.chkChangeFS.TabIndex = 33;
			// 
			// lblFontSize
			// 
			this.lblFontSize.BackColor = System.Drawing.SystemColors.Control;
			this.lblFontSize.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblFontSize.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblFontSize.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblFontSize.Location = new System.Drawing.Point(26, 128);
			this.lblFontSize.Name = "lblFontSize";
			this.lblFontSize.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblFontSize.Size = new System.Drawing.Size(80, 17);
			this.lblFontSize.TabIndex = 32;
			this.lblFontSize.Text = "Atom Font Size";
			// 
			// tabDBInput
			// 
			this.tabDBInput.Controls.Add(this.chkEmtStructure);
			this.tabDBInput.Controls.Add(this.lblEnterQuery);
			this.tabDBInput.Controls.Add(this.lblOR);
			this.tabDBInput.Controls.Add(this.lblBuildQuery);
			this.tabDBInput.Controls.Add(this.txtQryBox);
			this.tabDBInput.Controls.Add(this.chkCurrQry);
			this.tabDBInput.Controls.Add(this.txttheStructure);
			this.tabDBInput.Controls.Add(this.label13);
			this.tabDBInput.Controls.Add(this.txtoutputID);
			this.tabDBInput.Controls.Add(this.label12);
			this.tabDBInput.Controls.Add(this.txtoutTable);
			this.tabDBInput.Controls.Add(this.label11);
			this.tabDBInput.Controls.Add(this.txtConnStr2);
			this.tabDBInput.Controls.Add(this.label8);
			this.tabDBInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabDBInput.Location = new System.Drawing.Point(4, 22);
			this.tabDBInput.Name = "tabDBInput";
			this.tabDBInput.Padding = new System.Windows.Forms.Padding(3);
			this.tabDBInput.Size = new System.Drawing.Size(795, 287);
			this.tabDBInput.TabIndex = 3;
			this.tabDBInput.Text = "Database Input";
			this.tabDBInput.UseVisualStyleBackColor = true;
			// 
			// chkEmtStructure
			// 
			this.chkEmtStructure.AutoSize = true;
			this.chkEmtStructure.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkEmtStructure.Location = new System.Drawing.Point(19, 93);
			this.chkEmtStructure.Name = "chkEmtStructure";
			this.chkEmtStructure.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkEmtStructure.Size = new System.Drawing.Size(148, 19);
			this.chkEmtStructure.TabIndex = 58;
			this.chkEmtStructure.Text = "Skip empty structures  ";
			this.chkEmtStructure.UseVisualStyleBackColor = true;
			// 
			// lblEnterQuery
			// 
			this.lblEnterQuery.AutoSize = true;
			this.lblEnterQuery.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblEnterQuery.Location = new System.Drawing.Point(382, 125);
			this.lblEnterQuery.Name = "lblEnterQuery";
			this.lblEnterQuery.Size = new System.Drawing.Size(163, 15);
			this.lblEnterQuery.TabIndex = 57;
			this.lblEnterQuery.Text = "I want to enter my own query";
			// 
			// lblOR
			// 
			this.lblOR.AutoSize = true;
			this.lblOR.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblOR.Location = new System.Drawing.Point(295, 125);
			this.lblOR.Name = "lblOR";
			this.lblOR.Size = new System.Drawing.Size(24, 15);
			this.lblOR.TabIndex = 56;
			this.lblOR.Text = "OR";
			// 
			// lblBuildQuery
			// 
			this.lblBuildQuery.AutoSize = true;
			this.lblBuildQuery.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBuildQuery.Location = new System.Drawing.Point(20, 125);
			this.lblBuildQuery.Name = "lblBuildQuery";
			this.lblBuildQuery.Size = new System.Drawing.Size(238, 15);
			this.lblBuildQuery.TabIndex = 55;
			this.lblBuildQuery.Text = "Build a query with the following parameters";
			// 
			// txtQryBox
			// 
			this.txtQryBox.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtQryBox.Location = new System.Drawing.Point(385, 196);
			this.txtQryBox.Multiline = true;
			this.txtQryBox.Name = "txtQryBox";
			this.txtQryBox.Size = new System.Drawing.Size(243, 52);
			this.txtQryBox.TabIndex = 54;
			// 
			// chkCurrQry
			// 
			this.chkCurrQry.AutoSize = true;
			this.chkCurrQry.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkCurrQry.Location = new System.Drawing.Point(385, 169);
			this.chkCurrQry.Name = "chkCurrQry";
			this.chkCurrQry.Size = new System.Drawing.Size(104, 19);
			this.chkCurrQry.TabIndex = 53;
			this.chkCurrQry.Text = "Use this query";
			this.chkCurrQry.UseVisualStyleBackColor = true;
			// 
			// txttheStructure
			// 
			this.txttheStructure.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txttheStructure.Location = new System.Drawing.Point(134, 228);
			this.txttheStructure.Name = "txttheStructure";
			this.txttheStructure.Size = new System.Drawing.Size(122, 22);
			this.txttheStructure.TabIndex = 51;
			this.txttheStructure.Text = "base64_cdx";
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(16, 226);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(68, 21);
			this.label13.TabIndex = 52;
			this.label13.Text = "Structure";
			// 
			// txtoutputID
			// 
			this.txtoutputID.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtoutputID.Location = new System.Drawing.Point(134, 200);
			this.txtoutputID.Name = "txtoutputID";
			this.txtoutputID.Size = new System.Drawing.Size(122, 22);
			this.txtoutputID.TabIndex = 49;
			this.txtoutputID.Text = "Mol_ID";
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(16, 199);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(43, 21);
			this.label12.TabIndex = 50;
			this.label12.Text = "ID";
			// 
			// txtoutTable
			// 
			this.txtoutTable.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtoutTable.Location = new System.Drawing.Point(134, 170);
			this.txtoutTable.Name = "txtoutTable";
			this.txtoutTable.Size = new System.Drawing.Size(122, 22);
			this.txtoutTable.TabIndex = 47;
			this.txtoutTable.Text = "MolTable";
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(16, 170);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(122, 16);
			this.label11.TabIndex = 48;
			this.label11.Text = "Table With Structures";
			// 
			// txtConnStr2
			// 
			this.txtConnStr2.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtConnStr2.Location = new System.Drawing.Point(23, 47);
			this.txtConnStr2.Name = "txtConnStr2";
			this.txtConnStr2.Size = new System.Drawing.Size(552, 22);
			this.txtConnStr2.TabIndex = 45;
			this.txtConnStr2.Text = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\WKInsight.mdb;";
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(20, 28);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(96, 16);
			this.label8.TabIndex = 46;
			this.label8.Text = "Connection String";
			// 
			// tabSDFStructGrab
			// 
			this.tabSDFStructGrab.Controls.Add(this.lblSdfDirectoryMessage);
			this.tabSDFStructGrab.Controls.Add(this.lblSdfProcess);
			this.tabSDFStructGrab.Controls.Add(this.cmbSdfSelect);
			this.tabSDFStructGrab.Controls.Add(this.chkSdfAppExistTable);
			this.tabSDFStructGrab.Controls.Add(this.txtSdfTableName);
			this.tabSDFStructGrab.Controls.Add(this.label16);
			this.tabSDFStructGrab.Controls.Add(this.txtSdfIDField);
			this.tabSDFStructGrab.Controls.Add(this.label17);
			this.tabSDFStructGrab.Controls.Add(this.btnBrowseSdfFile);
			this.tabSDFStructGrab.Controls.Add(this.txtInputSDF);
			this.tabSDFStructGrab.Controls.Add(this.label14);
			this.tabSDFStructGrab.Controls.Add(this.button1);
			this.tabSDFStructGrab.Controls.Add(this.txtSdfAccessDB);
			this.tabSDFStructGrab.Controls.Add(this.label15);
			this.tabSDFStructGrab.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabSDFStructGrab.Location = new System.Drawing.Point(4, 22);
			this.tabSDFStructGrab.Name = "tabSDFStructGrab";
			this.tabSDFStructGrab.Padding = new System.Windows.Forms.Padding(3);
			this.tabSDFStructGrab.Size = new System.Drawing.Size(795, 287);
			this.tabSDFStructGrab.TabIndex = 4;
			this.tabSDFStructGrab.Text = "SDF Input";
			this.tabSDFStructGrab.UseVisualStyleBackColor = true;
			// 
			// lblSdfDirectoryMessage
			// 
			this.lblSdfDirectoryMessage.BackColor = System.Drawing.SystemColors.Control;
			this.lblSdfDirectoryMessage.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblSdfDirectoryMessage.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSdfDirectoryMessage.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblSdfDirectoryMessage.Location = new System.Drawing.Point(506, 37);
			this.lblSdfDirectoryMessage.Name = "lblSdfDirectoryMessage";
			this.lblSdfDirectoryMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblSdfDirectoryMessage.Size = new System.Drawing.Size(273, 48);
			this.lblSdfDirectoryMessage.TabIndex = 95;
			// 
			// lblSdfProcess
			// 
			this.lblSdfProcess.AutoSize = true;
			this.lblSdfProcess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.lblSdfProcess.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSdfProcess.Location = new System.Drawing.Point(22, 43);
			this.lblSdfProcess.Name = "lblSdfProcess";
			this.lblSdfProcess.Size = new System.Drawing.Size(127, 15);
			this.lblSdfProcess.TabIndex = 94;
			this.lblSdfProcess.Text = "SDF Process Selection";
			// 
			// cmbSdfSelect
			// 
			this.cmbSdfSelect.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmbSdfSelect.FormattingEnabled = true;
			this.cmbSdfSelect.Items.AddRange(new object[] {
            "DB - Base64_CDX",
            "DB - MolFiles",
            "Directory"});
			this.cmbSdfSelect.Location = new System.Drawing.Point(222, 37);
			this.cmbSdfSelect.Name = "cmbSdfSelect";
			this.cmbSdfSelect.Size = new System.Drawing.Size(273, 23);
			this.cmbSdfSelect.TabIndex = 0;
			this.cmbSdfSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSdfSelect_SelectedIndexChanged);
			// 
			// chkSdfAppExistTable
			// 
			this.chkSdfAppExistTable.AutoSize = true;
			this.chkSdfAppExistTable.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkSdfAppExistTable.Location = new System.Drawing.Point(222, 204);
			this.chkSdfAppExistTable.Name = "chkSdfAppExistTable";
			this.chkSdfAppExistTable.Size = new System.Drawing.Size(188, 19);
			this.chkSdfAppExistTable.TabIndex = 4;
			this.chkSdfAppExistTable.Text = "Append to existing database table";
			this.chkSdfAppExistTable.UseVisualStyleBackColor = true;
			// 
			// txtSdfTableName
			// 
			this.txtSdfTableName.Enabled = false;
			this.txtSdfTableName.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSdfTableName.Location = new System.Drawing.Point(222, 178);
			this.txtSdfTableName.Name = "txtSdfTableName";
			this.txtSdfTableName.Size = new System.Drawing.Size(274, 22);
			this.txtSdfTableName.TabIndex = 3;
			this.txtSdfTableName.Text = "STRUCTURE";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.label16.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.Location = new System.Drawing.Point(22, 183);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(70, 15);
			this.label16.TabIndex = 89;
			this.label16.Text = "Table Name";
			// 
			// txtSdfIDField
			// 
			this.txtSdfIDField.Enabled = false;
			this.txtSdfIDField.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSdfIDField.Location = new System.Drawing.Point(222, 243);
			this.txtSdfIDField.Name = "txtSdfIDField";
			this.txtSdfIDField.Size = new System.Drawing.Size(274, 22);
			this.txtSdfIDField.TabIndex = 5;
			this.txtSdfIDField.Text = "CDBREGNO";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.label17.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.Location = new System.Drawing.Point(22, 248);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(49, 15);
			this.label17.TabIndex = 87;
			this.label17.Text = "ID Field";
			// 
			// btnBrowseSdfFile
			// 
			this.btnBrowseSdfFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnBrowseSdfFile.Location = new System.Drawing.Point(525, 100);
			this.btnBrowseSdfFile.Name = "btnBrowseSdfFile";
			this.btnBrowseSdfFile.Size = new System.Drawing.Size(64, 22);
			this.btnBrowseSdfFile.TabIndex = 6;
			this.btnBrowseSdfFile.Text = "Browse....";
			this.btnBrowseSdfFile.UseVisualStyleBackColor = true;
			this.btnBrowseSdfFile.Click += new System.EventHandler(this.btnBrowseSdfFile_Click);
			// 
			// txtInputSDF
			// 
			this.txtInputSDF.Enabled = false;
			this.txtInputSDF.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtInputSDF.Location = new System.Drawing.Point(221, 102);
			this.txtInputSDF.Name = "txtInputSDF";
			this.txtInputSDF.Size = new System.Drawing.Size(274, 22);
			this.txtInputSDF.TabIndex = 1;
			this.txtInputSDF.Text = "E:\\wda_sdfs_merged.sdf";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.label14.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(21, 102);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(84, 15);
			this.label14.TabIndex = 80;
			this.label14.Text = "Input SDF File";
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(526, 142);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 22);
			this.button1.TabIndex = 7;
			this.button1.Text = "Browse....";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtSdfAccessDB
			// 
			this.txtSdfAccessDB.Enabled = false;
			this.txtSdfAccessDB.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSdfAccessDB.Location = new System.Drawing.Point(222, 142);
			this.txtSdfAccessDB.Name = "txtSdfAccessDB";
			this.txtSdfAccessDB.Size = new System.Drawing.Size(274, 22);
			this.txtSdfAccessDB.TabIndex = 2;
			this.txtSdfAccessDB.Text = "C:\\Test.mdb";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.label15.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(22, 147);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(141, 15);
			this.label15.TabIndex = 77;
			this.label15.Text = "Output Access Database";
			// 
			// tabDBOptions
			// 
			this.tabDBOptions.Controls.Add(this.Label3);
			this.tabDBOptions.Controls.Add(this.btnBrowseDB);
			this.tabDBOptions.Controls.Add(this.lblInsTblNm);
			this.tabDBOptions.Controls.Add(this.txtMFtbl);
			this.tabDBOptions.Controls.Add(this.txtMWMFtbl);
			this.tabDBOptions.Controls.Add(this.txtMWtbl);
			this.tabDBOptions.Controls.Add(this.txtConnString);
			this.tabDBOptions.Controls.Add(this.chkMFs);
			this.tabDBOptions.Controls.Add(this.Label10);
			this.tabDBOptions.Controls.Add(this.chkmwandmf);
			this.tabDBOptions.Controls.Add(this.Label9);
			this.tabDBOptions.Controls.Add(this.chkMWs);
			this.tabDBOptions.Controls.Add(this.chkDoDBOps);
			this.tabDBOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabDBOptions.Location = new System.Drawing.Point(4, 22);
			this.tabDBOptions.Name = "tabDBOptions";
			this.tabDBOptions.Size = new System.Drawing.Size(795, 287);
			this.tabDBOptions.TabIndex = 1;
			this.tabDBOptions.Text = "Chemical property Output";
			this.tabDBOptions.UseVisualStyleBackColor = true;
			// 
			// Label3
			// 
			this.Label3.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label3.Location = new System.Drawing.Point(160, 48);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(336, 16);
			this.Label3.TabIndex = 52;
			this.Label3.Text = "You can browse to an access database with the existing tables";
			// 
			// btnBrowseDB
			// 
			this.btnBrowseDB.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowseDB.Location = new System.Drawing.Point(584, 72);
			this.btnBrowseDB.Name = "btnBrowseDB";
			this.btnBrowseDB.Size = new System.Drawing.Size(72, 24);
			this.btnBrowseDB.TabIndex = 51;
			this.btnBrowseDB.Text = "Browse";
			// 
			// lblInsTblNm
			// 
			this.lblInsTblNm.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInsTblNm.Location = new System.Drawing.Point(264, 112);
			this.lblInsTblNm.Name = "lblInsTblNm";
			this.lblInsTblNm.Size = new System.Drawing.Size(104, 16);
			this.lblInsTblNm.TabIndex = 50;
			this.lblInsTblNm.Text = "Table Name";
			// 
			// txtMFtbl
			// 
			this.txtMFtbl.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtMFtbl.Location = new System.Drawing.Point(264, 200);
			this.txtMFtbl.Name = "txtMFtbl";
			this.txtMFtbl.Size = new System.Drawing.Size(104, 22);
			this.txtMFtbl.TabIndex = 7;
			this.txtMFtbl.Text = "MolecularFormulas";
			// 
			// txtMWMFtbl
			// 
			this.txtMWMFtbl.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtMWMFtbl.Location = new System.Drawing.Point(264, 136);
			this.txtMWMFtbl.Name = "txtMWMFtbl";
			this.txtMWMFtbl.Size = new System.Drawing.Size(104, 22);
			this.txtMWMFtbl.TabIndex = 3;
			this.txtMWMFtbl.Text = "StructureInfo";
			// 
			// txtMWtbl
			// 
			this.txtMWtbl.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtMWtbl.Location = new System.Drawing.Point(264, 168);
			this.txtMWtbl.Name = "txtMWtbl";
			this.txtMWtbl.Size = new System.Drawing.Size(104, 22);
			this.txtMWtbl.TabIndex = 5;
			this.txtMWtbl.Text = "MolecularWeights";
			// 
			// txtConnString
			// 
			this.txtConnString.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtConnString.Location = new System.Drawing.Point(24, 72);
			this.txtConnString.Name = "txtConnString";
			this.txtConnString.Size = new System.Drawing.Size(552, 22);
			this.txtConnString.TabIndex = 1;
			this.txtConnString.Text = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\emp.mdb;";
			// 
			// chkMFs
			// 
			this.chkMFs.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkMFs.Location = new System.Drawing.Point(152, 200);
			this.chkMFs.Name = "chkMFs";
			this.chkMFs.Size = new System.Drawing.Size(96, 16);
			this.chkMFs.TabIndex = 6;
			this.chkMFs.Text = "Populate MFs";
			// 
			// Label10
			// 
			this.Label10.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label10.Location = new System.Drawing.Point(216, 16);
			this.Label10.Name = "Label10";
			this.Label10.Size = new System.Drawing.Size(368, 16);
			this.Label10.TabIndex = 46;
			this.Label10.Text = "DB Operations can only perform inserts into existing tables";
			// 
			// chkmwandmf
			// 
			this.chkmwandmf.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkmwandmf.Location = new System.Drawing.Point(56, 136);
			this.chkmwandmf.Name = "chkmwandmf";
			this.chkmwandmf.Size = new System.Drawing.Size(200, 16);
			this.chkmwandmf.TabIndex = 2;
			this.chkmwandmf.Text = "Put MW and MF in the same table";
			// 
			// Label9
			// 
			this.Label9.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label9.Location = new System.Drawing.Point(24, 48);
			this.Label9.Name = "Label9";
			this.Label9.Size = new System.Drawing.Size(96, 16);
			this.Label9.TabIndex = 44;
			this.Label9.Text = "Connection String";
			// 
			// chkMWs
			// 
			this.chkMWs.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkMWs.Location = new System.Drawing.Point(152, 168);
			this.chkMWs.Name = "chkMWs";
			this.chkMWs.Size = new System.Drawing.Size(96, 16);
			this.chkMWs.TabIndex = 4;
			this.chkMWs.Text = "Populate MWs";
			// 
			// chkDoDBOps
			// 
			this.chkDoDBOps.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkDoDBOps.Location = new System.Drawing.Point(24, 16);
			this.chkDoDBOps.Name = "chkDoDBOps";
			this.chkDoDBOps.Size = new System.Drawing.Size(176, 16);
			this.chkDoDBOps.TabIndex = 0;
			this.chkDoDBOps.Text = "Perform DB Operations";
			// 
			// lblProcessing
			// 
			this.lblProcessing.BackColor = System.Drawing.SystemColors.Control;
			this.lblProcessing.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblProcessing.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProcessing.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblProcessing.Location = new System.Drawing.Point(26, 461);
			this.lblProcessing.Name = "lblProcessing";
			this.lblProcessing.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblProcessing.Size = new System.Drawing.Size(229, 65);
			this.lblProcessing.TabIndex = 46;
			this.lblProcessing.Text = "Processing";
			// 
			// lblGifName
			// 
			this.lblGifName.BackColor = System.Drawing.SystemColors.Control;
			this.lblGifName.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblGifName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblGifName.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblGifName.Location = new System.Drawing.Point(26, 526);
			this.lblGifName.Name = "lblGifName";
			this.lblGifName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblGifName.Size = new System.Drawing.Size(326, 18);
			this.lblGifName.TabIndex = 47;
			// 
			// cmdCreateFiles
			// 
			this.cmdCreateFiles.BackColor = System.Drawing.SystemColors.Control;
			this.cmdCreateFiles.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdCreateFiles.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmdCreateFiles.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdCreateFiles.Location = new System.Drawing.Point(468, 461);
			this.cmdCreateFiles.Name = "cmdCreateFiles";
			this.cmdCreateFiles.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdCreateFiles.Size = new System.Drawing.Size(138, 33);
			this.cmdCreateFiles.TabIndex = 49;
			this.cmdCreateFiles.Text = "Process Structures";
			this.cmdCreateFiles.UseVisualStyleBackColor = false;
			this.cmdCreateFiles.Click += new System.EventHandler(this.cmdCreateFiles_Click);
			// 
			// btnExit
			// 
			this.btnExit.BackColor = System.Drawing.SystemColors.Control;
			this.btnExit.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnExit.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnExit.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnExit.Location = new System.Drawing.Point(750, 461);
			this.btnExit.Name = "btnExit";
			this.btnExit.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnExit.Size = new System.Drawing.Size(61, 33);
			this.btnExit.TabIndex = 50;
			this.btnExit.Text = "Exit";
			this.btnExit.UseVisualStyleBackColor = false;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// lblTabMessage
			// 
			this.lblTabMessage.AutoSize = true;
			this.lblTabMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.lblTabMessage.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTabMessage.ForeColor = System.Drawing.Color.DarkRed;
			this.lblTabMessage.Location = new System.Drawing.Point(13, 82);
			this.lblTabMessage.Name = "lblTabMessage";
			this.lblTabMessage.Size = new System.Drawing.Size(86, 15);
			this.lblTabMessage.TabIndex = 52;
			this.lblTabMessage.Text = "Tabs Message";
			// 
			// cmbProcessTab
			// 
			this.cmbProcessTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbProcessTab.Font = new System.Drawing.Font("Times New Roman", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cmbProcessTab.FormattingEnabled = true;
			this.cmbProcessTab.Items.AddRange(new object[] {
            "Directory of files Input",
            "Database Input (see Database Input tab)",
            "SDF Input (see SDF Input tab)"});
			this.cmbProcessTab.Location = new System.Drawing.Point(535, 31);
			this.cmbProcessTab.Name = "cmbProcessTab";
			this.cmbProcessTab.Size = new System.Drawing.Size(239, 23);
			this.cmbProcessTab.TabIndex = 2;
			this.cmbProcessTab.SelectedIndexChanged += new System.EventHandler(this.cmbProcessTab_SelectedIndexChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(827, 579);
			this.Controls.Add(this.cmbProcessTab);
			this.Controls.Add(this.lblTabMessage);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.cmdCreateFiles);
			this.Controls.Add(this.lblGifName);
			this.Controls.Add(this.lblProcessing);
			this.Controls.Add(this.tabOptions);
			this.Controls.Add(this.btnBrowseGet);
			this.Controls.Add(this.txtGetFilesPath);
			this.Controls.Add(this.lblInput);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.tabOptions.ResumeLayout(false);
			this.tabGOO.ResumeLayout(false);
			this.tabGOO.PerformLayout();
			this.tabAOO.ResumeLayout(false);
			this.tabAOO.PerformLayout();
			this.tabDBInput.ResumeLayout(false);
			this.tabDBInput.PerformLayout();
			this.tabSDFStructGrab.ResumeLayout(false);
			this.tabSDFStructGrab.PerformLayout();
			this.tabDBOptions.ResumeLayout(false);
			this.tabDBOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnBrowseGet;
        public System.Windows.Forms.TextBox txtGetFilesPath;
        public System.Windows.Forms.Label lblInput;
        internal System.Windows.Forms.TabControl tabOptions;
        internal System.Windows.Forms.TabPage tabGOO;
        internal System.Windows.Forms.Button btnBrowsePut;
        public System.Windows.Forms.Label Label5;
        public System.Windows.Forms.TextBox txtRes;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.Label lblRes;
        internal System.Windows.Forms.CheckBox chkCreateFiles;
        public System.Windows.Forms.TextBox txtType;
        public System.Windows.Forms.TextBox txtPutFilePath;
        public System.Windows.Forms.Label Label1;
        public System.Windows.Forms.Label lblType;
        public System.Windows.Forms.Label lblOutput;
        public System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TabPage tabAOO;
        internal System.Windows.Forms.Button btnBrowseSD;
        public System.Windows.Forms.TextBox txtSetDoc;
        public System.Windows.Forms.TextBox txtAtomFT;
        public System.Windows.Forms.TextBox txtAtomFS;
        internal System.Windows.Forms.CheckBox chkUseSetDoc;
        public System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.CheckBox chkChangeFT;
        public System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.CheckBox chkChangeFS;
        public System.Windows.Forms.Label lblFontSize;
        internal System.Windows.Forms.TabPage tabDBOptions;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Button btnBrowseDB;
        internal System.Windows.Forms.Label lblInsTblNm;
        internal System.Windows.Forms.TextBox txtMFtbl;
        internal System.Windows.Forms.TextBox txtMWMFtbl;
        internal System.Windows.Forms.TextBox txtMWtbl;
        internal System.Windows.Forms.TextBox txtConnString;
        internal System.Windows.Forms.CheckBox chkMFs;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.CheckBox chkmwandmf;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.CheckBox chkMWs;
        internal System.Windows.Forms.CheckBox chkDoDBOps;
        public System.Windows.Forms.Label lblProcessing;
        public System.Windows.Forms.Label lblGifName;
        private System.Windows.Forms.TabPage tabDBInput;
        internal System.Windows.Forms.TextBox txtoutTable;
        internal System.Windows.Forms.Label label11;
        internal System.Windows.Forms.TextBox txtConnStr2;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txttheStructure;
        internal System.Windows.Forms.Label label13;
        internal System.Windows.Forms.TextBox txtoutputID;
        internal System.Windows.Forms.Label label12;
        internal System.Windows.Forms.TextBox txtQryBox;
		private System.Windows.Forms.CheckBox chkCurrQry;
		private System.Windows.Forms.Label lblEnterQuery;
		private System.Windows.Forms.Label lblOR;
        private System.Windows.Forms.Label lblBuildQuery;
        public System.Windows.Forms.Button cmdCreateFiles;
        private System.Windows.Forms.CheckBox chkEmtStructure;
        public System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TabPage tabSDFStructGrab;
        private System.Windows.Forms.CheckBox chkSdfAppExistTable;
        private System.Windows.Forms.TextBox txtSdfTableName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtSdfIDField;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnBrowseSdfFile;
        private System.Windows.Forms.TextBox txtInputSDF;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtSdfAccessDB;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbSdfSelect;
        private System.Windows.Forms.Label lblSdfProcess;
        private System.Windows.Forms.Label lblTabMessage;
        public System.Windows.Forms.Label lblSdfDirectoryMessage;
        private System.Windows.Forms.ComboBox cmbProcessTab;
    }
}

