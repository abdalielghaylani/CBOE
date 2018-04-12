<%
Set c = Server.CreateObject("ADODB.Connection")
c.open("file name=" & Server.MapPath("/D3/") & "\config\d3.udl;password=" & Application("D3_PWD") & ";User id=" & Application("D3_USERNAME"))
set RS = c.execute ("select count(*) degcount from D3_DEGS")
response.write "There are currently " & RS("degcount").value & " degradants in the database."
%>