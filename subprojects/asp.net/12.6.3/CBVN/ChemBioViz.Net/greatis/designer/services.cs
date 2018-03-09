/*  GREATIS FORM DESIGNER FOR .NET
 *  Some Services Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.Design;
using System.Text.RegularExpressions;

namespace Greatis
{
   namespace FormDesigner
   {
      #region DesignerOptionService
      enum LayoutMode
      {
         SnapLines = 0,
         SnapToGrid = 1
      }

      class DesignerOptionsImpl : DesignerOptions
      {
         internal DesignerOptionsImpl()
         {
            this.GridSize = new Size(8, 8);
            this.ShowGrid = true;
            this.UseSmartTags = true;
            this.UseSnapLines = true;
            this.LayoutMode = LayoutMode.SnapLines;
            this.ObjectBoundSmartTagAutoShow = true;
            this.EnableInSituEditing = true;
            this.SnapToGrid = true;
            this.UseOptimizedCodeGeneration = false;
         }
         public LayoutMode LayoutMode
         {
            get
            {
               return layoutMode;
            }
            set { layoutMode = value; }
         }
         private LayoutMode layoutMode = LayoutMode.SnapToGrid;
      }

      class DesignerOptionServiceImpl : DesignerOptionService
      {
         protected override void PopulateOptionCollection(DesignerOptionCollection options)
         {
            if (options.Parent == null)
            {
               base.CreateOptionCollection(options, "DesignerOptions", new DesignerOptionsImpl());
            }
         }
      }
      #endregion

      #region MenuCommand Service
      class MenuCommandService : IMenuCommandService
      {
         private IDesignerHost host;
         private IDictionary commands;
         private IDictionary globalVerbs;
         private IDictionary menuItemVerb;
         private ContextMenu menu;

         internal event AddingVerbHandler AddingVerb;

         public MenuCommandService(IDesignerHost host)
         {
            this.host = host;
            commands = new Hashtable();
            globalVerbs = new Hashtable();
            menuItemVerb = new Hashtable();

            menu = new ContextMenu();
         }

         #region IMenuCommandService Members

         public void AddCommand(MenuCommand command)
         {
            if (command == null)
               throw new NullReferenceException("command");

            if (FindCommand(command.CommandID) == null)
               commands.Add(command.CommandID, command);
            else
               throw new InvalidOperationException("adding existing command");
         }

         public void RemoveCommand(MenuCommand command)
         {
            if (command == null)
               throw new NullReferenceException("command");

            commands.Remove(command.CommandID);
         }

         public MenuCommand FindCommand(CommandID commandID)
         {
            return (MenuCommand)commands[commandID];
         }

         public bool GlobalInvoke(CommandID commandID)
         {
            // firstly try found GlobalVerb
            DesignerVerb dv = globalVerbs[commandID] as DesignerVerb;
            if (dv != null)
            {
               dv.Invoke();
               return true;
            }

            // next find command in menu
            MenuCommand command = FindCommand(commandID);
            if (command != null)
            {
               command.Invoke();
               return true;
            }
            return false;
         }

         public void AddVerb(DesignerVerb verb)
         {
            if (verb == null)
               throw new NullReferenceException("verb");

            //
            // we can't add verb with existing CommandID
            // throwing exception can point this
            //
            //if( globalVerbs.Contains(verb.CommandID) == false )
            globalVerbs.Add(verb.CommandID, verb);
         }

         public void RemoveVerb(DesignerVerb verb)
         {
            if (verb == null)
               throw new NullReferenceException("verb");

            globalVerbs.Remove(verb.CommandID);
         }

         public void ShowContextMenu(CommandID menuID, int x, int y)
         {
            DesignerVerbCollection currentVerbs = Verbs;

            ISelectionService ss = (ISelectionService)host.GetService(typeof(ISelectionService));
            Control primarySelection = ss.PrimarySelection as Control;
            Control comp = (primarySelection != null) ? (Control)primarySelection : (Control)host.RootComponent;
            Point pt = comp.PointToScreen(new Point(0, 0));

            menu.MenuItems.Clear();
            menuItemVerb.Clear();
            foreach (DesignerVerb verb in currentVerbs)
            {
               if( AddingVerb != null && !AddingVerb(ss.PrimarySelection as IComponent, verb) )
                  continue;

               MenuItem menuItem = new MenuItem(verb.Text);
               menuItem.Click += new EventHandler(MenuItemClickHandler);
               menuItemVerb.Add(menuItem, verb);

               menuItem.Enabled = verb.Enabled;
               menuItem.Checked = verb.Checked;
               menu.MenuItems.Add(menuItem);
            }

            menu.Show(comp, new Point(x - pt.X, y - pt.Y));
         }

         public DesignerVerbCollection Verbs
         {
            get
            {
               DesignerVerbCollection verbs = new DesignerVerbCollection();

               foreach (DesignerVerb verb in globalVerbs.Values)
                  verbs.Add(verb);

               ISelectionService ss = (ISelectionService)host.GetService(typeof(ISelectionService));
               ICollection selectedComponents = ss.GetSelectedComponents();
               foreach (IComponent component in selectedComponents)
               {
                  IDesigner designer = host.GetDesigner(component);
                  if (designer != null)
                  {
                     DesignerVerbCollection componentVerbs = designer.Verbs;
                     if (componentVerbs != null)
                        verbs.AddRange(designer.Verbs);
                  }
               }
               return verbs;
            }
         }

         #endregion

         private void MenuItemClickHandler(object sender, EventArgs e)
         {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem != null)
            {
               DesignerVerb verb = (DesignerVerb)menuItemVerb[menuItem];
               if (verb != null)
               {
                  try { verb.Invoke(); }
                  catch { }
               }
            }
         }
      }
      #endregion

      #region TypeDescriptorFilterService
      class TypeDescriptorFilterService : ITypeDescriptorFilterService
      {
         private IDesignerHost host;
         internal event FilterEventHandler FilterAtt;
         internal event FilterEventHandler FilterEvnts;
         internal event FilterEventHandler FilterProps;

         public TypeDescriptorFilterService(IDesignerHost hst)
         {
            host = hst;
         }

         public bool FilterAttributes(IComponent component, IDictionary attributes)
         {
            bool retVal = false;
            IDesigner designer = host.GetDesigner(component);
            if (designer is IDesignerFilter)
            {
               IDesignerFilter designerFilter = (IDesignerFilter)designer;

               designerFilter.PreFilterAttributes(attributes);
               designerFilter.PostFilterAttributes(attributes);

               retVal = true;
            }

            if (FilterAtt != null && !(component is DesignSurface))
            {
               FilterEventArgs args = new FilterEventArgs();
               args.data = attributes;
               args.caching = true;

               FilterAtt(component, ref args);

               return args.caching;
            }
            return retVal;
         }

         public bool FilterEvents(IComponent component, IDictionary events)
         {
            bool retVal = false;
            IDesigner designer = host.GetDesigner(component);
            if (designer is IDesignerFilter)
            {
               IDesignerFilter designerFilter = (IDesignerFilter)designer;

               designerFilter.PreFilterEvents(events);
               designerFilter.PostFilterEvents(events);

               retVal = true;
            }

            if (FilterEvnts != null && !(component is DesignSurface))
            {
               FilterEventArgs args = new FilterEventArgs();
               args.data = events;
               args.caching = true;

               FilterEvnts(component, ref args);
               return args.caching;
            }
            return retVal;
         }

         public bool FilterProperties(IComponent component, IDictionary properties)
         {
            bool retVal = false;
            IDesigner designer = host.GetDesigner(component);
            if (designer is IDesignerFilter)
            {
               IDesignerFilter designerFilter = (IDesignerFilter)designer;

               designerFilter.PreFilterProperties(properties);
               designerFilter.PostFilterProperties(properties);

               retVal = true;
            }

            if (FilterAtt != null && !(component is DesignSurface))
            {
               FilterEventArgs args = new FilterEventArgs();
               args.data = properties;
               args.caching = true;

               FilterProps(component, ref args);
               return args.caching;
            }
            return retVal;
         }


      }
      #endregion

      #region NameService
      internal class NameService : INameCreationService
      {
         private IContainer cntr;
         public NameService(IContainer container)
         {
            cntr = container;
         }

         public string CreateName(IContainer container, Type dataType)
         {
            int i = 0;
            string name = dataType.Name;
            if (container == null)
                return name;

            while (true)
            {
               i++;
               if (container.Components[name + i.ToString()] == null)
                  break;
            }

            return name + i.ToString();
         }

         public void ValidateName(string name)
         {
            if (!IsValidName(name))
               throw new ArgumentException("Invalid name: " + name);
         }

         public bool IsValidName(string name)
         {
            // Check that we were actually passed a name, with a length
            if (name == null || name.Length == 0)
               return false;

            // First character must be a letter
            if (!Char.IsLetter(name, 0))
               return false;

            // Make sure there's nothing in the proposed name that isn't a letter or digit
			// MODIFIED BY CS (JD): allows special charactors specified within the Regex expression
             Regex allowSpecialChars = new Regex(@"^[a-zA-Z0-9.\-_,;:~,@,.,#,$,%,!,^,&,*,','\',<,>,=,+\?!\[\]\{\}\(\)]$"); 
             if(allowSpecialChars.IsMatch(name))
                return false;

            return (cntr.Components[name] == null);
         }
      }
      #endregion

      #region SelectionService
      internal class SelectionService : ISelectionService
      {
         private ISelectionService iss;
         private object root;

         public SelectionService(ISelectionService parent)
         {
            iss = parent;
         }

         public event EventHandler SelectionChanging
         {
            add { iss.SelectionChanging += value; }
            remove { iss.SelectionChanging -= value; }
         }

         public event EventHandler SelectionChanged
         {
            add { iss.SelectionChanged += value; }
            remove { iss.SelectionChanged -= value; }
         }

         public ICollection GetSelectedComponents()
         {
            return iss.GetSelectedComponents();
         }

         public object PrimarySelection
         {
            get { return iss.PrimarySelection; }
         }

         public int SelectionCount
         {
            get { return iss.SelectionCount; }
         }

         public bool GetComponentSelected(object component)
         {
            return iss.GetComponentSelected(component);
         }

         public void SetSelectedComponents(ICollection components, SelectionTypes selectionType)
         {
            if (root != null)
            {
               if (components != null)
               {
                  ArrayList array = new ArrayList();
                  foreach (object obj in components)
                  {
                      if (obj == root)
                          continue;
                      array.Add(obj);
                  }
                  if (array.Count > 0)
                  {
                     iss.SetSelectedComponents(array, selectionType);
                     return;
                  }
               }
               iss.SetSelectedComponents(null, selectionType);
            } else
               iss.SetSelectedComponents(components, selectionType);
         }

         public void SetRootComponent(object root)
         {
            this.root = root;
         }

         public void SetSelectedComponents(ICollection components)
         {
            SetSelectedComponents(components, SelectionTypes.Replace);
         }
      }
      #endregion

      #region UIService
      class UIService : IUIService
      {
         private Hashtable m_Styles = new Hashtable();
         private IDesignerHost m_Host;

         public UIService(IDesignerHost host)
         {
            m_Host = host;
         }

         public System.Windows.Forms.DialogResult ShowMessage(string message, string caption, System.Windows.Forms.MessageBoxButtons buttons)
         {
            return MessageBox.Show(GetDialogOwnerWindow(), message, caption, buttons);
         }

         void System.Windows.Forms.Design.IUIService.ShowMessage(string message, string caption)
         {
            MessageBox.Show(GetDialogOwnerWindow(), message, caption);
         }

         void System.Windows.Forms.Design.IUIService.ShowMessage(string message)
         {
            MessageBox.Show(GetDialogOwnerWindow(), message);
         }

         public bool ShowComponentEditor(object component, IWin32Window parent)
         {
            return false;
         }

         public bool CanShowComponentEditor(object component)
         {
            return false;
         }

         public IDictionary Styles
         {
            get { return m_Styles; }
         }

         public System.Windows.Forms.DialogResult ShowDialog(Form form)
         {
            try
            {
               form.ShowDialog(GetDialogOwnerWindow());
            }
            catch
            {
               return DialogResult.Cancel;
            }
            return form.DialogResult;
         }

         public bool ShowToolWindow(Guid toolWindow)
         {
            return false;
         }

         public IWin32Window GetDialogOwnerWindow()
         {
            IComponent root = m_Host.RootComponent;
            if (root == null || !(root is IWin32Window))
               return null;
            return (IWin32Window)root;
         }

         public void ShowError(Exception ex, string message)
         {
            MessageBox.Show(GetDialogOwnerWindow(), ex.Message, message);
         }

         void System.Windows.Forms.Design.IUIService.ShowError(Exception ex)
         {
            MessageBox.Show(GetDialogOwnerWindow(), ex.Message);
         }

         void System.Windows.Forms.Design.IUIService.ShowError(string message)
         {
            MessageBox.Show(GetDialogOwnerWindow(), message);
         }

         public void SetUIDirty()
         {
            DesignerHost host = m_Host as DesignerHost;
            if (host == null) return;

            host.Owner.SetDirty();
         }
      }
      #endregion

      #region UndoEngine
      class UndoEngineImpl : UndoEngine
      {
         private ArrayList m_Undo = new ArrayList();
         private int m_Pos = 0;

         public UndoEngineImpl(IServiceProvider provider)
            : base(provider)
         {
            IMenuCommandService mcs = (IMenuCommandService)provider.GetService(typeof(IMenuCommandService));

            mcs.AddCommand(new MenuCommand(new EventHandler(DoUndo), StandardCommands.Undo));
            mcs.AddCommand(new MenuCommand(new EventHandler(DoRedo), StandardCommands.Redo));
         }

         public EventHandler UndoCountChanges;

         void DoUndo(object sender, EventArgs e)
         {
            if (m_Pos > 0)
            {
               UndoEngine.UndoUnit uu = (UndoEngine.UndoUnit)m_Undo[m_Pos - 1];
               uu.Undo();
               m_Pos--;
            }
         }

         void DoRedo(object sender, EventArgs e)
         {
            if (m_Pos < m_Undo.Count)
            {
               UndoEngine.UndoUnit uu = (UndoEngine.UndoUnit)m_Undo[m_Pos];
               uu.Undo();
               m_Pos++;
            }
         }

         public int RedoCount
         {
            get { return m_Undo.Count - m_Pos; }
         }

         public int UndoCount
         {
            get { return m_Pos; }
         }

         public void ResetUndo()
         {
            m_Undo.Clear();
            m_Pos = 0;

            if (UndoCountChanges != null) UndoCountChanges(this, new EventArgs());
         }

         protected override void AddUndoUnit(UndoEngine.UndoUnit unit)
         {
            m_Undo.RemoveRange(m_Pos, m_Undo.Count - m_Pos);
            m_Undo.Add(unit);
            m_Pos = m_Undo.Count;

            if (UndoCountChanges != null) UndoCountChanges(this, new EventArgs());
         }

         protected override void DiscardUndoUnit(UndoEngine.UndoUnit unit)
         {
            m_Undo.Remove(unit);
            base.DiscardUndoUnit(unit);

            if (UndoCountChanges != null) UndoCountChanges(this, new EventArgs());
         }
      }
      #endregion
   }
}
