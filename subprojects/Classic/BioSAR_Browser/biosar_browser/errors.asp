<%if Session("errorbiosar_browserbase_form_group") = true then
	Response.write Session("messagebiosar_browserbase_form_group")
	if instr(Session("messagebiosar_browserbase_form_group"),"An error occurred in connecting to the datasource") > 0 then
		Response.write "<br><br> If you were attempting to open a form also check that you have configured the database password correctly under Schema Management."
	end if
	Response.write "<a href=""/chemoffice/"">" & "Return to ChemOffice Enterprise " & "</a>"
Response.end
End If
%>