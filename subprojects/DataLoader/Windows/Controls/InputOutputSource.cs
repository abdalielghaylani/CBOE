using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Calculation.Parser;
using CambridgeSoft.COE.DataLoader.Windows;
using CambridgeSoft.COE.DataLoader.Windows.Common;
using CambridgeSoft.COE.DataLoader.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// Control used by InputOutputMapper to provide mapping choices for output fields
    /// </summary>
    public partial class InputOutputSource : UserControl
    {
        #region enum
        /// <summary>
        /// Used to set/get the currently visible/active state
        /// </summary>
        // <typeparam name="Default">Default</typeparam>
        public enum Showing
        {
            /// <summary>
            /// CheckBox
            /// </summary>
            Checkbox,
            /// <summary>
            /// Calculation
            /// </summary>
            Calculation,
            /// <summary>
            /// DateTimePicker
            /// </summary>
            Date,
            /// <summary>
            /// Nothing visible (Null output field)
            /// </summary>
            Default,
            /// <summary>
            /// ComboBox with mappable input fields
            /// </summary>
            Map,
            /// <summary>
            /// Nothing visible (No output field)
            /// </summary>
            None,
            /// <summary>
            /// ComboBox with picklist
            /// </summary>
            Picklist,
            /// <summary>
            /// TextBox
            /// </summary>
            Textbox
        };
        #endregion

        #region data
        private CalculationBox _CalculationBox = null;
        private string _strCalculationError;
        private int _nCalculationErrorColumn;
        private int _cCalculationErrorColumns;
        private bool _bIsValid;
        private string _strOutputFieldName;
        private string _strOutputFormName;
        private MappingTypeGeneric _eOutputType;
        private string _strOutputTypeSpecification;
        #endregion

        #region properties

        /// <summary>
        /// Get/set whether this mapping is valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _bIsValid;
            }
            private set
            {
                if (_bIsValid != value)
                {
                    _bIsValid = value;
                    OnIsValidChanged(new EventArgs());
                }
                return;
            }
        } // IsValid

        /// <summary>
        /// Set the name of the corresponding output field
        /// </summary>
        public string OutputFieldName
        {
            get
            {
                return _strOutputFieldName;
            }
            set
            {
                _strOutputFieldName = value;
                return;
            }
        } // OutputFieldName

        /// <summary>
        /// Set the name of the corresponding output field's form
        /// </summary>
        public string OutputFormName
        {
            get
            {
                return _strOutputFormName;
            }
            set
            {
                _strOutputFormName = value;
                return;
            }
        } // OutputFormName

        /// <summary>
        /// Set the output type of the corresponding output field
        /// (eg. String)
        /// </summary>
        public string OutputType
        {
            set
            {
                _eOutputType = new MappingTypeGeneric(value);
                return;
            }
        } // OutputType

        private MappingTypeGeneric.GenericType OutputTypeValue
        {
            get
            {
                return _eOutputType.TypeValue;
            }
        } // OutputTypeValue

        /// <summary>
        /// Set the output type specification of the corresponding output field
        /// (eg. the length of a string field)
        /// </summary>
        public string OutputTypeSpecification
        {
            private get
            {
                return _strOutputTypeSpecification;
            }
            set
            {
                _strOutputTypeSpecification = value;
                return;
            }
        } // OutputTypeSpecification

        #endregion

        #region event declaration/definition
        /// <summary>
        /// Information about calculation changed for use by <see cref="CalculationChanged"/>
        /// </summary>
        public class CalculationChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Parser.Error
            /// </summary>
            public string _strParserError;
            /// <summary>
            /// Parser.ErrorColumn
            /// </summary>
            public int _nParserErrorColumn;
            /// <summary>
            /// Parser.ErrorColumns
            /// </summary>
            public int _cParserErrorColumns;
            private string _strSource;
            private object _oValue;
            /// <summary>
            /// Property to expose the source
            /// </summary>
            public string Source
            {
                get
                {
                    return _strSource;
                }
            } // Source
            /// <summary>
            /// Property to expose the value (type)
            /// </summary>
            public object Value
            {
                get
                {
                    return _oValue;
                }
            } // Value
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="vstrSource"></param>
            /// <param name="voValue"></param>
            public CalculationChangedEventArgs(string vstrSource, object voValue)
            {
                _strParserError = string.Empty;
                _nParserErrorColumn = 0;
                _cParserErrorColumns = 0;
                _strSource = vstrSource;
                _oValue = voValue;
                return;
            }
        } // class CalculationChangedEventArgs

        /// <summary>
        /// Delegate for CalculationChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CalculationChangedEvent(object sender, CalculationChangedEventArgs e);

        /// <summary>
        /// The CalculationChanged event is raised when the calculation changes
        /// </summary>
        public event CalculationChangedEvent CalculationChanged;
        private void OnCalculationChanged(CalculationChangedEventArgs e)
        {
            if (CalculationChanged != null)
            {
                CalculationChanged(this, e);
            }
            return;
        } // OnCalculationChanged()

        /// <summary>
        /// Delegate for IsValidChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void IsValidChangedEvent(object sender, EventArgs e);

        /// <summary>
        /// The IsValidChanged event is raised when IsValid property changes
        /// </summary>
        public event IsValidChangedEvent IsValidChanged;
        private void OnIsValidChanged(EventArgs e)
        {
            if (IsValidChanged != null)
            {
                IsValidChanged(this, e);
            }
            return;
        } // OnIsValidChanged()
        #endregion

        #region data (controls)
        private System.Windows.Forms.PictureBox _Calculation_PictureBox;
        private System.Windows.Forms.RichTextBox _Calculation_RichTextBox;
        private CalculationParser _Parser; // required for calculations
        private System.Windows.Forms.CheckBox _Checked_Checkbox;
        private System.Windows.Forms.ContainerControl _Checked_Container;
        private System.Windows.Forms.DateTimePicker _Date_DateTimePicker;
        private System.Windows.Forms.ComboBox _FieldList_ComboBox;
        private System.Windows.Forms.ComboBox _Picklist_ComboBox;
        private System.Windows.Forms.RichTextBox _Text_RichTextBox;
        private Control _ControlShowing;
        private Showing _WhichShowing;
        #endregion

        #region properties (controls)
        /// <summary>
        /// Set the Calculation and implictly creates a RichTextBox
        /// </summary>
        public string Calculation
        {
            set
            {
                if (_Calculation_RichTextBox == null)
                {
                    _Calculation_RichTextBox = UIBase.GetRichTextBox();
                    {
                        switch (_eOutputType.TypeName)
                        {
                            case "Boolean": { _Calculation_RichTextBox.Tag = (Boolean)false; break; }
                            case "Date": { _Calculation_RichTextBox.Tag = (DateTime)DateTime.Now; break; }
                            case "Decimal": { _Calculation_RichTextBox.Tag = (Double)1.0; break; }
                            case "Integer": { _Calculation_RichTextBox.Tag = (Int32)1; break; }
                            case "String": { _Calculation_RichTextBox.Tag = (string)""; break; }
                            default: { _Calculation_RichTextBox.Tag = new Object(); break; }
                        }
                    }
                    _Calculation_RichTextBox.Visible = false;
                    Controls.Add(_Calculation_RichTextBox);
                    if (Height < _Calculation_RichTextBox.Height) Height = _Calculation_RichTextBox.Height;
                    _Calculation_RichTextBox.Enter += new EventHandler(Calculation_RichTextBox_Enter);
                    _Calculation_RichTextBox.TextChanged += new EventHandler(Calculation_RichTextBox_TextChanged);
                    _Calculation_RichTextBox.Validating += new CancelEventHandler(Calculation_RichTextBox_Validating);

                    _Calculation_PictureBox = UIBase.GetPictureBox(UIBase.PictureBoxType.Ellipsis);
                    _Calculation_RichTextBox.Width -= _Calculation_PictureBox.Width;
                    _Calculation_PictureBox.Left = _Calculation_RichTextBox.Width;
                    _Calculation_PictureBox.Top = (_Calculation_RichTextBox.Height - _Calculation_PictureBox.Height) / 2;
                    //_Calculation_PictureBox.Enabled = false; // true if parsing error
                    Controls.Add(_Calculation_PictureBox);
                    _Calculation_PictureBox.Click += new EventHandler(Calculation_PictureBox_Click);
                }
                return;
            }
        } // Calculation

        /// <summary>
        /// Return calculation RichTextBox for outside monitoring
        /// </summary>
        public RichTextBox Calculation_RichTextBox
        {
            get
            {
                return _Calculation_RichTextBox;
            }
        } // Calculation_RichTextBox

        /// <summary>
        /// Set the CheckBox state and implictly creates a CheckBox
        /// </summary>
        public bool Checked
        {
            set
            {
                if (_Checked_Container == null)
                {
                    _Checked_Container = new System.Windows.Forms.ContainerControl();
                    _Checked_Container.Height = Height;
                    _Checked_Container.Width = Width;
                    _Checked_Checkbox = UIBase.GetCheckBox();
                    _Checked_Checkbox.Height = 12;
                    _Checked_Checkbox.Width = 12;
                    _Checked_Checkbox.Top = (Height - _Checked_Checkbox.Height) / 2;
                    _Checked_Container.Controls.Add(_Checked_Checkbox);
                }
                _Checked_Checkbox.Checked = value;
                _Checked_Container.Visible = false;
                //if (_Checked_Container.MinimumSize.Width < _Checked_Container.Width) _Checked_Container.MinimumSize = new Size(_Checked_Container.Width, _Checked_Container.MinimumSize.Height);
                Controls.Add(_Checked_Container);
                if (Height < _Checked_Container.Height) Height = _Checked_Container.Height;
                return;
            }
        } // Checked

        /// <summary>
        /// Set the DateTimePicker value and implictly creates a DateTimePicker
        /// </summary>
        public string Date
        {
            set
            {
                if (_Date_DateTimePicker == null) _Date_DateTimePicker = UIBase.GetDateTimePicker();
                _Date_DateTimePicker.Format = DateTimePickerFormat.Short;
                _Date_DateTimePicker.MaxDate = DateTime.Now;
                _Date_DateTimePicker.Text = value;
                _Date_DateTimePicker.Visible = false;
                if (_Date_DateTimePicker.MinimumSize.Width < _Date_DateTimePicker.Width) _Date_DateTimePicker.MinimumSize = new Size(_Date_DateTimePicker.Width, _Date_DateTimePicker.MinimumSize.Height);
                if (MinimumSize.Width < _Date_DateTimePicker.Width) MinimumSize = new Size(_Date_DateTimePicker.Width, MinimumSize.Height);
                Controls.Add(_Date_DateTimePicker);
                if (Height < _Date_DateTimePicker.Height) Height = _Date_DateTimePicker.Height;
                return;
            }
        } // Date

        /// <summary>
        /// Get whether the "text" value is blank
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                bool bRet = false;
                switch (WhichShowing)
                {
                    case Showing.Calculation: { bRet = (_Calculation_RichTextBox.Text == string.Empty); break; }
                    case Showing.Textbox: { bRet = (_Text_RichTextBox.Text == string.Empty); break; }
                }
                return bRet;
            }
        } // IsEmpty

        /// <summary>
        /// Set the contents of a ComboBox and implictly create the ComboBox
        /// This ComboBox represents a list of mappable input fields
        /// Sets SelectIndex based on OutputFieldName if possible
        /// </summary>
        public string FieldList
        {
            set
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(value);
                XmlNode oXmlNodeFieldlist = oXmlDocument.DocumentElement;
                if (_FieldList_ComboBox == null) _FieldList_ComboBox = UIBase.GetComboBox();
                _FieldList_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                _FieldList_ComboBox.Visible = false;
                int xMax = 16;
                foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                {
                    string strInputFieldDbName = oXmlNodeField.Attributes["dbname"].Value;
                    string strInputFieldName = oXmlNodeField.Attributes["name"].Value;
                    _FieldList_ComboBox.Items.Add(new COEItemPair<string>(strInputFieldName, strInputFieldDbName));
                    Size size = TextRenderer.MeasureText(strInputFieldName, _FieldList_ComboBox.Font);
                    if (xMax < size.Width) xMax = size.Width;
                }
                if (_FieldList_ComboBox.Items.Count > 0)
                {
                    _FieldList_ComboBox.SelectedIndex = _FieldList_ComboBox.FindStringExact(OutputFieldName);
                    if (_FieldList_ComboBox.SelectedIndex < 0) _FieldList_ComboBox.SelectedIndex = 0;
                }
                else
                {
                    _FieldList_ComboBox.SelectedIndex = -1; // WJC Hmmm
                }
                _FieldList_ComboBox.Width = xMax + 16; // WJC dropdown
                if (_FieldList_ComboBox.MinimumSize.Width < _FieldList_ComboBox.Width) _FieldList_ComboBox.MinimumSize = new Size(_FieldList_ComboBox.Width, _FieldList_ComboBox.MinimumSize.Height);
                Controls.Add(_FieldList_ComboBox);
                if (Height < _FieldList_ComboBox.Height) Height = _FieldList_ComboBox.Height;
                return;
            }
        } // FieldList

        /// <summary>
        /// Return FieldList ComboBox for outside monitoring
        /// </summary>
        public ComboBox FieldList_ComboBox
        {
            get
            {
                return _FieldList_ComboBox;
            }
        } // FieldList_ComboBox

        /// <summary>
        /// Return the currently selected field list item
        /// </summary>
        public string FieldList_Value
        {
            get
            {
                string strRet = string.Empty;
                if (_WhichShowing == Showing.Map)
                {
                    Object oSelectedItem = _FieldList_ComboBox.SelectedItem;
                    strRet = (oSelectedItem != null) ? ((COEItemPair<string>)oSelectedItem).ToString() : string.Empty;
                }
                return strRet;
            }
        } // FieldList_Value

        /// <summary>
        /// Set the parser required for calculations
        /// </summary>
        public CalculationParser Parser
        {
            get
            {
                return _Parser;
            }
            set
            {
                _Parser = value;
                return;
            }
        }

        /// <summary>
        /// Set the contents of a ComboBox and implictly create the ComboBox
        /// This ComboBox represents a picklist
        /// </summary>
        public XmlNode Picklist
        {
            set
            {
                if (_Picklist_ComboBox == null) _Picklist_ComboBox = UIBase.GetComboBox();
                _Picklist_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                _Picklist_ComboBox.Visible = false;
                int xMax = 16;
                if (value != null)
                {
                    XmlNode oXmlNodeItems;
                    oXmlNodeItems = (value.Name == "items") ? value : value.SelectSingleNode("items");
                    if (oXmlNodeItems != null)
                    {
                        foreach (XmlNode oXmlNodeItem in oXmlNodeItems)
                        {
                            int nItem = Convert.ToInt32(oXmlNodeItem.Attributes["value"].Value);
                            string strItem = oXmlNodeItem.InnerText.Trim();
                            _Picklist_ComboBox.Items.Add(new COEItemPair<int>(strItem, nItem));
                            Size size = TextRenderer.MeasureText(oXmlNodeItem.InnerText, _Picklist_ComboBox.Font);
                            if (xMax < size.Width) xMax = size.Width;
                        } // foreach (XmlNode oXmlNodeItem in oXmlNodeItems)
                        _Picklist_ComboBox.Sorted = true;
                        _Picklist_ComboBox.SelectedIndex = (_Picklist_ComboBox.Items.Count > 0) ? 0 : -1;
                    } // if (oXmlNodeItems != null)
                }
                _Picklist_ComboBox.Width = xMax + 16; // WJC dropdown
                if (_Picklist_ComboBox.MinimumSize.Width < _Picklist_ComboBox.Width) _Picklist_ComboBox.MinimumSize = new Size(_Picklist_ComboBox.Width, _Picklist_ComboBox.MinimumSize.Height);
                Controls.Add(_Picklist_ComboBox);
                if (Height < _Picklist_ComboBox.Height) Height = _Picklist_ComboBox.Height;
                return;
            }
        } // Picklist

        /// <summary>
        /// Sets the content of a TextBox and implicitly creates the TextBox
        /// The length and other characteristics are controlled by OutputType and OutputTypeSpecification
        /// (It is assumed that these have already been set)
        /// </summary>
        public override string Text
        {
            set
            {
                if (_Text_RichTextBox == null)
                {
                    _Text_RichTextBox = UIBase.GetRichTextBox();
                    _Text_RichTextBox.Enter += new EventHandler(Text_RichTextBox_Enter);
                    _Text_RichTextBox.TextChanged += new EventHandler(Text_RichTextBox_TextChanged);
                    _Text_RichTextBox.Validating += new CancelEventHandler(Text_RichTextBox_Validating);
                }
                if (
                    (OutputTypeValue == MappingTypeGeneric.GenericType.Decimal) ||
                    (OutputTypeValue == MappingTypeGeneric.GenericType.Integer) ||
                    (OutputTypeValue == MappingTypeGeneric.GenericType.String)
                )
                {
                    if (OutputTypeSpecification.Length > 0)
                    {
                        int nMaxLength = Convert.ToInt32(OutputTypeSpecification);
                        _Text_RichTextBox.MaxLength = nMaxLength;
                    }
                }
                _Text_RichTextBox.Text = value;
                _Text_RichTextBox.Visible = false;
                if (_Text_RichTextBox.MinimumSize.Width < _Text_RichTextBox.Width) _Text_RichTextBox.MinimumSize = new Size(_Text_RichTextBox.Width, _Text_RichTextBox.MinimumSize.Height);
                Controls.Add(_Text_RichTextBox);
                if (Height < _Text_RichTextBox.Height) Height = _Text_RichTextBox.Height;
                return;
            } // set
        } // Text

        /// <summary>
        /// Get/set which subcontrol (if any) is active
        /// </summary>
        public Showing WhichShowing
        {
            get
            {
                return _WhichShowing;
            } // get
            set
            {
                if (_WhichShowing != value)
                {
                    Control ctlTo = null;
                    if (_ControlShowing != null) _ControlShowing.Visible = false;
                    switch (value)
                    {
                        default: // falls through to Default
                        case Showing.Default: { ctlTo = null; IsValid = true; break; }
                        case Showing.Calculation: { ctlTo = _Calculation_RichTextBox; Calculation_RichTextBox_TextChanged(_Calculation_RichTextBox, new EventArgs()); break; }
                        case Showing.Checkbox: { ctlTo = _Checked_Container; IsValid = true; break; }
                        case Showing.Date: { ctlTo = _Date_DateTimePicker; IsValid = true;  break; }
                        case Showing.Map: { ctlTo = _FieldList_ComboBox; IsValid = (_FieldList_ComboBox.Items.Count > 0); break; }
                        case Showing.None: { ctlTo = null; IsValid = true; break; }
                        case Showing.Picklist: { ctlTo = _Picklist_ComboBox; IsValid = (_Picklist_ComboBox.Items.Count > 0); break; }
                        case Showing.Textbox: { ctlTo = _Text_RichTextBox; Text_RichTextBox_TextChanged(_Text_RichTextBox, new EventArgs()); break; }
                    } // switch (_WhichShowing)
                    if (ctlTo != null)
                    {
                        _WhichShowing = value;
                        _ControlShowing = ctlTo;
                        _ControlShowing.Width = Width;
                        Visible = _ControlShowing.Visible = true;
                        InputOutputSource_Resize(this, new EventArgs());
                        //Fix for CSBR 157933- Crash while changing the "Alias" field type as constant.
                        if (_ControlShowing == _Calculation_RichTextBox)
                            _Calculation_PictureBox.Visible = true;
                        else if(_ControlShowing ==_Text_RichTextBox)
                            _Calculation_PictureBox.Visible = false;
                        else if((_Calculation_PictureBox != null) && (_ControlShowing == _Picklist_ComboBox))
                            _Calculation_PictureBox.Visible = false;
                        TabStop = Enabled = _ControlShowing.TabStop;
                    }
                    else if ((value == Showing.Default) || (value == Showing.None))
                    {
                        _WhichShowing = value;
                        Visible = false;
                        if ((_Calculation_PictureBox != null) && (_Calculation_PictureBox.Visible)) _Calculation_PictureBox.Visible = false;
                        TabStop = Enabled = false;
                    }
                    else
                    {
                        throw new Exception("fix me");   // BUG
                    }
                }
                return;
            } // set
        } // WhichShowing

        #endregion

        #region methods
        /// <summary>
        /// Insert mapping into larger XML specification
        /// </summary>
        /// <param name="voCOEXmlTextWriter"></param>
        /// <param name="vstrFields"></param>
        public void GetMapping(XmlTextWriter voCOEXmlTextWriter, string vstrFields)
        {
            voCOEXmlTextWriter.WriteStartElement("field");
            voCOEXmlTextWriter.WriteAttributeString("name", OutputFieldName);
            switch (_WhichShowing)
            {
                case Showing.Calculation:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "calculation");
                        voCOEXmlTextWriter.WriteAttributeString("value", _Calculation_RichTextBox.Text);
                        if (vstrFields != string.Empty)
                        {
                            voCOEXmlTextWriter.WriteAttributeString("fields", vstrFields);
                        }
                        break;
                    }
                case Showing.Checkbox:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "checkbox");
                        voCOEXmlTextWriter.WriteAttributeString("value", ((_Checked_Checkbox.Checked) ? "1" : "0"));
                        break;
                    }
                case Showing.Date:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "date");
                        DateTime dt = _Date_DateTimePicker.Value;
                        voCOEXmlTextWriter.WriteAttributeString("value", dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString());
                        break;
                    }
                case Showing.Default:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "default");
                        break;
                    }
                case Showing.Map:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "map");
                        voCOEXmlTextWriter.WriteAttributeString("value", ((COEItemPair<string>)_FieldList_ComboBox.SelectedItem).Value);
                        voCOEXmlTextWriter.WriteAttributeString("fields", ((COEItemPair<string>)_FieldList_ComboBox.SelectedItem).ToString());
                        break;
                    }
                case Showing.None:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "none");
                        break;
                    }
                case Showing.Picklist:
                    {
                        voCOEXmlTextWriter.WriteAttributeString("source", "picklist");
                        string selectedValue = string.Empty;
                        //support for empty picklists
                        if (_Picklist_ComboBox.SelectedIndex >= 0)
                        {
                            selectedValue = ((COEItemPair<int>)_Picklist_ComboBox.SelectedItem).Value.ToString();
                        }
                        voCOEXmlTextWriter.WriteAttributeString("value", selectedValue);
                        break;
                    }
                case Showing.Textbox:
                    {
                        string strtext = _Text_RichTextBox.Text;
                        string outRichTextBox = null;
                        if (OutputTypeValue == MappingTypeGeneric.GenericType.Decimal)
                        {
                            double dout = double.Parse(strtext);
                            outRichTextBox = dout.ToString();
                        }
                        else
                        {
                            outRichTextBox = strtext;
                        }
                        voCOEXmlTextWriter.WriteAttributeString("source", "constant");
                        voCOEXmlTextWriter.WriteAttributeString("value", outRichTextBox.ToString());
                        break;
                    }
            }
            voCOEXmlTextWriter.WriteEndElement();   // field
            return;
        } // GetMapping()

        /// <summary>
        /// Setup control based on XML for this field
        /// </summary>
        /// <param name="vxmlMapping"></param>
        /// <returns></returns>
        public string MappingSet(string vxmlMapping)
        {
            string strSource = string.Empty;
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(vxmlMapping);
            XmlNode oXmlNodeField = oXmlDocument.DocumentElement;
            {
                XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["source"];
                if (oXmlAttribute != null) strSource = oXmlAttribute.Value;
            }
            string strValue = string.Empty;
            {
                XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["value"];
                if (oXmlAttribute != null) strValue = oXmlAttribute.Value;
            }
            switch (strSource)
            {
                case "calculation":
                    {
                        _Calculation_RichTextBox.Text = strValue;
                        WhichShowing = Showing.Calculation;
                        break;
                    }
                case "checkbox":
                    {
                        _Checked_Checkbox.Checked = (strValue != "0");
                        WhichShowing = Showing.Checkbox;
                        break;
                    }
                case "date":
                    {
                        string[] strYYYYMMDD = strValue.Split('/');
                        _Date_DateTimePicker.Value = new DateTime(Convert.ToInt32(strYYYYMMDD[0]), Convert.ToInt32(strYYYYMMDD[1]), Convert.ToInt32(strYYYYMMDD[2]));
                        WhichShowing = Showing.Date;
                        break;
                    }
                case "map":
                    {
                        for (int nIndex = 0; nIndex < _FieldList_ComboBox.Items.Count; nIndex++)
                        {
                            if (((COEItemPair<string>)_FieldList_ComboBox.Items[nIndex]).Value == strValue)
                            {
                                _FieldList_ComboBox.SelectedIndex = nIndex;
                                break;
                            }
                        }
                        WhichShowing = Showing.Map;
                        break;
                    }
                case "none":
                    {
                        WhichShowing = Showing.None;
                        break;
                    }
                case "picklist":
                    {
                        for (int nIndex = 0; nIndex < _Picklist_ComboBox.Items.Count; nIndex++)
                        {
                            if (((COEItemPair<int>)_Picklist_ComboBox.Items[nIndex]).Value.ToString() == strValue)
                            {
                                _Picklist_ComboBox.SelectedIndex = nIndex;
                                break;
                            }
                        }
                        WhichShowing = Showing.Picklist;
                        break;
                    }
                case "constant":
                    {
                        _Text_RichTextBox.Text = strValue;
                        WhichShowing = Showing.Textbox;
                        break;
                    }
                case "default":
                    {
                        WhichShowing = Showing.Default;    // What about None ???
                        break;
                    }
                default:
                    {
                        int n = 1; n = 1 / (n - 1); // throw up
                        break;
                    }
            } // switch (strSource)
            return strSource;
        } // MappingSet()
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public InputOutputSource()
        {
            _Checked_Checkbox = null;
            _Checked_Container = null;
            _FieldList_ComboBox = null;
            _Picklist_ComboBox = null;
            _Date_DateTimePicker = null;
            _Calculation_PictureBox = null;
            _Calculation_RichTextBox = null;
            _Text_RichTextBox = null;
            _ControlShowing = null;
            Height = 1; // Will get bumped up by the controls as added
            InitializeComponent();
            Resize +=new EventHandler(InputOutputSource_Resize);
            Enabled = false;
            TabStop = false;
        } // InputOutputSource()
        #endregion

        #region events
        private void Calculation_PictureBox_Click(object sender, EventArgs e)
        {
            if (_CalculationBox == null) _CalculationBox = new CalculationBox(ref _Parser);
            Calculation_RichTextBox_Enter(_Calculation_RichTextBox, new EventArgs());
            _CalculationBox.FormulaRtf = _Calculation_RichTextBox.Rtf;
            _CalculationBox.ReturnType = _Calculation_RichTextBox.Tag;
            _CalculationBox.ShowDialog(this);
            _Calculation_RichTextBox.Rtf = _CalculationBox.FormulaRtf;
            Calculation_RichTextBox_Validating(_Calculation_RichTextBox, new CancelEventArgs());
            return;
        } // Calculation_PictureBox_Click()

        private void Calculation_RichTextBox_Enter(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            // Make certain any red content is reset to black.
            if (rtb != null)
            {
                UIBase.RichTestBox_Unmark(rtb);
            }
            return;
        } // Calculation_RichTextBox_Enter()

        private void Calculation_RichTextBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (rtb != null)
            {
                CalculationChangedEventArgs calculationChangedEventArgs = new CalculationChangedEventArgs(rtb.Text, rtb.Tag);
                OnCalculationChanged(calculationChangedEventArgs);
                _strCalculationError = calculationChangedEventArgs._strParserError;
                _nCalculationErrorColumn = calculationChangedEventArgs._nParserErrorColumn;
                _cCalculationErrorColumns = calculationChangedEventArgs._cParserErrorColumns;
                IsValid = (_strCalculationError == string.Empty);
                {
                    ImageList il = (ImageList)_Calculation_PictureBox.Tag;
                    _Calculation_PictureBox.Image = il.Images[(IsValid == false) ? 0 : 1];
                    //_Calculation_PictureBox.Enabled = (IsValid == false);
                }
            }
            return;
        } // Calculation_RichTextBox_TextChanged)

        private void Calculation_RichTextBox_Validating(object sender, CancelEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (rtb != null)
            {
                string strSource = rtb.Text;
                if (strSource != string.Empty)
                {
                    bool bFailed = (_strCalculationError != string.Empty);
                    if (bFailed)
                    {
                        int nColumn = _nCalculationErrorColumn;
                        int cColumns = _cCalculationErrorColumns;
                        if ((nColumn >= 0) && (cColumns > 0) && ((nColumn + cColumns) <= strSource.Length))
                        {
                            UIBase.RichTestBox_MarkError(rtb, nColumn, cColumns);
                        }
                        else
                        {
                            UIBase.RichTestBox_MarkUnknownError(rtb);
                        }
                    }
                    else
                    {
                        rtb.Select(rtb.Text.Length, 0);
                    }
                } // if (strSource != string.Empty)
            }
            return;
        } // Calculation_RichTextBox_Validating()

        private void InputOutputSource_Resize(object sender, EventArgs e)
        {
            InputOutputSource ctlInputOutputSource = (InputOutputSource)sender;
            int nWidth = ctlInputOutputSource.Width;
            if (_Checked_Container != null) _Checked_Container.Width = nWidth;
            if (_Date_DateTimePicker != null) _Date_DateTimePicker.Width = nWidth;
            if (_FieldList_ComboBox != null) _FieldList_ComboBox.Width = nWidth;
            if (_Picklist_ComboBox != null) _Picklist_ComboBox.Width = nWidth;
            if (_Calculation_RichTextBox != null)
            {
                _Calculation_RichTextBox.Width = nWidth - _Calculation_PictureBox.Width - 2;
                _Calculation_PictureBox.Left = _Calculation_RichTextBox.Width;
            }
            if (_Text_RichTextBox != null) _Text_RichTextBox.Width = nWidth;
            return;
        } // InputOutputSource_Resize()

        private void Text_RichTextBox_Enter(object sender, EventArgs e)
        {
            Calculation_RichTextBox_Enter(sender, e);   // Reuse rather than copy/paste
            return;
        } // Text_RichTextBox_Enter()

        private void Text_RichTextBox_TextChanged(object sender, EventArgs e)
        {
            bool bIsValid = MappingTypeGeneric.CanParse(_eOutputType, _Text_RichTextBox.Text);
            if (bIsValid)
            {
                ;   // WJC Now check OutputSpecification because it may still be invalid even though it parses
            }
            IsValid = bIsValid;
            return;
        } // Text_RichTextBox_TextChanged)

        private void Text_RichTextBox_Validating(object sender, CancelEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (rtb != null)
            {
                if (rtb.Text != string.Empty)
                {
                    if (IsValid == false)
                    {
                        UIBase.RichTestBox_MarkError(rtb);
                    }
                } // if (rtb.Text != string.Empty)
            }
            return;
        } // Text_RichTextBox_Validating()

        #endregion
    } // class InputOutputSource
}
