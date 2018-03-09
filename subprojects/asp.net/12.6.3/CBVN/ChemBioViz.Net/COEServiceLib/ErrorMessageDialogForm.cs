using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace COEServiceLib
{
    public partial class ErrorMessageDialogForm : Form
    {
        private Label ErrorMessagelabel;
        private PictureBox pictureBox1;
        private TextBox ErrorMessageDescTextBox;
        private Label ExpandMessageLabel;
        private PictureBox ExpandPictureBox;
        private LinkLabel CopyLinkLabel;
        private Button OkButton;
    
        public bool IsCollapsed { get; set; }

        public ErrorMessageDialogForm()
        {
            InitializeComponent();
        }

        private void ErrorMessageDialogForm_Load(object sender, EventArgs e)
        {                        
            IsCollapsed = true;
            CollapseAndExpand();
        }

        public void SetErrorMessage(string dialogTitle, string errMessage, string errMessageDesc)
        {
            this.Text = dialogTitle;
            this.ErrorMessagelabel.Text = errMessage;
            this.ErrorMessageDescTextBox.Text = errMessageDesc;
        }

        private void ExpandPictureBox_Click(object sender, EventArgs e)
        {
            CollapseAndExpand();
        }

        private void CollapseAndExpand()
        {
            if (!IsCollapsed)
            {
                this.Height = 300;
                this.ExpandMessageLabel.Text = "Hide details";
                this.ExpandPictureBox.Image = global::COEServiceLib.Properties.Resources.Resources_15_FilterPanelTableCollapse;
            }
            else
            {
                this.Height = 150;
                this.ExpandMessageLabel.Text = "Show details";
                this.ExpandPictureBox.Image = global::COEServiceLib.Properties.Resources.Resources_15_FilterPanelTableExpand;
            }
            IsCollapsed = !IsCollapsed;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void InitializeComponent()
        {
            this.ErrorMessagelabel = new System.Windows.Forms.Label();
            this.ErrorMessageDescTextBox = new System.Windows.Forms.TextBox();
            this.ExpandMessageLabel = new System.Windows.Forms.Label();
            this.OkButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ExpandPictureBox = new System.Windows.Forms.PictureBox();
            this.CopyLinkLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpandPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ErrorMessagelabel
            // 
            this.ErrorMessagelabel.AutoSize = true;
            this.ErrorMessagelabel.Location = new System.Drawing.Point(61, 20);
            this.ErrorMessagelabel.MaximumSize = new System.Drawing.Size(300, 0);
            this.ErrorMessagelabel.Name = "ErrorMessagelabel";
            this.ErrorMessagelabel.Size = new System.Drawing.Size(130, 14);
            this.ErrorMessagelabel.TabIndex = 7;
            this.ErrorMessagelabel.Text = "Error Message Text Here.";
            // 
            // ErrorMessageDescTextBox
            // 
            this.ErrorMessageDescTextBox.Location = new System.Drawing.Point(21, 136);
            this.ErrorMessageDescTextBox.Multiline = true;
            this.ErrorMessageDescTextBox.Name = "ErrorMessageDescTextBox";
            this.ErrorMessageDescTextBox.ReadOnly = true;
            this.ErrorMessageDescTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ErrorMessageDescTextBox.Size = new System.Drawing.Size(350, 120);
            this.ErrorMessageDescTextBox.TabIndex = 11;
            // 
            // ExpandMessageLabel
            // 
            this.ExpandMessageLabel.AutoSize = true;
            this.ExpandMessageLabel.Location = new System.Drawing.Point(44, 101);
            this.ExpandMessageLabel.Name = "ExpandMessageLabel";
            this.ExpandMessageLabel.Size = new System.Drawing.Size(71, 14);
            this.ExpandMessageLabel.TabIndex = 10;
            this.ExpandMessageLabel.Text = "Show Details";
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(296, 96);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 8;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::COEServiceLib.Properties.Resources.Resources_32_Error;
            this.pictureBox1.Location = new System.Drawing.Point(21, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // ExpandPictureBox
            // 
            this.ExpandPictureBox.Image = global::COEServiceLib.Properties.Resources.Resources_15_FilterPanelTableExpand;
            this.ExpandPictureBox.Location = new System.Drawing.Point(21, 100);
            this.ExpandPictureBox.Name = "ExpandPictureBox";
            this.ExpandPictureBox.Size = new System.Drawing.Size(15, 15);
            this.ExpandPictureBox.TabIndex = 9;
            this.ExpandPictureBox.TabStop = false;
            this.ExpandPictureBox.Click += new System.EventHandler(this.ExpandPictureBox_Click);
            // 
            // CopyLinkLabel
            // 
            this.CopyLinkLabel.AutoSize = true;
            this.CopyLinkLabel.Location = new System.Drawing.Point(320, 258);
            this.CopyLinkLabel.Name = "CopyLinkLabel";
            this.CopyLinkLabel.Size = new System.Drawing.Size(53, 14);
            this.CopyLinkLabel.TabIndex = 13;
            this.CopyLinkLabel.TabStop = true;
            this.CopyLinkLabel.Text = "Copy info";
            this.CopyLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CopyLinkLabel_LinkClicked);
            // 
            // ErrorMessageDialogForm
            // 
            this.ClientSize = new System.Drawing.Size(392, 273);
            this.Controls.Add(this.CopyLinkLabel);
            this.Controls.Add(this.ErrorMessagelabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ErrorMessageDescTextBox);
            this.Controls.Add(this.ExpandMessageLabel);
            this.Controls.Add(this.ExpandPictureBox);
            this.Controls.Add(this.OkButton);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageDialogForm";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.ErrorMessageDialogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpandPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CopyLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(this.ErrorMessageDescTextBox.Text);
        }
    }
}
