<%@ Language=VBScript %>
<%Response.Expires=0
%>
<HTML>
<HEAD>
<TITLE>Helper.asp</TITLE>
<script language= "javascript">
theWindow = <%=Application("mainwindow")%>
formmode = "<%=formmode%>"
if (formmode == ""){
	formmode = theWindow.formmode

}
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->

<body <%=Application("BODY_BACKGROUND")%>>
<P>&nbsp;</P>
</body>
</html>