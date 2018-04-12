using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// UI for optional output configuration
    /// </summary>
    public partial class OutputConfiguration : UIBase
    {
        #region data
        private Configuration _OutputConfigurationGroupBox;
        #endregion

        #region properties
        /// <summary>
        /// Get property that indicates whether configuration is available
        /// </summary>
        public bool HasSettings
        {
            get
            {
                return _OutputConfigurationGroupBox.HasSettings;
            }
        } // HasSettings

        /// <summary>
        /// Get the configuration requirements and settings
        /// Set the configuration settings
        /// </summary>
        public string Settings
        {
            get
            {
                return _OutputConfigurationGroupBox.Settings;
            }
            set
            {
                _OutputConfigurationGroupBox.Settings = value;
            }
        } // Settings
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public OutputConfiguration()
        {
            StatusText = "Configure the output";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _OutputConfigurationGroupBox added later by set Configuration
            _OutputConfigurationGroupBox = new Configuration();
            Controls.Add(_OutputConfigurationGroupBox);
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(OutputConfiguration_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // OutputConfiguration()
        #endregion

        #region event handlers
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept();
            return;
        } // AcceptButton_Click()

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        } // CancelButton_Click()

        private void OutputConfiguration_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                _OutputConfigurationGroupBox.Top = y;
                y += _OutputConfigurationGroupBox.Height + UIBase.ExtraPadding.Bottom;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _OutputConfigurationGroupBox.Left = x;
                x += _OutputConfigurationGroupBox.Width;
                Width = x;
                // Horizontal
                x = 0;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                //
                if (_OutputConfigurationGroupBox.Width < Width) _OutputConfigurationGroupBox.Width = Width;
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            }
            return;
        } // OutputConfiguration_Layout()
        #endregion
    } // class OutputConfiguration
}
