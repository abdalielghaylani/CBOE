<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bdebugPrint

bdebugPrint = false
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = "ContainerID=" & Request("ContainerID") & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/custom/API/CancelOrder.asp", "ChemInv", FormData)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Cancel Order</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var openNodes = "<%=Session("TreeViewOpenNodes" & TreeID)%>";
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>
			<% 
			If bdebugPrint then
				Response.Write httpresponse
				Response.End
			End if
			If CLng(httpResponse) > 0 then
				Response.Write "<center><SPAN class=""GuiFeedback"">Container order has been canceled</SPAN><center>"
				Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0);opener.focus();window.close()</SCRIPT>" 	
			else				
				Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
				Response.Write "<center><SPAN class=""GuiFeedback"">Container order could not be canceled</SPAN></center>"
				Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""/cheminv/graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>