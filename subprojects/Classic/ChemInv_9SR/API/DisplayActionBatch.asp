<%@ Language=VBScript %>
<%
Dim Conn
Dim Cmd
Dim isCommit

Response.Expires = -1
Session("CurrentLocationID") = Request("LocationID")
SessionDir = Application("TempFileDirectory" & "ChemInv") & "Sessiondir"  & "\" & Session.sessionid
ActionBatchFile = SessionDir & "\InvActionBatch.xml" 

' Get the posted XML
ActionBatchXML = Request.Form("ActionBatchXML") 
'Response.Write "XML= " & ActionBatchXML
'Response.end
'Create the data tree
dim xmlDoc
set xmlDoc = Server.CreateObject("Msxml2.DOMDocument")
xmlDoc.async = false
If  ActionBatchXML <> "" then
	' Load the xml received in the post and save it to session dir
	xmlDoc.loadXML(ActionBatchXML)
	xmlDoc.save(ActionBatchFile)
Else
	'Load the xml from session dir file
	xmlDoc.load(ActionBatchFile)
End if

' Create xsl transform tree
dim xslDoc
set xslDoc = Server.CreateObject("Msxml2.DOMDocument")
xslDoc.async = false
xslDoc.load(server.MapPath("/cheminv/api/ActionBatch.xsl"))

' Transform and return in response
xmlDoc.transformNodeToObject xslDoc, Response
%>
