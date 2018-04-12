<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

action = Request("action")
PlateTypeID = Request("ID")


If action <> "create" then
	Call GetInvConnection()
	SQL = "SELECT plate_type_name, max_freeze_thaw FROM inv_plate_types WHERE plate_type_ID=" & PlateTypeID
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
	PlateTypeName=RS("plate_type_name").value
	MaxFreezeThaw= RS("max_freeze_thaw").value
End if

%>
<html>
<head>
<title><%=Ucase(Left(action,1)) & Mid(action, 2, Len(action)-1)%> Plate Type</title>
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
		if (document.form1.PlateTypeName.value.length == 0) {
			errmsg = errmsg + "- Plate Type Name is required.\r";
			bWriteError = true;
		}
		// Max Freeze Thaw is required
		if (document.form1.MaxFreezeThaw.length == 0) {
			errmsg = errmsg + "- Max Freeze/Thaw Cycles is required.\r";
			bWriteError = true;
		}
		else{
			//  Max Freeze Thaw must be a number
			if (!isPositiveNumber(document.form1.MaxFreezeThaw.value)){
			errmsg = errmsg + "- Max Freeze/Thaw Cycles must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.MaxFreezeThaw.value > 9999){
			errmsg = errmsg + "- Max Freeze/Thaw CyclesMaximum allowed is 9999.\r";
			bWriteError = true;
			}
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
<form name="form1" action="ManagePlateType_action.asp?action=<%=action%>" method="POST">
<input type="hidden" name="PlateTypeID" value="<%=PlateTypeID%>">


<table border="0">
<%if action = "delete" then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Are you sure you want to delete this Plate Type:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Plate Type Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="PlateTypeName" value="<%=PlateTypeName%>" disabled>
		</td>
	</tr>

<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Plate Types Attributes:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Plate Type Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="PlateTypeName" value="<%=PlateTypeName%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Max Freeze/Thaw Cycles:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="5" NAME="MaxFreezeThaw" value="<%=MaxFreezeThaw%>">
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
