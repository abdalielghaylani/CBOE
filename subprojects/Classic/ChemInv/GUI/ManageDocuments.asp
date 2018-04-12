<%@ Language=VBScript %>
<%'SYAN created 11/17/2003 to link to docmanager%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
	
FK_value = Request("FK_value")
FK_name = Request("FK_name")
Table_name = Request("Table_name")
URLType = Request("URLType")
LinkType = request("LINK_TYPE")
%>
<html>
<head>
	<title>Manage Documents</title>
	<script LANGUAGE="javascript">
		window.focus();
	</script>

	<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>



<script language="javascript">
	function launchDocMgrWindow(){
		window.open( '/docmanager/default.asp?dataaction=db&formgroup=base_form_group&dbname=docmanager&extAppName=cheminv&LinkType=<%=LinkType%>&linkfieldname=<%=FK_name%>&showselect=true&extlinkid=<%=FK_value%>', 'docmgrwindow', 'toolbar=no,location=no,scrollbars=yes,width=800,height=600')
	}
</script>

<%

If  Session("SEARCH_DOCS" & "ChemInv") then

	showdelete = true

	pData = "ReturnType=html&showlogo=true&extLinkID=" & FK_value & "&LinkType=" & LinkType & "&showsubmitdate=true&showsubmitter=true&showdelete=" & showdelete
	pData = pData & "&csusername=" & Session("username" & "ChemInv")  & "&csuserid=" & Session("UserID" & "ChemInv")
	pHostName = Application("DOCMANAGER_SERVER_NAME")

	pTarget = "/docmanager/docmanager/externallinks/getDocumentsNoGUI.asp"
	pUserAgent = "ChemInv"
	pMethod = "POST"
					
	'URL = "http://" & pHostName & "/" & pTarget
	URL = Application("SERVER_TYPE") & pHostName & pTarget
	
	
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("Msxml2.ServerXMLHTTP")

	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
	objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objXmlHttp.setRequestHeader "User-Agent", pUserAgent
	objXmlHttp.send pData

	' Print out the request status:
	StatusCode = objXmlHttp.status

	If StatusCode <> "200" then
		httpResponse = objXmlHttp.responseText
		'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
	Else
		httpResponse = objXmlHttp.responseText
	End If

	Response.Write "<br><br>"
				
	Response.Write "<table width=""600"" border=""0"" cellpadding=""0"" cellspacing=""0"">"
	Response.Write "<tr><td width=""600"">"
	Response.Write "<b>Associated Documents in Doc Manager</b>"	
	Response.Write "<br><br>"
	Response.write httpResponse
				
	Response.Write "</td></tr></table>"
else%>
	You do not have the appropriate privileges to add or view documents. Please ask the administrators to grant you DOCMGR_EXTERNAL role and log back in to try again.
<%end if%>

<%If Session("SEARCH_DOCS" & "ChemInv") then%>
	<a href="#" onclick="javascript:launchDocMgrWindow();return false" class="MenuLink">Link To Document</a>
<%else%>
	<a href="#" onclick="alert('In order to link to documents, you need to have DOCMGR_EXTERNAL privileges. Please ask administrators to grant you DOCMGR_EXTERNAL permission and log back in to try again.')">
<%end if%>
<br>
<br><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>

</center>
</body>
</html>

