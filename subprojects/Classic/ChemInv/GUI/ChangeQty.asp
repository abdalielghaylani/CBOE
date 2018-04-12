<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
QtyRemaining = Request("QtyRemaining")
ContainerID = Request("ContainerID")
TotalQtyReserved = Request("TotalQtyReserved")
if TotalQtyReserved = "" then
	TotalQtyReserved = 0
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Change Amount Remaining in an Inventory Container</title>
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
		if (document.form1.QtyRemaining.value.length != 0) {
			// Amount remaining must be a number
			if (!isNumber(document.form1.QtyRemaining.value)){
				errmsg = errmsg + "- Quantity remaining must be a number.\r";
				bWriteError = true;
			}
			//Fixing CSBR-75186
			if (document.form1.QtyRemaining.value.length == 1){
				if(isNaN(document.form1.QtyRemaining.value)){ 
				    errmsg = errmsg + "- Quantity remaining must be a number.\r";
				    bWriteError = true;
				}
			}
			
			if (document.form1.QtyRemaining.value < 0){
				errmsg = errmsg + "- Quantity remaining cannot be a negative number.\r";
				bWriteError = true;
			}
			if (document.form1.QtyRemaining.value < <%=TotalQtyReserved%> && document.form1.okbutton.value=='yes'){
				warningmsg = warningmsg + "- The Quantity Remaining is less than the quantity currently reserved for the selected containers.\rIf you choose 'OK', all active reservations will be removed from the containers.\rDo you want to proceed?\r";
				bWriteWarning = true;
			}
			var m = document.form1.QtyRemaining.value.toString();		
			if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Quantity Remaining amount has wrong decimal operator.\r";
			bWriteError = true;
			}
		}
		// If the field has a value it must be a number
		if (document.form1.QtyRemoved.value.length != 0) {
			// Amount removed must be a number
			if (!isNumber(document.form1.QtyRemoved.value)){
    			errmsg = errmsg + "- Quantity removed must be a number.\r";
				bWriteError = true;
			}
			
			//Fixing CSBR-75186
			if (document.form1.QtyRemoved.value.length == 1){
				if(isNaN(document.form1.QtyRemoved.value)){ 
				    errmsg = errmsg + "- Quantity removed must be a number.\r";
				    bWriteError = true;
				}
			}
			
			// Quantity removed cannot exceed Original Quantity Remaining
			else if (!bWriteError && Number(document.form1.QtyRemoved.value) > Number(document.form1.OriginalQtyRemaining.value)) {
				errmsg = errmsg + "- Quantity removed cannot exceed Quantity Remaining.\r";
				document.form1.QtyRemoved.value = document.form1.OriginalQtyRemaining.value;
				document.form1.QtyRemaining.value = 0;
				bWriteError = true;
			}
			// cannot be negative
			if (document.form1.QtyRemoved.value < 0){
				errmsg = errmsg + "- Quantity removed cannot be a negative number.\r";
				bWriteError = true;
			}
			if ((document.form1.QtyRemaining.value - document.form1.QtyRemoved.value) < <%=TotalQtyReserved%> && document.form1.QtyRemoved.value!=0 && document.form1.okbutton.value=='yes'){
				warningmsg = warningmsg + "- The quantity you are removing from the Quantity Remaining is less than the quantity currently reserved for the selected containers.\rIf you choose 'OK', all active reservations will be removed from the containers.\rDo you want to proceed?\r";
				bWriteWarning = true;
			}
			var m = document.form1.QtyRemoved.value.toString();		
			if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Quantity Removed amount has wrong decimal operator.\r";
			bWriteError = true;
			}
			if (!bWriteError) {
				if (document.form1.QtyRemoved.value != 0)  {
				document.form1.QtyRemaining.value = document.form1.OriginalQtyRemaining.value - document.form1.QtyRemoved.value;
				}
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
	
	function calculateQtyRemaining(){
		var bError = false;
		bError = !ValidateQtyChange(); 		

		if (!bError) {
			// Set the value for quantity remaining
			if (document.form1.QtyRemoved.value != 0)  {
				document.form1.QtyRemaining.value = document.form1.OriginalQtyRemaining.value - document.form1.QtyRemoved.value;
			}
		}
	}
	
	function calculateQtyRemoved() {
		var bError = false;
			
		//bError = !ValidateQtyChange(); 		
			
		//if (!bError) {
			// Set the value for quantity removed
		document.form1.QtyRemoved.value = 0;
		//}
	}
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="ChangeQty_action.asp" method="POST" onsubmit="return ValidateQtyChange();">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="OriginalQtyRemaining" value="<%=QtyRemaining%>">
<input Type="hidden" name="okbutton" value="">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Set either the quantity remaining in this container or<br>the quantity removed from this container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Quantity Remaining (<%=Request("UOMAbv")%>):
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="QtyRemaining" VALUE="<%=QtyRemaining%>" onChange="calculateQtyRemoved()" onFocus="calculateQtyRemaining()">
		</td>
	</tr>
	<tr>
		<td align="right"><span class="GuiFeedback">OR</span></td>
		<td></td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Quantity Removed (<%=Request("UOMAbv")%>):
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="QtyRemoved" VALUE="0">
		</td>
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
