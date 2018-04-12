<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Delete an empty Inventory Location</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var openNodes = "<%=selectedNodes%>";
//-->
</script>
</head>
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim bDebugPrint

LocationID = Request("LocationID")

bDebugPrint = false
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = "LocationID=" & LocationID & "&Recursively=" & Request("Recursively") & Credentials

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteLocation.asp", "ChemInv", FormData)

%>

<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE WIDTH="400" NOWRAP>
			<% 
			if bDebugPrint then
				Response.Write httpresponse
				Response.End
			End if
			If isNumeric(httpResponse) then
				'Remove the node to deleted from the nodes to be selected
				'selectedNodes = Replace(Session("TreeViewOpenNodes1"), "&node=" & LocationID , "")
				'Remove the node from the open node list
				Session("TreeViewOpenNodes1") = Replace(Session("TreeViewOpenNodes1"), "&node=" & LocationID & "&" , "&")
				
				If CLng(httpResponse) >= 0 then
					Response.Write "<center><SPAN class=""GuiFeedback"">Location has been deleted</SPAN><center>"
					bclearNodes = "0"
					' refresh the tree for recusive deletes	
					if request("Recursively") = "1" then bclearNodes = "1"
					if Session("GUIReturnURL") = "" then
						Response.Write "<SCRIPT language=JavaScript>SelectLocationNode(" & bclearNodes & ", " & httpResponse & ", " & LocationID & ", '" & Session("TreeViewOpenNodes1") & "'); opener.focus(); window.close();</SCRIPT>"							
					Else
						Response.write OkButton(Session("GUIReturnURL"), "")	
					End if
				
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) &  "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Location could not be deleted</SPAN></center>"
					Response.Write "<P><center><a HREF=""#"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"		
				End if
			Else
					Response.Write "<center><P><CODE>" & httpResponse &  "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Location could not be deleted</SPAN></center>"
					Response.Write "<P><center><a HREF=""#"" onclick=""history.back(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a></center>"		
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>