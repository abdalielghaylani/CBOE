// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExecutor.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace COEConfigurationPublishTool
{
    using System.Collections.Generic;

    /// <summary>
    /// The work flow executor
    /// </summary>
    internal abstract class WorkflowExecutor
    {
        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        internal string[] Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// initialize the workflow
        /// </summary>
        /// <param name="args">The parameter</param>
        internal virtual void Initialize(string[] args)
        {
            this.Parameters = args;
        }

        /// <summary>
        /// Run the workflow
        /// </summary>
        internal abstract void Run();
    }
}
