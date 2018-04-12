<%@ Language=VBScript %>
<%Response.Expires=0
dbkey=Request("dbname")
formgroup = Request("formgroup")
formmode =Request("formmode")
%>
<HTML>
<HEAD>
<TITLE>Helper.asp</TITLE>
<script language= "javascript">
theWindow = <%=Application("mainwindow")%>
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<% action = request.querystring("action")%>
</HEAD>
<BODY background="<%=Application("UserWindowBackground")%>">
<P>&nbsp;</P>
</body>
</html>