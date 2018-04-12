CREATE OR REPLACE PACKAGE "REPORTS"
IS
	TYPE  CURSOR_TYPE IS REF CURSOR;
	
   FUNCTION CREATEREPORT
      (pReportName IN inv_Reports.ReportName%Type,
       pReportDisplayName IN inv_Reports.ReportDisplayName%Type,
	     pQueryName IN inv_Reports.QueryName%Type,
	     pReportSQL IN inv_Reports.ReportSQL%Type,
	     pReport_Desc IN inv_Reports.Report_Desc%Type,
	     pReportTypeID IN inv_Reports.ReportType_ID%Type
	     )
	     RETURN inv_Reports.ID%Type;

   FUNCTION CREATEREPORT_NOSQL
      (pReportName IN inv_Reports.ReportName%Type,
       pReportDisplayName IN inv_Reports.ReportDisplayName%Type,
	     pQueryName IN inv_Reports.QueryName%Type,
	     pReport_Desc IN inv_Reports.Report_Desc%Type,
	     pReportTypeID IN inv_Reports.ReportType_ID%Type
	     )
	     RETURN inv_Reports.ID%Type;			 

   FUNCTION UPDATEREPORT
      (pID IN inv_Reports.ID%Type,
						p_reportTypeID inv_reports.reporttype_id%TYPE,						
       pReportName IN inv_Reports.ReportName%Type,
	     pReportDisplayName IN inv_Reports.ReportDisplayName%Type,
	     pReport_Desc IN inv_Reports.Report_Desc%Type,
	     pQueryName IN inv_Reports.QueryName%Type,
	     pReportSQL IN inv_Reports.ReportSQL%Type
	     )
	     RETURN inv_Reports.ID%Type;

   FUNCTION DELETEREPORT
      (pReportID IN inv_ReportParams.Report_ID%Type
	     )
	     RETURN inv_ReportParams.Report_ID%Type;

PROCEDURE GETREPORT(
		pReportID IN inv_reports.id%TYPE,
		O_RS OUT CURSOR_TYPE) ;			 
END REPORTS;
/
show errors;
