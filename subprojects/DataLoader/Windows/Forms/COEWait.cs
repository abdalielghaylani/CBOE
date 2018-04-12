using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.DataLoader.Windows.Controls;   // For UIBase.GetGif

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
    /// <summary>
    /// This Form provides UI during time consuming operations
    /// </summary>
    public partial class COEWait : Form
    {
        #region data for actual controls
        private readonly ToolStripButton _tsbCancel = new ToolStripButton();
        private readonly ToolStripProgressBar _tspbProgress = new ToolStripProgressBar();
        private readonly ToolStripLabel _tslProgress = new ToolStripLabel();
        private readonly ToolStripLabel _tslProgressBack = new ToolStripLabel();
        private readonly ToolStripStatusLabel _tsslStatus = new ToolStripStatusLabel();
        private readonly StatusStrip _ssStatus = new StatusStrip();
        #endregion

        #region properties
        /// <summary>
        /// Access to the Cancel button
        /// </summary>
        public ToolStripButton Button
        {
            get
            {
                return _tsbCancel;
            }
        } // Button

        /// <summary>
        /// Access to the progress bar; used for finite progress
        /// </summary>
        public ToolStripProgressBar ProgressBar
        {
            get
            {
                return _tspbProgress;
            }
        } // ProgressBar

        /// <summary>
        /// Access to the label; used for icon display indefinite progress
        /// </summary>
        public ToolStripLabel ProgressLabel
        {
            get
            {
                return _tslProgress;
            }
        } // ProgressLabel

        /// <summary>
        /// Access to the label background; used for icon display indefinite progress
        /// </summary>
        public ToolStripLabel ProgressLabelBack
        {
            get
            {
                return _tslProgressBack;
            }
        } // ProgressLabelBack

        /// <summary>
        /// Access to the status label; used for status text
        /// </summary>
        public ToolStripStatusLabel StatusLabel
        {
            get
            {
                return _tsslStatus;
            }
        } // StatusLabel

        #endregion

        #region constructors

        /// <summary>
        /// !!
        /// </summary>
        public COEWait()
        {
            InitializeComponent();
            //this.BackColor = Color.HotPink; // For debugging
            _ssStatus.Name = "_ssStatus";
            _ssStatus.SizingGrip = false;
            // Status label
            {
                StatusLabel.Name = "tsslStatus";
                StatusLabel.Alignment = ToolStripItemAlignment.Left;
                StatusLabel.AutoSize = false;
                StatusLabel.TextAlign = ContentAlignment.MiddleLeft;
                _ssStatus.Items.Add(StatusLabel);
            }
            // Cancel button
            {
                Button.Name = "tsbCancel";
                Button.AutoSize = true;
                Button.Click += new EventHandler(tsbCancel_Click);
                Button.Text = "Stop";
                _ssStatus.Items.Add(Button);
            }
            // Progress bar
            {
                ProgressBar.Name = "tspbProgress";
                ProgressBar.Alignment = ToolStripItemAlignment.Right;
                ProgressBar.AutoSize = false;
                ProgressBar.Width = (1 + 12 * (1 + 18 + 1) + 2);
                _ssStatus.Items.Add(ProgressBar);
            }
            // Progress icons
            {
                ProgressLabel.Name = "tslProgress";
                ProgressLabel.Alignment = ToolStripItemAlignment.Right;
                ProgressLabel.AutoSize = false;
                //?ProgressLabel.Margin = new Padding(0, 2, 0, 3);   // 0, 1, 0, 2
                ProgressLabel.Margin = new Padding(0, 3, 0, 2);   // 0, 1, 0, 2
                ProgressLabel.Width = 24 * (ProgressBar.Width / 24);
                {
                    ImageList il = new ImageList();
                    il.ImageSize = new Size(240, 24);
                    {
                        Image oImage;
                        oImage = UIBase.GetGif("BlueBar00");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("BlueBar04");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("BlueBar08");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("BlueBar12");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("BlueBar16");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("BlueBar20");
                        il.Images.Add(oImage);
                    }
                    ProgressLabel.Tag = new Object[] { il, 0 };
                    int nImage = (Int32)((Object[])ProgressLabel.Tag)[1];
                    ProgressLabel.BackgroundImage = ((ImageList)((Object[])ProgressLabel.Tag)[0]).Images[nImage];
                    ProgressLabel.BackgroundImageLayout = ImageLayout.Tile;
                }
                _ssStatus.Items.Add(ProgressLabel);

                ProgressLabelBack.Name = "tslProgress";
                ProgressLabelBack.Alignment = ToolStripItemAlignment.Right;
                ProgressLabelBack.AutoSize = false;
                //?ProgressLabel.Margin = new Padding(0, 2, 0, 3);   // 0, 1, 0, 2
                ProgressLabelBack.Margin = new Padding(0, 3, 0, 2);   // 0, 1, 0, 2
                ProgressLabelBack.Width = 24 * (ProgressBar.Width / 24);
                {
                    ImageList il = new ImageList();
                    il.ImageSize = new Size(240, 24);
                    {
                        Image oImage;
                        oImage = UIBase.GetGif("GrayBar00");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("GrayBar04");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("GrayBar08");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("GrayBar12");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("GrayBar16");
                        il.Images.Add(oImage);
                        oImage = UIBase.GetGif("GrayBar20");
                        il.Images.Add(oImage);
                    }
                    ProgressLabelBack.Tag = new Object[] { il, 0 };
                    int nImage = (Int32)((Object[])ProgressLabelBack.Tag)[1];
                    ProgressLabelBack.BackgroundImage = ((ImageList)((Object[])ProgressLabelBack.Tag)[0]).Images[nImage];
                    ProgressLabelBack.BackgroundImageLayout = ImageLayout.Tile;
                }
                _ssStatus.Items.Add(ProgressLabelBack);
            }
            //
            _ssStatus.Height = (1 + 2 + 24 + 2);
            this.Controls.Add(_ssStatus);

            // If the caption is empty and ControlBox is false then the caption bar does not show at all
            this.Text = string.Empty;
            this.Anchor = AnchorStyles.None;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            // HelpButton
            // Icon
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(COEWait_KeyDown);
            this.Layout += new LayoutEventHandler(COEWait_Layout);
            // ShowIcon (true)
            this.ShowInTaskbar = false;

            this.StartPosition = FormStartPosition.Manual;
            this.Width = _ssStatus.Width + (this.Width - this.ClientSize.Width);
            //this.Height = _ssStatus.Height + (this.Height - this.ClientSize.Height);
            this.Height = _ssStatus.Height;
            return;
        } // COEWait()

        #endregion

        #region event declarations
        /// <summary>
        /// Cancel is raised when the Cancel button is pressed
        /// </summary>
        public event EventHandler Cancel;
        #endregion

        #region event handlers

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void COEWait_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DL;
        }

        private void COEWait_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        } // COEWait_KeyDown()

        void COEWait_Layout(object sender, LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                int nWidth = Width;
                nWidth -= 1;
                if (ProgressBar.Visible) nWidth -= ProgressBar.Width;
                if (ProgressLabel.Visible) nWidth -= ProgressLabel.Width;
                if (Button.Visible) nWidth -= Button.Width;
                StatusLabel.Width = nWidth;
            }
            return;
        } // COEWait_Layout()

        /// <summary>
        /// EventHandler for the Cancel button
        /// </summary>
        private void OnCancel(EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, e);
            }
            return;
        } // OnCancel()

        private void tsbCancel_Click(object sender, EventArgs e)
        {
            OnCancel(new EventArgs());
            return;
        }// tsbCancel_Click()

        #endregion

    } // class COEWait
}