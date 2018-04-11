using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Utility;

namespace CambridgeSoft.COE.Framework.COESearchService
{
    public class InputFieldsToSearchCriteria
    {
        public static SearchCriteria GetSearchCriteria(string[] fieldCriteria, string[] searchOptions, string[] domain, string domainFieldName, COEDataView dataView)
        {
            SearchCriteria sc = new SearchCriteria();
            string op;
            string tablefield;
            string fieldvalue;
            string s = string.Empty;
            string t = string.Empty;
            string f = string.Empty;
            int tableid = 0, fieldid = 0;
            int tableindex = 0;
            string fieldtype = string.Empty;
            string indextype = string.Empty;
            int i;
            List<string> lookupByValueFields = GetLookupByValueFields(ref searchOptions);
            if(domain != null && domain.Length > 0)
            {
                //tableindex = GetResultTableIndex(dataView, dataView.BaseTableName);
                //Changes on 12Feb 10
                tableindex = GetResultTableIndexOnTabName(dataView, dataView.BaseTableName);
                if(tableindex == -1)
                    throw new Exception("The dataview has a wrong basetable.");


                string domainAlias = string.Empty;

                if(!string.IsNullOrEmpty(domainFieldName))
                {
                    domainAlias = domainFieldName;

                }
                else
                {
                    domainAlias = GetTablePrimaryKeyAlias(dataView.Tables[tableindex]);
                }

                if(string.IsNullOrEmpty(domainAlias))
                    throw new Exception("The basetable's primary key in dataview does not have an alias.");

                string[] tableAndField = domainAlias.Split('.');
                //

                if(tableAndField.Length < 2)
                    throw new Exception("Wrong domain field name, it must be in the form TABLE.FIELD");

                if(tableAndField.Length == 2)
                {
                    //The escape character has replaced. It's used to retain the exact alias name if alias name contain dot operator
                    t = tableAndField[0].Replace('\n', '.');
                    f = tableAndField[1].Replace('\n', '.');

                    tableid = GetResultTableID(dataView, t);
                    tableindex = GetResultTableIndex(dataView, t);
                    fieldid = GetResultFieldID(dataView, tableindex, f);
                    fieldtype = GetResultFieldType(dataView, tableindex, f);
                    indextype = GetResultFieldIndexTpe(dataView, tableindex, f);
                }
                else if(tableAndField.Length == 3)
                {
                    //The escape character has replaced. It's used to retain the exact alias name if alias name contain dot operator
                    s = tableAndField[0].Replace('\n', '.');
                    t = tableAndField[1].Replace('\n', '.');
                    f = tableAndField[2].Replace('\n', '.');

                    tableid = GetResultTableID(dataView, s, t);
                    tableindex = GetResultTableIndex(dataView, t);
                    fieldid = GetResultFieldID(dataView, tableindex, f);
                    fieldtype = GetResultFieldType(dataView, tableindex, f);
                    indextype = GetResultFieldIndexTpe(dataView, tableindex, f);
                }

                //
                if(tableid == -1 || tableindex == -1)
                    throw new Exception("Table \"" + t + "\" was not found on dataview.");
                if(fieldid == -1)
                    throw new Exception("Field \"" + f + "\" was not found on dataview under Table \"" + t + "\".");

                string stringValue = string.Join(",", domain);

                SearchCriteria.SearchCriteriaItem scDomain = BuildSingleCriteriaItem(tableid, fieldid, " IN ", fieldtype, indextype, stringValue, sc.Items.Count, searchOptions, lookupByValueFields.Contains(domainAlias));
                sc.Items.Add(scDomain);
            }

            //loop throug the string array
            i = 0;
            if(fieldCriteria != null)
            {
                foreach(string currentFieldCriteria in fieldCriteria)
                {

                    //currentFieldCriteria will be in the format TABLENAME.FIELDNAME OPERATOR VALUE
                    // examples: 
                    // MOLTABLE.STRUCTURE EXACT c1ccccc1  //note that here the operator is * EXACT * including the spaces
                    // MOLTABLE.NAME = benzene  //there may or may not be spaces here MOLTABLE.NAME=benzene

                    //now take the currentFieldCriteria and split it into a field, operator and value
                    op = GetSearchOperator(currentFieldCriteria);
                    if(op == "RETRIEVEALL")
                    {
                        if(fieldCriteria.Length > 1)
                        {
                            throw new Exception("RETRIEVEALL operator cannot be combined with other filters");
                        }
                        if(sc.Items.Count > 0)
                        {
                            throw new Exception("RETRIEVEALL operator cannot be combined with Domain searches");
                        }
                        break;
                    }

                    if(string.IsNullOrEmpty(op))
                    {
                        throw new Exception("Invalid operator in the following fieldCriteria: \"" + currentFieldCriteria + "\"");
                    }

                    //                    string[] opar = new string[] { op };

                    // 11.0.4 - Setting the start index from which the operator is to be search from the search value
                    //int startAt = currentFieldCriteria.LastIndexOf('"') == -1 ? 0 : currentFieldCriteria.LastIndexOf('"');
                    //tablefield = currentFieldCriteria.Substring(0, currentFieldCriteria.IndexOf(op)).Trim();
                    //fieldvalue = currentFieldCriteria.Substring(currentFieldCriteria.IndexOf(op) + op.Length);                    
                    tablefield = currentFieldCriteria.Substring(0, currentFieldCriteria.LastIndexOf(op)).Trim();
                    fieldvalue = currentFieldCriteria.Substring(currentFieldCriteria.LastIndexOf(op) + op.Length);

                    // 11.0.4 - Replacing the \r with quote, if exists in the search value and removing the quotes from tablefield value
                    fieldvalue = fieldvalue.Replace('\r', '"');
                    tablefield = tablefield.Replace("\"", "");

                    string[] tableAndField = tablefield.Split('.');
                    if(tableAndField.Length < 2)
                        throw new Exception("Wrong criteria field name \"" + tablefield + "\", it must be in the form TABLE.FIELD");
                    if(tableAndField.Length == 2)
                    {
                        //The escape character has replaced. It's used to retain the exact alias name if the name contain dot operator
                        t = tableAndField[0].Replace('\n', '.');
                        f = tableAndField[1].Replace('\n', '.');

                        tableid = GetResultTableID(dataView, t);
                        tableindex = GetResultTableIndex(dataView, t);
                        fieldid = GetResultFieldID(dataView, tableindex, f);
                        fieldtype = GetResultFieldType(dataView, tableindex, f);
                        indextype = GetResultFieldIndexTpe(dataView, tableindex, f);
                    }
                    else if(tableAndField.Length == 3)
                    {
                        //The escape character has replaced. It's used to retain the exact alias name if the name contain dot operator
                        s = tableAndField[0].Replace('\n', '.');
                        t = tableAndField[1].Replace('\n', '.');
                        f = tableAndField[2].Replace('\n', '.');

                        tableid = GetResultTableID(dataView, s, t);
                        tableindex = GetResultTableIndex(dataView, t);
                        fieldid = GetResultFieldID(dataView, tableindex, f);
                        fieldtype = GetResultFieldType(dataView, tableindex, f);
                        indextype = GetResultFieldIndexTpe(dataView, tableindex, f);
                    }

                    if(tableid == -1 || tableindex == -1)
                        throw new Exception("Table \"" + t + "\" was not found on dataview.");
                    if(fieldid == -1)
                        throw new Exception("Field \"" + f + "\" was not found on dataview under Table \"" + t + "\".");

                    if(lookupByValueFields.Contains(tablefield))
                    {
                        GetLookupDisplayInfo(dataView, tableid, tableindex, fieldid, ref fieldtype, ref indextype);
                    }

                    SearchCriteria.SearchExpression sci = BuildCriteriaItem(tableid, fieldid, op, fieldtype, indextype, fieldvalue, sc.Items.Count, searchOptions, lookupByValueFields.Contains(tablefield));

                    sc.Items.Add(sci);

                    i++;
                }
            }
            return sc;
        }

        private static void GetLookupDisplayInfo(COEDataView dataView, int tableid, int tableindex, int fieldid, ref string fieldtype, ref string indextype)
        {
            foreach(COEDataView.Field sourceField in dataView.Tables[tableindex].Fields)
            {
                if(sourceField.Id == fieldid)
                {
                    int lookupdisplayFieldId = sourceField.LookupDisplayFieldId;
                    for(int i = 0; i < dataView.Tables.Count; i++ )
                    {
                        bool foundField = false;
                        foreach(COEDataView.Field field in dataView.Tables[i].Fields)
                        {
                            if(field.Id == lookupdisplayFieldId)
                            {
                                foundField = true;
                                fieldtype = field.DataType.ToString();
                                indextype = field.IndexType.ToString();
                                break;
                            }
                        }
                        if(foundField)
                            break;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// An option is to list the lookup by value field by doing a string of this kind: LookupByValueFields: Table.FieldA, Table.FieldB
        /// </summary>
        /// <param name="searchOptions">The search options</param>
        /// <returns>If a lookup by value field option was specified would return a list of them.</returns>
        private static List<string> GetLookupByValueFields(ref string[] searchOptions)
        {
            List<string> result = new List<string>();
            for(int j = 0; j < searchOptions.Length; j++)
            {
                string option = searchOptions[j];
                if(option.ToLower().Trim().StartsWith("lookupbyvaluefields"))
                {
                    string fieldlist = option.Substring(option.IndexOf(":") + 1).Trim();
                    result = new List<string>(fieldlist.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                    for(int i = 0; i < result.Count; i++)
                    {
                        result[i] = result[i].Trim();
                    }
                    searchOptions[j] = string.Empty;
                    break;
                }
            }            
            return result;
        }

        public static void AddSimilarityIfNecessary(string[] fieldCriteria, ref ResultsCriteria resultsCriteria, COEDataView dataView)
        {
            if(fieldCriteria != null && fieldCriteria.Length > 0)
            {
                string fieldCriteriaStr = string.Join(",", fieldCriteria);
                if(fieldCriteriaStr.Contains(" SIMILARITY "))
                {
                    foreach(string criteria in fieldCriteria)
                    {
                        if(criteria.Contains(" SIMILARITY "))
                        {
                            string tableName = criteria.Substring(0, criteria.IndexOf("."));
                            int tableId = GetResultTableID(dataView, tableName);
                            int tableIndex = GetResultTableIndex(dataView, tableName);
                            string fieldName = criteria.Substring(criteria.IndexOf(".") + 1, criteria.IndexOf(" SIMILARITY ") - criteria.IndexOf("."));
                            int fieldId = GetResultFieldID(dataView, tableIndex, fieldName);

                            ResultsCriteria.Similarity similarity = new ResultsCriteria.Similarity();
                            similarity.Id = fieldId;

                            ResultsCriteria.Screen screen = new ResultsCriteria.Screen();
                            screen.Structure = criteria.Substring(criteria.IndexOf(" SIMILARITY ") + 12);
                            screen.Alias = "screen";

                            similarity.ScreenResultCriteria = screen;

                            foreach(ResultsCriteria.ResultsCriteriaTable table in resultsCriteria.Tables)
                            {
                                if(table.Id == tableId)
                                {
                                    table.Criterias.Add(similarity);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetSearchOperator(string srchValue)
        {
             int indexSpace = srchValue.IndexOf(" ");
            if(indexSpace > -1)
            srchValue = srchValue.Substring(indexSpace, (srchValue.Length - indexSpace)); 
            string operators = "(>=|<=|>|<|=|!=|-|<>| EXACT | SUBSTRUCTURE | SIMILARITY | FULL | IDENTITY | BETWEEN | LIKE | CONTAINS | IN | EQUALS | STARTSWITH | ENDWITH | MOLWEIGHT | FORMULA | NOTCONTAINS | NOTLIKE |RETRIEVEALL)";
            System.Text.RegularExpressions.Regex rgxOprSym = new System.Text.RegularExpressions.Regex("NOT *" + operators, System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            // 11.0.4 - Setting the start index from which the operator is to be search from the search value
            int startAt = srchValue.LastIndexOf('"') == -1 ? 0 : srchValue.LastIndexOf('"');

            // 11.0.4 - Starting the search of operator from the searchvalue after the quote character
            //if(rgxOprSym.Matches(srchValue).Count > 0)
            if (rgxOprSym.Matches(srchValue, startAt).Count > 0)
            {                
                //return rgxOprSym.Match(srchValue).Value;                
                return rgxOprSym.Match(srchValue, startAt).Value;                
            }
            else
            {
                rgxOprSym = new System.Text.RegularExpressions.Regex(operators, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // 11.0.4 - Starting the search of operator from the searchvalue after the quote character 
                //if(rgxOprSym.Matches(srchValue).Count > 0)
                if (rgxOprSym.Matches(srchValue, startAt).Count > 0)
                {
                    //return rgxOprSym.Match(srchValue).Value;                    
                    return rgxOprSym.Match(srchValue, startAt).Value;                    
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        private static int GetResultTableID(COEDataView dataView, string tableName)
        {
            for(int x = 0; x < dataView.Tables.Count; x++)
            {
                if(dataView.Tables[x].Alias.ToString().Trim().Equals(tableName.Trim(), StringComparison.OrdinalIgnoreCase))
                    return dataView.Tables[x].Id;
            }
            return -1;
        }

        private static int GetResultTableID(COEDataView dataView, string schemaName, string tableName)
        {
            for(int x = 0; x < dataView.Tables.Count; x++)
            {
                if((dataView.Tables[x].Alias.ToString().Trim().Equals(tableName.Trim(), StringComparison.OrdinalIgnoreCase)) && dataView.Tables[x].Database.Equals(schemaName, StringComparison.OrdinalIgnoreCase))
                    return dataView.Tables[x].Id;
            }
            return -1;

        }

        private static int GetResultFieldID(COEDataView dataView, int tableIndex, string aliasName)
        {
            try
            {
                for(int x = 0; x < dataView.Tables[tableIndex].Fields.Count; x++)
                {
                    //if (dataView.Tables[tableIndex].Fields[x].Alias.ToString().ToLower().Trim() == aliasName)
                    if(dataView.Tables[tableIndex].Fields[x].Alias.ToString().Trim().Equals(aliasName.Trim(), StringComparison.OrdinalIgnoreCase))
                        return dataView.Tables[tableIndex].Fields[x].Id;
                }
                return -1;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static int GetResultTableIndex(COEDataView dataView, string tableName)
        {

            for(int x = 0; x < dataView.Tables.Count; x++)
            {
                if(dataView.Tables[x].Alias.ToString().Trim().Equals(tableName.Trim(), StringComparison.OrdinalIgnoreCase))
                    return x;
            }
            return -1;
        }
        private static int GetResultTableIndexOnTabName(COEDataView dataView, string tableName)
        {

            for(int x = 0; x < dataView.Tables.Count; x++)
            {
                if(dataView.Tables[x].Name.ToString().Trim().Equals(tableName.Trim(), StringComparison.OrdinalIgnoreCase))
                    return x;
            }
            return -1;
        }

        private static string GetTablePrimaryKeyAlias(COEDataView.DataViewTable table)
        {
            foreach(COEDataView.Field field in table.Fields)
            {
                if(field.Id == int.Parse(table.PrimaryKey))
                {
                    return field.Alias;
                }
            }
            return null;
        }

        private static string GetResultFieldType(COEDataView dataView, int tableIndex, string aliasName)
        {
            try
            {
                for(int x = 0; x < dataView.Tables[tableIndex].Fields.Count; x++)
                {
                    //if (dataView.Tables[tableIndex].Fields[x].Alias.ToString().ToLower().Trim().ToString() == aliasName)
                    if(dataView.Tables[tableIndex].Fields[x].Alias.ToString().Equals(aliasName.Trim(), StringComparison.OrdinalIgnoreCase))
                        return dataView.Tables[tableIndex].Fields[x].DataType.ToString();
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static string GetResultFieldIndexTpe(COEDataView dataView, int tableIndex, string aliasName)
        {

            try
            {
                for(int x = 0; x < dataView.Tables[tableIndex].Fields.Count; x++)
                {
                    if(dataView.Tables[tableIndex].Fields[x].Alias.ToString().Trim().Equals(aliasName.Trim(), StringComparison.OrdinalIgnoreCase))
                        return dataView.Tables[tableIndex].Fields[x].IndexType.ToString();
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static SearchCriteria.SearchExpression BuildCriteriaItem(int tableid, int fieldid, string op, string fieldtype, string indextype, string fieldvalue, int criteriaid, string[] searchoptions, bool useLookupByValue)
        {
            SearchCriteria.LogicalCriteria logicalCriteria = new SearchCriteria.LogicalCriteria();
            logicalCriteria.LogicalOperator = SearchCriteria.COELogicalOperators.Or;


            if(fieldvalue.ToUpper().Contains(" OR "))
            {
                string[] fieldValues = fieldvalue.Replace(" or ", " OR").Replace(" Or ", " OR").Replace(" oR ", " OR").Split(new string[] { " OR" }, StringSplitOptions.None);
                string innerOperator = op;
                string innerFieldValue = fieldValues[0];
                logicalCriteria.Items.Add(BuildSingleCriteriaItem(tableid, fieldid, innerOperator, fieldtype, indextype, innerFieldValue, criteriaid, searchoptions, useLookupByValue));
                int i = 1;
                do
                {
                    innerOperator = GetSearchOperator(fieldValues[i]);
                    innerFieldValue = fieldValues[i].Replace(innerOperator, string.Empty).Trim();
                    logicalCriteria.Items.Add(BuildSingleCriteriaItem(tableid, fieldid, innerOperator, fieldtype, indextype, innerFieldValue, criteriaid, searchoptions, useLookupByValue));
                    i++;
                } while(i < fieldValues.Length && !string.IsNullOrEmpty(innerOperator));

                return logicalCriteria;
            }
            else
            {
                return BuildSingleCriteriaItem(tableid, fieldid, op, fieldtype, indextype, fieldvalue, criteriaid, searchoptions, useLookupByValue);
            }

        }

        private static SearchCriteria.SearchCriteriaItem BuildSingleCriteriaItem(int tableid, int fieldid, string op, string fieldtype, string indextype, string fieldvalue, int criteriaid, string[] searchoptions, bool useLookupByValue)
        {
            SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.NumericalCriteria searchNumericCriteria = new SearchCriteria.NumericalCriteria();
            SearchCriteria.TextCriteria searchTextCriteria = new SearchCriteria.TextCriteria();
            SearchCriteria.StructureCriteria searchStructureCriteria = new SearchCriteria.StructureCriteria();
            SearchCriteria.CSMolWeightCriteria searchMolweightCriteria = new SearchCriteria.CSMolWeightCriteria();
            SearchCriteria.CSFormulaCriteria searchFormulaCriteria = new SearchCriteria.CSFormulaCriteria();
            SearchCriteria.DateCriteria searchDateCriteria = new SearchCriteria.DateCriteria();

            bool negateCriteria = false;
            
            if(op.ToUpper().StartsWith("NOT "))
            {
                op = op.Remove(0, 3);
                negateCriteria = true;
            }

            //base structure field on indextype
            if(indextype.ToUpper() == "CS_CARTRIDGE")
            {
                fieldtype = "STRUCTURE";
            }

            switch(fieldtype.ToUpper())
            {
                case "DATE":
                    switch(op.ToUpper())
                    {
                        case ">=":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.GTE;
                            break;
                        case "<=":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.LTE;
                            break;
                        case "=":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                            break;
                        case "<":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.LT;
                            break;
                        case ">":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.GT;
                            break;
                        case "!=":
                        case "<>":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.NOTEQUAL;
                            break;
                        case " BETWEEN ":
                            searchDateCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                            fieldvalue = fieldvalue.ToUpper().Replace(" AND ", "-");
                            break;
                        default:
                            break;
                    }
                    searchDateCriteria.InnerText = fieldvalue;

                    item.Criterium = searchDateCriteria;
                    item.ID = criteriaid;

                    item.FieldId = fieldid;
                    item.TableId = tableid;
                    item.Modifier = string.Empty;
                    break;
                case "REAL":
                case "INTEGER":
                    switch(op.ToUpper())
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
                        case "!=":
                        case "<>":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.NOTEQUAL;
                            break;
                        case " IN ":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.IN;
                            break;
                        case " BETWEEN ":
                            searchNumericCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                            fieldvalue = fieldvalue.ToUpper().Replace(" AND ", "-");
                            break;
                        default:
                            break;
                    }

                    searchNumericCriteria.Trim = SearchCriteria.Positions.None;
                    searchNumericCriteria.InnerText = fieldvalue;

                    item.Criterium = searchNumericCriteria;
                    item.ID = criteriaid;

                    item.FieldId = fieldid;
                    item.TableId = tableid;
                    item.Modifier = string.Empty;

                    break;
                case "TEXT":
                case "BOOLEAN":
                case "BOOL":
                    char[] charsToTrim = { '%', ' ' };
                    switch(op.ToUpper())
                    {
                        case " LIKE ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.LIKE;
                            string newfieldvalue = fieldvalue.Trim(charsToTrim);//Fixed CSBR-152871
                            newfieldvalue = newfieldvalue.Replace("%", "¬%");
                            if(fieldvalue.StartsWith("%"))
                            {
                                newfieldvalue = "%" + newfieldvalue;
                            }
                            if(fieldvalue.EndsWith("%"))
                            {
                                newfieldvalue += "%";
                            }

                            fieldvalue = newfieldvalue;
                            break;
                        case " NOTLIKE ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.NOTLIKE;
                            newfieldvalue = fieldvalue.Trim(charsToTrim);//Fixed CSBR-152871
                            newfieldvalue = newfieldvalue.Replace("%", "¬%");
                            if(fieldvalue.StartsWith("%"))
                            {
                                newfieldvalue = "%" + newfieldvalue;
                            }
                            if(fieldvalue.EndsWith("%"))
                            {
                                newfieldvalue += "%";
                            }

                            fieldvalue = newfieldvalue;
                            break;
                        case " CONTAINS ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.CONTAINS;
                            fieldvalue = "%" + fieldvalue.Replace("%", "¬%") + "%";
                            break;
                        case " NOTCONTAINS ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.NOTCONTAINS;
                            fieldvalue = "%" + fieldvalue.Replace("%", "¬%") + "%";
                            break;
                        case " EQUALS ":
                        case "=":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.EQUAL;
                            break;
                        case " IN ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.IN;
                            break;
                        case " STARTSWITH ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.STARTSWITH;
                            fieldvalue = fieldvalue.Replace("%", "¬%") + "%";
                            break;
                        case " ENDWITH ":
                            searchTextCriteria.Operator = SearchCriteria.COEOperators.ENDWITH;
                            fieldvalue = fieldvalue.Replace("%", "¬%").Insert(0, "%");
                            break;
                        default:
                            break;
                    }
                    searchTextCriteria.InnerText = fieldvalue;
                    // if the op starts with a capitalized letter, then it is case sensitive.
                    //CBOE-1145 In order to make the list search work with the " IN " Operator in CBVExcel 
                    searchTextCriteria.CaseSensitive = ((op != "=" && op != " IN " && op.Trim().StartsWith(op.Trim().Remove(1).ToUpper())) || fieldtype.ToUpper()=="BOOLEAN" || fieldtype.ToUpper()=="BOOL") ? SearchCriteria.COEBoolean.Yes : SearchCriteria.COEBoolean.No; //CSBR-163077 (Boolean field)
                  
                    item.Criterium = searchTextCriteria;
                    item.ID = criteriaid;

                    item.FieldId = fieldid;
                    item.TableId = tableid;
                    item.Modifier = string.Empty;
                    break;
                case "STRUCTURE":
                    if(op.ToUpper().Contains(" MOLWEIGHT "))
                    {
                        //work on fieldvalue to extract operator and value
                        searchMolweightCriteria.Implementation = "CSCARTRIDGE";
                        searchMolweightCriteria.Value = fieldvalue;

                        item.Criterium = searchMolweightCriteria;
                        item.ID = criteriaid;

                        item.FieldId = fieldid;
                        item.TableId = tableid;
                        item.Modifier = string.Empty;

                        break;
                    }
                    else if(op.ToUpper().Contains(" FORMULA "))
                    {
                        //work on fieldvalue to extract operator and value
                        searchFormulaCriteria.Value = fieldvalue;

                        item.Criterium = searchFormulaCriteria;
                        item.ID = criteriaid;

                        item.FieldId = fieldid;
                        item.TableId = tableid;
                        item.Modifier = string.Empty;

                        break;
                    }
                    else
                    {
                        string options = string.Empty;

                        if(searchoptions != null)
                        {
                            foreach(string option in searchoptions)
                            {
                                if(!string.IsNullOrEmpty(option) && !options.Contains(option))
                                options += option + ",";
                            }
                        }
                        switch(op.ToUpper())
                        {
                            case " IDENTITY ":
                                options += "IDENTITY=YES,";
                                break;
                            case " EXACT ":
                                options += "FULL=YES,";
                                break;
                            case " SUBSTRUCTURE ":
                                options += "FULL=NO,";
                                break;
                            case " SIMILARITY ":
                                options += "SIMILAR=YES,";
                                break;
                            case " FULL ":
                                options += "FULL=YES,";
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
                        searchStructureCriteria.CartridgeParams = options;
                        searchStructureCriteria.Value = fieldvalue;
                        break;
                    }
                default:
                    throw new Exception("The field type \"" + fieldtype + "\" has no operation allowed.");
            }

            item.Criterium.Negate = negateCriteria ? SearchCriteria.COEBoolean.Yes : SearchCriteria.COEBoolean.No;
            item.SearchLookupByID = !useLookupByValue;
            return item;
        }
    }
}
