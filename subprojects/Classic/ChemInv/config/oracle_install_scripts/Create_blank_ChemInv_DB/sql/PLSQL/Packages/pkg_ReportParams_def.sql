CREATE OR REPLACE PACKAGE "REPORTPARAMS"
IS
	TYPE  CURSOR_TYPE IS REF CURSOR;

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

			 PROCEDURE GETREPORTPARAMS(
			 		pReportID IN inv_reports.id%TYPE,
							O_RS OUT CURSOR_TYPE) ;			 			 
END REPORTPARAMS;
/
show errors;
