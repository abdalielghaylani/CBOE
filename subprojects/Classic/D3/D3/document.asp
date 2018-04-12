<%@ Language=VBScript %>
<%
Response.Expires = -1
docid = Request("docid")
if docid <> "" then
	pHostName = Application("DOCMANAGER_SERVER_NAME")
	pTarget = "docmanager/docmanager/externallinks/getDocumentsNoGUI.asp?csusername=D3browser&csuserid=D3browser&DocID=" & docid
	pUserAgent = "D3"
	pMethod = "POST"
						
	URL = "http://" & pHostName & "/" & pTarget
				
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
	objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objXmlHttp.setRequestHeader "User-Agent", pUserAgent
	objXmlHttp.send pData



	'Response.write objXmlHttp.getAllResponseHeaders
	response.ContentType = objXmlHttp.getResponseHeader("Content-Type")

	Response.BinaryWrite objXmlHttp.responseBody
else%>
	<P>Parameter missing.
	<P><a href="#" onclick="window.close();return true;">Close window</a>
<%end if


%>