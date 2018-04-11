using System;
using System.Collections.Generic;
using System.Text;
using AssemblySettings;
using System.Reflection;
using System.IO;

namespace SpotfireIntegration.SpotfireAddin {
    
    partial class COESettings
    {
        // This custom constructor ensures that the settings are read from the custom COESettings.config file
        // which is expected to be located next to the SpotfireIntegration.SpotfireAddin.dll
        COESettings() : base(new ConfigurationFileApplicationSettings(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\COESettings.config", typeof(COESettings))) { }
    } 
}
