<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Response.Expires = -1

Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint
bDebugPrint = false
bLoad = true

InvSchema = Application("CHEMINV_USERNAME")
dbkey = "cheminv"

clear = Request.QueryString("clear")
Message = Request("message")
RequestID = Request("RequestID")
LocationID = Request("DeliveryLocationID")
ShipToName = Request("ShipToName")
OrderID = Request("OrderID")
if isEmpty(OrderID) then OrderID = Session("OrderID")
selectChckBox = Request("selectChckBox")
'Response.Write OrderID
'Response.End
'if (isEmpty(OrderID)) then OrderID = null
'if not IsEmpy(OrderID) then clear = 0

isFirst = "true"
GetInvConnection()
if len(clear) = 0 then clear = 0 
Set myDict = multiSelect_dict

if clear then
	myDict.RemoveAll
	Session("OrderID") = ""
	OrderID = ""
	selectChckBox = ""
	'Response.Redirect "order_list.asp?message=" & Message
Else
	AddContainerID = Request("AddContainerID")
	if len(AddContainerID) > 0 and not (myDict.Exists(cint(AddContainerID))) then
		bLoad = false
		'add the container to the order
		'if NOT myDict.Exists(Trim(AddContainerID)) then	myDict.Add Trim(AddContainerID), true
		
		'determine the shipped order associated with this container
		SQL = "SELECT DISTINCT order_id_fk FROM inv_order_containers, inv_orders WHERE order_id_fk = order_id AND order_status_id_fk = 2 AND container_id_fk = ?"
		'Response.Write SQL & ":" & AddContainerID
		Set Cmd = GetCommand(Conn, SQL, adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("AddContainerID", 5, 1, 0, AddContainerID)
		Set RS = server.CreateObject("ADODB.recordset")
		RS.Open Cmd
		
		if not (RS.BOF or RS.EOF) then 
			tempOrderID = RS("order_id_fk")
			if not isEmpty(OrderID) and OrderID <> "" then
				myDict.RemoveAll
				selectChckBox = ""				
				msg = msg & "<BR>Only 1 order can be received at a time.<BR>"
			end if
			OrderID = tempOrderID
			bLoad = true
		else
			'error message
			msg = msg & "This container is not associated with a shipped order.<BR>"
		end if
	elseif len(AddContainerID) > 0 and myDict.Exists(cint(AddContainerID)) then
		selectChckBox = selectChckBox & "," & AddContainerID
	End if
	RmvContainerID = Request("RmvContainerID")
	if len(RmvContainerID) > 0 then
		bLoad = false
		if myDict.Exists(cint(RmvContainerID)) then	myDict.Remove(cint(RmvContainerID))
	End if
End if

if not isEmpty(OrderID) and OrderID <> "" and bLoad then
	bLoad = false
	isFirst = "false"
	Set Cmd = GetCommand(Conn, InvSchema & ".REQUESTS.GETORDER", 4)		 
	Cmd.Parameters.Append Cmd.CreateParameter("ORDERID", 5, 1, 0, OrderID) 

	if bdebugPrint then
		Response.Write "Parameters:<BR>"
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
		Response.end
	else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		if NOT (RS.EOF OR RS.BOF) then
			LocationID = RS("delivery_location_id_fk")
			ShipToName = RS("ship_to_name")
			SampleContainerIDs = RS("SampleContainerIDs")
			'Response.Write SampleContainerIDs
		
			arrSampleContainerIDs = split(SampleContainerIDs,",")
			for i = 0 to ubound(arrSampleContainerIDs)
				currContainerID = cint(arrSampleContainerIDs(i))
				'Response.Write DictionaryToList(myDict) & "<BR>"
				'Response.Write currContainerID & "<BR>"
				'Response.Write myDict.Exists(currContainerID) & "<BR>"
				'Response.Write myDict.Exists(cint(currContainerID)) & "<BR>"
				if not myDict.Exists(currContainerID) then myDict.Add currContainerID, true
				'RS.MoveNext
			next
			'Response.Write SampleContainerIDs
			'Response.End
		else
			Response.Write ""
		end if
	end if

elseif not IsEmpty(RequestID) and RequestID <> "" and bLoad then
	'add samples associated with this request to the order, only if the container isn't in another open/shipped order
	SQL = "SELECT container_id_fk FROM inv_request_samples rs WHERE rs.container_id_fk not in (select container_id_fk from inv_order_containers, inv_orders where order_id_fk = order_id and order_status_id_fk in (1,2)) AND request_id_fk = ?"
	'Response.Write SQL & ":" & RequestID
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("RequestID", 5, 1, 0, RequestID)
	Set RS = server.CreateObject("ADODB.recordset")
	RS.Open Cmd

	While not (RS.BOF or RS.EOF)
		rsContainerID = RS("Container_ID_FK")
		if not myDict.Exists(rsContainerID) then myDict.Add rsContainerID, true
		RS.MoveNext
	wend
end if
%>
<HTML>
<HEAD>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
-->
</script>
<SCRIPT LANGUAGE=javascript>
	function SelectMarked(){
				//var reml = document.form1.removeList;
				//var len = reml.value.length
				//if (reml.value.substring(len -1 , len) == ","){
				//	reml.value = reml.value.substring(0, len - 1);
				//}
				//alert(reml.value);
				//document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>";
				elm = document.form1.selectChckBox;
				var SelectedList = " ";
				if (elm.length){
					for (i=0; i< elm.length; i++){
						//alert(elm.length);
						if (elm[i].checked){
							SelectedList = SelectedList + elm[i].value + ","
							//alert(elm[i].value);
							//elm[i].checked = bCheck;
							//if (!bCheck) alert(cbObj[i].value);
						}
					}
					SelectedList = SelectedList.substr(0,(SelectedList.length-1));
				}
				else{
					if (elm.checked){
							SelectedList = elm.value;
							//elm.checked = bCheck;
							//if (!bCheck) alert(elm.value);
					}
				}
				//alert(elm.value);
				//top.ScanFrame.location = "ReceiveOrderScan.asp?selectChckBox=" + elm.value;
				top.ScanFrame.location = "ReceiveOrderScan.asp?selectChckBox=" + SelectedList;
				//document.form1.submit();
				//reml.value =""; 
			}
			
			function CheckAll(bCheck){
				var cbObj = document.form1.selectChckBox;
				
				if (cbObj.length){
					for (i=0; i< cbObj.length; i++){
						if (cbObj[i].checked ^ bCheck){
							cbObj[i].checked = bCheck;
							if (!bCheck) Removals(cbObj[i].value, false);
						}
					}
				}
				else{
					if (cbObj.checked ^ bCheck){
							cbObj.checked = bCheck;
							if (!bCheck) Removals(cbObj.value, false);
						}
				}
				SelectMarked();	
			}
		
		
		
			function Removals(id, bRemove){
				var idc = id + ",";
				var reml = document.form1.removeList;
			
				if (bRemove){
					if (reml.value.indexOf(idc) >=0){
						reml.value = reml.value.replace(idc,"");
					}
				}
				else{
					if (reml.value.indexOf(idc) ==  -1){
						reml.value += id + ",";
					}
				}
				//alert(reml.value);				
			}
			function HappySound(){
				snd1.src='/cheminv/sounds/yes1.wav';
				return true;
			}
	
			function SadSound(){
				snd1.src='/cheminv/sounds/no1.wav';
			}
			
			function SadSound2(){
				snd1.src='/cheminv/sounds/no2.wav';
			}

</SCRIPT>
</HEAD>
<BODY>
<center>
<form name="form1" action="" method="POST">
<INPUT TYPE="hidden" NAME="OrderContainerIDs" VALUE="<%=DictionaryToList(myDict)%>">
<INPUT TYPE="hidden" NAME="Action" VALUE="">
<INPUT TYPE="hidden" NAME="OrderID" VALUE="<%=OrderID%>">
<INPUT TYPE="hidden" NAME="isFirst" VALUE="<%=isFirst%>">
<input type="hidden" name="removeList">
<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0">
<TR><TD>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
				<%if not isEmpty(OrderID) and OrderID <> "" then %>
				<a class="MenuLink" HREF="#" onclick="document.form1.action = 'ReceiveOrder_action.asp'; document.form1.Action.value='receive'; document.form1.submit(); return false">Receive Containers</a>
				<%end if%>
				<%if myDict.Count > 0 then%>
				|
				<a class="MenuLink" HREF="ReceiveOrder_list.asp?clear=1" target="ListFrame">Clear List</a>			
				<%end if%>
		</td>
	</tr>
</table>
</TD></TR>
<%if myDict.Count > 0 then%>
<TR><TD>
<BR clear="all"><BR>
<%
	Session("OrderID") = OrderID
	'GetInvConnection()
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
				Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No containers</Span></TD></tr>")
			Else
				While (Not RS.EOF)
					ContainerID = RS("Container_ID")
					ContainerBarcode = RS("barcode")
					ContainerName = RS("Container_Name")
					LocationName = RS("Location_Name")
					CurrentUserID = RS("User_ID")
					QtyRemaining = RS("Qty_Remaining") & " " & RS("Unit_Abreviation")
					Path = RS("Path")
					
					sChecked = ""
					if len(AddContainerID)>0 and cStr(ContainerID) = AddContainerID then sChecked = "checked"
					if instr(selectChckBox, ContainerID)>0 then sChecked = "checked"
		%>
					<tr>
						<td align=center>
							<span id="_<%=ContainerBarcode%>"><INPUT TYPE="checkbox" VALUE="<%=ContainerID%>" NAME="selectChckBox" onclick="SelectMarked();" <%=sChecked%>><%=ContainerBarcode%></span>
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
							<a class="MenuLink" title="Remove this container from the selection list" HREF="#" ONCLICK="document.location = 'ReceiveOrder_list.asp?OrderID=<%=OrderID%>&RmvContainerID=<%=ContainerID%>';">Remove</a>
						</td>
						<%if sChecked = "checked" then%>
						<script language="javascript">
							SelectMarked();
						</script>
						<%end if%>
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
</TD></TR>
<%end if%>
<TR><TD>
<bgsound src="" id="snd1">
<table border="0" align="center">
<%
	if myDict.count = 0 then 
		msg = msg & "Scan container barcodes to add them to the selection list."
	elseif myDict.count = 1 then 
		msg = msg & "There is 1 container in the selection list."
	Elseif myDict.count > 1 then
		msg = msg & "There are " & myDict.Count & " containers in the selection list."
	End if
	Response.Write "<TR><TD COLSPAN=""2""><BR><center><span class=""GUIFeedback"">" & Request("message") & "<BR>" & msg &  "<BR><BR></span></center></TD></TR>"

%>
</table>
</TD></TR>
</TABLE>
</form>
</center>

<%
	'Set Session("multiSelectDict") = myDict
	'Set myDict = Nothing
%>
</BODY>
</HTML>
<%
if clear then
	response.write "<SCRIPT LANGUAGE=""JavaScript"">top.ScanFrame.location = 'ReceiveOrderScan.asp';</SCRIPT>"
end if
%>
