using System;
using System.Collections.Generic;

using CambridgeSoft.COE.Framework.Services;
using CambridgeSoft.COE.DataLoader.Core.Contracts;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Carries instructional messages and process metadata most commonly used by workflow services.
    /// </summary>
    [Serializable()]
    public class JobParameters
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public JobParameters() { }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileReaderConstructorInfo">for FileReader creation</param>
        /// <param name="dataMappingInfo">for data-translation</param>
        /// <param name="actionableRecordIndices">
        /// the subset of source record indices to perform an action against
        /// </param>
        public JobParameters(
            SourceFileInfo fileReaderConstructorInfo
            , Mappings dataMappingInfo
            , IndexRanges actionableRecordIndices
            )
        {
            this._dataSourceInformation = fileReaderConstructorInfo;
            this._mappings = dataMappingInfo;
            this._actionRanges = actionableRecordIndices;
        }

        /// <summary>
        /// Provides a mechanism for populating constructor-data about the source file.
        /// </summary>
        /// <param name="pathToInputFile">the full path to the file</param>
        /// <param name="typeOfInputFile">the classification of the data-source</param>
        // Not used by reference detection. Commented out for now.
        //public void InitializeDataSourceInfo(string pathToInputFile, SourceFileType typeOfInputFile)
        //{
        //    SourceFileInfo sourceInfo = new SourceFileInfo(pathToInputFile, typeOfInputFile);
        //    _dataSourceInformation = sourceInfo;
        //}

        private SourceFileInfo _dataSourceInformation;
        /// <summary>
        /// Used to keep information needed by source record export function, such as file extention.
        /// </summary>
        public SourceFileInfo DataSourceInformation
        {
            get { return _dataSourceInformation; }
            set { _dataSourceInformation = value; }
        }

        private IFileReader _fileReader = null;
        /// <summary>
        /// The source file reader
        /// </summary>
        public IFileReader FileReader
        {
            get { return _fileReader; }
            set { _fileReader = value; }
        }

        private DataMapping.Mappings _mappings = new Mappings();
        /// <summary>
        /// Used by the MappingService, provides the metadata required to perform data-translative processes.
        /// </summary>
        public DataMapping.Mappings Mappings
        {
            get { return _mappings; }
            set { _mappings = value; }
        }

        private IndexRanges _actionRanges = new IndexRanges();
        /// <summary>
        /// Used by the FileReader, provides a list of record-indices (as index-ranges) against which an action
        /// will be taken, such as 'export-to-file' or 'persist-to-repository'.
        /// </summary>
        public IndexRanges ActionRanges
        {
            get { return _actionRanges; }
            set { _actionRanges = value; }
        }

        private List<ISourceRecord> _sourceRecords = null;
        /// <summary>
        /// The list of ISourceRecord to be processed.
        /// </summary>
        public List<ISourceRecord> SourceRecords
        {
            get { return _sourceRecords; }
            set { _sourceRecords = value; }
        }

        private List<IDestinationRecord> _destinationRecords = null;
        /// <summary>
        /// The list of IDestinationRecord to be processed.
        /// </summary>
        public List<IDestinationRecord> DestinationRecords
        {
            get { return _destinationRecords; }
            set { _destinationRecords = value; }
        }

        private TargetActionType _targetActionType = TargetActionType.Unknown;
        /// <summary>
        /// The type of action to perform
        /// </summary>
        public TargetActionType TargetActionType
        {
            get { return _targetActionType; }
            set { _targetActionType = value; }
        }

        private string _userName = string.Empty;
        /// <summary>
        /// UserName for the current action
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password = string.Empty;
       /// <summary>
       /// Password for the current action.
       /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
