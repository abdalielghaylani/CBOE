CREATE OR REPLACE  PACKAGE "&&SchemaName"."REPORTPARAMS" 
IS     
   FUNCTION INSERTPARAM
      (pReportID IN inv_ReportParams.Report_ID%Type,
	     pParamDisplayName IN inv_ReportParams.ParamDisplayName%Type,
	     pParamName IN inv_ReportParams.ParamName%Type,
	     pParamType IN inv_ReportParams.ParamType%Type,
	     pIsRequired IN inv_ReportParams.IsRequired%Type
	     )
	     RETURN inv_ReportParams.ParamName%Type;

   FUNCTION DELETEPARAM
      (pReportID IN inv_ReportParams.Report_ID%Type
	     )
	     RETURN inv_ReportParams.Report_ID%Type;

END REPORTPARAMS;
/
show errors;
