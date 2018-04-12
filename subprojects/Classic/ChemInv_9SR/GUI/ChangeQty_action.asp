<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%'Response.end%>
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Credentials

DestinationLocationID = Request("ParentID")
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
FormData = Request.Form & Credentials
'Response.Write formdata
'Response.end
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateContainerQtyRemaining.asp", "ChemInv", FormData)

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Change Amount Remaining in an Inventory Conatainer</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If IsNumeric(httpresponse) then 
				If CLng(httpResponse) > 0 then
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					ContainerID = Request("ContainerID")
					Response.Write "<center><SPAN class=""GuiFeedback"">Amount remaining has been changed.</SPAN></center>"
					'Response.Write "<SCRIPT LANGUAGE=javascript>opener.location.reload();opener.focus();window.close()</SCRIPT>"  
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("LocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & ContainerID & "); opener.focus(); //window.close();</SCRIPT>" 
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Amount remaining could not be changed</SPAN></center>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
				Response.Write httpresponse
				Response.end
			End if
			%>

		</TD>
	</TR>
</TABLE>
</Body>