<%@ Language=VBScript %>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Approvals</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">

function UncheckReject(element){
	var NumContainers = document.form1.NumContainers.value;
	if (NumContainers == 1) {
		document.form1.RejectedContainerIDList.checked = false;
	}
	else {
		for (i=0; i<NumContainers; i++) {
			if (document.form1.RejectedContainerIDList[i].value == element.value){
				document.form1.RejectedContainerIDList[i].checked = false;
			}
		}	
	
	}
}

function UncheckApprove(element){
	var NumContainers = document.form1.NumContainers.value;
	if (NumContainers == 1) {
		document.form1.ApprovedContainerIDList.checked = false;
	}
	else {
		for (i=0; i<NumContainers; i++) {
			if (document.form1.ApprovedContainerIDList[i].value == element.value){
				document.form1.ApprovedContainerIDList[i].checked = false;
			}
		}	
	
	}
}
</script>

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
strError = "Error:ManageApprovals_display.asp<BR>"

CurrentUserID = request("CurrentUserID")
CurrentLocationID = request("CurrentLocationID")
containerBarcode = request("containerBarcode")
fromDate = request("fromDate")
toDate = request("toDate")

dbkey = Application("appkey")

if CurrentLocationID = "" then CurrentLocationID =0
if CurrentUserID = "" then CurrentUserID = NULL
if ContainerBarcode = "" then ContainerBarcode = NULL
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

Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".APPROVALS.GetContainers(?,?,?,?,?,?)}", adCmdText)	
'GetInvConnection()
'Set Cmd = GetCommand(Conn, Application("CHEMINV_USERNAME") & ".APPROVALS.GetContainers", 4)		 

Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTLOCATIONID",131, 1, 0, CurrentLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PFROMDATE",200, 1, 200, FromDate)
Cmd.Parameters.Append Cmd.CreateParameter("PTODATE",200, 1, 200, ToDate)
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, 1, 30, CurrentUserID)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERBARCODE",200, 1, 50, ContainerBarcode)
Cmd.Parameters.Append Cmd.CreateParameter("PSTATUSID",adNumeric, 1, 0, Application("StatusCertified"))
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	'Response.End
	caption = "The following containers await approval:"
%>
	<center>
	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="ApproveContainer_action.asp" method="POST">
	<input type="hidden" name="StatusApproved" value="<%=Application("StatusApproved")%>">
	<input type="hidden" name="StatusDefault" value="<%=Application("DefConStatusID")%>">
	<table border="1">
	<tr>
		<th>
		</th>
		<th>
			ContainerID
		</th>
		<th>
			Container Name
		</th>
		<th>
			Current User
		</th>
		<th>
			Container Status
		</th>
		<th>
			Current Location
		</th>
		<th>
			Date Certified
		</th>
		<th>
			Approve
		</th>
		<th>
			Reject
		</th>
	</tr>
<%
	NumContainers = 0
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=8><span class=""GUIFeedback"">No containers found.</Span></TD></tr>")
	Else
		While (Not RS.EOF)
			ContainerID = RS("container_id")
			ContainerBarcode = RS("Barcode")
			ContainerName = RS("Container_Name") 
			CurrentLocation = RS("Location_Name")
			CurrentUser = RS("CurrentUser")
			Status = RS("Container_Status_Name")
			DateCertified = RS("Date_Certified")
			NumContainers = NumContainers+1
%>
			<tr>
				<td align="right">	
							<%IF CBool(Application("SHOW_DOCMANAGER_LINK")) then%>
								<%If Session("SEARCH_DOCS" & dbkey) then%>
								<a class="MenuLink" href="Manage%20Documents%20for%20this%20container" onclick="OpenDialog('/cheminv/gui/manageDocuments.asp?FK_value=<%=ContainerID%>&FK_name=CONTAINER%20ID&Table_Name=INV_CONTAINERS&LINK_TYPE=CHEMINVCONTAINERID', 'Documents_Window', 2); return false;" title="Manage documents associated to this compound">Manage Documents</a>
								<%else%>
								<a class="MenuLink" href="Manage%20Documents%20for%20this%20container" onclick="alert('You do not have the appropriate privileges to add or view documents. Please ask the administrators to grant you DOCMGR_EXTERNAL role and log back in to try again.'); return false;">Manage Documents</a>
								<%end if%>
								<br>
							<%end if%>
				</td>
				<td align="center"> 
					<%=ContainerBarcode%>
				</td>
				<td align="center"> 
					<%=TruncateInSpan(ContainerName, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(CurrentUser, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(Status, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(CurrentLocation, 15, "")%> 
				</td>
				<td align="center">
					<%=TruncateInSpan(DateCertified, 13, "")%>	
				</td>
				<td align="center">
					<input type="checkbox" name="ApprovedContainerIDList" value="<%=ContainerID%>" onclick="UncheckReject(this);")>
				</td>	
				<td align="center">
					<input type="checkbox" name="RejectedContainerIDList" value="<%=ContainerID%>" onclick="UncheckApprove(this);")>
				</td>	
			</tr>
			<%rs.MoveNext
		Wend%>
	</table>
	<input type="hidden" name="NumContainers" value="<%=NumContainers%>">
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



