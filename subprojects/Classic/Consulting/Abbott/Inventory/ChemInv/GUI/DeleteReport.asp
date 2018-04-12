<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->


<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

ReportSQL = Request("ReportSQL")
ReportName = Request("ReportName")
ReportDisplayName = Request("ReportDisplayName")
Report_ID = Request("Report_ID")
ReportType_ID = Request("ReportType_ID")
QueryName = Request("QueryName")
HitlistID = Request("HitListID")
ContainerList = Request("ContainerList")
ContainerID = Request("ContainerID")
if ContainerID = "0" then ContainerID = ""
LocationID = Request("LocationID")
CompoundID = Request("CompoundID")
ReportFormat = Request("ReportFormat")
isCustomReport = Request("isCustomReport")
ShowInList = Request("ShowInList")
PlateID = Request("PlateID")
if PlateID = "0" then PlateID = ""
PlateList = Request("PlateList")

If ReportFormat = "" then ReportFormat = Application("DefaultReportFormat")

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

FormData = "&Report_ID=" & Report_ID & Credentials

	

If ReportFormat = "" then ReportFormat = "NULL"
If ReportName = "" then ReportName = "NULL"
If Report_ID = "" then Report_ID = 0

if (Report_ID <> 0 AND Request.Form("doit")= "1") then
	FormData = Request.form & "&ServerName=" & ServerName & Credentials
	httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteReport.asp" , "ChemInv", FormData)
	'Response.write FormData
	'Response.end
Response.Write httpResponse
Response.end
End if

%>

<html>
<head>
<title><%=Application("appTitle")%> -- Edit Report Details</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->


<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->

</script>
</head>
<body>

<%If InStr(1,httpResponse,"Error") = 0 Then%>
	<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
		<tr>
			<td valign="top" align="left">
				<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
			</td>
			<td align="right" valign="top">
				<a class="MenuLink" href="Close" onclick="parent.close(); return false">Close</a>
			</td>
		</tr>
		<tr>
			<td width="100%" colspan=2>
	<table width="100%">
		<tr>
			<td>
				&nbsp;
			</td>
			<td valign="top" align="center">
				<form name="cows_input_form" action="DeleteReport.asp" method="POST">
				<table border="0" cellspacing="0" cellpadding="0">
					<tr>
					<%Dim Conn%>
					<%if isempty(ReportType_ID) then%>					
							<%
							Response.write ShowPicklist("Select a report type:", "ReportType_ID", "", "SELECT ReportTypeDesc AS DisplayText, ReportType_ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reporttypes")
							%>
						</td>
						<td>
							<input type="hidden" value="0" name="doit">
							<input type="submit" value="Go" name="submit">
						</td>
					</tr>
					<tr>
						<td colspan="2" align="right"> 
						&nbsp;	
						</td>
					</tr>
					<%elseif Report_ID=0 then%>	
						</td>
						<td align="right" nowrap valign=top>
							Select a report layout:&nbsp;
						</td>
						<td align="left">
						<input type="hidden" value="<%=ReportType_ID%>" name="ReportType_ID">
							<%
							if InStr(ReportType_ID,",") then
							  Response.write ShowRPTSelectBox("Report_ID", Report_ID, "SELECT ReportDisplayName AS DisplayText, ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ReportType_ID =" & right(ReportType_ID,len(ReportType_ID)-1))
							else
							  Response.write ShowRPTSelectBox("Report_ID", Report_ID, "SELECT ReportDisplayName AS DisplayText, ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ReportType_ID =" & ReportType_ID)
							end if
							%> 
						</td>
						<td valign=top>
							<input type="hidden" value="0" name="doit">
							<input type="submit" value="Go" name="submit">
						</td>
					</tr>
					<tr>
						<td colspan="2" align="right"> 
						&nbsp;	
						</td>
					</tr>
					<%else%>
						</td>
						<td align="right" valign="top">
							Delete this report and parameters:
						</td>
						<td>
							<input type="hidden" value="<%=Report_ID%>" name="Report_ID">	
							<%'Response.end%>
							<input TYPE="text" SIZE="40" STYLE="background-color:#d3d3d3;" NAME="ReportDisplayName" VALUE="<%=GetListFromSQL("SELECT ReportDisplayName FROM " & Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)%>" READONLY>
						</td>
					    <td>
							<input type="hidden" value="1" name="doit">
							<input type="submit" value="Go" name="submit">
						</td>	
					</tr>
					<%end if%>
				</table>
				</form>
			</td>
		</tr>
	</table>
<%End if%>		
</body>
</html>
