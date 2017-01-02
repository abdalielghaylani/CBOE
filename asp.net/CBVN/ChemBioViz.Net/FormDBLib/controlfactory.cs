using System;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace FormDBLib
{
    #region ControlFactory

    /// <summary>
    /// Summary description for FormControlFactory.
    /// </summary>
    public class ControlFactory
    {
        #region Constructors
        public ControlFactory()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        ///  Instantiate the control given its name or type
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Control CreateControl(Control control)
        {
            try
            {
                Control ctrl = null;
                switch (control.GetType().Name)
                {
                    case "Label":
                        ctrl = new Label();
                        break;
                    case "TextBox":
                        ctrl = new TextBox();
                        break;
                    case "PictureBox":
                        ctrl = new PictureBox();
                        break;
                    case "ListView":
                        ctrl = new ListView();
                        break;
                    case "ComboBox":
                        ctrl = new ComboBox();
                        break;
                    case "Button":
                        ctrl = new Button();
                        break;
                    case "CheckBox":
                        ctrl = new CheckBox();
                        break;
                    case "MonthCalender":
                        ctrl = new MonthCalendar();
                        break;
                    case "DateTimePicker":
                        ctrl = new DateTimePicker();
                        break;
                    default:
                        ctrl = (Control)Activator.CreateInstance(control.GetType());
                        break;
                }
                return ctrl;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("create control failed:" + ex.Message);
                return new Control();
            }
        }
        //---------------------------------------------------------------------		
        public static void SetControlProperties(Control ctrl, Hashtable propertyList)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ctrl);

            foreach (PropertyDescriptor myProperty in properties)
            {
                if (propertyList.Contains(myProperty.Name))
                {
                    Object obj = propertyList[myProperty.Name];
                    try
                    {
                        myProperty.SetValue(ctrl, obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public static Control CloneCtrl(Control ctrl, bool bWithBindings)
        {
            Control newCtrl = ControlFactory.CreateControl(ctrl);
            if (newCtrl != null)
                UseClonedCtrl(ctrl, bWithBindings, newCtrl);
            return newCtrl;
        }
        //---------------------------------------------------------------------
        public static void UseClonedCtrl(Control ctrl, bool bWithBindings, Control newCtrl)
        {
            CBFormCtrl cbCtrl = new CBFormCtrl(ctrl);
            ControlFactory.SetControlProperties(newCtrl, cbCtrl.PropertyList);

            foreach (Binding b in ctrl.DataBindings)
            {
                if (bWithBindings)
                {
                    Binding bNew = new Binding(b.PropertyName, b.DataSource, b.BindingMemberInfo.BindingMember, b.FormattingEnabled,
                                            b.DataSourceUpdateMode, b.NullValue, b.FormatString);
                    newCtrl.DataBindings.Add(bNew);
                }
            }
        }
        //---------------------------------------------------------------------
        public static void CopyProperties(Control ctrlFrom, Control ctrlTo)
        {
            CBFormCtrl cbCtrl = new CBFormCtrl(ctrlFrom);
            ControlFactory.SetControlProperties(ctrlTo, cbCtrl.PropertyList);
        }
        //---------------------------------------------------------------------
        public static void CopyObjectProperties(Object objFrom, Object objTo)
        {
            Hashtable propertyList = new Hashtable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objFrom);
            foreach (PropertyDescriptor prop in properties)
            {
                try
                {
                    bool bIsHidden = prop.SerializationVisibility == DesignerSerializationVisibility.Hidden;
                    if (prop.PropertyType.IsSerializable && !bIsHidden)
                    {
                        if (objFrom != null && prop.GetValue(objFrom) != null)
                            propertyList.Add(prop.Name, prop.GetValue(objFrom));
                    }
                }
                catch (Exception)
                {
                }
            }

            PropertyDescriptorCollection propertiesTo = TypeDescriptor.GetProperties(objTo);
            foreach (PropertyDescriptor prop in propertiesTo)
            {
                if (propertyList.Contains(prop.Name))
                {
                    Object obj = propertyList[prop.Name];
                    try
                    {
                        prop.SetValue(objTo, obj);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        public static void CopyCtrl2ClipBoard(Control ctrl)
        {
            CBFormCtrl cbCtrl = new CBFormCtrl(ctrl);
            IDataObject ido = new DataObject();

            ido.SetData(CBFormCtrl.Format.Name, true, cbCtrl);
            Clipboard.SetDataObject(ido, false);

        }
        //---------------------------------------------------------------------
        public static Control GetCtrlFromClipBoard()
        {
            Control ctrl = new Control();
            IDataObject ido = Clipboard.GetDataObject();
            //Coverity Bug Fix : CID  13109 
            if (ido != null && ido.GetDataPresent(CBFormCtrl.Format.Name))
            {
                CBFormCtrl cbCtrl = ido.GetData(CBFormCtrl.Format.Name) as CBFormCtrl;
                //Coverity Bug Fix  13028 
                if (cbCtrl != null)
                {
                    ctrl = ControlFactory.CreateControl((Control)cbCtrl.Ctrl);
                    ControlFactory.SetControlProperties(ctrl, cbCtrl.PropertyList);
                }

            }
            return ctrl;
        }
        #endregion
    }

    #endregion

    #region Clipboard Support

    /// <summary>
    /// Summary description for CBFormCtrl.
    /// </summary>
    [Serializable()]
    public class CBFormCtrl//clipboard form control
    {
        #region Variables
        private static DataFormats.Format m_format;
        private Control m_ctrl;
        private string m_ctrlName;
        private string m_partialName;
        private Hashtable m_propertyList = new Hashtable();
        #endregion

        #region Properties
        public Control Ctrl
        {
            get { return m_ctrl; }
            set { m_ctrl = value; }
        }

        public static DataFormats.Format Format
        {
            get
            {
                return m_format;
            }
        }
        public string CtrlName
        {
            get
            {
                return m_ctrlName;
            }
            set
            {
                m_ctrlName = value;
            }
        }

        public string PartialName
        {
            get
            {
                return m_partialName;
            }
            set
            {
                m_partialName = value;
            }
        }

        public Hashtable PropertyList
        {
            get
            {
                return m_propertyList;
            }

        }
        #endregion

        #region Constructors
        public CBFormCtrl()
        {
        }
        //---------------------------------------------------------------------
        static CBFormCtrl()
        {
            // Registers a new data format with the windows Clipboard
            m_format = DataFormats.GetFormat(typeof(CBFormCtrl).FullName);
        }
        //---------------------------------------------------------------------
        public CBFormCtrl(Control ctrl)
        {
            CtrlName = ctrl.GetType().Name;
            PartialName = ctrl.GetType().Namespace;

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ctrl);

            foreach (PropertyDescriptor myProperty in properties)
            {
                try
                {
                    if (myProperty.PropertyType.IsSerializable)
                        // NOTE: even if prop is serializable, it might be marked hidden, i.e., this is true:
                        // myProperty.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden)
                        // but if we exclude these, we lose data
                        m_propertyList.Add(myProperty.Name, myProperty.GetValue(ctrl));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    //do nothing, just continue
                }
            }
        }
        #endregion
    }

    #endregion
}
