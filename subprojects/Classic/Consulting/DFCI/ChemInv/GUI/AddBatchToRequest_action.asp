<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->


<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim Cmd
Dim Conn
Dim httpResponse
Dim FormData
Dim ServerName

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

CSUserName = Session("UserName" & "cheminv")
CSUserID = Session("UserID" & "cheminv")

'Response.Write(Request.Form & "<br>")

'-- Write Receipt to INV_DOCS
APIURL = "/cheminv/api/AddBatchRequestReceipt.asp"
QueryString = "RequestID=" & Request("RequestID")
QueryString = QueryString & "&QtyAvailable=" & Request("QtyAvailable")
QueryString = QueryString & "&QtyRequired=" & Request("QtyRequired")
QueryString = QueryString & "&FIELD_3=" & Request("FIELD_3")
QueryString = QueryString & Credentials
ReceiptID = CShttpRequest2("POST", ServerName, APIURL, "ChemInv", QueryString)
'Response.write("ReceiptID: " & ReceiptID & "<br>")
'Response.End

'-- Write Worksheet to INV_DOCS
APIURL = "/cheminv/api/AddBatchRequestWorksheet.asp"
QueryString = "RackIDList=" & Request("RackIDList")
QueryString = QueryString & "&ContainerIDList=" & Request("ContainerIDList")
QueryString = QueryString & "&ContainerGridPositionIDList=" & Request("ContainerGridPositionIDList")
QueryString = QueryString & "&RequestID=" & Request("RequestID")
QueryString = QueryString & Credentials
WorksheetID = CShttpRequest2("POST", ServerName, APIURL, "ChemInv", QueryString)
'Response.write("WorksheetID: " & WorksheetID & "<br>")
'Response.End

FormData = Request.Form & Credentials
APIURL = "/cheminv/api/AddBatchToRequest.asp"
'Response.End
httpResponse = CShttpRequest2("POST", ServerName, APIURL, "ChemInv", FormData)


'Response.End

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Fulfill Batch Request</title>
<script language=javascript src="/cheminv/choosecss.js"></script>
<script language=javascript src="/cheminv/gui/refreshgui.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	var openNodes = "<%=Session("TreeViewOpenNodes" & TreeID)%>";
//-->
</script>
</head>
<body>
<br><br><br><br><br><br>
<table align="center" border="0" cellpadding="0" cellspacing="0" bgcolor="#ffffff">
	<tr>
		<td height="50" valign="middle" nowrap>
			<%
			If IsNumeric(httpresponse) then 
				If Clng(httpResponse) > 0 then
					Session("sTab") = "Requests"
					LocationName = Replace(Session("CurrentLocationName"), "\", "\\")
					Response.Write "<center><SPAN class=""GuiFeedback"">Request has been processed</SPAN><center>"
					Response.Write	""

					'-- Ok button
					Response.Write "<P><center><a HREF=""Ok"" onclick=""if (opener){opener.location.reload();} window.close(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a>"

					'-- Print Receipt Button
					Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/cheminv/gui/viewdoc.asp?docid=" & ReceiptID & "', 'DiagReceipt', 2); return false;"" title=""Print Receipt"">")
					Response.Write("<img SRC=""/cheminv/graphics/sq_btn/print_receipt_btn.gif"" border=""0"" title=""Close dialog window""></a>")

					'-- Print Worksheet Button
					Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/cheminv/gui/viewdoc.asp?docid=" & WorksheetID & "', 'DiagWorksheet', 2); return false;"" title=""Print Worksheet"">")
					Response.Write("<img SRC=""/cheminv/graphics/sq_btn/print_worksheet_btn.gif"" border=""0"" title=""Close dialog window""></a>")

					Response.Write("</center>")
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
					Response.Write "<center><SPAN class=""GuiFeedback"">Request could not be processed</SPAN></center>"
					Response.Write "<P><center><a HREF=""Ok"" onclick=""opener.focus();window.close(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0"" title=""Close dialog window""></a></center>"		
				End if
			Else
				Response.Write FormatApiError(APIURL, httpresponse)
				Response.end
			End if
			
			Function FormatApiError(APIURL, ErrMsg)
				FormatApiError = "<center><table width=""80%""><tr><th nowrap valign=""top"">API Error at:</th><td>" & APIURL & "</td></tr><tr><th nowrap valign=top>Oracle Error:</th><td>" & ErrMsg & "</td></tr></table></center>"
			End function
			%>
		</td>
	</tr>
</table>
</body>