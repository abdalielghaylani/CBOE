<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%
Dim Conn
Dim Cmd
Dim RS
Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(myDict)
	numContainers=myDict.count
	ContainerName =  numContainers & " containers will be retired"
Else
	ContainerID = Session("ContainerID")
	ContainerName = Request("ContainerName")
End if
LocationID = Application("DefRetireLocationID")
QtyRemaining = Request("QtyRemaining")
ContainerStatusID = Application("DefRetireStatusID")
'CSBR ID:121187
'Change Done by : Manoj Unnikrishnan 
'Purpose: Added the check for container id; If no container selected during multi select results in error
'Date: 06/07/2010
if isNull(QtyRemaining) or QtyRemaining="" and numContainers<=cint(Application("MAX_UPDATEABLE_CONTAINERS")) and len(ContainerID) > 0 then 
'End of Change #121187#
    SQL= " Select container_id, qty_remaining from inv_containers where  container_id in ("  & ContainerID & ")"
    Call GetInvCommand(sql, 1)
	Set RS = Cmd.Execute
	ContainerArr= split(ContainerID,",")
	RS.movefirst 
    do while not RS.eof
	    for each container in ContainerArr
	        if clng(container)= clng(RS("container_id")) then 
	            if isNull(QtyRemaining) or QtyRemaining="" then 
	                QtyRemaining = RS("qty_remaining")
	                QtyNullValue= ""
                else    
	                QtyRemaining = QtyRemaining & "," & RS("qty_remaining")
	                QtyNullValue= QtyNullValue & ","
	            end if
           	    RS.movenext 
	            exit for
	        end if 
	    next
    loop
else
    QtyNullValue= ""   	
end if 
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Retire an Inventory Container</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateMove(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
   		    <%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not athorized for this location.\r";
			    alert(errmsg);
			    return;
            }
        
         <%end if%>	
		<% if Lcase(Request("multiSelect")) = "true" then %>
		    var numContainers = <%=myDict.count%>;
		<% else %>
		    var numContainers = 1;
		<% end if%>
        // determine whether there are enough open rack positions
		if (document.form1.isRack.value == "1") {
		    if (AreEnoughRackPositions(numContainers,document.form1.LocationID.value) == false) {
				errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				bWriteError = true;
		    }
		}
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="RetireContainer_action.asp" method="POST">
<%if numContainers>cint(Application("MAX_UPDATEABLE_CONTAINERS")) then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You cannot retire more then <%=cint(Application("MAX_UPDATEABLE_CONTAINERS"))%> containers at a time.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right">
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>
<%elseif len(ContainerID) = 0 then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select containers to retire.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
<%else%>
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="QtyRemaining" value="<%=QtyRemaining%>">
<input Type="hidden" name="tempQtyRemaining" value="<%=QtyRemaining%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<input type="hidden" name="comments" value="<%=Session("Comments")%>">
<input Type="hidden" name="QtyNullValue" value="<%=QtyNullValue%>">
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Retire an inventory container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container to retire:
		</td>
		<td>
			<input TYPE="tetx" SIZE="44" onfocus="blur()" VALUE="<%=ContainerName%>" disabled>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false%> 
		</td>
	</tr>
	<tr>
		<%= ShowPickList("Container Status:", "ContainerStatusID", ContainerStatusID,"SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_Container_Status ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap>
			Comments:
		</td>
		<td valign="top">
			<textarea rows="5" cols="35" name="RetireComments" wrap="hard"></textarea>
		</td>
	</tr>
	<tr>
		<td align"right" nowrap></td><td><input type="checkbox" name="zeroQty_cb" value="1" onclick="this.checked ? document.form1.QtyRemaining.value = document.form1.QtyNullValue.value :  document.form1.QtyRemaining.value = document.form1.tempQtyRemaining.value">Set the quantity remaining to zero</td>	
		<script LANGUAGE="javascript">document.form1.zeroQty_cb.click();</script>

	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
<%end if %>	
</form>
</center>
</body>
</html>
