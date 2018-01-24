// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetJChemPassword.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries
{
    /// <summary>
    /// SQL statement "call jchem_core_pkg.use_password('password');".
    /// </summary>
    public class SetJChemPassword : NonQuery
    {
        // the password want to set into JChem service
        private readonly string password;

        public SetJChemPassword(string password)
        {
            this.password = password;
        }

        public override string GetDependantString(DBMSType databaseType)
        {
            return string.Format("call jchem_core_pkg.use_password('{0}')", this.password);
        }
    }
}
