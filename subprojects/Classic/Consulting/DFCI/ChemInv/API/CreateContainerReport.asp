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
strError = "Error:CreateContainerReport<BR>"

'RPT paths
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportArchiveDBPath =  RPTPath & "reportqueuearchive.mdb"	
ReportDBPath = Application("ReportDBPath")
ReportsHTTPPath = Application("ReportsHTTPPath") 

'Required Paramenters
ContainerList= Request("ContainerList")
ReportName = Request("ReportName")

'Optional parameters
ReportFormat = Request("ReportFormat")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateContainerReport.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(ContainerList) then
	strError = strError & "ContainerList is a required parameter<BR>"
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

' Parse ContainerList
tempArr = Split(ContainerList,",")
ConInList = " Container_ID IN (" 
For i = 0 to Ubound(tempArr)
	' Check for ranges
	RangeArr = Split(tempArr(i),"-")
	if Ubound(RangeArr) > 0 then
		if CLng(RangeArr(0)) < CLng(RangeArr(1)) then
			Low = RangeArr(0)
			high = RangeArr(1)
		Else
			Low = RangeArr(1)
			High = RangeArr(0)
		End if
		ConRangeSQL = ConRangeSQL & " OR (Container_ID >= " & Low & " AND Container_ID <= " & High & ")"
	Else
		ConInList = ConInList & tempArr(i) & ","
	End if
Next
If InStr(ConInList, ",") then 
	ConInList = Left(ConInList, Len(ConInList)-1) & ")"
Else
	' There is no IN clause, so remove the leading OR from RangeSQL
	ConInList = ""
	ConRangeSQL = Right(ConRangeSQL, Len(ConRangeSQL)-3)
End if

%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/ContainerSQL.asp"-->
<%
QueryText = SQL & "AND " & ConInList & ConRangeSQL
QueryName = "qryContainerAttributes"

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
