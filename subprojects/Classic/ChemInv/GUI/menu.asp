<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%
if not Session("IsFirstRequest") then 
	StoreASPSessionID()
else
	Session("IsFirstRequest") = false
	Response.Write("<html><body onload=""window.location.reload()"">&nbsp;<form name=""form1""></form></body></html>")	
end if

Dim Conn
Response.ExpiresAbsolute = Now()
Session("isSearchMode") = False
dbkey = "ChemInv"
if request("clear") then
	Session("GUIReturnURL") = ""
	Response.Write "<SCRIPT language=javascript>window.close();</script>"
Else
	Session("GUIReturnURL") = "/cheminv/gui/menu.asp"
end if

'-- determine if RLS is enabled
GetInvConnection()
SQL = "SELECT value FROM " & Application("OraSchemaName") & ".globals WHERE id = 'RLS_ENABLED'"
SET rsRLS = Conn.Execute(SQL)
rlsEnabled = rsRLS("value")

%>
<!-- #include file="../../cs_security/variables.asp" -->
<!-- #include file="../custom/gui/custom_menu.asp" -->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title><%=Application("appTitle")%> -- Administrative Menu</title>
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
		<script language="JavaScript">
		window.focus()
		var DialogWindow;
		</script>
	</head>
	<body bgcolor="#FFFFFF" xonunload="document.form1.submit()">
	<div align="center">
	<form name="form2" method="post">
	
		<table border="0" cellspacing="0" cellpadding="10">
			<tr>
				<td valign="top"><img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0"></td>
				<td align="right">
					<%If Application("Admin_required") then%>
					<br><br><font face="Arial" color="#42426f" size="1"><b>Current login:&nbsp;<%=Ucase(Session("UserNameChemInv"))%></b></font>
					<%End if%>
				</td>
			</tr>
			<tr>
				<td colspan="2" align="left">

				<table border="0" width="425" cellpadding="4"><tr><td bgcolor="#e1e1e1">
					<table cellpadding="0" cellspacing="0"><tr><td valign="middle">&nbsp;&nbsp;
						<a class="MenuLink" HREF="Search%20Inventory" ONCLICK="OpenDialog('../inputtoggle.asp?dataaction=db&amp;dbname=cheminv', 'SearchDiag', 2); return false;"><img SRC="../graphics/btn_search.gif" border="0" WIDTH="15" HEIGHT="14"></a>
					</td><td valign="middle">&nbsp;
						<a class="MenuLink" HREF="Search%20Inventory" ONCLICK="OpenDialog('../inputtoggle.asp?dataaction=db&amp;dbname=cheminv', 'SearchDiag', 2); return false;"><strong>Search Inventory</strong></a>
					</td></tr></table>
				</td></tr><tr><td>
					<table border="0" cellpadding="5" width="100%"><!--<tr><td colspan="3">						<strong>Locations</strong> »&nbsp;						<%If Session("INV_CREATE_LOCATION" & dbkey) then%>							<a class="MenuLink" HREF="NewLocation.asp?ReturnURL=menu.asp">New</a> |						<%End if%>						<%If Session("INV_EDIT_LOCATION" & dbkey) then%>													<a class="MenuLink" HREF="NewLocation.asp?GetData=db&amp;ReturnURL=menu.asp">Edit</a> |						<%End if%>						<%If Session("INV_MOVE_LOCATION" & dbkey) then%>							<a class="MenuLink" HREF="MoveLocation.asp?ReturnURL=menu.asp">Move</a> |						<%End if%>						<%If Session("INV_DELETE_LOCATION" & dbkey) then%>													<a class="MenuLink" HREF="DeleteLocation.asp?ReturnURL=menu.asp" title="Delete location">Delete</a>						<%End if%>					</td></tr>--><tr><td valign="top">
						<div class="tasktitle">Inventory Management</div>
						<%if true then%>
							<a class="MenuLink" HREF="/cheminv/gui/manageTables.asp?dbkey=cheminv">Manage Tables</a><br />
						<%End if%>						
						<%if true then%>
							<a class="MenuLink" HREF="/cheminv/gui/ManageBatchFields.asp">Manage Grouping Fields</a><br /> 
						<%End if%>				
						<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
							<a class="MenuLink" href="/cheminv/gui/ManageReports.asp">Manage Reports</a><br />
						<%End if%>
						<%If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/gui/AuditReport_frset.asp">Analyze Audit Trail</a><br />
						<%End if%>
						<% Response.write(RenderCustomInventoryMenu()) %>
					</td><td width="10">&nbsp;</td><td valign="top">
						<div class="tasktitle">Security Management</div>
						<%if Session("CSS_CHANGE_PASSWORD" & dbkey) then%>
							<a class="MenuLink" href="#" onclick="OpenDialog('/COEManager/Forms/SecurityManager/ContentArea/ChangePassword.aspx?appName=INVENTORY', 'Mpwd', 2); return false">Change Password</a><br />
						<%End if%>
						<%if Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey) then%>
							<a class="MenuLink" HREF="/COEManager/Forms/SecurityManager/ContentArea/ManageUsers.aspx?appName=INVENTORY">Manage Users</a><br />
						<%End if%>
						<%if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>
							<a class="MenuLink" HREF="/COEManager/Forms/SecurityManager/ContentArea/ManageRoles.aspx?appName=INVENTORY">Manage Roles</a><br />
						<%End if%>
						<%if rlsEnabled = "1" and (Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey)) then%>
							<a class="MenuLink" HREF="/cheminv/GUI/ManageRoleLocations.asp?refresh=1">Manage Role Locations</a><br />
						<%End if%>
						<% Response.write(RenderCustomSecurityMenu()) %>
					</td></tr>
					<tr><td valign="top">
						<div class="tasktitle">Container Management</div>
						<%if Application("ShowCertify") and Session("INV_APPROVE_CONTAINER" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/gui/ManageApprovals_frset.asp?filter=true">Manage Approvals</a><br />
						<%End if%>
						<%if Application("AllowRequests") then%>
							<a class="MenuLink" HREF="/cheminv/gui/manageRequests_frset.asp?filter=true">Manage Requests</a><br />
						<%End if%>
						
						<%'-- CSBR-138430-SMathur- Changed the permission of manage sample link.
						if Application("ShowRequestSample") and Session("INV_SAMPLE_APPROVE" & dbkey) or Session("INV_SAMPLE_DISPENSE" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/gui/ManageSampleRequests_frset.asp?filter=true&amp;RequestTypeID=2&amp;RequestStatusID=2">Manage Sample Requests</a><br />
							<a class="MenuLink" HREF="/Cheminv/GUI/ManageOrders_frset.asp?filter=true">Manage Orders</a><br />
						<%End if%>
						<%If  Session("INV_ORDER_CONTAINER" & dbkey) and  Session("INV_MANAGE_SUBSTANCES" & dbkey) then%>
						<a class="MenuLink" HREF="/cheminv/gui/receiving_frset.asp">Manage Container Receiving</a><br />
						<%End if%>
						<% Response.write(RenderCustomContainerMenu()) %>
					</td><td width="10">&nbsp;</td><td valign="top">
						<div class="tasktitle">Plate Management</div>
						<%if Application("PLATES_ENABLED") AND Session("INV_CREATE_PLATE" & dbkey) then%>
							<a class="MenuLink" HREF="PlateSettings.asp?ReturnURL=menu.asp">Plate Settings</a><br />
							<!-- Create Plates from Excel and Pipette Log -->
							<a class="MenuLink" href="/cheminv/gui/CreatePlatesFromExcel.asp?ReturnURL=menu.asp">Create Plates from Excel</a><br />
							<a class="MenuLink" HREF="/cheminv/gui/CreatePlatesFromTextFile.asp?ReturnURL=menu.asp">Create Plates from Text File</a><br />							
						<%End if%>
						<% Response.write(RenderCustomPlateMenu()) %>
					</td></tr>
					
					<tr><td valign="top" colspan="3">
						<div class="tasktitle">Grid Management</div>
						<script LANGUAGE="javascript">
							window.focus();
							function ValidateClick(elmName, url, action){
								if ((document.form2[elmName].value == "") && (action != "create")){
									alert("Please select an item to " + action + " from the list.")
								}
								else{
									document.form2.action = url + "?action=" + action + "&ID=" + document.form2[elmName].value+"&GridType=";
									document.form2.submit();
								}
							}
						</script>
						<table cellpadding="0" cellspacing="0">
						<tr><td align="right" nowrap>
								<span title="define a grid of sublocations where you can store inventory containers or plates">Grid Formats:&nbsp;</span>
						</td><td>
								<%=ShowSelectBox2("GridFormatID", "", "SELECT Grid_Format_ID AS Value, Name AS DisplayText FROM inv_Grid_Format WHERE Grid_Format_Type_FK = 10 ORDER BY Name ASC", 45, RepeatString(30, "&nbsp;"), "")%>
								<%=GetNewEditDelLinks("GridFormatID", "ManageGridFormat.asp")%>
						</td></tr>
						</table>
						<!--						<tr><td colspan="2">&nbsp;</td></tr>						<tr><td colspan="2">							<a class="MenuLink" href="NewLocation.asp?LocationType=rack">Create New Rack</a> | 							<a class="MenuLink" href="NewLocation.asp?LocationType=rack&amp;GetData=db">Edit Rack</a> |							<a class="MenuLink" href="MoveLocation.asp?LocationType=rack">Move Rack</a> |							<a class="MenuLink" href="DeleteLocation.asp?LocationType=rack">Delete Rack</a>						</td></tr>						-->
					</td></tr>
				 
				 <% if Session("INV_APPROVE_CONTAINER" & dbkey) then%>
					<tr><td valign="top" colspan="3">
						<div class="tasktitle">Organization Management</div>
						<script language="javascript">
							window.focus();
							function ValidateClick(elmName, url, action){
								if ((document.form2[elmName].value == "") && (action != "create")){
									alert("Please select an item to " + action + " from the list.")
								}
								else{
									document.form2.action = url + "?action=" + action + "&ID=" + document.form2[elmName].value;
									document.form2.submit();
								}
							}
						</script>
						<table cellpadding="0" cellspacing="0">
						<tr><td align="right" nowrap>
								<span title="Define organizations">Organizations:&nbsp;</span>
						</td><td>
								<%=ShowSelectBox2("OrgUnitID", "", "SELECT Org_Unit_ID AS Value, Org_Name AS DisplayText FROM INV_ORG_UNIT ORDER BY Org_Name ASC", 45, RepeatString(30, "&nbsp;"), "")%>
								<%=GetNewEditDelLinks("OrgUnitID", "ManageOrganization.asp")%>
						</td></tr>
						</table>
					</td></tr>
					<% end if %>
					
					<!--					<tr><td>						<%If Application("RACKS_ENABLED") then%>						<div class="tasktitle">Rack Management</div>						<% end if %>					</td><td>&nbsp;</td><td valign="top">					</td></tr>					-->
					</table>
				</td></tr><tr><td bgcolor="#e1e1e1">&nbsp;&nbsp;
					<a class="MenuLink" href="View%20Help" ONCLICK="OpenDialog('<%=Application("HelpFile")%>','HelpDiag',1); return false;"><strong>Help</strong></a> | 
					<a class="MenuLink" href="/cheminv/logoff.asp" target="_top"><strong>Log Off</strong></a>
				</td></tr><tr><td align="right">
					<a href="#" onclick="document.form1.submit(); opener.focus();return false;"><img SRC="/cheminv/graphics/sq_btn/close_dialog_btn.gif" border="0"></a>
				</td></tr>
				</table>
						
					
				</td>
			</tr>
		</table>
	</form>
	<form method="get" name="form1">
		<input type="hidden" name="clear" value="1">
	</form>			

	</div>
		
	</body>
</html>

