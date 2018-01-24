namespace RegistrationWebserviceTest {
    partial class WebServiceTest {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.UserNameTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TempIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.ProceedLinkLabel = new System.Windows.Forms.LinkLabel();
            this.LinkContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProceedButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ReviewButton = new System.Windows.Forms.Button();
            this.ReviewLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.PickListCriteria = new System.Windows.Forms.TextBox();
            this.PickListListBox = new System.Windows.Forms.ListBox();
            this.FillPickListButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.LinkContextMenuStrip.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // UserNameTextBox
            // 
            this.UserNameTextBox.Location = new System.Drawing.Point(107, 12);
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.UserNameTextBox.TabIndex = 2;
            this.UserNameTextBox.Text = "T5_85";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(107, 35);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(100, 20);
            this.PasswordTextBox.TabIndex = 3;
            this.PasswordTextBox.Text = "T5_85";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.UserNameTextBox);
            this.panel1.Controls.Add(this.PasswordTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 60);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.BrowseButton);
            this.panel2.Controls.Add(this.FileNameLabel);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(792, 100);
            this.panel2.TabIndex = 5;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(359, 72);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoEllipsis = true;
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.Location = new System.Drawing.Point(107, 16);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(110, 13);
            this.FileNameLabel.TabIndex = 1;
            this.FileNameLabel.Text = "Browse for a file first...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Xml File Name";
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.TempIDLinkLabel);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.ProceedLinkLabel);
            this.panel3.Controls.Add(this.ProceedButton);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 160);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(792, 111);
            this.panel3.TabIndex = 6;
            // 
            // TempIDLinkLabel
            // 
            this.TempIDLinkLabel.AutoEllipsis = true;
            this.TempIDLinkLabel.AutoSize = true;
            this.TempIDLinkLabel.Enabled = false;
            this.TempIDLinkLabel.Location = new System.Drawing.Point(139, 24);
            this.TempIDLinkLabel.Name = "TempIDLinkLabel";
            this.TempIDLinkLabel.Size = new System.Drawing.Size(160, 13);
            this.TempIDLinkLabel.TabIndex = 4;
            this.TempIDLinkLabel.TabStop = true;
            this.TempIDLinkLabel.Text = "Click here to get a Temporary ID";
            this.TempIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TempIDLinkLabel_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Temporary Id";
            // 
            // ProceedLinkLabel
            // 
            this.ProceedLinkLabel.AutoEllipsis = true;
            this.ProceedLinkLabel.AutoSize = true;
            this.ProceedLinkLabel.ContextMenuStrip = this.LinkContextMenuStrip;
            this.ProceedLinkLabel.Enabled = false;
            this.ProceedLinkLabel.Location = new System.Drawing.Point(139, 56);
            this.ProceedLinkLabel.Name = "ProceedLinkLabel";
            this.ProceedLinkLabel.Size = new System.Drawing.Size(149, 13);
            this.ProceedLinkLabel.TabIndex = 2;
            this.ProceedLinkLabel.TabStop = true;
            this.ProceedLinkLabel.Text = "Click Proceed to get the link...";
            this.ProceedLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ProceedLinkLabel_LinkClicked);
            // 
            // LinkContextMenuStrip
            // 
            this.LinkContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyToolStripMenuItem});
            this.LinkContextMenuStrip.Name = "LinkContextMenuStrip";
            this.LinkContextMenuStrip.Size = new System.Drawing.Size(157, 26);
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.CopyToolStripMenuItem.Text = "Copy ShortCut";
            this.CopyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // ProceedButton
            // 
            this.ProceedButton.Enabled = false;
            this.ProceedButton.Location = new System.Drawing.Point(359, 83);
            this.ProceedButton.Name = "ProceedButton";
            this.ProceedButton.Size = new System.Drawing.Size(75, 23);
            this.ProceedButton.TabIndex = 1;
            this.ProceedButton.Text = "Proceed";
            this.ProceedButton.UseVisualStyleBackColor = true;
            this.ProceedButton.Click += new System.EventHandler(this.ProceedButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Resulting URI to visit";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.DeleteButton);
            this.panel4.Controls.Add(this.ReviewButton);
            this.panel4.Controls.Add(this.ReviewLinkLabel);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 271);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(792, 100);
            this.panel4.TabIndex = 7;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Enabled = false;
            this.DeleteButton.Location = new System.Drawing.Point(441, 67);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteButton.TabIndex = 5;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ReviewButton
            // 
            this.ReviewButton.Enabled = false;
            this.ReviewButton.Location = new System.Drawing.Point(360, 67);
            this.ReviewButton.Name = "ReviewButton";
            this.ReviewButton.Size = new System.Drawing.Size(75, 23);
            this.ReviewButton.TabIndex = 3;
            this.ReviewButton.Text = "Review";
            this.ReviewButton.UseVisualStyleBackColor = true;
            this.ReviewButton.Click += new System.EventHandler(this.ReviewButton_Click);
            // 
            // ReviewLinkLabel
            // 
            this.ReviewLinkLabel.AutoEllipsis = true;
            this.ReviewLinkLabel.AutoSize = true;
            this.ReviewLinkLabel.ContextMenuStrip = this.LinkContextMenuStrip;
            this.ReviewLinkLabel.Enabled = false;
            this.ReviewLinkLabel.Location = new System.Drawing.Point(140, 46);
            this.ReviewLinkLabel.Name = "ReviewLinkLabel";
            this.ReviewLinkLabel.Size = new System.Drawing.Size(145, 13);
            this.ReviewLinkLabel.TabIndex = 3;
            this.ReviewLinkLabel.TabStop = true;
            this.ReviewLinkLabel.Text = "Click Review to get the link...";
            this.ReviewLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReviewLinkLabel_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Resulting URI to visit";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(593, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "After submiting the compound/mixture using the previous URI, you will be able to " +
                "go to the following URI to review/register it.";
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.PickListCriteria);
            this.panel5.Controls.Add(this.PickListListBox);
            this.panel5.Controls.Add(this.FillPickListButton);
            this.panel5.Controls.Add(this.label9);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 371);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(792, 177);
            this.panel5.TabIndex = 8;
            // 
            // PickListCriteria
            // 
            this.PickListCriteria.Location = new System.Drawing.Point(107, 15);
            this.PickListCriteria.Name = "PickListCriteria";
            this.PickListCriteria.Size = new System.Drawing.Size(553, 20);
            this.PickListCriteria.TabIndex = 5;
            // 
            // PickListListBox
            // 
            this.PickListListBox.FormattingEnabled = true;
            this.PickListListBox.Location = new System.Drawing.Point(107, 52);
            this.PickListListBox.Name = "PickListListBox";
            this.PickListListBox.Size = new System.Drawing.Size(655, 108);
            this.PickListListBox.TabIndex = 4;
            // 
            // FillPickListButton
            // 
            this.FillPickListButton.Location = new System.Drawing.Point(687, 15);
            this.FillPickListButton.Name = "FillPickListButton";
            this.FillPickListButton.Size = new System.Drawing.Size(75, 23);
            this.FillPickListButton.TabIndex = 3;
            this.FillPickListButton.Text = "Fill";
            this.FillPickListButton.UseVisualStyleBackColor = true;
            this.FillPickListButton.Click += new System.EventHandler(this.FillPickListButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Picklist criteria: ";
            // 
            // WebServiceTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(792, 549);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "WebServiceTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registration\'s Web Service Tester";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.LinkContextMenuStrip.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button ProceedButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel ProceedLinkLabel;
        private System.Windows.Forms.ContextMenuStrip LinkContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem CopyToolStripMenuItem;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button ReviewButton;
        private System.Windows.Forms.LinkLabel ReviewLinkLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel TempIDLinkLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ListBox PickListListBox;
        private System.Windows.Forms.Button FillPickListButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox PickListCriteria;
    }
}