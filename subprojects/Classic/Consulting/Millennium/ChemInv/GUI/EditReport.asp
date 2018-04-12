<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>

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
ReportTypeID = Request("ReportTypeID")
ExistingReportTypeID = Request("ExistingReportTypeID")
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
If ReportFormat = "" then ReportFormat = "NULL"
If ReportName = "" then ReportName = "NULL"
If Report_ID = "" then Report_ID = 0
%>

<html>
<head>
<title><%=Application("appTitle")%> -- Edit Report Details</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->


<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->

</script>
</head>
<body>

<%If InStr(1,httpResponse,"Error") = 0 Then%>
	<table border="0" cellspacing="0" cellpadding="2" width="700" align="left">
		<tr>
			<td valign="top" align="left">
				<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
			</td>
			<td valign="top" align=right>
				<a class="MenuLink" href="Close" onclick="parent.close(); return false">Close</a> | <a class=menulink href="DeleteReport.asp?ReportTypeID=<%=ReportTypeID%>&Report_ID=<%=Report_ID%>">Delete Report</a>
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
				<%if isempty(ReportTypeID) or Report_ID=0 then%>
					<form name="cows_input_form" action="EditReport.asp" method="POST">
				<%else%>
					<form name="cows_input_form" action="EditReport_action.asp" method="POST">
				<%end if%>
				<table border="0" cellspacing="0" cellpadding="0">
					<tr>
					<%Dim Conn%>
					<%if isempty(ReportTypeID) then%>					
							<%
							Response.write ShowPicklist("Select a report type:", "ReportTypeID", "", "SELECT ReportTypeDesc AS DisplayText, ReportType_ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reporttypes")
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
						<input type="hidden" value="<%=ReportTypeID%>" name="ReportTypeID">
							<%
							if InStr(ReportTypeID,",") then
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
							Report Display Name:
						</td>
						<td>
							<input type="hidden" value="<%=ExistingReportTypeID%>" name="ReportTypeID">
							<input type="hidden" value="<%=Report_ID%>" name="Report_ID">	
							<%'Response.end%>
							<input TYPE="text" SIZE="40" NAME="ReportDisplayName" VALUE="<%=GetListFromSQL("SELECT ReportDisplayName FROM " & Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)%>">
						</td>
					</tr>
					<tr>	
						<td align="right" valign="top">MS Access Query Name:
						</td>
						<td>
							<input TYPE="text" SIZE="40" NAME="QueryName" VALUE="<%=GetListFromSQL("SELECT QueryName FROM " & Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)%>">
						</td>
					</tr>
					<tr>
						<td align="right" valign="top">MS Access Report Name:
						</td>
						<td>
							<input TYPE="text" SIZE="40" NAME="ReportName" VALUE="<%=GetListFromSQL("SELECT ReportName FROM " & Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)%>">
							</td>
						<td>
							&nbsp;
						</td>	
					</tr>
					<tr>
						<td align="right" valign="top">Report Description:
						</td>
						<td>
							<textarea cols=40 rows=3 NAME="ReportDesc"><%=GetListFromSQL("SELECT Report_Desc FROM " & Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)%></textarea>
						</td>
						<td>
							&nbsp;
						</td>	
					</tr>
					<tr>
						<td align="right" nowrap valign="top">Report SQL Query:
						</td>
						<td>
							<textarea cols=40 rows=7 NAME="QuerySQL"><%=GetListFromSQL("SELECT ReportSQL FROM " & Application("CHEMINV_USERNAME") & ".inv_Reports WHERE ID=" & Report_ID)%></textarea>
							</td>
						<td>
							&nbsp;
						</td>	
					</tr>
					<% iRecLast = -1
					ParamNames=GetArrayFromSQL("SELECT ParamName,ParamType,isrequired, ParamDisplayName FROM " &  Application("CHEMINV_USERNAME") & ".inv_ReportParams WHERE Report_ID=" & Report_ID)
						if isArray(ParamNames) then
	  						iRecFirst = LBound(ParamNames, 2)
	  						iRecLast = UBound(ParamNames, 2)	  
	  					For i = iRecFirst To iRecLast%>
	  				<tr>
	  				<td>&nbsp;
	  				</td>
	  				  <td colspan=2>
	  				  <%if i=0 then%>WHERE<%else%>AND<%end if%>&nbsp;<input type="text" size="30" Maxlength="50" NAME="Parameter<%=i+1%>Name" VALUE=<%=Rtrim(ParamNames(0,i))%>>&nbsp;=&nbsp;
<%if Rtrim(ParamNames(1,i)) = "num" then%><select NAME="Parameter<%=i+1%>Type"><option value="num" selected>Number</option><option value="text">Text</option><option value="start_date">Date Range Start</option><option value="end_date">Date Range End</option><option value="user_name">Username</option><option value="location">Location</option>	</select>
<%elseif Rtrim(ParamNames(1,i)) = "text" then%><select NAME="Parameter<%=i+1%>Type"><option value="num">Number</option><option value="text" selected>Text</option><option value="start_date">Date Range Start</option><option value="end_date">Date Range End</option><option value="user_name">Username</option><option value="location">Location</option>	</select>
<%elseif Rtrim(ParamNames(1,i)) = "user_name" then%><select NAME="Parameter<%=i+1%>Type"><option value="num">Number</option><option value="text">Text</option><option value="start_date">Date Range Start</option><option value="end_date">Date Range End</option><option value="user_name" selected>Username</option><option value="location">Location</option>	</select>
<%elseif Rtrim(ParamNames(1,i)) = "start_date" then%><select NAME="Parameter<%=i+1%>Type"><option value="num">Number</option><option value="text">Text</option><option value="start_date" selected>Date Range Start</option><option value="end_date">Date Range End</option><option value="user_name">Username</option><option value="location">Location</option>	</select>
<%elseif Rtrim(ParamNames(1,i)) = "location" then%><select NAME="Parameter<%=i+1%>Type"><option value="num">Number</option><option value="text">Text</option><option value="start_date">Date Range Start</option><option value="end_date">Date Range End</option><option value="user_name">Username</option><option value="location" selected>Location</option>	</select>
<%else%><select NAME="Parameter<%=i+1%>Type"><option value="num">Number</option><option value="text">Text</option><option value="start_date">Date Range Start</option><option value="end_date">Date Range End</option><option value="user_name">Username</option><option value="location">Location</option>	</select><%end if%>
						<input type="hidden" size="30" Maxlength="50" NAME="Parameter<%=i+1%>DisplayName" VALUE=<%=Rtrim(ParamNames(3,i))%>>
					 Required? <input type="checkbox" NAME="Parameter<%=i+1%>IsRequired" VALUE="1" <%if Rtrim(ParamNames(2,i))=1 then %> checked <%end if%>>
					  </td></tr>
					<%next%>
					<%end if%>
					<tr>
						<td align="right" nowrap>
							&nbsp;
						</td>
						<td align="left">
							&nbsp;
						</td>
						<td>
						<%if iRecLast <>-1 then%>
							<input type="hidden" value=<%=iRecLast+1%> name="NumParams">
						<%end if%>
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
