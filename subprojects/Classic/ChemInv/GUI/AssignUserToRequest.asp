<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
RequestID = Request("RequestID")
caption = "Assign user to this Request"
UserID = uCase(Session("UserName" & "cheminv"))
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Assign User To Request</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateRequest(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";

		if (document.form1.RequestID.value.length == 0){
			errmsg = errmsg + "- RequestID is required.\r";
			bWriteError = true;
		}

		if (document.form1.UserID.value.length == 0){
			errmsg = errmsg + "- Please select a user to assign to the Request.\r";
			bWriteError = true;
		}

		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>

<form name="form1" action="AssignUserToRequest_action.asp" method="post">
<table border="0">

	<tr><td colspan="2">
			<span class="GuiFeedback"><%=caption%></span><br><br>
	</td></tr>
	
	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Request ID:</span>
		</td>
		<td colspan="3">
			<input type="text" size="20" maxlength="50" name="RequestID" value="<%=RequestID%>" class="readOnly" readonly>
		</td>
	</tr>
	<tr>
		<%=ShowPickList("<span class=""required"">Assign to User:</span>", "UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="document.form1.submit();"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>		
</table>	
</form>

</center>
</body>
</html>

