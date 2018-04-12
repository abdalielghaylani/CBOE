<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim action
Dim httpResponse
Dim FormData
Dim ServerName
Dim Conn
Dim Cmd
Dim RS
Dim LocationID
Dim ShipToName
Dim SampleContainerIDs
Dim OrderStatusID
Dim ShippingConditions

action = Request("action")
OrderID = Request("OrderID")
InvSchema = Application("CHEMINV_USERNAME")
dbkey = "cheminv"

Call GetInvConnection()

if( action = "AssignContainer" ) then
    NewContainerID = Request("ContainerID")
    StatusText = "Container assigned to order."
elseif( action = "AssignRequest" ) then
    ' Get the list of containers already assigned to this request
    StatusText = "Request assigned to order."
end if

Set Cmd = GetCommand(Conn, InvSchema & ".REQUESTS.GETORDER", 4)
Cmd.Parameters.Append Cmd.CreateParameter("ORDERID", 5, 1, 0, OrderID) 
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
if NOT (RS.EOF OR RS.BOF) then
	LocationID = RS("delivery_location_id_fk")
	ShipToName = RS("ship_to_name")
	SampleContainerIDs = RS("SampleContainerIDs")
	'OrderStatusID = RS("Order_Status_ID_FK")
	ShippingConditions = RS("Shipping_Conditions")
	
	if( not IsNull(ShipToName) ) then
	    ShipToName = server.urlencode(ShipToName)
	end if	
	
	if( not IsNull(ShippingConditions) ) then
	    ShippingConditions = server.urlencode(ShippingConditions)
	end if
	
	if not isNull(SampleContainerIDs) then
	    SampleContainerIDs = SampleContainerIDs & "," & NewContainerID
	else
	    SampleContainerIDs = NewContainerID
	end if
end if
		
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")

FormData = "OrderID=" & OrderID
FormData = FormData & "&DeliveryLocationID=" & LocationID
FormData = FormData & "&ShipToName=" & ShipToName
FormData = FormData & "&ShippingConditions=" & ShippingConditions
FormData = FormData & "&SampleContainerIDs=" & SampleContainerIDs
FormData = FormData & Credentials

'Response.Write FormData
'Response.end

httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/EditOrder.asp" , "ChemInv", FormData)
out = httpResponse


%>

<html>
<head>
<title><%=Application("appTitle")%></title>
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
			Select Case theAction
				Case "Success"
					Response.Write "<SPAN class=""GuiFeedback"">" & StatusText & "</SPAN>"
					Response.Write "<script language=javascript>"
					Response.Write "  function OnOK()"
					Response.Write "  {"										
                    Response.Write "      window.close();"
					Response.Write "  }"
				    Response.Write "</script>"
				    Response.Write "<P><center><a HREF=""#"" ONCLICK=""OnOK();return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"					
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
