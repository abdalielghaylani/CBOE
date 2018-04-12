<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint 
bDebugPrint = false

TableName = Request("TableName")
TablePKID = Request("TablePKID")
AddressID = Request("AddressID")

' for new table entry (like a new location) then don't insert the address, just capture the information and update the opener form
bNewTableEntry = false
if TablePKID = "" then bNewTableEntry = true
bNewAddress = true
if AddressID <> "" then bNewAddress = false

actionText = "Add"
if not bNewAddress then actionText = "Edit"
instructions = actionText & " an address"

if not bNewTableEntry then
	'Get supplier or address name
	GetInvConnection()	
	if TableName="inv_suppliers" then
	  SQL = "SELECT SUPPLIER_NAME FROM INV_SUPPLIERS WHERE supplier_id = " & TablePKID
	  Set RS = Conn.Execute(SQL)
	  instructions = actionText & " the address for supplier " & RS("SUPPLIER_NAME")
	  Set RS = Nothing 
	elseif TableName="inv_locations" then
	  SQL = "SELECT LOCATION_NAME FROM INV_LOCATIONS WHERE location_id = " & TablePKID
	  Set RS = Conn.Execute(SQL)
	  instructions = actionText & " the address for location " & RS("LOCATION_NAME") 
	  Set RS = Nothing 
	end if
end if
If not bNewAddress then
	'Get Address Info
	'GetInvConnection()
	SQL = "SELECT * FROM INV_ADDRESS WHERE address_id = ?"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("AddressID", 5, 1, 0, AddressID)
	Set RS = Cmd.Execute
	
	if RS.BOF or RS.EOF then
		sError = sError & "Error.<BR><BR>"
		bError = true	
	else
		ContactName = RS("Contact_Name")
		Address1 = RS("Address1")
		Address2 = RS("Address2")
		Address3 = RS("Address3")
		Address4 = RS("Address4")
		City = RS("City")
		StateIDFK = RS("State_ID_FK")
		CountryIDFK = RS("Country_ID_FK")
		ZIP = RS("ZIP")
		FAX = RS("FAX")
		Phone = RS("Phone")
		Email = RS("Email")
	end if
else
	'initialize variables
	ContactName = ""
	Address1 = ""
	Address2 = ""
	Address3 = ""
	Address4 = ""
	City = ""
	StateIDFK = ""
	CountryIDFK = ""
	ZIP = ""
	FAX = ""
	Phone = ""
	Email = ""
end if


%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=actionText%> an Address</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
    
	// Validates container attributes
	function ValidateAddress(){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		
		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.action = "EditAddress_action.asp";
			document.form1.submit();
		}
	}
	 
//-->
</script>
</head>

<body>
<form name="form1" method="POST">
<input TYPE="hidden" NAME="TableName" Value="<%=TableName%>">
<input TYPE="hidden" NAME="TablePKID" Value="<%=TablePKID%>">
<input TYPE="hidden" NAME="AddressID" Value="<%=AddressID%>">
<INPUT TYPE="hidden" NAME="bNewTableEntry" VALUE="<%=bNewTableEntry%>">

<table border="0" cellspacing="0" cellpadding="0" width="500">
	<tr height="25">
		<td align="center" valign="top" colspan="2"><span class="GuiFeedback"><%=Instructions%></span>&nbsp;</td>
	</tr>
	<% if TableName="inv_suppliers" then%>
	<tr height="25">
		<td align="right" valign="top" nowrap>Contact Name:</td>
		<td>
			<input type="text" name="ContactName" size="30" value="<%=ContactName%>">
		</td>
	</tr>
	<%end if%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="50%">Address 1:</td>
		<td  width="50%">
			<input type="text" name="Address1" size="30" value="<%=Address1%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>Address 2:</td>
		<td>
			<input type="text" name="Address2" size="30" value="<%=Address2%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>Address 3:</td>
		<td>
			<input type="text" name="Address3" size="30" value="<%=Address3%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>Address 4:</td>
		<td>
			<input type="text" name="Address4" size="30" value="<%=Address4%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>City:</td>
		<td align=left>
			<input type="text" name="City" size="30" value="<%=City%>">
		</td>
	</tr>
	<tr height="25">	
		<%=ShowPickList("State/Province:", "StateIDFK", StateIDFK,"SELECT State_ID AS Value, State_Abbreviation AS DisplayText FROM inv_States ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">	
		<%=ShowPickList("Country:", "CountryIDFK", CountryIDFK,"SELECT Country_ID AS Value, Country_Name AS DisplayText FROM inv_country ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>ZIP:</td>
		<td>
			<input type="text" name="ZIP" size="30" value="<%=ZIP%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>FAX:</td>
		<td>
			<input type="text" name="FAX" size="30" value="<%=FAX%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>Phone:</td>
		<td>
			<input type="text" name="Phone" size="30" value="<%=Phone%>">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>Email:</td>
		<td>
			<input type="text" name="Email" size="30" value="<%=Email%>">
		</td>
	</tr>

	<tr>
		<td colspan="2" align="right" height="20" valign="bottom"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
			<a HREF="#" onclick="ValidateAddress(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</body>
</html>