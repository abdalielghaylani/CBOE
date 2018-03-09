/*  GREATIS FORM DESIGNER FOR .NET           */
/*  Copyright (C) 2004-2007 Greatis Software */
/*  http://www.greatis.com/dotnet/formdes/   */
/*  http://www.greatis.com/bteam.html        */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Greatis.FormDesigner
{
   #region ToolboxControl class
   /// <summary>
   /// New Toolbox control with category support
   /// </summary>
   public class ToolboxControlEx : System.Windows.Forms.UserControl, Greatis.FormDesigner.IToolboxView
   {
      private readonly string DefaultCategory = "General";
      private System.Windows.Forms.ImageList images;
      private System.ComponentModel.IContainer components;
      private Greatis.FormDesigner.ToolboxList list;

      private ToolboxService toolboxService;

      private Hashtable toolboxItems; // Key - categoryName, Value - ArrayList<ToolboxItemWithImage>

      internal enum PictureIndex
      {
         piPlus = 0,
         piMinus,
         piArrow
      }

      internal enum CategoryState
      {
         Expanded,
         Collapsed
      }

      /// <summary>
      /// Default constructor
      /// </summary>
      public ToolboxControlEx()
      {
         InitializeComponent();

         toolboxItems = new Hashtable();

         list.Click += new EventHandler(OnItemClick);
         list.DoubleClick += new EventHandler(OnItemDoubleClick);
         list.ItemDrag += new ItemDragEventHandler(OnItemDrag);

         AddCategory(DefaultCategory);

         toolboxService = new ToolboxService(this);
      }

      /// <summary>
      /// Gets or sets backgound color of the toolbox control
      /// </summary>
      public override Color BackColor
      {
         get { return list.BackColor; }
         set { list.BackColor = value; }
      }

      /// <summary>
      /// Gets or sets backgound color of the item under mouse cursor
      /// </summary>
      public Color ItemUnderMouseColor
      {
         get { return list.ItemUnderMouseColor; }
         set { list.ItemUnderMouseColor = value; }
      }

      /// <summary>
      /// Gets or sets backgound color of the selected item under mouse cursor
      /// </summary>
      public Color SelectedItemUnderMouseColor
      {
         get { return list.SelectedItemUnderMouseColor; }
         set { list.SelectedItemUnderMouseColor = value; }
      }

      /// <summary>
      /// Gets or sets backgound color of the selected item
      /// </summary>
      public Color SelectedItemColor
      {
         get { return list.SelectedItemColor; }
         set { list.SelectedItemColor = value; }
      }

      /// <summary>
      /// Gets or sets frame color of the item under mouse cursor
      /// </summary>
      public Color FrameColor
      {
         get { return list.FrameColor; }
         set { list.FrameColor = value; }
      }

      /// <summary>
      /// Gets or sets backgound color of the group item
      /// </summary>
      public Color GroupColor
      {
         get { return list.GroupColor; }
         set { list.GroupColor = value; }
      }

      /// <summary>
      /// Gets or sets designer form toolbox control
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

      /// <summary>
      /// Adds item into toolbos control
      /// </summary>
      /// <param name="toolboxItem">toolbox item</param>
      /// <param name="category">category for item</param>
      public void AddToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem, string category)
      {
         toolboxService.AddToolboxItem(toolboxItem, category);
      }

      /// <summary>
      /// Adds toolbox item into General category
      /// </summary>
      /// <param name="toolboxItem">toolbox item</param>
      public void AddToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem)
      {
         toolboxService.AddToolboxItem(toolboxItem);
      } 

      private void AddCategory(string name)
      {
         ToolboxItemEx group = new CategoryItem(name, true);
         ToolboxItemEx pointer = new PointerItem();

         list.Items.AddRange( new ToolboxItemEx[] { group, pointer } );

         toolboxItems[name] = new ArrayList();
      }

      private void CollapseCategory(int categoryItem)
      {
         ToolboxItemEx item = (ToolboxItemEx)list.Items[categoryItem];

         if( item.groupControl == false || (CategoryState)(item.tag) == CategoryState.Collapsed )
            return;

         item.tag = CategoryState.Collapsed;
         
         list.BeginUpdate();

         item.image = (int)PictureIndex.piPlus;
         list.Invalidate(list.GetItemRectangle(categoryItem));

         ArrayList categoryItems = (ArrayList)toolboxItems[item.text];

         categoryItem++;
         list.Items.RemoveAt(categoryItem); // remove Pointer
         int i=0;
         while( i < categoryItems.Count )
         {
            list.Items.RemoveAt(categoryItem);
            i++;
         }

         list.EndUpdate();
      }

      private void ExpandCategory(int categoryItem)
      {
         ToolboxItemEx item = (ToolboxItemEx)list.Items[categoryItem];

         if( item.groupControl == false || (CategoryState)(item.tag) == CategoryState.Expanded )
            return;

         item.tag = CategoryState.Expanded;
         
         list.BeginUpdate();

         item.image = (int)PictureIndex.piMinus;
         list.Invalidate(list.GetItemRectangle(categoryItem));

         ArrayList categoryItems = (ArrayList)toolboxItems[item.text];

         list.Items.Insert(++categoryItem, new PointerItem());

         int i=0;
         while( i < categoryItems.Count )
         {
            ToolboxItemWithImage tbItem = (ToolboxItemWithImage)categoryItems[i];            
            ToolboxItemEx newItem = new TBItem(tbItem.item.DisplayName, tbItem.image, tbItem.item);
            list.Items.Insert(++categoryItem, newItem);
            
            i++;
         }

         list.EndUpdate();
      }

      private int FindCategoryItem(string category)
      {
         int i=0;
         foreach(ToolboxItemEx item in list.Items)
         {
            if( item.groupControl && item.text == category )
               return i;
            i++;
         }
         return -1;
      }

      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ToolboxControlEx));
         this.list = new Greatis.FormDesigner.ToolboxList();
         this.images = new System.Windows.Forms.ImageList(this.components);
         this.SuspendLayout();
         // 
         // list
         // 
         this.list.BackColor = System.Drawing.SystemColors.Control;
         this.list.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.list.Dock = System.Windows.Forms.DockStyle.Fill;
         this.list.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
         this.list.FrameColor = System.Drawing.Color.Black;
         this.list.GroupColor = System.Drawing.SystemColors.ControlDark;
         this.list.Images = this.images;
         this.list.ItemHeight = 22;
         this.list.ItemUnderMouseColor = System.Drawing.SystemColors.InactiveCaptionText;
         this.list.Location = new System.Drawing.Point(0, 0);
         this.list.Name = "list";
         this.list.Size = new System.Drawing.Size(136, 192);
         this.list.TabIndex = 0;
         // 
         // images
         // 
         this.images.ImageSize = new System.Drawing.Size(16, 16);
         this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
         this.images.TransparentColor = System.Drawing.Color.Magenta;
         // 
         // ToolboxControlEx
         // 
         this.Controls.Add(this.list);
         this.Name = "ToolboxControlEx";
         this.Size = new System.Drawing.Size(136, 192);
         this.ResumeLayout(false);

      }

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if( toolboxService != null )
               toolboxService.Dispose();

            if( components != null )
               components.Dispose();
         }
         base.Dispose( disposing );
      }

      private void OnItemDrag(object sender, ItemDragEventArgs arg)
      {
         if( arg.item.groupControl == true )
            return;

         System.Drawing.Design.ToolboxItem tag = arg.item.tag as System.Drawing.Design.ToolboxItem;
         if( tag != null && BeginDragAndDrop != null )
            BeginDragAndDrop(this, null);
      }

      private void OnItemDoubleClick(object sender, EventArgs e)
      {
         ToolboxItemEx tb = list.CurrentSelected;
         
         if( tb.groupControl == true )
            return;

         System.Drawing.Design.ToolboxItem tag = tb.tag as System.Drawing.Design.ToolboxItem;
         if( tag != null && DropControl != null)
            DropControl(this, new EventArgs());
      }

      private void OnItemClick(object sender, EventArgs e)
      {
         int selected = list.SelectedIndex;
         if( selected < 0 )
            return;

         ToolboxItemEx selectedItem = (ToolboxItemEx)list.Items[selected];
         if( selectedItem.groupControl )
         {
            if( (CategoryState)selectedItem.tag == CategoryState.Expanded )
               CollapseCategory(selected);
            else
               ExpandCategory(selected);
         }
      }

      #region IToolboxView Members

      public void AddItem(System.Drawing.Design.ToolboxItem item, string category)
      {
         if( category == null )
            category = DefaultCategory;

         if( toolboxItems.ContainsKey(category) == false )
            AddCategory(category);

         ArrayList categoryItems = (ArrayList)toolboxItems[category];
         int imageIndex = images.Images.Add(item.Bitmap, item.Bitmap.GetPixel(0,0));

         categoryItems.Add(new ToolboxItemWithImage(item, imageIndex));

         int categoryIndex = FindCategoryItem(category);
         ToolboxItemEx newItem = new TBItem(item.DisplayName, imageIndex, item);
         list.Items.Insert(categoryIndex + 1 + categoryItems.Count, newItem);
      }

      public void RemoveItem(System.Drawing.Design.ToolboxItem item, string category)
      {
         if( category == null )
            category = DefaultCategory;

         if( toolboxItems.ContainsKey(category) == false )
            return;

         ArrayList categoryItems = (ArrayList)toolboxItems[category];
         int index = 0;
         foreach(ToolboxItemWithImage tbi in categoryItems)
         {
            if( tbi.item == item )
            {
               categoryItems.RemoveAt(index);

               int categoryIndex = FindCategoryItem(category);
               list.Items.RemoveAt(categoryIndex + 1 + index);

               break;
            }
            index++;
         }
      }

      public event System.EventHandler BeginDragAndDrop;

      [Browsable(false)]
      public Cursor CurrentCursor
      {
         get
         {
            ToolboxItemEx item = list.CurrentSelected;
            if( item == null )
               return Cursors.Arrow;
            
            System.Drawing.Design.ToolboxItem tag = item.tag as System.Drawing.Design.ToolboxItem;
            return (tag == null) ? Cursors.Arrow : Cursors.Cross;
         }
      }

      [Browsable(false)]
      public Greatis.FormDesigner.ToolboxCategoryCollection Items
      {
         get
         {
            ToolboxCategoryItem[] tbItems = new ToolboxCategoryItem[toolboxItems.Count];
            
            int i=0;
            foreach(DictionaryEntry de in toolboxItems)
            {
               tbItems[i].name = (string)de.Key;

               ArrayList categoryItems = de.Value as ArrayList;
               System.Drawing.Design.ToolboxItem[] ic = new System.Drawing.Design.ToolboxItem[categoryItems.Count];

               int idx = 0;
               foreach( ToolboxItemWithImage tiwi in categoryItems )
                  ic[idx++] = tiwi.item;

               tbItems[i].items = new System.Drawing.Design.ToolboxItemCollection(ic);
            }

            return new Greatis.FormDesigner.ToolboxCategoryCollection(tbItems);
         }
      }

      public event System.EventHandler DropControl;

      [Browsable(false)]
      public string SelectedCategory
      {
         get
         {
            int index = list.CurrentSelectedIndex;
            if( index < 0 )
               return null;

            ToolboxItemEx item;
            do
               item = (ToolboxItemEx)list.Items[index--];
            while( item.groupControl == false && index >= 0 );

            return (item.groupControl) ? item.text : null;
         }
         set
         {
            int index = FindCategoryItem(value);

            if( index < 0 )
               return;

            ToolboxItemEx item = (ToolboxItemEx)list.Items[index];
            if( (CategoryState)item.tag == CategoryState.Collapsed )
               this.ExpandCategory(index);
            
            list.CurrentSelectedIndex = index + 1;
         }
      }

      [Browsable(false)]
      public System.Drawing.Design.ToolboxItem SelectedItem
      {
         get
         {
            ToolboxItemEx item = list.CurrentSelected;
            return (item!= null) ? item.tag as System.Drawing.Design.ToolboxItem : null;
         }
         set
         {
            if( value == null )
            {
               list.CurrentSelectedIndex = -1;
            }
         }
      }

      #endregion
   }

   internal class ToolboxItemWithImage
   {
      public System.Drawing.Design.ToolboxItem item;
      public int image;

      public ToolboxItemWithImage(System.Drawing.Design.ToolboxItem item, int image)
      {
         this.item = item;
         this.image = image;
      }
   }

   internal class ToolboxItemEx
   {
      public string text;
      public int image;
      public bool groupControl;
      public object tag;


      protected ToolboxItemEx(string text, int image, bool groupControl)
      {
         this.text = text;
         this.image = image;
         this.groupControl = groupControl;

         this.tag = null;
      }
   }

   internal class CategoryItem : ToolboxItemEx
   {
      public CategoryItem(string text, bool expanded) :
         base(text, (expanded) ? (int)ToolboxControlEx.PictureIndex.piMinus : (int)ToolboxControlEx.PictureIndex.piPlus , true)
      {
         tag = (expanded) ? ToolboxControlEx.CategoryState.Expanded : ToolboxControlEx.CategoryState.Collapsed;
      }
   }

   internal class TBItem : ToolboxItemEx
   {
      public TBItem(string text, int image, System.Drawing.Design.ToolboxItem item) :
         base(text, image, false)
      {
         tag = item;
      }
   }

   internal class PointerItem : ToolboxItemEx
   {
      public PointerItem() :
         base("Pointer", (int)ToolboxControlEx.PictureIndex.piArrow, false)
      {
      }
   }

   //internal class 
   #endregion

   #region ToolboxList class

   struct ItemDragEventArgs
   {
      public ToolboxItemEx item;

      public ItemDragEventArgs(ToolboxItemEx item)
      {
         this.item = item;
      }
   }

   internal delegate void ItemDragEventHandler(object sender, ItemDragEventArgs arg);
   
   /// <summary>
   /// List of the toolbox items
   /// </summary>
   internal class ToolboxList : System.Windows.Forms.ListBox
   {
      private int itemUnderMouse = -1, curSelected = -1;

      private Color umColor = SystemColors.InactiveCaptionText;
      private Color umSelColor = SystemColors.InactiveCaption;
      private Color selColor = Color.AliceBlue;
      private Color groupColor = SystemColors.ControlDark;
      private Color frameColor = Color.Black;

      private readonly int DragDistance = 3;
      private Point mouseClickOrigin;
      
      private int groupControlHeightDiff = 5;

      private ImageList imageList;

      public ToolboxList()
      {
      }

      public event ItemDragEventHandler ItemDrag;

      /// <summary>
      /// Gets or sets backgound color of the item under mouse cursor
      /// </summary>
      public Color ItemUnderMouseColor
      {
         get { return umColor; }
         set { umColor = value; }
      }

      
      public Color SelectedItemUnderMouseColor
      {
         get { return umSelColor; }
         set { umSelColor = value; }
      }

      public Color SelectedItemColor
      {
         get { return selColor; }
         set { selColor = value; }
      }

      /// <summary>
      /// Gets or sets frame color of the item under mouse cursor
      /// </summary>
      public Color FrameColor
      {
         get { return frameColor; }
         set { frameColor = value; }
      }

      /// <summary>
      /// Gets or sets backgound color of the group item
      /// </summary>
      public Color GroupColor
      {
         get { return groupColor; }
         set { groupColor = value; }
      }

      public ToolboxItemEx CurrentSelected
      {
         get { return (curSelected >=0 ) ? Items[curSelected] as ToolboxItemEx: null; }
      }

      public int CurrentSelectedIndex
      {
         get { return curSelected; }
         set { ChangeSelection(value); }
      }

      /// <summary>
      /// 
      /// </summary>
      public ImageList Images
      {
         get { return imageList; }
         set { imageList = value; }
      }

      private int GetItemAt(Point pt)
      {
         int i = TopIndex, count = Items.Count;

         while( i < count )
         {
            Rectangle bounds = GetItemRectangle(i);

            if( bounds.Contains(pt) )
               return i;

            i++;
         }
         return -1;
      }

      private int Distance(Point p1, Point p2)
      {
         int x = p1.X - p2.X;
         int y = p1.Y - p2.Y;

         return (int)Math.Sqrt(x*x + y*y);
      }

      protected override void OnMouseMove(MouseEventArgs e)
      {
         if( (e.Button & MouseButtons.Left) != 0 )
         {
            if( Distance(mouseClickOrigin, Control.MousePosition) > DragDistance && curSelected >= 0)
            {
               if( ItemDrag != null )
               {
                  ToolboxItemEx dItem = Items[curSelected] as ToolboxItemEx;
                  ItemDrag(this, new ItemDragEventArgs(dItem));
               }
            }
            return;
         }

         Point mousePoint = PointToClient(Control.MousePosition);
         int item = GetItemAt(mousePoint);

         if( item != -1 && itemUnderMouse != item )
         {
            ToolboxItemEx tbItem = Items[item] as ToolboxItemEx;

            if( itemUnderMouse != -1 )
               PaintItem(itemUnderMouse, false, null);
            
            itemUnderMouse = item;
            PaintItem(itemUnderMouse, true, null);
         }

         base.OnMouseMove(e);
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         if( itemUnderMouse != -1 )
         {
            PaintItem(itemUnderMouse, false, null);
            itemUnderMouse = -1;
         }
         base.OnMouseLeave(e);
      }


      protected void PaintItem(int item,  bool underMouse, Graphics graphics)
      {
         Graphics g = (graphics == null) ? Graphics.FromHwnd(this.Handle) : graphics;
       
         Rectangle bounds = GetItemRectangle(item);

         bool itemSelected = (item == curSelected);
         ToolboxItemEx tbItem = Items[item] as ToolboxItemEx;
         string text = tbItem.text;
         
         underMouse = (underMouse && !tbItem.groupControl);

         Color backColor = this.BackColor;
         if( tbItem.groupControl && itemSelected )
            backColor = this.SelectedItemColor;
         else if( tbItem.groupControl )
            backColor = this.GroupColor;
         else if( underMouse && itemSelected )
            backColor = this.SelectedItemUnderMouseColor;
         else if( underMouse )
            backColor = this.ItemUnderMouseColor;
         else if( itemSelected )
            backColor = this.SelectedItemColor;

         Brush backBrush = new SolidBrush( backColor );
         Brush foreBrush = new SolidBrush( this.ForeColor );

         // draw background & frame
         g.FillRectangle(backBrush, bounds);         
         if( underMouse || itemSelected )
         {
            Pen pen = new Pen(FrameColor, 1);

            bounds.Size = new Size(bounds.Size.Width-1, bounds.Size.Height-1);
            g.DrawRectangle(pen, bounds);
            
            pen.Dispose();
         }

         // restore bounds
         bounds = GetItemRectangle(item);

         // draw image
         int imageHeight = 0;
         int imageWidth = 0;

         if( imageList != null )
         {
            imageHeight = imageList.ImageSize.Height;
            imageWidth = imageList.ImageSize.Width;

            int offset = (tbItem.groupControl) ? 0 : 2;

            if( tbItem.image >= 0 )
               imageList.Draw(g, bounds.Left+offset, bounds.Top+offset, imageWidth, imageHeight, tbItem.image);
         }

         // draw text
         bounds.Location = new Point(bounds.Left+imageWidth+2, bounds.Top);

         SizeF size = g.MeasureString(text, this.Font);
         int vOffset = (bounds.Height - (int)size.Height) / 2;
         int hOffset = 1;

         Font textFont = this.Font;
         if( tbItem.groupControl )
            textFont = new Font(textFont.FontFamily, textFont.Size, FontStyle.Bold);

         bounds.Inflate(-hOffset, - vOffset);         
         g.DrawString(text, textFont, foreBrush, bounds);
         
         foreBrush.Dispose();
         backBrush.Dispose();

         if( tbItem.groupControl )
            textFont.Dispose();

         if( graphics == null )
            g.Dispose();
      }

      protected override void OnDrawItem(DrawItemEventArgs e)
      {
         if( e.Index < 0 || e.Index >= Items.Count)
            return;

         Point mousePosition = this.PointToClient(Control.MousePosition);
         Rectangle bounds = GetItemRectangle(e.Index);

         bool underMouse = bounds.Contains(mousePosition) & (Control.MouseButtons == 0);
         PaintItem(e.Index, underMouse, e.Graphics);
      }

      protected override void OnMeasureItem(MeasureItemEventArgs e)
      {
         if( e.Index < 0 || e.Index >= Items.Count)
            return;

         ToolboxItemEx item = Items[e.Index] as ToolboxItemEx;
         e.ItemWidth = this.ClientSize.Width;
         e.ItemHeight = (item.groupControl) ? this.ItemHeight - groupControlHeightDiff : this.ItemHeight;
      }

      private void ChangeSelection(int newSelected)
      {
         if( newSelected == curSelected )
            return;

         if( curSelected >= 0 )
         {
            int saveSelected = curSelected;

            curSelected = -1;
            PaintItem(saveSelected, false, null);
         }

         if( newSelected >= 0 )
         {
            curSelected = newSelected;
            PaintItem(curSelected, false, null);
         }
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         mouseClickOrigin = Control.MousePosition;
         int index = this.GetItemAt(this.PointToClient(Control.MousePosition));
         if( index >= 0 )
            ChangeSelection(index);

         base.OnMouseDown (e);
      }
   }
   #endregion
}
