// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhereClauseDirectFormula.cs" company="PerkinElmer Inc.">
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
    /// This class to generate a "fmlamatch(field)" or "fmlalike(field)" SQL Statement for Direct Cartridge.
    /// </summary>
    internal class WhereClauseDirectFormula : WhereClauseBinaryOperation
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClauseDirectFormula"/> class to its default values. 
        /// </summary>
        public WhereClauseDirectFormula()
        {
            Full = false;
            dataField = new Field();
            val = new Value();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether it is a equal search.
        /// </summary>
        public bool Full { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string representation of the Where clause formula for the given database and adds
        /// its param values to the list of param values of the query that comes in "values".
        /// </summary>
        /// <param name="databaseType">The database the get the string for.</param>
        /// <param name="values">The existing param values of a query.</param>
        /// <returns>A string of the form: (CartridgeSchema.FormulaContains(:1, :2, :3)=1).</returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values)
        {
            if (databaseType != DBMSType.ORACLE)
            {
                throw new Exception("This clause only works in Oracle implementations");
            }

            // fmlamatch(fld, 'c')=1
            // fmlalike(fld, 'c')=1
            var builder = new StringBuilder();
            builder.Append(Full ? "fmlamatch(" : "fmlalike(");
            builder.Append(ParameterHelper.GetDirectStructureFieldString(dataField, GetFullName(dataField)));

            builder.Append(", ");
            ParameterHelper.AppendParameter(this, ref builder, ref values, val);

            builder.Append(")=1");

            return builder.ToString();
        }

        #endregion
    }
}
