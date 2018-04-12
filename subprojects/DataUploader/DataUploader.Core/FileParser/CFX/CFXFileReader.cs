using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

using CambridgeSoft.COE.DataLoader.Core;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.FileParser.Access;
using CambridgeSoft.COE.DataLoader.Core.FileParser.CSV;

namespace CambridgeSoft.COE.DataLoader.Core.FileParser.CFX
{
    /// <summary>
    /// Enables reading basic component data (structure and some properties) from a ChemFinder
    /// format (CFX). Wraps the AccessOleDbReader for reading the MSAccess data, and MolServer
    /// for reading chemical structural data.
    /// </summary>
    public class CFXFileReader : FileReaderBase
    {
        //for MolServer itself
        [DllImport("USER32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int RegisterClipboardFormat(string format);
        private int _chemDrawInterchangeFormat = 0;

        private const string ACCESS_XREF_FIELD = "Mol_ID";
        private const string CHEMDRAW_INTERCHANGE_FORMAT = "ChemDraw Interchange Format";

        private StreamReader _reader = null;
        private MolServer.Document _molDocument = null;
        private AccessOleDbReader _dbReader = null;

        #region > Properties <

        private bool _hasValidReferences;
        /// <summary>
        /// Read only. Set by the instance constructor if the structure and DB files exist.
        /// </summary>
        public bool HasValidReferences
        {
            get { return _hasValidReferences; }
        }

        private FileInfo _cfxFileInfo;
        /// <summary>
        /// Read only
        /// </summary>
        public FileInfo CfxFileInfo
        {
            get { return this._cfxFileInfo; }
        }

        #endregion

        #region > Constructors <

        /// <summary>
        /// Provides the file path for the XML document which contains the metadata for reading the
        /// associated database and structure files.
        /// </summary>
        /// <remarks>
        /// The core data-extraction process centers around the Access table, using the Mol_ID
        /// column to cross-reference the structures via the Molserver COM service wrapper.
        /// </remarks>
        /// <param name="filePath">the complete path to the file to be read</param>
        public CFXFileReader(string filePath)
            : base(filePath)
        {
            _reader = new StreamReader(this._stream);
            this._cfxFileInfo = new FileInfo(filePath);
            this._chemDrawInterchangeFormat = RegisterClipboardFormat(CHEMDRAW_INTERCHANGE_FORMAT);

            // If this registers with a return code of 0, it has failed
            if (this._chemDrawInterchangeFormat == 0)
                throw new Exception(string.Format("Unable to register {0}", CHEMDRAW_INTERCHANGE_FORMAT));
            else
                Initialize();
        }

        private void Initialize()
        {
            XmlDocument doc = new XmlDocument();

            //set up molserver
            _molDocument = new MolServer.Document();
            try
            {
                // open the MSAccess table or view
                string cfxDocContents = this._reader.ReadToEnd();
                _reader.Close();

                if (!string.IsNullOrEmpty(cfxDocContents))
                {
                    doc.LoadXml(cfxDocContents);
                    XmlNode connectionNode = doc.SelectSingleNode("/form/connections");
                    if (connectionNode != null)
                    {
                        string accessFileName = connectionNode.Attributes["filename"].Value;
                        //Coverity fix - CID 19214 
                        string accessTableName = string.Empty;
                        if (connectionNode.Attributes["tablename"].Value != null)
                        {
                            accessTableName = connectionNode.Attributes["tablename"].Value;
                        }
                        if (!string.IsNullOrEmpty(accessFileName) && !string.IsNullOrEmpty(accessTableName))
                        {
                            string parentDirectory = this._cfxFileInfo.DirectoryName;
                            string fullDbPath = Path.Combine(parentDirectory, accessFileName);
                            _dbReader = new AccessOleDbReader(fullDbPath, accessTableName, MSOfficeVersion.Unknown);
                        }
                    }

                    // open the base document
                    OpenMolDocument();

                    _hasValidReferences = true;
                }
            }
            catch
            {
                _hasValidReferences = false;
                throw;
            }
            finally
            {
                if (_molDocument != null && _molDocument.IsOpen)
                    _molDocument.Close();
            }
        }

        #endregion

        #region > Utilities <

        private void OpenMolDocument()
        {
            if (_molDocument != null && !_molDocument.IsOpen)
                _molDocument.Open(
                    this._cfxFileInfo.FullName
                    , Convert.ToInt32(MolServer.MSOpenModes.kMSReadOnly)
                    , string.Empty
                );
        }

        private MolServer.Molecule GetMoleculeById(int molId)
        {
            MolServer.Molecule mol = this._molDocument.GetMol(molId);
            return mol;
        }

        //private void ReadNoDatabase()
        //{
        //    // if no MSAccess document...what then?
        //    if (record == null || molecule == null)
        //    {
        //        molecule = this._molDocument.GetMol(_currentRecordIndex);
        //        string[] values = new string[1];
        //        values[0] = molecule.index;
        //        record = new CSVSourceRecord(this._currentRecordIndex, values);
        //    }
        //}

        #endregion

        #region > IFileReader Members <

        public override int CountAll()
        {
            _totalRecordCount = _dbReader.CountAll();
            return _totalRecordCount;
        }

        public override void ReadNext()
        {
            this.OnRecordParsing(new RecordParsingEventArgs(_currentRecordIndex));

            if (_currentRecordIndex < RecordCount)
            {
                ISourceRecord record = null;
                MolServer.Molecule molecule = null;
                OpenMolDocument();

                // is it safe to assume there will be an MS access row for every Molserver.Molecule?
                if (_dbReader != null)
                {
                    record = _dbReader.GetNext();
                    _current = record;
                    int molId = Convert.ToInt32(record.FieldSet[ACCESS_XREF_FIELD]);
                    molecule = this._molDocument.GetMol(molId);
                }

                if (record != null)
                {
                    string structure = string.Empty;

                    if (molecule != null)
                    {

                        object dataObj = molecule.DataObject.GetData(_chemDrawInterchangeFormat);
                        structure = Convert.ToBase64String((byte[])dataObj);
                        SourceFieldTypes.SetValue("Structure", structure, record);

                        // Everything else
                        SourceFieldTypes.SetValue("Index", molecule.index, record);
                        SourceFieldTypes.SetValue("Name", molecule.Name, record);
                        SourceFieldTypes.SetValue("MolFormula", molecule.Formula, record);
                        SourceFieldTypes.SetValue("MolWeight", molecule.MolWeight, record);
                        SourceFieldTypes.SetValue("CanonicalCode", molecule.CanonicalCode, record);
                    }
                    else
                    {
                        if (!_current.FieldSet.ContainsKey("Structure"))
                        {
                            SourceFieldTypes.SetValue("Structure", structure, record);
                        }
                        //for else, the "structure" field has already set by CSVFileParser class.

                        SourceFieldTypes.SetValue("Index", string.Empty, record);
                        SourceFieldTypes.SetValue("Name", string.Empty, record);
                        SourceFieldTypes.SetValue("MolFormula", string.Empty, record);
                        SourceFieldTypes.SetValue("MolWeight", string.Empty, record);
                        SourceFieldTypes.SetValue("CanonicalCode", string.Empty, record);
                        //these five SetValue operation may cause TypeDefinition field type mess, I'll check them later.
                    }

                    _parsedRecordCount++;
                    _currentRecordIndex++;

                    this.OnRecordParsed(new RecordParsedEventArgs(_current));
                }
            }
            else
            {
                _current = null;
            }
        }

        public override ISourceRecord GetNext()
        {
            ReadNext();
            return Current; ;
        }

        public override void Seek(int recordIndex)
        {
            if (recordIndex <= this.RecordCount)
            {
                _currentRecordIndex = recordIndex;
                if (_dbReader != null)
                    _dbReader.Seek(_currentRecordIndex);
            }
            else
            {
                _currentRecordIndex = this.RecordCount;
                if (_dbReader != null)
                    _dbReader.Seek(this.RecordCount);
            }
        }

        public override void Rewind()
        {
            this._currentRecordIndex = 0;
            this._parsedRecordCount = 0;
            this._current = null;

            if (_dbReader != null)
                _dbReader.Rewind();
        }

        public override void Close()
        {
            if (_dbReader != null)
                _dbReader.Close();
        }

        #endregion
    }
}
