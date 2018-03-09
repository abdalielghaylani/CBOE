// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterWorkItem.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter.FilterThread
{
    using Spotfire.Dxp.Framework.Threading;
    using Spotfire.Dxp.Data;
    //using CBVNSFCoreChemistryCLR;

    /// <summary>
    /// 
    /// </summary>
    public interface IStructureFilterWorkItem
    {
        /// <summary>The result after the filter has been applied. The key is the ID field, the value is true to display the row and false to hide it.
        /// </summary>
        bool[] Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //RGroupDecompositionData RGroupResult { get; set; }

        /// <summary>
        /// </summary>
        bool LockedUI { get; set; }

        /// <summary>The active filter settings used to apply the changes.
        /// </summary>
        StructureFilterSettings FilterSettings { get; set; }

        /// <summary>The active filter settings used to apply the changes.
        /// </summary>
        //ICoreChemistryCLR CoreChemistryClr { get; set; }

        /// <summary>The dataTableId at the time the item was created
        /// </summary>
        Guid DataTableId { get; }

        /// <summary>
        /// </summary>
        string DataColumnName { get; }
    }

    /// <summary>The worker item defined for the structure filter thread operation
    /// </summary>
    public class StructureFilterWorkItem : WorkItem, IStructureFilterWorkItem
    {
        #region Constants and Fields

        internal enum WorkerOperation
        {
            ClearString,
            ExecuteFilter,
            SetString,
            ExecuteRGD
        };

        private readonly Guid _dataTableId;
        private readonly string _dataColumnName;

        #endregion

        #region Constructors and Destructors

        /// <summary>The constructor for the structure filter work item class.
        /// </summary>
        /// <param name="workOp"></param>
        /// <param name="filterSettings"></param>
        /// <param name="ccClr"></param>
        /// <param name="handler">The handler for the work item</param>
        /// <param name="locked"></param>
        /// <param name="dataTableReference"></param>
        /// <param name="dataColumnReference"></param>
        internal StructureFilterWorkItem(WorkerOperation workOp, StructureFilterSettings filterSettings,
                                         WorkItemHandler<StructureFilterWorkItem> handler,
                                         bool locked, DataTable dataTableReference, DataColumn dataColumnReference)
        {
            Operation = workOp;
            FilterSettings = filterSettings;
            //CoreChemistryClr = ccClr;
            PushResultHandler(handler);
            LockedUI = locked;
            _dataTableId = dataTableReference.Id;
            _dataColumnName = dataColumnReference.Name;
        }

        #endregion

        #region Properties

        /// <summary>The operation to be performed
        /// </summary>
        internal WorkerOperation Operation { get; set; }

        /// <summary>The result after the filter has been applied. The key is the ID field, the value is true to display the row and false to hide it.
        /// </summary>
        public bool[] Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public RGroupDecompositionData RGroupResult { get; set; }

        /// <summary>
        /// </summary>
        public bool LockedUI { get; set; }

        /// <summary>The active filter settings used to apply the changes.
        /// </summary>
        public StructureFilterSettings FilterSettings { get; set; }

        ///// <summary>The active filter settings used to apply the changes.
        ///// </summary>
        //public ICoreChemistryCLR CoreChemistryClr { get; set; }

        /// <summary>The dataTableId at the time the item was created
        /// </summary>
        public Guid DataTableId
        {
            get
            {
                return _dataTableId;
            }
        }
        /// <summary>
        /// </summary>
        public string DataColumnName
        {
            get
            {
                return _dataColumnName;
            }
        }

        #endregion
    }
}