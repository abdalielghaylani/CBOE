// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetProcessHost.cs" company="PerkinElmer Inc.">
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

namespace StructureSearchSupport
{
    /// <summary>
    /// A <see cref="ProcessHost"/> for our own .Net built processes.
    /// </summary>
    /// <remarks>A process that is hosted via this class should probably be built using the <see cref="CommandProcessor"/>.</remarks>
    ////[FriendVisible(true)]
    internal sealed class NetProcessHost : ProcessHost
    {
        /// <summary>
        /// The external executable.
        /// </summary>
        private readonly string _externalExecutable;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetProcessHost"/> class.
        /// </summary>
        /// <param name="externalExecutable">The external executable.</param>
        public NetProcessHost(string externalExecutable)
        {
            _externalExecutable = externalExecutable;
        }

        /// <summary>
        /// Gets ExecutableName.
        /// </summary>
        protected override string ExecutableName
        {
            get
            {
                return _externalExecutable;
            }
        }

        /// <summary>
        /// Gets Arguments.
        /// </summary>
        protected override string Arguments
        {
            get
            {
                return string.Empty;
            }
        }
    }
}