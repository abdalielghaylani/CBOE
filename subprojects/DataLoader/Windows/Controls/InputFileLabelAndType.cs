using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Windows;
using CambridgeSoft.COE.DataLoader.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// UI to allow labelling and type override for input fields
    /// </summary>
    public partial class InputFileLabelAndType : UIBase
    {
        #region >Properties and data<

        private DataSet _oDataSetPreview;

        //Controls
        private System.Windows.Forms.TextBox[] _PreviewTextBox;
        private System.Windows.Forms.Label _RecordLabel;
        private System.Windows.Forms.PictureBox _FirstButton;
        private System.Windows.Forms.PictureBox _PrevButton;
        private int _cRecordTextBox = 0;
        private System.Windows.Forms.TextBox _RecordTextBox;
        private System.Windows.Forms.PictureBox _NextButton;
        private System.Windows.Forms.PictureBox _LastButton;
        private System.Windows.Forms.Label _OfLabel;
        private BindingManagerBase _BindingManagerBase;

        //Header controls
        private System.Windows.Forms.TableLayoutPanel _HeaderGrid;
        private System.Windows.Forms.Label[] _HeaderLabel;

        //Main table
        private System.Windows.Forms.TableLayoutPanel _MainGrid;
        private System.Windows.Forms.TextBox[] _MainLabel;
        private System.Windows.Forms.ComboBox[] _MainComboBox;

        //Save/load
        private readonly PersistSettings _SettingsForm = new PersistSettings("InputFileLabelAndType");
        private readonly InputFileSort _SortForm = new InputFileSort();
        private readonly System.Windows.Forms.Button _LoadButton;
        private readonly System.Windows.Forms.Button _SaveButton;
        private readonly System.Windows.Forms.Button _SortButton;

        //Duplicate tracking
        private Dictionary<string, string> _dictUndo = new Dictionary<string, string>();
        private Dictionary<string, int> _dictCounts = new Dictionary<string, int>();


        private const String EMPTY_ELEMENT = "<fieldlist />";
        /// <summary>
        /// Set property to attach a DataSet for preview
        /// </summary>
        public DataSet DataSetPreview
        {
            set
            {
                _oDataSetPreview = value;

                if (_oDataSetPreview == null)
                {
                    this._dictCounts.Clear();
                    this._dictUndo.Clear();
                    this._BindingManagerBase = null;
                    if (this._PreviewTextBox != null)
                        RefreshPreviewGrid();
                }
                else
                {
                    // old style _BindingManagerBase = this.BindingContext[_oDataSetPreview, _oDataSetPreview.Tables[0].TableName];
                    _BindingManagerBase = this.BindingContext[_oDataSetPreview.Tables[0], null];
                    _BindingManagerBase.Position = 0;
                    _RecordTextBox.Tag = (Object)Convert.ToString(_BindingManagerBase.Position + 1);
                    _RecordTextBox.Text = _RecordTextBox.Tag.ToString();
                    _OfLabel.Text = "of " + Convert.ToString(_BindingManagerBase.Count) + " preview records";
                    _OfLabel.Width = _OfLabel.PreferredWidth;
                    _FirstButton.Enabled = _PrevButton.Enabled = (_BindingManagerBase.Position > 0) && (_BindingManagerBase.Count > 0);
                    _LastButton.Enabled = _NextButton.Enabled = ((_BindingManagerBase.Position + 1) < _BindingManagerBase.Count) && (_BindingManagerBase.Count > 0);
                }

                bool bNewDataSetPreview = (_oDataSetPreview != null);
                if (bNewDataSetPreview && this._PreviewTextBox != null)
                {
                    for (int nInputField = 0; nInputField < _PreviewTextBox.Length; nInputField++)
                    {
                        _PreviewTextBox[nInputField].DataBindings.Clear();
                        string[] strDbInfo = ((string)_MainLabel[nInputField].Tag).Split(';');
                        Binding b = new Binding("Text", _oDataSetPreview.Tables[0], strDbInfo[0], true, DataSourceUpdateMode.OnPropertyChanged);
                        if (strDbInfo[1] == "Time")
                        {
                            b.Format += new ConvertEventHandler(PreviewTextBox_Format_Time);
                        }
                        _PreviewTextBox[nInputField].DataBindings.Add(b);
                    }
                }
                if (this._BindingManagerBase != null)
                {
                    _BindingManagerBase.PositionChanged += new EventHandler(_BindingBaseManager_PositionChanged);
                }
                return;

            }
        } // DataSetPreview

        /// <summary>
        /// Get property to return XML of the input field list
        /// Set property to initialize the UI with the input field list
        /// </summary>
        public string InputFieldSort
        {
            get { return _SortForm.InputFieldSort; }
            set
            {
                //JED: why?
                return;

                _SortForm.InputFieldSort = value;
                return;
            }
        } // InputFieldSort

        /// <summary>
        /// Get property to return XML of the input field list
        /// Set property to initialize the UI with the input field list
        /// </summary>
        public string InputFieldSpec
        {
            get
            {
                COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                oCOEXmlTextWriter.WriteStartElement("fieldlist");
                for (int nInputField = 0; nInputField < _MainLabel.GetLength(0); nInputField++)
                {
                    if (_MainComboBox[nInputField] != null)
                    {
                        string[] strDbInfo = ((string)_MainLabel[nInputField].Tag).Split(';');
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("dbname", strDbInfo[0]);
                        oCOEXmlTextWriter.WriteAttributeString("dbtype", strDbInfo[1]);
                        if (_MainComboBox[nInputField].Enabled == false)
                        {
                            oCOEXmlTextWriter.WriteAttributeString("dbtypereadonly", "true");
                        }
                        oCOEXmlTextWriter.WriteAttributeString("name", _MainLabel[nInputField].Text.Trim());
                        oCOEXmlTextWriter.WriteAttributeString("type", _MainComboBox[nInputField].SelectedItem.ToString());
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                }
                oCOEXmlTextWriter.WriteEndElement();
                string xmlFieldSpec = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                oCOEXmlTextWriter.Close();
                return xmlFieldSpec;
            } // get
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(value);
                    XmlNode oXmlNodeFieldlist = oXmlDocument.DocumentElement;

                    //configure some controls
                    _MainGrid.RowCount = oXmlNodeFieldlist.ChildNodes.Count;
                    _MainLabel = new System.Windows.Forms.TextBox[_MainGrid.RowCount];
                    _MainComboBox = new System.Windows.Forms.ComboBox[_MainGrid.RowCount];
                    _PreviewTextBox = new System.Windows.Forms.TextBox[_MainGrid.RowCount];


                    // Rows
                    int nInputField = -1, nRowindex = 0;
                    int[] xMax = { -1, -1 };
                    for (int nColumn = 0; nColumn < 2; nColumn++)
                    {
                        Size size = TextRenderer.MeasureText(_HeaderLabel[nColumn].Text, _HeaderLabel[nColumn].Font);
                        xMax[nColumn] = size.Width;
                    }
                    string[] strTypeNames = MappingTypeGeneric.BasicNames();
                    string strNotLoaded = string.Empty;

                    _MainGrid.Controls.Clear();
                    foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist)
                    {
                        nInputField++;
                        nRowindex = nInputField;
                        // dbname
                        string strDbname = oXmlNodeField.Attributes["dbname"].Value;

                        //// lookup
                        //for (nInputField = 0; nInputField < _MainLabel.GetLength(0); nInputField++)
                        //{
                        //    if (_MainLabel[nInputField].Text == strDbname) break;
                        //}
                        //if (nInputField == _MainLabel.GetLength(0))
                        //{
                        //    if (strNotLoaded != string.Empty) strNotLoaded += ",";
                        //    strNotLoaded += strDbname;
                        //    continue;
                        //}

                        // other parts of oXmlNodeField
                        string strDbtypePrefix = oXmlNodeField.Attributes["dbtype"].Value;
                        bool bDbtypeReadonly = (oXmlNodeField.Attributes["dbtypereadonly"] != null) ? oXmlNodeField.Attributes["dbtypereadonly"].Value.ToLower() == "true" : false;
                        string strDbtypeSuffix = string.Empty;
                        {
                            int indexOf = strDbtypePrefix.IndexOf('[');
                            if ((indexOf > 0) && (strDbtypePrefix.Substring(indexOf, 2) != "[]"))
                            {
                                strDbtypeSuffix = strDbtypePrefix.Substring(indexOf);
                                strDbtypePrefix = strDbtypePrefix.Remove(indexOf);
                            }
                        }
                        string strName;
                        {
                            XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["name"];
                            strName = (oXmlAttribute != null) ? oXmlAttribute.Value : strDbname;
                        }
                        string strTypePrefix = strDbtypePrefix;
                        string strTypeSuffix = strDbtypeSuffix;
                        {
                            XmlAttribute oXmlAttribute = oXmlNodeField.Attributes["type"];
                            if (oXmlAttribute != null)
                            {
                                strTypePrefix = oXmlAttribute.Value;
                                {
                                    int indexOf = strTypePrefix.IndexOf('[');
                                    if ((indexOf > 0) && (strTypePrefix.Substring(indexOf, 2) != "[]"))
                                    {
                                        strTypeSuffix = strTypePrefix.Substring(indexOf);
                                        strTypePrefix = strTypePrefix.Remove(indexOf);
                                    }
                                }
                            }
                            else
                            {
                                MappingTypeDb eMappingType = new MappingTypeDb(strDbtypePrefix);
                                strTypePrefix = eMappingType.BasicTypeName;
                            }
                        }
                        // Store
                        // Label
                        _MainGrid.RowStyles.Clear();
                        _MainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

                        _MainLabel[nInputField] = UIBase.GetTextBox();
                        _MainLabel[nInputField].GotFocus += new EventHandler(MainLabel_GotFocus);
                        _MainLabel[nInputField].TextChanged += new EventHandler(MainLabel_TextChanged);
                        _MainLabel[nInputField].Tag = strDbname + ";" + strDbtypePrefix + strDbtypeSuffix; // Must be before .Text
                        _MainLabel[nInputField].Text = strName;
                        {
                            Size size = TextRenderer.MeasureText(_MainLabel[nInputField].Text, _MainLabel[nInputField].Font);
                            if (xMax[0] < size.Width) xMax[0] = size.Width;
                        }
                       // _MainGrid.Controls.Add(_MainLabel[nInputField], 0, nInputField);

                        //<!-- add start
                        // lookup
                        for (nInputField = 0; nInputField < _MainLabel.GetLength(0); nInputField++)
                        {
                            if (_MainLabel[nInputField] != null && _MainLabel[nInputField].Text == strDbname)
                                break;

                        }
                        if (nInputField == _MainLabel.GetLength(0))
                        {
                            if (strNotLoaded != string.Empty) strNotLoaded += ",";
                            strNotLoaded += strDbname;
                            //removing the textbox that is already added to the array since lookup for loop did not break
                            _MainLabel[nRowindex] = null;
                            //resetting the arraycount.
                            nInputField = nRowindex - 1;

                            continue;
                        }
                        //--> add end

                        _MainGrid.Controls.Add(_MainLabel[nInputField], 0, nInputField);

                        // Type
                        _MainComboBox[nInputField] = UIBase.GetComboBox();
                        _MainComboBox[nInputField].Enabled = (bDbtypeReadonly == false);
                        _MainComboBox[nInputField].DropDownStyle = ComboBoxStyle.DropDownList;
                        _MainComboBox[nInputField].GotFocus += new EventHandler(MainComboBox_GotFocus);
                        foreach (string strTypeName in strTypeNames)
                        {
                            if (strTypeName != "Error")
                            {
                                _MainComboBox[nInputField].Items.Add(strTypeName + strTypeSuffix);
                            }
                        }

                        _MainComboBox[nInputField].SelectedIndex = _MainComboBox[nInputField].FindStringExact(strTypePrefix + strTypeSuffix);
                        if (_MainComboBox[nInputField].SelectedIndex < 0)
                        {
                            _MainComboBox[nInputField].Items.Add("*" + strTypePrefix + strTypeSuffix);
                            _MainComboBox[nInputField].SelectedIndex = _MainComboBox[nInputField].Items.Count - 1;
                        }

                        if (_MainComboBox[nInputField].SelectedIndex < 0) _MainComboBox[nInputField].SelectedIndex = (_MainComboBox[nInputField].Items.Count > 0) ? 0 : -1;
                        _MainGrid.Controls.Add(_MainComboBox[nInputField], 1, nInputField);

                        // Preview
                        _PreviewTextBox[nInputField] = UIBase.GetTextBox();
                        {
                            if (_oDataSetPreview != null)
                            {
                                Binding b = new Binding("Text", _oDataSetPreview.Tables[0], strDbname);
                                if (strTypePrefix == "Time")
                                {
                                    b.Format += new ConvertEventHandler(PreviewTextBox_Format_Time);
                                }
                                _PreviewTextBox[nInputField].DataBindings.Add(b);
                            }
                        }
                        _PreviewTextBox[nInputField].Enabled = false;
                        _PreviewTextBox[nInputField].Dock = DockStyle.Fill;
                        _MainGrid.Controls.Add(_PreviewTextBox[nInputField], 2, nInputField);

                    } // foreach (XmlNode oXmlNodeField in oXmlNodeFieldlist.FirstChild)

                    if (strNotLoaded != string.Empty)
                    {
                        MessageBox.Show("These fields could not be restored: " + strNotLoaded, "Load", MessageBoxButtons.OK);
                    }
                    for (nInputField = 0; nInputField < _MainLabel.GetLength(0); nInputField++)
                    {
                        if (_MainComboBox[nInputField] != null)
                        {
                            for (int nType = 0; nType < _MainComboBox[nInputField].Items.Count; nType++)
                            {
                                Size size = TextRenderer.MeasureText(_MainComboBox[nInputField].Items[nType].ToString(), _MainComboBox[nInputField].Font);
                                if (xMax[1] < size.Width) xMax[1] = size.Width;
                            }
                        }
                    }
                    xMax[1] += 24;  // WJC combobox button width
                    _HeaderLabel[0].Width = _HeaderGrid.Margin.Left + xMax[0] + _HeaderGrid.Margin.Right;
                    _HeaderLabel[1].Width = _HeaderGrid.Margin.Left + xMax[1] + _HeaderGrid.Margin.Right;
                    for (nInputField = 0; nInputField < _MainLabel.GetLength(0); nInputField++)
                    {
                        if (_MainComboBox[nInputField] != null)
                        {
                            _MainLabel[nInputField].Width = xMax[0];
                            _MainComboBox[nInputField].Width = xMax[1];
                        }
                    }
                    _SortForm.InputFieldSpec = InputFieldSpec;

                }
                else
                {
                    _PreviewTextBox = null;
                    _cRecordTextBox = 0;
                }
                return;

            }
        }

        #endregion

        #region >Events, Delegates & Event-handlers<

        /// <summary>
        /// Delegate for SortChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SortChangedEvent(object sender, SortChangedEventArgs e);

        /// <summary>
        /// The SortChanged event is raised when the sort criteria changes
        /// </summary>
        public event SortChangedEvent SortChanged;

        private void OnSortChanged(SortChangedEventArgs e)
        {
            if (SortChanged != null)
            {
                SortChanged(this, e);
            }
            return;
        } // OnSortChanged()

        private void PreviewTextBox_Format_Time(object sender, ConvertEventArgs e)
        {
            string strValue = e.Value.ToString();
            int indexOf = strValue.IndexOf(' ');
            if (indexOf > 0)
            {
                e.Value = strValue.Substring(indexOf).Trim();
            }
            return;
        }

        private void FirstButton_Click(object sender, EventArgs e)
        {
            _BindingManagerBase.Position = 0;
            _FirstButton.Enabled = _PrevButton.Enabled = false;
            _LastButton.Enabled = _NextButton.Enabled = ((_BindingManagerBase.Position + 1) < _BindingManagerBase.Count);
            return;
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            _BindingManagerBase.Position--;
            _FirstButton.Enabled = _PrevButton.Enabled = (_BindingManagerBase.Position > 0);
            _LastButton.Enabled = _NextButton.Enabled = true;
            return;
        }

        private void RecordTextBox_Enter(object sender, EventArgs e)
        {
            _cRecordTextBox++;
            return;
        }

        private void RecordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RecordSet();
            }
            return;
        }

        private void RecordTextBox_Leave(object sender, EventArgs e)
        {
            _cRecordTextBox--;
            RecordSet();
            return;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            _BindingManagerBase.Position++;
            _FirstButton.Enabled = _PrevButton.Enabled = true;
            _LastButton.Enabled = _NextButton.Enabled = ((_BindingManagerBase.Position + 1) < _BindingManagerBase.Count);
            return;
        }

        private void LastButton_Click(object sender, EventArgs e)
        {
            _BindingManagerBase.Position = (_BindingManagerBase.Count - 1);
            _FirstButton.Enabled = _PrevButton.Enabled = (_BindingManagerBase.Position > 0);
            _LastButton.Enabled = _NextButton.Enabled = false;
            return;
        }

        private void _BindingBaseManager_PositionChanged(object sender, EventArgs e)
        {
            _RecordTextBox.Tag = Convert.ToString(_BindingManagerBase.Position + 1);
            _RecordTextBox.Text = _RecordTextBox.Tag.ToString();
            return;
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            //TODO Verify tat there are no duplicate labels. do not allow empty labels
            if (_cRecordTextBox == 0)
                OnAccept();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Load;
            _SettingsForm.ShowDialog(this);
            if (_SettingsForm.Settings.Length > 0)
            {
                InputFieldSpec = _SettingsForm.Settings;
            }

            // Disabling the Next button as there are not elements in the xml to process further.
            AcceptButton.Enabled = !InputFieldSpec.Contains(EMPTY_ELEMENT);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Save;
            {
                string xmlMappings = InputFieldSpec;
                xmlMappings = COEXmlTextWriter.Pretty(xmlMappings);
                _SettingsForm.Settings = xmlMappings;
            }
            _SettingsForm.ShowDialog(this);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
        }

        private void SortButton_Click(object sender, EventArgs e)
        {
            _SortForm.ShowDialog(this);
            if (_SortForm.InputFieldSortChanged)
            {
                string xmlInputFieldSort = InputFieldSort;
                OnSortChanged(new SortChangedEventArgs(xmlInputFieldSort));
            }
        }

        private void MainComboBox_GotFocus(object sender, EventArgs e)
        {
            if ((_MainGrid.VerticalScroll.Value % _MainGrid.VerticalScroll.SmallChange) != 0)
                MainGrid_Scroll(
                    _MainGrid
                    , new ScrollEventArgs(
                        ScrollEventType.ThumbPosition
                        , _MainGrid.VerticalScroll.Value + _MainGrid.VerticalScroll.SmallChange / 2
                    )
                );
        }

        private void MainGrid_Scroll(object sender, ScrollEventArgs e)
        {
            TableLayoutPanel oTableLayoutPanel = sender as TableLayoutPanel;
            if ((e.ScrollOrientation == ScrollOrientation.VerticalScroll) && (e.Type != ScrollEventType.ThumbTrack))
            {
                e.NewValue = (int)(
                    (double)(e.NewValue + 1) / oTableLayoutPanel.VerticalScroll.SmallChange
                ) * oTableLayoutPanel.VerticalScroll.SmallChange;
                oTableLayoutPanel.VerticalScroll.Value = e.NewValue;
            }
            return;
        }

        private void MainLabel_GotFocus(object sender, EventArgs e)
        {
            if ((_MainGrid.VerticalScroll.Value % _MainGrid.VerticalScroll.SmallChange) != 0)
                MainGrid_Scroll(
                    _MainGrid
                    , new ScrollEventArgs(
                        ScrollEventType.ThumbPosition
                        , _MainGrid.VerticalScroll.Value + _MainGrid.VerticalScroll.SmallChange / 2
                    )
                );
        }

        private void MainLabel_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string strText = textBox.Text.Trim();   // Note that we are trimming
                string[] strDbInfo = ((string)textBox.Tag).Split(';');

                if (_dictUndo.ContainsKey(strDbInfo[0]))
                {
                    string strPrev = _dictUndo[strDbInfo[0]];
                    _dictCounts[strPrev] = _dictCounts[strPrev] - 1;
                    if (_dictCounts[strPrev] == 0) _dictCounts.Remove(strPrev);
                    _dictUndo[strDbInfo[0]] = strText;
                }
                else
                {
                    _dictUndo.Add(strDbInfo[0], strText);
                }

                if (_dictCounts.ContainsKey(strText))
                    _dictCounts[strText] = _dictCounts[strText] + 1;
                else
                    _dictCounts.Add(strText, 1);

                AcceptButton.Enabled = _SaveButton.Enabled = (
                    (_dictCounts.Count == _dictUndo.Count)
                    && (_dictCounts.ContainsKey(string.Empty) == false)
                );

                if (textBox.Text != textBox.Text.TrimStart(' '))
                    //JED: somebody knew this wasn't a good idea...see comment on next line
                    textBox.Text = textBox.Text.TrimStart(' '); // Scary, but will not be infinite
            }
        }

        private void InputFileLabelAndType_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
                RefreshPreviewGrid();
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// Creates the controls required for the form.
        /// </summary>
        public InputFileLabelAndType()
        {
            this.StatusText = "Confirm input file data types; relabel input fields as desired";
            InitializeComponent();

            // Programmatically add control(s)
            this.SuspendLayout();

            // _HeaderGrid
            _HeaderGrid = UIBase.GetTableLayoutPanel();
            _HeaderGrid.AutoScroll = false;
            _HeaderGrid.AutoSize = false;
            _HeaderGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _HeaderGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            _HeaderGrid.Height = 1;   // Set a small amount, it will grow
            // Rows
            _HeaderGrid.RowCount = 1;
            _HeaderGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // Columns
            _HeaderGrid.ColumnCount = 3;
            _HeaderLabel = new System.Windows.Forms.Label[_HeaderGrid.ColumnCount];
            // Column 0
            _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            _HeaderLabel[0] = UIBase.GetHeaderLabel();
            _HeaderLabel[0].BorderStyle = BorderStyle.Fixed3D;
            _HeaderLabel[0].Margin = new System.Windows.Forms.Padding(0);
            _HeaderLabel[0].Dock = DockStyle.Fill;
            _HeaderLabel[0].TextAlign = ContentAlignment.MiddleLeft;
            _HeaderLabel[0].Text = "Label";
            _HeaderGrid.Controls.Add(_HeaderLabel[0], 0, 0);
            // Column 1
            _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            _HeaderLabel[1] = UIBase.GetHeaderLabel();
            _HeaderLabel[1].BorderStyle = BorderStyle.Fixed3D;
            _HeaderLabel[1].Margin = new System.Windows.Forms.Padding(0);
            _HeaderLabel[1].Dock = DockStyle.Fill;
            _HeaderLabel[1].TextAlign = ContentAlignment.MiddleLeft;
            _HeaderLabel[1].Text = "Type";
            _HeaderGrid.Controls.Add(_HeaderLabel[1], 1, 0);
            // Column 2
            _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            _HeaderLabel[2] = UIBase.GetHeaderLabel();
            _HeaderLabel[2].BorderStyle = BorderStyle.Fixed3D;
            _HeaderLabel[2].Margin = new System.Windows.Forms.Padding(0);
            _HeaderLabel[2].Dock = DockStyle.Fill;
            _HeaderLabel[2].TextAlign = ContentAlignment.MiddleLeft;
            _HeaderLabel[2].Text = "Preview";
            //
            _HeaderGrid.Height = _HeaderGrid.Margin.Top + _HeaderLabel[0].Height + _HeaderGrid.Margin.Bottom;
            _HeaderGrid.Controls.Add(_HeaderLabel[2], 2, 0);
            this.pnlHeader.Controls.Add(_HeaderGrid);
            _HeaderGrid.Dock = DockStyle.Fill;

            // _MainGrid
            _MainGrid = UIBase.GetTableLayoutPanel();
            // Grid
            _MainGrid.AutoScroll = true;
            //_MainGrid.AutoSize = true;
            _MainGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _MainGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            _MainGrid.Height = 1;   // Set a small amount, it will grow
            // Columns
            _MainGrid.ColumnCount = 3;
            _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            _MainGrid.Scroll += new ScrollEventHandler(MainGrid_Scroll);
            // Rows filled in later
            this.pnlMain.Controls.Add(_MainGrid);
            _MainGrid.Dock = DockStyle.Fill;

            // Navigation and Action buttons
            _SaveButton = UIBase.GetButton(ButtonType.Save);
            this.pnlNavigation.Controls.Add(_SaveButton);

            _LoadButton = UIBase.GetButton(ButtonType.Load);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            this.pnlNavigation.Controls.Add(_LoadButton);
            _LoadButton.Left = _SaveButton.Right + 5;

            _SortButton = UIBase.GetButton(ButtonType.Sort);
            this.pnlNavigation.Controls.Add(_SortButton);
            _SortButton.Left = _LoadButton.Right + 5;

            _RecordLabel = UIBase.GetLabel();
            _RecordLabel.TextAlign = ContentAlignment.MiddleLeft;
            _RecordLabel.Text = "Record:";
            _RecordLabel.Width = _RecordLabel.PreferredWidth;
            this.pnlNavigation.Controls.Add(_RecordLabel);
            _RecordLabel.Left = _SortButton.Right + 5;

            int navButtonWidth = 16;   // WJC was 32

            _FirstButton = UIBase.GetPictureBox(PictureBoxType.First);
            _FirstButton.Enabled = false;
            _FirstButton.Width = navButtonWidth;
            this.pnlNavigation.Controls.Add(_FirstButton);
            _FirstButton.Left = _RecordLabel.Right + 15;

            _PrevButton = UIBase.GetPictureBox(PictureBoxType.Prev);
            _PrevButton.Enabled = false;
            _PrevButton.Width = navButtonWidth;
            this.pnlNavigation.Controls.Add(_PrevButton);
            _PrevButton.Left = _FirstButton.Right + 5;

            _RecordTextBox = UIBase.GetTextBox();
            _RecordTextBox.TextAlign = HorizontalAlignment.Center;
            _RecordTextBox.Width = 48;   // WJC
            this.pnlNavigation.Controls.Add(_RecordTextBox);
            _RecordTextBox.Left = _PrevButton.Right + 5;

            _NextButton = UIBase.GetPictureBox(PictureBoxType.Next);
            _NextButton.Enabled = false;
            _NextButton.Width = navButtonWidth;
            this.pnlNavigation.Controls.Add(_NextButton);
            _NextButton.Left = _RecordTextBox.Right + 5;

            _LastButton = UIBase.GetPictureBox(PictureBoxType.Last);
            _LastButton.Enabled = false;
            _LastButton.Width = navButtonWidth;
            this.pnlNavigation.Controls.Add(_LastButton);
            _LastButton.Left = _NextButton.Right + 5;

            _OfLabel = UIBase.GetLabel();
            _OfLabel.TextAlign = ContentAlignment.MiddleLeft;
            _OfLabel.Text = "of";
            _OfLabel.Width = _OfLabel.PreferredWidth;
            this.pnlNavigation.Controls.Add(_OfLabel);
            _OfLabel.Left = _LastButton.Right + 5;

            // Accept and Cancel buttons
            this.pnlOkCancel.Controls.Add(AcceptButton);
            this.pnlOkCancel.Controls.Add(CancelButton);
            AcceptButton.Left = CancelButton.Right + 5;

            // events
            _SaveButton.Click += new EventHandler(SaveButton_Click);
            _LoadButton.Click += new EventHandler(LoadButton_Click);
            _SortButton.Click += new EventHandler(SortButton_Click);
            _FirstButton.Click += new EventHandler(FirstButton_Click);
            _PrevButton.Click += new EventHandler(PrevButton_Click);
            _RecordTextBox.Enter += new EventHandler(RecordTextBox_Enter);
            _RecordTextBox.KeyUp += new KeyEventHandler(RecordTextBox_KeyUp);
            _RecordTextBox.Leave += new EventHandler(RecordTextBox_Leave);
            _NextButton.Click += new EventHandler(NextButton_Click);
            _LastButton.Click += new EventHandler(LastButton_Click);
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);

            //JED:
            Layout += new LayoutEventHandler(InputFileLabelAndType_Layout);

            this.ResumeLayout();
            PerformLayout();
        }

        /// <summary>
        /// Provides for button-based navigator through the preview data.
        /// </summary>
        private void RecordSet()
        {
            if (_RecordTextBox.Text != _RecordTextBox.Tag.ToString())
            {
                try
                {
                    int nPosition = Convert.ToInt32(_RecordTextBox.Text) - 1;
                    if ((nPosition >= 0) && (nPosition < _BindingManagerBase.Count))
                    {
                        _BindingManagerBase.Position = nPosition;
                        _FirstButton.Enabled = _PrevButton.Enabled = (_BindingManagerBase.Position > 0) && (_BindingManagerBase.Count > 0);
                        _LastButton.Enabled = _NextButton.Enabled = ((_BindingManagerBase.Position + 1) < _BindingManagerBase.Count) && (_BindingManagerBase.Count > 0);
                    }
                    else
                        _RecordTextBox.Text = _RecordTextBox.Tag.ToString();  // WJC quiet reset (out of range)
                }
                catch
                {
                    _RecordTextBox.Text = _RecordTextBox.Tag.ToString();  // WJC quiet reset (invalid)
                }
            }
        }

        /// <summary>
        /// JED: Resets certain UI control properties to conform to the Input object fields list.
        /// </summary>
        public void RefreshPreviewGrid()
        {
            if (this.ParentForm != null)
            {

                this.SuspendLayout();

                int thisControlIndex = this.ParentForm.Controls.IndexOfKey("InputFileLabelAndType") - 1;
                this.ParentForm.Controls["InputFileLabelAndType"].Anchor =
                    AnchorStyles.Left & AnchorStyles.Top & AnchorStyles.Right & AnchorStyles.Bottom;
                this.Top = this.ParentForm.Controls[thisControlIndex].Bottom;

                this.ResumeLayout();
            }
        }
    }

    /// <summary>
    /// Information about calculation changed for use by <see cref="InputFileLabelAndType.SortChanged"/>
    /// </summary>
    public class SortChangedEventArgs : EventArgs
    {
        /// <summary>
        /// SortSpec
        /// </summary>
        private string _strSortSpec;

        /// <summary>
        /// Property to expose the Sort Spec
        /// </summary>
        public string SortSpec
        {
            get { return _strSortSpec; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vstrSortSpec"></param>
        public SortChangedEventArgs(string vstrSortSpec)
        {
            _strSortSpec = vstrSortSpec;
        }
    }
}
