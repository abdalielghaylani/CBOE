using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;

namespace COESearchServiceTest
{
    public partial class ClientConfiguration : Form
    {
        COEClientConfigurationManager manager = null;
        public LoggingEnabledDelegate LoggingEnabledCallback;

        public ClientConfiguration()
        {
            InitializeComponent();

            //create the coeclientconfigurationmanager, this will populate the manager properties
            //with the current configuration
            manager = new COEClientConfigurationManager();

            //add a property grid to the windows form and set the manager as the selected object
            this.propertyGrid1.SelectedObject = manager;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            //saves the coe configuration to the local client configuration.
            manager.SaveSettings();

            LoggingEnabledCallback(manager.LoggingEnabled);
            this.propertyGrid1.Dispose();
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.propertyGrid1.Dispose();
            this.Close();
        }
    }
}