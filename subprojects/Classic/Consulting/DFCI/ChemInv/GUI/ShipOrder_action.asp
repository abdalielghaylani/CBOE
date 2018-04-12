<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Credentials

'for each key in Request.Form
'	Response.Write key & "=" & request(key) & "<BR>"
'next

if Request.Form("ShippedOrderIDList") = "" and Request.Form("OrderID") = "" then
	Response.Write "<center><SPAN class=""GuiFeedback"">You must check at least one order to ship.</SPAN></center>"
	Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
	Response.end
End if 
OrderID = Request("OrderID")
ShippedOrderIDList = Request("ShippedOrderIDList")
if OrderID <> "" then
	if len(trim(ShippedOrderIDList)) > 0 then
		ShippedOrderIDList = ShippedOrderIDList & "," & OrderID
	else
		ShippedOrderIDList = OrderID
	end if
end if
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
'FormData = Request.Form & Credentials
FormData = "ShippedOrderIDList=" & ShippedOrderIDList
FormData = FormData & Credentials
'Response.Write FormData
'Response.End

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/ShipOrder.asp", "ChemInv", FormData)

'Response.Write httpResponse & ":test"
'Response.End
if instr(httpResponse,",") then
	arrResponse = split(httpResponse,",")
	numShipped = ubound(arrResponse) + 1
else 
	numShipped = 1
end if

action = Request("action")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Ship Order</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>

<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			If IsNumeric(numShipped) then
			    Response.Write "<center><SPAN class=""GuiFeedback"">"
			    if( numShipped = 1 ) then
					Response.Write "One order has been shipped."
				else
				    Response.Write numShipped & " orders have been shipped."
				end if
				Response.Write "</SPAN></center>" & vblf
				if action = "edit" then
					Response.Write "<script language=""JavaScript"">window.close();</script>" 
				else
					Response.Write "<script language=javascript>"
					Response.Write	"if (parent.opener.top.DisplayFrame){parent.opener.top.DisplayFrame.location.reload();}"
					Response.Write "</script>"
					Response.Write "<P><center><a HREF=""3"" onclick=""if(top.ActionFrame){top.ActionFrame.document.form1.submit();} else if(top.ScanFrame){top.document.location.reload();} return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>" 
				end if
			Else
				Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P></center>"
				Response.Write "<center><SPAN class=""GuiFeedback"">Requests could not be approved or declined.</SPAN></center>"
				Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				'Response.Write httpresponse
				'Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>