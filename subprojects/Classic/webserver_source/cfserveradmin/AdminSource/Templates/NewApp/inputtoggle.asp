<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey = Request("dbname")
formgroup = Request("formgroup")

Session("CurrentLocation" & dbkey & formgroup) = ""

response.redirect "default.asp?formgroup=" & formgroup & "&dataaction=db&dbname=" & dbkey
%>