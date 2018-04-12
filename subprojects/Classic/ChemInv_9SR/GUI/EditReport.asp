<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim Conn
Dim Cmd
bDebugPrint = false

Report_ID = Request("Report_ID")
stepValue = "1"
nextStepValue = Request("nextStepValue")
if not isEmpty(nextStepValue) then stepValue = nextStepValue

if stepValue = "1" then
	'-- Get the report layout data from the db
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".Reports.GetReport(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PREPORTID",131, 1, 0, Report_ID)
	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
	Else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
	end if
	ReportTypeID = RS("ReportType_ID")
	ReportName = RS("ReportName")
	ReportDisplayName = RS("ReportDisplayName")
	ReportSQL = RS("ReportSQL")
	QueryName = RS("QueryName")
	ReportDesc = RS("Report_Desc")
	NumParams = RS("NumParams")
	if cInt(NumParams) > 0 then
		nextStepValue = "2"
	else
		nextStepValue = "1"
	end if
else
	'-- Get the report data from the posted values
	ReportSQL = Request("QuerySQL")
	ReportName = Request("ReportName")
	ReportDisplayName = Request("ReportDisplayName")
	ReportTypeID = Request("ReportTypeID")
	QueryName = Request("QueryName")
	ReportDesc = Request("ReportDesc")
	NumParams = Request("NumParams")
	
	'-- Get the parameter info for this report
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".ReportParams.GetReportParams(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PREPORTID",131, 1, 0, Report_ID)
	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
	Else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
	end if
end if

%>

<html>
<head>
<title><%=Application("appTitle")%> -- Edit a Report Layout</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
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
		// Query Name is required if it exists
		
		if (document.form1.QueryName && document.form1.QueryName.value.length == 0) {
			errmsg = errmsg + "- Microsoft Access Query Name is required.\r";
			bWriteError = true;
		}
		// # of Parameters is required
		if (document.form1.NumParams.value.length == 0) {
			errmsg = errmsg + "- # of Report Parameters is required.\r";
			bWriteError = true;
		}
		else{
			// # of Parameters must be a number
			if (!isWholeNumber(document.form1.NumParams.value)){
			errmsg = errmsg + "- # of Report Parameters must be a number zero or greater.\r";
			bWriteError = true;
			}
		}
		if (document.form1.stepValue.value == 2 || document.form1.NumParams.value=="0") {
			document.form1.action='EditReport_action.asp';

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
	
	function DisplayWizardGUI(numParams) {
		if (numParams == "0") {
			document.all.wizardTop.style.display = "none";
			document.all.wizardBottom.style.display= "none";
			document.all.nonWizardBottom.style.display = "block";
			document.form1.nextStepValue.value = "1";
		}
		else{
			document.all.wizardTop.style.display = "block";
			document.all.wizardBottom.style.display= "block";
			document.all.nonWizardBottom.style.display = "none";
			document.form1.nextStepValue.value = "2";
		}
	}
	
//-->

</script>
</head>
<body>
<center>
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Edit a Report Layout.</span><br /><br />
		<span id="wizardTop">
		<table bgcolor="#e1e1e1" width="100%"><tr><td align="center">
			Step <strong><%=stepValue%></strong> of 2
		</td></tr></table><br /><br /></span>
	</td></tr>
	<tr>
	<td colspan="2" valign="top" align="center">
		<form name="form1" action="#" method="POST">
		<input type="hidden" name="stepValue" value="<%=stepValue%>" />
		<input type="hidden" name="nextStepValue" value="<%=nextStepValue%>" />
		<input type="hidden" name="Report_ID" value="<%=Report_ID%>">	
		<table border="0" cellspacing="0" cellpadding="0">
		<tr>
		<%
		Response.write ShowPicklist("<span class=""required"">Report Type:</span>", "ReportTypeID", ReportTypeID, "SELECT ReportTypeDesc AS DisplayText, ReportType_ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_Reporttypes")
		%>
		</tr>
		<tr>		
			<td align="right" nowrap><span class="required">Microsoft Access Report Name:</span></td>
			<td align="left" colspan="2">
				<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportName" VALUE="<%=ReportName%>">
			</td>
		</tr>
		<tr>
		  	<td align="right" nowrap><span class="required">Report Display Name:</span></td>
		  	<td colspan="2">
		  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="ReportDisplayName"  VALUE="<%=ReportDisplayName%>">
		  	</td>
		</tr>
		<tr>
		  	<td align="right" nowrap><span class="required">Microsoft Access Query Name:</span></td>
		  	<td colspan="2">
		  		<input TYPE="text" SIZE="30" Maxlength="50" NAME="QueryName" VALUE="<%=QueryName%>">
		  	</td>
		</tr>
		<%if stepValue = "1" then%>
		<!--<input type="hidden" name="QueryName" value="<%=QueryName%>">-->
		<input type="hidden" name="QuerySQL" value="<%=ReportSQL%>">	
		<tr>
		  	<td align="right" nowrap><span class="required"># of Report Parameters:</span></td>
		  	<td colspan="2">
		  		<input TYPE="text" SIZE="5" Maxlength="50" NAME="NumParams" VALUE="<%=NumParams%>" onchange="DisplayWizardGUI(this.value);">
		  	</td>
		</tr>
		<%elseif stepValue = "2" then%>
		<tr>
		  	<td align="right" nowrap><span class="required"># of Report Parameters:</span></td>
		  	<td colspan="2">
		  		<input TYPE="text" SIZE="5" READONLY STYLE="background-color:#d3d3d3;" Maxlength="50" NAME="NumParams" VALUE="<%=NumParams%>">
		  	</td>
		</tr>
		<%end if%>
		<tr>
		  	<td align="right" nowrap valign="top">Report Description:</td>
		  	<td colspan="2">
		  		<textarea NAME="ReportDesc" rows="3" cols="45"><%=ReportDesc%></textarea>
		  	</td>
		</tr>
		<%if stepValue = "2" then%>
		<tr>
		  	<td align="right" nowrap valign="top">Report SQL Query:</td>
		  	<td colspan="2">
		  		<textarea NAME="QuerySQL" rows="10" cols="45"><%=ReportSQL%></textarea>
		  	</td>
		</tr>
			<%
			for i=1 to NumParams
				
				'-- initialize vars
				paramName = ""
				paramDisplayName = ""
				paramType = ""
				checked = ""
							
				'-- fill in as much or as parameter info as is already defined
				if not RS.EOF then
					paramName = RS("ParamName")
					paramDisplayName = RS("ParamDisplayName")
					isRequired = RS("isRequired")
					if isRequired = "1" then checked = "CHECKED"
					paramType = RS("ParamType")
					RS.MoveNext
				end if
			%>
			<tr>
			<td align="right"><span class="required">Parameter #<%=i%> Display Name:</span></td>
			<td>
				<input type="text" size="40" Maxlength="50" NAME="Parameter<%=i%>DisplayName" VALUE="<%=paramDisplayName%>">&nbsp;&nbsp;Required? <input type="checkbox" NAME="Parameter<%=i%>IsRequired" VALUE="1" <%=checked%>>
			</td>
			</tr>
			<tr>
				<td align="right" nowrap><span class="required">Parameter #<%=i%> Name (table.field):</span></td>
				<td>
					<input type="text" size="40" Maxlength="50" NAME="Parameter<%=i%>Name" VALUE="<%=paramName%>">
				</td>
			</tr>
			<tr>
				<td align="right">
					Parameter #<%=i%> Type:
				</td>
				<td><%=GetParamTypeSelect("Parameter" & i & "Type", paramType)%></td>
			</tr>
			<%next%>

		
		<%end if%>
		<%if stepValue = "1" then%>
		<tr><td colspan="2" align="right">
			<span id="wizardBottom">
			<a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;
			<a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			</span>
			<span id="nonWizardBottom" style="display:none;">
			<a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;&nbsp;&nbsp;&nbsp;
			<a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			</span>
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
</body>
</html>
<%if stepValue = "1" then%>
<script language="javascript">
DisplayWizardGUI(document.form1.NumParams.value);
</script>
<%end if%>
<%
Function GetParamTypeSelect(selectName, selectedValue)
	str = "<select NAME=""" & selectName & """>"
	str = str & "<option value=""num"""
	if selectedValue = "num" then str = str & " SELECTED"
	str = str & ">Number</option>"
	str = str & "<option value=""text"""
	if selectedValue = "text" then str = str & " SELECTED"
	str = str & ">Text</option>"
	str = str & "<option value=""start_date"""
	if selectedValue = "start_date" then str = str & " SELECTED"
	str = str & ">Date Range Start Date</option>"
	str = str & "<option value=""end_date"""
	if selectedValue = "end_date" then str = str & " SELECTED"
	str = str & ">Date Range End Date</option>"
	str = str & "<option value=""user_name"""
	if selectedValue = "user_name" then str = str & " SELECTED"
	str = str & ">Username</option>"
	str = str & "<option value=""location"""
	if selectedValue = "location" then str = str & " SELECTED"
	str = str & ">Location</option>"
	str = str & "</select>"
	GetParamTypeSelect = str
end function
%>
