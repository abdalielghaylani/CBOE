// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhereClauseDirectMolWeight.cs" company="PerkinElmer Inc.">
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
    using System.Diagnostics;
    using System.Text;

    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

    /// <summary>
    /// This class to generate a "molwt(field)" SQL Statement for Direct Cartridge.
    /// </summary>
    internal class WhereClauseDirectMolWeight : WhereClauseBinaryOperation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClauseDirectMolWeight"/> class to its default values. 
        /// </summary>
        public WhereClauseDirectMolWeight()
        {
            dataField = new Field();
            val = new Value("0.00", DbType.Decimal);
            Val2 = new Value("0.00", DbType.Decimal);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the compare method string.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the second parameter
        /// </summary>
        public Value Val2 { get; set; }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets the string representation of the molweight clause for the underlying database, and adds
        /// its param values to the already existing list of values of a query.
        /// </summary>
        /// <param name="databaseType">The database.</param>
        /// <param name="values">The already existing param values of a query.</param>
        /// <returns>The string representation.</returns>
        protected override string GetDependantString(DBMSType databaseType, List<Value> values)
        {
            if (databaseType != DBMSType.ORACLE)
            {
                throw new Exception("This clause only works in Oracle implementations");
            }

            var builder = new StringBuilder();
            var strOp = string.Empty;

            if (COEDataView.MimeTypes.CHEMICAL_X_SMILES == dataField.MimeType)
            {
                strOp = string.Format("mdlaux.molwt(NULL, 'SMILES:'||{0})", GetFullName(dataField));
            }
            else
            {
                strOp = string.Format("mdlaux.molwt(NULL, {0})", GetFullName(dataField));
            }
                
            // Legacy bad code: EQUAL also means BETWEEN
            if (Operator == "EQUAL")
            {
                // (molwt(mol(fld)) >= 100 AND molwt(mol(fld)) <= 200)
                builder.Append("(");
                builder.Append(strOp);
                builder.Append(">=");
                ParameterHelper.AppendParameter(this, ref builder, ref values, val);
                builder.Append(" AND ");
                builder.Append(strOp);
                builder.Append("<=");
                ParameterHelper.AppendParameter(this, ref builder, ref values, Val2);
                builder.Append(")");
            }
            else
            {
                builder.Append(strOp);
                switch (Operator)
                {
                    // EQUAL has been implemented by "BETWEEN", 
                    //case "EQUAL":
                    //    builder.Append("=");
                    //    break;
                    case "GT":
                        builder.Append(">");
                        break;
                    case "LT":
                        builder.Append("<");
                        break;
                    case "GTE":
                        builder.Append(">=");
                        break;
                    case "LTE":
                        builder.Append("<=");
                        break;
                    default:
                        Debug.Assert(false, "Invalid compare method for Mol Weight query!");
                        break;
                }

                ParameterHelper.AppendParameter(this, ref builder, ref values, val);
            }

            return builder.ToString();
        }

        #endregion
    }
}
