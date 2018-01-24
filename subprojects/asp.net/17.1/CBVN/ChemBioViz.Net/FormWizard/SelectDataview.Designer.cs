namespace FormWizard
{
    partial class SelectDataview
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
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDataview));
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.availableFieldsTreeView = new Infragistics.Win.UltraWinTree.UltraTree();
            this.label2 = new System.Windows.Forms.Label();
            this.fieldContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDefaultFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAllFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addUniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.availableFieldsImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.availableFieldsTreeView)).BeginInit();
            this.fieldContextMenuStrip.SuspendLayout();
            this.tableContextMenuStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.searchTextBox.Location = new System.Drawing.Point(6, 6);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(198, 20);
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // availableFieldsTreeView
            // 
            this.availableFieldsTreeView.AllowDrop = true;
            this.availableFieldsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableFieldsTreeView.Location = new System.Drawing.Point(6, 46);
            this.availableFieldsTreeView.Name = "availableFieldsTreeView";
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.ExtendedAutoDrag;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.None;
            this.availableFieldsTreeView.Override = _override1;
            this.availableFieldsTreeView.Size = new System.Drawing.Size(198, 497);
            this.availableFieldsTreeView.TabIndex = 1;
            this.availableFieldsTreeView.SelectionDragStart += new System.EventHandler(this.availableFieldsTreeView_SelectionDragStart);
            this.availableFieldsTreeView.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(this.availableFieldsTreeView_MouseEnterElement);
            this.availableFieldsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.availableFieldsTreeView_DragDrop);
            this.availableFieldsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.availableFieldsTreeView_DragOver);
            this.availableFieldsTreeView.DoubleClick += new System.EventHandler(this.availableFieldsTreeView_DoubleClick);
            this.availableFieldsTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.availableFieldsTreeView_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Available Tables and Fields";
            // 
            // fieldContextMenuStrip
            // 
            this.fieldContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.fieldContextMenuStrip.Name = "fieldContextMenuStrip";
            this.fieldContextMenuStrip.Size = new System.Drawing.Size(94, 26);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // tableContextMenuStrip
            // 
            this.tableContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDefaultFieldsToolStripMenuItem,
            this.addAllFieldsToolStripMenuItem,
            this.addUniToolStripMenuItem});
            this.tableContextMenuStrip.Name = "tableContextMenuStrip";
            this.tableContextMenuStrip.Size = new System.Drawing.Size(220, 92);
            // 
            // addDefaultFieldsToolStripMenuItem
            // 
            this.addDefaultFieldsToolStripMenuItem.Name = "addDefaultFieldsToolStripMenuItem";
            this.addDefaultFieldsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.addDefaultFieldsToolStripMenuItem.Text = "Add Default Fields";
            this.addDefaultFieldsToolStripMenuItem.Click += new System.EventHandler(this.addDefaultFieldsToolStripMenuItem_Click);
            // 
            // addAllFieldsToolStripMenuItem
            // 
            this.addAllFieldsToolStripMenuItem.Name = "addAllFieldsToolStripMenuItem";
            this.addAllFieldsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.addAllFieldsToolStripMenuItem.Text = "Add All Fields";
            this.addAllFieldsToolStripMenuItem.Click += new System.EventHandler(this.addAllFieldsToolStripMenuItem_Click);
            // 
            // addUniToolStripMenuItem
            // 
            this.addUniToolStripMenuItem.Name = "addUniToolStripMenuItem";
            this.addUniToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.addUniToolStripMenuItem.Text = "Add Unique Key and Structure";
            this.addUniToolStripMenuItem.Click += new System.EventHandler(this.addUniToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.searchTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.availableFieldsTreeView, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(210, 549);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // availableFieldsImageList
            // 
            this.availableFieldsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("availableFieldsImageList.ImageStream")));
            this.availableFieldsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.availableFieldsImageList.Images.SetKeyName(0, "clear.png");
            this.availableFieldsImageList.Images.SetKeyName(1, "StrViewer_button.png");
            this.availableFieldsImageList.Images.SetKeyName(2, "Resources_16_ColumnDateTime.png");
            this.availableFieldsImageList.Images.SetKeyName(3, "Resources_16_ColumnBoolean.png");
            this.availableFieldsImageList.Images.SetKeyName(4, "Resources_16_ColumnInteger.png");
            this.availableFieldsImageList.Images.SetKeyName(5, "Resources_16_ColumnReal.png");
            this.availableFieldsImageList.Images.SetKeyName(6, "Resources_16_ColumnString.png");
            // 
            // SelectDataview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SelectDataview";
            this.Size = new System.Drawing.Size(210, 549);
            ((System.ComponentModel.ISupportInitialize)(this.availableFieldsTreeView)).EndInit();
            this.fieldContextMenuStrip.ResumeLayout(false);
            this.tableContextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox searchTextBox;
        private Infragistics.Win.UltraWinTree.UltraTree availableFieldsTreeView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDefaultFieldsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAllFieldsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addUniToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip fieldContextMenuStrip;
        internal System.Windows.Forms.ContextMenuStrip tableContextMenuStrip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.ImageList availableFieldsImageList;
    }
}
