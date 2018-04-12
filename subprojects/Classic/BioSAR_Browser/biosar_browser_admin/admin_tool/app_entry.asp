<%
' store the dbkey in a session var
if request("dbname") <> "" Then
	Session("dbkey_admin_tools") = request("dbname")
	Session("UserValidated" & request("dbname")) = 1 
end if
%>