namespace ChemControls
{
    partial class ChemDrawForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChemDrawForm));
            this.chemDrawCtl = new DualCdaxControl();
            ((System.ComponentModel.ISupportInitialize)(this.chemDrawCtl)).BeginInit();
            this.SuspendLayout();
            // 
            // chemDrawCtl
            // 
            this.chemDrawCtl.Control.Enabled = true;
            this.chemDrawCtl.Control.Location = new System.Drawing.Point(301, 257);
            this.chemDrawCtl.Control.Name = "chemDrawCtl";
            this.chemDrawCtl.SetOcxState(resources);
            this.chemDrawCtl.Control.Size = new System.Drawing.Size(192, 192);
            this.chemDrawCtl.Control.TabIndex = 0;
            this.chemDrawCtl.Control.Visible = false;
            // 
            // ChemDrawForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 313);
            this.Controls.Add(this.chemDrawCtl.Control);
            this.Name = "ChemDrawForm";
            this.Text = "ChemDrawForm";
            ((System.ComponentModel.ISupportInitialize)(this.chemDrawCtl)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private DualCdaxControl chemDrawCtl;
    }
}