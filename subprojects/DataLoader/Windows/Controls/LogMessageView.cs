using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Data;
using CambridgeSoft.COE.DataLoader.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// View LogMessageList
    /// </summary>
    public partial class LogMessageView : UserControl
    {
        #region SeverityInfo
        private class SeverityInfo
        {
            #region data
            private bool _Enabled;
            private readonly System.Windows.Forms.Label _Label;
            private int _Count;
            private string _Name;
            #endregion

            #region properties
            public bool Enabled
            {
                get
                {
                    return _Enabled;
                }
                set
                {
                    _Enabled = value;
                    if (Enabled)
                    {
                        _Label.BackColor = UIBase.DefaultBackColor;
                        _Label.BorderStyle = BorderStyle.FixedSingle;
                        _Label.Padding = new Padding(0);
                    }
                    else
                    {
                        _Label.BackColor = UIBase.White;
                        _Label.BorderStyle = BorderStyle.None;
                        _Label.Padding = new Padding(1);
                    }
                    return;
                }
            } // Enabled
            public int Count
            {
                get
                {
                    return _Count;
                }
                set
                {
                    _Count = value;
                    _Label.Text = Count.ToString() + " " + Name + ((Count != 1) ? "s" : string.Empty);
                    return;
                }
            } // Count
            public string Name
            {
                get
                {
                    return _Name;
                }
                set
                {
                    _Name = value;
                    return;
                }
            } // Name
            #endregion

            #region contructors
            public SeverityInfo(System.Windows.Forms.Label vLabel, string vstrName, bool vbEnabled)
            {
                _Label = vLabel;
                Enabled = vbEnabled;
                Name = vstrName;
                Count = 0;  // Count must come after Name
                return;
            } // SeverityInfo()
            #endregion

        } // class SeverityInfo
        #endregion

        #region data
        private readonly System.Windows.Forms.Label _SummaryLabel;
        private readonly System.Windows.Forms.TextBox _SummaryFilename;
        private readonly System.Windows.Forms.TextBox _SummaryTime;
        private readonly System.Windows.Forms.TextBox _SummaryCount;

        private readonly System.Windows.Forms.TableLayoutPanel _SeverityStrip;
        private readonly System.Windows.Forms.Label[] _SeverityLabel;

        private readonly System.Windows.Forms.TableLayoutPanel _HeaderGrid;
        private readonly System.Windows.Forms.Label[] _HeaderLabel;
        private readonly System.Windows.Forms.TableLayoutPanel _MainGrid;
        private readonly System.Windows.Forms.ImageList _SeverityImageList;
        private readonly System.Windows.Forms.PictureBox[] _MainSeverity;
        private readonly System.Windows.Forms.TextBox[] _MainSource;
        private readonly System.Windows.Forms.TextBox[] _MainTransaction;
        private readonly System.Windows.Forms.TextBox[] _MainMessage;
        private readonly System.Windows.Forms.ContainerControl[] _MainMessageContainer;
        private readonly System.Windows.Forms.VScrollBar _MainVScrollBar;
        #endregion

        #region property data
        private Job _Job;
        private string _LogMessagePath;
        private DataSet _LogMessageDataSet;
        #endregion

        #region properties
        /// <summary>
        /// Set Job
        /// Which in turn affects JobSpec
        /// </summary>
        public Job LoaderJob
        {
            protected get
            {
                return _Job;
            }
            set
            {
                _Job = value;
                return;
            }
        } 

        /// <summary>
        /// Set path to the log messages
        /// </summary>
        public string LogMessagePath
        {
            get
            {
                return _LogMessagePath;
            }
            set
            {
                _LogMessagePath = value;
                FileStream oFileStream = File.OpenRead(_LogMessagePath);
                bool[] bSeverity = { false, false, false };
                int[] cSeverity = new int[3];
                DataSet logMessageDataSet = LogMessageList.MakeDataSet(oFileStream, true, bSeverity, ref cSeverity);
                oFileStream.Close();
                for (int n = 0; n < 3; n++)
                {
                    SeverityInfo securityInfo = (SeverityInfo)_SeverityLabel[n].Tag;
                    securityInfo.Count = cSeverity[n];
                }
                Dictionary<string, string> dictSummary = new Dictionary<string, string>();
                DataTable oDataTable = logMessageDataSet.Tables[0];
                foreach (DataRow oDataRow in oDataTable.Rows)
                {
                    string[] strMessage = oDataRow[3].ToString().Split(new char[] { '\t' }, 2);
                    try
                    {
                        dictSummary.Add(strMessage[0], strMessage[1]);
                    }
                    catch
                    {
                        ;
                    }
                }
                _SummaryFilename.Text = "Log filename " + Path.GetFileNameWithoutExtension(_LogMessagePath);
                {
                    string strText = string.Empty;
                    try
                    {
                        DateTime dtStart = DateTime.Parse(dictSummary["Start"]);
                        DateTime dtEnd = DateTime.Parse(dictSummary["End"]);
                        strText += "Started " + dtStart;
                        strText += " and ended " + dtEnd;
                        double dElapsed = double.Parse(dictSummary["Elapsed"]);
                        dElapsed = Math.Round(dElapsed, (dElapsed >= 1) ? 0 : 3);
                        strText += " with an elapsed time of " + dElapsed + " second" + ((dElapsed != 1.0) ? "s" : string.Empty);
                    }
                    catch
                    {
                        ;
                    }
                    _SummaryTime.Text = strText;
                }
                {
                    string strText = string.Empty;
                    try
                    {
                        int cProcessed = Int32.Parse(dictSummary["Processed"]);
                        strText += "Processed a total of " + cProcessed;
                        strText += " record" + ((cProcessed != 1) ? "s" : string.Empty);
                        int cErrors = Int32.Parse(dictSummary["Errors"]);
                        strText += " with " + cErrors;
                        strText += " record" + ((cErrors != 1) ? "s" : string.Empty);
                        strText += " not stored due to errors";
                    }
                    catch
                    {
                        ;
                    }
                    _SummaryCount.Text = strText;
                }
                if (LoaderJob != null)
                {
                    try
                    {
                        LoaderJob.Load(dictSummary["Specification"]);
                    }
                    catch
                    {
                        ;
                    }
                }
                FillMainGrid();
                return;
            } // set
        } // LogMessagePath
        #endregion

        #region event declaration/definition
        /// <summary>
        /// Delegate for HeightChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void HeightChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// The HeightChanged event is raised when the height is changed
        /// </summary>
        public event HeightChangedEventHandler HeightChanged;
        private void OnHeightChanged(EventArgs e)
        {
            if (HeightChanged != null)
            {
                HeightChanged(this, e);
            }
            return;
        } // OnHeightChanged()
        #endregion

        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public LogMessageView()
        {
            InitializeComponent();
            // _SeverityImageList
            _SeverityImageList = new ImageList();
            _SeverityImageList.Images.Add("Error", UIBase.GetIcon("Error"));
            _SeverityImageList.Images.Add("Warning", UIBase.GetIcon("Warning"));
            _SeverityImageList.Images.Add("Info", UIBase.GetIcon("Info"));
            SuspendLayout();
            // _SummaryLabel
            _SummaryLabel = UIBase.GetHeaderLabel();
            _SummaryLabel.Text = "Summary";
            Controls.Add(_SummaryLabel);
            // _SummaryFilename
            _SummaryFilename = UIBase.GetTextBox();
            _SummaryFilename.Enabled = false;
            Controls.Add(_SummaryFilename);
            // _SummaryTime
            _SummaryTime = UIBase.GetTextBox();
            _SummaryTime.Enabled = false;
            _SummaryLabel.Height = _SummaryTime.Height;
            Controls.Add(_SummaryTime);
            // _SummaryCount
            _SummaryCount = UIBase.GetTextBox();
            _SummaryCount.Enabled = false;
            Controls.Add(_SummaryCount);
            // _SeverityStrip
            _SeverityStrip = UIBase.GetTableLayoutPanel();
            {
                _SeverityStrip.AutoScroll = false;
                _SeverityStrip.AutoSize = false;
                _SeverityStrip.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _SeverityStrip.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None; // Single
                _SeverityStrip.Height = 1;   // Set a small amount, it will grow
                // Rows
                _SeverityStrip.RowCount = 1;
                _SeverityStrip.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                // Columns
                _SeverityStrip.ColumnCount = 3;
                _SeverityLabel = new Label[_SeverityStrip.ColumnCount];
                // Column 0 to 2
                _SeverityStrip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _SeverityLabel[0] = UIBase.GetLabel();
                _SeverityLabel[0].Click += new EventHandler(SeverityLabel_Click);
                _SeverityLabel[0].Dock = DockStyle.Fill;
                _SeverityLabel[0].Image = _SeverityImageList.Images["Error"];
                _SeverityLabel[0].ImageAlign = ContentAlignment.MiddleLeft;
                _SeverityLabel[0].Margin = new Padding(0);
                _SeverityLabel[0].Tag = new SeverityInfo(_SeverityLabel[0], "Error", true);
                _SeverityLabel[0].TextAlign = ContentAlignment.MiddleRight;
                _SeverityStrip.Controls.Add(_SeverityLabel[0], 0, 0);

                _SeverityStrip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _SeverityLabel[1] = UIBase.GetLabel();
                _SeverityLabel[1].Click += new EventHandler(SeverityLabel_Click);
                _SeverityLabel[1].Dock = DockStyle.Fill;
                _SeverityLabel[1].Image = _SeverityImageList.Images["Warning"];
                _SeverityLabel[1].ImageAlign = ContentAlignment.MiddleLeft;
                _SeverityLabel[1].Margin = new Padding(0);
                _SeverityLabel[1].Tag = new SeverityInfo(_SeverityLabel[1], "Warning", true);
                _SeverityLabel[1].TextAlign = ContentAlignment.MiddleRight;
                _SeverityStrip.Controls.Add(_SeverityLabel[1], 1, 0);
                
                _SeverityStrip.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _SeverityLabel[2] = UIBase.GetLabel();
                _SeverityLabel[2].Click += new EventHandler(SeverityLabel_Click);
                _SeverityLabel[2].Dock = DockStyle.Fill;
                _SeverityLabel[2].Image = _SeverityImageList.Images["Info"];
                _SeverityLabel[2].ImageAlign = ContentAlignment.MiddleLeft;
                _SeverityLabel[2].Margin = new Padding(0);
                _SeverityLabel[2].Tag = new SeverityInfo(_SeverityLabel[2], "Message", true);
                _SeverityLabel[2].TextAlign = ContentAlignment.MiddleRight;
                //_SeverityStrip.Controls.Add(_SeverityLabel[2], 2, 0);
                //
                _SeverityStrip.Height = _SeverityStrip.Margin.Top + _SeverityLabel[0].Height + _SeverityStrip.Margin.Bottom;
                _SeverityStrip.Controls.Add(_SeverityLabel[2], 2, 0);
                Controls.Add(_SeverityStrip);
            }
            // _HeaderGrid
            _HeaderGrid = UIBase.GetTableLayoutPanel();
            {
                _HeaderGrid.AutoScroll = false;
                _HeaderGrid.AutoSize = false;
                _HeaderGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _HeaderGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                _HeaderGrid.Height = 1;   // Set a small amount, it will grow
                // Rows
                _HeaderGrid.RowCount = 1;
                _HeaderGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                // Columns
                _HeaderGrid.ColumnCount = 4;
                _HeaderLabel = new System.Windows.Forms.Label[_HeaderGrid.ColumnCount];
                // Column 0
                _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _HeaderLabel[0] = UIBase.GetHeaderLabel();
                _HeaderLabel[0].BorderStyle = BorderStyle.Fixed3D;
                _HeaderLabel[0].Margin = new System.Windows.Forms.Padding(0);
                _HeaderLabel[0].Dock = DockStyle.Fill;
                _HeaderLabel[0].TextAlign = ContentAlignment.MiddleLeft;
                _HeaderLabel[0].Text = string.Empty;  // Severity
                _HeaderGrid.Controls.Add(_HeaderLabel[0], 0, 0);
                // Column 1
                _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _HeaderLabel[1] = UIBase.GetHeaderLabel();
                _HeaderLabel[1].BorderStyle = BorderStyle.Fixed3D;
                _HeaderLabel[1].Margin = new System.Windows.Forms.Padding(0);
                _HeaderLabel[1].Dock = DockStyle.Fill;
                _HeaderLabel[1].TextAlign = ContentAlignment.MiddleLeft;
                _HeaderLabel[1].Text = "Source";
                _HeaderGrid.Controls.Add(_HeaderLabel[1], 1, 0);
                // Column 2
                _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _HeaderLabel[2] = UIBase.GetHeaderLabel();
                _HeaderLabel[2].BorderStyle = BorderStyle.Fixed3D;
                _HeaderLabel[2].Margin = new System.Windows.Forms.Padding(0);
                _HeaderLabel[2].Dock = DockStyle.Fill;
                _HeaderLabel[2].TextAlign = ContentAlignment.MiddleLeft;
                _HeaderLabel[2].Text = "Transaction";
                _HeaderGrid.Controls.Add(_HeaderLabel[2], 2, 0);
                // Column 3
                _HeaderGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _HeaderLabel[3] = UIBase.GetHeaderLabel();
                _HeaderLabel[3].BorderStyle = BorderStyle.Fixed3D;
                _HeaderLabel[3].Margin = new System.Windows.Forms.Padding(0);
                _HeaderLabel[3].Dock = DockStyle.Fill;
                _HeaderLabel[3].TextAlign = ContentAlignment.MiddleLeft;
                _HeaderLabel[3].Text = "Message";
                //_HeaderGrid.Controls.Add(_HeaderLabel[3], 3, 0);
                //
                _HeaderGrid.Height = _HeaderGrid.Margin.Top + _HeaderLabel[0].Height + _HeaderGrid.Margin.Bottom;
                _HeaderGrid.Controls.Add(_HeaderLabel[3], 3, 0);
                Controls.Add(_HeaderGrid);
            }
            // _MainGrid
            _MainGrid = UIBase.GetTableLayoutPanel();
            {
                // Grid
                _MainGrid.AutoScroll = false;
                _MainGrid.AutoSize = false;
                _MainGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _MainGrid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                _MainGrid.Height = 1;   // Set a small amount, it will grow
                // Columns
                _MainGrid.ColumnCount = 4;
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                _MainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
                // Preallocate rows
                _MainGrid.RowCount = 32;
                _MainGrid.Tag = 0;
                _MainSeverity = new PictureBox[_MainGrid.RowCount];
                _MainSource = new TextBox[_MainGrid.RowCount];
                _MainTransaction = new TextBox[_MainGrid.RowCount];
                _MainMessage = new TextBox[_MainGrid.RowCount];
                _MainMessageContainer = new ContainerControl[_MainGrid.RowCount];
                for (int nRow = 0; nRow < _MainGrid.RowCount; nRow++)
                {
                    _MainSeverity[nRow] = UIBase.GetPictureBox();
                    _MainSeverity[nRow].Enabled = true;
                    _MainSource[nRow] = UIBase.GetTextBox();
                    _MainSource[nRow].Enabled = false;
                    _MainTransaction[nRow] = UIBase.GetTextBox();
                    _MainTransaction[nRow].Enabled = false;
                    _MainTransaction[nRow].TextAlign = HorizontalAlignment.Right;
                    _MainMessage[nRow] = UIBase.GetTextBox();
                    _MainMessage[nRow].Enabled = false;
                    _MainMessageContainer[nRow] = new ContainerControl();
                    {
                        PictureBox oPictureBox = UIBase.GetPictureBox();
                        oPictureBox.Image = UIBase.GetIcon("Zoom_in").ToBitmap();
                        oPictureBox.Left = 0;
                        oPictureBox.Top = 0;
                        oPictureBox.Width = oPictureBox.Image.Width;
                        oPictureBox.Height = oPictureBox.Image.Height;
                        oPictureBox.Name = "ZoomIn";
                        _MainMessageContainer[nRow].Controls.Add(oPictureBox);
                        oPictureBox.Click += new EventHandler(ZoomIn_Click);
                        _MainMessage[nRow].Left = oPictureBox.Left + oPictureBox.Width + oPictureBox.Margin.Right;
                    }
                    _MainMessage[nRow].Name = "Message";
                    _MainMessageContainer[nRow].Controls.Add(_MainMessage[nRow]);
                    _MainMessageContainer[nRow].Height = _MainMessage[nRow].Height;
                    _MainMessageContainer[nRow].Width = _MainMessage[nRow].Left + _MainMessage[nRow].Width;

                    _MainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
                    _MainGrid.Controls.Add(_MainSeverity[nRow], 0, nRow);
                    _MainGrid.Controls.Add(_MainSource[nRow], 1, nRow);
                    _MainGrid.Controls.Add(_MainTransaction[nRow], 2, nRow);
                    _MainGrid.Controls.Add(_MainMessageContainer[nRow], 3, nRow);
                } // for (int nRow = 0; nRow < _MainGrid.RowCount; nRow++)
                Controls.Add(_MainGrid);
            }
            //
            _MainVScrollBar = new VScrollBar();
            _MainVScrollBar.Visible = false;
            _MainVScrollBar.Width = 16;
            Controls.Add(_MainVScrollBar);
            _MainVScrollBar.Scroll += new ScrollEventHandler(MainVScrollBar_Scroll);
            Visible = false;
            Height = 0;
            Layout += new LayoutEventHandler(LogMessageView_Layout);
            //
            ResumeLayout(false);
            return;
        } // LogMessageView()
        #endregion

        #region event handlers
        private void LogMessageView_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds") && Visible)
            {
                int cTotalRows = (int)_MainGrid.Tag; // Total rows
                int cGridRows = (cTotalRows < _MainGrid.RowCount) ? cTotalRows : _MainGrid.RowCount; // Rows used in grid
                // Vertical
                {
                    int y = 0;
                    _SummaryLabel.Top = y;
                    y += _SummaryLabel.Height /* + UIBase.ExtraPadding.Bottom */;   // Want tight
                    _SummaryFilename.Top = y;
                    y += _SummaryFilename.Height + UIBase.ExtraPadding.Bottom;
                    _SummaryTime.Top = y;
                    y += _SummaryTime.Height + UIBase.ExtraPadding.Bottom;
                    _SummaryCount.Top = y;
                    y += _SummaryCount.Height + UIBase.ExtraPadding.Bottom;
                    y += UIBase.ExtraPadding.Bottom;
                    _SeverityStrip.Top = y;
                    y += _SeverityStrip.Height + UIBase.ExtraPadding.Bottom;
                    if (_HeaderGrid.Visible)
                    {
                        _HeaderGrid.Top = y;
                        y += _HeaderGrid.Bounds.Height;
                    }
                    _MainGrid.Top = y;
                    {   // AutoSize _MainGrid
                        int yMax = MaximumSize.Height;
                        yMax -= y;
                        if (yMax > 0)
                        {
                            int cRowsMax;
                            int nHeight = 0;
                            for (cRowsMax = 0; cRowsMax < cGridRows; cRowsMax++)
                            {
                                int nBottom = _MainMessage[cRowsMax].Parent.Bounds.Bottom;
                                nBottom += _MainGrid.Margin.Bottom + 1;   // WJC 1 is cell border
                                if (nBottom > yMax)
                                {
                                    break;
                                }
                                nHeight = nBottom;
                            } // for (cRowsMax = 0; cRowsMax < cGridRows; cRowsMax++)
                            _MainGrid.Height = nHeight;
                            if (cRowsMax < cGridRows)
                            {
                                _MainVScrollBar.Visible = true;
                            }
                            else
                            {
                                _MainVScrollBar.Visible = false;
                            }
                            _MainVScrollBar.Maximum = (cTotalRows - 1); // 0 relative
                            _MainVScrollBar.LargeChange = cRowsMax;
                            _MainVScrollBar.SmallChange = 1;
                        }
                    }
                    y += _MainGrid.Bounds.Height + UIBase.ExtraPadding.Top;
                    Height = y; // was MaximumSize.Height;
                    _MainVScrollBar.Top = _MainGrid.Top;
                    _MainVScrollBar.Height = _MainGrid.Height;
                } // Vertical
                // Horizontal
                {
                    int x = 0;
                    _SummaryLabel.Left = x;
                    _SummaryFilename.Left = x;
                    _SummaryTime.Left = x;
                    _SummaryCount.Left = x;
                    _SeverityStrip.Left = x;
                    for (int n = 0; n < 3; n++)
                    {
                        _SeverityLabel[n].Left = x;
                        _SeverityLabel[n].Width = _SeverityLabel[n].Image.Width + UIBase.ExtraPadding.Left + TextRenderer.MeasureText(_SeverityLabel[n].Text, _SeverityLabel[n].Font).Width;
                        x += _SeverityLabel[n].Width;
                    }
                    _SeverityStrip.Width = x;
                    x = 0;
                    _HeaderGrid.Left = x;
                    _MainGrid.Left = x;
                    {
                        SuspendLayout();
                        int nWidth = MaximumSize.Width;
                        _HeaderGrid.Width = nWidth;
                        _MainGrid.Width = nWidth;
                        if (cGridRows > 0)
                        {
                            int xPreview = _HeaderLabel[3].Location.X;
                            nWidth -= xPreview;
                            nWidth -= (_MainGrid.Margin.Left + _MainGrid.Margin.Right);
                            if (_MainVScrollBar.Visible)
                            {
                                _MainGrid.Width -= 16;
                                nWidth -= 16;
                                _MainVScrollBar.Left = _MainGrid.Left + _MainGrid.Width;
                            }
                            if (nWidth < 0) nWidth = 0;
                            _MainGrid.SuspendLayout();
                            _HeaderLabel[3].Width = nWidth + _MainGrid.Margin.Right;
                            for (int nRow = 0; nRow < _MainGrid.RowCount; nRow++)
                            {
                                _MainMessage[nRow].Width = nWidth - _MainMessage[nRow].Left;
                                _MainMessage[nRow].Parent.Width = nWidth;
                            }
                            _MainGrid.ResumeLayout(false);
                        }
                        ResumeLayout(false);
                    }
                    x += _MainGrid.Bounds.Width;
                    if (_MainVScrollBar.Visible)
                    {
                        x += _MainVScrollBar.Width;
                    }
                    Width = x;
                    _SummaryLabel.Width = Width;
                    _SummaryFilename.Width = Width;
                    _SummaryTime.Width = Width;
                    _SummaryCount.Width = Width;
                } // Horizontal
            } // if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            return;
        } // LogMessageView_Layout()

        private void MainVScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollBar oScrollBar = sender as ScrollBar;
            if (e.Type != ScrollEventType.ThumbTrack)
            {
                if (oScrollBar != null)
                {
                    if ((oScrollBar.Value != e.NewValue) || (e.Type == ScrollEventType.EndScroll))
                    {
                        ScrollMainGrid(e.NewValue);
                    }
                }
            }
            return;
        } // MainVScrollBar_Scroll()

        private void SeverityLabel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = sender as System.Windows.Forms.Label;
            if (label != null)
            {
                SeverityInfo securityInfo = (SeverityInfo)label.Tag;
                securityInfo.Enabled = !securityInfo.Enabled;
                if (securityInfo.Count > 0) FillMainGrid();
            }
            return;
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.PictureBox pictureBox = sender as System.Windows.Forms.PictureBox;
            if (pictureBox != null)
            {
                System.Windows.Forms.ContainerControl containerControl = (ContainerControl)pictureBox.Parent;
                System.Windows.Forms.TextBox textBox = (TextBox)containerControl.Controls["Message"];
                MessageBoxRichText oMessageBoxRichText = new MessageBoxRichText("Message");
                oMessageBoxRichText.MessageText = textBox.Text;
                oMessageBoxRichText.StartPosition = FormStartPosition.CenterParent;
                Form oForm = FindForm();
                oMessageBoxRichText.Size = new Size(textBox.Width, oForm.ClientSize.Height);
                oMessageBoxRichText.ShowDialog();
            }
            return;
        }
        #endregion

        #region methods
        private void FillMainGrid()
        {
            FileStream oFileStream = File.OpenRead(_LogMessagePath);
            bool[] bSeverity = { false, false, false };
            for (int n = 0; n < 3; n++)
            {
                bSeverity[n] = ((SeverityInfo)_SeverityLabel[n].Tag).Enabled;
            }
            int[] cSeverity = new int[1];
            _LogMessageDataSet = LogMessageList.MakeDataSet(oFileStream, false, bSeverity, ref cSeverity);
            oFileStream.Close();
            DataTable oDataTable = _LogMessageDataSet.Tables[0];
            _MainGrid.Tag = oDataTable.Rows.Count;
            int[] xMax = { -1, -1, -1, -1 };
            xMax[0] = _SeverityImageList.Images[0].Width;
            for (int nColumn = 1; nColumn < 4; nColumn++)
            {
                Size size = TextRenderer.MeasureText(_HeaderLabel[nColumn].Text, _HeaderLabel[nColumn].Font);
                xMax[nColumn] = size.Width;
            }
            foreach (Image i in _SeverityImageList.Images)
            {
                int nWidth = i.Width;
                if (xMax[0] < nWidth) xMax[0] = nWidth;
            }
            // Should do the whole list but this is the longest
            {
                int nWidth = TextRenderer.MeasureText("Calculation", _MainSeverity[0].Font).Width;
                if (xMax[1] < nWidth) xMax[1] = nWidth;
            }
#if OVERKILL
            foreach (DataRow oDataRow in oDataTable.Rows)
            {
                int nWidth;
                nWidth = TextRenderer.MeasureText(oDataRow.ItemArray[2].ToString(), _MainTransaction[0].Font).Width;
                if (xMax[2] < nWidth) xMax[2] = nWidth;
                nWidth = TextRenderer.MeasureText(oDataRow.ItemArray[3].ToString(), _MainMessage[0].Font).Width;
                if (xMax[3] < nWidth) xMax[3] = nWidth;
            } // foreach (DataRow oDataRow in oDataTable.Rows)
#endif
            for (int nColumn = 0; nColumn < 3; nColumn++)
            {
                _HeaderLabel[nColumn].Width = 3 + xMax[nColumn] + 3;
            }
            for (int nRow = 0; nRow < _MainSeverity.Length; nRow++)
            {
                _MainSeverity[nRow].Width = xMax[0];
                _MainSource[nRow].Width = xMax[1];
                _MainTransaction[nRow].Width = xMax[2];
            }
            _MainVScrollBar.Maximum = oDataTable.Rows.Count;
            _MainVScrollBar.Minimum = 0;
            _MainVScrollBar.Value = 0;
            _MainVScrollBar.LargeChange = (_MainVScrollBar.Maximum < 32) ? _MainVScrollBar.Maximum : 32;
            ScrollMainGrid(0);
            Visible = true;
            _HeaderGrid.Visible = ((int)_MainGrid.Tag > 0);
            OnLayout(new LayoutEventArgs(this, "Bounds"));
            OnHeightChanged(new EventArgs());   // Tell parent our height has changed
            return;
        } // FillMainGrid()

        private void ScrollMainGrid(int vFirstRow)
        {
            DataTable oDataTable = _LogMessageDataSet.Tables[0];
            int cTotalRows = (int)_MainGrid.Tag; // Total rows
            int cGridRows = (cTotalRows < _MainVScrollBar.LargeChange) ? cTotalRows : _MainVScrollBar.LargeChange; // Rows used in grid
            for (int nRow = 0; nRow < cGridRows; nRow++)
            {
                DataRow oDataRow = oDataTable.Rows[vFirstRow + nRow];
                {
                    string strSeverity;
                    switch (oDataRow.ItemArray[0].ToString())
                    {
                        case "C": { strSeverity = "Error"; break; }
                        case "E": { strSeverity = "Error"; break; }
                        case "I": { strSeverity = "Info"; break; }
                        case "W": { strSeverity = "Warning"; break; }
                        default: { strSeverity = "Error"; break; }
                    }
                    _MainSeverity[nRow].Image = _SeverityImageList.Images[strSeverity];
                }
                _MainSeverity[nRow].Height = _MainSeverity[nRow].Image.Height;
                {
                    string strSource = oDataRow.ItemArray[1].ToString();
                    switch (strSource)
                    {
                        case "C": { strSource = "Calculation"; break; }
                        case "I": { strSource = "Input"; break; }
                        case "J": { strSource = "Job"; break; }
                        case "M": { strSource = "Mapping"; break; }
                        case "O": { strSource = "Output"; break; }
                        case "U": { strSource = "Unknown"; break; }
                    } // switch (strSource)
                    _MainSource[nRow].Text = strSource;
                }
                _MainTransaction[nRow].Text = oDataRow.ItemArray[2].ToString();
                _MainMessage[nRow].Text = oDataRow.ItemArray[3].ToString();
            } // for (int nRow = 0; nRow < cGridRows; nRow++)
            return;
        } // ScrollMainGrid()

        #endregion

    } // class LogMessageView
}
