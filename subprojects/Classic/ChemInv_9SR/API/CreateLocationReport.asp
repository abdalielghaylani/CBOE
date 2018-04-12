<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
Dim strError
Dim bWriteError
Dim PrintDebug
Dim SortbyFieldName
Dim SortDirectionTxt
Dim SortDirection

bDebugPrint = False
bWriteError = False
strError = "Error:CreateLocationReport<BR>"

'RPT paths
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportArchiveDBPath =  RPTPath & "reportqueuearchive.mdb"	
ReportDBPath = Application("ReportDBPath")
ReportsHTTPPath = Application("ReportsHTTPPath") 

'Required Paramenters
LocationID= Request("LocationID")
ReportName = Request("ReportName")
CompoundID = Request("CompoundID")
ShowInList = Request("ShowInList")
If ShowInList = "plates" then
	PlateID = Request("PlateID")
else
	ContainerID = Request("ContainerID")
end if

'Optional parameters
ReportFormat = Request("ReportFormat")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateLocationReport.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationID) AND IsEmpty(CompoundID) then
	strError = strError & "Either LocationID or CompoundID are required parameters<BR>"
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
If Len(ContainerID) > 0 then
	SQL = SQL & " AND Inv_Containers.Container_ID=" & ContainerID	
ElseIf Len(PlateID) > 0 then
	SQL = SQL & " p.Plate_ID=" & PlateID
Else
	if ShowInList = "plates" then
		if Len(LocationID) > 0 then
			SQL = SQL & " inv_Locations.Location_ID =" &  LocationID	
		End if
	else 
		if Len(LocationID) > 0 then
			SQL = SQL & " AND inv_Locations.Location_ID =" &  LocationID	
		End if
		if Len(CompoundID) > 0 then
			SQL = SQL & " AND Inv_Containers.Compound_ID_FK = " & CompoundID
		End if
	end if
End if
QueryText = SQL
if ShowInList = "plates" then
	QueryName = "qryPlateLocationReport"
else
	QueryName = "qryLocationReport"
end if

'Response.Write SQL
'Response.End

if bDebugPrint then
	'Debugging section
	Response.write("QueuePath   : " & ReportQueuePath & "<br>")
	Response.write("DatabasePath: " & ReportDbPath & "<br>")
	Response.write("ReportName  : " & ReportName & "<br>")
	Response.write("ReportDirectory  : " & ReportDirectory & "<br>")
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
End if
%>
