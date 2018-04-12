<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
	ContainerID = Request("ContainerID")
	numRows = Request("numRows")
	if numRows = "" then numRows = 6
	Dim Cmd
	Dim Conn
	Dim SQL
	
	'SQL="SELECT c.raid, rid," &_
	'	"		getLocationPath(new_value) AS LocationPath, TimeStamp," &_ 
	'	"		(SELECT location_name from inv_locations WHERE location_id = new_value) AS LocationName," &_
  	'	"		(SELECT old_value from audit_column where column_name= 'CURRENT_USER_ID_FK' AND RAID = c.raid) AS fromUser," &_
	'	"		(SELECT new_value from audit_column where column_name= 'CURRENT_USER_ID_FK' AND RAID = c.raid) AS toUser " &_
	'	"FROM	audit_column c, audit_row r " &_
	'	"WHERE	c.raid = r.raid " &_
	'	"AND	column_Name = 'LOCATION_ID_FK' " &_
	'	"AND	rid = (SELECT rid from inv_containers where container_ID = ?)"
	'Call GetInvCommand(, adCmdText)	
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".GUIUTILS.GetRecentLocations(?,?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERID",131, 1, 0, ContainerID)
	Cmd.Parameters("PCONTAINERID").Precision = 9	
	Cmd.Properties ("PLSQLRSet") = TRUE
	Cmd.Parameters.Append Cmd.CreateParameter("PNumRows",5, 1, 0, numRows)  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
%>
<html>
<head>
	<title>Recent Container Location History</title>
	<script LANGUAGE="javascript">
		window.focus();
	</script>

	<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</head>
<body>
	<center>
	<form name="form1" action="" method="post">
	<span class="GuiFeedback">Recent Container History:</span>
	<select name="numRows">
		<%
		for i = 6 to 51 step 5
			if cint(numRows) = i then selectText = "selected"
			Response.Write "<option value=""" & i & """ " & selectText & ">" & i-1 & "</option>"
			selectText = ""
		next
		%>
	</select>
	<br><br>
	<table border="1" cellpadding="3" cellspacing="0">
	<tr>
		<th align="center">
			Location
		</th>
		<th align="center">
			From User
		</th>
		<th align="center">
			To User
		</th>
		<th align="center">
			Date
		</th>
	</tr>
<%
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No history found for this container</Span></TD></tr></table>")
	Else
		While (Not RS.EOF)
%>
			<tr>
				<td>
					<span title="<%=RS("LocationPath").value%>"><%= htmlNull(RS("LocationName").value)%></span>
				</td>
				<td align="center">
					<%= htmlNull(RS("fromUser").value)%>
				</td>
				<td align="center">
					<%= htmlNull(RS("toUser").value)%>
				</td>
				<td>
					<%=RS("TimeStamp").value%>
				</td>
			</tr>
			<%rs.MoveNext
		Wend
		Response.Write "</table>"
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing		
%>
<br>
<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
<a HREF="#" onclick="document.form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
</form>
	
</center>
</body>
</html>
