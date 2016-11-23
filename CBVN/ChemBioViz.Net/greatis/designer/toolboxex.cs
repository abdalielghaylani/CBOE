/*  GREATIS FORM DESIGNER FOR .NET           */
/*  Copyright (C) 2004-2007 Greatis Software */
/*  http://www.greatis.com/dotnet/formdes/   */
/*  http://www.greatis.com/bteam.html        */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Greatis.FormDesigner 
{
   /// <summary>
   /// New window with Toolbox control with category support
   /// </summary>
   public class ToolboxEx : System.Windows.Forms.Form
   {
      private Greatis.FormDesigner.ToolboxControlEx toolboxControl1;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      /// <summary>
      /// Default consructor
      /// </summary>
      public ToolboxEx()
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
      public ToolboxControlEx ToolboxCtrl
      {
         get { return toolboxControl1; }
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if(components != null)
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.toolboxControl1 = new Greatis.FormDesigner.ToolboxControlEx();
         this.SuspendLayout();
         // 
         // toolboxControl1
         // 
         this.toolboxControl1.Designer = null;
         this.toolboxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.toolboxControl1.Location = new System.Drawing.Point(0, 0);
         this.toolboxControl1.Name = "toolboxControl1";
         this.toolboxControl1.SelectedCategory = null;
         this.toolboxControl1.SelectedItem = null;
         this.toolboxControl1.Size = new System.Drawing.Size(224, 270);
         this.toolboxControl1.TabIndex = 0;
         // 
         // ToolboxEx
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(224, 270);
         this.Controls.Add(this.toolboxControl1);
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.ShowInTaskbar = false;
         this.Name = "ToolboxEx";
         this.Text = "Tool Box";
         this.ResumeLayout(false);

      }
      #endregion
   }
}
