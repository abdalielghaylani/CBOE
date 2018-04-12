<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<html>
<head>
<title><%=Application("appTitle")%> -- Delete an Inventory Substance</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/utils.js"></SCRIPT>
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
Dim Conn
Dim Cmd

CompoundID = Request("CompoundID")
Session("CurrentLocation" & dbkey & formgroup)=""
bDebugPrint = false
ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
FormData = "CompoundID=" & CompoundID & Credentials
ManageMode = Request("ManageMode")
SelectCompoundID = Request("SelectCompoundID")
SelectSN = Request("SelectSN")
if SelectCompoundID <> "" then
	GetChemInvConnection()
	sql = "UPDATE " & Application("CHEMINV_USERNAME") & ".inv_containers SET compound_id_fk = -1 WHERE compound_id_fk=?"
	Set Cmd = GetCommand(Conn, sql, 1)
	Cmd.Parameters.Append Cmd.CreateParameter("CompoundID", 131, 1, 0, CompoundID)
	Cmd.execute
End if
if ManageMode = "1" then 
	Session("GUIReturnURL") = "/cheminv/inputtoggle.asp?formgroup=substances_form_group&dbname=cheminv&formmode=list"
Else
	Session("GUIReturnURL") = ""
End if
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteSubstance.asp", "ChemInv", FormData)

%>

<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>
			<% 
			if bDebugPrint then
				Response.Write httpresponse
				Response.End
			End if
			If isNumeric(httpResponse) then
				If CLng(httpResponse) >= 0 then
					Response.Write "<center><SPAN class=""GuiFeedback"">Substance has been deleted</SPAN><center>"
					if Session("GUIReturnURL") = "" then
						if SelectCompoundID <> "" then
							Response.Write "<SCRIPT language=JavaScript>DoSelectSubstance(" & SelectCompoundID & ",'" & SelectSN & "');</SCRIPT>"
						End if
						Response.Write "<SCRIPT language=JavaScript>opener.focus(); window.close();</SCRIPT>"							
						
					Else
						Response.write OkButton(Session("GUIReturnURL"), "")	
					End if
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) &  "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Substance could not be deleted</SPAN></center>"
					Response.Write "<P><center><a HREF=""#"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"		
				End if
			Else
				Response.Write httpResponse
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>