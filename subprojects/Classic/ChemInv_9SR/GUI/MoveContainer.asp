<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(myDict)
	ContainerBarcode =  myDict.count & " containers will be moved."
Else
	ContainerID = Session("ContainerID")
	ContainerBarcode = Session("Barcode")
End if

LocationID = Request.QueryString("LocationID")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move an Inventory Container</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateMove(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.LocationID.value)){
			errmsg = errmsg + "- Destination must be a number.\r";
			bWriteError = true;
			}
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
<form name="form1" xaction="echo.asp" action="MoveContainer_action.asp" method="POST">
<%if len(ContainerID) = 0 then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select containers to move.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
<%else%>
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Move an inventory container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container to move:
		</td>
		<td>
			<input TYPE="tetx" SIZE="44" onfocus="blur()" VALUE="<%=ContainerBarcode%>" disabled>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false%> 
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
<%end if%>
</form>
</center>
</body>
</html>
