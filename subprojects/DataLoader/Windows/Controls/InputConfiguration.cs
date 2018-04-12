using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// !!
    /// </summary>
    public partial class InputConfiguration : UIBase
    {
        #region data
        private Configuration _grpConfiguration;
        #endregion

        #region properties
        /// <summary>
        /// Get property that indicates whether configuration is available
        /// </summary>
        public bool HasSettings
        {
            get
            {
                return _grpConfiguration.HasSettings;
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
                return _grpConfiguration.Settings;
            }
            set
            {
                _grpConfiguration.Settings = value;
                return;
            }
        } // Settings

        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public InputConfiguration()
        {
            StatusText = "Configure input";
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _grpConfiguration added later by set Configuration
            _grpConfiguration = new Configuration();
            Controls.Add(_grpConfiguration);
            // btnAccept
            Controls.Add(AcceptButton);
            // btnCancel
            Controls.Add(CancelButton);
            // events
            AcceptButton.Click += new EventHandler(AcceptButton_Click);
            CancelButton.Click += new EventHandler(CancelButton_Click);
            Layout += new LayoutEventHandler(InputConfiguration_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            return;
        } // InputConfiguration()
        #endregion

        #region event handlers
        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
            return;
        }
        private void AcceptButton_Click(object sender, EventArgs e)
        {
            OnAccept();
            return;
        }
        private void InputConfiguration_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Bounds"))
            {
                // Vertical
                int y = 0;
                _grpConfiguration.Top = y;
                y += _grpConfiguration.Height + UIBase.ExtraPadding.Top;
                CancelButton.Top = y;
                AcceptButton.Top = y;
                y += AcceptButton.Height;
                Height = y;
                // Horizontal
                int x = 0;
                _grpConfiguration.Left = x;
                x += _grpConfiguration.Width;
                Width = x;
                // Horizontal
                x = 0;
                CancelButton.Left = x;
                x += CancelButton.Width;
                AcceptButton.Left = x;
                x += AcceptButton.Width;
                if (Width < x) Width = x;
                //
                if (_grpConfiguration.Width < Width) _grpConfiguration.Width = Width;
                x = Width;
                x -= AcceptButton.Width;
                AcceptButton.Left = x;
                x -= CancelButton.Width;
                CancelButton.Left = x;
            }
            return;
        } // InputConfiguration_Layout()
        #endregion
    } // class InputConfiguration
}
