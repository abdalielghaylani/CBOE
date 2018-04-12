<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
ContainerCount = 1
if Lcase(Request("multiSelect")) = "true" then 
	ContainerCount =  multiSelect_dict.count
	if ContainerCount = 0 then 	
		action = "noContainers"
	else
		ContainerID = DictionaryToList(multiSelect_dict)
		ContainerBarcode =  multiSelect_dict.count & " containers will be deleted."
	end if
Else
	ContainerID = Session("ContainerID")
	ContainerBarcode = Session("Barcode")
End if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an Inventory Container</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
-->
</script>
</head>
<body>
<center>
<%if action = "noContainers" then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select containers to delete.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
<%elseif ContainerCount > 300 then %>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You cannot delete more than 300 containers at a time.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
<% else %>

<form name="form1" xaction="echo.asp" action="DeleteContainer_action.asp" method="POST">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to move this container to the trash?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container to delete:
		</td>
		<td>
			<input TYPE="text" SIZE="50" Maxlength="50" onfocus="blur()" VALUE="<%=ContainerBarcode%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	

</form>
<%end if%>
</center>
</body>
</html>
