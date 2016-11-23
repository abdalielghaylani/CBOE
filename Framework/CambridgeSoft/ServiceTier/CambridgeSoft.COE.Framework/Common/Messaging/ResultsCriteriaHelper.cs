// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultsCriteriaHelper.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Common
{
    using System.Linq;
    using CambridgeSoft.COE.Framework.Properties;

    /// <summary>
    /// Helper methods for result criteria
    /// </summary>
    public static class ResultsCriteriaHelper
    {
        /// <summary>
        /// Generate new results criteria which contains all related data table through using table relationship
        /// </summary>
        /// <param name="dataView">COEDataVuew object</param>
        /// <param name="resultsCriteria">orginal results criteria</param>
        /// <returns>new results criteria</returns>
        public static ResultsCriteria GenerateFullResultsCriteria(COEDataView dataView, ResultsCriteria resultsCriteria)
        {
            // Create new result criteria for return
            var newResultsCriteria = new ResultsCriteria();
            newResultsCriteria.GetFromXML(resultsCriteria.ToString());

            // Complement absent result criteria
            ComplementResultsCriteria(dataView, newResultsCriteria);

            return newResultsCriteria;
        }

        // This function will add some mandatory result criteria for the requirement of hitlist, 
        // relathions and aggregation.
        private static void ComplementResultsCriteria(COEDataView dataView, ResultsCriteria resultsCriteria)
        {
            // Add aggreation tables and fields
            ComplementAggregateResultCriteria(dataView, resultsCriteria);

            // Add absent parent result tables
            ComplementParentTables(dataView, resultsCriteria);

            // Add absent primary key fields
            AddAbsentPrimaryKeyFields(dataView, resultsCriteria);

            // Add relationship fields
            AddAbsentRelationFields(dataView, resultsCriteria);
        }

        // Add necessary tables and criteria for aggregation. For example, user draged a criteria from child table 
        // to parent table to do aggregation but didn't add the child table in result criteria. In this case, we 
        // have to add the child table by ourselves.
        private static void ComplementAggregateResultCriteria(COEDataView dataView, ResultsCriteria resultsCriteria)
        {
            // Clone a temp result criteria since we are goting to edit the original one
            var tempResultsCriteria = new ResultsCriteria();
            tempResultsCriteria.GetFromXML(resultsCriteria.ToString());

            // Add absent aggregation tables and child fileds
            foreach (var table in tempResultsCriteria.Tables.ToArray())
            {
                var aggregateFuntions =
                    (from c in table.Criterias where c is ResultsCriteria.AggregateFunction select c).ToList();

                // Add the child tables and columns to be aggregated
                foreach (ResultsCriteria.AggregateFunction func in aggregateFuntions.ToArray())
                {
                    foreach (ResultsCriteria.Field field in func.Parameters)
                    {
                        // Get the table that the field from
                        var tableId = dataView.GetFieldById(field.Id).ParentTableId;

                        // The table aready in result criteria or not
                        if (resultsCriteria.Tables.Exists(t => t.Id == tableId))
                        {
                            var existedTable = resultsCriteria.Tables.First(t => t.Id == tableId);

                            // Add aggregation field
                            var fieldName = dataView.GetFieldById(field.Id);

                            if (fieldName.Name.Equals(func.Alias))
                            {
                                existedTable.AppendFieldCriteria(field.Id, func.Alias);
                            }
                            else
                            {
                                // Have changed the alias, check if the alias is duplicate in resultCriteria
                                var isExist = existedTable.Criterias.Any(x => x.Alias == func.Alias);

                                if (isExist)
                                {
                                    int counter = 1;
                                    var alias = func.Alias;
                                    alias = alias + "(" + (++counter) + ")";

                                    while (existedTable.Criterias.Any(x => x.Alias == alias))
                                    {
                                        alias = alias + "(" + (++counter) + ")";
                                    }

                                    existedTable.AppendFieldCriteria(field.Id, alias);
                                }
                                else
                                {
                                    existedTable.AppendFieldCriteria(field.Id, func.Alias);
                                }
                            }
                        }
                        else
                        {
                            // Create a new result table since it's not existed
                            var newTable = new ResultsCriteria.ResultsCriteriaTable { Id = tableId };

                            // Add aggregation field
                            newTable.AppendFieldCriteria(field.Id, func.Alias);

                            resultsCriteria.Add(newTable);
                        }
                    }
                }
            }

            // Remove aggregation result criteria
            foreach (var table in resultsCriteria.Tables)
            {
                var aggregateFuntions =
                    (from c in table.Criterias where c is ResultsCriteria.AggregateFunction select c).ToList();
                
                foreach (var func in aggregateFuntions)
                {
                    table.Criterias.Remove(func);
                }
            }
        }

        // Add absent parent tables
        private static void ComplementParentTables(COEDataView dataView, ResultsCriteria resultsCriteria)
        {
            // Clone a temp result criteria since we are goting to modify the original one
            var tempResultsCriteria = new ResultsCriteria();
            tempResultsCriteria.GetFromXML(resultsCriteria.ToString());

            foreach (var table in tempResultsCriteria.Tables)
            {
                AddParentTables(dataView, resultsCriteria, table.Id);
            }
        }

        /// <summary>
        /// Recursion method to add parent table into the result criteria
        /// </summary>
        /// <param name="dataView"> the dataview to query </param>
        /// <param name="tableId"> the child table id </param>
        /// <param name="resultsCriteria"> the new result criteria to be modified </param>
        private static void AddParentTables(COEDataView dataView, ResultsCriteria resultsCriteria, int tableId)
        {
            // Get all parent relations for the given table
            var parents = from r in dataView.Relationships
                                  where r.Child == tableId
                                  select new { r.Parent, r.ParentKey };

            foreach (var p in parents)
            {
                // Add parent table if it not existed
                if (!resultsCriteria.Tables.Exists(t => t.Id == p.Parent))
                {
                    var parentTable = new ResultsCriteria.ResultsCriteriaTable { Id = p.Parent };
                    resultsCriteria.Add(parentTable);
                }

                // Move up a level
                AddParentTables(dataView, resultsCriteria, p.Parent);
            }
        }

        // Add primary key fields if it was absent
        private static void AddAbsentPrimaryKeyFields(COEDataView dataView, ResultsCriteria resultsCriteria)
        {
            foreach (var table in resultsCriteria.Tables)
            {
                // Add primary key field
                var pk = int.Parse(dataView.GetTablePrimaryKeyById(table.Id));
                resultsCriteria.AddResultCriteria(dataView, table.Id, pk);
            }
        }

        /// <summary>
        /// Add foreign key fields for result criteria table
        /// </summary>
        /// <param name="dataView"> the query data view </param>
        /// <param name="table"> the result criteria talbe </param>
        private static void AddAbsentRelationFields(COEDataView dataView, ResultsCriteria resultsCriteria)
        {
            // Add relation fields
            foreach (var relation in dataView.Relationships)
            {
                if (resultsCriteria.Tables.Exists(t => t.Id == relation.Parent)
                    && resultsCriteria.Tables.Exists(t => t.Id == relation.Child))
                {
                    // add parent key in parent result table
                    resultsCriteria.AddResultCriteria(dataView, relation.Parent, relation.ParentKey);

                    // add child key in child result table
                    resultsCriteria.AddResultCriteria(dataView, relation.Child, relation.ChildKey);
                }
            }
        }

        // Add result field if it not existed
        private static void AddResultCriteria(
            this ResultsCriteria resultsCriteria, COEDataView dataView, int tableId, int fieldId)
        {
            var table = resultsCriteria.GetResultTable(tableId);
            if (!table.Criterias.Exists(c => (c is ResultsCriteria.Field) && ((ResultsCriteria.Field)c).Id == fieldId))
            {
                var field = new ResultsCriteria.Field { Id = fieldId, Alias = dataView.GetFieldById(fieldId).Alias };
                table.Criterias.Add(field);
            }
        }
    }
}
