<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS
Dim lRecordCount
Dim ReportTypeID

ReportName = Request("ReportName")
ShowInList = Request("ShowInList")
bRunReport = Request("bRunReport")

GetInvConnection()
				
' Build the RS manually so we can get the record count
Set RS = Server.CreateObject("ADODB.Recordset")
RS.CursorLocation = 3   ' adUseClient
RS.CursorType = 3       ' adOpenStatic
RS.ActiveConnection = Conn

If ShowInList = "plates" then
    ReportTypeID = 7
else
    ReportTypeID = 1
end if
				
RS.Open( "SELECT ReportDisplayName AS DisplayText, ReportName AS value FROM  " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ReportType_ID = " & ReportTypeID )
lRecordCount = RS.RecordCount

' If there is only one instance of the report type to run, just run it now instead of requiring the user to select 
' it from the combo box
if( lRecordCount = 1 ) then
    bRunReport = "true"
    ReportName = RS.Fields("value").Value
end if

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

ReportFormat = Application("DefaultReportFormat")
If ReportFormat = "" then ReportFormat = "SNP"
'If ReportFormat <> "SNP" AND ReportFormat <> "PDF" then ReportFormat = "SNP"

If ShowInList = "plates" then
	If ReportName = "" then ReportName = Application("DefaultPlateLabelReport")
	PlateID = Request("PlateID")
	title = Application("appTitle") & " -- Print Plate Labels"	
	API = "/cheminv/api/CreatePlateReport.asp"
	FormData = "ReportFormat=" & ReportFormat & "&PlateList=" & PlateID & "&ReportName=" & ReportName & Credentials
else
	If ReportName = "" then ReportName = Application("DefaultContainerLabelReport")
	ContainerID = Request("ContainerID")
	title = Application("appTitle")  & " -- Print Container Labels"	
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
<script language=javascript>
    function OnDocLoad()
    {
        window.focus();
<%
    if( lRecordCount = 1 ) then
%>
        PrintReport();
<%
    end if
%>
    }
</script>
</head>
<body onload="OnDocLoad();">
	<center>
	<table border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align="left">
				<FORM name="form1" action="#" method="POST">
				<input type="hidden" name="ShowInList" value="<%=ShowInList%>">
				<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
				<input type="hidden" name="PlateID" value="<%=PlateID%>">
				<input type="hidden" name="bRunReport" value="true">
				<%
				    if( lRecordCount > 1 ) then
				%>
				Label Type:&nbsp;
				<%
				        'Response.write ShowRPTSelectBox("ReportName", ReportName, )
				        Response.write( BuildSelectBox(RS, "ReportName", ReportName, 0, "Select One", "NULL", 1, false, "", "" ) )				
				    else				
				%>
				<input type="hidden" name="ReportName" value="<% = ReportName %>">				
				<%
				    end if
				%>
				
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
	<script language=javascript>
	    function PrintReport()
	    {
            SnapshotViewer.PrintSnapshot(true);
        }
	</script>
<%
	Case "PDF", "XLS", "RTF"
		'Response.Clear
		if bRunReport = "true" then
		Response.Redirect httpResponse
		end if
%>
    <script language=javascript>
	    function PrintReport()
	    {
            window.print();
        }
    </script>
<%		
End Select
%>
				</td>
			</tr>
			<tr>
				<td align=right>
					<BR>
<% if bRunReport = "true" then %>
					<a class="MenuLink" HREF="#" alt="Print this label" onclick="PrintReport();return false;">Print</a> |
<% end if %>
					<a class="MenuLink" HREF="Close this window" onclick="window.close();return false">Close</a>
				</td>
			</tr>	
		</table>
	</center>	
<%Else
	Response.Write "<BR><BR><CENTER><P><CODE>" & httpResponse & "</CODE></P>"
	Response.Write "<SPAN class=""GuiFeedback"">Label could not be created</SPAN></center>"			
End if

' Cleanup
RS.Close
Set RS = nothing
%>

</body>
</html>		