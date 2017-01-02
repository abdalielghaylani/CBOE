using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;


namespace ChemBioVizExcelAddIn
{
    class SearchCriteriaUtility : CriteriaUtilities
    {
        # region _ Variables _

        private Excel::Worksheet nSheet;

        private List<string> tableList = new List<string>();

        private Dictionary<string, string> tableFields = new Dictionary<string, string>();

        #endregion

        #region _ Constructor _

        public SearchCriteriaUtility(ref Excel::Worksheet worksheet)
        {
            nSheet = worksheet;
        }
        public SearchCriteriaUtility()
        {
            nSheet = null;
        }

        #endregion

        public DataTable GetSearchCriteriaData()
        {
            Excel::Worksheet nSheet = this.nSheet as Excel::Worksheet;

            string value = string.Empty;

            string tableId = string.Empty;
            string fieldId = string.Empty;
            string fieldName = string.Empty;
            string fieldAlias = string.Empty;
            string fieldDataType = string.Empty;
            string searchValue = string.Empty;
            string searhOperator = string.Empty;
            string indexType = string.Empty;

            string searchOption = string.Empty;
            DataTable rcInfo = new DataTable("rc");
            rcInfo.Columns.Add("tableId", typeof(string));
            rcInfo.Columns.Add("fieldId", typeof(string));
            rcInfo.Columns.Add("fieldName", typeof(string));
            rcInfo.Columns.Add("fieldAlias", typeof(string));
            rcInfo.Columns.Add("fieldDataType", typeof(string));
            rcInfo.Columns.Add("searchValue", typeof(string));
            rcInfo.Columns.Add("searchOperator", typeof(string));
            rcInfo.Columns.Add("searchOption", typeof(string));

            StringBuilder sb = new StringBuilder();

            tableList.Clear();
            int maxCnt = Global.CBVNewColumnIndex + Global.StartUpRowPosition(nSheet);
            for (int x = 1; x < maxCnt; x++)
            {
                tableId = GetHeaderTableInfo(x, nSheet, Global.ID);
                fieldId = GetHeaderColumnInfo(x, nSheet, Global.ID);
                fieldName = GetHeaderColumnInfo(x, nSheet,Global.NAME);
                fieldAlias = GetHeaderColumnInfo(x, nSheet,Global.ALIAS);
                indexType = GetHeaderColumnInfo(x, nSheet, Global.INDEXTYPE);             

                //base structure field on indextype
                if (indexType.ToUpper() == "CS_CARTRIDGE")
                    fieldDataType = "STRUCTURE";
                else
                    fieldDataType = GetHeaderColumnInfo(x, nSheet, "datatype");

                searchValue = GetSearchColumnInfo(x, nSheet);
                searhOperator = GetSearchOperator(searchValue.Trim());

                searchOption = GetSearchOptionInfo(x, nSheet);
                               
                ///split the values if range type operator found inside search criteria
                string[] srchRngVal = searchValue.Split('-'); 
                string[] srchRngOpr = { ">=", "<=" };

                if (searchValue != String.Empty)
                {
                    if (searchValue.Contains("-"))
                        // char[] splitter = { '-' };
                        srchRngVal = searchValue.Split('-');
                    else
                    {
                        srchRngVal[0] = GetExactSearchValue(searchValue); //Store the search value (at first index) if range type operator doesn't exist
                        srchRngOpr[0] = searhOperator; // 
                    }

                    if (srchRngVal.Length <= 2)
                    {
                        for (int y = 0; y < srchRngVal.Length; y++)
                        {
                            if (!tableList.Contains(tableId))
                            {
                                tableList.Add(tableId);
                            }

                            DataRow dr = rcInfo.NewRow();

                            sb.Remove(0, sb.Length);
                            sb.Append(tableId);
                            dr["tableId"] = sb.ToString();

                            sb.Remove(0, sb.Length);
                            sb.Append(fieldId);
                            dr["fieldId"] = sb.ToString();

                            sb.Remove(0, sb.Length);
                            sb.Append(fieldId);
                            dr["fieldName"] = sb.ToString();

                            sb.Remove(0, sb.Length);
                            sb.Append(fieldAlias);
                            dr["fieldAlias"] = sb.ToString();

                            sb.Remove(0, sb.Length);
                            sb.Append(fieldDataType);
                            dr["fieldDataType"] = sb.ToString();

                            sb.Remove(0, sb.Length);
                            sb.Append(srchRngVal[y].ToString());
                            dr["searchValue"] = sb.ToString();

                            sb.Remove(0, sb.Length);
                            sb.Append(srchRngOpr[y].ToString());
                            dr["searchOperator"] = sb.ToString();
                            sb.Remove(0, sb.Length);
                            sb.Append(searchOption.ToString());
                            dr["searchOption"] = sb.ToString();

                            rcInfo.Rows.Add(dr);
                        }
                    }
                }
            }
            return rcInfo;
        }

        public SearchCriteria GetSearchCriteria()
        {
            SearchCriteria searchCriteria = new SearchCriteria();
            DataTable dtSrchCrData = GetSearchCriteriaData();
            int srchLoopIndx = 0;

            if (dtSrchCrData == null  || dtSrchCrData.Rows.Count <=0)
                return null;
            foreach (DataRow drSrchCrData in dtSrchCrData.Rows)
            {
                srchLoopIndx++;
                SearchCriteriaUtility scUtil = new SearchCriteriaUtility(ref nSheet);
                SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
                SearchCriteria.NumericalCriteria searchNumericCriteria = new SearchCriteria.NumericalCriteria();
                SearchCriteria.TextCriteria searchTextCriteria = new SearchCriteria.TextCriteria();
                SearchCriteria.StructureCriteria searchStructureCriteria = new SearchCriteria.StructureCriteria();
                string fieldDataType = drSrchCrData["fieldDataType"].ToString().ToUpper();
                switch (fieldDataType)
                {
                    case "INTEGER":
                        switch (drSrchCrData["SearchOperator"].ToString())
                        {
                            case ">=":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.GTE;
                                break;

                            case "<=":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.LTE;
                                break;

                            case "=":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                                break;

                            case "<":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.LT;
                                break;

                            case ">":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.GT;
                                break;

                            case "%":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                                break;
                            case "<>":
                                searchNumericCriteria.Operator = SearchCriteria.COEOperators.NOTEQUAL;
                                break;
                            default:
                                break;
                        }
                        searchNumericCriteria.Trim = SearchCriteria.Positions.None;
                        searchNumericCriteria.InnerText = drSrchCrData["searchValue"].ToString();
                        //equalHitList.InnerText = "24,28"; 
                        item.Criterium = searchNumericCriteria;
                        item.ID = srchLoopIndx;

                        item.FieldId = Convert.ToInt32(drSrchCrData["fieldId"].ToString());
                        item.TableId = Convert.ToInt32(drSrchCrData["tableId"].ToString());
                        item.Modifier = string.Empty;
                        searchCriteria.Items.Add(item);
                        break;
                    case "TEXT":
                        if (drSrchCrData["fieldName"].ToString().ToLower() == "base64_cdx")
                        {
                            fieldDataType = "structure";
                            break;
                        }
                        switch (drSrchCrData["SearchOperator"].ToString())
                        {                           
                            case "%":
                                searchTextCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                                break;
                            case "=":
                                searchTextCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                                break;
                            default:
                                break;
                        }
                        searchTextCriteria.Trim = SearchCriteria.Positions.None;
                        searchTextCriteria.DefaultWildCardPosition = SearchCriteria.Positions.Both;
                        searchTextCriteria.InnerText = drSrchCrData["searchValue"].ToString();
                        //equalHitList.InnerText = "24,28"; 
                        item.Criterium = searchTextCriteria;
                        item.ID = srchLoopIndx;

                        item.FieldId = Convert.ToInt32(drSrchCrData["fieldId"].ToString());
                        item.TableId = Convert.ToInt32(drSrchCrData["tableId"].ToString());
                        item.Modifier = string.Empty;
                        searchCriteria.Items.Add(item);
                        break;

                    //structure:
                    case "STRUCTURE":

                        //string options = string.Empty;
                        //foreach (string option in searchoptions)
                        //{
                        //    options += option + ",";
                        //}

                        //switch (op)
                        //{
                        //    case " EXACT ":
                        //        options += "FULL=YES,";
                        //        break;

                        //    case " SUBSTRUCTURE ":
                        //        options += "FULL=NO,";
                        //        break;

                        //    case " SIMILARITY ":
                        //        options += "SIMILAR=YES,";
                        //        break;

                        //    default:
                        //        break;
                        //}
                        ////since it will always end with a comma get rid of last one.
                        //options = options.Substring(0, options.Length - 1);

                        //switch (drSrchCrData["SearchOperator"].ToString())
                        //{
                        //    case "%":
                        //        searchTextCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                        //        break;
                        //    case "=":
                        //        searchTextCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                        //        break;
                        //    default:
                        //        break;
                        //}
                        //searchTextCriteria.Trim = SearchCriteria.Positions.None;

                        item.Criterium = searchStructureCriteria;
                        item.ID = srchLoopIndx;

                        item.FieldId = Convert.ToInt32(drSrchCrData["fieldId"].ToString());
                        item.TableId = Convert.ToInt32(drSrchCrData["tableId"].ToString());
                        item.Modifier = string.Empty;
                        searchStructureCriteria.Implementation = "CSCARTRIDGE";
                        searchStructureCriteria.Value = drSrchCrData["searchValue"].ToString();
                        searchStructureCriteria.Similar = SearchCriteria.COEBoolean.No;
                        searchCriteria.Items.Add(item);
                        break;

                    default:
                        break;
                }           
            }
            searchCriteria.XmlNS = "COE.SearchCriteria";
            searchCriteria.SearchCriteriaID = 1;
            return searchCriteria;
        }
              
        private string GetSearchOperator(string srchValue)
        {
            //Regex rgxOprSym = new Regex("{|>=|<=|>|<|=|!=|-|<>|%|}");
            //Regex rgxOprStr = new Regex("|GTE|LTE|GT|LT|EQUQL|NOTEQUAL|IN|NOTIN|LIKE|");
            Regex rgxOprSym = new Regex("{|>=|<=|>|<|=|!=|-|<>|%| EXACT | SUBSTRUCTURE | SIMILARITY | BETWEEN}");

            if (rgxOprSym.Matches(srchValue).Count == 1)
            {
                return rgxOprSym.Match(srchValue).Value.ToString();
            }
            else
            {
                return "";
            }
        }

        private string GetExactSearchValue(string srchValue)
        {
            Regex rgxOprSym = new Regex("{|>=|<=|>|<|=|!=|-|<>|%|}");
            // Regex rgxOprStr = new Regex("|GTE|LTE|GT|LT|EQUQL|NOTEQUAL|IN|NOTIN");

            if (rgxOprSym.Matches(srchValue).Count == 1)
            {
             // if(rgxOprSym.Match(srchValue).Value.ToString()!= "-")
                return srchValue.Replace(rgxOprSym.Match(srchValue).Value, "");
            //return srchValue;
            }
            else
            {
                return srchValue;
            }
        }


        #region "Not in use"
        /*
        public SearchCriteria GetSearchCriteria(string[] SearchOptions, string[] Domain)
        {
            SearchCriteria sc = new SearchCriteria();
            string op;
            string tablefield;
            string fieldvalue;
            string t;
            string f;
            int tableid, fieldid;
            int tableindex;
            string fieldtype;
            string indextype;
            int i = 0;

            Excel::Worksheet nSheet = this.nSheet as Excel::Worksheet;

            string value = string.Empty;

            string tableId = string.Empty;
            string fieldId = string.Empty;
            string fieldName = string.Empty;
            string tableField = string.Empty;
            string fieldValue = string.Empty;
            string fieldAlias = string.Empty;
            string fieldDataType = string.Empty;
            string fieldCriteria = string.Empty;
            string searhOperator = string.Empty;
            string indexType = string.Empty;           
            string opr = string.Empty;
            //loop throug the string array


            for (int x = 1; x < Global.CBVNewColumnIndex; x++)
            {
                tableId = GetHeaderTableInfo(x, nSheet, "id");
                fieldId = GetHeaderColumnInfo(x, nSheet, "id");
                fieldName = GetHeaderColumnInfo(x, nSheet, "name");
                fieldDataType = GetHeaderColumnInfo(x, nSheet, "datatype");
                fieldAlias = GetHeaderColumnInfo(x, nSheet, "alias");
                indexType = GetHeaderColumnInfo(x, nSheet, "indextype");
                
                //filed data type have assigned structure, if alias name is equal structure

                fieldCriteria = GetSearchColumnInfo(x, nSheet);
                if (fieldCriteria != string.Empty)
                {

                    opr = GetSearchOperator(fieldCriteria.Trim());
                    string[] opar = new string[] { opr };
                    tableField = fieldCriteria.Split(opar, StringSplitOptions.RemoveEmptyEntries)[0];
                    fieldValue = fieldCriteria.Split(opar, StringSplitOptions.RemoveEmptyEntries)[1];


                    //if (fieldAlias.ToUpper().Trim() == "STRUCTURE")
                    //    fieldDataType = "STRUCTURE";
                    //else
                    //    fieldDataType = GetHeaderColumnInfo(x, nSheet, "datatype");

                    //searchValue = GetSearchColumnInfo(x, nSheet);
                    //searhOperator = GetSearchOperator(searchValue.Trim());

                    SearchCriteria.SearchCriteriaItem sci = BuildCriteriaItem(Convert.ToInt32(tableId), Convert.ToInt32(fieldId), opr, fieldDataType, indexType, fieldValue, i, SearchOptions);

                    sc.Items.Add(sci);

                    i++;
                }
            }
            sc.XmlNS = "COE.SearchCriteria";
            sc.SearchCriteriaID = 1;

            return sc;


            //foreach (string currentFieldCriteria in FieldCriteria)
            //{

            //    //currentFieldCriteria will be in the format TABLENAME.FIELDNAME OPERATOR VALUE
            //    // examples: 
            //    // MOLTABLE.STRUCTURE EXACT c1ccccc1  //note that here the operator is * EXACT * including the spaces
            //    // MOLTABLE.NAME = benzene  //there may or may not be spaces here MOLTABLE.NAME=benzene
            //    //i++;
            //    //now take the currentFieldCriteria and split it into a field, operator and value
            //    op = GetSearchOperator(currentFieldCriteria);
            //    string[] opar = new string[] { op };
            //    tablefield = currentFieldCriteria.Split(opar, StringSplitOptions.RemoveEmptyEntries)[0];
            //    fieldvalue = currentFieldCriteria.Split(opar, StringSplitOptions.RemoveEmptyEntries)[1];

            //    t = tablefield.Split('.')[0];
            //    f = tablefield.Split('.')[1];

            //    tableid = GetResultTableID(_dataViewBO.COEDataView, t);
            //    tableindex = GetResultTableIndex(_dataViewBO.COEDataView, t);
            //    fieldid = GetResultFieldID(_dataViewBO.COEDataView, tableindex, f);
            //    fieldtype = GetResultFieldType(_dataViewBO.COEDataView, tableindex, f);
            //    indextype = GetResultFieldIndexTpe(_dataViewBO.COEDataView, tableindex, f);


            //    SearchCriteria.SearchCriteriaItem sci = BuildCriteriaItem(tableid, fieldid, op, fieldtype, indextype, fieldvalue, i, SearchOptions);

            //    sc.Items.Add(sci);

            //    i++;
            //}
            //sc.XmlNS = "COE.SearchCriteria";
            //sc.SearchCriteriaID = 1;

            //return sc;
        }

        public SearchCriteria.SearchCriteriaItem BuildCriteriaItem(int tableid, int fieldid, string op, string fieldtype, string indextype, string fieldvalue, int criteriaid, string[] searchoptions)
        {
            //SearchCriteria searchCriteria = new SearchCriteria();
            //DataTable dtSrchCrData = GetSearchCriteriaData(dataView, searchValues);
            //int srchLoopIndx = 0;

            //if (dtSrchCrData == null || dtSrchCrData.Rows.Count <= 0)
            //	return null;
            //foreach (DataRow drSrchCrData in dtSrchCrData.Rows)
            //{
            //srchLoopIndx++;
            //SearchCriteriaUtilityS scUtil = new SearchCriteriaUtilityS();
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.NumericalCriteria searchNumericCriteria = new SearchCriteria.NumericalCriteria();
            SearchCriteria.TextCriteria searchTextCriteria = new SearchCriteria.TextCriteria();
            SearchCriteria.StructureCriteria searchStructureCriteria = new SearchCriteria.StructureCriteria();


            //base structure field on indextype
            if (indextype.ToUpper() == "CSCARTRIDGE")
            {
                fieldtype = "STRUCTURE";
            }

            //string fieldDataType = drSrchCrData["fieldDataType"].ToString().ToUpper();
            //switch (fieldDataType)
            switch (fieldtype.ToUpper().ToString())
            {
                case "INTEGER":
                    switch (op)
                    {
                        case ">=":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.GTE;
                            break;

                        case "<=":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.LTE;
                            break;

                        case "=":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                            break;

                        case "<":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.LT;
                            break;

                        case ">":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.GT;
                            break;

                        case "%":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                            break;
                        case "<>":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.NOTEQUAL;
                            break;
                        default:
                            break;
                    }
                    searchNumericCriteria.Trim = SearchCriteria.Positions.None;
                    //searchNumericCriteria.InnerText = drSrchCrData["searchValue"].ToString();
                    searchNumericCriteria.InnerText = fieldvalue;

                    item.Criterium = searchNumericCriteria;
                    //item.ID = srchLoopIndx; ;
                    item.ID = criteriaid;

                    //item.FieldId = Convert.ToInt32(drSrchCrData["fieldId"].ToString());
                    //item.TableId = Convert.ToInt32(drSrchCrData["tableId"].ToString());
                    item.FieldId = fieldid;
                    item.TableId = tableid;
                    item.Modifier = string.Empty;
                    //searchCriteria.Items.Add(item);
                    return item;
                    break;
                case "TEXT":
                    switch (op)
                    {
                        case "%":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                            break;
                        case "=":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                            break;
                        default:
                            break;
                    }
                    //searchNumericCriteria.InnerText = drSrchCrData["searchValue"].ToString();
                    searchNumericCriteria.InnerText = fieldvalue;

                    item.Criterium = searchTextCriteria;
                    //item.ID = srchLoopIndx; ;
                    item.ID = criteriaid;

                    //item.FieldId = Convert.ToInt32(drSrchCrData["fieldId"].ToString());
                    //item.TableId = Convert.ToInt32(drSrchCrData["tableId"].ToString());
                    item.FieldId = fieldid;
                    item.TableId = tableid;
                    item.Modifier = string.Empty;
                    //searchCriteria.Items.Add(item);
                    return item;
                    break;

                //structure:
                case "STRUCTURE":
                    string options = string.Empty;
                    foreach (string option in searchoptions)
                    {
                        options += option + ",";
                    }

                    switch (op)
                    {
                        case " EXACT ":
                            options += "FULL=YES,";
                            break;

                        case " SUBSTRUCTURE ":
                            options += "FULL=NO,";
                            break;

                        case " SIMILARITY ":
                            options += "SIMILAR=YES,";
                            break;

                        default:
                            break;
                    }

                    //since it will always end with a comma get rid of last one.
                    options = options.Substring(0, options.Length - 1);

                    item.Criterium = searchStructureCriteria;
                    item.ID = criteriaid;

                    item.FieldId = fieldid;
                    item.TableId = tableid;
                    item.Modifier = string.Empty;
                    searchStructureCriteria.Implementation = "CSCARTRIDGE";
                    //searchStructureCriteria.Format = "BASE64CDX";
                    searchStructureCriteria.Value = fieldvalue;


                    return item;
                    //break;

                default:
                    return null;
                    //break;

            }

        }*/
        #endregion
    }
}
