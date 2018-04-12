<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Conn
							
bNewTableEntry = cBool(Request("bNewTableEntry"))
'if this is for a new table entry, don't create an address just pass the information back to the opener

if bNewTableEntry then
	Address1 = Request("Address1")
	Address2 = Request("Address2")
	Address3 = Request("Address3")
	Address4 = Request("Address4")
	City = Request("City")
	StateIDFK = Request("StateIDFK")
	CountryIDFK = Request("CountryIDFK")
	ZIP = Request("ZIP")
	FAX = Request("FAX")
	Phone = Request("Phone")
	Email = Request("Email")
	
%>
<html>
<head>
<script language="JavaScript">
<!--Hide JavaScript
	if (opener.document.form1.Address1) {
		opener.UpdateAddressInfo('<%=Address1%>','<%=Address2%>','<%=Address3%>','<%=Address4%>','<%=City%>','<%=StateIDFK%>','<%=CountryIDFK%>','<%=ZIP%>','<%=FAX%>','<%=Phone%>','<%=Email%>');
		opener.focus();
		window.close();	
	}
//-->
</script>
</head>
<body>
</Body>
<%
else

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = Request.Form & Credentials

'Response.Write FormData
'Response.End
'Update the containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/updateAddress.asp", "ChemInv", FormData)
'Response.Write httpResponse
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Edit an Address</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE>
			<% 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					Response.Write "<center><SPAN class=""GuiFeedback"">Address has been updated</SPAN></center>"
			%>
					<SCRIPT LANGUAGE="javascript">
						if (opener.document.form1.isRequestSamples || opener.document.form1.bUpdateAddress) {
							opener.UpdateAddress(<%=Request("TablePKID")%>); 
						}
						//if you are adding an address from the locations page, update the link
						else if (opener.document.form1.LocationID) {
							opener.UpdateAddressLink(<%=Request("TablePKID")%>,<%=httpResponse%>);
						}
						else if (opener) {
							opener.location.reload();
						}
						opener.focus(); window.close();						
					</SCRIPT>
			<%
					'Response.Write "<SCRIPT LANGUAGE=javascript>opener.UpdateAddress(0); opener.focus(); window.close();</SCRIPT>" 
					Response.Write "<P><a HREF=""3"" onclick=""window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Address could not be updated</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>
<%end if%>