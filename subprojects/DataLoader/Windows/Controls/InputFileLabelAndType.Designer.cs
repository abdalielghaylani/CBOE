namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    partial class InputFileLabelAndType
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlOkCancel = new System.Windows.Forms.Panel();
            this.pnlNavigation = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHeader.AutoScroll = true;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(327, 32);
            this.pnlHeader.TabIndex = 0;
            // 
            // pnlMain
            // 
            this.pnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMain.AutoScroll = true;
            this.pnlMain.Location = new System.Drawing.Point(3, 41);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(327, 244);
            this.pnlMain.TabIndex = 1;
            // 
            // pnlOkCancel
            // 
            this.pnlOkCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOkCancel.Location = new System.Drawing.Point(154, 329);
            this.pnlOkCancel.Name = "pnlOkCancel";
            this.pnlOkCancel.Size = new System.Drawing.Size(176, 32);
            this.pnlOkCancel.TabIndex = 2;
            // 
            // pnlNavigation
            // 
            this.pnlNavigation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNavigation.Location = new System.Drawing.Point(3, 291);
            this.pnlNavigation.Name = "pnlNavigation";
            this.pnlNavigation.Size = new System.Drawing.Size(302, 32);
            this.pnlNavigation.TabIndex = 3;
            // 
            // InputFileLabelAndType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlNavigation);
            this.Controls.Add(this.pnlOkCancel);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Name = "InputFileLabelAndType";
            this.Size = new System.Drawing.Size(333, 363);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlOkCancel;
        private System.Windows.Forms.Panel pnlNavigation;
    }
}
