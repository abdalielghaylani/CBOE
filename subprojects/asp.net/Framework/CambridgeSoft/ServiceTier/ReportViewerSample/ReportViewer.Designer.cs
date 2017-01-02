using CambridgeSoft.COE.Framework.ServerControls.Reporting.Win;
namespace CambridgeSoft.COE.Framework.ReportViewer
{
    partial class ReportViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportViewer));
            this.DataViewButton = new System.Windows.Forms.Button();
            this.DataViewTextBox = new System.Windows.Forms.TextBox();
            this.ResultsCriteriaTextBox = new System.Windows.Forms.TextBox();
            this.SearchCriteriaTextBox = new System.Windows.Forms.TextBox();
            this.ResultsCriteriaButton = new System.Windows.Forms.Button();
            this.SearchCriteriaButton = new System.Windows.Forms.Button();
            this.SearchButton = new System.Windows.Forms.Button();
            this.PagingInfoButton = new System.Windows.Forms.Button();
            this.PagingInfoTextBox = new System.Windows.Forms.TextBox();
            this.FormErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.StatusTextBox = new System.Windows.Forms.RichTextBox();
            this.EditResultsCriteriaButton = new System.Windows.Forms.Button();
            this.GenerateReportButton = new System.Windows.Forms.Button();
            this.EditSearchCriteriaButton = new System.Windows.Forms.Button();
            this.EditPagingInfoButton = new System.Windows.Forms.Button();
            this.UseHitlistIdRadioButton = new System.Windows.Forms.RadioButton();
            this.UseDatasetRadioButton = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.CreateReportLinkLabel = new System.Windows.Forms.LinkLabel();
            this.RecordPickingButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.UserNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CategoriesComboBox = new System.Windows.Forms.ComboBox();
            this.ReportTypesComboBox = new System.Windows.Forms.ComboBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SampleDataSetPassingReportViewer = new CambridgeSoft.COE.Framework.ServerControls.Reporting.Win.COEReportViewer();
            this.reportPreviewer1 = new CambridgeSoft.COE.Framework.ServerControls.Reporting.Win.COEReportViewer();
            this.ReportSelectionUserControl = new CambridgeSoft.COE.Framework.ServerControls.Reporting.Win.COEReportSelector();
            ((System.ComponentModel.ISupportInitialize)(this.FormErrorProvider)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataViewButton
            // 
            this.DataViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DataViewButton.Location = new System.Drawing.Point(278, 442);
            this.DataViewButton.Name = "DataViewButton";
            this.DataViewButton.Size = new System.Drawing.Size(24, 23);
            this.DataViewButton.TabIndex = 2;
            this.DataViewButton.Text = "...";
            this.DataViewButton.UseVisualStyleBackColor = true;
            this.DataViewButton.Click += new System.EventHandler(this.DataViewButton_Click);
            // 
            // DataViewTextBox
            // 
            this.DataViewTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DataViewTextBox.Location = new System.Drawing.Point(6, 444);
            this.DataViewTextBox.Name = "DataViewTextBox";
            this.DataViewTextBox.Size = new System.Drawing.Size(266, 20);
            this.DataViewTextBox.TabIndex = 3;
            this.DataViewTextBox.Text = "Pick a DataView...";
            this.DataViewTextBox.TextChanged += new System.EventHandler(this.DataViewTextBox_TextChanged);
            // 
            // ResultsCriteriaTextBox
            // 
            this.ResultsCriteriaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ResultsCriteriaTextBox.Location = new System.Drawing.Point(6, 470);
            this.ResultsCriteriaTextBox.Name = "ResultsCriteriaTextBox";
            this.ResultsCriteriaTextBox.Size = new System.Drawing.Size(266, 20);
            this.ResultsCriteriaTextBox.TabIndex = 4;
            this.ResultsCriteriaTextBox.Text = "Pick a ResultsCriteria...";
            // 
            // SearchCriteriaTextBox
            // 
            this.SearchCriteriaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchCriteriaTextBox.Location = new System.Drawing.Point(6, 496);
            this.SearchCriteriaTextBox.Name = "SearchCriteriaTextBox";
            this.SearchCriteriaTextBox.Size = new System.Drawing.Size(266, 20);
            this.SearchCriteriaTextBox.TabIndex = 5;
            this.SearchCriteriaTextBox.Text = "Pick a Search Criteria...";
            // 
            // ResultsCriteriaButton
            // 
            this.ResultsCriteriaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ResultsCriteriaButton.Location = new System.Drawing.Point(278, 468);
            this.ResultsCriteriaButton.Name = "ResultsCriteriaButton";
            this.ResultsCriteriaButton.Size = new System.Drawing.Size(24, 23);
            this.ResultsCriteriaButton.TabIndex = 6;
            this.ResultsCriteriaButton.Text = "...";
            this.ResultsCriteriaButton.UseVisualStyleBackColor = true;
            this.ResultsCriteriaButton.Click += new System.EventHandler(this.ResultsCriteriaButton_Click);
            // 
            // SearchCriteriaButton
            // 
            this.SearchCriteriaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchCriteriaButton.Location = new System.Drawing.Point(278, 494);
            this.SearchCriteriaButton.Name = "SearchCriteriaButton";
            this.SearchCriteriaButton.Size = new System.Drawing.Size(24, 23);
            this.SearchCriteriaButton.TabIndex = 7;
            this.SearchCriteriaButton.Text = "...";
            this.SearchCriteriaButton.UseVisualStyleBackColor = true;
            this.SearchCriteriaButton.Click += new System.EventHandler(this.SearchCriteriaButton_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchButton.Location = new System.Drawing.Point(359, 442);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 8;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // PagingInfoButton
            // 
            this.PagingInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PagingInfoButton.Location = new System.Drawing.Point(278, 520);
            this.PagingInfoButton.Name = "PagingInfoButton";
            this.PagingInfoButton.Size = new System.Drawing.Size(24, 23);
            this.PagingInfoButton.TabIndex = 10;
            this.PagingInfoButton.Text = "...";
            this.PagingInfoButton.UseVisualStyleBackColor = true;
            this.PagingInfoButton.Click += new System.EventHandler(this.PagingInfoButton_Click);
            // 
            // PagingInfoTextBox
            // 
            this.PagingInfoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PagingInfoTextBox.Location = new System.Drawing.Point(6, 522);
            this.PagingInfoTextBox.Name = "PagingInfoTextBox";
            this.PagingInfoTextBox.Size = new System.Drawing.Size(266, 20);
            this.PagingInfoTextBox.TabIndex = 9;
            this.PagingInfoTextBox.Text = "Pick a Paging Info...";
            // 
            // FormErrorProvider
            // 
            this.FormErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.FormErrorProvider.ContainerControl = this;
            // 
            // StatusTextBox
            // 
            this.StatusTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusTextBox.Location = new System.Drawing.Point(440, 443);
            this.StatusTextBox.Name = "StatusTextBox";
            this.StatusTextBox.Size = new System.Drawing.Size(543, 96);
            this.StatusTextBox.TabIndex = 12;
            this.StatusTextBox.Text = "";
            this.StatusTextBox.WordWrap = false;
            // 
            // EditResultsCriteriaButton
            // 
            this.EditResultsCriteriaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditResultsCriteriaButton.Location = new System.Drawing.Point(320, 468);
            this.EditResultsCriteriaButton.Name = "EditResultsCriteriaButton";
            this.EditResultsCriteriaButton.Size = new System.Drawing.Size(33, 23);
            this.EditResultsCriteriaButton.TabIndex = 13;
            this.EditResultsCriteriaButton.Text = "Edit";
            this.EditResultsCriteriaButton.UseVisualStyleBackColor = true;
            this.EditResultsCriteriaButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // GenerateReportButton
            // 
            this.GenerateReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GenerateReportButton.Location = new System.Drawing.Point(359, 466);
            this.GenerateReportButton.Name = "GenerateReportButton";
            this.GenerateReportButton.Size = new System.Drawing.Size(74, 23);
            this.GenerateReportButton.TabIndex = 14;
            this.GenerateReportButton.Text = "Build Report";
            this.GenerateReportButton.UseVisualStyleBackColor = true;
            this.GenerateReportButton.Click += new System.EventHandler(this.GenerateReportButton_Click);
            // 
            // EditSearchCriteriaButton
            // 
            this.EditSearchCriteriaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditSearchCriteriaButton.Location = new System.Drawing.Point(320, 494);
            this.EditSearchCriteriaButton.Name = "EditSearchCriteriaButton";
            this.EditSearchCriteriaButton.Size = new System.Drawing.Size(33, 23);
            this.EditSearchCriteriaButton.TabIndex = 15;
            this.EditSearchCriteriaButton.Text = "Edit";
            this.EditSearchCriteriaButton.UseVisualStyleBackColor = true;
            // 
            // EditPagingInfoButton
            // 
            this.EditPagingInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditPagingInfoButton.Location = new System.Drawing.Point(320, 519);
            this.EditPagingInfoButton.Name = "EditPagingInfoButton";
            this.EditPagingInfoButton.Size = new System.Drawing.Size(33, 23);
            this.EditPagingInfoButton.TabIndex = 16;
            this.EditPagingInfoButton.Text = "Edit";
            this.EditPagingInfoButton.UseVisualStyleBackColor = true;
            // 
            // UseHitlistIdRadioButton
            // 
            this.UseHitlistIdRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UseHitlistIdRadioButton.AutoSize = true;
            this.UseHitlistIdRadioButton.Checked = true;
            this.UseHitlistIdRadioButton.Location = new System.Drawing.Point(14, 416);
            this.UseHitlistIdRadioButton.Name = "UseHitlistIdRadioButton";
            this.UseHitlistIdRadioButton.Size = new System.Drawing.Size(81, 17);
            this.UseHitlistIdRadioButton.TabIndex = 17;
            this.UseHitlistIdRadioButton.TabStop = true;
            this.UseHitlistIdRadioButton.Text = "Use HitlistId";
            this.UseHitlistIdRadioButton.UseVisualStyleBackColor = true;
            // 
            // UseDatasetRadioButton
            // 
            this.UseDatasetRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UseDatasetRadioButton.AutoSize = true;
            this.UseDatasetRadioButton.Location = new System.Drawing.Point(94, 416);
            this.UseDatasetRadioButton.Name = "UseDatasetRadioButton";
            this.UseDatasetRadioButton.Size = new System.Drawing.Size(84, 17);
            this.UseDatasetRadioButton.TabIndex = 18;
            this.UseDatasetRadioButton.Text = "Use Dataset";
            this.UseDatasetRadioButton.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(2, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(998, 575);
            this.tabControl1.TabIndex = 19;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.CreateReportLinkLabel);
            this.tabPage2.Controls.Add(this.RecordPickingButton);
            this.tabPage2.Controls.Add(this.SampleDataSetPassingReportViewer);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(990, 549);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Single DataView Report";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Enter += new System.EventHandler(this.tabPage2_Enter);
            // 
            // CreateReportLinkLabel
            // 
            this.CreateReportLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CreateReportLinkLabel.AutoSize = true;
            this.CreateReportLinkLabel.Enabled = false;
            this.CreateReportLinkLabel.Location = new System.Drawing.Point(7, 489);
            this.CreateReportLinkLabel.Name = "CreateReportLinkLabel";
            this.CreateReportLinkLabel.Size = new System.Drawing.Size(73, 13);
            this.CreateReportLinkLabel.TabIndex = 4;
            this.CreateReportLinkLabel.TabStop = true;
            this.CreateReportLinkLabel.Text = "Create Report";
            this.CreateReportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DesignReportLinkLabel_LinkClicked);
            // 
            // RecordPickingButton
            // 
            this.RecordPickingButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RecordPickingButton.Location = new System.Drawing.Point(426, 516);
            this.RecordPickingButton.Name = "RecordPickingButton";
            this.RecordPickingButton.Size = new System.Drawing.Size(113, 23);
            this.RecordPickingButton.TabIndex = 3;
            this.RecordPickingButton.Text = "Pick a record...";
            this.RecordPickingButton.UseVisualStyleBackColor = true;
            this.RecordPickingButton.Click += new System.EventHandler(this.RecordPickingButton_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.UserNameTextBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.CategoriesComboBox);
            this.tabPage1.Controls.Add(this.ReportTypesComboBox);
            this.tabPage1.Controls.Add(this.UseDatasetRadioButton);
            this.tabPage1.Controls.Add(this.UseHitlistIdRadioButton);
            this.tabPage1.Controls.Add(this.EditPagingInfoButton);
            this.tabPage1.Controls.Add(this.DataViewButton);
            this.tabPage1.Controls.Add(this.EditSearchCriteriaButton);
            this.tabPage1.Controls.Add(this.DataViewTextBox);
            this.tabPage1.Controls.Add(this.GenerateReportButton);
            this.tabPage1.Controls.Add(this.ResultsCriteriaTextBox);
            this.tabPage1.Controls.Add(this.EditResultsCriteriaButton);
            this.tabPage1.Controls.Add(this.SearchCriteriaTextBox);
            this.tabPage1.Controls.Add(this.StatusTextBox);
            this.tabPage1.Controls.Add(this.ResultsCriteriaButton);
            this.tabPage1.Controls.Add(this.PagingInfoButton);
            this.tabPage1.Controls.Add(this.SearchCriteriaButton);
            this.tabPage1.Controls.Add(this.PagingInfoTextBox);
            this.tabPage1.Controls.Add(this.SearchButton);
            this.tabPage1.Controls.Add(this.reportPreviewer1);
            this.tabPage1.Controls.Add(this.ReportSelectionUserControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(990, 549);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Report Listing";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(634, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Category";
            // 
            // UserNameTextBox
            // 
            this.UserNameTextBox.Location = new System.Drawing.Point(836, 19);
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.Size = new System.Drawing.Size(147, 20);
            this.UserNameTextBox.TabIndex = 22;
            this.UserNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserNameTextBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(443, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Type";
            // 
            // CategoriesComboBox
            // 
            this.CategoriesComboBox.FormattingEnabled = true;
            this.CategoriesComboBox.Items.AddRange(new object[] {
            "",
            "Containers",
            "Locations",
            "Compounds"});
            this.CategoriesComboBox.Location = new System.Drawing.Point(688, 19);
            this.CategoriesComboBox.Name = "CategoriesComboBox";
            this.CategoriesComboBox.Size = new System.Drawing.Size(121, 21);
            this.CategoriesComboBox.TabIndex = 20;
            this.CategoriesComboBox.SelectedIndexChanged += new System.EventHandler(this.CategoriesComboBox_SelectedIndexChanged);
            // 
            // ReportTypesComboBox
            // 
            this.ReportTypesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReportTypesComboBox.FormattingEnabled = true;
            this.ReportTypesComboBox.Location = new System.Drawing.Point(484, 19);
            this.ReportTypesComboBox.Name = "ReportTypesComboBox";
            this.ReportTypesComboBox.Size = new System.Drawing.Size(121, 21);
            this.ReportTypesComboBox.Sorted = true;
            this.ReportTypesComboBox.TabIndex = 19;
            this.ReportTypesComboBox.SelectedIndexChanged += new System.EventHandler(this.ReportTypesComboBox_SelectedIndexChanged);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // SampleDataSetPassingReportViewer
            // 
            this.SampleDataSetPassingReportViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SampleDataSetPassingReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SampleDataSetPassingReportViewer.DataSource = null;
            this.SampleDataSetPassingReportViewer.HitlistId = -1;
            this.SampleDataSetPassingReportViewer.Location = new System.Drawing.Point(3, 6);
            this.SampleDataSetPassingReportViewer.Name = "SampleDataSetPassingReportViewer";
            this.SampleDataSetPassingReportViewer.ReportBuilder = null;
            this.SampleDataSetPassingReportViewer.ReportBuilderMeta = ((CambridgeSoft.COE.Framework.Common.Messaging.ReportBuilderMeta)(resources.GetObject("SampleDataSetPassingReportViewer.ReportBuilderMeta")));
            this.SampleDataSetPassingReportViewer.ReportId = 0;
            this.SampleDataSetPassingReportViewer.ShowMenu = true;
            this.SampleDataSetPassingReportViewer.ShowToolbar = true;
            this.SampleDataSetPassingReportViewer.Size = new System.Drawing.Size(981, 476);
            this.SampleDataSetPassingReportViewer.TabIndex = 2;
            this.SampleDataSetPassingReportViewer.ExitRequested += new System.EventHandler(this.SampleDataSetPassingReportViewer_ExitRequested);
            this.SampleDataSetPassingReportViewer.EditReportRequested += new System.EventHandler(this.reportPreviewer1_EditReportRequested);
            // 
            // reportPreviewer1
            // 
            this.reportPreviewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reportPreviewer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.reportPreviewer1.DataSource = null;
            this.reportPreviewer1.HitlistId = -1;
            this.reportPreviewer1.Location = new System.Drawing.Point(6, 55);
            this.reportPreviewer1.Name = "reportPreviewer1";
            this.reportPreviewer1.ReportBuilder = null;
            this.reportPreviewer1.ReportBuilderMeta = ((CambridgeSoft.COE.Framework.Common.Messaging.ReportBuilderMeta)(resources.GetObject("reportPreviewer1.ReportBuilderMeta")));
            this.reportPreviewer1.ReportId = 0;
            this.reportPreviewer1.ShowMenu = true;
            this.reportPreviewer1.ShowToolbar = true;
            this.reportPreviewer1.Size = new System.Drawing.Size(981, 355);
            this.reportPreviewer1.TabIndex = 1;
            this.reportPreviewer1.ExitRequested += new System.EventHandler(this.reportPreviewer1_ExitRequested);
            this.reportPreviewer1.EditReportRequested += new System.EventHandler(this.reportPreviewer1_EditReportRequested);
            // 
            // ReportSelectionUserControl
            // 
            this.ReportSelectionUserControl.ApplicationName = null;
            this.ReportSelectionUserControl.Category = null;
            this.ReportSelectionUserControl.DataViewId = -1;
            this.ReportSelectionUserControl.Location = new System.Drawing.Point(3, 5);
            this.ReportSelectionUserControl.Name = "ReportSelectionUserControl";
            this.ReportSelectionUserControl.ReportType = CambridgeSoft.COE.Framework.COEReportingService.COEReportType.List;
            this.ReportSelectionUserControl.ShowCreateNewButton = false;
            this.ReportSelectionUserControl.ShowDeleteButton = false;
            this.ReportSelectionUserControl.ShowPrivateReports = false;
            this.ReportSelectionUserControl.Size = new System.Drawing.Size(434, 44);
            this.ReportSelectionUserControl.TabIndex = 0;
            this.ReportSelectionUserControl.UserName = null;
            this.ReportSelectionUserControl.SelectedReportBuilderMetaChanged += new System.EventHandler(this.reportSelectionUserControl1_SelectedReportBuilderInfoChanged);
            this.ReportSelectionUserControl.DeleteSelectedReport += new System.EventHandler(this.ReportSelectionUserControl_DeleteSelectedReport);
            this.ReportSelectionUserControl.CreateNewReport += new System.EventHandler(this.ReportSelectionUserControl_CreateNewReport);
            // 
            // ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 577);
            this.Controls.Add(this.tabControl1);
            this.Name = "ReportViewer";
            this.Text = "ReportViewer";
            this.Load += new System.EventHandler(this.ReportViewer_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReportViewer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.FormErrorProvider)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private COEReportSelector ReportSelectionUserControl;
        private COEReportViewer reportPreviewer1;
        private System.Windows.Forms.Button DataViewButton;
        private System.Windows.Forms.TextBox DataViewTextBox;
        private System.Windows.Forms.TextBox ResultsCriteriaTextBox;
        private System.Windows.Forms.TextBox SearchCriteriaTextBox;
        private System.Windows.Forms.Button ResultsCriteriaButton;
        private System.Windows.Forms.Button SearchCriteriaButton;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button PagingInfoButton;
        private System.Windows.Forms.TextBox PagingInfoTextBox;
        private System.Windows.Forms.ErrorProvider FormErrorProvider;
        private System.Windows.Forms.RichTextBox StatusTextBox;
        private System.Windows.Forms.Button EditResultsCriteriaButton;
        private System.Windows.Forms.Button EditPagingInfoButton;
        private System.Windows.Forms.Button EditSearchCriteriaButton;
        private System.Windows.Forms.Button GenerateReportButton;
        private System.Windows.Forms.RadioButton UseDatasetRadioButton;
        private System.Windows.Forms.RadioButton UseHitlistIdRadioButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button RecordPickingButton;
        private COEReportViewer SampleDataSetPassingReportViewer;
        private System.Windows.Forms.LinkLabel CreateReportLinkLabel;
        private System.Windows.Forms.ComboBox ReportTypesComboBox;
        private System.Windows.Forms.ComboBox CategoriesComboBox;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}