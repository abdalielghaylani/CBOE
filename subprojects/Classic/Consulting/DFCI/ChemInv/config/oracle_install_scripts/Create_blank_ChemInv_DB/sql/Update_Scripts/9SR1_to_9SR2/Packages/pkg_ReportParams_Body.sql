CREATE OR REPLACE PACKAGE BODY "REPORTPARAMS"
IS
   FUNCTION INSERTPARAM
      (pReportID IN inv_ReportParams.Report_ID%Type,
	     pParamDisplayName IN inv_ReportParams.ParamDisplayName%Type,
	     pParamName IN inv_ReportParams.ParamName%Type,
	     pParamType IN inv_ReportParams.ParamType%Type,
	     pIsRequired IN inv_ReportParams.IsRequired%Type
	     )
	     RETURN inv_ReportParams.ParamName%Type AS
	   newPARAMNAME inv_REPORTPARAMS.PARAMNAME%Type;
	 BEGIN
	   INSERT INTO   inv_REPORTPARAMS
	                 (Report_ID, ParamDisplayName, ParamName, ParamType, IsRequired)
	   VALUES        (pReportID, pParamDisplayName, pParamName, pParamType, pIsRequired)
	   RETURNING ParamName INTO newPARAMNAME;
	 RETURN newPARAMNAME;
	 END INSERTPARAM;

   FUNCTION DELETEPARAM
      (pReportID IN inv_ReportParams.Report_ID%Type
	     )
	     RETURN inv_ReportParams.Report_ID%Type AS
	 BEGIN
	   DELETE FROM inv_REPORTPARAMS
	   WHERE Report_ID = pReportID;
	 RETURN 1;
	 END DELETEPARAM;

	 PROCEDURE GETREPORTPARAMS(
		pReportID IN inv_reports.id%TYPE,
		O_RS OUT CURSOR_TYPE) 
		AS
		BEGIN
		OPEN O_RS FOR
			SELECT	* FROM inv_reportparams WHERE report_id = pReportID;
END GETREPORTPARAMS;			
	 
END REPORTPARAMS;
/
show errors;
