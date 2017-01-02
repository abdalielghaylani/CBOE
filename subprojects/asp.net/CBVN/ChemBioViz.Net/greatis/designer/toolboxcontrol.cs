/*  GREATIS FORM DESIGNER FOR .NET
 *  Toolbox Control Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.Drawing.Design;
using System.ComponentModel.Design;

using Greatis.FormDesigner;

namespace Greatis.FormDesigner
{
   /// <summary>
   /// class implemented IToolboxView interface
   /// </summary>
   public class ToolboxControl : System.Windows.Forms.UserControl, IToolboxView
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.Windows.Forms.ToolBar toolBar;
      private System.ComponentModel.IContainer components;
      private System.Windows.Forms.ImageList imageList;
      private System.Windows.Forms.ToolBarButton selectButton;

      private int selectedButton;

      private ToolboxService toolboxService;


      /// <summary>
      /// Default constructor
      /// </summary>
      public ToolboxControl()
      {
         // This call is required by the Windows.Forms Form Designer.
         InitializeComponent();

         toolBar.Buttons[0].Pushed = true;
         selectedButton = 0;
         toolBar.DoubleClick += new EventHandler(toolBar_DoubleClick);
         toolBar.MouseDown += new MouseEventHandler(toolBar_MouseDown);
         toolBar.MouseMove += new MouseEventHandler(toolBar_MouseMove);

         toolboxService = new ToolboxService(this);
      }

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            toolboxService.Dispose();

            toolBar.DoubleClick -= new EventHandler(toolBar_DoubleClick);
            toolBar.MouseDown -= new MouseEventHandler(toolBar_MouseDown);
            toolBar.MouseMove -= new MouseEventHandler(toolBar_MouseMove);
 

            if(components != null)
               components.Dispose();
         }
         base.Dispose( disposing );
      }

      #region PrevVersion_Compatibility
      /// <summary>
      /// Gets default Designer, add new designer into list of the associated deisgners
      /// </summary>
      public Greatis.FormDesigner.Designer Designer
      {
         set 
         {
            toolboxService.Designer = value;
         }
         get 
         {
            return toolboxService.Designer;
         }
      }

      public void AddToolboxItem(ToolboxItem toolboxItem, string category)
      {
         toolboxService.AddToolboxItem(toolboxItem, category);
      }

      public void AddToolboxItem(ToolboxItem toolboxItem)
      {
         toolboxService.AddToolboxItem(toolboxItem);
      }
      #endregion

#if V1      
      #region Component Designer generated code
      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ToolboxControl));
         this.toolBar = new System.Windows.Forms.ToolBar();
         this.selectButton = new System.Windows.Forms.ToolBarButton();
         this.imageList = new System.Windows.Forms.ImageList(this.components);
         this.SuspendLayout();
         // 
         // toolBar
         // 
         this.toolBar.AllowDrop = true;
         this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
         this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.selectButton});
         this.toolBar.ButtonSize = new System.Drawing.Size(60, 16);
         this.toolBar.Dock = System.Windows.Forms.DockStyle.Fill;
         this.toolBar.DropDownArrows = true;
         this.toolBar.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
         this.toolBar.ImageList = this.imageList;
         this.toolBar.Location = new System.Drawing.Point(0, 0);
         this.toolBar.Name = "toolBar";
         this.toolBar.ShowToolTips = true;
         this.toolBar.Size = new System.Drawing.Size(80, 28);
         this.toolBar.TabIndex = 0;
         this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
         this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
         // 
         // selectButton
         // 
         this.selectButton.ImageIndex = 0;
         this.selectButton.Pushed = true;
         this.selectButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.selectButton.Text = "Select";
         // 
         // imageList
         // 
         this.imageList.ImageSize = new System.Drawing.Size(16, 16);
         this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
         this.imageList.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // ToolboxControl
         // 
         this.Controls.Add(this.toolBar);
         this.Font = new System.Drawing.Font("Tahoma", 8F);
         this.Name = "ToolboxControl";
         this.Size = new System.Drawing.Size(80, 236);
         this.ResumeLayout(false);
      }
      #endregion
#else
      #region Component Designer generated code
      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolboxControl));
         this.toolBar = new System.Windows.Forms.ToolBar();
         this.selectButton = new System.Windows.Forms.ToolBarButton();
         this.imageList = new System.Windows.Forms.ImageList(this.components);
         this.SuspendLayout();
         // 
         // toolBar
         // 
         this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
         this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                   this.selectButton});
         this.toolBar.ButtonSize = new System.Drawing.Size(60, 16);
         this.toolBar.CausesValidation = false;
         this.toolBar.Dock = System.Windows.Forms.DockStyle.Fill;
         this.toolBar.DropDownArrows = true;
         this.toolBar.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.toolBar.ImageList = this.imageList;
         this.toolBar.Location = new System.Drawing.Point(0, 0);
         this.toolBar.Name = "toolBar";
         this.toolBar.ShowToolTips = true;
         this.toolBar.Size = new System.Drawing.Size(80, 28);
         this.toolBar.TabIndex = 0;
         this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
         this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
         // 
         // selectButton
         // 
         this.selectButton.ImageIndex = 0;
         this.selectButton.Name = "selectButton";
         this.selectButton.Pushed = true;
         this.selectButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
         this.selectButton.Text = "Select";
         // 
         // imageList
         // 
         this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
         this.imageList.TransparentColor = System.Drawing.Color.Magenta;
         this.imageList.Images.SetKeyName(0, "");
         // 
         // ToolboxControl
         // 
         this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
         this.Controls.Add(this.toolBar);
         this.Font = new System.Drawing.Font("Tahoma", 8F);
         this.Name = "ToolboxControl";
         this.Size = new System.Drawing.Size(80, 236);
         this.ResumeLayout(false);
         this.PerformLayout();

      }
      #endregion
#endif

      #region IToolboxView Members
      public void RemoveItem(ToolboxItem toolboxItem, string category)
      {
         foreach(ToolBarButton b in toolBar.Buttons)
         {
            if( b.Tag is ToolboxItem && b.Tag == toolboxItem )
            {
               b.Tag = null;
               toolBar.Buttons.Remove(b);

               break;
            }
         }
      }

      public void AddItem(ToolboxItem toolboxItem, string category)
      {
         int index = imageList.Images.Add(toolboxItem.Bitmap, toolboxItem.Bitmap.GetPixel(0,0));
         ToolBarButton button = new ToolBarButton(toolboxItem.DisplayName);
         
         toolBar.Buttons.Add(button);
         
         button.Style = ToolBarButtonStyle.ToggleButton;
         button.ImageIndex = index;
         button.Tag = toolboxItem;

         int diff = Width - ClientRectangle.Width;
         this.toolBar.ButtonSize = new Size(toolBar.Buttons[0].Rectangle.Right, this.toolBar.ButtonSize.Height);
         this.Width = this.toolBar.ButtonSize.Width + diff;
      }

      [Browsable(false)]
      public Cursor CurrentCursor
      {
         get { return (selectedButton == 0) ? Cursors.Arrow : Cursors.Cross; }
      }


      [Browsable(false)]
      public ToolboxCategoryCollection Items
      {
         get
         {
            ToolboxItem[] items = new ToolboxItem[toolBar.Buttons.Count-1];
            for(int i=1; i<toolBar.Buttons.Count; i++)
               items[i-1] = (ToolboxItem)toolBar.Buttons[i].Tag;
         
            ToolboxCategoryItem categoryItem = new ToolboxCategoryItem(null, new ToolboxItemCollection(items));

            return new ToolboxCategoryCollection(new ToolboxCategoryItem[1] { categoryItem } );
         }
      }

      [Browsable(false)]
      public ToolboxItem SelectedItem
      {
         get
         {
            if( selectedButton == 0 )
               return null;

            return (toolBar.Buttons[selectedButton].Tag is ToolboxItem) ? 
               (ToolboxItem)toolBar.Buttons[selectedButton].Tag : null;
         }

         set
         {
            if( value == null )
            {
               if( selectedButton != 0 )
                  toolBar.Buttons[selectedButton].Pushed = false;
         
               selectedButton = 0;
               toolBar.Buttons[selectedButton].Pushed = true;
            } else
            {
               for( int i=1; i<toolBar.Buttons.Count; i++ )
               {
                  ToolBarButton b = toolBar.Buttons[i];
                  if( b.Tag == value )
                  {
                     if( selectedButton != i )
                     {
                        toolBar.Buttons[selectedButton].Pushed = false;
                        selectedButton = i;
                        toolBar.Buttons[selectedButton].Pushed = true;
                     }
                     break;
                  }
               }
            }
         }
      }

      [Browsable(false)]
      public string SelectedCategory
      {
         get { return null; }
         set { }
      }

      public event EventHandler BeginDragAndDrop;
      public event EventHandler DropControl;
      #endregion

      private void toolBar_DoubleClick(object sender, EventArgs e)
      {
         if( DropControl != null )
            DropControl(this, null);
      }


      void toolBar_MouseMove(object sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
         {
            if( BeginDragAndDrop != null )
               BeginDragAndDrop(this, null);
         }
      }

      private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
      {
         int index = toolBar.Buttons.IndexOf(e.Button);
         if (selectedButton != index)
            toolBar.Buttons[selectedButton].Pushed = false;

         e.Button.Pushed = true;
         selectedButton = index;
      }

      private void toolBar_MouseDown(object sender, MouseEventArgs e)
      {          
         Point pt = toolBar.PointToClient(this.PointToScreen(new Point(e.X, e.Y)));
         foreach( ToolBarButton b in toolBar.Buttons )
         {
            if( b.Rectangle.Contains(pt) )
            {
               toolBar_ButtonClick(this, new ToolBarButtonClickEventArgs(b));
               break;
            }
         }
      }
   }
}
