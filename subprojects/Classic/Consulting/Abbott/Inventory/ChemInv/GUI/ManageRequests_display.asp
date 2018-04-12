<%@ Language=VBScript %>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Manage Requests</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

bDebugPrint = false
bWriteError = False
strError = "Error:ManageRequests_display.asp<BR>"

dateFormatString = Application("DATE_FORMAT_STRING")
UserID = request("userID")
DeliverToLocationID = request("DeliverToLocationID")
CurrentLocationID = request("CurrentLocationID")
containerBarcode = request("containerBarcode")
fromDate = request("fromDate")
toDate = request("toDate")
RequestType = Ucase(request("RequestType"))

if UserID = "" then username = "NULL"
if RequestType = "" then RequestType = "PENDING"
if DeliverToLocationID = "" then DeliverToLocationID =0
if CurrentLocationID = "" then CurrentLocationID =0
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
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetRequests(?,?,?,?,?,?,?,?,?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERTOLOCATIONID",131, 1, 0, DeliverToLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTLOCATIONID",131, 1, 0, CurrentLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PFROMDATE",200, 1, 200, FromDate)
Cmd.Parameters.Append Cmd.CreateParameter("PTODATE",200, 1, 200, ToDate)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTCOMMENTS",200, 1, 2000, Comments)
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, 1, 30, UserID)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERBARCODE",200, 1, 30, ContainerBarcode)
Cmd.Parameters.Append Cmd.CreateParameter("PRequestType",200, 1, 30, RequestType)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTTYPEID",131, 1, 0, 1)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTSTATUSID",131, 1, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPTONAME",200, 1, 255, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE

if RequestType = "PENDING" then
	caption = "The following requests are pending:"
	disable1 = "disabled"
else
	caption = "The following requests have been delivered:"
	disable2 = "disabled"
end if

%>
	<center>
	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="deliver_request_action.asp" method="POST">
	<table border="1">
	<tr>
		<th>
			&nbsp;
		</th>
		<th>
			ContainerID
		</th>
		<th>
			Container Name
		</th>
		<th>
			Requested By
		</th>
		<th>
			Delivery Location
		</th>
		<th>
			Amount
		</th>
		<th>
			Date Requested
		</th>
		<%if Ucase(RequestType) = "PENDING" then
			colSpan = "9"	
		%>
		<th>
			Date Required
		</th>
		<th>
			Delivered?
		</th>
		<%else
			colSpan = "8"
		%>
		<th align="center">
			Date Delivered
		</td>
		<%end if%>
	</tr>
<%
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=""" & colSpan & """><span class=""GUIFeedback"">No requests found</Span></TD></tr>")
	Else
		While (Not RS.EOF)
			ContainerID = RS("container_id_fk")
			ContainerBarcode = RS("Barcode")
			ContainerName = RS("Container_Name") 
			RequestID = RS("Request_ID")
			RequestUserID = RS("RUserID")
			QtyRequired = RS("Qty_Required")
			UOMAbv = RS("unit_abreviation")
			DateRequested = RS("timestamp")
			DateRequired = RS("Date_Required")
			DateDelivered = RS("Date_Delivered")
			DestinationLocationName = RS("Location_Name")
			editData = "ContainerID=" & ContainerID & "&ContainerName=" & ContainerName & "&UOMAbv=" & UOMAbv &  "&RequestID=" & RequestID 
			Comments = RS("request_comments")	
%>
			<tr>
				<%if Ucase(RequestType) = "PENDING" then%>
				<td align="center"> 
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=edit&amp;<%=editdata%>', 'Diag', 1); return false">Edit</a>
				</td>
				<%else%>
				<td align="center"> 
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=undodelivery&amp;<%=editdata%>', 'Diag', 1); return false">Undo</a>
				</td>
				<%end if%>
				<td align="center"> 
					<span title="<%=Comments%>"><%=ContainerBarcode%></span> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(ContainerName, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(RequestUserID, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(DestinationLocationName, 15, "")%> 
				</td>
				<td align="right"> 
					<%=QtyRequired & " " & UOMAbv%>
				</td>
				<td align="center">
					<%=TruncateInSpan(DateRequested, 11, "")%>
				</td>
			<%if Ucase(RequestType) = "PENDING" then%>
				<td align="center">
					<%=TruncateInSpan(DateRequired, 11, "")%>
				</td>
				
				<td align="center">
					<input type="checkbox" name="DeliveredRequestIDList" value="<%=RequestID%>">
				</td>
			<%else%>
				<td align="center">
					<%=TruncateInSpan(DateDelivered, 11, "")%>	
				</td>
			<%end if%>	
			</tr>
			<%rs.MoveNext
		Wend%>
	</table>
<%	
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing
End if
%>
<table>
	<tr>
		<td colspan="8" align="right"> 
			<input type="image" src="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21">
		</td>
	</tr>	
</table>
</form>
</center>
</body>
</html>



