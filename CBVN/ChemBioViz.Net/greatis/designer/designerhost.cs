/*  GREATIS FORM DESIGNER FOR .NET
 *  Private Designer Interface Implementation
 *  Copyright (C) 2004-2008 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using System.Diagnostics;

using System.IO;
using System.Collections.Specialized;
using System.Reflection;

using System.Runtime.Remoting;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Greatis
{
   namespace FormDesigner
   {
      internal class DesignerHost : DesignSurfaceManager, IDesignEvents
      {
         private Hashtable m_Parents = new Hashtable();
         private Hashtable m_SelectedTab = new Hashtable();
         private Hashtable m_ExistingControls = new Hashtable();

         public bool m_ErrorMessages = false;

         private SortedList m_designedContainers = new SortedList();

         private string m_log;

         private Control m_DesignedForm;
         private Control m_DesignContainer;

         private Designer m_Owner;
         private IDesignerHost m_Host;

         public DesignerHost()
         {
            m_designedContainers.Add("DevExpress.XtraGrid.GridControl", "ViewCollection");
            m_designedContainers.Add("DevExpress.XtraGrid.Views.Grid.GridView", "Columns");

            ServiceContainer.AddService(typeof(DesignerOptionService), new DesignerOptionServiceImpl());

            ServiceContainer.AddService(typeof(IDesignEvents), this);
            //ServiceContainer.AddService(typeof(EnvDTE.ProjectItem), new DTE());

            DesignSurface ds = CreateDesignSurface(this.ServiceContainer);
            ActiveDesignSurface = ds;
            m_Host = (IDesignerHost)ds.GetService(typeof(IDesignerHost));

            MenuCommandService mcs = new MenuCommandService(Host);
            mcs.AddingVerb += new AddingVerbHandler(LocalMenuAddingVerb);
            ServiceContainer.AddService(typeof(IMenuCommandService), mcs);

            TypeDescriptorFilterService tdfs = new TypeDescriptorFilterService(Host);
            tdfs.FilterAtt += new FilterEventHandler(FilterAtt);
            tdfs.FilterEvnts += new FilterEventHandler(FilterEvnts);
            tdfs.FilterProps += new FilterEventHandler(FilterProps);
            ServiceContainer.AddService(typeof(ITypeDescriptorFilterService), tdfs);

            ServiceContainer.AddService(typeof(EventFilter), new EventFilter(this));
            ServiceContainer.AddService(typeof(INameCreationService), new NameService(m_Host.Container));

            ISelectionService iss = (ISelectionService)m_Host.GetService(typeof(ISelectionService));
            m_Host.RemoveService(typeof(ISelectionService));
            m_Host.AddService(typeof(ISelectionService), new SelectionService(iss));

            m_Host.AddService(typeof(IUIService), new UIService(m_Host));

            IComponentChangeService css = (IComponentChangeService)m_Host.GetService(typeof(IComponentChangeService));
            css.ComponentAdded += new ComponentEventHandler(OnComponentAdded);

            ComponentSerializationService csrs = new CodeDomComponentSerializationService(ds);
            m_Host.AddService(typeof(ComponentSerializationService), csrs);
            UndoEngineImpl uei = new UndoEngineImpl(m_Host);
            m_Host.AddService(typeof(UndoEngine), uei);
         }

         #region IDesignEvents Members

         public event FilterEventHandler FilterAttributes;

         public event FilterEventHandler FilterEvents;

         public event FilterEventHandler FilterProperties;

         public event AddingVerbHandler AddingVerb;

         public event AllowDesignHandler AllowDesign;

         #endregion

         public SortedList DesignedContainers
         {
            get { return m_designedContainers; }
         }

         protected override DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider)
         {
            return new HostDesignSurface(parentProvider, this);
         }

         private bool LocalMenuAddingVerb(IComponent primarySelection, DesignerVerb verb)
         {
             if (AddingVerb != null)
                 return AddingVerb(primarySelection, verb);
             return true;
         }

         internal void FilterAtt(IComponent component, ref FilterEventArgs args)
         {
             if (FilterAttributes != null)
                 FilterAttributes(component, ref args);
         }

         internal void FilterEvnts(IComponent component, ref FilterEventArgs args)
         {
             if (FilterEvents != null)
                 FilterEvents(component, ref args);
         }

         internal void FilterProps(IComponent component, ref FilterEventArgs args)
         {
             if (FilterProperties != null)
                 FilterProperties(component, ref args);
         }

         private void OnComponentAdded(object sender, ComponentEventArgs arg)
         {
            // save selected tab index
            TabControl tc = arg.Component as TabControl;
            if (tc != null)
            {
               TabPage selected = tc.SelectedTab;
               TabControl.TabPageCollection tabPages = tc.TabPages;

               int index = 0;
               foreach (TabPage page in tabPages)
               {
                  if (page == selected)
                  {
                     m_SelectedTab[tc] = index;
                     break;
                  }
                  index++;
               }
            }
         }

         internal IDesignerHost Host
         {
            get
            {
               if( m_Host == null )
                  m_Host = (IDesignerHost)ActiveDesignSurface.GetService(typeof(IDesignerHost));

               return m_Host;
            }
         }

         internal Control /*Form*/ DesignedForm
         {
            get { return m_DesignedForm; }
            set
            {
               if (m_DesignedForm != null)
               {
                  Form designedForm = (m_DesignedForm is Form) ? (Form)m_DesignedForm : m_DesignedForm.FindForm();
                  if (designedForm != null)
                     designedForm.Closed -= new EventHandler(DesginedFormClosed);
               }
               if (value != null)
               {
                  Form designedForm = (value is Form) ? (Form)value : value.FindForm();
                  if (designedForm != null)
                     designedForm.Closed += new EventHandler(DesginedFormClosed);
               }
               m_DesignedForm = value;
            }
         }

         internal Control DesignContainer
         {
            set { m_DesignContainer = value; }
            get { return m_DesignContainer; }
         }

         internal string LogName
         {
            set { m_log = value; }
            get { return m_log; }
         }

         private void WriteToLog(string str)
         {
            if( m_log != null )
            {
               using (StreamWriter strm = File.AppendText(m_log))
               {
                  strm.WriteLine(str);
               }
            }
         }

         private void DesginedFormClosed(object sendr, EventArgs a)
         {
            DesignedForm = null;
         }

         internal Designer Owner
         {
            get { return m_Owner; }
            set
            {
               UndoEngineImpl uei = (UndoEngineImpl)m_Host.GetService(typeof(UndoEngine));

               if( m_Owner != null && uei != null )
                  uei.UndoCountChanges -= new EventHandler(m_Owner.UndoCountChanged);
               m_Owner = value;
               if( m_Owner != null && uei != null )
                  uei.UndoCountChanges += new EventHandler(m_Owner.UndoCountChanged);
            }
         }

         private void LoadUIStyles()
         {
            IUIService svc = (IUIService)m_Host.GetService(typeof(IUIService));

            IDictionary styles = svc.Styles;
            styles.Clear();
            styles.Add("HighlightColor", SystemColors.HighlightText);
            styles.Add("DialogFont", Control.DefaultFont);
         }

         private Control FindEqualControl(Control parent, Control check)
         {
            foreach (Control ctrl in parent.Controls)
            {
               if (ctrl.Name == check.Name)
                  return ctrl;
            }
            return null;
         }

         private void CopyProperties(object source, object dest)
         {
            PropertyDescriptorCollection pdSource = TypeDescriptor.GetProperties(source);
            PropertyDescriptorCollection pdDest = TypeDescriptor.GetProperties(dest);

            foreach (PropertyDescriptor pd in pdSource)
            {
               if (pd.IsReadOnly || pd.IsBrowsable == false) continue;

               PropertyDescriptor pdest = pdDest.Find(pd.Name, false);
               if (pdest != null)
               {
                  try
                  {
                     object value = pd.GetValue(source);
                     object destVal = pdest.GetValue(dest);
                     if (!dest.Equals(value))
                        pdest.SetValue(dest, value);
                  }
                  catch
                  {
                  }
               }
            }

         }

         private void AddChildControls(IComponent comp)
         {
            Control ctrl = comp as Control;
            if (ctrl != null && ctrl.Controls.Count > 0)
            {
               IDesigner d = m_Host.GetDesigner(comp);
               if (typeof(ParentControlDesigner).IsAssignableFrom(d.GetType()))
               {
                  Control[] controls = new Control[ctrl.Controls.Count];
                  ctrl.Controls.CopyTo(controls, 0);
                  AddingControls(controls, ctrl);
               }
            }
         }

         private void AddDataSetComponents(DataSet ds)
         {
            IContainer container = (IContainer)m_Host.GetService(typeof(IContainer));
            foreach (DataTable table in ds.Tables)
            {
               container.Add(table, table.TableName);
               foreach (DataColumn column in table.Columns)
               {
                  container.Add(column, column.ColumnName);
               }
            }
         }

         private void CheckContainedComponents(IComponent control)
         {
            Type ct = control.GetType();

            PropertyInfo pi = null;
            foreach (String key in m_designedContainers.Keys)
               if (ct.FullName.IndexOf(key) >= 0)
               {
                  pi = ct.GetProperty(m_designedContainers[key] as string);
                  break;
               }

            if (pi == null)
            {
               if (control is DataSet)
                  AddDataSetComponents(control as DataSet);
               return;
            }

            object v = pi.GetValue(control, null);
            if (v == null)
               return;

            if (v is IList)
            {
               IEnumerator en = (v as IList).GetEnumerator();
               while (en.MoveNext())
               {
                  IComponent comp = en.Current as IComponent;
                  if (comp != null)
                  {
                     m_Host.Container.Add(comp, null);
                     AddChildControls(comp);
                  }
               }
            }
            else if (v is IComponent)
               m_Host.Container.Add(v as IComponent, null);
         }

         private bool CanInitializeExisting(object control)
         {
            return (control is TabControl || control is DataGridView);
         }

         private void AddingControls(Array controls, Control parent)
         {
            foreach (Control refCtrl in controls)
            {
               Control control = refCtrl;
               try
               {
                  if (m_DesignContainer != null)
                  {
                     Control ctrl = FindEqualControl(parent, control);
                     if (ctrl != null)
                     {
                        CopyProperties(control, ctrl);
                        control = ctrl;
                     }
                  }
                  // set new parent for control one by one
                  // if we docking 2 controls (top & fill),
                  // then changing parent in foreach loop breaking layout
                  control.Parent = parent;

                  // some controls in V2 need reset parent (TabControl, some custom controls)
                  //control.Parent = null;  //CSBR-110421: Saving a long (vertical height) form from browse mode alters form layout and subform col selection
                  // commenting out the above line causes breakage

                  m_Host.Container.Add(control, control.Name);

                  PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(control);
                  PropertyDescriptor pd = pdc["Text"];
                  string text = null;
                  if (pd != null)
                     text = (string)pd.GetValue(control);

                  // init designerr (need for DevExpress)
                  IComponentInitializer cd = m_Host.GetDesigner(control) as IComponentInitializer;
                  if (cd != null)
                  {
                     if (CanInitializeExisting(control))
                        cd.InitializeExistingComponent(null);
                     else
                        cd.InitializeNewComponent(null);
                  }

                  if (pd != null)
                     pd.SetValue(control, text);

                  // save TabControl SelectedTab
                  if (control is TabControl)
                  {
                     TabControl tc = control as TabControl;
                     if (tc.TabCount > 1)
                        tc.SelectedTab = tc.TabPages[0];
                  }
                  control.Parent = parent;
                  CheckContainedComponents(control);
               }
               catch
               {
               }

               if (control.Controls.Count != 0 && m_Host.GetDesigner(control) is ParentControlDesigner)
               {
                  Control[] childControls = new Control[control.Controls.Count];
                  control.Controls.CopyTo(childControls, 0);

                  AddingControls(childControls, control);
               }
            }
         }

         private void MakeExistingControlList(Control designed, Control runtime)
         {
            foreach (Control dc in designed.Controls)
               m_ExistingControls.Add(dc, runtime.Controls[dc.Name]);
         }

         internal void StartDesign()
         {
            Control[] designedControls = new Control[m_DesignedForm.Controls.Count];
            m_DesignedForm.Controls.CopyTo(designedControls, 0);

            HostDesignSurface ds = (HostDesignSurface)ActiveDesignSurface;
            if (ds.IsLoaded == false)
            {
               if (m_DesignContainer == null)
               {
                  RootControl dcs = new RootControl();
                  ds.BeginLoad(dcs.GetType());
               }
               else
               {
                  ds.BeginLoad(m_DesignedForm.GetType());
                  MakeExistingControlList(m_Host.RootComponent as Control, m_DesignedForm);
               }
            }

            Control c = ds.View as Control;
            if (m_DesignContainer != null)
            {
               Control root = m_Host.RootComponent as Control;
               CopyProperties(m_DesignedForm, root);
               root.Name = m_DesignedForm.Name;
               root.Size = m_DesignedForm.Size;

               c.Parent = m_DesignContainer;
            }
            else
            {
               c.Parent = DesignedForm;
               (m_Host.RootComponent as RootControl).DesignedControl = m_DesignedForm;
            }


            c.Location = new Point(0, 0);
            c.Dock = DockStyle.Fill;
            c.Visible = true;

            if (m_DesignContainer == null)
            {
               SelectionService iss = (SelectionService)m_Host.GetService(typeof(ISelectionService));
               iss.SetRootComponent(m_Host.RootComponent);
               iss.SetSelectedComponents(null);
            }

            LoadUIStyles();

            AddingControls(designedControls, m_Host.RootComponent as Control);

            EventFilter ef = (EventFilter)GetService(typeof(EventFilter));
            Application.AddMessageFilter(ef);

            UndoEngineImpl uei = (UndoEngineImpl)m_Host.GetService(typeof(UndoEngine));
            uei.ResetUndo();
         }

         internal ArrayList extInvoke = new ArrayList();
         void AddExtProviders(Control control, PropertyDescriptorCollection pc, Type epdType)
         {
            foreach (PropertyDescriptor p in pc)
            {
               if (!epdType.IsAssignableFrom(p.GetType())) continue;

               object value = p.GetValue(control);
               if (value == null) continue;

               object provider = p.GetType().InvokeMember("provider",
                  BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                  null, p, null) as IComponent;
               if (provider == null) continue;

               MethodInfo mi = provider.GetType().GetMethod("Set" + p.Name);
               if (mi == null) continue;

               //mi.Invoke(provider, new object[] { control, value });

               ExtInvokeParam ep = new ExtInvokeParam();
               ep.mi = mi;
               ep.provider = provider;
               ep.param = new object[] { control, value };

               extInvoke.Add(ep);
            }
         }

         void RestoreControls(Array controls, Control parent)
         {
            Assembly a = Assembly.GetAssembly(typeof(System.ComponentModel.PropertyDescriptor));
            Type epdType = a.GetType("System.ComponentModel.ExtendedPropertyDescriptor");

            ISelectionService iss = (ISelectionService)m_Host.GetService(typeof(ISelectionService));
            iss.SetSelectedComponents(null);

            foreach (Control refCtrl in controls)
            {
               // change visiblity for the component
               bool value = true, enableValue = true;
               Control control = refCtrl;
               try
               {

                  PropertyDescriptorCollection pc = TypeDescriptor.GetProperties(control);
                  PropertyDescriptor pd = pc.Find("Visible", true);
                  if (pd != null)
                     value = (bool)pd.GetValue(control);
                  /*
                  value = (bool)typeof(Control).InvokeMember("GetState",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, control, new object[] { 2 });
                  */
                  control.Visible = value;

                  pd = pc.Find("Enabled", true);
                  if (pd != null)
                     enableValue = (bool)pd.GetValue(control);

                  AddExtProviders(control, pc, epdType);
               }
               catch
               {
               }

               if (control.Controls.Count != 0 && m_Host.GetDesigner(control) is ParentControlDesigner)
               {
                  Control[] childControls = new Control[control.Controls.Count];
                  control.Controls.CopyTo(childControls, 0);

                  RestoreControls(childControls, control);
               }

               if (control.Site != null)
                  control.Name = control.Site.Name;
               try
               {
                  if (m_ExistingControls.ContainsKey(control))
                  {
                     Control dest = m_ExistingControls[control] as Control;
                     if (dest != null)
                        CopyProperties(control, dest);
                  }
                  else
                  {
                     m_Host.Container.Remove(control);  //CSBR-110421: Saving a long (vertical height) form from browse mode alters form layout and subform col selection
                     if (m_DesignContainer != null)
                     {
                        Control ctrl = FindEqualControl(parent, control);
                        if (ctrl != null)
                        {
                           CopyProperties(control, ctrl);
                           control = ctrl;
                        }
                     }
                     control.Parent = parent;

                     // disabling control, if need
                     // we can disabling only after removing control
                     if (enableValue == false)
                        control.Enabled = false;

                  }

                  TabControl tc = control as TabControl;
                  if (tc != null)
                  {
                     int index = 0;
                     object val = m_SelectedTab[tc];
                     if (val != null)
                        index = (int)val;

                     TabControl.TabPageCollection tabPages = tc.TabPages;
                     if (index >= tabPages.Count)
                        index = 0;
                     tc.SelectedTab = tabPages[index];
                  }
               }
               catch
               {
               }
            }
         }

         private void RemoveComponents()
         {
            ComponentCollection components = m_Host.Container.Components;

            foreach (IComponent component in components)
            {
               if (component is Control)
                  continue;

               m_Host.Container.Remove(component);
            }
         }

         private void UpdateExtProviders()
         {
            foreach (ExtInvokeParam ep in extInvoke)
            {
               try
               {
                  ep.mi.Invoke(ep.provider, new object[] { ep.param[0], null });
               }
               catch
               {
               }
               try
               {
                  ep.mi.Invoke(ep.provider, ep.param);
               }
               catch
               {
               }
            }
            extInvoke.Clear();
         }
         // <object, 
         //private Hashtable

         internal void StopDesign()
         {
            EventFilter ef = (EventFilter)GetService(typeof(EventFilter));
            Application.RemoveMessageFilter(ef);

            Control root = m_Host.RootComponent as Control;
            Control[] designedControls = new Control[root.Controls.Count];
            root.Controls.CopyTo(designedControls, 0);

            RestoreControls(designedControls, m_DesignedForm);

            RemoveComponents();

            m_SelectedTab.Clear();

            if( m_DesignContainer != null )
               CopyProperties(m_Host.RootComponent as Control, m_DesignedForm);
            else
               (m_Host.RootComponent as RootControl).DesignedControl = null;

            HostDesignSurface ds = (HostDesignSurface)ActiveDesignSurface;
            ((Control)ds.View).Parent = null;

            UpdateExtProviders();
         }
      }
   }

   internal struct ExtInvokeParam
   {
      public MethodInfo mi;
      public object provider;
      public object[] param;
   }
}
