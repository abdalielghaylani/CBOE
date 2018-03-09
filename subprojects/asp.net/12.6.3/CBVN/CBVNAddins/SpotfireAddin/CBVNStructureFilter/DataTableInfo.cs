// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterPanel.cs" company="PerkinElmer Inc.">
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

using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CBVNStructureFilter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading;

    //using CBVNSFCoreChemistryCLR;
    using Spotfire.Dxp.Application;
    using Spotfire.Dxp.Data;

    /// <summary>
    /// 
    /// </summary>
    public class DataTableInfoMgr
    {
        /// <summary>
        /// Each data set has its own set of information
        /// </summary>
        private static readonly Dictionary<Guid, DataTableInfo> DataTableInfos = new Dictionary<Guid, DataTableInfo>();

        /// <summary>
        /// 
        /// </summary>
        public class DataTableInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public enum SearchStateEnum
            {
                /// <summary>
                /// 
                /// </summary>
                Busy,
                /// <summary>
                /// 
                /// </summary>
                NoStructureColumns,
                /// <summary>
                /// 
                /// </summary>
                Editting,
                /// <summary>
                /// 
                /// </summary>
                Enabled
            }

            //private CoreChemistryCLR _coreChemistryClr;
            private ProgressIndicatorInfo _progressInfo;

            internal DataTableInfo(bool validTable)
            {
                SearchState = validTable ? SearchStateEnum.Enabled : SearchStateEnum.NoStructureColumns;
                SavedSearches = new List<SavedSearchSettings>();
            }

            //internal CoreChemistryCLR CoreChemistry
            //{
            //    get
            //    {
            //        try
            //        {
            //            Monitor.Enter(this);
            //            return _coreChemistryClr ??
            //                   (_coreChemistryClr = new CoreChemistryCLR(ProgressInfo));
            //        }
            //        finally
            //        {
            //            Monitor.Exit(this);
            //        }
            //    }
            //    set { _coreChemistryClr = value; }
            //}

            internal StructureFilterSettings LastDispatched { get; set; }

            internal ProgressIndicatorInfo ProgressInfo
            {
                get { return _progressInfo ?? (_progressInfo = new ProgressIndicatorInfo()); }
                set { _progressInfo = value; }
            }

            internal string LastDispatchedColumnName { get; set; }

            internal SearchStateEnum SearchState { get; set; }

            internal List<SavedSearchSettings> SavedSearches { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool ContainsKey(Guid id)
        {
            try
            {
                Monitor.Enter(DataTableInfos);
                return DataTableInfos.ContainsKey(id);
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        //internal static CoreChemistryCLR GetCoreChemistry(StructureFilterModel filterModel)
        //{
        //    try
        //    {
        //        Monitor.Enter(DataTableInfos);

        //        return filterModel.DataTableReference == null
        //                   ? null
        //                   : GetCoreChemistry(filterModel.DataTableReference.Id);
        //    }
        //    finally
        //    {
        //        Monitor.Exit(DataTableInfos);
        //    }
        //}

        //internal static CoreChemistryCLR GetCoreChemistry(Guid id)
        //{
        //    try
        //    {
        //        Monitor.Enter(DataTableInfos);

        //        return DataTableInfos.ContainsKey(id) ? DataTableInfos[id].CoreChemistry : null;
        //    }
        //    finally
        //    {
        //        Monitor.Exit(DataTableInfos);
        //    }
        //}

        internal static StructureFilterSettings GetLastDispatched(StructureFilterModel model)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                if (model == null)
                {
                    return null;
                }
                return DataTableInfos.ContainsKey(model.DataTableReference.Id)
                           ? DataTableInfos[model.DataTableReference.Id].LastDispatched
                           : null;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static void ClearLastDispatched(StructureFilterModel model)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                if ((model == null) || (model.DataTableReference == null))
                {
                    return;
                }
                if (DataTableInfos.ContainsKey(model.DataTableReference.Id))
                {
                    DataTableInfos[model.DataTableReference.Id].LastDispatched = null;
                }
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static ProgressIndicatorInfo GetProgressInfo(StructureFilterModel model)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                if (model == null)
                {
                    return null;
                }
                return DataTableInfos.ContainsKey(model.DataTableReference.Id)
                           ? DataTableInfos[model.DataTableReference.Id].ProgressInfo
                           : null;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static ProgressIndicatorInfo GetProgressInfo(Guid id)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                return DataTableInfos.ContainsKey(id) ? DataTableInfos[id].ProgressInfo : null;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static void SetLastDispatched(StructureFilterModel model)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                if (model.DataTableReference == null || !DataTableInfos.ContainsKey(model.DataTableReference.Id))
                {
                    return;
                }

                DataTableInfos[model.DataTableReference.Id].LastDispatched = model.StructureFilterSettings;
                DataTableInfos[model.DataTableReference.Id].LastDispatchedColumnName = model.DataColumnReference == null
                                                                                           ? null
                                                                                           : model.DataColumnReference
                                                                                                  .Name;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static string GetLastDispatchedColumnName(StructureFilterModel model)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                if (model.DataTableReference == null)
                {
                    return null;
                }
                return DataTableInfos.ContainsKey(model.DataTableReference.Id)
                           ? DataTableInfos[model.DataTableReference.Id].LastDispatchedColumnName
                           : null;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="state"></param>
        public static void SetSearchState(Guid tableId, DataTableInfo.SearchStateEnum state)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                if (!DataTableInfos.ContainsKey(tableId))
                {
                    return;
                }
                DataTableInfos[tableId].SearchState = state;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static DataTableInfo.SearchStateEnum GetSearchState(Guid tableId)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                return !DataTableInfos.ContainsKey(tableId)
                           ? DataTableInfo.SearchStateEnum.NoStructureColumns
                           : DataTableInfos[tableId].SearchState;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static void CancelCurrentOperation(Guid tableId)
        {
            try
            {
                Monitor.Enter(DataTableInfos);
                if (!DataTableInfos.ContainsKey(tableId))
                {
                    return;
                }
                //DataTableInfos[tableId].CoreChemistry.CancelCurrentOperation = true;
                DataTableInfos[tableId].LastDispatched = null;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static bool IsCancelled(Guid tableId)
        {
            try
            {
                Monitor.Enter(DataTableInfos);
                if (!DataTableInfos.ContainsKey(tableId))
                {
                    return false;
                }
                return false;//DataTableInfos[tableId].CoreChemistry.CancelCurrentOperation;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="document"></param>
        /// <param name="validTable"></param>
        public static void Add(Guid tableId, Document document, bool validTable)
        {
            if (tableId == Guid.Empty)
            {
                return;
            }
            try
            {
                Monitor.Enter(DataTableInfos);
                DataTableInfos.Add(tableId, new DataTableInfo(validTable));
                //LoadCoreChemistryClr(document);
                LoadSavedSearches(document);
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static void Clear()
        {
            DataTableInfos.Clear();
        }

        private static bool SaveTableUsesEmbeddedData(DataSaveSettings saveSettings, Guid tableId)
        {
            foreach (var tableSettings in saveSettings.DataTableSettings)
            {
                if (tableSettings.TableId == tableId)
                {
                    if (tableSettings.UseLinkedData)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        private static void WriteTableIdToStream(Stream stream, Guid id)
        {
            stream.Write(id.ToByteArray(), 0, 16);
        }

        private static Guid ReadTableIdFromStream(Stream stream)
        {
            var guidBytes = new byte[16];
            stream.Read(guidBytes, 0, 16);

            return new Guid(guidBytes);
        }

        //private static void WriteCoreChemistryClrToStream(Stream stream, CoreChemistryCLR coreChemistryClr)
        //{
        //    coreChemistryClr.PersistData(stream);
        //}

        //private static CoreChemistryCLR ReadCoreChemistryClrFromStream(Stream stream, ProgressIndicatorInfo progressIndicatorInfo)
        //{
        //    return new CoreChemistryCLR(stream, progressIndicatorInfo);
        //}

        private static void WriteSavedSearchesToStream(Stream stream, List<SavedSearchSettings> searchSettings)
        {
            stream.Write(BitConverter.GetBytes(searchSettings.Count), 0, sizeof(int));
            var serializer = new BinaryFormatter();
            foreach (var search in searchSettings)
            {
                serializer.Serialize(stream, search);
            }
        }

        private static List<SavedSearchSettings> ReadSavedSearchesFromStream(Stream stream)
        {
            var len = new byte[sizeof(int)];
            stream.Read(len, 0, sizeof(int));
            var numSettings = BitConverter.ToInt32(len, 0);
            var searches = new List<SavedSearchSettings>();

            var deserializer = new BinaryFormatter();
            for (var i = 0; i < numSettings; i++)
            {
                searches.Add((SavedSearchSettings)deserializer.Deserialize(stream));
            }
            return searches;
        }

        //internal static void SaveCoreChemistryClr(Document document)
        //{
        //    var savedCcclr = new Collection<CoreChemistryNode.ImmutableByteArray>();
        //    foreach (var dataTableInfo in DataTableInfos)
        //    {
        //        if (SaveTableUsesEmbeddedData(document.Data.SaveSettings, dataTableInfo.Key))
        //        {
        //            if (dataTableInfo.Value.SearchState != DataTableInfo.SearchStateEnum.NoStructureColumns)
        //            {
        //                var stream = new MemoryStream();
        //                WriteTableIdToStream(stream, dataTableInfo.Key);
        //                WriteCoreChemistryClrToStream(stream, dataTableInfo.Value.CoreChemistry);
        //                savedCcclr.Add(new CoreChemistryNode.ImmutableByteArray(stream.ToArray()));
        //            }
        //        }
        //    }

        //    var coreChemistryNode = document.CustomNodes.AddNewIfNeeded<CoreChemistryNode>();
        //    coreChemistryNode.CoreChemistryCollection = savedCcclr;
        //}

        //internal static void LoadCoreChemistryClr(Document document)
        //{
        //    CoreChemistryNode coreChemistryNode;
        //    if (document.CustomNodes.TryGetNode(out coreChemistryNode))
        //    {
        //        foreach (var bytesValue in coreChemistryNode.CoreChemistryCollection)
        //        {
        //            var stream = new MemoryStream(bytesValue.Data);
        //            var tableId = ReadTableIdFromStream(stream);
        //            if (!DataTableInfos.ContainsKey(tableId))
        //            {
        //                var dataTableInfo = new DataTableInfo(true);
        //                DataTableInfos.Add(tableId, dataTableInfo);
        //            }

        //            DataTableInfos[tableId].CoreChemistry = ReadCoreChemistryClrFromStream(stream, DataTableInfos[tableId].ProgressInfo);
        //        }

        //        // The streams are no longer required since they will be recreated if the document is saved again.
        //        coreChemistryNode.ClearCollection();
        //    }
        //}

        internal static void AddSavedSearch(Guid tableId, SavedSearchSettings search)
        {
            bool lockAcquired = false;
            try
            {
                Monitor.Enter(DataTableInfos, ref lockAcquired);

                if (!DataTableInfos.ContainsKey(tableId))
                {
                    return;
                }
                if (DataTableInfos[tableId].SavedSearches.Any(item => search.CompareTo(item) == 0))
                {
                    return;
                }
                DataTableInfos[tableId].SavedSearches.Insert(0, search);

                if (DataTableInfos[tableId].SavedSearches.Count > FilterUtilities.PreviousSearchesMax)
                {
                    DataTableInfos[tableId].SavedSearches.RemoveAt(DataTableInfos[tableId].SavedSearches.Count - 1);
                }
            }
            finally
            {
                if (lockAcquired)
                    Monitor.Exit(DataTableInfos);
            }
        }

        internal static List<SavedSearchSettings> GetSavedSearches(Guid tableId)
        {
            try
            {
                Monitor.Enter(DataTableInfos);

                return !DataTableInfos.ContainsKey(tableId) ? null : DataTableInfos[tableId].SavedSearches;
            }
            finally
            {
                Monitor.Exit(DataTableInfos);
            }
        }

        internal static void SaveSavedSearches(Document document)
        {
            var savedSearches = new Collection<SavedSearchesNode.ImmutableByteArray>();
            foreach (var dataTableInfo in DataTableInfos)
            {
                if (SaveTableUsesEmbeddedData(document.Data.SaveSettings, dataTableInfo.Key))
                {
                    if (dataTableInfo.Value.SearchState != DataTableInfo.SearchStateEnum.NoStructureColumns)
                    {
                        var stream = new MemoryStream();
                        WriteTableIdToStream(stream, dataTableInfo.Key);
                        WriteSavedSearchesToStream(stream, dataTableInfo.Value.SavedSearches);
                        savedSearches.Add(new SavedSearchesNode.ImmutableByteArray(stream.ToArray()));
                    }
                }
            }

            var savedSearchesNode = document.CustomNodes.AddNewIfNeeded<SavedSearchesNode>();
            savedSearchesNode.SavedSearchesCollection = savedSearches;
        }

        internal static void LoadSavedSearches(Document document)
        {
            SavedSearchesNode savedSearchesNode;
            if (document.CustomNodes.TryGetNode(out savedSearchesNode))
            {
                foreach (var bytesValue in savedSearchesNode.SavedSearchesCollection)
                {
                    var stream = new MemoryStream(bytesValue.Data);
                    var tableId = ReadTableIdFromStream(stream);
                    if (!DataTableInfos.ContainsKey(tableId))
                    {
                        var dataTableInfo = new DataTableInfo(true);
                        DataTableInfos.Add(tableId, dataTableInfo);
                    }

                    DataTableInfos[tableId].SavedSearches = ReadSavedSearchesFromStream(stream);
                }

                // The streams are no longer required since they will be recreated if the document is saved again.
                savedSearchesNode.SavedSearchesCollection.Clear();
            }
        }
    }
}
