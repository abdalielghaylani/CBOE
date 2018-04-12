<%	
Response.expires=0
'Copyright CambridgeSoft Corporation 1998-2001 all rights reserved
'if Not Session("UserValidated" & "chemacx") = 1 then
'	response.redirect "login.asp?dbname=chemacx&formgroup=base_form_group&perform_validate=0"
'end if

dbkey = Request("dbname")
formgroup = Request("formgroup")

Session("CurrentLocation" & dbkey) = ""

response.redirect "default.asp?formgroup=" & formgroup & "&dataaction=db&dbname=" & dbkey
%>