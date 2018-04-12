<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(multiSelect_dict)
	ContainerBarcode =  multiSelect_dict.count & " containers will be deleted"
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
	window.focus();
-->
</script>
</head>
<body>
<center>

<%
if ContainerID = "" then %>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>No Container is selected, please <br />close this window and select a container.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a></a>
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
			<input TYPE="tetx" SIZE="30" Maxlength="50" onfocus="blur()" VALUE="<%=ContainerBarcode%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="submit(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	

<% end if %>


</form>
</center>
</body>
</html>
