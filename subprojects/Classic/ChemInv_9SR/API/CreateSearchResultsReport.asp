<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:CreateSearchResultsReport<BR>"

'RPT paths
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportArchiveDBPath =  RPTPath & "reportqueuearchive.mdb"		
ReportDBPath = Application("ReportDBPath")
ReportsHTTPPath = Application("ReportsHTTPPath") 

'Required Paramenters
ReportName = Request("ReportName")
HitlistID = Request("HitlistID")

'Optional parameters
ReportFormat = Request("ReportFormat")
ShowInList = Request("ShowInList")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateSearchResultsReport.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(HitlistID) then
	strError = strError & "HitlistID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ReportName) then
	strError = strError & "ReportName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'Check optional parameters
if ReportFormat = "" then
	ReportFormat = "SNP"
End if

If ShowInList = "plates" then
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/PlateSQL.asp"-->
<%
else
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/ContainerSQL.asp"-->
<%
end if

if ShowInList = "plates" then
	SQL = Replace(SQL, " p , ", " p, " & Application("CHEMINV_USERNAME") & ".CSDOHitlist, ")
	SQL = SQL & _ 
			"p.ROWID = " & Application("CHEMINV_USERNAME") & ".CSDOHitlist.ID " & _
			"AND " & _ 
			Application("CHEMINV_USERNAME") & ".CSDOHitlist.HitlistID = " & HitListID
	QueryName = "qryPlateAttributesFromPlateIDHitlistID"
else 
	SQL = Replace(SQL, "FROM " & vblf & "inv_Containers", "FROM " & Application("CHEMINV_USERNAME") & ".CSDOHitlist, inv_Containers")
	SQL = SQL & "AND " & _ 
			"inv_Containers.ROWID = " & Application("CHEMINV_USERNAME") & ".CSDOHitlist.ID " & _
			"AND " & _ 
			Application("CHEMINV_USERNAME") & ".CSDOHitlist.HitlistID =" & HitlistID   
	QueryName = "qryContainerAttributesFromContainerIDHitlistID"
end if


QueryText = SQL 

if bDebugPrint then
	'Debugging section
	Response.write("QueuePath   : " & ReportQueuePath & "<br>")
	Response.write("DatabasePath: " & ReportDbPath & "<br>")
	Response.write("ReportName  : " & ReportName & "<br>")
	Response.write("ReportDirectory  : " & ReportsHTTPPath & "<br>")
	Response.write("QueryName  : " & QueryName & "<br>")
	Response.write("QueryText  : " & QueryText & "<br>")
	Response.write("ReportFormat: " & ReportFormat & "<br>")
Else
	'Create RPT
	Set ReportQ = Server.CreateObject("ReportQ.CReportQ")
	ReportFileName = ReportQ.MakeReport(ReportQueuePath, ReportDBPath, ReportName, QueryName, QueryText, ReportFormat)
	Response.ContentType = "text/html"
	If InStr(1,ReportFileName,"Report Error") = 0 Then	
		Response.Write ReportsHTTPPath & ReportFileName
	Else 
		Response.Write ReportFileName
	End if
	Set ReportQ = Nothing
End if
%>
