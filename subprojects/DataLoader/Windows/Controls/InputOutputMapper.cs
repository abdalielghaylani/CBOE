using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Calculation.Parser;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Windows;
using CambridgeSoft.COE.DataLoader.Windows.Common;
using CambridgeSoft.COE.DataLoader.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// UI for the mapping of input field, etc. to output fields
    /// </summary>
    public partial class InputOutputMapper : UIBase
    {
        #region data
        // data members
        // fixed UI elements
        // Left
        private System.Windows.Forms.TableLayoutPanel _MainHeader;
        private System.Windows.Forms.Label[] _MainHeaderLabel;
        // variable UI elements (grid with 4 columns)
        private System.Windows.Forms.TableLayoutPanel _MainGrid;
        private System.Windows.Forms.Label[] _MainGridLabel;
        private System.Windows.Forms.Label[] _MainGridType;
        private System.Windows.Forms.ComboBox[] _MainGridComboBox;
        private InputOutputSource[] _MainGridInputOutputSource;
        // Right
        private System.Windows.Forms.TableLayoutPanel _InputFieldsHeader;
        private System.Windows.Forms.Label[] _InputFieldsLabel;
        // variable UI elements (grid with 3 columns)
        private System.Windows.Forms.TableLayoutPanel _InputFieldsGrid;
        private System.Windows.Forms.Label[] _InputFieldsGridLabel;
        private System.Windows.Forms.Label[] _InputFieldsGridType;
        private System.Windows.Forms.Label[] _InputFieldsGridUseage;
        // Bottom
        private readonly System.Windows.Forms.Button _LoadButton;
        private readonly System.Windows.Forms.Button _PreviewButton;
        private readonly System.Windows.Forms.Button _SaveButton;
        private readonly PersistSettings _SettingsForm = new PersistSettings("InputOutputMapper");
        // for properties to interact
        private string _xmlInputFieldSpec;    // set InputFieldSpec
        private string _xmlOutputFieldSpec;  // set OutputFieldSpec
        // more effecient then reparsing _xmlInputFieldSpec repeatedly ?
        private List<string> InputFieldNameList;
        private List<string> InputFieldTypeList;
        // for calculation parsing and error tracking
        private CalculationParser _Parser = null;
        // for calculation and potential preview
        private DataSet _DataSetPreview = null;
        //
        protected static string ctlTargetFieldName = null;
        private int _cInvalid = 0;
        private Dictionary<string, int> _dictMapsFieldUseage = new Dictionary<string, int>();
        private Dictionary<string, int> _dictCalculationsFieldUseage = new Dictionary<string, int>();
        #endregion

        #region properties
        /// <summary>
        /// Set output DataSet (assumed DataColumns and no DataRows); so calculations know which fields will be available.
        /// </summary>
        public DataSet DataSetPreview
        {
            set
            {
                _DataSetPreview = value;
                if (_DataSetPreview == null)
                {
                    return; // WJC TODO throw fit instead
                }
                // See InputFileLabelAndType for binding example
                return;
            }
        }

        /// <summary>
        /// <c>InputFieldSpec</c>
        /// <para>This property is used to set the list of input object labels that are availble for mapping.</para>
        /// <para>The input label requirements are determined by the current input object.</para>
        /// <value>Is a List of strings. Each string is a semicolon separated pair of values.</value>
        /// Note that <c>OutputFieldSpec</c> must also be set before the UI is built and <c>InputOutputMapper</c> is ready to use.
        /// <seealso cref="OutputFieldSpec"/>
        /// </summary>
        public string InputFieldSpec
        {
            set
            {
                _xmlInputFieldSpec = value;
                InputFieldNameList = new List<string>();
                InputFieldTypeList = new List<string>();
                if (!string.IsNullOrEmpty(_xmlInputFieldSpec))
                {
                    XmlDocument oXmlDocumentInput = new XmlDocument();
                    oXmlDocumentInput.LoadXml(_xmlInputFieldSpec);
                    XmlNode oXmlNodeInputFieldlists = oXmlDocumentInput.DocumentElement;
                    foreach (XmlNode oXmlNodeInputField in oXmlNodeInputFieldlists)
                    {
                        if (oXmlNodeInputField.Attributes.GetNamedItem("name") != null)
                        {
                            string strName = oXmlNodeInputField.Attributes["name"].Value;
                            InputFieldNameList.Add(strName);
                        }
                        if (oXmlNodeInputField.Attributes.GetNamedItem("type") != null)
                        {
                            string strType = oXmlNodeInputField.Attributes["type"].Value;
                            InputFieldTypeList.Add(strType);
                        }
                    }
                }
                BuildUI();
            }
        }

        /// <summary>
        /// Get property to return invalid count
        /// Set property to set invalid count
        /// Has side-effect on Button Enabled properties
        /// </summary>
        private int Invalid
        {
            get
            {
                return _cInvalid;
            }
            set
            {
                _cInvalid = value;
                bool bEnabled = ((_dictMapsFieldUseage.Count > 0) && (_dictMapsFieldUseage.ContainsKey(string.Empty) == false) && (Invalid == 0));
                if (AcceptButton != null) AcceptButton.Enabled = bEnabled;
                if (_SaveButton != null) _SaveButton.Enabled = bEnabled;
                return;
            }
        }

        /// <summary>
        /// Get property to return the output mappings
        /// Set property to restore the output mappings
        /// ! See also Save and Load
        /// </summary>
        public string Mappings
        {
            get
            {
                string xmlMappings;
                if (_MainGridInputOutputSource != null)
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlists");
                    string strName = null;
                    for (int nOutputField = 0; nOutputField < _MainGridInputOutputSource.Length; nOutputField++)
                    {
                        string strOutputFormName = _MainGridInputOutputSource[nOutputField].OutputFormName;
                        if ((strName == null) || (strName != strOutputFormName))
                        {
                            if (strName != null)
                            {
                                oCOEXmlTextWriter.WriteEndElement();   // fieldlist
                            }
                            strName = strOutputFormName;
                            oCOEXmlTextWriter.WriteStartElement("fieldlist");
                            oCOEXmlTextWriter.WriteAttributeString("name", strName);
                        }
                        string strFields = string.Empty;
                        if (_Parser != null) {
                            Dictionary<string, int> dictFieldUseage = _Parser.CalculationGetFieldCounts(Convert.ToString(nOutputField));
                            foreach (KeyValuePair<string, int> kvp in dictFieldUseage)
                            {
                                if (strFields.Length > 0) strFields += ";";
                                strFields += kvp.Key;
                            }
                        }
                        _MainGridInputOutputSource[nOutputField].GetMapping(oCOEXmlTextWriter, strFields);
                    }
                    if (strName != null)
                    {
                        oCOEXmlTextWriter.WriteEndElement();   // fieldlist
                    }
                    oCOEXmlTextWriter.WriteEndElement();   // fieldlists
                    xmlMappings = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
                else
                {
                    throw new Exception("Why did I code this path?");
#if WHY
                    XmlDocument oXmlDocumentFieldSpec = new XmlDocument();
                    oXmlDocumentFieldSpec.LoadXml(_xmlInputFieldSpec);
                    XmlNode oXmlNodeFieldSpecRoot = oXmlDocumentFieldSpec.DocumentElement;
                    XmlNodeList oXmlNodeListFields = oXmlNodeFieldSpecRoot.SelectNodes("child::field");
                    foreach (XmlNode oXmlNodeField in oXmlNodeListFields)
                    {
                        {
                            XmlAttribute oXmlAttribute = oXmlDocumentFieldSpec.CreateAttribute("source");
                            oXmlAttribute.Value = "map";
                            oXmlNodeField.Attributes.Append(oXmlAttribute);
                        }
                        {
                            XmlAttribute oXmlAttribute = oXmlDocumentFieldSpec.CreateAttribute("value");
                            oXmlAttribute.Value = oXmlNodeField.Attributes["name"].Value;
                            oXmlNodeField.Attributes.Append(oXmlAttribute);
                        }
                    }
                    xmlMappings = COEXmlTextWriter.Pretty(oXmlDocumentFieldSpec.OuterXml);
#endif
                }
                return xmlMappings;
            } // get
            set
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(value);
                XmlNode oXmlNodeRoot = oXmlDocument.DocumentElement;
                string strNotLoaded = string.Empty;
                XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::field");
                foreach (XmlNode oXmlNodeField in oXmlNodeList)
                {
                    string strFormName = oXmlNodeField.ParentNode.Attributes["name"].Value;
                    string strName = oXmlNodeField.Attributes["name"].Value;
                    strName = strName.Replace('?', 'o'); // WJC TODO need a better fix
                    int nOutputField = -1;
                    for (nOutputField = 0; nOutputField < _MainGridLabel.Length; nOutputField++)
                    {
                        if ((_MainGridInputOutputSource[nOutputField].OutputFormName == strFormName) && (_MainGridLabel[nOutputField].Text == strName))
                        {
                            string strSource = _MainGridInputOutputSource[nOutputField].MappingSet(oXmlNodeField.OuterXml);
                            InputOutputSource.Showing eShowing = _MainGridInputOutputSource[nOutputField].WhichShowing;
                            for (int nIndex = 0; nIndex < _MainGridComboBox[nOutputField].Items.Count; nIndex++)
                            {
                                if (((COEItemPair<InputOutputSource.Showing>)_MainGridComboBox[nOutputField].Items[nIndex]).Value == eShowing)
                                {
                                    _MainGridComboBox[nOutputField].SelectedIndex = nIndex;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (nOutputField == _MainGridLabel.Length)
                    {
                        if (strNotLoaded != string.Empty) strNotLoaded += ",";
                        strNotLoaded += strName;
                        continue;
                    }
                } // foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                if (strNotLoaded != string.Empty)
                {
                    MessageBox.Show("These fields could not be restored: " + strNotLoaded, "Load", MessageBoxButtons.OK);
                }
                return;
            } // set
        }

        /// <summary>
        /// <c>OutputFieldSpec</c>
        /// <para>This property is used to set the output field requirements.</para>
        /// <para>The output field requirements are determined by the current output object.</para>
        /// <value>Is an XML representation of the field requirements</value>
        /// Note that <c>InputListLabel</c> must also be set before the UI is built and <c>InputOutputMapper</c> is ready to use.
        /// <seealso cref="InputFieldSpec"/>
        /// </summary>
        public string OutputFieldSpec
        {
            set
            {
                _xmlOutputFieldSpec = value;
                if (_xmlOutputFieldSpec.Trim().Contains("<fieldlists />"))
                    _xmlOutputFieldSpec = string.Empty;

                BuildUI();
            }
        }

        /// <summary>
        /// Returns the Parser after initializing if necessary.
        /// </summary>
        private CalculationParser Parser
        {
            get
            {
                if (_Parser == null)
                {   // WJC should be gotten from the Output object (tricky!)
                    _Parser = new CalculationParser();
                    _Parser.TypeAddFunctions(Type.GetType("System.Boolean"));
                    _Parser.TypeAddFunctions(Type.GetType("System.Char"));
                    _Parser.TypeAddFunctions(Type.GetType("System.Convert"));
                    _Parser.TypeAddFunctions(Type.GetType("System.DateTime"));
                    _Parser.TypeAddFunctions(Type.GetType("System.Double"));
                    _Parser.TypeAddFunctions(Type.GetType("System.Int32"));
                    _Parser.TypeAddFunctions(Type.GetType("System.Math"));
                    _Parser.TypeAddFunctions(Type.GetType("System.String"));
                    //_Parser.ObjectAddFunctions(new MyFunctions());    // Add function exposed by OutputObject
                    DataViewManager dvm = new DataViewManager(_DataSetPreview);
                    _Parser.DataViewManagerAddFields(dvm);
                }
                return _Parser;
            }
        }
        /// <summary>
        /// Returns Field selected from Output fields Dropdown.
        /// </summary>
        public string selectedctlTargetFieldName
        {
            get
            {
                return ctlTargetFieldName;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// The <c>InputOutputMapper</c> constructor created the fixed portion of the UI.
        /// The rest of the UI is built after the <c>InputFieldSpec</c> and <c>OutputFieldSpec</c> have been set.
        /// <seealso cref="InputFieldSpec"/>
        /// <seealso cref="OutputFieldSpec"/>
        /// </summary>
        public InputOutputMapper()
        {
            StatusText = "Map desired values to the output fields";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _MainHeader
            _MainHeader = UIBase.GetTableLayoutPanel();
            {
                _MainHeader.AutoScroll = false;
                _MainHeader.AutoSize = false;
                _MainHeader.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _MainHeader.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                _MainHeader.Height = 1;   // Set a small amount, it will grow
                // Rows
                _MainHeader.RowCount = 1;
                _MainHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute));
                // Columns
                _MainHeader.ColumnCount = 4;
                _MainHeaderLabel = new System.Windows.Forms.Label[_MainHeader.ColumnCount];
                // Column 0
                _MainHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainHeaderLabel[0] = UIBase.GetHeaderLabel();
                _MainHeaderLabel[0].BorderStyle = BorderStyle.Fixed3D;
                _MainHeaderLabel[0].Margin = new System.Windows.Forms.Padding(0);
                _MainHeaderLabel[0].Dock = DockStyle.Fill;
                _MainHeaderLabel[0].TextAlign = ContentAlignment.MiddleLeft;
                _MainHeaderLabel[0].Text = "Output Field";
                _MainHeader.Controls.Add(_MainHeaderLabel[0], 0, 0);
                // Column 1
                _MainHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainHeaderLabel[1] = UIBase.GetHeaderLabel();
                _MainHeaderLabel[1].BorderStyle = BorderStyle.Fixed3D;
                _MainHeaderLabel[1].Margin = new System.Windows.Forms.Padding(0);
                _MainHeaderLabel[1].Dock = DockStyle.Fill;
                _MainHeaderLabel[1].TextAlign = ContentAlignment.MiddleLeft;
                _MainHeaderLabel[1].Text = "Type";
                _MainHeader.Controls.Add(_MainHeaderLabel[1], 1, 0);
                // Column 2
                _MainHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainHeaderLabel[2] = UIBase.GetHeaderLabel();
                _MainHeaderLabel[2].BorderStyle = BorderStyle.Fixed3D;
                _MainHeaderLabel[2].Margin = new System.Windows.Forms.Padding(0);
                _MainHeaderLabel[2].Dock = DockStyle.Fill;
                _MainHeaderLabel[2].TextAlign = ContentAlignment.MiddleLeft;
                _MainHeaderLabel[2].Text = "Value From";
                _MainHeader.Controls.Add(_MainHeaderLabel[2], 2, 0);
                // Column 3
                _MainHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainHeaderLabel[3] = UIBase.GetHeaderLabel();
                _MainHeaderLabel[3].BorderStyle = BorderStyle.Fixed3D;
                _MainHeaderLabel[3].Margin = new System.Windows.Forms.Padding(0);
                _MainHeaderLabel[3].Dock = DockStyle.Fill;
                _MainHeaderLabel[3].TextAlign = ContentAlignment.MiddleLeft;
                _MainHeaderLabel[3].Text = "Value";
                //
                _MainHeader.Height = _MainHeader.Margin.Top + _MainHeaderLabel[3].Height + _MainHeader.Margin.Bottom;
                _MainHeader.Controls.Add(_MainHeaderLabel[3], 3, 0);
                Controls.Add(_MainHeader);
            }
            // _MainGrid
            _MainGrid = UIBase.GetTableLayoutPanel();
            {
                // Grid
                _MainGrid.AutoScroll = false;
                _MainGrid.AutoSize = false;
                _MainGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _MainGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                // Columns
                _MainGrid.ColumnCount = _MainHeader.ColumnCount;
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute));
                _MainGrid.Scroll += new ScrollEventHandler(MainGrid_Scroll);
                // Rows (added later)
                Controls.Add(_MainGrid);
            }
            // _MainHeader
            _InputFieldsHeader = UIBase.GetTableLayoutPanel();
            {
                _InputFieldsHeader.AutoScroll = false;
                _InputFieldsHeader.AutoSize = false;
                _InputFieldsHeader.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _InputFieldsHeader.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                _InputFieldsHeader.Height = _MainHeader.Height;   // Match Lft header
                // Rows
                _InputFieldsHeader.RowCount = 1;
                _InputFieldsHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute));
                // Columns
                _InputFieldsHeader.ColumnCount = 3;
                _InputFieldsLabel = new System.Windows.Forms.Label[_InputFieldsHeader.ColumnCount];
                // Column 0
                _InputFieldsHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _InputFieldsLabel[0] = UIBase.GetHeaderLabel();
                _InputFieldsLabel[0].BorderStyle = BorderStyle.Fixed3D;
                _InputFieldsLabel[0].Margin = new System.Windows.Forms.Padding(0);
                _InputFieldsLabel[0].Dock = DockStyle.Fill;
                _InputFieldsLabel[0].TextAlign = ContentAlignment.MiddleLeft;
                _InputFieldsLabel[0].Text = "Input Field";
                _InputFieldsHeader.Controls.Add(_InputFieldsLabel[0], 0, 0);
                //
                _InputFieldsHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _InputFieldsLabel[1] = UIBase.GetHeaderLabel();
                _InputFieldsLabel[1].BorderStyle = BorderStyle.Fixed3D;
                _InputFieldsLabel[1].Margin = new System.Windows.Forms.Padding(0);
                _InputFieldsLabel[1].Dock = DockStyle.Fill;
                _InputFieldsLabel[1].TextAlign = ContentAlignment.MiddleLeft;
                _InputFieldsLabel[1].Text = "Type";
                _InputFieldsHeader.Controls.Add(_InputFieldsLabel[1], 1, 0);
                //
                _InputFieldsHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _InputFieldsLabel[2] = UIBase.GetHeaderLabel();
                _InputFieldsLabel[2].BorderStyle = BorderStyle.Fixed3D;
                _InputFieldsLabel[2].Margin = new System.Windows.Forms.Padding(0);
                _InputFieldsLabel[2].Dock = DockStyle.Fill;
                _InputFieldsLabel[2].TextAlign = ContentAlignment.MiddleLeft;
                _InputFieldsLabel[2].Text = "Used?";
                _InputFieldsHeader.Controls.Add(_InputFieldsLabel[2], 2, 0);
                Controls.Add(_InputFieldsHeader);
            }
            // _InputFieldsGrid
            _InputFieldsGrid = UIBase.GetTableLayoutPanel();
            {
                // Grid
                _InputFieldsGrid.AutoScroll = false;
                _InputFieldsGrid.AutoSize = false;
                _InputFieldsGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _InputFieldsGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                // Columns
                _InputFieldsGrid.ColumnCount = _InputFieldsHeader.ColumnCount;
                _InputFieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _InputFieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _InputFieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute));
                _InputFieldsGrid.Scroll += new ScrollEventHandler(InputFieldsGrid_Scroll);
                // Rows (added later)
                Controls.Add(_InputFieldsGrid);
            }
            // _SaveButton
            _SaveButton = UIBase.GetButton(ButtonType.Save);
            Controls.Add(_SaveButton);
            // _LoadButton
            _LoadButton = UIBase.GetButton(ButtonType.Load);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            Controls.Add(_LoadButton);
            // _PreviewButton
            _PreviewButton = UIBase.GetButton(ButtonType.Preview);
//            Controls.Add(_PreviewButton); // WJC Not implemented yet
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            _SaveButton.Click += new EventHandler(SaveButton_Click);
            _LoadButton.Click += new EventHandler(LoadButton_Click);
            _PreviewButton.Click += new EventHandler(PreviewButton_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(InputOutputMapper_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // InputOutputMapper()
        #endregion

        #region methods
        /// <summary>
        /// <c>BuildUI</c>
        /// <para>Constructs the variable portion of the UI.</para>
        /// Note that <c>InputFieldSpec</c> and <c>OutputFieldSpec</c> must be set before <c>BuildUI</c> can be invoked.
        /// <seealso cref="InputFieldSpec"/>
        /// <seealso cref="OutputFieldSpec"/>
        /// </summary>
        private void BuildUI()
        {
            Visible = false;
            //
            if (!string.IsNullOrEmpty(_xmlOutputFieldSpec) && !string.IsNullOrEmpty(_xmlInputFieldSpec))
            {
                SuspendLayout();
                XmlDocument oXmlDocumentOutput = new XmlDocument();
                oXmlDocumentOutput.LoadXml(_xmlOutputFieldSpec);
                XmlNode oXmlNodeOutputFieldlists = oXmlDocumentOutput.DocumentElement;
                int cOutputFields = oXmlNodeOutputFieldlists.SelectNodes("descendant::field").Count;
                int cInputFields = InputFieldNameList.Count;
                _MainGridLabel = new System.Windows.Forms.Label[cOutputFields];
                _MainGridType = new System.Windows.Forms.Label[cOutputFields];
                _MainGridComboBox = new System.Windows.Forms.ComboBox[cOutputFields];
                _MainGridInputOutputSource = new InputOutputSource[cOutputFields];
                int[] xMaxLft = { -1, -1, -1, -1};
                for (int nColumn = 0; nColumn < xMaxLft.Length; nColumn++)
                {
                    Size size = TextRenderer.MeasureText(_MainHeaderLabel[nColumn].Text, _MainHeaderLabel[nColumn].Font);
                    xMaxLft[nColumn] = size.Width;
                }
                int yMax = -1;
                int nOutputField = -1;  // Will be preincremented
                _MainGrid.Controls.Clear();
                _MainGrid.RowCount = 0;
                foreach (XmlNode oXmlNodeOutputFieldlist in oXmlNodeOutputFieldlists)
                {
                    string sectionName = oXmlNodeOutputFieldlist.Attributes["name"].Value;
                    string sectionCaption = string.Empty;

                    //defend against "output-to-text" import options, which won't have gui sections
                    if (oXmlNodeOutputFieldlist.Attributes.GetNamedItem("caption") != null)
                        sectionCaption = oXmlNodeOutputFieldlist.Attributes["caption"].Value;
                    else
                        sectionCaption = sectionName;

                    if (sectionCaption.Length > 0)
                    {
                        _MainGrid.RowCount++;
                        _MainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                        System.Windows.Forms.Label lblSection = UIBase.GetLabel();
                        lblSection.Dock = DockStyle.Fill;
                        lblSection.Margin = new Padding(0);
                        lblSection.TextAlign = ContentAlignment.MiddleCenter;
                        lblSection.Text = sectionCaption;
                        _MainGrid.Controls.Add(lblSection, 0, _MainGrid.RowCount - 1);
                        Control ctl = _MainGrid.GetControlFromPosition(0, _MainGrid.RowCount - 1);
                        _MainGrid.SetColumnSpan(ctl, _MainGrid.ColumnCount);
                    }
                    //XmlNode oXmlNodeFields = oXmlNodeFieldlist.SelectSingleNode("fields");
                    //
                    // Rows
                    foreach (XmlNode oXmlNodeOutputField in oXmlNodeOutputFieldlist)
                    {
                        nOutputField++;
                        _MainGrid.RowCount++;
                        _MainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                        _MainGridInputOutputSource[nOutputField] = new InputOutputSource();
                        // Output Field
                        string strFieldName = oXmlNodeOutputField.Attributes["name"].Value.Trim();
                        string strFieldFormatPrefix = oXmlNodeOutputField.Attributes["format"].Value.Trim();
                        string strFieldFormatSuffix = string.Empty;
                        {
                            int indexOf = strFieldFormatPrefix.IndexOf('[');
                            if ((indexOf > 0) && (strFieldFormatPrefix.Substring(indexOf,2) != "[]"))
                            {
                                strFieldFormatSuffix = strFieldFormatPrefix.Substring(indexOf);
                                strFieldFormatPrefix = strFieldFormatPrefix.Remove(indexOf);
                            }
                        }
                        string strFieldFormatSpecification = oXmlNodeOutputField.Attributes["specification"].Value.Trim();
                        MappingTypeGeneric eOutputType = new MappingTypeGeneric(strFieldFormatPrefix);
                        _MainGridInputOutputSource[nOutputField].OutputFieldName = strFieldName;
                        _MainGridInputOutputSource[nOutputField].OutputFormName = oXmlNodeOutputField.ParentNode.Attributes["name"].Value.Trim();
                        _MainGridInputOutputSource[nOutputField].OutputType = strFieldFormatPrefix + strFieldFormatSuffix;
                        _MainGridInputOutputSource[nOutputField].OutputTypeSpecification = strFieldFormatSpecification;
                        _MainGridLabel[nOutputField] = UIBase.GetLabel();
                        _MainGridLabel[nOutputField].Height = _MainGridLabel[nOutputField].PreferredHeight;
                        _MainGridLabel[nOutputField].Margin = new Padding(3, 3, 3, 3);
                        _MainGridLabel[nOutputField].TextAlign = ContentAlignment.MiddleLeft;
                        _MainGridLabel[nOutputField].Text = strFieldName.Replace("&", "&&");
                        {
                            Size size = TextRenderer.MeasureText(_MainGridLabel[nOutputField].Text, _MainGridLabel[nOutputField].Font);
                            if (xMaxLft[0] < size.Width) xMaxLft[0] = size.Width;
                        }
                        _MainGrid.Controls.Add(_MainGridLabel[nOutputField], 0, _MainGrid.RowCount - 1);
                        _MainGridType[nOutputField] = UIBase.GetLabel();
                        _MainGridType[nOutputField].Height = _MainGridType[nOutputField].PreferredHeight;
                        _MainGridType[nOutputField].Margin = new Padding(3, 3, 3, 3);
                        _MainGridType[nOutputField].TextAlign = ContentAlignment.MiddleLeft;
                        _MainGridType[nOutputField].Text = strFieldFormatPrefix + strFieldFormatSuffix;
                        {
                            Size size = TextRenderer.MeasureText(_MainGridType[nOutputField].Text, _MainGridType[nOutputField].Font);
                            if (xMaxLft[1] < size.Width) xMaxLft[1] = size.Width;
                        }
                        _MainGrid.Controls.Add(_MainGridType[nOutputField], 1, _MainGrid.RowCount - 1);
                        //! bug if (yMax < _MainGridLabel[nOutputField].Height) yMax = _MainGridLabel[nOutputField].Height;
                        // How
                        _MainGridComboBox[nOutputField] = UIBase.GetComboBox();
                        _MainGridLabel[nOutputField].Height = _MainGridComboBox[nOutputField].PreferredHeight;
                        _MainGridType[nOutputField].Height = _MainGridComboBox[nOutputField].PreferredHeight;
                        _MainGridComboBox[nOutputField].DropDownStyle = ComboBoxStyle.DropDownList;
                        _MainGridComboBox[nOutputField].Tag = nOutputField;    // SelectedIndexChanged depends on Tag
                        _MainGridComboBox[nOutputField].GotFocus += MainGridComboBox_GotFocus;
                        _MainGridComboBox[nOutputField].SelectedIndexChanged += MainGridComboBox_SelectedIndexChanged;
                        XmlNode oXmlNodeSources = oXmlNodeOutputField.SelectSingleNode("sources");
                        string strDefault = oXmlNodeSources.Attributes["default"].Value;
                        foreach (XmlNode oXmlNodeSource in oXmlNodeSources)
                        {
                            string strAction;
                            InputOutputSource.Showing eShowing = InputOutputSource.Showing.Default;
                            switch (oXmlNodeSource.Name)
                            {
                                case "calculation":
                                    {
                                        strAction = "Calculation";
                                        eShowing = InputOutputSource.Showing.Calculation;
                                        _MainGridInputOutputSource[nOutputField].Calculation = string.Empty;
                                        _MainGridInputOutputSource[nOutputField].Tag = nOutputField;
                                        _MainGridInputOutputSource[nOutputField].CalculationChanged += new InputOutputSource.CalculationChangedEvent(MainGridInputOutputSource_CalculationChanged);
                                        break;
                                    }
                                case "checkbox":
                                    {
                                        strAction = "Checkbox";
                                        eShowing = InputOutputSource.Showing.Checkbox;
                                        _MainGridInputOutputSource[nOutputField].Checked = false;
                                        break;
                                    }
                                case "constant":
                                    {
                                        strAction = "Constant";
                                        eShowing = InputOutputSource.Showing.Textbox;
                                        _MainGridInputOutputSource[nOutputField].Text = string.Empty;
                                        break;
                                    }
                                case "date":
                                    {
                                        strAction = "Date";
                                        eShowing = InputOutputSource.Showing.Date;
                                        _MainGridInputOutputSource[nOutputField].Date = string.Empty;
                                        if (xMaxLft[3] < _MainGridInputOutputSource[nOutputField].Width) xMaxLft[3] = _MainGridInputOutputSource[nOutputField].Width;
                                        break;
                                    }
                                case "default":
                                    {
                                        strAction = "Default";
                                        eShowing = InputOutputSource.Showing.Default;
                                        break;
                                    }
                                case "map":
                                    {
                                        strAction = "Input field";
                                        eShowing = InputOutputSource.Showing.Map;
                                        string xmlValidInputFieldSpec;
                                        {
                                            XmlDocument oXmlDocumentInput = new XmlDocument();
                                            oXmlDocumentInput.LoadXml(_xmlInputFieldSpec);
                                            XmlNode oXmlNodeInputFieldlists = oXmlDocumentInput.DocumentElement;

                                            COEXmlTextWriter oCOEXmlTextWriter = oCOEXmlTextWriter = new COEXmlTextWriter();
                                            // open fieldlist
                                            oCOEXmlTextWriter.WriteStartElement("fieldlist");
                                            foreach (XmlNode oXmlNodeInputField in oXmlNodeInputFieldlists)
                                            {
                                                string strTypePrefix = oXmlNodeInputField.Attributes["type"].Value;
                                                string strTypeSuffix = string.Empty;
                                                {
                                                    int indexOf = strTypePrefix.IndexOf('[');
                                                    if ((indexOf > 0) && (strTypePrefix.Substring(indexOf,2) != "[]"))
                                                    {
                                                        strTypeSuffix = strTypePrefix.Substring(indexOf);
                                                        strTypePrefix = strTypePrefix.Remove(indexOf);
                                                    }
                                                }
                                                MappingTypeGeneric eInputType = new MappingTypeGeneric(strTypePrefix);
                                                if ((MappingTypeGeneric.CanAssign(eInputType, eOutputType)) && (strFieldFormatSuffix == strTypeSuffix))
                                                {
                                                    // open field
                                                    oCOEXmlTextWriter.WriteStartElement("field");
                                                    {
                                                        // (field) dbname
                                                        {
                                                            oCOEXmlTextWriter.WriteStartAttribute("dbname");
                                                            oCOEXmlTextWriter.WriteString(oXmlNodeInputField.Attributes["dbname"].Value);
                                                            oCOEXmlTextWriter.WriteEndAttribute();
                                                        }
                                                        // (field) name
                                                        {
                                                            oCOEXmlTextWriter.WriteStartAttribute("name");
                                                            oCOEXmlTextWriter.WriteString(oXmlNodeInputField.Attributes["name"].Value);
                                                            oCOEXmlTextWriter.WriteEndAttribute();
                                                        }
                                                    }
                                                    // close field
                                                    oCOEXmlTextWriter.WriteEndElement();
                                                }
                                            }
                                            // close fieldlist
                                            oCOEXmlTextWriter.WriteEndElement();
                                            xmlValidInputFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                                            oCOEXmlTextWriter.Close();
                                        }
                                        _MainGridInputOutputSource[nOutputField].FieldList = xmlValidInputFieldSpec;
                                        ComboBox cmb = _MainGridInputOutputSource[nOutputField].FieldList_ComboBox;
                                        cmb.Tag = nOutputField;
                                        cmb.SelectedIndexChanged += new EventHandler(MainGridInputOutputSource_SelectedIndexChanged);
                                        break;
                                    }
                                case "none":
                                    {
                                        strAction = "Not used";
                                        eShowing = InputOutputSource.Showing.None;
                                        break;
                                    }
                                case "picklist":
                                    {
                                        strAction = "Constant";
                                        eShowing = InputOutputSource.Showing.Picklist;
                                        _MainGridInputOutputSource[nOutputField].Picklist = oXmlNodeSource;
                                        break;
                                    }
                                default: { strAction = "*" + oXmlNodeSource.Name; break; }  // WJC ERROR
                            }
                            {
                                COEItemPair<InputOutputSource.Showing> kvp = new COEItemPair<InputOutputSource.Showing>(strAction, eShowing);
                                _MainGridComboBox[nOutputField].Items.Add(kvp);
                                if (oXmlNodeSource.Name == strDefault)
                                {
                                    _MainGridComboBox[nOutputField].SelectedIndex = _MainGridComboBox[nOutputField].Items.Count - 1;
                                }
                            }
                            {
                                Size size = TextRenderer.MeasureText(strAction, _MainGridComboBox[nOutputField].Font);
                                if (xMaxLft[2] < size.Width) xMaxLft[2] = size.Width;
                            }
                        } // foreach (XmlNode oXmlNodeSource in oXmlNodeSources)
                        if (_MainGridInputOutputSource[nOutputField].FieldList_ComboBox != null)
                        {
                            // Choose mapping if there is a name match
                            // Giving preference to 'ms_' prefix
                            int nIndex = _MainGridInputOutputSource[nOutputField].FieldList_ComboBox.FindStringExact("ms_" +strFieldName);
                            if (nIndex < 0) nIndex = _MainGridInputOutputSource[nOutputField].FieldList_ComboBox.FindStringExact(strFieldName);
                            if (nIndex >= 0)
                            {
                                _MainGridComboBox[nOutputField].SelectedIndex = _MainGridComboBox[nOutputField].FindStringExact("Input field");
                                _MainGridInputOutputSource[nOutputField].FieldList_ComboBox.SelectedIndex = nIndex;
                            }
                        }
                        if (_MainGridComboBox[nOutputField].SelectedIndex == -1)
                        {
                            _MainGridComboBox[nOutputField].SelectedIndex = (_MainGridComboBox[nOutputField].Items.Count > 0) ? 0 : -1;
                        }
                        _MainGrid.Controls.Add(_MainGridComboBox[nOutputField], 2, _MainGrid.RowCount - 1);
                        if (yMax < _MainGridComboBox[nOutputField].Height) yMax = _MainGridComboBox[nOutputField].Height;
                        _MainGrid.Controls.Add(_MainGridInputOutputSource[nOutputField], 3, _MainGrid.RowCount - 1);
                        if (yMax < _MainGridInputOutputSource[nOutputField].Height) yMax = _MainGridInputOutputSource[nOutputField].Height;
                        // events
                        _MainGridInputOutputSource[nOutputField].GotFocus += new EventHandler(MainGridInputOutputSource_GotFocus);
                        _MainGridInputOutputSource[nOutputField].IsValidChanged += new InputOutputSource.IsValidChangedEvent(MainGridInputOutputSource_IsValidChanged);
                        _MainGridInputOutputSource[nOutputField].Validated += new EventHandler(MainGridInputOutputSource_Validated);
#if OBSOLETE
                        _MainGridInputOutputSource[nOutputField].Validating += new CancelEventHandler(MainGridInputOutputSource_Validating);
#endif
                    } // foreach (XmlNode oXmlNodeField in oXmlNodeFields)
                } // foreach (XmlNode oXmlNodeFieldlist in oXmlNodeFieldlists)

                int[] xMaxRht = { -1, -1, -1 };
                for (int nColumn = 0; nColumn < xMaxRht.Length; nColumn++)
                {
                    Size size = TextRenderer.MeasureText(_InputFieldsLabel[nColumn].Text, _InputFieldsLabel[nColumn].Font);
                    xMaxRht[nColumn] = size.Width;
                }
                { // Process input fields
                    _InputFieldsGrid.Controls.Clear();
                    _InputFieldsGrid.RowCount = 0;
                    _InputFieldsGridLabel = new System.Windows.Forms.Label[cInputFields];
                    _InputFieldsGridType = new System.Windows.Forms.Label[cInputFields];
                    _InputFieldsGridUseage = new System.Windows.Forms.Label[cInputFields];
                    for (int nInputField = 0; nInputField < cInputFields; nInputField++)
                    {
                        string strInputFieldName = InputFieldNameList[nInputField];
                        string strInputFieldType = InputFieldTypeList[nInputField];
                        // Compute Lft maximum ComboBox width
                        Size size = TextRenderer.MeasureText(strInputFieldName, _MainGridInputOutputSource[0].Font);
                        if (xMaxLft[3] < size.Width) xMaxLft[3] = size.Width;
                        // Add to Rht
                        _InputFieldsGrid.RowCount++;
                        _InputFieldsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                        //
                        _InputFieldsGridLabel[nInputField] = UIBase.GetLabel();
                        _InputFieldsGridLabel[nInputField].Height = _MainGridLabel[0].Height;  // WJC
                        _InputFieldsGridLabel[nInputField].Margin = new Padding(3, 3, 3, 3);
                        _InputFieldsGridLabel[nInputField].TextAlign = ContentAlignment.MiddleLeft;
                        _InputFieldsGridLabel[nInputField].Text = strInputFieldName.Replace("&", "&&");
                        {
                            Size InputSize = TextRenderer.MeasureText(_InputFieldsGridLabel[nInputField].Text, _InputFieldsGridLabel[nInputField].Font);
                            if (xMaxRht[0] < InputSize.Width) xMaxRht[0] = InputSize.Width;
                        }
                        _InputFieldsGrid.Controls.Add(_InputFieldsGridLabel[nInputField], 0, _InputFieldsGrid.RowCount - 1);
                        //
                        _InputFieldsGridType[nInputField] = UIBase.GetLabel();
                        _InputFieldsGridType[nInputField].Height = _MainGridLabel[0].Height;  // WJC
                        _InputFieldsGridType[nInputField].Margin = new Padding(3, 3, 3, 3);
                        _InputFieldsGridType[nInputField].TextAlign = ContentAlignment.MiddleLeft;
                        _InputFieldsGridType[nInputField].Text = strInputFieldType;
                        {
                            Size InputSize = TextRenderer.MeasureText(_InputFieldsGridType[nInputField].Text, _InputFieldsGridType[nInputField].Font);
                            if (xMaxRht[1] < InputSize.Width) xMaxRht[1] = InputSize.Width;
                        }
                        _InputFieldsGrid.Controls.Add(_InputFieldsGridType[nInputField], 1, _InputFieldsGrid.RowCount - 1);
                        //
                        _InputFieldsGridUseage[nInputField] = UIBase.GetLabel();
                        _InputFieldsGridUseage[nInputField].Height = _MainGridLabel[0].Height;  // WJC
                        _InputFieldsGridUseage[nInputField].Margin = new Padding(3, 3, 3, 3);
                        _InputFieldsGridUseage[nInputField].TextAlign = ContentAlignment.MiddleLeft;
                        _InputFieldsGridUseage[nInputField].Text = "No";
                        // No need to MeasureText
                        _InputFieldsGrid.Controls.Add(_InputFieldsGridUseage[nInputField], 2, _InputFieldsGrid.RowCount - 1);
                    } // for (int nInputField = 0; nInputField < cInputFields; nInputField++)
                }
                { // Adjust Lft
                    xMaxLft[2] += 24;  // WJC combobox button width
                    xMaxLft[3] += 24;  // WJC combobox button width
                    _MainHeaderLabel[0].Width = _MainHeader.Margin.Left + xMaxLft[0] + _MainHeader.Margin.Right;
                    _MainHeaderLabel[1].Width = _MainHeader.Margin.Left + xMaxLft[1] + _MainHeader.Margin.Right;
                    _MainHeaderLabel[2].Width = _MainHeader.Margin.Left + xMaxLft[2] + _MainHeader.Margin.Right;
                    _MainHeaderLabel[3].Update();
                    for (int nField = 0; nField < cOutputFields; nField++)
                    {
                        _MainGridLabel[nField].Width = xMaxLft[0];
                        _MainGridType[nField].Width = xMaxLft[1];
                        _MainGridComboBox[nField].Width = xMaxLft[2];
                        _MainGridInputOutputSource[nField].Width = xMaxLft[3];
                    }
                    yMax += _MainGrid.Padding.Top + _MainGrid.Margin.Top;
                    yMax += _MainGrid.Padding.Bottom + _MainGrid.Margin.Bottom;
                    for (int nRow = 0; nRow < _MainGrid.RowCount; nRow++)
                    {
                        _MainGrid.RowStyles[nRow].SizeType = SizeType.Absolute;
                        _MainGrid.RowStyles[nRow].Height = yMax;
                    }
                    // Size
                    _MainHeader.Width = _MainGrid.Width = _MainGridInputOutputSource[0].Bounds.Right + 1 + _MainGrid.Margin.Right + _MainGrid.Padding.Right; // +1 for border
                    _MainGrid.Height = _MainGrid.RowCount * (_MainGrid.Padding.Top + _MainGrid.Margin.Top + _MainGridInputOutputSource[0].Height + _MainGrid.Margin.Bottom + _MainGrid.Padding.Bottom);
                }
                { // Adjust Rht
                    _InputFieldsLabel[0].Width = _InputFieldsHeader.Margin.Left + xMaxRht[0] + _InputFieldsHeader.Margin.Right;
                    _InputFieldsLabel[1].Width = _InputFieldsHeader.Margin.Left + xMaxRht[1] + _InputFieldsHeader.Margin.Right;
                    _InputFieldsLabel[2].Width = _InputFieldsHeader.Margin.Left + xMaxRht[2] + _InputFieldsHeader.Margin.Right;
                    //_InputFieldsLabel[2].Update();
                    for (int nInputField = 0; nInputField < cInputFields; nInputField++)
                    {
                        _InputFieldsGridLabel[nInputField].Width = xMaxRht[0];
                        _InputFieldsGridType[nInputField].Width = xMaxRht[1];
                        _InputFieldsGridUseage[nInputField].Width = xMaxRht[2];
                    }
                    // Use yMax from Lft
                    for (int nRow = 0; nRow < _InputFieldsGrid.RowCount; nRow++)
                    {
                        _InputFieldsGrid.RowStyles[nRow].SizeType = SizeType.Absolute;
                        _InputFieldsGrid.RowStyles[nRow].Height = yMax;
                    }
                    // Size
                    _InputFieldsHeader.Width = _InputFieldsGrid.Width = _InputFieldsGridUseage[0].Bounds.Right + _InputFieldsGrid.Margin.Right + _InputFieldsGrid.Padding.Right + 1; // WJC not sure why +1
                    _InputFieldsGrid.Height = _InputFieldsGrid.RowCount * (_InputFieldsGrid.Padding.Top + _InputFieldsGrid.Margin.Top + _InputFieldsGridLabel[0].Height + _InputFieldsGrid.Margin.Bottom + _InputFieldsGrid.Padding.Bottom);
                }
                UpdateInputFieldsList();
                ResumeLayout(true);
                Visible = true;
            } // if (_xmlOutputFieldSpec.Length > 0)
            return;
        } // BuildUI()

        private void UpdateInputFieldsList()
        {
            if (_InputFieldsGridLabel != null)
            {
                // Recount field useage
                {
                    _dictCalculationsFieldUseage = new Dictionary<string, int>();
                    _dictMapsFieldUseage = new Dictionary<string, int>();
                    for (int nOutputField = 0; nOutputField < _MainGridInputOutputSource.Length; nOutputField++)
                    {
                        InputOutputSource.Showing eShowing = InputOutputSource.Showing.None;
                        if (_MainGridComboBox[nOutputField] != null) eShowing = ((COEItemPair<InputOutputSource.Showing>)_MainGridComboBox[nOutputField].SelectedItem).Value;
                        if ((eShowing == InputOutputSource.Showing.Calculation) && (_Parser != null))
                        {
                            string strCalculationName = Convert.ToString(nOutputField);
                            _Parser.CalculationIncrementFieldCounts(strCalculationName, ref _dictCalculationsFieldUseage);
                        }
                        else if (eShowing == InputOutputSource.Showing.Map)
                        {
                            string strMap = _MainGridInputOutputSource[nOutputField].FieldList_Value;
                            if (_dictMapsFieldUseage.ContainsKey(strMap) == false) _dictMapsFieldUseage.Add(strMap, 0);
                            _dictMapsFieldUseage[strMap]++;
                        }
                    }
                }
                // Update field list
                for (int nInputField = 0; nInputField < _InputFieldsGridLabel.Length; nInputField++)
                {
                    string strInputFieldName = _InputFieldsGridLabel[nInputField].Text;
                    string strUseage;
                    if ((_dictCalculationsFieldUseage.ContainsKey(strInputFieldName)) || (_dictMapsFieldUseage.ContainsKey(strInputFieldName)))
                    {
                        strUseage = "Yes";
                    }
                    else
                    {
                        strUseage = "No";
                    }
                    _InputFieldsGridUseage[nInputField].Text = strUseage;
                }
                // Update button states
                Invalid = Invalid;  // Side effect causes button enabled states to update
            } // if (_InputFieldsGridLabel != null)
            return;
        } // UpdateInputFieldsList()
        #endregion

        #region event handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            // Nothing to check but the OutputObject might reject
            OnAccept();
            return;
        } // AcceptButton_Click()

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()

        private void InputOutputMapper_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                _MainHeader.Top = y;
                _InputFieldsHeader.Top = y;
                y += _MainHeader.Bounds.Height;
                _MainGrid.Top = y;
                _InputFieldsGrid.Top = y;
                {   // AutoSize _MainGrid
                    int yMax = MaximumSize.Height;
                    yMax -= _MainHeader.Bounds.Height;
                    yMax -= UIBase.ExtraPadding.Top;
                    yMax -= _SaveButton.Height;
                    yMax -= AcceptButton.Height;
                    if (yMax > 0)
                    {
                        int cRowsMax;
                        int nHeight = 0;
                        if (_MainGrid.AutoScroll)
                        {
                            _MainGrid.AutoScroll = false;
                            _MainGrid.Width -= 16;
                        }
                        if (_MainGridInputOutputSource != null)
                        {
                            for (cRowsMax = 0; cRowsMax < _MainGridInputOutputSource.Length; cRowsMax++)
                            {
                                // WJC do this over taking advantage of the fixed row height. GetControlFromPosition ?
                                int nBottom = _MainGridComboBox[cRowsMax].Bounds.Bottom;
                                nBottom += _MainGrid.Margin.Bottom + 1;   // WJC 1 is cell border
                                if (nBottom > yMax)
                                {
                                    _MainGrid.AutoScroll = true;
                                    int nRow = _MainGrid.GetCellPosition(_MainGridComboBox[cRowsMax]).Row;
                                    _MainGrid.VerticalScroll.LargeChange = nRow * (_MainGrid.Margin.Top + _MainGridComboBox[cRowsMax].Bounds.Height + _MainGrid.Margin.Bottom + 1);
                                    _MainGrid.VerticalScroll.SmallChange = _MainGrid.Margin.Top + _MainGridComboBox[cRowsMax].Bounds.Height + _MainGrid.Margin.Bottom + 1;
                                    _MainGrid.Width += 16;
                                    break;
                                }
                                nHeight = nBottom;
                            }
                        }
                        _MainGrid.Height = nHeight;
                    }
                }
                {   // AutoSize _InputFieldsGrid
                    int yMax = MaximumSize.Height;
                    yMax -= _InputFieldsHeader.Bounds.Height;
                    yMax -= UIBase.ExtraPadding.Top;
                    yMax -= _SaveButton.Height;
                    yMax -= AcceptButton.Height;
                    if (yMax > 0)
                    {
                        int cRowsMax;
                        int nHeight = 0;
                        bool bAutoScroll = _InputFieldsGrid.AutoScroll; // incoming value
                        if (_InputFieldsGrid.AutoScroll)
                        {
                            _InputFieldsGrid.AutoScroll = false;
                            _InputFieldsGrid.Width -= 16;
                        }
                        if (_InputFieldsGridLabel != null)
                        {
                            for (cRowsMax = 0; cRowsMax < _InputFieldsGridLabel.Length; cRowsMax++)
                            {
                                // WJC do this over taking advantage of the fixed row height. GetControlFromPosition ?
                                int nBottom = _InputFieldsGridLabel[cRowsMax].Bounds.Bottom;
                                nBottom += _InputFieldsGrid.Margin.Bottom + 1;   // WJC 1 is cell border
                                if (nBottom > yMax)
                                {
                                    _InputFieldsGrid.AutoScroll = true;
                                    int nRow = _InputFieldsGrid.GetCellPosition(_InputFieldsGridLabel[cRowsMax]).Row;
                                    _InputFieldsGrid.VerticalScroll.LargeChange = nRow * (_InputFieldsGrid.Margin.Top + _InputFieldsGridLabel[cRowsMax].Bounds.Height + _InputFieldsGrid.Margin.Bottom + 1);
                                    _InputFieldsGrid.VerticalScroll.SmallChange = _InputFieldsGrid.Margin.Top + _InputFieldsGridLabel[cRowsMax].Bounds.Height + _InputFieldsGrid.Margin.Bottom + 1;
                                    _InputFieldsGrid.Width += 16;
                                    break;
                                }
                                nHeight = nBottom;
                            }
                            if (_InputFieldsGrid.AutoScroll != bAutoScroll)
                            {
                                int newWidth = _InputFieldsLabel[2].Width;
                                if (_InputFieldsGrid.AutoScroll) newWidth -= 16;
                                foreach (Label lbl in _InputFieldsGridUseage)
                                {
                                    lbl.Width = newWidth;
                                }
                                _InputFieldsGrid.ColumnStyles[2].Width = newWidth;
                            }
                        }
                        _InputFieldsGrid.Height = nHeight;
                    }
                }
                y += (_MainGrid.Bounds.Height > _InputFieldsGrid.Bounds.Height) ? _MainGrid.Bounds.Height : _InputFieldsGrid.Bounds.Height;
                y += UIBase.ExtraPadding.Top;
                _SaveButton.Top = y;
                _LoadButton.Top = y;
                _PreviewButton.Top = y;
                y += _SaveButton.Height + UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = MaximumSize.Height;    // Pin to the top
                
                // Horizontal
                int x = 0;
                _MainHeader.Left = x;
                _MainGrid.Left = x;
                // WJC Resize any preview column here (see InputFileLabelAndType)
                x += _MainGrid.Bounds.Width;
                x += 24;
                //_InputFieldsHeader.Left = x;  // deferred
                //_InputFieldsGrid.Left = x;    // deferred
                x += _InputFieldsGrid.Bounds.Width;
                Width = x;
                // Horizontal
                x = 0;
                _SaveButton.Left = x;
                x += _SaveButton.Width;
                _LoadButton.Left = x;
                x += _LoadButton.Width;
                _PreviewButton.Left = x;
                x += _PreviewButton.Width;
                if (Width < x) Width = x;
                // Horizontal
                x = 0;
                //CancelButton.Left = x;    // deferred
                x += CancelButton.Width;
                //AcceptButton.Left = x;    // deferred
                x += AcceptButton.Width;
                if (Width < x) Width = x;

                Width = MaximumSize.Width;
                x = Width;
                x -= _InputFieldsGrid.Bounds.Width;
                _InputFieldsHeader.Left = x;  // WJC
                _InputFieldsGrid.Left = x;    // WJC
                x -= 24;
                if (_MainGrid.Width != x)
                {
                    _MainHeader.Width = x;
                    _MainGrid.Width = x;
                    if (x > 0)
                    {

                        int newWidth = x - (_MainGridComboBox[0].Bounds.Right + _MainGridComboBox[0].Margin.Right);
                        if (this._MainGridInputOutputSource != null)
                        {
                            newWidth -= _MainGridInputOutputSource[0].Margin.Horizontal;
                            newWidth -= 1;  // border
                            if (_MainGrid.AutoScroll)
                            {
                                newWidth -= 16;
                            }
                            foreach (InputOutputSource ios in _MainGridInputOutputSource)
                            {
                                ios.Width = newWidth;
                            }
                            _MainGrid.ColumnStyles[3].Width = (newWidth > 0) ? newWidth : 0;
                        }
                    }
                }

                Width = MaximumSize.Width;
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
                MainGrid_Scroll(_MainGrid, new ScrollEventArgs(ScrollEventType.ThumbPosition, _MainGrid.VerticalScroll.Value));
                InputFieldsGrid_Scroll(_InputFieldsGrid, new ScrollEventArgs(ScrollEventType.ThumbPosition, _InputFieldsGrid.VerticalScroll.Value));
            } // if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            return;
        } // InputOutputMapper_Layout()

        private void LoadButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Load;
            _SettingsForm.ShowDialog(this);
            if (_SettingsForm.Settings.Length > 0)
            {
                Mappings = _SettingsForm.Settings;
            }
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            return;
        } // LoadButton_Click()

        private void InputFieldsGrid_Scroll(object sender, ScrollEventArgs e)
        {
            TableLayoutPanel oTableLayoutPanel = sender as TableLayoutPanel;
            if ((e.ScrollOrientation == ScrollOrientation.VerticalScroll) && (e.Type != ScrollEventType.ThumbTrack))
            {
                e.NewValue = (int)((double)(e.NewValue + 1) / oTableLayoutPanel.VerticalScroll.SmallChange) * oTableLayoutPanel.VerticalScroll.SmallChange;
                oTableLayoutPanel.VerticalScroll.Value = e.NewValue;
            }
            return;
        } // InputFieldsGrid_Scroll()

        private void MainGrid_Scroll(object sender, ScrollEventArgs e)
        {
            TableLayoutPanel oTableLayoutPanel = sender as TableLayoutPanel;
            if ((e.ScrollOrientation == ScrollOrientation.VerticalScroll) && (e.Type != ScrollEventType.ThumbTrack))
            {
                e.NewValue = (int)((double)(e.NewValue + 1) / oTableLayoutPanel.VerticalScroll.SmallChange) * oTableLayoutPanel.VerticalScroll.SmallChange;
                oTableLayoutPanel.VerticalScroll.Value = e.NewValue;
            }
            return;
        } // MainGrid_Scroll()

        private void MainGridComboBox_GotFocus(object sender, EventArgs e)
        {
            if ((_MainGrid.VerticalScroll.Value % _MainGrid.VerticalScroll.SmallChange) != 0)
            {
                MainGrid_Scroll(_MainGrid, new ScrollEventArgs(ScrollEventType.ThumbPosition, _MainGrid.VerticalScroll.Value + _MainGrid.VerticalScroll.SmallChange / 2));
            }
            return;
        } // MainGridComboBox_GotFocus()

        private void MainGridComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox cmb = (System.Windows.Forms.ComboBox)sender;
            InputOutputSource ctlTarget = _MainGridInputOutputSource[System.Convert.ToInt32(cmb.Tag)];
            ctlTargetFieldName = ctlTarget.OutputFieldName.ToString();
            InputOutputSource.Showing eShowing = ((COEItemPair<InputOutputSource.Showing>)cmb.SelectedItem).Value;
            if ((eShowing == InputOutputSource.Showing.Calculation) && (ctlTarget.Parser == null))
            {
                ctlTarget.Parser = Parser;
            }
            ctlTarget.WhichShowing = eShowing;
            if (ctlTarget.Enabled)
            {
                ctlTarget.Focus();
            } //Fix for CSBR 128492- Equivalents should be required when you are mapping a Fragment 
            if (ctlTarget.FieldList_Value.ToString() != "")
            {

                if ((ComboBox)ctlTarget.FieldList_ComboBox != null)
                {  
                    switch(ctlTargetFieldName)
                    {
                        case "Fragment 1": MessageBox.Show("Please select relevent Salt", "Fragments/Fragment Equivalents Mapping", MessageBoxButtons.OK); break;
                        case "Fragment 2": MessageBox.Show("Please select relevent Solvate", "Fragments/Fragment Equivalents Mapping", MessageBoxButtons.OK); break;
                        case "Fragment 1 Equivalents": MessageBox.Show("Please select relevent Salt Equivalent", "Fragments/Fragment Equivalents Mapping", MessageBoxButtons.OK); break;
                        case "Fragment 2 Equivalents": MessageBox.Show("Please select relevent Solvate Equivalent", "Fragments/Fragment Equivalents Mapping", MessageBoxButtons.OK); break;
                    }
                }
            }
            UpdateInputFieldsList();
            return;
        } // MainGridComboBox_SelectedIndexChanged()

        private void MainGridInputOutputSource_IsValidChanged(object sender, EventArgs e)
        {
            InputOutputSource inputOutputSource = sender as InputOutputSource;
            if (inputOutputSource != null)
            {
                if (inputOutputSource.IsValid)
                {
                    Invalid--;
                }
                else
                {
                    Invalid++;
                }
                return;
            }
        } // MainGridInputOutputSource_IsValidChanged()

        private void MainGridInputOutputSource_CalculationChanged(object sender, InputOutputSource.CalculationChangedEventArgs e)
        {
            InputOutputSource inputOutputSource = sender as InputOutputSource;
            {
                if (inputOutputSource == null)
                    throw new System.NullReferenceException();
                int nOutputField = Convert.ToInt32(inputOutputSource.Tag);   // Which field
                string strSource = e.Source;
                string strCalculationName = Convert.ToString(nOutputField);
                bool bFailed = false;
                if (strSource != string.Empty)
                {
                    bFailed = Parser.CalculationAdd(strCalculationName, e.Value, strSource);
                    if (bFailed)
                    {
                        e._strParserError = Parser.Error;
                        e._nParserErrorColumn = Parser.ErrorColumn;
                        e._cParserErrorColumns = Parser.ErrorColumns;
                    }
                }
                else
                {
                    Parser.CalculationRemove(strCalculationName);
                    e._strParserError = "The expression is empty";
                } // if (strSource != string.Empty)
                UpdateInputFieldsList();
            }
            return;
        } // MainGridInputOutputSource_CalculationChanged()

        // WJC Not currently raised - need to change InputOutputSource
        private void MainGridInputOutputSource_GotFocus(object sender, EventArgs e)
        {
            if ((_MainGrid.VerticalScroll.Value % _MainGrid.VerticalScroll.SmallChange) != 0)
            {
                MainGrid_Scroll(_MainGrid, new ScrollEventArgs(ScrollEventType.ThumbPosition, _MainGrid.VerticalScroll.Value + _MainGrid.VerticalScroll.SmallChange / 2));
            }
            return;
        } // MainGridInputOutputSource_GotFocus()

        private void MainGridInputOutputSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInputFieldsList();
            return;
        } //MainGridInputOutputSource_SelectedIndexChanged(

        private void MainGridInputOutputSource_Validated(object sender, EventArgs e)
        {
            InputOutputSource _MainGridInputOutputSource = sender as InputOutputSource;
            if (_MainGridInputOutputSource != null)
            {
                if (_MainGridInputOutputSource.IsEmpty)
                {
                    int nOutputField = Convert.ToInt32(_MainGridInputOutputSource.Tag);
                    int nDefault = _MainGridComboBox[nOutputField].FindStringExact("Default");
                    if (nDefault == -1) nDefault = _MainGridComboBox[nOutputField].FindStringExact("Not used"); ;
                    if (nDefault != -1) _MainGridComboBox[nOutputField].SelectedIndex = nDefault;
                }
            }
            return;
        } // MainGridInputOutputSource_Validated()

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("UNIMPLEMENTED");
            return;
        } // PreviewButton_Click()

        private void SaveButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Save;
            {
                string xmlMappings = Mappings;
                xmlMappings = COEXmlTextWriter.Pretty(xmlMappings); // OK InputOutputMapper::Save()
                _SettingsForm.Settings = xmlMappings;
            }
            _SettingsForm.ShowDialog(this);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            return;
        } // SaveButton_Click()

        #endregion

    } // class InputOutputMapper
}
