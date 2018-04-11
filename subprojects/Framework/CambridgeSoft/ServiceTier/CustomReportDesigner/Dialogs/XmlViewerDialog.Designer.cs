namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    partial class XmlViewerDialog
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
            this.XmlViewerWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // XmlViewerWebBrowser
            // 
            this.XmlViewerWebBrowser.AllowNavigation = false;
            this.XmlViewerWebBrowser.AllowWebBrowserDrop = false;
            this.XmlViewerWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XmlViewerWebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.XmlViewerWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.XmlViewerWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.XmlViewerWebBrowser.Name = "XmlViewerWebBrowser";
            this.XmlViewerWebBrowser.Size = new System.Drawing.Size(698, 533);
            this.XmlViewerWebBrowser.TabIndex = 0;
            this.XmlViewerWebBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // XmlViewerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(698, 533);
            this.Controls.Add(this.XmlViewerWebBrowser);
            this.MinimizeBox = false;
            this.Name = "XmlViewerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "XmlViewerDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser XmlViewerWebBrowser;
    }
}