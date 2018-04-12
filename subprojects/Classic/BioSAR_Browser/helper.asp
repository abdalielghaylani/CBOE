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
if (!<%=Application("mainwindow")%>.mainFrame){
	theWindow = <%=Application("mainwindow")%>
}
else{
	theWindow = <%=Application("mainwindow")%>.mainFrame
}
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<% action = request.querystring("action")

Select case UCase(action)
case "CHANGELIST_TABLEIDS"
	id = request("table_id")
	idFound = false
	if Session("ChangeList") <> "" then
		temp = split(Session("ChangeList"), ",", -1)
		for i = 0 to UBound(temp)
			if temp(i) = id then
				idFound = true
			else
				if newChangeList <> "" then
					newChangeList = newChangeList & "," & temp(i)
				else
					newChangeList = temp(i)
				end if
			end if
		next
		if idFound = false then
			if newChangeList <> "" then
				newChangeList = newChangeList & "," & id
			else
				newChangeList = id
			end if
		end if	
	else
	
		newChangeList = id
	end if	
	Session("ChangeList") = newChangeList
	
	
End Select%>
</HEAD>
<body <%=Application("BODY_BACKGROUND")%>>
<P>&nbsp;</P>
</body>
</html>