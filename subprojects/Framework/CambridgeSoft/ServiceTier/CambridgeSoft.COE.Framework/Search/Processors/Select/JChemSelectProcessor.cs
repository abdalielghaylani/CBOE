﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JChemSelectProcessor.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Search.Processors.Select
{
    using CambridgeSoft.COE.Framework.COESearchService;
    using CambridgeSoft.COE.Framework.COESearchService.Processors;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;

    /// <summary>
    /// This class will be used to set the transient password in JChem server before each JChem query
    /// </summary>
    public class JChemSelectProcessor : SelectProcessor
    {
        public override void PreProcess(DAL searchDAL)
        {
            // Create set password command
            var pwd = Utilities.GetPwdFromOracleConnStr(searchDAL.DALManager.Database.ConnectionStringWithoutCredentials);
            var setpwd = new SetJChemPassword(pwd);

            searchDAL.ExecuteNonQuery(setpwd);
        }

        public override void Process(ResultsCriteria.IResultsCriteriaBase item)
        {
        }

        public override void PostProcess(DAL searchDAL)
        {
        }
    }
}