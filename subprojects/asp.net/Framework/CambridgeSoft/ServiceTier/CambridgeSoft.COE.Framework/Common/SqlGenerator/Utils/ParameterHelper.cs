// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterHelper.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Text;

    using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;

    /// <summary>
    /// The helper class for appending SQL parameter.
    /// </summary>
    public class ParameterHelper
    {
        /// <summary>
        /// Append a parameter into SQL string.
        /// </summary>
        /// <param name="clause">
        /// The where clause which use the parameter.
        /// </param>
        /// <param name="builder">
        /// The string builder for the SQL string.
        /// </param>
        /// <param name="values">
        /// The parameter values.
        /// </param>
        /// <param name="value">
        /// The new parameter value.
        /// </param>
        public static void AppendParameter(
            WhereClauseItem clause,
            ref StringBuilder builder,
            ref List<Value> values,
            Value value)
        {
            if (clause == null)
            {
                throw new ArgumentException(@"Clause can't be null!", "clause");
            }

            builder.Append(clause.ParameterHolder);
            if (clause.UseParametersByName)
            {
                builder.Append(values.Count);
            }

            values.Add(value);
        }

        /// <summary>
        /// Append a structure parameter. 
        /// Oracle will throw exception "ORA-01704: string literal too long" if the given
        /// structure length exceeding 4000 characters. This function wll help to generat
        /// the parameter into several sections. These sections are joined by CONCAT().
        /// </summary>
        /// <param name="clause">
        /// The where clause which use the parameter.
        /// </param>
        /// <param name="builder">
        /// The string builder for the SQL string.
        /// </param>
        /// <param name="values">
        /// The parameter values.
        /// </param>
        /// <param name="structure">
        /// The structure parameter.
        /// </param>
        public static void AppendStructure(
            WhereClauseItem clause,
            ref StringBuilder builder,
            ref List<Value> values,
            Value structure)
        {
            if (clause == null)
            {
                throw new ArgumentException(@"Clause can't be null!", "clause");
            }

            if (structure == null)
            {
                throw new ArgumentException(@"Structure can't be null!", "structure");
            }

            const int MaxSizeOfVarchar2 = 4000;
            var strs = Utilities.SplitStringByLength(structure.Val, MaxSizeOfVarchar2);

            if (strs.Count > 1)
            {
                // CONCAT(to_clob(:n),CONCAT(to_clob(:n+1), to_clob(:n+2)))
                var idx = 0;
                var rightBracket = ")";
                for (; idx < strs.Count - 1; idx++)
                {
                    builder.Append("CONCAT(to_clob(");
                    AppendParameter(clause, ref builder, ref values, new Value(strs[idx], DbType.String));
                    builder.Append("),");
                    rightBracket += ")";
                }

                builder.Append("to_clob(");
                AppendParameter(clause, ref builder, ref values, new Value(strs[idx], DbType.String));
                builder.Append(rightBracket);
            }
            else
            {
                // to_clob(:n)
                builder.Append("to_clob(");
                AppendParameter(clause, ref builder, ref values, structure);
                builder.Append(")");
            }
        }

        /// <summary>
        /// The get direct structure field title.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="fullName">
        /// The full Name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDirectStructureFieldString(IColumn field, string fullName)
        {
            if (field == null)
            {
                throw new ArgumentException(@"field can't be null!", "field");
            }

            // 1,Some DirectCartridge operators only support Molecule Object data field as input parameter
            // This function will add convert operator if the given field is not in Molecule Object format
            // 2,To support direct cartridge8.0, we do special process on data.
            string fieldString = string.Empty;

            if (COEDataView.MimeTypes.CHEMICAL_X_DATADIRECT_CTAB == field.MimeType)
            {
                fieldString = fullName;
            }
            else if (COEDataView.MimeTypes.CHEMICAL_X_SMILES == field.MimeType)
            {
                fieldString = string.Format("mol({0})", "'SMILES:'||" + "nvl(" + fullName + ",'*')");
            }
            else
            {
                fieldString = string.Format("mol({0})", fullName);
            }

            return fieldString;
        }
    }
}
