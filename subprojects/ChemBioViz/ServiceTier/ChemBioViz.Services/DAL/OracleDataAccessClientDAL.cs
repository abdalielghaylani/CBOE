using System;
using System.Collections.Generic;
using System.Data;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;

namespace CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService
{
    public class OracleDataAccessClientDAL : DAL {
        internal override DataSet GetDataSet(string databaseName, COEDataView dataView, SearchCriteria searchCriteria, ResultsCriteria resultsCriteria, PagingInfo pagingInfo) {
            COESearch search = new COESearch();
            

            SearchResponse response = search.DoSearch(searchCriteria
                , resultsCriteria, pagingInfo, dataView);
            return response.ResultsDataSet;
        }

        internal override void InsertRecord(DataSet dataset) {
            List<Insert> inserts = new List<Insert>();
            // TODO: begin transaction
            foreach(DataTable table in dataset.Tables){
                Insert insert = new Insert();
                insert.MainTable = new Table(table.TableName);
                foreach(DataColumn column in table.Columns){
                    Field field = new Field();
                    field.FieldName = column.ColumnName;
                    insert.Fields.Add(field);
                }

                foreach(DataRow row in table.Rows) {
                    foreach(DataColumn col in table.Columns){
                        Value val = new Value();
                        val.Val = row[col.ColumnName].ToString();
                        insert.ParamValues.Add(val);
                    }
                    inserts.Add(insert);
                    // TODO: execute insert.ToString();
                }
            }
            // TODO: end transaction.
        }

        internal override void UpdateRecord(DataSet dataset) {
            List<Insert> inserts = new List<Insert>();
            // TODO: begin transaction
            foreach(DataTable table in dataset.Tables) {
                Insert insert = new Insert();
                insert.MainTable = new Table(table.TableName);
                foreach(DataColumn column in table.Columns) {
                    Field field = new Field();
                    field.FieldName = column.ColumnName;
                    insert.Fields.Add(field);
                }

                foreach(DataRow row in table.Rows) {
                    foreach(DataColumn col in table.Columns) {
                        Value val = new Value();
                        val.Val = row[col.ColumnName].ToString();
                        insert.ParamValues.Add(val);
                    }
                    inserts.Add(insert);
                    // TODO: execute insert.ToString();
                }
            }
            // TODO: end transaction.
        }
    }
}
