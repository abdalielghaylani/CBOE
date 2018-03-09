/*  GREATIS FORM DESIGNER FOR .NET
 *  Form Treasury
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System.Windows.Forms;
using Greatis.FormDesigner;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel.Design.Serialization;
using System;
using System.Reflection;
using System.Drawing;

using System.Data;

namespace Greatis
{
    namespace FormDesigner
    {
        /// <summary>
        /// Represents form serialization/deserialization
        /// </summary>
        [ToolboxBitmap(typeof(Treasury))]
        [LicenseProvider(typeof(TreasuryLP))]
        public class Treasury : Component, ITreasury, IDesignerSerializationService
        {
            private string logFile;
            private LoadModes loadMode = LoadModes.Default;
            private StoreModes storeMode = StoreModes.Default;

            // <string, ArrayList<ComponentProperty>>
            private ReferencedCollection referencedComponents = new ReferencedCollection();

            // <string, Component>
            private Hashtable loadedComponents = new Hashtable();

            // <string, ArrayList<Extender>
            private Hashtable extenders = new Hashtable();

            // <ControlBindingsCollection, ArrayList<InstanceDescriptorLoader> >
            private Hashtable lazyList = new Hashtable();

            private ArrayList initedObjects = new ArrayList();

            private License license = null;

            private Type epdType = null; // System.ComponentModel.ExtendedPropertyDescriptor

            private bool errorMessages = false;
            private bool versionWrited = false;

            private string currentVersion = "";

            private IDesignerHost designerHost;

            private const string OBJECT = "object";
            private const string NAME = "name";
            private const string ASSEMBLY = "assembly";
            private const string CONTENT = "content";
            private const string BINARY = "binary";
            private const string INSTANCE_DESCRIPTOR = "instance descriptor";
            private const string CONSTRUCTOR = "constructor";
            private const string MODE = "mode";
            private const string DATA = "Data";
            private const string PARAM = "Param";
            private const string COLLECTION = "collection";
            private const string ITEMTYPE = "itemtype";
            private const string CONTROL = "control";
            private const string REFERENCE = "reference";
            private const string VERSION = "TreasuryVersion";
            private const string OBJECT_COLLECTION = "object_collection";
            private const string PROVIDER = "provider";
            private const string PROPTYPE = "prop_type";
            private const string NULL = "null";

            private const int VERSION_ID = 1;
            private const int MAX_TRIAL_CONTROLS = 15;

#if TRIAL
         int m_loading;

         private void ShowTrialRestriction()
         {
         }
#endif
            /// <summary>
            /// Default constructor
            /// </summary>
            public Treasury()
            {
                license = LicenseManager.Validate(typeof(Treasury), this);
                DrillDown += new DrillDownHandler(DrillDownDefault);

                // get ExtendedPropertyDescriptor type
                Assembly a = Assembly.GetAssembly(typeof(System.ComponentModel.PropertyDescriptor));
                epdType = a.GetType("System.ComponentModel.ExtendedPropertyDescriptor");
            }

            protected override void Dispose(bool disposing)
            {
                if (license != null)
                {
                    license.Dispose();
                    license = null;
                }

                base.Dispose(disposing);
            }

            private void WriteToLog(string str)
            {
                if (logFile != null)
                {
                    using (StreamWriter strm = File.AppendText(logFile))
                    {
                        strm.WriteLine(str);
                    }
                }
            }

            /// <summary>
            /// Gets or sets load mode for loading form
            /// </summary>
            [Description("Gets or sets load mode for loading form"),
            DefaultValue(LoadModes.Default)]
            public LoadModes LoadMode
            {
                get { return loadMode; }
                set { loadMode = value; }
            }

            /// <summary>
            /// Gets or sets load mode for storing form
            /// </summary>
            [Description("Control storing components on form"),
            DefaultValue(StoreModes.Default)]
            public StoreModes StoreMode
            {
                get { return storeMode; }
                set { storeMode = value; }
            }

            /// <summary>
            /// Enables or disables showing error message box on exception
            /// </summary>
            [Description("Enables or disables showing error message box on exception"),
            DefaultValue(false)]
            public bool ShowErrorMessage
            {
                get { return errorMessages; }
                set { errorMessages = value; }
            }

            /// <summary>
            /// Loads controls into parent from passed reader
            /// </summary>
            /// <param name="parent">parent for loaded controls</param>
            /// <param name="reader">reader object containing the data</param>
            public void Load(Control parent, IReader reader)
            {
                Load(parent, reader, null, false);
            }

            /// <summary>
            /// Sets parent properties and loads controls into parent from passed reader
            /// </summary>
            /// <param name="parent">parent for loaded controls</param>
            /// <param name="reader">reader object containing the data</param>
            public void LoadRoot(Control parent, IReader reader)
            {
                Load(parent, reader, null, true);
            }

            /// <summary>
            /// Loads controls into parent from passed layout
            /// </summary>
            /// <param name="parent">parent for loaded controls</param>
            /// <param name="layout">string containing the data</param>
            public void Load(Control parent, string layout)
            {
                if (layout.Length == 0)
                {
                    throw (new ArgumentException());
                }

                MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(layout));
                XMLFormReader reader = new XMLFormReader(stream);
                Load(parent, reader, null, false);
                reader.Dispose();
            }

            /// <summary>
            /// Sets parent properties and loads controls into parent from passed layout
            /// </summary>
            /// <param name="parent">parent for loaded controls</param>
            /// <param name="layout">string containing the data</param>
            public void LoadRoot(Control parent, string layout)
            {
                if (layout.Length == 0)
                {
                    throw (new ArgumentException());
                }

                MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(layout));
                XMLFormReader reader = new XMLFormReader(stream);
                Load(parent, reader, null, true);
                reader.Dispose();
            }

            /// <summary>
            /// Serializes objects with embeded controls into string
            /// </summary>
            /// <param name="parents">collection of the parent controls</param>
            /// <returns>serialized string</returns>
            public string Store(Control[] parents)
            {
                string layout = "";

                MemoryStream stream = new MemoryStream();
                XMLFormWriter writer = new XMLFormWriter(stream);

                Treasury treasury = new Treasury();

                Store(parents, writer);
                layout = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                writer.Dispose();

                return layout;
            }

            private bool DrillDownDefault(IComponent control)
            {
#if V2
                if (control is BindingNavigator)
                    return false;
#endif
                if (!(control is ContainerControl))
                    return true;
                if (control is Form)
                    return true;
                if (control.GetType().ToString().IndexOf("DesignSurface") != -1)
                    return true;

                return false;
            }

            //
            // in some cases in desgin time we need change top control (e.g. Form) to the rootControl
            //
            private Control SubstRoot(Control rootControl, Control control)
            {
                if (rootControl == null || rootControl == control)
                    return control;

                //
                // if rootControl reachable from control, then control lays on the desing surface
                // and we can return the control
                //
                Control parent = control.Parent;
                while (parent != null)
                {
                    if (parent == rootControl)
                        return control;
                    parent = parent.Parent;
                }

                //
                // we can't reach rootControl from control, then control above rootControl
                // need return rootControl as topControl
                //
                return rootControl;
            }

            private bool CanConvert(TypeConverter cnv, Type type)
            {
                return (cnv.CanConvertFrom(type) && cnv.CanConvertTo(type));
            }

            internal void BeforeWriting()
            {
                versionWrited = false;
            }

            internal void AfterWriting()
            {
                versionWrited = false;
            }

            private string GetObjectName(object control)
            {
                if (control is Control)
                    return ((Control)control).Name;

                if (designerHost != null)
                {
                    INameCreationService ncs = (INameCreationService)designerHost.GetService(typeof(INameCreationService));
                    IContainer cntr = (IContainer)designerHost.GetService(typeof(IContainer));

                    return ncs.CreateName(cntr, control.GetType());
                }

                if (control is DataTable)
                    return ((DataTable)control).TableName;

                if (control is DataColumn)
                    return ((DataColumn)control).ColumnName;

                return "";
            }

            private void StoreHead(object control, IWriter writer)
            {
                Hashtable attributes = new Hashtable();

                string name = "";
                if (control is IComponent && ((IComponent)control).Site != null)
                    name = ((IComponent)control).Site.Name;
                if (name == "")
                    name = GetObjectName(control);

                attributes[NAME] = name;
                attributes[ASSEMBLY] = control.GetType().AssemblyQualifiedName;

                if (versionWrited == false)
                {
                    attributes[VERSION] = VERSION_ID.ToString();
                    versionWrited = true;
                }
                writer.WriteStartElement(OBJECT, attributes);
            }

            private void StoreBinary(string name, byte[] value, IWriter writer, IComponent provider)
            {
                Hashtable attributes = new Hashtable();
                attributes[MODE] = BINARY;
                if (provider != null)
                {
                    attributes[PROVIDER] = provider.Site.Name;
                    attributes[PROPTYPE] = value.GetType().AssemblyQualifiedName;
                }

                writer.WriteValue(name, Convert.ToBase64String(value), attributes);
            }

            private void StoreInstanceDescriptor(string name, InstanceDescriptor id, object value, Control rootControl, IWriter writer, IComponent provider)
            {
                Hashtable attributes = new Hashtable();

                if (provider != null)
                {
                    attributes[PROVIDER] = provider.Site.Name;
                    attributes[PROPTYPE] = value.GetType().AssemblyQualifiedName;
                }

                if (id.Arguments.Count == 0 && id.MemberInfo.Name == ".ctor")
                {
                    attributes[MODE] = CONSTRUCTOR;
                    writer.WriteStartElement(name, attributes);
                    writer.WriteValue(ASSEMBLY, value.GetType().AssemblyQualifiedName, null);
                }
                else
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, id.MemberInfo);
                    string val = Convert.ToBase64String(stream.ToArray());

                    attributes[MODE] = INSTANCE_DESCRIPTOR;

                    writer.WriteStartElement(name, attributes);
                    writer.WriteValue(DATA, val, null);

                    foreach (object arg in id.Arguments)
                    {
                        if (arg == null)
                        {
                            attributes.Clear();
                            attributes[NULL] = "1";
                            writer.WriteValue(PARAM, "", attributes);
                        }
                        else
                            StoreValue(PARAM, arg, rootControl, writer);
                    }
                }


                if (!id.IsComplete)
                    StoreProperties(value, rootControl, writer);
                writer.WriteEndElement(name);
            }

            private int StoreValue(string name, object value, Control rootControl, IWriter writer)
            {
                return StoreValue(name, value, rootControl, writer, null);
            }

            private int StoreValue(string name, object value, Control rootControl, IWriter writer, IComponent provider)
            {
                if (value == null)
                    return 0;

                TypeConverter converter = TypeDescriptor.GetConverter(value);
                if (value is IComponent)
                {
                    if (((IComponent)value).Site != null && ((IComponent)value).Site.Container == designerHost)
                    {
                        Hashtable attributes = new Hashtable();
                        attributes[MODE] = REFERENCE;
                        writer.WriteValue(name, ((IComponent)value).Site.Name, attributes);
                    }
                    else
                    {
                        ISite isite = ((IComponent)value).Site;
                        StoreObjectAsProperty(name, value, rootControl, writer);
                    }
                }
                else if (CanConvert(converter, typeof(string)))
                {
                    string strValue = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
                    Hashtable attributes = null;
                    if (provider != null)
                    {
                        attributes = new Hashtable();
                        attributes[PROVIDER] = provider.Site.Name;
                        attributes[PROPTYPE] = value.GetType().AssemblyQualifiedName;
                    }
                    writer.WriteValue(name, strValue, attributes);
                }
                else if (CanConvert(converter, typeof(byte[])))
                {
                    try
                    {
                        byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));
                        StoreBinary(name, data, writer, provider);
                    }
                    catch
                    {
                    }
                }
                else if (CanConvert(converter, typeof(InstanceDescriptor)))
                {
                    InstanceDescriptor id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));
                    StoreInstanceDescriptor(name, id, value, rootControl, writer, provider);
                }
                else if (value.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, value);
                    StoreBinary(name, stream.ToArray(), writer, provider);
                }
                else if (value is IList)
                    return StoreList(name, (IList)value, rootControl, writer);
                else
                    return StoreObjectAsProperty(name, value, rootControl, writer);
                return 1;
            }

            private int StoreList(string name, IList value, Control rootControl, IWriter writer)
            {
                if (value.Count == 0)
                    return 0;

                Hashtable attributes = new Hashtable();
                attributes[COLLECTION] = "true";
                attributes[ITEMTYPE] = value[0].GetType().AssemblyQualifiedName;

                ILazyWrite lw = writer as ILazyWrite;
                if (lw != null)
                    lw.Begin();

                writer.WriteStartElement(name, attributes);

                int ctr = 0, stored = 0;
                foreach (object el in value)
                {
                    stored += StoreValue("Item" + ctr.ToString(), el, rootControl, writer);
                    ctr++;
                }
                writer.WriteEndElement(name);
                if (lw != null)
                    lw.End(stored == 0);

                return stored;
            }

            private int StoreObjectAsProperty(string propName, object value, Control rootControl, IWriter writer)
            {
                Hashtable attributes = new Hashtable();
                attributes[CONTROL] = "true";

                ILazyWrite lw = writer as ILazyWrite;
                if (lw != null)
                    lw.Begin();

                writer.WriteStartElement(propName, attributes);
#if TRIAL
            int saveCount = m_loading;
            m_loading = 0;
#endif
                int controlStored = StoreControl(value, rootControl, writer);
#if TRIAL
            m_loading = saveCount;
#endif
                writer.WriteEndElement(propName);

                if (lw != null)
                    lw.End(controlStored == 0);

                return controlStored;
            }

            internal int StoreMember(object control, PropertyDescriptor prop, Control rootControl, IWriter writer)
            {
                int stored = 0;

                object value = null;
                DesignerSerializationVisibilityAttribute visibility =
                   (DesignerSerializationVisibilityAttribute)prop.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
                switch (visibility.Visibility)
                {
                    case DesignerSerializationVisibility.Visible:
                        {
                            IComponent provider = null;
                            if (prop.Name == "Visible" && control is Control && (control as Control).Parent.Visible == false)
                            {
                                //PropertyDescriptorCollection pc = TypeDescriptor.GetProperties(control);
                                value = prop.GetValue(control);
                                /*
                                  value = typeof(Control).InvokeMember("GetState",
                                  BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                                  null, control, new object[] { 2 });
                                */
                            }
                            else
                            {
                                if (epdType.IsAssignableFrom(prop.GetType()))
                                {
                                    provider = prop.GetType().InvokeMember("provider",
                                       BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                                       null, prop, null) as IComponent;

                                    if (provider != null && provider.Site == null)
                                        provider = null;
                                }
                                value = prop.GetValue(control);
                            }

                            if (prop.IsReadOnly == false)
                                stored += StoreValue(prop.Name, value, rootControl, writer, provider);
                            break;
                        }
                    case DesignerSerializationVisibility.Content:
                        value = prop.GetValue(control);
                        if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                            stored += StoreList(prop.Name, (IList)value, rootControl, writer);
                        else if (IsDataCollection(prop.PropertyType))
                        {
                            PropertyInfo pi = value.GetType().GetProperty("List", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
                            IList list = (IList)pi.GetValue(value, new object[] { });
                            stored += StoreList(prop.Name, list, rootControl, writer);
                        }
                        else
                            /*if (value is Control)*/
                            stored += StoreObjectAsProperty(prop.Name, value, rootControl, writer);
                        break;
                    default:
                        break;
                }
                return stored;
            }

            // return stored properties
            private int StoreProperties(object control, Control rootControl, IWriter writer)
            {
                int stored = 0;
                PropertyDescriptorCollection pd = TypeDescriptor.GetProperties(control);
                foreach (PropertyDescriptor prop in pd)
                {
                    try
                    {
                        // serialize all properties or values, thoes changes from the defaul
                        if ((storeMode != StoreModes.AllProperties &&
                              prop.ShouldSerializeValue(control) != true) ||
                             prop.Name == "Controls")
                        {
                            continue;
                        }
                        stored += StoreMember(control, prop, rootControl, writer);
                    }
                    catch (Exception e)
                    {
                        string message = "Exception while store property " + prop.Name + "\n" + e.Message;
                        if (errorMessages)
                            MessageBox.Show(message);
                        WriteToLog(message);
                    }
                }
                return stored;
            }

            private void StoreTail(object control, IWriter writer)
            {
                writer.WriteEndElement(OBJECT);
            }

            internal int StoreControl(object control, Control rootControl, IWriter writer)
            {
#if TRIAL
            m_loading++;
            if( m_loading > MAX_TRIAL_CONTROLS )
            {
               ShowTrialRestriction();
               return 0;
            }
#endif
                int stored = 0;
                StoreHead(control, writer);
                stored += StoreProperties(control, rootControl, writer);

                Control currentControl = control as Control;
                if (/*rootControl != null &&*/ currentControl != null)
                {
                    bool needDrill = false;
                    foreach (DrillDownHandler d in drillList)
                    {
                        if (d(currentControl))
                        {
                            needDrill = true;
                            break;
                        }
                    }

                    if (needDrill)
                    {
                        Control subst = SubstRoot(rootControl, currentControl);

                        foreach (Control child in subst.Controls)
                        {
#if TRIAL
                     if( m_loading > MAX_TRIAL_CONTROLS )
                     {
                        StoreTail(control, writer);
                        ShowTrialRestriction();
                        return stored;
                     }
#endif

                            if (designerHost != null)
                            {
                                if (child.Site == null || child.Site.GetService(typeof(IDesignerHost)) != designerHost)
                                    continue;
                            }

                            stored += StoreControl(child, rootControl, writer);
                        }
                    }
                }

                StoreTail(control, writer);
                return stored;
            }

            private Type CreateType(string type)
            {
                int sep = type.IndexOf(",");
                string typeName = type.Substring(0, sep);
                string assemblyName = type.Substring(sep + 1).Trim();

                Assembly a;
                WriteToLog("Load Assembly " + assemblyName);
                try
                {
                    a = Assembly.Load(assemblyName);
                }
                catch (Exception e)
                {
                    WriteToLog("Exception while loading assembly: " + e.Message);
                    a = null;
                }
                if (a == null)
                {
                    if (errorMessages == true)
                        MessageBox.Show("Error create type " + type + "\nCan't load assembly " + assemblyName);
                    return null;
                }

                return a.GetType(typeName);
            }

            private object CreateObject(string type, string name, bool createComponent)
            {
                Type controlType = CreateType(type);
                if (controlType == null) return null;

                object o = null;
                if (designerHost != null && createComponent)
                    o = designerHost.CreateComponent(controlType, name);

                if (o == null)
                {
                    WriteToLog("Creating component " + controlType.ToString());
                    try
                    {
                        o = Activator.CreateInstance(controlType);
                        if (o is Control)
                            ((Control)o).Name = name;
                    }
                    catch (Exception e)
                    {
                        if (errorMessages == true)
                            MessageBox.Show("Exception in Activator.CreateInstance(" + controlType.ToString() + ")\n" + e.Message);
                        WriteToLog("Exception while creating component " + e.Message);
                        o = null;
                    }
                    if (o != null)
                        WriteToLog("Component created");
                }

                if (o != null && name != null && o is IComponent)
                {
                    if (loadedComponents.ContainsKey(name) == false)
                        loadedComponents.Add(name, o);
                }
                if (o != null && o is ISupportInitialize)
                {
                    initedObjects.Add(o);
                    ((ISupportInitialize)o).BeginInit();
                }
                return o;
            }

            private Control FindControl(Control.ControlCollection cc, string name)
            {
                foreach (Control c in cc)
                {
                    if (c.Name == name)
                        return c;
                }
                return null;
            }

            private bool SkipControl(IReader reader, bool readNext)
            {
                int i = 1;
                while (true)
                {
                    if (reader.Read() == false)
                        return false;

                    if (reader.State == ReaderState.EndElement)
                    {
                        if (i == 1)
                            return (readNext) ? reader.Read() : true;
                        i--;
                    }
                    else if (reader.State == ReaderState.StartElement)
                        i++;
                }
            }

            private Control InstantinateControl(Control parent, IReader reader)
            {
                string name, type;
                type = (string)reader.Attributes[ASSEMBLY];
                name = (string)reader.Attributes[NAME];
                if (name == null)
                    name = reader.Name;

                Control curctrl = null;

                if (loadMode == LoadModes.Duplicate)
                {
                    // in design-time creating control with unique name
                    // in run-time names of the controls may be same
                    if (type != null)
                        curctrl = (Control)CreateObject(type, (designerHost == null) ? name : null, true);
                }
                else
                {
                    if (parent != null && name != null && name != "")
                        curctrl = FindControl(parent.Controls, name);
                    if (curctrl == null)
                    {
                        if (loadMode != LoadModes.ModifyExisting)
                            curctrl = (Control)CreateObject(type, name, true);
                    }
                }
                return curctrl;
            }

            private object LoadStrings(IReader reader, int length)
            {
                String[] strings = new String[length];

                for (int index = 0; index < length; index++)
                {
                    reader.Read();
                    strings.SetValue(reader.Value, index);
                }

                while (reader.State != ReaderState.EndElement)
                    if (reader.Read() == false)
                        break;

                return strings;
            }

            private String loadedObjectName;
            private object LoadObject(IReader reader, object c, bool readName)
            {
                string name=string.Empty, type;
                type = (string)reader.Attributes[ASSEMBLY];
                if (readName)
                {
                    name = (string)reader.Attributes[NAME];
                    if (name == null)
                        name = reader.Name;

                }
                else
                {
                    c = CreateObject(type, null, true);
                    //Coverity Bug Fix CID 11431 
                    if (c is IComponent)
                        name = (c as IComponent).Site.Name;
                }


                loadedObjectName = name;
                if (type.IndexOf("String[]") != -1)
                {
                    object length = reader.Attributes["Length"];
                    if (length != null)
                        return LoadStrings(reader, Convert.ToInt32(length));
                }

                if (c == null)
                {
                    if (name != "")
                        c = FindComponent(name);
                    if (c == null)
                        c = CreateObject(type, name, true);
                }
                else
                {
                    if (c is ISupportInitialize)
                    {
                        initedObjects.Add(c);
                        ((ISupportInitialize)c).BeginInit();
                    }
                }

                if (name != null && name != "" && c is IComponent)
                {
                    if (loadedComponents.ContainsKey(name) == false)
                        loadedComponents.Add(name, c);
                }

                LoadProperties(c, reader);
                return c;
            }

            private void LoadControls(Stack owners, IReader reader)
            {
                while (true)
                {
                    if (reader.State == ReaderState.StartElement)
                    {
#if TRIAL
                  m_loading++;
                  if( m_loading < MAX_TRIAL_CONTROLS )
                  {
#endif
                        Control parent = (Control)owners.Peek();

                        Control curctrl = InstantinateControl(parent, reader);
                        if (curctrl == null)
                        {
                            if (SkipControl(reader, true) == false)
                                break;
                            continue;
                        }
                        LoadProperties(curctrl, reader);

                        parent.Controls.Add(curctrl);
                        owners.Push(curctrl);

#if TRIAL
                  } else
                  {
                     ShowTrialRestriction();
                     return;
                  }
#endif
                    }
                    else if (reader.State == ReaderState.EndElement)
                    {
#if TRIAL
                  if( owners.Count != 0 )
#endif
                        owners.Pop();
                        if (reader.Read() == false)
                            break;
                        if (owners.Count == 0)
                            break;
                    }
                    else
                        if (reader.Read() == false)
                            break;
                }
            }

            private object LoadBinary(TypeConverter converter, IReader reader)
            {
                object value = null;
                byte[] data = (byte[])Convert.FromBase64String(reader.Value);
                if (CanConvert(converter, typeof(byte[])))
                    value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, data);
                else
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream(data);
                    value = formatter.Deserialize(stream);
                }
                return value;
            }

            private object LoadConstructor(IReader reader)
            {
                while (true)
                {
                    if (reader.Read() == false)
                        return null;
                    if (reader.State == ReaderState.Value && reader.Name == ASSEMBLY)
                        break;
                }

                object instance = CreateObject(reader.Value, null, false);
                //LoadInstanceProperties(reader, instance);
                LoadProperties(instance, reader);
                return instance;
            }

            private object LoadInstanceDescriptor(IReader reader, bool needLazyLoad)
            {
                while (true)
                {
                    if (reader.Read() == false)
                        return null;
                    if (reader.State == ReaderState.Value && reader.Name == DATA)
                        break;
                }

                byte[] data = Convert.FromBase64String(reader.Value);
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);
                MemberInfo mi = (MemberInfo)formatter.Deserialize(stream);
                object[] args = null;
                bool lazyLoad = false;

                if (mi is MethodBase)
                {
                    ParameterInfo[] paramInfos = ((MethodBase)mi).GetParameters();
                    args = new object[paramInfos.Length];
                    int idx = 0;

                    while (idx < paramInfos.Length)
                    {
                        if (reader.Read() == false)
                            return null;

                        if (reader.State == ReaderState.Value && reader.Name == PARAM)
                        {
                            if (reader.Attributes.Count != 0)
                            {
                                string mode = (string)reader.Attributes[MODE];
                                if (mode != null && mode == REFERENCE)
                                {
                                    lazyLoad = true;
                                    LazyParam lp = new LazyParam();
                                    lp.name = reader.Value;
                                    args[idx++] = lp;

                                    continue;
                                }
                            }
                            object value = (reader.Attributes[NULL] != null) ? null : LoadValue(paramInfos[idx].ParameterType, null, null, reader);
                            args[idx++] = value;
                        }
                        if (idx == paramInfos.Length)
                            break;
                    }
                }

                if (lazyLoad || needLazyLoad)
                {
                    InstanceDescriptorLoader idl = new InstanceDescriptorLoader();
                    idl.memberInfo = mi;
                    idl.args = args;

                    int level = 0;
                    do
                    {
                        if (reader.Read() == false)
                            break;
                        if (reader.State == ReaderState.StartElement)
                            level++;
                        else if (reader.State == ReaderState.EndElement)
                        {
                            if (level == 0)
                                break;
                            else
                                level--;
                        }
                    } while (true);
                    return idl;
                }
                InstanceDescriptor id = new InstanceDescriptor(mi, args);
                try
                {
                    object instance = id.Invoke();

                    LoadProperties(instance, reader);
                    return instance;
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            private void LoadList(IList list, string typeName, Object control, IReader reader, PropertyDescriptor pd)
            {
                // loading list elements
                while (true)
                {
                    object tval = null;
                    // reading only text
                    if (reader.Read() == false)
                        break;
                    if (reader.State == ReaderState.EndElement)
                        break;
                    //if (reader.State == ReaderState.Value)
                    {
                        Type valueType;
                        if (typeName != null)
                            valueType = Type.GetType(typeName);
                        else
                        {
                            string elementType = (string)reader.Attributes[ITEMTYPE];
                            if (elementType == null)
                                continue;
                            valueType = Type.GetType(elementType);
                        }
                        if (typeof(IList).IsAssignableFrom(valueType))
                        {
                            tval = Activator.CreateInstance(valueType);
                            LoadList((IList)tval, (string)reader.Attributes[ITEMTYPE], null, reader, null);
                        }
                        else
                            tval = LoadValue(valueType, pd, control, reader);
                    }

                    if (tval != null)
                    {
                        InstanceDescriptorLoader idl = tval as InstanceDescriptorLoader;
                        if (idl != null)
                        {
                            object v = pd.GetValue(control);
                            if (IsDataCollection(v.GetType()))
                            {
                                if (lazyList.ContainsKey(v) == false)
                                    lazyList.Add(v, new ArrayList());
                                ((ArrayList)lazyList[v]).Add(idl);
                            }
                            //if (lazyList.ContainsKey(list) == false)
                            //   lazyList.Add(list, new ArrayList());
                            //((ArrayList)lazyList[list]).Add(idl);
                        }
                        else
                        {
                            try
                            {
                                list.Add(tval);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            private object LoadCollectionItem(IReader reader)
            {
                bool forwardReader = reader.Name.StartsWith("Item");

                // move to object def
                if (forwardReader)
                    reader.Read();

                object o = LoadObject(reader, null, true);

                // eat end tag
                if (forwardReader)
                    reader.Read();

                return o;
            }

            // remove all, except desgined items
            private void ClearList(IList list)
            {
                if (designerHost == null)
                {
                    try
                    {
                        list.Clear();
                    }
                    catch
                    {
                    }

                    return;
                }

                ArrayList removed = new ArrayList();
                foreach (object el in list)
                {
                    IComponent comp = el as IComponent;
                    if (comp == null || comp.Site == null || comp.Site.Container != designerHost)
                        removed.Add(comp);
                }

                foreach (object el in removed)
                {
                    list.Remove(el);
                }
            }

            private object LoadValue(Type propType, PropertyDescriptor p, object control, IReader reader)
            {
                // check collection
                string name = reader.Name;
                object value = null;

                int count = reader.Attributes.Count;
                if (count != 0 && reader.Attributes.ContainsKey(PROVIDER)) count--;
                if (count != 0 && reader.Attributes.ContainsKey(PROPTYPE)) count--;

                if (count != 0)
                {
                    string mode = (string)reader.Attributes[MODE];
                    if (mode != null)
                    {
                        if (mode == BINARY)
                            value = LoadBinary(TypeDescriptor.GetConverter(propType), reader);
                        else if (mode == INSTANCE_DESCRIPTOR)
                            value = LoadInstanceDescriptor(reader, LazyLoadInstance(propType));
                        else if (mode == CONSTRUCTOR)
                            value = LoadConstructor(reader);
                        else if (mode == REFERENCE) // save reference on external component for resolving after loading
                        {
                            if (control != null)
                            {
                                if (IsDataCollection(p.PropertyType))
                                {
                                    object v = p.GetValue(control);
                                    if (lazyList.ContainsKey(v) == false)
                                        lazyList.Add(v, new ArrayList());
                                    ((ArrayList)lazyList[v]).Add(reader.Value);
                                }
                                else
                                {
                                    ComponentProperty compProp = (typeof(IList).IsAssignableFrom(p.PropertyType)) ?
                                          new ListProperty(control, p) :
                                          new ComponentProperty(control, p);

                                    referencedComponents.Add(reader.Value, compProp);
                                }
                            }
                        }
                    }
                    else if (reader.Attributes[CONTENT] != null)
                    {
                        PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(control);
                        PropertyDescriptor pd = pdc.Find(name, false);
                        value = pd.GetValue(control);
                        LoadProperties(value, reader);
                    }
                    else if (reader.Attributes[COLLECTION] != null)
                    {
                        // check for empty collection
                        if (control == null || p == null || reader.State != ReaderState.StartElement)
                            return null;

                        string typeName = (string)reader.Attributes[ITEMTYPE];
                        object val = p.GetValue(control);

                        IList list = val as IList;
                        if (list == null)
                        {
                            PropertyInfo pi = val.GetType().GetProperty("List", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
                            if (pi != null)
                                list = (IList)pi.GetValue(val, new object[] { });
                        }
                        if (list != null)
                        {
                            ClearList(list);
                            LoadList(list, typeName, control, reader, p);
                        }
                    }
                    else if (reader.Attributes[CONTROL] != null)
                    {
                        Stack owners = new Stack();
                        bool assignValue = false;

                        // if element look like <el> <object .../> </el>
                        // the reader's state is value (reader eat object)
                        if (reader.State == ReaderState.Value)
                            return null;

                        //
                        // check if LoadValue called from LoadList, then return created control
                        //
                        if (typeof(ICollection).IsAssignableFrom(p.PropertyType))
                            return LoadCollectionItem(reader);

                        value = p.GetValue(control);
                        if (value != null)
                        {
                            reader.Read(); // skip object definition
                            if (reader.State != ReaderState.Value) // if it isn't empty element
                                LoadProperties(value, reader);
                        }
                        else
                        {
                            // creating control
                            value = InstantinateControl(null, reader);
                            assignValue = true;
                        }
                        if (value is Control)
                        {
                            owners.Push(value);
                            LoadControls(owners, reader);
                        }
                        else
                            reader.Read(); // skip end element

                        if (assignValue)
                            p.SetValue(control, value);
                        return null;
                    }
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(propType);
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        try
                        {
                            if (currentVersion == null)
                                value = converter.ConvertFrom(null, CultureInfo.CurrentCulture, reader.Value);
                            else
                                value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, reader.Value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    if (value == null && propType == typeof(Object))
                        value = reader.Value;
                }
                return value;
            }

            private void LoadProperties(object control, IReader reader)
            {
                string node = reader.Name;
                PropertyDescriptorCollection pd = TypeDescriptor.GetProperties(control);

                while (true)
                {
                    if (reader.Read() == false)
                        break;

                    // check for end node & child controls
                    bool nameAttribute = reader.Attributes.ContainsKey(NAME);
                    if (reader.State == ReaderState.EndElement || nameAttribute)
                        break;

                    string name = reader.Name;
                    if (name == "Controls") // work around
                        continue;

                    Control cc = control as Control;
                    if (name == "Name" && cc != null && cc.Name != "")
                        continue;

                    PropertyDescriptor p = pd.Find(name, false);
                    if (p != null)
                    {
                        object value = LoadValue(p.PropertyType, p, control, reader);
                        if (value != null)
                        {
                            try
                            {
                                p.SetValue(control, value);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        bool loaded = false;
                        string provider = (string)reader.Attributes[PROVIDER];
                        string type = (string)reader.Attributes[PROPTYPE];
                        string propName = "Set" + reader.Name;
                        if (provider != null && type != null)
                        {
                            Type propType = CreateType(type);
                            if (propType != null)
                            {
                                loaded = true;
                                object value = LoadValue(propType, null, control, reader);
                                if (value != null)
                                {
                                    ArrayList al = extenders[provider] as ArrayList;
                                    if (al == null) al = new ArrayList();

                                    Extender e = new Extender();
                                    e.control = control;
                                    e.value = value;
                                    e.property = propName;
                                    al.Add(e);

                                    extenders[provider] = al;
                                }
                            }
                        }

                        if (!loaded && reader.State == ReaderState.StartElement)
                            SkipControl(reader, false);
                    }
                }
                if (ComponentLoaded != null && control is IComponent)
                    ComponentLoaded(control as IComponent);
            }

            private bool HaveParentInList(Control control, Hashtable parentsList)
            {
                if (control == null)
                    return false;

                Control parent = control.Parent;
                while (parent != null)
                {
                    if (parentsList[parent] != null)
                        return true;
                    parent = parent.Parent;
                }
                return false;
            }

            private IComponent FindComponent(string name)
            {
                if (designerHost == null || name == null) return null;

                IContainer ic = (IContainer)designerHost.GetService(typeof(IContainer));
                foreach (IComponent c in ic.Components)
                {
                    if (c.Site.Name == name)
                        return c;
                }
                return null;
            }

            #region ITreasury Members
            /// <summary>
            /// Gets or sets designer host
            /// </summary>
            public IDesignerHost DesignerHost
            {
                get { return designerHost; }
                set
                {
                    if (designerHost != null)
                    {
                        designerHost.RemoveService(typeof(IDesignerSerializationService));
                        // remove in new version
                        //#if V2
                        //                  designerHost.RemoveService(typeof(ComponentSerializationService));
                        //#endif
                        // remove in new version
                    }

                    designerHost = value;

                    if (designerHost != null)
                    {
                        designerHost.AddService(typeof(IDesignerSerializationService), this);
                        // remove in new version
                        //#if V2
                        //                  designerHost.AddService(typeof(ComponentSerializationService), new ComponentSerializationServiceImpl(this));
                        //#endif
                        // remove in new version
                    }
                }
            }

            /// <summary>
            /// Loads controls into parent from passed reader
            /// </summary>
            /// <param name="parent">parent for loaded controls</param>
            /// <param name="reader">reader object containing the data</param>
            /// <param name="components">array of the loaded components</param>
            /// <param name="ignoreParent">if true, supresses loading parent properties</param>
            public void Load(Control parent, IReader reader, IComponent[] components, bool ignoreParent)
            {
                if (parent == null)
                {
                    // throw new ArgumentNullException("parent");
                    return;
                }

                initedObjects.Clear();

                Control rootControl = null;
                if (designerHost != null)
                {
                    rootControl = designerHost.RootComponent as Control;
                    WriteToLog("Design-time loading mode");
                }

                reader.Read();
                currentVersion = (string)reader.Attributes[VERSION];

                if (reader.Name == OBJECT_COLLECTION) // move to form definition
                    reader.Read();

                if (ignoreParent == false)
                    LoadProperties(parent, reader);
                parent = SubstRoot(rootControl, parent);

                Stack owners = new Stack();
                owners.Push(parent);

                loadedComponents.Clear();
                referencedComponents.Clear();
                lazyList.Clear();

                PrepareParent(reader, ignoreParent, parent);

#if TRIAL
            m_loading = 0;
#endif
                LoadControls(owners, reader);

                LoadComponents(reader, components);

                SetReferences();

                SetExtendProviders();

                InvokeAddRange();

                foreach (ISupportInitialize objects in initedObjects)
                    objects.EndInit();

                AddBindings();

                initedObjects.Clear();
            }

            private void SetExtendProviders()
            {
                foreach (DictionaryEntry de in extenders)
                {
                    object extender = loadedComponents[de.Key];
                    if (extender == null) continue;

                    ArrayList al = de.Value as ArrayList;
                    if (al != null)
                    {
                        foreach (Extender e in al)
                        {
                            MethodInfo mi = extender.GetType().GetMethod(e.property);
                            if (mi == null) continue;
                            mi.Invoke(extender, new object[] { e.control, e.value });
                        }
                    }
                }
            }

            private void PrepareParent(IReader reader, bool ignoreParent, Control parent)
            {
                //if (ignoreParent == false)
                //   LoadProperties(parent, reader);

                if (loadMode == LoadModes.EraseForm)
                {
                    if (designerHost != null)
                    {
                        IContainer ic = (IContainer)designerHost.GetService(typeof(IContainer));
                        ISelectionService iss = (ISelectionService)designerHost.GetService(typeof(ISelectionService));
                        iss.SetSelectedComponents(null);

                        foreach (IComponent c in ic.Components)
                        {
                            if (c != designerHost.RootComponent)
                            {
                                try
                                {
                                    ic.Remove(c);
                                }
                                catch
                                {
                                }
                                if (c is IDisposable)
                                    (c as IDisposable).Dispose();
                            }
                        }
                    }
                    else
                        foreach (Control control in parent.Controls)
                            control.Dispose();
                }
            }

            private void LoadComponents(IReader reader, IComponent[] components)
            {
                int lctr = 0;

                while (reader.State != ReaderState.EOF)
                {
                    while (reader.State != ReaderState.StartElement)
                    {
                        if (reader.Read() == false)
                            break;
                    }

                    if (reader.State != ReaderState.StartElement)
                        break;

                    string name = (string)reader.Attributes[NAME];
                    if (name == null)
                        continue;

                    // add component to loadedComponents
                    LoadObject(reader, (components != null && lctr < components.Length) ? components[lctr++] : null, true);
                }
            }

            private void SetReferences()
            {
                foreach (ReferencedItem de in referencedComponents)
                {
                    object ctrl = loadedComponents[de.key];
                    if (ctrl == null)
                        continue;

                    foreach (ComponentProperty cp in de.properties)
                        cp.SetProperty(ctrl);
                }
            }

            private void AddBindings()
            {
                foreach (DictionaryEntry de in lazyList)
                {
                    object key = de.Key;

                    try
                    {
                        ControlBindingsCollection db = key as ControlBindingsCollection;
                        if (db != null)
                        {
                            //Coverity BUG FIX CID 11432 
                            ArrayList theArrayList = de.Value as ArrayList;
                            if (theArrayList != null)
                            {
                                foreach (InstanceDescriptorLoader idl in theArrayList)
                                {
                                    object value = CreateInstance(idl);
                                    if (value != null)
                                        db.Add((Binding)value);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            private bool ContainsItem(IEnumerable list, object value)
            {
                if (list == null) return false;

                foreach (object el in list)
                    if (el == value)
                        return true;

                return false;
            }

            private void InvokeAddRange()
            {
                foreach (DictionaryEntry de in lazyList)
                {
                    object key = de.Key;

                    MethodInfo mi = key.GetType().GetMethod("AddRange");
                    if (mi == null)
                        continue;

                    ParameterInfo[] pi = mi.GetParameters();
                    if (pi.Length != 1 || !pi[0].ParameterType.IsArray)
                        continue;

                    ArrayList list = de.Value as ArrayList;
                    ArrayList destList = new ArrayList();
                    //Coverity Bug Fix CID 19026 
                    if (list != null)
                    {
                        object arrayItem = null;
                        foreach (object item in list)
                        {
                            arrayItem = item;
                            if (item is string)
                                arrayItem = loadedComponents[(string)item];
                            else if (item is InstanceDescriptorLoader)
                                arrayItem = CreateInstance((InstanceDescriptorLoader)item);
                            if (arrayItem != null && !ContainsItem(key as IEnumerable, arrayItem))
                                destList.Add(arrayItem);
                        }

                        Array array = (Array)Activator.CreateInstance(pi[0].ParameterType,
                           new object[] { destList.Count });
                        destList.CopyTo(array);

                        try
                        {
                            mi.Invoke(key, new object[] { array });
                        }
                        catch
                        {
                        }
                    }
                }
            }

            bool LazyLoadInstance(Type type)
            {
                return (typeof(DataRelation).IsAssignableFrom(type) ||
                        typeof(Constraint).IsAssignableFrom(type));
            }

            bool IsDataCollection(Type type)
            {
                return (typeof(System.Data.InternalDataCollectionBase).IsAssignableFrom(type) ||
                    //typeof(BindingsCollection).IsAssignableFrom(type));
                        typeof(BaseCollection).IsAssignableFrom(type));
            }

            object CreateInstance(InstanceDescriptorLoader idl)
            {
                for (int i = 0; i < idl.args.Length; i++)
                {
                    LazyParam lp = idl.args[i] as LazyParam;
                    if (lp != null)
                        idl.args[i] = loadedComponents[lp.name];
                }

                InstanceDescriptor id = new InstanceDescriptor(idl.memberInfo, idl.args);
                return id.Invoke();
            }

            /// <summary>
            /// Serializes objects with embeded controls int writer
            /// </summary>
            /// <param name="components">collection of the parent components</param>
            /// <param name="writer">destination writer</param>
            public void Store(IComponent[] components, IWriter writer)
            {
                if (components.Length == 0)
                    return;

                BeforeWriting();

                // write top element if collection have 2 or more elements
                if (components.Length > 1)
                {
                    Hashtable attributes = new Hashtable();
                    attributes[VERSION] = VERSION_ID.ToString();

                    versionWrited = true;
                    writer.WriteStartElement(OBJECT_COLLECTION, attributes);
                }

                Control rootControl = null;
                if (designerHost != null)
                    rootControl = designerHost.RootComponent as Control;

#if TRIAL
            m_loading = 0;
#endif
                Hashtable componentList = new Hashtable();
                foreach (IComponent component in components)
                {
                    Control curControl = component as Control;
                    if (curControl != null)
                        componentList[curControl] = 1;
                }

                foreach (IComponent component in components)
                {
                    if (HaveParentInList(component as Control, componentList) == false)
                        StoreControl(component, rootControl, writer);
                }

                if (components.Length > 1)
                    writer.WriteEndElement(OBJECT_COLLECTION);

                writer.Flush();

                AfterWriting();
            }

            private ArrayList drillList = new ArrayList();
            /// <summary>
            /// Fires when Treasury try to drill serialized down control
            /// </summary>
            public event DrillDownHandler DrillDown
            {
                add
                {
                    drillList.Add(value);
                }
                remove
                {
                    drillList.Remove(value);
                }
            }

            /// <summary>
            /// Fires when component completly loaded into the form
            /// </summary>
            public event ComponentLoadedHandler ComponentLoaded;

            /// <summary>
            /// Sets log file name
            /// </summary>
            public string LogFile
            {
                set { logFile = value; }
                get { return logFile; }
            }
            #endregion

            #region IDesignerSerializationService Members

            private Hashtable pointers;
            /// <summary>
            /// Serializes the specified collection of objects and stores them in a serialization data object.
            /// </summary>
            /// <param name="objects">a collection of objects to serialize</param>
            /// <returns>an object that contains the serialized state of the specified collection of objects</returns>
            public object Serialize(ICollection objects)
            {
                string serializationData;
                using (StringWriter stringWriter = new StringWriter())
                {
                    //Coverity Bug Fix CID 13154 
                    using (IWriter writer = new TextFormWriter(stringWriter))
                    {
                        BeforeWriting();

                        foreach (object obj in objects)
                        {
                            String[] names = obj as String[];
                            if (names != null)
                            {
                                Hashtable attributes = new Hashtable();
                                attributes[NAME] = "";
                                attributes[ASSEMBLY] = names.GetType().AssemblyQualifiedName;
                                attributes["Length"] = names.Length;
                                if (versionWrited == false)
                                {
                                    attributes[VERSION] = VERSION_ID.ToString();
                                    versionWrited = true;
                                }
                                writer.WriteStartElement(OBJECT, attributes);
                                foreach (string name in names)
                                    writer.WriteValue(DATA, name, null);
                                writer.WriteEndElement(OBJECT);
                            }
                            else
                            {
                                IComponent component = obj as IComponent;
                                if (component == null)
                                {
                                    if (obj is Hashtable)
                                        pointers = (Hashtable)((Hashtable)obj).Clone();
                                    continue;
                                }
                                ISite saveSite = component.Site;
                                if (designerHost == null && saveSite != null)
                                    designerHost = (IDesignerHost)saveSite.GetService(typeof(IDesignerHost));

                                component.Site = null;
                                StoreControl(component, null, writer);
                                component.Site = saveSite;
                            }
                        }
                    }
                    serializationData = stringWriter.ToString();
                
                }

                AfterWriting();
                return serializationData;
            }

            internal ICollection Deserialize(IReader reader, bool setParent)
            {
                ArrayList components = new ArrayList();

                reader.Read();
                currentVersion = (string)reader.Attributes[VERSION];
                Control parent = designerHost.RootComponent as Control;
                //Coverity Bug Fix CID 13052 
                if (parent != null)
                {
                    Point mouseLocation = parent.PointToClient(Cursor.Position);

                    while (true)
                    {
                        if (reader.State == ReaderState.StartElement)
                        {
                            Object c = LoadObject(reader, null, false);
                            if (c != null && c is IComponent)
                            {
                                if (designerHost != null)
                                {
                                    Control ctrl = c as Control;
                                    if (ctrl != null)
                                    {
                                        if (pointers != null && pointers.ContainsKey(loadedObjectName))
                                        {
                                            Point origin = (Point)pointers[loadedObjectName];
                                            Point newLocation = new Point(mouseLocation.X - origin.X, mouseLocation.Y - origin.Y);
                                            ctrl.Location = newLocation;
                                        }
                                        ctrl.Parent = (setParent) ? parent : null;
                                        ctrl.Text = ctrl.Name;
                                    }
                                }
                                components.Add(c);
                            }
                        }
                        if (reader.Read() == false)
                            break;
                    }
                }
                return components;
            }

            /// <summary>
            /// Deserializes the specified serialization data object and returns a collection of objects represented by that data
            /// </summary>
            /// <param name="serializationData">an object consisting of serialized data</param>
            /// <returns>an ICollection of objects rebuilt from the specified serialization data object</returns>
            public ICollection Deserialize(object serializationData)
            {
                string stringData = serializationData as string;
                if (stringData == null)
                    return null;

                ICollection components = null;
                using (StringReader stringReader = new StringReader(stringData))
                {
                    //Coverity Bug Fix CID 10901 (Local Analysis)
                    using (IReader reader = new TextFormReader(stringReader))
                    {
                        components = Deserialize(reader, true);
                    }
                }
                pointers = null;
                if (designerHost != null)
                {
                    ISelectionService iss = (ISelectionService)designerHost.GetService(typeof(ISelectionService));
                    iss.SetSelectedComponents(components, SelectionTypes.Replace);
                }
                return components;
            }
            #endregion
        }

        internal class ComponentProperty
        {
            protected PropertyDescriptor property;
            protected object component;

            public ComponentProperty(object component, PropertyDescriptor property)
            {
                this.component = component;
                this.property = property;
            }

            public virtual void SetProperty(object value)
            {
                object curValue = property.GetValue(component);

                if (curValue != value)
                    property.SetValue(component, value);
            }

            public virtual object GetProperty()
            {
                object value = property.GetValue(component);
                return value;
            }
        }

        internal class ListProperty : ComponentProperty
        {
            public ListProperty(object component, PropertyDescriptor property) :
                base(component, property)
            {
            }

            public override void SetProperty(object value)
            {
                object val = property.GetValue(component);
                if (val == null) return;

                IList list = val as IList;
                if (list == null)
                {
                    PropertyInfo pi = val.GetType().GetProperty("List", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (pi != null)
                        list = (IList)pi.GetValue(val, new object[] { });
                }
                if (list != null)
                {
                    try
                    {
                        list.Add(value);
                    }
                    catch
                    {
                    }
                }
            }

            public override object GetProperty()
            {
                return null;
            }
        }

        internal class ReferencedItem
        {
            internal string key;
            internal ArrayList properties = new ArrayList();
        }

        internal class LazyParam
        {
            public string name;
        }

        internal class InstanceDescriptorLoader
        {
            public MemberInfo memberInfo;
            public object[] args;
        }

        internal class ReferencedCollection : ArrayList
        {
            internal ReferencedCollection() { }

            public void Add(string key, ComponentProperty property)
            {
                foreach (ReferencedItem item in this)
                {
                    if (item.key == key)
                    {
                        item.properties.Add(property);
                        return;
                    }
                }

                ReferencedItem newItem = new ReferencedItem();
                newItem.key = key;
                newItem.properties.Add(property);
                Add(newItem);
            }
        }
    }
    internal struct Extender
    {
        internal object control;
        internal object value;
        internal string property; // add Set
    }

}
