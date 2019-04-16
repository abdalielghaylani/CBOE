// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JChemSearchProcessor.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Search.Processors
{
    using System.Xml;

    using CambridgeSoft.COE.Framework.COESearchService.Processors;
    using CambridgeSoft.COE.Framework.Common;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;

    using DAL = CambridgeSoft.COE.Framework.COESearchService.DAL;

    /// <summary>
    /// This class will be used to set the transient password in JChem server before each JChem query
    /// </summary>
    public class JChemSearchProcessor : SearchProcessor
    {
        public JChemSearchProcessor(SearchCriteria.SearchCriteriaItem item)
            : base(item)
        {
        }

        public JChemSearchProcessor(XmlNode xmlNode)
            : base(xmlNode)
        {
        }

        public override void PreProcess(DAL searchDAL, COEDataView dataview)
        {
            // Create set password command
            //var pwd = Utilities.GetPwdFromOracleConnStr(searchDAL.DALManager.Database.ConnectionStringWithoutCredentials);

            var encryptedPassword = searchDAL.DALManager.DatabaseData.Password;
            if (Utilities.IsEncrypted(searchDAL.DALManager.DatabaseData.FipsEabled, encryptedPassword)) 
            {
                encryptedPassword = Utilities.Decrypt(searchDAL.DALManager.DatabaseData.FipsEabled, encryptedPassword);
            }

            var setpwd = new SetJChemPassword(encryptedPassword);

            searchDAL.ExecuteNonQuery(setpwd);
        }

        public override void Process(Query query)
        {
        }

        public override void PostProcess(DAL searchDAL)
        {
        }
    }
}
