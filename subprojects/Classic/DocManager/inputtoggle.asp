<% 

formgroup = Request.QueryString("formgroup")
Session("CurrentLocation" & "docmanager") = ""

Response.Redirect "/docmanager/default.asp?formgroup=" & formgroup & "&dbname=docmanager"

%>


