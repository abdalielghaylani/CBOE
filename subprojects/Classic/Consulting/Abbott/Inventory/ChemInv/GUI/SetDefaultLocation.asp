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
	Session("DefaultLocation") = Request.Form("LocationID")
	'Response.Write(Session("DefaultLocation"))
	'Response.End
	
	rc= WriteUserProperty(Session("UserNameCheminv"), "INVDefLoc", Session("DefaultLocation"))
	
	Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & "); opener.focus(); window.close();</SCRIPT>" 
	'Response.Write "<SCRIPT LANGUAGE=javascript>window.close();</SCRIPT>"
	Response.end
End if

%>
<form name="form1" method="POST">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Do you want to set this location as your default (home) inventory location?</span><br><br>
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
