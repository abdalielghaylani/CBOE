using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.NCDS_DataLoader.Controls;
using System.IO;

namespace CambridgeSoft.NCDS_DataLoader.Forms
{
    public partial class PersistSettings : Form
    {
        #region enums and types
        /// <summary>
        /// Indicates save versus load
        /// </summary>
        public enum DirectionType
        {
            /// <summary>
            /// Comma delimited
            /// </summary>
            Save,
            /// <summary>
            /// Tab delimited
            /// </summary>
            Load
        };
        #endregion

        #region data
        // data members
        private readonly System.Windows.Forms.Button _AcceptButton;
        private readonly System.Windows.Forms.Button _CancelButton;
        private DirectionType _DirectionType = DirectionType.Save;
        private readonly System.Windows.Forms.Label _FilenameLabel;
        private readonly System.Windows.Forms.TextBox _FilenameTextBox;
        //private readonly System.Windows.Forms.HScrollBar _HScrollBar;
        private readonly System.Windows.Forms.Button _RenameButton;
        private readonly System.Windows.Forms.Button _RemoveButton;
        private readonly System.Windows.Forms.ListView _FilenamesListView;
        private readonly string _ApplicationFolder = CambridgeSoft.COE.Framework.COEConfigurationService.COEConfigurationBO.ConfigurationBaseFilePath + Application.ProductName + @"\";
        private bool _CenteredToOwner = false;
        private DialogResult _DialogButton;
        private Point _Origin = new Point(-1, -1);
        private readonly string _SettingsFolder;
        private string _xmlSettings = string.Empty;
        #endregion

        #region properties

        /// <summary>
        /// Get count of available settings
        /// </summary>
        public int Count
        {
            get
            {
                return _FilenamesListView.Items.Count;
            }
        } // Count

        /// <summary>
        /// Get which dialog button was pressed
        /// </summary>
        public DialogResult DialogButton
        {
            get
            {
                return _DialogButton;
            }
        } // DialogButton

        /// <summary>
        /// Get / set Direction
        /// </summary>
        public DirectionType Direction
        {
            get
            {
                return _DirectionType;
            }
            set
            {
                _DirectionType = value;
                return;
            }
        } // Direction

        /// <summary>
        /// Get / set Settings
        /// </summary>
        public string Settings
        {
            get
            {
                return _xmlSettings;
            }
            set
            {
                _xmlSettings = value;
                return;
            }
        } // Settings
        #endregion

        #region constructors
        /// <summary>
        /// Construct for to locally save settings
        /// </summary>
        public PersistSettings(string vSettingsType)
        {
            _SettingsFolder = _ApplicationFolder;
            if (vSettingsType.Length > 0)
            {
                _SettingsFolder += vSettingsType;
                if (_SettingsFolder.EndsWith("\\") == false)
                {
                    _SettingsFolder += "\\";
                }
            }
            InitializeComponent();
            BackColor = UIBase.LightGray;
            SuspendLayout();

            _FilenamesListView = UIBase.GetListView();

            _FilenamesListView.View = View.Details;

            _FilenamesListView.FullRowSelect = true;
            _FilenamesListView.GridLines = true;
            _FilenamesListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _FilenamesListView.HideSelection = false;
            _FilenamesListView.MultiSelect = false;
            _FilenamesListView.Sorting = SortOrder.Ascending;
            _FilenamesListView.Columns.Add("Name");
            Controls.Add(_FilenamesListView);
            // Build list otherwise the initial Count is zero
            if (Directory.Exists(_SettingsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(_SettingsFolder);
                FileInfo[] fia = di.GetFiles("*.xml");
                foreach (FileInfo fi in fia)
                {
                    ListViewItem lvi = new ListViewItem(Path.GetFileNameWithoutExtension(fi.Name));
                    _FilenamesListView.Items.Add(lvi);
                }
            }

            _FilenameLabel = UIBase.GetLabel();
            _FilenameLabel.TextAlign = ContentAlignment.MiddleLeft;
            _FilenameLabel.Width = _FilenameLabel.PreferredWidth;
            Controls.Add(_FilenameLabel);
            _FilenameTextBox = UIBase.GetTextBox();
            _FilenameTextBox.Tag = _FilenameTextBox.SelectionStart + "|" + _FilenameTextBox.Text; // Initialize
            Controls.Add(_FilenameTextBox);
            _RenameButton = UIBase.GetButton(UIBase.ButtonType.Rename);
            _RenameButton.Enabled = false;
            Controls.Add(_RenameButton);
            _RemoveButton = UIBase.GetButton(UIBase.ButtonType.Remove);
            _RemoveButton.Enabled = false;
            Controls.Add(_RemoveButton);
            AcceptButton = _AcceptButton = UIBase.GetButton(UIBase.ButtonType.Accept);
            _AcceptButton.Enabled = (_FilenameTextBox.Text.Length > 0);
            Controls.Add(_AcceptButton);
            CancelButton = _CancelButton = UIBase.GetButton(UIBase.ButtonType.Cancel);
            Controls.Add(_CancelButton);

            _FilenamesListView.BeforeLabelEdit += new LabelEditEventHandler(FilenamesListView_BeforeLabelEdit);
            _FilenamesListView.AfterLabelEdit += new LabelEditEventHandler(FilenamesListView_AfterLabelEdit);
            _FilenamesListView.SelectedIndexChanged += new EventHandler(FilenamesListView_SelectedIndexChanged);
            _FilenameTextBox.TextChanged += new EventHandler(FilenameTextBox_TextChanged);
            _RemoveButton.Click += new EventHandler(RemoveButton_Click);
            _RenameButton.Click += new EventHandler(RenameButton_Click);
            _AcceptButton.Click += new EventHandler(AcceptButton_Click);
            _CancelButton.Click += new EventHandler(CancelButton_Click);

            FormClosed += new FormClosedEventHandler(PersistSettings_FormClosed);
            Layout += new LayoutEventHandler(PersistSettings_Layout);
            Shown += new EventHandler(PersistSettings_Shown);

            ResumeLayout(false);
            PerformLayout();

            return;
        }

        #endregion

        #region event handlers

        void PersistSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Origin.X = Left;
            _Origin.Y = Top;
            return;
        }

        void FilenamesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListView oListView = sender as ListView;
            string strBefore = _FilenamesListView.Items[e.Item].Text;
            if (e.Label != null)
            {
                string strAfter = e.Label.TrimStart();
                if (strAfter.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                {
                    Path.GetFileNameWithoutExtension(strAfter);
                    if (strBefore != strAfter)
                    {
                        if (File.Exists(_SettingsFolder + "\\" + strAfter + ".xml"))
                        {
                            if (strBefore.ToLower() != strAfter.ToLower())
                            {
                                MessageBox.Show("The setting '" + strAfter + "' already exists.", "Save settings rename", MessageBoxButtons.OK);
                                e.CancelEdit = true;
                            }
                        }
                        if (strAfter == string.Empty)
                        {
                            MessageBox.Show("The setting name cannot be blank.", "Save settings rename", MessageBoxButtons.OK);
                            e.CancelEdit = true;
                        }
                        if (e.CancelEdit == false)
                        {
                            try
                            {
                                File.Move(_SettingsFolder + "\\" + strBefore + ".xml", _SettingsFolder + "\\" + strAfter + ".xml");
                                ListViewItem lvi = oListView.SelectedItems[0];
                                lvi.Text = strAfter;
                                _FilenameTextBox.Text = strAfter;
                                lvi.Selected = true;
                            }
                            catch
                            {
                                MessageBox.Show("Unable to rename the setting.", "Save settings rename", MessageBoxButtons.OK);
                                e.CancelEdit = true;
                            }
                        }
                    }
                }
                else
                {
                    string strInvalid = string.Empty;
                    string strInvalidFileNameChars = new string(Path.GetInvalidFileNameChars());
                    for (int n = 0; n < e.Label.Length; n++)
                    {
                        string chThis = e.Label.Substring(n, 1);
                        int indexOf = strInvalidFileNameChars.IndexOf(chThis);
                        if (indexOf != -1)
                        {
                            if (strInvalid.Contains(chThis) == false)
                            {
                                strInvalid += chThis;
                            }
                        }
                    }
                    MessageBox.Show("Cannot rename the setting because of the following invalid character" + (strInvalid.Length > 1 ? "s" : string.Empty) + ":'" + strInvalid + "'.", "Save settings rename", MessageBoxButtons.OK);
                    e.CancelEdit = true;
                }
            }
            _RenameButton.Enabled = _RemoveButton.Enabled = _AcceptButton.Enabled = true;
            _FilenamesListView.LabelEdit = false;
            return;
        }

        void FilenamesListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            _RenameButton.Enabled = _RemoveButton.Enabled = _AcceptButton.Enabled = false;
            return;
        }

        void FilenamesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _FilenamesListView.SelectedItems;
            if (_bInTextChanged == false)
            {
                if (slvic.Count == 0)
                {
                    _FilenameTextBox.Text = string.Empty;
                }
                else
                {   // Assume 1 item
                    _FilenameTextBox.Text = slvic[0].Text;
                }
            }
            _RenameButton.Enabled = _RemoveButton.Enabled = (slvic.Count > 0);
            return;
        } // FilenamesListView_SelectedIndexChanged()

        private bool _bInTextChanged = false;
        void FilenameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_bInTextChanged == false)
            {
                _bInTextChanged = true;
                bool bOK = false;
                try
                {
                    bOK = Path.GetFileNameWithoutExtension(_FilenameTextBox.Text) == _FilenameTextBox.Text;
                }
                catch
                {
                    ;
                }
                if (bOK)
                {
                    _FilenameTextBox.Tag = _FilenameTextBox.SelectionStart + "|" + _FilenameTextBox.Text;
                    ListViewItem lviMatch = null;
                    foreach (ListViewItem lvi in _FilenamesListView.Items)
                    {
                        if (lvi.Text.ToLower() == _FilenameTextBox.Text.ToLower())
                        {
                            lviMatch = lvi;
                            break;
                        }
                    }
                    if (lviMatch == null)
                    {
                        ListView.SelectedListViewItemCollection slvic = _FilenamesListView.SelectedItems;
                        if (slvic.Count > 0)
                        {
                            slvic[0].Selected = false;
                        }
                    }
                    else
                    {
                        if (lviMatch.Selected == false)
                        {
                            lviMatch.Selected = true;
                        }
                    }
                }
                else
                {
                    string[] strTag = _FilenameTextBox.Tag.ToString().Split('|');
                    _FilenameTextBox.Text = strTag[1];
                    _FilenameTextBox.SelectionStart = Convert.ToInt32(strTag[0]);
                }
                _FilenameTextBox.Text = _FilenameTextBox.Text.TrimStart();
                _AcceptButton.Enabled = (_FilenameTextBox.Text.Length > 0);
                _bInTextChanged = false;
            }
            return;
        } // FilenameTextBox_TextChanged()

        void PersistSettings_Shown(object sender, EventArgs e)
        {
            if (_DirectionType == DirectionType.Save)
            {
                Text = "Save settings";
                _FilenameLabel.Text = "Save to:";
                _FilenameTextBox.Enabled = true;
            }
            else
            {
                Text = "Load settings";
                _FilenameLabel.Text = "Load from:";
                _FilenameTextBox.Enabled = false;
            }
            _FilenameLabel.Width = _FilenameLabel.PreferredWidth;
            // Rebuild list
            _FilenamesListView.Items.Clear();
            if (Directory.Exists(_SettingsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(_SettingsFolder);
                FileInfo[] fia = di.GetFiles("*.xml");
                foreach (FileInfo fi in fia)
                {
                    ListViewItem lvi = new ListViewItem(Path.GetFileNameWithoutExtension(fi.Name));
                    _FilenamesListView.Items.Add(lvi);
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
        } // PersistSettings_Shown()

        void RemoveButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to remove the setting '" + _FilenameTextBox.Text + "'?", "Save settings remove", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                File.Delete(_SettingsFolder + "\\" + _FilenameTextBox.Text + ".xml");
                ListView.SelectedListViewItemCollection slvic = _FilenamesListView.SelectedItems;
                if (slvic.Count > 0)
                { // Paranoid since the item must be currently selected
                    _FilenamesListView.Items.RemoveAt(slvic[0].Index);
                }
            }
            return;
        }  // RemoveButton_Click()

        void RenameButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection slvic = _FilenamesListView.SelectedItems;
            if (slvic.Count > 0)
            { // Paranoid since the item must be currently selected
                _FilenamesListView.LabelEdit = true;
                slvic[0].BeginEdit();
            }
            return;
        }  // RenameButton_Click()

        void AcceptButton_Click(object sender, EventArgs e)
        {
            _DialogButton = DialogResult.OK;
            do
            {
                if (_DirectionType == DirectionType.Save)
                {
                    if (Directory.Exists(_SettingsFolder) == false)
                    {
                        DirectoryInfo di = Directory.CreateDirectory(_SettingsFolder);
                    }
                    bool bExists = File.Exists(_SettingsFolder + "\\" + _FilenameTextBox.Text + ".xml");
                    if (bExists)
                    {
                        DialogResult dr = MessageBox.Show("File '" + _FilenameTextBox.Text + "' exists. " + "Are you sure you want to overwrite it?", "Save settings overwrite", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.No)
                        {
                            break; // User does not want to overwrite
                        }
                    }
                    try
                    {
                        StreamWriter oStreamWriter = File.CreateText(_SettingsFolder + "\\" + _FilenameTextBox.Text + ".xml");
                        oStreamWriter.Write(Settings);
                        oStreamWriter.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to save settings");
                        break;
                    }
                    if (bExists == false)
                    {
                        ListViewItem lvi = new ListViewItem(_FilenameTextBox.Text);
                        _FilenamesListView.Items.Add(lvi);
                    }
                }
                else
                {   // Load assumed
                    try
                    {
                        StreamReader oStreamReader = File.OpenText(_SettingsFolder + "\\" + _FilenameTextBox.Text + ".xml");
                        Settings = oStreamReader.ReadToEnd();
                        oStreamReader.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to load settings");
                        break;
                    }
                }
                _FilenameTextBox.Text = string.Empty;
                Close();
            } while (false);
            return;
        }  // AcceptButton_Click()

        void CancelButton_Click(object sender, EventArgs e)
        {
            _DialogButton = DialogResult.Cancel;
            _FilenameTextBox.Text = string.Empty;
            return;
        }  // CancelButton_Click()

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersistSettings_Load(object sender, EventArgs e)
        {
            //Icon = Properties.Resources.DL;
        }

        private void PersistSettings_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && ((e.AffectedProperty == "Bounds") || (e.AffectedProperty == "Visible")))
            {
                // Vertical
                int y = 0;
                _FilenamesListView.Top = y;
                {
                    int yAvailable = ClientSize.Height;
                    yAvailable -= _AcceptButton.Height;
                    yAvailable -= UIBase.ExtraPadding.Top;
                    yAvailable -= _FilenameTextBox.Height;
                    yAvailable -= UIBase.ExtraPadding.Top;
                    if (yAvailable < 64) yAvailable = 64;   // WJC fix hard-coded
                    yAvailable /= 14;
                    yAvailable *= 14;
                    yAvailable = 1 + _FilenamesListView.Margin.Top + yAvailable + _FilenamesListView.Margin.Bottom + 1;
                    _FilenamesListView.Height = yAvailable;
                }
                y += _FilenamesListView.Height;
                y += _FilenamesListView.Margin.Bottom;
                y += UIBase.ExtraPadding.Top;
                {
                    int maxH = _FilenameLabel.Height;
                    if (maxH < _FilenameTextBox.Height) maxH = _FilenameTextBox.Height;
                    _FilenameLabel.Top = y + (maxH - _FilenameLabel.Height) / 2;
                    _FilenameTextBox.Top = y + (maxH - _FilenameTextBox.Height) / 2;
                    y += maxH;
                }
                y += UIBase.ExtraPadding.Top;
                _RenameButton.Top = y;
                _RemoveButton.Top = y;
                _AcceptButton.Top = y;
                _CancelButton.Top = y;
                y += _AcceptButton.Height;
                // Horizontal
                int x = 0;
                int maxX = -1;
                _FilenamesListView.Top = x;
                x += _FilenamesListView.Width;
                x = 0;
                _FilenameLabel.Left = x;
                if (maxX < x) maxX = x;
                x += _FilenameLabel.Width;
                _FilenameTextBox.Left = x;
                {
                    int nWidth = ClientSize.Width - _FilenameTextBox.Left;
                    int nWidthMin = TextRenderer.MeasureText(new string('X', 32), _FilenameTextBox.Font).Width;
                    if (nWidth < nWidthMin) nWidth = nWidthMin;
                    _FilenameTextBox.Width = nWidth;
                }
                x += _FilenameTextBox.Width;
                if (maxX < x) maxX = x;
                x = 0;
                _RenameButton.Left = x;
                x += _RenameButton.Width;
                _RemoveButton.Left = x;
                x += _RemoveButton.Width;
                x += UIBase.ExtraPadding.Right;
                //_AcceptButton.Left = x;   // WJC reduce flicker
                x += _AcceptButton.Width;
                //_CancelButton.Left = x;   // WJC reduce flicker
                x += _CancelButton.Width;
                x += 24;    // Gripper
                if (maxX < x) maxX = x;
                _FilenamesListView.Width = maxX;
                _FilenamesListView.Columns[0].Width = maxX - 4;
                _FilenameTextBox.Width = maxX - _FilenameTextBox.Left;

                x = maxX;
                x -= 24;    // Gripper
                x -= _CancelButton.Width;
                _CancelButton.Left = x;
                x -= _AcceptButton.Width;
                _AcceptButton.Left = x;

                if ((ClientSize.Width < maxX) || (ClientSize.Height < y)) ClientSize = new Size(maxX, y);

            }
            return;
        } // PersistSettings_Layout()

        #endregion

    } // class PresistSsettings
}