<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
ContainerID = Session("ContainerID")
ContainerName = Session("ContainerName")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Cancel Container Order</title>
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
<form name="form1" xaction="echo.asp" action="CancelOrder_action.asp" method="POST">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to cancel the order for this container?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container Name:
		</td>
		<td>
			<input TYPE="tetx" SIZE="60" Maxlength="50" onfocus="blur()" VALUE="<%=ContainerName%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td align="right" nowrap>
			CAS:
		</td>
		<td>
			<input TYPE="tetx" SIZE="15" Maxlength="15" onfocus="blur()" VALUE="<%=Session("CAS")%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td align="right" nowrap>
			Supplier:
		</td>
		<td>
			<input TYPE="tetx" SIZE="60" Maxlength="50" onfocus="blur()" VALUE="<%=Session("SupplierName")%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td align="right" nowrap>
			Catalog #:
		</td>
		<td>
			<input TYPE="tetx" SIZE="15" Maxlength="15" onfocus="blur()" VALUE="<%=Session("SupplierCatNum")%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td align="right" nowrap>
			Date Ordered:
		</td>
		<td>
			<input TYPE="tetx" SIZE="15" Maxlength="15" onfocus="blur()" VALUE="<%=Session("DateOrdered")%>" disabled id="tetx1" name="tetx1">
		</td>
	<tr>
		<td align="right" nowrap>
			Comments:
		</td>
		<td valign="top">
			<textarea rows="7" cols="45" name="iComments" wrap="hard" disabled><%=Session("Comments")%></textarea>
		</td>
	<tr height="25">
		<%= ShowPickList("Owner:", "iOwnerID", Session("OwnerID"), "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People WHERE User_ID = '" & Session("OwnerID") & "'")%>
	</tr>
	<tr height="25">
		<%= ShowPickList("Current user:", "iCurrentUserID", Session("CurrentUserID"), "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People WHERE User_ID = '" & Session("CurrentUserID") & "'")%>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="submit(); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
