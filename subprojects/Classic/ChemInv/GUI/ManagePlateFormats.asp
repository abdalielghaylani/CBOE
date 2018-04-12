<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim RS

action = Request("action")
PlateFormatID = Request("ID")


If action <> "create" then
	GetInvConnection()
	SQL = "SELECT plate_format_name, phys_plate_id_fk FROM inv_plate_format WHERE plate_format_ID=?"
	
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PlateIDs", 131, 1, 0, PlateFormatID)
	Set RS = Cmd.Execute
	PlateFormatName=RS("plate_format_name").value
	PhysPlateID= RS("phys_plate_id_fk").value
	
End If

%>
<html>
<head>
<title><%=Ucase(Left(action,1)) & Mid(action, 2, Len(action)-1)%> Plate Format</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--
	window.focus();
	
	function Validate(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
	<%if action <> "delete" then%>
		// Name is required
		if (document.form1.pPlateFormatName.value.length == 0) {
			errmsg = errmsg + "- Plate Format Name is required.\r";
			bWriteError = true;
		}
		// Phys Plate Type is required
		if (document.form1.pPhysPlateID.value == "") {
			errmsg = errmsg + "- Physical Plate is required.\r";
			bWriteError = true;
		}
	<%end if%>
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
//-->
</script>
</head>

<body>
<center>
<form name="form1" action="ManagePlateFormat_action.asp?action=<%=action%>" method="POST">
<input type="hidden" name="pPlateFormatID" value="<%=PlateFormatID%>">

<table border="0">
<%if action = "delete" then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Are you sure you want to delete this Plate Format:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Plate Format Name:</span>
		</td>
		<td>
		
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="pPlateFormatName" value='<%=PlateFormatName%>' disabled>
		</td>
	</tr>

<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Plate Format Attributes:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Plate Format Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="pPlateFormatName" value='<%=PlateFormatName%>'>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Physical Plate:<span>
		</td>
		<td>
			<%=ShowSelectBox2("pPhysPlateID", PhysPlateID, "SELECT phys_plate_ID AS Value, phys_plate_name AS DisplayText FROM inv_physical_plate ORDER BY phys_plate_Name ASC", 45, RepeatString(50, "&nbsp;"), "")%>
		</td>
	</tr>
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</center>
</body>
</html>
