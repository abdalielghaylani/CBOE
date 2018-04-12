using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Common;
using CambridgeSoft.COE.DataLoader.Data;
using CambridgeSoft.COE.DataLoader.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// UI for additional job configuration
    /// </summary>
    public partial class JobConfiguration : UIBase
    {
        // data members
        private System.Windows.Forms.Label _TotalLabel;
        private System.Windows.Forms.Label _StartLabel;
        private System.Windows.Forms.RichTextBox _StartRichTextBox;
        private System.Windows.Forms.GroupBox _ProcessCountGroupBox;
        private System.Windows.Forms.RadioButton[] _ProcessCountRadioButton;
        private System.Windows.Forms.RichTextBox _ProcessCountRichTextBox;
        private System.Windows.Forms.Label _ProcessCountLabel;
        private System.Windows.Forms.GroupBox _ProcessWhenGroupBox;
        private System.Windows.Forms.RadioButton[] _ProcessWhenRadioButton;
        private System.Windows.Forms.Button _SaveButton;
        private System.Windows.Forms.Button _LoadButton;
        private readonly PersistSettings _SettingsForm = new PersistSettings("JobConfiguration");

        private Job _Job = null;
        private int _cInvalid = 0;
        private bool _bJobChanged = true;

        #region properties
        /// <summary>
        /// Track count of visible invalid controls
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
                UpdateButtons();
                return;
            }
        } // Invalid

        /// <summary>
        /// Connection to the <see cref="Job"/> which we are configuring
        /// </summary>
        public Job Job
        {
            get
            {
                return _Job;
            }
            set
            {
                _Job = value;
                if (_Job != null)
                {
                    Invalid = 0;
                    _Job.Changes += new Job.JobChangesEventHandler(Job_Changes);
                    int cRecords = Int32.MaxValue;
                    {
                        int nRecords = _Job.InputRecords;
                        if (nRecords != int.MaxValue)
                        {
                            if (_Job.InputRecordsApproximate)
                            {
                                _TotalLabel.Text = "There are " + "approximately " + nRecords + " input record" + ((nRecords != 1) ? "s" : string.Empty) + ".";
                            }
                            else
                            {
                                cRecords = nRecords;
                                _TotalLabel.Text = "There are " + nRecords + " input record" + ((nRecords != 1) ? "s" : string.Empty) + ".";
                            }
                        }
                        else
                        {
                            _TotalLabel.Text = "There are an unknown number of input records.";
                        }
                    }
                    // Because of the side-effect of validation JobCount must be set before JobStart
                    if (_Job.JobCount != Int32.MaxValue)
                    {
                        cRecords = _Job.JobCount;

                        // set jobcount when value is NOT Max Int because MaxValue signifies the ALL option
                        _ProcessCountRichTextBox.Text = _Job.JobCount.ToString();
                        _ProcessCountRadioButton[1].Checked = true; // select second radio
                    }
                    else
                    {
                        // JobCount = Int32.MaxValue means ALL records
                        _ProcessCountRadioButton[0].Checked = true; // select the ALL radio
                    }

                    _ProcessCountRichTextBox.Tag = (bool)true;

                    // JobStart
                    _StartRichTextBox.Text = _Job.JobStart.ToString();
                    _StartRichTextBox.Tag = (bool)true;
                } // if (_Job != null)
                return;
            }
        } // Job

        /// <summary>
        /// Track whether the Job Configuration has changed
        /// </summary>
        public bool JobChanged
        {
            get
            {
                return _bJobChanged;
            }
            set
            {
                _bJobChanged = value;
                UpdateButtons();
                return;
            }
        } // JobChanged

        #endregion

        #region constructors
        /// <summary>
        /// Initialize the job configuration UI
        /// </summary>
        public JobConfiguration()
        {
            StatusText = "Configure job";
            InitializeComponent();
            SuspendLayout();
            // Programmatically add control(s)
            _TotalLabel = UIBase.GetLabel();
            _TotalLabel.AutoSize = true;
            Controls.Add(_TotalLabel);
            _StartLabel = UIBase.GetLabel();
            _StartLabel.AutoSize = true;
            _StartLabel.Text = "Start processing with input record ";
            Controls.Add(_StartLabel);
            _StartRichTextBox = UIBase.GetRichTextBox();
            _StartRichTextBox.Enter += new EventHandler(StartRichTextBox_Enter);
            _StartRichTextBox.TextChanged += new EventHandler(StartRichTextBox_TextChanged);
            _StartRichTextBox.Validating += new CancelEventHandler(StartRichTextBox_Validating);
            _StartRichTextBox.Tag = (bool)true;
            _StartRichTextBox.Text = "1";
            _StartRichTextBox.Width = TextRenderer.MeasureText("0000000", _StartRichTextBox.Font).Width;
            Controls.Add(_StartRichTextBox);
            //
            _ProcessCountGroupBox = UIBase.GetGroupBox();
            _ProcessCountGroupBox.Text = "Process Count";
            {
                int yAlign = (_StartRichTextBox.Height - _StartLabel.Height) / 2;
                _ProcessCountRadioButton = new RadioButton[2];
                _ProcessCountRadioButton[0] = UIBase.GetRadioButton();
                _ProcessCountRadioButton[0].AutoSize = true;
                _ProcessCountRadioButton[0].Checked = true;
                _ProcessCountRadioButton[0].Text = "All";
                _ProcessCountRadioButton[0].Left = _ProcessCountGroupBox.Padding.Left + _ProcessCountRadioButton[0].Margin.Left;
                _ProcessCountRadioButton[0].Top = _ProcessCountGroupBox.Padding.Top  + _ProcessCountRadioButton[0].Margin.Top + (_ProcessCountRadioButton[0].PreferredSize.Height + 1) / 2;
                _ProcessCountGroupBox.Controls.Add(_ProcessCountRadioButton[0]);
                _ProcessCountRadioButton[1] = UIBase.GetRadioButton();
                _ProcessCountRadioButton[1].AutoSize = true;
                _ProcessCountRadioButton[1].Checked = false;
                _ProcessCountRadioButton[1].CheckedChanged += new EventHandler(ProcessCountRadioButton_CheckedChanged);
                _ProcessCountRadioButton[1].Text = string.Empty;
                _ProcessCountRadioButton[1].Left = _ProcessCountGroupBox.Padding.Left + _ProcessCountRadioButton[1].Margin.Left;
                _ProcessCountRadioButton[1].Top = _ProcessCountRadioButton[0].Top + _ProcessCountRadioButton[0].PreferredSize.Height + yAlign;
                _ProcessCountGroupBox.Controls.Add(_ProcessCountRadioButton[1]);
                //
                _ProcessCountRichTextBox = UIBase.GetRichTextBox();
                _ProcessCountRichTextBox.Enter += new EventHandler(ProcessCountRichTextBox_Enter);
                _ProcessCountRichTextBox.TextChanged += new EventHandler(ProcessCountRichTextBox_TextChanged);
                _ProcessCountRichTextBox.Validating += new CancelEventHandler(ProcessCountRichTextBox_Validating);
                _ProcessCountRichTextBox.Left = _ProcessCountRadioButton[1].Left + _ProcessCountRadioButton[1].PreferredSize.Width + _ProcessCountRadioButton[1].Margin.Right;
                _ProcessCountRichTextBox.MaxLength = 7;
                _ProcessCountRichTextBox.Top = _ProcessCountRadioButton[1].Top - yAlign;
                _ProcessCountRichTextBox.Width = TextRenderer.MeasureText(new string('0', _ProcessCountRichTextBox.MaxLength), _ProcessCountRichTextBox.Font).Width;
                _ProcessCountGroupBox.Controls.Add(_ProcessCountRichTextBox);
                //
                _ProcessCountLabel = UIBase.GetLabel();
                _ProcessCountLabel.AutoSize = true;
                _ProcessCountLabel.Text = " records";
                _ProcessCountLabel.Left = _ProcessCountRichTextBox.Left + _ProcessCountRichTextBox.Width;
                _ProcessCountLabel.Top = _ProcessCountRadioButton[1].Top;
                _ProcessCountGroupBox.Controls.Add(_ProcessCountLabel);
                //
                _ProcessCountGroupBox.Width = _ProcessCountLabel.Left + _ProcessCountLabel.Width + _ProcessCountGroupBox.Padding.Right;
                _ProcessCountGroupBox.Height = _ProcessCountRichTextBox.Top + _ProcessCountRichTextBox.Height + _ProcessCountRichTextBox.Margin.Bottom + _ProcessCountGroupBox.Padding.Bottom;
            }
            Controls.Add(_ProcessCountGroupBox);
            //
            _ProcessWhenGroupBox = UIBase.GetGroupBox();
            _ProcessWhenGroupBox.Text = "Process When";
            {
                int xMax = -1;
                {
                    int x = _ProcessWhenGroupBox.Padding.Left * 3 + TextRenderer.MeasureText(_ProcessWhenGroupBox.Text, _ProcessWhenGroupBox.Font).Width + _ProcessWhenGroupBox.Padding.Right * 3;
                    if (xMax < x) xMax = x;
                }
                _ProcessWhenRadioButton = new RadioButton[2];
                _ProcessWhenRadioButton[0] = UIBase.GetRadioButton();
                _ProcessWhenRadioButton[0].AutoSize = true;
                _ProcessWhenRadioButton[0].Checked = true;
                _ProcessWhenRadioButton[0].CheckedChanged += new EventHandler(ProcessWhenRadioButton_CheckedChanged);
                _ProcessWhenRadioButton[0].Text = "Now";
                _ProcessWhenRadioButton[0].Left = _ProcessWhenGroupBox.Padding.Left + _ProcessWhenRadioButton[0].Margin.Left;
                _ProcessWhenRadioButton[0].Top = _ProcessWhenGroupBox.Padding.Top + _ProcessWhenRadioButton[0].Margin.Top + (_ProcessWhenRadioButton[0].PreferredSize.Height + 1) / 2;
                {
                    int x = _ProcessWhenRadioButton[0].Left + _ProcessWhenRadioButton[0].PreferredSize.Width + _ProcessWhenGroupBox.Padding.Right;
                    if (xMax < x) xMax = x;
                }
                _ProcessWhenGroupBox.Controls.Add(_ProcessWhenRadioButton[0]);
                _ProcessWhenRadioButton[1] = UIBase.GetRadioButton();
                _ProcessWhenRadioButton[1].AutoSize = true;
                _ProcessWhenRadioButton[1].Checked = false;
                _ProcessWhenRadioButton[1].Left = _ProcessWhenGroupBox.Padding.Left + _ProcessWhenRadioButton[1].Margin.Left;
                _ProcessWhenRadioButton[1].Top = _ProcessWhenRadioButton[0].Top + _ProcessWhenRadioButton[0].PreferredSize.Height;
                _ProcessWhenRadioButton[1].Text = "Later";
                {
                    int x = _ProcessWhenRadioButton[1].Left + _ProcessWhenRadioButton[1].PreferredSize.Width + _ProcessWhenGroupBox.Padding.Right;
                    if (xMax < x) xMax = x;
                }
                _ProcessWhenGroupBox.Controls.Add(_ProcessWhenRadioButton[1]);
                _ProcessWhenGroupBox.Width = xMax;
                _ProcessWhenGroupBox.Height = _ProcessWhenRadioButton[1].Top + _ProcessWhenRadioButton[0].PreferredSize.Height + _ProcessWhenGroupBox.Margin.Bottom + _ProcessWhenGroupBox.Padding.Bottom;
            }
            Controls.Add(_ProcessWhenGroupBox);

            //JED: Not useful at this time; default to process NOW
            _ProcessWhenGroupBox.Visible = false;
            _ProcessWhenRadioButton[0].Checked = true;

            //
            _SaveButton = UIBase.GetButton(ButtonType.Save);
            _SaveButton.Click += new EventHandler(SaveButton_Click);
            Controls.Add(_SaveButton);
            _LoadButton = UIBase.GetButton(ButtonType.Load);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            _LoadButton.Click += new EventHandler(LoadButton_Click);
            Controls.Add(_LoadButton);
            // 
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(JobConfiguration_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        }

        #endregion

        #region helper methods
        private bool ProcessCount_Validate()
        {
            bool bIsValid = false;
            int nProcessCount = 0;
            try
            {
                nProcessCount = Int32.Parse(_ProcessCountRichTextBox.Text);
                if (Job.InputRecordsApproximate || Job.InputRecordsUnknown)
                {
                    bIsValid = (nProcessCount >= 1);
                }
                else
                {
                    bIsValid = ((nProcessCount >= 1) && ((Int32.Parse(_StartRichTextBox.Text) - 1 + nProcessCount) <= Job.InputRecords));
                }
            }
            catch
            {
                ;
            }
            if (bIsValid)
            {
                Job.JobCount = nProcessCount;
            }
            RichTextBox_TagUpdate(_ProcessCountRichTextBox, bIsValid);
            return bIsValid;
        } // ProcessCount_IsValid()
        private void RichTextBox_TagUpdate(RichTextBox rtb, bool vbValid)
        {
            if (rtb.Tag != null)
            {
                if ((bool)rtb.Tag != vbValid)
                {
                    rtb.Tag = vbValid;
                    if (rtb.Focused || ((rtb == _ProcessCountRichTextBox) && _ProcessCountRadioButton[1].Checked))
                    {
                        if (vbValid)
                        {
                            Invalid--;
                            if (rtb.Focused == false) UIBase.RichTestBox_Unmark(rtb);
                        }
                        else
                        {
                            Invalid++;
                            if (rtb.Focused == false) UIBase.RichTestBox_MarkError(rtb);
                        }
                    }
                }
            }
            else
            {
                rtb.Tag = vbValid;
            }
            return;
        } // RichTextBox_TagUpdate()
        private bool Start_Validate()
        {
            bool bIsValid = false;
            int nStart = 0;
            try
            {
                nStart = Int32.Parse(_StartRichTextBox.Text);
                if (Job.InputRecordsApproximate)
                {
                    bIsValid = (nStart >= 1);
                }
                else
                {
                    bIsValid = ((nStart >= 1) && (nStart <= Job.InputRecords));
                }
            }
            catch
            {
                ;
            }
            if (bIsValid)
            {
                Job.JobStart = nStart;
            }
            RichTextBox_TagUpdate(_StartRichTextBox, bIsValid);
            return bIsValid;
        } // Start_IsValid()
        private void UpdateButtons()
        {
            if (AcceptButton != null) AcceptButton.Enabled = (Invalid == 0) && (_ProcessWhenRadioButton[0].Checked || (JobChanged == false));
            if (_SaveButton != null) _SaveButton.Enabled = (Invalid == 0) && JobChanged;
            return;
        } // UpdateButtons()
        #endregion

        #region event handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept();
            return;
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        }
        private void JobConfiguration_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int yAlign = (_StartRichTextBox.Height - _StartLabel.Height) / 2;
                int y = 0;
                _TotalLabel.Top = y;
                y += _TotalLabel.Height;
                _StartLabel.Top = y + yAlign;
                _StartRichTextBox.Top = y;
                y += _StartRichTextBox.Height;
                y += UIBase.ExtraPadding.Top;
                _ProcessCountGroupBox.Top = y + _ProcessCountGroupBox.Margin.Top;
                y += _ProcessCountGroupBox.Margin.Bottom + _ProcessCountGroupBox.Height;
                y += UIBase.ExtraPadding.Top;
                _ProcessWhenGroupBox.Top = y + _ProcessWhenGroupBox.Margin.Top;
                y += _ProcessWhenGroupBox.Margin.Bottom + _ProcessWhenGroupBox.Height;
                y += UIBase.ExtraPadding.Top;
                _SaveButton.Top = y;
                _LoadButton.Top = y;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _TotalLabel.Left = x;
                x += _TotalLabel.Width;
                if (Width < x) Width = x;
                //
                x = 0;
                _StartLabel.Left = x;
                x += _StartLabel.Width;
                _StartRichTextBox.Left = x;
                x += _StartRichTextBox.Width;
                if (Width < x) Width = x;
                //
                x = 0;
                _ProcessCountGroupBox.Left = x + _ProcessCountGroupBox.Margin.Left;
                x += _ProcessCountGroupBox.Margin.Right + _ProcessCountGroupBox.Width;
                if (Width < x) Width = x;
                //
                x = 0;
                _ProcessWhenGroupBox.Left = x + _ProcessWhenGroupBox.Margin.Left;
                x += _ProcessWhenGroupBox.Margin.Right + _ProcessWhenGroupBox.Width;
                if (Width < x) Width = x;
                //
                x = 0;
                _SaveButton.Left = x;
                x += _SaveButton.Width;
                _LoadButton.Left = x;
                x += _LoadButton.Width;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                // right align two buttons
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            }
            return;
        } // JobConfiguration_Layout()
        private void Job_Changes(object sender, Job.JobChangesEventArgs e)
        {
            Job thisJob = sender as Job;
            Job.JobChangesEventArgs je = e as Job.JobChangesEventArgs;
            JobChanged = true;
            return;
        } // Job_Changes()

        private void LoadButton_Click(object sender, EventArgs e)
        {
            // // // Confirm loading over current job
            _SettingsForm.Direction = PersistSettings.DirectionType.Load;
            _SettingsForm.ShowDialog(this);
            if (_SettingsForm.DialogButton == DialogResult.OK)
            {
                string xmlJob = _SettingsForm.Settings;
                Job tmpJob = new Job();
                if (tmpJob.Load(xmlJob))
                {
                    MessageBox.Show("The job could not be loaded.\n" + String.Join("\n", tmpJob.MessageList.ToMessageArray()), "Load a previously saved job", MessageBoxButtons.OK);
                }
                else
                {
                    Job.Load(xmlJob);
                    Job = Job;
                    JobChanged = false;
                }
                tmpJob = null;  // Doesn't hurt
            }
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            return;
        } // LoadButton_Click()
        private void ProcessCountRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton thisRadioButton = sender as RadioButton;
            if (thisRadioButton != null)
            {
                if (thisRadioButton.Checked)
                {
                    _ProcessCountRichTextBox.Focus();
                    if (_ProcessCountRichTextBox.Tag != null)
                    {
                        if ((bool)_ProcessCountRichTextBox.Tag)
                        {
                            if (Job.JobCount != Convert.ToInt32(_ProcessCountRichTextBox.Text)) Job.JobCount = Convert.ToInt32(_ProcessCountRichTextBox.Text);
                        }
                        else
                        {
                            Job.JobCount = Job.InputRecords;
                            Invalid++;
                        }
                    }
                }
                else
                {
                    Job.JobCount = Int32.MaxValue;
                    if (_ProcessCountRichTextBox.Tag != null)
                    {
                        if ((bool)_ProcessCountRichTextBox.Tag == false)
                        {
                            UIBase.RichTestBox_Unmark(_ProcessCountRichTextBox);
                            Invalid--;
                        }
                    }
                }
            }
            return;
        } // ProcessCountRadioButton_CheckedChanged()
        private void ProcessCountRichTextBox_Enter(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (_ProcessCountRadioButton[0].Checked)
            {
                _ProcessCountRadioButton[0].Checked = false;
                _ProcessCountRadioButton[1].Checked = true;
            }
            if (rtb != null)
            {
                UIBase.RichTestBox_Unmark(rtb);
                rtb.SelectAll();
            }
            return;
        } // ProcessCountRichTextBox_Enter()
        private void ProcessCountRichTextBox_TextChanged(object sender, EventArgs e)
        {
            //RichTextBox rtb = sender as RichTextBox;
            ProcessCount_Validate();
            return;
        } // ProcessCountRichTextBox_TextChanged()
        private void ProcessCountRichTextBox_Validating(object sender, CancelEventArgs e)
        {
            StartRichTextBox_Validating(sender, e); // Rather than copy/paste
            return;
        } // ProcessCountRichTextBox_Validating()
        private void ProcessWhenRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            //RadioButton thisRadioButton = sender as RadioButton;
            UpdateButtons();
            return;
        } // ProcessWhenRadioButton_CheckedChanged()
        private void SaveButton_Click(object sender, EventArgs e)
        {
            _SettingsForm.Direction = PersistSettings.DirectionType.Save;
            {
                string xmlJob = _Job.Save();
                xmlJob = COEXmlTextWriter.Pretty(xmlJob); // OK JobConfiguration::Save()
                _SettingsForm.Settings = xmlJob;
                // ???
            }
            _SettingsForm.ShowDialog(this);
            JobChanged = (_SettingsForm.DialogButton == DialogResult.Cancel);
            _LoadButton.Enabled = (_SettingsForm.Count > 0);
            return;
        } // SaveButton_Click()
        private void StartRichTextBox_Enter(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (rtb != null)
            {
                UIBase.RichTestBox_Unmark(rtb);
                rtb.SelectAll();
            }
            return;
        } // StartRichTextBox_Enter()
        private void StartRichTextBox_TextChanged(object sender, EventArgs e)
        {
            //RichTextBox rtb = sender as RichTextBox;
            bool bIsValid = Start_Validate();
            if (bIsValid) ProcessCount_Validate();
            return;
        } // StartRichTextBox_TextChanged()
        private void StartRichTextBox_Validating(object sender, CancelEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (rtb != null)
            {
                if ((bool)rtb.Tag == false)
                {
                    if (rtb.Text != string.Empty)
                    {
                        UIBase.RichTestBox_MarkError(rtb);
                    }
                    else
                    {
                        UIBase.RichTestBox_MarkUnknownError(rtb);
                    }
                }
            }
            return;
        } // StartRichTextBox_Validating()
        #endregion
    } // class JobConfiguration
}
