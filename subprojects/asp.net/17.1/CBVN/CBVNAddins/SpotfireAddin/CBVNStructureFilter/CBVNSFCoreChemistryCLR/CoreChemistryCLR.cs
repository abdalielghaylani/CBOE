// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreChemistryCLR.cs" company="PerkinElmer Inc.">
// Copyright © 2013 PerkinElmer Inc. 
// 940 Winter Street, Waltham, MA 02451. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using CambridgeSoft.CoreChemistry.CoreChemistryCLR;
using Spotfire.Dxp.Data;
using Spotfire.Dxp.Framework.Persistence;
using CBVNStructureFilter.Properties;
//using CBVNSFCoreChemistryCLR.Properties;

namespace CBVNStructureFilter
{
    /// <summary>A class containing the results of an R-Group decomposition
    /// </summary>
    public class RGroupDecompositionData
    {
        /// <summary>The constructor for the R-Group data class
        /// </summary>
        /// <param name="numGroups">The number of groups found</param>
        /// <param name="templateStructure">The string representation of the template if returned in Mol format</param>
        /// <param name="cdxStructure">The cdx structure for the template if returned in cdx format</param>
        /// <param name="groupLabels">A list of labels identifying the groups that were found.</param>
        public RGroupDecompositionData(int numGroups, string templateStructure, byte[] cdxStructure, List<string> groupLabels)
        {
            GroupLabels = groupLabels;
            CDXStructure = cdxStructure;
            TemplateStructure = templateStructure;
            NumberOfGroups = numGroups;
            Cancelled = false;
            Error = false;
            ErrorMessage = string.Empty;
            StructureType = StructureStringType.Unknown;
        }

        /// <summary>The constructor for the R-Group data class
        /// </summary>
        /// <param name="numGroups">The number of groups found</param>
        /// <param name="templateStructure">The string representation of the template if returned in Mol format</param>
        /// <param name="cdxStructure">The cdx structure for the template if returned in cdx format</param>
        /// <param name="groupLabels">A list of labels identifying the groups that were found.</param>
        /// <param name="cancelled">True if the operation was cancelled.</param>
        public RGroupDecompositionData(int numGroups, string templateStructure, byte[] cdxStructure, List<string> groupLabels, bool cancelled)
        {
            GroupLabels = groupLabels;
            CDXStructure = cdxStructure;
            TemplateStructure = templateStructure;
            NumberOfGroups = numGroups;
            Cancelled = cancelled;
            Error = false;
            ErrorMessage = string.Empty;
            StructureType = StructureStringType.Unknown;
        }

        /// <summary>The constructor for the R-Group data class
        /// </summary>
        /// <param name="numGroups">The number of groups found</param>
        /// <param name="templateStructure">The string representation of the template if returned in Mol format</param>
        /// <param name="cdxStructure">The cdx structure for the template if returned in cdx format</param>
        /// <param name="groupLabels">A list of labels identifying the groups that were found.</param>
        /// <param name="error">True if an error occured..</param>
        /// <param name="errorMessage">The error if applicable.</param>
        public RGroupDecompositionData(int numGroups, string templateStructure, byte[] cdxStructure, List<string> groupLabels, bool error, string errorMessage)
        {
            GroupLabels = groupLabels;
            CDXStructure = cdxStructure;
            TemplateStructure = templateStructure;
            NumberOfGroups = numGroups;
            Cancelled = false;
            Error = error;
            ErrorMessage = errorMessage;
            StructureType = StructureStringType.Unknown;
        }

        /// <summary>
        /// The number of groups found
        /// </summary>
        public int NumberOfGroups { get; internal set; }
        /// <summary>
        /// The strutcure type of the past in and returned structures
        /// </summary>
        public StructureStringType StructureType { get; internal set; }
        /// <summary>
        /// The string representation of the template if returned in Mol format
        /// </summary>
        public string TemplateStructure { get; internal set; }
        /// <summary>
        /// The cdx structure for the template if returned in cdx format
        /// </summary>
        public byte[] CDXStructure { get; internal set; }
        /// <summary>
        /// A list of labels identifying the groups that were found.
        /// </summary>
        public List<string> GroupLabels { get; internal set; }
        /// <summary>
        /// True if the operation was cancelled
        /// </summary>
        public bool Cancelled { get; internal set; }
        /// <summary>
        /// True if an error was encountered. Error Message is set to the error that occured
        /// </summary>
        public bool Error { get; internal set; }
        /// <summary>
        /// The error that occured.
        /// </summary>
        public string ErrorMessage { get; internal set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICoreChemistryCLR
    {
        /// <summary>
        /// 
        /// </summary>
        bool ReloadStarted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool CancelCurrentOperation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool HasCDXRGroupData { get; }

        /// <summary>
        /// 
        /// </summary>
        ProgressIndicatorInfo ProgressIndicatorInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        string IndexProgressString { get; }

        /// <summary>
        /// 
        /// </summary>
        string SearchProgressString { get; }

        /// <summary>
        /// 
        /// </summary>
        string RGDProgressString { get; }

        /// <summary>
        /// 
        /// </summary>
        bool Redo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        void PersistData(Stream stream);

        /// <summary>
        /// Return the object to an uninitialized state
        /// </summary>
        void ClearLoaded();

        /// <summary>
        /// 
        /// </summary>
        void ClearFilterSettings();

        /// <summary>This function will add all the strucures to the chemical structure library.
        /// </summary>
        /// <param name="mimeType">The type of the structure data stored in the structure column</param>
        /// <param name="dataTable">The active data table used to do the filtering</param>
        /// <param name="dataColumn">The column within the data table that has the structures</param>
        /// <param name="errorMessage">Set to the message from any exception that may have occured when False is returned, empty otherwise</param>
        /// <returns>True on success, False on Failure</returns>
        bool AddMolStorageStrings(string mimeType, DataTable dataTable, DataColumn dataColumn, out string errorMessage);

        /// <summary>This function will go through all structures that have been added to the library and determine if they should be displayed or not based on the current settings.
        /// </summary>
        /// <param name="filterSettings">The filter settings defined by the user</param>
        /// <param name="errorMessage">Set to the message from any exception that may have occured when null is returned, empty otherwise</param>
        /// <returns>An array of boolean values that indicate which rows should be displayed</returns>
        bool[] PerformStructureFilter(StructureFilterSettings filterSettings, out string errorMessage);

        /// <summary>Checks the filter settings to determine in they are valid.
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <returns>true if a filter has been setup, false otherwise.</returns>
        bool ValidSearch(StructureFilterSettings filterSettings);

        /// <summary>Checks to see that we have data to filter
        /// </summary>
        /// <returns>true if the count is > 0, false otherwise.</returns>
        bool HaveData();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>number of rows indexed</returns>
        int NumberOfRowsIndexed();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dataColumn"></param>
        /// <returns></returns>
        bool DataChanged(DataTable dataTable, DataColumn dataColumn);

        /// <summary>Determines if the settings in the passed in structure filter settings are different than the settings in the last used
        /// structure filter settings class or if the data table has changed.
        /// The first time this is called true will always be returned as there is no last settings class.
        /// </summary>
        /// <param name="currentSettings">The current structure filter settings class</param>
        /// <param name="dataTable"></param>
        /// <param name="dataColumn"></param>
        /// <returns>true if any setting is different in the current class and false if all settings are the same.</returns>
        bool SettingsChanged(StructureFilterSettings currentSettings, DataTable dataTable, DataColumn dataColumn);

        /// <summary>Returns information for the R-Group decomposition performed
        /// </summary>
        /// <param name="filterSettings">The filter settings</param>
        /// <param name="idColumn"></param>
        /// <param name="structureStringType"></param>
        /// <returns>True on success, false on failure</returns>
        RGroupDecompositionData RGroupDecomposeData(StructureFilterSettings filterSettings, DataColumn idColumn, StructureStringType structureStringType);

        /// <summary>Returns information for the R-Group decomposition performed
        /// </summary>
        /// <param name="queryMol">The structure the query is based on</param>
        /// <param name="idColumn"></param>
        /// <param name="structureType"></param>
        /// <returns>A class containing the decompose data.</returns>
        RGroupDecompositionData RGroupDecomposeData(byte[] queryMol, DataColumn idColumn, StructureStringType structureType);

        /// <summary>Get the structure to display for the given ID value and groupIndex
        /// </summary>
        /// <param name="idValue">The ID for the row to get the data from.</param>
        /// <param name="groupIndex">The index of the group required.</param>
        /// <param name="name">The name of the substituent</param>
        /// <returns>A byte array representing the structure to display</returns>
        byte[] GetRowStructureForRgroup(object idValue, int groupIndex, out string name);

        /// <summary>
        /// Get a list of R-Group column names which were returned by the analysis
        /// </summary>
        /// <returns>A string array containing the names of the R-Groups which were returned by the analysis</returns>
        string[] GetRgroupNames();
    }

    /// <summary>
    /// </summary>
    public class CoreChemistryCLR : ICoreChemistryCLR
    {
        #region Constants and Fields

        [Serializable]
        [PersistenceVersion(1, 0)]
        private class PersistedData : ISerializable
        {
            private const string MstPropertyName = "Mst";
            private InMemoryMst _mst;
            private RGroupAnalyzer _rga;
            private const string MolIndexPropertyName = "molIndex";
            private readonly Dictionary<int, int> _molIndex = new Dictionary<int, int>();
            private readonly Dictionary<string, int> _rgdIndex = new Dictionary<string, int>();
            private const string LastFilterSettingsPropertyName = "LastFilterSettings";
            private StructureFilterSettings _lastFilterSettings;
            private int _numGroups;

            public InMemoryMst Mst
            {
                get { return _mst; }
                set { _mst = value; }
            }
            public RGroupAnalyzer Rga
            {
                get { return _rga; }
                set { _rga = value; }
            }
            public Dictionary<int, int> MolIndex
            {
                get { return _molIndex; }
            }
            public Dictionary<string, int> RGDIndex
            {
                get { return _rgdIndex; }
            }
            public StructureFilterSettings LastFilterSettings
            {
                get { return _lastFilterSettings; }
                set { _lastFilterSettings = value; }
            }
            public int NumGroups
            {
                get { return _numGroups; }
                set { _numGroups = value; }
            }
            private const string LoadedDataTableIdPropertyName = "LoadedDataTableId";
            /// <summary>
            /// Last loaded DataTable, identified by DataTable.Id
            /// </summary>
            public Guid LoadedDataTableId { get; set; }
            private const string LoadedDataColumnNamePropertyName = "LoadedDataColumnName";
            /// <summary>
            /// Last loaded DataColumn, identified by DataColumn.Name
            /// </summary>
            public string LoadedDataColumnName { get; set; }

            internal PersistedData()
            {
                _numGroups = 0;
                _mst = CreateInMemoryMst();
            }

            internal PersistedData(SerializationInfo info, StreamingContext context)
            {
                // R-group info is not serialized
                _mst = (InMemoryMst)info.GetValue(MstPropertyName, typeof(InMemoryMst));
                _molIndex = (Dictionary<int, int>)info.GetValue(MolIndexPropertyName, typeof(Dictionary<int, int>));
                _lastFilterSettings = (StructureFilterSettings)info.GetValue(LastFilterSettingsPropertyName, typeof(StructureFilterSettings));
                // set RGA off so it will be re-executed
                _lastFilterSettings = new StructureFilterSettings(_lastFilterSettings.ChemicalDocumentField,
                                                                  _lastFilterSettings.FilterMode,
                                                                  _lastFilterSettings.IncludeHits,
                                                                  _lastFilterSettings.SimularityMode,
                                                                  _lastFilterSettings.SimularityPercent, false,
                                                                  _lastFilterSettings.FilterStructure,
                                                                  _lastFilterSettings.RGroupNicknames);
                LoadedDataTableId = (Guid)info.GetValue(LoadedDataTableIdPropertyName, typeof(Guid));
                LoadedDataColumnName = (string)info.GetValue(LoadedDataColumnNamePropertyName, typeof(string));
            }

            /// <summary>Implements ISerializable.</summary>
            [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                // R-group info is not serialized
                info.AddValue(MstPropertyName, _mst);
                info.AddValue(MolIndexPropertyName, _molIndex);
                info.AddValue(LastFilterSettingsPropertyName, _lastFilterSettings);
                info.AddValue(LoadedDataTableIdPropertyName, LoadedDataTableId);
                info.AddValue(LoadedDataColumnNamePropertyName, LoadedDataColumnName);
            }
        }

        private PersistedData _persistedData;
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger("LeadDiscovery");
        private readonly ProgressIndicatorInfo _progressIndicatorInfo;

        #endregion

        #region Constructors and Destructors
        /// <summary>CoreChemistryCLR constructor
        /// </summary>
        public CoreChemistryCLR(ProgressIndicatorInfo progressIndicatorInfoIn)
        {
            _progressIndicatorInfo = progressIndicatorInfoIn;
            _persistedData = new PersistedData();

            CancelCurrentOperation = false;
        }

        /// <summary>
        /// Constructor used when persisted data saved to stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="progressIndicatorInfoIn"></param>
        public CoreChemistryCLR(Stream stream, ProgressIndicatorInfo progressIndicatorInfoIn)
        {
            _progressIndicatorInfo = progressIndicatorInfoIn;

            IFormatter formatter = new BinaryFormatter();
            _persistedData = (PersistedData)formatter.Deserialize(stream);

            CancelCurrentOperation = false;
        }

        #endregion

        #region Field get/set definitions
        
        /// <summary>
        /// 
        /// </summary>
        public bool ReloadStarted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CancelCurrentOperation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasCDXRGroupData
        {
            get { return _persistedData.Rga != null && _persistedData.Rga.ReturnedStructureType == "chemical/x-cdx"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProgressIndicatorInfo ProgressIndicatorInfo
        {
            get { return _progressIndicatorInfo; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string IndexProgressString
        {
            get { return Resources.CoreChemistryCLR_ProgressIndex_Message; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SearchProgressString
        {
            get { return Resources.CoreChemistryCLR_ProgressInfo_Searching; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RGDProgressString
        {
            get { return Resources.CoreChemistryCLR_ProgressInfo_RGD; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Redo { get; set; }

        /// <summary>
        /// Used only when calling ApplyFilter when loading a document. We do not want the Row Count event to erase saved data.
        /// </summary>
        public bool IgnoreRowChange { get; set; }

        #endregion

        #region PublicMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void PersistData(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _persistedData);
        }

        /// <summary>This function is used to get preferred MimeType of structure data.
        /// </summary>
        /// <param name="structure">the structure data need to be identified</param>
        /// <returns>MimeType</returns>
        public static string GetPreferredStructureMimeType(string structure)
        {
            var mimeType = GetNonPreferredStructureMimeType(structure);
            return SOfficialTypes.ContainsKey(mimeType) ? SOfficialTypes[mimeType] : string.Empty;

        }
        /// <summary>This function is used to get non-preffered MimeType of structure data.
        /// </summary>
        /// <param name="structure">the structure data need to be identified</param>
        /// <returns>MimeType</returns>
        public static string GetNonPreferredStructureMimeType(string structure)
        {
           // string str;
          //  List<string> props = Utils.QueryPropertyList();
            var ascStr = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(structure));
            return Utils.GetStructureMimeType(ascStr);
        }

        /// <summary>This function is used to get property list from the specified lib.
        /// </summary>
        /// <param name="libName">LibName</param>
        /// <returns>property list</returns>
        public static List<string> GetChemistryPropertyList(string libName)
        {
            return null;// Utils.QueryPropertyList(libName);
        }

        /// <summary>This function is used to get property list from the specified lib.
        /// </summary>
        /// <param name="structureData"></param>
        /// <param name="propName"></param>
        /// <param name="libName"></param>
        /// <returns>property list</returns>
        public static string ComputeChemistryProperty(
                                    string structureData,
                                    string propName,
                                    string libName)
        {

            return string.Empty; // Utils.ComputeChemProperty(structureData, propName, libName);
        }

        /// <summary>
        /// Overlay structure, used for Scaffold Alignment
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="targetStructure"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string Overlay(string structure, string targetStructure, int timeout = 0)
        {
            try
            {
                //var ascStr = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(structure));
                //var ascTargetStr = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(targetStructure));
                //return Utils.Overlay(ascStr, ascTargetStr, timeout);
                return string.Empty;
            }
            catch (Exception)
            {
                return structure;
            }
        }

        /// <summary>
        /// Get the largest common sub-structure from two structures, used for Scaffold Alignment
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="targetStructure"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string MostCommSubStructure(string structure, string targetStructure, int timeout = 0)
        {
            try
            {
                //var ascStr = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(structure));
                //var ascTargetStr = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(targetStructure));
                //return Utils.MostCommSubStructure(ascStr, ascTargetStr, timeout);
                return string.Empty;
            }
            catch (Exception)
            {
                return targetStructure;
            }
        }

        /// <summary>
        /// Return the object to an uninitialized state
        /// </summary>
        public void ClearLoaded()
        {
            try
            {
                Monitor.Enter(this);

                ClearIndexies();
                ResetInternalVariables();
                ClearFilterSettings();
            }
            catch (Exception e)
            {
                _log.Error("ClearLoaded threw an exception:", e);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearFilterSettings()
        {
            _persistedData.LastFilterSettings = null;
        }

        /// <summary>This function will add all the strucures to the chemical structure library.
        /// </summary>
        /// <param name="mimeType">The type of the structure data stored in the structure column</param>
        /// <param name="dataTable">The active data table used to do the filtering</param>
        /// <param name="dataColumn">The column within the data table that has the structures</param>
        /// <param name="errorMessage">Set to the message from any exception that may have occured when False is returned, empty otherwise</param>
        /// <returns>True on success, False on Failure</returns>
        public bool AddMolStorageStrings(string mimeType, DataTable dataTable, DataColumn dataColumn, out string errorMessage)
        {
            try
            {
                Monitor.Enter(this);
                errorMessage = string.Empty;

                if (!ValidTableInfo(dataTable, dataColumn))
                {
                    return false;
                }

                SetProgress(string.Format(Resources.CoreChemistryCLR_ProgressIndex_Message, 0, dataTable.RowCount), dataTable.RowCount, 0);

                if (!DoAddMolStorageStrings(mimeType, dataTable, dataColumn))
                {
                    return false;
                }

                InitializeInternalData(dataTable.Id, dataColumn.Name);

                _log.DebugFormat(
                    "AddMolStorageStrings for data table {0} with {1} rows: number rows in molIndex now: {2}",
                    dataTable.Name, dataTable.RowCount, _persistedData.MolIndex.Count);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                // rollback any changes
                ClearLoaded();
                return false;
            }
            finally
            {
                Monitor.Exit(this);
            }
            return true;
        }

        /// <summary>This function will go through all structures that have been added to the library and determine if they should be displayed or not based on the current settings.
        /// </summary>
        /// <param name="filterSettings">The filter settings defined by the user</param>
        /// <param name="errorMessage">Set to the message from any exception that may have occured when null is returned, empty otherwise</param>
        /// <returns>An array of boolean values that indicate which rows should be displayed</returns>
        public bool[] PerformStructureFilter(StructureFilterSettings filterSettings, out string errorMessage)
        {
            try
            {
                _persistedData.LastFilterSettings = new StructureFilterSettings(filterSettings);
                errorMessage = string.Empty;
                if (HaveData() && ValidSearch(filterSettings))
                {
                    return DoPerformFilter(filterSettings);
                }
                return SetAllVisible();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>Checks the filter settings to determine in they are valid.
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <returns>true if a filter has been setup, false otherwise.</returns>
        public bool ValidSearch(StructureFilterSettings filterSettings)
        {
            return filterSettings != null && filterSettings.FilterStructure != null && filterSettings.FilterStructure.Length > 0;
        }

        /// <summary>Checks to see that we have data to filter
        /// </summary>
        /// <returns>true if the count is > 0, false otherwise.</returns>
        public bool HaveData()
        {
            return (_persistedData.MolIndex.Count > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>number of rows indexed</returns>
        public int NumberOfRowsIndexed()
        {
            return _persistedData.MolIndex.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dataColumn"></param>
        /// <returns></returns>
        public bool DataChanged(DataTable dataTable, DataColumn dataColumn)
        {
            try
            {
                return (TablesOrColumnsAreNull(dataTable, dataColumn) || TableOrColumnIsDifferent(dataTable, dataColumn));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Determines if the settings in the passed in structure filter settings are different than the settings in the last used
        /// structure filter settings class or if the data table has changed.
        /// The first time this is called true will always be returned as there is no last settings class.
        /// </summary>
        /// <param name="currentSettings">The current structure filter settings class</param>
        /// <param name="dataTable"></param>
        /// <param name="dataColumn"></param>
        /// <returns>true if any setting is different in the current class and false if all settings are the same.</returns>
        public bool SettingsChanged(StructureFilterSettings currentSettings, DataTable dataTable, DataColumn dataColumn)
        {
            if (DataChanged(dataTable, dataColumn))
            {
                return true;
            }
            return _persistedData.LastFilterSettings == null || _persistedData.LastFilterSettings.IsChanged(currentSettings);
        }

        /// <summary>Returns information for the R-Group decomposition performed
        /// </summary>
        /// <param name="filterSettings">The filter settings</param>
        /// <param name="idColumn"></param>
        /// <param name="structureStringType"></param>
        /// <returns>True on success, false on failure</returns>
        public RGroupDecompositionData RGroupDecomposeData(StructureFilterSettings filterSettings, DataColumn idColumn, StructureStringType structureStringType)
        {
            try
            {
                Monitor.Enter(this);
                _persistedData.RGDIndex.Clear();
            }
            catch (Exception e)
            {
                _log.Error("RGroupDecomposeData threw an error:", e);
                return new RGroupDecompositionData(0, "", null, null, true, Resources.CoreChemistryCLR_RGroupDecomposeError);
            }
            finally
            {
                Monitor.Exit(this);
            }

            try
            {
                _persistedData.LastFilterSettings = new StructureFilterSettings(filterSettings);
                return RGroupDecomposeData(GetBytes(filterSettings.FilterStructure), idColumn, structureStringType);
            }
            catch (Exception e)
            {
                _log.Error("RGroupDecomposeData threw an error:", e);
                return new RGroupDecompositionData(0, "", null, null, true, Resources.CoreChemistryCLR_RGroupDecomposeError);
            }
        }

        /// <summary>Returns information for the R-Group decomposition performed
        /// </summary>
        /// <param name="queryMol">The structure the query is based on</param>
        /// <param name="idColumn"></param>
        /// <param name="structureType"></param>
        /// <returns>A class containing the decompose data.</returns>
        public RGroupDecompositionData RGroupDecomposeData(byte[] queryMol, DataColumn idColumn, StructureStringType structureType)
        {
            var returnData = new RGroupDecompositionData(0, "", null, null);
            _persistedData.NumGroups = 0;

            returnData.StructureType = structureType;
            if (!DoAnalyze(queryMol, structureType, returnData))
            {
                return returnData;
            }
            SetReturnData(idColumn, returnData);
            return returnData;
        }

        /// <summary>Get the structure to display for the given ID value and groupIndex
        /// </summary>
        /// <param name="idValue">The ID for the row to get the data from.</param>
        /// <param name="groupIndex">The index of the group required.</param>
        /// <param name="name">The name of the substituent</param>
        /// <returns>A byte array representing the structure to display</returns>
        public byte[] GetRowStructureForRgroup(object idValue, int groupIndex, out string name)
        {
            name = string.Empty;
            try
            {
                if (groupIndex >= _persistedData.NumGroups)
                {
                    return null;
                }
                var idData = GetColumnString(idValue);
                if (_persistedData.RGDIndex.ContainsKey(idData))
                {
                    return _persistedData.Rga.GetSubstituent(_persistedData.RGDIndex[idData], groupIndex, out name);
                }
            }
            catch (Exception e)
            {
                _log.Error("GetRowStructureForRgroup threw an exception:", e);
            }
            return null;
        }

        /// <summary>
        /// Get a list of R-Group column names which were returned by the analysis
        /// </summary>
        /// <returns>A string array containing the names of the R-Groups which were returned by the analysis</returns>
        public string[] GetRgroupNames()
        {
            if (_persistedData.Rga == null || _persistedData.Rga.RGroups == null)
            {
                return null;
            }
            return _persistedData.Rga.RGroups.ToArray();
        }

        #endregion

        #region PrivateMethods

        private bool DoAddMolStorageStrings(string mimeType, DataTable dataTable, DataColumn dataColumn)
        {
            // Note that using cursors seems to be faster than iterating over RowValues
            var cursor = GetDataValueCursor(dataColumn);
            var progressPos = 0;

            var timeInfo = new TimeStats("Generate Index");

            if (dataTable.GetRows(cursor).Any(row => !AddCurrentRowToIndex(mimeType, cursor, row, dataTable.RowCount, timeInfo, ref progressPos)))
            {
                timeInfo.LogTotals(progressPos, true);
                return false;
            }

            timeInfo.LogTotals(dataTable.RowCount, true);
            return true;
        }

        private void InitializeInternalData(Guid dataTableId, string dataColumnName)
        {
            _persistedData.LastFilterSettings = null;
            _persistedData.LoadedDataTableId = dataTableId;
            _persistedData.LoadedDataColumnName = dataColumnName;
        }

        private bool AddCurrentRowToIndex(string mimeType, DataValueCursor[] cursor, DataRow row, int max, TimeStats timeInfo, ref int progressPos)
        {
            if (CancelCurrentOperation)
            {
                ClearLoaded();
                return false;
            }

            var val = PutMol(mimeType, cursor);
            // if the row has invalid values, consider it blank
            if (val >= 0)
            {
                _persistedData.MolIndex.Add(val, progressPos);
                _log.DebugFormat("Added row {0}", row.Index);
            }
            else
            {
                _log.DebugFormat("Skipped row {0}", row.Index);
            }
            progressPos++;
            SetProgress(string.Format(Resources.CoreChemistryCLR_ProgressIndex_Message, progressPos, max), max, progressPos);

            timeInfo.CalculateMinimumMaximum(progressPos.ToString(CultureInfo.InvariantCulture));
            return true;
        }

        private int PutMol(string mimeType, DataValueCursor[] cursor)
        {
            return string.IsNullOrEmpty(mimeType)
                       ? _persistedData.Mst.PutMol(GetColumnStringBytes(cursor[0].CurrentDataValue.ValidValue))
                       : _persistedData.Mst.PutMol(GetColumnStringBytes(cursor[0].CurrentDataValue.ValidValue), mimeType);
        }

        private static DataValueCursor[] GetDataValueCursor(DataColumn dataColumn)
        {
            var cursor = new DataValueCursor[1];
            if (dataColumn.RowValues.DataType == DataType.Binary)
            {
                cursor[0] = DataValueCursor.Create<BinaryLargeObject>(dataColumn);
            }
            else
            {
                cursor[0] = DataValueCursor.CreateFormatted(dataColumn);
            }
            return cursor;
        }

        private static bool ValidTableInfo(DataTable dataTable, DataColumn dataColumn)
        {
            if (dataTable == null || dataColumn == null)
            {
                return false;
            }
            return true;
        }

        private bool TablesOrColumnsAreNull(DataTable dataTable, DataColumn dataColumn)
        {
            return (_persistedData.LoadedDataTableId == Guid.Empty || string.IsNullOrEmpty(_persistedData.LoadedDataColumnName) || dataTable == null || dataColumn == null);
        }

        private bool TableOrColumnIsDifferent(DataTable dataTable, DataColumn dataColumn)
        {
            return (_persistedData.LoadedDataTableId != dataTable.Id || _persistedData.LoadedDataColumnName != dataColumn.Name);
        }

        private void ClearIndexies()
        {
            _persistedData.MolIndex.Clear();
            _persistedData.RGDIndex.Clear();
        }

        private void ResetInternalVariables()
        {
            _persistedData.LoadedDataTableId = Guid.Empty;
            _persistedData.LoadedDataColumnName = null;

            _persistedData.Mst = CreateInMemoryMst();

            _persistedData.Rga = null;
        }

        private static InMemoryMst CreateInMemoryMst()
        {
            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            return new InMemoryMst(KeyTypes.kMstFile_Key_Normal | KeyTypes.kMstFile_Key_Similar);
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
        }


        private bool DoAnalyze(byte[] queryMol, StructureStringType structureType, RGroupDecompositionData returnData)
        {
            try
            {
                // Do RGroup Analysis
                _persistedData.Rga = new RGroupAnalyzer(queryMol, _persistedData.Mst);
                _persistedData.Rga.ProgressChanged += mst_ProgressChanged;
                _persistedData.Rga.ReturnedStructureType = MapStructureStringTypeToCCCLRSupportedType(structureType);

                _persistedData.NumGroups = _persistedData.Rga.Analyze();
                if (_persistedData.NumGroups == 0)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.Error("DoAnalyze threw an error:", e);

                returnData.Error = true;
                returnData.ErrorMessage = Resources.CoreChemistryCLR_RGroupDecomposeError;
                return false;
            }
            finally
            {
                if (_persistedData.Rga != null)
                {
                    _persistedData.Rga.ProgressChanged -= mst_ProgressChanged;
                    ResetProgressIndicator();
                }
            }

            if (_persistedData.Rga == null)
            {
                returnData.Cancelled = true;
                return false;
            }
            return true;
        }

        private void ResetProgressIndicator()
        {
            _progressIndicatorInfo.ProgressCurVal = 0;
        }

        private void SetReturnData(DataColumn idColumn, RGroupDecompositionData returnData)
        {
            try
            {
                byte[] templateBytes = _persistedData.Rga.GetTemplate();

                returnData.NumberOfGroups = _persistedData.NumGroups;
                if (templateBytes != null)
                {
                    if (_persistedData.Rga.ReturnedStructureType == "chemical/x-cdx")
                    {
                        returnData.CDXStructure = templateBytes;
                    }
                    else
                    {
                        returnData.TemplateStructure = Encoding.ASCII.GetString(templateBytes);
                    }
                    returnData.GroupLabels = _persistedData.Rga.RGroups;

                    InitializeRGDIndex(idColumn);
                    returnData.Cancelled = CancelCurrentOperation;
                }
            }
            catch (Exception e)
            {
                _log.Error("SetReturnData threw an error:", e);

                returnData.Error = true;
                returnData.ErrorMessage = Resources.CoreChemistryCLR_RGroupDecomposeError;
            }
        }

        private string MapStructureStringTypeToCCCLRSupportedType(StructureStringType structureType)
        {
            // note these are taken from the current supported types in the CCCLR assembly (Utils.h, class MimeTypes)
            if (structureType == StructureStringType.CDX)
            {
                return "chemical/x-cdx";
            }
            return "chemical/x-mdl-molfile";
        }

        private void SetProgress(ProgressTypes type, int max, int curVal)
        {
            Monitor.Enter(_progressIndicatorInfo);
            _progressIndicatorInfo.ProgressType = type;
            _progressIndicatorInfo.ProgressMaxVal = max;
            _progressIndicatorInfo.ProgressCurVal = curVal;
            Monitor.Exit(_progressIndicatorInfo);
        }

        private void SetProgress(string text, int max, int curVal)
        {
            Monitor.Enter(_progressIndicatorInfo);
            _progressIndicatorInfo.ProgressInformation = text;
            _progressIndicatorInfo.ProgressMaxVal = max;
            _progressIndicatorInfo.ProgressCurVal = curVal;
            Monitor.Exit(_progressIndicatorInfo);
        }

        private void SetProgress(int max, int curVal)
        {
            Monitor.Enter(_progressIndicatorInfo);
            _progressIndicatorInfo.ProgressMaxVal = max;
            _progressIndicatorInfo.ProgressCurVal = curVal;
            Monitor.Exit(_progressIndicatorInfo);
        }
        private void SetProgress(int curVal)
        {
            Monitor.Enter(_progressIndicatorInfo);
            _progressIndicatorInfo.ProgressCurVal = curVal;
            Monitor.Exit(_progressIndicatorInfo);
        }

        /// <summary>Converts a string into a byte array.
        /// </summary>
        /// <param name="inputString">The string to convert</param>
        /// <returns>The converted byte array</returns>
        internal byte[] GetBytes(string inputString)
        {
            try
            {
                var bytes = new byte[inputString.Length * sizeof(char)];
                Buffer.BlockCopy(inputString.ToCharArray(), 0, bytes, 0, bytes.Length);
                return Encoding.Convert(Encoding.Unicode, Encoding.ASCII, bytes);
            }
            catch (Exception e)
            {
                _log.Error("GetBytes threw an error:", e);
                return null;
            }
        }

        /// <summary>Creates and fills the SearchOptions class required by the library from the internal StructureFilterSettings class
        /// </summary>
        /// <param name="filterSettings">The internal structure filter settings class.</param>
        /// <returns>The SearchOptions class required by the library.</returns>
        private SearchOptions getSearchOptions(StructureFilterSettings filterSettings)
        {
            var searchOptions = new SearchOptions
                {
                    FullStructure = filterSettings.FilterMode == StructureFilterSettings.FilterModeEnum.FullStructure,
                    Similarity = filterSettings.FilterMode == StructureFilterSettings.FilterModeEnum.Simularity,
                    SimilarityThreshold = filterSettings.SimularityPercent
                };

            return searchOptions;
        }

        private bool[] DoPerformFilter(StructureFilterSettings filterSettings)
        {
            Monitor.Enter(this);
            // All values in the displayRowIndicator array will default to false when created.
            var displayRowIndicator = new bool[_persistedData.MolIndex.Count];

            try
            {
                _persistedData.Mst.ProgressChanged += mst_ProgressChanged;
                var displayIndexies = _persistedData.Mst.Search(GetBytes(filterSettings.FilterStructure), getSearchOptions(filterSettings));

                if ((displayIndexies == null) || (CancelCurrentOperation))
                {
                    return displayRowIndicator;
                }

                SetProgress(Resources.CoreChemistryCLR_ProgressInfo_Searching, 100, 0);

                SetRowIndicator(displayIndexies, displayRowIndicator);

                _log.DebugFormat("doPerformFilter completed for data table: {0}", _persistedData.LoadedDataTableId);
                return displayRowIndicator;
            }
            catch (Exception e)
            {
                _log.Error("DoPerformFilter threw an error:", e);
                return displayRowIndicator;
            }
            finally
            {
                _persistedData.Mst.ProgressChanged -= mst_ProgressChanged;
                ResetProgressIndicator();
                Monitor.Exit(this);
            }
        }

        private void SetRowIndicator(List<int> displayIndexies, bool[] displayRowIndicator)
        {
            var progressPos = 0;
            foreach (var displayIndex in displayIndexies)
            {
                // Set the value to true for any that should be shown.
                displayRowIndicator[_persistedData.MolIndex[displayIndex]] = true;

                progressPos++;
                SetProgress(100, progressPos * 100 / displayIndexies.Count);

                if (CancelCurrentOperation)
                {
                    return;
                }
            }
        }

        private bool[] SetAllVisible()
        {
            try
            {
                Monitor.Enter(this);
                var displayRowIndicator = new bool[_persistedData.MolIndex.Count];
                var progressPos = 0;

                SetProgress(0);
                for (var currentIndex = 0; currentIndex < _persistedData.MolIndex.Count; currentIndex++)
                {
                    displayRowIndicator[_persistedData.MolIndex[currentIndex]] = true;
                    progressPos++;
                    SetProgress(progressPos * 100 / _persistedData.MolIndex.Count);
                }
                return displayRowIndicator;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        private byte[] GetColumnStringBytes(object value)
        {
            return GetBytes(GetColumnString(value));
        }

        private string GetColumnString(object value)
        {
            var o = value as BinaryLargeObject;
            if (o == null)
            {
                return value.ToString();
            }
            var binaryLargeObject = o;
            using (StreamReader sr = new StreamReader(binaryLargeObject.GetByteStream()))
            {
                string returnValue = sr.ReadToEnd();
                return returnValue;
            }
        }

        private void InitializeRGDIndex(IDataColumn idColumn)
        {
            if (CancelCurrentOperation)
            {
                _persistedData.RGDIndex.Clear();
                return;
            }

            var progressPos = 0;

            SetProgress(Resources.CoreChemistryCLR_ProgressInfo_RGD, 100, 0);

            // Setup the mapping between the ID and the core chemistry hit index
            foreach (var hitIndex in _persistedData.Rga.Hits)
            {
                if (CancelCurrentOperation)
                {
                    _persistedData.RGDIndex.Clear();
                    return;
                }
                if (!AddToIndex(idColumn, hitIndex))
                {
                    return;
                }

                progressPos++;
                SetProgress(progressPos * 100 / _persistedData.Rga.Hits.Count);
            }
        }

        private bool AddToIndex(IDataColumn idColumn, int hitIndex)
        {
            try
            {
                _log.DebugFormat("InitializeRGDIndex: Adding HitIndex: {0}, Row: {1}", hitIndex, hitIndex);
                _persistedData.RGDIndex.Add(GetColumnString(idColumn.RowValues.GetValue(_persistedData.MolIndex[hitIndex]).ValidValue), hitIndex);

                return true;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("InitializeRGDIndex: An exception occured at HitIndex: {0}, Row: {1}", hitIndex, _persistedData.MolIndex[hitIndex]);
                _log.Error(ex);
            }
            return false;
        }

        void mst_ProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            _log.DebugFormat("mst_ProgressChanged: Progress changed called: {0}% complete of operation {1}", args.ProgressPercent, args.ProgressType);

            if (CancelCurrentOperation)
            {
                args.Cancel = true;
                return;
            }

            if ((_progressIndicatorInfo.ProgressCurVal != args.ProgressPercent) || (_progressIndicatorInfo.ProgressType != args.ProgressType))
            {
                SetProgress(args.ProgressType, 100, args.ProgressPercent);
            }
        }
        #endregion

        #region Private Mime definition
        //CDX

        private const string MimeTypeCdx = "chemical/x-cdx"; //preferred
        private const string MimeTypeCdx1 = "chemical/cdx";
        private const string MimeTypeXml = "text/xml";
        private const string MimeTypeCdxml = "chemical/x-cdxml";
        private const string MimeTypeCdxml1 = "chemical/cdxml";
        private const string MimeTypeChemdraw = "chemical/x-chemdraw";
        //Molfile
        private const string MimeTypeMdlmolfile = "chemical/x-mdl-molfile"; //preferred
        private const string MimeTypeMdlmolfile1 = "chemical/mdl-molfile";
        private const string MimeTypeMsimolfile = "chemical/x-msi-molfile";
        private const string MimeTypeMsimolfile1 = "chemical/msi-molfile";
        //Molfile v3000 reserved
        private const string MimeTypeMolfilev3000 = "chemical/x-mdl-molfile-v3000";
        private const string MimeTypeMolfilev3000A = "chemical/mdl-molfile-v3000";
        //SMILES
        private const string MimeTypeSmiles = "chemical/x-daylight-smiles"; //preferred
        private const string MimeTypeSmiles1 = "chemical/daylight-smiles";
        private const string MimeTypeSmiles2 = "chemical/x-smiles";
        private const string MimeTypeSmiles3 = "chemical/smiles";

        private static readonly Dictionary<string, string> SOfficialTypes = new Dictionary<string, string>
                                                                             {
                                                                                 //CDX
                                                                                 {MimeTypeCdx,MimeTypeCdx},
                                                                                 {MimeTypeCdx1,MimeTypeCdx},
                                                                                 {MimeTypeXml,MimeTypeCdx},
                                                                                 {MimeTypeCdxml,MimeTypeCdx},
                                                                                 {MimeTypeCdxml1,MimeTypeCdx},
                                                                                 {MimeTypeChemdraw,MimeTypeCdx},
                                                                                 //Molfile
                                                                                 {MimeTypeMdlmolfile,MimeTypeMdlmolfile},
                                                                                 {MimeTypeMdlmolfile1,MimeTypeMdlmolfile},
                                                                                 {MimeTypeMsimolfile,MimeTypeMdlmolfile},
                                                                                 {MimeTypeMsimolfile1,MimeTypeMdlmolfile},
                                                                                 {MimeTypeMolfilev3000,MimeTypeMdlmolfile},
                                                                                 {MimeTypeMolfilev3000A,MimeTypeMdlmolfile},
                                                                                 //SMILES
                                                                                 {MimeTypeSmiles,MimeTypeSmiles},
                                                                                 {MimeTypeSmiles1,MimeTypeSmiles},
                                                                                 {MimeTypeSmiles2,MimeTypeSmiles},
                                                                                 {MimeTypeSmiles3,MimeTypeSmiles}
                                                                              };
        #endregion
    }
}
