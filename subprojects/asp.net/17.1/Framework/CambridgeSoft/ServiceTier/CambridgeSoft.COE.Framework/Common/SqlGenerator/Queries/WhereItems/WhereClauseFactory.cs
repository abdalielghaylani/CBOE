using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Xml;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Common.Utility;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Types.Exceptions;
using System.IO;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems
{
    /// <summary>
    /// Creates the right Where Clause Item subclass.
    /// </summary>
    public sealed class WhereClauseFactory
    {
        #region Public Methods
        private static Field GetField(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataView, int fieldId, XmlNode operationNode)
        {
            if (fieldId > 0)
            {
                //bool searchLookupById = operationNode.ParentNode.Attributes["searchLookupByID"] == null || string.IsNullOrEmpty(operationNode.ParentNode.Attributes["searchLookupByID"].Value) || operationNode.ParentNode.Attributes["searchLookupByID"].Value.ToLower() != "false";
                bool searchLookupById = true;
                //Coverity Bug Fix CID 11478 
                if (operationNode != null && operationNode.ParentNode != null)
                {
                    if (operationNode.ParentNode.Attributes["searchLookupByID"] != null)
                    {
                        string searchLookupByIdValue = operationNode.ParentNode.Attributes["searchLookupByID"].Value;
                        searchLookupById = string.IsNullOrEmpty(searchLookupByIdValue) || (searchLookupByIdValue.ToLower() != "false" && searchLookupByIdValue.ToLower() != "0");
                    }
                }

                if (searchLookupById)
                {
                    return dataView.GetField(fieldId);
                }
                else
                {
                    IColumn col = dataView.GetColumn(fieldId);
                    if (col is Lookup)
                        return (((Lookup)col)).LookupDisplayField as Field;
                    else
                        return dataView.GetField(fieldId);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates the right Where Clause Item subclass.
        /// </summary>
        /// <param name="field">Field name in database.</param>
        /// <param name="operationNode">Parameters in an xml node format.</param>
        /// <returns>A WhereClauseItem created with the right subclass.</returns>
        public static WhereClauseItem CreateWhereClauseItem(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataView, int fieldId, XmlNode operationNode)
        {
            WhereClauseItem item = null;
           

            bool caseSensitive = (operationNode.Attributes["caseSensitive"] != null &&
                                  operationNode.Attributes["caseSensitive"].Value.ToLower() == "no") ?
                                  false : true;

            SearchCriteria.Positions trimPosition = (operationNode.Attributes["trim"] != null &&
                                                     operationNode.Attributes["trim"].Value.Trim() != string.Empty) ?
                                                     trimPosition = COEConvert.ToPositions(operationNode.Attributes["trim"].Value) :
                                                     SearchCriteria.Positions.None;

            bool negate = (operationNode.Attributes["negate"] != null &&
                           operationNode.Attributes["negate"].Value.ToLower() == "yes") ?
                           true : false;

            Field field = GetField(dataView, fieldId, operationNode);
            //Coverity Bug Fix CID 11478 
            //if (field != null)
            //{
            //Commented if condition, as it was breaking search.
                switch (operationNode.Name.ToLower().Trim())
                {
                    case "textcriteria":
                        item = ParseTextCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "datecriteria":
                        item = ParseDateCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "numericalcriteria":
                        item = ParseNumericalCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "verbatimcriteria":
                        item = ParseVerbatimCriteriaOperation(null, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "structurecriteria":
                        item = ParseStructureCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "structurelistcriteria":
                        item = ParseStructureListCriteriaOperation(field, operationNode, negate);
                        break;
                    case "directflexmatchcriteria":
                        item = ParseDirectFlexMatchCriteriaOperation(field, operationNode, negate);
                        break;
                    case "directsimilarcriteria":
                        item = ParseDirectSimilarCriteriaOperation(field, operationNode, negate);
                        break;
                    case "directssscriteria":
                        item = ParseDirectSssCriteriaOperation(field, operationNode, negate);
                        break;
                    case "jchemstructurecriteria":
                        item = ParseJChemStructureCriteriaOperation(field, operationNode, negate);
                        break;
                    case "molweightcriteria":
                        item = ParseMolWeightCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "directmolweightcriteria":
                        item = ParseDirectMolWeightCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "jchemmolweightcriteria":
                        item = ParseJChemMolWeightCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "formulacriteria":
                        item = ParseFormulaCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "directformulacriteria":
                        item = ParseDirectFormulaCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "jchemformulacriteria":
                        item = ParseJChemFormulaCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "customcriteria":
                        item = ParseCustomCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "fulltextcriteria":
                        item = ParseFullTextCriteriaOperation(field, operationNode, negate, trimPosition, caseSensitive);
                        break;
                    case "hitlistcriteria":
                        item = ParseHitlistCriteriaOperation(field, operationNode, negate, dataView);
                        break;
                }
            //}
            //Coverity Bug Fix CID 11478 
            //if (item != null)
            //{
                item.Hint = (operationNode.Attributes["hint"] != null &&
                                      !string.IsNullOrEmpty(operationNode.Attributes["hint"].Value)) ?
                                      operationNode.Attributes["hint"].Value : string.Empty;

                item.AggregateFunctionName = (operationNode.ParentNode.Attributes["aggregateFunctionName"] != null &&
                                      !string.IsNullOrEmpty(operationNode.ParentNode.Attributes["aggregateFunctionName"].Value)) ?
                                      operationNode.ParentNode.Attributes["aggregateFunctionName"].Value : string.Empty;
            //}
            return item;
        }

        /// <summary>
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="criteriaNode"></param>
        /// <returns></returns>
        public static WhereClauseLogical CreateWhereClauseLogical(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataView, XmlNode criteriaNode)
        {
            WhereClauseLogical item = new WhereClauseLogical();

            if (criteriaNode.ChildNodes.Count == 1)
            {
                throw new Exception("The logical group: " + criteriaNode.Name + ", must enclose two or more criteria.");
            }

            foreach (XmlNode expressionNode in criteriaNode.ChildNodes)
            {
                if (expressionNode.NodeType == XmlNodeType.Element)
                {
                    WhereClauseBase clauseBase = null;
                    switch (expressionNode.Name.ToLower())
                    {
                        case "logicalcriteria":
                        case "groupcriteria":
                            clauseBase = CreateWhereClauseLogical(dataView, expressionNode);
                            break;
                        case "searchcriteriaitem":
                            XmlNodeList operationList = expressionNode.ChildNodes;
                            foreach (XmlNode operationNode in operationList)
                            {
                                clauseBase = CreateWhereClauseItem(dataView, int.Parse(expressionNode.Attributes["fieldid"].Value), operationNode);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported Expression: " + expressionNode.Name);
                            break;
                    }
                    
                        item.Items.Add(clauseBase);
                    
                }
            }

            if (criteriaNode.Attributes["operator"] != null && criteriaNode.Attributes["operator"].Value != string.Empty)
                item.LogicalOperator = COEConvert.ToCOELogicalOperators(criteriaNode.Attributes["operator"].Value);
            else
                item.LogicalOperator = SearchCriteria.COELogicalOperators.And;

            return item;
        }

        /// <summary>
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="criteriaNode"></param>
        /// <returns></returns>
        public static WhereClauseBase CreateWhereClause(CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataView, XmlNode criteriaNode)
        {
            WhereClauseBase item = null;

            switch (criteriaNode.Name.ToLower())
            {
                case "logicalcriteria":
                case "groupcriteria":
                    item = CreateWhereClauseLogical(dataView, criteriaNode);
                    break;
                case "searchcriteriaitem":
                    XmlNodeList operationList = criteriaNode.ChildNodes;
                    foreach (XmlNode operationNode in operationList)
                    {
                        item = CreateWhereClauseItem(dataView, int.Parse(criteriaNode.Attributes["fieldid"].Value), operationNode);
                    }
                    break;
                default:
                    throw new Exception("Unsupported Expression: " + criteriaNode.Name);
                    break;
            }
            return item;
        }
        #endregion

        #region Private Methods

        private static WhereClauseItem ParseTextCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseItem item = null;

            string operation = XmlTranslation.GetOperation(operationNode, WhereClauseTypes.TextCriteria);
            string value = operationNode.InnerText;

            bool normalizeChemicalName = (operationNode.Attributes["normalizedChemicalName"] != null &&
                                          operationNode.Attributes["normalizedChemicalName"].Value.ToLower() == "yes") ?
                                          true : false;

            bool hillFormula = (operationNode.Attributes["hillFormula"] != null &&
                                operationNode.Attributes["hillFormula"].Value.ToLower() == "yes") ?
                                true : false;
            if (hillFormula)
                throw new SQLGeneratorException(string.Format("{0} - Attribute Unimplemented: {1}", "TextCriteria", "hillFormula"));

            switch (operation)
            {
                case "equal":
                    item = new WhereClauseEqual();
                    ((WhereClauseEqual)item).Negate = negate;
                    ((WhereClauseEqual)item).TrimPosition = trimPosition;
                    ((WhereClauseEqual)item).CaseSensitive = caseSensitive;
                    ((WhereClauseEqual)item).NormalizeChemicalName = normalizeChemicalName;
                    ((WhereClauseEqual)item).DataField = field;
                    ((WhereClauseEqual)item).Val = new Value(value, DbType.String);
                    break;
                case "gt":
                    item = new WhereClauseGreaterThan();
                    ((WhereClauseGreaterThan)item).Negate = negate;
                    ((WhereClauseGreaterThan)item).TrimPosition = trimPosition;
                    ((WhereClauseGreaterThan)item).GreaterEqual = false;
                    ((WhereClauseGreaterThan)item).NormalizeChemicalName = normalizeChemicalName;
                    ((WhereClauseGreaterThan)item).DataField = field;
                    ((WhereClauseGreaterThan)item).Val = new Value(value, DbType.String);
                    break;
                case "gte":
                    item = new WhereClauseGreaterThan();
                    ((WhereClauseGreaterThan)item).Negate = negate;
                    ((WhereClauseGreaterThan)item).TrimPosition = trimPosition;
                    ((WhereClauseGreaterThan)item).GreaterEqual = true;
                    ((WhereClauseGreaterThan)item).NormalizeChemicalName = normalizeChemicalName;
                    ((WhereClauseGreaterThan)item).DataField = field;
                    ((WhereClauseGreaterThan)item).Val = new Value(value, DbType.String);
                    break;
                case "lt":
                    item = new WhereClauseLessThan();
                    ((WhereClauseLessThan)item).Negate = negate;
                    ((WhereClauseLessThan)item).TrimPosition = trimPosition;
                    ((WhereClauseLessThan)item).LessEqual = false;
                    ((WhereClauseLessThan)item).NormalizeChemicalName = normalizeChemicalName;
                    ((WhereClauseLessThan)item).DataField = field;
                    ((WhereClauseLessThan)item).Val = new Value(value, DbType.String);
                    break;
                case "lte":
                    item = new WhereClauseLessThan();
                    ((WhereClauseLessThan)item).Negate = negate;
                    ((WhereClauseLessThan)item).TrimPosition = trimPosition;
                    ((WhereClauseLessThan)item).LessEqual = true;
                    ((WhereClauseLessThan)item).NormalizeChemicalName = normalizeChemicalName;
                    ((WhereClauseLessThan)item).DataField = field;
                    ((WhereClauseLessThan)item).Val = new Value(value, DbType.String);
                    break;
                case "in":
                    item = new WhereClauseIn();
                    ((WhereClauseIn)item).Negate = negate;
                    ((WhereClauseIn)item).TrimPosition = trimPosition;
                    ((WhereClauseIn)item).CaseSensitive = caseSensitive;
                    ((WhereClauseIn)item).DataField = field;
                    ((WhereClauseIn)item).NormalizeChemicalName = normalizeChemicalName;
                    string[] vals = value.Split(',');
                    Value[] values = new Value[vals.Length];
                    for (int i = 0; i < vals.Length; i++)
                    {
                        string newValue = vals[i].Trim();
                        if (!caseSensitive)
                            newValue = newValue.ToLower();
                        values[i] = new Value(newValue, DbType.String);
                    }
                    ((WhereClauseIn)item).Values = values;
                    break;
                case "like":
                    item = new WhereClauseLike();
                    ((WhereClauseLike)item).Negate = negate;
                    ((WhereClauseLike)item).TrimPosition = trimPosition;
                    ((WhereClauseLike)item).CaseSensitive = caseSensitive;
                    ((WhereClauseLike)item).FullWordSearch = (operationNode.Attributes["fullWordSearch"] != null &&
                                                               operationNode.Attributes["fullWordSearch"].Value.ToLower() == "yes") ?
                                                               true : false;
                    ((WhereClauseLike)item).NormalizeChemicalName = normalizeChemicalName;
                    if (operationNode.Attributes["defaultWildCardPosition"] != null)
                        ((WhereClauseLike)item).WildCardPosition = COEConvert.ToPositions(operationNode.Attributes["defaultWildCardPosition"].Value);
                    ((WhereClauseLike)item).DataField = field;
                    ((WhereClauseLike)item).Val = new Value(value, DbType.String);
                    break;
                case "notlike":
                    item = new WhereClauseNotLike();
                    ((WhereClauseNotLike)item).Negate = negate;
                    ((WhereClauseNotLike)item).TrimPosition = trimPosition;
                    ((WhereClauseNotLike)item).CaseSensitive = caseSensitive;
                    ((WhereClauseNotLike)item).FullWordSearch = (operationNode.Attributes["fullWordSearch"] != null &&
                                                               operationNode.Attributes["fullWordSearch"].Value.ToLower() == "yes") ?
                                                               true : false;
                    ((WhereClauseNotLike)item).NormalizeChemicalName = normalizeChemicalName;
                    if (operationNode.Attributes["defaultWildCardPosition"] != null)
                        ((WhereClauseNotLike)item).WildCardPosition = COEConvert.ToPositions(operationNode.Attributes["defaultWildCardPosition"].Value);
                    ((WhereClauseNotLike)item).DataField = field;
                    ((WhereClauseNotLike)item).Val = new Value(value, DbType.String);
                    break;
                case "contains":
                    item = new WhereClauseContains();
                    ((WhereClauseContains)item).Negate = negate;
                    ((WhereClauseContains)item).TrimPosition = trimPosition;
                    ((WhereClauseContains)item).CaseSensitive = caseSensitive;
                    ((WhereClauseContains)item).DataField = field;
                    ((WhereClauseContains)item).Val = new Value(value, DbType.String);
                    break;
                case "notcontains":
                    item = new WhereClauseNotContains();
                    ((WhereClauseNotContains)item).Negate = negate;
                    ((WhereClauseNotContains)item).TrimPosition = trimPosition;
                    ((WhereClauseNotContains)item).CaseSensitive = caseSensitive;
                    ((WhereClauseNotContains)item).DataField = field;
                    ((WhereClauseNotContains)item).Val = new Value(value, DbType.String);
                    break;
                case "endwith":
                    item = new WhereClauseEndsWith();
                    ((WhereClauseEndsWith)item).Negate = negate;
                    ((WhereClauseEndsWith)item).TrimPosition = trimPosition;
                    ((WhereClauseEndsWith)item).CaseSensitive = caseSensitive;
                    ((WhereClauseEndsWith)item).DataField = field;
                    ((WhereClauseEndsWith)item).Val = new Value(value, DbType.String);
                    break;
                case "startswith":
                    item = new WhereClauseStartsWith();
                    ((WhereClauseStartsWith)item).Negate = negate;
                    ((WhereClauseStartsWith)item).TrimPosition = trimPosition;
                    ((WhereClauseStartsWith)item).CaseSensitive = caseSensitive;
                    ((WhereClauseStartsWith)item).DataField = field;
                    ((WhereClauseStartsWith)item).Val = new Value(value, DbType.String);
                    break;
                case "not in":
                    //TODO: implement NOT IN comand in textCriterias
                    break;
                case "notequal":
                    item = new WhereClauseNotEqual();
                    ((WhereClauseNotEqual)item).Negate = negate;
                    ((WhereClauseNotEqual)item).TrimPosition = trimPosition;
                    ((WhereClauseNotEqual)item).CaseSensitive = caseSensitive;
                    ((WhereClauseNotEqual)item).NormalizeChemicalName = normalizeChemicalName;
                    ((WhereClauseNotEqual)item).DataField = field;
                    ((WhereClauseNotEqual)item).Val = new Value(value, DbType.String);
                    break;

                default:
                    throw new SQLGeneratorException(string.Format("{0} - Operator not supported: {1}", "TextCriteria", operation));
                    break;
            }

            return item;
        }

        private static WhereClauseItem ParseDateCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseItem item = null;

            string operation = XmlTranslation.GetOperation(operationNode, WhereClauseTypes.DateCriteria);
            string value = operationNode.InnerText;
            CultureInfo culture = new CultureInfo("en-US");
            if (operationNode.Attributes["culture"] != null && !string.IsNullOrEmpty(operationNode.Attributes["culture"].Value))
            {
                culture = new CultureInfo(operationNode.Attributes["culture"].Value);
            }

            DateTime date = DateTime.MinValue;

            switch (operation)
            {
                case "gt":
                    item = new WhereClauseGreaterThan();
                    date = DateTime.SpecifyKind(DateTime.Parse(value, culture), DateTimeKind.Utc);
                    DateTime gtDateTime = date.Date.AddDays(1).AddSeconds(-1); //Set the time as 23:59:59
                    ((WhereClauseGreaterThan)item).Val = new Value(gtDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                    ((WhereClauseGreaterThan)item).GreaterEqual = false;
                    ((WhereClauseGreaterThan)item).TrimPosition = trimPosition;
                    ((WhereClauseGreaterThan)item).DataField = field;
                    ((WhereClauseGreaterThan)item).Negate = negate;
                    break;
                case "gte":
                    item = new WhereClauseGreaterThan();
                    date = DateTime.SpecifyKind(DateTime.Parse(value, culture), DateTimeKind.Utc);
                    DateTime gteDateTime = date.Date;
                    ((WhereClauseGreaterThan)item).Val = new Value(gteDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                    ((WhereClauseGreaterThan)item).GreaterEqual = true;
                    ((WhereClauseGreaterThan)item).TrimPosition = trimPosition;
                    ((WhereClauseGreaterThan)item).DataField = field;
                    ((WhereClauseGreaterThan)item).Negate = negate;
                    break;
                case "equal":
                    try
                    {
                        item = new WhereClauseBetween();
                        date = DateTime.SpecifyKind(DateTime.Parse(value, culture), DateTimeKind.Utc);

                        DateTime startDateTime = date.Date;
                        DateTime endDateTime = date.Date.AddDays(1).AddSeconds(-1);

                        ((WhereClauseBetween)item).DataField = field;
                        ((WhereClauseBetween)item).Negate = negate;
                        ((WhereClauseBetween)item).Values = new Value[2];
                        ((WhereClauseBetween)item).Values[0] = new Value(startDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                        ((WhereClauseBetween)item).Values[1] = new Value(endDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                    }
                    catch (FormatException)
                    {
                        item = new WhereClauseBetween();
                        ((WhereClauseBetween)item).DataField = field;
                        ((WhereClauseBetween)item).Negate = negate;

                        string[] arrayValue = value.Split('-');
                        if (arrayValue.Length == 2)
                        {
                            DateTime startDateTime = DateTime.SpecifyKind(DateTime.Parse(arrayValue[0], culture), DateTimeKind.Utc).Date;
                            DateTime endDateTime = DateTime.SpecifyKind(DateTime.Parse(arrayValue[1], culture), DateTimeKind.Utc).Date.AddDays(1).AddSeconds(-1);
                            Value[] ranges = new Value[2];
                            ranges[0] = new Value(startDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                            ranges[1] = new Value(endDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                            ((WhereClauseBetween)item).Values = ranges;
                        }
                        else
                        {
                            Exception rangeException = new Exception("The range " + value.ToString() + " is invalid in SearchCriteria.");
                            throw rangeException;
                        }
                    }
                    break;
                case "lt":
                    item = new WhereClauseLessThan();
                    date = DateTime.SpecifyKind(DateTime.Parse(value, culture), DateTimeKind.Utc);
                    DateTime ltDateTime = date.Date;
                    ((WhereClauseLessThan)item).Val = new Value(ltDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                    ((WhereClauseLessThan)item).LessEqual = false;
                    ((WhereClauseLessThan)item).TrimPosition = trimPosition;
                    ((WhereClauseLessThan)item).DataField = field;
                    ((WhereClauseLessThan)item).Negate = negate;
                    break;
                case "lte":
                    item = new WhereClauseLessThan();
                    date = DateTime.SpecifyKind(DateTime.Parse(value, culture), DateTimeKind.Utc);
                    DateTime lteDateTime = date.Date.AddDays(1).AddSeconds(-1);
                    ((WhereClauseLessThan)item).Val = new Value(lteDateTime.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                    ((WhereClauseLessThan)item).LessEqual = true;
                    ((WhereClauseLessThan)item).TrimPosition = trimPosition;
                    ((WhereClauseLessThan)item).DataField = field;
                    ((WhereClauseLessThan)item).Negate = negate;
                    break;
                case "in":
                    item = new WhereClauseIn();
                    string[] vals = value.Split(',');
                    Value[] values = new Value[vals.Length];
                    for (int i = 0; i < vals.Length; i++)
                    {
                        date = DateTime.SpecifyKind(DateTime.Parse(vals[i].Trim(), culture), DateTimeKind.Utc).Date;
                        values[i] = new Value(date.ToUniversalTime().ToString(Constants.DatesFormat), DbType.String);
                    }
                    ((WhereClauseIn)item).Values = values;
                    ((WhereClauseIn)item).TrimPosition = trimPosition;
                    ((WhereClauseIn)item).DataField = field;
                    ((WhereClauseIn)item).Negate = negate;
                    break;
                default:
                    throw new SQLGeneratorException(string.Format("{0} - Operator not supported: {1}", "DateCriteria", operation));
                    break;
            }
            return item;
        }

        private static WhereClauseItem ParseNumericalCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseItem item = null;

            string operation = XmlTranslation.GetOperation(operationNode, WhereClauseTypes.NumericalCriteria);
            string value = operationNode.InnerText;

            switch (operation)
            {
                case "gt":
                    item = new WhereClauseGreaterThan();
                    ((WhereClauseGreaterThan)item).DataField = field;
                    ((WhereClauseGreaterThan)item).Val = new Value();
                    // Remove group separators from numerical value
                    ((WhereClauseGreaterThan)item).Val.Val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty);
                    //((WhereClauseGreaterThan) item).Val.Val = value;
                    try
                    {
                        int intValue = int.Parse(value);
                        ((WhereClauseGreaterThan)item).Val.Type = DbType.Int32;
                    }
                    catch (FormatException)
                    {
                        try
                        {
                            double doubleValue = double.Parse(value);
                            ((WhereClauseGreaterThan)item).Val.Type = DbType.Decimal;
                        }
                        catch (Exception e)
                        {

                            throw new COEDecimalFormatConversionException(value);
                        }
                    }
                    ((WhereClauseGreaterThan)item).GreaterEqual = false;
                    ((WhereClauseGreaterThan)item).TrimPosition = trimPosition;
                    ((WhereClauseGreaterThan)item).Negate = negate;
                    break;
                case "gte":
                    item = new WhereClauseGreaterThan();
                    ((WhereClauseGreaterThan)item).DataField = field;
                    ((WhereClauseGreaterThan)item).Val = new Value();
                    // Remove group separators from numerical value
                    ((WhereClauseGreaterThan)item).Val.Val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty);
                    try
                    {
                        int intValue = int.Parse(value);
                        ((WhereClauseGreaterThan)item).Val.Type = DbType.Int32;
                    }
                    catch (FormatException)
                    {
                        try
                        {
                            double doubleValue = double.Parse(value);
                            ((WhereClauseGreaterThan)item).Val.Type = DbType.Decimal;
                        }
                        catch (Exception e)
                        {
                            throw new COEDecimalFormatConversionException(value);
                        }
                    }
                    ((WhereClauseGreaterThan)item).GreaterEqual = true;
                    ((WhereClauseGreaterThan)item).TrimPosition = trimPosition;
                    ((WhereClauseGreaterThan)item).Negate = negate;
                    break;
                case "equal":
                    item = new WhereClauseEqual();
                    ((WhereClauseEqual)item).DataField = field;
                    ((WhereClauseEqual)item).Val = new Value();
                    ((WhereClauseEqual)item).CaseSensitive = caseSensitive;
                    ((WhereClauseEqual)item).TrimPosition = trimPosition;
                    ((WhereClauseEqual)item).Negate = negate;
                    try
                    {
                        ((WhereClauseEqual)item).Val.Val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty);
                        if (value.ToLower() != "null" && value.ToLower() != "not null")
                        {
                            int intValue = int.Parse(value);
                            ((WhereClauseEqual)item).Val.Type = DbType.Int32;
                        }
                    }
                    catch (FormatException)
                    {
                        try
                        {
                            double doubleValue = double.Parse(value);
                            ((WhereClauseEqual)item).Val.Type = DbType.Decimal;
                        }
                        catch (FormatException)
                        {
                            item = new WhereClauseBetween();
                            ((WhereClauseBetween)item).DataField = field;

                            value = value.Replace(" ", "");
                            if (value.IndexOf("--") > 0)
                                value = value.Replace("--", "/-");
                            else
                            {
                                if (value.StartsWith("-"))
                                {
                                    value = value.Trim('-').Replace('-', '/');
                                    value = '-' + value;
                                }
                                else
                                    value = value.Trim('-').Replace('-', '/');
                            }
                            string[] arrayValue = value.Split('/');

                            if (arrayValue.Length == 2)
                            {
                                Value[] ranges = new Value[arrayValue.Length];
                                for (int i = 0; i < arrayValue.Length; i++)
                                {
                                    ranges[i] = new Value();
                                    ranges[i].Val = arrayValue[i];
                                    try
                                    {
                                        int intValue = int.Parse(arrayValue[i]);
                                        ranges[i].Type = DbType.Int32;
                                    }
                                    catch (FormatException)
                                    {
                                        try
                                        {
                                            double doubleValue = double.Parse(arrayValue[i]);
                                            ranges[i].Type = DbType.Decimal;
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new COEDecimalFormatConversionException(arrayValue[i]);
                                        }
                                    }
                                }
                                ((WhereClauseBetween)item).Values = ranges;
                                ((WhereClauseBetween)item).TrimPosition = trimPosition;
                                ((WhereClauseBetween)item).Negate = negate;
                            }
                            else
                            {
                                //Exception rangeException = new Exception("The range " + value.ToString() + " is invalid in SearchCriteria.");
                                //throw rangeException;
                                throw new COEDecimalFormatConversionException(value);
                            }

                        }
                    }
                    break;
                case "lt":
                    item = new WhereClauseLessThan();
                    ((WhereClauseLessThan)item).DataField = field;
                    ((WhereClauseLessThan)item).Val = new Value();
                    ((WhereClauseLessThan)item).Val.Val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty);
                    ((WhereClauseLessThan)item).TrimPosition = trimPosition;
                    try
                    {
                        int intValue = int.Parse(value);
                        ((WhereClauseLessThan)item).Val.Type = DbType.Int32;
                    }
                    catch (FormatException)
                    {
                        try
                        {
                            double doubleValue = double.Parse(value);
                            ((WhereClauseLessThan)item).Val.Type = DbType.Decimal;
                        }
                        catch (Exception e)
                        {
                            throw new COEDecimalFormatConversionException(value);
                        }
                    }
                    ((WhereClauseLessThan)item).LessEqual = false;
                    ((WhereClauseLessThan)item).Negate = negate;
                    break;
                case "lte":
                    item = new WhereClauseLessThan();
                    ((WhereClauseLessThan)item).DataField = field;
                    ((WhereClauseLessThan)item).Val = new Value();
                    ((WhereClauseLessThan)item).Val.Val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty);
                    ((WhereClauseLessThan)item).TrimPosition = trimPosition;
                    try
                    {
                        int intValue = int.Parse(value);
                        ((WhereClauseLessThan)item).Val.Type = DbType.Int32;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            double doubleValue = double.Parse(value);
                            ((WhereClauseLessThan)item).Val.Type = DbType.Decimal;
                        }
                        catch (FormatException e)
                        {
                            throw new COEDecimalFormatConversionException(value);
                        }
                    }
                    ((WhereClauseLessThan)item).LessEqual = true;
                    ((WhereClauseLessThan)item).Negate = negate;
                    break;
                case "in":
                    try
                    {
                        value = Utilities.NumExpressionToString(value, field.FieldType == DbType.Int32);
                    }
                    catch (FormatException ex)
                    {
                        throw new COENumericFormatConversionException(value + ":" + field.FieldName);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    string[] vals_ = value.Split(',');
                    item = new WhereClauseIn();
                    ((WhereClauseIn)item).DataField = field;
                    Value[] values_ = new Value[vals_.Length];
                    for (int i = 0; i < vals_.Length; i++)
                    {
                        values_[i] = new Value();
                        values_[i].Val = vals_[i];
                        try
                        {
                            int intValue = int.Parse(vals_[i]);
                            values_[i].Type = DbType.Int32;
                        }
                        catch (FormatException)
                        {
                            try
                            {
                                double doubleValue = double.Parse(vals_[i]);
                                values_[i].Type = DbType.Decimal;
                            }
                            catch (Exception e)
                            {
                                throw new COEDecimalFormatConversionException(vals_[i]);
                            }

                        }
                    }
                    ((WhereClauseIn)item).Values = values_;
                    ((WhereClauseIn)item).TrimPosition = trimPosition;
                    ((WhereClauseIn)item).Negate = negate;
                    break;
                case "notequal":
                    item = new WhereClauseNotEqual();
                    ((WhereClauseNotEqual)item).DataField = field;
                    ((WhereClauseNotEqual)item).Val = new Value();
                    ((WhereClauseNotEqual)item).Val.Val = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty);
                    ((WhereClauseNotEqual)item).CaseSensitive = caseSensitive;
                    try
                    {
                        int intValue = int.Parse(value);
                        ((WhereClauseNotEqual)item).Val.Type = DbType.Int32;
                    }
                    catch (FormatException)
                    {
                        try
                        {
                            double doubleValue = double.Parse(value);
                            ((WhereClauseNotEqual)item).Val.Type = DbType.Decimal;
                        }
                        catch (Exception e)
                        {
                            throw new COEDecimalFormatConversionException(value);
                        }
                    }
                    ((WhereClauseNotEqual)item).TrimPosition = trimPosition;
                    ((WhereClauseNotEqual)item).Negate = negate;
                    break;
                case "like":
                    item = new WhereClauseLike();
                    if (operationNode.Attributes["defaultWildCardPosition"] != null)
                        ((WhereClauseLike)item).WildCardPosition = COEConvert.ToPositions(operationNode.Attributes["defaultWildCardPosition"].Value);
                    ((WhereClauseLike)item).DataField = field;
                    ((WhereClauseLike)item).Val = new Value(value, DbType.String);
                    break;
                default:
                    throw new SQLGeneratorException(string.Format("{0} - Operator not supported: {1}", "NumericalCriteria", operation));
                    break;
            }

            return item;
        }

        private static WhereClauseVerbatim ParseVerbatimCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseVerbatim item = new WhereClauseVerbatim();
            foreach (XmlNode currentNode in operationNode.ChildNodes)
            {
                if (currentNode.Name.Trim().ToLower() == "verbatim")
                {
                    item.Verbatim = currentNode.InnerText;
                }
                else if (currentNode.Name.Trim().ToLower() == "parameter")
                {
                    Value parameter = new Value();

                    double doubleValue = 0.0;
                    int intValue = 0;
                    if (double.TryParse(currentNode.InnerText, out doubleValue))
                        parameter.Type = DbType.Double;
                    else if (int.TryParse(currentNode.InnerText, out intValue))
                        parameter.Type = DbType.Int32;
                    else
                        parameter.Type = DbType.String;

                    parameter.Val = currentNode.InnerText;
                    item.Parameters.Add(parameter);
                }
            }
            return item;
        }

        private static WhereClauseStructure ParseStructureCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseStructure item = new WhereClauseStructure();
            XmlNode implementationNode = operationNode.FirstChild;

            switch (implementationNode.Name.ToLower())
            {
                case "cscartridgestructurecriteria":
                    item.DataField = field;
                    item.Val = new Value(implementationNode.InnerText, DbType.String);
                    //LJB: 2/2/2009 need to change this to allow the cartridge schema to be altered.  Am hardcodeing to Oracle.dbmstype, since there appears
                    //no way of assertaining what this is and need to be seriously refactored
                    item.CartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
                    //item.CartridgeSchema = "CSCARTRIDGE"; //TODO: Set the schema name of the cartridge in the xml.
                    item.Negate = negate;

                    if (implementationNode.Attributes["absoluteHitsRel"] != null)
                        item.AbsoluteHitsRel = GetBoolFromString(implementationNode.Attributes["absoluteHitsRel"].Value);
                    if (implementationNode.Attributes["relativeTetStereo"] != null)
                        item.RelativeTetStereo = GetBoolFromString(implementationNode.Attributes["relativeTetStereo"].Value);
                    if (implementationNode.Attributes["tetrahedralStereo"] != null)
                        item.TetrahedralStereo = COEConvert.ToTetrahedralStereoMatching(implementationNode.Attributes["tetrahedralStereo"].Value);
                    if (implementationNode.Attributes["simThreshold"] != null)
                        item.SimThreshold = int.Parse(implementationNode.Attributes["simThreshold"].Value);
                    if (implementationNode.Attributes["reactionCenter"] != null)
                        item.ReactionCenter = GetBoolFromString(implementationNode.Attributes["reactionCenter"].Value);
                    if (implementationNode.Attributes["fullSearch"] != null)
                        item.FullSearch = GetBoolFromString(implementationNode.Attributes["fullSearch"].Value);
                    if (implementationNode.Attributes["tautometer"] != null)
                        item.Tautometer = GetBoolFromString(implementationNode.Attributes["tautometer"].Value);
                    if (implementationNode.Attributes["fragmentsOverlap"] != null)
                        item.FragmentsOverlap = GetBoolFromString(implementationNode.Attributes["fragmentsOverlap"].Value);
                    if (implementationNode.Attributes["permitExtraneousFragmentsIfRXN"] != null)
                        item.PermitExtraneousFragmentsIfRXN = GetBoolFromString(implementationNode.Attributes["permitExtraneousFragmentsIfRXN"].Value);
                    if (implementationNode.Attributes["permitExtraneousFragments"] != null)
                        item.PermitExtraneousFragments = GetBoolFromString(implementationNode.Attributes["permitExtraneousFragments"].Value);
                    if (implementationNode.Attributes["doubleBondStereo"] != null)
                        item.DoubleBondStereo = GetBoolFromString(implementationNode.Attributes["doubleBondStereo"].Value);
                    if (implementationNode.Attributes["hitAnyChargeHetero"] != null)
                        item.HitAnyChargeHetero = GetBoolFromString(implementationNode.Attributes["hitAnyChargeHetero"].Value);
                    if (implementationNode.Attributes["identity"] != null)
                        item.Identity = GetBoolFromString(implementationNode.Attributes["identity"].Value);
                    if (implementationNode.Attributes["hitAnyChargeCarbon"] != null)
                        item.HitAnyChargeCarbon = GetBoolFromString(implementationNode.Attributes["hitAnyChargeCarbon"].Value);
                    if (implementationNode.Attributes["similar"] != null)
                        item.Similar = GetBoolFromString(implementationNode.Attributes["similar"].Value);
                    if (implementationNode.Attributes["highlight"] != null)
                        item.Highlight = GetBoolFromString(implementationNode.Attributes["highlight"].Value);
                    if (implementationNode.Attributes["ignoreImplicitHydrogens"] != null)
                        item.IgnoreImplicitHydrogens = GetBoolFromString(implementationNode.Attributes["ignoreImplicitHydrogens"].Value);
                    if (implementationNode.Attributes["cartridgeParams"] != null)
                        item.CartridgeParams = implementationNode.Attributes["cartridgeParams"].Value.Trim();
                    if (implementationNode.Attributes["format"] != null)
                        item.Format = implementationNode.Attributes["format"].Value.Trim();
                    break;
                default:
                    throw new SQLGeneratorException(Resources.UnimplementedCriteria);
            }

            return item;
        }

        private static WhereClauseItem ParseStructureListCriteriaOperation(Field field, XmlNode operationNode, bool negate)
        {
            WhereClauseStructureList item = new WhereClauseStructureList();
            item.DataField = field;
            if (!string.IsNullOrEmpty(operationNode.InnerText))
                item.Molecules = ParsingUtilities.ParseList(new MemoryStream(UnicodeEncoding.ASCII.GetBytes(operationNode.InnerText)), "sdf").ToArray();

            item.Negate = negate;
            return item;
        }

        private static WhereClauseDirectFlexmatch ParseDirectFlexMatchCriteriaOperation(Field field, XmlNode operationNode,
            bool negate)
        {
            var item = new WhereClauseDirectFlexmatch();

            if (operationNode.Attributes["flexmatchparameters"] != null)
            {
                item.FlexmatchParameters = operationNode.Attributes["flexmatchparameters"].Value;
            }

            if (operationNode.Attributes["flexmatchnumber"] != null)
            {
                item.FlexmatchNumber = operationNode.Attributes["flexmatchnumber"].Value;
            }

            item.DataField = field;
            item.Val = new Value(operationNode.InnerText, DbType.String);
            item.Negate = negate;
            return item;
        }

        private static WhereClauseDirectSimilar ParseDirectSimilarCriteriaOperation(Field field, XmlNode operationNode,
            bool negate)
        {
            var item = new WhereClauseDirectSimilar();

            if (operationNode.Attributes["similarityvalues"] != null)
            {
                item.Similarityvalues = operationNode.Attributes["similarityvalues"].Value;
            }

            if (operationNode.Attributes["similarnumber"] != null)
            {
                item.SimilarNumber = operationNode.Attributes["similarnumber"].Value;
            }

            item.DataField = field;
            item.Val = new Value(operationNode.InnerText, DbType.String);
            item.Negate = negate;
            return item;
        }

        private static WhereClauseDirectSss ParseDirectSssCriteriaOperation(Field field, XmlNode operationNode,
            bool negate)
        {
            var item = new WhereClauseDirectSss();

            if (operationNode.Attributes["option"] != null)
            {
                item.Option = operationNode.Attributes["option"].Value;
            }

            if (operationNode.Attributes["sssnumber"] != null)
            {
                item.SssNumber = operationNode.Attributes["sssnumber"].Value;
            }

            item.DataField = field;
            item.Val = new Value(operationNode.InnerText, DbType.String);
            item.Negate = negate;
            return item;
        }

        private static WhereClauseJCCompare ParseJChemStructureCriteriaOperation(Field field, XmlNode operationNode, bool negate)
        {
            var item = new WhereClauseJCCompare();

            item.DataField = field;
            item.Val = new Value(operationNode.InnerText, DbType.String);
            item.Negate = negate;

            if (operationNode.Attributes["searchType"] != null)
            {
                item.SearchType = operationNode.Attributes["searchType"].Value.Trim();
            }
            if (operationNode.Attributes["simThreshold"] != null)
            {
                item.SimThreshold = int.Parse(operationNode.Attributes["simThreshold"].Value);
            }

            return item;
        }

        private static WhereClauseMolWeight ParseMolWeightCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseMolWeight item = new WhereClauseMolWeight();
            XmlNode implementationNode = operationNode.FirstChild;

            if (implementationNode == null)
                throw new SQLGeneratorException("Missing 'CSCartridgeMolWeightCriteria' required xml child node");
            double minMass = 0.0;

            if (implementationNode.Attributes["min"] != null)
                double.TryParse(implementationNode.Attributes["min"].Value, out minMass);

            double maxMass = 0.0;

            if (implementationNode.Attributes["max"] != null)
                double.TryParse(implementationNode.Attributes["max"].Value, out maxMass);

            switch (implementationNode.Name.ToLower())
            {
                case "cscartridgemolweightcriteria":
                    //LJB: 2/2/2009 need to change this to allow the cartridge schema to be altered.  Am hardcodeing to Oracle.dbmstype, since there appears
                    //no way of assertaining what this is and need to be seriously refactored
                    item.CartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
                    //item.CartridgeSchema = "CSCARTRIDGE"; //TODO: Set the schema name of the cartridge in the xml.
                    item.DataField = field;
                    item.Val = new Value("0.00", DbType.Decimal);
                    item.MinMass = minMass;
                    item.MaxMass = maxMass;
                    item.Negate = negate;
                    break;
                default:
                    throw new SQLGeneratorException(Resources.UnimplementedCriteria);
            }
            return item;
        }

        private static WhereClauseDirectMolWeight ParseDirectMolWeightCriteriaOperation(Field field,
            XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            var item = new WhereClauseDirectMolWeight {DataField = field, Negate = negate};

            if (operationNode.Attributes["operator"] != null)
            {
                item.Operator = operationNode.Attributes["operator"].Value;
            }

            if (operationNode.Attributes["parameter2"] != null)
            {
                double para2;
                double.TryParse(operationNode.Attributes["parameter2"].Value, out para2);
                item.Val2 = new Value(para2.ToString(CultureInfo.InvariantCulture), DbType.Decimal);
            }

            double para1;
            double.TryParse(operationNode.InnerText, out para1);
            item.Val = new Value(para1.ToString(CultureInfo.InvariantCulture), DbType.Decimal);

            return item;
        }

        private static WhereClauseItem ParseJChemMolWeightCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            var item = new WhereClauseJChemMolWeight { DataField = field, Negate = negate };

            if (operationNode.Attributes["operator"] != null)
            {
                item.Operator = operationNode.Attributes["operator"].Value;
            }

            if (operationNode.Attributes["parameter2"] != null)
            {
                double para2;
                double.TryParse(operationNode.Attributes["parameter2"].Value, out para2);
                item.Val2 = new Value(para2.ToString(CultureInfo.InvariantCulture), DbType.Decimal);
            }

            double para1;
            double.TryParse(operationNode.InnerText, out para1);
            item.Val = new Value(para1.ToString(CultureInfo.InvariantCulture), DbType.Decimal);

            return item;
        }

        private static WhereClauseFormula ParseFormulaCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseFormula item = new WhereClauseFormula();
            XmlNode implementationNode = operationNode.FirstChild;
            string value = implementationNode.InnerText;
            bool full = (implementationNode.Attributes["full"] != null &&
                                   implementationNode.Attributes["full"].Value.ToLower() == "yes") ?
                                   true : false;
            switch (implementationNode.Name.ToLower())
            {
                case "cscartridgeformulacriteria":
                    //TODO: Set the schema name of the cartridge in the xml.
                    if (value.StartsWith("'") || value.StartsWith("\"") || value.StartsWith("="))
                    {
                        full = true;
                        value = value.Remove(0, 1);
                        if (value.EndsWith("'") || value.EndsWith("\""))
                            value = value.Remove(value.Length - 1, 1);
                    }
                    //LJB: 2/2/2009 need to change this to allow the cartridge schema to be altered.  Am hardcodeing to Oracle.dbmstype, since there appears
                    //no way of assertaining what this is and need to be seriously refactored
                    item.CartridgeSchema = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE);
                    //item.CartridgeSchema = "CSCARTRIDGE"; //TODO: Set the schema name of the cartridge in the xml.
                    item.DataField = field;
                    item.Val = new Value(value, DbType.String);
                    item.Full = full;
                    item.Negate = negate;
                    break;
                default:
                    throw new SQLGeneratorException(Resources.UnimplementedCriteria);
            }
            return item;
        }

        private static WhereClauseDirectFormula ParseDirectFormulaCriteriaOperation(Field field,
            XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            var item = new WhereClauseDirectFormula();
            var inner = operationNode.InnerText;
            var full = (operationNode.Attributes["full"] != null &&
                        operationNode.Attributes["full"].Value.ToLower() == "true");

            if (inner.StartsWith("'") || inner.StartsWith("\"") || inner.StartsWith("="))
            {
                full = true;
                inner = inner.Remove(0, 1);
                if (inner.EndsWith("'") || inner.EndsWith("\""))
                {
                    inner = inner.Remove(inner.Length - 1, 1);
                }
            }
            item.DataField = field;
            item.Val = new Value(inner, DbType.String);
            item.Full = full;
            item.Negate = negate;

            return item;
        }

        private static WhereClauseItem ParseJChemFormulaCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            var item = new WhereClauseJChemFormula();
            var inner = operationNode.InnerText;
            var full = (operationNode.Attributes["full"] != null &&
                        operationNode.Attributes["full"].Value.ToLower() == "true");

            if (inner.StartsWith("'") || inner.StartsWith("\"") || inner.StartsWith("="))
            {
                full = true;
                inner = inner.Remove(0, 1);
                if (inner.EndsWith("'") || inner.EndsWith("\""))
                {
                    inner = inner.Remove(inner.Length - 1, 1);
                }
            }
            item.DataField = field;
            item.Val = new Value(inner, DbType.String);
            item.Full = full;
            item.Negate = negate;

            return item;
        }

        private static WhereClauseItem ParseCustomCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            string parserClassName = string.Empty;
            string assemblyFullName = string.Empty;
            WhereClauseItem item = null;

            string xmlNamespace = "COE";
            XmlDocument mappings = new XmlDocument();
            mappings.LoadXml(ConfigurationUtilities.GetMappingsXml());
            XmlNamespaceManager manager = new XmlNamespaceManager(mappings.NameTable);
            manager.AddNamespace(xmlNamespace, "COE.Mappings");

            try
            {
                XmlNode node = mappings.SelectSingleNode("//" + xmlNamespace + ":whereClause[@name='" + operationNode.FirstChild.Name.Trim().ToLower() + "']", manager);

                if (node != null && node.NodeType != XmlNodeType.Comment)
                {
                    IWhereClauseParser parser = null;
                    if (node.Attributes["assemblyName"] != null && node.Attributes["assemblyName"].Value.Trim().ToLower() != "cambridgesoft.coe.core.common.sqlgenerator")
                    {
                        System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(node.Attributes["assemblyName"].Value.Trim());

                        parser = (IWhereClauseParser)assembly.CreateInstance(node.Attributes["parserClassName"].Value.Trim());
                    }
                    else
                    {
                        Type parserClass = Type.GetType(node.Attributes["parserClassName"].Value.Trim());
                        if (parserClass == null)
                            throw new SQLGeneratorException(Resources.ReflectionErrors.Replace("&clauseName", operationNode.Name.Trim()).Replace("&className", node.Attributes["parserClassName"].Value.Trim()));

                        ConstructorInfo parserDefaultConstructor = parserClass.GetConstructor(System.Type.EmptyTypes);
                        if (parserDefaultConstructor == null)
                            throw new SQLGeneratorException(Resources.SelectClauseWithoutDefaultConstructor.Replace("&className", operationNode.Name.Trim()));

                        parser = (IWhereClauseParser)parserDefaultConstructor.Invoke(null);
                    }

                    item = ((IWhereClauseParser)parser).CreateInstance(operationNode, field);

                    return item;
                }
                else
                    throw new SQLGeneratorException(Resources.WhereClauseNotFound + " " + operationNode.FirstChild.Name.Trim());
            }
            catch (System.Xml.XPath.XPathException)
            {
                throw new SQLGeneratorException(Resources.WhereClauseNotSupported + " " + operationNode.FirstChild.Name.Trim());
            }
            catch (InvalidCastException)
            {
                throw new SQLGeneratorException(Resources.WhereParserNotImplemented.Replace("&clauseName", operationNode.FirstChild.Name.Trim()));
            }

            /*            if(operationNode != null && operationNode.NodeType != XmlNodeType.Comment) {
                            if(operationNode.Attributes["parserClassName"] == null || string.IsNullOrEmpty(operationNode.Attributes["parserClassName"].Value))
                                throw new Exception("The custom criteria has no parser class");
                            else
                                parserClassName = operationNode.Attributes["parserClassName"].Value;


                            if(operationNode.Attributes["assemblyFullName"] != null && !string.IsNullOrEmpty(operationNode.Attributes["assemblyFullName"].Value))
                                assemblyFullName = operationNode.Attributes["assemblyFullName"].Value;

                            IWhereClauseParser parser = null;
                            if(!string.IsNullOrEmpty(assemblyFullName)) {
                                System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assemblyFullName);
                                parser = (IWhereClauseParser) assembly.CreateInstance(parserClassName);
                            } else {
                                Type parserType = Type.GetType(parserClassName);
                                if(parserType == null)
                                    throw new Exception();

                                System.Reflection.ConstructorInfo parserDefaultConstructor = parserType.GetConstructor(System.Type.EmptyTypes);
                                if(parserDefaultConstructor == null)
                                    throw new Exception();

                                parser = (IWhereClauseParser) parserDefaultConstructor.Invoke(null);
                            }

                            item = parser.CreateInstance(operationNode, field);
                        }
                        return item;*/
        }

        private static WhereClauseItem ParseFullTextCriteriaOperation(Field field, XmlNode operationNode, bool negate, SearchCriteria.Positions trimPosition, bool caseSensitive)
        {
            WhereClauseItem item = null;

            string operation = XmlTranslation.GetOperation(operationNode, WhereClauseTypes.TextCriteria);
            string value = operationNode.InnerText;

            item = new WhereClauseFullText();
            ((WhereClauseFullText)item).Negate = negate;
            ((WhereClauseFullText)item).DataField = field;
            ((WhereClauseFullText)item).Val = new Value(value, DbType.String);

            return item;
        }

        private static WhereClauseHitlist ParseHitlistCriteriaOperation(Field field, XmlNode operationNode, bool negate, CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.DataView dataView)
        {
            WhereClauseHitlist item = new WhereClauseHitlist();

            item.DataField = field;
            item.Val = new Value(operationNode.InnerText, DbType.String);
            item.Negate = negate;

            if (operationNode.Attributes["hitlistType"] != null)
                item.HitlistType = (HitListType)Enum.Parse(typeof(HitListType), operationNode.Attributes["hitlistType"].Value);
            if (operationNode.Attributes["sourceJoiningFieldStr"] != null && !string.IsNullOrEmpty(operationNode.Attributes["sourceJoiningFieldStr"].Value))
            {
                item.SourceJoiningFieldStr = operationNode.Attributes["sourceJoiningFieldStr"].Value;
            }
            return item;
        }

        private static bool GetBoolFromString(string value)
        {
            if (value.ToUpper() == "YES")
                return true;
            else
                return false;
        }
        #endregion
    }
}
