// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureFilterWorkerFactory.cs" company="PerkinElmer Inc.">
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
    using Spotfire.Dxp.Application.Extension;
    using Spotfire.Dxp.Framework.DocumentModel;
    using Spotfire.Dxp.Framework.Threading;

    /// <summary>The factory required to create the structure filter thread.
    /// </summary>
    internal class StructureFilterWorkerFactory : CustomWorkerFactory<StructureFilterWorker, StructureFilterModel>
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override StructureFilterWorker CreateCore()
        {
            return new StructureFilterWorker();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerModel"></param>
        /// <returns></returns>
        protected override Trigger GetWorkerModelTriggerCore(StructureFilterModel workerModel)
        {
            return Trigger.CreatePropertyTrigger(workerModel, StructureFilterModel.PropertyNames.FilterSettings);
        }

        protected override ThreadingModel GetThreadingModelCore()
        {
            return ThreadingModel.CreateDedicatedThreadingModel(20);
        }

        #endregion
    }
}