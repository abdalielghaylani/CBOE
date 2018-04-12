<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript" type="text/javascript">
function validate(){
  var bWriteError = false;
  var errmsg = "Please fix the following problems:\r\r";
  
  if (document.forms[0].ReportName.value==''){
        errmsg = errmsg  + "-Report name is required.\r";
	    bWriteError= true;
  }
  if (document.forms[0].ReportDisplayName.value==''){
        errmsg = errmsg  + "-Report display name is required.\r";
	    bWriteError= true;
  }
  if (document.forms[0].QueryName.value==''){
        errmsg = errmsg  + "-Query name is required.\r";
	    bWriteError= true;
  }
  
  
  if (document.forms[0].NumParams.value!=''){
	if (!isPositiveNumber(document.forms[0].NumParams.value)){
	    errmsg = errmsg  + "-Number of parameters must be a positive number.\r";
	    bWriteError= true;
	}
	else{
	    if (document.forms[0].QuerySQL.value=='Select * from cheminvdb2.') {
	        errmsg = errmsg  + "-Please Enter a valid SQL Query\r";
	        bWriteError= true;
	    }
	}
  }
  if (bWriteError){
    alert(errmsg);
  }  
  else{
    document.forms[0].submit();
  }

}

</script> 

<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Conn
ReportName = Request("ReportName")
ReportDesc = Request("ReportDesc")
ReportTypeID = Request("ReportTypeID")
ReportDisplayName = Request("ReportDisplayName")
QueryName = Request("QueryName")
QuerySQL = Request("QuerySQL")
NumParams = Request("NumParams")
if IsEmpty(NumParams) OR NumParams="" then 
	NumParams=0
else
	NumParams = FormatNumber(Request("NumParams"))
end if

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))

%>

<html>
<head>
<title><%=Application("appTitle")%> -- Add Report</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>


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
				<a class="MenuLink" href="Close" onclick="window.location='/cheminv/gui/ManageReports.asp?ReportTypeID=<%=ReportTypeID%>'; return false;">Close</a>
			</td>
		</tr>
		<%if (QueryName <> "NULL" AND ReportName <> "NULL" AND QuerySQL <> "NULL" AND Request.Form("doit")= "1") then%>
		<tr><td>Report Submitted</td></tr>
		<%else%>
		<tr>
			<td colspan="2" valign="top" align="center">	
			<!-- CSBR-76339 -->		
			<% if ReportName = "NULL" or ReportName ="" then%>
				<form name="form1" action="AddReport.asp" method="POST">
			<%Else%>
				<form name="form1" action="AddReport_action.asp" method="POST">
			<%End IF  %>
				<table border="0" cellspacing="0" cellpadding="0">
					<tr>
					<%if ReportName = "NULL" or ReportName ="" then%>	
							<%
							Response.write ShowPicklist2("Select a report type:", "ReportTypeID", ReportTypeID, "SELECT ReportTypeDesc AS DisplayText, ReportType_ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reporttypes", "45", "All Types", "-1", "")
							%>
					</tr>
					<tr>		
			<!-- 07-06-2010: Manoj Unnikrishnan: Added span tag as required for the required fields CSBR-121461 -->		
						<td align="right" nowrap><span class="required">
							Enter Microsoft Access Report Name:&nbsp;</span>
						</td>
						<td align="left" colspan=2>
							<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportName">
						</td>
					</tr>
					<tr>
					  	<td align=right nowrap><span class="required">
					  		Enter Report Display Name:&nbsp;</span>
					  	</td>
					  	<td colspan=2>
					  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportDisplayName">
					  	</td>
					</tr>
					<tr>
					  	<td align=right nowrap><span class="required">
					  		Enter Microsoft Access Query Name:&nbsp;</span>
					  	</td>
			<!-- End of change for CSBR-121461 -->		
					  	<td colspan=2>
					  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="QueryName">
					  	</td>
					</tr>
					<tr>
					  	<td align=right nowrap valign=top>
					  		Enter Report SQL Query:&nbsp;
					  	</td>
					  	<td colspan=2>
					  		<textarea NAME="QuerySQL" rows="10" cols="45">Select * from cheminvdb2.</textarea>
					  	</td>
					</tr>
					<tr>
					  	<td align=right nowrap valign=top>
					  		Enter Report Description:&nbsp;
					  	</td>
					  	<td colspan=2>
					  		<textarea NAME="ReportDesc" rows="3" cols="45"></textarea>
					  	</td>
					</tr>
					<tr>
					  	<td align=right nowrap>
					  		Enter # of Parameters to Configure:&nbsp;
					  	</td>
					  	<td colspan=2>
					  		<input TYPE="text" SIZE="5" Maxlength="50" NAME="NumParams">
					  	</td>
					</tr>
					<tr>
						<td colspan=3 align = right>
							<input type="submit" value="Go" name="submit" onclick="javascript:validate(); return false;">
						</td>
					</tr>
					<tr>
						<td colspan="2" align="right"> 
						&nbsp;	
						</td>
					</tr>	
					<%else%>
						<td align="right" nowrap>
							Microsoft Access Report Name:&nbsp;
						</td>
						<td align="left" colspan=2>
							<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportName" class="readOnly" VALUE="<%=ReportName%>" READONLY>
						</td>
					</tr>
					<tr>
					  	<td align=right nowrap>
					  		Report Display Name:
					  	</td>
					  	<td colspan=2>
					  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportDisplayName" class="readOnly" VALUE="<%=ReportDisplayName%>" READONLY>
					  	</td>
					</tr>
	
					<tr>
					  	<td align=right nowrap>
					  		Microsoft Access Query Name:
					  	</td>
					  	<td colspan=2>
					  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="QueryName" class="readOnly" VALUE="<%=QueryName%>" READONLY>
					  		<input TYPE="hidden" NAME="QuerySQL" VALUE="<%=QuerySQL%>">
					  		<input TYPE="hidden" NAME="ReportDesc" VALUE="<%=ReportDesc%>">
					  		<input TYPE="hidden" NAME="ReportTypeID" VALUE="<%=ReportTypeID%>">
					  		<input TYPE="hidden" NAME="NumParams" VALUE="<%=NumParams%>">
					  	</td>
					</tr>
					<%for i =1 to NumParams%>

					<tr>
						<td align=right>
							Parameter #<%=i%> Display Name:
						</td>
						<td>
							<input type="text" size="30" Maxlength="50" NAME="Parameter<%=i%>DisplayName">
						</td>
						<td align=right nowrap>
							Required? <input type="checkbox" NAME="Parameter<%=i%>IsRequired" VALUE=1>
						</td>
					</tr>
					<tr>
						<td align=right nowrap>
							Parameter #<%=i%> Name (table.field):
						</td>
						<td>
							<input type="text" size="30" Maxlength="50" NAME="Parameter<%=i%>Name">
						</td>
					</tr>
					<tr>
						<td align=right>
							Parameter #<%=i%> Type:
						</td>
						<td>
							<select NAME="Parameter<%=i%>Type">
								<option value="num">Number</option>
								<option value="text">Text</option>
								<option value="start_date">Date Range Start Date</option>
								<option value="end_date">Date Range End Date</option>
								<option value="user_name">Username</option>
								<option value="location">Location</option>
							</select>
						</td>
					</tr>
					<%next%>
					<tr>
						<td colspan=3 align=center>
							<input type="hidden" value="1" name="doit">
							<input type="submit" value="Add Report" name="submit">
						</td>	
					</tr>
					<%end if%>
					<%end if%>
				</table>
				</form>
			</td>
		</tr>
	</table>
<%Else

	Response.Write "<table border=0 width=""90%""><tr><td><BR><BR><CENTER><P><CODE>" & httpResponse & "</CODE></P>"
	Response.Write "<SPAN class=""GuiFeedback"">Report Data is not complete</SPAN></td></tr></table></center>"			
%>
<%End if%>		
</body>
</html>
