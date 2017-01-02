namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    partial class ResultsCriteriaEditor
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
            this.AvailableCriteriumsTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SelectedCriteriumPropertiesEditor = new CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs.PropertiesEditor();
            this.ExitButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AvailableCriteriumsTreeView
            // 
            this.AvailableCriteriumsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AvailableCriteriumsTreeView.Location = new System.Drawing.Point(0, 0);
            this.AvailableCriteriumsTreeView.Name = "AvailableCriteriumsTreeView";
            this.AvailableCriteriumsTreeView.Size = new System.Drawing.Size(445, 595);
            this.AvailableCriteriumsTreeView.TabIndex = 0;
            this.AvailableCriteriumsTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AvailableCriteriumsTreeView_NodeMouseClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.AvailableCriteriumsTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.SelectedCriteriumPropertiesEditor);
            this.splitContainer1.Size = new System.Drawing.Size(897, 595);
            this.splitContainer1.SplitterDistance = 445;
            this.splitContainer1.TabIndex = 2;
            // 
            // SelectedCriteriumPropertiesEditor
            // 
            this.SelectedCriteriumPropertiesEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectedCriteriumPropertiesEditor.Criterium = null;
            this.SelectedCriteriumPropertiesEditor.Location = new System.Drawing.Point(0, 0);
            this.SelectedCriteriumPropertiesEditor.Name = "SelectedCriteriumPropertiesEditor";
            this.SelectedCriteriumPropertiesEditor.Size = new System.Drawing.Size(448, 595);
            this.SelectedCriteriumPropertiesEditor.TabIndex = 1;
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.Location = new System.Drawing.Point(810, 601);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 3;
            this.ExitButton.Text = "Cancel";
            this.ExitButton.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.Location = new System.Drawing.Point(729, 601);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 4;
            this.OkButton.Text = "Accept";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ResultsCriteriaEditor
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ExitButton;
            this.ClientSize = new System.Drawing.Size(897, 636);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ResultsCriteriaEditor";
            this.ShowInTaskbar = false;
            this.Text = "ResultsCriteriaEditor";
            this.Load += new System.EventHandler(this.ResultsCriteriaEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView AvailableCriteriumsTreeView;
        private PropertiesEditor SelectedCriteriumPropertiesEditor;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button OkButton;
    }
}