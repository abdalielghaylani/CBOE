<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

bDebugPrint = false

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials
if request("locationContentsMode") = 3 then
	FormData = FormData & "&PlateTypeList=" & Request("PlateMapID")
end if
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CreateLocation.asp", "ChemInv", FormData)

' add an address if one was entered

bAddAddress = cBool(Request("bAddAddress"))
if bAddAddress then
	FormData = FormData & "&TableName=inv_locations"
	FormData = FormData & "&TablePKID=" & httpResponse
	FormData = FormData & "&AddressID="
	'Response.Write FormData
	httpResponse2 = CShttpRequest2("POST", ServerName, "/cheminv/api/updateAddress.asp", "ChemInv", FormData)
	'Response.Write httpResponse2
	'Response.End
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Crerate a New Inventory Location</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var openNodes = "<%=Session("TreeViewOpenNodes1")%>";
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If bdebugPrint then
				Response.Write httpresponse
				Response.End
			End if 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) > 0 then
					FormData = "LocationID=" & httpResponse & "&AllowContainers=" & request("AllowContainers") & "&ContainerTypeIDList=" & ContainerTypeIDList & Credentials
					httpResponse2 = CShttpRequest2("POST", ServerName, "/cheminv/api/ExcludeContainerTypes.asp", "ChemInv", FormData)
					'Response.Write httpResponse2
					'Response.end
					if NOT isNumeric(httpResponse2) then
						Response.Write httpResponse2
						Response.end
					else
						if httpResponse2 < 0 then
							Response.Write "<P><CODE>" & Application(httpResponse) & "</CODE></P>"
							Response.Write "<SPAN class=""GuiFeedback"">Could not process container type exclusions.</SPAN>"
							Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
							Response.end
						End if
					End if
					Response.Write "<SPAN class=""GuiFeedback"">New location has been created</SPAN>"
					if Session("GUIReturnURL") = "" then
						Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(0, " & httpResponse & "); opener.focus(); window.close();</SCRIPT>"
					Else
						Response.write OkButton(Session("GUIReturnURL"), "")	
					End if
				else				
					Response.Write "<P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">New location could not be created</SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpResponse
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>