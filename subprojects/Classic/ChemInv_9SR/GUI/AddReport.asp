<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->


<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim Conn
stepValue = "1"
nextStepValue = Request("nextStepValue")
if not isEmpty(nextStepValue) then stepValue = nextStepValue
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
	NumParams = cint(Request("NumParams"))
end if
%>

<html>
<head>
<title><%=Application("appTitle")%> -- Add a Report Layout</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	
	function Validate() {
		var bWriteError = false;
		var bWriteWarning = false;
		var errmsg = "Please fix the following problems:\r\r";
		var warningmsg = "Please address the following warnings:\r\r";

		// Report Name is required
		if (document.form1.ReportName.value.length == 0) {
			errmsg = errmsg + "- Microsoft Access Report Name is required.\r";
			bWriteError = true;
		}
		// Report Display Name is required
		if (document.form1.ReportDisplayName.value.length == 0) {
			errmsg = errmsg + "- Report Display Name is required.\r";
			bWriteError = true;
		}
		// Query Name is required
		if (document.form1.QueryName.value.length == 0) {
			errmsg = errmsg + "- Microsoft Access Query Name is required.\r";
			bWriteError = true;
		}
		
		// # parameters if present must be a number
		if (document.form1.NumParams.value.length >0 && !isWholeNumber(document.form1.NumParams.value)){
			errmsg = errmsg + "- # of Report Parameters must be a number with a value of zero or greater.\r";
			bWriteError = true;
		}
		
		

		if (document.form1.stepValue.value == 2) {
			document.form1.action='AddReport_action.asp';

			if (document.form1.NumParams.value > 0)
			{
				for (i=1; i<= document.form1.NumParams.value; i++)
				{
					// parameter display name is required
					var paramDisplayName = eval("document.form1.Parameter" + i + "DisplayName.value");
					if (paramDisplayName.length == 0)
					{
						errmsg = errmsg + "- Parameter #" + i + " Display Name is required.\r";
						bWriteError = true;
					}
					// parameter name is required
					var paramName = eval("document.form1.Parameter" + i + "Name.value");
					if (paramName.length == 0)
					{
						errmsg = errmsg + "- Parameter #" + i + " Name is required.\r";
						bWriteError = true;
					}
				}
			}
		}
		if (bWriteError){
			alert(errmsg);
		}
		else{
		
			var bcontinue = true;

			// Report warnings, user can choose to accept or cancel
			bConfirmWarning = true;
			if (bWriteWarning) {
				bConfirmWarning = confirm(warningmsg);
			}
			if (!bConfirmWarning) var bcontinue = false;
			if (bcontinue) document.form1.submit();
		}
	
	}
//-->
</script>
</head>
<body>
<center>
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Create a Report Layout.</span><br /><br />
		<table bgcolor="#e1e1e1" width="100%"><tr><td align="center">
			Step <strong><%=stepValue%></strong> of 2
		</td></tr></table><br /><br />
	</td></tr>
	<tr>
		<td colspan="2" valign="top" align="center">
			<form name="form1" action="#" method="POST">
			<input type="hidden" name="stepValue" value="<%=stepValue%>" />
			<input type="hidden" name="nextStepValue" value="2" />

			<table border="0" cellspacing="0" cellpadding="0">
				<%if stepValue = "1" then%>
				<tr>
				<%
				Response.write ShowPicklist("<span class=""required"">Report Type:</span>", "ReportTypeID", "", "SELECT ReportTypeDesc AS DisplayText, ReportType_ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reporttypes")
				%>
				</tr>
				<tr>		
					<td align="right" nowrap><span class="required">Microsoft Access Report Name:</span></td>
					<td align="left" colspan="2">
						<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportName">
					</td>
				</tr>
				<tr>
				  	<td align="right" nowrap><span class="required">Report Display Name:</span></td>
				  	<td colspan="2">
				  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportDisplayName">
				  	</td>
				</tr>
				<tr>
				  	<td align="right" nowrap><span class="required">Microsoft Access Query Name:</span></td>
				  	<td colspan="2">
				  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="QueryName">
				  	</td>
				</tr>
				<tr>
				  	<td align="right" nowrap valign="top">Report SQL Query:</td>
				  	<td colspan="2">
				  		<textarea NAME="QuerySQL" rows="10" cols="45"></textarea>
				  	</td>
				</tr>
				<tr>
				  	<td align="right" nowrap valign="top">Report Description:</td>
				  	<td colspan="2">
				  		<textarea NAME="ReportDesc" rows="3" cols="45"></textarea>
				  	</td>
				</tr>
				<tr>
				  	<td align="right" nowrap># of Report Parameters:</td>
				  	<td colspan="2">
				  		<input TYPE="text" SIZE="5" Maxlength="50" NAME="NumParams">
				  	</td>
				</tr>
				<tr>
					<td colspan="2" align="right"> 
					&nbsp;	
					</td>
				</tr>	
				<%else%>
				<tr>
					<td align="right" nowrap>
						Microsoft Access Report Name:&nbsp;
					</td>
					<td align="left" colspan="2">
						<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportName" STYLE="background-color:#d3d3d3;" VALUE="<%=ReportName%>" READONLY>
					</td>
				</tr>
				<tr>
				  	<td align="right" nowrap>
				  		Report Display Name:
				  	</td>
				  	<td colspan="2">
				  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportDisplayName" STYLE="background-color:#d3d3d3;" VALUE="<%=ReportDisplayName%>" READONLY>
				  	</td>
				</tr>
						<tr>
				  	<td align="right" nowrap>
				  		Microsoft Access Query Name:
				  	</td>
				  	<td colspan="2">
				  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="QueryName" STYLE="background-color:#d3d3d3;" VALUE="<%=QueryName%>" READONLY>
				  		<input TYPE="hidden" NAME="QuerySQL" VALUE="<%=QuerySQL%>">
				  		<input TYPE="hidden" NAME="ReportDesc" VALUE="<%=ReportDesc%>">
				  		<input TYPE="hidden" NAME="ReportTypeID" VALUE="<%=ReportTypeID%>">
				  		<input TYPE="hidden" NAME="NumParams" VALUE="<%=NumParams%>">
				  	</td>
				</tr>
				<%for i =1 to NumParams%>
					<tr>
					<td align="right">
						<span class="required">Parameter #<%=i%> Display Name:</span>
					</td>
					<td>
						<input type="text" size="40" Maxlength="50" NAME="Parameter<%=i%>DisplayName">&nbsp;&nbsp;Required? <input type="checkbox" NAME="Parameter<%=i%>IsRequired" VALUE="1">
					</td>
				</tr>
				<tr>
					<td align="right" nowrap>
						<span class="required">Parameter #<%=i%> Name (table.field):</span>
					</td>
					<td>
						<input type="text" size="40" Maxlength="50" NAME="Parameter<%=i%>Name">
					</td>
				</tr>
				<tr>
					<td align="right">
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
				<%end if%>
				<%if stepValue = "1" then%>
				<tr><td colspan="2" align="right">
					<a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;
					<a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				</td></tr>
				<%else%>
				<tr><td colspan="3" align="right">
					<a HREF="#" onclick="history.go(-2); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;
					<a HREF="#" onclick="history.go(-1);"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a>
					<a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				</td></tr>
				<%end if%>
				
			</table>
			</form>
		</td>
	</tr>
</table>
</center>
</body>
</html>