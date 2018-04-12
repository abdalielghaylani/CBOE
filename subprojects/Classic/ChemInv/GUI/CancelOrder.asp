<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS

OrderID = Request("OrderID")

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Cancel an Order</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
<center>
<form name="form1" action="CancelOrder_action.asp" method="POST">
<INPUT TYPE="hidden" NAME="OrderID" VALUE="<%=OrderID%>">

<table border="0">
	<TR>
		<TD colspan="2" ALIGN="center"><span class="GUIFeedback">If you enter a reason for cancelling this order, <BR>it will be appended to the order comments.</span><br></TD>
	</TR>
	<tr>
		<TD ALIGN="right" WIDTH="200" NOWRAP>Order ID:</TD>
		<td align="left" width="400"><input TYPE="text" CLASS="GrayedText" SIZE="10" Maxlength="50" VALUE="<%=OrderID%>" READONLY></td>
	</tr>
	<TR><TD COLSPAN="2">
	<TABLE BORDER="0">
		<tr>
			<td align="right" valign="top" nowrap width="200">
				Reason for Cancel:
			</td>
			<td valign="top">
				<textarea rows="5" cols="32" name="CancelReason" wrap="hard"></textarea>
			</td>
		</tr>
	</TABLE>
	</TD></TR>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close();"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="document.form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>