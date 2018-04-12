<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName

DeliveryLocationID = Request("LocationID")
ShipToName = Request("ShipToName")
ShippingConditions = Request("ShippingConditions")
SampleContainerIDs = Request("SampleContainerIDs")
OrderID = Request("OrderID")
Action = Request("Action")


'for each key in Request.Form
'	Response.Write key & "=" & request(key) & "<BR>"
'next
'Response.End


ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

if Action = "save" then
	if isEmpty(OrderID) or OrderID = "" then
		APIPage = "CreateOrder.asp"
		FormData = "DeliveryLocationID=" & DeliveryLocationID
		FormData = FormData & "&ShipToName=" & server.urlencode(ShipToName)
		FormData = FormData & "&ShippingConditions=" & server.urlencode(ShippingConditions)
		FormData = FormData & "&SampleContainerIDs=" & SampleContainerIDs
		FormData = FormData & Credentials
		'FormData = FormData & "" & 
		StatusText = "Order created."
	else
		APIPage = "EditOrder.asp"
		FormData = "OrderID=" & OrderID
		FormData = FormData & "&DeliveryLocationID=" & DeliveryLocationID
		FormData = FormData & "&ShipToName=" & server.urlencode(ShipToName)
		FormData = FormData & "&ShippingConditions=" & server.urlencode(ShippingConditions)
		FormData = FormData & "&SampleContainerIDs=" & SampleContainerIDs
		FormData = FormData & Credentials
		'FormData = FormData & "" & 
		StatusText = "Order edited."
	end if
Else
	Response.Write "Error: Action is required."
	Response.end
End if	

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/" & APIPage , "ChemInv", FormData)
out = httpResponse
'Response.Write httpResponse
'Response.end
OrderID = httpResponse
Session("OrderID") = OrderID

%>

<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(out) then
				if Clng(out) > 0 then
					containerList = out
					theAction = "Success"
				Else
					theAction = "WriteAPIError"
				End if
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "Success"
					Response.Write "<SPAN class=""GuiFeedback"">" & StatusText & "</SPAN>"
					Response.Write "<script language=javascript>"
					Response.Write	"if (parent.opener.top.DisplayFrame){parent.opener.top.DisplayFrame.location.reload();}"
					'Response.Write "if (parent.opener) {alert(parent.opener.top.DisplayFrame.location);}"
					'Response.Write	"if (opener){opener.location.reload();}"
					Response.Write "</script>"
					'Response.write "<Script language=javascript>window.close();</script>"		
					Response.Write "<P><center><a HREF=""#"" ONCLICK=""parent.location.href='order_frset.asp?OrderID=" & out & "';""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>New Container cannot be created</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
