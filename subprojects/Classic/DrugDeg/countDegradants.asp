<%
Set c = Server.CreateObject("ADODB.Connection")

c.open("file name=" & Server.MapPath("/drugdeg/") & "\config\drugdeg.udl;password=" & Application("DRUGDEG_PWD") & ";User id=" & Application("DRUGDEG_USERNAME"))
set RS = c.execute ("select count(*) degcount from DRUGDEG_DEGS")
response.write "There are currently " & RS("degcount").value & " degradants in the database."
%>