<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/API/apiUtils.asp"-->
<html>
<head>
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim out
Dim FormData
Dim ServerName
Dim Credentials
Dim iReceiveCount
Dim SampleContainerIDs
Dim OrderContainerID
Dim aOrderContainerIDs
Dim RemoveContainerID
Dim aRemoveContainerIDs
Dim iRemoveCount
Dim iTotalCount
Dim theAction
Dim ToBeProcessedCount
Dim bOrderClosed

OrderID = Request("OrderID")
ToBeProcessedCount = CInt(Request("ToBeProcessedCount"))
ReceivedList = Request("ReceivedList")
RemovedList = Request("RemovedList")
ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))

theAction = "Success"
bOrderClosed = false

' First remove any containers from the order, if necessary
iRemoveCount = 0
if( Len( RemovedList ) > 0 ) then
    FormData = "RemovedContainerIDs=" & RemovedList
    FormData = FormData & "&OrderID=" & OrderID
    FormData = FormData & Credentials

    httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/RemoveOrderContainers.asp", "ChemInv", FormData )

    if( instr( httpResponse, "Error:" ) or instr( httpResponse, "ORA-" ) ) then
        theAction = "RemoveOrderAPIError"
        out = httpResponse
    elseif instr(httpResponse,",") then
	    arrResponse = split(httpResponse,",")
	    iRemoveCount = ubound(arrResponse) + 1
    else        
	    iRemoveCount = 1
    end if    
end if

' Now receive the containers
iReceiveCount = 0
if( Len( ReceivedList ) > 0 and (theAction = "Success") ) then
    FormData = "ReceivedContainerIDs=" & ReceivedList
    FormData = FormData & "&OrderID=" & OrderID
    FormData = FormData & "&StatusID=" & Application("StatusApproved")
    FormData = FormData & Credentials

    httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/ReceiveOrder.asp", "ChemInv", FormData)    

    if( instr( httpResponse, "Error:" ) or instr( httpResponse, "ORA-" ) ) then
        theAction = "ReceiveOrderAPIError"
        out = httpResponse
    elseif instr(httpResponse,",") then
	    arrResponse = split(httpResponse,",")
	    iReceiveCount = ubound(arrResponse) + 1
    else        
	    iReceiveCount = 1
    end if    
end if

' Close the order if all containers have been addressed
if( (iRemoveCount + iReceiveCount) = ToBeProcessedCount and theAction = "Success" ) then    
    FormData = "OrderID=" & OrderID
    FormData = FormData & Credentials
    
    httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CloseOrder.asp", "ChemInv", FormData)
    
    if( instr( httpResponse, "Error:" ) or instr( httpResponse, "ORA-" ) ) then
        theAction = "CloseOrderAPIError"
        out = httpResponse    
    else 
        bOrderClosed = true
    end if
end if

%>
<title><%=Application("appTitle")%> -- Receive Order</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="JavaScript">
	window.focus();
	
	function OnOK()
	{	    
<%
    if( bOrderClosed ) then
        ' All containers have been handled; close the form	    
%>
        parent.opener.location.reload();
	    top.close();	    
<%
    else
        ' There are still containers left to process, just reload the form        
%>
        top.location.reload();
	    
<%
    end if
%>
	}
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD>
			<%
			Select Case theAction
				Case "Success"
				    Response.Write "<center><SPAN class=""GuiFeedback"">"
				    if( iRemoveCount = 1 ) then
				        Response.Write "One container has been removed.<br />"
				    elseif ( iRemoveCount > 1 ) then
				        Response.Write iRemoveCount & " containers have been removed.<br />"
				    end if
				    
				    if( iReceiveCount = 1 ) then
				        Response.Write "One container has been received.<br />"
				    elseif ( iReceiveCount > 1 ) then
				        Response.Write iReceiveCount & " containers have been received.<br />"
				    end if				    
				    
				    if( bOrderClosed ) then
				        Response.Write "All containers have been accounted for; the order is now closed.<br />"
				    end if
                    Response.Write "<P><a HREF=""#"" onclick='OnOK();return false;'><img SRC=../graphics/ok_dialog_btn.gif border=0></a>"
				    
				    Response.Write "</SPAN></center><br />"
				Case "EditOrderWriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>Could not remove containers from order</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "EditOrderWriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
				Case "ReceiveOrderAPIError"
				    Response.Write "<center><P><CODE>" & out & "</CODE></P></center>"
				    Response.Write "<center><SPAN class=""GuiFeedback"">Containers could not be received.</SPAN></center>"
				    Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
                case "RemoveOrderAPIError"
                    Response.Write "<center><P><CODE>" & out & "</CODE></P></center>"
				    Response.Write "<center><SPAN class=""GuiFeedback"">Containers could not be removed.</SPAN></center>"
				    Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				case "CloseOrderAPIError"
    				Response.Write "<center><P><CODE>" & out & "</CODE></P></center>"
				    Response.Write "<center><SPAN class=""GuiFeedback"">Order could not be closed.</SPAN></center>"
				    Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End select
			%>
		</TD>
	</TR>
</TABLE>
</Body>
</html>