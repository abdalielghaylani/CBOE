namespace ChemControls
{
    partial class CustomizeGridDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizeGridDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CardView = new System.Windows.Forms.RadioButton();
            this.StandardView = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cellsOptimized = new System.Windows.Forms.RadioButton();
            this.cellsHorizontal = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelStyleWithCells = new System.Windows.Forms.RadioButton();
            this.labelStyleSeparate = new System.Windows.Forms.RadioButton();
            this.FilteringBox = new System.Windows.Forms.GroupBox();
            this.FilteringDisabled = new System.Windows.Forms.RadioButton();
            this.FilteringEnabled = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.childTableNamesCombo = new System.Windows.Forms.ComboBox();
            this.oneChildTableHorizontal = new System.Windows.Forms.RadioButton();
            this.allChildTablesVertical = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.FilteringBox.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CardView);
            this.groupBox1.Controls.Add(this.StandardView);
            this.groupBox1.Location = new System.Drawing.Point(12, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(139, 80);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View style";
            // 
            // CardView
            // 
            this.CardView.AutoSize = true;
            this.CardView.Location = new System.Drawing.Point(7, 44);
            this.CardView.Name = "CardView";
            this.CardView.Size = new System.Drawing.Size(73, 17);
            this.CardView.TabIndex = 1;
            this.CardView.TabStop = true;
            this.CardView.Text = "Card View";
            this.CardView.UseVisualStyleBackColor = true;
            this.CardView.CheckedChanged += new System.EventHandler(this.CardView_CheckedChanged);
            // 
            // StandardView
            // 
            this.StandardView.AutoSize = true;
            this.StandardView.Location = new System.Drawing.Point(7, 20);
            this.StandardView.Name = "StandardView";
            this.StandardView.Size = new System.Drawing.Size(94, 17);
            this.StandardView.TabIndex = 0;
            this.StandardView.TabStop = true;
            this.StandardView.Text = "Standard View";
            this.StandardView.UseVisualStyleBackColor = true;
            this.StandardView.CheckedChanged += new System.EventHandler(this.StandardView_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cellsOptimized);
            this.groupBox2.Controls.Add(this.cellsHorizontal);
            this.groupBox2.Location = new System.Drawing.Point(12, 109);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(139, 75);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cell arrangement in rows";
            // 
            // cellsOptimized
            // 
            this.cellsOptimized.AutoSize = true;
            this.cellsOptimized.Location = new System.Drawing.Point(7, 44);
            this.cellsOptimized.Name = "cellsOptimized";
            this.cellsOptimized.Size = new System.Drawing.Size(71, 17);
            this.cellsOptimized.TabIndex = 1;
            this.cellsOptimized.TabStop = true;
            this.cellsOptimized.Text = "Optimized";
            this.cellsOptimized.UseVisualStyleBackColor = true;
            this.cellsOptimized.CheckedChanged += new System.EventHandler(this.cellsOptimized_CheckedChanged);
            // 
            // cellsHorizontal
            // 
            this.cellsHorizontal.AutoSize = true;
            this.cellsHorizontal.Location = new System.Drawing.Point(7, 20);
            this.cellsHorizontal.Name = "cellsHorizontal";
            this.cellsHorizontal.Size = new System.Drawing.Size(70, 17);
            this.cellsHorizontal.TabIndex = 0;
            this.cellsHorizontal.TabStop = true;
            this.cellsHorizontal.Text = "horizontal";
            this.cellsHorizontal.UseVisualStyleBackColor = true;
            this.cellsHorizontal.CheckedChanged += new System.EventHandler(this.cellsHorizontal_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelStyleWithCells);
            this.groupBox3.Controls.Add(this.labelStyleSeparate);
            this.groupBox3.Location = new System.Drawing.Point(174, 109);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(96, 75);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Label style";
            // 
            // labelStyleWithCells
            // 
            this.labelStyleWithCells.AutoSize = true;
            this.labelStyleWithCells.Location = new System.Drawing.Point(7, 44);
            this.labelStyleWithCells.Name = "labelStyleWithCells";
            this.labelStyleWithCells.Size = new System.Drawing.Size(71, 17);
            this.labelStyleWithCells.TabIndex = 1;
            this.labelStyleWithCells.TabStop = true;
            this.labelStyleWithCells.Text = "With cells";
            this.labelStyleWithCells.UseVisualStyleBackColor = true;
            this.labelStyleWithCells.CheckedChanged += new System.EventHandler(this.labelStyleWidthCells_CheckedChanged);
            // 
            // labelStyleSeparate
            // 
            this.labelStyleSeparate.AutoSize = true;
            this.labelStyleSeparate.Location = new System.Drawing.Point(7, 20);
            this.labelStyleSeparate.Name = "labelStyleSeparate";
            this.labelStyleSeparate.Size = new System.Drawing.Size(68, 17);
            this.labelStyleSeparate.TabIndex = 0;
            this.labelStyleSeparate.TabStop = true;
            this.labelStyleSeparate.Text = "Separate";
            this.labelStyleSeparate.UseVisualStyleBackColor = true;
            this.labelStyleSeparate.CheckedChanged += new System.EventHandler(this.labelStyleSeparate_CheckedChanged);
            // 
            // FilteringBox
            // 
            this.FilteringBox.Controls.Add(this.FilteringDisabled);
            this.FilteringBox.Controls.Add(this.FilteringEnabled);
            this.FilteringBox.Location = new System.Drawing.Point(12, 191);
            this.FilteringBox.Name = "FilteringBox";
            this.FilteringBox.Size = new System.Drawing.Size(139, 77);
            this.FilteringBox.TabIndex = 3;
            this.FilteringBox.TabStop = false;
            this.FilteringBox.Text = "Filtering";
            // 
            // FilteringDisabled
            // 
            this.FilteringDisabled.AutoSize = true;
            this.FilteringDisabled.Location = new System.Drawing.Point(7, 44);
            this.FilteringDisabled.Name = "FilteringDisabled";
            this.FilteringDisabled.Size = new System.Drawing.Size(66, 17);
            this.FilteringDisabled.TabIndex = 1;
            this.FilteringDisabled.TabStop = true;
            this.FilteringDisabled.Text = "Disabled";
            this.FilteringDisabled.UseVisualStyleBackColor = true;
            this.FilteringDisabled.CheckedChanged += new System.EventHandler(this.FilteringDisabled_CheckedChanged);
            // 
            // FilteringEnabled
            // 
            this.FilteringEnabled.AutoSize = true;
            this.FilteringEnabled.Location = new System.Drawing.Point(7, 20);
            this.FilteringEnabled.Name = "FilteringEnabled";
            this.FilteringEnabled.Size = new System.Drawing.Size(64, 17);
            this.FilteringEnabled.TabIndex = 0;
            this.FilteringEnabled.TabStop = true;
            this.FilteringEnabled.Text = "Enabled";
            this.FilteringEnabled.UseVisualStyleBackColor = true;
            this.FilteringEnabled.CheckedChanged += new System.EventHandler(this.FilteringEnabled_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.childTableNamesCombo);
            this.groupBox4.Controls.Add(this.oneChildTableHorizontal);
            this.groupBox4.Controls.Add(this.allChildTablesVertical);
            this.groupBox4.Location = new System.Drawing.Point(12, 275);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(258, 84);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Arrangement of child tables";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(160, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "right to base table";
            // 
            // childTableNamesCombo
            // 
            this.childTableNamesCombo.FormattingEnabled = true;
            this.childTableNamesCombo.Location = new System.Drawing.Point(28, 39);
            this.childTableNamesCombo.Name = "childTableNamesCombo";
            this.childTableNamesCombo.Size = new System.Drawing.Size(128, 21);
            this.childTableNamesCombo.TabIndex = 2;
            // 
            // oneChildTableHorizontal
            // 
            this.oneChildTableHorizontal.AutoSize = true;
            this.oneChildTableHorizontal.Location = new System.Drawing.Point(7, 44);
            this.oneChildTableHorizontal.Name = "oneChildTableHorizontal";
            this.oneChildTableHorizontal.Size = new System.Drawing.Size(14, 13);
            this.oneChildTableHorizontal.TabIndex = 1;
            this.oneChildTableHorizontal.TabStop = true;
            this.oneChildTableHorizontal.UseVisualStyleBackColor = true;
            this.oneChildTableHorizontal.CheckedChanged += new System.EventHandler(this.oneChildTableHorizontal_CheckedChanged);
            // 
            // allChildTablesVertical
            // 
            this.allChildTablesVertical.AutoSize = true;
            this.allChildTablesVertical.Location = new System.Drawing.Point(7, 20);
            this.allChildTablesVertical.Name = "allChildTablesVertical";
            this.allChildTablesVertical.Size = new System.Drawing.Size(192, 17);
            this.allChildTablesVertical.TabIndex = 0;
            this.allChildTablesVertical.TabStop = true;
            this.allChildTablesVertical.Text = "All child tables under the base table";
            this.allChildTablesVertical.UseVisualStyleBackColor = true;
            this.allChildTablesVertical.CheckedChanged += new System.EventHandler(this.allChildTablesVertical_CheckedChanged);
            // 
            // CustomizeGridDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 396);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.FilteringBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CustomizeGridDialog";
            this.Text = "CustomizeGridDialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.FilteringBox.ResumeLayout(false);
            this.FilteringBox.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton CardView;
        private System.Windows.Forms.RadioButton StandardView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton cellsOptimized;
        private System.Windows.Forms.RadioButton cellsHorizontal;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton labelStyleWithCells;
        private System.Windows.Forms.RadioButton labelStyleSeparate;
        private System.Windows.Forms.GroupBox FilteringBox;
        private System.Windows.Forms.RadioButton FilteringDisabled;
        private System.Windows.Forms.RadioButton FilteringEnabled;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox childTableNamesCombo;
        private System.Windows.Forms.RadioButton oneChildTableHorizontal;
        private System.Windows.Forms.RadioButton allChildTablesVertical;
    }
}