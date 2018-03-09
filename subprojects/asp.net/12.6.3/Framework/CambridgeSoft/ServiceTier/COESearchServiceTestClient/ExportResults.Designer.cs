namespace COESearchServiceTest
{
    partial class ExportResults
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
            this.ExportTextResults = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ExportTextResults
            // 
            this.ExportTextResults.Location = new System.Drawing.Point(12, 12);
            this.ExportTextResults.Multiline = true;
            this.ExportTextResults.Name = "ExportTextResults";
            this.ExportTextResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ExportTextResults.Size = new System.Drawing.Size(671, 506);
            this.ExportTextResults.TabIndex = 0;
            // 
            // ExportResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 549);
            this.Controls.Add(this.ExportTextResults);
            this.Name = "ExportResults";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ExportResults_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExportTextResults;
    }
}