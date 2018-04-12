using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Windows.Controls;

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
    /// <summary>
    /// Form to specify input file sorting
    /// </summary>
    public partial class InputFileSort : Form
    {
        #region data
        // data members
        private readonly PersistSettings _SettingsForm = new PersistSettings("InputFileSort");
        private bool _CenteredToOwner = false;
        private Point _Origin = new Point(-1, -1);
        private string _xmlInputFieldSort; // As set. Used for Cancel
        private readonly System.Windows.Forms.ListView _InputFieldSpecListView;
        private readonly System.Windows.Forms.Button _SaveButton;
        private readonly System.Windows.Forms.Button _LoadButton;
        private readonly System.Windows.Forms.Button _AddButton;
        private readonly System.Windows.Forms.Button _RemoveButton;
        private readonly System.Windows.Forms.ListView _InputFieldSortListView;
        private readonly System.Windows.Forms.ImageList _InputFieldSortImageList;
        private readonly System.Windows.Forms.Button _MoveUpButton;
        private readonly System.Windows.Forms.Button _MoveDownButton;
        private readonly System.Windows.Forms.Button _SortAscendingButton;
        private readonly System.Windows.Forms.Button _SortDescendingButton;
        private readonly System.Windows.Forms.Button _AcceptButton;
        private readonly System.Windows.Forms.Button _CancelButton;
        #endregion

        #region properties
        /// <summary>
        /// Set / get InputFieldSort
        /// </summary>
        public string InputFieldSort
        {
            get
            {
                string xmlRet = string.Empty;
                if (_InputFieldSortListView.Items.Count > 0)
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldsortlist");
                    foreach (ListViewItem lvi in _InputFieldSortListView.Items)
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("dbname", lvi.Text);
                        oCOEXmlTextWriter.WriteAttributeString("orderby", lvi.ImageKey);
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    oCOEXmlTextWriter.WriteEndElement();
                    xmlRet = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
                return xmlRet;
            }
            set
            {
                {   // Move all items from Sort to Spec
                    foreach (ListViewItem lvi in _InputFieldSortListView.Items)
                    {
                        lvi.ImageKey = string.Empty;
                        lvi.Remove();
                        _InputFieldSpecListView.Items.Add(lvi);
                    }
                }
                if (value.Length > 0)
                {
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(value);
                    XmlNode oXmlNodeFieldlist = oXmlDocument.SelectSingleNode("fieldsortlist");
                    string strNotLoaded = string.Empty;
                    foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                    {
                        string strColumnName = oXmlNode.Attributes["dbname"].Value.ToString();
                        string strOrderBy = oXmlNode.Attributes["orderby"].Value.ToString();
                        // Remove
                        ListViewItem lvi = _InputFieldSpecListView.FindItemWithText(strColumnName);
                        if (lvi == null)
                        {
                            if (strNotLoaded != string.Empty) strNotLoaded += ",";
                            strNotLoaded += strColumnName;
                            continue;
                        }
                        lvi.Remove();
                        // Add
                        lvi = new ListViewItem(strColumnName);
                        lvi.ImageKey = strOrderBy;
                        _InputFieldSortListView.Items.Add(lvi);
                    } // foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                    if (strNotLoaded != string.Empty)
                    {
                        MessageBox.Show("These fields could not be restored: " + strNotLoaded, "Load", MessageBoxButtons.OK);
                    }
                }
                _SaveButton.Enabled = (_InputFieldSortListView.Items.Count > 0);
                return;
            }
        } // InputFieldSort

        /// <summary>
        /// Get InputFieldSortChanged
        /// </summary>
        public bool InputFieldSortChanged
        {
            get
            {
                return (_xmlInputFieldSort != InputFieldSort);
            }
        } // InputFieldSortChanged

        /// <summary>
        /// Set InputFieldSpec
        /// </summary>
        public string InputFieldSpec
        {
            set
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(value);
                XmlNode oXmlNodeFieldlist = oXmlDocument.SelectSingleNode("fieldlist");
                int xMax = -1;
                _InputFieldSpecListView.Items.Clear();
                foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                {
                    string strColumnName = oXmlNode.Attributes["dbname"].Value.ToString();
                    ListViewItem lvi = new ListViewItem(strColumnName);
                    _InputFieldSpecListView.Items.Add(lvi);
                    int nWidth = TextRenderer.MeasureText(lvi.Text, lvi.Font).Width + 6;
                    if (xMax < nWidth) xMax = nWidth;
                } // foreach (XmlNode oXmlNode in oXmlNodeFieldlist)
                //Fix For CSBR-158069- Height of "Sort" window becomes larger than the screen size
                _InputFieldSpecListView.Height = Math.Min(527, (_InputFieldSpecListView.Items.Count > 0) ? _InputFieldSpecListView.GetItemRect(_InputFieldSpecListView.Items.Count - 1).Bottom : 0);
                _InputFieldSpecListView.Height += 1 + _InputFieldSpecListView.Margin.Bottom;
                _InputFieldSpecListView.Width = 16 + xMax;
                _InputFieldSpecListView.Columns[0].Width = _InputFieldSpecListView.Width - 4;

                _InputFieldSortListView.Items.Clear();
                _InputFieldSortListView.Height = _InputFieldSpecListView.Height;
                _InputFieldSortListView.Width = 16 + xMax;
                _InputFieldSortListView.Columns[0].Width = _InputFieldSortListView.Width - 4;
                return;
            }
        } // InputFieldSpec

        #endregion

        #region constructors
        /// <summary>
        /// Construct for to locally save settings
        /// </summary>
        public InputFileSort()
        {
            InitializeComponent();
            BackColor = UIBase.LightGray;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            SuspendLayout();

            _InputFieldSpecListView = UIBase.GetListView();

            _InputFieldSpecListView.View = View.Details;

            _InputFieldSpecListView.FullRowSelect = true;
            _InputFieldSpecListView.GridLines = true;
            _InputFieldSpecListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _InputFieldSpecListView.HideSelection = false;
            //Fix For CSBR-158069- Height of "Sort" window becomes larger than the screen size
            _InputFieldSpecListView.Scrollable = true;
            _InputFieldSpecListView.MultiSelect = false;
            _InputFieldSpecListView.Sorting = SortOrder.Ascending;

            _InputFieldSpecListView.Columns.Add("Fields");

            Controls.Add(_InputFieldSpecListView);

            // _SaveButton
            _SaveButton = UIBase.GetButton(UIBase.ButtonType.Save);
            _SaveButton.Enabled = false;
            Controls.Add(_SaveButton);
            // _LoadButton
            _LoadButton = UIBase.GetButton(UIBase.ButtonType.Load);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            Controls.Add(_LoadButton);

            _AddButton = UIBase.GetButton(UIBase.ButtonType.AddItem);
            _AddButton.Enabled = false;
            Controls.Add(_AddButton);
            _RemoveButton = UIBase.GetButton(UIBase.ButtonType.RemoveItem);
            _RemoveButton.Enabled = false;
            Controls.Add(_RemoveButton);

            _InputFieldSortListView = UIBase.GetListView();

            _InputFieldSortListView.View = View.Details;

            _InputFieldSortListView.FullRowSelect = true;
            _InputFieldSortListView.GridLines = true;
            _InputFieldSortListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _InputFieldSortListView.HideSelection = false;
            //Fix For CSBR-158069- Height of "Sort" window becomes larger than the screen size
            _InputFieldSortListView.Scrollable = true;
            _InputFieldSortListView.MultiSelect = false;
            _InputFieldSortListView.Sorting = SortOrder.None;

            _InputFieldSortImageList = new ImageList();
            _InputFieldSpecListView.SmallImageList = _InputFieldSortImageList;  // WJC this is a necessary hack to make the line heights equal
            _InputFieldSortListView.SmallImageList = _InputFieldSortImageList;

            _InputFieldSortListView.Columns.Add("Sort Fields");

            Controls.Add(_InputFieldSortListView);

            _MoveUpButton = UIBase.GetButton(UIBase.ButtonType.MoveUp);
            _MoveUpButton.Enabled = false;
            Controls.Add(_MoveUpButton);
            _MoveDownButton = UIBase.GetButton(UIBase.ButtonType.MoveDown);
            _MoveDownButton.Enabled = false;
            Controls.Add(_MoveDownButton);

            _SortAscendingButton = UIBase.GetButton(UIBase.ButtonType.SortAscending);
            _InputFieldSortImageList.Images.Add("Ascending", _SortAscendingButton.Image);
            _SortAscendingButton.Enabled = false;
            Controls.Add(_SortAscendingButton);
            _SortDescendingButton = UIBase.GetButton(UIBase.ButtonType.SortDescending);
            _InputFieldSortImageList.Images.Add("Descending", _SortDescendingButton.Image);
            _SortDescendingButton.Enabled = false;
            Controls.Add(_SortDescendingButton);

            AcceptButton = _AcceptButton = UIBase.GetButton(UIBase.ButtonType.Accept);
            Controls.Add(_AcceptButton);
            CancelButton = _CancelButton = UIBase.GetButton(UIBase.ButtonType.Cancel);
            Controls.Add(_CancelButton);

            _InputFieldSpecListView.SelectedIndexChanged += new EventHandler(InputFieldSpecListView_SelectedIndexChanged);
            _InputFieldSpecListView.DoubleClick += new EventHandler(InputFieldSpecListView_DoubleClick);
            _SaveButton.Click += new EventHandler(SaveButton_Click);
            _LoadButton.Click += new EventHandler(LoadButton_Click);
            _AddButton.Click += new EventHandler(AddButton_Click);
            _RemoveButton.Click += new EventHandler(RemoveButton_Click);
            _InputFieldSortListView.SelectedIndexChanged += new EventHandler(InputFieldSortListView_SelectedIndexChanged);
            _InputFieldSortListView.DoubleClick += new EventHandler(InputFieldSortListView_DoubleClick);
            _MoveUpButton.Click += new EventHandler(MoveUpButton_Click);
            _MoveDownButton.Click += new EventHandler(MoveDownButton_Click);
            _SortAscendingButton.Click += new EventHandler(SortAscendingButton_Click);
            _SortDescendingButton.Click += new EventHandler(SortDescendingButton_Click);
            _AcceptButton.Click += new EventHandler(AcceptButton_Click);
            _CancelButton.Click += new EventHandler(CancelButton_Click);
            FormClosed += new FormClosedEventHandler(InputFileSort_FormClosed);
            Layout += new LayoutEventHandler(InputFileSort_Layout);
            Shown += new EventHandler(InputFileSort_Shown);

            ResumeLayout(false);
            PerformLayout();
            return;
        } // InputFileSort()
        #endregion

        #region event handlers

        private void InputFileSort_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DL;
        }

        void InputFileSort_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Origin.X = Left;
            _Origin.Y = Top;
            return;
        } // InputFileSort_FormClosed

        void InputFieldSpecListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSpecListView.SelectedItems;
            _AddButton.Enabled = (slvic.Count > 0);
            return;
        } // InputFieldSpecListView_SelectedIndexChanged()

        void InputFieldSpecListView_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSpecListView.SelectedItems;
            if (slvic.Count > 0)
            {
                AddButton_Click(sender, e);
            }
            return;
        } // InputFieldSpecListView_DoubleClick()

        void AddButton_Click(object sender, EventArgs e)
        {
            {
                ListView.SelectedListViewItemCollection slvic = _InputFieldSpecListView.SelectedItems;
                foreach (ListViewItem lvi in slvic)
                {
                    lvi.Selected = false;
                    lvi.Remove();
                    lvi.ImageKey = "Ascending";
                    _InputFieldSortListView.Items.Add(lvi);
                }
            }
            InputFieldSortListView_SelectedIndexChanged(sender, e);
            _SaveButton.Enabled = (_InputFieldSortListView.Items.Count > 0);
            return;
        }  // AddButton_Click()

        void InputFieldSortListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
            if (slvic.Count > 0)
            {
                _RemoveButton.Enabled = true;
                ListViewItem lvi = slvic[0];
                _MoveUpButton.Enabled = (lvi.Index > 0);
                _MoveDownButton.Enabled = (lvi.Index < (_InputFieldSortListView.Items.Count - 1));
                _SortAscendingButton.Enabled = (lvi.ImageKey == "Descending");
                _SortDescendingButton.Enabled = (_SortAscendingButton.Enabled == false);
            }
            else
            {
                _RemoveButton.Enabled = false;
                _MoveUpButton.Enabled = false;
                _MoveDownButton.Enabled = false;
                _SortAscendingButton.Enabled = false;
                _SortDescendingButton.Enabled = false;
            }
            return;
        } // InputFieldSortListView_SelectedIndexChanged()

        void InputFieldSortListView_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
            if (slvic.Count > 0)
            {
                RemoveButton_Click(sender, e);
            }
            return;
        } // InputFieldSortListView_DoubleClick()

        void RemoveButton_Click(object sender, EventArgs e)
        {
            {
                ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
                foreach (ListViewItem lvi in slvic)
                {
                    lvi.Selected = false;
                    lvi.ImageKey = string.Empty;
                    lvi.Remove();
                    _InputFieldSpecListView.Items.Add(lvi);
                }
            }
            InputFieldSortListView_SelectedIndexChanged(sender, e);
            _SaveButton.Enabled = (_InputFieldSortListView.Items.Count > 0);
            return;
        }  // RemoveButton_Click()

        void InputFileSort_Shown(object sender, EventArgs e)
        {
            _xmlInputFieldSort = InputFieldSort;    // Save incoming sort list
            {
                ListView.SelectedListViewItemCollection slvic = _InputFieldSpecListView.SelectedItems;
                foreach (ListViewItem lvi in slvic)
                {
                    lvi.Selected = false;
                }
            }
            {
                ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
                foreach (ListViewItem lvi in slvic)
                {
                    lvi.Selected = false;
                }
            }
            if (_CenteredToOwner)
            {
                Left = _Origin.X;
                Top = _Origin.Y;
            }
            else
            {
                _CenteredToOwner = true;
                Point ptScreen = new Point(Owner.Left + (Owner.Width - Width) / 2, Owner.Top + (Owner.Height - Height) / 2);
                Left = ptScreen.X;
                Top = ptScreen.Y;
                _Origin.X = Left;
                _Origin.Y = Top;
            }
            return;
        } // InputFileSort_Shown()

        void MoveUpButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
            ListViewItem lvi = slvic[0];
            int nIndex = lvi.Index;
            lvi.Remove();
            _InputFieldSortListView.Items.Insert(nIndex - 1, lvi);
            lvi.Selected = true;
            return;
        }  // MoveUpButton_Click()

        void MoveDownButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
            ListViewItem lvi = slvic[0];
            int nIndex = lvi.Index;
            lvi.Remove();
            _InputFieldSortListView.Items.Insert(nIndex + 1, lvi);
            lvi.Selected = true;
            return;
        }  // MoveDownButton_Click()

        void SortAscendingButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
            ListViewItem lvi = slvic[0];
            lvi.ImageKey = "Ascending";
            InputFieldSortListView_SelectedIndexChanged(sender, e);
            return;
        }  // SortAscendingButton_Click()

        void SortDescendingButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _InputFieldSortListView.SelectedItems;
            ListViewItem lvi = slvic[0];
            lvi.ImageKey = "Descending";
            InputFieldSortListView_SelectedIndexChanged(sender, e);
            return;
        }  // SortDescendingButton_Click()

        private void SaveButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Save;
            {
                string xmlSorts = InputFieldSort;
                xmlSorts = COEXmlTextWriter.Pretty(xmlSorts); // OK
                _SettingsForm.Settings = xmlSorts;
            }
            _SettingsForm.ShowDialog(this);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            return;
        } // SaveButton_Click()

        private void LoadButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Load;
            _SettingsForm.ShowDialog(this);
            InputFieldSort = _SettingsForm.Settings;
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            return;
        } // LoadButton_Click()

        void AcceptButton_Click(object sender, EventArgs e)
        {
            Close();
            return;
        }  // AcceptButton_Click()

        void CancelButton_Click(object sender, EventArgs e)
        {
            InputFieldSort = _xmlInputFieldSort;    // Restore incoming sort specification
            return;
        }  // CancelButton_Click()

        private void InputFileSort_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && ((e.AffectedProperty == "Bounds") || (e.AffectedProperty == "Visible")))
            {
                // Vertical
                int y = 0;
                _InputFieldSpecListView.Top = y;
                _InputFieldSortListView.Top = y;
                y += _InputFieldSpecListView.Height;
                y += UIBase.ExtraPadding.Top;

                _AddButton.Top = y / 2 - _AddButton.Height;
                _RemoveButton.Top = y / 2 + _AddButton.Height;

                {
                    int maxy = _RemoveButton.Top + _RemoveButton.Height + UIBase.ExtraPadding.Top;
                    if (maxy < y) maxy = y;
                    _SaveButton.Top = maxy;
                    _LoadButton.Top = maxy;
                }
                _MoveUpButton.Top = y;
                y += _MoveUpButton.Height;
                _MoveDownButton.Top = y;
                y += _MoveDownButton.Height;
                y += UIBase.ExtraPadding.Top;
                _SortAscendingButton.Top = y;
                y += _SortAscendingButton.Height;
                _SortDescendingButton.Top = y;
                y += _SortDescendingButton.Height;
                y += UIBase.ExtraPadding.Top;

                _AcceptButton.Top = y;
                _CancelButton.Top = y;
                y += _AcceptButton.Height;

                // Horizontal
                int x = 0;
                int maxX = -1;
                _InputFieldSpecListView.Left = x;
                _SaveButton.Left = x;
                x += _SaveButton.Width;
                _LoadButton.Left = x;
                if (x < _InputFieldSpecListView.Width) x = _InputFieldSpecListView.Width;

                x += UIBase.ExtraPadding.Left;
                _AddButton.Left = x;
                _RemoveButton.Left = x;
                x += _AddButton.Width;
                x += UIBase.ExtraPadding.Right;

                _InputFieldSortListView.Left = x;
                _MoveUpButton.Left = x;
                _MoveDownButton.Left = x;
                _SortAscendingButton.Left = x;
                _SortDescendingButton.Left = x;
                _MoveUpButton.Width = _InputFieldSortListView.Width;
                _MoveDownButton.Width = _InputFieldSortListView.Width;
                _SortAscendingButton.Width = _InputFieldSortListView.Width;
                _SortDescendingButton.Width = _InputFieldSortListView.Width;
                x += _InputFieldSortListView.Width;
                if (maxX < x) maxX = x;
                x = 0;
                _AcceptButton.Left = x;
                x += _AcceptButton.Width;
                _CancelButton.Left = x;
                x += _CancelButton.Width;
                if (maxX < x) maxX = x;
                //x _InputFieldSpecListView.Width = maxX;    // WJC adjust

                x = maxX;
                x -= _CancelButton.Width;
                _CancelButton.Left = x;
                x -= _AcceptButton.Width;
                _AcceptButton.Left = x;

                if ((ClientSize.Width < maxX) || (ClientSize.Height < y)) ClientSize = new Size(maxX, y);

            }
            return;
        } // InputFileSort_Layout()

        #endregion
    } // class InputFileSort
}