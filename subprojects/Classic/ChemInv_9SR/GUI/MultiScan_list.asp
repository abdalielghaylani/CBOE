<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint

Response.Expires = -1
InvSchema = Application("CHEMINV_USERNAME")
if Session("DefaultLocation") = "" then Session("DefaultLocation")= GetUserProperty(Session("UserNameCheminv"),"INVDefLoc")
If Session("DefaultLocation")="" OR IsNULL(Session("DefaultLocation"))  then Session("DefaultLocation")= 0	

bDebugPrint = false
dbkey = "cheminv"

clear = Request.QueryString("clear")
if len(clear) = 0 then clear = 0 
Set myDict = multiSelect_dict

if clear then
	myDict.RemoveAll
	Response.Redirect "multiscan_list.asp?message=" & Request("message")
Else
	AddContainerID = Request("AddContainerID")
	if len(AddContainerID) > 0 then
		if NOT myDict.Exists(Trim(AddContainerID)) then
			myDict.Add Trim(AddContainerID), true
		End if
	End if
	RmvContainerID = Request("RmvContainerID")
	if len(RmvContainerID) > 0 then
		if myDict.Exists(Trim(RmvContainerID)) then
			myDict.Remove(Trim(RmvContainerID))
		End if
	End if
End if
%>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</HEAD>
<BODY>
<%if myDict.Count > 0 then%>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<%If Session("INV_CHECKOUT_CONTAINER" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?multiscan=1&multiSelect=true&action=out' , 'Diag', 1); return false">Check Out</a>
				|
			<%end if%>
			<%If Session("INV_CHECKIN_CONTAINER" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?multiscan=1&multiSelect=true&action=in', 'Diag', 1); return false">Check In</a>
				|
			<%end if%>
			<%If Session("INV_MOVE_CONTAINER" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MoveContainer.asp?multiscan=1&multiSelect=true', 'Diag', 1); return false">Move Containers</a>
				|
			<%End if%>
			<%If Session("INV_RETIRE_CONTAINER" & dbkey) then%>
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RetireContainer.asp?multiscan=1&multiSelect=true', 'Diag', 1); return false">Retire Containers</a>
				|
			<%End if%>
			<%If Session("INV_DELETE_CONTAINER" & dbkey) then%>	
				<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/DeleteContainer.asp?multiscan=1&multiSelect=true', 'Diag', 1); return false">Delete Containers</a>		
				|
			<%End if%>		
				<a class="MenuLink" HREF="multiscan_list.asp?clear=1" target="ListFrame">Clear List</a>			
		</td>
	</tr>
</table>
<BR clear="all"><BR>
<%
	GetInvConnection()
	Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETKEYCONTAINERATTRIBUTES", 4)		 
	Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERIDLIST", 200, 1, 2000, DictionaryToList(myDict)) 
	Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERBARCODELIST", 200, 1, 2000, NULL) 
	if bdebugPrint then
		Response.Write "Parameters:<BR>"
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
			Response.write Session("awolContainerBarcodeList") & "<BR>"	
		'Response.end
	else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		'Get AwolContainer Attributes
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If NOT (RS.EOF AND RS.BOF) then
			temparr = RS.GetRows()
			RecordCount = Ubound(temparr,2) + 1
			RS.MoveFirst
		Else
			RecordCount = 0
		End if
	end if
%>
<center>
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td>
			<table border="0">
			<tr height="40">
				<td colspan="3">
					<font size=1>Click on a link above to perform an action on <BR>all containers on the list.</font>
				</td>
				
				<th colspan="3" align="center">
					&nbsp;
				</th>
			</tr>
			<Tr>
				<th>
					Barcode
				</th>
				<th>
					Container Name
				</th>
				<th>
					Location
				</th>
				<th>
					User
				</th>
				<th>
					Qty Remaining
				</th>
				<th>
					Remove?
				</th>
			</Tr>
		<%
			If (RS.EOF AND RS.BOF) then
				Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No awol containers</Span></TD></tr>")
			Else
				While (Not RS.EOF)
					ContainerID = RS("Container_ID")
					ContainerBarcode = RS("barcode")
					ContainerName = RS("Container_Name")
					LocationName = RS("Location_Name")
					CurrentUserID = RS("User_ID")
					QtyRemaining = RS("Qty_Remaining") & " " & RS("Unit_Abreviation")
					Path = RS("Path")
		%>
					<tr>
						<td align=center>
							<%=ContainerBarcode%>
						</td>
						<td align=right> 
							<%=TruncateInSpan(ContainerName, 15, "")%>
						</td>
						<td align=center> 
							<span title="<%=Path%>"><%=LocationName%></span> 
						</td>
						<td align=center>
							<%=CurrentUserID%>
						</td>
						<td align=center>
							<%=QtyRemaining%>
						</td>
						<td align="center">
							<a class="MenuLink" title="Remove this container from the selection list" HREF="multiscan_list.asp?RmvContainerID=<%=ContainerID%>">remove</a>
						</td>
					</tr>
					<%rs.MoveNext
				Wend
				Response.Write "</table></center>"
			End if
			RS.Close
			Conn.Close
			Set RS = nothing
			Set Cmd = nothing
			Set Conn = nothing
			%>
		</td>
	</tr>
</table>
</center>
<%end if%>
<%
	if myDict.count = 0 then 
		msg = "Scan container barcodes to add them to the selection list."
	Elseif myDict.count > 1 then
		msg = "There are " & myDict.Count & " containers in the selection list."
	End if
	Response.Write "<BR><BR><BR><BR><BR><BR><BR><center><span class=""GUIFeedback"">" & Request("message") & "<BR><BR><BR>" & msg &  "</span></center>"
	Set Session("multiSelectDict") = myDict
	Set myDict = Nothing
%>
</BODY>
</HTML>
