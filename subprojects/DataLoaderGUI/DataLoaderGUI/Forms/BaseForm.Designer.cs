namespace CambridgeSoft.DataLoaderGUI.Forms
{
    partial class BaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            this._TitleLabel = new System.Windows.Forms.Label();
            this._CommentLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _TitleLabel
            // 
            this._TitleLabel.AutoSize = true;
            this._TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this._TitleLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._TitleLabel.Location = new System.Drawing.Point(10, 10);
            this._TitleLabel.Name = "_TitleLabel";
            this._TitleLabel.Size = new System.Drawing.Size(140, 15);
            this._TitleLabel.TabIndex = 17;
            this._TitleLabel.Text = "Select Imported File ";
            // 
            // _CommentLabel
            // 
            this._CommentLabel.AutoSize = true;
            this._CommentLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._CommentLabel.Location = new System.Drawing.Point(30, 35);
            this._CommentLabel.Name = "_CommentLabel";
            this._CommentLabel.Size = new System.Drawing.Size(213, 13);
            this._CommentLabel.TabIndex = 18;
            this._CommentLabel.Text = "Please select the sample file to be imported.";
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(842, 473);
            this.Controls.Add(this._TitleLabel);
            this.Controls.Add(this._CommentLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DataLoader";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BaseForm_FormClosed);
            this.Load += new System.EventHandler(this.BaseForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _TitleLabel;
        private System.Windows.Forms.Label _CommentLabel;
    }
}

