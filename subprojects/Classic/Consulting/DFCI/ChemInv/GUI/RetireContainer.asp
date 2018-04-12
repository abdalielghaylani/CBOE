<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(myDict)
	ContainerName =  myDict.count & " containers will be retired"
Else
	ContainerID = Session("ContainerID")
	ContainerName = Request("ContainerName")
End if
LocationID = Application("DefRetireLocationID")
QtyRemaining = Request("QtyRemaining")
ContainerStatusID = Application("DefRetireStatusID")
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
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="QtyRemaining" value="<%=QtyRemaining%>">
<input Type="hidden" name="tempQtyRemaining" value="<%=QtyRemaining%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<input type="hidden" name="comments" value="<%=Session("Comments")%>">
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
	<tr><!--DFCI Changes.  Do not allow container to be zeroed out -->
		<td align"right" nowrap></td><td><input type="checkbox" name="zeroQty_cb" value="1" onclick="this.checked ? document.form1.QtyRemaining.value = '' :  document.form1.QtyRemaining.value = document.form1.tempQtyRemaining.value" disabled>Set the quantity remaining to zero</td>	
		<script LANGUAGE="javascript">//document.form1.zeroQty_cb.click();</script>
	<!--END DFCI Edits -->
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateMove(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
