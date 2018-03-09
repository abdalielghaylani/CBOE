/*  GREATIS FORM DESIGNER FOR .NET
 *  Public Designer Interface Implementation
 *  Copyright (C) 2004-2008 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using Greatis.FormDesigner;

namespace Greatis
{
   namespace FormDesigner
   {
      /// <summary>
      /// Provides data for the FilterAttributes, FilterEvents, FilterProperties events.
      /// </summary>
      public struct FilterEventArgs
      {
         /// <summary>
         /// collection of the component properties, events or attributes
         /// </summary>
         public IDictionary data;

         /// <summary>
         /// If true the set of filtered events is cached, false if the filter service must query again.
         /// </summary>
         public bool caching;
      }

      /// <summary>
      /// Represents the method that will handle event for filtering component data
      /// </summary>
      /// <param name="component">Processed component</param>
      /// <param name="args">A FilterEventArgs that contains the event data.</param>
      public delegate void FilterEventHandler(IComponent component, ref FilterEventArgs args);
      
      /// <summary>
      /// Represents the method that will handle event for adding verb into local menu
      /// </summary>
      /// <param name="primarySelection">component for which local menu will be shown</param>
      /// <param name="verb">adding verb</param>
      /// <returns>true if verb can be added to local menu</returns>
      public delegate bool AddingVerbHandler(IComponent primarySelection, DesignerVerb verb);


      /// <summary>
      /// Represents the method that create components 
      /// </summary>
      /// <param name="format">The data format that the creator handles. </param>
      /// <param name="serializedObject">The object which contains the data to create a components</param>
      /// <param name="host">The IDesignerHost to host the components</param>
      /// <returns>Created components</returns>
      public delegate IComponent[] CreateComponentsCallback(string format, object serializedObject, IDesignerHost host);

      
      /// <summary>
      /// Represents the method that will handle event for allowing design component
      /// </summary>
      /// <param name="component">designed component</param>
      /// <returns>true for allowing design component</returns>
      public delegate bool AllowDesignHandler(IComponent component);

      /// <summary>
      /// Provides interface for designer events 
      /// </summary>
      public interface IDesignEvents
      {
         // ITypeDescriptorFilterService

         /// <summary>
         /// Fires when PropertyGrid shows component attributess
         /// </summary>
         event FilterEventHandler FilterAttributes;

         /// <summary>
         /// Fires when PropertyGrid shows component events
         /// </summary>
         event FilterEventHandler FilterEvents;

         /// <summary>
         /// Fires when PropertyGrid shows component properties
         /// </summary>
         event FilterEventHandler FilterProperties;

         /// <summary>
         /// Fires when designer needs to known whether component can be designed
         /// </summary>
         event AllowDesignHandler AllowDesign;

         // IMenuService
         /// <summary>
         /// Fires when designer shows local menu for selected component
         /// </summary>
         event AddingVerbHandler AddingVerb;
      }

      /// <summary>
      /// Main component of FormDesigner.Net
      /// </summary>
      [ToolboxBitmap(typeof(Designer))]
      [LicenseProvider(typeof(DesignerLP))]
      public class Designer : Component
      {
         /// <summary>
         /// Default constructor for Designer class
         /// </summary>
         public Designer()
         {
            m_license = LicenseManager.Validate(typeof(Designer), this);

            m_Active = false;

            m_Designer = new DesignerHost();
            m_Designer.Owner = this;

            m_creatorCallbacks = new Hashtable();
         }

         /// <summary>
         /// Destroy designer
         /// </summary>
         ~Designer()
         {
            Dispose();
         }

         #region GridOptions region
         /// <summary>
         /// Gets or sets a value that enables or disables smart tags in the designer
         /// </summary>
         [Description("Gets or sets a value that enables or disables smart tags in the designer."),
         DefaultValue(true)]
         public bool UseSmartTags
         {
            get
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["UseSmartTags"];
               return (bool)pd.GetValue(null);
            }
            set
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["UseSmartTags"];
               pd.SetValue(null, value);
            }
         }

         /// <summary>
         /// Gets or sets a value that specifies whether a designer shows a component's smart tag panel automatically on creation
         /// </summary>
         [Description("Gets or sets a value that specifies whether a designer shows a component's smart tag panel automatically on creation."),
         DefaultValue(true)]
         public bool ObjectBoundSmartTagAutoShow
         {
            get
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["ObjectBoundSmartTagAutoShow"];
               return (bool)pd.GetValue(null);
            }
            set
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["ObjectBoundSmartTagAutoShow"];
               pd.SetValue(null, value);
            }
         }

         /// <summary>
         /// Snaping to the grid on/off
         /// </summary>
         [Description("Snaping to the grid on/off."),
         DefaultValue(true)]
         public bool SnapToGrid
         {
            get
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["SnapToGrid"]; 
               return (bool)pd.GetValue(null);
            }
            set
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["SnapToGrid"];
               pd.SetValue(null, value);
            }
         }

         /// <summary>
         /// Show the grid on/off
         /// </summary>
         [Description("Show the grid on/off."),
         DefaultValue(true)]
         public bool ShowGrid
         {
            get
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["ShowGrid"];
               return (bool)pd.GetValue(null);
            }
            set
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["ShowGrid"];
               pd.SetValue(null, value);
            }
         }

         /// <summary>
         /// Size of the grid of the designing form
         /// </summary>
         [Description("Size of the grid of the designing form."),
         DefaultValue(0x00080008)]
         public Size GridSize
         {
            get
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["GridSize"];
               return (Size)pd.GetValue(null);
            }
            set
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["GridSize"];
               pd.SetValue(null, value);
            }
         }

         /// <summary>
         /// Using snap lines for aligning controls
         /// </summary>
         [Description("Using snap lines for aligning controls."),
         DefaultValue(true)]
         public bool UseSnapLines
         {
            get
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["UseSnapLines"];
               return (bool)pd.GetValue(null);
            }
            set
            {
               DesignerOptionService dos = (DesignerOptionService)m_Designer.Host.GetService(typeof(DesignerOptionService));
               PropertyDescriptor pd = dos.Options.Properties["UseSnapLines"];
               pd.SetValue(null, value);
            }
         }
      #endregion

         /// <summary>
         /// Registers handler for creating components
         /// <param name="format">The data format that the creator handles</param>
         /// <param name="creator">A CreateComponentsCallback that can create a components when drag and drop events occured</param>
         /// </summary>
         [Browsable(false)]
         public bool RegisterDragAndDropHandler(string format, CreateComponentsCallback creator)
         {
            IToolboxService its = (IToolboxService)m_Designer.Host.GetService(typeof(IToolboxService));
            if( its == null )
               return false;

            ToolboxItemCreatorCallback callback = new ToolboxItemCreatorCallback(CreateToolboxItem);
            its.AddCreator(callback, format);

            m_creatorCallbacks[format] = creator;

            return true;
         }

         /// <summary>
         /// Containers, that have designed components in properties except Control.
         /// In collection key is a type name and value is a property name.
         /// </summary>
         [Browsable(false)]
         public SortedList DesignedContainers
         {
            get { return m_Designer.DesignedContainers; }
         }

         /// <summary>
         /// Unregisters handler for creating components
         /// </summary>
         /// <param name="format"></param>
         public void UnregisterDragAndDropHandler(string format)
         {
            IToolboxService its = (IToolboxService)m_Designer.Host.GetService(typeof(IToolboxService));
            if( its == null )
               return;

            ToolboxItemCreatorCallback callback = new ToolboxItemCreatorCallback(CreateToolboxItem);
            its.RemoveCreator(format);
            m_creatorCallbacks.Remove(format);
         }

         private ToolboxItem CreateToolboxItem(object serializedObject, string format)
         {
            IDataObject dobj = serializedObject as IDataObject;

            CreateComponentsCallback creator = (CreateComponentsCallback)m_creatorCallbacks[format];
            if( creator != null )
               return new ToolboxItemHelper(format, serializedObject, creator);

            return null;
         }

         /// <summary>
         /// Sets or gets activity state of the designer
         /// </summary>
         [Browsable(false)]
         public bool Active
         {
            get { return m_Active; }
            set
            {
               if( m_Active != value )
               {
                  m_Active = value;
                  if( m_FormTreasury != null )
                  {
                     m_FormTreasury.DesignerHost = ( value ) ? m_Designer.Host : null;
                  }
                     
                  EnableEdit(value);
               }
            }
         }

         /// <summary>
         /// Sets designed control
         /// </summary>
         [Description("Sets designed form.")]
         public Control /*Form*/ DesignedForm
         {
            get { return m_Designer.DesignedForm; }
            set { m_Designer.DesignedForm = value; }
         }

         /// <summary>
         /// Enables or disables showing error message box on exception
         /// </summary>
         [Description("Enables or disables showing error message box on exception"),
         DefaultValue(false)]
         public bool ShowErrorMessage
         {
            get { return m_Designer.m_ErrorMessages; }
            set 
            {
               m_Designer.m_ErrorMessages = value;

               if (m_FormTreasury != null)
                  m_FormTreasury.ShowErrorMessage = value;
            }
         }

         /// <summary>
         /// Gets object implemented IDesignerHost interface 
         /// </summary>
         [Browsable(false)]
         public IDesignerHost DesignerHost
         {
            get { return m_Designer.Host; }
         }


         /// <summary>
         /// Sets and gets log file name
         /// </summary>
         [Browsable(false)]
         public string LogFile
         {
            set 
            { 
               m_Designer.LogName = value;
               if (m_FormTreasury != null) m_FormTreasury.LogFile = value;
            }
            get
            {
               return m_Designer.LogName;
            }
         }

         /// <summary>
         /// Gets dirty state of the designed control
         /// </summary>
         [Browsable(false)]
         public bool IsDirty
         {
            get { return m_IsDirty; }
         }

         /// <summary>
         /// Sets control for loading/storing form
         /// </summary>
         [Description("Sets control for loading/storing form"),
         Browsable(true)]
         public ITreasury FormTreasury
         {
            get { return m_FormTreasury; }
            set
            {
               if (m_FormTreasury != null)
                  m_FormTreasury.DesignerHost = null;

               m_FormTreasury = value;

               if (m_FormTreasury != null)
                  m_FormTreasury.ShowErrorMessage = m_Designer.m_ErrorMessages;
            }
         }

         /// <summary>
         /// Gets or sets load mode for loading form
         /// </summary>
         [Description("Gets or sets load mode for loading form"),
         DefaultValue(LoadModes.Default)]
         public LoadModes LoadMode
         {
            get { return (m_FormTreasury!=null) ? m_FormTreasury.LoadMode : LoadModes.Default; }
            set { if (m_FormTreasury!=null) m_FormTreasury.LoadMode = value; }
         }

         /// <summary>
         /// Gets or sets form content as XML
         /// </summary>
         [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
         public string LayoutXML
         {
            get
            {
               StringWriter stringWriter = new StringWriter();
               XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
               using( IWriter writer = new XMLFormWriter(xmlWriter) )
               {
                  StoreInternal(writer);
               }

               ClearDirty();
               return stringWriter.ToString();
            }
            set
            {
               XmlTextReader xmlReader = new XmlTextReader(new StringReader(value));
               using( IReader reader = new XMLFormReader(xmlReader) )
               {
                  LoadInternal(reader);
               }
               ClearDirty();
            }
         }

         private bool LoadInternal(IReader reader)
         {
            if (m_FormTreasury == null)
            {
               NoTreasuryControl();
               return false;
            }

            m_FormTreasury.Load(m_Designer.DesignedForm, reader, m_designedComponents, false);
            ClearDirty();

            return true;
         }

         /// <summary>
         /// Loads designed control from file
         /// </summary>
         /// <param name="fileName">name of the file</param>
         public void LoadFromFile(string fileName)
         {
            using( TextFormReader r = new TextFormReader(fileName) )
               LoadInternal(r);
         }

         /// <summary>
         /// Loads designed control from XmlReader
         /// </summary>
         /// <param name="reader">XML reader</param>
         public void Load(ref XmlReader reader)
         {
            using( XMLFormReader r = new XMLFormReader(reader) )
               LoadInternal(r);
         }

         /// <summary>
         /// Contanier control for external form editing. 
         /// When this property is assigned, the edited form is inserted into assigned control, 
         /// if this property is null, the form is edited on-the-fly. When DesignContainer is assigned, 
         /// the DesignControl property is used only for obtaining the top-level edited form. 
         /// </summary>
         public Control DesignContainer
         {
            set { m_Designer.DesignContainer = value; }
            get { return m_Designer.DesignContainer; }
         }

         private bool StoreInternal(IWriter writer)
         {
            if (m_FormTreasury == null)
            {
               NoTreasuryControl();
               return false;
            }

            int countComponents = 1;
            ComponentCollection components = m_Designer.Host.Container.Components;
            foreach( IComponent component in components )
            {
               if( !(component is Control) )
                  countComponents ++;
            }
            IComponent[] controls = new IComponent[countComponents];
            controls[0] = (Active && DesignContainer != null) ? m_Designer.Host.RootComponent : m_Designer.DesignedForm;

            countComponents = 1;

            // firstly store designed components 
            Hashtable dch = new Hashtable();
            if( m_designedComponents != null )
            {
               foreach(IComponent component in m_designedComponents )
               {
Array.Resize<IComponent>(ref controls, controls.Length + 1);	// added by JD
                  controls[countComponents++] = component;
                  dch[component] = true;
               }
            }

            foreach( IComponent component in components )
            {
               if ( !(component is Control) && !dch.ContainsKey(component) )
                  controls[countComponents++] = component as IComponent;
            }

            m_FormTreasury.Store(controls, writer);
            ClearDirty();

            return true;
         }

         /// <summary>
         /// Serialize designed control into XmlWriter
         /// </summary>
         /// <param name="writer">XmlWriter</param>
         public void Store(ref XmlWriter writer)
         {
            using( XMLFormWriter w = new XMLFormWriter(writer) )
               StoreInternal(w);
         }

         /// <summary>
         /// Serialize designed control into file
         /// </summary>
         /// <param name="fileName">name of the file</param>
         public void SaveToFile( string fileName )
         {
            using( TextFormWriter w = new TextFormWriter(fileName) )
               StoreInternal(w);
         }

         public void SelectAll()
         {
             ISelectionService ss = (ISelectionService)m_Designer.Host.GetService(typeof(ISelectionService));
             Control parent = m_Designer.Host.RootComponent as Control;
             ss.SetSelectedComponents(parent.Controls, SelectionTypes.Replace);
         }

         /// <summary>
         /// Copies selected controls to clipboard
         /// </summary>
         public void CopyControlsToClipboard()
         {
            if (m_FormTreasury == null)
            {
               NoTreasuryControl();
               return;
            }

            ISelectionService ss = (ISelectionService)m_Designer.Host.GetService(typeof(ISelectionService));
            ICollection cc = ss.GetSelectedComponents();
            if (cc.Count == 0)
               return;

            using (StringWriter stringWriter = new StringWriter())
            {
               IWriter writer = new TextFormWriter(stringWriter);

               Control[] controls = new Control[cc.Count];
               int idx = 0;
               foreach (IComponent component in cc)
                  controls[idx++] = component as Control;

               m_FormTreasury.Store(controls, writer);
               Clipboard.SetDataObject(stringWriter.ToString());
            }
         }

         private ArrayList pastedControls = new ArrayList();
         /// <summary>
         /// Paste controls from clipboard into designed control
         /// </summary>
         public void PasteControlsFromClipboard()
         {
            pastedControls.Clear();

            if (m_FormTreasury == null)
            {
               NoTreasuryControl();
               return;
            }

            IDataObject data = Clipboard.GetDataObject();
            if (!data.GetDataPresent(typeof(string)))
               return;

            LoadModes saveLoadMode = m_FormTreasury.LoadMode;
            // force duplicate control
            m_FormTreasury.LoadMode = LoadModes.Duplicate;

            ISelectionService ss = (ISelectionService)m_Designer.Host.GetService(typeof(ISelectionService));
            
            // Find parent control
            Control pasteParent = m_Designer.Host.RootComponent as Control;
            if (ss.SelectionCount == 1)
            {
               ICollection selected = ss.GetSelectedComponents();
               IEnumerator en = selected.GetEnumerator();
               en.MoveNext();

               Control ctrl = en.Current as Control;
               if (ctrl != null)
               {
                  IDesigner d = m_Designer.Host.GetDesigner(ctrl);
                  if (typeof(System.Windows.Forms.Design.ParentControlDesigner).IsAssignableFrom(d.GetType()))
                     pasteParent = ctrl;
               }
            }

            using (StringReader stringReader = new StringReader(data.GetData(typeof(string)) as string))
            {
               IReader reader = new TextFormReader(stringReader);

               m_FormTreasury.ComponentLoaded += new ComponentLoadedHandler(PasteControlHandler);
               m_FormTreasury.Load(m_Designer.DesignedForm, reader, null, true);
               m_FormTreasury.ComponentLoaded -= new ComponentLoadedHandler(PasteControlHandler);
            }

            foreach (Control ctrl in pastedControls)
            {
               if (pasteParent != null) ctrl.Parent = pasteParent;

               ctrl.Location = CheckLocation(m_Designer.Host.RootComponent as Control, ctrl.Location);
               if( m_Designer.Host.Container.Components[ctrl.Text] != null ) // меняем текст только дефолтный
                  ctrl.Text = ctrl.Name;
               ctrl.BringToFront();
            }

            Control[] sa = new Control[pastedControls.Count];
            pastedControls.CopyTo(sa);
            ss.SetSelectedComponents(sa, SelectionTypes.Replace);

            SetDirty();
            m_FormTreasury.LoadMode = saveLoadMode;
         }

         private Point CheckLocation(Control form, Point location)
         {
            if (form == null)
               return location;
            foreach (Control control in form.Controls)
            {
               if (control.Location == location)
               {
                  location += GridSize;
                  return CheckLocation(form, location);
               }
            }
            return location;
         }

         private void PasteControlHandler(IComponent component)
         {
            if (m_FormTreasury == null)
            {
               NoTreasuryControl();
               return;
            }

            Control cc = component as Control;
            if (cc != null)
               pastedControls.Add(cc);
         }

         /// <summary>
         /// Deletes selected controls
         /// </summary>
         public void DeleteSelected()
         {
            if( m_Active == false )
               return;

            ISelectionService ss = (ISelectionService)m_Designer.Host.GetService(typeof(ISelectionService));
            if(ss.SelectionCount == 0)
               return;

            ICollection selectedControls = ss.GetSelectedComponents();
            
            foreach( IComponent c in selectedControls )
            {
               try
               {
                  m_Designer.Host.Container.Remove(c);
                  c.Dispose();
               }
               catch
               {
               }
            }
            ss.SetSelectedComponents(null);
         }

         /// <summary>
         /// Makes selected controls same size
         /// </summary>
         /// <param name="resize">resize options</param>
         public void MakeSameSize( ResizeType resize )
         {
            if( m_Active == false )
               return;

            ISelectionService ss = (ISelectionService)m_Designer.Host.GetService(typeof(ISelectionService));
            if(ss.SelectionCount < 2)
               return;

            ICollection selectedControls = ss.GetSelectedComponents();
            IComponentChangeService ccs = (IComponentChangeService)m_Designer.Host.GetService(typeof(IComponentChangeService));
            DesignerTransaction dt = m_Designer.Host.CreateTransaction("Make Same Size");
            using(dt)
            {
               Control primary = (Control)ss.PrimarySelection;
               Point origin = primary.Location;
               Size size = primary.Size;
               //ss.SetSelectedComponents(null);
               foreach( Control c in selectedControls )
               {
                  if( c != primary )
                  {
                     Size controlSize = c.Size;

                     if( (resize & ResizeType.SameWidth) != 0 )
                        controlSize.Width = size.Width;
                     if( (resize & ResizeType.SameHeight) != 0 )
                        controlSize.Height = size.Height;

                     MemberDescriptor md = TypeDescriptor.GetProperties(c)["Size"];

                     ccs.OnComponentChanging(c, md);
                     Size oldValue = c.Size;
                     c.Size = controlSize;
                     ccs.OnComponentChanged(c, md, oldValue, controlSize);
                  }
               }
               //ss.SetSelectedComponents(selectedControls);
               
               dt.Commit();
            }
         }

         /// <summary>
         /// Align selected controls according passed parameter
         /// </summary>
         /// <param name="align">type of the alignment</param>
         public void Align( AlignType align )
         {
            if( m_Active == false )
               return;

            ISelectionService ss = (ISelectionService)m_Designer.Host.GetService(typeof(ISelectionService));
            if(ss.SelectionCount < 2)
               return;

            IComponentChangeService ccs = (IComponentChangeService)m_Designer.Host.GetService(typeof(IComponentChangeService));
            DesignerTransaction dt = m_Designer.Host.CreateTransaction("Align");
            using( dt )
            {
               ICollection selectedControls = ss.GetSelectedComponents();
               Control primary = (Control)ss.PrimarySelection;
               Point origin = primary.Location;
               Size size = primary.Size;

               //ss.SetSelectedComponents(null);
               foreach( Control c in selectedControls )
               {
                  if( c != primary )
                  {
                     Point location = c.Location;
                     if( (align & AlignType.Left) != 0 )
                        location.X = origin.X;
                     else if( (align & AlignType.Right) != 0 )
                        location.X = origin.X + size.Width - c.Size.Width;
                     else if( (align & AlignType.Center) != 0 )
                        location.X = origin.X + (size.Width - c.Size.Width)/2;

                     if( (align & AlignType.Top) != 0 )
                        location.Y = origin.Y;
                     else if( (align & AlignType.Bottom) != 0 )
                        location.Y = origin.Y + size.Height - c.Size.Height;
                     else if( (align & AlignType.Middle) != 0 )
                        location.Y = origin.Y + (size.Height - c.Size.Height)/2;

                     MemberDescriptor md = TypeDescriptor.GetProperties(c)["Location"];

                     ccs.OnComponentChanging(c, md);
                     Point oldValue = c.Location;
                     c.Location = location;
                     ccs.OnComponentChanged(c, md, oldValue, location);
                  }
               }
               //ss.SetSelectedComponents(selectedControls);

               dt.Commit();
            }
         }

         /// <summary>
         /// Provides functionality required by sites
         /// </summary>
         public override ISite Site
         {
            get { return base.Site; }
            set 
            {
               if( base.Site != value )
               {
                  base.Site = value;
               }
            }
         }

         /// <summary>
         /// Fires when undo count changed 
         /// </summary>
         public event EventHandler UndoCountChanges;

         /// <summary>
         /// Undoing last change
         /// </summary>
         /// <returns>true if operation completed</returns>
         public bool Undo()
         {
            IMenuCommandService ims = (IMenuCommandService)m_Designer.Host.GetService(typeof(IMenuCommandService));
            ims.GlobalInvoke(StandardCommands.Undo);

            return true;
         }

         /// <summary>
         /// Redoing last undo
         /// </summary>
         /// <returns>true if operation completed</returns>
         public bool Redo()
         {
            IMenuCommandService ims = (IMenuCommandService)m_Designer.Host.GetService(typeof(IMenuCommandService));
            ims.GlobalInvoke(StandardCommands.Redo);

            return true;
         }

         /// <summary>
         /// Gets count of the possible undo steps
         /// </summary>
         [Browsable(false)]
         public int UndoCount
         {
            get
            {
               int uc = 0;

               UndoEngineImpl ue = m_Designer.Host.GetService(typeof(UndoEngine)) as UndoEngineImpl;
               if (ue != null) uc = ue.UndoCount;

               return uc;
            }
         }

         /// <summary>
         /// Clear undo buffer
         /// </summary>
         public void ResetUndo()
         {
            UndoEngineImpl ue = m_Designer.Host.GetService(typeof(UndoEngine)) as UndoEngineImpl;
            if (ue != null) ue.ResetUndo();
         }

         /// <summary>
         /// Gets count of the possible redo steps
         /// </summary>
         [Browsable(false)]
         public int RedoCount
         {
            get
            {
               int rc = 0;

               UndoEngineImpl ue = m_Designer.Host.GetService(typeof(UndoEngine)) as UndoEngineImpl;
               if (ue != null) rc = ue.RedoCount;

               return rc;
            }
         }

         /// <summary>
         /// Fires when user releases a key
         /// </summary>
         public event KeyEventHandler KeyUp
         {
            add
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.KeyUp += value;
            }
            remove
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.KeyUp -= value;
            }
         }

         /// <summary>
         /// Fires when the user presses a key
         /// </summary>
         public event KeyEventHandler KeyDown
         {
            add
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.KeyDown += value;
            }
            remove
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.KeyDown -= value;
            }
         }

         /// <summary>
         /// Fires when user double-clicked mouse button
         /// </summary>
         public event EventHandler DoubleClick
         {
            add
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.DoubleClick += value;
            }
            remove
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.DoubleClick -= value;
            }
         }

         /// <summary>
         /// Fires when user releases mouse button
         /// </summary>
         public event MouseEventHandler MouseUp
         {
            add
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.MouseUp += value;
            }
            remove
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.MouseUp -= value;
            }
         }

         /// <summary>
         /// Fires when user presses mouse button
         /// </summary>
         public event MouseEventHandler MouseDown
         {
            add
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.MouseDown += value;
            }
            remove
            {
               EventFilter ef = (EventFilter)m_Designer.Host.GetService(typeof(EventFilter));
               ef.MouseDown -= value;
            }
         }

         /// <summary>
         /// Fires when deisgner trying to design component
         /// </summary>
         public event AllowDesignHandler AllowDesign
         {
            add
            {
               IDesignEvents de = (IDesignEvents)m_Designer.Host.GetService(typeof(IDesignEvents));
               de.AllowDesign += value;
            }
            remove
            {
               IDesignEvents de = (IDesignEvents)m_Designer.Host.GetService(typeof(IDesignEvents));
               de.AllowDesign -= value;
            }
         }

         /// <summary>
         /// Fires when dirty flags changed
         /// </summary>
         public event EventHandler DirtyChanged;

         /// <summary>
         /// Sets and gets array of components which must be designed
         /// </summary>
         public IComponent[] DesignedComponents
         {
            get { return m_designedComponents; }
            set { m_designedComponents = value; }
         }

         internal void UndoCountChanged(object o, EventArgs e)
         {
            if (UndoCountChanges != null) UndoCountChanges(this, new EventArgs());
         }

         private void NoTreasuryControl()
         {
            MessageBox.Show("Designer is not linked with FormTreasury control. All storing and loading features are not available.");
         }

         protected override void Dispose(bool disposing)
         {
            if( m_license != null )
            {
               m_license.Dispose();
               m_license = null;
            }
            base.Dispose (disposing);
         }

         protected void EnableEdit( bool enable )
         {
            if( enable == true )
            {
               ClearDirty();
               m_Designer.StartDesign();

               RegisterListners();
               if( m_designedComponents != null )
               {
                  IContainer container = (IContainer)m_Designer.Host.GetService(typeof(IContainer));
                  foreach( IComponent component in m_designedComponents )
                  {
                     container.Add(component);  // this is where site name is assigned, e.g. "BindingSource1"
                  }
               }
            } 
            else
            {
               m_Designer.StopDesign();
               ClearDirty();

               UnregisterListners();
            }
         }

         /// <summary>
         /// Sets dirty flag
         /// </summary>
         public void SetDirty()
         {
            m_IsDirty = true;

            if( DirtyChanged != null ) DirtyChanged(this, new EventArgs());
         }

         /// <summary>
         /// Clears dirty flag
         /// </summary>
         public void ClearDirty()
         {
            m_IsDirty = false;

            if( DirtyChanged != null ) DirtyChanged(this, new EventArgs());
         }

         private void RegisterListners()
         {
            IComponentChangeService cc = (IComponentChangeService)m_Designer.Host.GetService(typeof(IComponentChangeService));
            cc.ComponentAdded += new ComponentEventHandler(ComponentAddedOrRemoved);
            cc.ComponentChanged += new ComponentChangedEventHandler(ComponentChanged);
         }

         private void UnregisterListners()
         {
            IComponentChangeService cc = (IComponentChangeService)m_Designer.Host.GetService(typeof(IComponentChangeService));
            cc.ComponentAdded -= new ComponentEventHandler(ComponentAddedOrRemoved);
            cc.ComponentChanged -= new ComponentChangedEventHandler(ComponentChanged);
         }

         private void ComponentAddedOrRemoved( object sender, ComponentEventArgs e )
         {
            SetDirty();
         }

         private void ComponentChanged( object sender, ComponentChangedEventArgs e )
         {
            SetDirty();
         }

         private bool m_Active;
         private DesignerHost m_Designer;
         private IComponent[] m_designedComponents;

         private ITreasury m_FormTreasury = null;

         private bool m_IsDirty;
         private License m_license = null;

         private Hashtable m_creatorCallbacks;
      }

      internal class ToolboxItemHelper : ToolboxItem
      {
         private CreateComponentsCallback callback;
         private object serializedObject;
         private string format;

         public ToolboxItemHelper(string format, object serializedObject, CreateComponentsCallback callback)
         {
            this.callback = callback;
            this.format = format;
            this.serializedObject = serializedObject;
         }

         [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")] 
         protected override System.ComponentModel.IComponent[] CreateComponentsCore(System.ComponentModel.Design.IDesignerHost host)
         {
            return callback(format, serializedObject, host);
         }
      }

      /// <summary>
      /// Options for alignment selected components
      /// </summary>
      [Flags]
      public enum AlignType
      {
         /// <summary>
         /// The selected control is aligned with the left edge of the primary selected control
         /// </summary>
         Left = 0x1,

         /// <summary>
         /// The selected control is aligned with the right edge of the primary selected control
         /// </summary>
         Right = 0x2,

         /// <summary>
         /// The selected control is aligned with the center line of the primary selected control
         /// </summary>
         Center = 0x4,

         /// <summary>
         /// The selected control is aligned with the upper edge of the primary selected control
         /// </summary>
         Top = 0x8,

         /// <summary>
         /// The selected control is aligned with the middle of the primary selected control
         /// </summary>
         Middle = 0x10,

         /// <summary>
         /// The selected control is aligned with the lower edge of the primary selected control
         /// </summary>
         Bottom = 0x20
      }

      /// <summary>
      /// Options for resizing selected components
      /// </summary>
      [Flags]
      public enum ResizeType
      {
         /// <summary>
         /// Make selected controls same width with the primary selected control
         /// </summary>
         SameWidth = 0x1,

         /// <summary>
         /// Make selected controls same height with the primary selected control
         /// </summary>
         SameHeight = 0x2
      }

   }
}
