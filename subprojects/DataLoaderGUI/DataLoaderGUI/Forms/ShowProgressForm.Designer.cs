namespace CambridgeSoft.DataLoaderGUI.Forms
{
    partial class ShowProgressForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.StopWatchLabel = new System.Windows.Forms.Label();
            this.CopyLogButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.ResultProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.WaitLabel = new System.Windows.Forms.Label();
            this.ScanPanel = new System.Windows.Forms.Panel();
            this.InvalidRecordsScanLabel = new System.Windows.Forms.Label();
            this.DuplicateRecordsLabel = new System.Windows.Forms.Label();
            this.UniqueRecordsLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ImportPanel = new System.Windows.Forms.Panel();
            this.InvalidRecordsImportLabel = new System.Windows.Forms.Label();
            this.NoActionRecordsLabel = new System.Windows.Forms.Label();
            this.PermRecordsLabel = new System.Windows.Forms.Label();
            this.TempRecordsLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.ScanPanel.SuspendLayout();
            this.ImportPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Controls.Add(this.StopWatchLabel);
            this.panel1.Controls.Add(this.CopyLogButton);
            this.panel1.Controls.Add(this.OkButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 182);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(594, 34);
            this.panel1.TabIndex = 1;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(488, 6);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // StopWatchLabel
            // 
            this.StopWatchLabel.AutoSize = true;
            this.StopWatchLabel.Location = new System.Drawing.Point(218, 11);
            this.StopWatchLabel.Name = "StopWatchLabel";
            this.StopWatchLabel.Size = new System.Drawing.Size(64, 13);
            this.StopWatchLabel.TabIndex = 3;
            this.StopWatchLabel.Text = "00:00:00,00";
            // 
            // CopyLogButton
            // 
            this.CopyLogButton.AutoSize = true;
            this.CopyLogButton.Location = new System.Drawing.Point(15, 6);
            this.CopyLogButton.Name = "CopyLogButton";
            this.CopyLogButton.Size = new System.Drawing.Size(122, 23);
            this.CopyLogButton.TabIndex = 2;
            this.CopyLogButton.Text = "Copy Log to ClipBoard";
            this.CopyLogButton.UseVisualStyleBackColor = true;
            this.CopyLogButton.Click += new System.EventHandler(this.CopyLogButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(405, 6);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ResultProgressBar
            // 
            this.ResultProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ResultProgressBar.Location = new System.Drawing.Point(0, 167);
            this.ResultProgressBar.Name = "ResultProgressBar";
            this.ResultProgressBar.Size = new System.Drawing.Size(594, 15);
            this.ResultProgressBar.TabIndex = 2;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 9);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(147, 13);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Text = "(0%) 0 of 0 records processed";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // WaitLabel
            // 
            this.WaitLabel.AutoSize = true;
            this.WaitLabel.Location = new System.Drawing.Point(261, 80);
            this.WaitLabel.Name = "WaitLabel";
            this.WaitLabel.Size = new System.Drawing.Size(73, 13);
            this.WaitLabel.TabIndex = 5;
            this.WaitLabel.Text = "Please Wait...";
            this.WaitLabel.Visible = false;
            // 
            // ScanPanel
            // 
            this.ScanPanel.Controls.Add(this.InvalidRecordsScanLabel);
            this.ScanPanel.Controls.Add(this.DuplicateRecordsLabel);
            this.ScanPanel.Controls.Add(this.UniqueRecordsLabel);
            this.ScanPanel.Controls.Add(this.label4);
            this.ScanPanel.Controls.Add(this.label2);
            this.ScanPanel.Controls.Add(this.label1);
            this.ScanPanel.Location = new System.Drawing.Point(0, 38);
            this.ScanPanel.Name = "ScanPanel";
            this.ScanPanel.Size = new System.Drawing.Size(594, 100);
            this.ScanPanel.TabIndex = 6;
            // 
            // InvalidRecordsScanLabel
            // 
            this.InvalidRecordsScanLabel.AutoSize = true;
            this.InvalidRecordsScanLabel.Location = new System.Drawing.Point(121, 51);
            this.InvalidRecordsScanLabel.Name = "InvalidRecordsScanLabel";
            this.InvalidRecordsScanLabel.Size = new System.Drawing.Size(13, 13);
            this.InvalidRecordsScanLabel.TabIndex = 1;
            this.InvalidRecordsScanLabel.Text = "0";
            // 
            // DuplicateRecordsLabel
            // 
            this.DuplicateRecordsLabel.AutoSize = true;
            this.DuplicateRecordsLabel.Location = new System.Drawing.Point(121, 30);
            this.DuplicateRecordsLabel.Name = "DuplicateRecordsLabel";
            this.DuplicateRecordsLabel.Size = new System.Drawing.Size(13, 13);
            this.DuplicateRecordsLabel.TabIndex = 1;
            this.DuplicateRecordsLabel.Text = "0";
            // 
            // UniqueRecordsLabel
            // 
            this.UniqueRecordsLabel.AutoSize = true;
            this.UniqueRecordsLabel.Location = new System.Drawing.Point(121, 9);
            this.UniqueRecordsLabel.Name = "UniqueRecordsLabel";
            this.UniqueRecordsLabel.Size = new System.Drawing.Size(13, 13);
            this.UniqueRecordsLabel.TabIndex = 1;
            this.UniqueRecordsLabel.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Invalid Records :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Duplicate Records :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unique Records :";
            // 
            // ImportPanel
            // 
            this.ImportPanel.Controls.Add(this.InvalidRecordsImportLabel);
            this.ImportPanel.Controls.Add(this.NoActionRecordsLabel);
            this.ImportPanel.Controls.Add(this.PermRecordsLabel);
            this.ImportPanel.Controls.Add(this.TempRecordsLabel);
            this.ImportPanel.Controls.Add(this.label6);
            this.ImportPanel.Controls.Add(this.label9);
            this.ImportPanel.Controls.Add(this.label10);
            this.ImportPanel.Controls.Add(this.label11);
            this.ImportPanel.Location = new System.Drawing.Point(0, 38);
            this.ImportPanel.Name = "ImportPanel";
            this.ImportPanel.Size = new System.Drawing.Size(594, 100);
            this.ImportPanel.TabIndex = 7;
            // 
            // InvalidRecordsImportLabel
            // 
            this.InvalidRecordsImportLabel.AutoSize = true;
            this.InvalidRecordsImportLabel.Location = new System.Drawing.Point(127, 72);
            this.InvalidRecordsImportLabel.Name = "InvalidRecordsImportLabel";
            this.InvalidRecordsImportLabel.Size = new System.Drawing.Size(13, 13);
            this.InvalidRecordsImportLabel.TabIndex = 1;
            this.InvalidRecordsImportLabel.Text = "0";
            // 
            // NoActionRecordsLabel
            // 
            this.NoActionRecordsLabel.AutoSize = true;
            this.NoActionRecordsLabel.Location = new System.Drawing.Point(127, 51);
            this.NoActionRecordsLabel.Name = "NoActionRecordsLabel";
            this.NoActionRecordsLabel.Size = new System.Drawing.Size(13, 13);
            this.NoActionRecordsLabel.TabIndex = 1;
            this.NoActionRecordsLabel.Text = "0";
            // 
            // PermRecordsLabel
            // 
            this.PermRecordsLabel.AutoSize = true;
            this.PermRecordsLabel.Location = new System.Drawing.Point(127, 30);
            this.PermRecordsLabel.Name = "PermRecordsLabel";
            this.PermRecordsLabel.Size = new System.Drawing.Size(13, 13);
            this.PermRecordsLabel.TabIndex = 1;
            this.PermRecordsLabel.Text = "0";
            // 
            // TempRecordsLabel
            // 
            this.TempRecordsLabel.AutoSize = true;
            this.TempRecordsLabel.Location = new System.Drawing.Point(127, 9);
            this.TempRecordsLabel.Name = "TempRecordsLabel";
            this.TempRecordsLabel.Size = new System.Drawing.Size(13, 13);
            this.TempRecordsLabel.TabIndex = 1;
            this.TempRecordsLabel.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Invalid Records :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 51);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "No Action Records :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 30);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Permanent Records :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Temp Records :";
            // 
            // ShowProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 216);
            this.Controls.Add(this.WaitLabel);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.ResultProgressBar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ScanPanel);
            this.Controls.Add(this.ImportPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ShowProgressForm";
            this.ShowInTaskbar = false;
            this.Text = "ShowProgress";
            this.Load += new System.EventHandler(this.ShowProgress_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ScanPanel.ResumeLayout(false);
            this.ScanPanel.PerformLayout();
            this.ImportPanel.ResumeLayout(false);
            this.ImportPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar ResultProgressBar;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button CopyLogButton;
        private System.Windows.Forms.Label StopWatchLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label WaitLabel;
        private System.Windows.Forms.Panel ScanPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label InvalidRecordsScanLabel;
        private System.Windows.Forms.Label DuplicateRecordsLabel;
        private System.Windows.Forms.Label UniqueRecordsLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel ImportPanel;
        private System.Windows.Forms.Label NoActionRecordsLabel;
        private System.Windows.Forms.Label PermRecordsLabel;
        private System.Windows.Forms.Label TempRecordsLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label InvalidRecordsImportLabel;
        private System.Windows.Forms.Label label6;
    }
}