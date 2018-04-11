using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace CambridgeSoft.COE.Patcher.Repository
{
    /// <summary>
    /// The Repository that uses file system as the source of configurations.
    /// </summary>
    public class FileSystemConfigurationRepository : IConfigurationRepository
    {
        private string _folderPath = string.Empty;

        public FileSystemConfigurationRepository(string folderPath)
        {
            this._folderPath = folderPath;
        }

        #region IConfigurationRepository Members

        public IEnumerable<XmlDocument> GetAllCoeFormGroups()
        {
            List<XmlDocument> allCoeFormGroups = new List<XmlDocument>();
            XmlDocument coeFormGroup = null;

            DirectoryInfo di = new DirectoryInfo(Path.Combine(_folderPath, "CoeForms"));
            foreach (FileInfo fi in di.GetFiles())
            {
                coeFormGroup = new XmlDocument();
                coeFormGroup.Load(fi.FullName);

                allCoeFormGroups.Add(coeFormGroup);
            }

            return allCoeFormGroups;
        }

        public IEnumerable<XmlDocument> GetAllCoeDataViews()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public IEnumerable<XmlDocument> GetAllConfigurationSettings()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public XmlDocument GetCoeObjectConfig()
        {
            XmlDocument coeObjectConfig = new XmlDocument();
            coeObjectConfig.Load(Path.Combine(_folderPath, "COEObjectConfig.xml"));

            return coeObjectConfig;
        }

        public XmlDocument GetFrameworkConfig()
        {
            XmlDocument fwConfig = new XmlDocument();
            fwConfig.Load(_folderPath + @"\COEFrameworkConfig.xml");

            return fwConfig;
        }

        public XmlDocument GetSingleCoeFormGroupById(string formGroupId)
        {
            string filePath = Path.Combine(_folderPath, "CoeForms\\" + formGroupId + ".xml");
            XmlDocument coeFormGroup = new XmlDocument();
            coeFormGroup.Load(filePath);

            return coeFormGroup;
        }

        public XmlDocument GetSingleCOEDataViewById(string dataViewId)
        {
            string filePath = Path.Combine(_folderPath, "CoeDataViews\\" + dataViewId + ".xml");
            XmlDocument coeDataView = new XmlDocument();
            coeDataView.Load(filePath);

            return coeDataView;
        }

        #endregion
    }
}
