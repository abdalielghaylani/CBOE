// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhereClauseJCCompare.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

    /// <summary>
    /// This class to generate a JChem structure comparing "jc_compare(field, value, 't:f')" SQL Statement for JChem Cartridge.
    /// </summary>
    internal class WhereClauseJCCompare : WhereClauseBinaryOperation
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClauseJCCompare"/> class to its default values. 
        /// </summary>
        public WhereClauseJCCompare()
        {
            dataField = new Field();
            val = new Value();
            // Default is full search.
            SearchType = "f";
            // By default to do full search, and simThreshold is not required.
            SimThreshold = -1;
        }

        #endregion

        /// <summary>
        /// Gets or sets the similarity percentage. It is taken into account when SIMILAR=YES is set as well, 
        /// otherwise disregarded. Default value (used if it is omitted) is 90 percent.
        /// </summary>
        public int SimThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Structure search type. the valid values are t:f, t:s, t:t.
        /// </summary> 
        public string SearchType
        {
            get;
            set;
        }

        #region Methods

        /// <summary>
        /// Gets the string representation of the Where clause full structure searching for the given database and adds
        /// its param values to the list of param values of the query that comes in "values".
        /// </summary>
        /// <param name="databaseType">The database the get the string for.</param>
        /// <param name="values">The existing param values of a query.</param>
        /// <returns>A string of the form: (jc_compare(field, value, 't:f')=1 .</returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values)
        {
            if (databaseType != DBMSType.ORACLE)
            {
                throw new Exception("This clause only works in Oracle implementations");
            }

            var builder = new StringBuilder();
            // Need to convert the varchar to clob as there is no binding for jc_compare(varchar, clob).
            builder.Append(string.Format("jc_compare(to_clob({0})", GetFullName(dataField)));
            builder.Append(",");
            ParameterHelper.AppendStructure(this, ref builder, ref values, val);
            builder.Append(", ");
            builder.AppendFormat("'t:{0}", SearchType);
            if (SimThreshold >= 0)
            {
                builder.AppendFormat(" simThreshold:{0}", (float)SimThreshold / 100);
            }
            // TODO: add additional parameters for advance search.
            builder.Append("')=1");

            return builder.ToString();
        }

        #endregion
    }
}
