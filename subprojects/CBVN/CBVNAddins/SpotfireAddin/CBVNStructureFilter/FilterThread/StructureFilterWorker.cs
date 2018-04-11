// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterWorker.cs" company="PerkinElmer Inc.">
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

namespace CBVNStructureFilter.FilterThread
{
    using System;
    using System.Threading;
    using log4net;
    using Spotfire.Dxp.Application.Extension;

    /// <summary>The class that does the work for the structure filter thread.
    /// </summary>
    internal class StructureFilterWorker : CustomWorker<StructureFilterModel, StructureFilterWorkItem>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StructureFilterWorker));

        #region Methods

        /// <summary>Function the does the thread processing
        /// </summary>
        /// <param name="model">The model defined for the thread</param>
        /// <param name="workItem">the worker item defined for this set of operations</param>
        protected override void DoWorkCore(StructureFilterModel model, StructureFilterWorkItem workItem)
        {
            //if (workItem.CoreChemistryClr == null)
            //{
            //    return;
            //}
            try
            {
                switch (workItem.Operation)
                {
                    case StructureFilterWorkItem.WorkerOperation.ClearString:
                        DoClearString(workItem);
                        return;
                    case StructureFilterWorkItem.WorkerOperation.ExecuteFilter:
                        DoExecuteFilter(workItem, model, true);
                        return;
                    case StructureFilterWorkItem.WorkerOperation.SetString:
                        DoSetString(workItem, model);
                        return;
                    case StructureFilterWorkItem.WorkerOperation.ExecuteRGD:
                        DoExecuteRGD(workItem, model);
                        break;
                }
            }
            catch (Exception e)
            {
                //workItem.RGroupResult = null;
                workItem.Result = null;
                Log.Error(e);
            }
            workItem.Result = null;
        }

        internal void DoClearString(IStructureFilterWorkItem workItem)
        {
            //workItem.CoreChemistryClr.ClearLoaded();
        }

        internal void DoExecuteFilter(IStructureFilterWorkItem workItem, StructureFilterModel model, bool initializeProgress)
        {
            if (!model.ValidDataTable())
            {
                return;
            }

            var stringsAreSet =
                //!workItem.CoreChemistryClr.DataChanged(model.DataTableReference, model.DataColumnReference) ||
                DoSetString(workItem, model);

            if (!stringsAreSet)
            {
                return;
            }

            if (initializeProgress)
            {
                //InitializeProgressInfo(workItem.CoreChemistryClr.ProgressIndicatorInfo, workItem.CoreChemistryClr.SearchProgressString, 100);
            }
            //string errMsg;
            //workItem.Result = workItem.CoreChemistryClr.PerformStructureFilter(workItem.FilterSettings, out errMsg);
            //if (!string.IsNullOrEmpty(errMsg))
            //{
            //    Log.Error(errMsg);
            //}
        }

		internal void DoExecuteRGD(IStructureFilterWorkItem workItem, StructureFilterModel model)
        {
            if (!model.ValidDataTable())
            {
                return;
            }

            //InitializeProgressInfo(workItem.CoreChemistryClr.ProgressIndicatorInfo, workItem.CoreChemistryClr.RGDProgressString, model.DataColumnReference.RowValues.Count);
            if (FilteringRequired(workItem, model))
            {
                DoExecuteFilter(workItem, model, false);
            }

            var structureType = StructureStringTypeHelper.MimeTypeToStructureStringType(model.DataColumnReference.Properties.ContentType);

            // note only MOL and CDX are currently supported, convert all others to MOL
		    structureType = structureType == StructureStringType.CDX ? StructureStringType.CDX : StructureStringType.Molfile;
            //workItem.RGroupResult = workItem.CoreChemistryClr.RGroupDecomposeData(workItem.FilterSettings, model.DataColumnReference, structureType);
        }

        private static bool FilteringRequired(IStructureFilterWorkItem workItem, StructureFilterModel model)
        {
            return false;// !StructureMatchColumnExists(model) || workItem.CoreChemistryClr.DataChanged(model.DataTableReference, model.DataColumnReference);
        }

        private static bool StructureMatchColumnExists(StructureFilterModel model)
        {
            return model.HasStructureMatchColumn();
        }

        private bool DoSetString(IStructureFilterWorkItem workItem, StructureFilterModel model)
        {
            if (model.DataTableReference == null || model.DataColumnReference == null)
            {
                return false;
            }
            //if (!workItem.CoreChemistryClr.DataChanged(model.DataTableReference, model.DataColumnReference))
            //{
            //    return true;
            //}    

            // ensure no previous data remains
            DoClearString(workItem);

            //InitializeProgressInfo(workItem.CoreChemistryClr.ProgressIndicatorInfo, string.Format(workItem.CoreChemistryClr.IndexProgressString, 0, model.DataColumnReference.RowValues.Count), model.DataColumnReference.RowValues.Count);

            string errorMsg = string.Empty;
            var result = false;// workItem.CoreChemistryClr.AddMolStorageStrings(model.DataColumnReference.Properties.ContentType, model.DataTableReference, model.DataColumnReference, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                Log.Error(errorMsg);
            }
            return result;
        }

        static void InitializeProgressInfo(ProgressIndicatorInfo currentProgressInfo, string message, int maxCount)
        {
            if (currentProgressInfo == null)
            {
                return;
            }

            Monitor.Enter(currentProgressInfo);
            currentProgressInfo.ProgressInformation = message;
            currentProgressInfo.ProgressMaxVal = maxCount;
            currentProgressInfo.ProgressCurVal = 0;
            Monitor.Exit(currentProgressInfo);
        }

        #endregion
    }
}
