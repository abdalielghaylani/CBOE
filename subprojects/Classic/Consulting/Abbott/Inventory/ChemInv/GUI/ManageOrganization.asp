<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

action = Request("action")
p_OrgUnitID = Request("ID")


If action <> "create" then
	Call GetInvConnection()
	SQL = "SELECT org_unit_id, org_name, org_type_id_fk FROM inv_org_unit WHERE ORG_UNIT_ID=" & p_OrgUnitID
	Set RS= Conn.Execute(SQL)
	OrgUnitID = RS("org_unit_id")
	OrgName = Replace(RS("org_name"),"""","&quot;")
	OrgType = RS("org_type_id_fk")
Else
	OrgUnitID = 0
End if

%>
<html>
<head>
<title>Manage Organizations</title>
<script language="javascript" src="/cheminv/choosecss.js"></script>
<script language="javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript" src="/cheminv/utils.js"></script>

<script language="JavaScript">
<!--
	var currGridFormat = "<%=currGridFormat%>";
	window.focus();
	
	function Validate(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
	<%if action <> "delete" then%>
		// Name is required
		if (document.form1.p_Name.value.length == 0) {
			errmsg = errmsg + "- Organization name is required.\r";
			bWriteError = true;
		} else {
			var dbName = "<%=OrgName%>";
			if (document.form1.p_Name.value != dbName){
				if (ValidateUniqueName('organization',document.form1.p_Name.value) >= 1) {
					errmsg = errmsg + "- The Name already exists, please enter a different name.\r";
					bWriteError = true;
				}
			}
		}
		
		// Organization Type is required
		if (document.form1.p_OrgType.options[document.form1.p_OrgType.selectedIndex].value == 0) {
			errmsg = errmsg + "- Organization type is required.\r";
			bWriteError = true;
		}
		vMemberList = "";
		for (i=0; i < document.form1.selectedUsers.length; i++){
			if (document.form1.selectedUsers[i].value != "NULL"){
				if (vMemberList.length > 0) vMemberList = vMemberList + ","
				vMemberList = vMemberList + document.form1.selectedUsers[i].value
			}
		}
		document.form1.p_Users.value = vMemberList;
		<%end if%>
		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
	
	function appendOptionLast(num)
	{
		
		if (document.form1.users.selectedIndex==-1) {
			alert("Please select at least one user to add.");
		}else{
			var bSetValue = true;
			for (j=0; j < document.form1.users.length; j++){
				var elOptNew = document.createElement('option');
				var elSel = document.getElementById('selectedUsers');
				if (document.form1.users[j].selected){
					for (i=0; i < document.form1.selectedUsers.length; i++){			
						if (document.form1.users.options[j].value == document.form1.selectedUsers.options[i].value){
							bSetValue = false;
						}
						if (document.form1.selectedUsers.options[i].value == "NULL"){
							document.form1.selectedUsers.remove(i);
						}
					}
					if (bSetValue){
						elOptNew.text = document.form1.users.options[j].text;
						elOptNew.value = document.form1.users.options[j].value;
						elSel.add(elOptNew);
						bSetValue = true;
					}else{
						alert("The selected user already exists");
					}
				}
			}
		}

	}
	function removeOptionLast()
	{

		if (document.form1.selectedUsers.selectedIndex==-1) {
			alert("Please select at least one user to remove.");
		}else{
			var bSetValue = true;
			for (i=0; i < document.form1.selectedUsers.length; i++){
				if (document.form1.selectedUsers[i].selected){
					document.form1.selectedUsers.remove(i);
				}
			}
		}
		if(document.form1.selectedUsers.length == 0){
			var elOptNew = document.createElement('option');
			var elSel = document.getElementById('selectedUsers');
			elOptNew.text = "No Members";
			elOptNew.value = "NULL";
			elSel.add(elOptNew);
		}

	}

//-->
</script>

</head>

<body>
<center>
<form name="form1" action="ManageOrganization_action.asp?action=<%=action%>" method="post">
<input type="hidden" name="p_OrgUnitID" value="<%=p_OrgUnitID%>" />
<input type="hidden" name="p_Users" value />
<input type="hidden" name="p_Roles" value />

<table border="0">
<%if action = "delete" then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Are you sure you want to delete this organization?:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Organization Name:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="25" Maxlength="50" NAME="p_Name" value="<%=OrgName%>" disabled>
		</td>
	</tr>

<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Organization Details:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Organization Name:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="25" Maxlength="50" NAME="p_Name" value="<%=OrgName%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Organization Type:<span>
		</td>
		<td>
			<%=ShowSelectBox2("p_OrgType", OrgType, "SELECT Enum_ID AS Value, Enum_Value AS DisplayText FROM inv_enumeration WHERE Eset_ID_FK=10 ORDER BY Enum_Value", 55, "", "")%>
		</td>
	</tr>
	<tr>
		<td colspan="2">
		<table border="0" cellpadding="5">
		<tr><td colspan="3">Users associated with organization</td></tr>
		<tr>
		<%=ShowPickList3("", "users", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText, '' as DefaultValue FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 55, "", "", "", 6, true, "")%>
		<td align="center">
			<a href="javascript:appendOptionLast()"><img src="/cheminv/graphics/sq_btn/add_role_sq_btn.gif" border="0" /></a><br />
			<a href="javascript:removeOptionLast()"><img src="/cheminv/graphics/sq_btn/remove_role_btn.gif" border="0" /></a><br />
		</td>
		<%=ShowPickList3("", "selectedUsers", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText, '' as DefaultValue FROM CS_SECURITY.People Where User_ID in (SELECT USER_ID_FK FROM INV_ORG_USERS Where Org_Unit_ID_FK=" & OrgUnitID & ") ORDER BY lower(Last_Name) ASC", 55, "", "", "", 6, true, "No Members")%>
		</tr>
		</table>
		</td>
	</tr>
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="history.back(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a href="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>	
</form>
</center>

</body>
</html>
