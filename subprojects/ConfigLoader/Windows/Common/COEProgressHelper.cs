using System;
using System.ComponentModel;
using CambridgeSoft.COE.ConfigLoader.Windows.Forms;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Common
{
    /// <summary>
    /// UI for time comsuming operations
    /// </summary>
    public class COEProgressHelper
    {
        #region data
        #region constants
        private const int _kTicksPerMillisecond = 10000;
        #endregion

        #region property data
        private const string _kstrCancelConfirmation = "Are you sure you want to stop this operations?";
        private string _strCancelConfirmation = _kstrCancelConfirmation; // One shot
        private bool _bCancellationPending = false;
        private UInt32 _uIntervalMs = 500;
        private bool _bIsRunning = false;
        private const int _knMinimum = 0;
        private int _nMinimum = _knMinimum;    // One shot
        private const int _knMaximum = 100;
        private int _nMaximum = _knMaximum;    // One shot
        private System.Windows.Forms.Form _frmParent = null;
        private System.Windows.Forms.StatusStrip _ssStatusStrip = null;
        private const bool _kbSupportsCancellation = true;
        private bool _bSupportsCancellation = _kbSupportsCancellation; // One shot
        private int _nValue = 0;
        #endregion

        #region private data
        #region UI elements
        private COEWait _COEWait = null;
        private System.Windows.Forms.ToolStripButton _ToolStripButton = null;
        private System.Windows.Forms.ToolStripLabel _ToolStripLabel = null;
        private System.Windows.Forms.ToolStripLabel _ToolStripLabelBack = null;
        private System.Windows.Forms.ToolStripProgressBar _ToolStripProgressBar = null;
        private System.Windows.Forms.ToolStripStatusLabel _ToolStripStatusLabel = null;
        #endregion
        #region other
        private bool _bHasUI;
        private long _lDoEventsTicks;
        private long _lStatusTextTicks;
        private long _lValueTicks;
        #endregion
        #endregion
        #endregion

        #region properties
        /// <summary>
        /// Get/set the refresh interval; be careful
        /// </summary>
        public string CancelConfirmation
        {
            get
            {
                return _strCancelConfirmation;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set CancelConfirmation from within a ProgressSection");
                }
                _strCancelConfirmation = value;
                return;
            }
        } // CancelConfirmation

        /// <summary>
        /// Query if Cancel is desired.
        /// </summary>
        public bool CancellationPending
        {
            get
            {
                DoEvents();
                return _bCancellationPending;
            }
            private set
            {
                _bCancellationPending = value;
                return;
            }
        } // CancellationPending

        /// <summary>
        /// Get/set the refresh interval; be careful
        /// </summary>
        public UInt32 IntervalMs
        {
            get
            {
                return _uIntervalMs;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set IntervalMs from within a ProgressSection");
                }
                _uIntervalMs = value;
                return;
            }
        } // IntervalMs

        /// <summary>
        /// True if we are in a ProgressSection
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return (_bIsRunning);
            }
            private set
            {
                _bIsRunning = value;
                return;
            }
        } // IsRunning

        /// <summary>
        /// Maximum value for the progress bar; one shot that resets to 100
        /// </summary>
        public int Maximum
        {
            get
            {
                return _nMaximum;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set Maximum from within a ProgressSection");
                }
                _nMaximum = value;
                return;
            }
        } // Maximum

        /// <summary>
        /// Minimum value for the progress bar; one shot that resets to 0
        /// </summary>
        public int Minimum
        {
            get
            {
                return _nMinimum;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set Minimum from within a ProgressSection");
                }
                _nMinimum = value;
                return;
            }
        } // Minimum

        /// <summary>
        /// The Parent is the form that will be enabled/disabled
        /// </summary>
        public System.Windows.Forms.Form Parent
        {
            private get {
                return _frmParent;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set Form from within a ProgressSection");
                }
                _frmParent = value;
                return;
            }
        } // Parent

        /// <summary>
        /// Progress as a double between 0.0 and 1.0
        /// </summary>
        public double Progress
        {
            get
            {
                return (Maximum > Minimum) ? (double)(Value - Minimum) / (Maximum - Minimum) : 0.0;
            }
        } // Progress

        /// <summary>
        /// The status strip will be (partially) overlaid with the COEWaitForm
        /// </summary>
        public System.Windows.Forms.StatusStrip StatusStrip
        {
            private get
            {
                return _ssStatusStrip;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set StatusStrip from within a ProgressSection");
                }
                _ssStatusStrip = value;
                return;
            }
        } // StatusStrip

        /// <summary>
        /// Set the status bar text
        /// </summary>
        public string StatusText
        {
            set
            {
                if (IsRunning == false)
                {
                    throw new Exception("Cannot set Status except within a ProgressSection");
                }
                string strStatusText = value;
                if (_bHasUI == true)
                {
                    long lNow = DateTime.Now.Ticks / _kTicksPerMillisecond;
                    if (_lStatusTextTicks < (lNow - IntervalMs))
                    {
                        _lStatusTextTicks = lNow;
                        _ToolStripStatusLabel.Text = strStatusText;
                        _ToolStripStatusLabel.Owner.Update();
                    }
                    DoEvents();
                }
                return;
            } // set
        } // StatusText

        /// <summary>
        /// Indicate whether Cancel is supported; one shot that resets to true
        /// </summary>
        public bool SupportsCancellation
        {
            private get
            {
                return _bSupportsCancellation;
            }
            set
            {
                if (IsRunning)
                {
                    throw new Exception("Cannot set SupportsCancellation from within a ProgressSection");
                }
                _bSupportsCancellation = value;
                return;
            }
        } // SupportsCancellation

        /// <summary>
        /// Get/set the progress bar value
        /// </summary>
        public int Value
        {
            get
            {
                return _nValue;
            } // get
            set
            {
                if (IsRunning == false)
                {
                    throw new Exception("Cannot set ProgressValue except within a ProgressSection");
                }
                _nValue = value;
                if (_bHasUI == true)
                {
                    long lNow = DateTime.Now.Ticks / _kTicksPerMillisecond;
                    if (_lValueTicks < (lNow - IntervalMs))
                    {
                        _lValueTicks = lNow;
                        if (_ToolStripProgressBar.Visible)
                        {
                            if (Value < _ToolStripProgressBar.Minimum) Value = _ToolStripProgressBar.Minimum;
                            if (Value > _ToolStripProgressBar.Maximum) Value = _ToolStripProgressBar.Maximum;
                            _ToolStripProgressBar.Value = Value;
                        }
                        if (_ToolStripLabel.Visible)
                        {
#if VERSION1
                            _ToolStripLabel.BackgroundImage.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
#endif
                            {
                                System.Windows.Forms.ImageList il = (System.Windows.Forms.ImageList)((Object[])_ToolStripLabel.Tag)[0];
                                int nImage = (Int32)((Object[])_ToolStripLabel.Tag)[1];
                                nImage = (nImage + 1) % il.Images.Count;
                                ((Object[])_ToolStripLabel.Tag)[1] = nImage;
                                _ToolStripLabel.BackgroundImage = il.Images[nImage];
                            }
                            {
                                System.Windows.Forms.ImageList il = (System.Windows.Forms.ImageList)((Object[])_ToolStripLabelBack.Tag)[0];
                                int nImage = (Int32)((Object[])_ToolStripLabelBack.Tag)[1];
                                nImage = (nImage + 1) % il.Images.Count;
                                ((Object[])_ToolStripLabelBack.Tag)[1] = nImage;
                                _ToolStripLabelBack.BackgroundImage = il.Images[nImage];
                            }
                            if (Maximum > Minimum)
                            {
                                _ToolStripLabel.Width = (int)(Progress * 240);
                                _ToolStripLabelBack.Width = 240 - _ToolStripLabel.Width;
                            }
                        }
                    }
                    DoEvents();
                }
                return;
            } // set
        } // Value

        #endregion

        #region support methods
        /// <summary>
        /// Announce the desire to Cancel
        /// </summary>
        public void Cancel()
        {
            if (IsRunning)
            {
                if (SupportsCancellation)
                {
                    CancellationPending = true;
                }
            }
            return;
        } // Cancel()

        private void DoEvents()
        {
            long lNow = DateTime.Now.Ticks / _kTicksPerMillisecond;
            if (_lDoEventsTicks < (lNow - IntervalMs))
            {
                _lDoEventsTicks = lNow;
                // ??? Parent.BringToFront();
                if (_COEWait != null) _COEWait.BringToFront();
                System.Windows.Forms.Application.DoEvents();
            }
            return;
        } // DoEvents()

        #endregion

        #region delegate and main method
        // Delegate required by ProgressSection
        /// <summary>
        /// <para>Time consuming code to be executed in the <see cref="COEWait"/> context</para>
        /// <para>For use with <see cref="ProgressSection"/></para>
        /// </summary>
        public delegate void ProgressHelper_TimeConsumingCode();
        // ProgressSection
        /// <summary>
        /// Used to encapsulate time consuming code with feedback and cancel if a UI is present
        /// </summary>
        /// <param name="TimeConsumingCode">Ecapsulates the time comsuming code. See <see cref="ProgressHelper_TimeConsumingCode"/></param>
        /// <example>
        /// Example of ProgressSection usage:
        /// <code>
        /// Ph.ProgressSection(delegate()
        /// {
        ///     int cRows = oDataTable.Rows.Count;
        ///     int nRows;
        ///     foreach (DataRow oDataRow in oDataSet.Tables[0].Rows)
        ///     {
        ///         if (Ph.CancellationPending) break;
        ///         Ph.ProgressValue = (100 * nRow) / cRows;
        ///         Ph.StatusText = "Writing to text file. Row " + (1 + nRow) + " of " + cRows;
        ///         ...
        ///         nRow++;
        ///     } // foreach (DataRow oDataRow in oDataSet.Tables[0].Rows)
        /// });
        /// </code>
        /// </example>
        public void ProgressSection(ProgressHelper_TimeConsumingCode TimeConsumingCode)
        {
            IsRunning = true;
            _bHasUI = (Parent != null);
            if (_bHasUI)
            {
                _COEWait = new COEWait();   // WJC add constructor that takes Parent ???
                _COEWait.Button.BackColor = Parent.BackColor;
                _COEWait.StatusLabel.BackColor = Parent.BackColor;
                _COEWait.Button.BackColor = Parent.BackColor;
                _COEWait.ProgressBar.BackColor = Parent.BackColor;
                _COEWait.ProgressLabel.BackColor = Parent.BackColor;
                _COEWait.Visible = true;
                // Extract controls from the COEWait
                _ToolStripButton = _COEWait.Button;
                _ToolStripStatusLabel = _COEWait.StatusLabel;
                _ToolStripProgressBar = _COEWait.ProgressBar;
                _ToolStripLabel = _COEWait.ProgressLabel;
                _ToolStripLabelBack = _COEWait.ProgressLabelBack;
            }

            string strStatusSave = (_ToolStripStatusLabel != null) ? _ToolStripStatusLabel.Text : "";
            if (_ToolStripButton != null)
            {
                _ToolStripButton.Visible = SupportsCancellation;
            }
            if (_ToolStripProgressBar != null)
            {
                if (Maximum > 0)
                {
                    _ToolStripLabel.Visible = false;
                    _ToolStripProgressBar.Value = _ToolStripProgressBar.Minimum = 0;
                    _ToolStripProgressBar.Minimum = Minimum;
                    _ToolStripProgressBar.Maximum = Maximum;
                    _ToolStripProgressBar.Visible = true;
                    _ToolStripProgressBar.Visible = false;  // Fooling
                    _ToolStripLabel.Visible = true; // Fooling
                }
                else if (Maximum == 0)
                {
                    _ToolStripProgressBar.Visible = false;
                    _ToolStripLabel.Visible = true;
                }
                else
                {
                    _ToolStripProgressBar.Visible = false;
                    _ToolStripLabel.Visible = false;
                }
                _ToolStripLabelBack.Visible = _ToolStripLabel.Visible;
            }

            if (_bHasUI)
            {
                _lDoEventsTicks = _lValueTicks = _lStatusTextTicks = 0; // Formerly (DateTime.Now.Ticks / _kTicksPerMillisecond);
                {
                    if (Parent != null)
                    {
                        Parent.Enabled = false;
                        Parent.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Parent_FormClosing);
                        Parent.MaximizeBox = false;
                        Parent.MinimizeBox = false;
                    }
                    //
                    CancellationPending = false;
                    _COEWait.Cancel += new EventHandler(COEWait_Cancel);
                    _COEWait.Left = Parent.Left + (Parent.Width - Parent.ClientSize.Width) / 2 + StatusStrip.Left;
                    _COEWait.Top = Parent.Top + (Parent.Height - Parent.ClientSize.Height - Parent.Margin.Bottom) + StatusStrip.Top;
                    _COEWait.Width = Parent.ClientSize.Width;
                    _COEWait.Show();
                    _COEWait.Update();
                    _COEWait.Activate();
                }
            }
            TimeConsumingCode();
            if (_bHasUI)
            {
                _COEWait.Hide();
                if (Parent != null)
                {
                    Parent.MaximizeBox = true;
                    Parent.MinimizeBox = true;
                    Parent.Enabled = true;
                    Parent.FormClosing -= Parent_FormClosing;
                    Parent.Activate();
                    _COEWait = null;
                }
            }

            if (_ToolStripProgressBar != null)
            {
                _ToolStripProgressBar.Value = _ToolStripProgressBar.Minimum;
                _ToolStripProgressBar.Visible = false;
            }
            if (_ToolStripStatusLabel != null) _ToolStripStatusLabel.Text = strStatusSave;
            if (_bHasUI) System.Windows.Forms.Application.DoEvents();    // WJC not helpful?
            Value = 0;
            CancellationPending = false;
            // One shots
            IsRunning = false;
            SupportsCancellation = _kbSupportsCancellation;
            Minimum = _knMinimum;
            Maximum = _knMaximum;
            CancelConfirmation = _kstrCancelConfirmation;

            return;
        } // ProgressSection()
        #endregion

        #region event handlers
        void Parent_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            e.Cancel = true;
            return;
        } // frmProgressHelper_FormClosing()

        void COEWait_Cancel(object sender, EventArgs e)
        {
            _ToolStripButton.Enabled = false;
            System.Windows.Forms.DialogResult dr;
            if (CancelConfirmation != "")
            {
                dr = System.Windows.Forms.MessageBox.Show(CancelConfirmation, "Stop", System.Windows.Forms.MessageBoxButtons.YesNo);
            }
            else
            {
                dr = System.Windows.Forms.DialogResult.Yes; // Treat no confirmation message as an automatic "Yes"
            }
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                _ToolStripButton.Text = "Cancel pending";
                CancellationPending = true;
            }
            else
            {
                _ToolStripButton.Enabled = true;
            }
            return;
        } // COEWait_Cancel()
        #endregion
    } // class COEProgressHelper
}
