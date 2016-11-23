using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.AddIns;
using IronPython.Hosting;
using System.Xml;
using System.IO;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    [Serializable]
    public class PythonAddIn : IAddIn
    {
        #region Methods
        string _currentScript;

        public PythonAddIn()
        {
        }

        public void OnEventHandler(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(_currentScript))
            {
                IRegistryRecord record = (IRegistryRecord)sender;
                PythonEngine python = new PythonEngine();

                EngineModule engineModule = python.CreateModule();

                engineModule.Globals.Add(record.GetType().Name, record);

                python.Execute(_currentScript, engineModule);
            }
        }
        #endregion

        #region IAddIn Members
        IRegistryRecord _registryRecord;

        public IRegistryRecord RegistryRecord
        {
            get
            {
                return _registryRecord;
            }
            set
            {
                _registryRecord = value;
            }
        }

        public void Initialize(string xmlConfiguration)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlConfiguration);

            XmlNode fileNode = document.SelectSingleNode("ScriptFile");
            if (fileNode != null)
            {
                if (File.Exists(fileNode.InnerText))
                    _currentScript = File.ReadAllText(fileNode.InnerText);
                else
                    throw new Exception(string.Format("File not found: {0}", fileNode.InnerText));
            }
        }

        #endregion
    }
}
