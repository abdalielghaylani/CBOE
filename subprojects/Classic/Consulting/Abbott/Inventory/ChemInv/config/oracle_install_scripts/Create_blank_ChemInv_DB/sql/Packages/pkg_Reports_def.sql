CREATE OR REPLACE  PACKAGE "&&SchemaName"."REPORTS"      
IS

   FUNCTION CREATEREPORT
      (pReportName IN inv_Reports.ReportName%Type,
       pReportDisplayName IN inv_Reports.ReportDisplayName%Type,
	     pQueryName IN inv_Reports.QueryName%Type,
	     pReportSQL IN inv_Reports.ReportSQL%Type,
	     pReport_Desc IN inv_Reports.Report_Desc%Type,
	     pReportTypeID IN inv_Reports.ReportType_ID%Type
	     )
	     RETURN inv_Reports.ID%Type;

   FUNCTION UPDATEREPORT
      (pID IN inv_Reports.ID%Type,
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
	     
END REPORTS;
/
show errors;
