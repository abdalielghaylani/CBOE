/*  GREATIS FORM DESIGNER FOR .NET
 *  IToolboxService Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Greatis.FormDesigner
{
    /// <summary>
    /// Top-level category item in the toolbox service
    /// </summary>
    public class ToolboxCategoryItem
    {
        /// <summary>
        /// Category name
        /// </summary>
        public string name;

        /// <summary>
        /// Collection of items in the category
        /// </summary>
        public ToolboxItemCollection items;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Category name</param>
        /// <param name="items">Items in category</param>
        public ToolboxCategoryItem(string name, ToolboxItemCollection items)
        {
            this.name = name;
            this.items = items;
        }
    }

    /// <summary>
    /// Collection of toolbox categories
    /// </summary>
    public class ToolboxCategoryCollection : ReadOnlyCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the toolbox category collection using the specified array of the toolbox categories
        /// </summary>
        /// <param name="items">Categories in toolbox</param>
        public ToolboxCategoryCollection(ToolboxCategoryItem[] items)
        {
            InnerList.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the toolbox category collection using the specified category collection
        /// </summary>
        /// <param name="reference">Source category collection</param>
        public ToolboxCategoryCollection(ToolboxCategoryCollection reference)
        {
            InnerList.AddRange(reference);
        }

        /// <summary>
        /// Gets the category item at the specified index
        /// </summary>
        /// <param name="index">Index of category item</param>
        /// <returns></returns>
        public ToolboxCategoryItem this[int index]
        {
            get
            {
                if (index < 0 || index > Count - 1)
                    throw new IndexOutOfRangeException("ToolboxCategoryCollection index out of range");

                return (ToolboxCategoryItem)InnerList[index];
            }
        }
    }

    /// <summary>
    /// Provides the visual toolbox functionality for toolbox service
    /// </summary>
    public interface IToolboxView
    {
        /// <summary>
        /// Returns current mouse cursor shape
        /// </summary>
        Cursor CurrentCursor
        {
            get; // return null || Cursors.Arrow if no current selected items
        }

        /// <summary>
        /// Returns toolbox category collection
        /// </summary>
        ToolboxCategoryCollection Items
        {
            get;
        }

        /// <summary>
        /// Sets of gets selected toolbox item
        /// </summary>
        ToolboxItem SelectedItem
        {
            get;
            set; // if item == null then need unselect current item
        }

        /// <summary>
        /// Sets of gets selected toolbox category item
        /// </summary>
        string SelectedCategory
        {
            get;
            set;
        }

        /// <summary>
        /// Adds new toolbox item to the specified category
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="category">Target category, can be null</param>
        void AddItem(ToolboxItem item, string category);

        /// <summary>
        /// Removes toolbox item from the specified category
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <param name="category">Category, can be null</param>
        void RemoveItem(ToolboxItem item, string category);

        /// <summary>
        /// Refreshes the state of the toolbox items
        /// </summary>
        void Refresh();

        /// <summary>
        /// Notify toolbox service for item drag-and-drop start
        /// </summary>
        event EventHandler BeginDragAndDrop;
        /// <summary>
        /// Notify toolbox service for item dropping be double-click the item in the toolbox control
        /// </summary>
        event EventHandler DropControl;
    }

    /// <summary>
    /// Class implemented IToolboxService interface, see information about members in MSDN
    /// </summary>
    public class ToolboxService : IToolboxService, IDisposable
    {
        private IToolboxView view;

        private ArrayList designers = new ArrayList();
        private Designer defaultDesigner;

        private Hashtable creators = new Hashtable();

        /// <summary>
        /// Initializes a new instance of the toolbox service using the specified toolbox view
        /// </summary>
        /// <param name="toolboxView"></param>
        public ToolboxService(IToolboxView toolboxView)
        {
            view = toolboxView;

            view.BeginDragAndDrop += new EventHandler(OnDragAndDrop);
            view.DropControl += new EventHandler(OnDropControl);
        }

        /// <summary>
        /// Releases all resources used by the ToolboxService
        /// </summary>
        public void Dispose()
        {
            view.BeginDragAndDrop -= new EventHandler(OnDragAndDrop);
            view.DropControl -= new EventHandler(OnDropControl);
        }

        private void OnDragAndDrop(object sender, EventArgs e)
        {
            Control ctrl = view as Control;
            ToolboxItem item = view.SelectedItem;

            if (item != null && ctrl != null)
            {
                DataObject data = (DataObject)SerializeToolboxItem(item);
                DragDropEffects dropEffect = ctrl.DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void OnDropControl(object sender, EventArgs e)
        {
            ToolboxItem item = view.SelectedItem;
            if (item == null)
                return;

            Designer designer = Designer;
            DesignerTransaction dt = designer.DesignerHost.CreateTransaction("Create component");
            using (dt)
            {
                IComponent ic = item.CreateComponents(designer.DesignerHost)[0];
                if (ic != null && ic is Control)
                {
                    try
                    {
                        IDesignerHost dh = designer.DesignerHost;
                        Control parent = (Control)dh.RootComponent;
                        IComponentChangeService iccs = (IComponentChangeService)dh.GetService(typeof(IComponentChangeService));

                        ComponentDesigner cd = (ComponentDesigner)dh.GetDesigner(ic);
#if V1
               cd.OnSetComponentDefaults(); 
#else
                        cd.InitializeNewComponent(new Hashtable());
#endif

                        iccs.OnComponentChanging(ic, null);

                        Control ctrl = ic as Control;
                        ctrl.SuspendLayout();
                        ctrl.Parent = parent;
                        ctrl.Location = new Point((parent.Width - ctrl.Width) / 2, (parent.Height - ctrl.Height) / 2);
                        ctrl.Text = ctrl.Name;
                        ctrl.ResumeLayout();

                        iccs.OnComponentChanged(ic, null, null, null);

                        ISelectionService ss = (ISelectionService)dh.GetService(typeof(ISelectionService));
                        Control[] selected = new Control[1] { (Control)ic };
                        ss.SetSelectedComponents(selected, SelectionTypes.Replace);
                    }
                    catch
                    {
                    }

                }
                dt.Commit();
            }

            view.SelectedItem = null;
        }

        private bool FindDesigner(Greatis.FormDesigner.Designer designer)
        {
            foreach (Designer current in designers)
            {
                if (current == designer)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Removes designer from list of the associated designers
        /// </summary>
        /// <param name="designer">removed designer</param>
        public void RemoveDesigner(Greatis.FormDesigner.Designer designer)
        {
            if (FindDesigner(designer) == false)
                return;

            designer.DesignerHost.RemoveService(typeof(IToolboxService));
            designers.Remove(designer);
            if (defaultDesigner == designer)
            {
                if (designers.Count > 0)
                    defaultDesigner = (Designer)designers[0];
                else
                    defaultDesigner = null;
            }
        }

        /// <summary>
        /// Gets default Designer, add new designer into list of the associated deisgners
        /// </summary>
        public Greatis.FormDesigner.Designer Designer
        {
            set
            {
                if (value == null)
                {
                    RemoveDesigner(defaultDesigner);
                }
                else
                {
                    if (FindDesigner(value) == false)
                        AddDesigner(value);
                }
                defaultDesigner = value;
            }
            get
            {
                return defaultDesigner;
            }
        }

        /// <summary>
        /// add new designer into list of the associated deisgners
        /// </summary>
        /// <param name="designer">added designer</param>
        public void AddDesigner(Designer designer)
        {
            if (designer != null)
            {
                designer.DesignerHost.AddService(typeof(IToolboxService), this);
                designers.Add(designer);
                if (designers.Count == 1)
                    defaultDesigner = designer;
            }
        }

        #region IToolboxService Members
        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, System.ComponentModel.Design.IDesignerHost host)
        {
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, System.ComponentModel.Design.IDesignerHost host)
        {
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
        {
            IDataObject dataObject = serializedObject as IDataObject;
            if (dataObject != null)
            {
                if (dataObject.GetDataPresent(typeof(ToolboxItem)))
                    return (ToolboxItem)dataObject.GetData(typeof(ToolboxItem));

                string[] formats = dataObject.GetFormats();
                foreach (string format in formats)
                {
                    ToolboxItemCreatorCallback callback = (ToolboxItemCreatorCallback)creators[format];
                    if (callback != null)
                    {
                        ToolboxItem ti = null;
                        try
                        {
                            ti = callback(serializedObject, format);
                        }
                        catch
                        {
                        }
                        if (ti != null)
                            return ti;
                    }
                }
            }

            return null;
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject)
        {
            return DeserializeToolboxItem(serializedObject, null);
        }

        public object SerializeToolboxItem(ToolboxItem toolboxItem)
        {
            DataObject data = (toolboxItem == null) ? null : new DataObject(toolboxItem);
            return data;
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
        {
            view.RemoveItem(toolboxItem, category);
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem)
        {
            view.RemoveItem(toolboxItem, null);
        }

        public bool SetCursor()
        {
            Cursor curCursor = view.CurrentCursor;
            if (curCursor == null || curCursor == Cursors.Arrow)
            {
                foreach (Designer designer in designers)
                {
                    if (designer.DesignContainer != null || designer.DesignedForm == null)
                        return false;
                    designer.DesignedForm.Cursor = Cursors.Arrow;
                }
                return false;
            }

            foreach (Designer designer in designers)
            {
                if (designer.DesignContainer != null)
                    designer.DesignContainer.Cursor = curCursor;
                else
                    designer.DesignedForm.Cursor = curCursor;
            }
            return true;
        }

        public void AddToolboxItem(ToolboxItem toolboxItem, string category)
        {
            view.AddItem(toolboxItem, category);
        }

        public void AddToolboxItem(ToolboxItem toolboxItem)
        {
            view.AddItem(toolboxItem, null);
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
        {
            creators[format] = creator;
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format)
        {
            AddCreator(creator, format, null);
        }

        public void RemoveCreator(string format, IDesignerHost host)
        {
            creators.Remove(format);
        }

        public void RemoveCreator(string format)
        {
            RemoveCreator(format, null);
        }

        public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
        {
            foreach (ToolboxCategoryItem categoryItem in view.Items)
            {
                if (categoryItem.name == category)
                    return new ToolboxItemCollection(categoryItem.items);
            }

            return null;
        }

        public ToolboxItemCollection GetToolboxItems(string category)
        {
            return GetToolboxItems(category, null);
        }

        public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
        {
            ArrayList itemArray = new ArrayList();
            foreach (ToolboxCategoryItem categoryItem in view.Items)
                itemArray.AddRange(categoryItem.items);

            ToolboxItem[] items = new ToolboxItem[itemArray.Count];
            itemArray.CopyTo(items);

            return new ToolboxItemCollection(items);
        }

        public ToolboxItemCollection GetToolboxItems()
        {
            return GetToolboxItems((IDesignerHost)null);
        }

        public CategoryNameCollection CategoryNames
        {
            get
            {
                ToolboxCategoryCollection items = view.Items;
                if (items.Count != 0)
                {
                    string[] names = new string[items.Count];

                    int i = 0;
                    foreach (ToolboxCategoryItem item in items)
                        names[i++] = item.name;

                    return new CategoryNameCollection(names);
                }
                return null;
            }
        }

        public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
        {
            return view.SelectedItem;
        }

        public ToolboxItem GetSelectedToolboxItem()
        {
            return GetSelectedToolboxItem(null);
        }

        public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
        {
            view.SelectedItem = toolboxItem;
        }

        public bool IsSupported(object serializedObject, ICollection filterAttributes)
        {
            return true;
        }

        public bool IsSupported(object serializedObject, IDesignerHost host)
        {
            return true;
        }

        public void SelectedToolboxItemUsed()
        {
            view.SelectedItem = null;
        }

        public bool IsToolboxItem(object serializedObject, IDesignerHost host)
        {
            if (this.DeserializeToolboxItem(serializedObject, host) != null)
                return true;
            return false;
        }

        public bool IsToolboxItem(object serializedObject)
        {
            return IsToolboxItem(serializedObject, null);
        }

        public string SelectedCategory
        {
            get { return view.SelectedCategory; }
            set { view.SelectedCategory = value; }
        }

        public void Refresh()
        {
            view.Refresh();
        }

        #endregion
    }
}
