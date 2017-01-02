/*  GREATIS FORM DESIGNER FOR .NET
 *  ToolBox Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Drawing.Design;
using System.ComponentModel.Design;

namespace Greatis.FormDesigner
{
   /// <summary>
   /// Summary description for Toolbox.
   /// </summary>
   public class Toolbox : System.Windows.Forms.Form
   {
      private Greatis.FormDesigner.ToolboxControl toolboxControl1;

      public Toolbox()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         //
         // TODO: Add any constructor code after InitializeComponent call
         //
      }

      /// <summary>
      /// Gets Toolbox control
      /// </summary>
      public ToolboxControl ToolboxCtrl
      {
         get { return toolboxControl1; }
      }
      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         base.Dispose( disposing );
      }

      protected override void OnClosed(EventArgs e)
      {
         toolboxControl1.Designer = null;
         //designer.DesignerHost.RemoveService(typeof(IToolboxService));
      }

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.toolboxControl1 = new Greatis.FormDesigner.ToolboxControl();
         this.SuspendLayout();
         // 
         // toolboxControl1
         // 
         this.toolboxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.toolboxControl1.Font = new System.Drawing.Font("Tahoma", 8F);
         this.toolboxControl1.Location = new System.Drawing.Point(0, 0);
         this.toolboxControl1.Name = "toolboxControl1";
         this.toolboxControl1.SelectedCategory = null;
         this.toolboxControl1.Size = new System.Drawing.Size(80, 236);
         this.toolboxControl1.TabIndex = 0;
         // 
         // Toolbox
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(80, 236);
         this.Controls.Add(this.toolboxControl1);
         this.Font = new System.Drawing.Font("Tahoma", 8F);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
         this.Name = "Toolbox";
         this.ShowInTaskbar = false;
         this.Text = "Toolbox";
         this.ResumeLayout(false);

      }
      #endregion
   }
}
