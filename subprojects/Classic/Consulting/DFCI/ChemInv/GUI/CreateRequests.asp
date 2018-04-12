<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
BumpUP = Request("BumpUp")
if BumpUp = "" then
	BumpUp = 0
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create replenishment requests</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function ValidateQtyChange(){
		
		var bWriteError = false;
		var bWriteWarning = false;
		var errmsg = "Please fix the following problems:\r\r";
		var warningmsg = "Please address the following warnings:\r\r";

		// One of the fields must have data
		if (document.form1.QtyRemaining.value.length == 0 && document.form1.QtyRemoved.value.length == 0) {		
			errmsg = errmsg + "- You must enter a value for either Quantity Remaining or Quantity Removed.\r";
			bWriteError = true;
		}
		// If the field has a value it must be a number
		if (document.form1.BumpUp.value.length != 0) {
			// Amount remaining must be a number
			if (!isNumber(document.form1.QtyRemaining.value)){
				errmsg = errmsg + "- Bump up % must be a number.\r";
				bWriteError = true;
			}

		}
		
		document.form1.okbutton.value='';
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			var bcontinue = true;
			// Report warnings, user can choose to accept or cancel
			bConfirmWarning = true;
			if (bWriteWarning) {
				bConfirmWarning = confirm(warningmsg);
			}
			if (!bConfirmWarning) var bcontinue = false;

			return bcontinue;
		}
	}
	
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="CreateRequests_action.asp" method="POST" onsubmit="return ValidateQtyChange();">
<input Type="hidden" name="okbutton" value="">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Click OK to generate replenishment requests.<BR>  Enter a bump up % to increase the thresholds for critical drugs.</span><br><br>
		</td>
	</tr>
	<tr>
		<td>
			Bump Up Percent
		</td>
	</tr>
	<tr>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="BumpUp" VALUE="<%=BumpUp%>">
		</td>
	</tr>
	<tr>
		<td align="right"><span class="GuiFeedback"></span></td>
		<td></td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			<input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21" onclick="javascript:document.form1.okbutton.value='yes';">
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
