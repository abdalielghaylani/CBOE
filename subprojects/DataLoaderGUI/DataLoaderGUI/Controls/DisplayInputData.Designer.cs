namespace CambridgeSoft.DataLoaderGUI.Controls
{
    partial class DisplayInputData
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
            this.components = new System.ComponentModel.Container();
            this._BatchMarkbutton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this._Inversebutton = new System.Windows.Forms.Button();
            this._ExportButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblWait = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _BatchMarkbutton
            // 
            this._BatchMarkbutton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._BatchMarkbutton.Location = new System.Drawing.Point(15, 435);
            this._BatchMarkbutton.Name = "_BatchMarkbutton";
            this._BatchMarkbutton.Size = new System.Drawing.Size(75, 29);
            this._BatchMarkbutton.TabIndex = 17;
            this._BatchMarkbutton.Text = "Batch Mark";
            this._BatchMarkbutton.UseVisualStyleBackColor = false;
            this._BatchMarkbutton.Click += new System.EventHandler(this._BatchMarkbutton_Click);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SelectAllButton.Location = new System.Drawing.Point(98, 435);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(75, 29);
            this.SelectAllButton.TabIndex = 17;
            this.SelectAllButton.Text = "Select All";
            this.SelectAllButton.UseVisualStyleBackColor = false;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // _Inversebutton
            // 
            this._Inversebutton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._Inversebutton.Location = new System.Drawing.Point(181, 435);
            this._Inversebutton.Name = "_Inversebutton";
            this._Inversebutton.Size = new System.Drawing.Size(75, 29);
            this._Inversebutton.TabIndex = 17;
            this._Inversebutton.Text = "Inverse";
            this._Inversebutton.UseVisualStyleBackColor = false;
            this._Inversebutton.Click += new System.EventHandler(this._Inversebutton_Click);
            // 
            // _ExportButton
            // 
            this._ExportButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this._ExportButton.Location = new System.Drawing.Point(264, 435);
            this._ExportButton.Name = "_ExportButton";
            this._ExportButton.Size = new System.Drawing.Size(75, 29);
            this._ExportButton.TabIndex = 17;
            this._ExportButton.Text = "Export to";
            this._ExportButton.UseVisualStyleBackColor = false;
            this._ExportButton.Click += new System.EventHandler(this._ExportButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblWait);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(900, 425);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View";
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.Location = new System.Drawing.Point(355, 206);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(190, 13);
            this.lblWait.TabIndex = 0;
            this.lblWait.Text = "Please wait while loading the records...";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DisplayInputData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._ExportButton);
            this.Controls.Add(this._Inversebutton);
            this.Controls.Add(this.SelectAllButton);
            this.Controls.Add(this._BatchMarkbutton);
            this.Name = "DisplayInputData";
            this.Size = new System.Drawing.Size(900, 475);
            this.Load += new System.EventHandler(this.DisplayInputData_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _BatchMarkbutton;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button _Inversebutton;
        private System.Windows.Forms.Button _ExportButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblWait;
    }
}
