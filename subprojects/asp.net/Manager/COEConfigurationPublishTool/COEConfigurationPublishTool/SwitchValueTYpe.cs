// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchValueType.cs" company="PerkinElmer Inc.">
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
    /// <summary>
    /// The switch type enumeration types.
    /// </summary>
    public enum SwitchValueType
    {
        /// <summary>
        /// Text string.
        /// </summary>
        Text,

        /// <summary>
        /// Integer type.
        /// </summary>
        Int,

        /// <summary>
        /// Boolean type.
        /// </summary>
        Bool,

        /// <summary>
        /// Options type.
        /// </summary>
        Options,
    }
}