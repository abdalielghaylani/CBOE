<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

action = Request("action")
WellFormatID = Request("ID")


If action <> "create" then
	Call GetInvConnection()
	SQL = "SELECT enum_value FROM inv_enumeration WHERE enum_ID=" & WellFormatID
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
	WellFormatName=RS("enum_value").value
End if

%>
<html>
<head>
<title><%=Ucase(Left(action,1)) & Mid(action, 2, Len(action)-1)%> Well Format</title>
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
		if (document.form1.pWellFormatName.value.length == 0) {
			errmsg = errmsg + "- Well Format Name is required.\r";
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
<form name="form1" action="ManageWellContentType_action.asp?action=<%=action%>" method="POST">
<input type="hidden" name="pWellFormatID" value="<%=WellFormatID%>">


<table border="0">
<%if action = "delete" then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Are you sure you want to delete this Well Format:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Well Format Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="WellFormatName" value="<%=WellFormatName%>" disabled>
		</td>
	</tr>

<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Well Type Attributes:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Well Format Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="pWellFormatName" value="<%=WellFormatName%>">
		</td>
	</tr>
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>	
</form>
</center>
</body>
</html>
