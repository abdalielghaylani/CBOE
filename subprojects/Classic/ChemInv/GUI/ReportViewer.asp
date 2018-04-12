<%@ EnableSessionState=False Language=VBScript%>

<html>
<head>
<title><%=Application("appTitle")%> -- Report Viewer</title>

 <%

SequenceNo = Request.QueryString("SequenceNo")
RPTPath = Application("RPT_PATH")
ReportQueuePath = RPTPath & "reportqueue.mdb"
ReportsHTTPPath = Application("ReportsHTTPPath") 


Set ReportQ = Server.CreateObject("ReportQ.CReportQ")

ReportFileName = ReportQ.GetReportStatus(ReportQueuePath, SequenceNo)
 
if  InStr(1,ReportFileName,"REPORT: ") <= 0 then
 Response.Write("<meta http-equiv=""refresh"" content=""15"">")
end if

%>

</head>
<body>

 
<%
 
if  InStr(1,ReportFileName,"REPORT: ") > 0 then
    
	Dim arr
        arr = Split(ReportFileName,": ")	

	if InStr(1,UCase(arr(1)),"SNP") > 0 then
	%>
	<center>
	<OBJECT classid="CLSID:F0E42D40-368C-11D0-AD81-00A0C90DC8D9" codeBase="/cfserverasp/rpt/Snapview.ocx" 
	height=600 id=SnapshotViewer style="LEFT: 0px; TOP: 0px" width=800 VIEWASTEXT>
	<PARAM NAME="_ExtentX" VALUE="21167">
	<PARAM NAME="_ExtentY" VALUE="15875">
	<PARAM NAME="_Version" VALUE="65536">
	<PARAM NAME="SnapshotPath" VALUE="<%=(ReportsHTTPPath & arr(1))%>">
	<PARAM NAME="Zoom" VALUE="5">
	<PARAM NAME="AllowContextMenu" VALUE="-1">
	<PARAM NAME="ShowNavigationButtons" VALUE="-1"></OBJECT>
	</center>
	<%
	
	else 
	
	    Response.Write "<table border=0 width=""100%""><tr><td><BR><CENTER><P><CODE>Report generated sucessfully. <BR/><a href='" & (ReportsHTTPPath & arr(1)) & "'>Click here</a> to view the report</CODE></P></td></tr></table>"
        end if
else 
    Response.Write "<table border=0 width=""100%""><tr><td><BR><CODE>Reporting engine is heavily loaded.<BR/>There are total " & ReportFileName & " report(s) in print queue including this one, hence your report is added in report print queue.<BR/><BR/>Once your report is generated successfully, a link will appear here and you can click the same to view the report. Meanwhile you can minimize this page and continue working on Inventory. </CODE></td></tr><tr><td><CODE><font color=""red""><BR/><B>Warning:<BR/>Please do not close the page, this will result in loosing the report your are generating.</font></B></CODE></td></tr></table>"
End if



%>

</body>
</html>