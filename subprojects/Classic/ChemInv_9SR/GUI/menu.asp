<%
Response.ExpiresAbsolute = Now()
Session("isSearchMode") = False
dbkey = "ChemInv"
if request("clear") then
	Session("GUIReturnURL") = ""
	Response.Write "<SCRIPT language=javascript>window.close();</script>"
Else
	Session("GUIReturnURL") = "/cheminv/gui/menu.asp"
end if
%>
<!-- #include file="../../cs_security/variables.asp" -->
<!-- #include file="../custom/gui/custom_menu.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->

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
		<style type="text/css">
		td
		{
			font-size: 8pt;
		    font-family: verdana,arial,helvetica
		}		
		</style>
	</head>
	<body bgcolor="#FFFFFF" xonunload="document.form1.submit()">
	<div align="center">
	
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
						<a class="MenuLink" HREF="Search%20Inventory" ONCLICK="OpenDialog('../inputtoggle.asp?dataaction=db&amp;dbname=cheminv', 'SearchDiag', 2); return false;"><img SRC="../graphics/btn_search.gif" border="0" width="15" height="14"></a>
					</td><td valign="middle">&nbsp;
						<a class="MenuLink" HREF="Search%20Inventory" ONCLICK="OpenDialog('../inputtoggle.asp?dataaction=db&amp;dbname=cheminv', 'SearchDiag', 2); return false;"><strong>Search Inventory</strong></a>
					</td></tr></table>
				</td></tr><tr><td>
					<table border="0" cellpadding="10"><tr><td colspan="3">
						<strong>Locations</strong> »&nbsp;
						<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
							<a class="MenuLink" HREF="NewLocation.asp?ReturnURL=menu.asp">New</a> |
						<%End if%>
						<%If Session("INV_EDIT_LOCATION" & dbkey) then%>						
							<a class="MenuLink" HREF="NewLocation.asp?GetData=db&amp;ReturnURL=menu.asp">Edit</a> |
						<%End if%>
						<%If Session("INV_MOVE_LOCATION" & dbkey) then%>
							<a class="MenuLink" HREF="MoveLocation.asp?ReturnURL=menu.asp">Move</a> |
						<%End if%>
						<%If Session("INV_DELETE_LOCATION" & dbkey) then%>						
							<a class="MenuLink" HREF="DeleteLocation.asp?ReturnURL=menu.asp" title="Delete location">Delete</a>
						<%End if%>
					</td></tr><tr><td valign="top">
						<strong>Inventory Management</strong><br /><br />
						<%if true then%>
							<a class="MenuLink" HREF="/cheminv/gui/manageTables.asp?dbkey=cheminv">Manage Tables</a><br />
						<%End if%>						
						<%If Session("INV_CREATE_LOCATION" & dbkey) then%>
							<a class="MenuLink" href="/cheminv/gui/ManageReports.asp">Manage Reports</a><br />
						<%End if%>
						<%If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/gui/AuditReport_frset.asp">Analyze Audit Trail</a><br />
						<%End if%>
						<% Response.write(RenderCustomInventoryMenu()) %>
					</td><td width="10">&nbsp;</td><td valign="top">
						<strong>Security Management</strong><br /><br />
						<%if Session("CSS_CHANGE_PASSWORD" & dbkey) then%>
							<a class="MenuLink" href="#" onclick="OpenDialog('/cheminv/cs_security/Password.asp?PrivTableName=CHEMINV_PRIVILEGES&amp;dbkey=cheminv', 'Mpwd', 2); return false">Change Password</a><br />
						<%End if%>
						<%if Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/cs_security/manageUsers.asp?PrivTableName=CHEMINV_PRIVILEGES&amp;dbkey=cheminv">Manage Users</a><br />
						<%End if%>
						<%if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/cs_security/manageRoles.asp?PrivTableName=CHEMINV_PRIVILEGES&amp;dbkey=cheminv">Manage Roles</a><br />
						<%End if%>
						<% Response.write(RenderCustomSecurityMenu()) %>
						<br />
					</td></tr>
					<tr><td valign="top">
						<strong>Container Management</strong><br /><br />
						<%if Application("ShowCertify") and Session("INV_APPROVE_CONTAINER" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/gui/ManageApprovals_frset.asp?filter=true">Manage Approvals</a><br />
						<%End if%>
						<%if Application("AllowRequests") then%>
							<a class="MenuLink" HREF="/cheminv/gui/manageRequests_frset.asp?filter=true">Manage Container Requests</a><br />
						<%End if%>
						<%if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then%>
							<a class="MenuLink" HREF="/cheminv/gui/ManageSampleRequests_frset.asp?filter=true&amp;RequestTypeID=2&amp;RequestStatusID=2">Manage Sample Requests</a><br />
							<a class="MenuLink" HREF="/Cheminv/GUI/ManageOrders_frset.asp?filter=true">Manage Orders</a><br />
						<%End if%>
						<% Response.write(RenderCustomContainerMenu()) %>
					</td><td width="10">&nbsp;</td><td valign="top">
						<strong>Plate Management</strong><br /><br />
						<%if Application("PLATES_ENABLED") AND Session("INV_CREATE_PLATE" & dbkey) then%>
							<a class="MenuLink" HREF="PlateSettings.asp?ReturnURL=menu.asp">Plate Settings</a><br />
							<!-- Create Plates from Excel and Pipette Log -->
							<a class="MenuLink" href="/cheminv/gui/CreatePlatesFromExcel.asp?ReturnURL=menu.asp">Create Plates from Excel</a><br />
							<a class="MenuLink" HREF="/cheminv/gui/CreatePlatesFromTextFile.asp?ReturnURL=menu.asp">Create Plates from Text File</a><br />							
						<%End if%>
						<% Response.write(RenderCustomPlateMenu()) %>
					</td></tr>
					</table>
				</td></tr><tr><td bgcolor="#e1e1e1">&nbsp;&nbsp;
					<a class="MenuLink" href="View%20Help" ONCLICK="OpenDialog('/cheminv/help/help.asp','HelpDiag',1); return false;"><strong>Help</strong></a> | 
					<a class="MenuLink" href="/cheminv/logoff.asp" target="_top"><strong>Log Off</strong></a>
				</td></tr><tr><td align="right">
					<a href="#" onclick="document.form1.submit(); opener.focus();return false;"><img SRC="../graphics/close_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				</td></tr>
				</table>
						
					
				</td>
			</tr>
		</table>
	<form method="get" name="form1">
		<input type="hidden" name="clear" value="1">
	</form>			

	</div>
		
	</body>
</html>

