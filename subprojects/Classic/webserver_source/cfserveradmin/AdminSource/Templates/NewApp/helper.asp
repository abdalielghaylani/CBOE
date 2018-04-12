<%@ Language=VBScript %>
<%Response.Expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey=Request("dbname")
formgroup = Request("formgroup")
formmode =Request("formmode")
%>
<HTML>
<HEAD>
<TITLE>Helper.asp</TITLE>
<script language= "javascript">
if (!<%=Application("mainwindow")%>.mainFrame){
	theWindow = <%=Application("mainwindow")%>
}
else{
	theWindow = <%=Application("mainwindow")%>.mainFrame
}
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<% action = request.querystring("action")%>
</HEAD>
<body <%=Application("BODY_BACKGROUND")%>>
<P>&nbsp;</P>
</body>
</html>