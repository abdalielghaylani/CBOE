<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

bDebugPrint = false
bWriteError = False
strError = "Error:ManageOrders_display.asp<BR>"

dateFormatString = Application("DATE_FORMAT_STRING")
ShipToName = Request("ShipToName")
DeliveryLocationID = Request("DeliveryLocationID")
OrderStatusID = Request("OrderStatusID")
fromDate = Request("fromDate")
toDate = Request("toDate")

'for each key in Request.Form
'	Response.Write key & "=" & request(key) & "<BR>"
'next

'if ShipToName = "" then ShipToName = "NULL"
if OrderStatusID = "" then OrderStatusID = "1"
if DeliveryLocationID = "" then DeliveryLocationID = 0
if fromDate = "" then
	fromDate = "NULL"
Elseif IsDate(fromDate) then
	fromDate = GetOracleDateString3(fromDate)
Else
	strError = strError & "From Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if
if toDate = "" then
	toDate = "NULL"
Elseif IsDate(toDate) then
	toDate = GetOracleDateString2(toDate)
Else
	strError = strError & "To Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Response.Expires = -1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetOrders(?,?,?,?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPTONAME",200, 1, 255, GetOracleString(ShipToName))
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERYLOCATIONID",131, 1, 0, DeliveryLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PORDERSTATUSID",131, 1, 0, OrderStatusID)
Cmd.Parameters.Append Cmd.CreateParameter("PFROMDATE",200, 1, 200, FromDate)
Cmd.Parameters.Append Cmd.CreateParameter("PTODATE",200, 1, 200, ToDate)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERBARCODE",200, 1, 30, ContainerBarcode)
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE

if OrderStatusID = "1" then
	caption = "The following orders are new:"
elseif OrderStatusID = "2" then	
	caption = "The following orders have been shipped:"
elseif OrderStatusID = "3" then	
	caption = "The following orders have been closed:"
elseif OrderStatusID = "4" then	
	caption = "The following orders have been cancelled:"
end if

%>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Manage Requests</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<SCRIPT LANGUAGE="JavaScript">
function ChooseAction(){
	var OrderStatusID = document.form1.OrderStatusID.value;

	if (OrderStatusID == "1") 
		action = "ShipOrder_action.asp";
	else if (OrderStatusID == "2") 
		action = "";
	
	
	document.form1.action = action;	
	document.form1.submit();
	//alert(document.form1.ApprovedRequestIDList.value);
	//alert(document.form1.DeclinedRequestIDList.value);
	//ApproveRequestSample_action.asp
}
</SCRIPT>
</head>
<body>
	<center>
	<form name="form1" action="" method="POST" OnSubmit="ChooseAction();return false;">
	<INPUT TYPE="hidden" NAME="OrderStatusID" VALUE="<%=OrderStatusID%>">
	<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0">
	<TR><TD>
	<table border="0" cellspacing="0" width="100%" cellpadding="2" align="left">
		<tr>
			<td align="right" valign="top" nowrap>
					<%if OrderStatusID = "1" then %>
					<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Order_frset.asp?action=create&clear=1', 'Diag', 1); return false;">Create Order</a>
					<%elseif OrderStatusID = "2" then%>
					<!--<a class="MenuLink" HREF="#" onclick="document.form1.Action.value='ship';document.form1.submit(); return false">Ship Order</a>-->
					<%end if%>
			</td>
		</tr>
		<TR><TD ALIGN="center"><span class="GUIFeedback"><%=Caption%></span></TD></TR>
	</table>
	</TD></TR>
	<TR><TD>
	<table border="1">
	<tr>
		<th>
			&nbsp;
		</th>
		<th>
			Order ID
		</th>
		<th>
			Num Containers
		</th>
		<th>
			Ship to Name
		</th>
		<th>
			Order Status
		</th>
		<th>
			Delivery Location
		</th>
		<th>
			Date Created
		</th>
		<%if OrderStatusID = "1" then%>
		<th>
			Ship
		</th>
		<%elseif OrderStatusID ="2" or OrderStatusID="3" then%>
		<th align="center">
			Date Shipped
		</th>
		<%if OrderStatusID="3" then%>
			<th>
				Date Received
			</th>
		<%end if%>
		<!--
		<th align="center">
			Date Received
		</th>
		-->
		<%end if%>
	</tr>
<%
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=9><span class=""GUIFeedback"">No orders found.</Span></TD></tr>")
	Else
		While (Not RS.EOF)
			OrderID = RS("order_id")
			NumContainers = RS("NumContainers")
			ShipToName = RS("ship_to_name")
			OrderStatusID = RS("order_status_id_fk")
			OrderStatusName = RS("order_status_name")
			DeliveryLocationID = RS("delivery_location_id_fk")
			DeliveryLocationName = RS("location_name")
			DateCreated = RS("date_created")
			DateShipped = RS("date_shipped")
			DateReceived = RS("date_received")
			editData = "OrderID=" & OrderID
			Path = RS("Path")
			link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_frset.asp?Clear=1&Action=edit&OrderID=" & OrderID & "', 'Diag', 2); return false"">Create Order</a>"
			
%>
			<tr>
				<%if OrderStatusID = "1" then%>
				<td align="center"> 
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Order_frset.asp?clear=1&action=edit&amp;<%=editdata%>', 'Diag', 1); return false">Edit</a>
					| <a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/CancelOrder.asp?clear=1&<%=editdata%>', 'Diag', 1); return false">Cancel</a>
				</td>
				<%else%>
				<td align="center"> 
					<a Class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Order_list.asp?Clear=1&Action=edit&OrderID=<%=OrderID%>', 'Diag', 2); return false">View</a>
					<!--<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=undodelivery&amp;<%=editdata%>', 'Diag', 1); return false">Undo</a>-->
				</td>
				<%end if%>
				<td align="center"> 
					<%=OrderID%>
				</td>
				<td align="center"> 
					<%=NumContainers%>
				</td>
				<td align="center"> 
					<%=TruncateInSpan(ShipToName, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(OrderStatusName, 15, "")%> 
				</td>
				<td align="center"> 
					<span title="<%=Path%>"><%=DeliveryLocationName%> </span>
				</td>
				<td align="center">
					<%=TruncateInSpan(DateCreated, 11, "")%>
				</td>
			<%if OrderStatusID = "1" then%>
				<td align="center">
					<input type="checkbox" name="ShippedOrderIDList" value="<%=OrderID%>")>
				</td>	
			<%elseif OrderStatusID ="2" or OrderStatusID="3" then%>
				<td align="center">
					<%=TruncateInSpan(DateShipped, 11, "")%>	
				</td>
				<%if OrderStatusID="3" then%>
					<td align="center">
						<%=TruncateInSpan(DateReceived, 11, "")%>	
					</td>
				<%end if%>
				<!--
				<td align="center">
					<%=TruncateInSpan(DateReceived, 11, "")%>	
				</td>
				-->
			<%end if%>	
			</tr>
			<%rs.MoveNext
		Wend%>
<%	
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing
End if
%>
	</table>
	</TD></TR>
	<%if OrderStatusID = "1" or OrderStatusID = "2" then%>
	<TR><TD ALIGN="center"><BR>
		<input type="image" src="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21">
	</TD></TR>
	<%end if%>
</form>
</center>
</body>
</html>



