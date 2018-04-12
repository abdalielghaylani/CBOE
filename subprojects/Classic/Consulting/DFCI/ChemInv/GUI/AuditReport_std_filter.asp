<%@ Language=VBScript %>
<%
Dim Conn
Dim Cmd
Dim RS
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft FrontPage 5.0">
<title><%=Application("appTitle")%> -- Audit Report</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
<script language="javascript">
function appDisplay(appValue) {
	if (appValue == 'inv') {
		document.all.inv_filter.style.display = 'block';
		document.all.inv_tables.style.display = 'block';
	}
	else {
		document.all.inv_filter.style.display = 'none';
		document.all.inv_tables.style.display='none';
		
	}
	
	if (appValue == 'docmanager') {
		document.all.docmanager_tables.style.display = 'block';
	}
	else {
		document.all.docmanager_tables.style.display = 'none';
	}
	
	if (appValue == 'cs_security') {
		document.all.cs_security_tables.style.display = 'block';
	}
	else {
		document.all.cs_security_tables.style.display = 'none';
	}
	
	document.all.app.value = appValue;
}

function Validate() {
	var bWriteError = false;
	var errmsg = "Please fix the following problems:\r";

	// Low RID if present must be a number
	if (document.form1.lo_rid.value.length >0 && !isPositiveNumber(document.form1.lo_rid.value)){
		errmsg = errmsg + "- Starting RID must be a positive number.\r";
		bWriteError = true;
	}
	// High RID if present must be a number
	if (document.form1.hi_rid.value.length >0 && !isPositiveNumber(document.form1.hi_rid.value)){
		errmsg = errmsg + "- Ending RID must be a positive number.\r";
		bWriteError = true;
	}
	// CompoundID if present must be a number
	if (document.form1.compoundID.value.length >0 && !isPositiveNumber(document.form1.compoundID.value)){
		errmsg = errmsg + "- CompoundID must be a positive number.\r";
		bWriteError = true;
	}
	// From Date must be a date
	if (document.form1.fromDate.value.length > 0 && !isDate(document.form1.fromDate.value)){
		errmsg = errmsg + "- From Date must be in " + dateFormatString + " format.\r";
		bWriteError = true;
	}
	// To Date must be a date
	if (document.form1.toDate.value.length > 0 && !isDate(document.form1.toDate.value)){
		errmsg = errmsg + "- To Date must be in " + dateFormatString + " format.\r";
		bWriteError = true;
	}
	// Report problems
	if (bWriteError){
		alert(errmsg);
	}
	else{
		document.form1.submit();
	}
}
</script>
<!--End of SYAN modification-->

</head>
<body>

<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
	<tr>
		<td valign="top" align="left">
			<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
		</td>
		<td align="right" valign="top">
			<a class="MenuLink" target="_top" href="AuditReport_frset.asp?ft=std" disabled>Standard</a> |
			<a class="MenuLink" target="_top" href="AuditReport_frset.asp?ft=Aggregate">Aggregate</a>
		</td>
	</tr>
	
	<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
		<tr><td colspan="2"><input type="radio" name="appSwitch" checked value="inv" onclick="appDisplay(this.value)">Inventory
				<%if ucase(Application("Docmanager_server_name")) <> "NULL" then%><input type="radio" name="appSwitch" value="docmanager" onclick="appDisplay(this.value)">Docmanager<%end if%>
				<%if false then%><input type="radio" name="appSwitch" value="cs_security" onclick="appDisplay(this.value)">CS_SECURITY<%end if%>
			</td>
		</tr>
	<!--End of SYAN modification-->
</table>


<br clear="all">
<form name="form1" target="DisplayFrame" action="AuditReport_std_display.asp" method="POST">
<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
	<input type="hidden" name="app">
<!--End of SYAN modification-->

<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Standard Audit Reports</b>		
		</td>
	</tr>
	<tr>
		<td rowspan="3" valign="top">
			<table>
				<tr>
					<th colspan="4" align="center">
						Filter Criteria					
					</th>
				</tr>
				<tr>
					<td align="left" valign="top" nowrap colspan="2">
						<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
						<span id="inv_tables" style="display=block">
						<table border="0">
							<tr>
								<td>
									Table:
								</td>
								
								<td valign="top">
									<select name="tableName">
										<option value selected>  
										<option value="inv_containers">Containers</option>
										<option value="inv_locations">Locations</option>
										<option value="inv_compounds">Compounds</option>
										<option value="inv_url">Links</option>
										<option value="inv_requests">Requests</option>
										<option value="inv_suppliers">Suppliers</option>
										<option value="inv_orders">Orders</option>
										<option value="inv_order_containers">Order Containers</option>
										<option value="inv_container_checkin_details">Container Checkin Details</option>
										<option value="inv_synonyms">Synonyms</option>
										</select>
								</td>
							</tr>
						</table>
						</span>
						
						<span id="docmanager_tables" style="display=none">
						<table>
							<tr>
								<td>
									Table:
								</td>
								
								<td valign="top">
									<select name="docmgr_tableName">
										<option value selected>
										<option value="docmgr_external_links">External Links</option>
										<option value="docmgr_documents">Documents</option>
										</select>
								</td>
							</tr>
						</table>
						</span>

						<!--End of SYAN modification-->
						<span id="cs_security_tables" style="display=none">
						<table>
							<tr>
								<td>
									Table:
								</td>
								
								<td valign="top">
									<select name="css_tableName">
										<option value selected>
										<option value="cheminv_privileges">Inventory Privileges</option>
										<option value="cs_security_privileges">CS Security Privileges</option>
										<option value="docmanager_privileges">DocManager Privileges</option>
										<option value="people">People</option>
										<option value="security_roles">Security Roles</option>
										<option value="login">Login</option>
										<option value="logout">Logout</option>
										</select>
								</td>
							</tr>
						</table>
						</span>
					</td>
					
					<td align="right" valign="top" nowrap>
						From Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "fromDate:form1", "DATE_PICKER:TEXT", "15")%>
						<!--<input type="text" name="fromDate" size="11" value><a href onclick="return PopUpDate(&quot;fromDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						User:
					</td>
					<td valign="top">
						<%=ShowSelectBox2("userID", "", "SELECT DISTINCT CS_SECURITY.People.User_ID AS Value, CS_SECURITY.People.Last_Name||DECODE(CS_SECURITY.People.First_Name, NULL, '', ', '||CS_SECURITY.People.First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(DisplayText) ASC", 40, RepeatString(18, "&nbsp;"), "")%>
					</td>
					<td align="right" valign="top" nowrap>
						To Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "toDate:form1", "DATE_PICKER:TEXT", "15")%>
						<!--<input type="text" name="toDate" size="11" value><a href onclick="return PopUpDate(&quot;toDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						RID:
					</td>
					<td align="right">
						<input type="text" name="lo_rid" size="8">&nbsp;-&nbsp;<input type="text" name="hi_rid" size="8">
					</td>
					<td align="right" valign="top" nowrap>
						Action:
					</td>
					<td valign="top">
						<select name="action" onchange="(this.value == 'U') ? document.all.updateCriteria_ctrl.style.display = 'block' :document.all.updateCriteria_ctrl.style.display = 'none' ">
							<option value selected>
							<option value="U">Update
							<option value="D">Delete
							<option value="I">Insert
						</select>
					</td>
				</tr>
				
				<tr><td colspan="4">
					<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
					<span id="inv_filter" style="display=block">
					<!--End of SYAN modification-->
					<table border="0">
							<tr>
								<td align="right" valign="top" nowrap>
									LocationID:
								</td>
								<td valign="top">
									<%=GetBarcodeIcon()%><input type="text" name="locationBarcode" value="<%=Request("locationBarcode")%>" size="8">
								</td>
								<td align="right" valign="top" nowrap>
									CompoundID:
								</td>
								<td valign="top">
									<input type="text" name="compoundID" value="<%=Request("CompoundID")%>" size="8">
								</td>
							</tr>
							<tr>
								<td align="right" valign="top" nowrap>
									ContainerID:
								</td>
								<td valign="top">
									<%=GetBarcodeIcon()%><input type="text" name="containerBarcode" value="<%=Request("containerBarcode")%>" size="8">
								</td>
								<td align="right" valign="top" nowrap>
									&nbsp;
								</td>
								<td valign="top">
									&nbsp;
								</td>
							</tr>
						</table>
					</span>
				</tr>
			</table>
		</td>
		
		<td rowspan="3" valign="top">
			<span id="updateCriteria_ctrl" style="display=none">
			<table>
				<tr>
					<th colspan="4" align="center">
						Update Criteria					
					</th>
				</tr>	
				<tr>
					<td align="right">Column Name:</td>
					<td><input type="text" size="15" name="column_name"></td>
				</tr>	
				<tr>
					<td align="right">Old value:</td>
					<td><input type="text" size="15" name="old_value"></td>
				</tr>
				<tr>
					<td align="right">New value:</td>
					<td><input type="text" size="15" name="new_value"></td>
				</tr>	
			</table>
			</span>
		</td>
		<td rowspan="3" valign="bottom" align="right">
			<input type="button" value="Reset" onclick="top.location.reload()" id="button1" name="button1">
			<input type="button" value="Filter" onclick="Validate();return false;" id="submit1" name="submit1">
			<br><br>
			<%=GetCancelButton()%>
		</td>
	</tr>

</table>
<!--
<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
	<tr>
		<td align="right" valign="top">
			<a class="MenuLink" target="_new" href="/cheminv/Gui/CreateReport_frset.asp?isCustomReport=1&amp;ReportTypeID=8">View Audit Report</a> 
		</td>
	</tr>
</table> -->
</form>
</body>
</html>