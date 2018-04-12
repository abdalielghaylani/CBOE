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
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft FrontPage 5.0">
<title><%=Application("appTitle")%> -- Audit Report</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
</head>

<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
<script language="javascript">
function appDisplay(appValue) {
	if (appValue == 'inv') {
		document.all.inv_tables.style.display = 'block';
	}
	else {
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
</script>
<!--End of SYAN modification-->


<body>

<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
	<tr>
		<td valign="top" align="left">
			<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
		</td>
		<td align="right" valign="top">
			<a class="MenuLink" target="_top" href="AuditReport_frset.asp?ft=std&CompoundID=<%=Request("CompoundID")%>">Standard</a> |
			<a class="MenuLink" target="_top" href="#" onclick="return false" disabled>Aggregate</a>
		</td>
	</tr>
	<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
	<tr><td colspan="2">
			<input type="radio" name="appSwitch" checked value="inv" onclick="appDisplay(this.value)">Inventory
			<%if false then%><input type="radio" name="appSwitch" value="cs_security" onclick="appDisplay(this.value)">CS_SECURITY<%end if%>
		</td>
	</tr>
	<!--End of SYAN modification-->
	
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="AuditReport_Aggregate_display.asp" method="POST">
<!--SYAN added on 1/20/2005 to fix CSBR-50830-->
	<input type="hidden" name="app">
<!--End of SYAN modification-->

<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Aggregate Audit Reports</b>		
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
										<option value="INV_CUSTOM_CPD_FIELD_VALUES">Compound Custom Field Values</option>										
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
						<table border="0">
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
						<%call ShowInputField("", "", "fromDate:form1:" & LastWeek, "DATE_PICKER:TEXT", "15")%>							
						<!--<input type="text" name="fromDate" size="11" value="<%=LastWeek%>"><a href onclick="return PopUpDate(&quot;fromDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
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
						<%call ShowInputField("", "", "toDate:form1:" & Today , "DATE_PICKER:TEXT", "15")%>
						<!--<input type="text" name="toDate" size="11" value="<%=Today%>"><a href onclick="return PopUpDate(&quot;toDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Action:
					</td>
					<td valign="top">
						<select name="action">
							<option value selected>
							<option value="U">Update
							<option value="D">Delete
							<option value="I">Insert
						</select>
					</td>
					<td align="right" valign="top" nowrap>
						&nbsp;
					</td>
					<td align="right">
						&nbsp;
					</td>
				</tr>
			</table>
		</td>
		
		<td rowspan="3" valign="top">
			<table>	
					<tr><th>Group by Fields</th></tr>
					<tr><td><input type="checkbox" name="grpByTableName" value="1">&nbsp;Table Name</td></tr>
					<tr><td><input type="checkbox" name="grpByUserName" value="1" checked>&nbsp;Modified by</td></tr>
					<tr><td><input type="checkbox" name="grpByAction" value="1">&nbsp;Action</td></tr>
			</table>
		</td>
		<td rowspan="3" valign="top">
			<table>
				<tr>
					<th colspan="4" align="center">
						Group by Date				
					</th>
				</tr>
				<tr>
					<td>
					<input type="radio" name="grpByDate" value="H">&nbsp;Hour<br>
					<input type="radio" name="grpByDate" value="D" checked>&nbsp;Day<br>
					<input type="radio" name="grpByDate" value="M">&nbsp;Month<br>
					<input type="radio" name="grpByDate" value="Y">&nbsp;Year<br>
					</td>
				</tr>
			</table>
		</td>
		
		<td rowspan="3" valign="bottom" align="right">
			<input type="button" value="Reset" onclick="top.location.reload()">
			<input type="submit" value="Filter" id="submit1" name="submit1">
			<br>
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
</table>
-->
</form>
</body>
</html>