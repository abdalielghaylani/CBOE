<%@ Language=VBScript %>
<%
LocationID = Request("LocationID")
trashType = Request("trashType")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Empty a Trash Location</title>
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
<form name="form1" xaction="echo.asp" action="EmptyTrash_action.asp" method="POST">
<input Type="hidden" name="LocationID" value="<%=LocationID%>">
<input TYPE="hidden" NAME="trashType" value="<%=trashType%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to permanently delete all <%=trashType%> in this location?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
