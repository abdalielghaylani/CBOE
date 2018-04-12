<%
' This does not work becouse of double cshttp relay problem :(
' bail out with CreateSubstance2.asp
%>

<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateSubstance.asp", "ChemInv", FormData)

If inStr(1,httpResponse,"<DUPLICATESTRUCTURE") > 0 then
	' Duplicate found. Parse out the duplicate MolID
	firstQuote = InStr(1,httpResponse,"""")
	secondQuote = InStr(firstQuote +1,httpResponse,"""")
	dupMolID = Mid(httpResponse, firstQuote + 1 , SecondQuote - firstQuote - 1)
	Response.Write dupMolID
Elseif inStr(1,httpResponse,"<NEWSTRUCTURE") > 0 then
	' Success! record added.  Parse out the new CompoundId
	firstQuote = InStr(1,httpResponse,"""")
	secondQuote = InStr(firstQuote +1,httpResponse,"""")
	newCompoundID = Mid(httpResponse, firstQuote + 1 , SecondQuote - firstQuote - 1)
	Response.Write newCompoundID
Else
	' Something went wrong with Add_record action
	strError = strError & httpResponse  & "<BR>"
	Response.Write strError
	Response.End
End if
Response.end
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Crerate a New Inventory Substance</title>
<script language="JavaScript">
<!--Hide JavaScript
	focus();

   if (navigator.appName == "Netscape"){
      document.write('<LINK REL=STYLESHEET HREF="../cheminv_ns.css" TYPE="text/css">');
      }
   else{
      document.write('<LINK REL=STYLESHEET HREF="../cheminv_ie.css" TYPE="text/css">');
   }
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>
			<% 
			If IsNumeric(httpResponse) then
				Response.Write "<SPAN class=""GuiFeedback"">New location has been created</SPAN>"
				Response.Write "<P><center><a HREF=""#"" onclick=""opener.location.href='../cheminv/buildlist.asp?LocationID=" & request("ParentID") &  "'; window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			else				
				Response.Write "<P><CODE>" & httpResponse & "</CODE></P>"
				Response.Write "<SPAN class=""GuiFeedback"">New location could not be created</SPAN>"
				Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>