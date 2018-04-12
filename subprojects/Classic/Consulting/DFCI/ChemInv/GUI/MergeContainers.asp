<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn

Action = Request("Action")
ContainerID = Request("ContainerID")
ParentContainerID = Request("ParentContainerID")
MergeContainerIDs = Request("MergeContainerIDs")

if MergeContainerIDs <> "" then Action = ""

titleText = "Merge Containers"
instructionTextA = "Select the containers you want to merge with the current container."
instructionTextB = "Confirm the merge."

if Action <> "SelectIDs" then
	'Response.Write MergeContainerIDs
	'Response.Write Session("QtyMax")
	'Response.End	
	QtyMax = CLng(Session("QtyMax"))
	Call GetInvConnection()
	SQL = "SELECT sum(qty_remaining) AS sum FROM inv_containers WHERE container_id in (" & MergeContainerIDs & "," & ContainerID & ")"
	Set rsTotalQty = Conn.Execute(SQL)

	TotalQty = CLng(rsTotalQty("sum"))
	
	rsTotalQty.Close
	Set rsTotalQty = nothing
	
	if TotalQty > QtyMax then
		instructionTextA = "Error: The sum of quantites for the selected containers exceeds the container size.<br><br>Select the containers you want to merge with the current container."
		Action = "SelectIDs"
	end if
end if


%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=titleText%></title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function ValidateQtyChange(){
		var bWriteError = false;
		var MergeContainersIDs = document.form1.MergeContainerIDs.value;
		var errmsg = "Please fix the following problems:\r";
		
		// MergeContainersIDs is required
		if (document.form1.MergeContainerIDs.value.length == 0) {
			errmsg = errmsg + "- You must select a merging container ID.\r";
			bWriteError = true;
		}
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			return true;
		}	
   }	
-->
</script>	
</head>
<body>
<center>
<%if Action = "SelectIDs" then%>
	<form name="form1" action="#" method="POST" onsubmit="return ValidateQtyChange();">
<%else%>
	<form name="form1" action="../gui/MergeContainers_action.asp" method="POST" onsubmit="return ValidateQtyChange();">
	<input Type="hidden" name="MergeContainerIDs" value="<%=MergeContainerIDs%>">
<%end if%>
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="ParentContainerID" value="<%=ParentContainerID%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">
			<%
			if Action = "SelectIDs" then
				Response.Write instructionTextA
			else
				Response.Write instructionTextB
			end if
			%>
			</span><br><br>
		</td>
	</tr>
	<%if ParentContainerID = "" then%>
	<tr>
		<td colspan="2">There are no mergeable containers.</td>
	</tr>
	
	<%else%>
		<%if Action = "SelectIDs" then%>
		<tr>
			<td align="right" valign="top" nowrap>Select Merging Container IDs:</td>
			<td>		
				<%
				SQL = "SELECT container_id AS Value, barcode AS DisplayText FROM inv_containers WHERE container_id = " & ParentContainerID & " OR parent_container_id_fk = " & ParentContainerID & " AND container_id != " & ContainerID & " ORDER BY DisplayText" 
				Response.Write ShowMultiSelectBox("MergeContainerIDs", "", SQL, 100,"" ,RepeatString(30, "&nbsp;") , 10, 1)
				%>
			</td>
		</tr>
		<%else%>
		<tr>
			<td align="right" valign="top" nowrap>Merging Container IDs:</td>
			<td><input TYPE="text" SIZE="20" Maxlength="50" NAME="AllContainerIDs" class="readOnly" VALUE="<%=(ContainerID & "," & MergeContainerIDs)%>" READONLY></td>
		</tr>
		<tr>
			<td align="right" valign="top" nowrap>New Quantity Remaining:</td>
			<td><input TYPE="text" SIZE="20" Maxlength="50" NAME="NewQtyRemaining" class="readOnly" VALUE="<%=TotalQty%>" READONLY></td>
		</tr>

		<%end if%>

	<%end if%>
	<%
	if Action = "SelectIDs" then 
	%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21" id=image1 name=image1>
		</td>
	</tr>	
	<%else%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back();"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21" id=image2 name=image2>
		</td>
	</tr>	
	<%end if%>	
	<tr>
		<td></td>
		<td><%Response.Write RepeatString(30, "&nbsp;")%></td>
	
</table>	
</form>
</center>
</body>
</html>
<%
if Action <> "SelectIDs" then
	Conn.Close
	Set Conn = nothing
end if
%>