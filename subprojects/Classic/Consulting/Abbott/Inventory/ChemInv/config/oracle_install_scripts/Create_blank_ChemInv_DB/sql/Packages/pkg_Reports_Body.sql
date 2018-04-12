CREATE OR REPLACE  PACKAGE BODY "&&SchemaName"."REPORTS"           
IS
   FUNCTION CREATEREPORT
      (pReportName IN inv_Reports.ReportName%Type,
       pReportDisplayName IN inv_Reports.ReportDisplayName%Type,
	     pQueryName IN inv_Reports.QueryName%Type,
	     pReportSQL IN inv_Reports.ReportSQL%Type,
	     pReport_Desc IN inv_Reports.Report_Desc%Type,
	     pReportTypeID IN inv_Reports.ReportType_ID%Type
	     )
	   RETURN inv_Reports.ID%Type AS
	   newREPORTID inv_REPORTS.ID%Type;
	 BEGIN
	   INSERT INTO   inv_REPORTS
	                 (ReportName, ReportDisplayName, Report_Desc, QueryName, ReportSQL, ReportType_ID)
	   VALUES        (pReportName, pReportDisplayName, pReport_Desc, pQueryName, pReportSQL, pReportTypeID)
	   RETURNING ID INTO newREPORTID;
	 RETURN newReportID;
	 END CREATEREPORT;

  FUNCTION UPDATEREPORT
      (pID IN inv_Reports.ID%Type,
       pReportName IN inv_Reports.ReportName%Type,
	     pReportDisplayName IN inv_Reports.ReportDisplayName%Type,
	     pReport_Desc IN inv_Reports.Report_Desc%Type,
	     pQueryName IN inv_Reports.QueryName%Type,
	     pReportSQL IN inv_Reports.ReportSQL%Type
	     )
	   RETURN inv_Reports.ID%Type AS
	   newREPORTID inv_REPORTS.ID%Type;
	 BEGIN
	   UPDATE   inv_REPORTS
	   SET   ReportName = pReportName,
	         ReportDisplayName = pReportDisplayName,
	         Report_Desc = pReport_Desc,
	         QueryName = pQueryName,
	         ReportSQL = pReportSQL
	   WHERE ID = pID
	   RETURNING ID INTO newREPORTID;
	 RETURN newReportID;
	 END UPDATEREPORT;

   FUNCTION DELETEREPORT
      (pReportID IN inv_ReportParams.Report_ID%Type
	     )
	     RETURN inv_ReportParams.Report_ID%Type AS
	   oldReportID inv_ReportParams.Report_ID%Type;
	 BEGIN
	   DELETE FROM inv_REPORTS
	   WHERE ID = pReportID;
	 RETURN 1;
	 END DELETEREPORT;
END REPORTS;
/
show errors;

