using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils {

    /// <summary>
    /// Provides checking against SQL Injection
    /// </summary>
    class SqlInjection {
        #region Methods
        /// <summary>
        /// Find SQL Injection in sql code.
        /// </summary>
        /// <param name="sqlCode">A bit of SQL Code that will execute against the database.</param>
        /// <returns>True if an sql Injection attempting is found.</returns>
        public static bool FindSqlInjection(string sqlCode) {
            bool isFound = false;
            Regex regExp = new Regex(@"DROP|ALTER|INSERT|DELETE|UPDATE|GRANT|CREATE|TRUNCATE|[^A-Za-z0-9=()?,.* /'""<>%_!-+]");
            
            if(regExp.IsMatch(sqlCode.ToUpper()))
                isFound = true;
            else
                isFound = false;
            
            return isFound;
        }
        #endregion
    }
}
