using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.Xml;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COESearchService;

namespace CambridgeSoft.COE.Framework.COEExportService
{

    internal class CBVExcelFormatter : IFormInterchangeFormatter
    {
        /// <summary>
        /// Getting the form details from the client applications and convert it into excel xml 
        /// </summary>
        /// <param name="coedataView">Dataview</param>
        /// <param name="searchCriteria">SearchCriteria fields</param>
        /// <param name="resultCriteria">ResultCriteria fields</param>
        /// <param name="searchInput">SearchInput parameters such as Domain, DomainName etc.</param>
        /// <param name="pagingInfo">Paging details</param>
        /// <param name="serverInfo">Server details, to be displayed in the Login Dialog box of CBVExcel Addin</param>
        /// <param name="userName">Username to be displayed in the Login Dialog box of CBVExcel Addin<</param>
        /// <param name="formName">name to be given to excel sheet<</param>
        /// <returns>string containing xml spreadsheet</returns>
        public string GetForm(COEDataView coedataView, SearchCriteria searchCriteria, ResultsCriteria resultCriteria, PagingInfo pagingInfo, ServerInfo serverInfo, string userName, string formName)
        {
            
            string tier = string.Empty;
            string dataviewId = string.Empty;
            string fieldCriterias = string.Empty;
            string domains = string.Empty;
            string searchOptions = string.Empty;
            string resultFields = string.Empty;
            string criteriaFld = string.Empty;

            System.Text.StringBuilder sbExcelXml = new System.Text.StringBuilder();
            SearchInput searchInput = new SearchInput();
            
            // Setting the default values, if values are not passed           
            if (serverInfo == null)
                serverInfo = new ServerInfo();

            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.RecordCount = 500;
                pagingInfo.HitListID = 0;
                pagingInfo.Start = 1;
                pagingInfo.End = 100001;
            }

            // Server tier //
            tier = serverInfo.Is3TierServer ? COEFormInterchange.tier_3 : COEFormInterchange.tier_2;
            // Server tier //

            // dataview id //
            dataviewId = coedataView == null ? "-1" : coedataView.DataViewID.ToString();
            // dataview id //

            // Field Criterias - Getting the Search Criterias based upon which the searching will be done in CBV Excel //
            foreach (SearchCriteria.SearchCriteriaItem searchCriteriaItem in searchCriteria.Items)
            {
                if (!String.IsNullOrEmpty(searchCriteriaItem.Criterium.Value))
                {
                    string searchCriteriaItemType = searchCriteriaItem.Criterium.GetType().Name;
                    string coeOperator = string.Empty;
                    

                    switch (searchCriteriaItemType.ToLower())
                    {
                        case "textcriteria":
                            if (((SearchCriteria.TextCriteria)(searchCriteriaItem.Criterium)).Operator.ToString().Equals(SearchCriteria.COEOperators.LIKE.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                coeOperator = SearchCriteria.COEOperators.EQUAL.ToString();
                            }
                            else
                            {
                                coeOperator = ((SearchCriteria.TextCriteria)(searchCriteriaItem.Criterium)).Operator.ToString();
                            }
                            criteriaFld = string.Empty;
                            break;
                        case "numericalcriteria":
                            coeOperator = ((SearchCriteria.NumericalCriteria)(searchCriteriaItem.Criterium)).Operator.ToString();
                            criteriaFld = string.Empty;
                            break;
                        case "datecriteria":
                            coeOperator = ((SearchCriteria.DateCriteria)(searchCriteriaItem.Criterium)).Operator.ToString();
                            criteriaFld = string.Empty;
                            break;
                        case "structurecriteria":
                            coeOperator = string.Empty;

                            if (((SearchCriteria.StructureCriteria)(searchCriteriaItem.Criterium)).Identity == SearchCriteria.COEBoolean.Yes)
                            {
                                criteriaFld = COEFormInterchange.criteriaFields.IDENTITY.ToString();
                            }
                            else if (((SearchCriteria.StructureCriteria)(searchCriteriaItem.Criterium)).FullSearch == SearchCriteria.COEBoolean.Yes)
                            {
                                criteriaFld = COEFormInterchange.criteriaFields.FULL.ToString();
                            }
                            else if (((SearchCriteria.StructureCriteria)(searchCriteriaItem.Criterium)).Similar == SearchCriteria.COEBoolean.Yes)
                            {
                                criteriaFld = COEFormInterchange.criteriaFields.SIMILARITY.ToString();
                            }
                            else
                            {
                                criteriaFld = COEFormInterchange.criteriaFields.SUBSTRUCTURE.ToString();
                            }
                            break;
                        case "molweightcriteria":                            
                            coeOperator = string.Empty;
                            criteriaFld = COEFormInterchange.criteriaFields.MOLWEIGHT.ToString();
                            break;
                        case "formulacriteria":                            
                            coeOperator = string.Empty;
                            criteriaFld = COEFormInterchange.criteriaFields.FORMULA.ToString();
                            break;
                        default:
                            coeOperator = string.Empty;
                            criteriaFld = string.Empty;
                            break;
                    }

                    if (coedataView != null)
                    {
                        for (int cnt = 0; cnt < coedataView.Tables.Count; cnt++)
                        {                          

                            if (coedataView.Tables[cnt].Id == searchCriteriaItem.TableId)
                            {
                                for (int fld = 0; fld < coedataView.Tables[cnt].Fields.Count; fld++)
                                {
                                    if (coedataView.Tables[cnt].Fields[fld].Id == searchCriteriaItem.FieldId)
                                    {                                       
                                        if (fieldCriterias == string.Empty)
                                        {
                                            fieldCriterias = coedataView.Tables[cnt].Database + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Alias + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Fields[fld].Alias + COEFormInterchange.delimiter + criteriaFld + COEFormInterchange.delimiter + coeOperator + COEFormInterchange.delimiter + searchCriteriaItem.Criterium.Value;
                                        }
                                        else
                                        {
                                            fieldCriterias += COEFormInterchange.separator + coedataView.Tables[cnt].Database + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Alias + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Fields[fld].Alias + COEFormInterchange.delimiter + criteriaFld + COEFormInterchange.delimiter + coeOperator + COEFormInterchange.delimiter + searchCriteriaItem.Criterium.Value;
                                        }
                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
            // Field Criterias //


            // Domains //
            foreach (string domain in searchInput.Domain)
            {
                if (domains == string.Empty)
                {
                    domains = ParseText(domain);
                }
                else
                {
                    domains += "," + ParseText(domain);
                }
            }
            // Domains //

            
            // Search Options //
            foreach (string option in searchInput.SearchOptions)
            {
                if (searchOptions == string.Empty)
                {
                    searchOptions = option;
                }
                else
                {
                    searchOptions += "," + option;
                }
            }
            // Search Options //

            // Result Fields i.e fields to be displayed in the CBV Excel sheet //

            foreach (ResultsCriteria.ResultsCriteriaTable resultCriteriaTable in resultCriteria.Tables)
            {
                if (coedataView != null)
                {
                    for (int cnt = 0; cnt < coedataView.Tables.Count; cnt++)
                    {
                        if (coedataView.Tables[cnt].Id == resultCriteriaTable.Id)
                        {
                            foreach (ResultsCriteria.IResultsCriteriaBase criteria in resultCriteriaTable.Criterias)
                            {
                                int fieldId = -1;
                                string columnPrefix = string.Empty;

                                if (criteria.GetType().Name.Equals("field", StringComparison.OrdinalIgnoreCase))
                                {
                                    fieldId = ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.Field)(criteria)).Id;
                                }
                                // CBV Excel Export. It's same as field.  The "highlightedstructure" term is used in CVBN for structure column. 
                                else if (criteria.GetType().Name.Equals("highlightedstructure", StringComparison.OrdinalIgnoreCase))
                                {
                                    fieldId = ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.HighlightedStructure)(criteria)).Id;
                                }
                                else if (criteria.GetType().Name.Equals("SqlFunction", StringComparison.OrdinalIgnoreCase))
                                {
                                    for (int i = 0; i < ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.SQLFunction)(criteria)).Parameters.Count; i++)
                                    {
                                        object parameter = (object)((CambridgeSoft.COE.Framework.Common.ResultsCriteria.SQLFunction)(criteria)).Parameters[i];

                                        if (parameter.GetType().Name.Equals("Formula", StringComparison.OrdinalIgnoreCase))
                                        {
                                            fieldId = ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.Formula)(parameter)).Id;
                                            columnPrefix = "FORMULA";
                                            break;
                                        }
                                        else if (parameter.GetType().Name.Equals("MolWeight", StringComparison.OrdinalIgnoreCase))
                                        {
                                            fieldId = ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.MolWeight)(parameter)).Id;
                                            columnPrefix = "MOLWEIGHT";
                                            break;
                                        }
                                    }
                                }
                                else if (criteria.GetType().Name.Equals("Formula", StringComparison.OrdinalIgnoreCase))
                                {
                                    fieldId = ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.Formula)(criteria)).Id;
                                    columnPrefix = "FORMULA";
                                }
                                else if (criteria.GetType().Name.Equals("MolWeight", StringComparison.OrdinalIgnoreCase))
                                {
                                    fieldId = ((CambridgeSoft.COE.Framework.Common.ResultsCriteria.MolWeight)(criteria)).Id;
                                    columnPrefix = "MOLWEIGHT";
                                }

                                for (int fld = 0; fld < coedataView.Tables[cnt].Fields.Count; fld++)
                                {
                                    if (coedataView.Tables[cnt].Fields[fld].Id == fieldId)
                                    {
                                        if (resultFields == string.Empty)
                                        {
                                            if (string.IsNullOrEmpty(columnPrefix))
                                            {
                                                resultFields = coedataView.Tables[cnt].Database + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Alias + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Fields[fld].Alias;
                                            }
                                            else
                                            {
                                                resultFields = coedataView.Tables[cnt].Database + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Alias + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Fields[fld].Alias + COEFormInterchange.fieldDelimiter + columnPrefix;
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(columnPrefix))
                                            {
                                                resultFields += COEFormInterchange.columnSeparator[0] + coedataView.Tables[cnt].Database + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Alias + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Fields[fld].Alias;
                                            }
                                            else
                                            {
                                                resultFields += COEFormInterchange.columnSeparator[0] + coedataView.Tables[cnt].Database + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Alias + COEFormInterchange.fieldDelimiter + coedataView.Tables[cnt].Fields[fld].Alias + COEFormInterchange.fieldDelimiter + columnPrefix;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            // Result Fileds //

            sbExcelXml.Append(ExcelHeader());

            // Create First Worksheet tag
            sbExcelXml.Append("<Worksheet ss:Name=\"" + formName + "\">");

            // Then Table Tag
            sbExcelXml.Append("<Table>");

            // Header Row
            sbExcelXml.Append("<tr>");

            sbExcelXml.Append("<td>Username</td>");
            sbExcelXml.Append("<td>Servername</td>");
            sbExcelXml.Append("<td>Tier</td>");
            sbExcelXml.Append("<td>SSL</td>");
            sbExcelXml.Append("<td>DataviewID</td>");
            sbExcelXml.Append("<td>FieldCriteria</td>");
            sbExcelXml.Append("<td>Domain</td>");
            sbExcelXml.Append("<td>DomainFieldName</td>");
            sbExcelXml.Append("<td>ReturnSimilarityScores</td>");
            sbExcelXml.Append("<td>SearchOptions</td>");
            sbExcelXml.Append("<td>ResultFields</td>");
            sbExcelXml.Append("<td>ResultSetID</td>");
            sbExcelXml.Append("<td>PageSize</td>");
            sbExcelXml.Append("<td>Start</td>");
            sbExcelXml.Append("<td>End</td>");
            sbExcelXml.Append("<td>Sheetname</td>");
            // Setting Key and corresponding Value in the Exported Excel Sheet to identify that the sheet contains the exported schema details
            sbExcelXml.Append("<td>" + COEFormInterchange.key + "</td>");
            sbExcelXml.Append("<td>HitListQueryType</td>"); //CSBR - 153844

            sbExcelXml.Append("</tr>");

            sbExcelXml.Append("<tr>");
            sbExcelXml.Append("<td>" + userName + "</td>");
            sbExcelXml.Append("<td>" + serverInfo.ServerName + "</td>");
            sbExcelXml.Append("<td>" + tier + "</td>");
            sbExcelXml.Append("<td>" + serverInfo.IsSSL + "</td>");
            sbExcelXml.Append("<td>" + dataviewId + "</td>");
            if (!string.IsNullOrEmpty(fieldCriterias))
                fieldCriterias = System.Web.HttpUtility.HtmlEncode(fieldCriterias); //Fixed CSBR - 152134
            sbExcelXml.Append("<td>" + fieldCriterias + "</td>");
            sbExcelXml.Append("<td>" + domains + "</td>");
            sbExcelXml.Append("<td>" + searchInput.DomainFieldName + "</td>");
            sbExcelXml.Append("<td>" + searchInput.ReturnSimilarityScores + "</td>");
            sbExcelXml.Append("<td>" + searchOptions + "</td>");
            if (!string.IsNullOrEmpty(resultFields))
                resultFields = System.Web.HttpUtility.HtmlEncode(resultFields);//Fixed CSBR - 152134
            sbExcelXml.Append("<td>" + resultFields + "</td>");
            sbExcelXml.Append("<td>" + pagingInfo.HitListID + "</td>");
            sbExcelXml.Append("<td>" + pagingInfo.RecordCount + "</td>");
            sbExcelXml.Append("<td>" + pagingInfo.Start + "</td>");
            sbExcelXml.Append("<td>" + pagingInfo.End + "</td>");
            sbExcelXml.Append("<td>" + formName + "</td>");
            sbExcelXml.Append("<td>" + COEFormInterchange.value + "</td>");
            sbExcelXml.Append("<td>" + pagingInfo.HitListQueryType + "</td>"); //CSBR - 153844
            sbExcelXml.Append("</tr>");

            sbExcelXml.Append("</Table>");

            sbExcelXml.Append("</Worksheet>");

            // Close the Workbook tag (in Excel header you can see the Workbook tag)
            sbExcelXml.Append("</Workbook>\n");

            return ConvertHTMLToExcelXML(sbExcelXml.ToString());
        }

        #region ExcelHeader

        /// <summary>
        /// Generating xml in the form of Excel Spreadsheet
        /// </summary>
        /// <returns></returns>
        private string ExcelHeader()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<?xml version=\"1.0\"?>\n");
            sb.Append("<?mso-application progid=\"Excel.Sheet\"?>\n");
            sb.Append("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
            sb.Append("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
            sb.Append("xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ");
            sb.Append("xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
            sb.Append("xmlns:html=\"http://www.w3.org/TR/REC-html40\">\n");
            sb.Append("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">\n");
            sb.Append("</ExcelWorkbook>\n");

            return sb.ToString();
        }

        #endregion ExcelHeader

        #region ConvertHTMLToExcelXML

        /// <summary>
        /// Final Filtaration of String Code generated by above code
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        private static string ConvertHTMLToExcelXML(string strHtml)
        {

            // Just to replace TR with Row
            strHtml = strHtml.Replace("<tr>", "<Row ss:AutoFitHeight=\"1\" >\n");
            strHtml = strHtml.Replace("</tr>", "</Row>\n");

            //replace the cell tags
            strHtml = strHtml.Replace("<td>", "<Cell><Data ss:Type=\"String\">");
            strHtml = strHtml.Replace("</td>", "</Data></Cell>\n");

            return strHtml;
        }

        #endregion ConvertHTMLToExcelXML

        #region ParseText

        private string ParseText(string text)
        {
                StringBuilder sb = new StringBuilder(text);
                sb.Replace("<", "&lt;");
                sb.Replace(">", "&gt;");               
                sb.Replace("\"", "&quot;");
                sb.Replace("&", "&amp;");
                return sb.ToString();
            }

        #endregion PareseText

        }
  
}

