// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressIndicatorInfo.cs" company="PerkinElmer Inc.">
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
using Spotfire.Dxp.Framework.Persistence;
//using CambridgeSoft.CoreChemistry.CoreChemistryCLR;

namespace CBVNStructureFilter
{
    /// <summary>
    /// Progress mode
    /// </summary>
    [PersistenceVersion(9, 0)]
    public enum ProgressModeEnum
    {
        /// <summary>
        /// </summary>
        Filtering,
        /// <summary>
        /// </summary>
        Screening,
        /// <summary>
        /// </summary>
        ABAS,
        /// <summary>
        /// </summary>
        Analyze
    }

    /// <summary>
    /// </summary>
    [Serializable]
    [PersistenceVersion(9, 0)]
    public class ProgressIndicatorInfo
    {
        private string _progressInformation;
        private int _progressMaxVal;
        private int _progressCurVal;
        //private ProgressTypes _progressType;
        private bool _progressDisplayed;

        /// <summary>
        /// </summary>
        public string ProgressInformation
        {
            get { return _progressInformation; }
            set { _progressInformation = value; }
        }
        /// <summary>
        /// </summary>
        public int ProgressMaxVal
        {
            get { return _progressMaxVal; }
            set { _progressMaxVal = value; }
        }
        /// <summary>
        /// </summary>
        public int ProgressCurVal
        {
            get { return _progressCurVal; }
            set { _progressCurVal = value; }
        }
        ///// <summary>
        ///// </summary>
        //public ProgressTypes ProgressType
        //{
        //    get { return _progressType; }
        //    set { _progressType = value; }
        //}
        /// <summary>
        /// </summary>
        public bool ProgressDisplayed
        {
            get { return _progressDisplayed; }
            set { _progressDisplayed = value; }
        }
    }
}
