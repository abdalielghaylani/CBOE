<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
ReportName = Request("ReportName")
ShowInList = Request("ShowInList")
bRunReport = Request("bRunReport")

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

ReportFormat = Application("DefaultReportFormat")
If ReportFormat = "" then ReportFormat = "SNP"
'If ReportFormat <> "SNP" AND ReportFormat <> "PDF" then ReportFormat = "SNP"

If ShowInList = "plates" then
	If ReportName = "" then ReportName = Application("DefaultPlateLabelReport")
	PlateID = Request("PlateID")
	title = Application("appTitle") & " -- Print Plate Labels"
	ReportTypeID = 7
	API = "/cheminv/api/CreatePlateReport.asp"
	FormData = "ReportFormat=" & ReportFormat & "&PlateList=" & PlateID & "&ReportName=" & ReportName & Credentials
else
	If ReportName = "" then ReportName = Application("DefaultContainerLabelReport")
	ContainerID = Request("ContainerID")
	title = Application("appTitle")  & " -- Print Container Labels"
	ReportTypeID = 1
	API = "/cheminv/api/CreateContainerReport.asp"
	FormData = "ReportFormat=" & ReportFormat & "&ContainerList=" & ContainerID & "&ReportName=" & ReportName & Credentials
end if
if bRunReport = "true" then
	'Response.Write ServerName & ":" & FormData & ":" & API
	httpResponse = CShttpRequest2("POST", ServerName, API, "ChemInv", FormData)
	'Response.Write  httpResponse
	'Response.end
End if
If InStr(1,httpResponse,"Report Error") = 0 Then
%>
<html>
<head>	
	<title><%=title%></title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
</head>
<body onload="window.focus()">
	<center>
	<table border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align="left">
				<FORM name="form1" action="#" method="POST">
				<input type="hidden" name="ShowInList" value="<%=ShowInList%>">
				<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
				<input type="hidden" name="PlateID" value="<%=PlateID%>">
				<input type="hidden" name="bRunReport" value="true">
				Label Type:&nbsp;
				<%
				Dim Conn
				Response.write ShowRPTSelectBox("ReportName", ReportName, "SELECT ReportDisplayName AS DisplayText, ReportName AS value FROM  " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ReportType_ID = " & ReportTypeID)
				%>
				&nbsp;<input type="submit" value="Go">
				
				</form>
				<SCRIPT LANGUAGE=javascript>document.form1.ReportName.onchange = function(){document.form1.submit()}</SCRIPT>
			</td>	
		</tr>
			<tr>
				<td>
<%Select Case UCase(ReportFormat)
	Case "" 
		Response.Write "<center><table border=0 width=""90%""><tr><td ALIGN=center><BR><BR><BR><BR><BR><BR><BR>"
	Response.Write "<SPAN class=""GuiFeedback"">Select the desired report and format from above</SPAN></td></tr></table></center>"
	Case "SNP"
%>
	<center>
	<OBJECT classid="CLSID:F0E42D60-368C-11D0-AD81-00A0C90DC8D9" codeBase="/cfserverasp/rpt/Snapview.ocx" 
	height=600 id=SnapshotViewer style="LEFT: 0px; TOP: 0px" width=800 VIEWASTEXT>
    <param name="_ExtentX" value="10583">
    <param name="_ExtentY" value="3307">
    <param name="_Version" value="65536">
    <param name="SnapshotPath" value="<%=httpResponse%>">
    <param name="Zoom" value="5">
    <param name="AllowContextMenu" value="-1">
    <param name="ShowNavigationButtons" value="-1"></OBJECT>
	</center>
<%
	Case "PDF", "XLS", "RTF"
		'Response.Clear
		if bRunReport = "true" then
		Response.Redirect httpResponse
		end if
End Select
%>
				</td>
			</tr>
			<tr>
				<td align=right>
					<BR>
					<a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a>
				</td>
			</tr>	
		</table>
	</center>
<%Else
	Response.Write "<BR><BR><CENTER><P><CODE>" & httpResponse & "</CODE></P>"
	Response.Write "<SPAN class=""GuiFeedback"">Label could not be created</SPAN></center>"			
End if
%>

</body>
</html>		