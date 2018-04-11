using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CambridgeSoft.COE.Framework.Common;
using System.Data;
using System.Xml;
using System.ComponentModel;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace CambridgeSoft.COE.Framework.COEExportService
{
   
    internal class XML : FormatterBase,IFormatterAdvanced
    {
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEAdvancedExport");

        public string FormatData(DataSet dataSet,COEDataView dataView, SearchCriteria searchCriteria, ResultsCriteria resultCriteria, PagingInfo pageInfo, string userName, string SARSheetName)
        {
            string criteriaFld = String.Empty;

            COESearchService.SearchInput searchInput = new CambridgeSoft.COE.Framework.COESearchService.SearchInput();    

            StringBuilder SARSheetXML = new StringBuilder();
            SARSheetXML.Append("<SARSheetImport>\n");
            //For the time being the username and password has same
             SARSheetXML.Append("<UserInfo username=\"" + userName + "\" password=\"" + userName +"\" />\n");
            SARSheetXML.Append("<DataviewInfo dataviewid=\"" + dataView.DataViewID + "\" />\n");
            SARSheetXML.Append("<searchInput>\n");
            SARSheetXML.Append("<FieldCriteria>\n");

            foreach (SearchCriteria.SearchCriteriaItem searchCriteriaItem in searchCriteria.Items)
            {
                if (!String.IsNullOrEmpty(searchCriteriaItem.Criterium.Value))
                {
                    for (int tab = 0; tab < dataView.Tables.Count; tab++)
                    {
                        if (dataView.Tables[tab].Id == searchCriteriaItem.TableId)
                        {
                            for (int fld = 0; fld < dataView.Tables[tab].Fields.Count; fld++)
                            {
                                if (dataView.Tables[tab].Fields[fld].Id == searchCriteriaItem.FieldId)
                                {
                                    //SARSheetXML.Append("<string>" + dataView.Tables[tab].Database + "." + dataView.Tables[tab].Alias + "."+ dataView.Tables[tab].Fields[fld].Alias + searchCriteriaItem.Criterium.Value + "</string>\n");

                                    // Fix Coverity: CID-28980 Never use ab varible, comment it
                                    ////System.Web.UI.HtmlControls.HtmlGenericControl ab = new System.Web.UI.HtmlControls.HtmlGenericControl(">");
                                    
                                    if (searchCriteriaItem.Criterium.GenerateXmlSnippet().Contains("CSCartridgeFormulaCriteria"))
                                        criteriaFld = "FORMULA";

                                    else if (searchCriteriaItem.Criterium.GenerateXmlSnippet().Contains("CSCartridgeMolWeightCriteria"))
                                        criteriaFld = "MOLWEIGHT";


                                    SARSheetXML.Append("<string>" + dataView.Tables[tab].Database + "." + dataView.Tables[tab].Name + "." + dataView.Tables[tab].Fields[fld].Name + " " + criteriaFld +" " + parsetext(searchCriteriaItem.Criterium.Value) + "</string>\n");
                                    ////// Coverity Fix CID - 10531 (from local server)
                                    ////ab.Dispose();
                                }
                            }
                        }
                    }
                }                
            }
            SARSheetXML.Append("</FieldCriteria>\n");
            SARSheetXML.Append("<Domain>");

            
            //foreach (string domain in searchInput.Domain)
            //{
            //    SARSheetXML.Append("<string>" + domain + "<string>\n");
            //}

            //foreach (SearchCriteria.DomainCriteria domainCriteria in searchCriteria.Items)
            //{
            //    SARSheetXML.Append("<string>" + domainCriteria.Value + "<string>\n");
            //}

            SARSheetXML.Append("</Domain>\n");


            /*SARSheetXML.Append("<DomainFieldName>" + searchInput.DomainFieldName + "</DomainFieldName>\n");
            SARSheetXML.Append("<ReturnPartialResults>" + searchInput.ReturnPartialResults + "</ReturnPartialResults>");
            SARSheetXML.Append("<ReturnPartialResults>" + searchInput.ReturnSimilarityScores + "</ReturnPartialResults>");
            SARSheetXML.Append("<SearchOptions>");
            foreach (string option in searchInput.SearchOptions)
            {
                SARSheetXML.Append("<string>" + option + "<string>\n");
            }
            SARSheetXML.Append("</SearchOptions>\n");

            SARSheetXML.Append("<AvoidHitList>" + searchInput.AvoidHitList+"</AvoidHitList>"); */
            SARSheetXML.Append("</searchInput>\n");

            SARSheetXML.Append("<resultFields>");

            /*
            for (int tab = 0; tab < resultCriteria.Tables.Count; tab++)
            {

                for (int fld = 0; fld < resultCriteria.Tables[tab].Criterias.Count; fld++)
                {
                   // SARSheetXML.Append("<string>" + resultCriteria.Tables[tab].Criterias[fld].Alias + "</string>\n");                   
                }
            }*/

            //Binding result fields
            foreach (ResultsCriteria.ResultsCriteriaTable resultCriteriaTable in resultCriteria.Tables)
            {               
                    for (int tab = 0; tab < dataView.Tables.Count; tab++)
                    {
                        if (dataView.Tables[tab].Id == resultCriteriaTable.Id)
                        {                          
                           /*for (int fld = 0; fld < resultCriteriaTable.Criterias.Count; fld++)
                            {                                
                                
                            SARSheetXML.Append("<string>" + dataView.Tables[tab].Database + "." + dataView.Tables[tab].Alias + "." + resultCriteriaTable.Criterias[fld].Alias + "</string>\n");  
                            }*/

                            for (int fld = 0; fld < dataView.Tables[tab].Fields.Count; fld++)
                            {
                                SARSheetXML.Append("<string>" + dataView.Tables[tab].Database + "." + dataView.Tables[tab].Name + "." + dataView.Tables[tab].Fields[fld].Name + "." + "</string>\n");
                            }
                        }
                    }                
            }
            SARSheetXML.Append("</resultFields>\n");

            //Binding result information
            SARSheetXML.Append("<resultPageInfo>");
            /*SARSheetXML.Append("<ResultSetID>"+resultPageInfo.ResultSetID+ "</ResultSetID>");
            SARSheetXML.Append("<PageSize>" + resultPageInfo.PageSize + "</PageSize>");;
            SARSheetXML.Append("<Start>"+resultPageInfo.Start+"</Start>");
            SARSheetXML.Append("<End>"+resultPageInfo.End+"</End>");*/

            SARSheetXML.Append("<ResultSetID>" + pageInfo.PagingInfoID + "</ResultSetID>");
            SARSheetXML.Append("<PageSize>" + pageInfo.RecordCount + "</PageSize>"); ;
            SARSheetXML.Append("<Start>" + pageInfo.Start + "</Start>");
            SARSheetXML.Append("<End>" + pageInfo.End + "</End>");
            SARSheetXML.Append("</resultPageInfo>\n");

            SARSheetXML.Append("<sarSheetInfo>");
            SARSheetXML.Append("<sheetname>" + SARSheetName + "</sheetname>");
            SARSheetXML.Append("</sarSheetInfo>\n");
            SARSheetXML.Append("</SARSheetImport>");

            return SARSheetXML.ToString();
        }

        private string parsetext(string text)
        {
                StringBuilder sb = new StringBuilder(text);
                sb.Replace("<", "&lt;");
                sb.Replace(">", "&gt;");               
                sb.Replace("\"", "&quot;");
                return sb.ToString();
        }

    }
  
}

