<%@ Language=VBScript %>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
</HEAD>

<BODY>

<P>

<%
'Response.Write Request.querystring
'Response.Write "<br>"

If Request("showpage") = "links" then
	linktext = "Click Here For External Links"
Else
	linktext = "Click here for document preview"
End If
%>

<a href="docmanager_form.asp?<%=Request.querystring%>" target="main"><%=linktext%></a>





</BODY>
</HTML>