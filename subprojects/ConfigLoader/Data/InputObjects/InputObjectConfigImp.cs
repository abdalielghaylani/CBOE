using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.ConfigLoader.Windows.Controls;

namespace CambridgeSoft.COE.ConfigLoader.Data.InputObjects
{
    class InputObjectConfigImp : InputObject
    {

        private readonly List<string> _File = new List<string>();

        private int _FilePosition;
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
        }

        public override string Configuration
        {
            get
            {
                return base.Configuration;
            } // get
        }

        public InputObjectConfigImp()
        {
            Filter = "";
            IsValid = true;
            return;
        }

        public override void CloseDb()
        {
            return;
        }

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
        }

        public override bool CloseTable()
        {
            ClearMessages();
            return HasMessages;
        }

        private void OpenTableRecurse(string vDirectory)
        {
            string[] directories = Directory.GetDirectories(vDirectory);
            foreach (string directory in directories)
            {
                OpenTableRecurse(directory);
            }
            string[] files = Directory.GetFiles(vDirectory, "*.xml");
            foreach (string file in files)
            {
                _File.Add(file);
            }
            return;
        }

        public override bool OpenTable()
        {
            ClearMessages();
            do
            {
                _File.Clear();
                OpenTableRecurse(Db);
                Records = _File.Count;
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlist");
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("dbname", "Metadata");
                        oCOEXmlTextWriter.WriteAttributeString("dbtype", "System.String");
                        oCOEXmlTextWriter.WriteAttributeString("name", "Metadata");
                        oCOEXmlTextWriter.WriteAttributeString("type", "string");
                        oCOEXmlTextWriter.WriteEndElement();
                    }
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
        }

        public override bool OpenDataSet(int vnStart, int vcLimit)
        {
            OpenDataSet();
            FilePosition = 0;
            Minimum = Value = (Int32)0;
            Maximum = (Int32)_File.Count;
            return HasMessages;
        }

        public override bool ReadDataSet(int vcLimit, ref System.Data.DataSet riDataSet)
        {
            riDataSet = DataSetForJob;
            DataTable oDataTable = riDataSet.Tables[0];
            while (vcLimit-- > 0)
            {
                if (FilePosition >= Records) break;
                DataRow oDataRow = oDataTable.NewRow();
                string path = _File[FilePosition++];
                string Xml = File.ReadAllText(path);
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.LoadXml(Xml);
                XmlNode oXmlNode = oXmlDocument.DocumentElement;
                XmlAttribute oXmlAttribute = oXmlNode.Attributes["subdirectory"];
                string subDirectory = oXmlAttribute.Value.ToString();
                oDataRow[0] = subDirectory;
                oDataRow[1] = Xml;
                oDataTable.Rows.Add(oDataRow);
            } // while (vcLimit-- > 0)
            Value = (Int32)FilePosition;
            return HasMessages;
        }

        protected override DataSet ReadDataSetForPreview()
        {
            return null;
        }

    }
}
