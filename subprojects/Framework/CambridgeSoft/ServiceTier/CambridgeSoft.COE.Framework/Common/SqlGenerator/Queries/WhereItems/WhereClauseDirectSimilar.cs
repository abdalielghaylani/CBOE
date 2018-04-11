// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhereClauseDirectSimilar.cs" company="PerkinElmer Inc.">
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
    using System.Data;
    using System.Text;

    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

    /// <summary>
    /// This class to generate a "similar(field)" SQL Statement for Direct Cartridge.
    /// </summary>
    internal class WhereClauseDirectSimilar : WhereClauseBinaryOperation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClauseDirectSimilar"/> class to its default values. 
        /// </summary>
        public WhereClauseDirectSimilar()
        {
            Similarityvalues = string.Empty;
            SimilarNumber = string.Empty;
            dataField = new Field();
            val = new Value();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a VARCHAR2 containing the similarity threshold values (as percentages) 
        /// and an optional 'SUB' or 'SUPER' flag
        /// </summary>
        public string Similarityvalues { get; set; }

        /// <summary>
        /// Gets or sets a number that is equal to the similar-number parameter used with the similarity 
        /// operator. This parameter only applies if you use the similarity operator.
        /// </summary>
        public string SimilarNumber { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the string representation of the Where clause formula for the given database and adds
        /// its param values to the list of param values of the query that comes in "values".
        /// </summary>
        /// <param name="databaseType">The database to get the string for.</param>
        /// <param name="values">The existing param values of a query.</param>
        /// <returns>A string of the form: (CartridgeSchema.FormulaContains(:1, :2, :3)=1).</returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values)
        {
            if (databaseType != DBMSType.ORACLE)
            {
                throw new Exception("This clause only works in Oracle implementations");
            }

            // similar(mol(fld),
            //   to_clob('\r\n  ChemDraw02131415062D\r\n\r\n  6  6  0  0  0  0  0  ...'),
            //   '65 SUB')=1
            var builder = new StringBuilder();
            builder.Append(string.Format("similar({0}", ParameterHelper.GetDirectStructureFieldString(dataField, GetFullName(dataField))));

            builder.Append(", ");
            ParameterHelper.AppendStructure(this, ref builder, ref values, val);

            builder.Append(", ");
            ParameterHelper.AppendParameter(this, ref builder, ref values, new Value(Similarityvalues, DbType.String));

            if (!string.IsNullOrEmpty(SimilarNumber))
            {
                builder.Append(", ");
                ParameterHelper.AppendParameter(this, ref builder, ref values, new Value(SimilarNumber, DbType.String));
            }

            builder.Append(")=1");

            return builder.ToString();
        }

        #endregion
    }
}
