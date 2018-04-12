using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.ConfigLoader.Windows.Controls;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEGenericObjectStorageService;

namespace CambridgeSoft.COE.ConfigLoader.Data.InputObjects
{
    class InputObjectConfigExp : InputObject
    {
        //Constructor
        public InputObjectConfigExp()
        {
            Filter = "";
            IsValid = true;
            return;
        }

        //Properties / Data
        private readonly List<string> _File = new List<string>();
        private int _FilePosition;
        private string _user = Csla.ApplicationContext.User.Identity.Name;

        public override string Configuration
        {
            get
            {
                return base.Configuration;
            } // get
        } // Configuration
        private int FilePosition
        {
            get
            {
                return _FilePosition;
            }
            set
            {
                _FilePosition = value;
                return;
            }
        } // FilePosition

        //Methods
        public override void CloseDb()
        {
            return;
        } // CloseDb()

        public override bool OpenDb()
        {
            ClearMessages();
            do
            {
                // Build TableList
                {
                    ClearTableList();
                    AddTableToTableList("ConfigTable");  // Just to indicate there there is a single table
                }
            } while (false);
            return HasMessages;
        } // OpenDb()

        public override bool CloseTable()
        {
            ClearMessages();
            return HasMessages;
        } // CloseTable()

        public override bool OpenTable()
        {
            ClearMessages();
            do
            {
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlist");
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("dbname", "XML");
                        oCOEXmlTextWriter.WriteAttributeString("dbtype", "System.String");
                        oCOEXmlTextWriter.WriteAttributeString("name", "XML");
                        oCOEXmlTextWriter.WriteAttributeString("type", "string");
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    oCOEXmlTextWriter.WriteEndElement();
                    InputFieldSpec = UIBase.FormatXmlString(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
            } while (false);
            return HasMessages;
        } // OpenTable()

        static string Wrap(
            string vstrFolder,
            string DatabaseName,
            string Description,
            int FormGroup,
            int ID,
            bool IsPublic,
            string Name,
            string UserName,
            string vXml
        )
        {
            string strRet = "";
            COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
            oCOEXmlTextWriter.WriteStartElement("configuration");
            oCOEXmlTextWriter.WriteAttributeString("subdirectory", vstrFolder);
            oCOEXmlTextWriter.WriteAttributeString("databasename", DatabaseName);
            oCOEXmlTextWriter.WriteAttributeString("description", Description);
            oCOEXmlTextWriter.WriteAttributeString("formgroup", FormGroup.ToString());
            oCOEXmlTextWriter.WriteAttributeString("id", ID.ToString());
            oCOEXmlTextWriter.WriteAttributeString("ispublic", IsPublic.ToString());
            oCOEXmlTextWriter.WriteAttributeString("name", Name);
            oCOEXmlTextWriter.WriteAttributeString("username", UserName);
            {
                oCOEXmlTextWriter.WriteStartElement("xml");
                oCOEXmlTextWriter.WriteCData(vXml);
                oCOEXmlTextWriter.WriteEndElement();
            }
            oCOEXmlTextWriter.WriteEndElement();
            // strRet = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString); slow!
            strRet = oCOEXmlTextWriter.XmlString;
            oCOEXmlTextWriter.Close();
            return strRet;
        } // Wrap()

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            OpenDataSet();
            _File.Clear();
            // // Populate list of records
            // Dataviews
            {
                COEDataViewBOList oCOEDataViewBOList = COEDataViewBOList.GetDataViewDataList();
                foreach (COEDataViewBO oCOEDataViewBO in oCOEDataViewBOList)
                {
                    if (!oCOEDataViewBO.IsDeleted)
                    {
                        string DatabaseName = oCOEDataViewBO.DatabaseName;  // database
                        string Description = oCOEDataViewBO.Description;    // description EMPTY
                        int FormGroup = oCOEDataViewBO.FormGroup;
                        int ID = oCOEDataViewBO.ID;
                        bool IsPublic = oCOEDataViewBO.IsPublic;
                        string Name = oCOEDataViewBO.Name;  // name EMPTY
                        string UserName = oCOEDataViewBO.UserName;
                        _File.Add(Wrap(
                            @"DataViews",
                            DatabaseName,
                            Description,
                            FormGroup,
                            ID,
                            IsPublic,
                            Name,
                            UserName,
                            oCOEDataViewBO.COEDataView.ToString()
                        ));
                    }
                }
            }
            // Forms\Client
            // Forms\Web
            {
                COEFormBOList oCOEFormBOList = COEFormBOList.GetCOEFormBOList();
                foreach (COEFormBO oCOEFormBO in oCOEFormBOList)
                {
                    if (!oCOEFormBO.IsDeleted)
                    {
                        COEFormBO oCOEFormBOthis = COEFormBO.Get(oCOEFormBO.ID);
                        string DatabaseName = oCOEFormBOthis.DatabaseName;
                        string Description = oCOEFormBOthis.Description;
                        int FormGroup = oCOEFormBOthis.FormGroupId; // dataViewId
                        int ID = oCOEFormBOthis.ID; // id
                        bool IsPublic = oCOEFormBOthis.IsPublic;
                        string Name = oCOEFormBOthis.Name;
                        string UserName = oCOEFormBOthis.UserName;
                        _File.Add(Wrap(
                            @"Forms\Web",
                            DatabaseName,
                            Description,
                            FormGroup,
                            ID,
                            IsPublic,
                            Name,
                            UserName,
                            oCOEFormBOthis.COEFormGroup.ToString()
                        ));
                    }

                }
            }
            // GenericObjects
            {
                COEGenericObjectStorageBOList oCOEGenericObjectStorageBOList = COEGenericObjectStorageBOList.GetList();
                foreach (COEGenericObjectStorageBO oCOEGenericObjectStorageBO in oCOEGenericObjectStorageBOList)
                {
                    COEGenericObjectStorageBO oCOEGenericObjectStorageBOthis = COEGenericObjectStorageBO.Get(oCOEGenericObjectStorageBO.ID);
                    if (!oCOEGenericObjectStorageBOthis.IsDeleted)
                    {
                        string DatabaseName = oCOEGenericObjectStorageBOthis.DatabaseName;
                        string Description = oCOEGenericObjectStorageBOthis.Description;
                        int FormGroup = oCOEGenericObjectStorageBOthis.FormGroup;
                        int ID = oCOEGenericObjectStorageBOthis.ID;
                        bool IsPublic = oCOEGenericObjectStorageBOthis.IsPublic;
                        string Name = oCOEGenericObjectStorageBOthis.Name;
                        string UserName = oCOEGenericObjectStorageBOthis.UserName;
                        _File.Add(Wrap(
                            @"GenericObjects",
                            DatabaseName,
                            Description,
                            FormGroup,
                            ID,
                            IsPublic,
                            Name,
                            UserName,
                            oCOEGenericObjectStorageBOthis.COEGenericObject.ToString()
                        ));
                    }
                }
            }
            Records = _File.Count;
            FilePosition = 0;
            Minimum = Value = (Int32)0;
            Maximum = (Int32)Records;
            return HasMessages;
        } // OpenDataSet()

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = DataSetForJob;
            DataTable oDataTable = riDataSet.Tables[0];
            while (vcLimit-- > 0)
            {
                if (FilePosition >= Records) break;
                DataRow oDataRow = oDataTable.NewRow();
                oDataRow[0] = _File[FilePosition++];
                oDataTable.Rows.Add(oDataRow);
            } // while (vcLimit-- > 0)
            Value = (Int32)FilePosition;
            return HasMessages;
        } // ReadDataSet()

        protected override DataSet ReadDataSetForPreview()
        {
            return null;
        } // ReadDataSetForPreview()

    }
}
