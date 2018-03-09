namespace ChemControls
{
    partial class ChemDraw
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChemDraw));
            this.axChemDrawCtl1 = new DualCdaxControl();
            ((System.ComponentModel.ISupportInitialize)(this.axChemDrawCtl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axChemDrawCtl1
            // 
            this.axChemDrawCtl1.Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axChemDrawCtl1.Control.Enabled = true;
            this.axChemDrawCtl1.Control.Location = new System.Drawing.Point(0, 0);
            this.axChemDrawCtl1.Control.Name = "axChemDrawCtl1";
            this.axChemDrawCtl1.SetOcxState(resources);
            this.axChemDrawCtl1.Control.Size = new System.Drawing.Size(208, 220);
            this.axChemDrawCtl1.Control.TabIndex = 0;
            // 
            // ChemDraw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axChemDrawCtl1.Control);
            this.Name = "ChemDraw";
            this.Size = new System.Drawing.Size(208, 220);
            ((System.ComponentModel.ISupportInitialize)(this.axChemDrawCtl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DualCdaxControl axChemDrawCtl1;


    }
}
