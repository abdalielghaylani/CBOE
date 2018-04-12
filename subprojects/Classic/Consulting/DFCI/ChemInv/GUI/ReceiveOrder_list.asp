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
Dim iContainerCount
Dim Action
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
Action = Request("Action")
if isEmpty(OrderID) then OrderID = Session("OrderID")
selectChckBox = Request("SelectChckBox")
ScannedContainerID = Request("ScannedContainerID")
if IsNull(ScannedContainerID) then
    ScannedContainerID = ""
end if

isFirst = "true"
GetInvConnection()
if len(clear) = 0 then clear = 0 
Set myDict = multiSelect_dict
iContainerCount = 0

if clear then
	myDict.RemoveAll
	'Session("OrderID") = ""
	'OrderID = ""
	'selectChckBox = ""
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
    ' This needs to be removed?  There is no method for adding containers from requests using
    ' this form.
	
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
var iContainerCount = 0;
function ContainerOnCheck( CheckBoxElement, Action, bUpdateScanFrame )
{
    var ElementID = CheckBoxElement.id;
    var ContainerID = CheckBoxElement.value;
    var objOther = null;
    
    // First uncheck the other option if this one is being checked
    if( CheckBoxElement.checked )
    {
        if( Action == 'Receive' )
        {
            objOther = document.getElementById('Remove' + ContainerID);
        }
        else    // Action == 'Remove'
        {
            objOther = document.getElementById('Receive' + ContainerID);	                    
        }
        if( objOther )
        {
            objOther.checked = false;
        }
    }
    if( bUpdateScanFrame )
    {
        GetCheckBoxLists();
    }
}

function GetList( CheckBoxElement )
{
    var ReturnList = "";
    if( CheckBoxElement )
	{
	    if( CheckBoxElement.length )
	    {
	        // Multiple containers
		    for( i = 0; i < CheckBoxElement.length; i++ )
		    {
			    if( CheckBoxElement[i].checked )
			    {						    
				    ReturnList = ReturnList + CheckBoxElement[i].value + ",";				    
			    }
		    }
		    ReturnList = ReturnList.substr( 0, (ReturnList.length - 1) );
	    }
	    else
	    {
	        // Only one container
		    if( CheckBoxElement.checked )
		    {
				    ReturnList = CheckBoxElement.value;				    
		    }
	    }
	}
	return ReturnList;
}

function GetCheckBoxLists()
{				
	var elm = null;
	var ReceivedList = "";
	var RemovedList = "";
	var bChecked = false;
	
	ReceivedList = GetList( document.form1.ReceiveChckBox );
	RemovedList = GetList( document.form1.RemoveChckBox );	
	
	bChecked = (ReceivedList.length > 0) || (RemovedList.length > 0);
	
	document.form1.ReceivedList.value = ReceivedList;
	document.form1.RemovedList.value = RemovedList;
	
	elm = document.getElementById( "UpdateButton" );
	if( elm )
	{
	    // Only enable the update button if there are actionable items...
	    elm.style.display = bChecked ? "inline" : "none";	    
	}
	
	top.ScanFrame.location = "ReceiveOrderScan.asp?Clear=0&ContainerCount=" + iContainerCount + "&ReceiveChckBox=" + ReceivedList + "&RemoveChckBox=" + RemovedList + "&Action=<%=Action %>";
}

function CheckContainers( Action )
{
    var elm = null;
    if( Action == 'Receive' ) 
    {
	    elm = document.form1.ReceiveChckBox;
	}
	else
	{
	    // Action == 'Remove'
	    elm = document.form1.RemoveChckBox;
	}
	
	if( elm )
	{	    
	    if( elm.length )
	    {
		    for( i = 0; i < elm.length; i++ )
		    {
		        elm[i].checked = true;
		        ContainerOnCheck( elm[i], Action, false );
		    }
	    }
	    else
	    {
	        elm.checked = true;
		    ContainerOnCheck( elm, Action, false );
	    }
	}
	GetCheckBoxLists();
}

function ScanContainer( ContainerID )
{
    var ElementID = 'Receive' + ContainerID;
    var objElement = document.getElementById( ElementID );
    if( objElement )
    {
        objElement.checked = true;
        ContainerOnCheck( objElement, 'Receive', true )
    }
    else
    {
        alert( "The scanned container could not be found on this order." );
    }
}

function OnUpdate()
{
    document.form1.action = 'ReceiveOrder_action.asp';
    document.form1.Action.value = 'Receive';
    document.form1.submit();
}

</SCRIPT>
</HEAD>
<BODY>
<center>
<form name="form1" action="" method="POST">
<INPUT TYPE="hidden" NAME="ToBeProcessedCount" VALUE="">
<INPUT TYPE="hidden" NAME="Action" VALUE="">
<INPUT TYPE="hidden" NAME="OrderID" VALUE="<%=OrderID%>">
<INPUT TYPE="hidden" NAME="isFirst" VALUE="<%=isFirst%>">
<INPUT TYPE="hidden" NAME="ReceivedList" VALUE="">
<INPUT TYPE="hidden" NAME="RemovedList" VALUE="">

<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0">
<TR><TD>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>				
				<%if ( iContainerCount > 0 ) then%>
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
			<Tr>
				<th>
					<a href="#" onclick="CheckContainers('Receive');return false;">Received</a>
				</th>
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
					<a href="#" onclick="CheckContainers('Remove');return false;">Removed</a>
				</th>
			</Tr>
		<%
			If (RS.EOF AND RS.BOF) then
				Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No containers</Span></TD></tr>")
			Else
			    Set fldContainerID = RS.Fields("Container_ID")
				Set fldContainerBarcode = RS.Fields("barcode")
				Set fldContainerName = RS.Fields("Container_Name")
				Set fldLocationName = RS.Fields("location_name")
				Set fldCurrentUserID = RS.Fields("User_ID")
				Set fldQtyRemaining = RS.Fields("Qty_Remaining")
			    Set fldUnitAbbreviation = RS.Fields("Unit_Abreviation")
				Set fldPath = RS.Fields("Path")
				Set fldContainerStatus = RS.Fields("container_status_id_fk")
				
				While (Not RS.EOF)
				    ContainerStatus = fldContainerStatus.Value				    
				    
				    ' Only display the container if it has yet to be received
					if( CInt(ContainerStatus) <> CInt(Application("StatusApproved")) ) then
					    ContainerID = fldContainerID.Value
					    ContainerBarcode = fldContainerBarcode.Value
					    ContainerName = fldContainerName.Value
					    LocationName = fldLocationName.Value
					    CurrentUserID = fldCurrentUserID.Value
					    QtyRemaining = fldQtyRemaining.Value & " " & fldUnitAbbreviation.Value
					    Path = fldPath.Value
					    
					    iContainerCount = iContainerCount + 1					    
					    
					    sReceiveChecked = ""
					    if( ScannedContainerID <> "" ) then
					        if( CInt(ContainerID) = CInt(ScannedContainerID) ) then
                                sReceiveChecked = "checked"
                            end if
					    end if
					    if len(AddContainerID)>0 and cStr(ContainerID) = AddContainerID then sReceiveChecked = "checked"
					    if instr(ReceiveChckBox, ContainerID)>0 then sReceiveChecked = "checked"
    					
					    sRemoveChecked = ""
					    if len(RmvContainerID)>0 and cStr(ContainerID) = RmvContainerID then sRemoveChecked = "checked"
					    if instr(RemoveChckBox, ContainerID)>0 then sRemoveChecked = "checked"
		%>
					<tr>
						<td align=center>
							<INPUT TYPE="checkbox" id="Receive<%=ContainerID%>" VALUE="<%=ContainerID%>" NAME="ReceiveChckBox" onclick="ContainerOnCheck(this, 'Receive', true);" <%=sReceiveChecked%>>
						</td>
						<td align=center>
							<span name="<%=ContainerID%>"><%=ContainerBarcode%></span>
						</td>
						<td align=right> 
							<%=TruncateInSpan(ContainerName, 15, "")%>
						</td>
						<td align=center> 
							<span><%=LocationName%></span> 
						</td>
						<td align=center>
							<%=CurrentUserID%>
						</td>
						<td align=center>
							<%=QtyRemaining%>
						</td>
						<td align="center">
						    <INPUT TYPE="checkbox" id="Remove<%=ContainerID%>" VALUE="<%=ContainerID%>" NAME="RemoveChckBox" onclick="ContainerOnCheck(this, 'Remove', true);" <%=sRemoveChecked %>>
							<!-- <a class="MenuLink" title="Remove this container from the selection list" HREF="#" ONCLICK="document.location = 'ReceiveOrder_list.asp?OrderID=<%=OrderID%>&RmvContainerID=<%=ContainerID%>';">Remove</a> -->
						</td>
						<%if sChecked = "checked" then%>
						<script language="javascript">
							//SelectMarked();
						</script>
						<%end if%>
					</tr>
					<%
					
					end if      ' if( ContainerStatus <> Application("StatusApproved") ) then					
					
					rs.MoveNext
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
	if iContainerCount = 0 then 
		msg = msg & "Scan container barcodes to add them to the selection list."
	elseif iContainerCount = 1 then 
		msg = msg & "There is one container in the selection list."
	Elseif iContainerCount > 1 then
		msg = msg & "There are " & iContainerCount & " containers in the selection list."
	End if
	Response.Write "<TR><TD COLSPAN=""2""><BR><center><span class=""GUIFeedback"">" & Request("message") & "<BR>" & msg &  "<BR><BR></span></center></TD></TR>"
%>
</table>
</TD></TR>
<tr>
<td align="Right">
<%if not isEmpty(OrderID) and OrderID <> "" then %>
<a class="MenuLink" id="UpdateButton" style="display: none" "HREF="#" onclick="OnUpdate(); return false;"><img src="/cheminv/graphics/sq_btn/update.gif" alt="Click to update this order" border=0 /></a>
<% end if %>
<a class="MenuLink" HREF="#" onclick="if (parent){parent.window.close();}return false"><img SRC="/cheminv/graphics/sq_btn/close_dialog_btn.gif" border="0" alt="Close"></a>
</td>
</tr>
</TABLE>
</form>
</center>

</BODY>
</HTML>
<script language=javascript>
    iContainerCount = <% = iContainerCount %>;
    GetCheckBoxLists();
    document.form1.ToBeProcessedCount.value = iContainerCount;    
</script>
