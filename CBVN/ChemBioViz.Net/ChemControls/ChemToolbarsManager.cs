using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;


namespace ChemControls
{
    public class ChemToolbarsManager
    {
        UltraToolbarsManager manager;
        UltraToolbarsDockArea left;
        UltraToolbarsDockArea right;
        UltraToolbarsDockArea top;
        UltraToolbarsDockArea bottom;
        Container container;

        class ExMethodInfo
        {
            public object handler;              // the object to execute the method
            public MethodInfo mInfo;            // methodInfo
        }

        List<object> handlers;
        Dictionary<string, ExMethodInfo> methodCache;

        public UltraToolbarsManager Manager { get { return manager; } }

        public ChemToolbarsManager()
        {
            container = new Container();
            manager = new UltraToolbarsManager(container);
            left = new UltraToolbarsDockArea();
            right = new UltraToolbarsDockArea();
            top = new UltraToolbarsDockArea();
            bottom = new UltraToolbarsDockArea();

            handlers = new List<object>();
            methodCache = new Dictionary<string, ExMethodInfo>();

            manager.ToolClick += new ToolClickEventHandler(manager_ToolClick);

            manager.AfterToolActivate += new ToolEventHandler(manager_AfterToolActivate);
            manager.ToolValueChanged += new ToolEventHandler(manager_ToolValueChanged);

            Application.Idle += new EventHandler(Application_Idle);
        }

        void manager_ToolValueChanged(object sender, ToolEventArgs e)
        {
            if (isInIdle)
                return;

            string value = new string(new char[] { });

            ToolBase tool = e.Tool;
            if (tool is ComboBoxTool || tool is FontListTool)
            {
                ComboBoxTool c = tool as ComboBoxTool;
                value = c.Text;
            }

            Type type = GetType();
            List<Control> targets = GetTargetStack();
            foreach (Control target in targets)
            {
                ExMethodInfo exInfo;
                exInfo = FindMethod("Execute" + tool.Key, new Type[] { target.GetType(), typeof(object) });
                if (exInfo == null)
                    continue;
                exInfo.mInfo.Invoke(exInfo.handler, new object[] { target, value });
                break;
            }

        }

        void manager_AfterToolActivate(object sender, ToolEventArgs e)
        {
            savedFocus = GetFocusControl();
        }

        public void Setup(Form form)
        {
            manager.DesignerFlags = 1;
            manager.DockWithinContainer = form;
            manager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);

            left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            left.BackColor = System.Drawing.SystemColors.Control;
            left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            left.ForeColor = System.Drawing.SystemColors.ControlText;
            left.Location = new System.Drawing.Point(0, 27);
            left.Name = "_Form1_Toolbars_Dock_Area_Left";
            left.Size = new System.Drawing.Size(0, form.Height);
            left.ToolbarsManager = manager;

            right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            right.BackColor = System.Drawing.SystemColors.Control;
            right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            right.ForeColor = System.Drawing.SystemColors.ControlText;
            right.Location = new System.Drawing.Point(form.Width, 27);
            right.Name = "_Form1_Toolbars_Dock_Area_Right";
            right.Size = new System.Drawing.Size(0, form.Height);
            right.ToolbarsManager = manager;

            top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            top.BackColor = System.Drawing.SystemColors.Control;
            top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            top.ForeColor = System.Drawing.SystemColors.ControlText;
            top.Location = new System.Drawing.Point(0, 0);
            top.Name = "_Form1_Toolbars_Dock_Area_Top";
            top.Size = new System.Drawing.Size(form.Width, 27);
            top.ToolbarsManager = manager;

            bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            bottom.BackColor = System.Drawing.SystemColors.Control;
            bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            bottom.Location = new System.Drawing.Point(0, form.Height);
            bottom.Name = "_Form1_Toolbars_Dock_Area_Bottom";
            bottom.Size = new System.Drawing.Size(form.Width, 0);
            bottom.ToolbarsManager = manager;

            form.Controls.Add(left);
            form.Controls.Add(right);
            form.Controls.Add(top);
            form.Controls.Add(bottom);

            CreateTools();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        static Control GetFocusControl()
        {
            Control focusControl = null;
            IntPtr focusHandle = GetFocus();
            if (focusHandle != IntPtr.Zero)
                focusControl = Control.FromHandle(focusHandle);
            return focusControl;
        }

        Control savedFocus;

        List<Control> GetTargetStack()
        {
            Control focus = GetFocusControl();

            bool useFocus;
            if (focus == null)
                useFocus = false;
            else if (focus is Infragistics.Win.EmbeddableTextBoxWithUIPermissions)
                useFocus = focus.Parent is ChemDataGrid;
            else if (focus is UltraToolbarsDockArea)
                useFocus = false;
            else
                useFocus = true;

            if (!useFocus)
                focus = savedFocus;
            else
                savedFocus = focus;

            List<Control> targetStack = new List<Control>();
            for (Control c = focus; c != null; c = c.Parent)    // Add the focus and its parents
                targetStack.Add(c);                             // to the target stack
            return targetStack;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public uint ptx;
            public uint pty;
        }

        [DllImport("user32.dll")]
        static extern bool PeekMessage(out MSG lpMsg, HandleRef hWnd, uint MsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        int nextTool = 0;
        bool isInIdle = false;

        void Application_Idle(object sender, EventArgs e)
        {
            isInIdle = true;
            List<Control> targets = GetTargetStack();

            object[] tools = manager.Tools.All;
            int i, n = manager.Tools.Count;
            MSG msg;

            for (i = 0; i < n; i++)
            {
                nextTool = nextTool % n;            // ensure to go back to beginning
                object o = tools[nextTool++];
                ToolBase tool = o as ToolBase;
                //Coverity Bug Fix CID 11418 
                if (tool != null)
                {
                    tool.SharedProps.Enabled = true;    // set the enabled property to true by default
                    bool updated = false;
                    foreach (Control target in targets)
                    {
                        if (UpdateUI(tool, target))
                        {
                            updated = true;
                            break;
                        }
                    }
                    if (!updated)
                    {
                        if (tool is StateButtonTool)
                        {
                            StateButtonTool stool = tool as StateButtonTool;
                            stool.Checked = false;
                        }
                        tool.SharedProps.Enabled = false;
                    }
                }
                if (PeekMessage(out msg, new HandleRef(null, (IntPtr)0), 0, 0, 0))  // if any message lingers in the message loop, then break
                    break;

            }
            isInIdle = false;
        }

        bool UpdateUI(ToolBase tool, Control target)
        {
            Type type = GetType();
            ExMethodInfo xInfo = FindMethod("Update" + tool.Key, new Type[] { manager.GetType(), target.GetType() });
            if (xInfo == null)
                return false;
            xInfo.mInfo.Invoke(xInfo.handler, new object[] { manager, target });
            return true;
        }

        void manager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (isInIdle)
                return;

            ToolBase tool = e.Tool;
            Type type = GetType();
            List<Control> targets = GetTargetStack();
            foreach (Control target in targets)
            {
                ExMethodInfo xInfo;
                xInfo = FindMethod("Update" + tool.Key, new Type[] { manager.GetType(), target.GetType() });
                if (xInfo == null)
                    continue;
                xInfo = FindMethod("Execute" + tool.Key, new Type[] { target.GetType() });
                if (xInfo == null)
                    continue;
                xInfo.mInfo.Invoke(xInfo.handler, new object[] { target });
                break;
            }
        }

        List<string> PartitionString(string s, char term)
        {
            List<string> l = new List<string>();

            int start = 0;
            int end;

            for (; ; )
            {
                end = s.IndexOf(term, start);
                if (end == -1)
                    break;
                l.Add(s.Substring(start, end - start));
                start = end + 1;
            }

            l.Add(s.Substring(start));

            return l;
        }

        public void AddTool(string toolbarKey, string toolKey, string caption, object image, string tooltipText, Type type)
        {
            ToolBase tool = manager.Tools.Exists(toolKey) ? manager.Tools[toolKey] : null;

            Infragistics.Win.Appearance appearance;

            if (tool == null)
            {
                ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(string) });
                tool = constructorInfo.Invoke(new object[] { toolKey }) as ToolBase;

                appearance = new Infragistics.Win.Appearance();
                appearance.Image = image;
                //Coverity fix - CID 11417
                if (tool != null)
                {
                    tool.SharedProps.AppearancesSmall.Appearance = appearance;
                    tool.SharedProps.Caption = caption;
                    tool.SharedProps.ToolTipText = tooltipText;
                    tool.SharedProps.DisplayStyle = caption == null ? ToolDisplayStyle.ImageOnlyOnToolbars : ToolDisplayStyle.ImageAndText;

                    manager.Tools.Add(tool);
                }
            }

            List<string> keys = PartitionString(toolbarKey, '/');

            if (keys.Count == 1)
                tool = manager.Toolbars[toolbarKey].Tools.AddTool(tool.Key);
            else
            {
                PopupMenuTool menu = null;
                int i, n = keys.Count;
                //Coverity Bug Fix CID 11417 
                if (n >= 2)
                {
                    menu = manager.Toolbars[keys[0]].Tools[keys[1]] as PopupMenuTool;
                }
                if (menu != null)
                {
                    for (i = 2; i < n; i++)
                    {
                        menu = menu.Tools[keys[i]] as PopupMenuTool;
                    }
                    if (menu != null)
                        tool = menu.Tools.AddTool(tool.Key);
                }
                //for (i = 1; i < n; i++)
                //{
                //    if (i == 1)
                //        menu = manager.Toolbars[keys[0]].Tools[keys[1]] as PopupMenuTool;
                //    else
                //        menu = menu.Tools[keys[i]] as PopupMenuTool;
                //}
                //tool = menu.Tools.AddTool(tool.Key);
            }

            appearance = new Infragistics.Win.Appearance();
            appearance.Image = image;

            tool.InstanceProps.AppearancesSmall.Appearance = appearance;
            tool.InstanceProps.Caption = caption;
            tool.InstanceProps.DisplayStyle = caption == null ? ToolDisplayStyle.ImageOnlyOnToolbars : ToolDisplayStyle.ImageAndText;
        }

        public void AddTool(string toolbarKey, string toolKey, string caption, object image, string tooltipText)
        {
            AddTool(toolbarKey, toolKey, caption, image, tooltipText, typeof(ButtonTool));
        }

        public void AddToolbar(string key) { manager.Toolbars.AddToolbar(key); }

        void AddComboBoxTool(Infragistics.Win.ValueList list, string value, string text)
        {
            //Coverity Bug Fix CID 11800 
            using (Infragistics.Win.ValueListItem v = new Infragistics.Win.ValueListItem())
            {
                v.DataValue = value;
                v.DisplayText = text;
                list.ValueListItems.Add(v);
            }
        }

        void SetupFontSizeTool()
        {
            ComboBoxTool c = manager.Tools["FontSize"] as ComboBoxTool;

            Infragistics.Win.ValueList list = new Infragistics.Win.ValueList(0);

            AddComboBoxTool(list, "8", "8");
            AddComboBoxTool(list, "12", "12");
            AddComboBoxTool(list, "14", "14");
            AddComboBoxTool(list, "16", "16");
            AddComboBoxTool(list, "24", "24");
            //Coverity Bug Fix CID 11419 
            if (c != null)
            {
                c.ValueList = list;
                c.DropDownStyle = Infragistics.Win.DropDownStyle.DropDown;
            }
        }

        protected virtual void CreateTools()
        {
            AddHandler(new EditCommandsHandler());
            AddHandler(new GridCommandsHandler());

            AddToolbar("Menu");
            AddToolbar("Edit");
            AddToolbar("Style");
            AddToolbar("Designer");
            AddToolbar("DataGrid");

            AddTool("DataGrid", "GridView", "GridView", null, "grid view", typeof(StateButtonTool));
            AddTool("DataGrid", "CardView", "CardView", null, "card view", typeof(StateButtonTool));
            AddTool("DataGrid", "LabelsInHeaders", null, Resources.LabelsInHeaders, "labels in headers", typeof(StateButtonTool));
            AddTool("DataGrid", "LabelsInCells", null, Resources.LabelsInCells, "labels in cells", typeof(StateButtonTool));

            AddTool("Menu", "File", "File", null, null, typeof(PopupMenuTool));
            AddTool("Menu", "Edit", "Edit", null, null, typeof(PopupMenuTool));

            AddTool("Menu/Edit", "Undo", "Undo", Resources.Edit_UndoHS, "undo last action");

            AddTool("Edit", "Undo", null, Resources.Edit_UndoHS, "undo last action");
            AddTool("Edit", "Redo", null, Resources.Edit_RedoHS, "redo last action");
            AddTool("Edit", "Cut", null, Resources.Cut, "copy to clipboard and delete selection");
            AddTool("Edit", "Copy", null, Resources.Copy, "copy selection to clipboard");
            AddTool("Edit", "Paste", null, Resources.Paste, "paste clipboard to document");

            manager.Toolbars["Edit"].Tools["Cut"].InstanceProps.IsFirstInGroup = true;

            AddTool("Style", "Bold", null, Resources.boldhs, "bold style", typeof(StateButtonTool));
            AddTool("Style", "Italic", null, Resources.ItalicHS, "italic style", typeof(StateButtonTool));
            AddTool("Style", "FontName", null, null, "font name", typeof(FontListTool));
            AddTool("Style", "FontSize", null, null, "font size", typeof(ComboBoxTool));

            SetupFontSizeTool();

            AddTool("Designer", "ToggleDesignMode", "Designer", Resources.Icon1, "toggles design mode", typeof(StateButtonTool));
            AddTool("Designer", "AlignLeft", null, Resources.AlignObjectsLeftHS, "Align Left");
            AddTool("Designer", "AlignHCenter", null, Resources.AlignObjectsCenteredHorizontalHS, "Align center horizontally");
            AddTool("Designer", "AlignRight", null, Resources.AlignObjectsRightHS, "Align Right");

            AddTool("Designer", "AlignTop", null, Resources.AlignObjectsTopHS, "Align Top");
            AddTool("Designer", "AlignVCenter", null, Resources.AlignObjectsCenteredVerticalHS, "Align center vertically");
            AddTool("Designer", "AlignBottom", null, Resources.AlignObjectsBottomHS, "Align Bottom");

            AddTool("Designer", "MakeSameWidth", null, Resources.MakeSameWidth, "Make same width");
            AddTool("Designer", "MakeSameHeight", null, Resources.MakeSameHeight, "Make same height");

            AddTool("Designer", "BringToFront", null, Resources.BringToFrontHS, "bring to front");
            AddTool("Designer", "SendToBack", null, Resources.SendToBackHS, "send to back");

            manager.Toolbars["Designer"].Tools["AlignLeft"].InstanceProps.IsFirstInGroup = true;
            manager.Toolbars["Designer"].Tools["AlignTop"].InstanceProps.IsFirstInGroup = true;
            manager.Toolbars["Designer"].Tools["MakeSameWidth"].InstanceProps.IsFirstInGroup = true;
            manager.Toolbars["Designer"].Tools["BringToFront"].InstanceProps.IsFirstInGroup = true;


        }

        public void AddHandler(object o) { handlers.Add(o); methodCache.Clear(); }
        public void RemoveHandler(object o) { handlers.Remove(o); methodCache.Clear(); }

        ExMethodInfo FindMethod(string methodName, Type[] parameters)
        {
            string name = methodName;
            foreach (Type type in parameters)
            {
                name += ",";
                name += type.FullName;
            }

            if (methodCache.ContainsKey(name))
                return methodCache[name];

            MethodInfo mInfo = null;
            object handler = null;

            foreach (object o in handlers)
            {
                Type type = o.GetType();
                MethodInfo m = type.GetMethod(methodName, parameters);   // if newer, just overwrite
                if (m != null)
                {
                    handler = o;
                    mInfo = m;
                }
            }

            ExMethodInfo x = null;

            if (mInfo != null)
            {
                x = new ExMethodInfo();
                x.handler = handler;
                x.mInfo = mInfo;
            }

            methodCache[name] = x;                      // store it whether null or not
            return x;
        }
    }
}
