<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
action = Lcase(Request.QueryString("action"))
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials
'Response.Write FormData 
'Response.end
Select Case action
	Case "delete"
		APIURL = "/cheminv/api/DeleteReservation.asp"
	Case "edit"
		APIURL = "/cheminv/api/UpdateReservation.asp"
	Case "create"
		APIURL = "/cheminv/api/CreateReservation.asp"
End Select
httpResponse = CShttpRequest2("POST", ServerName, APIURL, "ChemInv", FormData)

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create a New Inventory Reservation</title>
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
			If IsNumeric(httpresponse) then 
				If Clng(httpResponse) > 0 then
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">Reservation has been processed</SPAN><center>"
					'Response.Write "<SCRIPT LANGUAGE=javascript>opener.parent.ListFrame.location.href='/cheminv/cheminv/buildlist.asp?LocationID=" & Session("CurrentLocationID") & "&LocationName=" & LocationName & "';window.close()</SCRIPT>"
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("LocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," &  Request("ContainerID") & "); opener.focus(); //window.close();</SCRIPT>" 
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Reservation could not be processed</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
				Response.Write httpresponse
				Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>