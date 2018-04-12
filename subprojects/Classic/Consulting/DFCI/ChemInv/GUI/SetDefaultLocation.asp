<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Set your default inventory location</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<center>
<%
Dim Conn
Dim RS
if Request.Form("action")= "Save" then
	DefLocType = Request.Form("DefLocType")
	if DefLocType = "INVDefLoc" then Session("DefaultLocation") = Request.Form("LocationID")
	
	rc= WriteUserProperty(Session("UserNameCheminv"), DefLocType, Request.Form("LocationID"))
	
	Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();</SCRIPT>" 
	'Response.Write "<SCRIPT LANGUAGE=javascript>window.close();</SCRIPT>"
	Response.end
End if
if Application("ALLOW_USER_TO_SET_DELIVERY_LOCATION") = "0" then disable=" disabled "
%>
<form name="form1" method="POST">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Set this location as your default:<BR></span>
					<BR>&nbsp;&nbsp;&nbsp;&nbsp;<input checked type="radio" name="DefLocType" value="INVDefLoc">Home location
					<BR>&nbsp;&nbsp;&nbsp;&nbsp;<input <%=disable%> type="radio" name="DefLocType" value="INVContainerOrderDeliveryLoc">Container order delivery location
					<BR><BR>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Location Name:
		</td>
		<td>
			<input TYPE="tetx" SIZE="30" Maxlength="50" onfocus="blur()" VALUE="<%=RTrunc(Session("CurrentLocationName"), 25)%>" disabled>
		</td>
	</tr>
	<input TYPE="hidden" NAME="LocationID" Value="<%=Session("CurrentLocationID")%>">
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="submit(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>
<input type="hidden" name="action" value="Save">	
</form>
</center>
</body>
</html>
